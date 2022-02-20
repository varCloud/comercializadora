ALTER TABLE PedidosEspecialesAbonoClientes add saldoInicial money default 0,saldoAntesOperacion money default 0,saldoDespuesOperacion money default 0,montoPagado money default 0



if not exists (select 1 from CatEstatusPedidoEspecialDetalle where descripcion like 'Rechazo Parcial por el Administrador')
begin
	insert into CatEstatusPedidoEspecialDetalle(descripcion) values ('Rechazo Parcial por el Administrador')
end

