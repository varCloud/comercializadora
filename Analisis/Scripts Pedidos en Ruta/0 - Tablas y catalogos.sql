

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


