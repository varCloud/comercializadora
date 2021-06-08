--USE [DB_A57E86_lluviadesarrollo]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TiendasXAlmacen]') AND type in (N'U'))
ALTER TABLE [dbo].[TiendasXAlmacen] DROP CONSTRAINT IF EXISTS [FK_TiendasXAlmacen_Almacenes1]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TiendasXAlmacen]') AND type in (N'U'))
ALTER TABLE [dbo].[TiendasXAlmacen] DROP CONSTRAINT IF EXISTS [FK_TiendasXAlmacen_Almacenes]
GO
/****** Object:  Table [dbo].[TiendasXAlmacen]    Script Date: 02/06/2021 11:04:40 p. m. ******/
DROP TABLE IF EXISTS [dbo].[TiendasXAlmacen]
GO
/****** Object:  Table [dbo].[AlmacenesXLineaProducto]    Script Date: 02/06/2021 11:04:40 p. m. ******/
DROP TABLE IF EXISTS [dbo].[AlmacenesXLineaProducto]
GO
/****** Object:  UserDefinedFunction [dbo].[obtnerLineasProductosXAlmacen]    Script Date: 02/06/2021 11:04:40 p. m. ******/
DROP FUNCTION IF EXISTS [dbo].[obtnerLineasProductosXAlmacen]
GO
/****** Object:  UserDefinedFunction [dbo].[ExisteProductoEnAlmancen]    Script Date: 02/06/2021 11:04:40 p. m. ******/
DROP FUNCTION IF EXISTS [dbo].[ExisteProductoEnAlmancen]
GO
/****** Object:  UserDefinedFunction [dbo].[ExisteProductoEnAlmancen]    Script Date: 02/06/2021 11:04:40 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ExisteProductoEnAlmancen]
(
@idAlmacen int
,@idProducto int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	declare @existe int 

		if  exists (Select 1 from Productos P join obtnerLineasProductosXAlmacen(null,@idAlmacen ) L on P.idLineaProducto =  L.idLineaProducto  where P.idProducto = @idProducto)
		begin
			set @existe =  1 --el producto existe
		end
		else 
		begin
			set @existe =  0 --el producto  NO existe 
		end
		/* fin  esta validacion quiza deberia de ir en una funcion */

		return @existe --el producto existe;

