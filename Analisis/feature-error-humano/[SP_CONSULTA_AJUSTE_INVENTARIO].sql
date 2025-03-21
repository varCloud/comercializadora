USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_AJUSTE_INVENTARIO]    Script Date: 9/2/2023 3:25:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/07/28
Objetivo		Consulta el ajuste de inventario
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_AJUSTE_INVENTARIO]

@idInventarioFisico int=null,
@idProducto int=null,
@idLineaProducto int=null,
@idAlmacen int=null

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
			
			    select f.idAjusteInventarioFisico,cantidadActual,cantidadEnFisico,cantidadAAjustar,f.fechaAlta,coalesce(f.ajustado,cast(0 as bit)) ajustado, f.errorHumano,
				f.idInventarioFisico,i.idEstatusInventarioFisico
				,p.idProducto,p.descripcion,dbo.obtenerPrecioCompra(p.idProducto,null) ultimoCostoCompra,
				l.idLineaProducto,l.descripcion DescripcionLinea,
				a.idAlmacen,a.Descripcion Almacen,
				u.idPiso,case when u.idPiso=0 then 'SIN ACOMODAR' else pi.descripcion end Piso,
				u.idPasillo,case when u.idPasillo=0 then 'SIN ACOMODAR' else pa.descripcion end Pasillo,
				u.idRaq,case when u.idRaq=0 then 'SIN ACOMODAR' else r.descripcion end Raq,
				us.idUsuario,coalesce(us.nombre,'') + ' ' + coalesce(us.apellidoPaterno,'') + ' ' + coalesce(us.apellidoMaterno,'')  nombreCompleto			
				into #AjusteInventarioFisico
				from AjusteInventarioFisico f
				join InventarioFisico i on f.idInventarioFisico=i.idInventarioFisico
				join Productos p on f.idProducto=p.idProducto
				join LineaProducto l on p.idLineaProducto=l.idLineaProducto
				join Ubicacion u on f.idUbicacion=u.idUbicacion
				join Almacenes a on u.idAlmacen=a.idAlmacen
				join CatPiso pi on u.idPiso=pi.idPiso
				join CatPasillo pa on u.idPasillo=pa.idPasillo
				join CatRaq r on u.idRaq=r.idRaq
				left join Usuarios us on f.idUsuario=us.idUsuario
				where f.idInventarioFisico=coalesce(@idInventarioFisico,f.idInventarioFisico)
				and p.idProducto=coalesce(@idProducto,p.idProducto)
				and p.idLineaProducto=coalesce(@idLineaProducto,p.idLineaProducto)
				and a.idAlmacen=coalesce(@idAlmacen,a.idAlmacen)
				and (cantidadActual>0 or cantidadEnFisico>0)
				order by p.descripcion , f.idAjusteInventarioFisico desc

				if not exists (select 1 from #AjusteInventarioFisico)
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
			select *
			from #AjusteInventarioFisico 		
			order by idInventarioFisico desc
			
			

					
		end -- reporte de estatus
		
	end -- procedimiento
	
