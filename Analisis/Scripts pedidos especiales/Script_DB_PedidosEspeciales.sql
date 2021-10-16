



insert into CatPiso  ( descripcion) values ( 'Resguardo')
select * from CatPiso 

insert into CatRaq ( descripcion) values ( 'Resguardo')
select * from CatRaq

insert into CatPasillo ( descripcion) values ( 'Resguardo')
select * from CatPasillo





insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(salida de mercancia por pedido especial)',	-1)
insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de mercancia por pedido especial)',	1)
insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de mercancia por pedido especial aceptado)',	1)
insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de mercancia por pedido especial rechazado)',	1)
insert into CatTipoMovimientoInventario (descripcion,operacion) values ('Actualizacion de Inventario(carga de mercancia por sobrante de pedido especial)', 1)

--select * from CatTipoMovimientoInventario

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
Insert into catestatuspedidoespecial (descripcion) values ('Entregado a crédito')


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
	
insert into CatEstatusCuentaPorCobrar (descripcion) values ('En Crédito.')
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
	

insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Solcitados')
insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Atendidos')
insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Rechazados')
insert into CatEstatusPedidoEspecialDetalle (descripcion) values ('Aceptados')





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
			idTipoPago						int,
			idUsuarioEntrega				int,				--Ruteo ,taxi,0 = (Cliente)
			numeroUnidadTaxi				varchar(100)		--0,36
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
			idTicketMayoreo						int,
			observaciones						varchar(255),
			ultimoCostoCompra					money,
			cantidadAceptada					float,
			cantidadAtendida					float,
			cantidadRechazada					float,
			idEstatusPedidoEspecialDetalle		int,
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
			cantidadAtendida					float
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





-- PedidosEspecialesCierresPedidosEspeciales

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesCierresPedidosEspeciales' and xtype = 'u')
	drop table PedidosEspecialesCierresPedidosEspeciales

