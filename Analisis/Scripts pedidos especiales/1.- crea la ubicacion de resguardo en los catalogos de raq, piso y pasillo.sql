/****** Script for SelectTopNRows command from SSMS  ******/
select * from CatEstatusPedidoEspecialDetalle
SELECT *  FROM [DB_A57E86_lluviadesarrollo].[dbo].[PedidosEspecialesDetalle]

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

