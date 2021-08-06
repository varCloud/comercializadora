


if not exists ( select 1 from ConfiguracionesVentas where descripcion like 'Requiere autorizacion para cierre de cajas.' )
	begin
		insert into ConfiguracionesVentas (descripcion, valor, activo) values ('Requiere autorizacion para cierre de cajas.', 1, cast(1 as bit) )
	end



-- usuario que puede autorizar un cierre de caja
if not exists ( select 1 from sys.columns where name = N'puedeAutorizarCierre' and Object_ID = Object_ID(N'Usuarios') )
begin
    alter table Usuarios add puedeAutorizarCierre bit default (0);
end
go

update Usuarios set puedeAutorizarCierre = cast(0 as bit)
update Usuarios set puedeAutorizarCierre = cast(1 as bit) where idUsuario = 2


-- agregar monto en el cierre de caja
if not exists ( select 1 from sys.columns where name = N'EfectivoEntregadoEnCierre' and Object_ID = Object_ID(N'RetirosCierreDia') )
begin
    alter table RetirosCierreDia add EfectivoEntregadoEnCierre money default (0);
end
go

-- agregar MontoTotalVentasTransferencias en el cierre de caja
if not exists ( select 1 from sys.columns where name = N'MontoTotalVentasTransferencias' and Object_ID = Object_ID(N'RetirosCierreDia') )
begin
    alter table RetirosCierreDia add MontoTotalVentasTransferencias money default (0);
end
go

-- agregar  en el cierre de caja
if not exists ( select 1 from sys.columns where name = N'MontoTotalVentasOtros' and Object_ID = Object_ID(N'RetirosCierreDia') )
begin
    alter table RetirosCierreDia add MontoTotalVentasOtros money default (0);
end
go




	