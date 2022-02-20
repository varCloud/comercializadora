-- se crea procedimiento SP_CONSULTA_ANIOS
if exists (select * from sysobjects where name like 'SP_CONSULTA_ANIOS' and xtype = 'p' )
	drop proc SP_CONSULTA_ANIOS
go

/*

Autor			Jessica Almonte Acosta
Fecha			2022/02/17
Objetivo		Consulta meses

*/

create proc SP_CONSULTA_ANIOS
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
						@fecha                  datetime,
					    @yearIni                int=2020 --año inicio

                select  @fecha=DATEADD(year, DATEDIFF(YEAR,0,(DATEADD(YEAR,-9, dbo.FechaActual()))), 0);

			end  --declaraciones 

			begin -- principal			

				with years as
				(
				select cast(1 as int) id, @fecha  fecha
				union all
				select cast((id) + 1 as int),dateadd(year, 1, fecha)
				from   years
				where  id<10
				)
				select year(fecha) Value, year(fecha) Text
				INTO #ANIOS
				from   years 
				where year(fecha)>=@yearIni           
				order by id ASC
				option (maxrecursion 10)

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
				SELECT * FROM #ANIOS order by Value desc
			end

				
		end -- reporte de estatus


	end  -- principal
go

grant exec on SP_CONSULTA_ANIOS to public
go
