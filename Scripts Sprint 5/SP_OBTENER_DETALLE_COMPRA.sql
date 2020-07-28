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
ALTER  PROCEDURE SP_OBTENER_DETALLE_COMPRA
@idCompra int  = null
AS
BEGIN
		select 200 Estatus , 'OK' Mensaje
		SELECT 
		 idCompraDetalle
		,idCompra
		,cantidad
		,precio
		,cantidadRecibida
		,isnull(observaciones,'') observaciones
		,isnull(P.idProducto,0)idProducto , P.descripcion
		,isnull(PC.idEstatusProductoCompra,0)idEstatusProductoCompra,PC.descripcion
		,isnull(U.idUsuario,0)idUsuario
		,isnull(U.nombre,'')+' '+isnull(U.apellidoPaterno,'')+' '+isnull(U.apellidoMaterno,'') nombre
		FROM [DB_A57E86_lluviadesarrollo].[dbo].[ComprasDetalle] CD 
		LEFT JOIN Productos P on CD.idProducto = P.idProducto
		LEFT JOIN CatEstatusProductoCompra PC on Pc.idEstatusProductoCompra = CD.idEstatusProductoCompra
		LEFT JOIN Usuarios U on U.idUsuario = CD.idUsuarioRecibio
		where Cd.idCompra = coalesce(@idCompra , CD.idCompra)
		
		-- Select * from CatEstatusProductoCompra
		-- select *  FROM [DB_A57E86_lluviadesarrollo].[dbo].[ComprasDetalle]


END
GO
