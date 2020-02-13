delete from  CatRoles
DBCC CHECKIDENT ('[CatRoles]', RESEED, 0);
insert into CatRoles (descripcion) values ('Gerente')
insert into CatRoles (descripcion) values ('SubGerente')
insert into CatRoles (descripcion) values ('Encargo de Almacen')
insert into CatRoles (descripcion) values ('Cajero')
select * from CatRoles

-------------------------------------------------------

delete from  [CatLineaProducto]
DBCC CHECKIDENT ('[CatLineaProducto]', RESEED, 0);

insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea QUIMICOS')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea JARCIERIA')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea PLASTICOS PARA EL HOGAR')

insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea TRAPEADOR')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea ESCOBA')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea INDUSTRIAL')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea HOGAR')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea BAS')


insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea BISUTERIA')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea BOLSA')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea JUGUETERIA')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea MPL')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea BORIS')


insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea MATRAPEADOR')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea CRISTAL')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea GENERAL')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea PELTRE')
insert into  [dbo].[CatLineaProducto] (descripcion) values ('Linea CORPORAL')


select * from [dbo].[CatLineaProducto]


delete from  [dbo].[CatTipoAlmacen]
DBCC CHECKIDENT ('[CatTipoAlmacen]', RESEED, 0);
go
insert into  [dbo].[CatTipoAlmacen] (descripcion) values ('Almacen General')
insert into  [dbo].[CatTipoAlmacen] (descripcion) values ('Sub Almacen')
insert into  [dbo].[CatTipoAlmacen] (descripcion) values ('Punto Venta')


select * from  [CatTipoAlmacen]

select * from [dbo].[Ubicacion]