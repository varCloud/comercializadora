-- USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_MESES]    Script Date: 19/09/2022 10:14:01 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
Fecha			2022/02/17
Objetivo		Consulta meses

*/

create proc [dbo].[SP_CONSULTA_ESTATUS_PROCESO_PRODUCCION]
@anio int=null
as

	begin -- principal

	set LANGUAGE Spanish;
	
		begin -- reporte de estatus
		 SELECT 200 status , 'ok' mensaje
		 SELECT idEstatusProcesoAgranel value ,descripcion text FROM CatEstatusProcesoAgranel where idEstatusProcesoAgranel > 1

				
		end -- reporte de estatus


	end  -- principal
