GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SP_APP_PEDIDOS_ESPECIALES_OBTENER_PEDIDOS_ESPECIALES_X_ALMACEN]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE SP_APP_PEDIDOS_ESPECIALES_OBTENER_PEDIDOS_ESPECIALES_X_ALMACEN
END
GO


CREATE PROCEDURE SP_APP_PEDIDOS_ESPECIALES_OBTENER_PEDIDOS_ESPECIALES_X_ALMACEN
@idAlmacen int,
@idEstatusPedidoEspecialDetalle int,
@fechaInicio datetime = null,
@fechaFin datetime = null
AS
BEGIN
		select 200 status  , 'pedido especial encontrado' mensaje
		select 
			A.idAlmacen idAlmacenOrigen , A.Descripcion descAlmacenOrigen,	A.Descripcion ,
			isnull(C.nombres,'')+' '+isnull(C.apellidoPaterno,'')+''+isnull(C.apellidoMaterno,'') nombreCliente,
			PED.descEstatusPedidoEspecial
			,PE.*
		from PedidosEspeciales PE
		join (select  PED.idPedidoEspecial,PED.idAlmacenDestino, case when PED.idEstatusPedidoEspecialDetalle <> 1 then 'Atendido' else 'Solicitado' end descEstatusPedidoEspecial
			from PedidosEspeciales  PE join  PedidosEspecialesDetalle PED
			on PE.idPedidoEspecial = PED.idPedidoEspecial and PED.idAlmacenDestino = @idAlmacen
			and PED.idEstatusPedidoEspecialDetalle = coalesce(@idEstatusPedidoEspecialDetalle, PED.idEstatusPedidoEspecialDetalle)
			group by PED.idPedidoEspecial, PED.idAlmacenDestino ,case when PED.idEstatusPedidoEspecialDetalle <> 1 then 'Atendido' else 'Solicitado' end) PED 
		ON PE.idPedidoEspecial = PED.idPedidoEspecial

					   		
		join Usuarios U on PE.idUsuario = U.idUsuario
		join Almacenes A on U.idAlmacen = A.idAlmacen
		join Clientes C on C.idCliente = PE.idCliente
		
		where cast(PE.fechaAlta as date) >= coalesce(cast(@fechaInicio as date),cast(PE.fechaAlta as date))
		and cast(PE.fechaAlta as date) <= coalesce(cast(@fechaFin as date),cast(PE.fechaAlta as date))


END
GO
