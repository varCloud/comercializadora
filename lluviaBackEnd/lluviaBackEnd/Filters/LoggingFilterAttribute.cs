using lluviaBackEnd.Models;
using lluviaBackEnd.Utilerias;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.Filters
{
    public class LoggingFilterAttribute : ActionFilterAttribute
    {
        #region Logging
        /// <summary>
        /// Access to the log4Net logging object
        /// </summary>
        //protected static readonly log4net.ILog log =
        //  log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private const string StopwatchKey = "DebugLoggingStopWatch";

        #endregion

        // Método ejecutado justo antes de la ejecución de la acción
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                int idUsuario = 0;
                string tmpMsg = string.Empty;
                base.OnActionExecuting(filterContext);

                // Recorremos los parámetros de la acción y los mostramos
                IDictionary<string, object> actionParameters = filterContext.ActionParameters;

                HttpContext context = HttpContext.Current;
                Sesion sesion = (Sesion)context.Session["UsuarioActual"];
                if (sesion != null)
                    idUsuario = sesion.idUsuario;


                string json = JsonConvert.SerializeObject(actionParameters, Formatting.Indented);
                BitacoraRequest<string> Bitacora = new BitacoraRequest<string>(idUsuario.ToString(), json, filterContext.Controller.ToString());
                Utils.EscribirLog(SerializerManager<BitacoraRequest<string>>.SerealizarObjtecToString(Bitacora));

            }
            catch (Exception ex)
            {
                Utils.EscribirLog("Error al escribir en log en el metodo OnActionExecuting " + ex.Message);

            }

        }

        // Método ejecutado justo después de la ejecución de la acción
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                int idUsuario = 0;
                string tmpMsg = string.Empty;
                base.OnActionExecuted(filterContext);

                HttpContext context = HttpContext.Current;
                Sesion sesion = (Sesion)context.Session["UsuarioActual"];
                if (sesion != null)
                    idUsuario = sesion.idUsuario;

                if (filterContext.Exception != null)
                {
                    //log.Error("Error durante la ejecución de la acción", filterContext.Exception);

                    string json = JsonConvert.SerializeObject(filterContext.Exception, Formatting.Indented);
                    BitacoraException<string> Bitacora = new BitacoraException<string>(idUsuario.ToString(), json, filterContext.Controller.ToString());
                    Utils.EscribirLog(SerializerManager<BitacoraException<string>>.SerealizarObjtecToString(Bitacora));

                }
                else
                {
                    string json = JsonConvert.SerializeObject(filterContext.Result, Formatting.Indented);
                    BitacoraResponse<string> Bitacora = new BitacoraResponse<string>(idUsuario.ToString(), json, filterContext.Controller.ToString());
                    Utils.EscribirLog(SerializerManager<BitacoraResponse<string>>.SerealizarObjtecToString(Bitacora));
                }
            }
            catch (Exception ex)
            {
                Utils.EscribirLog("Error al escribir en log en el metodo OnActionExecuted " + ex.Message);
            }     

        }
    }
}