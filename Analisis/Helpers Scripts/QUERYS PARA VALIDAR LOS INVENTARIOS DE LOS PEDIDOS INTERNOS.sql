 -- PRUEBAS
begin tran 

SELECT * FROM InventarioDetalleLog WHERE idProducto = 1460 ORDER BY idInventarioDetalleLOG desc
SELECT * FROM InventarioDetalle WHERE idProducto = 1460 ORDER BY idInventarioDetalle desc
select 'InventarioGeneral' , * from InventarioGeneral where idProducto = 1460
select sum(cantidad) as cantidad from InventarioDetalle WHERE idProducto = 1460
-- update InventarioGeneral set cantidad = 250 where  idProducto = 3361
	EXEC SP_APP_ACTUALIZA_ESTATUS_PEDIDO_INTERNO
	@idPedidoInterno=28786,
	@idUsuario=5,
	@idEstatusPedidoInterno=2,
	@idAlmacenOrigen=1,
	@idAlmacenDestino=4,
	@idUbcacion=1473,
	@observacion=null,
	@cantidadAtendida =400

	--EXEC SP_APP_ACEPTA_PEDIDO_INTERNO
	--@idPedidoInterno=28426,
	--@idUsuario=6,
	--@idAlmacenOrigen=4,
	--@idAlmacenDestino=1,
	--@observacion='todo bien',
	--@cantidadAceptada=50


	--EXEC SP_APP_RECHAZA_PEDIDO_INTERNO
	--@idPedidoInterno=26433,
	--@idUsuario=6,
	--@idAlmacenOrigen=4,
	--@idAlmacenDestino=1,
	--@observacion='Se nos fue el client'

	SELECT TM.descripcion, U.idAlmacen,U.idUbicacion,CP.descripcion, ID.* FROM InventarioDetalleLog ID 
	join Ubicacion U on ID.idUbicacion = U.idUbicacion 
	join CatPasillo CP on CP.idPasillo = U.idPasillo
	join CatTipoMovimientoInventario TM on TM.idTipoMovInventario = ID.idTipoMovInventario
	WHERE idProducto = 1460 ORDER BY idInventarioDetalleLOG desc

	SELECT * FROM InventarioDetalle WHERE idProducto = 1460 ORDER BY idInventarioDetalle desc
	select 'InventarioGeneral' , * from InventarioGeneral where idProducto = 1460
	select sum(cantidad) as cantidad from InventarioDetalle WHERE idProducto = 1460
rollback tran

--select * from Ubicacion

--select * from PedidosInternos order by fechaAlta desc

--select * from CatTipoMovimientoInventario

-- update InventarioGeneral set cantidad  = 5448 where idProducto = 582