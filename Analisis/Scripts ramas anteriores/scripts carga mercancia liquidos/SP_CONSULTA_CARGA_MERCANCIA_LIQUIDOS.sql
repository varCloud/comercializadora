--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_CARGA_MERCANCIA_LIQUIDOS]    Script Date: 25/06/2022 08:50:06 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select * from sysobjects where name like 'SP_CONSULTA_CARGA_MERCANCIA_LIQUIDOS' and xtype = 'p' )
	drop proc SP_CONSULTA_CARGA_MERCANCIA_LIQUIDOS
go


/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2022/06/25
Objetivo		Consulta los movimientos de carga de mercancia de liquidos
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CONSULTA_CARGA_MERCANCIA_LIQUIDOS]

	@idTipoMovInventario	int = null,
	@idRol					int = null,
	@idUsuario				int = null,
	@fechaIni				datetime = null,
	@fechaFin				datetime = null
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



				create table
					#movimientos
						(
							id							int identity(1,1),
							idInventarioDetalleLOG		int,
							idUbicacion					int,
							idProducto					int,
							cantidad					float,
							idUsuario					int,
							nombreUsuario				varchar(1000),
							fechaAlta					datetime,
							idRol						int,
							descripcionRol				varchar(200),
							idTipoMovInventario			int,
							descTipoMovInventario		varchar(200)
						)

			end  --declaraciones 

			begin -- principal
				
				-- validaciones
					if	(	
							@idRol		is null and
							@idUsuario	is null and
							@fechaIni	is null and
							@fechaFin	is null 
						)
						begin
							select @top=50
						end

					if (@fechaIni is null or @fechaFin is null)
					begin
						select	@fechaIni = dbo.FechaActual(),
								@fechaFin = dbo.FechaActual()
					end
				

					if ( @idTipoMovInventario is null or @idTipoMovInventario = 0 )
						begin
							insert into #movimientos (idInventarioDetalleLOG, idUbicacion, idProducto, cantidad, idUsuario, nombreUsuario, fechaAlta, idRol, descripcionRol, idTipoMovInventario, descTipoMovInventario)
							SELECT	top (@top) 
									idl.idInventarioDetalleLOG,
									idl.idUbicacion,
									idl.idProducto,
									idl.cantidad,
									idl.idUsuario,
									(u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno) as nombreUsuario,
									idl.fechaAlta,
									u.idRol,
									r.descripcion as descripcionRol,
									idl.idTipoMovInventario,
									c.descripcion as descTipoMovInventario
							from	InventarioDetalleLog idl
										join Usuarios u
											on u.idUsuario = idl.idUsuario
										join CatRoles r
											on r.idRol = u.idRol
										join CatTipoMovimientoInventario c
											on c.idTipoMovInventario = idl.idTipoMovInventario
							where	u.idUsuario = coalesce(@idUsuario, u.idUsuario)
								and idl.idTipoMovInventario in (26,27)
								and	u.idRol = coalesce(@idRol, u.idRol)
								and cast(idl.fechaAlta as date) >=coalesce(cast(@fechaIni as date),cast(idl.fechaAlta as date))
								and cast(idl.fechaAlta as date) <=coalesce(cast(@fechaFin as date),cast(idl.fechaAlta as date))
						end
					else
						begin
							insert into #movimientos (idInventarioDetalleLOG, idUbicacion, idProducto, cantidad, idUsuario, nombreUsuario, fechaAlta, idRol, descripcionRol, idTipoMovInventario, descTipoMovInventario)
							SELECT	top (@top) 
									idl.idInventarioDetalleLOG,
									idl.idUbicacion,
									idl.idProducto,
									idl.cantidad,
									idl.idUsuario,
									(u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno) as nombreUsuario,
									idl.fechaAlta,
									u.idRol,
									r.descripcion as descripcionRol,
									idl.idTipoMovInventario,
									c.descripcion as descTipoMovInventario
							from	InventarioDetalleLog idl
										join Usuarios u
											on u.idUsuario = idl.idUsuario
										join CatRoles r
											on r.idRol = u.idRol
										join CatTipoMovimientoInventario c
											on c.idTipoMovInventario = idl.idTipoMovInventario
							where	u.idUsuario = coalesce(@idUsuario, u.idUsuario)
								and idl.idTipoMovInventario = @idTipoMovInventario
								and	u.idRol = coalesce(@idRol, u.idRol)
								and cast(idl.fechaAlta as date) >=coalesce(cast(@fechaIni as date),cast(idl.fechaAlta as date))
								and cast(idl.fechaAlta as date) <=coalesce(cast(@fechaFin as date),cast(idl.fechaAlta as date))
						end

					if not exists (select 1 from #movimientos)
					begin
						select	@valido = cast(0 as bit),
								@status = -1,
								@mensaje = 'No se encontraron movimientos con esos términos de búsqueda.'
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

			if ( @valido = cast(1 as bit) )
				begin
					select	id,
							mov.idInventarioDetalleLOG, 
							mov.idUbicacion, 
							ubicaciones.descripcion as descripcionUbicacion, 
							mov.idProducto, 
							mov.cantidad,
							p.descripcion as descripcionProducto, 
							mov.idUsuario, 
							mov.nombreUsuario, 
							mov.fechaAlta, 
							mov.idRol, 
							mov.descripcionRol, 
							mov.idTipoMovInventario, 
							mov.descTipoMovInventario
					from	#movimientos mov
								join Productos p
									on p.idProducto = mov.idProducto
								join	(
											select	id.idUbicacion, u.idAlmacen, id.idProducto, a.descripcion
											from	InventarioDetalle id
														join Ubicacion u 
															on u.idUbicacion = id.idUbicacion
														join Almacenes a
															on a.idAlmacen = u.idAlmacen
										)ubicaciones
											on	ubicaciones.idUbicacion = mov.idUbicacion
											and	ubicaciones.idProducto = mov.idProducto
					order by fechaAlta

				end

			drop table #movimientos
					
		end -- reporte de estatus
		

	end  -- principal
go


grant exec on SP_CONSULTA_CARGA_MERCANCIA_LIQUIDOS to public
go
