--USE [DB_A57E86_comercializadora]
GO
/****** Object:  StoredProcedure [dbo].[SP_REGISTRA_COMPRA]    Script Date: 03/02/2022 06:44:56 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
Fecha			2020/04/26
Objetivo		Registrar una compra
*/

ALTER proc

	[dbo].[SP_REGISTRA_COMPRA]
	
	@idCompra int,
	@idProveedor int,
	@idUsuario int,
	@idStatusCompra int,
	@productos XML,
	@observaciones varchar(500),
	@idAlmacen int

as

	begin -- procedimiento
		
		begin try -- try principal
		
			begin -- inicio
			
				-- declaraciones
				declare	@status int = 200,
						@error_message varchar(255) = 'La compra se ha registrado de manera correcta',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@idStatusCompraActual int;
						
				declare	@tran_name varchar(32) = 'REGISTRA_COMPRA',
						@tran_count int = @@trancount,
						@tran_scope bit = 0

				if(@idCompra>0)
				  select @error_message= 'La compra se ha actualizado de manera correcta'
						

			end -- inicio
			
			begin -- validaciones
			
				-- -- se realiza la validación 1 = 0
				 if not exists(select 1 from proveedores where idProveedor=@idProveedor)
				 	raiserror('El proveedor no existe', 11, 0)

				 if(@idCompra>0)
				 begin
				  
				  if not exists(select 1 from Compras where idCompra=@idCompra)
					raiserror('La compra que desea actualizar no existe', 11, 0)

				  select @idStatusCompraActual=idStatusCompra from Compras where idCompra=@idCompra

				  if (@idStatusCompraActual in (3,4,5))
					raiserror('El estatus de la compra ya no puede ser actualizado', 11, 0)

				  end
				
			end -- validaciones

			begin

				select 
				Compra.productos.value('(idProducto)[1]','bigint') idProducto,
				Compra.productos.value('(cantidad)[1]','float') cantidad,
				Compra.productos.value('(precio)[1]','money') precio
				INTO #productosCompra
				from @productos.nodes('//ArrayOfProducto/Producto') as Compra(productos)				

			end
		
			begin -- transacción

				begin -- inicio

					if @tran_count = 0
						begin tran @tran_name
					else
						save tran @tran_name
				
					select @tran_scope = 1
				
				end -- inicio
				
				begin -- componente de la transacción
				
					if not exists(select 1 from compras where idCompra=@idCompra)
					begin
						INSERT INTO COMPRAS(idProveedor,fechaAlta,idUsuario,idStatusCompra,observaciones,idAlmacen)
						VALUES (@idProveedor,dbo.FechaActual(),@idUsuario,@idStatusCompra,@observaciones,@idAlmacen)

						select @idCompra=max(idCompra) from COMPRAS
					end
					else
					begin
						UPDATE Compras
						SET 
						idProveedor=@idProveedor,
						idStatusCompra=@idStatusCompra,
						observaciones=@observaciones,
						fechaCambioEstatus=case when @idStatusCompra<>@idStatusCompra then dbo.FechaActual() else fechaCambioEstatus end
						where idCompra=@idCompra
					end

					--registramos los productos
					if(@idCompra>0)
					BEGIN
						 --eliminamos los productos que no estan 
						  delete c from ComprasDetalle c
						  left join #productosCompra p on c.idProducto=p.idProducto
						  where idCompra=@idCompra and p.idProducto is null

						  --actualizamos los productos que ya estaban
						  UPDATE c
						  set precio=p.precio,
							  cantidad=dbo.redondear(p.cantidad)
						  from ComprasDetalle c
						  join #productosCompra p on c.idProducto=p.idProducto
						  where idCompra=@idCompra

						  --insertamos los nuevmos productos
						  INSERT INTO ComprasDetalle(idCompra,idProducto,cantidad,precio )
						  select @idCompra,p.idProducto,dbo.redondear(p.cantidad),p.precio 
						  from #productosCompra p
						  left join ComprasDetalle c on p.idProducto=c.idProducto and c.idCompra=@idCompra
						  where c.idProducto is null

						  if exists(select 1 from ComprasDetalle where idCompra=@idCompra and coalesce(idEstatusProductoCompra,0)=0 and @idStatusCompra=3)
							raiserror('No puede finalizar la compra si no tienen algun estatus todos los productos', 11, 0)

						  if exists(select 1 from ComprasDetalle where idCompra=@idCompra and idEstatusProductoCompra=coalesce(idEstatusProductoCompra,0) and @idStatusCompra=4)
							raiserror('No puede cancelar la compra ya que alguno de los productos tienen algun estatus', 11, 0)

					END

					--SI LA COMPRA YA FUERA REALIZADA O FINALIZADA ACTUALIZAMOS LOS VALORES DEL COSTO DE COMPRA
					if @idStatusCompra in (2,3)
					BEGIN
						--INSERTAMOS EN LA TABLA DE HISTORICOS EL COSTO DE COMPRA SI ES DIFERENTE AL QUE TENEMOS ACTUALMENTE EN LA TABLA DE PRODUCTOS
						INSERT INTO ProductosCostoCompra 
						(idProducto
						,descripcion
						,idUnidadMedida
						,idLineaProducto
						,cantidadUnidadMedida
						,fechaAlta
						,claveProdServ
						,precioIndividual
						,precioMenudeo
						,costoCompra
						,porcUtilidadIndividual
						,porcUtilidadMayoreo
						,idUnidadCompra
						,cantidadUnidadCompra)

						SELECT 
							PC.idProducto
							,descripcion
							,idUnidadMedida
							,idLineaProducto
							,cantidadUnidadMedida
							,[dbo].[FechaActual]()
							,claveProdServ
							,precioIndividual
							,precioMenudeo
							,PC.precio
							,porcUtilidadIndividual
							,porcUtilidadMayoreo
							,idUnidadCompra
							,cantidadUnidadCompra 
						FROM 
							#productosCompra PC 
							join Productos P on PC.idProducto = P.idProducto
							WHERE PC.precio <> P.ultimoCostoCompra

						--ACTUALIZAMOS EL COSTO DE COMPRA DEL PRODUCTO EN LA TABLA PRODUCTOS
						UPDATE P
						SET 
							ultimoCostoCompra=PC.precio
						FROM 
							Productos  P
							join #productosCompra PC on P.idProducto=PC.idProducto
						WHERE 
							P.ultimoCostoCompra <>  PC.precio
					END

				
				end -- componente de la transacción
				
				begin -- commit
				
					if @tran_count = 0
					
						begin -- si la transacción se inició dentro de este ámbito
						
							commit tran @tran_name
							select @tran_scope = 0
						
						end -- si la transacción se inició dentro de este ámbito
				
				end -- commit
			
			end
		
		end try -- try principal
		
		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@error_message = error_message(),
					@error_severity =
						case error_severity()
							when 11 then 'Error en validación'
							when 12 then 'Error en consulta'
							when 13 then 'Error en actualización'
							else 'Error general'
						end
						
			-- revertir transacción si es necesario
			if @tran_scope = 1
				rollback tran @tran_name
		
		end catch -- catch principal
		
		begin -- reporte de status
		
			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@error_severity error_severity,
					@error_message mensaje,
					@idCompra idCompra
				
		end -- reporte de status
		
	end -- procedimiento
