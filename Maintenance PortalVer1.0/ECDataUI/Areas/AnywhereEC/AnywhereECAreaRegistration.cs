using System.Web.Mvc;

namespace ECDataUI.Areas.AnywhereEC
{
    public class AnywhereECAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AnywhereEC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AnywhereEC_default",
                "AnywhereEC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}