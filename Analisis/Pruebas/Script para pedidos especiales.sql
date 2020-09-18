
BEGIN TRAN
--select * from  CatTipoMovimientoInventario
--SELECT * FROM MovimientosDeMercancia WHERE idPedidoInterno = 173
select top 1 * from InventarioDetalleLog where idUbicacion = 1 and idProducto = 1 order by  idInventarioDetalleLOG desc
select top 1 * from InventarioDetalleLog where idUbicacion = 1 and idProducto = 2 order by  idInventarioDetalleLOG desc
select top 1 * from InventarioDetalleLog where idUbicacion = 1 and idProducto = 3 order by  idInventarioDetalleLOG desc

exec SP_APP_APROBAR_PEDIDOS_INTERNOS_ESPECIALES
@productos='<?xml version="1.0" encoding="utf-8"?>
<ArrayOfProductosPedidoEspecial xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ProductosPedidoEspecial>
    <idPedidoInternoDetalle>465</idPedidoInternoDetalle>
    <idUbicacion>1</idUbicacion>
    <idProducto>1</idProducto>
    <cantidadAtendida>4</cantidadAtendida>
    <observaciones>solo tengo 4</observaciones>
    <cantidadSolicitada>0</cantidadSolicitada>
  </ProductosPedidoEspecial>
  <ProductosPedidoEspecial>
    <idPedidoInternoDetalle>466</idPedidoInternoDetalle>
    <idUbicacion>1</idUbicacion>
    <idProducto>2</idProducto>
    <cantidadAtendida>10</cantidadAtendida>
    <observaciones>todo ok </observaciones>
    <cantidadSolicitada>0</cantidadSolicitada>
  </ProductosPedidoEspecial>
  <ProductosPedidoEspecial>
    <idPedidoInternoDetalle>467</idPedidoInternoDetalle>
    <idUbicacion>1</idUbicacion>
    <idProducto>3</idProducto>
    <cantidadAtendida>20</cantidadAtendida>
    <observaciones>todo bien </observaciones>
    <cantidadSolicitada>0</cantidadSolicitada>
  </ProductosPedidoEspecial>
</ArrayOfProductosPedidoEspecial>',
@idPedidoInterno=173,
@idUsuario = 5,
@idAlmacenOrigen = 4,
@idAlmacenDestino = 1

select top 2 * from InventarioDetalleLog where idUbicacion = 1 and idProducto = 1 order by  idInventarioDetalleLOG desc
select top 2 * from InventarioDetalleLog where idUbicacion = 1 and idProducto = 2 order by  idInventarioDetalleLOG desc
select top 2 * from InventarioDetalleLog where idUbicacion = 1 and idProducto = 3 order by  idInventarioDetalleLOG desc
SELECT * FROM PedidosInternos WHERE  idPedidoInterno = 176

SELECT * FROM MovimientosDeMercancia WHERE idPedidoInterno = 176 and idEstatusPedidoInterno = 2
select * from InventarioDetalleLog where idPedidoInterno = 176
select * from PedidosInternosLog WHERE  idPedidoInterno = 176

ROLLBACK TRAN 