END
GO
/****** Object:  UserDefinedFunction [dbo].[obtnerLineasProductosXAlmacen]    Script Date: 02/06/2021 11:04:40 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   FUNCTION [dbo].[obtnerLineasProductosXAlmacen] (
    @idUsuario INT = null,
	@idAlmacen int = null
)
RETURNS  @lineasProducto TABLE 
(
		--contador int primary key identity  not null,
		idLineaProducto int not null , 
		idAlmacen int  not null,  
		descripcion varchar(255)  not null
)
AS
begin

	    --si es administrador y almacen es null, regresamos todas las lineas de productos
	   if exists(select 1 from Usuarios where idUsuario=coalesce(@idUsuario,0) and idRol=1 and coalesce(@idAlmacen,0)=0)
	   begin	   
		insert into @lineasProducto (idLineaProducto , idAlmacen,descripcion )
		select idLineaProducto,0 idAlmacen,descripcion from LineaProducto where activo=1
	   end
	   else
	   begin
	        if(coalesce(@idAlmacen,0)=0)
				select @idAlmacen=idAlmacen from Usuarios where idUsuario=coalesce(@idUsuario,0)

			insert into @lineasProducto (idLineaProducto , idAlmacen,descripcion )
			select distinct A.idLineaProducto, coalesce(@idAlmacen , 0) , LP.descripcion from 
			AlmacenesXLineaProducto A  
			join LineaProducto LP on LP.idLineaProducto = A.idLineaProducto and LP.activo=1
			where
			 A.idAlmacen = coalesce(@idAlmacen , A.idAlmacen)
			and A.activo=1 and A.activo=1;
	  end

return;
end
	
GO
/****** Object:  Table [dbo].[AlmacenesXLineaProducto]    Script Date: 02/06/2021 11:04:40 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AlmacenesXLineaProducto](
	[idAlmacenLineaProducto] [int] IDENTITY(1,1) NOT NULL,
	[idAlmacen] [int] NULL,
	[idLineaProducto] [int] NULL,
	[fechAlta] [datetime] NULL,
	[activo] [int] NULL,
 CONSTRAINT [PK_AlmacenesXLineaProducto] PRIMARY KEY CLUSTERED 
(
	[idAlmacenLineaProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TiendasXAlmacen]    Script Date: 02/06/2021 11:04:41 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TiendasXAlmacen](
	[idTiendaAlmacen] [int] IDENTITY(1,1) NOT NULL,
	[idTienda] [int] NULL,
	[idAlmacen] [int] NULL,
	[activo] [int] NULL,
	[fechaAlta] [datetime] NULL,
 CONSTRAINT [PK_TiendasAlmacen] PRIMARY KEY CLUSTERED 
(
	[idTiendaAlmacen] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AlmacenesXLineaProducto] ON 
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (1, 3, 1, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (2, 3, 2, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (3, 3, 4, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (4, 3, 5, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (5, 3, 6, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (6, 3, 7, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (7, 3, 9, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (8, 3, 10, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (9, 3, 11, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (10, 3, 12, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (11, 3, 13, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (12, 3, 19, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (13, 3, 20, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (14, 3, 21, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (15, 3, 22, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (16, 3, 23, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (17, 3, 25, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (18, 3, 26, CAST(N'2021-05-30T16:31:59.947' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (19, 4, 1, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (20, 4, 2, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (21, 4, 4, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (22, 4, 5, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (23, 4, 6, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (24, 4, 7, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (25, 4, 9, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (26, 4, 10, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (27, 4, 11, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (28, 4, 12, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (29, 4, 13, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (30, 4, 19, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (31, 4, 20, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (32, 4, 21, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (33, 4, 22, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (34, 4, 23, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (35, 4, 25, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (36, 4, 26, CAST(N'2021-05-30T16:32:21.597' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (37, 1, 1, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (38, 1, 2, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (39, 1, 4, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (40, 1, 5, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (41, 1, 6, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (42, 1, 7, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (43, 1, 9, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (44, 1, 10, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (45, 1, 11, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (46, 1, 12, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (47, 1, 13, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (48, 1, 19, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (49, 1, 20, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (50, 1, 21, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (51, 1, 22, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (52, 1, 23, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (53, 1, 25, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (54, 1, 26, CAST(N'2021-05-30T17:20:00.907' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (55, 2, 1, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (56, 2, 2, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (57, 2, 4, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (58, 2, 5, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (59, 2, 6, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (60, 2, 7, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (61, 2, 9, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (62, 2, 10, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (63, 2, 11, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (64, 2, 12, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (65, 2, 13, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (66, 2, 19, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (67, 2, 20, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 1)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (68, 2, 21, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (69, 2, 22, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (70, 2, 23, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (71, 2, 25, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
INSERT [dbo].[AlmacenesXLineaProducto] ([idAlmacenLineaProducto], [idAlmacen], [idLineaProducto], [fechAlta], [activo]) VALUES (72, 2, 26, CAST(N'2021-05-30T17:20:39.333' AS DateTime), 0)
GO
SET IDENTITY_INSERT [dbo].[AlmacenesXLineaProducto] OFF
GO
SET IDENTITY_INSERT [dbo].[TiendasXAlmacen] ON 
GO
INSERT [dbo].[TiendasXAlmacen] ([idTiendaAlmacen], [idTienda], [idAlmacen], [activo], [fechaAlta]) VALUES (1, 4, 1, 1, CAST(N'2021-05-31T16:26:29.727' AS DateTime))
GO
INSERT [dbo].[TiendasXAlmacen] ([idTiendaAlmacen], [idTienda], [idAlmacen], [activo], [fechaAlta]) VALUES (2, 3, 2, 1, CAST(N'2021-05-31T16:26:29.727' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[TiendasXAlmacen] OFF
GO
ALTER TABLE [dbo].[TiendasXAlmacen]  WITH CHECK ADD  CONSTRAINT [FK_TiendasXAlmacen_Almacenes] FOREIGN KEY([idTienda])
REFERENCES [dbo].[Almacenes] ([idAlmacen])
GO
ALTER TABLE [dbo].[TiendasXAlmacen] CHECK CONSTRAINT [FK_TiendasXAlmacen_Almacenes]
GO
ALTER TABLE [dbo].[TiendasXAlmacen]  WITH CHECK ADD  CONSTRAINT [FK_TiendasXAlmacen_Almacenes1] FOREIGN KEY([idAlmacen])
REFERENCES [dbo].[Almacenes] ([idAlmacen])
GO
ALTER TABLE [dbo].[TiendasXAlmacen] CHECK CONSTRAINT [FK_TiendasXAlmacen_Almacenes1]
GO
Alter table Compras add idAlmacen int null