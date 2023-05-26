using System.Web.Mvc;

namespace ECDataAPI.Areas.XELFiles
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
                "DataEntryCorrection",
                "DataEntryCorrection/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}