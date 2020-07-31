USE [DB_A57E86_lluviadesarrollo]
GO

--if not exists ( select Descripcion_Motivo from MOTIVO_REESTRUCTURA where Descripcion_Motivo like 'COVID COBRANZA' )
--begin
--	insert into MOTIVO_REESTRUCTURA (Descripcion_Motivo) values ('COVID COBRANZA')
--end


if not exists (select 1 from sys.columns where name = N'devoluciones' AND Object_ID = Object_ID(N'dbo.Ventas'))
begin
	alter table Ventas ADD  devoluciones int default (0);
end
GO

if not exists (select 1 from sys.columns where name = N'productosAgregados' AND Object_ID = Object_ID(N'dbo.Ventas'))
begin
	alter table Ventas ADD  productosAgregados int default (0);
end
GO

if not exists (select 1 from sys.columns where name = N'productosDevueltos' AND Object_ID = Object_ID(N'dbo.VentasDetalle'))
begin
	alter table VentasDetalle ADD  productosDevueltos int default (0);
end
GO



if not exists ( select 1 from CatTipoMovimientoInventario where descripcion like 'Devolucion' )
begin
	insert into CatTipoMovimientoInventario (descripcion, operacion) values ('Devolucion', -1)
end
go

if not exists ( select 1 from CatTipoMovimientoInventario where descripcion like 'Agregar Productos a la Venta' )
begin
	insert into CatTipoMovimientoInventario (descripcion, operacion) values ('Agregar Productos a la Venta', 1)
end
go


----------------------------------------------------------------------------------------------------------------------------------------------------------
--	[TBL_REESTRUCTURAS_UNIVERSO_VIGENTES_MARZO2020]
----------------------------------------------------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[modificacionesPermitidasVenta]') AND type in (N'U'))
	DROP TABLE [dbo].[modificacionesPermitidasVenta]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[modificacionesPermitidasVenta]') AND type in (N'U'))
BEGIN

create table 
	modificacionesPermitidasVenta
		(
			id				int identity(1,1),
			descripcion		varchar(255),
			cantidad		int
		)

END
GO
SET ANSI_PADDING OFF
GO
DENY  DELETE ON [dbo].[modificacionesPermitidasVenta] TO [public] AS [dbo]
GO
GRANT INSERT ON [dbo].[modificacionesPermitidasVenta] TO [public] WITH GRANT OPTION  AS [dbo]
GO
GRANT SELECT ON [dbo].[modificacionesPermitidasVenta] TO [public] WITH GRANT OPTION  AS [dbo]
GO
GRANT UPDATE ON [dbo].[modificacionesPermitidasVenta] TO [public] WITH GRANT OPTION  AS [dbo]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



insert into modificacionesPermitidasVenta (descripcion, cantidad) values ('Devoluciones a la Venta', 1)
insert into modificacionesPermitidasVenta (descripcion, cantidad) values ('Agregar Productos a la Venta', 1)

