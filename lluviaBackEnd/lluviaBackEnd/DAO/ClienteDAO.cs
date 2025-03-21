﻿using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using System.Data;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using AccesoDatos;

namespace lluviaBackEnd.DAO
{
    public class ClienteDAO
    {
        private IDbConnection _db;

        public Notificacion<Cliente> GuardarCliente(Cliente c)
        {
            Notificacion<Cliente> n;
            System.Globalization.TextInfo ti = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            try
            {
                _db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString());
                var parameters = new DynamicParameters();

                parameters.Add("@idCliente", c.idCliente);
                parameters.Add("@nombres" , c.esPersonaMoral ?  c.razonSocial : string.IsNullOrEmpty(c.nombres) ? "": ti.ToTitleCase(c.nombres));
                parameters.Add("@apellidoPaterno" , c.esPersonaMoral ? "" : string.IsNullOrEmpty(c.apellidoPaterno) ? "" : ti.ToTitleCase(c.apellidoPaterno)   );
                parameters.Add("@apellidoMaterno",  c.esPersonaMoral ? "" : string.IsNullOrEmpty(c.apellidoMaterno) ? "" : ti.ToTitleCase(c.apellidoMaterno)   );
                parameters.Add("@telefono",c.telefono);
                parameters.Add("@correo" , c.correo);
                parameters.Add("@rfc",( c.esPersonaMoral ? c.rfcPM : c.rfc));
                parameters.Add("@calle",c.calle);
                parameters.Add("@numeroExterior",c.numeroExterior);
                parameters.Add("@colonia",c.colonia);
                parameters.Add("@municipio",c.municipio);
                parameters.Add("@cp" ,c.cp);
                parameters.Add("@estado",c.estado);
                parameters.Add("@numeroInterior", c.numeroInterior);
                parameters.Add("@localidad", c.localidad);
                parameters.Add("@idTipoCliente",c.idTipoCliente);
                parameters.Add("@esPersonaMoral", c.esPersonaMoral);
                parameters.Add("@nombreContacto", c.nombreContacto);
                parameters.Add("@sociedadMercantil", c.sociedadMercantil);
                /******************PEDIDOS ESPECIALES********************************/
                parameters.Add("@latitud", c.latitud);
                parameters.Add("@longitud", c.longitud);
                parameters.Add("@nombreContactoPE", c.nombreContactoPE);
                parameters.Add("@telefonoContactoPE", c.telefonoContactoPE);
                parameters.Add("@correoContactoPE", c.correoContactoPE);
                parameters.Add("@diasCredito", c.diasCredito);
                parameters.Add("@montoMaximoCredito", c.montoMaximoCredito);
                parameters.Add("@usarDatosCliente", c.usarDatosCliente);
                parameters.Add("@idRegimenFiscal", c.idRegimenFiscal);

                n =  this._db.QuerySingle<Notificacion<Cliente>>("SP_INSERTA_ACTUALIZA_CLIENTES", parameters, commandType:CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return n;

        }

        public Notificacion<Cliente> EliminarCliente(Cliente c)
        {
            Notificacion<Cliente> n;
            try
            {
                _db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString());
                var parameters = new DynamicParameters();

                parameters.Add("idCliente", c.idCliente);
                parameters.Add("activo", c.nombres);
                n = this._db.QuerySingle<Notificacion<Cliente>>("SP_ACTUALIZA_STATUS_CLIENTES", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return n;

        }

        public List<Cliente> ObtenerClientes(Cliente c)
        {
            List<Cliente> lstClientes = new List<Cliente>();
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("idCliente", c.idCliente);
                    var result = this._db.QueryMultiple("SP_CONSULTA_CLIENTES",  parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200) {
                        lstClientes = result.Read<Cliente, TipoCliente, Cliente>(MapResults ,splitOn: "idTipoCliente").ToList();
                        lstClientes.Select(x => { x.nombreCompleto_ = x.nombres.ToUpper() + " " + x.apellidoPaterno.ToUpper() + " " + x.apellidoMaterno.ToUpper() ; return x; }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstClientes;
        }
       
        private Cliente MapResults(Cliente cliente, TipoCliente tipoCliente)
        {
            cliente.tipoCliente = tipoCliente;
            return cliente;
        }

        public List<TipoCliente> ObtenerTipoClientes(TipoCliente tipoCliente )
        {
            List<TipoCliente> lstClientes = null;
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idTipoCliente", tipoCliente.idTipoCliente);
                    var result = this._db.QueryMultiple("SP_CONSULTA_TIPOS_CLIENTES", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        lstClientes = result.Read<TipoCliente>().ToList();
                    }

                    lstClientes.Insert(0 , new TipoCliente() {  descripcion="--SELECCIONA--" });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstClientes;
        }


        public Notificacion<TipoCliente> GuardarTipoCliente(TipoCliente tipoCliente)
        {
            Notificacion<TipoCliente> notificacion = new Notificacion<TipoCliente>();
            try
            {
                _db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString());
                var parameters = new DynamicParameters();

                parameters.Add("@idTipoCliente", tipoCliente.idTipoCliente);
                parameters.Add("@descripcion", tipoCliente.descripcion);
                parameters.Add("@descuento", tipoCliente.descuento);
                parameters.Add("@activo", tipoCliente.activo);
                var result = _db.QueryMultiple("SP_INSERTA_ACTUALIZA_TIPOS_CLIENTES", parameters, commandType: CommandType.StoredProcedure);

                var r1 = result.ReadFirst();
                
                if (r1.status == 200)
                {
                    notificacion.Estatus = r1.status;
                    notificacion.Mensaje = r1.mensaje;
                    notificacion.Modelo = tipoCliente; //result.Read<TipoCliente>();
                }
                else
                {
                    notificacion.Estatus = r1.status;
                    notificacion.Mensaje = r1.mensaje;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;

        }

        public Notificacion<List<TipoCliente>> ObtenerTiposClientes(TipoCliente tipoCliente)
        {
            Notificacion<List<TipoCliente>> notificacion = new Notificacion<List<TipoCliente>>();
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("idTipoCliente", tipoCliente.idTipoCliente);
                    var result = _db.QueryMultiple("SP_CONSULTA_TIPOS_CLIENTES", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<TipoCliente>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<TipoCliente> EliminarTipoCliente(TipoCliente tipoCliente)
        {
            Notificacion<TipoCliente> notificacion = new Notificacion<TipoCliente>();
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idTipoCliente", tipoCliente.idTipoCliente);
                    parameters.Add("@activo", tipoCliente.activo);
                    var result = _db.QueryMultiple("SP_ACTUALIZA_STATUS_TIPOS_CLIENTES", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = tipoCliente;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = tipoCliente;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public List<SelectListItem> ObteneRegimenFiscal()
        {
            List<SelectListItem> lstRegimenFiscal = new List<SelectListItem>();
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    _db.Open();
                    var result = _db.QueryMultiple("SP_FACTURACION_OBTENER_REGIMEN_FISCAL", null, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        lstRegimenFiscal = result.Read<SelectListItem>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            lstRegimenFiscal.Insert(0, new SelectListItem { Text = "-- SELECCIONA --", Value = "" , Selected = true });
            return lstRegimenFiscal;
        }




        // Referencia para Consulta con dapper y  una clase que contiene otra clase
        //https://dzone.com/articles/tutorial-on-handling-multiple-resultsets-and-multi
        //https://dzone.com/articles/tutorial-on-handling-multiple-resultsets-and-multi

    }



}