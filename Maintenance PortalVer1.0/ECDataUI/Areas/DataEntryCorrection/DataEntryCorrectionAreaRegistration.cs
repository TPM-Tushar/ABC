using System.Web.Mvc;

namespace ECDataUI.Areas.DataEntryCorrection
{
    public class DataEntryCorrectionAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DataEntryCorrection";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DataEntryCorrection_default",
                "DataEntryCorrection/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}