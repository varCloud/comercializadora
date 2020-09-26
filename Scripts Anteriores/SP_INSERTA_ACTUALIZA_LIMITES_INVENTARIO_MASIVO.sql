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
	
