
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_PRODUCTOS]    Script Date: 27/09/2022 06:43:08 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los diferentes clientes del sistema
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_PRODUCTOS]

	@idProducto				int = null,
	@descripcion			varchar(255) = null,
	@idUnidadMedida			int = null,
	@idLineaProducto		int = null,
	@activo					bit = null,
	@articulo				varchar(255) = null,
	@claveProdServ			float = null,
	@fechaIni				datetime = null,
	@fechaFin				datetime = null,
	@idUsuario				int = null,
	@idAlmacen				int = null,
	@idPedidoEspecial		int=null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit),
						@idUbicacionProcesoProduccionAgranel int = 1004

				create table
					#Productos
						(
							contador					int identity (1,1),
							idProducto					int,
							descripcion					varchar(100),
							idUnidadMedida				int,
							idLineaProducto				int,
							cantidadUnidadMedida		float,
							codigoBarras				nvarchar(4000),
							fechaAlta					datetime,
							activo						bit,
							articulo					varchar(100),
							idClaveProdServ				float,
							precioIndividual			money,
							precioMenudeo				money,
							DescripcionLinea			varchar(500), 
							DescripcionUnidadMedida		varchar(500), 
							cantidad					float,
							descripcionConExistencias	varchar(500),
							costo						float,
							porcUtilidadIndividual		float,
							porcUtilidadMayoreo			float,
							cantidadRecibida            int,
							ultimoCostoCompra           money default(0),
							fraccion					bit,
							idUnidadCompra				int,
							descripcionUnidadCompra		varchar(500),
							cantidadUnidadCompra		int,
							
							
						)
						
			end  --declaraciones 

			begin -- principal

				select @idProducto = coalesce(@idProducto, 0)
				select @idUnidadMedida = coalesce(@idUnidadMedida, 0)
				select @idLineaProducto = coalesce(@idLineaProducto, 0)
				select @activo = cast(1 as bit) --coalesce(@activo, 1)
				select @claveProdServ = coalesce(@claveProdServ, 0)
				select @fechaIni = coalesce(@fechaIni, '19000101')
				select @fechaFin = coalesce(@fechaFin, '19000101')
				select @idUsuario = coalesce(@idUsuario, 0)

				if (OBJECT_ID('tempdb.dbo.#LineaProductoUsuario','U')) is not null
					drop table #LineaProductoUsuario

				CREATE TABLE #LineaProductoUsuario (contador int primary key identity ,
				idLineaProducto int , idAlmacen int ,  descripcion varchar(255))

				if(coalesce(@idUsuario,0)>0	or coalesce(@idAlmacen,0)>0)
				begin
					insert into #LineaProductoUsuario select * from obtnerLineasProductosXAlmacen(@idUsuario , @idAlmacen)
				end
				else
				begin
					insert into #LineaProductoUsuario(idLineaProducto,idAlmacen,descripcion) 
					select idLineaProducto,@idAlmacen,descripcion from LineaProducto where activo=1					
				end
				

				-- universo de productos
				insert into 
					#Productos 
						(
							idProducto,descripcion,idUnidadMedida,idLineaProducto,cantidadUnidadMedida,codigoBarras,
							fechaAlta,activo,articulo,idClaveProdServ,precioIndividual,precioMenudeo,
							DescripcionLinea,DescripcionUnidadMedida,cantidad,descripcionConExistencias,costo,
							porcUtilidadIndividual,porcUtilidadMayoreo,cantidadRecibida,fraccion,ultimoCostoCompra,idUnidadCompra,descripcionUnidadCompra,cantidadUnidadCompra
							
						)
				select	p.idProducto,upper(p.descripcion) as descripcion,p.idUnidadMedida,p.idLineaProducto,cantidadUnidadMedida,codigoBarras,
						p.fechaAlta,p.activo,articulo,claveProdServ,coalesce(precioIndividual, 0) as precioIndividual, coalesce(precioMenudeo, 0) as precioMenudeo,
						l.descripcion as DescripcionLinea, u.descripcion as DescripcionUnidadMedida, coalesce(g.cantidad, 0) as cantidad,
						case
							when g.cantidad is null then upper(p.descripcion) + '  - (S/E)'
							when g.cantidad = 0  then upper(p.descripcion) + '  - (S/E)'
							else upper(p.descripcion) + 
									'  - (E:' + cast(g.cantidad as varchar(500)) + ' / ' +
										 'ME:' + cast(isnull(p.precioIndividual,'') as varchar(500)) + ' / ' +
										 'MA:' + cast(isnull(p.precioMenudeo,'') as varchar(500)) +
										 ')'
						end as descripcionConExistencias,coalesce(p.ultimoCostoCompra,DBO.obtenerPrecioCompra(p.idProducto,GETDATE())) costo,
						porcUtilidadIndividual,porcUtilidadMayoreo,0 cantidadRecibida,dbo.LineaProductoFraccion(p.idLineaProducto,p.idProducto) fraccion,
						isnull(p.ultimoCostoCompra,0)
						,coalesce(p.idUnidadCompra,0),coalesce(unidad.descripcion,'') descripcionUnidadCompra,coalesce(cantidadUnidadCompra,0)
						
				from	Productos p
				inner join LineaProducto l 
					on p.idLineaProducto = l.idLineaProducto
				inner join CatUnidadMedida u
					on p.idUnidadMedida = u.idUnidadMedida
				left join InventarioGeneral g
					on g.idProducto = p.idProducto
				left join CatUnidadCompra unidad on p.idUnidadCompra=unidad.idUnidadCompra
				join #LineaProductoUsuario lineaUsr on lineaUsr.idLineaProducto=p.idLineaProducto
				where p.activo = @activo
				order by p.idProducto desc						

				-- se eliminan los productos en el piso 9 ya que esos no deben salir ene l combo de productos en una venta
				/*delete	#Productos
				where	idProducto in (select idProducto from InventarioDetalle where idUbicacion in (select idUbicacion from Ubicacion where idPiso = 9 ))
				*/
				-- se actualizan existencias si se consulta con el numero de usuario
				if ( @idUsuario > 0 ) 
					begin

					   declare @idAlmacenUsuario int

					   select @idAlmacenUsuario=idAlmacen from Usuarios where idUsuario=@idUsuario
						
						update	#Productos 
						set		cantidad = 0,
								descripcionConExistencias = upper(descripcion) + '  - (S/E)'

						UPDATE p
						set		cantidad = coalesce(d.cantidadTotal,0),
								descripcionConExistencias = upper(p.descripcion) +  case when coalesce(e.cantidadTotal,0)=0 then '  - (S/E)' else  '  - ( D:'  + cast(coalesce( (d.cantidadTotal), 0) as varchar) + ' / ' +
								'E:'  + cast(coalesce( (e.cantidadTotal), 0) as varchar) + ' / ' +
								'SA:'  + cast(coalesce( (sa.cantidadTotal), 0) as varchar) + ' / ' +
								'R:'  + cast(coalesce( (r.cantidadTotal), 0) as varchar) + ' / ' +
								'B:'  + cast(coalesce( (b.cantidadTotal), 0) as varchar) + ' / ' +
								'ME:' + cast(isnull(p.precioIndividual,'') as varchar) + ' / ' +
								'MA:' + cast(isnull(p.precioMenudeo,'') as varchar) +
							')' end
						from #Productos p
						left join (select i.idProducto,sum(i.cantidad) cantidadTotal from InventarioDetalle i join Ubicacion u on i.idUbicacion=u.idUbicacion and u.idAlmacen=@idAlmacenUsuario where i.cantidad>0 and u.idPiso not in (1003,	@idUbicacionProcesoProduccionAgranel)  group by i.idProducto) e on p.idProducto=e.idProducto --existencia total
						left join (select i.idProducto,sum(i.cantidad) cantidadTotal from InventarioDetalle i join Ubicacion u on i.idUbicacion=u.idUbicacion and u.idAlmacen=@idAlmacenUsuario where i.cantidad>0 and u.idPiso in (9) group by i.idProducto) b on p.idProducto=b.idProducto --existencia bloqueadas
						left join (select i.idProducto,sum(i.cantidad) cantidadTotal from InventarioDetalle i join Ubicacion u on i.idUbicacion=u.idUbicacion and u.idAlmacen=@idAlmacenUsuario where i.cantidad>0 and u.idPiso in (1000) group by i.idProducto) r on p.idProducto=r.idProducto --existencia resguardo
						left join (select i.idProducto,sum(i.cantidad) cantidadTotal from InventarioDetalle i join Ubicacion u on i.idUbicacion=u.idUbicacion and u.idAlmacen=@idAlmacenUsuario where i.cantidad>0 and u.idPiso in (0,1001) group by i.idProducto) sa on p.idProducto=sa.idProducto --existencia sin acomodar
						left join (select i.idProducto,sum(i.cantidad) cantidadTotal from InventarioDetalle i join Ubicacion u on i.idUbicacion=u.idUbicacion and u.idAlmacen=@idAlmacenUsuario where i.cantidad>0 and u.idPiso not in (9,1000,0,1001,1003,	@idUbicacionProcesoProduccionAgranel) group by i.idProducto) d on p.idProducto=d.idProducto --existencia disponible


						--update	#Productos
						--set		#Productos.cantidad = a.cantidad,
						--		#Productos.descripcionConExistencias = a.descripcionConExistencias
						--from	(
						--			select	id.idProducto, 
						--					coalesce( (sum(id.cantidad)), 0) as cantidad,
						--					case
						--						when coalesce( (sum(id.cantidad)), 0) is null then upper(p.descripcion) + '  - (S/E)'
						--						when coalesce( (sum(id.cantidad)), 0) = 0  then upper(p.descripcion) + '  - (S/E)'
						--						else upper(p.descripcion) + 
						--								'  - (E:'  + cast(coalesce( (sum(id.cantidad)), 0) as varchar(500)) + ' / ' +
						--									 'ME:' + cast(isnull(p.precioIndividual,'') as varchar(500)) + ' / ' +
						--									 'MA:' + cast(isnull(p.precioMenudeo,'') as varchar(500)) +
						--									 ')'
						--					end as descripcionConExistencias
						--			from	Usuarios u
						--						inner join Almacenes a
						--							on a.idAlmacen = u.idAlmacen
						--						inner join Ubicacion ub
						--							on ub.idAlmacen = a.idAlmacen
						--						inner join InventarioDetalle id
						--							on id.idUbicacion = ub.idUbicacion
						--						inner join Productos p
						--							on p.idProducto = id.idProducto
						--			where	u.idUsuario = @idUsuario
						--				and	ub.idPiso not in (9) -- se eliminan los productos en el piso 9 ya que esos no deben salir ene l combo de productos en una venta 
						--			group by id.idProducto, p.descripcion, p.precioIndividual, p.precioMenudeo
						--		)A
						--where	#Productos.idProducto = a.idProducto

					end

					--si vienes de un pedido especial solo consultamos los productos del pedido especial
					if(coalesce(@idPedidoEspecial,0)>0)
					begin

					   select @idAlmacen=idAlmacen from Usuarios where idUsuario=@idUsuario
					   
					   declare @idEstatusPedidoInterno int					   

					  if not exists(select 1 from PedidosInternos where idPedidoInterno=@idPedidoEspecial and idTipoPedidoInterno=2)
					    raiserror('El número de ticket de pedido especial no existe', 11, -1)

					   select @idEstatusPedidoInterno=idEstatusPedidoInterno from PedidosInternos where idPedidoInterno=@idPedidoEspecial
					  
					  if (@idEstatusPedidoInterno!=4)
					  begin
					     declare @msg varchar(100)
						 select @msg='El pedido especial se encuentra en estatus de ' + coalesce((select descripcion from catestatuspedidointerno where idEstatusPedidoInterno=@idEstatusPedidoInterno),'')
					     raiserror(@msg, 11, -1)
					  end

					  if exists(select 1 from PedidosInternos where idPedidoInterno=@idPedidoEspecial and idAlmacenOrigen<>@idAlmacen)
					    raiserror('El pedido especial no pertenece a tu almacen', 11, -1)

					  
					   UPDATE P SET cantidadRecibida=d.cantidadAceptada 
					   from pedidosInternosDetalle d
					   join #Productos p on d.idProducto=p.idProducto
					   where idPedidoInterno=@idPedidoEspecial and d.cantidadAceptada>0

					   --eliminamos todos los productos que no tengan cantidad aceptada
					   delete #Productos where cantidadRecibida=0

					   if not exists ( select 1 from #Productos )
						begin
							 raiserror('No existe productos para este pedido especial', 11, -1)
						end



					end
					else 
					begin
						if not exists ( select 1 from #Productos )
						begin
							select	@valido = cast(0 as bit),
									@status = -1,
									@mensaje = 'No se encontraron productos con esos términos de búsqueda.'
						end

					end
					
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
							

		-- si todo ok
			if ( @valido = 1 )
				begin
				
					select	* 
					from	#Productos p
					where	p.idProducto =		case
													when @idProducto = 0 then p.idProducto
													else @idProducto
												end

						and p.descripcion like	case
													when @descripcion is null then p.descripcion
													else '%' + @descripcion + '%'
												end

						and p.idUnidadMedida =	case
													when @idUnidadMedida = 0 then p.idUnidadMedida
													else @idUnidadMedida
												end

						and p.idLineaProducto =	case
													when @idLineaProducto = 0 then p.idLineaProducto
													else @idLineaProducto
												end

						and articulo like	case
												when @articulo is null then articulo
												else '%' + @articulo + '%' 
											end

						and cast(p.fechaAlta as date) >=	case
																when @fechaIni = '19000101' then cast(p.fechaAlta as date)
																else cast(@fechaIni as date)
															end

						and cast(p.fechaAlta as date) <=	case
																when @fechaFin = '19000101' then cast(p.fechaAlta as date)
																else cast(@fechaFin as date)
															end

						and p.activo = @activo
						order by p.descripcion
				end
				
		end -- reporte de estatus

	end  -- principal

