using System.Web.Mvc;

namespace ECDataUI.Areas.LogAnalysis
{
    public class LogAnalysisAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LogAnalysis";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LogAnalysis_default",
                "LogAnalysis/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}