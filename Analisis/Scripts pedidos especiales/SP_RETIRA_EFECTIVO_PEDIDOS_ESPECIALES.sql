IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_RETIRA_EFECTIVO_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_RETIRA_EFECTIVO_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/10/13
Objetivo		Realiza retiros de efectivo de pedidos especiales
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_RETIRA_EFECTIVO_PEDIDOS_ESPECIALES]

	@idEstacion						int,
	@idUsuario						int,
	@monto							money,
	@caso							int,  -- 1-Exceso de Efectivo / 2-Cierre de Caja
	@efectivoEntregadoEnCierre		money = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Se inserto correctamente el retiro de dinero.',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@idRetiro				int = 0,
						@idCierre				int = 0						
			end  --declaraciones 

			begin -- principal

				-- si todo bien
				-- caso 1 = exceso de efectivo
				if ( @caso = 1)
					begin
						insert into 
							PedidosEspecialesRetirosExcesoEfectivo 
								(montoRetiro,idUsuario,idEstacion,idEstatusRetiro,fechaAlta , montoAutorizado)
						values	(@monto, @idUsuario, @idEstacion,2 /*Realizado*/ ,dbo.FechaActual() , @monto)

						select	@idRetiro = max(idRetiro) 
						from	PedidosEspecialesRetirosExcesoEfectivo
						where	idUsuario = @idUsuario 
							and	idEstacion = @idEstacion
							and montoRetiro = @monto
					end

				-- cierre de caja
				if ( @caso = 2)
					begin

						select	@mensaje = 'Se realizo el cierre de la Estacion:' + nombre + ' correctamente.'
						from	Estaciones 
						where	idEstacion = @idEstacion
						
					end

			end -- principal

		end try

		begin catch 
		
			-- captura del error
			select	@status =			-error_state(),
					@error_procedure =	error_procedure(),
					@error_line =		error_line(),
					@mensaje =			error_message()
					
		end catch

		begin -- reporte de estatus

			select	@status Estatus,					
					@mensaje as  Mensaje,
					@idRetiro as idRetiro,
					@idCierre as idCierre

		end -- reporte de estatus

	end  -- principal
