IF EXISTS (SELECT * FROM sysobjects WHERE name='FnValidaInventario')  
DROP FUNCTION [dbo].[FnValidaInventario]

GO 


CREATE FUNCTION [dbo].[FnValidaInventario]
(
    @idUsuario int
)
RETURNS @result table
(
		Estatus	int null,
		Mensaje	varchar(255),
		idInventarioFisico int
)
AS
BEGIN

   declare @idSucursal int,@Status bit=0

   select @idSucursal=idSucursal from usuarios where idUsuario=@idUsuario

   IF EXISTS  (SELECT 1 FROM InventarioFisico WHERE ACTIVO = 1 and idSucursal=@idSucursal)
      insert @result
			(Estatus, Mensaje,idInventarioFisico)
			SELECT 200,'Ok',idInventarioFisico FROM InventarioFisico WHERE ACTIVO = 1 and idSucursal=@idSucursal
   ELSE
	insert @result
			(Estatus, Mensaje,idInventarioFisico)
			SELECT -1,'No existe  inventario fisico activo',0 idInventarioFisico

    RETURN

END

go



