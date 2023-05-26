using System.Web.Mvc;

namespace ECDataAPI.Areas.PhotoThumb
{
    public class PhotoThumbRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PhotoThumb";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PhotoThumb",
                "PhotoThumb/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}