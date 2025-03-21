--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  UserDefinedFunction [dbo].[obtenerPrecioCompra]    Script Date: 16/02/2022 21:47:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[obtenerPrecioCompra]
 (
 @idProducto AS int,
 @fecha as date=null
 )
RETURNS MONEY
AS
BEGIN

declare @precio float=0

select @fecha=coalesce(@fecha,dbo.fechaActual())

--select top 1 @precio=precio 
--from Compras c
--join ComprasDetalle d on c.idCompra=d.idCompra
--where --c.idStatusCompra=3 and 
--cantidadRecibida>0 and idProducto=@idProducto
--and cast(fechaRecibio as date)<=cast(@fecha as date)
--order by fechaRecibio desc



--SELECT top 1 @precio=coalesce(ultimoCostoCompra,0) FROM VentasDetalle VD join Ventas V on VD.idVenta = V.idVenta
--WHERE  
--VD.idProducto=@idProducto
--and V.idStatusVenta  = 1
--and cast(VD.fechaVentaDetalle as date)=cast(@fecha as date)
--order by fechaVentaDetalle desc

SELECT top 1 @precio=coalesce(costoCompra,0) FROM ProductosCostoCompra where idProducto=@idProducto
order by idProductoCostoCompra desc

-- si no existe el precio de compra a la fecha obtenemos el proximo que se capturo
if(@precio=0)
begin
	select  @precio=ultimoCostoCompra 
	from Productos P where idProducto = @idProducto
	--select top 1 @precio=precio 
	--from Compras c
	--join ComprasDetalle d on c.idCompra=d.idCompra
	--where --c.idStatusCompra=3 and 
	--cantidadRecibida>0 and idProducto=@idProducto
	--and cast(fechaRecibio as date)>cast(@fecha as date)
	--order by fechaRecibio asc
end

RETURN	 @precio
 
END