--use DB_A57E86_lluviadesarrollo
--go

alter table InventarioDetalle alter column cantidad float 
go

alter table  Productos alter column cantidadUnidadCompra float
go

alter table InventarioDetalleLog alter column cantidad float
go

alter table InventarioDetalleLog alter column cantidadActual float
go

alter table ComprasDetalle alter column cantidadRecibida float
go

alter table ComprasDetalle alter column cantidad float
go

alter table ComprasDetalle alter column cantidadDevuelta float
go

if exists (SELECT * FROM sys.objects WHERE type = 'D' AND name = 'DF__PedidosIn__canti__721CCC2B')
begin
	ALTER TABLE PedidosInternosDetalle DROP CONSTRAINT DF__PedidosIn__canti__721CCC2B;
end
go

alter table PedidosInternosDetalle alter column cantidadAceptada float
go

if exists (SELECT * FROM sys.objects WHERE type = 'D' AND name = 'DF__PedidosIn__canti__7310F064')
begin
	ALTER TABLE PedidosInternosDetalle DROP CONSTRAINT DF__PedidosIn__canti__7310F064;
end
go

alter table PedidosInternosDetalle alter column cantidadRechazada float
go

alter table PedidosInternosDetalle alter column cantidadAtendida float
go

alter table PedidosInternosDetalle alter column cantidad float
go


alter table MovimientosDeMercancia alter column cantidad float
go


if exists (SELECT * FROM sys.objects WHERE type = 'D' AND name = 'DF__Movimient__canti__74F938D6')
begin 
	ALTER TABLE MovimientosDeMercancia DROP CONSTRAINT DF__Movimient__canti__74F938D6;
end
go

alter table MovimientosDeMercancia alter column cantidadAtendida float
go

alter table InventarioDetalleLog alter column cantidad float
go

alter table InventarioDetalleLog alter column cantidadActual float
go

alter table ClientesAtendidosRuta alter column cantidad float
go

alter table AjusteInventarioFisico alter column cantidadActual float
go

alter table AjusteInventarioFisico alter column cantidadEnFisico float
go

alter table AjusteInventarioFisico alter column cantidadAAjustar float
go

alter table InventarioGeneralLog alter column cantidad float
go
alter table InventarioGeneralLog alter column cantidadDespuesDeOperacion float
go



-- columna para verificar si a una venta se le puede agregar un complemento o devolucion
if not exists (select 1 from sys.columns where name = N'puedeHacerComplementos' and Object_ID = Object_ID(N'dbo.Ventas'))
BEGIN
	alter table Ventas add puedeHacerComplementos bit default 0
END
go

update Ventas set puedeHacerComplementos = cast(0 as bit)
go
