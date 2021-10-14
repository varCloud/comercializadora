GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SP_APP_PEDIDOS_ESPECIALES_OBTENER_DETALLE_PEDIDOS_ESPECIALES]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE SP_APP_PEDIDOS_ESPECIALES_OBTENER_DETALLE_PEDIDOS_ESPECIALES
END
GO


CREATE PROCEDURE SP_APP_PEDIDOS_ESPECIALES_OBTENER_DETALLE_PEDIDOS_ESPECIALES
@idPedidoEspecial int,
@idAlmacen int
AS
BEGIN
		if not exists (select * from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial)
		begin
			select -1 status , 'no existe pedido especial' mensaje
		end

		select 200 status  , 'pedido especial encontrado' mensaje

		select PED.* , P.descripcion descProdcuto from PedidosEspecialesDetalle PED join 
		Productos P on PED.idProducto = P.idProducto and P.activo = 1
		where idAlmacenDestino = coalesce(@idAlmacen,idAlmacenDestino) and idPedidoEspecial = @idPedidoEspecial


	

END
GO
