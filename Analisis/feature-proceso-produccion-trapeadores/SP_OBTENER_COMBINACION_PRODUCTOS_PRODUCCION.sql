GO
IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE object_id = OBJECT_ID(N'dbo.SP_OBTENER_COMBINACION_PRODUCTOS_PRODUCCION'))
BEGIN
   DROP PROCEDURE SP_OBTENER_COMBINACION_PRODUCTOS_PRODUCCION
END

GO
/****** Object:  StoredProcedure [dbo].[[SP_OBTENER_COMBINACION_PRODUCTOS_PRODUCCION]]    Script Date: 10/9/2023 11:58:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].SP_OBTENER_COMBINACION_PRODUCTOS_PRODUCCION
@idProducto int = null
AS
BEGIN
	SELECT 200 estatus, 'Ok' mensaje
	create table #productos_combinaciones(
			id int identity(1,1),
			idProductoProducido int,
			idProductoMateria1 float default(null),
			idProductoMateria2 int default(null),
			unidadMedidad varchar(500) default(null),
			valorUnidadMedida float default(null),
			idUnidadMedidad int default(null),
			productoProducido varchar(500) default(null),
			productoMateria1 varchar(500) default(null),
			productoMateria2 varchar(500) default(null)

	)


	--INSERTAMOS EL PRODUCTO A PRODUCIR
	insert into #productos_combinaciones (idProductoProducido, productoProducido)
	select idProductoProduccion, MAX(P.descripcion) from MateriasPrimasProduccion MPP join Productos P
	on P.idProducto = MPP.idProductoProduccion where MPP.activo = 1 and P.activo = 1 group by idProductoProduccion

	update #productos_combinaciones
	set #productos_combinaciones.idProductoMateria1 = P.idProducto
	, #productos_combinaciones.unidadMedidad = MPP.unidadMedidad
	, #productos_combinaciones.valorUnidadMedida = MPP.valorUnidadMedida
	, #productos_combinaciones.idUnidadMedidad = MPP.idUnidadMedidad
	, #productos_combinaciones.productoMateria1 = P.descripcion
	from MateriasPrimasProduccion MPP  join Productos P on MPP.idProductoMateriaProduccion = P.idProducto and P.idLineaProducto = 21 
	join #productos_combinaciones PC on PC.idProductoProducido = MPP.idProductoProduccion
	where P.idLineaProducto = 21 -- MATRA

	update #productos_combinaciones 
	set #productos_combinaciones.idProductoMateria2 = P.idProducto
	, #productos_combinaciones.productoMateria2 = P.descripcion
	from MateriasPrimasProduccion MPP  join Productos P on MPP.idProductoMateriaProduccion = P.idProducto 
	join #productos_combinaciones PC on PC.idProductoProducido = MPP.idProductoProduccion
	where P.idLineaProducto = 26 -- BASTONES

	select * from #productos_combinaciones PC where  PC.idProductoProducido = coalesce( @idProducto , PC.idProductoProducido)
	
END
