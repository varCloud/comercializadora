
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_TICKETS_PEDIDO_ESPECIAL]    Script Date: 09/02/2022 20:20:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2022/02/09
Objetivo		Consulta los abonos realizados de un cliente
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_ABONOS_CLIENTE_PEDIDO_ESPECIAL]

@idAbonoCliente bigInt=NULL,
@idCliente bigint=NULL

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',						 
						@diasDevoluciones int=0,
						@fechaActual date

			end  --declaraciones 

			begin -- principal			
				
			
					select 
					a.*,
					CONVERT(VARCHAR(10),a.fechaAlta,103) fechaAbono,
					CONVERT(VARCHAR(20),a.fechaAlta,114) horaAbono,
					c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno  as nombreCliente,
					u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno  as nombreUsuario,
					formaPago.descripcion as descFormaPago,
					case when coalesce(montoPagado,0)=0 then 0 else coalesce(montoPagado,0)-montoTotal end cambio
					INTO #AbonosCliente
					from PedidosEspecialesAbonoClientes a
					join clientes c on a.idCliente=c.idCliente
					join usuarios u on a.idUsuario=u.idUsuario
					inner join FactCatFormaPago formaPago on a.idFactFormaPago = formaPago.id
					where a.idCliente=coalesce(@idCliente,c.idCliente)
					and a.idAbonoCliente=coalesce(@idAbonoCliente,a.idAbonoCliente)
					and a.activo=1
					
					

					if not exists (select 1 from #AbonosCliente)
					begin
						select	
								@status = -1,
								@mensaje = 'No se encontraron abonos'
					end


			end -- principal

		end try

		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@mensaje = error_message()
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
           
		    if(@status=200)
				select * from #AbonosCliente order by fechaAlta desc
			
					
		end -- reporte de estatus
		

	end  -- principal

