----USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_APP_OBTENER_PRODUCTOS_PRODUCCION_AGRANEL]    Script Date: 21/07/2022 11:13:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].SP_APP_OBTENER_PRODUCTOS_PRODUCCION_AGRANEL
@idAlmacen int,
@idUsuario int = null,
@idEstatusProduccionAgranel int = null,
@fechaIni datetime = null,
@fechaFin datetime = null

AS
BEGIN
		
			-- SET @fechaIni = coalesce(@fechaIni , dbo.fechaActual())
			-- SET @fechaFin = coalesce(@fechaFin , dbo.fechaActual())

			if exists (select 1 from procesoProduccionAgranel PPA 
						join Ubicacion U on U.idUbicacion = PPA.idUbicacion 			
						where U.idAlmacen =@idAlmacen
						 and cast(PPA.fechaAlta as date) >= cast(coalesce(@fechaIni ,fechaAlta ) as date)
						 and cast(PPA.fechaAlta as date) <= cast(coalesce(@fechaFin , fechaAlta) as date)
						 and PPA.idEstatusProduccionAgranel = coalesce(@idEstatusProduccionAgranel ,  PPA.idEstatusProduccionAgranel))
			begin
					SELECT 200 Estatus , 'Ok' Mensaje
					SELECT 
					 PPA.idProcesoProduccionAgranel,
					 PPA.cantidad,
					 PPA.fechaAlta,
					 PPA.idEstatusProduccionAgranel,
					 PPA.idProducto, 
					 P.*, 
					 [dbo].[LineaProductoFraccion](P.idLineaProducto,null) fraccion
					 
					FROM procesoProduccionAgranel PPA 
						join Ubicacion U on U.idUbicacion = PPA.idUbicacion 
						join Productos P on P.idProducto = PPA.idProducto
					WHERE U.idAlmacen =@idAlmacen
						 and cast(PPA.fechaAlta as date) >= cast(coalesce(@fechaIni ,PPA.fechaAlta) as date)
						 and cast(PPA.fechaAlta as date) <= cast(coalesce(@fechaFin ,PPA.fechaAlta) as date)
						 and PPA.idEstatusProduccionAgranel = coalesce(@idEstatusProduccionAgranel ,  PPA.idEstatusProduccionAgranel)
					return
			end
			else
			begin
				SELECT -1 Estatus , 'No existen resultado con esos terminos de busqueda' Mensaje
				return
			end

				



END
