/****** Script for SelectTopNRows command from SSMS  ******/
select * from CatEstatusPedidoEspecialDetalle
SELECT * FROM [PedidosEspecialesDetalle]
select * from [dbo].[PedidosEspecialesMovimientosDeMercancia]

select * from [dbo].[CatPasillo]
select * from [dbo].[CatRaq]
select * from [dbo].[CatPiso]

select * from Ubicacion where idPasillo = 1000 and idRaq =1000 and idPiso =1000


DBCC CHECKIDENT ('CatPiso', RESEED, 999)
insert into [CatPiso] (descripcion) values ('Resguardo')
declare @indice int = 0
select @indice = max(idPiso) from [CatPiso] where idPiso < 1000
DBCC CHECKIDENT ('CatPiso', RESEED, @indice)

DBCC CHECKIDENT ('CatRaq', RESEED, 999)
insert into CatRaq (descripcion) values ('Resguardo')
select @indice = max(idRaq) from CatRaq where idRaq < 1000
DBCC CHECKIDENT ('CatRaq', RESEED, @indice)

DBCC CHECKIDENT ('CatPasillo', RESEED, 999)
insert into CatPasillo (descripcion) values ('Resguardo')
select @indice = max(idPasillo) from CatPasillo where idPasillo < 1000
DBCC CHECKIDENT ('CatPasillo', RESEED, @indice)


-- ubicacion para regresar productos de un pedido especial

DBCC CHECKIDENT ('CatPiso', RESEED, 1000)
insert into [CatPiso] (descripcion) values ('Devolución Pedido Especial')
select @indice = max(idPiso) from [CatPiso] where idPiso < 1000
DBCC CHECKIDENT ('CatPiso', RESEED, @indice)

DBCC CHECKIDENT ('CatRaq', RESEED, 1000)
insert into CatRaq (descripcion) values ('Devolución Pedido Especial')
select @indice = max(idRaq) from CatRaq where idRaq < 1000
DBCC CHECKIDENT ('CatRaq', RESEED, @indice)

DBCC CHECKIDENT ('CatPasillo', RESEED, 1000)
insert into CatPasillo (descripcion) values ('Devolución Pedido Especial')
select @indice = max(idPasillo) from CatPasillo where idPasillo < 1000
DBCC CHECKIDENT ('CatPasillo', RESEED, @indice)

select * from Ubicacion where idPasillo = 1001 and idRaq =1001 and idPiso =1001


-- se inserta forma de pago a credito para pedidos especiales
insert into [FactCatFormaPago] (id, formaPago, descripcion) values (100, '100','Crédito')

/*
select * from FactCatFormaPago where id = 100
select * from [dbo].[CatPasillo] where idPasillo = 1001
select * from [dbo].[CatRaq] where idRaq = 1001
select * from [dbo].[CatPiso] where idPiso = 1001
*/

if not exists ( select 1 from Ubicacion where idPasillo = 1001 and idRaq = 1001 and idPiso = 1001 )
begin
	insert into Ubicacion (idAlmacen, idPasillo,idRaq,idPiso) 
	select idAlmacen, 1001,1001,1001 from Almacenes 
end

select * from Ubicacion where idPasillo = 1001
