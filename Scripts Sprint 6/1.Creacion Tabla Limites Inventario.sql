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


--INSERT INTO LimitesInventario(minimo,maximo,idProducto,idAlmacen,idUsuario,fechaAlta,fechaActualizacion)
--SELECT (idProducto+idAlmacen),(idProducto+idAlmacen)+10,idProducto,idAlmacen,4,getdate(),getdate() 
--FROM PRODUCTOS p
--cross join Almacenes a

