-- se crea procedimiento SP_CONSULTA_DEVOLUCIONES_PEDIDOS_ESPECIALESV2
if exists (select * from sysobjects where name like 'SP_CONSULTA_MESES' and xtype = 'p' )
	drop proc SP_CONSULTA_MESES
go

/*

Autor			Jessica Almonte Acosta
Fecha			2022/02/17
Objetivo		Consulta meses

*/

create proc SP_CONSULTA_MESES
@anio int=null
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
					    @fecha					datetime,
						@fechaActual            datetime


			select @fechaActual=dbo.FechaActual();
            --obtenemos el primero dia del año
			select @fecha=DATEADD(yy, DATEDIFF(yy, 0, @fechaActual), 0);

			--si anio es null se obtiene el actual
			select @anio=coalesce(@anio,year(@fechaActual))

			end  --declaraciones 

			begin -- principal			

				with meses as
				(
				select cast(1 as int) id, @fecha  fecha
				union all
				select cast((id) + 1 as int),dateadd(mm, 1, fecha)
				from   meses
				where  id < 12
				)
				select id Value,DATENAME(mm,fecha) Text
				INTO #MESES
				from   meses mes
				order by id ASC
				option (maxrecursion 12)

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
				SELECT *,case when  Value=(case when @anio=year(@fechaActual) then MONTH(@fechaActual) else 12 end) then cast(1 as bit) else cast(0 as bit) end Selected  FROM #MESES 
				where Value<=case when @anio=year(@fechaActual) then MONTH(@fechaActual) else 12 end  --si el año es igual al año actual se obtiene hasta el mes actual si no se consultan todos los meses
			 order by Value desc
			end

				
		end -- reporte de estatus


	end  -- principal
go

grant exec on SP_CONSULTA_MESES to public
go
