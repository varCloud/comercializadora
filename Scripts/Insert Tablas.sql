delete from  CatRoles
DBCC CHECKIDENT ('[CatRoles]', RESEED, 0);
insert into CatRoles (descripcion) values ('Administrador')
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

-------------------------------------------------------

delete from  [dbo].[CatTipoAlmacen]
DBCC CHECKIDENT ('[CatTipoAlmacen]', RESEED, 0);
go
insert into  [dbo].[CatTipoAlmacen] (descripcion) values ('Almacen General')
insert into  [dbo].[CatTipoAlmacen] (descripcion) values ('Sub Almacen')
insert into  [dbo].[CatTipoAlmacen] (descripcion) values ('Punto Venta')

--------------------------------------------------------------------------

delete from  [dbo].CatPasillo
DBCC CHECKIDENT ('CatPasillo', RESEED, 0);
go
insert into CatPasillo (descripcion) 
sELECT Char(number+65)  as pasillo  FROM master.dbo.spt_values WHERE name IS NULL AND   number < 26
select * from CatPasillo

------------------------------------------------------------------------------

delete from  [dbo].CatRaq
DBCC CHECKIDENT ('CatRaq', RESEED, 0);
insert into CatRaq (descripcion) SELECT DISTINCT n ='Raq: ' +cast(number as varchar) FROM master..[spt_values] WHERE number BETWEEN 1 AND 9

select * from CatRaq

-----------------------------------------------------------------------------

------------------------------------------------------------------------------

delete from  [dbo].CatPiso
DBCC CHECKIDENT ('CatPiso', RESEED, 0);
insert into CatPiso (descripcion) SELECT DISTINCT n ='Piso del Raq: ' +cast(number as varchar) FROM master..[spt_values] WHERE number BETWEEN 1 AND 9

select * from CatPiso

-----------------------------------------------------------------------------
if exists (select 1 from CatSucursales)
begin
	delete from  [dbo].CatSucursales
	DBCC CHECKIDENT ('CatSucursales', RESEED, 0);
end

insert into CatSucursales (descripcion) values('sucursal principal')
select * from CatSucursales
-----------------------------------------------------------------------------
if exists (select 1 from CatStatusVenta)
begin
	delete from  dbo.CatStatusVenta
	--DBCC CHECKIDENT ('CatStatusVenta', RESEED, 0);
end

insert into CatStatusVenta (idStatusVenta,descripcion) values(1,'Activa')
insert into CatStatusVenta (idStatusVenta,descripcion) values(2,'Cancelada')
select * from CatStatusVenta
-----------------------------------------------------------------------------
if exists (select 1 from CatTipoCliente)
begin
	delete from  dbo.CatTipoCliente
	DBCC CHECKIDENT ('CatTipoCliente', RESEED, 0);
end

insert into CatTipoCliente (descripcion,descuento,activo) values ('General', 0.0,1)
insert into CatTipoCliente (descripcion,descuento,activo) values ('A', 2.0,1)
insert into CatTipoCliente (descripcion,descuento,activo) values ('B', 5.0,1)
insert into CatTipoCliente (descripcion,descuento,activo) values ('VIP', 10.0,1)
select * from CatTipoCliente
-----------------------------------------------------------------------------

