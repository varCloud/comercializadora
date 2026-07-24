--USE [DB_A57E86_lluviadesarrollo]
GO

/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*

Objetivo	Consulta detalle (item por item, sin agrupar) de ProcesoProduccionAgranel,
			calculando el costo de produccion por renglon y el ultimo costo de compra
			del producto (mas reciente, sin importar la fecha del renglon).
			Filtra solo por rango de fecha (fechaAlta) y idEstatusProduccionAgranel.
status		200 = ok
			-1  = error

*/

CREATE proc [dbo].[SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO]

	@fechaIni datetime = null,
	@fechaFin datetime = null,
	@idEstatusProduccionAgranel int = null

as

	begin -- procedimiento
		begin -- inicio

			select 200 status, 'se encontraron resultados'

			SELECT
				r.idProcesoProduccionAgranel,
				r.idProducto,
				p.codigoBarras,
				p.descripcion descripcionProducto,
				r.cantidad,
				r.cantidadAceptada,
				r.cantidadRestante,
				r.fechaAlta,
				r.fechaUltimaActualizacion,
				r.idEstatusProduccionAgranel,
				case when EPA.idEstatusProcesoAgranel in (1,2) then 'Pendiente de procesar' else EPA.descripcion end descripcionEstatus,
				dbo.obtenerPrecioCompra(r.idProducto, null) ultimoCostoCompra,
				dbo.redondear(r.cantidadAceptada * dbo.obtenerPrecioCompra(r.idProducto, null)) costoProduccion
			FROM ProcesoProduccionAgranel r
			join Productos p on r.idProducto = p.idProducto
			join CatEstatusProcesoAgranel EPA on EPA.idEstatusProcesoAgranel = r.idEstatusProduccionAgranel
			where
				cast(r.fechaAlta as date) >= cast(coalesce(@fechaIni, r.fechaAlta) as date)
				and cast(r.fechaAlta as date) <= cast(coalesce(@fechaFin, r.fechaAlta) as date)
				and r.idEstatusProduccionAgranel = coalesce(@idEstatusProduccionAgranel, r.idEstatusProduccionAgranel)
			order by r.fechaAlta desc

		end -- inicio
	end -- procedimiento
GO
