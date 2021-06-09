use DB_A57E86_lluviadesarrollo
go


alter table complementos add montoPagadoAgregarProductos money
go

alter table complementos add montoAgregarProductos money
go

alter table DevolucionesDetalle add montoDevueltoComisionBancaria money default 0
go

update DevolucionesDetalle set montoDevueltoComisionBancaria = 0
go
