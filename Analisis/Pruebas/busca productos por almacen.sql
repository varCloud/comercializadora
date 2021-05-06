

select * from Productos P join  LineaProducto L
on P.idLineaProducto = L.idLineaProducto
where L.idLineaProducto is null

select L.idLineaProducto, P.idLineaProducto
from LineaProducto L Left join  Productos P
on P.idLineaProducto = L.idLineaProducto
where P.idLineaProducto is null


select * from LineaProducto where idLineaProducto not in (
select idLineaProducto from Productos
)


update LineaProducto set activo = 0 where idLineaProducto in (
3,
14,
15,
16,
17,
18
)

select * from Usuarios



select * from Productos where idLineaProducto in (8,26,13)

--8 BAS ESTOY YO
--13 producto de prueba 567 -- erny
--26 BASTON SI TIENE




select * from LineaProducto  order by descripcion asc


select * from Almacenes
select * from Usuarios where idAlmacen = 1
select P.idProducto, P.descripcion ,* from InventarioDetalle ID  
join Ubicacion  U on ID.idUbicacion = U.idUbicacion 
join Productos P on P.idProducto = ID.idProducto
where 
P.idProducto = 1570
--U.idAlmacen = 1 
and ID.cantidad >0 order by  P.descripcion asc


select * from Ubicacion where idUbicacion in (4,1)

select * from InventarioGeneral where idProducto = 1
select * from InventarioDetalle where idProducto =1 

select * from Productos

--delete from InventarioGeneral where idProducto = 1
--delete from InventarioDetalle where idProducto = 1