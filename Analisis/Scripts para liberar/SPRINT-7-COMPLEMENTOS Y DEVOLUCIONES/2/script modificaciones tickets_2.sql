--use DB_A57E86_lluviadesarrollo
--go


alter table ComplementosDetalle add montoComisionBancaria money default 0
go


update ComplementosDetalle set montoComisionBancaria = 0
go
