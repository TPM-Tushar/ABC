using System.Web.Mvc;

namespace ECDataAPI.Areas.KaveriIntegration
{
    public class KaveriIntegrationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "KaveriIntegration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "KaveriIntegration_default",
                "KaveriIntegration/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}