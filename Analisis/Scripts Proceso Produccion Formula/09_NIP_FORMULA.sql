/*
Objetivo	NIP compartido para ver la formula en la app movil. Reemplaza al NIP estatico de Web.config (appSettings contrasenaFormula).
			El NIP vive en BD (solo su hash), se valida en el servidor y solo el Administrador lo cambia desde el WMS.
			CREATE OR ALTER / IF OBJECT_ID => idempotente. Primer result set = fila de estatus (status/estatus 200 = ok, -1 = error, mensaje).

			Tablas
				ProcesoProduccionFormulaNip			UNA sola fila logica (idNip = 1 por DEFAULT + CHECK + PRIMARY KEY => una segunda fila es imposible).
													nipHash varbinary(32), salt uniqueidentifier, idUsuarioCambio int, fechaCambio datetime.
				ProcesoProduccionFormulaNipIntento	Una fila por idUsuario: intentosFallidos, bloqueadoHasta (NULL = sin bloqueo).

			Hash (exacto): nipHash = HASHBYTES('SHA2_256', CONVERT(varbinary(16), salt) + CONVERT(varbinary(10), nip))
				salt = bytes internos del uniqueidentifier (16 bytes), nip = bytes del varchar (4 digitos ASCII, 4 bytes). Nunca se guarda el NIP en claro.
				Tanto el SP de cambio como el de validacion usan esta misma expresion.

			SP_WMS_CAMBIAR_NIP_FORMULA @idUsuario int, @nipNuevo varchar(10)
				Solo Administrador (Usuarios.idRol = 1, EnumRoles.Administrador). NIP = exactamente 4 digitos.
				Inserta/actualiza la fila unica con salt nueva (NEWID()) y reinicia TODOS los intentos/bloqueos (un NIP nuevo desbloquea a todos).

			ProcesoProduccionFormulaNipAutorizacion	Una fila por idUsuario: vigenteHasta. La crea un NIP correcto (TTL en @minutosVigenciaAutorizacion, 2 min),
													SP_APP_OBTENER_FORMULA_PRODUCTO la consume (un solo uso) y SP_WMS_CAMBIAR_NIP_FORMULA las borra todas.

			Flujo en 2 pasos (la formula NUNCA viaja en la busqueda ni en la validacion; SP_APP_BUSCAR_PRODUCTOS_PROCESO_PRODUCCION_FORMULA ya no la devuelve):
				1. SP_APP_VALIDAR_NIP_FORMULA => solo fila de estatus; si es 200 deja la autorizacion.
				2. SP_APP_OBTENER_FORMULA_PRODUCTO => con autorizacion vigente devuelve la formula y la consume.

			SP_APP_OBTENER_FORMULA_PRODUCTO @idUsuario int, @idProducto int
				Mismo chequeo de rol que la validacion. Sin autorizacion vigente => -1 'Valide el NIP para ver la formula' y NINGUNA fila.
				Producto sin formula activa => -1 'El producto no tiene una formula de produccion activa' (NO consume la autorizacion).
				Exito => set 1 = status 200, set 2 = materias primas de la formula activa (idProducto, descripcion, idUnidadMedida,
				unidadMedida, cantidad POR LOTE) ordenadas por descripcion. La autorizacion se consume con DELETE atomico (concurrencia segura).

			SP_APP_VALIDAR_NIP_FORMULA @idUsuario int, @nip varchar(10)
				Devuelve SOLO la fila de estatus. NIP correcto => 200 + autorizacion (upsert); cualquier fallo => -1 y sin autorizacion.
				Purga las autorizaciones vencidas en cada llamada.
				Solo Administrador (1) o Encargado_produccion_liquidos (13). Cualquier otro rol => status -1 generico.
				Sin NIP configurado => -1. Bloqueado => -1 con minutos restantes.
				5 fallos consecutivos => bloqueo de 5 minutos (constantes al inicio del SP); un acierto reinicia el contador.
				Un NIP mal formado cuenta como intento fallido (no se puede evitar el contador enviando basura).
				La comparacion es entre varbinary(32) (hash), nunca contra el NIP en claro.
*/

