using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.DisableKaveri
{
    public class DisableKaveriAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DisableKaveri";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DisableKaveri_default",
                "DisableKaveri/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}