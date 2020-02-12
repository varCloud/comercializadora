USE [DB_A552FA_comercializadora]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VentasDetalle_Ventas]') AND parent_object_id = OBJECT_ID(N'[dbo].[VentasDetalle]'))
ALTER TABLE [dbo].[VentasDetalle] DROP CONSTRAINT [FK_VentasDetalle_Ventas]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VentasDetalle_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[VentasDetalle]'))
ALTER TABLE [dbo].[VentasDetalle] DROP CONSTRAINT [FK_VentasDetalle_Productos]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VentasDetalle_CatPuntoVenta]') AND parent_object_id = OBJECT_ID(N'[dbo].[VentasDetalle]'))
ALTER TABLE [dbo].[VentasDetalle] DROP CONSTRAINT [FK_VentasDetalle_CatPuntoVenta]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ventas_Clientes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ventas]'))
ALTER TABLE [dbo].[Ventas] DROP CONSTRAINT [FK_Ventas_Clientes]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Usuarios_CatRoles]') AND parent_object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
ALTER TABLE [dbo].[Usuarios] DROP CONSTRAINT [FK_Usuarios_CatRoles]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Usuarios_CatPuntoVenta]') AND parent_object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
ALTER TABLE [dbo].[Usuarios] DROP CONSTRAINT [FK_Usuarios_CatPuntoVenta]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_Ubicacion1]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion] DROP CONSTRAINT [FK_Ubicacion_Ubicacion1]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_Ubicacion]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion] DROP CONSTRAINT [FK_Ubicacion_Ubicacion]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_CatRaq]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion] DROP CONSTRAINT [FK_Ubicacion_CatRaq]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_CatPiso]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion] DROP CONSTRAINT [FK_Ubicacion_CatPiso]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_Usuarios]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia] DROP CONSTRAINT [FK_TransferenciaMercancia_Usuarios]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia] DROP CONSTRAINT [FK_TransferenciaMercancia_Productos]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_CatTipoTransferenciaMercancia]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia] DROP CONSTRAINT [FK_TransferenciaMercancia_CatTipoTransferenciaMercancia]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_CatAlmacen1]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia] DROP CONSTRAINT [FK_TransferenciaMercancia_CatAlmacen1]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_CatAlmacen]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia] DROP CONSTRAINT [FK_TransferenciaMercancia_CatAlmacen]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductosPorPrecio_CatTipoPrecio]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductosPorPrecio]'))
ALTER TABLE [dbo].[ProductosPorPrecio] DROP CONSTRAINT [FK_ProductosPorPrecio_CatTipoPrecio]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Productos_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[Productos]'))
ALTER TABLE [dbo].[Productos] DROP CONSTRAINT [FK_Productos_Productos]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Productos_CatLineaProducto]') AND parent_object_id = OBJECT_ID(N'[dbo].[Productos]'))
ALTER TABLE [dbo].[Productos] DROP CONSTRAINT [FK_Productos_CatLineaProducto]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioPuntoVenta_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioPuntoVenta]'))
ALTER TABLE [dbo].[InventarioPuntoVenta] DROP CONSTRAINT [FK_InventarioPuntoVenta_Productos]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioPuntoVenta_InventarioPuntoVenta]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioPuntoVenta]'))
ALTER TABLE [dbo].[InventarioPuntoVenta] DROP CONSTRAINT [FK_InventarioPuntoVenta_InventarioPuntoVenta]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioGeneral_InventarioGeneral]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioGeneral]'))
ALTER TABLE [dbo].[InventarioGeneral] DROP CONSTRAINT [FK_InventarioGeneral_InventarioGeneral]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioDetalle_Ubicacion1]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioDetalle]'))
ALTER TABLE [dbo].[InventarioDetalle] DROP CONSTRAINT [FK_InventarioDetalle_Ubicacion1]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioDetalle_Ubicacion]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioDetalle]'))
ALTER TABLE [dbo].[InventarioDetalle] DROP CONSTRAINT [FK_InventarioDetalle_Ubicacion]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compras_Compras]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compras]'))
ALTER TABLE [dbo].[Compras] DROP CONSTRAINT [FK_Compras_Compras]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Clientes_CatTipoCliente]') AND parent_object_id = OBJECT_ID(N'[dbo].[Clientes]'))
ALTER TABLE [dbo].[Clientes] DROP CONSTRAINT [FK_Clientes_CatTipoCliente]
GO
/****** Object:  Table [dbo].[VentasDetalle]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VentasDetalle]') AND type in (N'U'))
DROP TABLE [dbo].[VentasDetalle]
GO
/****** Object:  Table [dbo].[Ventas]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ventas]') AND type in (N'U'))
DROP TABLE [dbo].[Ventas]
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type in (N'U'))
DROP TABLE [dbo].[Usuarios]
GO
/****** Object:  Table [dbo].[Ubicacion]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ubicacion]') AND type in (N'U'))
DROP TABLE [dbo].[Ubicacion]
GO
/****** Object:  Table [dbo].[TransferenciaMercancia]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]') AND type in (N'U'))
DROP TABLE [dbo].[TransferenciaMercancia]
GO
/****** Object:  Table [dbo].[Proveedores]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Proveedores]') AND type in (N'U'))
DROP TABLE [dbo].[Proveedores]
GO
/****** Object:  Table [dbo].[ProductosPorPrecio]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductosPorPrecio]') AND type in (N'U'))
DROP TABLE [dbo].[ProductosPorPrecio]
GO
/****** Object:  Table [dbo].[Productos]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Productos]') AND type in (N'U'))
DROP TABLE [dbo].[Productos]
GO
/****** Object:  Table [dbo].[InventarioPuntoVenta]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventarioPuntoVenta]') AND type in (N'U'))
DROP TABLE [dbo].[InventarioPuntoVenta]
GO
/****** Object:  Table [dbo].[InventarioGeneral]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventarioGeneral]') AND type in (N'U'))
DROP TABLE [dbo].[InventarioGeneral]
GO
/****** Object:  Table [dbo].[InventarioDetalle]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventarioDetalle]') AND type in (N'U'))
DROP TABLE [dbo].[InventarioDetalle]
GO
/****** Object:  Table [dbo].[Compras]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Compras]') AND type in (N'U'))
DROP TABLE [dbo].[Compras]
GO
/****** Object:  Table [dbo].[Clientes]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clientes]') AND type in (N'U'))
DROP TABLE [dbo].[Clientes]
GO
/****** Object:  Table [dbo].[CatUnidadMedida]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatUnidadMedida]') AND type in (N'U'))
DROP TABLE [dbo].[CatUnidadMedida]
GO
/****** Object:  Table [dbo].[CatTipoTransferenciaMercancia]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatTipoTransferenciaMercancia]') AND type in (N'U'))
DROP TABLE [dbo].[CatTipoTransferenciaMercancia]
GO
/****** Object:  Table [dbo].[CatTipoPrecio]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatTipoPrecio]') AND type in (N'U'))
DROP TABLE [dbo].[CatTipoPrecio]
GO
/****** Object:  Table [dbo].[CatTipoCliente]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatTipoCliente]') AND type in (N'U'))
DROP TABLE [dbo].[CatTipoCliente]
GO
/****** Object:  Table [dbo].[CatRoles]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatRoles]') AND type in (N'U'))
DROP TABLE [dbo].[CatRoles]
GO
/****** Object:  Table [dbo].[CatRaq]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatRaq]') AND type in (N'U'))
DROP TABLE [dbo].[CatRaq]
GO
/****** Object:  Table [dbo].[CatPuntoVenta]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatPuntoVenta]') AND type in (N'U'))
DROP TABLE [dbo].[CatPuntoVenta]
GO
/****** Object:  Table [dbo].[CatPiso]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatPiso]') AND type in (N'U'))
DROP TABLE [dbo].[CatPiso]
GO
/****** Object:  Table [dbo].[CatPasillo]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatPasillo]') AND type in (N'U'))
DROP TABLE [dbo].[CatPasillo]
GO
/****** Object:  Table [dbo].[CatLineaProducto]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatLineaProducto]') AND type in (N'U'))
DROP TABLE [dbo].[CatLineaProducto]
GO
/****** Object:  Table [dbo].[CatAlmacen]    Script Date: 12/02/2020 12:25:52 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatAlmacen]') AND type in (N'U'))
DROP TABLE [dbo].[CatAlmacen]
GO
/****** Object:  Table [dbo].[CatAlmacen]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatAlmacen]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatAlmacen](
	[idAlmacen] [int] NOT NULL,
	[descripcion] [varchar](50) NULL,
 CONSTRAINT [PK_CatAlmacen] PRIMARY KEY CLUSTERED 
(
	[idAlmacen] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CatLineaProducto]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatLineaProducto]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatLineaProducto](
	[idLineaProducto] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CatLineaProducto] PRIMARY KEY CLUSTERED 
(
	[idLineaProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CatPasillo]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatPasillo]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatPasillo](
	[idPasillo] [int] NOT NULL,
	[descripcion] [varchar](50) NULL,
 CONSTRAINT [PK_CatPasillo] PRIMARY KEY CLUSTERED 
(
	[idPasillo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CatPiso]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatPiso]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatPiso](
	[idPiso] [int] NOT NULL,
	[descripcion] [varchar](50) NULL,
 CONSTRAINT [PK_CatPiso] PRIMARY KEY CLUSTERED 
(
	[idPiso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CatPuntoVenta]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatPuntoVenta]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatPuntoVenta](
	[idPuntoVenta] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CatPuntoVenta] PRIMARY KEY CLUSTERED 
(
	[idPuntoVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CatRaq]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatRaq]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatRaq](
	[idRaq] [int] NOT NULL,
	[descripcion] [varchar](50) NULL,
 CONSTRAINT [PK_CatRaq] PRIMARY KEY CLUSTERED 
(
	[idRaq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CatRoles]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatRoles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatRoles](
	[idRol] [int] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CatRoles] PRIMARY KEY CLUSTERED 
(
	[idRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CatTipoCliente]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatTipoCliente]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatTipoCliente](
	[idTipoCliente] [int] NOT NULL,
	[descripcion] [varchar](50) NULL,
 CONSTRAINT [PK_CatTipoCliente] PRIMARY KEY CLUSTERED 
(
	[idTipoCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CatTipoPrecio]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatTipoPrecio]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatTipoPrecio](
	[idTipoPrecio] [int] NOT NULL,
	[descripcion] [varchar](50) NULL,
 CONSTRAINT [PK_CatTipoPrecio] PRIMARY KEY CLUSTERED 
(
	[idTipoPrecio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CatTipoTransferenciaMercancia]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatTipoTransferenciaMercancia]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatTipoTransferenciaMercancia](
	[idTipoTransferenciaMercancia] [int] NOT NULL,
	[descripcion] [varchar](100) NULL,
 CONSTRAINT [PK_CatTipoTransferenciaMercancia] PRIMARY KEY CLUSTERED 
(
	[idTipoTransferenciaMercancia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CatUnidadMedida]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatUnidadMedida]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatUnidadMedida](
	[idUnidadMedida] [int] NOT NULL,
	[descripcion] [varchar](50) NULL,
 CONSTRAINT [PK_CatUnidadMedida] PRIMARY KEY CLUSTERED 
(
	[idUnidadMedida] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Clientes]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clientes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Clientes](
	[idCliente] [int] IDENTITY(1,1) NOT NULL,
	[nombres] [varchar](50) NULL,
	[apellidoPaterno] [varchar](50) NULL,
	[apellidoMaterno] [varchar](50) NULL,
	[telefono] [varchar](50) NULL,
	[correo] [varchar](50) NULL,
	[rfc] [varchar](50) NULL,
	[calle] [varchar](50) NULL,
	[numeroExterior] [varchar](50) NULL,
	[colonia] [varchar](50) NULL,
	[municipio] [varchar](50) NULL,
	[cp] [varchar](50) NULL,
	[estado] [varchar](50) NULL,
	[fechaAlta] [varchar](50) NULL,
	[activo] [varchar](50) NULL,
	[idTipoCliente] [int] NOT NULL,
 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED 
(
	[idCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Compras]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Compras]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Compras](
	[idCompra] [int] NOT NULL,
	[idProveedor] [int] NOT NULL,
	[cantidad] [float] NULL,
	[fechaAlta] [datetime] NULL,
 CONSTRAINT [PK_Compras] PRIMARY KEY CLUSTERED 
(
	[idCompra] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[InventarioDetalle]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventarioDetalle]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InventarioDetalle](
	[idInventarioDetalle] [int] IDENTITY(1,1) NOT NULL,
	[idProducto] [int] NULL,
	[cantidad] [float] NULL,
	[fechaAlta] [datetime] NULL,
	[idUbicacion] [int] NOT NULL,
 CONSTRAINT [PK_InventarioAlmacen] PRIMARY KEY CLUSTERED 
(
	[idInventarioDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[InventarioGeneral]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventarioGeneral]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InventarioGeneral](
	[contador] [int] NOT NULL,
	[idProducto] [int] NOT NULL,
	[cantidad] [float] NULL,
	[FechaAlta] [datetime] NULL,
 CONSTRAINT [PK_InventarioGeneral] PRIMARY KEY CLUSTERED 
(
	[idProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[InventarioPuntoVenta]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventarioPuntoVenta]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InventarioPuntoVenta](
	[idInventarioPuntoVenta] [int] IDENTITY(1,1) NOT NULL,
	[idPuntoVenta] [int] NOT NULL,
	[idProducto] [int] NOT NULL,
	[cantidad] [float] NOT NULL,
	[fechaAlta] [int] NOT NULL,
 CONSTRAINT [PK_InventarioPuntoVenta] PRIMARY KEY CLUSTERED 
(
	[idInventarioPuntoVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Productos]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Productos]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Productos](
	[idProducto] [int] NOT NULL,
	[descripcion] [nchar](10) NOT NULL,
	[idUnidadMedida] [int] NOT NULL,
	[idLineaProducto] [int] NOT NULL,
	[cantidadUnidadMedida] [float] NOT NULL,
	[codigoBarras] [varchar](max) NULL,
	[fechaAlta] [nchar](10) NOT NULL,
	[activo] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Productos] PRIMARY KEY CLUSTERED 
(
	[idProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductosPorPrecio]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductosPorPrecio]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ProductosPorPrecio](
	[idProducto] [int] IDENTITY(1,1) NOT NULL,
	[min] [decimal](18, 0) NULL,
	[max] [decimal](18, 0) NULL,
	[costo] [decimal](18, 0) NULL,
	[idTipoPrecio] [int] NULL,
 CONSTRAINT [PK_ProductosPorPrecio] PRIMARY KEY CLUSTERED 
(
	[idProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Proveedores]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Proveedores]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Proveedores](
	[idProveedor] [int] NOT NULL,
	[nombre] [varchar](50) NULL,
	[descripcion] [varchar](50) NULL,
	[telefono] [varchar](10) NULL,
	[direccion] [varchar](250) NULL,
 CONSTRAINT [PK_Proveedores] PRIMARY KEY CLUSTERED 
(
	[idProveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransferenciaMercancia]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TransferenciaMercancia](
	[idTransferenciaMercancia] [int] NOT NULL,
	[idUsuario] [int] NOT NULL,
	[idAlmacenOrigen] [int] NOT NULL,
	[idAlmacenDestino] [int] NOT NULL,
	[idProducto] [int] NOT NULL,
	[cantidad] [float] NOT NULL,
	[fechaAlta] [datetime] NOT NULL,
	[idTipoTransferenciaMercancia] [int] NOT NULL,
 CONSTRAINT [PK_TransferenciaMercancia] PRIMARY KEY CLUSTERED 
(
	[idTransferenciaMercancia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Ubicacion]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ubicacion]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Ubicacion](
	[idUbicacion] [int] IDENTITY(1,1) NOT NULL,
	[idAlmacen] [int] NULL,
	[idPasillo] [int] NULL,
	[idRaq] [int] NULL,
	[idPiso] [int] NULL,
 CONSTRAINT [PK_Ubicacion] PRIMARY KEY CLUSTERED 
(
	[idUbicacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Usuarios](
	[idUsuario] [int] NOT NULL,
	[idRol] [int] NOT NULL,
	[idPuntoVenta] [int] NOT NULL,
	[nombre] [varchar](50) NULL,
	[telefono] [varchar](10) NULL,
	[contrasena] [varchar](50) NULL,
 CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED 
(
	[idUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Ventas]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ventas]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Ventas](
	[idVenta] [int] NOT NULL,
	[idCliente] [int] NOT NULL,
	[cantidad] [float] NULL,
	[fechaAlta] [datetime] NULL,
	[montoTotal] [decimal](18, 0) NULL,
 CONSTRAINT [PK_Ventas] PRIMARY KEY CLUSTERED 
(
	[idVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[VentasDetalle]    Script Date: 12/02/2020 12:25:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VentasDetalle]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[VentasDetalle](
	[idVentaDetalle] [int] NOT NULL,
	[idVenta] [int] NOT NULL,
	[idProducto] [int] NOT NULL,
	[idPuntoVenta] [int] NOT NULL,
	[idUsuario] [int] NOT NULL,
	[cantidad] [float] NULL,
	[monto] [decimal](18, 0) NULL,
	[cantidadActualInvGeneral] [float] NULL,
	[cantidadAnteriorInvGeneral] [float] NULL,
 CONSTRAINT [PK_VentasDetalle] PRIMARY KEY CLUSTERED 
(
	[idVentaDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Clientes_CatTipoCliente]') AND parent_object_id = OBJECT_ID(N'[dbo].[Clientes]'))
ALTER TABLE [dbo].[Clientes]  WITH CHECK ADD  CONSTRAINT [FK_Clientes_CatTipoCliente] FOREIGN KEY([idTipoCliente])
REFERENCES [dbo].[CatTipoCliente] ([idTipoCliente])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Clientes_CatTipoCliente]') AND parent_object_id = OBJECT_ID(N'[dbo].[Clientes]'))
ALTER TABLE [dbo].[Clientes] CHECK CONSTRAINT [FK_Clientes_CatTipoCliente]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compras_Compras]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compras]'))
ALTER TABLE [dbo].[Compras]  WITH CHECK ADD  CONSTRAINT [FK_Compras_Compras] FOREIGN KEY([idProveedor])
REFERENCES [dbo].[Proveedores] ([idProveedor])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Compras_Compras]') AND parent_object_id = OBJECT_ID(N'[dbo].[Compras]'))
ALTER TABLE [dbo].[Compras] CHECK CONSTRAINT [FK_Compras_Compras]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioDetalle_Ubicacion]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioDetalle]'))
ALTER TABLE [dbo].[InventarioDetalle]  WITH CHECK ADD  CONSTRAINT [FK_InventarioDetalle_Ubicacion] FOREIGN KEY([idProducto])
REFERENCES [dbo].[Productos] ([idProducto])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioDetalle_Ubicacion]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioDetalle]'))
ALTER TABLE [dbo].[InventarioDetalle] CHECK CONSTRAINT [FK_InventarioDetalle_Ubicacion]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioDetalle_Ubicacion1]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioDetalle]'))
ALTER TABLE [dbo].[InventarioDetalle]  WITH CHECK ADD  CONSTRAINT [FK_InventarioDetalle_Ubicacion1] FOREIGN KEY([idUbicacion])
REFERENCES [dbo].[Ubicacion] ([idUbicacion])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioDetalle_Ubicacion1]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioDetalle]'))
ALTER TABLE [dbo].[InventarioDetalle] CHECK CONSTRAINT [FK_InventarioDetalle_Ubicacion1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioGeneral_InventarioGeneral]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioGeneral]'))
ALTER TABLE [dbo].[InventarioGeneral]  WITH CHECK ADD  CONSTRAINT [FK_InventarioGeneral_InventarioGeneral] FOREIGN KEY([idProducto])
REFERENCES [dbo].[Productos] ([idProducto])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioGeneral_InventarioGeneral]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioGeneral]'))
ALTER TABLE [dbo].[InventarioGeneral] CHECK CONSTRAINT [FK_InventarioGeneral_InventarioGeneral]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioPuntoVenta_InventarioPuntoVenta]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioPuntoVenta]'))
ALTER TABLE [dbo].[InventarioPuntoVenta]  WITH CHECK ADD  CONSTRAINT [FK_InventarioPuntoVenta_InventarioPuntoVenta] FOREIGN KEY([idPuntoVenta])
REFERENCES [dbo].[CatPuntoVenta] ([idPuntoVenta])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioPuntoVenta_InventarioPuntoVenta]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioPuntoVenta]'))
ALTER TABLE [dbo].[InventarioPuntoVenta] CHECK CONSTRAINT [FK_InventarioPuntoVenta_InventarioPuntoVenta]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioPuntoVenta_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioPuntoVenta]'))
ALTER TABLE [dbo].[InventarioPuntoVenta]  WITH CHECK ADD  CONSTRAINT [FK_InventarioPuntoVenta_Productos] FOREIGN KEY([idProducto])
REFERENCES [dbo].[Productos] ([idProducto])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventarioPuntoVenta_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventarioPuntoVenta]'))
ALTER TABLE [dbo].[InventarioPuntoVenta] CHECK CONSTRAINT [FK_InventarioPuntoVenta_Productos]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Productos_CatLineaProducto]') AND parent_object_id = OBJECT_ID(N'[dbo].[Productos]'))
ALTER TABLE [dbo].[Productos]  WITH CHECK ADD  CONSTRAINT [FK_Productos_CatLineaProducto] FOREIGN KEY([idLineaProducto])
REFERENCES [dbo].[CatLineaProducto] ([idLineaProducto])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Productos_CatLineaProducto]') AND parent_object_id = OBJECT_ID(N'[dbo].[Productos]'))
ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [FK_Productos_CatLineaProducto]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Productos_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[Productos]'))
ALTER TABLE [dbo].[Productos]  WITH CHECK ADD  CONSTRAINT [FK_Productos_Productos] FOREIGN KEY([idUnidadMedida])
REFERENCES [dbo].[CatUnidadMedida] ([idUnidadMedida])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Productos_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[Productos]'))
ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [FK_Productos_Productos]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductosPorPrecio_CatTipoPrecio]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductosPorPrecio]'))
ALTER TABLE [dbo].[ProductosPorPrecio]  WITH CHECK ADD  CONSTRAINT [FK_ProductosPorPrecio_CatTipoPrecio] FOREIGN KEY([idTipoPrecio])
REFERENCES [dbo].[CatTipoPrecio] ([idTipoPrecio])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductosPorPrecio_CatTipoPrecio]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductosPorPrecio]'))
ALTER TABLE [dbo].[ProductosPorPrecio] CHECK CONSTRAINT [FK_ProductosPorPrecio_CatTipoPrecio]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_CatAlmacen]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia]  WITH CHECK ADD  CONSTRAINT [FK_TransferenciaMercancia_CatAlmacen] FOREIGN KEY([idAlmacenOrigen])
REFERENCES [dbo].[CatAlmacen] ([idAlmacen])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_CatAlmacen]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia] CHECK CONSTRAINT [FK_TransferenciaMercancia_CatAlmacen]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_CatAlmacen1]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia]  WITH CHECK ADD  CONSTRAINT [FK_TransferenciaMercancia_CatAlmacen1] FOREIGN KEY([idAlmacenDestino])
REFERENCES [dbo].[CatAlmacen] ([idAlmacen])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_CatAlmacen1]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia] CHECK CONSTRAINT [FK_TransferenciaMercancia_CatAlmacen1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_CatTipoTransferenciaMercancia]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia]  WITH CHECK ADD  CONSTRAINT [FK_TransferenciaMercancia_CatTipoTransferenciaMercancia] FOREIGN KEY([idTipoTransferenciaMercancia])
REFERENCES [dbo].[CatTipoTransferenciaMercancia] ([idTipoTransferenciaMercancia])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_CatTipoTransferenciaMercancia]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia] CHECK CONSTRAINT [FK_TransferenciaMercancia_CatTipoTransferenciaMercancia]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia]  WITH CHECK ADD  CONSTRAINT [FK_TransferenciaMercancia_Productos] FOREIGN KEY([idProducto])
REFERENCES [dbo].[Productos] ([idProducto])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia] CHECK CONSTRAINT [FK_TransferenciaMercancia_Productos]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_Usuarios]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia]  WITH CHECK ADD  CONSTRAINT [FK_TransferenciaMercancia_Usuarios] FOREIGN KEY([idUsuario])
REFERENCES [dbo].[Usuarios] ([idUsuario])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransferenciaMercancia_Usuarios]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransferenciaMercancia]'))
ALTER TABLE [dbo].[TransferenciaMercancia] CHECK CONSTRAINT [FK_TransferenciaMercancia_Usuarios]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_CatPiso]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion]  WITH CHECK ADD  CONSTRAINT [FK_Ubicacion_CatPiso] FOREIGN KEY([idPiso])
REFERENCES [dbo].[CatPiso] ([idPiso])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_CatPiso]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion] CHECK CONSTRAINT [FK_Ubicacion_CatPiso]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_CatRaq]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion]  WITH CHECK ADD  CONSTRAINT [FK_Ubicacion_CatRaq] FOREIGN KEY([idRaq])
REFERENCES [dbo].[CatRaq] ([idRaq])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_CatRaq]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion] CHECK CONSTRAINT [FK_Ubicacion_CatRaq]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_Ubicacion]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion]  WITH CHECK ADD  CONSTRAINT [FK_Ubicacion_Ubicacion] FOREIGN KEY([idAlmacen])
REFERENCES [dbo].[CatAlmacen] ([idAlmacen])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_Ubicacion]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion] CHECK CONSTRAINT [FK_Ubicacion_Ubicacion]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_Ubicacion1]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion]  WITH CHECK ADD  CONSTRAINT [FK_Ubicacion_Ubicacion1] FOREIGN KEY([idPasillo])
REFERENCES [dbo].[CatPasillo] ([idPasillo])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ubicacion_Ubicacion1]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ubicacion]'))
ALTER TABLE [dbo].[Ubicacion] CHECK CONSTRAINT [FK_Ubicacion_Ubicacion1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Usuarios_CatPuntoVenta]') AND parent_object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_Usuarios_CatPuntoVenta] FOREIGN KEY([idPuntoVenta])
REFERENCES [dbo].[CatPuntoVenta] ([idPuntoVenta])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Usuarios_CatPuntoVenta]') AND parent_object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_CatPuntoVenta]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Usuarios_CatRoles]') AND parent_object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_Usuarios_CatRoles] FOREIGN KEY([idRol])
REFERENCES [dbo].[CatRoles] ([idRol])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Usuarios_CatRoles]') AND parent_object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_CatRoles]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ventas_Clientes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ventas]'))
ALTER TABLE [dbo].[Ventas]  WITH CHECK ADD  CONSTRAINT [FK_Ventas_Clientes] FOREIGN KEY([idCliente])
REFERENCES [dbo].[Clientes] ([idCliente])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Ventas_Clientes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Ventas]'))
ALTER TABLE [dbo].[Ventas] CHECK CONSTRAINT [FK_Ventas_Clientes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VentasDetalle_CatPuntoVenta]') AND parent_object_id = OBJECT_ID(N'[dbo].[VentasDetalle]'))
ALTER TABLE [dbo].[VentasDetalle]  WITH CHECK ADD  CONSTRAINT [FK_VentasDetalle_CatPuntoVenta] FOREIGN KEY([idPuntoVenta])
REFERENCES [dbo].[CatPuntoVenta] ([idPuntoVenta])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VentasDetalle_CatPuntoVenta]') AND parent_object_id = OBJECT_ID(N'[dbo].[VentasDetalle]'))
ALTER TABLE [dbo].[VentasDetalle] CHECK CONSTRAINT [FK_VentasDetalle_CatPuntoVenta]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VentasDetalle_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[VentasDetalle]'))
ALTER TABLE [dbo].[VentasDetalle]  WITH CHECK ADD  CONSTRAINT [FK_VentasDetalle_Productos] FOREIGN KEY([idProducto])
REFERENCES [dbo].[Productos] ([idProducto])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VentasDetalle_Productos]') AND parent_object_id = OBJECT_ID(N'[dbo].[VentasDetalle]'))
ALTER TABLE [dbo].[VentasDetalle] CHECK CONSTRAINT [FK_VentasDetalle_Productos]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VentasDetalle_Ventas]') AND parent_object_id = OBJECT_ID(N'[dbo].[VentasDetalle]'))
ALTER TABLE [dbo].[VentasDetalle]  WITH CHECK ADD  CONSTRAINT [FK_VentasDetalle_Ventas] FOREIGN KEY([idVenta])
REFERENCES [dbo].[Ventas] ([idVenta])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VentasDetalle_Ventas]') AND parent_object_id = OBJECT_ID(N'[dbo].[VentasDetalle]'))
ALTER TABLE [dbo].[VentasDetalle] CHECK CONSTRAINT [FK_VentasDetalle_Ventas]
GO
