/*
Objetivo	Los reportes de Consumo de MPL (MPL Agrupado / MPL Individual) incluyen tambien el consumo de materias primas
			de la produccion por formula (ProcesoProduccionDetalle unido a ProcesoProduccion por fecha), con UNION ALL.
			- Solo MPL consumidas (el producto terminado NO aparece).
			- cantidad solicitada = cantidad aceptada = cantidadConsumida; estatus 3 "Procesado correctamente".
			- Costo con dbo.obtenerPrecioCompra, igual que el resto de filas.
			- SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO agrega al FINAL las columnas:
				origen varchar(30)  ('Consumo MPL' | 'Produccion por formula')
				idProcesoProduccion bigint (NULL en las filas de ProcesoProduccionAgranel; idProcesoProduccionAgranel es NULL en las de formula)
			- Sin datos de formula el resultado es identico al original (columnas previas); verificado en BD local.
Revertir:	07a_ROLLBACK_SPs_reportes_MPL_originales.sql
Ejecutar con sqlcmd -f 65001 -I (archivo UTF-8).
NOTA: los meses cerrados de ReporteCostoProduccion ya estan cacheados y NO cambian; solo el mes actual (o meses sin cache) incorpora la formula.
*/

-- VERIFICACION PREVIA (0 = el SP original aun no incluye la formula)
select definition_has_union = case when OBJECT_DEFINITION(OBJECT_ID('SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO')) like '%ProcesoProduccionDetalle%' then 1 else 0 end
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/07/28
Objetivo		Consulta indicador de merma
status			200 = ok
				-1	= error

Modificado		Incluye el consumo de MPL de la produccion por formula (ProcesoProduccionDetalle) en el calculo mensual.
*/

CREATE OR ALTER proc [dbo].[SP_CONSULTA_COSTO_PRODUCCION]

/*@idInventarioFisico int=null,
@idLinea int=null,
@idAlmacen int=null*/
@mesCalculo int=null,
@anioCalculo int=null,
@idLinea int=null,
@idAlmacen int=null,
@silent int=0

