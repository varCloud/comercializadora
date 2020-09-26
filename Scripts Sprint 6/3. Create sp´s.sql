-- se crea procedimiento SP_OBTENER_LIMITES_INVENTARIO
if exists (select * from sysobjects where name like 'SP_OBTENER_LIMITES_INVENTARIO' and xtype = 'p')
	drop proc SP_OBTENER_LIMITES_INVENTARIO
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Obtener los limites maximo y minimos que de inventario por producto y almacen
status			200 = ok
				-1	= error
*/

Create proc [dbo].[SP_OBTENER_LIMITES_INVENTARIO]

@idProducto int=null,
@idAlmacen int=null,
@idEstatusLimiteInv int = null,
@idLineaProducto int=null 
/*
1	Invietario dentro de sus limites 
2	Cantidad superior por el maximo permitido
3	Cantidad por debajo del minimo permitido
*/

as

	begin -- procedimiento
		
		begin try -- try principal
		
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit)
	
						
			end -- inicio
			
		    
			begin
			
				select I.idLimiteInventario,i.minimo,i.maximo,p.idProducto,p.descripcion,p.codigoBarras,a.idAlmacen,a.Descripcion descripcionAlmacen, isnull(B.cantidad,0)cantidadInventario  
				,l.idLineaProducto,l.descripcion descripcionLineaProducto
				,case 
					  when isnull(B.cantidad,0) > = i.minimo and isnull(B.cantidad,0) <= i.maximo then 1
					  when isnull(B.cantidad,0) > i.maximo   then 2
					  when isnull(B.cantidad,0) < i.minimo then 3
				end idEstatusLimiteInventario
				into #LimitesInventario
				from LimitesInventario i
				LEFT JOIN(select I.idProducto , U.idAlmacen, sum(I.cantidad) cantidad 
				from InventarioDetalle  I join Ubicacion U on U.idUbicacion   = I .idUbicacion group by I.idProducto , U.idAlmacen) B  on i.idAlmacen = B.idAlmacen and i.idProducto = B.idProducto
				join Productos p on i.idProducto=p.idProducto
				join Almacenes a on i.idAlmacen=a.idAlmacen
				join LineaProducto l on p.idLineaProducto=l.idLineaProducto
				where 
				i.idProducto=coalesce(@idProducto,i.idProducto)
				and i.idAlmacen=coalesce(@idAlmacen,i.idAlmacen)
				and l.idLineaProducto=coalesce(@idLineaProducto,l.idLineaProducto)

				if not exists (select 1 from #LimitesInventario)
				begin
					select	@valido = cast(0 as bit),
							@status = -1,
							@error_message = 'No se encontraron resultados.'
				end
				

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
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@error_severity error_severity,
					@error_message mensaje

			if(@valido=1)
			select A.*, B.idEstatusLimiteInventario as idStatus, B.descripcion 
			from #LimitesInventario A join CatEstatusLimiteInventario B on A.idEstatusLimiteInventario = B.idEstatusLimiteInventario
			where B.idEstatusLimiteInventario = coalesce(@idEstatusLimiteInv, B.idEstatusLimiteInventario)

					
		end -- reporte de estatus
		
	end -- procedimiento
	

go

-- se crea procedimiento SP_OBTENER_ESTATUS_LIMITES_INVENTARIO
if exists (select * from sysobjects where name like 'SP_OBTENER_ESTATUS_LIMITES_INVENTARIO' and xtype = 'p')
	drop proc SP_OBTENER_ESTATUS_LIMITES_INVENTARIO
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Obtener estatus de los limites de inventario
status			200 = ok
				-1	= error
*/

Create proc [dbo].SP_OBTENER_ESTATUS_LIMITES_INVENTARIO

as

	begin -- procedimiento
		
		SELECT idEstatusLimiteInventario idStatus,descripcion
        FROM CatEstatusLimiteInventario
		
	end -- procedimiento

go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_LIMITES_INVENTARIO_MASIVO
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_LIMITES_INVENTARIO_MASIVO' and xtype = 'p')
	drop proc SP_INSERTA_ACTUALIZA_LIMITES_INVENTARIO_MASIVO
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Insertar o actualizar un limite de inventario de manera masiva
status			200 = ok
				-1	= error
*/

Create proc [dbo].[SP_INSERTA_ACTUALIZA_LIMITES_INVENTARIO_MASIVO]

@xmlLimitesInventario xml,
@idUsuario int

as

	begin -- procedimiento
		
		begin try -- try principal
		
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255),
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = ''
	
						
			end -- inicio
			
		    
			begin

				select 
				LimiteInventario.l.value('(codigoBarras)[1]','varchar(200)') codigoBarras,
				LimiteInventario.l.value('(descripcionAlmacen)[1]','varchar(200)') descripcionAlmacen,
				LimiteInventario.l.value('(minimo)[1]','int') minimo,
				LimiteInventario.l.value('(maximo)[1]','int') maximo,
				0 idAlmacen,
				0 idProducto
				INTO #LimitesInventario
				from @xmlLimitesInventario.nodes('//ArrayOfLimiteInvetario/LimiteInvetario') as LimiteInventario(l)

				update i set idProducto=p.idProducto
				FROM #LimitesInventario i
				join productos p on i.codigoBarras like p.codigoBarras

				update i set idAlmacen=a.idAlmacen
				FROM #LimitesInventario i
				join almacenes a on i.descripcionAlmacen=a.descripcion

				if exists(select 1 from #LimitesInventario where idProducto=0)
				begin
					DECLARE @ProductosNoEncontratos VARCHAR(8000) 

					SELECT @ProductosNoEncontratos = COALESCE(@ProductosNoEncontratos + ', ', '') + 
						ISNULL(codigoBarras, 'N/A')
					FROM #LimitesInventario where idProducto=0
					
					select -1 Estatus,'Los codigos de barra: '+@ProductosNoEncontratos+' no se encuentran registrados en la BD' Mensaje
					return
				end

				if exists(select 1 from #LimitesInventario where idAlmacen=0)
				begin
					DECLARE @AlmacenNoEncontratos VARCHAR(8000) 

					SELECT @AlmacenNoEncontratos = COALESCE(@AlmacenNoEncontratos + ', ', '') + 
						ISNULL(descripcionAlmacen, 'N/A')
					FROM #LimitesInventario where idAlmacen=0

	     			select -1 Estatus,'Los almacenes: '+@AlmacenNoEncontratos+' no se encuentran registrados en la BD' Mensaje
					return
				end

				if exists(select 1 from #LimitesInventario group by codigoBarras,descripcionAlmacen	having count(1)>1)
				begin
					DECLARE @ProductosRepetidos VARCHAR(8000) 

					SELECT @ProductosRepetidos = COALESCE(@ProductosRepetidos + ', ', '') + 
						'(' + ISNULL(codigoBarras, '') + ',' + ISNULL(descripcionAlmacen, '') +')'
					from #LimitesInventario
					group by codigoBarras,descripcionAlmacen
					having count(1)>1
					 
					select -1 Estatus,'Los registros: '+@ProductosRepetidos+' se encuentran repetidos en el archivo excel' Mensaje
					return
				end

				if exists(select 1 from #LimitesInventario where minimo>maximo)
				begin
					DECLARE @MinimoMayorMaximo VARCHAR(8000) 

					SELECT @MinimoMayorMaximo = COALESCE(@MinimoMayorMaximo + ', ', '') + 
						'(' + ISNULL(codigoBarras, '') + ',' + ISNULL(descripcionAlmacen, '') +')'
					from #LimitesInventario
					where minimo>maximo

					select -1 Estatus,'El valor minimo no puede ser mayor que el máximo de los registros: '+@MinimoMayorMaximo Mensaje
					return
				end

				--Actualizamos
				UPDATE a set minimo=b.minimo,maximo=b.maximo
				from LimitesInventario a 
				join #LimitesInventario b on a.idProducto=b.idProducto and a.idAlmacen=b.idAlmacen

				--Insertamos los que no se encuentran en la tabla LimitesInventario
				INSERT INTO LimitesInventario(minimo,maximo,idProducto,idAlmacen,idUsuario,fechaAlta,fechaActualizacion)
				select a.minimo,a.maximo,a.idProducto,a.idAlmacen,@idUsuario,dbo.fechaActual(),dbo.fechaActual() 
				from #LimitesInventario a left join LimitesInventario b on a.idProducto=b.idProducto and a.idAlmacen=b.idAlmacen
				where b.idLimiteInventario is null	
				
				SELECT @error_message='Archivo importado existosamente'			
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
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status Estatus,
					@error_procedure error_procedure,
					@error_line error_line,
					@error_severity error_severity,
					@error_message Mensaje
										
		end -- reporte de estatus
		
	end -- procedimiento

go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_LIMITE_INVENTARIO
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_LIMITE_INVENTARIO' and xtype = 'p')
	drop proc SP_INSERTA_ACTUALIZA_LIMITE_INVENTARIO
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Insertar o actualizar un limite de inventario
status			200 = ok
				-1	= error
*/

Create proc [dbo].[SP_INSERTA_ACTUALIZA_LIMITE_INVENTARIO]

@idProducto	int,
@idAlmacen	int,
@idUsuario	int,
@minimo	int,
@maximo	int

as

	begin -- procedimiento
		
		begin try -- try principal
		
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = ''
	
						
			end -- inicio
			
		    
			begin

			   if (coalesce(@idProducto,0)=0)
			   begin
						select -1 Estatus , 'Especifique el producto' Mensaje
						return 
				end

			    if (coalesce(@idAlmacen,0)=0)
			   begin
						select -1 Estatus , 'Especifique el almacen' Mensaje
						return 
				end
			


				if not exists(select 1 from LimitesInventario where idAlmacen=@idAlmacen and idProducto=@idProducto)
				   INSERT INTO LimitesInventario(minimo,maximo,idProducto,idAlmacen,idUsuario,fechaAlta,fechaActualizacion)
				   values(@minimo,@maximo,@idProducto,@idAlmacen,@idUsuario,dbo.fechaActual(),dbo.FechaActual())
				else
				 UPDATE LimitesInventario
				 set 
				  minimo=coalesce(@minimo,minimo),
				  maximo=coalesce(@maximo,minimo),
				  idUsuario=@idUsuario,
				  fechaActualizacion=dbo.fechaActual()
				  where idAlmacen=@idAlmacen and idProducto=@idProducto	
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
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status Estatus,
					@error_procedure error_procedure,
					@error_line error_line,
					@error_severity error_severity,
					@error_message Mensaje
										
		end -- reporte de estatus
		
	end -- procedimiento
	
GO

-- se crea procedimiento SP_OBTENER_LIMITES_INVENTARIO
if exists (select * from sysobjects where name like 'SP_APP_RECHAZAR_PEDIDOS_INTERNOS_ESPECIALES' and xtype = 'p')
	drop proc SP_APP_RECHAZAR_PEDIDOS_INTERNOS_ESPECIALES
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Obtener los limites maximo y minimos que de inventario por producto y almacen
status			200 = ok
				-1	= error
*/

Create proc [dbo].SP_APP_RECHAZAR_PEDIDOS_INTERNOS_ESPECIALES
@idPedidoInterno int ,
@idUsuario int,
@observacionGeneral varchar(1000) = null
AS
BEGIN
	BEGIN TRY
			
			declare
			@idEstatusPedidoActual int = 0,
			@idEstatusPedidoInterno int = 3, -- Rechazado
			@idAlmacenOrigen  int,
			@idAlmacenDestino int 

			SELECT
				 @idEstatusPedidoActual = IdEstatusPedidoInterno
				,@idAlmacenOrigen = idAlmacenOrigen
				,@idAlmacenDestino = idAlmacenDestino
			FROM PedidosInternos 
			WHERE
				@idPedidoInterno =idPedidoInterno


			if exists (select  1  from PedidosInternos where  @idPedidoInterno =idPedidoInterno and IdEstatusPedidoInterno <> 1)
			begin
				select -1 Estatus , 'No se puede rechazar el pedido actual, el pedido ya ha sido aprobado' mensaje
				return
			end

				BEGIN-- DECLARACIONES
					DECLARE
					@fecha  datetime
				END	

				SELECT @fecha  = [dbo].[FechaActual]()

				--insertamos en el log el estado anterior 
				INSERT INTO PedidosInternosLog
				(
					 idPedidoInterno
					,idAlmacenOrigen
					,idAlmacenDestino
					,idUsuario
					,IdEstatusPedidoInterno
					,fechaAlta
				)SELECT 
					 @idPedidoInterno
					,@idAlmacenDestino
					,@idAlmacenOrigen
					,@idUsuario
					,@idEstatusPedidoInterno
					,@fecha
				--FROM PedidosInternos
				--WHERE idPedidoInterno = @idPedidoInterno
								
				--INSERTAMOS LA ACTUALIZACION EN LA TABLA DE MOVIMIENTOS DE MERCANCIA
				INSERT INTO  MovimientosDeMercancia 
				(
				 idAlmacenOrigen
				,idAlmacenDestino
				,idProducto
				,cantidad
				,idPedidoInterno
				,idUsuario
				,fechaAlta
				,idEstatusPedidoInterno
				,observaciones
				,cantidadAtendida
				)
				SELECT 
				@idAlmacenOrigen,
				@idAlmacenDestino,
				PD.idProducto,
				PD.cantidad,
				P.idPedidoInterno,
				@idUsuario,
				@fecha,
				@idEstatusPedidoInterno,
				'Rechazado por el encargado de almacen',
				0
				FROM PedidosInternos P join PedidosInternosDetalle PD
				on  P.idPedidoInterno = PD.idPedidoInterno 
				WHERE 
				P.idPedidoInterno = @idPedidoInterno
				
				

				--INSERTAMOS LA ACUTALIACION DEL ESTATUS
				UPDATE PedidosInternos 
				SET IdEstatusPedidoInterno = @idEstatusPedidoInterno,
				observacion =  @observacionGeneral 								  
				WHERE idPedidoInterno =@idPedidoInterno
				
				--REGRESAMOS  EL VALOR A LA APLICACION
				SELECT 200 Estatus , 'OK' Mensaje 			

	END TRY
	BEGIN CATCH
		SELECT -1 Estatus, error_message() Mensaje,error_line() Errorlin
	END CATCH
	
END
	
go

-- se crea procedimiento SP_OBTENER_LIMITES_INVENTARIO
if exists (select * from sysobjects where name like 'SP_APP_OBTENER_PEDIDOS_INTERNOS_ESPECIALES_X_USUARIO' and xtype = 'p')
	drop proc SP_APP_OBTENER_PEDIDOS_INTERNOS_ESPECIALES_X_USUARIO
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Obtener los limites maximo y minimos que de inventario por producto y almacen
status			200 = ok
				-1	= error
*/

Create proc [dbo].SP_APP_OBTENER_PEDIDOS_INTERNOS_ESPECIALES_X_USUARIO
@idUsuario int = null,
@idPedidoInterno int=null,
@idEstatusPedidoInterno int = null,
@fechaInicio datetime = null,
@fechafin datetime = null

/*
1	Pedido Solicitado
2	Pedido Aprobado
3	Pedido Rechazado
4	Pedido Finalizado
5	Pedido Cancelado
*/
as

	begin -- procedimiento
		
		begin try -- try principal
		
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit)
						
			end -- inicio
		
		select 200 Estatus , 'OK' Mensaje  
			
		--SELECT * FROM cATeSTATUSpEDIDOiNTERNO

		SELECT 
		P.idPedidoInterno, ISNULL(P.descripcion,'') descripcionPedido,P.fechaAlta,p.observacion,
		EP.IdEstatusPedidoInterno ,ep.descripcion descripcionEstatus,
		P.idAlmacenOrigen,AO.idAlmacen, AO.Descripcion,
		P.idAlmacenDestino,AD.idAlmacen,AD.Descripcion,
		isnull(UA.nombre,'') +' '+ isnull(UA.apellidoPaterno,'')+' '+isnull(UA.apellidoMaterno,'') usuarioAtendio
		FROM PedidosInternos P
		JOIN Almacenes AO ON AO.idAlmacen = P.idAlmacenOrigen
		JOIN Almacenes AD ON AD.idAlmacen = P.idAlmacenDestino
	    JOIN CatEstatusPedidoInterno EP ON EP.IdEstatusPedidoInterno = P.IdEstatusPedidoInterno
		LEFT JOIN 
		(SELECT DISTINCT(idPedidoInterno) , idEstatusPedidoInterno,idUsuario FROM MovimientosDeMercancia where idPedidoInterno = coalesce(@idPedidoInterno, idPedidoInterno)) MMA
		 on  MMA.idEstatusPedidoInterno = 2 AND MMA.idPedidoInterno = P.idPedidoInterno
		 LEFT JOIN Usuarios UA ON UA.idUsuario = MMA.idUsuario

		where
		idTipoPedidoInterno = 2
		AND p.idUsuario = coalesce(@idUsuario ,P.idUsuario )
		ANd p.idPedidoInterno = coalesce(@idPedidoInterno, P.idPedidoInterno )
		ANd p.IdEstatusPedidoInterno = coalesce(@idEstatusPedidoInterno,P.idEstatusPedidoInterno)
		AND cast(P.fechaAlta as date) >= coalesce(cast(@fechaInicio as date),cast(P.fechaAlta as date))
		AND cast(P.fechaAlta as date) <= coalesce(cast(@fechaFin as date),cast(P.fechaAlta as date))

		

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
		
		end catch -- catch principal
		
		
	end -- procedimiento
	

go

-- se crea procedimiento SP_OBTENER_LIMITES_INVENTARIO
if exists (select * from sysobjects where name like 'SP_APP_OBTENER_PEDIDOS_INTERNOS_ESPECIALES_X_ALMACEN' and xtype = 'p')
	drop proc SP_APP_OBTENER_PEDIDOS_INTERNOS_ESPECIALES_X_ALMACEN
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Obtener los limites maximo y minimos que de inventario por producto y almacen
status			200 = ok
				-1	= error
*/

Create proc [dbo].SP_APP_OBTENER_PEDIDOS_INTERNOS_ESPECIALES_X_ALMACEN
@idAlmacen int = null,
@idPedidoInterno int=null,
@idEstatusPedidoInterno int = null,
@fechaInicio datetime = null,
@fechafin datetime = null
/*
1	Pedido Solicitado
2	Pedido Aprobado
3	Pedido Rechazado
4	Pedido Finalizado
5	Pedido Cancelado
*/
as
	begin -- procedimiento
		
		begin try -- try principal
		
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit)
						
			end -- inicio
		
		select 200 Estatus , 'Ok' Mensaje 
			
		SELECT 
		P.idPedidoInterno, ISNULL(P.descripcion,'')descripcionPedido,P.fechaAlta,
		isnull(UO.nombre,'') +' '+ isnull(UO.apellidoPaterno,'')+' '+isnull(UO.apellidoMaterno,'') usuarioSolicito,
		UO.idUsuario idUsuarioSolicito,
		EP.IdEstatusPedidoInterno ,ep.descripcion descripcionEstatus,
		P.idAlmacenOrigen,AO.idAlmacen, AO.Descripcion,
		P.idAlmacenDestino,AD.idAlmacen,AD.Descripcion		
		FROM PedidosInternos P
		JOIN Almacenes AO ON AO.idAlmacen = P.idAlmacenOrigen
		JOIN Almacenes AD ON AD.idAlmacen = P.idAlmacenDestino
		JOIN Usuarios UO ON UO.idUsuario = P.idUsuario
	    JOIN CatEstatusPedidoInterno EP ON EP.IdEstatusPedidoInterno = P.IdEstatusPedidoInterno
		where
		    P.idTipoPedidoInterno = 2
		AND P.idAlmacenDestino = coalesce(@idAlmacen ,P.idAlmacenDestino )
		ANd P.idPedidoInterno = coalesce(@idEstatusPedidoInterno,P.idPedidoInterno)
		AND cast(P.fechaAlta as date) >= coalesce(cast(@fechaInicio as date),cast(P.fechaAlta as date))
		AND cast(P.fechaAlta as date) <= coalesce(cast(@fechaFin as date),cast(P.fechaAlta as date))


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
		
		end catch -- catch principal
		
		
	end -- procedimiento
	

go

-- se crea procedimiento SP_OBTENER_LIMITES_INVENTARIO
if exists (select * from sysobjects where name like 'SP_APP_OBTENER_DETALLE_PEDIDOS_INTERNOS' and xtype = 'p')
	drop proc SP_APP_OBTENER_DETALLE_PEDIDOS_INTERNOS
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Obtener los limites maximo y minimos que de inventario por producto y almacen
status			200 = ok
				-1	= error
*/

Create proc [dbo].SP_APP_OBTENER_DETALLE_PEDIDOS_INTERNOS

@idPedidoInterno int=null
as

	begin -- procedimiento
		
		begin try -- try principal
		
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit)
						
			end -- inicio
		
		select 200 Estatus , 'Ok' Mensaje 
			
		SELECT 
		--P.idPedidoInterno,
		--P.IdEstatusPedidoInterno,
		--EP.descripcion descripcionEstatus,
		--Pd.cantidadAtendida,
		PD.idPedidoInternoDetalle,
		isnull(PD.cantidadAtendida, 0) cantidadAtendida,
		PD.idProducto ,
		PD.cantidad,
		Prod.descripcion,
		P.fechaAlta,
		isnull(p.observacion,'') observacion,
		isnull(M4.observaciones,'') observacionRechazaSolicita,
		isnull(MM.observaciones,'') observacionAtendio,
		isnull(M2.observaciones,'') observacionRechazaAtendio,
		isnull(M3.observaciones,'') observacionFinalizado,
		isnull(U.nombre,' ')+' '+isnull(U.apellidoPaterno,'')+' '+isnull(u.apellidoMaterno,'') usuarioAtendio,
		isnull(UU.nombre,' ')+' '+isnull(UU.apellidoPaterno,'')+' '+isnull(UU.apellidoMaterno,'') usuarioSolicito,
		isnull(URechazado.nombre,' ')+' '+isnull(URechazado.apellidoPaterno,'')+' '+isnull(URechazado.apellidoMaterno,'') usuarioRechaza,
		isnull(UAutoriza.nombre,' ')+' '+isnull(UAutoriza.apellidoPaterno,'')+' '+isnull(UAutoriza.apellidoMaterno,'') usuarioAutoriza,
	    MM.fechaAlta as fechaAtendido,
		M2.fechaAlta as fechaRechazado,
		M3.fechaAlta as fechaAutoriza,
		M4.fechaAlta as fechaRechazaSolicita
		 
		--P.idAlmacenOrigen,A.idAlmacen, A.Descripcion,
		--P.idAlmacenDestino,AB.idAlmacen,AB.Descripcion
		--* 
		FROM PedidosInternos   P join  PedidosInternosDetalle PD
		on P.idPedidoInterno = PD.idPedidoInterno   join Productos Prod
		on PD.idProducto = Prod.idProducto JOIN CatEstatusPedidoInterno EP
		ON EP.IdEstatusPedidoInterno =P.IdEstatusPedidoInterno join Almacenes A
		on P.idAlmacenOrigen = A.idAlmacen join Almacenes AB
		on P.idAlmacenDestino = AB.idAlmacen LEFT JOIN [dbo].[MovimientosDeMercancia] MM
		on MM.idPedidoInterno = P.idPedidoInterno and  MM.idEstatusPedidoInterno =2 LEFT JOIN Usuarios U
		on U.idUsuario = MM.idUsuario LEFT JOIN  Usuarios UU 
		on UU.idUsuario = P.idUsuario  LEFT JOIN [dbo].[MovimientosDeMercancia] M2
		on M2.idPedidoInterno = P.idPedidoInterno and  M2.idEstatusPedidoInterno =3 LEFT JOIN Usuarios URechazado on URechazado.idUsuario = M2.idUsuario
		  LEFT JOIN [dbo].[MovimientosDeMercancia] M3
		on M3.idPedidoInterno = P.idPedidoInterno and  M3.idEstatusPedidoInterno =4 LEFT JOIN Usuarios UAutoriza on UAutoriza.idUsuario = M3.idUsuario
		LEFT JOIN [dbo].[MovimientosDeMercancia] M4 on M4.idPedidoInterno = P.idPedidoInterno and  M4.idEstatusPedidoInterno =5 
		LEFT JOIN Usuarios URechazaSoclicita on URechazaSoclicita.idUsuario = M4.idUsuario

		where
			 P.idPedidoInterno = coalesce (@idPedidoInterno , P.idPedidoInterno)		   

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
		
		end catch -- catch principal
		
		
	end -- procedimiento
	
go

-- se crea procedimiento SP_OBTENER_LIMITES_INVENTARIO
if exists (select * from sysobjects where name like 'SP_APP_APROBAR_PEDIDOS_INTERNOS_ESPECIALES' and xtype = 'p')
	drop proc SP_APP_APROBAR_PEDIDOS_INTERNOS_ESPECIALES
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Obtener los limites maximo y minimos que de inventario por producto y almacen
status			200 = ok
				-1	= error
*/

Create proc [dbo].SP_APP_APROBAR_PEDIDOS_INTERNOS_ESPECIALES
@productos  xml,
@idPedidoInterno int ,
@idUsuario int,
@idAlmacenOrigen int, -- el usuario que esta logueado en la hand held
@idAlmacenDestino int, -- el usuario que solicito
@observacionGeneral varchar(1000) = null
AS
BEGIN
	BEGIN TRY
	
			
			declare
			@idEstatusPedidoActual int = 0,
			@idEstatusPedidoInterno int = 2 -- Aprobado
			select  @idEstatusPedidoActual = IdEstatusPedidoInterno from PedidosInternos where @idPedidoInterno =idPedidoInterno


			if exists (select  1  from PedidosInternos where IdEstatusPedidoInterno = @idEstatusPedidoInterno and @idPedidoInterno =idPedidoInterno)
			begin
				select -1 Estatus , 'El estatus del pedido actual es el mismo por favor verifica el estatus' mensaje
				return
			end

			IF OBJECT_ID('tempdb..#ProductosRecibidos') IS NOT NULL
			DROP TABLE #ProductosRecibidos
			create table #ProductosRecibidos
			(	
						indice int identity(1,1),	
						idPedidoInternoDetalle		int ,
						idUbicacion int ,
						idProducto			int ,				
						cantidadAtendida	int ,			
						observaciones		varchar(500),
						cantidadSolicitada int , 
			)

			INSERT INTO  #ProductosRecibidos 
						(
						
						 idPedidoInternoDetalle
						,idUbicacion
						,idProducto			
						,cantidadAtendida	
						,observaciones	
						,cantidadSolicitada		
						)
			SELECT  
					P.value('idPedidoInternoDetalle[1]', 'INT') AS idPedidoInternoDetalle,
					P.value('idUbicacion[1]', 'INT') AS cantidadSolicitada,
					P.value('idProducto[1]', 'INT') AS idProducto,
					P.value('cantidadAtendida[1]', 'INT') AS cantidadAtendida,
					P.value('observaciones[1]', 'varchar(1000)') AS observaciones,
					P.value('cantidadSolicitada[1]', 'INT') AS cantidadSolicitada
			FROM  @productos.nodes('//ArrayOfProductosPedidoEspecial/ProductosPedidoEspecial') AS x(P)
			
			--select * from #ProductosRecibidos
			declare  	@tran_name varchar(32) = 'PRODUCTO_PEDIDO_ESPECIAL',
						@tran_count int = @@trancount,
						@tran_scope bit = 0

			if @tran_count = 0
				begin tran @tran_name
			else
				save tran @tran_name
				
			select @tran_scope = 1

			--BEGIN TRAN 
				--OBTENEMOS LA FECHA MAS QUE NADA LA HORA ACTUAL DE NUESTRA ZONA HORARIA

				BEGIN-- DECLARACIONES

					DECLARE
					@indice int = 1,
					@max int = 0,
					@cantidadAtendida int = 0,
					@idProducto int = 0,
					@cantidadActual int  =0,
					@cantidadTotal int = 0,
					@idTipoMovInventario int = 12,
					@fecha  datetime,
					@idUbicacion int,
					@observacion varchar(1000),
					@idPedidoInternoDetalle int
				END	

				SELECT @fecha  = [dbo].[FechaActual]()

				--insertamos en el log el estado anterior 
				INSERT INTO PedidosInternosLog
				(
					 idPedidoInterno
					,idAlmacenOrigen
					,idAlmacenDestino
					,idUsuario
					,IdEstatusPedidoInterno
					,fechaAlta
				)select 
					 @idPedidoInterno
					,@idAlmacenOrigen
					,@idAlmacenDestino
					,@idUsuario
					,@idEstatusPedidoInterno
					,@fecha
				--FROM PedidosInternos
				--WHERE idPedidoInterno = @idPedidoInterno
								
				SELECT @max = max(indice) from #ProductosRecibidos
				
				
				WHILE(@indice <= @max)
				BEGIN
					print 'index :::'+cast(@indice as varchar)
					SELECT 
							 @idProducto =idProducto 
							,@cantidadAtendida =cantidadAtendida 
							,@idUbicacion = idUbicacion
							,@observacion  = observaciones
							,@idPedidoInternoDetalle = idPedidoInternoDetalle
					   FROM 
							#ProductosRecibidos 
					   WHERE indice = @indice

					   --INSERTAMOS LA ACTUALIZACION EN LA TABLA DE MOVIMIENTOS DE MERCANCIA
					   INSERT INTO  MovimientosDeMercancia 
						(
						 idAlmacenOrigen
						,idAlmacenDestino
						,idProducto
						,cantidad
						,idPedidoInterno
						,idUsuario
						,fechaAlta
						,idEstatusPedidoInterno
						,observaciones
						,cantidadAtendida
						)
						SELECT 
						@idAlmacenOrigen,
						@idAlmacenDestino,
						PD.idProducto,
						PD.cantidad,
						@idPedidoInterno,
						@idUsuario,
						@fecha,
						@idEstatusPedidoInterno,
						coalesce(@observacion,''),
						coalesce(@cantidadAtendida,0)
						FROM PedidosInternos P join PedidosInternosDetalle PD
						on  P.idPedidoInterno = PD.idPedidoInterno 
						WHERE 
						P.idPedidoInterno = @idPedidoInterno
						AND idPedidoInternoDetalle = @idPedidoInternoDetalle


			
				--INSERTAMOS LA ACTUALIZACION DE LA CANTIDAD QUE SE ATENDIO YA QUE PUEDE SER QUE NO SE ENVIE LA QUE SE SOLICITO
					UPDATE PedidosInternosDetalle 
					SET cantidadAtendida = @cantidadAtendida
					WHERE 
					 idPedidoInterno =@idPedidoInterno
					 AND idPedidoInternoDetalle = @idPedidoInternoDetalle
		
					DECLARE 
					@_IdAlmacenDestino int,
					@cantidadPedidoInterno int ,
					@cantidadActualInventario  int,
					@idTipoMonInventario int = 7, -- Salida de mercancia por pedido interno
					@cantidadDespuesDeOperacion int = 0

					SELECT  @cantidadPedidoInterno= isnull(cantidad,0)  
					FROM PedidosInternosDetalle 
					WHERE idPedidoInterno =@idPedidoInterno AND  idPedidoInternoDetalle = @idPedidoInternoDetalle

					--SI LA CANTIDAD QUE ATENDIMOS ES DIFERENTE QUE LA QUE SE PIDIO SE SETEA A CANTIDAD YA QUE CON ESA VARIABLE
					--SE HACEN LAS OPERACIONES PARA INVENTARIO DETALLE Y DETALLE LOG,
					--SI ES IGUAL  PUES NO AFECTA CON CUAL VARIABLE REALIZAMOS EL CALCULO
					IF (@cantidadAtendida != isnull(@cantidadPedidoInterno,0))
						SET @cantidadPedidoInterno = @cantidadAtendida

					--VALIDAMOS QUE EL ID PRODUCTO EXISTA EN EL INVENTARIO
					IF NOT EXISTS (SELECT 1 FROM InventarioDetalle WHERE idUbicacion = @idUbicacion and idProducto = @idProducto)
					BEGIN
						 RAISERROR('No existe producto cargado en el inventario del alamacen', 15, 217)
					END

					SELECT @cantidadActualInventario = cantidad  
					FROM InventarioDetalle 
					WHERE idUbicacion = @idUbicacion and idProducto = @idProducto
					
					-- VALIDAMOS QUE EL RESULTADO QUE SE OBTIENE NO SEA NULL PARA PODER  HACER LA RESTA
					SET @cantidadActualInventario = isnull(@cantidadActualInventario, 0)
				
					IF (@cantidadActualInventario < @cantidadPedidoInterno)
					BEGIN
						 RAISERROR('No existe suficiente canditad en el inventario para actualizar el pedido', 15, 217)
					END

					SET @cantidadDespuesDeOperacion =  @cantidadActualInventario-@cantidadPedidoInterno
					print 'insertamos en detalle log'
					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE LOG
					INSERT INTO InventarioDetalleLog (  idUbicacion,
														idProducto,
														cantidad,
														cantidadActual,
														idTipoMovInventario,
														idUsuario,
														fechaAlta,
														idPedidoInterno
														)
											VALUES ( @idUbicacion,
											         @idProducto,
													 @cantidadPedidoInterno ,
													 @cantidadDespuesDeOperacion,
													 @idTipoMonInventario /* Salida pedido */,
													 @idUsuario,
													 dbo.FechaActual(),
													 @idPedidoInterno)

					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE--
					UPDATE InventarioDetalle SET cantidad = @cantidadDespuesDeOperacion
					WHERE idUbicacion = @idUbicacion and idProducto = @idProducto

					SET @indice = @indice +1

				END -- while
				

				--INSERTAMOS LA ACUTALIACION DEL ESTATUS
				UPDATE PedidosInternos 
				SET IdEstatusPedidoInterno = @idEstatusPedidoInterno,
				observacion = case  when @observacionGeneral is null or  @observacionGeneral = '' then observacion
								    when @observacionGeneral is NOT null AND  @observacionGeneral  != '' then @observacion
							   end
				WHERE idPedidoInterno =@idPedidoInterno
				
			--VALIDAMOS SI LA TRANSACCION SE GENERO AQUI , AQUIMISMO SE HACE EL COMMIT	
		    if @tran_count = 0	
			begin -- si la transacción se inició dentro de este ámbito
						
				commit tran @tran_name
				select @tran_scope = 0
						
			end -- si la transacción se inició dentro de este ámbito

			select 200 Estatus , 'OK' Mensaje 
			DROP TABLE #ProductosRecibidos

	END TRY
	BEGIN CATCH
		SELECT -1 Estatus, error_message() Mensaje,error_line() Errorlin,
		cast(@idProducto as varchar) idProducto,
		cast(@idPedidoInternoDetalle as varchar) idPedidoInternoDetalle,
		cast(@cantidadActualInventario as varchar) cantidadActualInventario,
		cast(@cantidadPedidoInterno as varchar) cantidadPedidoInterno,
		cast(@idUbicacion as varchar) idUbicacion
		
		if @tran_scope = 1
			rollback tran @tran_name

	END CATCH
	
END

go

-- se crea procedimiento SP_GUARDA_PEDIDO_ESPECIAL
if exists (select * from sysobjects where name like 'SP_GUARDA_PEDIDO_ESPECIAL' and xtype = 'p' )
	drop proc SP_GUARDA_PEDIDO_ESPECIAL
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/09/10
Objetivo		Guarda el pedido especial
status			200 = ok
				-1	= error
*/

create proc SP_GUARDA_PEDIDO_ESPECIAL

	@xml					AS XML, 
	@idCliente				int,
	@idPedidoEspecial		int,
	@idUsuario				int,
	@idAlmacenOrigen		int,
	@descripcion			varchar(500)

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Se registro el pedido interno correctamente,',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@tran_name				varchar(32) = 'guardaPedidoInterno',
						@tran_count				int = @@trancount,
						@tran_scope				bit = 0,		
						@idAlmacenAtiende		int = 0,
						@nombreAlmacenAtiende	varchar(100),
						@fecha					datetime,
						@idUbicacionDestino		int = 0,
						@ini					int = 0,
						@fin					int = 0,
						@ini_					int = 0,
						@fin_					int = 0

				create table 
					#cantidades 
						(  
							id			int identity(1,1),
							cantidad	int
						)

				create table 
					#idProductos 
						(  
							id			int identity(1,1),
							idProducto	int
						)

				create table
					#pedidos
						(
							idProducto		int,
							cantidad		int
						)
					
				create table
					#productos
						(
							idProducto		int,
							idAlmacen		int,
							cantidad		int
						)

			end  --declaraciones 

			begin -- principal

				begin -- inicio transaccion
					if @tran_count = 0
						begin tran @tran_name
					else
						save tran @tran_name
					select @tran_scope = 1
				end -- inicio transaccion


				select @fecha = coalesce(@fecha, dbo.FechaActual())

				insert into #cantidades (cantidad)
				SELECT Producto.cantidad.value('.','NVARCHAR(200)') AS cantidad FROM @xml.nodes('//cantidad') as Producto(cantidad) 

				insert into #idProductos (idProducto)
				SELECT Producto.idProducto.value('.','NVARCHAR(200)') AS idProducto FROM @xml.nodes('//idProducto') as Producto(idProducto)

				
				if ( @idPedidoEspecial = 0 )  -- si es nuevo
					begin

						-- universo de pedidos
						insert into #pedidos (idProducto, cantidad)
						select	p.idProducto, coalesce( sum(c.cantidad), 0 )  as cantidad
						from	#cantidades c
								inner join #idProductos p_
									on c.id = p_.id
								inner join Productos p
									on p.idProducto = p_.idProducto
						group by p.idProducto
							

						-- universo de productos por idAlmacen
						insert into #productos (idProducto,idAlmacen,cantidad)
						select	id.idProducto, u.idAlmacen, coalesce( (sum(id.cantidad)), 0) as cantidad
						from	InventarioDetalle id
									inner join Ubicacion u
										on id.idUbicacion =u.idUbicacion
									inner join Productos p
										on p.idProducto = id.idProducto
									inner join Almacenes a
										on a.idAlmacen = u.idAlmacen
						where	id.idproducto in (select distinct idProducto from #pedidos )
							and	a.idTipoAlmacen in (1,2) -- Almacen General y Bodega
							and	a.idAlmacen not in (@idAlmacenOrigen)
						group by id.idProducto, u.idAlmacen, p.descripcion, p.precioIndividual,p.precioMenudeo
						order by idProducto
				
				
				
						-- buscamos quien tiene inventario para surtirlo
						-- eliminamos lo que no son tienen existencias para surtir el pedido 
						delete	#productos
						where	idAlmacen in	(
													select	pro.idAlmacen
													from	#pedidos p 
																inner join #productos pro 
																	on p.idProducto = pro.idProducto 
													where	pro.cantidad-p.cantidad <0
												)
										

						-- eliminamos las que no contienen todos los productos del pedido
						select @ini = min(idAlmacen) from #productos
						select @fin = max(idAlmacen) from #productos

				
						while ( @ini <= @fin )
							begin
						
								select '#productos'+CAST(@ini as varchar(10)) as _vuelta , * into #tempProductos from #productos  where idAlmacen = @ini
					
								-- si no cuenta con todas las existencias 
								if exists ( select * from #pedidos p left join #tempProductos t on p.idProducto = t.idProducto where t.idProducto is null )
									begin 
										--select 'elimina '+  CAST(@ini as varchar(10))  --para debuguear
										delete from #productos where idAlmacen = @ini
									end

								select @ini = min(idAlmacen) from #productos where idAlmacen > @ini
								drop table #tempProductos

							end -- while ( @ini <= @fin )

						if not exists ( select 1 from #productos )
							begin
								select @mensaje = 'No hay Almacenes que puedan atender el pedido.'
								raiserror (@mensaje, 11, -1)
							end

						-- seleccionamos el almacen que atendera el pedido (aquel que tenga mas existencias en total)
						select	top 1 @idAlmacenAtiende = idAlmacen
						from	#productos 
						group by idAlmacen
						order by SUM(cantidad) desc
										
										
						-- insertamos los registros del pedido interno
						insert	into PedidosInternos (idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta,observacion,idTipoPedidoInterno,descripcion)
						select	@idAlmacenOrigen, @idAlmacenAtiende, @idUsuario, 1 as IdEstatusPedidoInterno, @fecha as fechaAlta, null as observacion, 2 as idTipoPedidoInterno, @descripcion as descripcion

						select @idPedidoEspecial = max(idPedidoInterno) from PedidosInternos where idTipoPedidoInterno = 2
						
						insert	into PedidosInternosDetalle (idPedidoInterno,idProducto, cantidad,fechaAlta,cantidadAtendida,cantidadAceptada,cantidadRechazada) 
						select	@idPedidoEspecial as idPedidoInterno, idProducto, cantidad, @fecha as fechaAlta, 0 as cantidadAtendida, 0 as cantidadAceptada, 0 as cantidadRechazada
						from	#pedidos


						insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta)
						select	@idPedidoEspecial, @idAlmacenOrigen, idAlmacenDestino, @idUsuario, cast(1 as int ) as  IdEstatusPedidoInterno, @fecha
						from	PedidosInternos 
						where	idPedidoInterno = @idPedidoEspecial

						
						-- se insertan los movimientos de los productos al almacen destino
						insert into MovimientosDeMercancia (idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoInterno,idUsuario,fechaAlta,idEstatusPedidoInterno,observaciones,cantidadAtendida)
						select	@idAlmacenOrigen as idAlmacenDestino, @idAlmacenAtiende as idAlmacenDestino, idProducto, cantidad, @idPedidoEspecial as  idPedidoInterno,
								@idUsuario as idUsuario, @fecha as fechaAlta, 1 as idEstatusPedidoInterno, null as observaciones, cantidad as cantidadAtendida
						from	#pedidos
						

						---- calculamos las existencias del inventario despues de la venta
						--select idProducto, sum(cantidad) as cantidad into #totProductos from #pedidos group by idProducto

						--select	distinct idInventarioDetalle, id.idProducto, id.cantidad, fechaAlta, id.idUbicacion, 
						--		fechaActualizacion, cast(0 as int) as cantidadDescontada, 
						--		cast(0 as int) as cantidadFinal, a.idAlmacen
						--into	#tempExistencias 
						--from	Usuarios u
						--			inner join Almacenes a
						--				on a.idAlmacen = u.idAlmacen
						--			inner join Ubicacion ub
						--				on ub.idAlmacen = a.idAlmacen
						--			inner join InventarioDetalle id
						--				on id.idUbicacion = ub.idUbicacion
						--			inner join #totProductos p
						--				on p.idProducto = id.idProducto
						--where	a.idAlmacen = @idAlmacenAtiende
						--	and	id.cantidad > 0


						--if not exists ( select 1 from #tempExistencias)
						--	begin
						--		select @mensaje = 'No se realizo el pedido, no se cuenta con suficientes existencias en el inventario.'
						--		raiserror (@mensaje, 11, -1)
						--	end


						---- se calcula de que ubicaciones se van a descontar los productos
						--select @ini_ = min(idProducto), @fin_= max(idProducto) from #totProductos

						--while ( @ini_ <= @fin_ )
						--	begin
						--		declare	@cantidadProductos as int = 0
						--		select	@cantidadProductos = cantidad from #totProductos where idProducto = @ini_

						--		while ( @cantidadProductos > 0 )
						--			begin
						--				declare @cantidadDest as int = 0, @idInventarioDetalle as int = 0
						--				select	@cantidadDest = coalesce(max(cantidad), 0) from #tempExistencias where idProducto = @ini_ and cantidadDescontada = 0
						--				select	@idInventarioDetalle = idInventarioDetalle from #tempExistencias where idProducto = @ini_ and cantidadDescontada = 0 and cantidad = @cantidadDest

						--				-- si ya no hay ubicaciones con existencias a descontar
						--				if ( @cantidadDest = 0 )
						--					begin
						--						update  #tempExistencias set cantidadDescontada = (select cantidad from #totProductos where idProducto = @ini_)
						--						where	idProducto = @ini_
						--						select @cantidadProductos = 0
						--					end
						--				else
						--					begin
						--						if ( @cantidadDest >= @cantidadProductos )
						--							begin 
						--								update #tempExistencias set cantidadDescontada = @cantidadProductos 
						--								where idProducto = @ini_ and idInventarioDetalle = @idInventarioDetalle
						--								select @cantidadProductos = 0 
						--							end
						--						else
						--							begin
						--								update	#tempExistencias set cantidadDescontada = @cantidadDest
						--								where	 idProducto = @ini_ and idInventarioDetalle = @idInventarioDetalle
						--								select @cantidadProductos = @cantidadProductos - @cantidadDest						
						--							end
						--					end 
						--			end

						--		select @ini_ = min(idProducto) from #totProductos where idProducto > @ini_

						--	end  -- while ( @ini_ <= @fin_ )

							
						--	update	#tempExistencias
						--	set		cantidadFinal = cantidad - cantidadDescontada

						--	-- si el inventario de los productos vendidos queda negativo se paso de productos = rollback
						--	if  exists	( select 1 from #tempExistencias where cantidadFinal < 0 )
						--	begin
						--		select @mensaje = 'No se realizo la venta, no se cuenta con suficientes existencias en el inventario.'
						--		raiserror (@mensaje, 11, -1)
						--	end

						--	-- se descuentan existencias de almacen origen
						--	--se actualiza inventario_general_log
						--	INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
						--	select a.idProducto,sum(a.cantidad),b.cantidad-sum(a.cantidad),dbo.FechaActual(),17 
						--	from #totProductos a
						--	join InventarioGeneral b on a.idProducto=b.idProducto
						--	group by a.idProducto,b.cantidad
					

						--	-- se actualiza inventario detalle
						--	update	InventarioDetalle
						--	set		cantidad = a.cantidadFinal,
						--			fechaActualizacion = dbo.FechaActual()
						--	from	(
						--				select	idInventarioDetalle, idProducto, cantidad, idUbicacion, cantidadDescontada, cantidadFinal
						--				from	#tempExistencias
						--			)A
						--	where	InventarioDetalle.idInventarioDetalle = a.idInventarioDetalle

						--	-- se actualiza el inventario general
						--	update	InventarioGeneral
						--	set		InventarioGeneral.cantidad = InventarioGeneral.cantidad - A.cantidad,
						--			fechaUltimaActualizacion = dbo.FechaActual()
						--	from	(
						--				select idProducto, cantidad from #totProductos
						--			)A
						--	where InventarioGeneral.idProducto = A.idProducto


						--	-- se inserta el InventarioDetalleLog
						--	insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idPedidoInterno)
						--	select	idUbicacion, idProducto, cantidadDescontada, cantidadFinal, cast(17 as int) as idTipoMovInventario,
						--			@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idPedidoEspecial as idPedidoInterno
						--	from	#tempExistencias
						--	where	cantidadDescontada > 0


						--	-- se agregann existencias a almacen destino en la ubicacion sin ubicacion
						--	if not exists	(
						--						select	1
						--						from	Ubicacion 
						--						where	idAlmacen = @idAlmacenOrigen
						--							and	idPasillo = 0
						--							and idRaq = 0
						--							and idPiso = 0
						--					)
						--		begin
						--			insert into Ubicacion (idAlmacen,idPasillo,idRaq,idPiso) values (@idAlmacenOrigen, 0, 0, 0)
						--		end

						--	select	@idUbicacionDestino = idUbicacion 
						--	from	Ubicacion 
						--	where	idAlmacen = @idAlmacenOrigen
						--		and	idPasillo = 0
						--		and idRaq = 0
						--		and idPiso = 0

								
						--	select	idProducto , @idUbicacionDestino as idUbicacionDestino, cantidad
						--	into	#ubicacionesDestino
						--	from	#pedidos
	

						--	-- si no existen las ubicaciones de los productos en almacen destino
						--	if  exists	(
						--					select	ud.idProducto as idProductoDestino
						--					from	#ubicacionesDestino ud
						--								left join InventarioDetalle id
						--									on	id.idProducto = ud.idProducto 
						--									and	id.idUbicacion = ud.idUbicacionDestino
						--					where	id.idProducto is null 
						--				)
						--		begin
						--			insert into InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
						--			select	ud.idProducto as idProductoDestino, 0 as cantidad, @fecha, @idUbicacionDestino, @fecha
						--			from	#ubicacionesDestino ud
						--						left join InventarioDetalle id
						--							on	id.idProducto = ud.idProducto 
						--							and	id.idUbicacion = ud.idUbicacionDestino
						--			where	id.idProducto is null 
						--		end


						--	--se actualiza inventario_general_log
						--	INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
						--	select a.idProducto,sum(a.cantidad),b.cantidad + sum(a.cantidad),dbo.FechaActual(),18 
						--	from #totProductos a
						--	join InventarioGeneral b on a.idProducto=b.idProducto
						--	group by a.idProducto,b.cantidad
					

						--	-- se actualiza inventario detalle
						--	update	InventarioDetalle
						--	set		cantidad = a.cantidadFinal,
						--			fechaActualizacion = dbo.FechaActual()
						--	from	(
						--				select	p.idProducto, ( p.cantidad + id.cantidad ) as cantidadFinal, id.idUbicacion
						--				from	#ubicacionesDestino p
						--							inner join InventarioDetalle id 
						--								on id.idProducto = p.idProducto 
						--				where	id.idUbicacion = @idUbicacionDestino
						--			)A
						--	where	InventarioDetalle.idProducto = a.idProducto
						--		and	InventarioDetalle.idUbicacion = a.idUbicacion

						--	-- se actualiza el inventario general
						--	update	InventarioGeneral
						--	set		InventarioGeneral.cantidad = InventarioGeneral.cantidad + A.cantidad,
						--			fechaUltimaActualizacion = dbo.FechaActual()
						--	from	(
						--				select idProducto, cantidad from #totProductos
						--			)A
						--	where InventarioGeneral.idProducto = A.idProducto


						--	 --se inserta el InventarioDetalleLog
						--	insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idPedidoInterno)
						--	select	@idUbicacionDestino, t.idProducto, cantidadDescontada, id.cantidad, cast(18 as int) as idTipoMovInventario,
						--			@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idPedidoEspecial as idPedidoInterno
						--	from	#tempExistencias t
						--				inner join InventarioDetalle id
						--				on t.idProducto = id.idProducto										
						--	where	cantidadDescontada > 0
						--		and id.idUbicacion = @idUbicacionDestino
							
						
						select @nombreAlmacenAtiende = descripcion from Almacenes where idAlmacen = @idAlmacenAtiende
						select @mensaje = @mensaje + ' Sera atendido por: ' + @nombreAlmacenAtiende

						drop table #pedidos
						drop table #productos

					end -- si es nuevo
				else
					begin

						print 'edicion'

						-- universo de pedidos
						insert into #pedidos ( idProducto, cantidad )
						select	p.idProducto, coalesce( sum(c.cantidad), 0 )  as cantidad
						from	#cantidades c
								inner join #idProductos p_
									on c.id = p_.id
								inner join Productos p
									on p.idProducto = p_.idProducto
						group by p.idProducto
							

						-- universo de productos por idAlmacen
						insert into #productos (idProducto, idAlmacen, cantidad)
						select	id.idProducto, u.idAlmacen, coalesce( (sum(id.cantidad)), 0) as cantidad
						from	InventarioDetalle id
									inner join Ubicacion u
										on id.idUbicacion =u.idUbicacion
									inner join Productos p
										on p.idProducto = id.idProducto
									inner join Almacenes a
										on a.idAlmacen = u.idAlmacen
						where	id.idproducto in (select distinct idProducto from #pedidos )
							and	a.idTipoAlmacen in (1,2) -- Almacen General y Bodega
							and	a.idAlmacen not in (@idAlmacenOrigen)
						group by id.idProducto, u.idAlmacen, p.descripcion, p.precioIndividual,p.precioMenudeo
						order by idProducto
				
				
				
						-- buscamos quien tiene inventario para surtirlo
						-- eliminamos lo que no son tienen existencias para surtir el pedido 
						delete	#productos
						where	idAlmacen in	(
													select	pro.idAlmacen
													from	#pedidos p 
																inner join #productos pro 
																	on p.idProducto = pro.idProducto 
													where	pro.cantidad-p.cantidad <0
												)
										

						-- eliminamos las que no contienen todos los productos del pedido
						select @ini = min(idAlmacen) from #productos
						select @fin = max(idAlmacen) from #productos

				
						while ( @ini <= @fin )
							begin
						
								select '#productos'+CAST(@ini as varchar(10)) as _vuelta , * into #tempProductos_ from #productos  where idAlmacen = @ini
					
								-- si no cuenta con todas las existencias 
								if exists ( select * from #pedidos p left join #tempProductos_ t on p.idProducto = t.idProducto where t.idProducto is null )
									begin 
										--select 'elimina '+  CAST(@ini as varchar(10))  --para debuguear
										delete from #productos where idAlmacen = @ini
									end

								select @ini = min(idAlmacen) from #productos where idAlmacen > @ini
								drop table #tempProductos_

							end -- while ( @ini <= @fin )

						if not exists ( select 1 from #productos )
							begin
								select @mensaje = 'No hay Almacenes que puedan atender el pedido.'
								raiserror (@mensaje, 11, -1)
							end

						-- seleccionamos el almacen que atendera el pedido (aquel que tenga mas existencias en total)
						select	top 1 @idAlmacenAtiende = idAlmacen
						from	#productos 
						group by idAlmacen
						order by SUM(cantidad) desc
										
						
						-- si todo bien
						delete from PedidosInternosDetalle where idPedidoInterno = @idPedidoEspecial
						delete from PedidosInternosLog where idPedidoInterno = @idPedidoEspecial
						delete from MovimientosDeMercancia where idPedidoInterno = @idPedidoEspecial
						
						
						update	PedidosInternos
						set		idAlmacenOrigen = @idAlmacenOrigen,
								idAlmacenDestino = @idAlmacenAtiende,
								idUsuario = @idUsuario,
								IdEstatusPedidoInterno = 1,
								fechaAlta = @fecha,
								idTipoPedidoInterno = 2,
								descripcion = @descripcion
						where	idPedidoInterno = @idPedidoEspecial
						
						
						insert	into PedidosInternosDetalle (idPedidoInterno,idProducto, cantidad,fechaAlta,cantidadAtendida,cantidadAceptada,cantidadRechazada) 
						select	@idPedidoEspecial as idPedidoInterno, idProducto, cantidad, @fecha as fechaAlta, 0 as cantidadAtendida, 0 as cantidadAceptada, 0 as cantidadRechazada
						from	#pedidos

						insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta)
						select	@idPedidoEspecial, @idAlmacenOrigen, idAlmacenDestino, @idUsuario, cast(1 as int ) as  IdEstatusPedidoInterno, @fecha
						from	PedidosInternos 
						where	idPedidoInterno = @idPedidoEspecial


						-- se insertan los movimientos de los productos al almacen origen
						insert into MovimientosDeMercancia (idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoInterno,idUsuario,fechaAlta,idEstatusPedidoInterno,observaciones,cantidadAtendida)
						select	@idAlmacenOrigen as idAlmacenDestino, @idAlmacenAtiende as idAlmacenDestino, idProducto, cantidad, @idPedidoEspecial as  idPedidoInterno,
								@idUsuario as idUsuario, @fecha as fechaAlta, 1 as idEstatusPedidoInterno, null as observaciones, cantidad as cantidadAtendida
						from	#pedidos
						
						
						select @nombreAlmacenAtiende = descripcion from Almacenes where idAlmacen = @idAlmacenAtiende
						select @mensaje = @mensaje + ' Sera atendido por: ' + @nombreAlmacenAtiende

						drop table #pedidos
						drop table #productos

					end

				



				begin -- commit de transaccion
					if @tran_count = 0
						begin -- si la transacción se inició dentro de este ámbito
							commit tran @tran_name
							select @tran_scope = 0
						end -- si la transacción se inició dentro de este ámbito
				end -- commit de transaccion
					
				drop table #cantidades
				drop table #idProductos
				
			end -- principal

		end try

		begin catch 
		
			-- captura del error
			select	@status =			-error_state(),
					@error_procedure =	error_procedure(),
					@error_line =		error_line(),
					@mensaje =			error_message()

			-- revertir transacción si es necesario
			if @tran_scope = 1
				rollback tran @tran_name
					
		end catch

		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje,
					@idPedidoEspecial as idPedidoEspecial

		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_GUARDA_PEDIDO_ESPECIAL to public

go

-- se crea procedimiento SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS
if exists (select * from sysobjects where name like 'SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS' and xtype = 'p' )
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
							cantidadRechazada					int,
							observaciones						varchar(255)
						)
						

				create table 
					#ProductosPorPrecio
						(
							contador				int,
							idProducto				int,
							cantidad				int,
							min						int,
							max						int,
							max_cantidad			int,
							costo					money,
							precioIndividual		money,
							descuento				money,
							activo					bit
						)


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
						idTipoPedidoInterno, pi_.descripcion, idPedidoInternoDetalle, pid.idProducto, pro.descripcion as descProducto, pid.cantidadAceptada as cantidad, 
						cantidadAceptada, cantidadAtendida, cantidadRechazada
				from	PedidosInternos pi_
							inner join PedidosInternosDetalle pid
								on pid.idPedidoInterno = pi_.idPedidoInterno
							inner join Productos pro
								on pro.idProducto = pid.idProducto
				where	pi_.idPedidoInterno = @idPedidoEspecial


			-- insertamos las observaciones de movimientos de mercancia
			update	#productos
			set		#productos.observaciones = a.observaciones
			from	(
						select	mm.idPedidoInterno, mm.idProducto, pid.idPedidoInternoDetalle, pid.cantidadRechazada, mm.observaciones
						from	MovimientosDeMercancia mm
									inner join PedidosInternosDetalle pid
										on pid.idPedidoInterno = mm.idPedidoInterno
						where	mm.idPedidoInterno = @idPedidoEspecial
							and	pid.cantidadRechazada > 0
							and mm.idEstatusPedidoInterno in (3,4)
					)A
			where	#productos.idProducto = a.idProducto


			-- universo de productos
			insert into 
				#ProductosPorPrecio
					(idProducto,cantidad,min,max,costo,precioIndividual,activo)
			select	p.idProducto, sum(p.cantidad) as cantidad, cast(1 as int) as min_,				
					cast(11 as int) as max_, pro.precioIndividual, pro.precioIndividual, pro.activo
			from	#productos p
						inner join Productos pro
							on pro.idProducto=p.idProducto
			group by p.idProducto, pro.precioIndividual, pro.activo


			-- actualizamos el contador del max_cantidad para el caso de infinito
			update	#ProductosPorPrecio
			set		#ProductosPorPrecio.max_cantidad = A.max_cantidad
			from	(
						select	ppp.idProducto, 
								coalesce(max(ppp.max),0) as max_cantidad 
						from	ProductosPorPrecio ppp
									inner join #ProductosPorPrecio ppp_
										on ppp.idProducto = ppp_.idProducto
						where	ppp.activo = cast(1 as bit)
						group by ppp.idProducto
					)A
			where	#ProductosPorPrecio.idProducto = A.idProducto
			
			-- si se ejecuta precio de mayoreo cuando el ticket tiene 12 o + articulos
			if ( (select sum(cantidad) from #ProductosPorPrecio ) >= 12 )
				begin

					update	#ProductosPorPrecio 
					set		#ProductosPorPrecio.costo = a.precioMenudeo,
							min = 12, 
							max = 12
					from	(
								select	ppp.idProducto, p.precioIndividual, p.precioMenudeo
								from	#ProductosPorPrecio ppp 
											inner join Productos p 
												on ppp.idProducto = p.idProducto
							)A
					where	#ProductosPorPrecio.idProducto = a.idProducto
				
				end

				
			-- actualizamos los que caigan en un rango
			update	#ProductosPorPrecio
			set		#ProductosPorPrecio.min = a.min,
					#ProductosPorPrecio.max = a.max,
					#ProductosPorPrecio.costo = a.costo,
					#ProductosPorPrecio.contador = a.contador
			from	(
						select	ppp.contador, ppp.idProducto, ppp.min, ppp.max, ppp.costo, ppp.activo
						from	#ProductosPorPrecio ppp_
									inner join ProductosPorPrecio ppp
										on ppp.idProducto = ppp_.idProducto
						where	ppp_.cantidad between ppp.min and ppp.max
							and	ppp.activo = cast(1 as bit)
					)A
			where	#ProductosPorPrecio.idProducto = a.idProducto

			-- si hay max-cantidad
			if exists ( select 1 from #ProductosPorPrecio where cantidad > max_cantidad )
				begin
					
					update	#ProductosPorPrecio
					set		#ProductosPorPrecio.min = a.min,
							#ProductosPorPrecio.max = a.max,
							#ProductosPorPrecio.costo = a.costo,
							#ProductosPorPrecio.contador = a.contador
					from	(
								select	ppp.contador, ppp.idProducto, ppp.min, ppp.max, ppp.costo, ppp.activo
								from	#ProductosPorPrecio ppp_
											inner join ProductosPorPrecio ppp
												on ppp.idProducto = ppp_.idProducto
												and ppp_.max_cantidad = ppp.max
								where	ppp.activo = cast(1 as bit)
									and ppp_.cantidad > ppp_.max_cantidad
							)A
					where	#ProductosPorPrecio.idProducto = a.idProducto

				end

			-- actualizamos los descuentos
				update	#ProductosPorPrecio set descuento = precioIndividual - costo

		
			--select '#ProductosPorPrecio', * from #ProductosPorPrecio















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
					select	* , ppp.min, ppp.max, ppp.costo, ppp.descuento, (ppp.costo*p.cantidad) as monto, ( ppp.descuento*p.cantidad ) as ahorro 
					from	#productos p
								inner join #ProductosPorPrecio ppp 
									on p.idProducto = ppp.idProducto
				end
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS to public

go

-- se crea procedimiento SP_CONSULTA_PEDIDOS_ESPECIALES
if exists (select * from sysobjects where name like 'SP_CONSULTA_PEDIDOS_ESPECIALES' and xtype = 'p' )
	drop proc SP_CONSULTA_PEDIDOS_ESPECIALES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los pedidos especiales del sistema
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_PEDIDOS_ESPECIALES

	@IdEstatusPedidoInterno			int=NULL,
	@idAlmancenOrigen				int=NULL,
	@idAlmacenDestino				int=NULL,
	@idUsuario						int=NULL,
	@fechaIni						date=NULL,
	@fechaFin						date=NULL,
	@idPedidoInterno				int=NULL,
	@descripcion					varchar(300)=NULL,
	@idTipoPedidoInterno			int = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit),
						@top					bigint = 0x7fffffffffffffff --valor màximo


			end  --declaraciones 

			begin -- principal
				
				-- validaciones
					if	(	
							@IdEstatusPedidoInterno is null and
							@idAlmancenOrigen is null and
							@idAlmacenDestino is null and
							@idUsuario is null and
							@fechaIni is null and
							@fechaFin is null and
							@idPedidoInterno is null and
							@descripcion is null and 
							@idTipoPedidoInterno is null 
						)
						begin
							select @top=50
						end

				
					SELECT	top (@top) 
							pe.idPedidoInterno, pe.fechaAlta, pe.idTipoPedidoInterno,
							pe.idAlmacenOrigen,ao.Descripcion almacenOrigen,
							pe.idAlmacenDestino,ad.Descripcion almacenDestino,
							pe.idUsuario,coalesce(u.nombre,'') + ' ' + coalesce(u.apellidoPaterno,'') + ' ' + coalesce(u.apellidoMaterno,'') nombreCompleto,
							pe.IdEstatusPedidoInterno idStatus,s.descripcion as descripcionEstatus,
							pe.descripcion as descripcionPedido,
							cast(0 as int) as cantidad
					INTO	#pedidosEspeciales
					FROM	PedidosInternos pe
								join CatEstatusPedidoInterno s 
									on pe.IdEstatusPedidoInterno=s.IdEstatusPedidoInterno
								join Almacenes ao 
									on pe.idAlmacenOrigen=ao.idAlmacen
								join Almacenes ad 
									on pe.idAlmacenDestino=ad.idAlmacen
								join Usuarios u 
									on pe.idUsuario=u.idUsuario
					where	pe.idPedidoInterno = coalesce(@idPedidoInterno,pe.idPedidoInterno) 
						and	pe.IdEstatusPedidoInterno = coalesce(@IdEstatusPedidoInterno,pe.idestatuspedidointerno)
						and pe.idAlmacenOrigen = coalesce(@idAlmancenOrigen,pe.idAlmacenOrigen)
						and pe.idAlmacenDestino = coalesce(@idAlmacenDestino,pe.idAlmacenDestino)
						and pe.idUsuario = coalesce(@idUsuario,pe.idUsuario)
						and cast(pe.fechaAlta as date) >=coalesce(cast(@fechaIni as date),cast(pe.fechaAlta as date))
						and cast(pe.fechaAlta as date) <=coalesce(cast(@fechaFin as date),cast(pe.fechaAlta as date))
						and pe.idTipoPedidoInterno = coalesce(@idTipoPedidoInterno, pe.idTipoPedidoInterno)
					order by fechaAlta desc


					if ( @descripcion is not null )
						begin
						
							delete from #pedidosEspeciales 
							where idPedidoInterno not in	(	
																select idPedidoInterno 
																from	#pedidosEspeciales 
																where  descripcionPedido like '%' + @descripcion + '%' 
															)

						end


					--select * from #pedidosEspeciales
					
					update	#pedidosEspeciales
					set		cantidad = a.cantidad
					from	(
								select	pe.idPedidoInterno, sum( pid.cantidad) as cantidad
								from	#pedidosEspeciales pe
											inner join PedidosInternosDetalle pid
												on pe.idPedidoInterno = pid.idPedidoInterno
								where	pe.idTipoPedidoInterno = @idTipoPedidoInterno
								group by pe.idPedidoInterno
							)A
					where	#pedidosEspeciales.idPedidoInterno = a.idPedidoInterno



					if not exists (select 1 from #pedidosEspeciales)
					begin
						select	@valido = cast(0 as bit),
								@status = -1,
								@mensaje = 'No se encontraron pedidos especiales con esos términos de búsqueda.'
					end


			end -- principal

		end try

		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@mensaje = error_message()
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje

			select	idPedidoInterno as idPedidoEspecial,
					fechaAlta,
					coalesce(descripcionPedido, 'sin descripcion') as descripcion,
					cantidad,
					idAlmacenOrigen,
					idAlmacenOrigen as idAlmacen,
					almacenOrigen as descripcion,
					idAlmacenDestino,
					idAlmacenDestino as idalmacen,
					almacenDestino as descripcion,
					idUsuario,
					nombreCompleto,
					idStatus, 
					descripcionEstatus as descripcion
			from	#pedidosEspeciales
			
					
		end -- reporte de estatus
		

	end  -- principal
go

grant exec on SP_CONSULTA_PEDIDOS_ESPECIALES to public
go

-- se crea procedimiento SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL
if exists (select * from sysobjects where name like 'SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL' and xtype = 'p')
	drop proc SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/09/10
Objetivo		acepta o guarda el pedido especial
status			200 = ok
				-1	= error
*/

create proc SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL

	@xml					AS XML, 
	@idPedidoEspecial		int,
	@idUsuario				int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Se actualizo el pedido especial correctamente,',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@tran_name				varchar(32) = 'actualizaPedidoEspecial',
						@tran_count				int = @@trancount,
						@tran_scope				bit = 0,		
						@idAlmacenOrigen		int = 0,		
						@idUbicacionOrigen		int = 0,
						@idUbicacionDestino		int = 0,
						@fecha					datetime

				create table 
					#cantidades 
						(  
							id			int identity(1,1),
							cantidad	int
						)

				create table 
					#idProductos 
						(  
							id			int identity(1,1),
							idProducto	int
						)

				create table 
					#idPedidoInternoDetalles 
						(  
							id						int identity(1,1),
							idPedidoInternoDetalle	int
						)

				create table 
					#cantidadesAtendidas
						(  
							id						int identity(1,1),
							cantidadAtendida		int
						)

				create table 
					#cantidadesAceptadas
						(  
							id						int identity(1,1),
							cantidadAceptada		int
						)


				create table 
					#cantidadesRechazadas
						(  
							id						int identity(1,1),
							cantidadRechazada		int
						)

				create table 
					#observaciones
						(  
							id						int identity(1,1),
							observacion				varchar(300)
						)

			end  --declaraciones 

			begin -- principal

				begin -- inicio transaccion
					if @tran_count = 0
						begin tran @tran_name
					else
						save tran @tran_name
					select @tran_scope = 1
				end -- inicio transaccion


				select @fecha = coalesce(@fecha, dbo.FechaActual())

				insert into #cantidades (cantidad)
				SELECT Producto.cantidad.value('.','NVARCHAR(200)') AS cantidad FROM @xml.nodes('//cantidad') as Producto(cantidad) 

				insert into #idProductos (idProducto)
				SELECT Producto.idProducto.value('.','NVARCHAR(200)') AS idProducto FROM @xml.nodes('//idProducto') as Producto(idProducto)

				insert into #idPedidoInternoDetalles (idPedidoInternoDetalle)
				SELECT Producto.idPedidoInternoDetalle.value('.','NVARCHAR(200)') AS idPedidoInternoDetalle FROM @xml.nodes('//idPedidoInternoDetalle') as Producto(idPedidoInternoDetalle)
				
				insert into #cantidadesAtendidas (cantidadAtendida)
				SELECT Producto.cantidadAtendida.value('.','NVARCHAR(200)') AS cantidadAtendida FROM @xml.nodes('//cantidadAtendida') as Producto(cantidadAtendida)

				insert into #cantidadesAceptadas (cantidadAceptada)
				SELECT Producto.cantidadAceptada.value('.','NVARCHAR(200)') AS cantidadAceptada FROM @xml.nodes('//cantidadAceptada') as Producto(cantidadAceptada)

				insert into #cantidadesRechazadas (cantidadRechazada)
				SELECT Producto.cantidadRechazada.value('.','NVARCHAR(200)') AS cantidadRechazada FROM @xml.nodes('//cantidadRechazada') as Producto(cantidadRechazada)

				insert into #observaciones (observacion)
				SELECT Producto.observacion.value('.','NVARCHAR(200)') AS observacion FROM @xml.nodes('//observacion') as Producto(observacion)
				
				select	@idAlmacenOrigen = idAlmacenOrigen ,
						@idUbicacionDestino = idAlmacenDestino
				from	PedidosInternos 
				where	idPedidoInterno = @idPedidoEspecial
				
				-- universo de pedidos especiales detalle
				select	@idPedidoEspecial as idPedidoInterno, pe.idPedidoInternoDetalle, p.idProducto, c.cantidad, cat.cantidadAtendida, cac.cantidadAceptada, car.cantidadRechazada,
						case
							when o.observacion = '0' then null
							else o.observacion
						end as observacion
				into	#pedido
				from	#idProductos p 
							inner join #cantidades c
								on p.id = c.id
							inner join #idPedidoInternoDetalles pe
								on pe.id = c.id
							inner join #cantidadesAceptadas cac
								on cac.id = c.id
							inner join #cantidadesAtendidas cat
								on cat.id = c.id
							inner join #cantidadesRechazadas car
								on car.id = c.id
							inner join #observaciones o
								on o.id = p.id
								
				--select * from #pedido

				begin  -- validaciones

					if not exists ( select 1 from PedidosInternos where idPedidoInterno = @idPedidoEspecial and IdEstatusPedidoInterno = 2 )
						begin
							select @mensaje = 'No es posible aceptar o rechazar el pedido si no ha sido atendido.'
							raiserror (@mensaje, 11, -1)
						end

					if exists	(
									select	1
									from	#pedido
									where	cantidadAtendida <> (cantidadAceptada+cantidadRechazada)
								)
						begin
							select @mensaje = 'No estan aceptados/rechazados todos los productos atendidos.'
							raiserror (@mensaje, 11, -1)
						end


				end  -- validaciones



				-- actualizamos estatus de PedidosInternos y PedidosInternosLog
				-- si se rechazo toda
				if	( 
						(select sum(cantidadAtendida) from #pedido ) =
						(select sum(cantidadRechazada) from #pedido ) 
					)
					begin
						update	PedidosInternos
						set		IdEstatusPedidoInterno = 3 --Pedido Rechazado
						where	idPedidoInterno = @idPedidoEspecial

						insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta)
						select	@idPedidoEspecial, idAlmacenDestino, idAlmacenOrigen, @idUsuario, cast(3 as int) as IdEstatusPedidoInterno, @fecha
						from	PedidosInternos
						where	idPedidoInterno = @idPedidoEspecial
					end
				else
					begin
						update	PedidosInternos
						set		IdEstatusPedidoInterno = 4 --Pedido Finalizado - puede tener productos rechazados
						where	idPedidoInterno = @idPedidoEspecial

						insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta)
						select	@idPedidoEspecial, idAlmacenDestino, idAlmacenOrigen, @idUsuario, cast(4 as int) as IdEstatusPedidoInterno, @fecha
						from	PedidosInternos
						where	idPedidoInterno = @idPedidoEspecial
					end

				
				--PedidosInternosDetalle
				update	PedidosInternosDetalle
				set		PedidosInternosDetalle.cantidadAceptada = a.cantidadAceptada,
						PedidosInternosDetalle.cantidadRechazada = a.cantidadRechazada						
				from	(
							select idPedidoInternoDetalle, idProducto, cantidadAtendida, cantidadAceptada, cantidadRechazada, observacion from #pedido
						)A
				where	PedidosInternosDetalle.idPedidoInternoDetalle = a.idPedidoInternoDetalle
				
				
				if exists ( select 1 from PedidosInternosDetalle where idPedidoInterno = @idPedidoEspecial and cantidadRechazada < 0 )
					begin
							select @mensaje = 'No se pueden aceptar/rechazar mas productos de los que fueron atendidos.'
							raiserror (@mensaje, 11, -1)
					end


				--MovimientosDeMercancia
				-- las aceptadas
				insert into MovimientosDeMercancia (idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoInterno,idUsuario,fechaAlta,idEstatusPedidoInterno,observaciones,cantidadAtendida)
				select	idAlmacenDestino, idAlmacenOrigen,  p.idProducto, pid.cantidad, p.idPedidoInterno , @idUsuario, @fecha, cast(4 as int) as IdEstatusPedidoInterno, 
						p.observacion, p.cantidadAceptada--p.cantidadAtendida
				from	PedidosInternos pi_
							inner join #pedido p
								on p.idPedidoInterno = pi_.idPedidoInterno
						inner join PedidosInternosDetalle pid 
							on pid.idPedidoInternoDetalle=p.idPedidoInternoDetalle
				where	pi_.idPedidoInterno = @idPedidoEspecial
					and	p.cantidadAceptada > 0

				-- las rechazadas 
				insert into MovimientosDeMercancia (idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoInterno,idUsuario,fechaAlta,idEstatusPedidoInterno,observaciones,cantidadAtendida)
				select	idAlmacenDestino,idAlmacenOrigen , p.idProducto, pid.cantidad, p.idPedidoInterno , @idUsuario, @fecha, cast(3 as int) as IdEstatusPedidoInterno, 
						p.observacion, p.cantidadRechazada--p.cantidadAtendida
				from	PedidosInternos pi_
							inner join #pedido p
								on p.idPedidoInterno = pi_.idPedidoInterno
							inner join PedidosInternosDetalle pid 
								on pid.idPedidoInternoDetalle=p.idPedidoInternoDetalle
				where	pi_.idPedidoInterno = @idPedidoEspecial
					and	p.cantidadRechazada > 0



				-- se agregann existencias a almacen origen en la ubicacion sin ubicacion
				if not exists	(
									select	1
									from	Ubicacion 
									where	idAlmacen = @idAlmacenOrigen
										and	idPasillo = 0
										and idRaq = 0
										and idPiso = 0
								)
					begin
						insert into Ubicacion (idAlmacen,idPasillo,idRaq,idPiso) values (@idAlmacenOrigen, 0, 0, 0)
					end

				select	@idUbicacionOrigen = idUbicacion 
				from	Ubicacion 
				where	idAlmacen = @idAlmacenOrigen
					and	idPasillo = 0
					and idRaq = 0
					and idPiso = 0

								
				select	idProducto , @idUbicacionOrigen as idUbicacionOrigen, cantidadAceptada, cantidadRechazada
				into	#ubicacionesOrigen
				from	#pedido
	

				-- si no existen las ubicaciones de los productos en almacen origen
				if  exists	(
								select	ud.idProducto as idProductoDestino
								from	#ubicacionesOrigen ud
											left join InventarioDetalle id
												on	id.idProducto = ud.idProducto 
												and	id.idUbicacion = ud.idUbicacionOrigen
								where	id.idProducto is null 
							)
					begin
						insert into InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
						select	uo.idProducto as idProductoDestino, 0 as cantidad, @fecha, @idUbicacionOrigen, @fecha
						from	#ubicacionesOrigen uo
									left join InventarioDetalle id
										on	id.idProducto = uo.idProducto 
										and	id.idUbicacion = uo.idUbicacionOrigen
						where	id.idProducto is null 
					end


				----se actualiza inventario_general_log aceptadas
				--INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
				--select	a.idProducto,sum(a.cantidadAceptada),b.cantidad + sum(a.cantidadAceptada),dbo.FechaActual(),18 
				--from	#pedido a
				--			join InventarioGeneral b 
				--				on a.idProducto=b.idProducto
				--where	a.cantidadAceptada > 0
				--group by a.idProducto,b.cantidad
					
				----se actualiza inventario_general_log rechazadas
				--INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
				--select	a.idProducto,sum(a.cantidadRechazada),b.cantidad + sum(a.cantidadRechazada),dbo.FechaActual(),18 
				--from	#pedido a
				--			join InventarioGeneral b 
				--				on a.idProducto=b.idProducto
				--where	a.cantidadRechazada > 0
				--group by a.idProducto,b.cantidad

				---- se actualiza el inventario general 
				--update	InventarioGeneral
				--set		InventarioGeneral.cantidad = InventarioGeneral.cantidad + A.cantidad,
				--		fechaUltimaActualizacion = dbo.FechaActual()
				--from	(
				--			select idProducto, (cantidadAceptada + cantidadRechazada ) as cantidad from #pedido
				--		)A
				--where InventarioGeneral.idProducto = A.idProducto

				
				-- se actualiza inventario detalle aceptada
				update	InventarioDetalle
				set		cantidad = a.cantidadFinal,
						fechaActualizacion = dbo.FechaActual()
				from	(
							select	p.idProducto, ( p.cantidadAceptada + id.cantidad ) as cantidadFinal, id.idUbicacion
							from	#ubicacionesOrigen p
										inner join InventarioDetalle id 
											on id.idProducto = p.idProducto 
							where	id.idUbicacion = @idUbicacionOrigen
						)A
				where	InventarioDetalle.idProducto = a.idProducto
					and	InventarioDetalle.idUbicacion = a.idUbicacion

				-- se actualiza inventario detalle rechazada
				update	InventarioDetalle
				set		cantidad = a.cantidadFinal,
						fechaActualizacion = dbo.FechaActual()
				from	(
							select	p.idProducto, ( p.cantidadRechazada + id.cantidad ) as cantidadFinal, id.idUbicacion
							from	#ubicacionesOrigen p
										inner join InventarioDetalle id 
											on id.idProducto = p.idProducto 
							where	id.idUbicacion = @idUbicacionDestino
						)A
				where	InventarioDetalle.idProducto = a.idProducto
					and	InventarioDetalle.idUbicacion = a.idUbicacion


				--se inserta el InventarioDetalleLog aceptadas
				insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idPedidoInterno)
				select	@idUbicacionOrigen, t.idProducto, cantidadAceptada, id.cantidad, cast(8 as int) as idTipoMovInventario,
						@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idPedidoEspecial as idPedidoInterno
				from	#pedido t
							inner join InventarioDetalle id
							on t.idProducto = id.idProducto										
				where	t.cantidadAceptada > 0
					and id.idUbicacion = @idUbicacionOrigen
							

				--se inserta el InventarioDetalleLog rechazadas
				insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idPedidoInterno)
				select	@idUbicacionDestino, t.idProducto, cantidadRechazada, id.cantidad, cast(10 as int) as idTipoMovInventario,
						@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idPedidoEspecial as idPedidoInterno
				from	#pedido t
							inner join InventarioDetalle id
							on t.idProducto = id.idProducto										
				where	t.cantidadRechazada > 0
					and id.idUbicacion = @idUbicacionDestino
													

		
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
					@mensaje =			error_message()

			-- revertir transacción si es necesario
			if @tran_scope = 1
				rollback tran @tran_name
					
		end catch

		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje,
					@idPedidoEspecial as idPedidoEspecial

		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL to public
go