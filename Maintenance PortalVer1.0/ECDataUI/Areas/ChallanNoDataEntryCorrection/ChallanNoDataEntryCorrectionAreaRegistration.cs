using System.Web.Mvc;

namespace ECDataUI.Areas.ChallanNoDataEntryCorrection
{
    public class ChallanNoDataEntryCorrectionAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ChallanNoDataEntryCorrection";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ChallanNoDataEntryCorrection_default",
                "ChallanNoDataEntryCorrection/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}