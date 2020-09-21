use [DB_A57E86_lluviadesarrollo]
go

-- se crea procedimiento SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS
if exists (select * from sysobjects where name like 'SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/09/10
Objetivo		CONSULTA LOS DETALLES DE LA TABLA DE PEDIDOS INTERNOS DETALLE
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS

	@idPedidoEspecial		int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@fecha					datetime

				create table
					#productos
						(
							contador							int identity(1,1),
							idPedidoInterno						int, 
							idAlmacenOrigen						int, 
							idAlmacenDestino					int, 
							idUsuario							int, 
							IdEstatusPedidoInterno				int, 
							fechaAlta							datetime, 
							observacion							varchar(255), 
							idTipoPedidoInterno					int, 
							descripcion							varchar(255), 
							idPedidoInternoDetalle				int, 
							idProducto							int, 
							descProducto						varchar(255), 
							cantidad							int, 
							cantidadAceptada					int, 
							cantidadAtendida					int, 
							cantidadRechazada					int
						)
						

				create table 
					#ProductosPorPrecio
						(
							contador				int,
							idProducto				int,
							cantidad				int,
							min						int,
							max						int,
							max_cantidad			int,
							costo					money,
							precioIndividual		money,
							descuento				money,
							activo					bit
						)


			end  --declaraciones 

			begin -- principal

				if not exists ( select 1 from PedidosInternos where idPedidoInterno = @idPedidoEspecial )
				begin
					select @mensaje = 'No existe el pedido, verifique por favor.'
					raiserror (@mensaje, 11, -1)
				end




				insert into 
					#productos
						(
							idPedidoInterno, idAlmacenOrigen, idAlmacenDestino, idUsuario, IdEstatusPedidoInterno, fechaAlta, observacion,
							idTipoPedidoInterno, descripcion, idPedidoInternoDetalle, idProducto, descProducto, cantidad, cantidadAceptada, 
							cantidadAtendida,cantidadRechazada
						)
				select	pi_.idPedidoInterno, idAlmacenOrigen, idAlmacenDestino, idUsuario, IdEstatusPedidoInterno, pi_.fechaAlta, observacion, 
						idTipoPedidoInterno, pi_.descripcion, idPedidoInternoDetalle, pid.idProducto, pro.descripcion as descProducto, cantidad, 
						cantidadAceptada, cantidadAtendida, cantidadRechazada
				from	PedidosInternos pi_
							inner join PedidosInternosDetalle pid
								on pid.idPedidoInterno = pi_.idPedidoInterno
							inner join Productos pro
								on pro.idProducto = pid.idProducto
				where	pi_.idPedidoInterno = @idPedidoEspecial






			-- universo de productos
			insert into 
				#ProductosPorPrecio
					(idProducto,cantidad,min,max,costo,precioIndividual,activo)
			select	p.idProducto, sum(p.cantidad) as cantidad, cast(1 as int) as min_,				
					cast(11 as int) as max_, pro.precioIndividual, pro.precioIndividual, pro.activo
			from	#productos p
						inner join Productos pro
							on pro.idProducto=p.idProducto
			group by p.idProducto, pro.precioIndividual, pro.activo


			-- actualizamos el contador del max_cantidad para el caso de infinito
			update	#ProductosPorPrecio
			set		#ProductosPorPrecio.max_cantidad = A.max_cantidad
			from	(
						select	ppp.idProducto, 
								coalesce(max(ppp.max),0) as max_cantidad 
						from	ProductosPorPrecio ppp
									inner join #ProductosPorPrecio ppp_
										on ppp.idProducto = ppp_.idProducto
						where	ppp.activo = cast(1 as bit)
						group by ppp.idProducto
					)A
			where	#ProductosPorPrecio.idProducto = A.idProducto
			
			-- si se ejecuta precio de mayoreo cuando el ticket tiene 12 o + articulos
			if ( (select sum(cantidad) from #ProductosPorPrecio ) >= 12 )
				begin

					update	#ProductosPorPrecio 
					set		#ProductosPorPrecio.costo = a.precioMenudeo,
							min = 12, 
							max = 12
					from	(
								select	ppp.idProducto, p.precioIndividual, p.precioMenudeo
								from	#ProductosPorPrecio ppp 
											inner join Productos p 
												on ppp.idProducto = p.idProducto
							)A
					where	#ProductosPorPrecio.idProducto = a.idProducto
				
				end

				
			-- actualizamos los que caigan en un rango
			update	#ProductosPorPrecio
			set		#ProductosPorPrecio.min = a.min,
					#ProductosPorPrecio.max = a.max,
					#ProductosPorPrecio.costo = a.costo,
					#ProductosPorPrecio.contador = a.contador
			from	(
						select	ppp.contador, ppp.idProducto, ppp.min, ppp.max, ppp.costo, ppp.activo
						from	#ProductosPorPrecio ppp_
									inner join ProductosPorPrecio ppp
										on ppp.idProducto = ppp_.idProducto
						where	ppp_.cantidad between ppp.min and ppp.max
							and	ppp.activo = cast(1 as bit)
					)A
			where	#ProductosPorPrecio.idProducto = a.idProducto

			-- si hay max-cantidad
			if exists ( select 1 from #ProductosPorPrecio where cantidad > max_cantidad )
				begin
					
					update	#ProductosPorPrecio
					set		#ProductosPorPrecio.min = a.min,
							#ProductosPorPrecio.max = a.max,
							#ProductosPorPrecio.costo = a.costo,
							#ProductosPorPrecio.contador = a.contador
					from	(
								select	ppp.contador, ppp.idProducto, ppp.min, ppp.max, ppp.costo, ppp.activo
								from	#ProductosPorPrecio ppp_
											inner join ProductosPorPrecio ppp
												on ppp.idProducto = ppp_.idProducto
												and ppp_.max_cantidad = ppp.max
								where	ppp.activo = cast(1 as bit)
									and ppp_.cantidad > ppp_.max_cantidad
							)A
					where	#ProductosPorPrecio.idProducto = a.idProducto

				end

			-- actualizamos los descuentos
				update	#ProductosPorPrecio set descuento = precioIndividual - costo

		
			--select '#ProductosPorPrecio', * from #ProductosPorPrecio















				if not exists ( select 1 from #productos )
				begin
					select @mensaje = 'No hay registros para el pedido seleccionado.'
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

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje,
					@idPedidoEspecial as idPedidoEspecial

			if exists (select 1 from #productos)
				begin
					select	* , ppp.min, ppp.max, ppp.costo, ppp.descuento, (ppp.costo*p.cantidad) as monto, ( ppp.descuento*p.cantidad ) as ahorro 
					from	#productos p
								inner join #ProductosPorPrecio ppp 
									on p.idProducto = ppp.idProducto
				end
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS to public
go



