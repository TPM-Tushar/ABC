
using CustomModels.Models.Alerts;
using CustomModels.Models.UserManagement;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.AnywhereEC.Controllers
{
    [KaveriAuthorizationAttribute]
    public class AnywhereECController : Controller
    {
        ChangePasswordNewModel ChangePasswordNewModel = null;
        string errorMessage = string.Empty;
        string UserID = Convert.ToString(KaveriSession.Current.UserID);

        
        [MenuHighlight]
        [EventAuditLogFilter(Description = "AnywhereEC forgot password View")]
        public ActionResult ForgotPasswordView()
        {
            try
            {
                KaveriSession.Current.KioskTokenID = 0;
                KaveriSession.Current.ParentMenuId = 0;
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ChangePassword;
                //ChangePasswordNewModel = new ChangePasswordNewModel();
                ServiceCaller caller = new ServiceCaller("AnywhereECApiController");
                int userId = Int32.Parse(UserID);
                ChangePasswordNewModel = caller.GetCall<ChangePasswordNewModel>("ForgotPasswordView", new { userid = userId });


                ChangePasswordNewModel.EncryptedUId = URLEncrypt.EncryptParameters(new String[] { "UserId=" + UserID });
                //ChangePasswordNewModel.NumberOfPreviousPasswordNotAllowed = NoOfPreviousPasswordsMatch;

                return View(ChangePasswordNewModel);
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Forgot Password View", URLToRedirect = "/Home/HomePage" });
            }
        }

        [EventAuditLogFilter(Description = "AnywhereEC forgot password")]
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult ForgotPassword(ChangePasswordNewModel changeNewPassword)
        {
            try
            {
                //VALIDATION
                if (string.IsNullOrEmpty(changeNewPassword.NewPassword))
                {
                    return Json(new { success = false, message = "Please enter new password" });
                }
                if (string.IsNullOrEmpty(changeNewPassword.ConfirmPassword))
                {
                    return Json(new { success = false, message = "Please Enter Retype New Password" });
                }


                ServiceCaller caller = new ServiceCaller("AnywhereECApiController");
                changeNewPassword.userID = Convert.ToInt32(UserID);


                if (changeNewPassword.NewPassword == changeNewPassword.ConfirmPassword)
                {
                    if (changeNewPassword.NewPassword.Length == 32 && changeNewPassword.NewPassword.Length == 32)
                    {
                        ChangePasswordResponseModel responseModel = caller.PostCall<ChangePasswordNewModel, ChangePasswordResponseModel>("SaveNewPassword", changeNewPassword, out errorMessage);
                        if (!String.IsNullOrEmpty(errorMessage))
                            return Json(new { success = false, message = errorMessage });

                        if (responseModel == null)
                        {
                            return Json(new { success = false, message = "Unable to save Password" });
                        }
                        else
                        {
                            return Json(new { success = responseModel.status, responseModel.message });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid Password" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Passwords are not matching" });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Saving new Password", URLToRedirect = "/Home/HomePage" });
            }
        }


        public ActionResult GetSessionSalt()
        {
            if (KaveriSession.Current.SessionSalt != null)
            {
                return Json(new
                {
                    success = true,
                    dataMessage = KaveriSession.Current.SessionSalt
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, dataMessage = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}