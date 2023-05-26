using CustomModels.Models.Common;
using ECDataAPI.BAL;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using ECDataAPI.Interface;
using ECDataAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace ECDataAPI.Filters
{


    public class EventApiAuditLogFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        ClientAuthenticationModel model = null;
        public string Description { get; set; }
        public short ServiceID { get; set; }



        protected DateTime start_time;
        public EventApiAuditLogFilter()
        {

        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            start_time = DateTime.Now;
        }


        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            ApiCommonFunctions commonFunctions = new ApiCommonFunctions();
            KaveriEntities dbContext = null;

            HttpContext context = HttpContext.Current;
           // akash need to change
            //var value= actionExecutedContext.Request.Properties["ServiceID"];
            try
            {
                dbContext = new KaveriEntities();

                TimeSpan duration = (DateTime.Now - start_time);
                //string area = ;
                string AuthenticationToken = actionExecutedContext.ActionContext.Request.Headers.Authorization.Parameter;

                string DecodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(AuthenticationToken));
                string[] UserNamePasswordArr = DecodedAuthenticationToken.Split(':');
               

                    model = new ClientAuthenticationModel()
                    {
                        ApiConsumerUserName = UserNamePasswordArr[0],
                        ApiConsumerPassword = UserNamePasswordArr[1]
                    };

                string controller = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
                string action = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;



                string username = model.ApiConsumerUserName;

              

                long ApiConsumerID= commonFunctions.GetApiConsumerID(model);


                if (ApiConsumerID == (long)ApiCommonEnum.APiConsumers.CdacKaveriMVCClient)
                {

            
                DateTime created_at = DateTime.Now;
                EventAuditLoging objEventAuditLog = new EventAuditLoging();

                objEventAuditLog.UserID = ApiConsumerID;// user.ApiConsumerID;
                objEventAuditLog.UrlAccessed = string.Concat("api", "/", controller, "/", action);
                objEventAuditLog.IPAddress = GetIPAddress();
                objEventAuditLog.Description = Description;
                objEventAuditLog.AuditDataTime = commonFunctions.GetDateTimeToStringwithTimeStamp(DateTime.Now);
              //  objEventAuditLog.ServiceID = ServiceID;

                commonFunctions.InsertApiEventAuditLog(objEventAuditLog);
                }
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public string GetIPAddress()
        {

            string IPAddress = null;
            if (HttpContext.Current != null)
            { // ASP.NET
                IPAddress = string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"])
                    ? HttpContext.Current.Request.UserHostAddress
                    : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            if (string.IsNullOrEmpty(IPAddress) || IPAddress.Trim() == "::1")
            { 
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