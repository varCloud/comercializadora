IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_OBTENER_USUARIOS_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_OBTENER_USUARIOS_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consulta los usuarios que han dado de alta algun pedido especial
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_OBTENER_USUARIOS_PEDIDOS_ESPECIALES]

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
				
			
					SELECT	u.idUsuario,u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoPaterno nombreUsuario
					into #usuarios
					FROM	PedidosEspeciales pe
								join Usuarios u
									on pe.idUsuario=u.idUsuario
					group by u.idUsuario,u.nombre,u.apellidoPaterno,u.apellidoPaterno					
					
					if not exists (select 1 from #usuarios)
					begin
						select	
								@status = -1,
								@mensaje = 'No se encontraron usuarios con pedidos especiales.'
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
				select * from #usuarios
			
					
		end -- reporte de estatus
		

	end  -- principal
