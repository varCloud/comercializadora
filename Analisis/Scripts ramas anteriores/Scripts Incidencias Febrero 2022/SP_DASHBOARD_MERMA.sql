-- se crea procedimiento SP_DASHBOARD_MERMA
if exists (select * from sysobjects where name like 'SP_DASHBOARD_MERMA' and xtype = 'p' )
	drop proc SP_DASHBOARD_MERMA
go

/*

Autor			Jessica Almonte Acosta
Fecha			2022/02/17
Objetivo		Consulta merma del mes actual y mes anterior

*/

create proc SP_DASHBOARD_MERMA
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
				EXEC SP_CONSULTA_MERMA @mesCalculo=@mesCalculo,
				@anioCalculo=@anioCalculo,
				@idLinea=null,
				@idAlmacen=null,
				@silent=1

				--validamos si no existe calculo de merma del mes anterior calculamos la merma del mes anterior
				IF NOT EXISTS(select 1 from ReporteMerma where UltimoDiaMesCalculo=@ultimoDiaMesAnterior)
				begin				
				select @mesCalculo=MONTH(@ultimoDiaMesAnterior),@anioCalculo=YEAR(@ultimoDiaMesAnterior);
				EXEC SP_CONSULTA_MERMA @mesCalculo=@mesCalculo,
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
				coalesce(sum(Merma),0) totalMerma,
				coalesce(sum(porcMerma),0) totalPorcMerma,
				coalesce(sum(costoMerma),0) totalCostoMerma,
				dbo.redondear(coalesce(sum(Merma)/count(1),0)) promedioMerma,
				dbo.redondear(coalesce(sum(porcMerma)/count(1),0)) promedioPorcMerma,
				dbo.redondear(coalesce(sum(costoMerma)/count(1),0)) promedioCostoMerma
				from ReporteMerma where UltimoDiaMesCalculo=@ultimoDiaMesActual
				UNION
				select 'Mes Anterior' descripcion ,@ultimoDiaMesAnterior UltimoDiaMesCalculo,
				coalesce(sum(Merma),0) totalMerma,
				coalesce(sum(porcMerma),0) totalPorcMerma,
				coalesce(sum(costoMerma),0) totalCostoMerma,
				dbo.redondear(coalesce(sum(Merma)/count(1),0)) promedioMerma,
				dbo.redondear(coalesce(sum(porcMerma)/count(1),0)) promedioPorcMerma,
				dbo.redondear(coalesce(sum(costoMerma)/count(1),0)) promedioCostoMerma
				from ReporteMerma where UltimoDiaMesCalculo=@ultimoDiaMesAnterior
			end

				
		end -- reporte de estatus


	end  -- principal
go

grant exec on SP_DASHBOARD_MERMA to public
go
