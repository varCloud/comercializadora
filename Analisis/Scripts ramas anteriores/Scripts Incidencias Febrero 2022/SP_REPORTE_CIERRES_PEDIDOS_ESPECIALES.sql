-- se crea procedimiento SP_REPORTE_CIERRES_PEDIDOS_ESPECIALES
if exists (select * from sysobjects where name like 'SP_REPORTE_CIERRES_PEDIDOS_ESPECIALES' and xtype = 'p' )
	drop proc SP_REPORTE_CIERRES_PEDIDOS_ESPECIALES
go

/*

Autor			Jessica Almonte Acosta
Fecha			2022/02/17
Objetivo		Consulta reporte de cierres de pedidos especiales

*/

create proc SP_REPORTE_CIERRES_PEDIDOS_ESPECIALES

    @idUsuario			int=null,	
	@fechaIni			date = null,
	@fechaFin			date = null
as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''
						

			end  --declaraciones 

			--begin -- principal
			 --   if(@idUsuario is null)
				--begin
				--	select @fechaIni = coalesce(@fechaIni, cast(dbo.FechaActual() as date))
				--	select @fechaFin = coalesce(@fechaFin, cast(dbo.FechaActual() as date))
				--end

				
				select	
				p.idCierrePedidoEspecial,
				p.fechaAlta fechaCierre,
				u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno nombreUsuario,
				d.VentasContado,
				d.VentasTC,
				d.VentasTransferencias,
				d.VentasOtrasFormasPago,
				d.VentasCredito,
				d.MontoDevoluciones,
				p.MontoIngresosEfectivo,
				p.MontoRetirosEfectivo,
				p.MontoCierreEfectivo,
				p.MontoCierreTC,
				COALESCE(p.EfectivoEntregadoEnCierre,0) EfectivoEntregadoEnCierre,
				p.noDevoluciones,
				p.NoTicketsEfectivo,
				p.NoTicketsCredito,
				p.NoPedidosEnResguardo
				INTO #PedidosEspecialesCierres
				from	PedidosEspecialesCierres p
							inner join Usuarios u 
								on u.idUsuario = p.idUsuario
						   inner join (select idCierrePedidoEspecial,SUM(VentasContado) VentasContado,SUM(VentasTC) VentasTC,SUM(VentasTransferencias) VentasTransferencias,SUM(VentasOtrasFormasPago) VentasOtrasFormasPago,sum(VentasCredito) VentasCredito,sum(MontoDevoluciones)  MontoDevoluciones
						   from pedidosespecialescierresdetalle group by idCierrePedidoEspecial) d on d.idCierrePedidoEspecial=p.idCierrePedidoEspecial			
				where	
				p.idEstatusRetiro=1 and
				u.idUsuario = coalesce(@idUsuario,u.idUsuario) 
					and cast(p.fechaAlta as date) >= coalesce(cast(@fechaIni as date),cast(p.fechaAlta as date)) 
					and cast(p.fechaAlta as date) <=coalesce(cast(@fechaFin as date),cast(p.fechaAlta as date))

				if not exists (select 1 from #PedidosEspecialesCierres)
				begin
					select @mensaje = 'No se encontraron cierres de pedidos especiales con esos términos de búsqueda.'
					raiserror (@mensaje, 11, -1)
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

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje as  mensaje

			select	* from	#PedidosEspecialesCierres 			
			order by fechaCierre
								
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_REPORTE_CIERRES_PEDIDOS_ESPECIALES to public
go
