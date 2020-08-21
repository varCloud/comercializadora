--almacen 4 solicita mercancia al 1
--uusuario 6 usuario 5
--969
declare 
@idProducto int=627,
@idUsuario int=6,
@idAlamacenOrigen int=4,
@idAlamacenDestino int=1,
@cantidad int=50

--EXEC SP_APP_GENERAR_PEDIDO_INTERNO @idProducto,@idUsuario,
--@idAlamacenOrigen,@idAlamacenDestino,@cantidad

declare 
@idPedidoInterno int=18,
@idUbcacion int=3,
@observacion varchar(500)='',
@idEstatusPedidoInterno int=2

select
@idUsuario=5,
@idAlamacenOrigen=1,
@idAlamacenDestino=4,
@cantidad=30

--EXEC [dbo].[SP_APP_ACTUALIZA_ESTATUS_PEDIDO_INTERNO] @idPedidoInterno,
--@idUsuario,@idEstatusPedidoInterno,@idAlamacenOrigen,@idAlamacenDestino,@idUbcacion,@observacion,
--@cantidad

select
@idUsuario=6,
@idAlamacenOrigen=4,
@idAlamacenDestino=1,
@cantidad=50


--EXEC [dbo].[SP_APP_ACEPTA_PEDIDO_INTERNO] @idPedidoInterno,@idUsuario,@idAlamacenOrigen,@idAlamacenDestino,
--@cantidad,@observacion

select
@idUsuario=6,
@idAlamacenOrigen=4,
@idAlamacenDestino=1--,



EXEC [dbo].[SP_APP_RECHAZA_PEDIDO_INTERNO] @idPedidoInterno,@idUsuario,@idAlamacenOrigen,@idAlamacenDestino,
@observacion

select * from InventarioGeneral where idproducto=@idProducto
select u.idAlmacen,* from InventarioDetalle d
join Ubicacion u on d.idUbicacion=u.idUbicacion where idproducto=@idProducto
select * from InventarioDetalleLog where idproducto=@idProducto
select * from PedidosInternos where idPedidoInterno=@idPedidoInterno

--select * from CatEstatusPedidoInterno

--select * from Ubicacion

--EXEC [dbo].[SP_AGREGAR_PRODUCTO_INVENTARIO] 626,1,100,5
--EXEC [dbo].[SP_APP_AGREGAR_PRODUCTO_INVENTARIO] 627,1,100,5,1,4

--select * from CatTipoMovimientoInventario

--Alter table PedidosInternosDetalle add cantidadRechazada int default 0 