-- VERIFICACION PREVIA
select name from sys.tables where name in ('ProcesoProduccionFormulaNip','ProcesoProduccionFormulaNipIntento','ProcesoProduccionFormulaNipAutorizacion')
select name from sys.procedures where name in ('SP_WMS_CAMBIAR_NIP_FORMULA','SP_APP_VALIDAR_NIP_FORMULA','SP_APP_OBTENER_FORMULA_PRODUCTO')
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.ProcesoProduccionFormulaNip', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.ProcesoProduccionFormulaNip
	(
		idNip int NOT NULL CONSTRAINT DF_ProcesoProduccionFormulaNip_idNip DEFAULT (1),
		nipHash varbinary(32) NOT NULL,
		salt uniqueidentifier NOT NULL,
		idUsuarioCambio int NOT NULL,
		fechaCambio datetime NOT NULL,
		CONSTRAINT PK_ProcesoProduccionFormulaNip PRIMARY KEY (idNip),
		CONSTRAINT CK_ProcesoProduccionFormulaNip_FilaUnica CHECK (idNip = 1)
	)
END
GO

IF OBJECT_ID('dbo.ProcesoProduccionFormulaNipIntento', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.ProcesoProduccionFormulaNipIntento
	(
		idUsuario int NOT NULL,
		intentosFallidos int NOT NULL CONSTRAINT DF_ProcesoProduccionFormulaNipIntento_intentos DEFAULT (0),
		bloqueadoHasta datetime NULL,
		CONSTRAINT PK_ProcesoProduccionFormulaNipIntento PRIMARY KEY (idUsuario)
	)
END
GO

IF OBJECT_ID('dbo.ProcesoProduccionFormulaNipAutorizacion', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.ProcesoProduccionFormulaNipAutorizacion
	(
		idUsuario int NOT NULL,
		vigenteHasta datetime NOT NULL,
		CONSTRAINT PK_ProcesoProduccionFormulaNipAutorizacion PRIMARY KEY (idUsuario)
	)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_WMS_CAMBIAR_NIP_FORMULA]
@idUsuario int,
@nipNuevo varchar(10)
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @rolAdministrador int = 1, -- EnumRoles.Administrador
			@idRol int, @salt uniqueidentifier, @ahora datetime = dbo.FechaActual()

	BEGIN TRY
		SELECT @idRol = idRol FROM Usuarios WHERE idUsuario = @idUsuario
		IF @idRol IS NULL OR @idRol <> @rolAdministrador
		BEGIN
			SELECT -1 status, -1 estatus, 'Solo el administrador puede cambiar el NIP de la formula' mensaje
			RETURN
		END

		-- Exactamente 4 digitos (DATALENGTH: LEN ignora espacios finales)
		IF @nipNuevo IS NULL OR DATALENGTH(@nipNuevo) <> 4 OR @nipNuevo LIKE '%[^0-9]%'
		BEGIN
			SELECT -1 status, -1 estatus, 'El NIP debe tener exactamente 4 digitos numericos' mensaje
			RETURN
		END

		SET @salt = NEWID()

		BEGIN TRAN
			IF EXISTS (SELECT 1 FROM ProcesoProduccionFormulaNip WITH (UPDLOCK, HOLDLOCK) WHERE idNip = 1)
				UPDATE ProcesoProduccionFormulaNip
				SET nipHash = HASHBYTES('SHA2_256', CONVERT(varbinary(16), @salt) + CONVERT(varbinary(10), @nipNuevo)),
					salt = @salt, idUsuarioCambio = @idUsuario, fechaCambio = @ahora
				WHERE idNip = 1
			ELSE
				INSERT INTO ProcesoProduccionFormulaNip (idNip, nipHash, salt, idUsuarioCambio, fechaCambio)
				VALUES (1, HASHBYTES('SHA2_256', CONVERT(varbinary(16), @salt) + CONVERT(varbinary(10), @nipNuevo)), @salt, @idUsuario, @ahora)

			-- Un NIP nuevo desbloquea a todos
			UPDATE ProcesoProduccionFormulaNipIntento SET intentosFallidos = 0, bloqueadoHasta = NULL

			-- Un NIP nuevo invalida cualquier autorizacion vigente
			DELETE FROM ProcesoProduccionFormulaNipAutorizacion
		COMMIT TRAN

		SELECT 200 status, 200 estatus, 'NIP de la formula actualizado correctamente' mensaje
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRAN
		SELECT -1 status, -1 estatus, 'Ha ocurrido un error al cambiar el NIP: ' + ERROR_MESSAGE() mensaje
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_APP_VALIDAR_NIP_FORMULA]
@idUsuario int,
@nip varchar(10)
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @rolAdministrador int = 1,                    -- EnumRoles.Administrador
			@rolEncargadoProduccionLiquidos int = 13,     -- EnumRoles.Encargado_producción_liquidos
			@maxIntentosFallidos int = 5,
			@minutosBloqueo int = 5,
			@minutosVigenciaAutorizacion int = 2,         -- TTL de la autorizacion que da un NIP correcto para pedir la formula
			@idRol int, @nipHash varbinary(32), @salt uniqueidentifier,
			@intentos int, @bloqueadoHasta datetime, @ahora datetime = dbo.FechaActual(),
			@minutosRestantes int, @intentosRestantes int, @mensaje varchar(200)

	BEGIN TRY
		SELECT @idRol = idRol FROM Usuarios WHERE idUsuario = @idUsuario
		IF @idRol IS NULL OR @idRol NOT IN (@rolAdministrador, @rolEncargadoProduccionLiquidos)
		BEGIN
			SELECT -1 status, -1 estatus, 'No tiene permiso para ver la formula' mensaje
			RETURN
		END

		SELECT @nipHash = nipHash, @salt = salt FROM ProcesoProduccionFormulaNip WHERE idNip = 1
		IF @nipHash IS NULL
		BEGIN
			SELECT -1 status, -1 estatus, 'El NIP de la formula no esta configurado, solicitelo al administrador' mensaje
			RETURN
		END

		BEGIN TRAN
			-- Limpieza oportunista de autorizaciones vencidas
			DELETE FROM ProcesoProduccionFormulaNipAutorizacion WHERE vigenteHasta <= @ahora

			-- Serializa los intentos del mismo usuario (HOLDLOCK evita doble insert y carreras en el contador)
			IF NOT EXISTS (SELECT 1 FROM ProcesoProduccionFormulaNipIntento WITH (UPDLOCK, HOLDLOCK) WHERE idUsuario = @idUsuario)
				INSERT INTO ProcesoProduccionFormulaNipIntento (idUsuario, intentosFallidos, bloqueadoHasta) VALUES (@idUsuario, 0, NULL)

			SELECT @intentos = intentosFallidos, @bloqueadoHasta = bloqueadoHasta
			FROM ProcesoProduccionFormulaNipIntento WITH (UPDLOCK, HOLDLOCK) WHERE idUsuario = @idUsuario

			IF @bloqueadoHasta IS NOT NULL AND @bloqueadoHasta > @ahora
			BEGIN
				SET @minutosRestantes = CEILING(DATEDIFF(SECOND, @ahora, @bloqueadoHasta) / 60.0)
				COMMIT TRAN
				SELECT -1 status, -1 estatus,
					   'Demasiados intentos fallidos. Intente de nuevo en ' + CAST(@minutosRestantes AS varchar(10)) + ' minuto(s)' mensaje
				RETURN
			END

			-- Bloqueo vencido: se reinicia el contador
			IF @bloqueadoHasta IS NOT NULL
			BEGIN
				SET @intentos = 0
				SET @bloqueadoHasta = NULL
			END

			IF @nip IS NOT NULL AND DATALENGTH(@nip) = 4 AND @nip NOT LIKE '%[^0-9]%'
			   AND HASHBYTES('SHA2_256', CONVERT(varbinary(16), @salt) + CONVERT(varbinary(10), @nip)) = @nipHash
			BEGIN
				UPDATE ProcesoProduccionFormulaNipIntento SET intentosFallidos = 0, bloqueadoHasta = NULL WHERE idUsuario = @idUsuario

				-- NIP correcto: autorizacion corta (una fila por usuario) que SP_APP_OBTENER_FORMULA_PRODUCTO consume una sola vez
				IF EXISTS (SELECT 1 FROM ProcesoProduccionFormulaNipAutorizacion WITH (UPDLOCK, HOLDLOCK) WHERE idUsuario = @idUsuario)
					UPDATE ProcesoProduccionFormulaNipAutorizacion
					SET vigenteHasta = DATEADD(MINUTE, @minutosVigenciaAutorizacion, @ahora) WHERE idUsuario = @idUsuario
				ELSE
					INSERT INTO ProcesoProduccionFormulaNipAutorizacion (idUsuario, vigenteHasta)
					VALUES (@idUsuario, DATEADD(MINUTE, @minutosVigenciaAutorizacion, @ahora))
				COMMIT TRAN
				SELECT 200 status, 200 estatus, 'NIP correcto' mensaje
				RETURN
			END

			SET @intentos = @intentos + 1
			IF @intentos >= @maxIntentosFallidos
			BEGIN
				UPDATE ProcesoProduccionFormulaNipIntento
				SET intentosFallidos = 0, bloqueadoHasta = DATEADD(MINUTE, @minutosBloqueo, @ahora) WHERE idUsuario = @idUsuario
				SET @mensaje = 'NIP incorrecto. Demasiados intentos fallidos, bloqueado por ' + CAST(@minutosBloqueo AS varchar(10)) + ' minuto(s)'
			END
			ELSE
			BEGIN
				UPDATE ProcesoProduccionFormulaNipIntento
				SET intentosFallidos = @intentos, bloqueadoHasta = NULL WHERE idUsuario = @idUsuario
				SET @intentosRestantes = @maxIntentosFallidos - @intentos
				SET @mensaje = 'NIP incorrecto. Intentos restantes: ' + CAST(@intentosRestantes AS varchar(10))
			END
		COMMIT TRAN

		SELECT -1 status, -1 estatus, @mensaje mensaje
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRAN
		SELECT -1 status, -1 estatus, 'Ha ocurrido un error al validar el NIP' mensaje
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_APP_OBTENER_FORMULA_PRODUCTO]
@idUsuario int,
@idProducto int
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @rolAdministrador int = 1,                    -- EnumRoles.Administrador
			@rolEncargadoProduccionLiquidos int = 13,     -- EnumRoles.Encargado_producción_liquidos
			@idRol int, @ahora datetime = dbo.FechaActual(), @idFormula bigint, @consumidas int

	BEGIN TRY
		SELECT @idRol = idRol FROM Usuarios WHERE idUsuario = @idUsuario
		IF @idRol IS NULL OR @idRol NOT IN (@rolAdministrador, @rolEncargadoProduccionLiquidos)
		BEGIN
			SELECT -1 status, -1 estatus, 'No tiene permiso para ver la formula' mensaje
			RETURN
		END

		BEGIN TRAN
			-- Sin autorizacion vigente no se revela nada (ni siquiera si el producto tiene formula)
			IF NOT EXISTS (SELECT 1 FROM ProcesoProduccionFormulaNipAutorizacion WITH (UPDLOCK, HOLDLOCK)
						   WHERE idUsuario = @idUsuario AND vigenteHasta > @ahora)
			BEGIN
				COMMIT TRAN
				SELECT -1 status, -1 estatus, 'Valide el NIP para ver la formula' mensaje
				RETURN
			END

			SELECT @idFormula = idProcesoProduccionFormula FROM ProcesoProduccionFormula WHERE idProducto = @idProducto AND activo = 1
			IF @idFormula IS NULL
			BEGIN
				COMMIT TRAN -- la autorizacion NO se consume
				SELECT -1 status, -1 estatus, 'El producto no tiene una formula de produccion activa' mensaje
				RETURN
			END

			-- Un solo uso: el DELETE es atomico; si una llamada concurrente ya la consumio, aqui no se borra ninguna fila
			DELETE FROM ProcesoProduccionFormulaNipAutorizacion WHERE idUsuario = @idUsuario AND vigenteHasta > @ahora
			SET @consumidas = @@ROWCOUNT
		COMMIT TRAN

		IF @consumidas = 0
		BEGIN
			SELECT -1 status, -1 estatus, 'Valide el NIP para ver la formula' mensaje
			RETURN
		END

		SELECT 200 status, 200 estatus, 'Consulta correcta' mensaje

		SELECT D.idProducto, P.descripcion, D.idUnidadMedida, U.descripcion unidadMedida, D.cantidad
		FROM ProcesoProduccionFormulaDetalle D
		JOIN Productos P ON P.idProducto = D.idProducto
		JOIN CatUnidadMedida U ON U.idUnidadMedida = D.idUnidadMedida
		WHERE D.idProcesoProduccionFormula = @idFormula
		ORDER BY P.descripcion
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRAN
		SELECT -1 status, -1 estatus, 'Ha ocurrido un error al obtener la formula' mensaje
	END CATCH
