using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ECDataAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
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

            string directoryPath = ConfigurationManager.AppSettings["KaveriApiExceptionLogPath"];

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

                Response.Redirect("/Login/Error");

            }
            catch (Exception)
            {
                // You've handled the error, so clear it. Leaving the server in an error state can cause unintended side effects as the server continues its attempts to handle the error.
                System.Web.HttpContext.Current.Server.ClearError();
                // Possible that a partially rendered page has already been written to response buffer before encountering error, so clear it.
                System.Web.HttpContext.Current.Response.Clear();
                // Finally redirect, transfer, or render a error view
                System.Web.HttpContext.Current.Response.Redirect("~/Login/Error");
            }
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

    }
}
