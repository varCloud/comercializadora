
IF NOT EXISTS(select 1 from CatModulos where descripcion like '%Inventario Fisico%')
INSERT INTO CatModulos(descripcion) values('Inventario Fisico')

declare @idModulo int

select @idModulo=idModulo from CatModulos where descripcion like 'Inventario Fisico'

INSERT INTO PermisosRolPorModulo(idModulo,idRol,tienePermiso)
select @idModulo,a.idRol,case when a.idRol=1 then 1 else 0 end tienePermiso
from [dbo].[PermisosRolPorModulo] a
left join PermisosRolPorModulo b on b.idModulo=@idModulo and a.idRol=b.idRol
where b.idRol is null
group by a.idRol

select * from PermisosRolPorModulo where idModulo=@idModulo




