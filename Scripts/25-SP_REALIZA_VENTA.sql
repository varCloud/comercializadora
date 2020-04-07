use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_REALIZA_VENTA
if exists (select * from sysobjects where name like 'SP_REALIZA_VENTA' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_REALIZA_VENTA
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		guarda la venta en el sistema
status			200 = ok
				-1	= error
*/

create proc SP_REALIZA_VENTA

  @xml AS XML

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Se registro la venta correctamente.',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@tran_name				varchar(32) = 'REALIZA_VTA',
						@tran_count				int = @@trancount,
						@tran_scope				bit = 0,
						@idCliente				int = 0,
						@idFactFormaPago		varchar(50) = '',
						@idUsuario				int = 0,
						@totalProductos			int = 0,
						@montoTotal				money = 0,
						@idVenta				int = 0,
						@descuento				money = 0.0

				create table
					#Ventas
						(	
							contador			int identity(1,1),
							idCliente			int,
							cantidad			float,
							fechaAlta			datetime,
							montoTotal			money,
							idUsuario			int,
							idStatusVenta		int,
							idFactFormaPago		int
						)

				create table
					#VentasDetalle
						(
							idVentaDetalle					int,
							idVenta							int,
							idProducto						int,
							cantidad						float,
							contadorProductosPorPrecio		int,
							monto							money,
							cantidadActualInvGeneral		float,
							cantidadAnteriorInvGeneral		float						
						)

				create table 
					#cantidades 
						(  
							id			int identity(1,1),
							cantidad	int
						)

				create table 
					#idProductos 
						(  
							id			int identity(1,1),
							idProducto	int
						)
						

			end  --declaraciones 

			begin -- principal

				begin -- inicio transaccion
					if @tran_count = 0
						begin tran @tran_name
					else
						save tran @tran_name
					select @tran_scope = 1
				end -- inicio transaccion


				select  @idCliente =		( SELECT top 1  Ventas.idCliente.value('.','NVARCHAR(200)') AS idCliente FROM @xml.nodes('//idCliente') as Ventas(idCliente))
				select  @idFactFormaPago =	( SELECT top 1  Ventas.formaPago.value('.','NVARCHAR(200)') AS formaPago FROM @xml.nodes('//formaPago') as Ventas(formaPago))
				select  @idUsuario =		( SELECT top 1  Ventas.idUsuario.value('.','NVARCHAR(200)') AS idUsuario FROM @xml.nodes('//idUsuario') as Ventas(idUsuario))
				select  @idVenta =			( SELECT top 1  Ventas.idVenta.value('.','NVARCHAR(200)') AS idVenta FROM @xml.nodes('//idVenta') as Ventas(idVenta))
				

				insert into #cantidades (cantidad)
				SELECT Ventas.cantidad.value('.','NVARCHAR(200)') AS cantidad FROM @xml.nodes('//cantidad') as Ventas(cantidad) 

				insert into #idProductos (idProducto)
				SELECT Ventas.idProducto.value('.','NVARCHAR(200)') AS idProducto FROM @xml.nodes('//idProducto') as Ventas(idProducto)

				
				-- universo de venta de productos
				select	p.idProducto, c.cantidad, pre.contador, pre.costo,
						pre.costo * c.cantidad as precioPorProducto
				into	#vta
				from	#cantidades c
						inner join #idproductos p
							on c.id=p.id
						inner join ProductosPorPrecio pre
							on pre.idProducto = p.idProducto
				where	c.cantidad between pre.min and pre.max 
					and	pre.activo = cast(1 as bit)
					

				-- si existe el producto en el inventario general
				if not exists	( 
									select 1 from InventarioGeneral where idProducto not in ( select distinct idProducto from #vta )
								)
				begin
					select @mensaje = 'El producto no se encuentra en el inventario.'
					raiserror (@mensaje, 11, -1)
				end
				
				-- validamos q tenga el suficiente inventario
				if  exists	( 
								select	i.cantidad - v.cantidad as cantidadActualInvGeneral,
										i.cantidad as cantidadAnteriorInvGeneral
								from	#vta v
										inner join InventarioGeneral i
											on v.idProducto = i.idProducto
								where	( ( i.cantidad - v.cantidad ) < 0 )
							)
				begin
					select @mensaje = 'No se cuenta con suficientes existencias en el inventario.'
					raiserror (@mensaje, 11, -1)
				end

				-- validamos si el cliente tiene descuento por aplicar
				select	@descuento = t.descuento
				from	clientes c
						inner join CatTipoCliente t
							on c.idTipoCliente = t.idTipoCliente
				where	c.idCliente = @idCliente

				-- si hay descuento
				if ( @descuento > 0.0 )
				begin
					update	#vta set #vta.precioPorProducto = #vta.precioPorProducto - (#vta.precioPorProducto * ( @descuento / 100 ))
				end

				-- si es una edicion de venta se cancela el ticket actual y para insertar los nuevos datos
				--if( @idVenta > 0 )
				--	begin
						
				--		update	Ventas	
				--		set		idStatusVenta = 2
				--		where	idVenta = @idVenta



				--	end
				

				-- si todo bien
				select @montoTotal = sum(precioPorProducto) from #vta
				select @totalProductos = sum(cantidad) from #vta


				-- inserta en tablas fisicas
				insert into Ventas (idCliente,cantidad,fechaAlta,montoTotal,idUsuario,idStatusVenta,idFactFormaPago)
				select	@idCliente as idCliente, @totalProductos as cantidad , GETDATE() as fechaAlta, 
						@montoTotal as montoTotal, @idUsuario as idUsuario, 1 as idStatusVenta,
						@idFactFormaPago as idFactFormaPago

				select @idVenta = max(idVenta) from Ventas

				insert into VentasDetalle (idVenta,idProducto,cantidad,contadorProductosPorPrecio,monto,cantidadActualInvGeneral,cantidadAnteriorInvGeneral)
				select	@idVenta as idVenta, v.idProducto, v.cantidad, v.contador, v.precioPorProducto, i.cantidad - v.cantidad as cantidadActualInvGeneral,
						i.cantidad as cantidadAnteriorInvGeneral
				from	#vta v
						inner join InventarioGeneral i
							on v.idProducto = i.idProducto

				-- actualizar el inventario general	
				update	InventarioGeneral
				set		cantidad = A.cantidadActualInvGeneral,
						fechaUltimaActualizacion = getdate()
				from	(
							select	idProducto, cantidadActualInvGeneral 
							from	VentasDetalle 
							where	idVenta = @idVenta
						)A
				where	InventarioGeneral.idProducto = A.idProducto



				begin -- commit de transaccion
					if @tran_count = 0
						begin -- si la transacción se inició dentro de este ámbito
							commit tran @tran_name
							select @tran_scope = 0
						end -- si la transacción se inició dentro de este ámbito
				end -- commit de transaccion
					
				drop table #Ventas
				drop table #VentasDetalle
				drop table #cantidades
				drop table #idProductos
				
			end -- principal

		end try

		begin catch 
		
			-- captura del error
			select	@status =			-error_state(),
					@error_procedure =	error_procedure(),
					@error_line =		error_line(),
					@mensaje =			error_message()

			-- revertir transacción si es necesario
			if @tran_scope = 1
				rollback tran @tran_name
					
		end catch

		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje

		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_REALIZA_VENTA to public
go






