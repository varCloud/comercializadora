use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_RANGOS_PRECIOS
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_RANGOS_PRECIOS' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_INSERTA_ACTUALIZA_RANGOS_PRECIOS
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		inserta y actualiza los diferentes rangos de precios del sistema
status			200 = ok
				-1	= error
*/

create proc SP_INSERTA_ACTUALIZA_RANGOS_PRECIOS

  @xml AS XML

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Rango de precios actualizado correctamente',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@idProducto				int = 0

						create table
							#contadores
								(
									id			int identity(1,1),
									contador	int
								)

						create table
							#mins
								(
									id			int identity(1,1),
									min_		int
								)

						create table
							#maxs
								(
									id			int identity(1,1),
									max_		int
								)

						create table
							#precios
								(
									id			int identity(1,1),
									costo		money
								)

			end  --declaraciones 

			begin -- principal

				insert into #contadores (contador)
				SELECT Precio.contador.value('.','NVARCHAR(200)') AS contadores FROM @xml.nodes('//contador') as Precio(contador) 

				insert into #mins (min_)
				SELECT Precio.min.value('.','NVARCHAR(200)') AS mins FROM @xml.nodes('//min') as Precio(min)

				insert into #maxs (max_)
				SELECT Precio.max.value('.','NVARCHAR(200)') AS mins FROM @xml.nodes('//max') as Precio(max)

				insert into #precios (costo)
				SELECT Precio.costo.value('.','NVARCHAR(200)') AS costos FROM @xml.nodes('//costo') as Precio(costo)

				select  @idProducto = ( SELECT top 1  Precio.idProducto.value('.','NVARCHAR(200)') AS idProducto FROM @xml.nodes('//idProducto') as Precio(idProducto))


				if exists ( select 1 from ProductosPorPrecio where idProducto = @idProducto )
					begin
						update	ProductosPorPrecio 
						set		activo = cast(0 as bit)
						where	idProducto = @idProducto
					end

				insert into ProductosPorPrecio (idProducto,min,max,costo, activo)
				select	@idProducto as idProducto, m.min_, x.max_, p.costo, cast(1 as bit)
				from	#contadores c
							inner join #mins m
								on m.id = c.id
							inner join #maxs x
								on x.id = c.id			
							inner join #precios p
								on p.id = c.id

				drop table #contadores
				drop table #mins
				drop table #maxs
				drop table #precios


				
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

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje

		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_INSERTA_ACTUALIZA_RANGOS_PRECIOS to public
go

