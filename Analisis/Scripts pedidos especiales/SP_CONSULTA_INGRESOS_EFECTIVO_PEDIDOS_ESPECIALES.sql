IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_CONSULTA_INGRESOS_EFECTIVO_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_CONSULTA_INGRESOS_EFECTIVO_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consulta los ingresos de efectivo de los pedidos especiales
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_CONSULTA_INGRESOS_EFECTIVO_PEDIDOS_ESPECIALES]

@idTipoIngreso				int=null,
@fecha						datetime = null,
@idIngresoPedidoEspecial	int = null,
@idUsuario					int = null,
@idAlmacen					int = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',						 
						@diasDevoluciones int=0,
						@fechaActual date

			end  --declaraciones 

			begin -- principal			
				
			
					select @fecha = coalesce(@fecha, dbo.FechaActual())
					select @fecha = cast(@fecha as date)

					select	idIngresoPedidoEspecial,monto,r.fechaAlta,r.idUsuario,
							u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,							
						    u.idSucursal, cast(cs.descripcion as varchar(200)) as Sucursal,
							u.idAlmacen, cast(al.Descripcion as varchar(200)) as Almacen,
							idTipoIngreso
					INTO #INGRESOS_EFECTIVO
					from	PedidosEspecialesIngresosEfectivo R
								inner join Usuarios u 
									on u.idUsuario = r.idUsuario
								inner join CatSucursales cs on cs.idSucursal = u.idSucursal
								inner join Almacenes al on al.idAlmacen = u.idAlmacen								
					where		
						r.idIngresoPedidoEspecial =  coalesce(@idIngresoPedidoEspecial,r.idIngresoPedidoEspecial)
						and r.idTipoIngreso=coalesce(@idTipoIngreso,r.idTipoIngreso)
						and r.idUsuario =  coalesce(@idUsuario,r.idUsuario)
						and al.idAlmacen =  coalesce(@idAlmacen,al.idAlmacen)
						and cast(r.fechaAlta as date) = cast(@fecha as date)
					order by R.fechaAlta


				if not exists ( select 1 from #INGRESOS_EFECTIVO)
				begin
					select @mensaje = 'No Existen Ingresos de efectivo el dia de hoy.'
					raiserror (@mensaje, 11, -1)
				end


			end -- principal

		end try

		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@mensaje = error_message()
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
           
		    if(@status=200)
				select * from #INGRESOS_EFECTIVO order by fechaAlta desc
			
					
		end -- reporte de estatus
		

	end  -- principal
