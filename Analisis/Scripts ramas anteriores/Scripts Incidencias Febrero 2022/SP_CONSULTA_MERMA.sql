--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_MERMA]    Script Date: 17/02/2022 15:59:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/07/28
Objetivo		Consulta indicador de merma
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_MERMA]

/*@idInventarioFisico int=null,
@idLinea int=null,
@idAlmacen int=null*/
@mesCalculo int=null,
@anioCalculo int=null,
@idLinea int=null,
@idAlmacen int=null,
@silent int=0

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
						@valido	bit = cast(1 as bit),
						@ultimoDiaMesCalculo date,
						@ultimoDiaMesAnterior date,
						@primerDiaMesCalculo date,
						@fechaActual datetime
	
						
			end -- inicio

			begin--asignamos variables

			select @fechaActual=dbo.FechaActual()

			select @mesCalculo=coalesce(@mesCalculo,month(@fechaActual)),@anioCalculo=coalesce(@anioCalculo,year(@fechaActual))

			--construimos la fecha de calculo en base al mes y anio y el primer dia
			select @primerDiaMesCalculo=DATEFROMPARTS(@anioCalculo,@mesCalculo, 01 ); 

			--obtenemos el ultimo dia mes calculo, si la fecha actual es menos al ultimo dia del mes, se toma la fecha actual
			select @ultimoDiaMesCalculo=case when cast(@fechaActual as date)< EOMONTH(@primerDiaMesCalculo) THEN cast(@fechaActual as date) ELSE EOMONTH(@primerDiaMesCalculo) END,
				   @ultimoDiaMesAnterior=EOMONTH(@primerDiaMesCalculo,-1) --obtenemos el ultimo dia del mes anterior


			--si el mes calculado es el actual se elimina registro de reporte merma para que se vuelva a calcular,debido a que el mes todavia no se termina
			IF (MONTH(@ultimoDiaMesCalculo)=MONTH(@fechaActual) and YEAR(@ultimoDiaMesCalculo)=YEAR(@fechaActual))
			BEGIN
			DELETE ReporteMerma where MONTH(UltimoDiaMesCalculo)=MONTH(@fechaActual) and YEAR(UltimoDiaMesCalculo)=YEAR(@fechaActual)
			END

			end
			
		    
			begin
			
				--select 
				--a.idInventarioFisico,
				--cast(f.FechaFin as date) FechaAlta,
				--sum(a.totalVentas) totalVentas,sum(a.totalCompras) totalCompras,
				--sum(a.cantidadActual) cantidadActual,sum(a.cantidadEnFisico) cantidadEnFisico,sum(a.cantidadActual)-sum(a.cantidadEnFisico) cantidadAAjustar,
				--case when dbo.obtenerPrecioCompra(p.idProducto,cast(f.FechaFin as date))=0 or sum(a.totalCompras)=0 then 0 else
				--(((sum(a.cantidadActual)-sum(a.cantidadEnFisico))*dbo.obtenerPrecioCompra(p.idProducto,cast(f.FechaFin as date)))/(sum(a.totalCompras)*dbo.obtenerPrecioCompra(p.idProducto,cast(f.FechaFin as date))))*100
				--end porcMerma,
				--p.idProducto,p.codigoBarras,p.descripcion,l.descripcion DescripcionLinea,dbo.obtenerPrecioCompra(p.idProducto,cast(f.FechaFin as date)) costo
				--INTO #MERMA
				--from InventarioFisico f
				--join AjusteInventarioFisico a on f.idInventarioFisico=a.idInventarioFisico
				--join Productos p on a.idProducto=p.idProducto
				--join LineaProducto l on p.idLineaProducto=l.idLineaProducto
				--join Ubicacion u on a.idUbicacion=u.idUbicacion
				--join Almacenes al on u.idAlmacen=al.idAlmacen
				--where f.idInventarioFisico=coalesce(@idInventarioFisico,f.idInventarioFisico)
				--and f.idEstatusInventarioFisico=3
				--and p.idLineaProducto=coalesce(@idLinea,p.idLineaProducto)
				--and al.idAlmacen=coalesce(@idAlmacen,al.idAlmacen)
				--group by a.idInventarioFisico,cast(f.FechaFin as date),p.codigoBarras,p.descripcion,l.descripcion,
				--p.idProducto,l.idLineaProducto,f.idInventarioFisico
				--order by p.idProducto

				--si no existe el calculo en la tabla reporteMerma realizamos calculo
				IF NOT EXISTS(select 1 from ReporteMerma where UltimoDiaMesCalculo=@ultimoDiaMesCalculo)
				begin
				

				--Registramos los productos que tuvieron merma
				INSERT INTO ReporteMerma(idProducto,merma,ultCostoCompra,fechaAlta,UltimoDiaMesCalculo,UltimoDiaMesAnterior)
				select a.idProducto,sum(cantidadAAjustar) merma,dbo.obtenerPrecioCompra(a.idProducto,@ultimoDiaMesCalculo),@fechaActual,
				@ultimoDiaMesCalculo,@ultimoDiaMesAnterior
				from InventarioFisico i
				join AjusteInventarioFisico a on i.idInventarioFisico=a.idInventarioFisico
				join Productos p on a.idProducto=p.idProducto
				where ajustado=1 and idEstatusInventarioFisico=3 and cantidadEnFisico<cantidadActual
				and cast(i.FechaFin as date)>=@primerDiaMesCalculo and cast(i.FechaFin as date)<=@ultimoDiaMesCalculo
				group by a.idProducto

				--obtenemos el inventario final por producto del mes anterior
				UPDATE r set inventarioFinalMesAnt=cantidad
				FROM ReporteMerma r
				join(
				select ultMov.idProducto,SUM(cantidadActual) cantidad
				from InventarioDetalleLog l
				join(
				select d.idProducto,u.idAlmacen,u.idUbicacion,MAX(idInventarioDetalleLOG) idInventarioDetalleLOG
				from 
				InventarioDetalleLog d
				join Ubicacion u on d.idUbicacion=u.idUbicacion
				where  CAST(fechaAlta as date)<=CAST(@ultimoDiaMesAnterior as date)
				--and d.idProducto=coalesce(@idProducto,d.idProducto)
				--and u.idAlmacen=coalesce(@idAlmacen,u.idAlmacen)
				group by d.idProducto,u.idUbicacion,u.idAlmacen) ultMov on l.idInventarioDetalleLOG=ultMov.idInventarioDetalleLOG
				group by ultMov.idProducto) i on r.idProducto=i.idProducto
				where UltimoDiaMesCalculo=@ultimoDiaMesCalculo


				--obtenemos las compras por producto del mes calculado 
				UPDATE r set totalCompras=c.totalCompras
				FROM ReporteMerma r
				join(
				select idProducto,sum(d.cantidadRecibida) totalCompras 
				from compras c
				join comprasDetalle d on c.idCompra=d.idCompra
				where idStatusCompra in (2,3) and d.cantidadRecibida>0 and cast(d.fechaRecibio as date)>=@primerDiaMesCalculo and cast(d.fechaRecibio as date)<=@ultimoDiaMesCalculo
				group by idProducto) c on r.idProducto=c.idProducto
				where UltimoDiaMesCalculo=@ultimoDiaMesCalculo

				--actualizamos inventarioSistema (inventario final mes anterior + total de compras)
				update ReporteMerma set inventarioSistema=dbo.redondear(inventarioFinalMesAnt + totalCompras) where UltimoDiaMesCalculo=@ultimoDiaMesCalculo

				--calculamos porcentaje merma (merma/inventario sistema)
				update ReporteMerma set porcMerma=case when inventarioSistema=0 then 0 else dbo.redondear((merma/inventarioSistema)*100) end where UltimoDiaMesCalculo=@ultimoDiaMesCalculo

				--calculamos costoMerma (merma *  ultimo costo compra)
				update ReporteMerma set costoMerma=dbo.redondear(merma * ultCostoCompra) where UltimoDiaMesCalculo=@ultimoDiaMesCalculo


				end
				SELECT r.*,p.codigoBarras,p.descripcion descripcionProducto,l.idLineaProducto,l.descripcion descripcionLinea
				INTO #MERMA
				FROM ReporteMerma r
				join productos p on r.idProducto=p.idProducto
				join LineaProducto l on p.idLineaProducto=l.idLineaProducto
				where UltimoDiaMesCalculo=@ultimoDiaMesCalculo 
				and p.idLineaProducto=coalesce(@idLinea,p.idLineaProducto)
				and 1=case when coalesce(@idAlmacen,0)>0 then dbo.ExisteProductoEnAlmancen(@idAlmacen,r.idProducto) else 1 end

				if not exists (select 1 from #MERMA)
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

		    if(@silent=0) 
			begin

				select	@status status,
						@error_procedure error_procedure,
						@error_line error_line,
						@error_severity error_severity,
						@error_message mensaje

				if(@valido=1)
				select *
				from #MERMA 
			end
			
			

					
		end -- reporte de estatus
		
	end -- procedimiento
	
