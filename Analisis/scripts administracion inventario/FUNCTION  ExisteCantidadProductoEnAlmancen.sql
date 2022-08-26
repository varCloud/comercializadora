GO
/****** Object:  UserDefinedFunction [dbo].[ExisteProductoEnAlmancen]    Script Date: 03/07/2022 07:11:14 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
alter FUNCTION [dbo].[ExisteCantidadProductoEnAlmancen]
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
			where idProducto = @idProducto and idAlmacen = @idAlmacen
			group by ID.idUbicacion,idAlmacen, idProducto
		) A
		SET @cantidad = COALESCE(@cantidad,0)

		/* fin  esta validacion quiza deberia de ir en una funcion */

		return @cantidad --el producto existe;

END
