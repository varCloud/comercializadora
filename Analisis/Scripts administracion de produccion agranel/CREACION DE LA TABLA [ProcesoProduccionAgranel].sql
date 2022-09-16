USE [DB_A57E86_lluviadesarrollo]
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel] DROP CONSTRAINT [FK_ProcesoProduccionAgranel_Usuarios]
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel] DROP CONSTRAINT [FK_ProcesoProduccionAgranel_Ubicacion]
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel] DROP CONSTRAINT [FK_ProcesoProduccionAgranel_Productos]
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel] DROP CONSTRAINT [FK_ProcesoProduccionAgranel_Almacenes]
GO

/****** Object:  Table [dbo].[ProcesoProduccionAgranel]    Script Date: 13/09/2022 12:09:13 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProcesoProduccionAgranel]') AND type in (N'U'))
DROP TABLE [dbo].[ProcesoProduccionAgranel]
GO

/****** Object:  Table [dbo].[ProcesoProduccionAgranel]    Script Date: 13/09/2022 12:09:13 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProcesoProduccionAgranel](
	[idProcesoProduccionAgranel] [bigint] IDENTITY(1,1) NOT NULL,
	[idProducto] [int] NULL,
	[idUbicacion] [int] NULL,
	[cantidad] [float] NULL,
	[cantidadAceptada] [float] NULL,
	[cantidadRestante] [float] NULL,
	[fechaAlta] [datetime] NULL,
	[fechaUltimaActualizacion] [datetime] NULL,
	[idEstatusProduccionAgranel] [int] NULL,
	[idUsuario] [int] NULL,
	[idAlmacen] [int] NULL,
 CONSTRAINT [PK_ProcesoProduccionAgranel] PRIMARY KEY CLUSTERED 
(
	[idProcesoProduccionAgranel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel]  WITH CHECK ADD  CONSTRAINT [FK_ProcesoProduccionAgranel_Almacenes] FOREIGN KEY([idAlmacen])
REFERENCES [dbo].[Almacenes] ([idAlmacen])
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel] CHECK CONSTRAINT [FK_ProcesoProduccionAgranel_Almacenes]
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel]  WITH CHECK ADD  CONSTRAINT [FK_ProcesoProduccionAgranel_Productos] FOREIGN KEY([idProducto])
REFERENCES [dbo].[Productos] ([idProducto])
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel] CHECK CONSTRAINT [FK_ProcesoProduccionAgranel_Productos]
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel]  WITH CHECK ADD  CONSTRAINT [FK_ProcesoProduccionAgranel_Ubicacion] FOREIGN KEY([idUbicacion])
REFERENCES [dbo].[Ubicacion] ([idUbicacion])
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel] CHECK CONSTRAINT [FK_ProcesoProduccionAgranel_Ubicacion]
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel]  WITH CHECK ADD  CONSTRAINT [FK_ProcesoProduccionAgranel_Usuarios] FOREIGN KEY([idUsuario])
REFERENCES [dbo].[Usuarios] ([idUsuario])
GO

ALTER TABLE [dbo].[ProcesoProduccionAgranel] CHECK CONSTRAINT [FK_ProcesoProduccionAgranel_Usuarios]
GO


