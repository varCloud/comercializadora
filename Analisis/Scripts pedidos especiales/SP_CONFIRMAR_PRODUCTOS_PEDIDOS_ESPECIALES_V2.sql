USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2]    Script Date: 17/09/2021 09:47:45 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- se crea procedimiento SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2
if exists (select * from sysobjects where name like 'SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2' and xtype = 'p' )
	drop proc SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2
go

/*
Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/09/10
Objetivo		confirma los productos del pedido especial
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2]

	@listaProductos						xml,
	@idPedidoEspecial					int,
	@idEstatusPedidoEspecial			int,	
	@idUsuarioEntrega					int,
	@numeroUnidadTaxi					varchar(255),
	@idEstatusCuentaPorCobrar			int,
	@montoTotal							float,
	@montoTotalcantidadAbonada			float

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status						int = 200,
						@mensaje					varchar(255) = '',
						@error_line					varchar(255) = '',
						@error_procedure			varchar(255) = '',
						@fecha						varchar(255) = '',
						@tran_name					varchar(32) = 'CONFIRMA_PRODUCTOS_PEDIDO_ESP',
						@tran_count					int = @@trancount,
						@tran_scope					bit = 0,						
						@valido						bit = cast(1 as bit)
						

				create table 
					#cantidadSolicitada 
						(  
							id						int identity(1,1),
							cantidadSolicitada		float
						)

				create table 
					#cantidadAtendida 
						(  
							id						int identity(1,1),
							cantidadAtendida		float
						)
				create table 
					#cantidadRechazada 
						(  
							id						int identity(1,1),
							cantidadRechazada		float
						)
				create table 
					#cantidadAceptada 
						(  
							id						int identity(1,1),
							cantidadAceptada		float
						)

				create table 
					#idProductos 
						(  
							id			int identity(1,1),
							idProducto	int
						)

				create table 
					#idPedidoEspecialDetalle
						(  
							id							int identity(1,1),
							idPedidoEspecialDetalle		int
						)

				create table 
					#observaciones
						(  
							id							int identity(1,1),
							observaciones				varchar(500)
						)


			end  --declaraciones 

			begin -- principal

				if not exists ( select 1 from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial )
				begin
					if ( @idPedidoEspecial > 0 )
					begin 
						select @mensaje = 'No existe el pedido especial solicitado.'
						select @valido = cast(0 as bit)
						raiserror (@mensaje, 11, -1)					
					end
				end
			
			
				begin -- inicio transaccion
					if @tran_count = 0
						begin tran @tran_name
					else
						save tran @tran_name
					select @tran_scope = 1
				end -- inicio transaccion


				select @fecha = coalesce(@fecha, dbo.FechaActual())

				insert into #cantidadSolicitada (cantidadSolicitada)
				SELECT Pedidos.cantidadSolicitada.value('.','NVARCHAR(200)') AS cantidadSolicitada FROM @listaProductos.nodes('//cantidadSolicitada') as Pedidos(cantidadSolicitada) 

				insert into #cantidadAtendida (cantidadAtendida)
				SELECT Pedidos.cantidadAtendida.value('.','NVARCHAR(200)') AS cantidadAtendida FROM @listaProductos.nodes('//cantidadAtendida') as Pedidos(cantidadAtendida) 

				insert into #cantidadRechazada  (cantidadRechazada )
				SELECT Pedidos.cantidadRechazada .value('.','NVARCHAR(200)') AS cantidadRechazada  FROM @listaProductos.nodes('//cantidadRechazada ') as Pedidos(cantidadRechazada ) 

				insert into #cantidadAceptada (cantidadAceptada)
				SELECT Pedidos.cantidadAceptada.value('.','NVARCHAR(200)') AS cantidadAceptada FROM @listaProductos.nodes('//cantidadAceptada') as Pedidos(cantidadAceptada) 

				insert into #idProductos (idProducto)
				SELECT Pedidos.idProducto.value('.','NVARCHAR(200)') AS idProducto FROM @listaProductos.nodes('//idProducto') as Pedidos(idProducto)

				insert into #idPedidoEspecialDetalle (idPedidoEspecialDetalle)
				SELECT Pedidos.idPedidoEspecialDetalle.value('.','NVARCHAR(200)') AS idAlmacen FROM @listaProductos.nodes('//idPedidoEspecialDetalle') as Pedidos(idPedidoEspecialDetalle)

				insert into #observaciones (observaciones)
				SELECT Pedidos.observaciones.value('.','NVARCHAR(200)') AS observaciones FROM @listaProductos.nodes('//observaciones') as Pedidos(observaciones)
				
				-- universo de venta de productos
				select	p.idProducto, 
						i.idPedidoEspecialDetalle,
						cs.cantidadSolicitada,
						ca.cantidadAtendida,
						cr.cantidadRechazada,
						cac.cantidadAceptada,
						o.observaciones
				into	#productos
				from	#idProductos p_
						inner join Productos p
							on p.idProducto = p_.idProducto
						inner join #idPedidoEspecialDetalle i
							on i.id = p_.id
						inner join #observaciones o
							on o.id = p_.id
						inner join #cantidadSolicitada cs
							on cs.id = p_.id
						inner join #cantidadAtendida ca
							on ca.id = p_.id
						inner join #cantidadRechazada cr
							on cr.id = p_.id
						inner join #cantidadAceptada cac
							on cac.id = p_.id

						

				--select * from #productos
							


				-- acualizamos estatus de pedido especial
				update	PedidosEspeciales
				set		idEstatusPedidoEspecial = @idEstatusPedidoEspecial,
						idUsuarioEntrega = @idUsuarioEntrega,
						numeroUnidadTaxi = @numeroUnidadTaxi												
				where	idPedidoEspecial = @idPedidoEspecial


				-- acualizamos estatus de pedido especial detalle
				if exists ( select 1 from #productos where cantidadSolicitada <> cantidadAceptada )
					begin
					
						update	PedidosEspecialesDetalle
						set		PedidosEspecialesDetalle.observaciones = a.observaciones,
								idEstatusPedidoEspecialDetalle = 5	--Atendidos/Incompletos
						from	(
									select	idProducto, idPedidoEspecialDetalle, observaciones 
									from	#productos
								)A
						where	PedidosEspecialesDetalle.idPedidoEspecialDetalle = a.idPedidoEspecialDetalle

					end
				else
					begin
					
						update	PedidosEspecialesDetalle
						set		PedidosEspecialesDetalle.idEstatusPedidoEspecialDetalle = 2	--Atendidos
						where	PedidosEspecialesDetalle.idPedidoEspecial = @idPedidoEspecial

					end



				begin -- commit de transaccion
					if @tran_count = 0
						begin -- si la transacción se inició dentro de este ámbito
							commit tran @tran_name
							select @tran_scope = 0
						end -- si la transacción se inició dentro de este ámbito
				end -- commit de transaccion				

			end -- principal

			end try

			begin catch 
		
				-- captura del error
				select	@status =			-error_state(),
						@error_procedure =	error_procedure(),
						@error_line =		error_line(),
						@mensaje =			error_message(),
						@valido = cast(0 as bit)
					
			end catch

			begin -- reporte de estatus

			--reporte de estatus
				select	@status status,
						@error_procedure error_procedure,
						@error_line error_line,
						@mensaje mensaje
							

			

				
			end -- reporte de estatus

	end  -- principal

grant exec on SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2 to public
go