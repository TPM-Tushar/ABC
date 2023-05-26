using CustomModels.Models.Alerts;
using CustomModels.Models.DisableKaveri;
using CustomModels.Models.HomePage;
using CustomModels.Models.MenuHelper;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ECDataUI.Controllers
{

    public class HomeController : Controller
    {


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            try
            {
                short office = KaveriSession.Current.OfficeID;
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //   [KaveriAuthorizationAttribute]
        // On 9-4-2019 by Shubham Bhagat for password change on first login
        //public ActionResult HomePage(bool IsFirstLogin = true)
        public ActionResult HomePage()
        {

            //added by Akash on 19-07-2018
            var urlReferer = Request.UrlReferrer;
            if (urlReferer == null)
                return RedirectToAction("SessionExpire", "Error");
            if (KaveriSession.Current.UserID == 0)
            {
                Session.Abandon();
                KaveriSession.Current.EndSession();
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //added by soujanya 05-10-2017 and commented cookies.add
                Response.Cookies.Clear();
                Response.Cookies["KaveriSessionID"].Expires = DateTime.Now.AddYears(-30);
                //Response.Cookies.Add(new HttpCookie("KaveriSessionID", ""));
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                return Redirect("/Error/SessionExpire");
            }
            else
            {

                //if (KaveriSession.Current.RoleID == (short)CommonEnum.RoleDetails.HOD)
                //    return RedirectToAction("ShowAllDashBoardList", "DashBoard", new { area = "DashBoard" });



                //added by akash(13-06-2018)
                KaveriSession.Current.ParentMenuId = 0;//To Reset ParentMenuID to display horizonatl menus Properly.
                                                      //    KaveriSession.Current.TopMostParentMenuId = 0;

                //To set Defalt Tab To be opened for SR Login
                KaveriSession.Current.SR_Home_NextTab = 0;//Addded by Akash

                CommonFunctions objCommon = new CommonFunctions();
                LoadMenuModel model = new LoadMenuModel();
                model.RoleID = KaveriSession.Current.RoleID;
                model.ParentMenuId = 0;
                model.UserID = KaveriSession.Current.UserID;
                model.OfficeID = KaveriSession.Current.OfficeID;
                ServiceCaller caller = new ServiceCaller("HomeApiController");



                //To check Password expiry of user.....(29-10-2018)
                PasswordDetailsModel objPasswordDetailsModel = new PasswordDetailsModel();
                objPasswordDetailsModel.RoleID = KaveriSession.Current.RoleID;
                objPasswordDetailsModel.UserID = KaveriSession.Current.UserID;
                PasswordDetailsModel reponceOfPasswordDetails = caller.PostCall<PasswordDetailsModel, PasswordDetailsModel>("GetUserPasswordDetails", objPasswordDetailsModel);

                // BELOW CODE INTEGRATED ON 05-08-2020
                // ADDED BY SHUBHAM BHAGAT ON 2-26-2020
                // CONDITION FOR TECHNICAL ADMIN TO SKIP PASSWORD CHANGE AFTER 30 DAYS
                if (KaveriSession.Current.RoleID != (int)CommonEnum.RoleDetails.TechnicalAdmin )
                {
                    //Added by mayank on 06/12/2021 TO SKIP SW support password expiry 
                    if (KaveriSession.Current.RoleID != (int)CommonEnum.RoleDetails.CDACSupportTeam )
                    {
                        if (reponceOfPasswordDetails.IsPasswordExpired)
                        {
                            string PasswordTooOldMessage = string.Empty;
                            TempData["PasswordTooOld"] = reponceOfPasswordDetails.ResponseMessage;
                            return RedirectToAction("ChangePassword", "UserProfileDetails", new { area = "UserManagement" });
                        } 
                    }
                }
                // ADDED BY SHUBHAM BHAGAT ON 2-26-2020
                // CONDITION FOR TECHNICAL ADMIN TO SKIP PASSWORD CHANGE AFTER 30 DAYS
                // ABOVE CODE INTEGRATED ON 05-08-2020

                // On 9-4-2019 by Shubham Bhagat for password change on first login
                if (!reponceOfPasswordDetails.IsFirstLogin)
                {
                    return RedirectToAction("ChangePassword", "UserProfileDetails", new { area = "UserManagement" });
                }
                HomePageModel response = caller.PostCall<LoadMenuModel, HomePageModel>("GetHomePageDetails", model);
                //Added By Tushar on 24 Nov 2022
                var TechDiagMenuId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TechDiagMenuId"]);
                if (response.MenuListReturn.Any(x => x.IntMenuId == TechDiagMenuId) && KaveriSession.Current.RoleID != (int)CommonEnum.RoleDetails.TechnicalAdmin)
                {
                    var itemToRemove = response.MenuListReturn.SingleOrDefault(r => r.IntMenuId == TechDiagMenuId && r.StrMenuName == "Technical Diagnostics");
                    if (itemToRemove != null)
                        response.MenuListReturn.Remove(itemToRemove);
                }

                // End By Tushar on 24 Nov 2022
                if (KaveriSession.Current.ModuleID != 0)
                {
                    response.DefaultModuleID = (int)KaveriSession.Current.ModuleID;
                    response.DefaultModuleName = KaveriSession.Current.ModuleName;

                }
                else
                {
                    response.DefaultModuleID = response.MenuListReturn[0].IntModuleId ?? 0;
                    response.DefaultModuleName = response.MenuListReturn[0].StrMenuName;
                }
                response.Years = objCommon.GetYearDropDown();
                response.YearID = DateTime.Now.Year;

                response.Months = objCommon.GetMonthDropDown();
                response.MonthID = DateTime.Now.Month + 1;

                //To directly redirect to search page Commented by Raman Kalegaonkar
                //if (KaveriSession.Current.RoleID == (short)CommonEnum.RoleDetails.HOD || KaveriSession.Current.RoleID == (short)CommonEnum.RoleDetails.DR)
                //{
                //    KaveriSession.Current.ModuleID = 7;
                //    KaveriSession.Current.ModuleName = "Dashboard";

                //    KaveriSession.Current.ServiceID = Convert.ToInt16(CommonEnum.enumServiceTypes.FirmRegistration);
                //    return RedirectToAction("MISReportsDetailsDetailsView", "MISReports", new { area = "Reports" });

                //}


                return View(response);

            }
        }



        public ActionResult HomePage_OfficeUser()
        {

            //added by Akash on 19-07-2018
            var urlReferer = Request.UrlReferrer;
            if (urlReferer == null)
                return RedirectToAction("Error", "Login");
            if (KaveriSession.Current.UserID == 0)
            {
                Session.Abandon();
                KaveriSession.Current.EndSession();
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //added by soujanya 05-10-2017 and commented cookies.add
                Response.Cookies.Clear();
                Response.Cookies["KaveriSessionID"].Expires = DateTime.Now.AddYears(-30);
                //Response.Cookies.Add(new HttpCookie("KaveriSessionID", ""));
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                return Redirect("/Error/SessionExpire");
            }
            else
            {

                //if (KaveriSession.Current.RoleID != (short)CommonEnum.RoleDetails.OnlineUser)
                //    return RedirectToAction("HomePage_OfficeUser");

                //added by akash(13-06-2018)
                KaveriSession.Current.ParentMenuId = 0;//To Reset ParentMenuID to display horizonatl menus Properly.
                                                      //    KaveriSession.Current.TopMostParentMenuId = 0;
                CommonFunctions objCommon = new CommonFunctions();
                LoadMenuModel model = new LoadMenuModel();
                model.RoleID = KaveriSession.Current.RoleID;
                model.ParentMenuId = 0;
                model.UserID = KaveriSession.Current.UserID;
                model.OfficeID = KaveriSession.Current.OfficeID;
                ServiceCaller caller = new ServiceCaller("HomeApiController");
                HomePageModel response = caller.PostCall<LoadMenuModel, HomePageModel>("GetHomePageDetails", model);

                if (KaveriSession.Current.ModuleID != 0)
                {
                    response.DefaultModuleID = (int)KaveriSession.Current.ModuleID;
                    response.DefaultModuleName = KaveriSession.Current.ModuleName;

                }
                else
                {
                    response.DefaultModuleID = response.MenuListReturn[0].IntModuleId ?? 0;
                    response.DefaultModuleName = response.MenuListReturn[0].StrMenuName;
                }
                response.Years = objCommon.GetYearDropDown();
                response.YearID = DateTime.Now.Year;

                response.Months = objCommon.GetMonthDropDown();
                response.MonthID = DateTime.Now.Month + 1;
                return View(response);

            }
        }

        [HttpPost]
        //  [KaveriAuthorizationAttribute]
        public ActionResult GetHomePageSideBarStatistics(LoadMenuModel menuObj)
        {
            if (ModelState.IsValid)
            {

                //    LoadMenuModel model = new LoadMenuModel();
                menuObj.RoleID = KaveriSession.Current.RoleID;
                menuObj.ParentMenuId = 0;
                menuObj.UserID = KaveriSession.Current.UserID;
                menuObj.OfficeID = KaveriSession.Current.OfficeID;

                if (menuObj.MonthID == 0 && menuObj.YearID == 0)
                { //******* For 1st request **************
                    menuObj.MonthID = DateTime.Now.Month + 1;
                    menuObj.YearID = DateTime.Now.Year;
                }

                ServiceCaller caller = new ServiceCaller("HomeApiController");
                MenuItems response = caller.PostCall<LoadMenuModel, MenuItems>("GetHomePageSideBarStatistics", menuObj);
                response.StrMenuName = menuObj.ModuleName;
                return Json(new { success = true, subModuleList = response.SubModuleList });
            }
            else
            {
                string errorMsg = ModelState.FormatErrorMessage();
                return Json(new { success = false, responseMsg = errorMsg });
            }
            // return View(response);
        }


        /// <summary>
        /// Redirect to appropriate Module
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[KaveriAuthorizationAttribute]
        public ActionResult RedirectToMenuPage(string id)
        {

            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index", "Error");

            if (KaveriSession.Current.UserID == 0)
            {
                Session.Abandon();
                KaveriSession.Current.EndSession();
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //added by soujanya 05-10-2017 and commented cookies.add
                Response.Cookies.Clear();
                Response.Cookies["KaveriSessionID"].Expires = DateTime.Now.AddYears(-30);
                //Response.Cookies.Add(new HttpCookie("KaveriSessionID", ""));
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                // Added by Tushar on 19 April 2022 for DeleteExpiredSessions
                //return Redirect("/Error/Index");
                return Redirect("/Error/SessionExpire");
            }
            int ParentMenuId = Convert.ToInt32(id.Split(new[] { '$' })[0]); //store in session as current module and as per module render menu
            KaveriSession.Current.ModuleID = String.IsNullOrEmpty(id.Split(new[] { '$' })[1]) ? 0 : Convert.ToInt32(id.Split(new[] { '$' })[1]); //store in session as current module and as per module render menu
            KaveriSession.Current.ModuleName = (id.Split(new[] { '$' })[2]).ToString();

            ServiceCaller service = new ServiceCaller("MenuHelperApiController"); //Get Submenu details 
            var SubMenuDetails = service.GetCall<MenuItems>("GetSubMenuDetails", new { ParentMenuID = ParentMenuId, KaveriSession.Current.RoleID });


            if (SubMenuDetails == null)
            {
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Child Menu details Not found.", URLToRedirect = "/Home/HomePage" });

            }
            if (ParentMenuId == (int)CommonEnum.ParentMenuIdEnum.UserManager)
            {
                //Added by Akash(16-04-2019) to skip below code for Menu i.e "UserManager" bcoz it doesn't has child menu.


            // RAFE 26-11-19
                //if(KaveriSession.Current.RoleID==(short)CommonEnum.RoleDetails.DepartmentAdmin)
                //         KaveriSession.Current.IsLandingPageChanged = true;


                //KaveriSession.Current.TopMostParentMenuId = (int)CommonEnum.ParentMenuIdEnum.RoleMenuMapping;
                //return RedirectToAction("RoleDetailsList", "RoleDetails", new { area = "UserManagement" });
            }
            else
            {
                //For normal parent-child menu
                KaveriSession.Current.TopMostParentMenuId = SubMenuDetails.IntMenuParentId;
            }



            KaveriSession.Current.TopMostParentMenuId = SubMenuDetails.IntMenuParentId;

            if (SubMenuDetails.isMenuIDParameter)
                return RedirectToAction(SubMenuDetails.StrAction, SubMenuDetails.StrController, new { area = SubMenuDetails.strAreaName, MenuID = SubMenuDetails.IntMenuId });
            else
                return RedirectToAction(SubMenuDetails.StrAction, SubMenuDetails.StrController, new { area = SubMenuDetails.strAreaName });

            #region Commented by akash(Redirecting to specific Modules.)

            //if (ParentMenuId == Convert.ToInt32(CommonEnum.ParentMenuIdEnum.Firm)) //158 //Firm
            //{
            //    KaveriSession.Current.ServiceID = Convert.ToInt16(CommonEnum.enumServiceTypes.FirmRegistration);
            //    return RedirectToAction("NavigationView", "Home", new { area = "" });

            //}
            //else if (ParentMenuId == Convert.ToInt32(CommonEnum.ParentMenuIdEnum.CertifiedCopy)) //82 //Certified Copy
            //{
            //    KaveriSession.Current.ServiceID = Convert.ToInt16(CommonEnum.enumServiceTypes.CertifiedCopies);
            //    return RedirectToAction("ReceivedCCApplications", "CertifiedCopy", new { area = "CertifiedCopy" });
            //}
            //else
            //{
            //    ServiceCaller service = new ServiceCaller("MenuHelperApiController"); //Get Submenu details 
            //    var SubMenuDetails = service.GetCall<MenuItems>("GetSubMenuDetails", new { ParentMenuID = ParentMenuId, RoleID = KaveriSession.Current.RoleID });

            //    if (SubMenuDetails.isMenuIDParameter)
            //        return RedirectToAction(SubMenuDetails.StrAction, SubMenuDetails.StrController, new { area = SubMenuDetails.strAreaName, MenuID = SubMenuDetails.IntMenuId });
            //    else
            //        return RedirectToAction(SubMenuDetails.StrAction, SubMenuDetails.StrController, new { area = SubMenuDetails.strAreaName });
            //}

            #endregion


        }



        /// <summary>
        /// This method Redirect to appropriate Module.
        /// </summary>
        /// <returns></returns>
        //[KaveriAuthorizationAttribute]
        public ActionResult NavigationView()
        {
            // int ModuleId = KaveriSession.Current.ModuleID ?? 0;
            int ModuleId = KaveriSession.Current.ModuleID;


            switch (ModuleId)
            {
                //case (int)CommonEnum.Modules.FirmRegistration:
                //    KaveriSession.Current.TopMostParentMenuId = (int)CommonEnum.ParentMenuIdEnum.Firm;


                //    return RedirectToAction("LoadFirmDetails", "FirmRegistration", new { area = "Firm" });


                //case (int)CommonEnum.Modules.CertifiedCopy:
                //    KaveriSession.Current.TopMostParentMenuId = (int)CommonEnum.ParentMenuIdEnum.CertifiedCopy;


                //    return RedirectToAction("LoadCCDetails", "CertifiedCopy", new { area = "CertifiedCopy" });

                default:

                    break;
            }
            return View();
        }




        static int ClientRequest = 0;
        static bool ClientSession = true;
        static bool ServerSession = false;
        private static System.Timers.Timer aTimer;
        static int ServerRequest = 1;
        static bool IsSessionExpired = false;

        [HttpPost]
        public JsonResult KeepSessionAlive(int request)
        {
            ClientRequest = request;
            if (request == 1)
            {
                SetTimer();
              //  IsSessionExpired = false;
            }
            ClientSession = true;

            //return RedirectToAction("SessionExpire", new RouteValueDictionary(new { controller = "Error", action = "SessionExpire" }));



            if (IsSessionExpired)
                return Json(new { success = false }); //Session Expired //And stop Server Timer.
            else
                return Json(new { success = true });


            // return new JsonResult { success=true };


        }

        private static void SetTimer()
        {
            // Create a timer with a ten second interval.
            aTimer = new System.Timers.Timer(7000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            ServerRequest++;
            if (ServerSession.CompareTo(ClientSession) == 0)
            {
                if ((3 * ClientRequest) % (3 * 7) == 0)//&& (7 * ServerRequest) % (3 * 7) == 0
                {
                    //Skip
                }
                else
                {
                    if (ServerSession.CompareTo(ClientSession) == 0)
                    {
                        aTimer.Stop();
                        aTimer.Enabled = false;
                        IsSessionExpired = true;
                        HomeController home = new HomeController();

                        home.RedirectToError();
                        //Redirect to error page from here only.............


                        //   KaveriSession.Current.IsSessionExpiredStatus = true;
                        //end session and stop timer
                    }
                }

            }
            ClientSession = false;
        }

        private ActionResult RedirectToError()
        {
           // Response.RedirectLocation
            return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Error", action = "Index" }));

        }

        /// LogOff Action
        /// </summary>
        /// <returns></returns>
        public ActionResult Logoff()
        {
            //UserSession.Current.EndSession();
            //new CommonFunctions().RegenerateId();
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return RedirectToAction("Index");
        }

        public ActionResult NotFound()
        {

            return RedirectToAction("NotFound");
        }


        public ActionResult UnAuthorized()
        {

            return RedirectToAction("UnAuthorized");
        }




        //Added by madhur on 15-02-2022


        [HttpGet]
        public ActionResult IsMobileNumberVerified()
        {
            ServiceCaller caller = new ServiceCaller("HomeApiController");

                long userID = KaveriSession.Current.UserID;
            string errorMessage = string.Empty;

            bool response = caller.GetCall<bool>("IsMobileNumberVerified", new { userID = userID.ToString() }, out errorMessage);

            if (response)
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        [EventAuditLogFilter(Description = "Send OTP to Mobile Number of Online user")]
        public ActionResult InputOTPFromUser()
        {
            string EncryptedUId = URLEncrypt.EncryptParameters(new String[] { "UserId=" + KaveriSession.Current.UserID });
            short IsOTPSent = (short)ECDataUI.Common.CommonEnum.IsOTPSent.OTPYetToBeSend;
            short OTPTypeId = (short)ECDataUI.Common.CommonEnum.OTPTypeId.MobileVerification;
            string errorMessage = string.Empty;
            if (IsOTPSent == (short)ECDataUI.Common.CommonEnum.IsOTPSent.OTPNotSent) // error case OTP was not sent to user -- shrininvas
            {
                return Json(new { success = false, message = "OTP could not be sent to your registered mobile number." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (IsOTPSent == (short)ECDataUI.Common.CommonEnum.IsOTPSent.OTPYetToBeSend)
                {
                    SendOTPRequestModel OTPRequest = new SendOTPRequestModel();
                    ServiceCaller caller = new ServiceCaller("HomeApiController");

                    //userID = KaveriSession.Current.UserID;

                    OTPRequest.OTPRequestEncryptedUId = EncryptedUId;
                    OTPRequest.OTPTypeId = OTPTypeId;

                    if (OTPRequest.OTPTypeId == (short)ECDataUI.Common.CommonEnum.OTPTypeId.MobileVerification)
                    {
                        OTPRequest.messageToBeSent = "OTP to verify your mobile number is ";
                    }

                    SendSMSResponseModel OTPResponse = caller.PostCall<SendOTPRequestModel, SendSMSResponseModel>("SendOTP", OTPRequest, out errorMessage);

                    //To bypass OTP service(SMS)
                    //SendSMSResponseModel OTPResponse = new SendSMSResponseModel();
                    //OTPResponse.errorCode = "0";
                    //OTPResponse.statusCode = "SUCCESS";

                    OTPValidationModelForCaptcha otpValidationModel = new OTPValidationModelForCaptcha();
                    otpValidationModel.EncryptedUId = URLEncrypt.EncryptParameters(new String[] { "UserId=" + KaveriSession.Current.UserID });
                    otpValidationModel.IsOTPSent = IsOTPSent;
                    otpValidationModel.OTPTypeId = OTPTypeId;
                    // Added by shubham bhagat on 20-04-2019 to show mobile number
                    otpValidationModel.MobileNumber = OTPResponse.MobileNumber;
                    if (OTPResponse.statusCode != null)
                    {
                        otpValidationModel.Message = "OTP has been sent to your registered mobile number.";

                    }


                    if (OTPResponse.errorCode.Equals("0"))
                    {


                        return Json(new { success = true, MobileNumberToDisplay = OTPResponse.MobileNumber, otpValidationModel.IsOTPSent, otpValidationModel.OTPTypeId, otpValidationModel.EncryptedUId }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { success = false, message = "OTP couldn't sent to your mobile number , please contact help desk." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    OTPValidationModelForCaptcha otpValidationModel = new OTPValidationModelForCaptcha();
                    otpValidationModel.EncryptedUId = EncryptedUId;
                    otpValidationModel.IsOTPSent = IsOTPSent;
                    otpValidationModel.OTPTypeId = OTPTypeId;
                    otpValidationModel.Message = "OTP is sent to registered mobile number.";

                    return View("OTPValidation", otpValidationModel);
                }
            }
        }




        [HttpPost]
        public ActionResult SendSMS(SendSMSRequestModel SMSRequest)
        {
            ServiceCaller caller = new ServiceCaller("HomeAPIController");
            string errorMessage = string.Empty;
            SendSMSResponseModel response = caller.PostCall<SendSMSRequestModel, SendSMSResponseModel>("SendSMS", SMSRequest, out errorMessage);

            if (response.errorCode.Equals("0"))
                return Json(new { success = true, message = "SMS sent successfully." });
            else
                return Json(new { success = false, message = response.errorString });
        }



        [HttpPost]
        [EventAuditLogFilter(Description = "Validate OTP to verify user")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult ValidateOTP(OTPValidationModelForCaptcha otpValidationModel)
        {

            string errorMessage = string.Empty;
            OTPValidationModel model = new OTPValidationModel();
            model.Id = otpValidationModel.Id;
            model.IsOTPSent = otpValidationModel.IsOTPSent;
            model.EncryptedOTP = otpValidationModel.EncryptedOTP;
            model.EncryptedUId = otpValidationModel.EncryptedUId;
            model.isToShowResponseMessage = otpValidationModel.isToShowResponseMessage;
            model.OTPTypeId = otpValidationModel.OTPTypeId;
            model.SessionSalt = KaveriSession.Current.SessionSalt;



            ServiceCaller caller = new ServiceCaller("HomeAPIController");

            //otpValidationModel.SessionSalt = ;

            ValidateOTPResponseModel response = caller.PostCall<OTPValidationModel, ValidateOTPResponseModel>("ValidateOTP", model, out errorMessage);

            //add here errorMessage as out parameter to check error occured while 
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Json(new { success = false, msg = errorMessage });
            }

            if (response.responseStatus == true)
            {
                if (otpValidationModel.IsOTPSent == (short)ECDataUI.Common.CommonEnum.IsOTPSent.OTPAlreadySent)
                {
                    return Json(new { success = true, msg = response.responseMessage, redirectToLoginPage = true, url = "/Login/UserLogin" });
                }
                else
                {
                    return Json(new { success = true, msg = response.responseMessage });
                }
            }
            else
            {
                if (otpValidationModel.IsOTPSent == (short)ECDataUI.Common.CommonEnum.IsOTPSent.OTPAlreadySent)
                {
                    return Json(new { success = false, msg = response.responseMessage, redirectToLoginPage = true, url = "/Login/UserLogin" });
                }
                else
                {
                    return Json(new { success = false, msg = response.responseMessage });
                }
            }
            }






        public ActionResult GetSessionSalt()
        {


            var urlReferer = Request.UrlReferrer;
            if (urlReferer == null)
                return RedirectToAction("Index", "Error");

            CommonFunctions fun = new CommonFunctions();
            fun.RegenerateSessionId();

            string EncryptionKey = "Gxv@#123";
            string StaticSalt = URLEncrypt.EncryptParameters(new String[] { "Key=" + EncryptionKey }, "1");



            KaveriSession.Current.SessionSalt = SHA512ChecksumWrapper.GenerateSalt(15);
            return Json(new
            {
                success = true,
                dataMessage = KaveriSession.Current.SessionSalt,
                SENCData = StaticSalt
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadModal()
        {
            OTPValidationModelForCaptcha otpValidationModel = new OTPValidationModelForCaptcha();
            otpValidationModel.IsCaptchaVerified = false;

            otpValidationModel.EncryptedUId = URLEncrypt.EncryptParameters(new String[] { "UserId=" + KaveriSession.Current.UserID });

            otpValidationModel.IsOTPSent = (short)ECDataUI.Common.CommonEnum.IsOTPSent.OTPYetToBeSend;
            otpValidationModel.OTPTypeId = (short)ECDataUI.Common.CommonEnum.OTPTypeId.MobileVerification;
            return View("OTPValidation", otpValidationModel);
        }
		
		//changes by madhur till here

            //Added By Tushar on 31 jan 2023
        public ActionResult GetMenuDisabledDetails()
        {
            ServiceCaller caller = new ServiceCaller("DisableKaveriAPIController");
            var RoleID = KaveriSession.Current.RoleID;
            var OfficeID = KaveriSession.Current.OfficeID;
            if (RoleID ==Convert.ToInt32(CommonEnum.RoleDetails.SR) && OfficeID != 0)
            {
                #region Code for Disable Menus for Sro offices
                //string MenuDisabledOfficeID = System.Configuration.ConfigurationManager.AppSettings["MenuDisabledOfficeID"];
                var MenuDisabledOfficeIDList = caller.GetCall<MenuDisabledOfficeIDModel>("GetMenuDisabledOfficeID");
                string MenuDisabledOfficeID = MenuDisabledOfficeIDList.DisabledOfficeID;
                if (!string.IsNullOrEmpty(MenuDisabledOfficeID))
                {
                    string[] DisabledSroCodeList = MenuDisabledOfficeID.Split(',');
                    if (DisabledSroCodeList.Contains(Convert.ToString(OfficeID)))
                    {
                        return Json(new { success = false, message = "Please use KAVERI 2 for this functionality." }, JsonRequestBehavior.AllowGet);
                    }
                }

                #endregion 
            }
            return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);

        }
        //End By Tushar on 31 Jan 2023
    }
}