using System.Web.Mvc;

namespace ECDataUI.Areas.RefundChallan
{
    public class RefundChallanAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RefundChallan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RefundChallan_default",
                "RefundChallan/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}