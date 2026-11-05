/*
Objetivo	Tablas de la funcion "Proceso Produccion (formulas)":
			ProcesoProduccionFormula          (formula: producto a producir + cantidad del lote)
			ProcesoProduccionFormulaDetalle   (materias primas por lote)
			ProcesoProduccion                 (cada produccion realizada; no se revierte ni edita)
			ProcesoProduccionDetalle          (copia del consumo de cada MPL de esa produccion)
Idempotente: cada objeto se crea solo si no existe.
*/

-- VERIFICACION PREVIA (0 filas la primera vez)
select name from sys.tables where name in ('ProcesoProduccionFormula','ProcesoProduccionFormulaDetalle','ProcesoProduccion','ProcesoProduccionDetalle')
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[ProcesoProduccionFormula]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProcesoProduccionFormula](
		[idProcesoProduccionFormula] [bigint] IDENTITY(1,1) NOT NULL,
		[idProducto] [int] NOT NULL,
		[cantidadLote] [float] NOT NULL,
		[idUnidadMedida] [int] NOT NULL,
		[activo] [bit] NOT NULL CONSTRAINT [DF_ProcesoProduccionFormula_activo] DEFAULT (1),
		[idUsuario] [int] NOT NULL,
		[fechaAlta] [datetime] NOT NULL,
		[fechaUltimaActualizacion] [datetime] NOT NULL,
		CONSTRAINT [PK_ProcesoProduccionFormula] PRIMARY KEY CLUSTERED ([idProcesoProduccionFormula] ASC),
		CONSTRAINT [CK_ProcesoProduccionFormula_cantidadLote] CHECK ([cantidadLote] > 0)
	)
	ALTER TABLE [dbo].[ProcesoProduccionFormula] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccionFormula_Productos] FOREIGN KEY([idProducto]) REFERENCES [dbo].[Productos]([idProducto])
	ALTER TABLE [dbo].[ProcesoProduccionFormula] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccionFormula_CatUnidadMedida] FOREIGN KEY([idUnidadMedida]) REFERENCES [dbo].[CatUnidadMedida]([idUnidadMedida])
	ALTER TABLE [dbo].[ProcesoProduccionFormula] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccionFormula_Usuarios] FOREIGN KEY([idUsuario]) REFERENCES [dbo].[Usuarios]([idUsuario])
	-- una sola formula activa por producto
	CREATE UNIQUE NONCLUSTERED INDEX [UX_ProcesoProduccionFormula_idProducto_activo] ON [dbo].[ProcesoProduccionFormula]([idProducto]) WHERE [activo] = 1
END
GO

IF OBJECT_ID(N'[dbo].[ProcesoProduccionFormulaDetalle]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProcesoProduccionFormulaDetalle](
		[idProcesoProduccionFormulaDetalle] [bigint] IDENTITY(1,1) NOT NULL,
		[idProcesoProduccionFormula] [bigint] NOT NULL,
		[idProducto] [int] NOT NULL,
		[cantidad] [float] NOT NULL,
		[idUnidadMedida] [int] NOT NULL,
		CONSTRAINT [PK_ProcesoProduccionFormulaDetalle] PRIMARY KEY CLUSTERED ([idProcesoProduccionFormulaDetalle] ASC),
		CONSTRAINT [CK_ProcesoProduccionFormulaDetalle_cantidad] CHECK ([cantidad] > 0),
		CONSTRAINT [UQ_ProcesoProduccionFormulaDetalle_formula_producto] UNIQUE ([idProcesoProduccionFormula], [idProducto])
	)
	ALTER TABLE [dbo].[ProcesoProduccionFormulaDetalle] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccionFormulaDetalle_ProcesoProduccionFormula] FOREIGN KEY([idProcesoProduccionFormula]) REFERENCES [dbo].[ProcesoProduccionFormula]([idProcesoProduccionFormula])
	ALTER TABLE [dbo].[ProcesoProduccionFormulaDetalle] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccionFormulaDetalle_Productos] FOREIGN KEY([idProducto]) REFERENCES [dbo].[Productos]([idProducto])
	ALTER TABLE [dbo].[ProcesoProduccionFormulaDetalle] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccionFormulaDetalle_CatUnidadMedida] FOREIGN KEY([idUnidadMedida]) REFERENCES [dbo].[CatUnidadMedida]([idUnidadMedida])
END
GO

