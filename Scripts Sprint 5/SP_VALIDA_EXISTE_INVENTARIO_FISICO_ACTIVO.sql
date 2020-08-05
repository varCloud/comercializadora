-- se crea procedimiento SP_VALIDA_EXISTE_INVENTARIO_FISICO_ACTIVO
if exists (select 1 from sysobjects where name like 'SP_VALIDA_EXISTE_INVENTARIO_FISICO_ACTIVO' and xtype = 'p')
	drop proc SP_VALIDA_EXISTE_INVENTARIO_FISICO_ACTIVO
go

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aaaa111111
Fecha			20200801
Objetivo		Objetivo
Proyecto		Sp que valida si existe un inventario fisico
Ticket			ticket

*/

create proc

	[dbo].[SP_VALIDA_EXISTE_INVENTARIO_FISICO_ACTIVO]
	
	@idUsuario int
	
as

		begin -- reporte de estatus


			SELECT Estatus,Mensaje FROM dbo.FnValidaInventario(@idUsuario)			
				
		end -- reporte de estatus

