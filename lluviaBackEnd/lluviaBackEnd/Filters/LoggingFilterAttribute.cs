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
        protected static readonly log4net.ILog log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string StopwatchKey = "DebugLoggingStopWatch";

        #endregion

        // Método ejecutado justo antes de la ejecución de la acción
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string tmpMsg = string.Empty;
            base.OnActionExecuting(filterContext);

            // Almacenamos el nombre del método
            //log.Debug(System.Reflection.MethodBase.GetCurrentMethod().ToString());

            // Recorremos los parámetros de la acción y los mostramos
            IDictionary<string, object> actionParameters = filterContext.ActionParameters;


            foreach (object values in actionParameters.Values)
            {
                tmpMsg += "[" + values.GetType().ToString() + "] ";
                string[] propertyNames = values.GetType().GetProperties().Select(p => p.Name).ToArray();
                foreach (var prop in propertyNames)
                {
                    var propValue = values.GetType().GetProperty(prop).GetValue(values);
                    tmpMsg += "[" + prop + ": " + propValue + "] ";
                }
            }
                

            log.Debug("Controller: " + filterContext.Controller);
            log.Debug("ActionParameters: " + tmpMsg);
            log.Debug(" --------------------------------------- ");
        }

        // Método ejecutado justo después de la ejecución de la acción
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string tmpMsg = string.Empty;
            base.OnActionExecuted(filterContext);

            // Almacenamos el nombre del método
            //log.Debug(System.Reflection.MethodBase.GetCurrentMethod().ToString());

            log.Debug("Controller: " + filterContext.Controller);
            // Recogemos el resultado
            ActionResult result = filterContext.Result;
            log.Debug("ActionResult: " + result.ToString());

            // Comprobamos si se ha producido alguna excepción durante la ejecución.
            // En caso afirmativo, la almacenamos
            if (filterContext.Exception != null)
                log.Error("Error durante la ejecución de la acción", filterContext.Exception);

            log.Debug(" --------------------------------------- ");

        }

        // Método ejecutado justo antes de la ejecución del resultado
        //public override void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    base.OnResultExecuting(filterContext);
        //    log.Debug(System.Reflection.MethodBase.GetCurrentMethod().ToString());

        //    log.Debug("Controller: " + filterContext.Controller);
        //    Recogemos el resultado
        //   ActionResult result = filterContext.Result;
        //    log.Debug("ActionResult: " + result.ToString());

        //    log.Debug(" --------------------------------------- ");
        //}

        //Método ejecutado justo después de la ejecución del resultado
        //public override void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    base.OnResultExecuted(filterContext);
        //    log.Debug(System.Reflection.MethodBase.GetCurrentMethod().ToString());
        //    log.Debug("Controller: " + filterContext.Controller);
        //    Recogemos el resultado
        //   ActionResult result = filterContext.Result;
        //    log.Debug("ActionResult: " + result.ToString());

        //    log.Debug(" --------------------------------------- ");
        //}


    }
}