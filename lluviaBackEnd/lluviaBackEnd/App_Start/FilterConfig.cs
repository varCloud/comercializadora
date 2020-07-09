using lluviaBackEnd.Filters;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LoggingFilterAttribute());
        }
    }
}
