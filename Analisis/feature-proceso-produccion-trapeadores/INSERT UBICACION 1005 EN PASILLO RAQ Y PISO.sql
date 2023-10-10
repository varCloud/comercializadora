 /********************* ROL *********************/

 select * from AlmacenesXLineaProducto where idAlmacen = 5
 insert into AlmacenesXLineaProducto (idAlmacen,idLineaProducto,fechAlta,activo) values (5,21,dbo.FechaActual(),1)
 insert into AlmacenesXLineaProducto (idAlmacen,idLineaProducto,fechAlta,activo) values (5,26,dbo.FechaActual(),1)

 /********************* ROL *********************/
	INSERT INTO CatRoles (descripcion,activo) VALUES ('Encargado de Producción (trapeadores)' ,1)

 /********************* PISO RAQ PASILLO *********************/

SET IDENTITY_INSERT CatPasillo ON
insert into CatPasillo  (idPasillo,descripcion) values (1005, 'en proceso de produccioón de trapeadores')
SET IDENTITY_INSERT CatPasillo OFF

SET IDENTITY_INSERT CatPiso ON
insert into CatPiso  (idPiso,descripcion) values (1005, 'en proceso de produccioón de trapeadores')
SET IDENTITY_INSERT CatPiso OFF

SET IDENTITY_INSERT CatRaq ON
insert into CatRaq  (idRaq,descripcion) values (1005, 'en proceso de produccioón de trapeadores')
SET IDENTITY_INSERT CatPasillo OFF


 /********************* MOVIMIENTOS DE INVENTARIO *********************/
select * from CatTipoMovimientoInventario
insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de proceso de produccion de trapeadores finalizados)' , 1)
insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(salida de mercancia por conversion de linea matra a trapeadores)' , -1)




 /********************* UBICACION PARA TRAPEADORES *********************/
insert into Ubicacion (idAlmacen,idPasillo,idRaq,idPiso) values (5,1005,1005,1005)
select * from Ubicacion where idPasillo = 1005