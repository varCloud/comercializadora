


if not exists ( select 1 from ConfiguracionesVentas where descripcion like 'Requiere autorizacion para cierre de cajas.' )
	begin
		insert into ConfiguracionesVentas (descripcion, valor, activo) values ('Requiere autorizacion para cierre de cajas.', 1, cast(1 as bit) )
	end



	