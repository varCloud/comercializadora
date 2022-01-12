	



 select * from CatRoles
 select * from CatModulos
 select * from PermisosRolPorModulo where idRol = 2 and idModulo =12
 --begin tran 

declare @idRol int = 0
if not exists (select * from CatRoles where descripcion like '%Cajero Pedidos Especiales%')
begin
	 insert into CatRoles (descripcion,activo) values ('Cajero Pedidos Especiales' ,1)
	 select @idRol = idRol from CatRoles where descripcion like '%Cajero Pedidos Especiales%' 
end

if not exists (select 1 from PermisosRolPorModulo where idRol = @idRol and idModulo =12)
begin
	insert into PermisosRolPorModulo (idRol,idModulo,tienePermiso) values (@idRol,12,1)
	
end

if not exists (select 1 from PermisosRolPorModulo where idRol = @idRol and idModulo =7)
begin
	insert into PermisosRolPorModulo (idRol,idModulo,tienePermiso) values (@idRol,7,1)
end


 select * from CatRoles
 select * from CatModulos
 select * from PermisosRolPorModulo where idRol = @idRol

-- update PermisosRolPorModulo set tienePermiso = 0 where idRol =3 and idModulo =12
--rollback tran



--select * from Usuarios

--exec SP_VALIDA_CONTRASENA 'cajitaPE','b2'