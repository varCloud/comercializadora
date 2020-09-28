----------------------------------------------------------------------------------------------------------------------------------------------------------
--	Nuevas tablas
----------------------------------------------------------------------------------------------------------------------------------------------------------
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

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_idTipoPedidoInterno')
	alter table PedidosInternos  drop constraint FK_idTipoPedidoInterno;
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatTipoPedidoInterno]') AND type in (N'U'))
	DROP TABLE [dbo].[CatTipoPedidoInterno]
GO

CREATE TABLE [dbo].[CatTipoPedidoInterno](
	
	idTipoPedidoInterno		int identity(1,1) NOT NULL,
	descripcion				varchar(255)

 CONSTRAINT [PK_idTipoPedidoInterno] PRIMARY KEY CLUSTERED 
(
	[idTipoPedidoInterno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT INSERT ON [dbo].[CatTipoPedidoInterno] TO [public] WITH GRANT OPTION  AS [dbo]
GO
GRANT SELECT ON [dbo].[CatTipoPedidoInterno] TO [public] WITH GRANT OPTION  AS [dbo]
GO
GRANT UPDATE ON [dbo].[CatTipoPedidoInterno] TO [public] WITH GRANT OPTION  AS [dbo]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------
--	Alter tablas
----------------------------------------------------------------------------------------------------------------------------------------------------------
if not exists (select * from information_schema.columns where table_name like 'PedidosInternos' and column_name like 'idVenta')
   alter table PedidosInternos add idVenta int null


-- id del nuevo catalogo CatTipoPedidoInterno
if not exists (select 1 from sys.columns where name = N'idTipoPedidoInterno' AND Object_ID = Object_ID(N'dbo.PedidosInternos'))
begin
	ALTER TABLE PedidosInternos ADD idTipoPedidoInterno int DEFAULT  1;
end
GO

-- descripcion para identificar el pedido en la app
if not exists (select 1 from sys.columns where name = N'descripcion' AND Object_ID = Object_ID(N'dbo.PedidosInternos'))
begin
	ALTER TABLE PedidosInternos ADD descripcion  varchar(500) default null;
end
GO


----------------------------------------------------------------------------------------------------------------------------------------------------------
--	Update registros
----------------------------------------------------------------------------------------------------------------------------------------------------------
update PedidosInternos set idTipoPedidoInterno = 1 where idTipoPedidoInterno is null 

UPDATE LineaProducto SET descripcion='Linea ENVASE' where idLineaProducto=19


----------------------------------------------------------------------------------------------------------------------------------------------------------
--	Nuevos Registros
----------------------------------------------------------------------------------------------------------------------------------------------------------
insert into CatEstatusLimiteInventario (descripcion) values ('Invietario dentro de sus limites ')  
insert into CatEstatusLimiteInventario (descripcion) values('Cantidad superior por el maximo permitido')
insert into CatEstatusLimiteInventario (descripcion) values('Cantidad por debajo del minimo permitido')

go

if not exists ( select 1 from CatModulos where descripcion like 'Pedidos Especiales' )
begin
	insert into CatModulos(descripcion) values ('Pedidos Especiales')
end
go

declare @idModulo as int 
select @idModulo = idModulo from CatModulos where descripcion like 'Pedidos Especiales'


-- permiso de epdidos especiales para admin
if not exists (select 1 from PermisosRolPorModulo where idRol = 1 and idModulo = @idModulo)
begin
	insert into PermisosRolPorModulo(idRol,idModulo,tienePermiso) values (1, @idModulo, 1)
end
go

--valores iniciales
if not exists(select 1 from CatTipoPedidoInterno where descripcion like 'Pedido Interno')
insert into CatTipoPedidoInterno (descripcion) values ('Pedido Interno')

if not exists(select 1 from CatTipoPedidoInterno where descripcion like 'Pedido Especial')
insert into CatTipoPedidoInterno (descripcion) values ('Pedido Especial')
go

-- movimeintos de pedidos internos en InventarioDetalleLog
--if not exists ( select idTipoMovInventario from  CatTipoMovimientoInventario where descripcion like 'Envío de mercancia - Pedido Especial' )
--begin
--	insert into CatTipoMovimientoInventario (descripcion, operacion) values ('Envío de mercancia - Pedido Especial', -1)
--end


--if not exists ( select idTipoMovInventario from  CatTipoMovimientoInventario where descripcion like 'Recepción de mercancia - Pedido Especial' )
--begin
--	insert into CatTipoMovimientoInventario (descripcion, operacion) values ('Recepción de mercancia - Pedido Especial', 1)
--end

--go

IF NOT EXISTS(SELECT * FROM LineaProducto WHERE descripcion like 'Linea LIQUIDOS')
INSERT INTO LineaProducto(descripcion,activo) values('Linea LIQUIDOS',1)

IF NOT EXISTS (SELECT 1 FROM catestatusPedidoInterno WHERE descripcion like 'Pedido Vendido')
INSERT INTO catestatusPedidoInterno(descripcion) values ('Pedido Vendido')


-- foranea en pedidosInternos
ALTER TABLE [dbo].[PedidosInternos]  WITH CHECK ADD  CONSTRAINT [FK_idTipoPedidoInterno] FOREIGN KEY([idTipoPedidoInterno])
REFERENCES [dbo].[CatTipoPedidoInterno] ([idTipoPedidoInterno])
GO

