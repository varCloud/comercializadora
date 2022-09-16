		--select * from InventarioDetalle where idProducto in (133)
		--select SUM(cantidad) as total from InventarioDetalle where idProducto in (133)
		--select * from InventarioGeneral where idProducto in (133)

begin tran
		create table #productos_ubicacion(
				id int identity(1,1),
				idProducto int,
				cantidad float,
				idInventarioDetalle bigint, 
				idUbicacion int,
		)
		declare @idUbicacion bigint = 3860 /*3860 si ubicacion almacen 1*/

		insert into #productos_ubicacion (idProducto ,cantidad ,idInventarioDetalle ,idUbicacion)
		select  top 5 idProducto , cantidad , idInventarioDetalle , idUbicacion from InventarioDetalle where idUbicacion = @idUbicacion and cantidad < 0
		--and idProducto not in (

		--	select ID.idProducto from PedidosInternos P 
		--	join PedidosInternosDetalle PD on P.idPedidoInterno  = PD.idPedidoInterno
		--	join InventarioDetalle ID on idUbicacion = 3860 and ID.cantidad > 0 and PD.idProducto = ID.idProducto
		--	where P.fechaAlta >='20220804' and IdEstatusPedidoInterno = 4
		--)

		select * from #productos_ubicacion
		

		DECLARE
		@ini int = 0,
		@fin int = 0
		select @ini = min(id), @fin= max(id) from #productos_ubicacion
		WHILE ( @ini <= @fin )
		BEGIN
				DECLARE
				@cantidad float = 0,
				@cantidadInventarioGeneral float = 0,
				@idInventarioDetalle int = 0,
				@idProducto bigint = 0


				SELECT @idProducto = idProducto, @idInventarioDetalle = @idInventarioDetalle ,@cantidad = cantidad from #productos_ubicacion where id = @ini

				--select * from InventarioDetalle where idProducto in (@idProducto)
				--select SUM(cantidad) as total from InventarioDetalle where idProducto in (@idProducto)
				--select * from InventarioGeneral where idProducto in (@idProducto)

						
				--SET DE VALORES
				SELECT @idProducto = idProducto, @idInventarioDetalle = idInventarioDetalle ,@cantidad = cantidad from #productos_ubicacion where id = @ini

				-- DESCOMENTAR PARA LOS VALORES NEGATIVOS
				--set valores negativos
				 update InventarioDetalle set cantidad = cantidad + @cantidad where idInventarioDetalle =(
				 		select max(ID.idInventarioDetalle)  from InventarioDetalle ID join Ubicacion U on ID.idUbicacion = U.idUbicacion
						where ID.idProducto = @idProducto and U.idAlmacen =1 and cantidad = (
									select max(cantidad) from InventarioDetalle ID join Ubicacion U on ID.idUbicacion = U.idUbicacion
									where ID.idProducto = @idProducto and U.idAlmacen =1)
						)

				 


				UPDATE InventarioDetalle set cantidad = 0  where idInventarioDetalle = @idInventarioDetalle and idUbicacion = @idUbicacion
				select @cantidadInventarioGeneral = (SUM(isnull(cantidad,0))) from InventarioDetalle where idProducto in (@idProducto)
				UPDATE InventarioGeneral set cantidad = (@cantidadInventarioGeneral)  where  idProducto in (@idProducto)
	
				select * from InventarioDetalle where idProducto in (@idProducto)
				select SUM(cantidad) as totalInventarioDetalle from InventarioDetalle where idProducto in (@idProducto)
				select * from InventarioGeneral where idProducto in (@idProducto)
				
				SET @ini = @ini +1;
		END
			
			select COUNT(*) from InventarioDetalle where idUbicacion = 3860 and cantidad < 0
			select COUNT(*) from InventarioDetalle where idUbicacion = 3860 and cantidad > 0
		drop table #productos_ubicacion
commit tran 