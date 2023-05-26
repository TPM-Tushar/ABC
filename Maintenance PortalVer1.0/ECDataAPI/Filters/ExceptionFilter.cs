using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;  
using System.Net.Http;
using System.Web.Http.Filters;
using ECDataAPI.Common;

namespace ECDataAPI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExceptionFilter: ExceptionFilterAttribute
    {
            public override void OnException(HttpActionExecutedContext actionExecutedContext)
            {
                string exceptionMessage = string.Empty;
                if (actionExecutedContext.Exception.InnerException == null)
                {
                    exceptionMessage = actionExecutedContext.Exception.Message;
                }
                else
                {
                    exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
                }
            //We can log this exception message to the file or database.  


            ApiExceptionLogs.LogError(actionExecutedContext.Exception);



            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An unhandled exception was thrown by service."),  
                        ReasonPhrase = "Internal Server Error. Please Contact Administrator."
                };
                actionExecutedContext.Response = response;
            }
  }
}