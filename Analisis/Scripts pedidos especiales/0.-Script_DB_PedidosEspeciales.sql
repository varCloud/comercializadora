
--insert into CatPiso  ( descripcion) values ( 'Resguardo')
--select * from CatPiso 

--insert into CatRaq ( descripcion) values ( 'Resguardo')
--select * from CatRaq

--insert into CatPasillo ( descripcion) values ( 'Resguardo')
--select * from CatPasillo

if not exists ( select 1 from CatTipoMovimientoInventario where descripcion like 'Actualizacion de Inventario(salida de mercancia por pedido especial)' )
begin
	insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(salida de mercancia por pedido especial)',	-1)
end

if not exists ( select 1 from CatTipoMovimientoInventario where descripcion like 'Actualizacion de Inventario(carga de mercancia por pedido especial)' )
begin
	insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de mercancia por pedido especial)',	1)
end

if not exists ( select 1 from CatTipoMovimientoInventario where descripcion like 'Actualizacion de Inventario(carga de mercancia por pedido especial aceptado)' )
begin
	insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de mercancia por pedido especial aceptado)',	1)
end

if not exists ( select 1 from CatTipoMovimientoInventario where descripcion like 'Actualizacion de Inventario(carga de mercancia por pedido especial cancelado)' )
begin
	insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de mercancia por pedido especial cancelado)',	1)
end

if not exists ( select 1 from CatTipoMovimientoInventario where descripcion like 'Actualizacion de Inventario(carga de mercancia por sobrante de pedido especial)' )
begin
	insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de mercancia por sobrante de pedido especial)', 1)
end

if not exists ( select 1 from CatTipoMovimientoInventario where descripcion like 'Actualizacion de Inventario(carga de mercancia por devolucion de pedido especial)' )
begin
	insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de mercancia por devolucion de pedido especial)', 1)
end

if not exists ( select 1 from CatEstatusRetiros where descripcion like 'Pendiente' )
begin
	insert into CatEstatusRetiros (idEstatusRetiro,descripcion) values (4,'Pendiente')
end

if not exists ( select 1 from CatTipoMovimientoInventario where descripcion like 'Actualizacion de Inventario(salida de mercancia por cancelacion en pedido especial)' )
begin
	insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(salida de mercancia por cancelacion en pedido especial)',	-1)
end

if not exists ( select 1 from CatTipoMovimientoInventario where descripcion like 'Actualizacion de Inventario(carga de mercancia rechazada por el administrador a peticion del cliente)' )
begin
	insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de mercancia rechazada por el administrador a peticion del cliente)',	1)
end

--delete CatTipoMovimientoInventario where idTipoMovInventario > 21

--select * from CatTipoMovimientoInventario
--DBCC CHECKIDENT ('[CatTipoMovimientoInventario]', RESEED, 21);
--GO
--------------------------------------------
--catalAgos
--------------------------------------------

--drop table PedidosEspecialesDevolucionesDetalle
--drop table PedidosEspecialesDevoluciones
--drop table PedidosEspecialesRetiroEfectivoPedidosEspeciales
--drop table PedidosEspecialesCierresPedidosEspecialesDetalle
--drop table PedidosEspecialesCierresPedidosEspeciales
--drop table PedidosEspecialesMovimientosDeMercancia
--drop table PedidosEspecialesAbonosCuentasPorCobrar
--drop table PedidosEspecialesCuentasPorCobrar
--drop table PedidosEspecialesLog
--drop table PedidosEspecialesDetalle
--drop table PedidosEspeciales


-- CatEstatusPedidoEspecial

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'CatEstatusPedidoEspecial' and xtype = 'u')
	drop table CatEstatusPedidoEspecial