END
GO

-- VERIFICACION POSTERIOR (3 tablas, 3 SPs)
select name from sys.tables where name in ('ProcesoProduccionFormulaNip','ProcesoProduccionFormulaNipIntento','ProcesoProduccionFormulaNipAutorizacion')
select name from sys.procedures where name in ('SP_WMS_CAMBIAR_NIP_FORMULA','SP_APP_VALIDAR_NIP_FORMULA','SP_APP_OBTENER_FORMULA_PRODUCTO')
GO

/*
ROLLBACK / NOTAS
	-- Quitar la funcionalidad (los SPs primero; las tablas despues). Al borrar las tablas se pierde el NIP configurado.
	-- DROP PROCEDURE dbo.SP_APP_OBTENER_FORMULA_PRODUCTO
	-- DROP PROCEDURE dbo.SP_APP_VALIDAR_NIP_FORMULA
	-- DROP PROCEDURE dbo.SP_WMS_CAMBIAR_NIP_FORMULA
	-- DROP TABLE dbo.ProcesoProduccionFormulaNipAutorizacion
	-- DROP TABLE dbo.ProcesoProduccionFormulaNipIntento
	-- DROP TABLE dbo.ProcesoProduccionFormulaNip

	-- Desbloquear a todos sin cambiar el NIP:
	-- UPDATE dbo.ProcesoProduccionFormulaNipIntento SET intentosFallidos = 0, bloqueadoHasta = NULL

	-- Revocar de inmediato todas las autorizaciones vigentes:
	-- DELETE FROM dbo.ProcesoProduccionFormulaNipAutorizacion

	-- Si en algun ambiente se llego a crear con el nombre anterior: DROP PROCEDURE dbo.SP_HH_VALIDAR_NIP_FORMULA (renombrado a SP_APP_*; el script no se habia ejecutado).
	-- Volver al estado previo a este flujo implica restaurar tambien el set 3 de SP_APP_BUSCAR_PRODUCTOS_PROCESO_PRODUCCION_FORMULA (script 06 previo)
	-- y la version de la app/backend que lo consume; no se recomienda (ese set exponia la formula a cualquier usuario de la app).

	NOTAS
	 - ORDEN DE DESPLIEGUE: scripts 06 y 09 -> backend (WebServices) -> app.
	   Con 06 aplicado y el backend/app anteriores la app no recibe materias primas (no truena, solo no muestra la formula).
	   Con el backend nuevo, la app anterior valida el NIP (200) pero la formula solo sale por api/WsProcesoProduccion/ObtenerFormulaProducto (app nueva).
	 - La autorizacion dura @minutosVigenciaAutorizacion (2 min), es de un solo uso y se pierde si el Administrador cambia el NIP.
	 - Hasta que el Administrador defina el NIP desde el WMS ("Cambiar NIP de formula") la app movil responde "El NIP de la formula no esta configurado".
	 - Los roles 1 y 13 estan fijos como variables DECLARE (en validar y en obtener formula); si cambian en EnumRoles (Models/Enumeraciones.cs) hay que actualizarlos en ambos SPs.
*/
