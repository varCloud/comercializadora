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
	
