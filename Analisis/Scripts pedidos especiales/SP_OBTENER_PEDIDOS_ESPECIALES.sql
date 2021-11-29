IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_OBTENER_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_OBTENER_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consulta los pedidos especiales
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_OBTENER_PEDIDOS_ESPECIALES]

@idCliente int=null,
@idUsuario int=null,
@idEstatusPedidoEspecial int=null,
@fechaIni date=null,
@fechaFin date=null,
@codigoBarras varchar(500)=null

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
				
			
					SELECT	
					pe.idPedidoEspecial,
					pe.idCliente,
					pe.cantidad,
					CONVERT(VARCHAR(10),pe.fechaAlta,103) + ' ' + CONVERT(VARCHAR(20),pe.fechaAlta,114) fechaAlta,
					pe.montoTotal,
					pe.idUsuario,
					pe.idEstatusPedidoEspecial,
					pe.idEstacion,
					ISNULL(pe.observaciones,'') observaciones,
					ISNULL(pe.codigoBarras,'') codigoBarras,
					pe.idTipoPago,
					pe.idUsuarioEntrega,
					pe.numeroUnidadTaxi,
					c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoPaterno nombreCliente,
					u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoPaterno nombreUsuario,
					e.descripcion estatusPedidoEspecial,
					'NO' facturado
					into #pedidosEspeciales
					FROM	PedidosEspeciales pe
								join Clientes c
									on pe.idCliente=c.idCliente
								join Usuarios u
									on pe.idUsuario=u.idUsuario
								join CatEstatusPedidoEspecial e on pe.idEstatusPedidoEspecial=e.idEstatusPedidoEspecial

					where 
							c.idCliente=coalesce(@idCliente,c.idCliente)	
							and u.idUsuario=coalesce(@idUsuario,u.idUsuario)
							and e.idEstatusPedidoEspecial=coalesce(@idEstatusPedidoEspecial,e.idEstatusPedidoEspecial)
							and cast(pe.fechaAlta as date)>=coalesce(@fechaIni,cast(pe.fechaAlta as date))
							and cast(pe.fechaAlta as date)<=coalesce(@fechaFin,cast(pe.fechaAlta as date))
					and coalesce(pe.codigoBarras,'')=coalesce(@codigoBarras,coalesce(pe.codigoBarras,''))
					

					if not exists (select 1 from #pedidosEspeciales)
					begin
						select	
								@status = -1,
								@mensaje = 'No se encontraron pedidos especiales.'
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
				select * from #pedidosEspeciales order by fechaAlta desc
			
					
		end -- reporte de estatus
		

	end  -- principal
