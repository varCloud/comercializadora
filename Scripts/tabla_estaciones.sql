USE [DB_A57E86_comercializadora]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Estaciones]') AND type in (N'U'))
DROP TABLE [dbo].[Estaciones]
GO

/****** Object:  Table [dbo].[CatAlmacen]    Script Date: 11/02/2020 05:10:57 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Estaciones](
	[idEstacion] [int] identity(1,1) NOT NULL,
	[idAlmacen] [int] NOT NULL,
	[macAdress] [varchar](250) NULL,
	[nombre] [varchar](250) NULL,
	[numero] [int] NULL,
	[configurado] [bit] NULL,
	[idUsuario] [int] NULL,
	[fechaAlta] [datetime] NULL,
	[idStatus] [int] NULL,

 CONSTRAINT [PK_Estaciones] PRIMARY KEY CLUSTERED 
(
	[idEstacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatEstaciones]') AND type in (N'U'))
DROP TABLE [dbo].[CatEstaciones]
GO

/****** Object:  Table [dbo].[CatAlmacen]    Script Date: 11/02/2020 05:10:57 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CatEstaciones](
	[idStatus] [int] NOT NULL,
	[descripcion] [varchar](50) NULL,
 CONSTRAINT [PK_CatEstaciones] PRIMARY KEY CLUSTERED 
(
	[idStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

insert into CatEstaciones (idStatus,descripcion) values (1,'Activa')
insert into CatEstaciones (idStatus,descripcion) values (2,'Inactiva')
