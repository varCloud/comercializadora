-- USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_FACTURACION_OBTENER_DETALLE_PEDIDO_ESPECIAL]    Script Date: 11/14/2022 11:56:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_FACTURACION_OBTENER_DETALLE_PEDIDO_ESPECIAL]
@idPedidoEspecial bigint
AS
BEGIN
declare 
@estatus int,
@mensaje varchar (200)
	
	if exists (	SELECT 1 FROM PedidosEspeciales  V join Clientes C on V.idCliente = C.idCliente where v.idPedidoEspecial =@idPedidoEspecial and ((C.nombres is null or c.nombres ='') or (c.rfc is null or c.rfc='')))
	begin
		select -1 Estatus, 'Por favor actualiza los datos del clientes es necesario Nombre y Rfc' as mensaje
		return
	end

	
	SELECT 200 Estatus, 'OK' Mensaje
	
	SELECT
	UPPER(REPLACE(dbo.LimpiarCaracteres(RTRIM(LTRIM(isnull(C.nombres,'')+' '+isnull(c.apellidoPaterno,'')+' '+isnull(c.apellidoMaterno,'')))),'  ',' ')) Nombre,
	UPPER(C.rfc) as Rfc,
	c.correo,
	coalesce(c.sociedadMercantil,'') sociedadMercantil,
	'Calle '+isnull(C.calle,'')+' '+isnull(c.numeroExterior,'')+' Colonia '+isnull(c.colonia,'')+' C.P '+ISNULL(c.cp,'')+' '+isnull(c.municipio,'')+' '+isnull(c.estado,'') domicilio,
	UCFDI.usoCFDI as UsoCFDI,
	UCFDI.descripcion as descripcionUsoCFDI,
    FP.formaPago as FormaPago,
	C.cp DomicilioFiscalReceptor,
	FP.descripcion descripcionFormaPago,
	RF.regimenFiscal RegimenFiscalReceptor,
	RF.descripcion descripcionRegimenFiscalReceptor
	FROM PedidosEspeciales  V 
	join Clientes C on V.idCliente = C.idCliente
	join FactCatUsoCFDI UCFDI on UCFDI.id = V.idFactUsoCFDI
	join FactCatFormaPago FP on FP.id = V.idFactFormaPago
	left join FactCatRegimenFiscal RF on RF.idRegimenFiscal = C.idCatRegimenFiscal
	where V.idPedidoEspecial = @idPedidoEspecial


	SELECT
	 P.claveProdServ ClaveProdserv,
	 UM.claveUnidadSAT ClaveUnidad ,
	 PED.cantidad Cantidad,
	 UM.unidadSat Unidad,
	 P.articulo NoIdentificacion,
	REPLACE(RTRIM(LTRIM(P.descripcion)),'&',' ') Descripcion,
	cast ((PED.precioVenta) as decimal (16,2)) ValorUnitario,
	
	 cast ((PED.monto) as decimal (16,2)) Importe
	 --LOS CAMPOS DE ARRIBA SON NECESARIOS PARA EL  NODO DEL CONCEPTO
	 /*cast ((VD.monto * VD.cantidad) as decimal (16,2)) Base,
	 '002'  Impuesto,
	 'Tasa' TipoFactor,
	 0.16 TasaOCuota,
	 cast (((VD.monto * VD.cantidad)*0.16) as decimal (16,2)) ImporteT */
	 --LOS CAMPOS DE ARRIBA SON NECESARIOS PARA EL NODO TRASLADO QUE EXISTE
	 --DENTRO DEL NODO DE CONCEPTOS ACTUALMENT NO SETEAN EN CODIGO C#
	from dbo.[PedidosEspecialesDetalle] as PED  join Productos P 
	on P.idProducto =  PED.idProducto join CatUnidadMedida UM 
	on UM.idUnidadMedida = P.idUnidadMedida 
	where 
	PED.idPedidoEspecial = @idPedidoEspecial
	AND PED.cantidad >0


	
	/***************************************/
	/***************************************/
	-- PARA EL COMPLEMENTO DE LA ADDENTA---
	/****************************************/
	/***************************************/

	-- REGRESA LOS CONCEPTOS CON DESCRIPCIONES MAS DETALLADAS DE LOS MISMOS -- 
	SELECT
	 P.claveProdServ ClaveProdserv,
	 PD.descripcion DescripcionClaveProdServ,
	 UM.claveUnidadSAT ClaveUnidad ,
	 UM.descripcionSAT DescripcionClaveUnidad,
	 PED.cantidad Cantidad,
	 UM.unidadSat Unidad,
	 P.articulo NoIdentificacion,
	 P.descripcion Descripcion,
	 cast ((PED.precioVenta) as decimal (16,2)) ValorUnitario,
	
	 cast ((PED.monto) as decimal (16,2)) Importe
	from dbo.[PedidosEspecialesDetalle] as PED  join Productos P 
	on P.idProducto =  PED.idProducto join CatUnidadMedida UM 
	on UM.idUnidadMedida = P.idUnidadMedida  join FactCatClaveProdServicio PD
	on PD.claveProdServ = P.claveProdServ
	where PED.idPedidoEspecial = @idPedidoEspecial
	AND PED.cantidad >0

END
