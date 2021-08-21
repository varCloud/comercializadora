IF NOT EXISTS(SELECT 1 FROM Almacenes WHERE Descripcion like 'Almacen Trapeadores')
INSERT INTO Almacenes(Descripcion,idTipoAlmacen,idSucursal)
values('Almacen Trapeadores',1,1)

go


IF NOT EXISTS(SELECT 1 FROM TiendasXAlmacen where idTienda=4 and idAlmacen=5)
INSERT TiendasXAlmacen(idTienda,idAlmacen,activo,fechaAlta)
values(4,5,1,dbo.FechaActual())

go

IF NOT EXISTS (SELECT 1 FROM AlmacenesXLineaProducto WHERE idAlmacen=5 and idLineaProducto=4)
INSERT INTO AlmacenesXLineaProducto(idAlmacen,idLineaProducto,fechAlta,activo)
values(5,4,dbo.FechaActual(),1)

go


UPDATE AlmacenesXLineaProducto set activo=0 where idAlmacen=1 and idLineaProducto=4

