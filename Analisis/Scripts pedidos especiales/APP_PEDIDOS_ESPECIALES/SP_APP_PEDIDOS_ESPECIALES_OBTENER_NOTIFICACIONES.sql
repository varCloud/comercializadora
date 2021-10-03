-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE SP_APP_PEDIDOS_ESPECIALES_OBTENER_NOTIFICACIONES
@idEstatusPedidoEspecialDetalle int = null, -- 1	Solcitados 2	Atendidos 3	Rechazados 4	Aceptados
@idAlmacenOrigen int = null, -- los obtiene del web service que regresa los pedidos internos
@idAlmacenDestino int = null-- los obtiene del web service que regresa los pedidos internos
AS
BEGIN

	if (@idAlmacenOrigen is null and @idAlmacenDestino is null)
	begin 
		select -1 Estatus , 'No pueden ir null los 2 almacenes ' Mensaje
		return 
	end
	

	if (OBJECT_ID('tempdb.dbo.#LineaProductoUsuario','U')) is not null
		drop table #LineaProductoUsuario
	CREATE TABLE #LineaProductoUsuario (contador int primary key identity ,
	idLineaProducto int , idAlamacen int ,  descripcion varchar(255))
		
	insert into #LineaProductoUsuario select * from obtnerLineasProductosXAlmacen(null , coalesce(@idAlmacenOrigen ,@idAlmacenDestino ))

	CREATE TABLE  #notificaciones(idPedidoEspecial INT null , idPedidoEspecialDetalle int null)

	INSERT INTO #notificaciones (idPedidoEspecial , idPedidoEspecialDetalle)
	select PE.idPedidoEspecial, PE.idPedidoEspecialDetalle from PedidosEspecialesDetalle PE JOIN
		(select idAlmacenDestino,idPedidoEspecial from PedidosEspecialesDetalle where
			notificado = 0
			AND idAlmacenOrigen = coalesce (@idAlmacenOrigen ,  idAlmacenOrigen)
			AND idAlmacenDestino = coalesce (@idAlmacenDestino ,  idAlmacenDestino)
			group by idPedidoEspecial, idAlmacenDestino) notificados
	on PE.idPedidoEspecial = notificados.idPedidoEspecial join Productos Prod 
	on Prod.idProducto = PE.idProducto join #LineaProductoUsuario L 
	on L.idLineaProducto = Prod.idLineaProducto
	WHERE
		PE.idEstatusPedidoEspecialDetalle = coalesce(@idEstatusPedidoEspecialDetalle,PE.idEstatusPedidoEspecialDetalle)
		AND PE.notificado = 0
		AND PE.idAlmacenOrigen = coalesce (@idAlmacenOrigen ,  PE.idAlmacenOrigen)
		AND PE.idAlmacenDestino = coalesce (@idAlmacenDestino ,  PE.idAlmacenDestino)
	order by PE.fechaAlta asc
	

	IF (SELECT COUNT(*)  FROM #notificaciones ) <= 0 
	BEGIN
		SELECT -1 status, 'No existen registros para notificar' mensaje;
		RETURN
	END

	UPDATE PedidosEspecialesDetalle SET notificado = 1	WHERE idPedidoEspecial in (select idPedidoEspecial from #notificaciones)

	select 200 status , 'notificaciones encontradas' mensaje
	select * from PedidosEspecialesDetalle P  join #notificaciones N on P.idPedidoEspecialDetalle = N.idPedidoEspecialDetalle

END
GO