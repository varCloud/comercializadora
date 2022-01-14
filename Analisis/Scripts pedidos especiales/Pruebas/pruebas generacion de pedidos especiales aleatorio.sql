begin tran

declare 
@idUsuario int=37,
@idEstatusPedidoEspecial int=1,
@TotalProductos int=150

declare @idCliente int=(select top 1 idCliente from clientes where idCliente>1 ORDER BY newid())
declare @idAlmacenOrigen int=(select idAlmacen from usuarios where idUsuario=@idUsuario)
declare @descuento float

select	@descuento = t.descuento
from	clientes c
		inner join CatTipoCliente t
			on c.idTipoCliente = t.idTipoCliente
where	c.idCliente = @idCliente


INSERT INTO PedidosEspeciales(idCliente,cantidad,fechaAlta,montoTotal,idUsuario,idEstatusPedidoEspecial,idEstacion)
select @idCliente,0 cantidad,dbo.fechaActual(),0 montoTotal,@idUsuario,@idEstatusPedidoEspecial,1 

declare @idPedidoEspecial int=(select max(idPedidoEspecial) from PedidosEspeciales)

INSERT INTO PedidosEspecialesDetalle(idPedidoEspecial,idProducto,idAlmacenOrigen,idAlmacenDestino,fechaAlta,cantidad,
monto,precioIndividual,precioMenudeo,precioRango,precioVenta,idTicketMayoreo,ultimoCostoCompra,idEstatusPedidoEspecialDetalle)
select top (@TotalProductos) @idPedidoEspecial idPedidoEspecial,p.idProducto,@idAlmacenOrigen idAlmacenOrigen,u.idAlmacen idAlmacenDestino ,
dbo.FechaActual() fechaAlta,FLOOR(RAND()*(i.cantidad-1)+1),
0 monto ,p.precioIndividual,p.precioMenudeo,0 precioRango,0 precioVenta,0 idTicketMayoreo,p.ultimoCostoCompra,@idEstatusPedidoEspecial idEstatusPedidoEspecialDetalle
from productos p join
inventarioDetalle i on p.idProducto=i.idProducto
join ubicacion u on i.idUbicacion=u.idUbicacion
where u.idAlmacen<>@idAlmacenOrigen and i.cantidad>0
group by p.idProducto,i.idUbicacion,i.cantidad,u.idAlmacen,p.ultimoCostoCompra,p.precioIndividual,p.precioMenudeo
ORDER BY newid()

update d set
precioRango=dbo.redondear(case when rango.precioRango is null then 0 else rango.precioRango end),
precioVenta=dbo.redondear(case when rango.precioRango is null then case when (d.cantidad) >= 6 then precioMenudeo else precioIndividual	end 
else rango.precioRango end) 
from PedidosEspecialesDetalle d 
left join (
	select	pd.idPedidoEspecialDetalle,ppp.costo as precioRango
	from	PedidosEspecialesDetalle pd
				inner join ProductosPorPrecio ppp
					on ppp.idProducto = pd.idProducto
				inner join (select idProducto,max(max) maxCantRango 
							from ProductosPorPrecio 
							where
							activo = cast(1 as bit)
							group by idProducto) maxRango 
					on ppp.idProducto=maxRango.idProducto				
	where	ppp.activo = cast(1 as bit) and pd.idPedidoEspecial=@idPedidoEspecial
		and	(pd.cantidad)>=min and (pd.cantidad)<=case when (pd.cantidad)>maxCantRango and max=maxCantRango
		then (pd.cantidad) else max end

)rango on d.idPedidoEspecialDetalle=rango.idPedidoEspecialDetalle
where d.idPedidoEspecial=@idPedidoEspecial

--si hay descuento actualizamos el precioVenta
if ( @descuento > 0.0 )
begin
	update	PedidosEspecialesDetalle set precioVenta = precioVenta - (precioVenta * ( @descuento / 100 )) where idPedidoEspecial=@idPedidoEspecial 
end

--actualizamos el monto
update	PedidosEspecialesDetalle set monto=dbo.redondear(precioVenta*cantidad) where idPedidoEspecial=@idPedidoEspecial;

--actualizamos cantidad y monto en la tabla pedidosEspeciales
update p set 
cantidad=dbo.redondear(pd.cantidad),
montoTotal=dbo.redondear(pd.monto) 
from PedidosEspeciales p
join (select sum(cantidad) cantidad ,sum(monto) monto,@idPedidoEspecial idPedidoEspecial from PedidosEspecialesDetalle where idPedidoEspecial=@idPedidoEspecial) pd on p.idPedidoEspecial=pd.idPedidoEspecial
where p.idPedidoEspecial=@idPedidoEspecial


select * from PedidosEspeciales where idPedidoEspecial=@idPedidoEspecial
select * from PedidosEspecialesDetalle where idPedidoEspecial=@idPedidoEspecial


commit tran


select * from pedidosEspeciales
