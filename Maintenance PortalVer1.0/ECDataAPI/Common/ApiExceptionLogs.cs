
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ECDataAPI.Common
{
    public static class ApiExceptionLogs
    {

        public static void LogError(Exception exception)
        {

            //Email developers, call fire department, log to database etc.
            #region Log Server Exceptions
            //To assist developer during testing phase.

            //    KaveriUILogPath

            string directoryPath = ConfigurationManager.AppSettings["KaveriApiExceptionLogPath"];

             //   string rootPath = Server.MapPath("/");
                // string directoryPath = rootPath + "/KUILog";

                directoryPath = System.IO.Path.Combine(directoryPath, DateTime.Now.Year.ToString(), DateTime.Now.Date.ToString("MMM"), DateTime.Now.Date.ToString("dd-MM-yyyy"));

                string filePath = directoryPath + "/Log " + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                System.IO.DirectoryInfo info = System.IO.Directory.CreateDirectory(directoryPath);

                using (System.IO.StreamWriter file = System.IO.File.AppendText(filePath))
                {
                    string format = "{0} : {1}";

                    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 100)));
                    file.WriteLine(string.Format(format, "Timestamp: ", DateTime.Now.ToString("hh:mm:ss tt")));
                    file.WriteLine(string.Format(format, "Message: ", exception.Message));
                    file.WriteLine(string.Format(format, "Stack Trace: ", exception.StackTrace));

                    Exception innerExp = GetInnerMostException(exception);
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
        

        }


        //Added By ShivamB on 13-02-2023 for logging exception with MethodName
        public static void LogError(Exception exception, string MethodName)
        {

            //Email developers, call fire department, log to database etc.
            #region Log Server Exceptions
            //To assist developer during testing phase.

            //    KaveriUILogPath

            string directoryPath = ConfigurationManager.AppSettings["KaveriApiExceptionLogPath"];

            //   string rootPath = Server.MapPath("/");
            // string directoryPath = rootPath + "/KUILog";

            directoryPath = System.IO.Path.Combine(directoryPath, DateTime.Now.Year.ToString(), DateTime.Now.Date.ToString("MMM"), DateTime.Now.Date.ToString("dd-MM-yyyy"));

            string filePath = directoryPath + "/Log " + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            System.IO.DirectoryInfo info = System.IO.Directory.CreateDirectory(directoryPath);

            using (System.IO.StreamWriter file = System.IO.File.AppendText(filePath))
            {
                string format = "{0} : {1}";

                file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 100)));
                file.WriteLine(string.Format(format, "MethodName : ", MethodName));
                file.WriteLine(string.Format(format, "Timestamp: ", DateTime.Now.ToString("hh:mm:ss tt")));
                file.WriteLine(string.Format(format, "Message: ", exception.Message));
                file.WriteLine(string.Format(format, "Stack Trace: ", exception.StackTrace));
                

                Exception innerExp = GetInnerMostException(exception);
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


        }
        //Added By ShivamB on 13-02-2023 for logging exception with MethodName








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