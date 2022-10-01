
/****** Object:  Table [dbo].[ReporteMerma]    Script Date: 14/09/2022 10:56:55 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReporteCostoProduccion]') AND type in (N'U'))
DROP TABLE [dbo].[ReporteCostoProduccion]
GO

/****** Object:  Table [dbo].[ReporteMerma]    Script Date: 14/09/2022 10:56:55 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReporteCostoProduccion](
	[idReporteCostoProduccion] [bigint] IDENTITY(1,1) NOT NULL,
	[idProducto] [bigint] NULL,
	[cantidadSolicitadaMesAnt] [float] NULL,
	[cantidadAceptadaFinalMesAnt] [float] NULL,
	--[totalCompras] [float] NULL,
	--[inventarioSistema] [float] NULL,
	--[merma] [float] NULL,
	--[cantidadCostoProduccion] [float] NULL,
	[porcCostoProduccion] [float] NULL,
	[ultCostoCompra] [money] NULL,
	[costoProduccionMerma] [money] NULL,
	[ultimoDiaMesCalculo] [date] NULL,
	[ultimoDiaMesAnterior] [date] NULL,
	[fechaAlta] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ReporteCostoProduccion] ADD  DEFAULT ((0)) FOR [cantidadAceptadaFinalMesAnt]
GO
ALTER TABLE [dbo].[ReporteCostoProduccion] ADD  DEFAULT ((0)) FOR [cantidadSolicitadaMesAnt]
GO


