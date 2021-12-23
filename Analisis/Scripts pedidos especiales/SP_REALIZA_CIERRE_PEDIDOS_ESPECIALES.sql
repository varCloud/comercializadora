IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_REALIZA_CIERRE_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_REALIZA_CIERRE_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/10/13
Objetivo		Registrar los ingresos de efectivo de pedidos especiales
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_REALIZA_CIERRE_PEDIDOS_ESPECIALES]

	@idUsuario int,
	@idEstacion int,
	@efectivoEntregadoEnCierre money	

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Se ha realizado el cierre de pedidos especiales de manera exitosa',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''

							

			end  --declaraciones 

			begin -- principal

				if not exists (select 1 from PedidosEspecialesIngresosEfectivo where idUsuario = @idUsuario and cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idTipoIngreso=1)
				begin
					select @mensaje = 'Por poder realizar un cierre de cajas, se requiere que se realize la apertura de cajas.'
					raiserror (@mensaje, 11, -1)
				end

				if exists (select 1 from PedidosEspecialesCierres where idUsuario = @idUsuario and cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idEstatusRetiro in (1,2)) 
				begin
					select @mensaje = 'Ya existe un cierre de cajas de hoy.'
					raiserror (@mensaje, 11, -1)
				end	

			    if not exists (select 1 from PedidosEspecialesCierres where idUsuario = @idUsuario and cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idEstatusRetiro=4)
				begin
					select @mensaje = 'No existe informaciòn del cierre de cajas.'
					raiserror (@mensaje, 11, -1)
				end

				update PedidosEspecialesCierres
				set EfectivoEntregadoEnCierre=@efectivoEntregadoEnCierre,
				fechaAlta=dbo.FechaActual(),
				idEstatusRetiro=1,
				idEstacion=@idEstacion
				where idUsuario=@idUsuario and cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idEstatusRetiro=4				

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
			
				begin
					select	@status Estatus,							
							@mensaje Mensaje							
				end

		end -- reporte de estatus

	end  -- principal
