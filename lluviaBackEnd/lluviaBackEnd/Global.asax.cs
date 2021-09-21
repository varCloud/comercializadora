using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using System.Web.Routing;
using lluviaBackEnd.App_Start;

namespace lluviaBackEnd
{
    public class MvcApplication : System.Web.HttpApplication
    {
       

        protected void Application_Start()
        {
            //JsonValueProviderFactory jsonValueProviderFactory = null;
            //foreach (var factory in ValueProviderFactories.Factories)
            //{
            //    if (factory is JsonValueProviderFactory)
            //    {
            //        jsonValueProviderFactory = factory as JsonValueProviderFactory;
            //    }
            //}
            ////remove the default JsonVAlueProviderFactory
            //if (jsonValueProviderFactory != null) ValueProviderFactories.Factories.Remove(jsonValueProviderFactory);
            ////add the custom one
            //ValueProviderFactories.Factories.Add(new CustomJsonValueProviderFactory());

            GlobalConfiguration.Configure(WebApiConfig.Register);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();


        }
    }
}
