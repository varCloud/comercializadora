use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_OBTIENE_CODIGO_ARTICULO
if exists (select * from sysobjects where name like 'SP_OBTIENE_CODIGO_ARTICULO' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_OBTIENE_CODIGO_ARTICULO
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		obtiene el codigo de artucilo sugerido en el alta de un producto
status			200 = ok
				-1	= error
*/

create proc SP_OBTIENE_CODIGO_ARTICULO

	@descripcion		varchar(255),
	@idLineaProducto	int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@prefijo				varchar(10) = '',
						@desc					varchar(10) = '',
						@totPalabras			int = 0,
						@valido					bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				--if not exists ( select 1 from CatUnidadMedida )
				--	begin
				--		select @mensaje = 'No existen unidades de medida registradas.'
				--		raiserror (@mensaje, 11, -1)
				--	end
				--else
				--	begin
				--		select @valido = cast(1 as bit)
				--	end

				select @prefijo = substring( (ltrim(rtrim(replace(descripcion, 'Linea', '')))), 1,2) + '-' from LineaProducto where idLineaProducto = @idLineaProducto 
				
				---- total de palabras
					create table #palabras (id int identity(1,1), letra varchar(50))

					DECLARE @xml xml, @str varchar(100), @delimiter varchar(10)
					SET @str = @descripcion
					SET @delimiter = ' '
					SET @xml = cast(('<X>'+replace(@str, @delimiter, '</X><X>')+'</X>') as xml)

					insert into #palabras (letra)
					SELECT C.value('.', 'varchar(10)') as value FROM @xml.nodes('X') as X(C)

					--select * from #palabras
					--drop table #palabras
					
					select @totPalabras = count(1) from #palabras

				if ( @totPalabras = 1 )
					begin
						select @desc = @prefijo +
							(select substring(letra, 1,3) from #palabras where id = 1)
					end
				
				if ( @totPalabras = 2 )
					begin
						select @desc = @prefijo +
							(select substring(letra, 1,1) from #palabras where id = 1)+
							(select substring(letra, 1,2) from #palabras where id = 2)
					end

				if ( @totPalabras >= 3 )
					begin
						select @desc = @prefijo +
							(select substring(letra, 1,1) from #palabras where id = 1)+
							(select substring(letra, 1,1) from #palabras where id = 2)+
							(select substring(letra, 1,1) from #palabras where id = 3)
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

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje as  mensaje

			select	UPPER(@desc) as articulo

			
							
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_OBTIENE_CODIGO_ARTICULO to public
go