as

	begin -- procedimiento

		begin try -- try principal

			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit),
						@ultimoDiaMesCalculo date,
						@ultimoDiaMesAnterior date,
						@primerDiaMesCalculo date,
						@fechaActual datetime


			end -- inicio

			begin--asignamos variables

			select @fechaActual=dbo.FechaActual()

			select @mesCalculo=coalesce(@mesCalculo,month(@fechaActual)),@anioCalculo=coalesce(@anioCalculo,year(@fechaActual))

			--construimos la fecha de calculo en base al mes y anio y el primer dia
			select @primerDiaMesCalculo=DATEFROMPARTS(@anioCalculo,@mesCalculo, 01 );

			--obtenemos el ultimo dia mes calculo, si la fecha actual es menos al ultimo dia del mes, se toma la fecha actual
			select @ultimoDiaMesCalculo=case when cast(@fechaActual as date)< EOMONTH(@primerDiaMesCalculo) THEN cast(@fechaActual as date) ELSE EOMONTH(@primerDiaMesCalculo) END,
				   @ultimoDiaMesAnterior=EOMONTH(@primerDiaMesCalculo,-1) --obtenemos el ultimo dia del mes anterior


			--si el mes calculado es el actual se elimina registro de reporte merma para que se vuelva a calcular,debido a que el mes todavia no se termina
			IF (MONTH(@ultimoDiaMesCalculo)=MONTH(@fechaActual) and YEAR(@ultimoDiaMesCalculo)=YEAR(@fechaActual))
			BEGIN
			    print 'entre'
				--select * from  ReporteMerma where MONTH(UltimoDiaMesCalculo)=MONTH(@fechaActual) and YEAR(UltimoDiaMesCalculo)=YEAR(@fechaActual)
				DELETE ReporteCostoProduccion where MONTH(UltimoDiaMesCalculo)=MONTH(@fechaActual) and YEAR(UltimoDiaMesCalculo)=YEAR(@fechaActual)
			END

			end


			begin

				--si no existe el calculo en la tabla reporteMerma realizamos calculo
				IF NOT EXISTS(select 1 from ReporteCostoProduccion where UltimoDiaMesCalculo=@ultimoDiaMesCalculo)
				begin
					print 'entre'
				    --- [ReporteCostoProduccion]


					INSERT INTO ReporteCostoProduccion(idProducto
														,cantidadSolicitadaMesAnt
														,cantidadAceptadaFinalMesAnt
														--,totalCompras
														--,cantidadCostoProduccion
														--,porcCostoProduccion
														,ultCostoCompra
														--,costoProduccionMerma
														,ultimoDiaMesCalculo
														,ultimoDiaMesAnterior
														,fechaAlta)
					select
						C.idProducto,
						dbo.redondear(sum(C.cantidad)),
						dbo.redondear(sum(C.cantidadAceptada)),
						--dbo.redondear(sum(PPA.cantidadAceptada)),
						dbo.obtenerPrecioCompra(C.idProducto,@ultimoDiaMesCalculo),
						@ultimoDiaMesCalculo,
						@ultimoDiaMesAnterior,
						@fechaActual
					from (
						-- consumo de MPL por produccion a granel (flujo existente)
						select PPA.idProducto, PPA.cantidad, PPA.cantidadAceptada
						from ProcesoProduccionAgranel PPA
						join Productos P on P.idProducto=PPA.idProducto
						where
							PPA.idEstatusProduccionAgranel >=3
							and cast(PPA.fechaAlta as date)>=@primerDiaMesCalculo and cast(PPA.fechaAlta as date)<=@ultimoDiaMesCalculo
						union all
						-- consumo de MPL por produccion por formula (equivale a estatus 3: solicitada = aceptada = consumida)
						select PPD.idProducto, PPD.cantidadConsumida, PPD.cantidadConsumida
						from ProcesoProduccionDetalle PPD
						join ProcesoProduccion PP on PP.idProcesoProduccion=PPD.idProcesoProduccion
						join Productos P on P.idProducto=PPD.idProducto
						where
							cast(PP.fechaAlta as date)>=@primerDiaMesCalculo and cast(PP.fechaAlta as date)<=@ultimoDiaMesCalculo
					) C
					group by C.idProducto


					--actualizamos inventarioSistema (inventario final mes anterior + total de compras)
					--update ReporteMerma set inventarioSistema=dbo.redondear(inventarioFinalMesAnt + totalCompras) where UltimoDiaMesCalculo=@ultimoDiaMesCalculo

					--calculamos porcentaje merma (merma/inventario sistema)
					update ReporteCostoProduccion set porcCostoProduccion= dbo.redondear((cantidadAceptadaFinalMesAnt * ultCostoCompra) / 100)  where @ultimoDiaMesCalculo=@ultimoDiaMesCalculo

					--calculamos costoMerma (merma *  ultimo costo compra)
					update ReporteCostoProduccion set costoProduccionMerma=dbo.redondear(cantidadAceptadaFinalMesAnt * ultCostoCompra) where UltimoDiaMesCalculo=@ultimoDiaMesCalculo


				end
				SELECT r.*,p.codigoBarras,p.descripcion descripcionProducto,l.idLineaProducto,l.descripcion descripcionLinea
				INTO #MERMA
				FROM ReporteCostoProduccion r
				join productos p on r.idProducto=p.idProducto
				join LineaProducto l on p.idLineaProducto=l.idLineaProducto
				where UltimoDiaMesCalculo=@ultimoDiaMesCalculo
				and p.idLineaProducto=coalesce(@idLinea,p.idLineaProducto)
				and 1=case when coalesce(@idAlmacen,0)>0 then dbo.ExisteProductoEnAlmancen(@idAlmacen,r.idProducto) else 1 end

				if not exists (select 1 from #MERMA)
				begin
					select	@valido = cast(0 as bit),
							@status = -1,
							@error_message = 'No se encontraron resultados.'
				end


			end


		end try -- try principal

		begin catch -- catch principal

			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@error_message = error_message(),
					@error_severity =
						case error_severity()
							when 11 then 'Error en validación'
							when 12 then 'Error en consulta'
							when 13 then 'Error en actualización'
							else 'Error general'
						end

		end catch -- catch principal

		begin -- reporte de estatus

		    if(@silent=0)
			begin

				select	@status status,
						@error_procedure error_procedure,
						@error_line error_line,
						@error_severity error_severity,
						@error_message mensaje

				if(@valido=1)
				 select * from #MERMA
				 --truncate table ReporteCostoProduccion
			end




		end -- reporte de estatus

	end -- procedimiento
GO

/*

Objetivo	Consulta detalle (item por item, sin agrupar) de ProcesoProduccionAgranel,
			calculando el costo de produccion por renglon y el ultimo costo de compra
			del producto (mas reciente, sin importar la fecha del renglon).
			Filtra solo por rango de fecha (fechaAlta) y idEstatusProduccionAgranel.
			Incluye (UNION ALL) el consumo de MPL de la produccion por formula, con las columnas
			origen e idProcesoProduccion al final.
status		200 = ok
			-1  = error

*/

CREATE OR ALTER proc [dbo].[SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO]

	@fechaIni datetime = null,
	@fechaFin datetime = null,
	@idEstatusProduccionAgranel int = null

as

	begin -- procedimiento
		begin -- inicio

			select 200 status, 'se encontraron resultados'

			SELECT X.*
			FROM (
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
					dbo.redondear(r.cantidadAceptada * dbo.obtenerPrecioCompra(r.idProducto, null)) costoProduccion,
					cast('Consumo MPL' as varchar(30)) origen,
					cast(null as bigint) idProcesoProduccion
				FROM ProcesoProduccionAgranel r
				join Productos p on r.idProducto = p.idProducto
				join CatEstatusProcesoAgranel EPA on EPA.idEstatusProcesoAgranel = r.idEstatusProduccionAgranel
				where
					cast(r.fechaAlta as date) >= cast(coalesce(@fechaIni, r.fechaAlta) as date)
					and cast(r.fechaAlta as date) <= cast(coalesce(@fechaFin, r.fechaAlta) as date)
					and r.idEstatusProduccionAgranel = coalesce(@idEstatusProduccionAgranel, r.idEstatusProduccionAgranel)

				UNION ALL

				-- consumo de MPL por produccion por formula: equivale a estatus 3 "Procesado correctamente"
				SELECT
					cast(null as bigint),
					d.idProducto,
					p.codigoBarras,
					p.descripcion,
					d.cantidadConsumida,
					d.cantidadConsumida,
					cast(0 as float),
					pp.fechaAlta,
					pp.fechaAlta,
					3,
					EPA.descripcion,
					dbo.obtenerPrecioCompra(d.idProducto, null),
					dbo.redondear(d.cantidadConsumida * dbo.obtenerPrecioCompra(d.idProducto, null)),
					cast('Produccion por formula' as varchar(30)),
					pp.idProcesoProduccion
				FROM ProcesoProduccionDetalle d
				join ProcesoProduccion pp on pp.idProcesoProduccion = d.idProcesoProduccion
				join Productos p on p.idProducto = d.idProducto
				join CatEstatusProcesoAgranel EPA on EPA.idEstatusProcesoAgranel = 3
				where
					cast(pp.fechaAlta as date) >= cast(coalesce(@fechaIni, pp.fechaAlta) as date)
					and cast(pp.fechaAlta as date) <= cast(coalesce(@fechaFin, pp.fechaAlta) as date)
					and 3 = coalesce(@idEstatusProduccionAgranel, 3)
			) X
			order by X.fechaAlta desc

		end -- inicio
	end -- procedimiento
GO

-- VERIFICACION POSTERIOR (1 = ya incluye la formula)
select definition_has_union = case when OBJECT_DEFINITION(OBJECT_ID('SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO')) like '%ProcesoProduccionDetalle%'
	and OBJECT_DEFINITION(OBJECT_ID('SP_CONSULTA_COSTO_PRODUCCION')) like '%ProcesoProduccionDetalle%' then 1 else 0 end
GO
