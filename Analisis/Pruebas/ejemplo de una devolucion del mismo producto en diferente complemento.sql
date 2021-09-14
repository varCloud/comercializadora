begin tran 
exec SP_REALIZA_VENTA
@xml =N'
<ArrayOfVentas xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Ventas>
    <idVenta>0</idVenta>
    <idCliente>0</idCliente>
    <cantidad>50</cantidad>
    <fechaAlta>0001-01-01T00:00:00</fechaAlta>
    <idUsuario>0</idUsuario>
    <idProducto>581</idProducto>
    <idLineaProducto>0</idLineaProducto>
    <fechaIni>0001-01-01T00:00:00</fechaIni>
    <fechaFin>0001-01-01T00:00:00</fechaFin>
    <contador>0</contador>
    <precio>0</precio>
    <costo>0</costo>
    <ganancia>0</ganancia>
    <formaPago>1</formaPago>
    <tipoConsulta>0</tipoConsulta>
    <idFactura>0</idFactura>
    <idEstatusFactura>0</idEstatusFactura>
    <usoCFDI>0</usoCFDI>
    <descuento>0</descuento>
    <montoTotal>0</montoTotal>
    <montoIVA>0</montoIVA>
    <idStatusVenta>0</idStatusVenta>
    <idFactFormaPago>0</idFactFormaPago>
    <idFactUsoCFDI>0</idFactUsoCFDI>
    <idSucursal>0</idSucursal>
    <ticketVistaPrevia>false</ticketVistaPrevia>
    <esDevolucion>false</esDevolucion>
    <esAgregarProductos>false</esAgregarProductos>
    <productosDevueltos>10</productosDevueltos>
    <productosAgregados>0</productosAgregados>
    <idVentaDetalle>163871</idVentaDetalle>
    <idAlmacen>0</idAlmacen>
    <precioVenta>0</precioVenta>
    <cantProductosLiq>0</cantProductosLiq>
    <idPedidoEspecial>0</idPedidoEspecial>
    <estatusVenta>0</estatusVenta>
    <fechaCancelacion>0001-01-01T00:00:00</fechaCancelacion>
    <idDevolucion>0</idDevolucion>
    <idComplemento>0</idComplemento>
    <puedeHacerComplementos>false</puedeHacerComplementos>
    <diasPasadosVentaInicial>0</diasPasadosVentaInicial>
    <tieneCompleODev>0</tieneCompleODev>
    <ultimoCostoCompra>0</ultimoCostoCompra>
  </Ventas>
  <Ventas>
    <idVenta>0</idVenta>
    <idCliente>0</idCliente>
    <cantidad>10</cantidad>
    <fechaAlta>0001-01-01T00:00:00</fechaAlta>
    <idUsuario>0</idUsuario>
    <idProducto>581</idProducto>
    <idLineaProducto>0</idLineaProducto>
    <fechaIni>0001-01-01T00:00:00</fechaIni>
    <fechaFin>0001-01-01T00:00:00</fechaFin>
    <contador>0</contador>
    <precio>0</precio>
    <costo>0</costo>
    <ganancia>0</ganancia>
    <formaPago>1</formaPago>
    <tipoConsulta>0</tipoConsulta>
    <idFactura>0</idFactura>
    <idEstatusFactura>0</idEstatusFactura>
    <usoCFDI>0</usoCFDI>
    <descuento>0</descuento>
    <montoTotal>0</montoTotal>
    <montoIVA>0</montoIVA>
    <idStatusVenta>0</idStatusVenta>
    <idFactFormaPago>0</idFactFormaPago>
    <idFactUsoCFDI>0</idFactUsoCFDI>
    <idSucursal>0</idSucursal>
    <ticketVistaPrevia>false</ticketVistaPrevia>
    <esDevolucion>false</esDevolucion>
    <esAgregarProductos>false</esAgregarProductos>
    <productosDevueltos>5</productosDevueltos>
    <productosAgregados>0</productosAgregados>
    <idVentaDetalle>163872</idVentaDetalle>
    <idAlmacen>0</idAlmacen>
    <precioVenta>0</precioVenta>
    <cantProductosLiq>0</cantProductosLiq>
    <idPedidoEspecial>0</idPedidoEspecial>
    <estatusVenta>0</estatusVenta>
    <fechaCancelacion>0001-01-01T00:00:00</fechaCancelacion>
    <idDevolucion>0</idDevolucion>
    <idComplemento>0</idComplemento>
    <puedeHacerComplementos>false</puedeHacerComplementos>
    <diasPasadosVentaInicial>0</diasPasadosVentaInicial>
    <tieneCompleODev>0</tieneCompleODev>
    <ultimoCostoCompra>0</ultimoCostoCompra>
  </Ventas>
</ArrayOfVentas>'
,@idCliente = 33
,@idFactFormaPago = 2
,@idFactUsoCFDI = 1
,@idVenta = 35792
,@idUsuario = 20
,@idEstacion = 1
,@aplicaIVA = 0
,@numClientesAtendidos = 0
,@tipoVenta  = 2
,@motivoDevolucion = 10
,@idPedidoEspecial =0
,@idVentaComplemento =0
,@montoTotalVenta = 15
,@montoPagado =0

rollback tran