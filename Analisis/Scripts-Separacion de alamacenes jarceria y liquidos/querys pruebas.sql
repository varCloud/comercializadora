
select * from Usuarios
select * from CatTipoAlmacen
select * from Usuarios where activo =1 and idAlmacen =4

select * from Almacenes

SELECT * FROM [dbo].[AlmacenesXLineaProducto]
--update Almacenes set Descripcion = 'Tienda Jarceria' where idAlmacen = 4
--update Almacenes set idTipoAlmacen = 1 where idAlmacen = 2
--update Almacenes set Descripcion = 'Tienda Liquidos', idTipoAlmacen = 3 where idAlmacen = 3

select * from LineaProducto

select * from obtnerLineasProductosXAlmacen(10, 4)
select * from obtnerLineasProductosXAlmacen(null, 4)

select * from InventarioDetalle where idProducto = 279
select * from Ubicacion where idUbicacion in (
select idUbicacion from InventarioDetalle where idProducto = 279
)

select * from Productos where idLineaProducto = 2
select * from Productos where idLineaProducto = 20
select * from PedidosInternos order by idPedidoInterno desc

select * from InventarioDetalle I join Ubicacion  U on I.idUbicacion = U.idUbicacion
where U.idAlmacen in (2,3)

select * from InventarioDetalleLog where idProducto = 2610 order by idInventarioDetalleLOG desc

select * from MovimientosDeMercancia where idProducto = 2610



INSERT INTO AlmacenesXLineaProducto (idAlmacen,idLineaProducto,fechAlta,activo) SELECT  1/*Almacen Jarceria*/,idLineaProducto,GETDATE(),
case when idLineaProducto = 20 then 0 else 1 end FROM  LineaProducto WHERE activo = 1

INSERT INTO AlmacenesXLineaProducto (idAlmacen,idLineaProducto,fechAlta,activo) SELECT  2/*Almacen Liquidos*/,idLineaProducto,GETDATE(),
case when idLineaProducto = 20 then 1 else 0 end FROM  LineaProducto WHERE activo = 1

INSERT INTO AlmacenesXLineaProducto (idAlmacen,idLineaProducto,fechAlta,activo) SELECT  3 /*tienda liquidos*/,idLineaProducto,GETDATE(),1 FROM  LineaProducto WHERE activo = 1
INSERT INTO AlmacenesXLineaProducto (idAlmacen,idLineaProducto,fechAlta,activo) SELECT  4/*tienda Jarceria*/,idLineaProducto,GETDATE(),1 FROM  LineaProducto WHERE activo = 1


update  AlmacenesXLineaProducto  set activo = 0 where idAlmacen = 3 and idLineaProducto <> 20
update  AlmacenesXLineaProducto  set activo = 0 where idAlmacen = 4 and idLineaProducto = 20


select * from [AlmacenesXLineaProducto] WHERE idAlmacen = 3 and activo = 1

select * from [AlmacenesXLineaProducto] WHERE idAlmacen = 4 and activo = 1


-- select * from  PedidosInternos where IdEstatusPedidoInterno = 2 and idPedidoInterno = 579
-- update  PedidosInternos set  notificado = 0 where IdEstatusPedidoInterno = 2 and idPedidoInterno = 579

select * from PedidosInternosDetalle where idPedidoInterno in (
select idPedidoInterno from  PedidosInternos where IdEstatusPedidoInterno = 2 
) and idProducto = 492

select * from Productos where idProducto in (1570,
492,
2631,
2630)


select * from MovimientosDeMercancia where idPedidoInterno = 3080 order by idMovMercancia

delete from  MovimientosDeMercancia where idMovMercancia in (5192,
5193)

select * from PedidosInternosDetalle where idPedidoInterno = 3080

select * from LineaProducto where activo = 1





exec [SP_APP_OBTENER_UBICACION_PRODUCTO_INVENTARIO]
@idAlmacen = 1 
,@idProducto = null 
,@EstatusProducto = 3

exec [SP_APP_OBTENER_UBICACION_PRODUCTO_INVENTARIO]
@idAlmacen = null
,@idProducto = 489 
,@EstatusProducto = 3
,@idUsuario = 10

select * from InventarioDetalle

-- update  AlmacenesXLineaProducto  set activo = 0 where idAlmacen = 4 and idLineaProducto = 20
-- update  AlmacenesXLineaProducto  set activo = 1 where idAlmacen = 4 and idLineaProducto = 2