use DB_A57E86_lluviadesarrollo
go

-- se crea procedimiento SP_CONSULTA_COMPLEMENTOS_VENTA
if exists (select * from sysobjects where name like 'SP_CONSULTA_COMPLEMENTOS_VENTA' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_CONSULTA_COMPLEMENTOS_VENTA
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/07/23
Objetivo		Consulta productos agregados/devueltos de una venta
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_COMPLEMENTOS_VENTA

	@idVenta					int,
	@idEstatusProductoVenta		int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@existeComplemento		bit = cast(1 as bit)
						
			end  --declaraciones 


			if not exists ( 
							select	1 
							from	Ventas v 
										inner join VentasDetalle vd on v.idVenta = vd.idVenta
							where	v.idVenta = @idVenta 
								and v.idStatusVenta = 1 
								and vd.idEstatusProductoVenta = @idEstatusProductoVenta
						  )
			begin
				select	@mensaje = 'No existe el complemento de venta.',
						@status = -1,
						@existeComplemento = cast(0 as bit)
			end	

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
			if ( @existeComplemento = cast(1 as bit) )
				begin

					select	* 
					from	Ventas v
								inner join VentasDetalle vd
									on v.idVenta = vd.idVenta
					where	v.idVenta = @idVenta
						and	v.idStatusVenta = 1 
						and vd.idEstatusProductoVenta = @idEstatusProductoVenta
					
				end
				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_COMPLEMENTOS_VENTA to public
go