IF OBJECT_ID(N'[dbo].[ProcesoProduccion]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProcesoProduccion](
		[idProcesoProduccion] [bigint] IDENTITY(1,1) NOT NULL,
		[idProcesoProduccionFormula] [bigint] NOT NULL,
		[idProducto] [int] NOT NULL,
		[lotes] [float] NOT NULL,
		[cantidadLote] [float] NOT NULL,
		[cantidadProducida] [float] NOT NULL,
		[idUnidadMedida] [int] NOT NULL,
		[idUsuario] [int] NOT NULL,
		[idAlmacen] [int] NOT NULL,
		[fechaAlta] [datetime] NOT NULL,
		CONSTRAINT [PK_ProcesoProduccion] PRIMARY KEY CLUSTERED ([idProcesoProduccion] ASC),
		CONSTRAINT [CK_ProcesoProduccion_lotes] CHECK ([lotes] > 0)
	)
	ALTER TABLE [dbo].[ProcesoProduccion] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccion_ProcesoProduccionFormula] FOREIGN KEY([idProcesoProduccionFormula]) REFERENCES [dbo].[ProcesoProduccionFormula]([idProcesoProduccionFormula])
	ALTER TABLE [dbo].[ProcesoProduccion] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccion_Productos] FOREIGN KEY([idProducto]) REFERENCES [dbo].[Productos]([idProducto])
	ALTER TABLE [dbo].[ProcesoProduccion] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccion_CatUnidadMedida] FOREIGN KEY([idUnidadMedida]) REFERENCES [dbo].[CatUnidadMedida]([idUnidadMedida])
	ALTER TABLE [dbo].[ProcesoProduccion] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccion_Usuarios] FOREIGN KEY([idUsuario]) REFERENCES [dbo].[Usuarios]([idUsuario])
	ALTER TABLE [dbo].[ProcesoProduccion] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccion_Almacenes] FOREIGN KEY([idAlmacen]) REFERENCES [dbo].[Almacenes]([idAlmacen])
	CREATE NONCLUSTERED INDEX [IX_ProcesoProduccion_idUsuario_fechaAlta] ON [dbo].[ProcesoProduccion]([idUsuario], [fechaAlta])
END
GO

IF OBJECT_ID(N'[dbo].[ProcesoProduccionDetalle]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProcesoProduccionDetalle](
		[idProcesoProduccionDetalle] [bigint] IDENTITY(1,1) NOT NULL,
		[idProcesoProduccion] [bigint] NOT NULL,
		[idProducto] [int] NOT NULL,
		[cantidadPorLote] [float] NOT NULL,
		[cantidadConsumida] [float] NOT NULL,
		[idUnidadMedida] [int] NOT NULL,
		CONSTRAINT [PK_ProcesoProduccionDetalle] PRIMARY KEY CLUSTERED ([idProcesoProduccionDetalle] ASC)
	)
	ALTER TABLE [dbo].[ProcesoProduccionDetalle] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccionDetalle_ProcesoProduccion] FOREIGN KEY([idProcesoProduccion]) REFERENCES [dbo].[ProcesoProduccion]([idProcesoProduccion])
	ALTER TABLE [dbo].[ProcesoProduccionDetalle] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccionDetalle_Productos] FOREIGN KEY([idProducto]) REFERENCES [dbo].[Productos]([idProducto])
	ALTER TABLE [dbo].[ProcesoProduccionDetalle] WITH CHECK ADD CONSTRAINT [FK_ProcesoProduccionDetalle_CatUnidadMedida] FOREIGN KEY([idUnidadMedida]) REFERENCES [dbo].[CatUnidadMedida]([idUnidadMedida])
	CREATE NONCLUSTERED INDEX [IX_ProcesoProduccionDetalle_idProcesoProduccion] ON [dbo].[ProcesoProduccionDetalle]([idProcesoProduccion])
END
GO

-- VERIFICACION POSTERIOR (4 tablas, FKs e indice unico filtrado)
select name from sys.tables where name in ('ProcesoProduccionFormula','ProcesoProduccionFormulaDetalle','ProcesoProduccion','ProcesoProduccionDetalle')
select name from sys.foreign_keys where name like 'FK_ProcesoProduccion%' and name not like '%Agranel%'
select name, filter_definition from sys.indexes where name = 'UX_ProcesoProduccionFormula_idProducto_activo'
GO
