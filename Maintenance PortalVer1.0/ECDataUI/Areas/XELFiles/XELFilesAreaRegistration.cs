using System.Web.Mvc;

namespace ECDataUI.Areas.XELFiles
{
    public class XELFilesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "XELFiles";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "XELFiles_default",
                "XELFiles/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}