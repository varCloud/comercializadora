	


-- select * from CatRoles
-- select * from CatModulos
-- select * from PermisosRolPorModulo where idRol = 2 and idModulo =12

update PermisosRolPorModulo set tienePermiso = 0 where idRol = 2 and idModulo =12

if not exists (select 1 from PermisosRolPorModulo where idRol = 3 and idModulo =12)
begin
	insert into PermisosRolPorModulo (idRol,idModulo,tienePermiso) values ( 3,12,1)
end


