USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  Table [dbo].[AlmacenesXLineaProducto]    Script Date: 30/05/2021 06:37:59 p. m. ******/
DROP TABLE IF EXISTS [dbo].[AlmacenesXLineaProducto]
GO
/****** Object:  Table [dbo].[AlmacenesXLineaProducto]    Script Date: 30/05/2021 06:37:59 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AlmacenesXLineaProducto](
	[idAlmacenLineaProducto] [int] IDENTITY(1,1) NOT NULL,
	[idAlmacen] [int] NULL,
	[idLineaProducto] [int] NULL,
	[fechAlta] [datetime] NULL,
	[activo] [int] NULL,
 CONSTRAINT [PK_AlmacenesXLineaProducto] PRIMARY KEY CLUSTERED 
(
	[idAlmacenLineaProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AlmacenesXLineaProducto] ON 
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (1, 3, 1, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (2, 3, 2, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (3, 3, 4, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (4, 3, 5, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (5, 3, 6, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (6, 3, 7, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (7, 3, 9, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (8, 3, 10, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (9, 3, 11, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (10, 3, 12, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (11, 3, 13, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (12, 3, 19, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (13, 3, 20, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (14, 3, 21, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (15, 3, 22, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (16, 3, 23, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (17, 3, 25, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (18, 3, 26, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (19, 4, 1, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (20, 4, 2, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (21, 4, 4, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (22, 4, 5, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (23, 4, 6, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (24, 4, 7, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (25, 4, 9, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (26, 4, 10, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (27, 4, 11, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (28, 4, 12, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (29, 4, 13, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (30, 4, 19, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (31, 4, 20, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (32, 4, 21, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (33, 4, 22, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (34, 4, 23, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (35, 4, 25, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (36, 4, 26, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
SET IDENTITY_INSERT [dbo].[AlmacenesXLineaProducto] OFF
GO
