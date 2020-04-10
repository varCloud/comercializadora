use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_CONSULTA_TICKET
if exists (select * from sysobjects where name like 'SP_CONSULTA_TICKET' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_CONSULTA_TICKET
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los datos del ticket del idVenta
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_TICKET
	@idVenta		int
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


			if not exists ( 
							select 1 from Ventas where idVenta = @idVenta and idStatusVenta = 1
						  )
			begin
				select	@mensaje = 'No existe la venta seleccionada.',
						@status = -1,
						@valido = cast(0 as bit)
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
			if ( @valido = cast(1 as bit) )
				begin
					
					select	v.idVenta, d.idProducto, d.cantidad, d.contadorProductosPorPrecio, d.monto as monto,
							p.descripcion as descProducto, 
							v.idCliente, 
							case
								when c.nombres is null then 'PÚBLICO EN GENERAL' 
								else c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno
							end as nombreCliente,
							u.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
							d.cantidadActualInvGeneral, d.cantidadAnteriorInvGeneral, v.fechaAlta							 
					from	Ventas v 
								inner join VentasDetalle d
									on v.idVenta = d.idVenta
								left join Clientes c
									on c.idCliente = v.idCliente
								inner join Usuarios u
									on u.idUsuario = v.idUsuario
								inner join Productos p
									on p.idProducto = d.idProducto
								inner join ProductosPorPrecio pp
									on pp.contador = contadorProductosPorPrecio
					where	v.idVenta = @idVenta
						and d.cantidad between pp.min and pp.max
							--and pp.activo = cast(1 as bit)

				end

				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_TICKET to public
go



