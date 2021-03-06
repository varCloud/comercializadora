-- se crea procedimiento SP_OBTENER_LIMITES_INVENTARIO
if exists (select * from sysobjects where name like 'SP_APP_OBTENER_DETALLE_PEDIDOS_INTERNOS' and xtype = 'p')
	drop proc SP_APP_OBTENER_DETALLE_PEDIDOS_INTERNOS
go

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/08/28
Objetivo		Obtener los limites maximo y minimos que de inventario por producto y almacen
status			200 = ok
				-1	= error
*/

Create proc [dbo].SP_APP_OBTENER_DETALLE_PEDIDOS_INTERNOS

@idPedidoInterno int=null
as

	begin -- procedimiento
		
		begin try -- try principal
		
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit)
						
			end -- inicio
		
		select 200 Estatus , 'Ok' Mensaje 
			
		SELECT 
		--P.idPedidoInterno,
		--P.IdEstatusPedidoInterno,
		--EP.descripcion descripcionEstatus,
		--Pd.cantidadAtendida,
		PD.idPedidoInternoDetalle,
		isnull(PD.cantidadAtendida, 0) cantidadAtendida,
		PD.idProducto ,
		PD.cantidad,
		Prod.descripcion,
		P.fechaAlta,
		isnull(p.observacion,'') observacion,
		isnull(M4.observaciones,'') observacionRechazaSolicita,
		isnull(MM.observaciones,'') observacionAtendio,
		isnull(M2.observaciones,'') observacionRechazaAtendio,
		isnull(M3.observaciones,'') observacionFinalizado,
		isnull(U.nombre,' ')+' '+isnull(U.apellidoPaterno,'')+' '+isnull(u.apellidoMaterno,'') usuarioAtendio,
		isnull(UU.nombre,' ')+' '+isnull(UU.apellidoPaterno,'')+' '+isnull(UU.apellidoMaterno,'') usuarioSolicito,
		isnull(URechazado.nombre,' ')+' '+isnull(URechazado.apellidoPaterno,'')+' '+isnull(URechazado.apellidoMaterno,'') usuarioRechaza,
		isnull(UAutoriza.nombre,' ')+' '+isnull(UAutoriza.apellidoPaterno,'')+' '+isnull(UAutoriza.apellidoMaterno,'') usuarioAutoriza,
	    MM.fechaAlta as fechaAtendido,
		M2.fechaAlta as fechaRechazado,
		M3.fechaAlta as fechaAutoriza,
		M4.fechaAlta as fechaRechazaSolicita
		 
		--P.idAlmacenOrigen,A.idAlmacen, A.Descripcion,
		--P.idAlmacenDestino,AB.idAlmacen,AB.Descripcion
		--* 
		FROM PedidosInternos   P join  PedidosInternosDetalle PD
		on P.idPedidoInterno = PD.idPedidoInterno   join Productos Prod
		on PD.idProducto = Prod.idProducto JOIN CatEstatusPedidoInterno EP
		ON EP.IdEstatusPedidoInterno =P.IdEstatusPedidoInterno join Almacenes A
		on P.idAlmacenOrigen = A.idAlmacen join Almacenes AB
		on P.idAlmacenDestino = AB.idAlmacen LEFT JOIN [dbo].[MovimientosDeMercancia] MM
		on MM.idPedidoInterno = P.idPedidoInterno and  MM.idEstatusPedidoInterno =2 LEFT JOIN Usuarios U
		on U.idUsuario = MM.idUsuario LEFT JOIN  Usuarios UU 
		on UU.idUsuario = P.idUsuario  LEFT JOIN [dbo].[MovimientosDeMercancia] M2
		on M2.idPedidoInterno = P.idPedidoInterno and  M2.idEstatusPedidoInterno =3 LEFT JOIN Usuarios URechazado on URechazado.idUsuario = M2.idUsuario
		  LEFT JOIN [dbo].[MovimientosDeMercancia] M3
		on M3.idPedidoInterno = P.idPedidoInterno and  M3.idEstatusPedidoInterno =4 LEFT JOIN Usuarios UAutoriza on UAutoriza.idUsuario = M3.idUsuario
		LEFT JOIN [dbo].[MovimientosDeMercancia] M4 on M4.idPedidoInterno = P.idPedidoInterno and  M4.idEstatusPedidoInterno =5 
		LEFT JOIN Usuarios URechazaSoclicita on URechazaSoclicita.idUsuario = M4.idUsuario

		where
			 P.idPedidoInterno = coalesce (@idPedidoInterno , P.idPedidoInterno)		   

		 end try -- try principal
		
		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@error_message = error_message(),
					@error_severity =
						case error_severity()
							when 11 then 'Error en validación'
							when 12 then 'Error en consulta'
							when 13 then 'Error en actualización'
							else 'Error general'
						end
		
		end catch -- catch principal
		
		
	end -- procedimiento
	
