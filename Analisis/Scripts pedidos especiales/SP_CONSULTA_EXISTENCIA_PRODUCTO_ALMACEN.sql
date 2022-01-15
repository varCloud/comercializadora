--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_EXISTENCIA_PRODUCTO_ALMACEN]    Script Date: 17/09/2021 09:47:45 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- se crea procedimiento SP_CONSULTA_EXISTENCIA_PRODUCTO_ALMACEN
if exists (select * from sysobjects where name like 'SP_CONSULTA_EXISTENCIA_PRODUCTO_ALMACEN' and xtype = 'p' )
	drop proc SP_CONSULTA_EXISTENCIA_PRODUCTO_ALMACEN
go

/*
Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/10/20
Objetivo		Consulta la existencia por idAlmacen y por idProducto
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CONSULTA_EXISTENCIA_PRODUCTO_ALMACEN]

	@idProducto				int,
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
					#Producto
						(
							contador					int identity (1,1),
							idProducto					int,
							idAlmacen					int,
							descripcion					varchar(100),
							cantidad					float							
						)
						
			end  --declaraciones 

			begin -- principal

				if not exists ( select idProducto from Productos where idProducto = @idProducto )
				begin
					select	@valido = cast(0 as bit),
							@status = -1,
							@mensaje = 'No se se encontro ese producto en el inventario.'
				end

				-- insert universo de productos
				insert into #Producto (idProducto,idAlmacen,descripcion,cantidad)
				select	idProducto, @idAlmacen, descripcion, cast(0 as float) as cantidad
				from	Productos
				where	idProducto = @idProducto

				-- actualizamos existencias
				update	#Producto
				set		#Producto.cantidad = a.cantidad
				from	(				
							select	p_.idProducto, p_.descripcion, ub_.idAlmacen, SUM(id_.cantidad) as cantidad
							from	Productos p_
										inner join InventarioDetalle id_
											on id_.idProducto = p_.idProducto
										inner join Ubicacion ub_
											on ub_.idUbicacion = id_.idUbicacion	
							where	p_.idProducto = @idProducto --id_.cantidad > 0
								and	ub_.idAlmacen = @idAlmacen
								and	ub_.idPasillo not in (9)
							group by p_.idProducto, p_.descripcion, ub_.idAlmacen	
						)A
				where	#Producto.idProducto = a.idProducto

					
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

					select * from #Producto

				end
				
		end -- reporte de estatus

	end  -- principal


grant exec on SP_CONSULTA_EXISTENCIA_PRODUCTO_ALMACEN to public
go