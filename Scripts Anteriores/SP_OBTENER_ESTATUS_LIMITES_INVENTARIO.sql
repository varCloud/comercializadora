-- se crea procedimiento SP_OBTENER_ESTATUS_LIMITES_INVENTARIO
if exists (select * from sysobjects where name like 'SP_OBTENER_ESTATUS_LIMITES_INVENTARIO' and xtype = 'p')
	drop proc SP_OBTENER_ESTATUS_LIMITES_INVENTARIO
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Obtener estatus de los limites de inventario
status			200 = ok
				-1	= error
*/

Create proc [dbo].SP_OBTENER_ESTATUS_LIMITES_INVENTARIO

as

	begin -- procedimiento
		
		SELECT idEstatusLimiteInventario idStatus,descripcion
        FROM CatEstatusLimiteInventario
		
	end -- procedimiento
	
