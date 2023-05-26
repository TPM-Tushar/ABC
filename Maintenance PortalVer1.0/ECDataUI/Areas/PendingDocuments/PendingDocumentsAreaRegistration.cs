using System.Web.Mvc;

namespace ECDataUI.Areas.PendingDocuments
{
    public class PendingDocumentsreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get 
            {
                return "PendingDocuments";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PendingDocuments",
                "PendingDocuments/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}