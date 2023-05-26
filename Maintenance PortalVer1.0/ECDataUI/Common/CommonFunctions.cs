using CustomModels.Models.Common;
using CustomModels.Models.UserManagement;
using ECDataUI.Session;
using System;
using System.IO;

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Globalization;
using System.Security.Cryptography;
using CustomModels.Models.ControllerAction;
using System.Web.Routing;
using FileTypeDetective;
using iTextSharp.text.pdf;
using iTextSharp.text;
using ECDataUI.Entity;
using System.Xml.Linq;
using System.Xml;

namespace ECDataUI.Common
{
    public class CommonFunctions
    {

        static string applicationURL = ConfigurationManager.AppSettings["ApplicationURL"];


        /// <summary>
        /// This method prepares the email body for Activation email & returns 'EmailModel'
        /// </summary>
        /// <param name="activationModel"></param>
        /// <returns></returns>
        public static EmailModel PrepareActivationEmail(UserActivationModel activationModel)
        {


            StringBuilder msgBody = new StringBuilder();


            msgBody.Append("<html><body> Dear Applicant, <br/>");

            msgBody.Append("<table>");
            //msgBody.Append("<tr><td>Activation Code:" + resendUserActivationModel.ActivationCode + "</td></tr>");
            msgBody.Append("<tr><td>Login Name:" + activationModel.Email + "</td></tr>");
            msgBody.Append("<tr><td>Please click on following link to activate the account.</td></tr>");
            msgBody.Append("<tr><td>" + applicationURL + "/Account/AccountActivationByLink?Id=" + activationModel.EncryptedId + "</td></tr></table></head></html>");


            EmailModel emailModel = new EmailModel();

            //  emailModel.EmailDate =DateTime.Now;

            //   DateTime date = Convert.ToDateTime(item.Date_Of_Birth); //To convert Nullable DateTime into String
            emailModel.EmailDate = String.Format("{0:dd/MM/yyyy}", DateTime.Now);//To arrange a Date in Specific Format.


            emailModel.EmailSubject = "Registration Account Activation Link for Kaveri";

            emailModel.RecepientAddress = activationModel.Email;
            emailModel.EmailContent = msgBody.ToString();


            return emailModel;
        }


        /// <summary>
        /// This method prepares the email body for Forgot password email & returns 'EmailModel'
        /// </summary>
        /// <param name="activationModel"></param>
        /// <returns></returns>
        public static EmailModel PrepareForgotPasswordEmail(UserActivationModel activationModel)
        {
           
            StringBuilder msgBody = new StringBuilder();
            msgBody.Append("<html><body> Dear Applicant, <br/>");

            msgBody.Append("<table>");
            msgBody.Append("<tr><td>Login Name:" + activationModel.Email + "</td></tr>");
            msgBody.Append("<tr><td>Please click on following link to change your password.</td></tr>");
            msgBody.Append("<tr><td>" + applicationURL + "/Account/ForgotPasswordByLink?Id=" + activationModel.EncryptedId + "</td></tr></table></head></html>");


            EmailModel emailModel = new EmailModel();

            //  emailModel.EmailDate =DateTime.Now;

            //   DateTime date = Convert.ToDateTime(item.Date_Of_Birth); //To convert Nullable DateTime into String
            emailModel.EmailDate = String.Format("{0:dd/MM/yyyy}", DateTime.Now);//To arrange a Date in Specific Format.


            emailModel.EmailSubject = "Forgot password Link for Kaveri";

            emailModel.RecepientAddress = activationModel.Email;
            emailModel.EmailContent = msgBody.ToString();


            return emailModel;


        }



