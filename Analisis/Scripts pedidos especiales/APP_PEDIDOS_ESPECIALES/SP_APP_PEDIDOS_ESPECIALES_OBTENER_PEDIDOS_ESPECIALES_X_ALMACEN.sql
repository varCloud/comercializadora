GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SP_APP_PEDIDOS_ESPECIALES_OBTENER_PEDIDOS_ESPECIALES_X_ALMACEN]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE SP_APP_PEDIDOS_ESPECIALES_OBTENER_PEDIDOS_ESPECIALES_X_ALMACEN
END
GO


CREATE PROCEDURE SP_APP_PEDIDOS_ESPECIALES_OBTENER_PEDIDOS_ESPECIALES_X_ALMACEN
@idAlmacen int,
@idEstatusPedidoEspecialDetalle int
AS
BEGIN
		select 200 Estatus  , 'pedido especial encontrado' Mensaje
		select PE.* , isnull(C.nombres,'')+' '+isnull(C.apellidoPaterno,'')+''+isnull(C.apellidoMaterno,'') nombreCliente from PedidosEspeciales PE
		join (select idPedidoEspecial from PedidosEspecialesDetalle where idAlmacenDestino = @idAlmacen and idEstatusPedidoEspecialDetalle = @idEstatusPedidoEspecialDetalle
				group by idPedidoEspecial,idAlmacenDestino) PED 
		ON PE.idPedidoEspecial = PED.idPedidoEspecial
		join Clientes C on C.idCliente = PE.idCliente

END
GO
