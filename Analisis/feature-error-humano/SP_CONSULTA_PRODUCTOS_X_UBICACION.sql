USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_PRODUCTOS_X_UBICACION]    Script Date: 9/16/2023 1:25:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta un producto y su ubicacion 
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_PRODUCTOS_X_UBICACION]

	@idProducto			int,
	@idSucursal			int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit),
						@fraccion				bit

				create table
					#existencias
						(
							idInventarioDetalle			int, 
							idProducto					int,
							descripcion					varchar(500),
							cantidad					float, 
							fechaAlta					datetime, 
							fechaActualizacion			datetime, 
							Almacen						varchar(300), 
							Pasillo						varchar(300), 
							Raq							varchar(300),
							Piso						varchar(300),
							idPasillo					int, 
							idRaq						int, 
							idPiso						int,
							idUbicacion				    int,
							fraccion					bit
						)

			end  --declaraciones 

			begin -- principal				

				--select @idAlmacen = 1

				select @fraccion=dbo.LineaProductoFraccion(null,@idProducto)
				
				insert into		
					#existencias
						(
							idInventarioDetalle,idProducto, descripcion,cantidad,fechaAlta,fechaActualizacion,
							Almacen,Pasillo,Raq,Piso,idPasillo,idRaq,idPiso,idUbicacion,fraccion
						)
				select	i.idInventarioDetalle, i.idProducto, pr.descripcion , dbo.redondear(i.cantidad), i.fechaAlta, i.fechaActualizacion, 
						a.Descripcion as Almacen, p.descripcion as Pasillo, r.descripcion as Raq, ps.descripcion as Piso,
						p.idPasillo, r.idRaq, ps.idPiso,u.idUbicacion,@fraccion
				from	InventarioDetalle i
							inner join Ubicacion u
								on u.idUbicacion = i.idUbicacion
							inner join CatPasillo p
								on p.idPasillo = u.idPasillo
							inner join Almacenes a
								on a.idAlmacen = u.idAlmacen
							inner join CatRaq r
								on r.idRaq = u.idRaq
							inner join CatPiso ps
								on ps.idPiso = u.idPiso
							inner join Productos pr
								on pr.idProducto = i.idProducto
				where	a.idSucursal = case	
											when @idSucursal = 0 then a.idSucursal
											else @idSucursal
										end
					and	i.idProducto = case	
											when @idProducto = 0 then i.idProducto
											else @idProducto
										end
					and cantidad>0
				order by i.idProducto
				
				-- si no existe
				if not exists ( select top 1 1 from #existencias)
					begin
						select	@mensaje = 'No hay existencias para este Producto',
								@valido = cast(0 as bit)
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
					@mensaje as  mensaje

			

			if ( @valido = cast(1 as bit) )
				begin
						
					select	*
					from	#existencias
					order by fechaAlta desc 

				end
								
		end -- reporte de estatus

	end  -- principal
