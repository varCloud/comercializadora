--use DB_A57E86_lluviadesarrollo
--go


alter table VentasDetalle add idComplementoDetalle int default 0
go


update VentasDetalle set idComplementoDetalle = 0
go
