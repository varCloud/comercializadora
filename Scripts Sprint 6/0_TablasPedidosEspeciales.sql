use DB_A57E86_lluviadesarrollo
go

----------------------------------------------------------------------------------------------------------------------------------------------------------
--	[CatTipoPedidoInterno]
----------------------------------------------------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatTipoPedidoInterno]') AND type in (N'U'))
	DROP TABLE [dbo].[CatTipoPedidoInterno]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CatTipoPedidoInterno](
	
	idTipoPedidoInterno		int identity(1,1) NOT NULL,
	descripcion				varchar(255)

 CONSTRAINT [PK_idTipoPedidoInterno] PRIMARY KEY CLUSTERED 
(
	[idTipoPedidoInterno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT INSERT ON [dbo].[CatTipoPedidoInterno] TO [public] WITH GRANT OPTION  AS [dbo]
GO
GRANT SELECT ON [dbo].[CatTipoPedidoInterno] TO [public] WITH GRANT OPTION  AS [dbo]
GO
GRANT UPDATE ON [dbo].[CatTipoPedidoInterno] TO [public] WITH GRANT OPTION  AS [dbo]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--valores iniciales
insert into CatTipoPedidoInterno (descripcion) values ('Pedido Interno')
insert into CatTipoPedidoInterno (descripcion) values ('Pedido Especial')
go


-- id del nuevo catalogo CatTipoPedidoInterno
if not exists (select 1 from sys.columns where name = N'idTipoPedidoInterno' AND Object_ID = Object_ID(N'dbo.PedidosInternos'))
begin
	ALTER TABLE PedidosInternos ADD idTipoPedidoInterno int DEFAULT  1;
end
GO

-- descripcion para identificar el pedido en la app
if not exists (select 1 from sys.columns where name = N'descripcion' AND Object_ID = Object_ID(N'dbo.PedidosInternos'))
begin
	ALTER TABLE PedidosInternos ADD descripcion  varchar(500) default null;
end
GO

-- foranea en pedidosInternos
ALTER TABLE [dbo].[PedidosInternos]  WITH CHECK ADD  CONSTRAINT [FK_idTipoPedidoInterno] FOREIGN KEY([idTipoPedidoInterno])
REFERENCES [dbo].[CatTipoPedidoInterno] ([idTipoPedidoInterno])
GO


update PedidosInternos set idTipoPedidoInterno = 1 where idTipoPedidoInterno is null 

/*
select * from CatTipoPedidoInterno
select * from CatEstatusPedidoInterno
select  * from Almacenes
select  * from Usuarios
select  * from InventarioGeneral

--1
insert into PedidosInternos (idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta,observacion,idTipoPedidoInterno) values(1,1,5,1,'20200903','',2)
insert into PedidosInternosDetalle (idPedidoInterno,idProducto,cantidad,fechaAlta,cantidadAtendida,cantidadAceptada,cantidadRechazada) values(4,199,6,'20200903',0,0,0)
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(4,1,1,5,1,'20200903')

--2
insert into PedidosInternos (idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta,observacion,idTipoPedidoInterno) values(1,1,5,2,'20200902','',2)
insert into PedidosInternosDetalle (idPedidoInterno,idProducto,cantidad,fechaAlta,cantidadAtendida,cantidadAceptada,cantidadRechazada) values(5,1,16,'20200902',16,16,0)
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(5,1,1,5,1,'20200902')
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(5,1,1,4,2,'20200903')

--3
insert into PedidosInternos (idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta,observacion,idTipoPedidoInterno) values(1,2,5,3,'20200901','',2)
insert into PedidosInternosDetalle (idPedidoInterno,idProducto,cantidad,fechaAlta,cantidadAtendida,cantidadAceptada,cantidadRechazada) values(6,4,116,'20200901',116,100,16)
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(6,1,2,5,1,'20200901')
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(6,1,2,4,3,'20200902')

--4
insert into PedidosInternos (idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta,observacion,idTipoPedidoInterno) values(1,3,4,4,'20200901','',2)
insert into PedidosInternosDetalle (idPedidoInterno,idProducto,cantidad,fechaAlta,cantidadAtendida,cantidadAceptada,cantidadRechazada) values(7,180,300,'20200901',300,300,0)
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(7,1,3,5,1,'20200901')
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(7,1,3,4,2,'20200902')
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(7,1,3,3,4,'20200903')

--5
insert into PedidosInternos (idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta,observacion,idTipoPedidoInterno) values(1,4,3,5,'20200901','',2)
insert into PedidosInternosDetalle (idPedidoInterno,idProducto,cantidad,fechaAlta,cantidadAtendida,cantidadAceptada,cantidadRechazada) values(8,344,60,'20200901',60,60,0)
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(8,1,4,5,1,'20200901')
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(8,1,4,4,2,'20200902')
insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta) values(8,1,4,3,5,'20200903')


--update PedidosInternosDetalle set cantidadAtendida=16,cantidadAceptada=16 where idPedidoInternoDetalle=5

select * from PedidosInternos where idPedidoInterno=8
select * from PedidosInternosDetalle where idPedidoInterno=8
select * from PedidosInternosLog where idPedidoInterno=8


select * from CatTipoPedidoInterno
select * from PedidosInternos where idPedidoInterno in (select idPedidoInterno from PedidosInternos where idTipoPedidoInterno = 2) order by idPedidoInterno
select * from PedidosInternosDetalle  where idPedidoInterno in (select idPedidoInterno from PedidosInternos where idTipoPedidoInterno = 2) order by idPedidoInterno
select * from PedidosInternosLog  where idPedidoInterno in (select idPedidoInterno from PedidosInternos where idTipoPedidoInterno = 2) order by idPedidoInterno

--select idPedidoInterno from PedidosInternos where idPedidoInterno = 2

--delete CatTipoPedidoInterno

--DBCC CHECKIDENT ('dbo.CatTipoPedidoInterno', RESEED, 0);  
--GO  

*/
