IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_OBTENER_CONFIGURACION_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_OBTENER_CONFIGURACION_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consulta el valor de configuracion de pedidos especiales
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_OBTENER_CONFIGURACION_PEDIDOS_ESPECIALES]
@idConfig int=null
as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''
			end  --declaraciones 

			begin -- principal
				
			
					SELECT * 
					into #ConfiguracionesPedidosEspeciales
					FROM ConfiguracionesPedidosEspeciales where idConfig=coalesce(@idConfig,idConfig) and activo=1

					if not exists (select 1 from #ConfiguracionesPedidosEspeciales)
					begin
						select	
								@status = -1,
								@mensaje = 'No se encontraron configuraciones.'
					end


			end -- principal

		end try

		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@mensaje = error_message()
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
           
		    if(@status=200)
				select * from #ConfiguracionesPedidosEspeciales
			
					
		end -- reporte de estatus
		

	end  -- principal
