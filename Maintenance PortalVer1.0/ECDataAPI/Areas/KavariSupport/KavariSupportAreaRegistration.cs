using System.Web.Mvc;

namespace ECDataAPI.Areas.KavariSupport
{
    public class KavariSupportAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "KavariSupport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "KavariSupport_default",
                "KavariSupport/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}