IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_FACTURACION_OBTENER_DATOS_FACTURA')
DROP PROCEDURE SP_FACTURACION_OBTENER_DATOS_FACTURA
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_FACTURACION_OBTENER_DATOS_FACTURA]
@idVenta bigint
AS
BEGIN
declare 
@estatus int,
@mensaje varchar (200)
	
	if exists (	SELECT 1 FROM Ventas  V join Clientes C on V.idCliente = C.idCliente where v.idVenta =@idVenta and ((C.nombres is null or c.nombres ='') or (c.rfc is null or c.rfc='')))
	begin
		select -1 Estatus, 'Por favor actualiza los datos del clientes es necesario Nombre y Rfc' as mensaje
		return
	end


	SELECT 200 Estatus, 'OK' Mensaje
	
	SELECT
	isnull(C.nombres,'')+' '+isnull(c.apellidoPaterno,'')+' '+isnull(c.apellidoMaterno,'') Nombre,
	UPPER(C.rfc) as Rfc,
	c.telefono,
	c.correo,
	'Calle '+isnull(C.calle,'')+' '+isnull(c.numeroExterior,'')+' Colonia '+isnull(c.colonia,'')+' C.P '+ISNULL(c.cp,'')+' '+isnull(c.municipio,'')+' '+isnull(c.estado,'') domicilio,
	tc.descripcion tipoCliente
	,UCFDI.usoCFDI as UsoCFDI,
	UCFDI.descripcion as descripcionUsoCFDI,
     V.idFactFormaPago as FormaPago,
	FP.descripcion descripcionFormaPago,
	v.montoTotal
	FROM Ventas  V 
	join Clientes C on V.idCliente = C.idCliente
	join CatTipoCliente tc on c.idTipoCliente=tc.idTipoCliente
	join FactCatUsoCFDI UCFDI on UCFDI.id = 3
	join FactCatFormaPago FP on FP.id = V.idFactFormaPago
	where V.idVenta = @idVenta

END