CREATE TABLE 
	CatEstatusPedidoEspecial 
		(
			idEstatusPedidoEspecial			int primary key identity(1,1),
			descripcion						varchar(255),
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON CatEstatusPedidoEspecial TO PUBLIC
GO
	

Insert into catestatuspedidoespecial (descripcion) values ('Solicitado')
Insert into catestatuspedidoespecial (descripcion) values ('Cotizado')
Insert into catestatuspedidoespecial (descripcion) values ('En resguardo')
Insert into catestatuspedidoespecial (descripcion) values ('Entregado y pagado')
Insert into catestatuspedidoespecial (descripcion) values ('Entregado a repartidor sin ser pagado')
Insert into catestatuspedidoespecial (descripcion) values ('Pagado')
Insert into catestatuspedidoespecial (descripcion) values ('Entregado a cr�dito')
Insert into catestatuspedidoespecial (descripcion) values ('Cancelado')


-- CatTipoPagoPedidoEspecial	

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'CatTipoPagoPedidoEspecial' and xtype = 'u')
	drop table CatTipoPagoPedidoEspecial

CREATE TABLE 
	CatTipoPagoPedidoEspecial 
		(
			idTipoPago			int primary key identity(1,1),
			descripcion			varchar(255)
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON CatTipoPagoPedidoEspecial TO PUBLIC
GO

--insert into CatTipoPagoPedidoEspecial (descripcion)	values ('')
	
	
	
-- CatEstatusCuentaPorCobrar

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'CatEstatusCuentaPorCobrar' and xtype = 'u')
	drop table CatEstatusCuentaPorCobrar

CREATE TABLE 
	CatEstatusCuentaPorCobrar 
		(
			idEstatusCuentaPorCobrar		int primary key identity(1,1),
			descripcion						varchar(255),
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON CatEstatusCuentaPorCobrar TO PUBLIC
GO
	
insert into CatEstatusCuentaPorCobrar (descripcion) values ('En Cr�dito.')
insert into CatEstatusCuentaPorCobrar (descripcion) values ('Liquidado.')
insert into CatEstatusCuentaPorCobrar (descripcion) values ('En Cobranza.')

	
	
	
-- CatEstatusPedidoEspecialDetalle

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'CatEstatusPedidoEspecialDetalle' and xtype = 'u')
	drop table CatEstatusPedidoEspecialDetalle

CREATE TABLE 
	CatEstatusPedidoEspecialDetalle 
		(
			idEstatusPedidoEspecialDetalle			int primary key identity(1,1),
			descripcion								varchar(255),
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON CatEstatusPedidoEspecialDetalle TO PUBLIC
GO	
	

insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Solicitados')
insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Atendidos')
insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Rechazados')
insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Aceptados')
insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Atendidos/Incompletos')
insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Rechazados por el Administrador')
insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Cancelados')

GO
IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'CatTipoTicketPedidosEspeciales' and xtype = 'u')
	drop table CatTipoTicketPedidosEspeciales

