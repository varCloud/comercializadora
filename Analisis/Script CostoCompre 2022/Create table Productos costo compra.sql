--USE [DB_A57E86_lluviadesarrollo]
GO

ALTER TABLE [dbo].ProductosCostoCompra DROP CONSTRAINT [FK_ProductosCostoCompra_Productos]
GO

ALTER TABLE [dbo].ProductosCostoCompra DROP CONSTRAINT [FK_ProductosCostoCompra_CatLineaProducto]
GO

ALTER TABLE [dbo].ProductosCostoCompra DROP CONSTRAINT [DF__ProductosCostoCompra__clave__379037E3]
GO

/****** Object:  Table [dbo].[Productos]    Script Date: 02/02/2022 10:42:30 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductosCostoCompra]') AND type in (N'U'))
DROP TABLE [dbo].ProductosCostoCompra
GO

/****** Object:  Table [dbo].[Productos]    Script Date: 02/02/2022 10:42:30 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].ProductosCostoCompra(
    [idProductoCostoCompra] [bigint] IDENTITY(1,1) not null,
	[idProducto] [int] NOT NULL,
	[descripcion] [varchar](100) NULL,
	[idUnidadMedida] [int] NOT NULL,
	[idLineaProducto] [int] NOT NULL,
	[cantidadUnidadMedida] [float] NOT NULL,
	[fechaAlta] [datetime] NULL,
	[claveProdServ] [varchar](1000) NULL,
	[precioIndividual] [money] NULL,
	[precioMenudeo] [money] NULL,
	[costoCompra] [money] NULL,
	[porcUtilidadIndividual] [float] NULL,
	[porcUtilidadMayoreo] [float] NULL,
	[idUnidadCompra] [int] NULL,
	[cantidadUnidadCompra] [float] NULL,
 CONSTRAINT [PK_ProductosCostoCompra] PRIMARY KEY CLUSTERED 
(
	idProductoCostoCompra ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
)-- ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].ProductosCostoCompra ADD  CONSTRAINT [DF__ProductosCostoCompra__clave__379037E3]  DEFAULT ('1010101') FOR [claveProdServ]
GO

ALTER TABLE [dbo].ProductosCostoCompra  WITH CHECK ADD  CONSTRAINT [FK_ProductosCostoCompra_CatLineaProducto] FOREIGN KEY([idLineaProducto])
REFERENCES [dbo].[LineaProducto] ([idLineaProducto])
GO

ALTER TABLE [dbo].ProductosCostoCompra CHECK CONSTRAINT [FK_ProductosCostoCompra_CatLineaProducto]
GO

ALTER TABLE [dbo].ProductosCostoCompra  WITH CHECK ADD  CONSTRAINT [FK_ProductosCostoCompra_Productos] FOREIGN KEY([idUnidadMedida])
REFERENCES [dbo].[CatUnidadMedida] ([idUnidadMedida])
GO

ALTER TABLE [dbo].ProductosCostoCompra CHECK CONSTRAINT [FK_ProductosCostoCompra_Productos]
GO

insert into ProductosCostoCompra(
idProducto
,descripcion
,idUnidadMedida
,idLineaProducto
,cantidadUnidadMedida
,fechaAlta
,claveProdServ
,precioIndividual
,precioMenudeo
,costoCompra
,porcUtilidadIndividual
,porcUtilidadMayoreo
,idUnidadCompra
,cantidadUnidadCompra)
select 
idProducto
,descripcion
,idUnidadMedida
,idLineaProducto
,cantidadUnidadMedida
,getdate()
,claveProdServ
,precioIndividual
,precioMenudeo
,ultimoCostoCompra
,porcUtilidadIndividual
,porcUtilidadMayoreo
,idUnidadCompra
,cantidadUnidadCompra from Productos where activo = 1



--select * from ProductosCostoCompra
