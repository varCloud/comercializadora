

select * from ProductosEnvasadosXAgranel

insert into ProductosEnvasadosXAgranel (idProductoEnvasado,
idProductoAgranel,
idProducoEnvase,
activo,
fechaAlta)
select idProductoENVASADO,idProductoAGRANAEL,idProductoENVASE,1,GETDATE() from relacion_productos_envasados

select 60/1000
select 300 * 0.5
select 300 * 1

select 
PE.*,
PE.idProductoEnvasado,
P.descripcion ProductoEnvasado,
PA.descripcion ProductoAgranel,
PEE.descripcion ProductoEnvase,
case 
when P.descripcion like '%500 ML%' then 0.5
when P.descripcion like '%1 LT%' then 1
when P.descripcion like '%60 ML%' then 0.060
end
from ProductosEnvasadosXAgranel PE 
    join Productos P on PE.idProductoEnvasado = P.idProducto
	join Productos PA on PE.idProductoAgranel = PA.idProducto
	join Productos PEE on PE.idProducoEnvase = PEE.idProducto


UPDATE PE
 set 
 PE.unidadMedidad = 'ML',
 PE.valorUnidadMedida =
			case 
			when P.descripcion like '%500 ML%' then 0.5
			when P.descripcion like '%1 LT%' then 1
			when P.descripcion like '%60 ML%' then 0.060
			end
from ProductosEnvasadosXAgranel PE 
    join Productos P on PE.idProductoEnvasado = P.idProducto
	join Productos PA on PE.idProductoAgranel = PA.idProducto
	join Productos PEE on PE.idProducoEnvase = PEE.idProducto
