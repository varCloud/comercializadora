use [DB_A57E86_lluviadesarrollo]
go

-- se crea procedimiento SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS
if exists (select * from sysobjects where name like 'SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/09/10
Objetivo		CONSULTA LOS DETALLES DE LA TABLA DE PEDIDOS INTERNOS DETALLE
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS

	@idPedidoEspecial		int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@fecha					datetime

				create table
					#productos
						(
							contador							int identity(1,1),
							idPedidoInterno						int, 
							idAlmacenOrigen						int, 
							idAlmacenDestino					int, 
							idUsuario							int, 
							IdEstatusPedidoInterno				int, 
							fechaAlta							datetime, 
							observacion							varchar(255), 
							idTipoPedidoInterno					int, 
							descripcion							varchar(255), 
							idPedidoInternoDetalle				int, 
							idProducto							int, 
							descProducto						varchar(255), 
							cantidad							int, 
							cantidadAceptada					int, 
							cantidadAtendida					int, 
							cantidadRechazada					int
						)
						--drop table  #productos 
			end  --declaraciones 

			begin -- principal

				if not exists ( select 1 from PedidosInternos where idPedidoInterno = @idPedidoEspecial )
				begin
					select @mensaje = 'No existe el pedido, verifique por favor.'
					raiserror (@mensaje, 11, -1)
				end




				insert into 
					#productos
						(
							idPedidoInterno, idAlmacenOrigen, idAlmacenDestino, idUsuario, IdEstatusPedidoInterno, fechaAlta, observacion,
							idTipoPedidoInterno, descripcion, idPedidoInternoDetalle, idProducto, descProducto, cantidad, cantidadAceptada, 
							cantidadAtendida,cantidadRechazada
						)
				select	pi_.idPedidoInterno, idAlmacenOrigen, idAlmacenDestino, idUsuario, IdEstatusPedidoInterno, pi_.fechaAlta, observacion, 
						idTipoPedidoInterno, pi_.descripcion, idPedidoInternoDetalle, pid.idProducto, pro.descripcion as descProducto, cantidad, 
						cantidadAceptada, cantidadAtendida, cantidadRechazada
				from	PedidosInternos pi_
							inner join PedidosInternosDetalle pid
								on pid.idPedidoInterno = pi_.idPedidoInterno
							inner join Productos pro
								on pro.idProducto = pid.idProducto
				where	pi_.idPedidoInterno = @idPedidoEspecial

				if not exists ( select 1 from #productos )
				begin
					select @mensaje = 'No hay registros para el pedido seleccionado.'
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

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje,
					@idPedidoEspecial as idPedidoEspecial

			if exists (select 1 from #productos)
				begin
					select	* from #productos
				end
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS to public
go



