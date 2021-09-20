USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_PRODUCTOS_ALMACEN]    Script Date: 17/09/2021 09:47:45 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- se crea procedimiento SP_CONSULTA_PRODUCTOS_ALMACEN
if exists (select * from sysobjects where name like 'SP_CONSULTA_PRODUCTOS_ALMACEN' and xtype = 'p' )
	drop proc SP_CONSULTA_PRODUCTOS_ALMACEN
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/09/10
Objetivo		Consulta los productos por idAlmacen para modulo de pedidos especiales V2
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CONSULTA_PRODUCTOS_ALMACEN]

	@idAlmacen				int 

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit)

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
							cantidad					int,
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


				--if (OBJECT_ID('tempdb.dbo.#LineaProductoUsuario','U')) is not null
				--	drop table #LineaProductoUsuario

				--CREATE TABLE #LineaProductoUsuario (contador int primary key identity ,
				--idLineaProducto int , idAlmacen int ,  descripcion varchar(255))

				--if(coalesce(@idUsuario,0)>0	or coalesce(@idAlmacen,0)>0)
				--begin
				--	insert into #LineaProductoUsuario select * from obtnerLineasProductosXAlmacen(21 , 4)
				--end
				--else
				--begin
				--	insert into #LineaProductoUsuario(idLineaProducto,idAlmacen,descripcion) 
				--	select idLineaProducto,@idAlmacen,descripcion from LineaProducto where activo=1					
				--end
				

				---- universo de productos
				--insert into 
				--	#Productos 
				--		(
				--			idProducto,descripcion,idUnidadMedida,idLineaProducto,cantidadUnidadMedida,codigoBarras,
				--			fechaAlta,activo,articulo,idClaveProdServ,precioIndividual,precioMenudeo,
				--			DescripcionLinea,DescripcionUnidadMedida,cantidad,descripcionConExistencias,costo,
				--			porcUtilidadIndividual,porcUtilidadMayoreo,cantidadRecibida,fraccion,ultimoCostoCompra,idUnidadCompra,descripcionUnidadCompra,cantidadUnidadCompra							
				--		)
				--select	p.idProducto,upper(p.descripcion) as descripcion,p.idUnidadMedida,p.idLineaProducto,cantidadUnidadMedida,codigoBarras,
				--		p.fechaAlta,p.activo,articulo,claveProdServ,coalesce(precioIndividual, 0) as precioIndividual, coalesce(precioMenudeo, 0) as precioMenudeo,
				--		l.descripcion as DescripcionLinea, u.descripcion as DescripcionUnidadMedida, coalesce(g.cantidad, 0) as cantidad,
				--		case
				--			when g.cantidad is null then upper(p.descripcion) + '  - (S/E)'
				--			when g.cantidad = 0  then upper(p.descripcion) + '  - (S/E)'
				--			else upper(p.descripcion) + 
				--					'  - (E:' + cast(g.cantidad as varchar(500)) + ' / ' +
				--						 'ME:' + cast(isnull(p.precioIndividual,'') as varchar(500)) + ' / ' +
				--						 'MA:' + cast(isnull(p.precioMenudeo,'') as varchar(500)) +
				--						 ')'
				--		end as descripcionConExistencias,coalesce(p.ultimoCostoCompra,DBO.obtenerPrecioCompra(p.idProducto,GETDATE())) costo,
				--		porcUtilidadIndividual,porcUtilidadMayoreo,0 cantidadRecibida,dbo.LineaProductoFraccion(p.idLineaProducto,p.idProducto) fraccion,
				--		isnull(p.ultimoCostoCompra,0)
				--		,coalesce(p.idUnidadCompra,0),coalesce(unidad.descripcion,'') descripcionUnidadCompra,coalesce(cantidadUnidadCompra,0)
						
				--from	Productos p
				--inner join LineaProducto l 
				--	on p.idLineaProducto = l.idLineaProducto
				--inner join CatUnidadMedida u
				--	on p.idUnidadMedida = u.idUnidadMedida
				--left join InventarioGeneral g
				--	on g.idProducto = p.idProducto
				--left join CatUnidadCompra unidad on p.idUnidadCompra=unidad.idUnidadCompra
				----join #LineaProductoUsuario lineaUsr on lineaUsr.idLineaProducto=p.idLineaProducto
				--where p.activo = cast(1 as bit) 
				--order by p.idProducto desc						

				----select * from InventarioGeneral
				----select * from InventarioDetalle





				-- se inserta todo el universo de productos
				insert into 
					#Productos 
						(
							idProducto,descripcion,idUnidadMedida,idLineaProducto,cantidadUnidadMedida,codigoBarras,
							fechaAlta,activo,articulo,idClaveProdServ,precioIndividual,precioMenudeo,
							DescripcionLinea,DescripcionUnidadMedida,cantidad,descripcionConExistencias,costo,
							porcUtilidadIndividual,porcUtilidadMayoreo,cantidadRecibida,fraccion,ultimoCostoCompra,idUnidadCompra,descripcionUnidadCompra,cantidadUnidadCompra							
						)
				select	p.idProducto,
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
						0 as cantidad,
						upper(p.descripcion) + '  - (S/E)' as descripcionConExistencias,
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
							inner join InventarioDetalle id
								on id.idProducto = p.idProducto
							inner join Ubicacion ub
								on ub.idUbicacion = id.idUbicacion	
							inner join LineaProducto l 
								on p.idLineaProducto = l.idLineaProducto
							inner join CatUnidadMedida u
								on p.idUnidadMedida = u.idUnidadMedida
							left join CatUnidadCompra unidad 
								on p.idUnidadCompra=unidad.idUnidadCompra								
				where	id.cantidad > 0
					and	ub.idPasillo not in (9)
				order by p.idProducto



				-- actualizamos existencias
				update	#Productos
				set		cantidad = existencias.cantidad,
						descripcionConExistencias =	upper(existencias.descripcion) + 
																					'  - (E:'  + cast(coalesce( (existencias.cantidad), 0) as varchar(500)) + ' / ' +
																						'ME:' + cast(isnull(existencias.precioIndividual,'') as varchar(500)) + ' / ' +
																						'MA:' + cast(isnull(existencias.precioMenudeo,'') as varchar(500)) +
																						')'
				from	(
							select	p_.idProducto, p_.descripcion, ub_.idAlmacen, p_.precioIndividual, p_.precioMenudeo,
									SUM(id_.cantidad) as cantidad
							from	Productos p_
										inner join InventarioDetalle id_
											on id_.idProducto = p_.idProducto
										inner join Ubicacion ub_
											on ub_.idUbicacion = id_.idUbicacion	
							where	id_.cantidad > 0
								and	ub_.idPasillo not in (9)
								and	ub_.idAlmacen = @idAlmacen
							group by p_.idProducto, p_.descripcion, p_.precioIndividual, p_.precioMenudeo, ub_.idAlmacen	
							--order by p_.idProducto
						)existencias
				where	#Productos.idProducto = existencias.idProducto
					






				--select	id.idProducto, 
				--		coalesce( (sum(id.cantidad)), 0) as cantidad,
				--		case
				--			when coalesce( (sum(id.cantidad)), 0) is null then upper(p.descripcion) + '  - (S/E)'
				--			when coalesce( (sum(id.cantidad)), 0) = 0  then upper(p.descripcion) + '  - (S/E)'
				--			else upper(p.descripcion) + 
				--					'  - (E:'  + cast(coalesce( (sum(id.cantidad)), 0) as varchar(500)) + ' / ' +
				--							'ME:' + cast(isnull(p.precioIndividual,'') as varchar(500)) + ' / ' +
				--							'MA:' + cast(isnull(p.precioMenudeo,'') as varchar(500)) +
				--							')'
				--		end as descripcionConExistencias
				--from	Usuarios u
				--			inner join Almacenes a
				--				on a.idAlmacen = u.idAlmacen
				--			inner join Ubicacion ub
				--				on ub.idAlmacen = a.idAlmacen
				--			inner join InventarioDetalle id
				--				on id.idUbicacion = ub.idUbicacion
				--			inner join Productos p
				--				on p.idProducto = id.idProducto
				--where	ub.idPasillo not in (9)
				----where	u.idUsuario = @idUsuario
				----	and	ub.idPiso not in (9) -- se eliminan los productos en el piso 9 ya que esos no deben salir ene l combo de productos en una venta 
				--group by id.idProducto, p.descripcion, p.precioIndividual, p.precioMenudeo

















				--select * from Ubicacion
				--select * from Almacenes
				--select * from CatUnidadCompra
				--select * from InventarioDetalle i join Ubicacion u on u.idUbicacion=i.idUbicacion where idAlmacen in (2,5)
				

				---- se actualizan existencias si se consulta con el numero de usuario
				--	update	#Productos 
				--	set		cantidad = 0,
				--			descripcionConExistencias = upper(descripcion) + '  - (S/E)'

				--	update	#Productos
				--	set		#Productos.cantidad = a.cantidad,
				--			#Productos.descripcionConExistencias = a.descripcionConExistencias
				--	from	(
				--				select	id.idProducto, 
				--						coalesce( (sum(id.cantidad)), 0) as cantidad,
				--						case
				--							when coalesce( (sum(id.cantidad)), 0) is null then upper(p.descripcion) + '  - (S/E)'
				--							when coalesce( (sum(id.cantidad)), 0) = 0  then upper(p.descripcion) + '  - (S/E)'
				--							else upper(p.descripcion) + 
				--									'  - (E:'  + cast(coalesce( (sum(id.cantidad)), 0) as varchar(500)) + ' / ' +
				--											'ME:' + cast(isnull(p.precioIndividual,'') as varchar(500)) + ' / ' +
				--											'MA:' + cast(isnull(p.precioMenudeo,'') as varchar(500)) +
				--											')'
				--						end as descripcionConExistencias
				--				from	Usuarios u
				--							inner join Almacenes a
				--								on a.idAlmacen = u.idAlmacen
				--							inner join Ubicacion ub
				--								on ub.idAlmacen = a.idAlmacen
				--							inner join InventarioDetalle id
				--								on id.idUbicacion = ub.idUbicacion
				--							inner join Productos p
				--								on p.idProducto = id.idProducto
				--				where	ub.idPasillo not in (9)
				--				--where	u.idUsuario = @idUsuario
				--				--	and	ub.idPiso not in (9) -- se eliminan los productos en el piso 9 ya que esos no deben salir ene l combo de productos en una venta 
				--				group by id.idProducto, p.descripcion, p.precioIndividual, p.precioMenudeo
				--			)A
				--	where	#Productos.idProducto = a.idProducto

			

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
				
					select	idProducto,
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
go