using System.Web.Mvc;

namespace ECDataUI.Areas.KaveriSupport
{
    public class KaveriSupportAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "KaveriSupport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "KaveriSupport_default",
                "KaveriSupport/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}