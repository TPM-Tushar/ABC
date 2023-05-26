using CustomModels.Models.Common;
using ECDataUI.Common;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ECDataUI.Filters
{
   

    public class EventAuditLogFilter : ActionFilterAttribute
    {
        public string Description { get; set; }
        protected DateTime start_time;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            start_time = DateTime.Now;
        }


        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            HttpContext context = HttpContext.Current;
            try
            {
                RouteData route_data = filterContext.RouteData;
                TimeSpan duration = (DateTime.Now - start_time);
                string area = (string)route_data.Values["Areas"];
                string controller = (string)route_data.Values["controller"];
                string action = (string)route_data.Values["action"];

                DateTime created_at = DateTime.Now;
                Common.CommonFunctions commonFunc = new Common.CommonFunctions();

                EventAuditLoging objEventAuditLog = new EventAuditLoging();

                objEventAuditLog.UserID = KaveriSession.Current.UserID;
                objEventAuditLog.UrlAccessed = string.Concat(area, "/", controller, "/", action);
                objEventAuditLog.IPAddress = GetIPAddress();
                objEventAuditLog.Description = Description;
                // objEventAuditLog.AuditDataTime = commonFunc.GetDateTimeToString(DateTime.Now);

                objEventAuditLog.AuditDataTime = commonFunc.GetDateTimeToStringwithTimeStamp(DateTime.Now);
                //objEventAuditLog.ServiceID = KaveriSession.Current.ServiceID;

                //    ServiceCaller service = new ServiceCaller("CommonFunctionsApi", new HttpClient(new LoggingHandler(new HttpClientHandler())));

                ServiceCaller service = new ServiceCaller("CommonsApiController");

                string errorMessage = string.Empty;
                var result = service.PostCall<EventAuditLoging, bool>("InsertAuditMvcEventLoging", objEventAuditLog, out errorMessage);//call to serive

                if (!string.IsNullOrEmpty(errorMessage))
                {



                    //*********** Exception handling *********

                    //errorMessage = "Exception occured while logging Audit Event";
                    //EventAuditLogException eventAuditException = new EventAuditLogException(errorMessage);

                    //Exception exception = eventAuditException;
                    //BaseError baseError = null;
                    //CustomHandleErrorAttribute customError = new CustomHandleErrorAttribute();
                    //customError.LogException(exception, baseError);


                    //context.Response.Redirect("/Login/Error", true);
                }
            }
            catch
            {
                //*********** Exception handling *********

                //EventAuditLogException eventAuditException = new EventAuditLogException("Exception occured while logging Audit Event");
                //Exception exception = eventAuditException;
                //BaseError baseError = null;
                //CustomHandleErrorAttribute customError = new CustomHandleErrorAttribute();
                //customError.LogException(exception, baseError);
            }


        }

        public string GetIPAddress()
        {
            //return IPAddress;

            string IPAddress = null;
            if (HttpContext.Current != null)
            { // ASP.NET
                IPAddress = string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"])
                    ? HttpContext.Current.Request.UserHostAddress
                    : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            if (string.IsNullOrEmpty(IPAddress) || IPAddress.Trim() == "::1")
            { // still can't decide or is LAN
                IPHostEntry Host = default(IPHostEntry);
                string Hostname = null;
                Hostname = System.Environment.MachineName;
                Host = Dns.GetHostEntry(Hostname);
                foreach (IPAddress IP in Host.AddressList)
                {
                    if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        IPAddress = Convert.ToString(IP);
                    }
                }
            }
            return IPAddress;

        }
    }
}