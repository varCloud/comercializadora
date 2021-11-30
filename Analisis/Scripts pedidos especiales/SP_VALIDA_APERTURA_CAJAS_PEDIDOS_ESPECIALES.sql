IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_VALIDA_APERTURA_CAJAS_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_VALIDA_APERTURA_CAJAS_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/10/13
Objetivo		Validar si existe una apertura de cajas
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_VALIDA_APERTURA_CAJAS_PEDIDOS_ESPECIALES]	
	@idUsuario						int
as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Caja Aperturada..',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''			
			end  --declaraciones 

			begin -- principal

				if not exists (select 1 from PedidosEspecialesIngresosEfectivo where idUsuario = @idUsuario and cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idTipoIngreso=1) 
				begin
					select @mensaje = 'Por favor aperture cajas.'
					raiserror (@mensaje, 11, -1)
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
					@mensaje as  Mensaje

		end -- reporte de estatus

	end  -- principal
