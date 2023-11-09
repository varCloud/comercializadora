
GO
IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE object_id = OBJECT_ID(N'dbo.SP_FACTURAS_OBTENER_PATH_ARCHIVO'))
BEGIN
   DROP PROCEDURE SP_FACTURAS_OBTENER_PATH_ARCHIVO
END

GO
/****** Object:  StoredProcedure [dbo].[SP_OBTENER_PRODUCTOS_ENVASES_LIQUIDOS_AGRANEL]    Script Date: 10/9/2023 5:16:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].SP_FACTURAS_OBTENER_PATH_ARCHIVO
   @idVenta int = null,
   @idPedidoEspecial int = null
AS
BEGIN
	if @idVenta is null and @idPedidoEspecial is null begin
		select -1 estatus, 'No pueden ser null los dos ids' mensaje
		return
	end

	if @idVenta is not null and @idPedidoEspecial is not null begin
		select -1 estatus, 'Al menos un elemento debe ser null de [@idVenta,@idPedidoEspecial]' mensaje
		return
	end

	if @idVenta is not null begin
		select * from Facturas where idVenta = @idVenta
		return
	end

		if @idPedidoEspecial is not null begin
		select * from FacturasPedidosEspeciales where idPedidoEspecial = @idPedidoEspecial
		return
	end
END
	