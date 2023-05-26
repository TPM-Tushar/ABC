using System.Web.Mvc;

namespace ECDataUI.Areas.BhoomiMapping
{
    public class BhoomiMappingAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get 
            {
                return "BhoomiMapping";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BhoomiMapping",
                "BhoomiMapping/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}