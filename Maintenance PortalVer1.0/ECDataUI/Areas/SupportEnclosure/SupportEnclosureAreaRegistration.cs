using System.Web.Mvc;

namespace ECDataUI.Areas.SupportEnclosure
{
    public class SupportEnclosureAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SupportEnclosure";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SupportEnclosure_default",
                "SupportEnclosure/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}