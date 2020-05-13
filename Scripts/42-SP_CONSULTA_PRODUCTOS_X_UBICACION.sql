use DB_A57E86_comercializadora
go

-- se crea procedimiento SP_CONSULTA_PRODUCTOS_X_UBICACION
if exists (select * from sysobjects where name like 'SP_CONSULTA_PRODUCTOS_X_UBICACION' and xtype = 'p' and db_name() = 'DB_A57E86_comercializadora')
	drop proc SP_CONSULTA_PRODUCTOS_X_UBICACION
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta un producto y su ubicacion 
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_PRODUCTOS_X_UBICACION

	@idProducto			int,
	@idAlmacen			int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit)

				create table
					#existencias
						(
							idInventarioDetalle			int, 
							idProducto					int, 
							cantidad					int, 
							fechaAlta					datetime, 
							fechaActualizacion			datetime, 
							Almacen						varchar(300), 
							Pasillo						varchar(300), 
							Raq							varchar(300),
							Piso						varchar(300)
						)

			end  --declaraciones 

			begin -- principal
				
				select @idAlmacen = 1
				
				insert into		
					#existencias
						(
							idInventarioDetalle,idProducto,cantidad,fechaAlta,fechaActualizacion,
							Almacen,Pasillo,Raq,Piso
						)
				select	i.idInventarioDetalle, i.idProducto, i.cantidad, i.fechaAlta, i.fechaActualizacion, 
						a.Descripcion as Almacen, p.descripcion as Pasillo, r.descripcion as Raq, ps.descripcion as Piso
				from	InventarioDetalle i
							inner join Ubicacion u
								on u.idUbicacion = i.idUbicacion
							inner join CatPasillo p
								on p.idPasillo = u.idPasillo
							inner join Almacenes a
								on a.idAlmacen = u.idAlmacen
							inner join CatRaq r
								on r.idRaq = u.idRaq
							inner join CatPiso ps
								on ps.idPiso = u.idPiso
				where	u.idAlmacen = @idAlmacen
					and	i.idProducto = case	
											when @idProducto = 0 then i.idProducto
											else @idProducto
										end
				order by i.idProducto
				
				-- si no existe
				if not exists ( select top 1 1 from #existencias)
					begin
						select	@mensaje = 'No hay existencias para este Producto',
								@valido = cast(0 as bit)
						raiserror (@mensaje, 11, -1)
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

			

			if ( @valido = cast(1 as bit) )
				begin
						
					select	*
					from	#existencias
					
				end
								
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_PRODUCTOS_X_UBICACION to public
go



