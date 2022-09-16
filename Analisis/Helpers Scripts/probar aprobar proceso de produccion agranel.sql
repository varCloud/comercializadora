select * from ProcesoProduccionAgranel
-- select * from CatTipoMovimientoInventario 

begin tran
	declare @idProd int = 780
	select * from InventarioDetalle where idProducto = @idProd
	select * from InventarioDetalleLog where idProducto in (@idProd) order by idInventarioDetalleLOG desc
	exec SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL
	'<ArrayOfProductosProduccionAgranel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	  <ProductosProduccionAgranel>
		<idProcesoProduccionAgranel>5</idProcesoProduccionAgranel>
		<idProducto>854</idProducto>
		<idUbicacion>3876</idUbicacion>
		<cantidadAtendida>100</cantidadAtendida>
		<observaciones>todo bien </observaciones>
		<idEstatusProduccionAgranel>0</idEstatusProduccionAgranel>
	  </ProductosProduccionAgranel>
	</ArrayOfProductosProduccionAgranel>',11,2
	
	select TM.descripcion,IDL.* from InventarioDetalleLog IDL join CatTipoMovimientoInventario TM on IDL.idTipoMovInventario = TM.idTipoMovInventario where idProducto in (@idProd) order by idInventarioDetalleLOG desc
	select * from Ubicacion where idUbicacion in (
		select idUbicacion from InventarioDetalleLog where idProducto in (@idProd) )
	select * from InventarioDetalle where idProducto = @idProd
	select * from InventarioGeneral where idProducto = @idProd
	select * from ProcesoProduccionAgranel
commit tran 


exec [SP_CONSULTA_COSTO_PRODUCCION]

@mesCalculo = 9
,@anioCalculo = 2022
,@idLinea = null
,@idAlmacen = null
,@silent = 0

exec [SP_DASHBOARD_COSTO_PRODUCCION]
select * from ReporteCostoProduccion

--select * from ProcesoProduccionAgranel



--select * from Ubicacion where idUbicacion = 3876

--select * from Ubicacion where idPasillo = 1000

--SELECT * from UbicacioN where idPasillo=1000 and idRaq=1000 and idPiso=1000 and idAlmacen=2;

select * from Productos where idLineaProducto = 12;


