using System.Web.Mvc;

namespace ECDataUI.Areas.IntegrationDashboard
{
    public class IntegrationDashboardAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "IntegrationDashboard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "IntegrationDashboard_default",
                "IntegrationDashboard/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}