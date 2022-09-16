--USE [DB_A57E86_lluviadesarrollo]
GO

/****** Object:  Table [dbo].[CatEstatusProcesoAgranel]    Script Date: 13/09/2022 12:07:23 p. m. ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatEstatusProcesoAgranel]') AND type in (N'U'))
DROP TABLE [dbo].[CatEstatusProcesoAgranel]
GO

/****** Object:  Table [dbo].[CatEstatusProcesoAgranel]    Script Date: 13/09/2022 12:07:23 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CatEstatusProcesoAgranel](
	[idEstatusProcesoAgranel] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
 CONSTRAINT [PK_CatEstatusProcesoAgranel] PRIMARY KEY CLUSTERED 
(
	[idEstatusProcesoAgranel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


