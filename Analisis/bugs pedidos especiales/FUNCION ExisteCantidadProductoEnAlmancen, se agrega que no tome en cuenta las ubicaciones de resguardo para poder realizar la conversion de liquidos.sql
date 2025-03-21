--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  UserDefinedFunction [dbo].[ExisteCantidadProductoEnAlmancen]    Script Date: 1/3/2023 12:58:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[ExisteCantidadProductoEnAlmancen]
(
@idAlmacen int
,@idProducto int
)
RETURNS float
AS
BEGIN
	-- Declare the return variable here
	declare @cantidad float = 0

		--AGRUPAMOS POR UBICACION, ALMACEN Y PRODUCTO PARA SABER SI EXISTA CANTIDAD DE UN PRODUCTO EN UN ALMACEN
		SELECT @cantidad = SUM(cantidad)  from (
			SELECT idProducto, SUM(ID.cantidad) cantidad , idAlmacen,ID.idUbicacion  
			FROM InventarioDetalle ID join Ubicacion U on ID.idUbicacion = U.idUbicacion  
			where idProducto = @idProducto and idAlmacen = @idAlmacen and U.idPiso < 1000
			group by ID.idUbicacion,idAlmacen, idProducto
		) A
		SET @cantidad = COALESCE(@cantidad,0)

		/* fin  esta validacion quiza deberia de ir en una funcion */

		return @cantidad --el producto existe;

END