        public void InitializeSession(LoginResponseModel model)
        {
            KaveriSession.Current.UserID = model.UserID;
            //KaveriSession.Current.FirmID = 1;//need to change
            KaveriSession.Current.OfficeID = model.OfficeId;
            KaveriSession.Current.RoleID = model.DefaultRoleId;

            KaveriSession.Current.ResourceID = model.ResourceID;
            KaveriSession.Current.ServiceID = 6;//need to change.
            KaveriSession.Current.FullName= model.UserFullName;
            //KaveriSession.Current.LevelId = model.LevelId;
            //uncommented by mayank on 06/08/2021
            KaveriSession.Current.LevelID = model.LevelId;
            KaveriSession.Current.SessionID = HttpContext.Current.Session.SessionID;
         //   KaveriSession.Current.IsSessionExpiredStatus = false;


        }



        public List<SelectListItem> GetEmptyDropDownWithSelectListItem(string sTextValue, string sOptionValue)
        {
            return new List<SelectListItem>() {     
                            new SelectListItem
                                {
                                    Text = sTextValue,
                                    Value = sOptionValue,
                                    Selected=true
                                }};
        }


        public SelectListItem GetDefaultSelectListItem(string sTextValue, string sOptionValue)
        {
            return new SelectListItem
            {
                Text = sTextValue,
                Value = sOptionValue,
            };
        }

