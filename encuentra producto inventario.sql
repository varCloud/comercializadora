


select idLineaProducto, * from productos where descripcion like '%SILLA CHILLONA%'
select * from LineaProducto




SELECT A.Descripcion,P.descripcion, L.descripcion linea , I.cantidad , U.* FROM InventarioDetalle I join Ubicacion U on I.idUbicacion = U.idUbicacion
join Almacenes A on A.idAlmacen = U.idAlmacen
join Productos P on P.idProducto = I.idProducto and P.activo = 1
join LineaProducto L on L.idLineaProducto = P.idLineaProducto
where P.idProducto = 2804


select * from Ubicacion where idUbicacion in (
SELECT idUbicacion FROM InventarioDetalle where idProducto = 2804
)

select * from LimitesInventario where  idProducto = 2804