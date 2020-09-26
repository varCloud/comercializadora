if exists (select * from sysobjects where name like 'LimitesInventario' and xtype = 'u')
	drop table LimitesInventario

CREATE TABLE dbo.LimitesInventario
(
    idLimiteInventario int identity(1,1) primary key,
	minimo int,
	maximo int,
    idProducto int foreign key  references Productos(idProducto),
	idAlmacen int  foreign key references Almacenes(idAlmacen),
	idUsuario int,
	fechaAlta datetime,
	fechaActualizacion datetime	
);

go


if exists (select * from sysobjects where name like 'CatEstatusLimiteInventario' and xtype = 'u')
	drop table CatEstatusLimiteInventario

CREATE TABLE dbo.CatEstatusLimiteInventario
(
    idEstatusLimiteInventario int identity(1,1) primary key,
	descripcion varchar(500),
);

go

insert into CatEstatusLimiteInventario (descripcion) values ('Invietario dentro de sus limites ')  
insert into CatEstatusLimiteInventario (descripcion) values('Cantidad superior por el maximo permitido')
insert into CatEstatusLimiteInventario (descripcion) values('Cantidad por debajo del minimo permitido')

if not exists (select * from information_schema.columns where table_name like 'PedidosInternos' and column_name like 'idVenta')
   alter table PedidosInternos add idVenta int null


--INSERT INTO LimitesInventario(minimo,maximo,idProducto,idAlmacen,idUsuario,fechaAlta,fechaActualizacion)
--SELECT (idProducto+idAlmacen),(idProducto+idAlmacen)+10,idProducto,idAlmacen,4,getdate(),getdate() 
--FROM PRODUCTOS p
--cross join Almacenes a

