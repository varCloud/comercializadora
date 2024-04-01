
IF EXISTS ( SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'SP_CONSULTA_INVENTARIO_GENERAL_UBICACION' )
	DROP PROCEDURE [SP_CONSULTA_INVENTARIO_GENERAL_UBICACION]
GO


/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_INVENTARIO_GENERAL_UBICACION]    Script Date: 31/03/2024 12:00:51 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create proc [dbo].[SP_CONSULTA_INVENTARIO_GENERAL_UBICACION]

	@tipo		int -- 1-Inventario General  /2- Inventario por Ubicación

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit)
						
			end  --declaraciones 

			---preparacion
			begin
			
			create table
				#inventario_general
					(
						idProducto				int,
						descripcion				varchar(100),
						ultimoCostoCompra		money,
						precioIndividual		money,
						precioMenudeo			money,
						cantidad				float
					)

			create table
				#inventario_ubicacion
					(
						idProducto				int,
						descripcion				varchar(100),
						ultimoCostoCompra		money,
						precioIndividual		money,
						precioMenudeo			money,
						cantidad				float,
						idPasillo				int,
						idRaq					int,
						idPiso					int
					)			
			

			end

			begin -- principal



				if (@tipo = 1)  -- 1-Inventario General  
					begin
						insert into 
							#inventario_general
								(idProducto, descripcion, ultimoCostoCompra, precioIndividual, precioMenudeo, cantidad)		
						select	p.idProducto, p.descripcion, p.ultimoCostoCompra, p.precioIndividual, p.precioMenudeo, ig.cantidad
						from	InventarioGeneral ig
								join Productos p
									on p.idProducto = ig.idProducto
						
						if not exists ( select 1 from #inventario_general )
							begin
								select	@valido = cast(0 as bit),
										@status = -1,
										@mensaje = 'No se encontro inventario con esos términos de búsqueda.'
							end
					end
				else
					begin		-- 2- Inventario por Ubicación

						insert into 
							#inventario_ubicacion
								(idProducto, descripcion, ultimoCostoCompra, precioIndividual, precioMenudeo, cantidad, idPasillo, idRaq, idPiso)
						select	p.idProducto, p.descripcion as descripcion_producto, p.ultimoCostoCompra, p.precioIndividual, p.precioMenudeo, 
								id.cantidad, u.idPasillo, u.idRaq,  u.idPiso
						from	InventarioDetalle id
								join Productos p 
									on p.idProducto = id.idProducto
								join Ubicacion u
									on u.idUbicacion = id.idUbicacion
								--join Almacenes a
								--	on a.idAlmacen = u.idAlmacen

						if not exists ( select 1 from #inventario_ubicacion )
							begin
								select	@valido = cast(0 as bit),
										@status = -1,
										@mensaje = 'No se encontro inventario con esos términos de búsqueda.'
							end
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

		-- si todo ok
			if ( @valido = 1 )
				begin		
					if (@tipo = 1)  -- 1-Inventario General  
						begin
							select	* 
							from	#inventario_general
							order by descripcion
						end
					else
						begin		-- 2- Inventario por Ubicación
							select	* 
							from	#inventario_ubicacion
							order by descripcion
						end 					
				end
				
		end -- reporte de estatus

	end  -- principal


GRANT EXECUTE ON SP_CONSULTA_INVENTARIO_GENERAL_UBICACION TO PUBLIC
go