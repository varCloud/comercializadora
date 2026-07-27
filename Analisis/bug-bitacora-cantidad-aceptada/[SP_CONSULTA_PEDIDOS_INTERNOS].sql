/*

Autor			Jessica Almonte Acosta
Fecha			2020/04/27
Objetivo		Consultar los pedidos internos

Actualizacion	2026/07/27 - se agrega pid.cantidadAceptada al resultado para poder
				distinguir la cantidad solicitada de la cantidad realmente aceptada
				en la pantalla de Bitacora (antes solo se regresaba pid.cantidad,
				que es la cantidad solicitada, nunca la aceptada).
*/

ALTER procedure [dbo].[SP_CONSULTA_PEDIDOS_INTERNOS]

@IdEstatusPedidoInterno int=NULL,
@idAlmancenOrigen int=NULL,
@idAlmacenDestino int=NULL,
@idUsuario int=NULL,
@idProducto int=NULL,
@fechaIni date=NULL,
@fechaFin date=NULL,
@idPedidoInterno int=NULL,
@idTipoPedidoInterno int = null

AS BEGIN

		begin try -- try principal

			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit),
						@top bigint=0x7fffffffffffffff--valor maximo

			end -- inicio

			begin

				--select @idTipoPedidoInterno = coalesce(@idTipoPedidoInterno, 1)
				--print @idTipoPedidoInterno

				if(	@IdEstatusPedidoInterno is null and
					@idAlmancenOrigen is null and
					@idAlmancenOrigen is null and
					@idUsuario is null and
					@idProducto is null and
					@fechaIni is null and
					@fechaFin is null and
					@idPedidoInterno is null and
					@idTipoPedidoInterno is null
					)

					begin
						select @top=50
					end

					SELECT top (@top)
					PI.idPedidoInterno,pi.fechaAlta,
					pi.idAlmacenOrigen,ao.Descripcion almacenOrigen,
					pi.idAlmacenDestino,ad.Descripcion almacenDestino,
					pi.idUsuario,coalesce(u.nombre,'') + ' ' + coalesce(u.apellidoPaterno,'') + ' ' + coalesce(u.apellidoMaterno,'') nombreCompleto,
					pi.IdEstatusPedidoInterno idStatus,s.descripcion,
					pid.idProducto,p.descripcion producto,pid.cantidad,pid.cantidadAceptada
					INTO #PEDIDOS_INTERNOS
					FROM PedidosInternos pi
					join PedidosInternosDetalle pid on pi.idPedidoInterno=pid.idPedidoInterno
					join CatEstatusPedidoInterno s on pi.IdEstatusPedidoInterno=s.IdEstatusPedidoInterno
					join Almacenes ao on pi.idAlmacenOrigen=ao.idAlmacen
					join Almacenes ad on pi.idAlmacenDestino=ad.idAlmacen
					join Usuarios u on pi.idUsuario=u.idUsuario
					join Productos p on pid.idProducto=p.idProducto
					where
					pi.idPedidoInterno=coalesce(@idPedidoInterno,pi.idPedidoInterno) and
					pi.IdEstatusPedidoInterno=coalesce(@IdEstatusPedidoInterno,pi.idestatuspedidointerno)
					and pi.idAlmacenOrigen=coalesce(@idAlmancenOrigen,pi.idAlmacenOrigen)
					and pi.idAlmacenDestino=coalesce(@idAlmacenDestino,pi.idAlmacenDestino)
					and pi.idUsuario=coalesce(@idUsuario,pi.idUsuario)
					and pid.idProducto=coalesce(@idProducto,pid.idProducto)
					and cast(pi.fechaAlta as date) >=coalesce(cast(@fechaIni as date),cast(pi.fechaAlta as date))
					and cast(pi.fechaAlta as date) <=coalesce(cast(@fechaFin as date),cast(pi.fechaAlta as date))
					and idTipoPedidoInterno = coalesce(@idTipoPedidoInterno, 1)
					order by fechaAlta desc

				if not exists (select 1 from #PEDIDOS_INTERNOS)
				begin
					select	@valido = cast(0 as bit),
							@status = -1,
							@error_message = 'No se encontraron pedidos internos con esos terminos de busqueda.'
				end

			end

		end try -- try principal

		begin catch -- catch principal

			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINAMICA'),
					@error_line = error_line(),
					@error_message = error_message(),
					@error_severity =
						case error_severity()
							when 11 then 'Error en validacion'
							when 12 then 'Error en consulta'
							when 13 then 'Error en actualizacion'
							else 'Error general'
						end

		end catch -- catch principal

		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@error_severity error_severity,
					@error_message mensaje

				select
			    idPedidoInterno,fechaAlta,
			    idAlmacenOrigen,idAlmacenOrigen idAlmacen,almacenOrigen Descripcion,
				idAlmacenDestino,idAlmacenOrigen idAlmacen,almacenDestino Descripcion,
				idUsuario,nombreCompleto,
				idStatus,descripcion,
				idProducto,producto descripcion,cantidad,cantidadAceptada
				from #PEDIDOS_INTERNOS
				order by fechaAlta desc

		end -- reporte de estatus

END
