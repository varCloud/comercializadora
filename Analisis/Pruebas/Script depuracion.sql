 
--DELETE FROM Compras
--DBCC CHECKIDENT ('Compras',RESEED, 0)

--DELETE FROM ComprasDetalle
--DBCC CHECKIDENT ('ComprasDetalle',RESEED, 0)

--DELETE FROM Proveedores

--DELETE from MovimientosDeMercancia
--DBCC CHECKIDENT ('MovimientosDeMercancia',RESEED, 0)

--DELETE FROM [dbo].[PedidosInternosLog]
--DBCC CHECKIDENT ('[PedidosInternosLog]',RESEED, 0)

--DELETE FROM [dbo].[PedidosInternosDetalle]
--DBCC CHECKIDENT ('[PedidosInternosDetalle]',RESEED, 0)

--DELETE FROM [dbo].[PedidosInternos]
--DBCC CHECKIDENT ('[PedidosInternos]',RESEED, 0)

--DELETE FROM  [dbo].[InventarioDetalle]
--DBCC CHECKIDENT ('[InventarioDetalle]',RESEED, 0)

--DELETE FROM  [dbo].[InventarioDetalleLog]
--DBCC CHECKIDENT ('[InventarioDetalleLog]',RESEED, 0)

--DELETE Usuarios where usuario!='admin' and contrasena!='admin'


--DELETE FROM [dbo].[VentasDetalle]
--DBCC CHECKIDENT ('[VentasDetalle]',RESEED, 0)

--DELETE FROM [dbo].[Ventas] 
--DBCC CHECKIDENT ('[Ventas]',RESEED, 0)

--DELETE FROM [dbo].[Estaciones] where idEstacion>1
--DBCC CHECKIDENT ('[Estaciones]',RESEED, 0)

--DELETE from [dbo].[Facturas]
--DBCC CHECKIDENT ('[Facturas]',RESEED, 0)

--DELETE FROM [dbo].[Clientes]
--DBCC CHECKIDENT ('[Clientes]',RESEED, 0)


--DELETE a from [dbo].[InventarioGeneral] a
--join Productos b on a.idProducto=b.idProducto
--where b.idProducto>627

--DELETE a from [dbo].[ProductosPorPrecio] a
--join Productos b on a.idProducto=b.idProducto
--where b.idProducto>627

--DELETE from Productos 
--where idProducto>627

--DELETE from [dbo].[LineaProducto] where idLineaProducto>18
--DBCC CHECKIDENT ('[LineaProducto]',RESEED, 18)


/************************************************/
/**   PARA LIMPAR INVENTARIO
************************************************/


DELETE FROM ComprasDetalle
DBCC CHECKIDENT ('[ComprasDetalle]',RESEED, 0)


DELETE FROM Compras
DBCC CHECKIDENT ('[compras]',RESEED, 0)

DELETE FROM InventarioGeneral
DBCC CHECKIDENT ('[InventarioGeneral]',RESEED, 0)


DELETE FROM InventarioDetalleLog
DBCC CHECKIDENT ('[InventarioDetalleLog]',RESEED, 0)

delete from InventarioDetalle
DBCC CHECKIDENT ('[InventarioDetalle]',RESEED, 0)

delete from MovimientosDeMercancia
DBCC CHECKIDENT ('[MovimientosDeMercancia]',RESEED, 0)

-- DELETE from Usuarios where idRol != 1






