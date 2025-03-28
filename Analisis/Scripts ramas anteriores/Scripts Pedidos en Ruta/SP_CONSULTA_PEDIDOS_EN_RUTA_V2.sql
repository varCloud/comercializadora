--USE [DB_A57E86_lluviadesarrollo]
--GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_PEDIDOS_EN_RUTA_V2]    Script Date: 06/03/2022 06:20:58 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select * from sysobjects where name like 'SP_CONSULTA_PEDIDOS_EN_RUTA_V2' and xtype = 'p' )
	drop proc SP_CONSULTA_PEDIDOS_EN_RUTA_V2
go

/*
Autor			Ernesto Aguilar
Fecha			2022/03/06
Objetivo		Consulta los pedidos en ruta
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CONSULTA_PEDIDOS_EN_RUTA_V2]

	@idUsuarioRuteo				int,
	@fechaIni					datetime,
	@fechaFin					datetime

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''
						

				create table	
					#pedidosEnRuta
						(
							id					int identity(1,1),
							idPedidoEspecial	int,
							idCliente			int,
							nombreCliente		varchar(300),
							montoTotal			float,
							cantidad			float,
							idUsuario			int,
							nombreUsuario		varchar(300),
							fechaAlta			datetime								
									
						)


			end  --declaraciones 

			begin  -- principal

			-- si todo ok

				insert into #pedidosEnRuta (idPedidoEspecial,idCliente,nombreCliente,montoTotal,cantidad,idUsuario,nombreUsuario,fechaAlta)
				select	p.idPedidoEspecial,
						c.idCliente,
						c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno as nombreCliente,
						p.montoTotal,
						p.cantidad,
						u.idUsuario,
						u.nombre +  ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
						p.fechaAlta								
				from	PedidosEspeciales p
							join Clientes c
								on c.idCliente = p.idCliente
							join Usuarios u
								on u.idUsuario = p.idUsuario

				where	p.idUsuarioRuteo =		case
														when @idUsuarioRuteo = 0 then idUsuarioRuteo
														when @idUsuarioRuteo is null then idUsuarioRuteo
														else @idUsuarioRuteo
													end

				and cast(p.fechaAlta as date) >=	case
														when @fechaIni = '19000101' then cast(p.fechaAlta as date)
														when @fechaIni is null then cast(p.fechaAlta as date)
														else cast(@fechaIni as date)
													end

				and cast(p.fechaAlta as date) <=	case
														when @fechaFin = '19000101' then cast(p.fechaAlta as date)
														when @fechaFin is null then cast(p.fechaAlta as date)
														else cast(@fechaFin as date)
													end
				and p.idEstatusPedidoEspecial in (9)
				and	p.liquidado = cast(0 as bit)
				order by p.fechaAlta desc


				if not exists ( select 1 from #pedidosEnRuta )
				begin
					
					select @mensaje = 'No existen pedidos en ruta con los criterios seleccionados.'
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

				if exists ( select 1 from #pedidosEnRuta )
				begin
					select * from #pedidosEnRuta order by id
				end

				
			end -- reporte de estatus



	end  -- principal
go

grant exec on SP_CONSULTA_PEDIDOS_EN_RUTA_V2 to public
go