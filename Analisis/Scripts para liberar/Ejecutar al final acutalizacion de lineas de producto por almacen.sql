/*
LIQUIDOS
20.	L�quidos
12.	MPL
19.	Envases
22.	Corporal
25.	Mascotas

JARCIER�A
26.	Bast�n
9.	Bisuter�a
10.	Bolsas
5.	Escobas
7.	Hogar
6.	Industrial
2.	Jarcier�a
11.	Jugueter�a
21.	Matra
23.	Pl�sticos
1.	Qu�micos 
4.	Trapeador
*/

--select * from Almacenes
--select * from [dbo].[AlmacenesXLineaProducto] where idAlmacen in (1,4) and activo = 1

--select * from [dbo].[AlmacenesXLineaProducto] where idAlmacen in (2,3)



update 
[dbo].[AlmacenesXLineaProducto] set activo  = 0 where idAlmacen in (1,4)

update 
[dbo].[AlmacenesXLineaProducto] set activo  = 1 where idAlmacen in (1,4)
and idLineaProducto in (26,9,10,5,7,6,2,11,21,23,1,4)


update 
[dbo].[AlmacenesXLineaProducto] set activo  = 0 where idAlmacen in (2,3)

update 
[dbo].[AlmacenesXLineaProducto] set activo  = 1 where idAlmacen in (2,3)
and idLineaProducto in (20,	12,	19,	22,	25)

-- select * from LineaProducto  where activo = 1 order by descripcion asc