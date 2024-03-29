--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_MERMA]    Script Date: 14/09/2022 10:46:12 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2020/07/28
Objetivo		Consulta indicador de merma
status			200 = ok
				-1	= error
*/

create proc [dbo].SP_CONSULTA_PROCESO_PRODUCCION

@idUsuario int = null,
@idEstatusProcesoProduccionAgranel int = null,
@fechaIni  datetime = null,
@fechaFin datetime  = null
as

	begin -- procedimiento
		begin -- inicio
				select 200 status , 'se encontraron resultados'
				SELECT r.*,p.codigoBarras,p.descripcion descripcionProducto,l.idLineaProducto,l.descripcion descripcionLinea ,
				case when EPA.idEstatusProcesoAgranel in (1,2) then 'Pendiende de procesar' else EPA.descripcion end descripcionEstatus,
				isnull(U.nombre,'')+' '+isnull(U.apellidoPaterno,'')+' '+isnull(U.apellidoMaterno,'') nombreUsuario
				FROM ProcesoProduccionAgranel r
				join productos p on r.idProducto=p.idProducto
				join LineaProducto l on p.idLineaProducto=l.idLineaProducto
				join CatEstatusProcesoAgranel EPA on EPA.idEstatusProcesoAgranel = r.idEstatusProduccionAgranel
				join Usuarios U on U.idUsuario = r.idUsuario
				where
				cast (r.fechaAlta as date) >= CAST(coalesce(@fechaIni, r.fechaAlta) as date) and cast (r.fechaAlta as date) <=CAST(coalesce(@fechaFin, r.fechaAlta) as date)
				AND r.idUsuario = coalesce(@idUsuario , r.idUsuario)
				AND r.idEstatusProduccionAgranel =  coalesce(@idEstatusProcesoProduccionAgranel ,  r.idEstatusProduccionAgranel)
		end
	end -- procedimiento
	
