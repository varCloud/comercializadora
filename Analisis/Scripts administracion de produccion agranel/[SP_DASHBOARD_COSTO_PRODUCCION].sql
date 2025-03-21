--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_DASHBOARD_MERMA]    Script Date: 14/09/2022 10:45:09 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
Fecha			2022/02/17
Objetivo		Consulta merma del mes actual y mes anterior

*/

CREATE proc [dbo].[SP_DASHBOARD_COSTO_PRODUCCION]
as

	begin -- principal

	set LANGUAGE Spanish;
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit),
					    @primerDiaMesActual		date,
						@ultimoDiaMesActual		date,
						@ultimoDiaMesAnterior	date,
						@fechaActual			datetime,
						@mesCalculo				int,
						@anioCalculo			int

            select @fechaActual=dbo.FechaActual()

			select @primerDiaMesActual=DATEADD(MM, DATEDIFF(MM,0,@fechaActual), 0)

			select  @ultimoDiaMesActual=cast(@fechaActual as date),
					@ultimoDiaMesAnterior=EOMONTH(@primerDiaMesActual,-1) --obtenemos el ultimo dia del mes anterior
			

			end  --declaraciones 

			begin -- principal			

				--calculamos la merma actual				
				select @mesCalculo=MONTH(@primerDiaMesActual),@anioCalculo=YEAR(@primerDiaMesActual);
				EXEC [SP_CONSULTA_COSTO_PRODUCCION] @mesCalculo=@mesCalculo,
				@anioCalculo=@anioCalculo,
				@idLinea=null,
				@idAlmacen=null,
				@silent=1

				--validamos si no existe calculo de merma del mes anterior calculamos la merma del mes anterior
				IF NOT EXISTS(select 1 from ReporteCostoProduccion where UltimoDiaMesCalculo=@ultimoDiaMesAnterior)
				begin				
				select @mesCalculo=MONTH(@ultimoDiaMesAnterior),@anioCalculo=YEAR(@ultimoDiaMesAnterior);
				EXEC [SP_CONSULTA_COSTO_PRODUCCION] @mesCalculo=@mesCalculo,
				@anioCalculo=@anioCalculo,
				@idLinea=null,
				@idAlmacen=null,
				@silent=1
				end

			end -- principal

		end try

		begin catch 
		
			-- captura del error
			select	@status =			-error_state(),
					@error_procedure =	error_procedure(),
					@error_line =		error_line(),
					@mensaje =			error_message()
					
		end catch

		begin -- reporte de estatus

		--reporte de estatus
			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
							

		-- si todo ok
			if (@status=200)
			begin
				select 'Mes Actual' descripcion ,@ultimoDiaMesActual UltimoDiaMesCalculo,
					coalesce(sum(cantidadAceptadaFinalMesAnt),0) totalCantidadAceptada,
					coalesce(sum(porcCostoProduccion),0) totalPorcCostoProduccion,
					coalesce(sum(costoProduccionMerma),0) totalCostoProduccion,
					dbo.redondear(coalesce(sum(cantidadAceptadaFinalMesAnt)/count(1),0)) promedioCantidadAceptada,
					dbo.redondear(coalesce(sum(porcCostoProduccion)/count(1),0)) promedioPorcCostoProduccion,
					dbo.redondear(coalesce(sum(costoProduccionMerma)/count(1),0)) promedioCostoProduccion
				from ReporteCostoProduccion where UltimoDiaMesCalculo=@ultimoDiaMesActual
				UNION
				select 'Mes Anterior' descripcion ,@ultimoDiaMesAnterior UltimoDiaMesCalculo,
					coalesce(sum(cantidadAceptadaFinalMesAnt),0) totalCantidadAceptada,
					coalesce(sum(porcCostoProduccion),0) totalPorcCostoProduccion,
					coalesce(sum(costoProduccionMerma),0) totalCostoProduccion,
					dbo.redondear(coalesce(sum(cantidadAceptadaFinalMesAnt)/count(1),0)) promedioCantidadAceptada,
					dbo.redondear(coalesce(sum(porcCostoProduccion)/count(1),0)) promedioPorcCostoProduccion,
					dbo.redondear(coalesce(sum(costoProduccionMerma)/count(1),0)) promedioCostoProduccion
				from ReporteCostoProduccion where UltimoDiaMesCalculo=@ultimoDiaMesAnterior
			end

				
		end -- reporte de estatus


	end  -- principal
