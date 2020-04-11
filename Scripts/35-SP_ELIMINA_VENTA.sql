use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_ELIMINA_VENTA
if exists (select * from sysobjects where name like 'SP_ELIMINA_VENTA' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_ELIMINA_VENTA
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		elimina la venta seleccionada
status			200 = ok
				-1	= error
*/

create proc SP_ELIMINA_VENTA

	@idVenta		int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Venta eliminada correctamente.',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit)

			end  --declaraciones 

			begin -- principal
				
				if not exists ( select 1 from Ventas where idVenta = @idVenta )
					begin
						select @mensaje = 'No Existe la venta seleccionada.'
						raiserror (@mensaje, 11, -1)
						select @valido = cast(0 as bit)
					end

				-- se regresa el inventario general

				update	Ventas 
				set		idStatusVenta = 2
				where	idVenta = @idVenta
				
				update	InventarioGeneral
				set		cantidad = a.cantidadSumada,
						fechaUltimaActualizacion = getdate()
				from	(

							select	v.idVenta, v.idProducto, i.cantidad + v.cantidad as cantidadSumada 
							from	VentasDetalle v
										inner join InventarioGeneral i
											on v.idProducto = i.idProducto
							where	v.idVenta = @idVenta
						) A
				where	InventarioGeneral.idProducto = A.idProducto

				-- se actualiza el status

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

			--if ( @idVenta = cast(1 as bit) )
			--	begin
					
			--	end

				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_ELIMINA_VENTA to public
go



