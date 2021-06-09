/*
LIQUIDOS
20.	Líquidos
12.	MPL
19.	Envases
22.	Corporal
25.	Mascotas

JARCIERÍA
26.	Bastón
9.	Bisutería
10.	Bolsas
5.	Escobas
7.	Hogar
6.	Industrial
2.	Jarciería
11.	Juguetería
21.	Matra
23.	Plásticos
1.	Químicos 
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