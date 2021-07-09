--use DB_A57E86_lluviadesarrollo
--go

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'ConfiguracionesVentas' and xtype = 'u' )
	drop table ConfiguracionesVentas
go

CREATE TABLE ConfiguracionesVentas (
  id_config					int PRIMARY KEY IDENTITY(1, 1),
  descripcion				varchar(300),
  valor						float,
  activo					bit
)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON ConfiguracionesVentas TO PUBLIC
go

insert into ConfiguracionesVentas ( descripcion, valor, activo ) 
values ('Dias para poder hacer complementos/devoluciones', 30, cast(1 as bit))

select * from ConfiguracionesVentas 



