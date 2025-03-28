
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_PRODUCTOS_ALMACEN]    Script Date: 27/09/2022 06:26:27 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/09/10
Objetivo		Consulta los productos por idAlmacen para modulo de pedidos especiales V2
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_PRODUCTOS_ALMACEN]

	@idAlmacen				int 

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@descripcionAlmacen	    varchar(100)='',
						@valido					bit = cast(1 as bit)

				create table
					#Productos
						(
							contador					int identity (1,1),
							idProducto					int,
							idAlmacen					int,
							Almacen						varchar(300),
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
				declare
				@idUbicacionProcesoProduccionAgranel int = 1004
			  --obtenemos la descripcion del almacen
			  select @descripcionAlmacen=descripcion from Almacenes where idAlmacen = @idAlmacen

				-- se inserta todo el universo de productos
				insert into 
					#Productos 
						(
							idProducto,idAlmacen,Almacen,descripcion,idUnidadMedida,idLineaProducto,cantidadUnidadMedida,codigoBarras,
							fechaAlta,activo,articulo,idClaveProdServ,precioIndividual,precioMenudeo,
							DescripcionLinea,DescripcionUnidadMedida,cantidad,descripcionConExistencias,costo,
							porcUtilidadIndividual,porcUtilidadMayoreo,cantidadRecibida,fraccion,ultimoCostoCompra,idUnidadCompra,descripcionUnidadCompra,cantidadUnidadCompra							
						)
				select	p.idProducto,
						@idAlmacen idAlmacen,
						@descripcionAlmacen Almacen,
						upper(p.descripcion) as descripcion, 
						p.idUnidadMedida,
						p.idLineaProducto,
						p.cantidadUnidadMedida,
						p.codigoBarras,
						p.fechaAlta,
						p.activo,
						p.articulo,
						p.claveProdServ,
						coalesce(p.precioIndividual, 0) as precioIndividual, 
						coalesce(p.precioMenudeo, 0) as precioMenudeo,
						l.descripcion as DescripcionLinea, 
						u.descripcion as DescripcionUnidadMedida, 
						coalesce(d.cantidadTotal,0) as cantidad,
						--upper(p.descripcion) + '  - (S/E)' as descripcionConExistencias,
						upper(p.descripcion) +  case when coalesce(e.cantidadTotal,0)=0 then '  - (S/E)' else  '  - ( D:'  + cast(coalesce( (d.cantidadTotal), 0) as varchar) + ' / ' +
								'E:'  + cast(coalesce( (e.cantidadTotal), 0) as varchar) + ' / ' +
								'SA:'  + cast(coalesce( (sa.cantidadTotal), 0) as varchar) + ' / ' +
								'R:'  + cast(coalesce( (r.cantidadTotal), 0) as varchar) + ' / ' +
								'B:'  + cast(coalesce( (b.cantidadTotal), 0) as varchar) + ' / ' +
								'ME:' + cast(isnull(p.precioIndividual,'') as varchar) + ' / ' +
								'MA:' + cast(isnull(p.precioMenudeo,'') as varchar) +
							')' end descripcionConExistencias,
						coalesce(p.ultimoCostoCompra,DBO.obtenerPrecioCompra(p.idProducto,GETDATE())) costo,
						porcUtilidadIndividual,
						porcUtilidadMayoreo,
						0 as cantidadRecibida,
						dbo.LineaProductoFraccion(p.idLineaProducto,p.idProducto) fraccion,
						isnull(p.ultimoCostoCompra,0),
						coalesce(p.idUnidadCompra,0),
						coalesce(unidad.descripcion,'') as descripcionUnidadCompra,
						coalesce(cantidadUnidadCompra,0)
				from	Productos p 
							--inner join InventarioDetalle id
								--on id.idProducto = p.idProducto
							--inner join Ubicacion ub
								--on ub.idUbicacion = id.idUbicacion	
							inner join LineaProducto l 
								on p.idLineaProducto = l.idLineaProducto
							inner join CatUnidadMedida u
								on p.idUnidadMedida = u.idUnidadMedida
							left join CatUnidadCompra unidad 
								on p.idUnidadCompra=unidad.idUnidadCompra
							left join (select i.idProducto,sum(i.cantidad) cantidadTotal from InventarioDetalle i join Ubicacion u on i.idUbicacion=u.idUbicacion and u.idAlmacen=@idAlmacen where i.cantidad>0 and u.idPiso not in (1003,@idUbicacionProcesoProduccionAgranel) group by i.idProducto) e on p.idProducto=e.idProducto --existencia total
							left join (select i.idProducto,sum(i.cantidad) cantidadTotal from InventarioDetalle i join Ubicacion u on i.idUbicacion=u.idUbicacion and u.idAlmacen=@idAlmacen where i.cantidad>0 and u.idPiso in (9) group by i.idProducto) b on p.idProducto=b.idProducto --existencia bloqueadas
							left join (select i.idProducto,sum(i.cantidad) cantidadTotal from InventarioDetalle i join Ubicacion u on i.idUbicacion=u.idUbicacion and u.idAlmacen=@idAlmacen where i.cantidad>0 and u.idPiso in (1000) group by i.idProducto) r on p.idProducto=r.idProducto --existencia resguardo
							left join (select i.idProducto,sum(i.cantidad) cantidadTotal from InventarioDetalle i join Ubicacion u on i.idUbicacion=u.idUbicacion and u.idAlmacen=@idAlmacen where i.cantidad>0 and u.idPiso in (0,1001) group by i.idProducto) sa on p.idProducto=sa.idProducto --existencia sin acomodar
							left join (select i.idProducto,sum(i.cantidad) cantidadTotal from InventarioDetalle i join Ubicacion u on i.idUbicacion=u.idUbicacion and u.idAlmacen=@idAlmacen where i.cantidad>0 and u.idPiso not in (9,1000,0,1001,1003,@idUbicacionProcesoProduccionAgranel) group by i.idProducto) d on p.idProducto=d.idProducto --existencia disponible
				where p.activo=1	--id.cantidad > 0
					--and	ub.idPasillo not in (9)
					--and	ub.idAlmacen = @idAlmacen
				order by p.idProducto

				--update	#Productos
				--set		#Productos.idAlmacen = a.idAlmacen,
				--		#Productos.Almacen = a.descripcion
				--from	(
				--			select idAlmacen, descripcion from Almacenes where idAlmacen = @idAlmacen
				--		)A

				-- actualizamos existencias
				--update	#Productos
				--set		cantidad = existencias.cantidad,
				--		descripcionConExistencias =	upper(existencias.descripcion) + 
				--																	'  - (E:'  + cast(coalesce( (existencias.cantidad), 0) as varchar(500)) + ' / ' +
				--																		'ME:' + cast(isnull(existencias.precioIndividual,'') as varchar(500)) + ' / ' +
				--																		'MA:' + cast(isnull(existencias.precioMenudeo,'') as varchar(500)) +
				--																		')'
				--from	(
				--			select	p_.idProducto, p_.descripcion, ub_.idAlmacen, p_.precioIndividual, p_.precioMenudeo,
				--					SUM(id_.cantidad) as cantidad
				--			from	Productos p_
				--						inner join InventarioDetalle id_
				--							on id_.idProducto = p_.idProducto
				--						inner join Ubicacion ub_
				--							on ub_.idUbicacion = id_.idUbicacion	
				--			where	id_.cantidad > 0
				--				and	ub_.idPasillo not in (9)
				--				and	ub_.idPiso not in (9)
				--				and	ub_.idAlmacen = @idAlmacen
				--			group by p_.idProducto, p_.descripcion, p_.precioIndividual, p_.precioMenudeo, ub_.idAlmacen								
				--		)existencias
				--where	#Productos.idProducto = existencias.idProducto
					

				if not exists ( select 1 from #Productos )
				begin
					select	@valido = cast(0 as bit),
							@status = -1,
							@mensaje = 'No se encontraron productos con esos términos de búsqueda.'
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
				
					select	distinct 
							idProducto,
							idAlmacen,
							Almacen,
							descripcion,
							idUnidadMedida,
							idLineaProducto,
							cantidadUnidadMedida,
							codigoBarras,
							fechaAlta,
							activo,
							articulo,
							idClaveProdServ,
							precioIndividual,
							precioMenudeo,
							DescripcionLinea,
							DescripcionUnidadMedida,
							cantidad,
							descripcionConExistencias,
							costo,
							porcUtilidadIndividual,
							porcUtilidadMayoreo,
							cantidadRecibida,
							fraccion,
							ultimoCostoCompra,
							idUnidadCompra,
							descripcionUnidadCompra,
							cantidadUnidadCompra	 
					from	#Productos p
					order by p.descripcion
				end
				
		end -- reporte de estatus

	end  -- principal

grant exec on SP_CONSULTA_PRODUCTOS_ALMACEN to public