CREATE TABLE 
	CatTipoTicketPedidosEspeciales 
		(
			idTipoTicketPedidoEspecial	int primary key identity(1,1),
			tipoTicket varchar(100)
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON CatTipoTicketPedidosEspeciales TO PUBLIC
GO

INSERT INTO CatTipoTicketPedidosEspeciales(tipoTicket)
values('Ticket Original'),('Ticket Devoluci�n')

GO


--------------------------------------------
-- Tablas
--------------------------------------------


--PedidosEspeciales	

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspeciales' and xtype = 'u')
	drop table PedidosEspeciales
	
CREATE TABLE 
	PedidosEspeciales 
		(
			idPedidoEspecial				bigint primary key IDENTITY(1, 1),
			idCliente						int,
			cantidad						float,
			fechaAlta						datetime,
			montoTotal						money,
			idUsuario						int,
			idEstatusPedidoEspecial			int,
			idEstacion						int,
			observaciones					varchar(500),
			codigoBarras					varchar(250),
			idTicketMayoreo					int,
			idUsuarioEntrega				int,				--Ruteo ,taxi,0 = (Cliente)
			numeroUnidadTaxi				varchar(100),		--0,36
			liquidado						bit default 0,
			idComision						int,
			diasCredito						int,
			fechaEntrega					datetime,
			idFactMetodoPago				int,
			idFactFormaPago					int,
			idFactUsoCFDI					int,
			montoPagado						money,
			idUsuarioCancelacion			int,
			fechaCancelacion				datetime,
			esRevisionHandHeld				bit
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspeciales TO PUBLIC
GO

ALTER TABLE PedidosEspeciales ADD FOREIGN KEY ([idCliente]) REFERENCES [Clientes] ([idCliente])
GO

ALTER TABLE PedidosEspeciales ADD FOREIGN KEY ([idUsuario]) REFERENCES [Usuarios] ([idUsuario])
GO

ALTER TABLE PedidosEspeciales ADD FOREIGN KEY ([idEstacion]) REFERENCES [Estaciones] ([idEstacion])
GO

ALTER TABLE PedidosEspeciales ADD FOREIGN KEY ([idEstatusPedidoEspecial]) REFERENCES [CatEstatusPedidoEspecial] ([idEstatusPedidoEspecial])
GO

--ALTER TABLE PedidosEspeciales ADD FOREIGN KEY ([idTipoPago]) REFERENCES [CatEstatusPedidoEspecial] ([idTipoPago])
--GO

ALTER TABLE PedidosEspeciales ADD FOREIGN KEY ([idUsuarioEntrega]) REFERENCES [Usuarios] ([idUsuario])
GO


--PedidosEspecialesDetalle

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesDetalle' and xtype = 'u')
	drop table PedidosEspecialesDetalle

CREATE TABLE 
	PedidosEspecialesDetalle 
		(
			idPedidoEspecialDetalle				bigint primary key IDENTITY(1, 1),
			idPedidoEspecial					bigint,
			idVenta								int,
			idProducto							int,
			idUbicacion							int,
			idAlmacenOrigen						int,
			idAlmacenDestino					int,
			fechaAlta							datetime,
			cantidad							float,
			monto								money,
			cantidadActualInvGeneral			float,
			cantidadAnteriorInvGeneral			float,
			precioIndividual					money,
			precioMenudeo						money,
			precioRango							money,
			precioVenta							money,
			montoIva							money,
			montoComisionBancaria				money,
			observaciones						varchar(255),
			ultimoCostoCompra					money,
			cantidadAceptada					float,
			cantidadAtendida					float,
			cantidadRechazada					float,
			idEstatusPedidoEspecialDetalle		int,
			observacionesConfirmar				varchar(255),
			notificado							bit
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesDetalle TO PUBLIC
GO
	
ALTER TABLE PedidosEspecialesDetalle ADD FOREIGN KEY (idPedidoEspecial) REFERENCES PedidosEspeciales (idPedidoEspecial)
GO

ALTER TABLE PedidosEspecialesDetalle ADD FOREIGN KEY (idVenta) REFERENCES Ventas (idVenta)
GO

ALTER TABLE PedidosEspecialesDetalle ADD FOREIGN KEY (idProducto) REFERENCES Productos (idProducto)
GO

ALTER TABLE PedidosEspecialesDetalle ADD FOREIGN KEY (idUbicacion) REFERENCES Ubicacion (idUbicacion)
GO

ALTER TABLE PedidosEspecialesDetalle ADD FOREIGN KEY (idAlmacenOrigen) REFERENCES Almacenes (idAlmacen)
GO

ALTER TABLE PedidosEspecialesDetalle ADD FOREIGN KEY (idAlmacenDestino) REFERENCES Almacenes (idAlmacen)
GO

ALTER TABLE PedidosEspecialesDetalle ADD FOREIGN KEY (idEstatusPedidoEspecialDetalle) REFERENCES CatEstatusPedidoEspecialDetalle (idEstatusPedidoEspecialDetalle)
GO

-- PedidosEspecialesLog
IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesLog' and xtype = 'u')
	drop table PedidosEspecialesLog

CREATE TABLE 
	PedidosEspecialesLog 
		(
			idPedidoEspecialLog				bigint primary key IDENTITY(1, 1),	
			idPedidoEspecial 				bigint,
			idCliente						int,
			fechaAlta						datetime,
			idEstatusPedidoEspecial			int,
			idUsuario						int,
			observaciones					varchar(255),
			idAlmacenOrigen					int,
			idAlmacenDestino				int,  
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesLog TO PUBLIC
GO
	
ALTER TABLE PedidosEspecialesLog ADD FOREIGN KEY (idPedidoEspecial) REFERENCES PedidosEspeciales (idPedidoEspecial)
GO

ALTER TABLE PedidosEspecialesLog ADD FOREIGN KEY (idCliente) REFERENCES Clientes (idCliente)
GO

ALTER TABLE PedidosEspecialesLog ADD FOREIGN KEY (idEstatusPedidoEspecial) REFERENCES CatEstatusPedidoEspecial (idEstatusPedidoEspecial)
GO

ALTER TABLE PedidosEspecialesLog ADD FOREIGN KEY (idUsuario) REFERENCES Usuarios (idUsuario)
GO

ALTER TABLE PedidosEspecialesLog ADD FOREIGN KEY (idAlmacenOrigen) REFERENCES Almacenes (idAlmacen)
GO

ALTER TABLE PedidosEspecialesLog ADD FOREIGN KEY (idAlmacenDestino) REFERENCES Almacenes (idAlmacen)
GO



-- PedidosEspecialesCuentasPorCobrar

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesCuentasPorCobrar' and xtype = 'u')
	drop table PedidosEspecialesCuentasPorCobrar

CREATE TABLE 
	PedidosEspecialesCuentasPorCobrar	
		(
			idCuentaPorCobrar				bigint primary key IDENTITY(1, 1),
			idPedidoEspecial				bigint,
			idCliente						int,
			idUsuario						int,
			idTipoPago						int,
			fechaAlta						datetime,
			SaldoInicial					money, --1000
			saldoActual						money, --800
			idEstatusCuentaPorCobrar		int,
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesCuentasPorCobrar TO PUBLIC
GO
	

ALTER TABLE PedidosEspecialesCuentasPorCobrar ADD FOREIGN KEY (idPedidoEspecial) REFERENCES PedidosEspeciales (idPedidoEspecial)
GO

ALTER TABLE PedidosEspecialesCuentasPorCobrar ADD FOREIGN KEY (idCliente) REFERENCES Clientes (idCliente)
GO

ALTER TABLE PedidosEspecialesCuentasPorCobrar ADD FOREIGN KEY (idUsuario) REFERENCES Usuarios (idUsuario)
GO

--ALTER TABLE PedidosEspecialesCuentasPorCobrar ADD FOREIGN KEY (idTipoPago) REFERENCES _ (idTipoPago)
--GO

ALTER TABLE PedidosEspecialesCuentasPorCobrar ADD FOREIGN KEY (idEstatusCuentaPorCobrar) REFERENCES CatEstatusCuentaPorCobrar (idEstatusCuentaPorCobrar)
GO

--PedidosEspecialesAbonoClientes
IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesAbonoClientes' and xtype = 'u')
	drop table PedidosEspecialesAbonoClientes

CREATE TABLE 
	PedidosEspecialesAbonoClientes 
		(
			idAbonoCliente			bigint primary key IDENTITY(1, 1),
			idUsuario				int,						
			monto					money,	
			montoIva				money,
			montoComision			money,
			montoTotal				money,
			idCliente				int,
			requiereFactura			bit,
			idFacturaAbono		    bigint,
			idFactura				int,			
			idFactFormaPago			int,
			idFactUsoCFDI			int,
			fechaAlta				datetime,
			activo					bit,
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesAbonoClientes TO PUBLIC





-- PedidosEspecialesAbonosCuentasPorCobrar

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesAbonosCuentasPorCobrar' and xtype = 'u')
	drop table PedidosEspecialesAbonosCuentasPorCobrar

CREATE TABLE 
	PedidosEspecialesAbonosCuentasPorCobrar 
		(
			idAbono					bigint primary key IDENTITY(1, 1),
			monto					money,
			fechaAlta				datetime,
			idCliente				int,
			idUsuario				int,
			idPedidoEspecial		bigint,
			idCuentaPorCobrar		bigint,
			idAbonoCliente			bigint,
			EsAbonoInicial			bit default 0,
			SaldoDespuesOperacion   money default 0
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesAbonosCuentasPorCobrar TO PUBLIC

GO

ALTER TABLE PedidosEspecialesAbonosCuentasPorCobrar ADD FOREIGN KEY (idCliente) REFERENCES Clientes (idCliente)
GO

ALTER TABLE PedidosEspecialesAbonosCuentasPorCobrar ADD FOREIGN KEY (idUsuario) REFERENCES Usuarios (idUsuario)
GO

ALTER TABLE PedidosEspecialesAbonosCuentasPorCobrar ADD FOREIGN KEY (idPedidoEspecial) REFERENCES PedidosEspeciales (idPedidoEspecial)
GO

ALTER TABLE PedidosEspecialesAbonosCuentasPorCobrar ADD FOREIGN KEY (idCuentaPorCobrar) REFERENCES PedidosEspecialesCuentasPorCobrar (idCuentaPorCobrar)
GO



-- PedidosEspecialesMovimientosDeMercancia

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesMovimientosDeMercancia' and xtype = 'u')
	drop table PedidosEspecialesMovimientosDeMercancia

CREATE TABLE 
	PedidosEspecialesMovimientosDeMercancia 
		(
			idMovMercancia						bigint primary key identity(1,1),
			idAlmacenOrigen						int,
			idAlmacenDestino					int,
			idProducto							int,
			cantidad							float,
			idPedidoEspecial					bigint,
			idUsuario							int,
			fechaAlta							datetime,
			idEstatusPedidoEspecialDetalle		int,
			observaciones						varchar(500),
			cantidadAtendida					float,
			idUbicacionOrigen					int null,
			idUbicacionDestino					int null,
		) 
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesMovimientosDeMercancia TO PUBLIC
GO

ALTER TABLE PedidosEspecialesMovimientosDeMercancia ADD FOREIGN KEY (idAlmacenOrigen) REFERENCES Almacenes (idAlmacen)
GO

ALTER TABLE PedidosEspecialesMovimientosDeMercancia ADD FOREIGN KEY (idAlmacenDestino) REFERENCES Almacenes (idAlmacen)
GO

ALTER TABLE PedidosEspecialesMovimientosDeMercancia ADD FOREIGN KEY (idProducto) REFERENCES Productos (idProducto)
GO

ALTER TABLE PedidosEspecialesMovimientosDeMercancia ADD FOREIGN KEY (idPedidoEspecial) REFERENCES PedidosEspeciales (idPedidoEspecial)
GO

ALTER TABLE PedidosEspecialesMovimientosDeMercancia ADD FOREIGN KEY (idUsuario) REFERENCES Usuarios (idUsuario)
GO

ALTER TABLE PedidosEspecialesMovimientosDeMercancia ADD FOREIGN KEY (idEstatusPedidoEspecialDetalle) REFERENCES CatEstatusPedidoEspecialDetalle (idEstatusPedidoEspecialDetalle)
GO





-- PedidosEspecialesCierres

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesCierres' and xtype = 'u')
	drop table PedidosEspecialesCierres

CREATE TABLE 
	PedidosEspecialesCierres	
		(
			idCierrePedidoEspecial				bigint primary key IDENTITY(1, 1),			
			idUsuario							int,
			idEstacion							int,
			fechaAlta							datetime,		
			idEstatusRetiro						int,		--este cat ya existe
			montoAutorizado						money,
			idUsuarioAut						int,
			MontoIngresosEfectivo				money,
			MontoRetirosEfectivo				money,
			MontoCierreEfectivo					money,
			MontoCierreTC						money,
			EfectivoEntregadoEnCierre			money,
			noDevoluciones						int,
			NoTicketsEfectivo					int,
			NoTicketsCredito					int,
			NoPedidosEnResguardo				int,
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesCierres TO PUBLIC
GO
	
ALTER TABLE PedidosEspecialesCierres ADD FOREIGN KEY (idUsuario) REFERENCES Usuarios (idUsuario)
GO

ALTER TABLE PedidosEspecialesCierres ADD FOREIGN KEY (idEstacion) REFERENCES Estaciones (idEstacion)
GO


-- PedidosEspecialesCierresDetalle

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesCierresDetalle' and xtype = 'u')
	drop table PedidosEspecialesCierresDetalle

CREATE TABLE 
	PedidosEspecialesCierresDetalle 
		(
			idCierreDetallePedidoEspecial		bigint primary key IDENTITY(1, 1),
			idCierrePedidoEspecial	bigint,
			idAlmacen					int,
			descripcion					varchar(250),
			VentasContado				money default 0,
			NoVentasContado				int default 0,
			VentasTC					money default 0,
			NoVentasTC					int default 0,
			VentasTransferencias		money default 0,
			NoVentasTransferencias		int default 0,
			VentasOtrasFormasPago		money default 0,
			NoVentasOtrasFormasPago		int default 0,
			VentasCredito				money default 0,
			NoVentasCredito				int default 0,
			MontoDevoluciones			money default 0,
			NoDevoluciones				int default 0,
			TotalEfectivo			    money default 0,
			TotalCreditoTransferencias  money default 0,
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesCierresDetalle TO PUBLIC
GO


--tickets pedidos especiales
IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'TicketsPedidosEspeciales' and xtype = 'u')
	drop table TicketsPedidosEspeciales

CREATE TABLE 
	TicketsPedidosEspeciales 
		(
			idTicketPedidoEspecial		bigint primary key identity(1,1),
			idTipoTicketPedidoEspecial	int,
			idPedidoEspecial			bigint,
			idUsuario					int,			
			cantidad					float,
			monto						money,
			comisionBancaria			money,
			montoIVA					money,
			montoTotal					money,
			fechaAlta					datetime,			
			observaciones				varchar(255)
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON TicketsPedidosEspeciales TO PUBLIC
GO

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'TicketsPedidosEspecialesDetalle' and xtype = 'u')
	drop table TicketsPedidosEspecialesDetalle

CREATE TABLE 
	TicketsPedidosEspecialesDetalle 
		(
			idTicketPedidoEspecialDetalle		bigint primary key identity(1,1),
			idTicketPedidoEspecial				bigint,			
			idPedidoEspecial					bigint,
			idPedidoEspecialDetalle				bigint,
			idProducto							int,			
			cantidad							float,
			monto								money,
			montoComision						money,
			montoIVA							money,
			montoTotal						    money,
			precioVenta							money,
			precioIndividual					money,
			precioMenudeo					    money,
			precioRango							money,
			cantidadActualInvGeneral			float,	
			cantidadAnteriorInvGeneral			float,
			fechaAlta							datetime,
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON TicketsPedidosEspecialesDetalle TO PUBLIC
GO







-- ConfiguracionesPedidosEspeciales

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'ConfiguracionesPedidosEspeciales' and xtype = 'u')
	drop table ConfiguracionesPedidosEspeciales

CREATE TABLE 
	ConfiguracionesPedidosEspeciales 
		(
			idConfig			int	primary key identity(1,1),
			descripcion			varchar(300),
			valor				float,
			activo				bit
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON ConfiguracionesPedidosEspeciales TO PUBLIC
GO

---- PedidosEspecialesIngresosEfectivo

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesIngresosEfectivo' and xtype = 'u')
	drop table PedidosEspecialesIngresosEfectivo

CREATE TABLE 
	PedidosEspecialesIngresosEfectivo 
		(
			idIngresoPedidoEspecial bigint primary key identity(1,1),
			monto money,
			idUsuario int,
			fechaAlta datetime,
			idTipoIngreso int
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesIngresosEfectivo TO PUBLIC
GO

ALTER TABLE PedidosEspecialesIngresosEfectivo ADD FOREIGN KEY (idUsuario) REFERENCES Usuarios (idUsuario)
GO
ALTER TABLE PedidosEspecialesIngresosEfectivo ADD FOREIGN KEY (idTipoIngreso) REFERENCES CatTipoIngreso (idTipoIngreso)
GO

---- PedidosEspecialesRetirosExcesoEfectivo

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesRetirosExcesoEfectivo' and xtype = 'u')
	drop table PedidosEspecialesRetirosExcesoEfectivo

CREATE TABLE 
	PedidosEspecialesRetirosExcesoEfectivo 
		(
			idRetiro bigint primary key identity(1,1),
			montoRetiro money,
			idUsuario int,
			idEstacion int,
			fechaAlta datetime,
			idEstatusRetiro int,
			montoAutorizado money,
			idUsuarioAut int
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesRetirosExcesoEfectivo TO PUBLIC
GO

ALTER TABLE PedidosEspecialesRetirosExcesoEfectivo ADD FOREIGN KEY (idUsuario) REFERENCES Usuarios (idUsuario)
GO

----Categorias
IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'Categorias' and xtype = 'u')
	drop table Categorias

CREATE TABLE 
	Categorias 
		(
			idCategoria			int primary key identity(1,1),
			descripcion			varchar(255),
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON Categorias TO PUBLIC
GO

insert into Categorias(descripcion) values('Jarcieria'),('Liquidos'),('Traperos')

----AlmacenesPorCategorias
IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'AlmacenesPorCategorias' and xtype = 'u')
	drop table AlmacenesPorCategorias

CREATE TABLE 
	AlmacenesPorCategorias 
		(
			contador			int primary key identity(1,1),
			idAlmacen			int,
			idCategoria			int
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON AlmacenesPorCategorias TO PUBLIC
GO

insert into AlmacenesPorCategorias(idAlmacen,idCategoria) values(1,1)--Almacen General
insert into AlmacenesPorCategorias(idAlmacen,idCategoria) values(2,2)--Almacen Liquidos
insert into AlmacenesPorCategorias(idAlmacen,idCategoria) values(3,2)--Tienda Liquidos
insert into AlmacenesPorCategorias(idAlmacen,idCategoria) values(4,1)--Tienda Jarcier�a
insert into AlmacenesPorCategorias(idAlmacen,idCategoria) values(5,3)--Almacen Trapeadores
GO

insert into ConfiguracionesPedidosEspeciales (descripcion,valor,activo) values ('Dias para poder hacer complementos/devoluciones.', 30, cast(1 as bit) )
insert into ConfiguracionesPedidosEspeciales (descripcion,valor,activo) values ('Requiere autorizacion para cierre de modulo de Pedidos Especiales.', 1, cast(1 as bit) )


-- rol de taxi
if not exists ( select 1 from CatRoles  where descripcion like '%Taxi%')
begin
	insert into CatRoles  (descripcion, activo) values ( 'Taxi', cast(1 as bit) )
end

update CatRoles set descripcion = 'Ruteo' where descripcion like 'Encargado Ruteo'
go

-- se agregan columnas en inventario detalle log para los movimientos de mercancia de pedidos especiales
if not exists (select 1 FROM sys.columns where Name = N'idPedidoEspecial' AND Object_ID = Object_ID(N'InventarioDetalleLog'))
BEGIN
	alter table InventarioDetalleLog add idPedidoEspecial bigint
END
go

------------------------------------------------------------------------------------------------------------------
--select * from PedidosEspeciales
--select * from PedidosEspecialesDetalle
--select * from PedidosEspecialesLog
--select * from PedidosEspecialesCuentasPorCobrar
--select * from PedidosEspecialesAbonosCuentasPorCobrar
--select * from PedidosEspecialesMovimientosDeMercancia
--select * from PedidosEspecialesCierresPedidosEspeciales
--select * from PedidosEspecialesCierresPedidosEspecialesDetalle
--select * from PedidosEspecialesRetiroEfectivoPedidosEspeciales
--select * from PedidosEspecialesDevoluciones
--select * from PedidosEspecialesDevolucionesDetalle
--select * from ConfiguracionesPedidosEspeciales
--select * from CatEstatusPedidoEspecial
--select * from CatTipoPagoPedidoEspecial
--select * from CatEstatusCuentaPorCobrar
--select * from CatEstatusPedidoEspecialDetalle
------------------------------------------------------------------------------------------------------------------


