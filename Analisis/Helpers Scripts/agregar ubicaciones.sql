


DBCC CHECKIDENT ('CatPasillo', RESEED, 1003)
DBCC CHECKIDENT ('CatRaq', RESEED, 1003)
DBCC CHECKIDENT ('CatPiso', RESEED, 1003)

insert into CatPasillo (descripcion) values ('en proceso de produccio�n agranel') 
insert into CatRaq (descripcion) values ('en proceso de produccio�n agranel') 
insert into CatPiso (descripcion) values ('en proceso de produccio�n agranel') 

DBCC CHECKIDENT ('CatPasillo', RESEED, 26)
DBCC CHECKIDENT ('CatRaq', RESEED, 25)
DBCC CHECKIDENT ('CatPiso', RESEED, 9)


select * from CatPasillo
select * from CatRaq
select * from CatPiso