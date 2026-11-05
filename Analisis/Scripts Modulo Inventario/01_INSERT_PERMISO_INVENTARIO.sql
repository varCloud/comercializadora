/*
Objetivo	Permiso del modulo 16 "Inventario" (menu Inventario > Agregar inventario almacen del WMS).
			CatModulos.idModulo es identity => se inserta el 16 con IDENTITY_INSERT (el 16 esta libre; 15 = Relacion Produccion).
			PermisosRolPorModulo.idPermiso es identity (sin FKs): una fila por rol de CatRoles,
			tienePermiso = 1 solo para idRol = 1 (Administrador, EnumRoles.Administrador), 0 en los demas.
			Por ahora solo el Administrador; para abrirlo a otro rol basta actualizar su fila (tienePermiso = 1).
Idempotente.
*/

-- VERIFICACION PREVIA (0 filas la primera vez)
select * from CatModulos where idModulo = 16
select * from PermisosRolPorModulo where idModulo = 16
GO

IF NOT EXISTS (SELECT 1 FROM CatModulos WHERE idModulo = 16)
BEGIN
	SET IDENTITY_INSERT CatModulos ON
	INSERT INTO CatModulos (idModulo, descripcion) VALUES (16, 'Inventario')
	SET IDENTITY_INSERT CatModulos OFF
END

INSERT INTO PermisosRolPorModulo (idRol, idModulo, tienePermiso)
SELECT R.idRol, 16, CASE WHEN R.idRol = 1 THEN 1 ELSE 0 END
FROM CatRoles R
WHERE NOT EXISTS (SELECT 1 FROM PermisosRolPorModulo P WHERE P.idModulo = 16 AND P.idRol = R.idRol)
GO

-- VERIFICACION POSTERIOR (modulo 16; una fila por rol; solo idRol 1 con permiso)
select * from CatModulos where idModulo = 16
select (select count(*) from CatRoles) roles, count(*) filas, sum(case when tienePermiso = 1 then 1 else 0 end) conPermiso,
	   min(case when tienePermiso = 1 then idRol end) rolConPermiso
from PermisosRolPorModulo where idModulo = 16
GO
