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

namespace lluviaBackEnd.DAO
{
    public class ClienteDAO
    {
        private IDbConnection _db;

        public Notificacion<Cliente> GuardarCliente(Cliente c)
        {
            Notificacion<Cliente> n;
            try
            {
                _db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString());
                var parameters = new DynamicParameters();

                parameters.Add("@idCliente", c.idCliente);
                parameters.Add("@nombres" , c.nombres);
                parameters.Add("@apellidoPaterno" , c.apellidoPaterno);
                parameters.Add("@apellidoMaterno", c.apellidoMaterno);
                parameters.Add("@telefono",c.telefono);
                parameters.Add("@correo" , c.correo);
                parameters.Add("@rfc",c.rfc);
                parameters.Add("@calle",c.calle);
                parameters.Add("@numeroExterior",c.numeroExterior);
                parameters.Add("@colonia",c.colonia);
                parameters.Add("@municipio",c.municipio);
                parameters.Add("@cp" ,c.cp);
                parameters.Add("@estado",c.estado);
                //parameters.Add("@fechaAlta",c.FechaAlta.ToString("yyyyMMdd"));
                //parameters.Add("@activo",c.activo);
                parameters.Add("@idTipoCliente",c.tipoCliente.idTipoCliente);
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
            List<Cliente> lstClientes = null;
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

        public List<TipoCliente> ObtenerTipoClientes()
        {
            List<TipoCliente> lstClientes = null;
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {

                    lstClientes = this._db.Query<TipoCliente>("SP_CONSULTA_TIPO_CLIENTES",  commandType: CommandType.StoredProcedure).ToList();
                    lstClientes.Insert(0 , new TipoCliente() { idTipoCliente=0 , descripcion="--SELECCIONA--" });
                    
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstClientes;
        }


        // Referencia para Consulta con dapper y  una clase que contiene otra clase
        //https://dzone.com/articles/tutorial-on-handling-multiple-resultsets-and-multi
        //https://dzone.com/articles/tutorial-on-handling-multiple-resultsets-and-multi

    }



}