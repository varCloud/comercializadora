use DB_A57E86_lluviadesarrollo
go

-- se crea procedimiento SP_CONSULTA_PEDIDOS_ESPECIALES
if exists (select * from sysobjects where name like 'SP_CONSULTA_PEDIDOS_ESPECIALES' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_CONSULTA_PEDIDOS_ESPECIALES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los pedidos especiales del sistema
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_PEDIDOS_ESPECIALES

	@IdEstatusPedidoInterno int=NULL,
	@idAlmancenOrigen int=NULL,
	@idAlmacenDestino int=NULL,
	@idUsuario int=NULL,
	@fechaIni date=NULL,
	@fechaFin date=NULL,
	@idPedidoInterno int=NULL,
	@idTipoPedidoInterno int = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit),
						@top					bigint = 0x7fffffffffffffff --valor màximo


			end  --declaraciones 

			begin -- principal
				
				-- validaciones
					if	(	
							@IdEstatusPedidoInterno is null and
							@idAlmancenOrigen is null and
							@idAlmacenDestino is null and
							@idUsuario is null and
							@fechaIni is null and
							@fechaFin is null and
							@idPedidoInterno is null and
							@idTipoPedidoInterno is null 
						)
						begin
							select @top=50
						end

				
					SELECT	top (@top) 
							pe.idPedidoInterno, pe.fechaAlta, pe.idTipoPedidoInterno,
							pe.idAlmacenOrigen,ao.Descripcion almacenOrigen,
							pe.idAlmacenDestino,ad.Descripcion almacenDestino,
							pe.idUsuario,coalesce(u.nombre,'') + ' ' + coalesce(u.apellidoPaterno,'') + ' ' + coalesce(u.apellidoMaterno,'') nombreCompleto,
							pe.IdEstatusPedidoInterno idStatus,s.descripcion as descripcionEstatus,
							pe.descripcion as descripcionPedido,
							cast(0 as int) as cantidad
					INTO	#pedidosEspeciales
					FROM	PedidosInternos pe
								join CatEstatusPedidoInterno s 
									on pe.IdEstatusPedidoInterno=s.IdEstatusPedidoInterno
								join Almacenes ao 
									on pe.idAlmacenOrigen=ao.idAlmacen
								join Almacenes ad 
									on pe.idAlmacenDestino=ad.idAlmacen
								join Usuarios u 
									on pe.idUsuario=u.idUsuario
					where	pe.idPedidoInterno = coalesce(@idPedidoInterno,pe.idPedidoInterno) 
						and	pe.IdEstatusPedidoInterno = coalesce(@IdEstatusPedidoInterno,pe.idestatuspedidointerno)
						and pe.idAlmacenOrigen = coalesce(@idAlmancenOrigen,pe.idAlmacenOrigen)
						and pe.idAlmacenDestino = coalesce(@idAlmacenDestino,pe.idAlmacenDestino)
						and pe.idUsuario = coalesce(@idUsuario,pe.idUsuario)
						and cast(pe.fechaAlta as date) >=coalesce(cast(@fechaIni as date),cast(pe.fechaAlta as date))
						and cast(pe.fechaAlta as date) <=coalesce(cast(@fechaFin as date),cast(pe.fechaAlta as date))
						and pe.idTipoPedidoInterno = coalesce(@idTipoPedidoInterno, pe.idTipoPedidoInterno)
					order by fechaAlta desc

					
					update	#pedidosEspeciales
					set		cantidad = a.cantidad
					from	(
								select	pe.idPedidoInterno, sum( pid.cantidad) as cantidad
								from	#pedidosEspeciales pe
											inner join PedidosInternosDetalle pid
												on pe.idPedidoInterno = pid.idPedidoInterno
								where	pe.idTipoPedidoInterno = @idTipoPedidoInterno
								group by pe.idPedidoInterno
							)A
					where	#pedidosEspeciales.idPedidoInterno = a.idPedidoInterno



					if not exists (select 1 from #pedidosEspeciales)
					begin
						select	@valido = cast(0 as bit),
								@status = -1,
								@mensaje = 'No se encontraron pedidos especiales con esos términos de búsqueda.'
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

			select	idPedidoInterno as idPedidoEspecial,
					fechaAlta,
					coalesce(descripcionPedido, 'sin descripcion') as descripcion,
					cantidad,
					idAlmacenOrigen,
					idAlmacenOrigen as idAlmacen,
					almacenOrigen as descripcion,
					idAlmacenDestino,
					idAlmacenDestino as idalmacen,
					almacenDestino as descripcion,
					idUsuario,
					nombreCompleto,
					idStatus, 
					descripcionEstatus as descripcion
			from	#pedidosEspeciales
				
			
					
		end -- reporte de estatus
		

	end  -- principal
go

grant exec on SP_CONSULTA_PEDIDOS_ESPECIALES to public
go



