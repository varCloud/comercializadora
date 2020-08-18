use DB_A57E86_lluviadesarrollo
go

-- se crea procedimiento SP_CONSULTA_UBICACION
if exists (select * from sysobjects where name like 'SP_CONSULTA_UBICACION' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_CONSULTA_UBICACION
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/07/23
Objetivo		Consulta el piso/raq/pasillo 0 para todas la ubicaciones
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_UBICACION

	@idSucursal			int = null,
	@idAlmacen			int = null,
	@idPasillo			int = null,
	@idRaq				int = null,
	@idPiso				int = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''

						
			end  --declaraciones 


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
							

		---- si todo ok
		
			select	a.idSucursal, u.idAlmacen, a.Descripcion, u.idUbicacion, u.idAlmacen, u.idPasillo, u.idPiso, u.idRaq, a.idTipoAlmacen 
			from	Almacenes a
						inner join Ubicacion u 
							on u.idAlmacen = a.idAlmacen
			where	a.idSucursal =	case 
										when @idSucursal is null then a.idSucursal 
										when @idSucursal = 0 then a.idSucursal 
										else @idSucursal 
									end
				and	u.idAlmacen =	case	
										when @idAlmacen is null then u.idAlmacen 
										when @idAlmacen = 0 then u.idAlmacen 
										else @idAlmacen 
									end
				and	u.idPasillo =	case 
										when @idPasillo is null then u.idPasillo 
										when @idPasillo = 0 then u.idPasillo 
										else @idPasillo 
									end
				and	u.idRaq =	case 
										when @idRaq is null then u.idRaq 
										when @idRaq = 0 then u.idRaq 
										else @idRaq 
									end
				and	u.idPiso	 =	case 
										when @idPiso is null then u.idPiso	 
										when @idPiso = 0 then u.idPiso	 
										else @idPiso	 
									end
				and u.idPasillo <> 0
				and u.idRaq <> 0
				and u.idPiso <> 0
					


				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_UBICACION to public
go



