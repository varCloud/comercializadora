/*
Objetivo	Permiso del modulo 15 "Relacion Produccion" (pantalla de formulas del WMS).
			CatModulos.idModulo es identity => se inserta el 15 con IDENTITY_INSERT (el 15 esta libre; 14 = Facturacion).
			PermisosRolPorModulo.idPermiso es identity (sin FKs): una fila por rol de CatRoles,
			tienePermiso = 1 solo para idRol = 1 (Administrador), 0 en los demas.
Idempotente.
*/

-- VERIFICACION PREVIA (0 filas la primera vez)
select * from CatModulos where idModulo = 15
select * from PermisosRolPorModulo where idModulo = 15
GO

IF NOT EXISTS (SELECT 1 FROM CatModulos WHERE idModulo = 15)
BEGIN
	SET IDENTITY_INSERT CatModulos ON
	INSERT INTO CatModulos (idModulo, descripcion) VALUES (15, 'Relacion Produccion')
	SET IDENTITY_INSERT CatModulos OFF
END

INSERT INTO PermisosRolPorModulo (idRol, idModulo, tienePermiso)
SELECT R.idRol, 15, CASE WHEN R.idRol = 1 THEN 1 ELSE 0 END
FROM CatRoles R
WHERE NOT EXISTS (SELECT 1 FROM PermisosRolPorModulo P WHERE P.idModulo = 15 AND P.idRol = R.idRol)
GO

-- VERIFICACION POSTERIOR (modulo 15; una fila por rol; solo idRol 1 con permiso)
select * from CatModulos where idModulo = 15
select (select count(*) from CatRoles) roles, count(*) filas, sum(case when tienePermiso = 1 then 1 else 0 end) conPermiso,
	   min(case when tienePermiso = 1 then idRol end) rolConPermiso
from PermisosRolPorModulo where idModulo = 15
GO
