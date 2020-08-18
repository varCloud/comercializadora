use DB_A57E86_lluviadesarrollo
go

-- se crea procedimiento SP_CONSULTA_COMPLEMENTOS_VENTA
if exists (select 1 from sysobjects where name like 'SP_APP_VALIDA_INVENTARIO_FISICO' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_APP_VALIDA_INVENTARIO_FISICO
go

/*

Autor			VIC
UsuarioRed		auhl373453
Fecha			2020/07/23
Objetivo		Consulta productos agregados/devueltos de una venta
status			200 = ok
				-1	= error
*/

create proc SP_APP_VALIDA_INVENTARIO_FISICO

as
	begin -- procedimiento
		begin try -- try principal
			  
			   IF EXISTS  (SELECT 1 FROM InventarioFisico WHERE ACTIVO = 1)
			   BEGIN
					SELECT 200 Estatus , 'OK' Mensaje , cast(idInventarioFisico as varchar)idInventarioFisico from InventarioFisico where activo = 1
			   END
			    BEGIN
					SELECT -1 Estatus , 'No existe  inventario fisico activo ' Mensaje ,'0' idInventarioFisico 
			   END


		end try -- fin del try 
		begin catch
			SELECT -1 Estatus, error_message() Mensaje,error_line() Errorline
		end catch

	end -- fin de procedimiento