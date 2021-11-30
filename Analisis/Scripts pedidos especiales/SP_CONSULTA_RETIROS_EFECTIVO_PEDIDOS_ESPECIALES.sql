IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_CONSULTA_RETIROS_EFECTIVO_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_CONSULTA_RETIROS_EFECTIVO_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consultar los retiros de efectivo
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_CONSULTA_RETIROS_EFECTIVO_PEDIDOS_ESPECIALES]

@idEstacion			int=null,
@fecha				datetime = null,
@idRetiro			int = null,
@idUsuario			int = null,
@idAlmacen			int = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''
			end  --declaraciones 

			begin -- principal
				
				select @fecha = coalesce(@fecha, dbo.FechaActual())
				select @fecha = cast(@fecha as date)
			
				select	1 tipoRetiro,idRetiro,montoRetiro,r.idUsuario,
						u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
						r.idEstacion, e.nombre as nombreEstacion, CONVERT(VARCHAR(10),r.fechaAlta,103) + ' ' + CONVERT(VARCHAR(20),r.fechaAlta,114) fechaAlta,
						r.idUsuario idUsuarioAut,coalesce(uAut.nombre,'') + ' ' + coalesce(uAut.apellidoPaterno,'') + ' ' + coalesce(uAut.apellidoMaterno,'') as nombreUsuarioAut,
						u.idSucursal, cast(cs.descripcion as varchar(200)) as descripcionSucursal,
						u.idAlmacen, cast(al.Descripcion as varchar(200)) as descripcionAlmacen,
						r.idEstatusRetiro idStatus,c.descripcion 
						into #PedidosEspecialesRetirosExcesoEfectivo
				from	PedidosEspecialesRetirosExcesoEfectivo R
							inner join Usuarios u 
								on u.idUsuario = r.idUsuario
							inner join Estaciones e
								on e.idEstacion = r.idEstacion
							inner join CatEstatusRetiros c on r.idEstatusRetiro=c.idEstatusRetiro
							inner join CatSucursales cs on cs.idSucursal = u.idSucursal
							inner join Almacenes al on al.idAlmacen = u.idAlmacen
							left join Usuarios uAut on r.idUsuarioAut=uAut.idUsuario
				where	r.idEstacion = coalesce(@idEstacion,r.idEstacion)
					and	cast(r.fechaAlta as date) = cast(@fecha as date)
					and r.idRetiro =  coalesce(@idRetiro,r.idRetiro)
					and r.idUsuario =  coalesce(@idUsuario,r.idUsuario)
					and al.idAlmacen =  coalesce(@idAlmacen,al.idAlmacen)
				order by R.fechaAlta
					

				if not exists (select 1 from #PedidosEspecialesRetirosExcesoEfectivo)
				begin
					select	
							@status = -1,
							@mensaje = 'No se encontraron retiros de exceso de efectivo.'
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
				select * from #PedidosEspecialesRetirosExcesoEfectivo order by fechaAlta desc
			
					
		end -- reporte de estatus
		

	end  -- principal
