using System.Web.Mvc;

namespace ECDataUI.Areas.SROScriptManager
{
    public class SROScriptManagerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SROScriptManager";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SROScriptManager_default",
                "SROScriptManager/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}