        public void RegenerateSessionId()
        {
            try
            {
                System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
                string oldId = manager.GetSessionID(System.Web.HttpContext.Current);
                string newId = manager.CreateSessionID(System.Web.HttpContext.Current);
                bool isAdd = false, isRedir = false;
                manager.SaveSessionID(System.Web.HttpContext.Current, newId, out isRedir, out isAdd);
                HttpApplication ctx = (HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
                HttpModuleCollection mods = ctx.Modules;
                System.Web.SessionState.SessionStateModule ssm = (System.Web.SessionState.SessionStateModule)mods.Get("Session");
                System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                SessionStateStoreProviderBase store = null;
                System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    if (field.Name.Equals("_store")) store = (SessionStateStoreProviderBase)field.GetValue(ssm);
                    if (field.Name.Equals("_rqId")) rqIdField = field;
                    if (field.Name.Equals("_rqLockId")) rqLockIdField = field;
                    if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;
                }
                object lockId = rqLockIdField.GetValue(ssm);
                if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(System.Web.HttpContext.Current, oldId, lockId);
                rqStateNotFoundField.SetValue(ssm, true);
                rqIdField.SetValue(ssm, newId);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public static bool IsValidInputString(string inputString, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool result = false;
            try
            {
                Regex RgxUrl = new Regex(@"^([^<>])([^-\s][a-zA-Z0-9_\s-<>])([a-zA-Z\u0C80-\u0CFF 0-9-/()]+)$");
                result = RgxUrl.IsMatch(inputString);
                if (result == false)
                {
                    errorMessage = "Input string is invalid.";
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
            return result;
        }

        /// <summary>
        /// Get Calendar / Financial years 
        /// </summary>
        /// <param name="IsFinancialYear"></param>
        /// <returns></returns>
        public List<SelectListItem> GetYearDropDown(bool IsFinancialYear = false)
        {
            try
            {
                //Financial Year Drop down for Will Deed
                var startYear = 2000;
                var lstYears = Enumerable.Range(startYear, DateTime.Now.Year - startYear + 1);
                List<SelectListItem> lstFinYear = new List<SelectListItem>();
                if (IsFinancialYear)
                {
                    foreach (var year in lstYears)
                    {
                        lstFinYear.Add(new SelectListItem { Value = year.ToString(), Text = (year + "-" + (year + 1)) });
                    }
                }
                else
                {
                    foreach (var year in lstYears)
                    {
                        lstFinYear.Add(new SelectListItem { Value = year.ToString(), Text = (year.ToString()) });
                    }
                }
                lstFinYear = lstFinYear.OrderByDescending(m => m.Value).ToList();
                lstFinYear.Insert(0, GetDefaultSelectListItem("Select Year", "0"));
                return lstFinYear;
            }
            catch (Exception )
            {

                throw ;
            }
        }


        public List<SelectListItem> GetMonthDropDown()
        {

            // ********************* Note *************
            //value 0 == 'Select Month'
            //      1 == 'All',
            //    2-13== 'Jan'To 'Dec'

            try
            {
                List<SelectListItem> listOfMonths = new List<SelectListItem>();
                int j = 1;
                foreach (var item in (CultureInfo.CurrentUICulture.DateTimeFormat.MonthNames))
                {
                    if (item != string.Empty)
                        listOfMonths.Add(new SelectListItem { Value = (++j).ToString(), Text = item, Selected = false });
                }
                listOfMonths.Insert(0, GetDefaultSelectListItem("Select Month", "0"));
                listOfMonths.Insert(1, GetDefaultSelectListItem("All", "1"));

                return listOfMonths;
            }
            catch (Exception )
            {

                throw ;
            }
        }

        /// <summary>
        /// CheckUserPermissionForRole
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="area"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool CheckUserPermissionForRole(short roleID, string area, string controller, string action)
            {

            try
            {

                var requestUri = ConfigurationManager.AppSettings["BaseURI"] + "api/AuthorizationApi/CheckUserPermissionForRole?roleID=" +
                    roleID + "&area=" + area + "&controller=" + controller + "&action=" + action;

                HttpResponseMessage serviceResponse = Task<HttpResponseMessage>.Run(async () => { return await new HttpClient().GetAsync(requestUri); }).Result;


                var result = false;

                //Access the status code and return 
                switch (serviceResponse.StatusCode)
                {
                    case HttpStatusCode.OK:

                        result = serviceResponse.Content.ReadAsAsync<bool>().Result;
                        break;
                    default:
                        return false;
                }
                return result;
            }
            catch (Exception )
            {

                throw ;
            }
        }


        /// <summary>
        /// Convert string datetime to DateTime object
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static DateTime? GetStringToDateTime(string strDate)
        {
            if (string.IsNullOrEmpty(strDate))
                return null;

            string[] formats = { "dd/MM/yyyy" };

            DateTime newDate;



            //bool boolDate = DateTime.TryParseExact(strDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out newDate);
            //if (boolDate)
            //{
            //    return newDate;
            //}
            //else
            //{

            //    throw new Exception("invalid Date. Error in Parsing");
            //}


            bool boolDate = DateTime.TryParse(DateTime.ParseExact(strDate, formats[0], null).ToString(), out newDate);
            if (boolDate)
            {
                return newDate;
            }
            else
            {

                throw new Exception("Invalid Date. Error in Parsing");
            }

            //return DateTime.ParseExact(strDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }


        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
             
        public String GetDateTimeToStringwithTimeStamp(DateTime date)
        {
            // return date.ToString("dd/MM/yyyy");
            return date.ToString("dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get IP Address
        /// </summary>
        /// <returns></returns>
        public static string GetUserIP()
        {
            string visitorsIPAddr = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                visitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                visitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
            }

            return visitorsIPAddr.Equals("::1") ? "127.0.0.1" : visitorsIPAddr;
        }
        public static void WriteErrorLog(string sText)
        {
            string directoryPath = ConfigurationManager.AppSettings["KaveriUILogPath"];
            directoryPath = directoryPath + "\\DeployedLog\\";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            directoryPath = directoryPath + "/Log " + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            using (System.IO.StreamWriter file = System.IO.File.AppendText(directoryPath))
            {
                string format = "{0} : {1}";

                file.WriteLine(string.Format(format, "Timestamp: ", DateTime.Now.ToString("hh:mm:ss tt")));

                file.WriteLine("text:" + sText);
                file.Flush();
            }


        }
        
        public static string GetHashOfDocument(byte[] FileBytes)
        {
            StringBuilder sBuilder = new StringBuilder();

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] HashedBytes = sha256Hash.ComputeHash(FileBytes);
                for (int i = 0; i < HashedBytes.Length; i++)
                {
                    sBuilder.Append(HashedBytes[i].ToString("x2"));
                }
            }
            return sBuilder.ToString();
        }
        public static void WriteTofile(string text)
        {
            string sDir = @"C:\ApiErr\";
            if (!Directory.Exists(sDir))
                Directory.CreateDirectory(sDir);

            using (System.IO.StreamWriter sw = System.IO.File.AppendText(sDir + "ApilandingPageLog.txt"))
            //using (StreamWriter sw = new StreamWriter(sDir + fileName))
            {
                sw.WriteLine("Date:" + DateTime.Now.ToString());
                sw.WriteLine(string.IsNullOrEmpty(text) ? "Parameter null." : text);
                //sw.Close();
            }
        }
               
        public static bool IsDateGreaterThanCurrentDate(DateTime date)
        {
            if (date.Date > DateTime.Now.Date)
            {
                return true;
            }
            else {
                return false;
            }
        }
        
        public static bool IsDateLessThanSpecificDate(DateTime date ,DateTime dateToCompare)
        {
            if (date.Date < dateToCompare.Date)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public DateTime ConvertStringToDateTime(string strDate)
        {
            string[] formats = { "dd/MM/yyyy" };
            DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo();
            dateTimeFormatInfo.ShortDatePattern = "dd/MM/yyyy";
            dateTimeFormatInfo.LongDatePattern = "dd/MM/yyyy hh:mm:ss";
            CultureInfo usaCulture = new CultureInfo("en-US");
            usaCulture.DateTimeFormat = dateTimeFormatInfo;
            return DateTime.ParseExact(strDate.Trim(), formats, usaCulture, DateTimeStyles.None);
        }
        
        public String ConvertDateTimeToString(DateTime date)
        {
            // return date.ToString("dd/MM/yyyy");
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///  Convert Datetime object to string datetime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public String ConvertDateTimeToStringwithTimeStamp(DateTime date)
        {
            // return date.ToString("dd/MM/yyyy");
            return date.ToString("dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
        }

        //added by amit 13-10-18 for getting all areas from project 

        public static IEnumerable<KaveriArea> GetAllAreas()
        {
            //for getting the all subclasses of the project
            var list = GetSubClasses<Controller>();

            // Get all controllers with their actions all actions are resides in it
            var getAllcontrollers = (from item in list
                                     let name = item.Name
                                     where !item.Name.StartsWith("T4MVC_")
                                     select new KaveriController()
                                     {
                                         Name = name.Replace("Controller", ""),
                                         Namespace = item.Namespace,
                                         MyActions = GetListOfAction(item)
                                     }).ToList();

            // Now we will get all areas that has been registered in route collection
            var getAllAreas = RouteTable.Routes.OfType<Route>()
                .Where(d => d.DataTokens != null && d.DataTokens.ContainsKey("area"))
                .Select(
                    r =>
                        new KaveriArea
                        {
                            Name = r.DataTokens["area"].ToString(),
                            Namespace = r.DataTokens["Namespaces"] as IList<string>,
                        }).ToList()
                .Distinct().OrderBy(c => c.Name).  ToList();
            getAllAreas.Insert(0, new KaveriArea()
            {
                Name = "NoArea",
                Namespace = new List<string>()
            {
                "ECDataUI.Controllers"
            }
            });
            String nameSpace = string.Empty;
            foreach (var area in getAllAreas)
            {
                var temp = new List<KaveriController>();
                foreach (var item in area.Namespace)
                {
                    if (item != "ECDataUI.Controllers")
                    {
                        string[] AreaNameSpace = item.Split('.');
                        nameSpace = "" + AreaNameSpace[0] + "." + AreaNameSpace[1] + "." + AreaNameSpace[2] + "." + "Controllers";
                    }
                    else
                    {
                        nameSpace = item;
                    }
                    List<KaveriController> list2 = getAllcontrollers.Where(x => x.Namespace == nameSpace).OrderBy(c => c.Name).ToList();
                    temp.AddRange(getAllcontrollers.Where(x => x.Namespace == nameSpace).OrderBy(c => c.Name).ToList());
                }
                area.KaveriControllers = temp;

            }

            return getAllAreas;
        }

        public static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }
        public static List<KaveriAction> GetListOfAction(Type controller)
        {
            var navItems = new List<KaveriAction>();

            // Get a descriptor of this controller
            ReflectedControllerDescriptor controllerDesc = new ReflectedControllerDescriptor(controller);

            // Look at each action in the controller
            foreach (ActionDescriptor action in controllerDesc.GetCanonicalActions().OrderBy(c => c.ActionName))
            {
                bool validAction = true;
                bool isHttpPost = false;
                bool isHttpGet = false;
                bool isHttpOther = false;
                // Get any attributes (filters) on the action
                object[] attributes = action.GetCustomAttributes(false);

                // Look at each attribute
                foreach (object filter in attributes)
                {
                    // Can we navigate to the action?
                    if (filter is ChildActionOnlyAttribute)
                    {
                        validAction = false;
                        break;
                    }
                    if (filter is HttpPostAttribute)
                    {
                        isHttpPost = true;
                    }
                    if (filter is HttpGetAttribute)
                    {
                        isHttpGet = true;
                    }
                    if (filter is HttpPutAttribute)
                    {
                        isHttpOther = true;
                    }
                    if (filter is HttpDeleteAttribute)
                    {
                        isHttpOther = true;
                    }
                    if (filter is HttpHeadAttribute)
                    {
                        isHttpOther = true;
                    }
                    if (filter is HttpPatchAttribute)
                    {
                        isHttpOther = true;
                    }
                    if (filter is HttpOptionsAttribute)
                    {
                        isHttpOther = true;
                    }

                }

                // Add the action to the list if it's "valid"
                if (validAction)
                    navItems.Add(new KaveriAction()
                    {
                        Name = action.ActionName,
                        IsHttpPost = isHttpPost,
                        IsHttpGet = isHttpGet,
                        IsHttpOther = isHttpOther
                    });
            }
            return navItems;
        }
        
        public string GetSHA1HashData(string data)
        {
            //create new instance of md5
            SHA1 sha1 = SHA1.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }

        public bool ValidateIsPdf(string basePath, HttpPostedFileBase tempFile)
        {
            try
            {
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                //tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));
                //FileInfo fileinfo = new FileInfo(Path.Combine(basePath, tempFile.FileName));


                string FileName = new FileInfo(tempFile.FileName).Name;
                tempFile.SaveAs(Path.Combine(basePath, FileName));
                FileInfo fileinfo = new FileInfo(Path.Combine(basePath, FileName));

                //fileinfo.Name
                // returns true if the file is PDF
                if (fileinfo.IsPdf())
                {
                    System.IO.File.Delete(Path.Combine(basePath, FileName));
                    return true;
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(basePath, FileName));
                    return false;
                }
            }
            catch (Exception )
            {
                return false;
            }
        }

        //Added by Raman 28-03-2019
        public void DeleteFileFromTemporaryFolder(string filePath)
        {
            //Try to delete file from server temparary folder
            //resume next if already used by another process/User.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            finally
            {
            }
        }

        #region 3-4-2019 For Table LOG by SB
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
        #endregion

        //Raman Kalegaonkar on 27-05-2019
        public iTextSharp.text.Font DefineNormaFont(string strFontName, float Size)
        {
            iTextSharp.text.Font fontNormal = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont(strFontName, Size, iTextSharp.text.Font.NORMAL));
            return fontNormal;
        }

        public byte[] AddpageNumber(byte[] inputArray)
        {
            byte[] pdfBytes = null;
            CommonFunctions objCommon = new CommonFunctions();
            iTextSharp.text.Font fntrow = objCommon.DefineNormaFont("Times New Roman", 12);

            using (MemoryStream stream = new MemoryStream())
            {

                PdfReader reader = new PdfReader(inputArray);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_MIDDLE, new Phrase("Page " + i.ToString() + " of " + pages, fntrow), 420f, 16f, 0);
                    }
                }
                pdfBytes = stream.ToArray();
            }

            return pdfBytes;

        }

        // ADDED BY SHUBHAM BHAGAT TO WRITE DEBUG LOG IN DATABASE ON 1-07-2019
        public static void WriteDebugLogInDB(int functionalityID, String logDesc, String className, String methodName)
        {
            ECDATA_DOCS_UIEntities ecDATA_DOCS_Entities = null;
            try
            {
                ecDATA_DOCS_Entities = new ECDATA_DOCS_UIEntities();
                bool result = ecDATA_DOCS_Entities.ECLOG_FUNCTIONALITY_MASTER.Where(x => x.FUNCTIONALITY_ID == functionalityID && x.STATUS == true).Any();
                if (result)
                {
                    ECLOG_FUNCTIONALITY_DEBUG_LOG obj = new ECLOG_FUNCTIONALITY_DEBUG_LOG();
                    obj.FUNCTIONALITY_ID = functionalityID;
                    obj.LOG_DESCRIPTION = logDesc;
                    obj.INSERT_DATETIME = DateTime.Now;
                    obj.CLASSNAME = className.Length > 499 ? className.Substring(0, 498) : className;
                    obj.METHODNAME = methodName.Length > 499 ? methodName.Substring(0, 498) : methodName;
                    ecDATA_DOCS_Entities.ECLOG_FUNCTIONALITY_DEBUG_LOG.Add(obj);
                    ecDATA_DOCS_Entities.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (ecDATA_DOCS_Entities != null)
                    ecDATA_DOCS_Entities.Dispose();
            }
        }
        //Added By RamanK for SAKALA Upload Details
        //public string PrettyXml(string xml)
        //{
        //    var stringBuilder = new StringBuilder();

        //    var element = XElement.Parse(xml);

        //    var settings = new XmlWriterSettings();
        //    settings.OmitXmlDeclaration = true;
        //    settings.Indent = true;
        //    settings.NewLineOnAttributes = true;

        //    using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
        //    {
        //        element.Save(xmlWriter);
        //    }

        //    return stringBuilder.ToString();
        //}

        public static string PrettyXml(string xml)
        {

            try
            {
                new XmlDocument().LoadXml(xml);//xml
                var stringBuilder = new StringBuilder();

                var element = XElement.Parse(xml);

                var settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineOnAttributes = true;

                using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    element.Save(xmlWriter);
                }

                return stringBuilder.ToString();
            }
            catch
            {
                return xml;
            }

        }
		
		//Added by mayank on 16/07/2020
        public static bool ValidateId(string id)
        {
            string reg = "^([0-9])*$";

            return Regex.Match(id, reg).Success;
        }


        public bool CheckForCompressionIssue(string srcFile, ref string errorMessage)
        {
            #region Variable Declarations
            Boolean result = true;
            System.Drawing.Bitmap bm = null;
            #endregion 

            //Added by Raman Kalegaonkar on 27-10-2020 for testing purpose

            //double count=GetStdDev(srcFile);

            #region Code to Detetct Backout Images in Tiff File Added by Raman Kalegaonkar on 19-08-2020
            try
            {
                bm = new System.Drawing.Bitmap(srcFile);

                int total = bm.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);

                for (int k = 0; k < total; ++k)
                {
                    //IF Tiff Image is tampered throws GDI+ Exception on selection of Active Frame
                    bm.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, k);

                }
                result = false;
            }
            catch (Exception EX)
            {
                return result;
            }
            #endregion
            return result;
        }
    }
}