

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



select * from [dbo].[productos_dic] where ClaveProductoServicio is null or ClaveProductoServicio = ''

select * from [dbo].[productos_dic] where ClaveProductoServicio is not null




select * from [LineaProducto]

--update [dbo].[productos_dic]   set Linea='PLASTICOS' where Linea = 'PLASTICOS+B962:K962'
--update [dbo].[productos_dic]   set Linea='PLASTICOS' where Linea = 'PLASTICOS+B962:K961'
--update [dbo].[productos_dic]   set Linea='PLASTICOS' where Linea = 'PLASTICO'


select * from [dbo].[productos_dic] A where a.Linea = 'PLASTICOS+B962:K961'
select * from [dbo].[productos_dic] A where a.Linea = 'PLASTICO'

select * from [dbo].[productos_dic] A where a.Linea = 'BOLSA'


select  a.Linea from [dbo].[productos_dic] A
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
,porcUtilidadMayoreo)
select 
a.DESCRIPCION
,b.idUnidadMedida 
,l.idLineaProducto
,1
,isnull(a.CodigoDeBarras,'')
,getdate()
,1
,isnull(a.CodigoDeBarras,'')
,replace(isnull(CAST(a.ClaveProductoServicio AS VARCHAR),''),' ','')
,a.PrecioMenudeo
,a.PrecioMayoreo
,a.CostoUltimo
,a.UtilidadMenudeo
,a.UtilidadMayoreo
from [dbo].[productos_dic] A
join CatUnidadMedida b on A.UnidadDeMedida = b.descripcion
join [LineaProducto] L on UPPER(replace(A.Linea,' ','')) = replace(replace(L.descripcion,'LINEA' , ''),' ','')


select * from Usuarios
select * from Almacenes
select * from LimitesInventario




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
