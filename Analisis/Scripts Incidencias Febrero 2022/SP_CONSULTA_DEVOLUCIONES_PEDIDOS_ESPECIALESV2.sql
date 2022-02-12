--use DB_A57E86_lluviadesarrollo
--go

-- se crea procedimiento SP_CONSULTA_DEVOLUCIONES_PEDIDOS_ESPECIALESV2
if exists (select * from sysobjects where name like 'SP_CONSULTA_DEVOLUCIONES_PEDIDOS_ESPECIALESV2' and xtype = 'p' )
	drop proc SP_CONSULTA_DEVOLUCIONES_PEDIDOS_ESPECIALESV2
go

/*

Autor			Ernesto Aguilar
Fecha			2022/02/07
Objetivo		CONSULTA LAS DEVOLUCIONES DE LAS VENTAS REALIZADAS DE LOS PEDIDOS ESPECIALES PARA VISTA DE REPORTES

*/

create proc SP_CONSULTA_DEVOLUCIONES_PEDIDOS_ESPECIALESV2

	@idPedidoEspecial		int = null,
	@idAlmacen				int = null,
	@idUsuario				int = null,
	@fechaIni				datetime = null,
	@fechaFin				datetime = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit)


			end  --declaraciones 

			begin -- principal
			
			if(@idAlmacen is null and @idUsuario is null and @idPedidoEspecial is null)
				begin
					select @fechaIni=coalesce(@fechaIni,cast(dbo.FechaActual() as date))
					select @fechaFin=coalesce(@fechaFin,cast(dbo.FechaActual() as date))
				end

				select 	dev.idPedidoEspecial, 
						dev.idUsuario, 
						u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
						Clientes.idCliente, 
						Clientes.nombreCliente,
						u.idSucursal,
						u.idAlmacen, 
						devD.idProducto, 
						p.descripcion as descripcionProducto,
						devD.cantidad, 
						devD.monto + devD.montoComision montoTotal,
						dbo.Redondear(devD.monto/devD.cantidad) precioVenta,
						dev.fechaAlta, 
						a.Descripcion as descAlmacen,
						p.codigoBarras
				into	#RESULT
				from	TicketsPedidosEspeciales  dev
							join TicketsPedidosEspecialesDetalle devD
								on dev.idPedidoEspecial = devD.idPedidoEspecial
						join Usuarios u 
							on dev.idUsuario=u.idUsuario			
						join ( 
								select	p.idPedidoEspecial,  
										p.idCliente, c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno as nombreCliente										
								from	PedidosEspeciales p
											join TicketsPedidosEspeciales t
												on t.idPedidoEspecial = p.idPedidoEspecial
											join Clientes c
												on p.idCliente = c.idCliente
							 ) Clientes 
							on dev.idPedidoEspecial = Clientes.idPedidoEspecial
						join Productos p 
							on devD.idProducto=p.idProducto
						join Almacenes a 
							on u.idAlmacen=a.idAlmacen

				where	dev.idTipoTicketPedidoEspecial = 2
					and devD.idPedidoEspecial = coalesce(@idPedidoEspecial, devD.idPedidoEspecial)										
					and	u.idAlmacen = coalesce(@idAlmacen, u.idAlmacen)										
					and	dev.idUsuario =coalesce(@idUsuario, u.idUsuario)					
					and cast(dev.fechaAlta as date) >=coalesce(cast(@fechaIni as date),cast(dev.fechaAlta as date))
					and cast(dev.fechaAlta as date) <=coalesce(cast(@fechaFin as date),cast(dev.fechaAlta as date))


				if not exists (	select 1 from #RESULT )
				begin
					select	@mensaje = 'No existen resultados para esos terminos de búsqueda.',
							@valido = cast(0 as bit)						
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

		--reporte de estatus
			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
							

		-- si todo ok
			if exists ( select 1 from #RESULT )
			begin
				select	idPedidoEspecial,
						idUsuario,
						nombreUsuario, 
						idCliente, 
						nombreCliente,
						idSucursal,
						idAlmacen,
						idProducto,
						descripcionProducto as producto,
						cantidad,
						precioVenta,
						montoTotal, 
						codigoBarras as codigoBarrasTicket,
						descAlmacen as tienda,
						fechaAlta

				from	#RESULT
				order by fechaAlta desc 
			end

				
		end -- reporte de estatus


	end  -- principal
go

grant exec on SP_CONSULTA_DEVOLUCIONES_PEDIDOS_ESPECIALESV2 to public
go
