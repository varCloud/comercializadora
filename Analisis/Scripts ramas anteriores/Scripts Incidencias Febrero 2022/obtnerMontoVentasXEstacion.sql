--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  UserDefinedFunction [dbo].[obtnerMontoVentasXEstacion]    Script Date: 22/02/2022 11:13:17 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[obtnerMontoVentasXEstacion] (
@fecha_ini date,
@fecha_fin date,
@idEstacion int=null
)
RETURNS  @VentasEstacion TABLE 
(
		
		idEstacion int not null ,
		nombre varchar(200),
		monto money null,
		montoPE money null
)
AS
begin

		--select	(SUM(v.montototal) + coalesce(sum(c.montoAgregarProductos),0)) - coalesce(sum(b.monto + coalesce(b.montoDevueltoComisionBancaria,0)),0) montoCanceladas,v.idEstacion
		--INTO #VENTAS_CANCELADAS			
		--from	Ventas v
		--	left join  Devoluciones a 
		--		on v.idVenta=a.idVenta
		--	left join DevolucionesDetalle b 
		--		on a.idDevolucion=b.idDevolucion
		--	left join Complementos c 
		--		on v.idVenta=c.idVenta
		--where	v.idEstacion=coalesce(@id_estacion,v.idEstacion)
		--	and cast(fechaCancelacion as date) = cast(dbo.FechaActual() as date) 
		--	and idStatusVenta = 2
		--group by v.idEstacion

		insert into @VentasEstacion (idEstacion , nombre,monto )
		select e.idEstacion,e.nombre + ' ' + cast(e.numero as varchar) nombre,
		coalesce(montoVentas,0) + coalesce(montoComplementos,0)-coalesce(montoDev,0) monto
		from Estaciones e
		--ventas
		left join (
			select coalesce(sum((montoTotal)),0) montoVentas,idEstacion
			from Ventas 
			where idEstacion=coalesce(@idEstacion,idEstacion)
			and CAST(fechaAlta as date)>=cast(@fecha_ini as date) and CAST(fechaAlta as date)<=cast(@fecha_fin as date) and idStatusVenta=1
			group by idEstacion
		) v on e.idEstacion=v.idEstacion
		--complementos
		left join (		
		select coalesce(sum((montoAgregarProductos)),0)montoComplementos,idEstacion
		from Complementos c
		where idEstacion=coalesce(@idEstacion,idEstacion)
		and CAST(fechaAlta as date)>=cast(@fecha_ini as date) and CAST(fechaAlta as date)<=cast(@fecha_fin as date)
		group by idEstacion		
		) c on e.idEstacion=c.idEstacion
		--devoluciones
		left join (
		select sum(b.monto + coalesce(b.montoDevueltoComisionBancaria,0))  montoDev,idEstacion	
		from Devoluciones a
		join DevolucionesDetalle b 
			on a.idDevolucion=b.idDevolucion
		where idEstacion=coalesce(@idEstacion,idEstacion)
		and CAST(a.fechaAlta as date)>=cast(@fecha_ini as date) and CAST(a.fechaAlta as date)<=cast(@fecha_fin as date)
		group by idEstacion
		) d on e.idEstacion=d.idEstacion		
		where e.idEstacion=coalesce(@idEstacion,e.idEstacion)

		--Actualizamos monto de pedidos especiales
		UPDATE c SET montoPE=coalesce(total.montoTotal,0) FROM @VentasEstacion c
		left join ( 
					SELECT SUM(coalesce(T.montoTotal,0)) montoTotal , P.idEstacion
					from TicketsPedidosEspeciales T join PedidosEspeciales P on T.idPedidoEspecial = P.idPedidoEspecial
						   
					where 
						CAST(T.fechaAlta as date)>=@fecha_ini and  CAST(T.fechaAlta as date)<=@fecha_fin
						and T.idTipoTicketPedidoEspecial = 1 
						and P.idEstatusPedidoEspecial in(4,6)
						and P.idEstacion=coalesce(@idEstacion,P.idEstacion)
					group by  P.idEstacion
					)
		total on c.idEstacion=total.idEstacion

		--Actualizamos monto de pedidos especiales DEVUELTOS
		UPDATE c SET montoPE= montoPE - coalesce(total.montoTotal,0)  FROM @VentasEstacion c
		left join ( 
					SELECT SUM(coalesce(T.montoTotal,0)) montoTotal , P.idEstacion
					from TicketsPedidosEspeciales T join PedidosEspeciales P on T.idPedidoEspecial = P.idPedidoEspecial
						   
					where 
						CAST(T.fechaAlta as date)>=@fecha_ini and  CAST(T.fechaAlta as date)<=@fecha_fin
						and T.idTipoTicketPedidoEspecial = 2
						and P.idEstatusPedidoEspecial in(4,6)
						and P.idEstacion=coalesce(@idEstacion,P.idEstacion)
					group by  P.idEstacion
					)
		total on c.idEstacion=total.idEstacion


		--Actualizamos monto de abonos de pedidos especiales 
		--EN LOS ABONOS NO TENEMOS ID ESTACION QUE LO  HIZO EL MONTO DE LOS ABONOS SE LO SUMAMOS S ALA ESTACION 1
		UPDATE c SET montoPE=montoPE+(coalesce(total.montoTotal,0)) FROM @VentasEstacion c
		left join ( 
		SELECT SUM(coalesce(montoTotal,0)) montoTotal
		FROM PedidosEspecialesAbonoClientes v
		where CAST(v.fechaAlta as date)>=@fecha_ini and  CAST(v.fechaAlta as date)<=@fecha_fin
	     )total on c.idEstacion=1
				

		
return;
end
	
