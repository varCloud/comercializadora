begin tran

select * from InventarioDetalle where  idProducto  = 744
select top 200 * from InventarioDetalleLog where  idProducto  = 744 order by idInventarioDetalleLOG desc

exec SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2
@listaProductos = N'
<ArrayOfProducto xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Producto>
    <idProducto>744</idProducto>
    <cantidadUnidadMedida>0</cantidadUnidadMedida>
    <activo>false</activo>
    <cantidad>0</cantidad>
    <fechaAlta>0001-01-01T00:00:00</fechaAlta>
    <fechaIni>0001-01-01T00:00:00</fechaIni>
    <fechaFin>0001-01-01T00:00:00</fechaFin>
    <contador>0</contador>
    <precioIndividual>0</precioIndividual>
    <precioMenudeo>0</precioMenudeo>
    <precio>0</precio>
    <idPasillo>0</idPasillo>
    <idRaq>0</idRaq>
    <idPiso>0</idPiso>
    <idAlmacen>0</idAlmacen>
    <idSucursal>0</idSucursal>
    <idUsuario>0</idUsuario>
    <costo>0</costo>
    <total>0</total>
    <observaciones>prueba ademas de que se rechaza en HH tambien rechazamos por admin </observaciones>
    <estatusProducto>
      <idEstatusProducto>0</idEstatusProducto>
    </estatusProducto>
    <cantidadRecibida>0</cantidadRecibida>
    <cantidadDevuelta>0</cantidadDevuelta>
    <porcUtilidadIndividual>0</porcUtilidadIndividual>
    <porcUtilidadMayoreo>0</porcUtilidadMayoreo>
    <unidadCompra>
      <idUnidadCompra>0</idUnidadCompra>
      <cantidadUnidadCompra>0</cantidadUnidadCompra>
    </unidadCompra>
    <fraccion>false</fraccion>
    <idUbicacion>0</idUbicacion>
    <ultimoCostoCompra>0</ultimoCostoCompra>
    <idPedidoEspecialDetalle>53256</idPedidoEspecialDetalle>
    <idPedidoEspecial>0</idPedidoEspecial>
    <idVenta>0</idVenta>
    <idAlmacenOrigen>0</idAlmacenOrigen>
    <idAlmacenDestino>0</idAlmacenDestino>
    <cantidadActualInvGeneral>0</cantidadActualInvGeneral>
    <cantidadAnteriorInvGeneral>0</cantidadAnteriorInvGeneral>
    <cantidadActualInvAlmacen>0</cantidadActualInvAlmacen>
    <precioVenta>0</precioVenta>
    <cantidadSolicitada>10</cantidadSolicitada>
    <cantidadAtendida>8</cantidadAtendida>
    <cantidadAceptada>6</cantidadAceptada>
    <cantidadRechazada>2</cantidadRechazada>
    <idTicketMayoreo>0</idTicketMayoreo>
    <idUsuarioRuteo>0</idUsuarioRuteo>
    <idEstatusPedidoEspecialDetalle>0</idEstatusPedidoEspecialDetalle>
    <id>0</id>
  </Producto>
</ArrayOfProducto>'
,@idPedidoEspecial = 6434
,@idEstatusPedidoEspecial = 6
,@idUsuarioEntrega = 9
,@numeroUnidadTaxi = 0
,@idEstatusCuentaPorCobrar = 0
,@montoPagado = 600
,@aCredito =0
,@aCreditoConAbono =0
,@aplicaIVA = 0
,@idFactFormaPago = 1
,@idFactUsoCFDI = 0
,@observacionesPedidoRuta = 'pruebas'
,@idUsuarioRuteo =0
,@esPedidoEnRuta = 0
,@idUsuarioLiquida = 0

select * from InventarioDetalle where  idProducto  = 744
select top 200 * from InventarioDetalleLog where  idProducto  = 744 order by idInventarioDetalleLOG desc
declare
@idPedidoEspecial bigint = 6434 
select * from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial
rollback tran 