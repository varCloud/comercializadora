GO

/****** Object:  Table [dbo].[Facturas]    Script Date: 27/11/2021 01:14:25 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].FacturasPedidosEspeciales') AND type in (N'U'))
DROP TABLE [dbo].[FacturasPedidosEspeciales]
GO

/****** Object:  Table [dbo].[Facturas]    Script Date: 27/11/2021 01:14:25 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FacturasPedidosEspeciales](
	[idFacturaPedidoEspecial] [bigint] IDENTITY(1,1) NOT NULL,
	[idPedidoEspecial] [bigint] NULL,
	[idUsuarioFacturacion] [bigint] NULL,
	[fechaTimbrado] [datetime] NULL,
	[fecha] [datetime] NULL,
	[UUID] [varchar](max) NULL,
	[idEstatusFactura] [int] NULL,
	[msjErrorFacturacion] [varchar](max) NULL,
	[fechaCancelacion] [datetime] NULL,
	[idUsuarioCancelacion] [bigint] NULL,
	[msjErrorCancelacion] [varchar](max) NULL,
	[pathArchivoFactura] [varchar](max) NULL,
	errorFactura int  default(0)
 CONSTRAINT [PK_FacturasPedidosEspeciales] PRIMARY KEY CLUSTERED 
(
	[idFacturaPedidoEspecial] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



GO

/****** Object:  Table [dbo].[Facturas]    Script Date: 27/11/2021 01:14:25 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].FacturasAbonos') AND type in (N'U'))
DROP TABLE [dbo].[FacturasAbonos]
GO

/****** Object:  Table [dbo].[Facturas]    Script Date: 27/11/2021 01:14:25 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FacturasAbonos](
	[idFacturAbono] [bigint] IDENTITY(1,1) NOT NULL,
	[idPedidoEspecial] [bigint] NULL,
	[idFacturaPedidoEspecial] [bigint] NULL,
	[idUsuarioFacturacion] [bigint] NULL,
	[fechaTimbrado] [datetime] NULL,
	[fecha] [datetime] NULL,
	[UUID] [varchar](max) NULL,
	[idEstatusFactura] [int] NULL,
	[msjErrorFacturacion] [varchar](max) NULL,
	[fechaCancelacion] [datetime] NULL,
	[idUsuarioCancelacion] [bigint] NULL,
	[msjErrorCancelacion] [varchar](max) NULL,
	[pathArchivoFactura] [varchar](max) NULL,
	errorFactura int default(0)
 CONSTRAINT [PK_FacturasAbonos] PRIMARY KEY CLUSTERED 
(
	idFacturAbono ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO




