
IF OBJECT_ID('CatTipoInventarioFisico') IS NOT NULL
BEGIN
DROP TABLE CatTipoInventarioFisico;
END

CREATE TABLE CatTipoInventarioFisico(
idTipoInventarioFisico int primary key,
tipoInventarioFisico varchar(500)
)
go

insert into CatTipoInventarioFisico(idTipoInventarioFisico,tipoInventarioFisico)
values(1,'General'),(2,'Individual')

go

IF NOT EXISTS(
        SELECT *
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'inventarioFisico' AND COLUMN_NAME = 'idTipoInventarioFisico'
    )
alter table inventarioFisico add idTipoInventarioFisico int default 1

go

update inventarioFisico set idTipoInventarioFisico=1 where idTipoInventarioFisico is null