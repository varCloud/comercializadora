--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_MERMA]    Script Date: 14/09/2022 10:46:12 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/07/28
Objetivo		Consulta indicador de merma
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CONSULTA_COSTO_PRODUCCION]

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
						PPA.idProducto,
						dbo.redondear(sum(PPA.cantidad)),
						dbo.redondear(sum(PPA.cantidadAceptada)),
						--dbo.redondear(sum(PPA.cantidadAceptada)),
						dbo.obtenerPrecioCompra(PPA.idProducto,@ultimoDiaMesCalculo),
						@ultimoDiaMesCalculo,
						@ultimoDiaMesAnterior,
						@fechaActual
					from ProcesoProduccionAgranel PPA
					join Productos P on P.idProducto=PPA.idProducto
					where 
						PPA.idEstatusProduccionAgranel >=3 
						and cast(PPA.fechaAlta as date)>=@primerDiaMesCalculo and cast(PPA.fechaAlta as date)<=@ultimoDiaMesCalculo
					group by PPA.idProducto

					
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
	
