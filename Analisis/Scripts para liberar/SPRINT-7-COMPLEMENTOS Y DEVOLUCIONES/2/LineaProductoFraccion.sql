--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  UserDefinedFunction [dbo].[ExisteProductoEnAlmancen]    Script Date: 12/06/2021 09:43:39 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	esta funcion determina si un producto o un linea de producto necesita decimales en el front -end 
-- =============================================
create FUNCTION [dbo].[LineaProductoFraccion]
(
 @idLineaProducto int = null
,@idProducto int = null
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	declare @necesitaDecimal int 
		
		if @idProducto is null
			select @necesitaDecimal = case when @idLineaProducto in (12,20,22,25) then 1 else 0 end 

		if @idLineaProducto is null
			select @necesitaDecimal = case when @idLineaProducto in (12,20,22,25) then 1 else 0 end
			from Productos p  join LineaProducto L on p.idLineaProducto = L.idLineaProducto and P.idProducto = @idProducto
			where p.idProducto = @idProducto
			 
		/* fin  esta validacion quiza deberia de ir en una funcion */

		return @necesitaDecimal --el producto existe;

END
