

#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   AccountController.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   14-04-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for user registration.
*/
#endregion


//t o tes


#region References
using CustomModels.Models.UserManagement;
using ECDataUI.Common;
using ECDataUI.Models.UserRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaLib;
using CustomModels.Models.Common;
using CustomModels.Security;

using ECDataUI.Session;
using ECDataUI.Filters;
using System.Text.RegularExpressions;
#endregion
namespace ECDataUI.Controllers
{
    public class AccountController : Controller
    {
        string errorMessage = string.Empty;

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This method returns User registration view.
        /// </summary>
        /// <returns></returns>
        [EventAuditLogFilter(Description = "User Registration")]
        public ActionResult UserRegistration()
        {
            try
            {
                string errorMessage = string.Empty;
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                int[] array = (int[])CommonEnum.MasterDropDownEnum.GetValues(typeof(CommonEnum.MasterDropDownEnum));
                EnumDropDownListModel dropdownmodel = new EnumDropDownListModel();
                dropdownmodel.DropdownListToFill = array;

                MasterDropDownModel masterModel = caller.PostCall<EnumDropDownListModel, MasterDropDownModel>("FillMasterDropDownModel", dropdownmodel, out errorMessage);
                UserViewModel model = new UserViewModel();
                //if (string.IsNullOrEmpty(errorMessage))
                //    model.CountryDropDown = masterModel.CountryDropDown;


                model.IdProofsTypeDropDown = masterModel.IdProofsTypeDropDown;
                model.CountryDropDown = masterModel.CountryDropDown;
                model.CountryID = (int)CommonEnum.Countries.India;

                return View(model);
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        ///// <summary>
        ///// POST call for user registration and also sent email for account activation.
        ///// </summary>
        ///// <param name="viewModel"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult UserRegistration(UserViewModel viewModel)
        //{

        //    ModelState.Remove("Password");
        //    ModelState.Remove("ConfirmPassword");

        //    if (ModelState.IsValid)
        //    {
        //        UserModel usermodel = new UserModel();

        //        // usermodel = model.CopyObject<UserModel>(usermodel);

        //        usermodel.UserID = viewModel.UserID;
        //        usermodel.FirstName = viewModel.FirstName;
        //        usermodel.LastName = viewModel.LastName;
        //        usermodel.Address1 = viewModel.Address1;
        //        usermodel.CountryID = viewModel.CountryID;
        //        usermodel.Email = viewModel.Email;
        //        usermodel.MobileNumber = viewModel.MobileNumber;
        //       // usermodel.PAN = viewModel.PAN;
        //        usermodel.Pincode = viewModel.Pincode;
        //        usermodel.Password = viewModel.Password;
        //        usermodel.ConfirmPassword = viewModel.ConfirmPassword;
        //        usermodel.IDProofID = viewModel.IDProofID;
        //        usermodel.IDProofNumber= viewModel.IDProofNumber;

        //        usermodel.Captcha = viewModel.Captcha;


        //        UserModel userModelRegistration = usermodel.TrimAllStringProperties<UserModel>();


        //        ServiceCaller caller = new ServiceCaller("UserRegistrationApiController");
        //        UserActivationModel response = caller.PostCall<UserModel, UserActivationModel>("RegisterUser", userModelRegistration);

        //        if (response.IsSuccessfullyInserted)
        //        {
        //            EmailModel emailModel = CommonFunctions.PrepareActivationEmail(response);

        //            string msg = caller.PostCall<EmailModel, string>("SendEmail", emailModel, out errorMessage);
        //            if (!string.IsNullOrEmpty(errorMessage))
        //            {
        //                return Json(new { success = false, errorMessage = errorMessage });
        //            }

        //            if (string.IsNullOrEmpty(msg))
        //            {
        //                return Json(new { success = true, responseMsg = response.ResponseMessage, errorMessage = errorMessage });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, responseMsg = msg, errorMessage = errorMessage });
        //            }
        //        }
        //        else
        //        {
        //            return Json(new { success = false, responseMsg = response.ResponseMessage, errorMessage = errorMessage });
        //        }
        //    }
        //    else
        //    {
        //        string errorMsg = ModelState.FormatErrorMessage();
        //        return Json(new { success = false, responseMsg = errorMsg, errorMessage = errorMessage });
        //    }
        //}




        /// <summary>
        /// POST call for user registration and also sent email for account activation.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UserRegistration(UserViewModel viewModel)
        {
            try
            {
                long Passlength = 128;
                long temp = viewModel.Password.Length;
                long temp2 = viewModel.ConfirmPassword.Length;

                if (viewModel.Password.Length != Passlength)
                {
                    return Json(new { success = false, responseMsg = "Please enter valid password.", errorMessage });
                }




                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");



                if (ModelState.IsValid)
                {
                    UserModel usermodel = new UserModel();

                    // usermodel = model.CopyObject<UserModel>(usermodel);

                    usermodel.UserID = viewModel.UserID;
                    usermodel.FirstName = viewModel.FirstName;
                    usermodel.LastName = viewModel.LastName;
                    usermodel.Address1 = viewModel.Address1;
                    usermodel.CountryID = viewModel.CountryID;
                    usermodel.Email = viewModel.Email;
                    usermodel.MobileNumber = viewModel.MobileNumber;
                    // usermodel.PAN = viewModel.PAN;
                    usermodel.Pincode = viewModel.Pincode;
                    usermodel.Password = viewModel.Password;
                    usermodel.ConfirmPassword = viewModel.ConfirmPassword;
                    usermodel.IDProofID = viewModel.IDProofID;
                    usermodel.IDProofNumber = viewModel.IDProofNumber;
                    usermodel.IsForIDProofForPartner = false;
                    usermodel.Captcha = viewModel.Captcha;


                    UserModel userModelRegistration = usermodel.TrimAllStringProperties<UserModel>();


                    ServiceCaller caller = new ServiceCaller("UserRegistrationApiController");
                    UserActivationModel response = caller.PostCall<UserModel, UserActivationModel>("RegisterUser", userModelRegistration);

                    if (response.IsSuccessfullyInserted)
                    {
                        EmailModel emailModel = CommonFunctions.PrepareActivationEmail(response);

                        string msg = caller.PostCall<EmailModel, string>("SendEmail", emailModel, out errorMessage);
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            return Json(new { success = false, errorMessage });
                        }

                        if (string.IsNullOrEmpty(msg))
                        {
                            return Json(new { success = true, responseMsg = response.ResponseMessage, errorMessage, response.IsOTPSent, EncryptedUId = response.EncryptedId }); // shrinivas 
                        }
                        else
                        {
                            return Json(new { success = true, responseMsg = msg, errorMessage, response.IsOTPSent, EncryptedUId = response.EncryptedId }); // shrinivas -- USER IS REGISTERED
                        }
                    }
                    else
                    {
                        return Json(new { success = false, errorMessage = response.ResponseMessage });
                    }
                }
                else
                {
                    string errorMsg = ModelState.FormatErrorMessageInString();
                    return Json(new { success = false, errorMessage = errorMsg });
                }
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return Json(new { success = false, errorMessage = "Error occured while processing your request." });

              
            }
        }


        [HttpGet]
        public ActionResult CheckUserNameAvailability(string userName)
            {
            string errorMessage = string.Empty;
            try
            {
                //long FirmID=  KaveriSession.Current.FirmID;
                String encryptedID = URLEncrypt.EncryptParameters(new String[] { "userName=" + userName });

                ServiceCaller caller = new ServiceCaller("UserRegistrationApiController");
                Boolean IsFirmNameExist = caller.GetCall<Boolean>("CheckUserNameAvailability", new {  encryptedID }, out errorMessage);

                if (IsFirmNameExist)
                {
                    return Json(new { success = true, message = "Firm name alredy exists." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// This method returns captcha image.
        public ActionResult GetCaptchaImage()
        {
            //return CaptchaLib.ControllerExtensions.Captcha(this,new MyCaptchaImage(),100,150,60);
            return CaptchaLib.ControllerExtensions.Captcha(this, new CaptchaLib.CaptchaImage(), 100, 200, 70);
        }


        /// <summary>
        /// This method returns account activation view.
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountActivation()
        {
            return View();
        }

        /// <summary>
        /// Post call for account activation & also send email for account activation.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Account Activation")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult AccountActivation(UserActivationViewModel viewModel)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    UserActivationModel model = new UserActivationModel();

                    model.Captcha = viewModel.Captcha;
                    model.Email = viewModel.Email;
                    model.IsSuccessfullyInserted = viewModel.IsSuccessfullyInserted;
                    model.MobileNumber = viewModel.MobileNumber;
                    model.UserID = viewModel.UserID;

                    ServiceCaller caller = new ServiceCaller("UserRegistrationApiController");
                    UserActivationModel response = caller.PostCall<UserActivationModel, UserActivationModel>("AccountActivation", model);

                    if (response.IsSuccessfullyInserted)
                    {
                        EmailModel emailModel = CommonFunctions.PrepareActivationEmail(response);

                        string msg = caller.PostCall<EmailModel, string>("SendEmail", emailModel);

                        if (string.IsNullOrEmpty(msg))
                        {

                            return Json(new { success = true, responseMsg = response.ResponseMessage });
                        }
                        else
                        {
                            return Json(new { success = false, errormsg = msg });

                        }
                    }
                    return Json(new { success = false, errormsg = response.ResponseMessage });

                }
                else
                {
                    string errorMsg = ModelState.FormatErrorMessage();
                    return Json(new { success = false, errormsg = errorMsg });
                }
            }
            catch (Exception e)
            {


                ExceptionLogs.LogException(e);
                return Json(new { success = false, errormsg = "Error occured while processing your request." });
            }


        }

        /// <summary>
        /// This method returns Forgot password view.
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Post call for Forgot password and also send email to change password.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult ForgotPassword(UserActivationViewModel viewModel)
        {
            try
            {
                //Changed and added by Shubham Bhagat on 13-4-2019
                ModelState.Remove("MobileNumber");

                if (ModelState.IsValid)
                {
                    UserActivationModel model = new UserActivationModel();

                    model.Captcha = viewModel.Captcha;
                    model.Email = viewModel.Email;
                    model.IsSuccessfullyInserted = viewModel.IsSuccessfullyInserted;
                    model.MobileNumber = viewModel.MobileNumber;
                    model.UserID = viewModel.UserID;


                    ServiceCaller caller = new ServiceCaller("UserRegistrationApiController");
                    UserActivationModel response = caller.PostCall<UserActivationModel, UserActivationModel>("ForgotPassword", model);

                    if (response==null)
                    {
                        CommonFunctions.WriteDebugLogInDB((int)Common.CommonEnum.FunctionalityMaster.ForgotPassword, "Unable to connect to ECData Portal API.Response object is null", "AccountController", "ForgotPassword");
                        return Json(new { success = false, responseMsg = "Error occured while fetching user details." });
                    }

                    if (response.IsRegisteredEmail)
                    {
                        EmailModel emailModel = CommonFunctions.PrepareForgotPasswordEmail(response);

                        string msg = caller.PostCall<EmailModel, string>("SendEmail", emailModel);

                        if (string.IsNullOrEmpty(msg))
                        {

                            return Json(new { success = true, responseMsg = response.ResponseMessage });
                        }
                        else
                        {
                            return Json(new { success = false, responseMsg = msg });

                        }

                    }
                    else
                    {
                        return Json(new { success = false, responseMsg = response.ResponseMessage });

                    }

                }
                else
                {
                    string errorMsg = ModelState.FormatErrorMessage();
                    //  return Json(new { success = false, responseMsg = errorMsg, errorMessage = errorMessage });

                    return Json(new { success = false, responseMsg = errorMsg });
                }
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return Json(new { success = false, responseMsg = "Error occured while processing your request." });
            }

        }


        /// <summary>
        /// This method returns view which appears after user clicks on Account activation link. 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult AccountActivationByLink(string Id)
        {
            try
            {
                ServiceCaller caller = new ServiceCaller("UserRegistrationApiController");

                string msg = caller.GetCall<string>("AccountActivationByLink", new { Id });

                ViewBag.Text = msg;

                return View();
            }
            catch (Exception e)
            {


                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
                
            }
        }

        /// <summary>
        /// This method returns view which appears after user clicks on forgot password link. 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult ForgotPasswordByLink(string Id)
        {
            try
            {
                ChangePasswordModel model = new ChangePasswordModel() { Id = Id };
                //check for tempering for id parameter

                return View(model);
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Post call for forgot password after click on forgot password link.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Forgot Password /Change password by link")]
        public ActionResult ForgotPasswordByLink(ChangePasswordModel model)
        {
            try
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                if (ModelState.IsValid)
                {
                    ServiceCaller caller = new ServiceCaller("UserRegistrationApiController");
                    string message = caller.PutCall<ChangePasswordModel, string>("ForgotPasswordByLink", model);

                    ChangePasswordModel ResponseModel = new ChangePasswordModel() { Message = message, isToShowResponseMessage = true };


                    return View(ResponseModel);
                }
                else
                {

                    return Json(new { success = false });
                }
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        public ActionResult ForgotPasswordResultView()
        {
            return View();
        }

        [HttpPost]
        public JsonResult KeepSessionAlive()
        {
            return new JsonResult { Data = "Success" };
            //return new JsonResult { Success=true};

        }


        public ActionResult GetSessionSalt()
        {


            var urlReferer = Request.UrlReferrer;
            if (urlReferer == null)
                return RedirectToAction("Index", "Error");

            CommonFunctions fun = new CommonFunctions();
            fun.RegenerateSessionId();


            KaveriSession.Current.SessionSalt = SHA512ChecksumWrapper.GenerateSalt(15);
            return Json(new { success = true, dataMessage = KaveriSession.Current.SessionSalt }, JsonRequestBehavior.AllowGet);
        }


    }
}