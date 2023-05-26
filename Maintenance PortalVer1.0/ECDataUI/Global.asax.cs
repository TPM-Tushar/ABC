using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace ECDataUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            if (null != HttpContext.Current)
            {
                HttpContext.Current.Response.Headers.Remove("X-Powered-By");
                HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
                HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");
                HttpContext.Current.Response.Headers.Remove("Server");
                //HttpContext.Current.Response.Headers.Remove("X-Frame-Options");
                HttpContext.Current.Response.Headers.Remove("X-SourceFiles");

                 
                HttpContext.Current.Response.AddHeader("x-frame-options", "SAMEORIGIN");
                Response.Headers.Add("Set-Cookie", "Domain=" + Request.Url.Host);
                Response.Headers.Add("Set-Cookie", "SameSite=strict");
                MvcHandler.DisableMvcResponseHeader = true;
            }
        }
        protected void Session_End(Object sender, EventArgs E)
        {
             
            Session.Abandon();
             
            Response.Cookies.Clear();
            Response.Cookies["KaveriSessionID"].Expires = DateTime.Now.AddYears(-30);
         
        }


        protected void Application_Error()
        {
            Exception unhandledException = Server.GetLastError();
            HttpException httpException = unhandledException as HttpException;
            Exception innerException = unhandledException.InnerException;
            //String strURL, strIP;
            //String strErrorCode = "0";
            //String strBrowser;

            #region Log Server Exceptions
            //To assist developer during testing phase.

        //    KaveriUILogPath
            
            string directoryPath = ConfigurationManager.AppSettings["KaveriUILogPath"];

            string rootPath = Server.MapPath("/");
           // string directoryPath = rootPath + "/KUILog";

            directoryPath = System.IO.Path.Combine(directoryPath, DateTime.Now.Year.ToString(), DateTime.Now.Date.ToString("MMM"), DateTime.Now.Date.ToString("dd-MM-yyyy"));

            string filePath = directoryPath + "/Log " + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            System.IO.DirectoryInfo info = System.IO.Directory.CreateDirectory(directoryPath);

            using (System.IO.StreamWriter file = System.IO.File.AppendText(filePath))
            {
                string format = "{0} : {1}";

                file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 100)));
                file.WriteLine(string.Format(format, "Timestamp: ", DateTime.Now.ToString("hh:mm:ss tt")));
                file.WriteLine(string.Format(format, "Message: ", unhandledException.Message));
                file.WriteLine(string.Format(format, "Stack Trace: ", unhandledException.StackTrace));

                Exception innerExp = GetInnerMostException(unhandledException);
                if (innerExp != null)
                {
                    file.WriteLine("--------Inner Exception--------");
                    file.WriteLine();
                    file.WriteLine(string.Format(format, "Message: ", innerExp.Message));
                    file.WriteLine(string.Format(format, "Stack Trace: ", innerExp.StackTrace));
                    file.WriteLine();
                    innerExp = innerExp.InnerException;
                }

                file.Flush();
            }

            #endregion
 
            try
            {

                // HttpContext ctx = HttpContext.Current;
                // ctx.Response.Clear();
                // RequestContext rc = ((MvcHandler)ctx.CurrentHandler).RequestContext;
                // rc.RouteData.Values["action"] = "Index";

                // rc.RouteData.Values["controller"] = "Error";
                //// rc.RouteData.Values["id"] = ErrorMessage;

                // IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
                // IController controller = factory.CreateController(rc, "HndlError");
                // controller.Execute(rc);
                // ctx.Server.ClearError();

                //Response.Redirect("/Error/Index");
                Response.RedirectPermanent("/Error/Index");
 
            }
            catch (Exception)
            { 
                // You've handled the error, so clear it. Leaving the server in an error state can cause unintended side effects as the server continues its attempts to handle the error.
                System.Web.HttpContext.Current.Server.ClearError();
                // Possible that a partially rendered page has already been written to response buffer before encountering error, so clear it.
                System.Web.HttpContext.Current.Response.Clear();
                // Finally redirect, transfer, or render a error view
                System.Web.HttpContext.Current.Response.Redirect("~/Error/Index");
            }
        }


        protected void Application_BeginRequest(Object sender, EventArgs E)
        {
            Context.Items["loadstarttime"] = DateTime.Now;
            Context.Items["RenderPageTime"] = string.Empty;
            // Stop Caching in IE
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            // Stop Caching in Firefox
            Response.Cache.SetNoStore();

        }
        protected void Application_EndRequest(Object sender, EventArgs E)
        {
            try
            {
                DateTime end = (DateTime)Context.Items["loadstarttime"];
                TimeSpan loadTime = DateTime.Now - end;
                //Application.Add("RenderPageTime", loadTime);
                Context.Items["RenderPageTime"] = loadTime;

            }
            catch (Exception)
            {
                throw ;
            }
          
        }

        protected void Application_PostRequestHandlerExecute(Object sender, EventArgs E)
        {
            Context.Items["RenderPageTime"] = "a";
             

        }
        public static Exception GetInnerMostException(Exception ex)
        {
            Exception currentEx = ex;
            while (currentEx.InnerException != null)
            {
                currentEx = currentEx.InnerException;
            }
            return currentEx;
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null && !authTicket.Expired)
                {
                    var roles = authTicket.UserData.Split(',');
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), roles);
                }
            }
        }

    }
}
