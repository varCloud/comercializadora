begin tran

	declare @idAlmacen as int = 4, @idUbicacion as int = 0

	if not exists ( select 1 from Ubicacion where idAlmacen = @idAlmacen and idPasillo=0 and idPiso=0 and idRaq=0 )
	begin
		insert into Ubicacion (idAlmacen,idPasillo,idRaq,idPiso) values (@idAlmacen,0,0,0)
	end

	select @idUbicacion = idUbicacion from Ubicacion where idAlmacen = @idAlmacen and idPasillo=0 and idPiso=0 and idRaq=0  


	select	p.idProducto,  
			(ABS(CHECKSUM(NewId())) % 9000) as cantidad,
			getdate() as fechaAlta,
			@idUbicacion as  idUbicacion,
			getdate() as fechaActualizacion
	into	#existencias_
	from	Productos p 
				left join InventarioDetalle id
					on id.idProducto = p.idProducto
	where	id.idProducto is null 

	insert into InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
	select idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion from #existencias_

	insert into InventarioGeneral(idProducto,cantidad,fechaUltimaActualizacion)
	select idProducto,cantidad,fechaActualizacion from #existencias_



	select * from InventarioDetalle
	select * from InventarioGeneral

	exec SP_CONSULTA_PRODUCTOS 
	@idProducto = 0
	,@descripcion = null
	,@idUnidadMedida = null
	,@idLineaProducto = null
	,@activo = 1
	,@articulo = null
	,@claveProdServ = null
	,@fechaIni = '19000101'
	,@fechaFin = '19000101'
	,@idUsuario = 3
	


rollback tran 