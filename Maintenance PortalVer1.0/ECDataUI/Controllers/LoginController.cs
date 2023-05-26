

#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   LoginController.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   14-04-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for user Login.
*/
#endregion









#region References
using CustomModels.Models.UserManagement;
using ECDataUI.Common;
using ECDataUI.Filters;
using CustomModels.Security;

using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CustomModels.Common;
#endregion

namespace ECDataUI.Controllers
{
    public class LoginController : Controller
    {

        #region Properties
        private static object threadLock = new object();

        #endregion

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        [EventAuditLogFilter(Description = "Login")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UserLogin(LoginViewModelTemp model)
        {

            if (ModelState.IsValid)
            {
                ServiceCaller caller = new ServiceCaller("LoginApiController");
                //  UserActivationModel response = caller.PostCall<UserModel, UserActivationModel>("PostUserDetail", model);


                LoginViewModel objLoginViewModel = new LoginViewModel();
                objLoginViewModel.EmailId = model.EmailId;
                objLoginViewModel.Password = model.Password;

                LoginResponseModel response = caller.PostCall<LoginViewModel, LoginResponseModel>("UserLogin", objLoginViewModel);

                if (response == null)
                {                    
                    CommonFunctions.WriteDebugLogInDB((int)Common.CommonEnum.FunctionalityMaster.Login, "Unable to connect to ECData Portal API.Response object is null", "LoginController", "UserLogin");
                    return Json(new { success = false, responseMsg = "Unable to connect to ECData Portal API." });
                }



                if (response.ResponseMessage != string.Empty)
                {
                    return Json(new { success = false, responseMsg = response.ResponseMessage });

                }
                string passwordHash = SHA512ChecksumWrapper.ComputeHash(response.Password, KaveriSession.Current.SessionSalt);



                // added by Akash on 29/10/2018 to prevent multiple login(session) of same user
                if (PreventMultipleLogin.IsUserLoggedIn.ContainsKey(response.UserID))
                {
                    PreventMultipleLogin.IsUserLoggedIn.Remove(response.UserID);
                    PreventMultipleLogin.IsLoggedSession.Remove(response.UserID);
                }


                if (passwordHash.Equals(model.Password.ToUpper()))
                {
                    CommonFunctions objCommon = new CommonFunctions();
                    objCommon.RegenerateSessionId();
                    objCommon.InitializeSession(response);


                    // added by Akash on 29 / 10 / 2018 to prevent multiple login(session) of same user
                    lock (threadLock)
                    {
                        if (KaveriSession.Current.UserID != 0)
                        {
                            PreventMultipleLogin.IsUserLoggedIn.Add(KaveriSession.Current.UserID, KaveriSession.Current.UserName);
                            PreventMultipleLogin.IsLoggedSession.Add(KaveriSession.Current.UserID, KaveriSession.Current.SessionID);
                        }
                    }
                    return Json(new { success = true, URL = "/Home/HomePage" });

                    //#region On 9-4-2019 by Shubham Bhagat for password change on first login
                    //if (response.IsFirstLogin)
                    //{
                    //    return Json(new { success = true, URL = "/Home/HomePage" });
                    //}
                    //else
                    //{
                    //    return Json(new { success = true, URL = "/Home/HomePage?"+ response.IsFirstLogin });
                    //}
                    //#endregion

                }
                else
                {
                    return Json(new { success = false, responseMsg = "Invalid credentials." });

                }

            }
            else
            {
                string errorMsg = ModelState.FormatErrorMessageInString();
                return Json(new { success = false, responseMsg = errorMsg });
                //return View();
            }

        }

        //[HttpPost]
        //[EventAuditLogFilter(Description = "Login")]
        //public ActionResult UserLogin(LoginViewModelTemp model)
        //{


        //    if (ModelState.IsValid)
        //    {



        //        ServiceCaller caller = new ServiceCaller("LoginApiController");
        //        //  UserActivationModel response = caller.PostCall<UserModel, UserActivationModel>("PostUserDetail", model);

        //        LoginResponseModel response = caller.PostCall<LoginViewModel, LoginResponseModel>("UserLogin", model);

        //        if (response.ResponseMessage != string.Empty)
        //        {
        //            return Json(new { success = false, responseMsg = response.ResponseMessage });

        //        }
        //        string passwordHash = SHA512ChecksumWrapper.ComputeHash(response.Password, KaveriSession.Current.SessionSalt);


        //        // added by Akash on 29/10/2018 to prevent multiple login(session) of same user
        //        if (PreventMultipleLogin.IsUserLoggedIn.ContainsKey(response.UserID))
        //        {
        //            PreventMultipleLogin.IsUserLoggedIn.Remove(response.UserID);
        //            PreventMultipleLogin.IsLoggedSession.Remove(response.UserID);
        //        }



        //        if (passwordHash.Equals(model.Password.ToUpper()))
        //        {
        //            CommonFunctions objCommon = new CommonFunctions();
        //            objCommon.RegenerateSessionId();
        //            objCommon.InitializeSession(response);

        //            // added by Akash on 29/10/2018 to prevent multiple login(session) of same user
        //            lock (threadLock)
        //            {
        //                if (KaveriSession.Current.UserID != 0)
        //                {
        //                    PreventMultipleLogin.IsUserLoggedIn.Add(KaveriSession.Current.UserID, KaveriSession.Current.UserName);
        //                    PreventMultipleLogin.IsLoggedSession.Add(KaveriSession.Current.UserID, KaveriSession.Current.SessionID);
        //                }
        //            }


        //            return Json(new { success = true, URL = "/Home/HomePage" });

        //        }
        //        else
        //        {
        //            return Json(new { success = false, responseMsg = "Invalid credentials." });

        //        }

        //    }
        //    else
        //    {

        //        return View();
        //    }

        //}



        [HttpGet]
        public ActionResult Error()
        {
            Session.Abandon();
            KaveriSession.Current.EndSession();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.Cookies.Clear();
            Response.Cookies["KaveriSessionID"].Expires = DateTime.Now.AddYears(-30);

            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame[] stackFrames = t.GetFrames();
            return View();
        }


        /// <summary>
        /// Update Log & Abondon Session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Logout")]
        public ActionResult Logout()
        {


            //log user logout details

            FormsAuthentication.SignOut();
            Session.Abandon();
            KaveriSession.Current.EndSession();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cookies.Clear();
            Response.Cookies["KaveriSessionID"].Expires = DateTime.Now.AddYears(-30);

            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            if (PreventMultipleLogin.IsUserLoggedIn.ContainsKey(KaveriSession.Current.UserID))
            {
                string previousSessionID = string.Empty;
                PreventMultipleLogin.IsLoggedSession.TryGetValue(KaveriSession.Current.UserID, out previousSessionID);
                if (KaveriSession.Current.SessionID == previousSessionID)
                {
                    PreventMultipleLogin.IsUserLoggedIn.Remove(KaveriSession.Current.UserID);
                    PreventMultipleLogin.IsLoggedSession.Remove(KaveriSession.Current.UserID);
                }
            }



            //  return Redirect("/Login/UserLogin");
            //            return RedirectToAction("UserLogin", "Login");

            //      return View("UserLogin");
            return Json(new { success = true, URL = "/Login/UserLogin" }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [EventAuditLogFilter(Description = "Logout")]
        public ActionResult LogoutError()
        {


            //log user logout details

            FormsAuthentication.SignOut();
            Session.Abandon();
            KaveriSession.Current.EndSession();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cookies.Clear();
            Response.Cookies["KaveriSessionID"].Expires = DateTime.Now.AddYears(-30);

            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return Redirect("/Login/UserLogin");
            //            return RedirectToAction("UserLogin", "Login");

            //      return View("UserLogin");
            //            return Json(new { success = true, URL = "/Login/UserLogin" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Get New Session Salt
        /// </summary>
        /// <returns></returns>
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

        public ActionResult GetCaptchaImage()
        {
            return CaptchaLib.ControllerExtensions.Captcha(this, new CaptchaLib.CaptchaImage(), 100, 220, 70);

        }

    }
}