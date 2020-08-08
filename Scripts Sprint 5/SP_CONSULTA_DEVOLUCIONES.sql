use DB_A57E86_lluviadesarrollo
go

-- se crea procedimiento SP_CONSULTA_DEVOLUCIONES
if exists (select * from sysobjects where name like 'SP_CONSULTA_DEVOLUCIONES' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_CONSULTA_DEVOLUCIONES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta las devoluciones de productos en las ventas
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_DEVOLUCIONES

	@idVenta		int = null
	
as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit)


			end  --declaraciones 

			begin -- principal
				
				
				select	*
				into	#VentasDetalle
				from	VentasDetalle 
				where	idVenta = case
										when @idVenta is null then idVenta
										when @idVenta = 0 then idVenta
										else @idVenta
								  end
					and	productosDevueltos > 0
				order by idVenta


				if not exists (	select 1 from #VentasDetalle )
				begin
					select	@mensaje = 'No existen devoluciones para esos terminos de búsqueda.',
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

		--reporte de estatus
			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
							

		-- si todo ok
			if exists ( select 1 from #VentasDetalle )
			begin
				select	idVentaDetalle
						idVenta,
						idProducto,
						cantidad,
						contadorProductosPorPrecio,
						monto,
						cantidadActualInvGeneral,
						cantidadAnteriorInvGeneral,
						precioIndividual,
						precioMenudeo,
						precioRango,
						precioVenta,
						montoIva,
						idEstatusProductoVenta,
						productosDevueltos
				from	#VentasDetalle
			end

				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_DEVOLUCIONES to public
go



