

delete from PedidosInternosDetalle
DBCC CHECKIDENT ('PedidosInternosDetalle', RESEED, 0) 
delete from InventarioDetalleLog
DBCC CHECKIDENT ('InventarioDetalleLog', RESEED, 0) 
delete from InventarioDetalle
DBCC CHECKIDENT ('InventarioDetalle', RESEED, 0) 
delete from InventarioGeneral
DBCC CHECKIDENT ('InventarioGeneral', RESEED, 0) 
delete from ProductosPorPrecio
DBCC CHECKIDENT ('ProductosPorPrecio', RESEED, 0) 
delete from VentasDetalle
DBCC CHECKIDENT ('VentasDetalle', RESEED, 0) 
delete from InventarioGeneralLog
DBCC CHECKIDENT ('InventarioGeneralLog', RESEED, 0) 
delete from ComprasDetalle
DBCC CHECKIDENT ('ComprasDetalle', RESEED, 0) 
delete from LimitesInventario
DBCC CHECKIDENT ('LimitesInventario', RESEED, 0) 
delete from Productos
DBCC CHECKIDENT ('Productos', RESEED, 0) 

select * from [Productos] order by descripcion asc

select * from [dbo].[productos_abril_2021] where ClaveProductoServicio is null or ClaveProductoServicio = ''

select * from [dbo].[productos_abril_2021] where ClaveProductoServicio is not null

select * from CatUnidadCompra

select * from CatUnidadMedida

select  * from LineaProducto order by descripcion asc



select * from Productos


select 
CAST(ClaveProductoServicio as varchar),
ClaveProductoServicio
from [dbo].[productos_abril_2021] 




select * from [dbo].[productos_dic] A where a.Linea = 'PLASTICO'

select * from [dbo].[productos_dic] A where a.Linea = 'BOLSA'


select  a.Linea from [dbo].[productos_abril_2021] A
left join [LineaProducto] b on UPPER(replace(A.Linea,' ','')) = replace(replace(b.descripcion,'LINEA' , ''),' ','')
where b.descripcion is null group by A.Linea

select  a.Linea from [dbo].[productos_dic] A
 join [LineaProducto] b on UPPER(replace(A.Linea,' ','')) = replace(replace(b.descripcion,'LINEA' , ''),' ','')


  
select replace(A.Linea,' ','')  from [dbo].[productos_dic] A
select  replace(replace(b.descripcion,'LINEA' , ''),' ','')  from [LineaProducto] b 

select  * from [productos_dic] A
left join [dbo].[FactCatClaveProdServicio] b ON A.ClaveProductoServicio =B.claveProdServ

select * from [productos_dic] A join [dbo].[FactCatClaveProdServicio] B ON replace(A.ClaveProductoServicio,' ','') = replace(B.claveProdServ,' ','')

insert into [Productos] 
(descripcion
,idUnidadMedida
,idLineaProducto
,cantidadUnidadMedida
,codigoBarras
,fechaAlta
,activo
,articulo
,claveProdServ
,precioIndividual
,precioMenudeo
,ultimoCostoCompra
,porcUtilidadIndividual
,porcUtilidadMayoreo
,idUnidadCompra
,cantidadUnidadCompra)

select 
a.DESCRIPCION
,b.idUnidadMedida 
,l.idLineaProducto
,1
,isnull(a.CodigoDeBarras,'')
,getdate()
,1
,isnull(a.CodigoDeBarras,'')
,isnull(a.ClaveProductoServicio,'')--replace(isnull(CAST(a.ClaveProductoServicio AS VARCHAR(100)),''),' ','')
,cast(a.PrecioMenudeo as decimal (18,2))
,cast(a.PrecioMayoreo as decimal(18,2))
,cast(a.CostoUltimo as decimal(18,2))
,cast(a.UtilidadMenudeo as decimal(18,2))
,cast(a.UtilidadMayoreo as decimal(18,2))
,UC.idUnidadCompra
,A.[CANTIDAD UNITARIA/ UNIDAD DE COMPRA]
from [dbo].[productos_abril_2021] A
join CatUnidadMedida b on A.UnidadDeMedida = b.descripcion
join [LineaProducto] L on UPPER(replace(A.Linea,' ','')) = replace(replace(L.descripcion,'LINEA' , ''),' ','')
join CatUnidadCompra UC on UPPER(replace(UC.descripcion,' ','')) = UPPER(replace(replace(A.UnidadCompra,' ' , ''),' ',''))

select * from Usuarios
select * from Almacenes
select * from LimitesInventario
select * from CatUnidadMedida




insert into LimitesInventario
(
minimo
,maximo
,idProducto
,idAlmacen
,idUsuario
,fechaAlta
,fechaActualizacion)
SELECT 
 cast(RAND()*(180-20)+20 as int)
,cast(RAND()*(350-180)+180 as int)
,P.idProducto
,1 as idAlmacen
,2 as idUsuario
,getdate()
,getdate()
from Productos P


SELECT 
 cast(RAND()*(180-20)+20 as int)
,cast(RAND()*(350-180)+180 as int)
,P.idProducto
,2 as idAlmacen
,2 as idUsuario
,getdate()
,getdate()
from Productos P join Productos a on a.idProducto = p.idProducto
