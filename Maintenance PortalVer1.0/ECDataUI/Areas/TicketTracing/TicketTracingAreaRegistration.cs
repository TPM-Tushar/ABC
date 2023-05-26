using System.Web.Mvc;

namespace ECDataUI.Areas.TicketTracing
{
    public class TicketTracingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TicketTracing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TicketTracing_default",
                "TicketTracing/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}