
alter table Devoluciones add  observaciones varchar(255)
alter table DevolucionesDetalle add  idDevolucion	int
alter table ComplementosDetalle add  idComplemento	int


update DevolucionesDetalle set DevolucionesDetalle.idDevolucion = d.idDevolucion from Devoluciones d inner join DevolucionesDetalle dd on d.idVenta = dd.idVenta

update ComplementosDetalle set ComplementosDetalle.idComplemento = d.idComplemento from Complementos d inner join ComplementosDetalle dd on d.idVenta = dd.idVenta


/*
select * from Devoluciones order by idDevolucion
select * from DevolucionesDetalle order by idDevolucion

select * from Complementos order by idventa
select * from ComplementosDetalle order by idventa

select dd.*, d.idDevolucion ,'',* from Devoluciones d inner join DevolucionesDetalle dd on d.idVenta = dd.idVenta

*/