CREATE TABLE 
	PedidosEspecialesCierresPedidosEspeciales	
		(
			idCierrePedidoEspecial				bigint primary key IDENTITY(1, 1),
			monto								money,
			idUsuario							int,
			idEstacion							int,
			fechaAlta							datetime,		
			idEstatusRetiro						int,		--este cat ya existe
			montoAutorizado						money,
			idUsuarioAut						int,
			MontoRetiroEfectivo					money,
			MontoTotalPedidosEspeciales			money,
			MontoCierreDia						money,
			MontoApertura						money,
			MontoIngresoEfectivo				money,
			MontoTotalPedidosContado			money,
			MontoTotalPedidosTarjeta			money,
			MontoTotalPedidosTransferencias		money,
			MontoTotalPedidosOtros				money,
			MontoTotalPedidosCancelados			money,
			MontoTotalPedidosDevueltos			money,
			TotalPedidos						int,	
			EfectivoEntregadoEnCierre			money,
			noDevoluciones						int,
			NoTicketsEfectivo					int,
			NoTicketsCredito					int,
			NoPedidosEnResguardo				int,
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesCierresPedidosEspeciales TO PUBLIC
GO
	
ALTER TABLE PedidosEspecialesCierresPedidosEspeciales ADD FOREIGN KEY (idUsuario) REFERENCES Usuarios (idUsuario)
GO

ALTER TABLE PedidosEspecialesCierresPedidosEspeciales ADD FOREIGN KEY (idEstacion) REFERENCES Estaciones (idEstacion)
GO

--ALTER TABLE PedidosEspecialesCierresPedidosEspeciales ADD FOREIGN KEY (idEstatusRetiro) REFERENCES CatEstatusRetiros (idEstatusRetiro)
--GO

ALTER TABLE PedidosEspecialesCierresPedidosEspeciales ADD FOREIGN KEY (idUsuarioAut) REFERENCES Usuarios (idUsuario)
GO




-- PedidosEspecialesCierresPedidosEspecialesDetalle

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesCierresPedidosEspecialesDetalle' and xtype = 'u')
	drop table PedidosEspecialesCierresPedidosEspecialesDetalle

CREATE TABLE 
	PedidosEspecialesCierresPedidosEspecialesDetalle 
		(
			idCierrePedidoEspecial		bigint primary key IDENTITY(1, 1),
			idAlmacen					int,
			VentasContado				int,
			VentasTC					int,
			VentasCredito				int,
			MontoDevoluciones			money,
			IngresoPorPagos				money
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesCierresPedidosEspecialesDetalle TO PUBLIC
GO

ALTER TABLE PedidosEspecialesCierresPedidosEspecialesDetalle ADD FOREIGN KEY (idAlmacen) REFERENCES Almacenes (idAlmacen)
GO



	

	



-- PedidosEspecialesRetiroEfectivoPedidosEspeciales

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesRetiroEfectivoPedidosEspeciales' and xtype = 'u')
	drop table PedidosEspecialesRetiroEfectivoPedidosEspeciales

CREATE TABLE 
	PedidosEspecialesRetiroEfectivoPedidosEspeciales 
		(
			idRetiroPedidoEspecial			bigint primary key IDENTITY(1, 1),
			montoRetiro						money,
			idUsuario						int,
			idEstacion						int,
			fechaAlta						datetime,
			idEstatusRetiro					int,
			montoAutorizado					money,
			idUsuarioAut					int,
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesRetiroEfectivoPedidosEspeciales TO PUBLIC
GO
	
ALTER TABLE PedidosEspecialesRetiroEfectivoPedidosEspeciales ADD FOREIGN KEY (idUsuario) REFERENCES Usuarios (idUsuario)
GO

ALTER TABLE PedidosEspecialesRetiroEfectivoPedidosEspeciales ADD FOREIGN KEY (idEstacion) REFERENCES Estaciones (idEstacion)
GO

--ALTER TABLE PedidosEspecialesRetiroEfectivoPedidosEspeciales ADD FOREIGN KEY (idEstatusRetiro) REFERENCES CatEstatusRetiros (idEstatusRetiro)
--GO

ALTER TABLE PedidosEspecialesRetiroEfectivoPedidosEspeciales ADD FOREIGN KEY (idUsuarioAut) REFERENCES Usuarios (idUsuario)
GO





-- PedidosEspecialesDevoluciones

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesDevoluciones' and xtype = 'u')
	drop table PedidosEspecialesDevoluciones

CREATE TABLE 
	PedidosEspecialesDevoluciones 
		(
			idDevolucion			bigint primary key identity(1,1),
			idVenta					int,
			idUsuario				int,
			idCliente				int,
			cantidad				float,
			fechaAlta				datetime,
			montoTotal				money,
			idFactFormaPago			int,
			idEstacion				int,
			observaciones			varchar(255)
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesDevoluciones TO PUBLIC
GO

ALTER TABLE PedidosEspecialesDevoluciones ADD FOREIGN KEY (idVenta) REFERENCES Ventas (idVenta)
GO

ALTER TABLE PedidosEspecialesDevoluciones ADD FOREIGN KEY (idUsuario) REFERENCES Usuarios (idUsuario)
GO

ALTER TABLE PedidosEspecialesDevoluciones ADD FOREIGN KEY (idCliente) REFERENCES Clientes (idCliente)
GO

ALTER TABLE PedidosEspecialesDevoluciones ADD FOREIGN KEY (idFactFormaPago) REFERENCES FactCatFormaPago (id)
GO

ALTER TABLE PedidosEspecialesDevoluciones ADD FOREIGN KEY (idEstacion) REFERENCES Estaciones (idEstacion)
GO


-- PedidosEspecialesDevolucionesDetalle

IF EXISTS (SELECT * FROM sysobjects WHERE NAME like 'PedidosEspecialesDevolucionesDetalle' and xtype = 'u')
	drop table PedidosEspecialesDevolucionesDetalle

CREATE TABLE 
	PedidosEspecialesDevolucionesDetalle 
		(
			idDevolucionDetalle					bigint primary key identity(1,1),
			idVenta								int,
			idProducto							int,
			cantidad							float,
			monto								money,
			idDevolucion						bigint,
			montoDevueltoComisionBancaria		money
		)
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON PedidosEspecialesDevolucionesDetalle TO PUBLIC
GO

ALTER TABLE PedidosEspecialesDevolucionesDetalle ADD FOREIGN KEY (idVenta) REFERENCES Ventas (idVenta)
GO

ALTER TABLE PedidosEspecialesDevolucionesDetalle ADD FOREIGN KEY (idProducto) REFERENCES Productos (idProducto)
GO

ALTER TABLE PedidosEspecialesDevolucionesDetalle ADD FOREIGN KEY (idDevolucion) REFERENCES PedidosEspecialesDevoluciones (idDevolucion)
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


insert into ConfiguracionesPedidosEspeciales (descripcion,valor,activo) values ('Dias para poder hacer complementos/devoluciones.', 30, cast(1 as bit) )
insert into ConfiguracionesPedidosEspeciales (descripcion,valor,activo) values ('Requiere autorizacion para cierre de modulo de Pedidos Especiales.', 1, cast(1 as bit) )


-- rol de taxi
insert into CatRoles  (descripcion, activo) values ( 'Taxi', cast(1 as bit) )



------------------------------------------------------------------------------------------------------------------
select * from PedidosEspeciales
select * from PedidosEspecialesDetalle
select * from PedidosEspecialesLog
select * from PedidosEspecialesCuentasPorCobrar
select * from PedidosEspecialesAbonosCuentasPorCobrar
select * from PedidosEspecialesMovimientosDeMercancia
select * from PedidosEspecialesCierresPedidosEspeciales
select * from PedidosEspecialesCierresPedidosEspecialesDetalle
select * from PedidosEspecialesRetiroEfectivoPedidosEspeciales
select * from PedidosEspecialesDevoluciones
select * from PedidosEspecialesDevolucionesDetalle
select * from ConfiguracionesPedidosEspeciales
select * from CatEstatusPedidoEspecial
select * from CatTipoPagoPedidoEspecial
select * from CatEstatusCuentaPorCobrar
select * from CatEstatusPedidoEspecialDetalle
------------------------------------------------------------------------------------------------------------------



