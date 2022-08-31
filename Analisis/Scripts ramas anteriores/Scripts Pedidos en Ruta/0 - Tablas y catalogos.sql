

/*

select * from CatEstatusPedidoEspecial
select * from PedidosEspeciales

*/

if not exists (select 1 from CatEstatusPedidoEspecial where descripcion like 'Pedido en Ruta' )
begin
	insert into CatEstatusPedidoEspecial (descripcion) values ('Pedido en Ruta')
end
go


-- columna para observaciones del pedido si este es en ruta
if not exists ( select 1 from sys.columns where name like 'observacionesPedidoRuta' and OBJECT_ID = OBJECT_ID(N'dbo.PedidosEspeciales' ) )
begin
	alter table PedidosEspeciales add observacionesPedidoRuta varchar(500)
end
go


-- columna para usuario de ruteo
if not exists ( select 1 from sys.columns where name like 'idUsuarioRuteo' and OBJECT_ID = OBJECT_ID(N'dbo.PedidosEspeciales' ) )
begin
	alter table PedidosEspeciales add idUsuarioRuteo int
end
go


-- columna para usuario que liquida
if not exists ( select 1 from sys.columns where name like 'idUsuarioLiquida' and OBJECT_ID = OBJECT_ID(N'dbo.PedidosEspeciales' ) )
begin
	alter table PedidosEspeciales add idUsuarioLiquida int
end
go


-- se agrega el tipo de ticket de pedido en ruta para cuando los pedidos en ruta son liquidados 
if not exists (select 1 from CatTipoTicketPedidosEspeciales where tipoTicket like 'Ticket Pedido en Ruta' )
begin
	insert into CatTipoTicketPedidosEspeciales (tipoTicket) values ('Ticket Pedido en Ruta')
end
go

-- se agregan campos en la tabla PedidosEspecialesCierresDetalle  para el cierre de PE
if not exists ( select 1 from sys.columns where name like 'MontoPedidosEnRuta' and OBJECT_ID = OBJECT_ID(N'dbo.PedidosEspecialesCierresDetalle' ) )
begin
	alter table PedidosEspecialesCierresDetalle add MontoPedidosEnRuta money
end
go

if not exists ( select 1 from sys.columns where name like 'NoPedidosEnRuta' and OBJECT_ID = OBJECT_ID(N'dbo.PedidosEspecialesCierresDetalle' ) )
begin
	alter table PedidosEspecialesCierresDetalle add NoPedidosEnRuta int
end
go


update PedidosEspecialesCierresDetalle set MontoPedidosEnRuta = 0.0, NoPedidosEnRuta = 0 where MontoPedidosEnRuta is null 
go 


if not exists ( select 1 from sys.columns where name like 'NoPedidosEnRuta' and OBJECT_ID = OBJECT_ID(N'dbo.PedidosEspecialesCierres' ) )
begin
	alter table PedidosEspecialesCierres add NoPedidosEnRuta int
end
go

update PedidosEspecialesCierres set NoPedidosEnRuta = 0 where NoPedidosEnRuta is null 
go 
