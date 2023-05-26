using System.Web.Mvc;

namespace ECDataUI.Areas.DynamicDataReader
{
    public class DynamicDataReaderAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DynamicDataReader";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DynamicDataReader_default",
                "DynamicDataReader/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}