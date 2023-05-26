#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   UserProfileDetailsController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for User Management module.
*/
#endregion

using CustomModels.Models.Alerts;
using CustomModels.Models.Common;
using CustomModels.Models.UserManagement;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Models.UserRegistration;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.UserManagement.Controllers
{
    [KaveriAuthorizationAttribute]
    public class UserProfileDetailsController : Controller
    {
        ChangePasswordNewModel otpValidationModel = null;

        string errorMessage = string.Empty;
        string UserID = Convert.ToString(KaveriSession.Current.UserID);

        /// <summary>
        /// PViewUserProfileDetails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PViewUserProfileDetails()
        {
            try
            {
                ServiceCaller caller = new ServiceCaller("UserProfileDetailsApiController");
                UserProfileModel response = caller.GetCall<UserProfileModel>("EditUserProfileDetails", new { EncryptedID = UserID });
                string errorMessage = string.Empty;
                ServiceCaller caller2 = new ServiceCaller("CommonsApiController");
                int[] array = (int[])CommonEnum.MasterDropDownEnum.GetValues(typeof(CommonEnum.MasterDropDownEnum));
                EnumDropDownListModel dropdownmodel = new EnumDropDownListModel();
                dropdownmodel.DropdownListToFill = array;

                MasterDropDownModel masterModel = caller2.PostCall<EnumDropDownListModel, MasterDropDownModel>("FillMasterDropDownModel", dropdownmodel, out errorMessage);
                response.IdProofsTypeDropDown = masterModel.IdProofsTypeDropDown;
                response.CountryDropDown = masterModel.CountryDropDown;
                return PartialView("PViewUserProfileDetails", response);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving UserProfile page", URLToRedirect = "/Home/HomePage" });

            }
        }

        /// <summary>
        /// View User Profile Details
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewUserProfileDetails()
        {
            try
            {
                //added by akash(14-06-2018) To reset horizontal menu's
                KaveriSession.Current.KioskTokenID = 0;
                KaveriSession.Current.ParentMenuId = 0;//To Display MenuBar(Horizontal) Properly.

                // Added BY SB on 2-04-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.UpdateProfile;

                return View("ViewUserProfileDetails");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving User Profile Details View", URLToRedirect = "/Home/HomePage" });
            }

        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            try
            {
                //added by akash(14-06-2018) To reset horizontal menu's
                KaveriSession.Current.KioskTokenID = 0;
                KaveriSession.Current.ParentMenuId = 0;//To Display MenuBar(Horizontal) Properly.
                                                       //  KaveriSession.Current.SubParentMenuToBeActive = (Int32)CommonEnum.SubParentMenuToBeActive.Registration;

                // Added BY SB on 2-04-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ChangePassword;


                otpValidationModel = new ChangePasswordNewModel();
                int NoOfPreviousPasswordsMatch = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfPreviousPasswordNotAllowed"]);

                otpValidationModel.EncryptedUId = URLEncrypt.EncryptParameters(new String[] { "UserId=" + UserID });
                otpValidationModel.NumberOfPreviousPasswordNotAllowed = NoOfPreviousPasswordsMatch;

                return View("ChangePassword", otpValidationModel);
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Change Password View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// SaveChangedPassword
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult SaveChangedPassword(ChangePasswordNewModel viewModel)
        {

            try
            {
                ServiceCaller caller = new ServiceCaller("UserProfileDetailsApiController");
                viewModel.userID = Convert.ToInt32(UserID);
                if (viewModel.NewPassword == viewModel.ConfirmPassword)
                {
                    if (viewModel.ConfirmPassword.Length == 128 && viewModel.CurrentPassword.Length == 128 && viewModel.NewPassword.Length == 128)
                    {
                        ChangePasswordResponseModel responseModel = caller.PostCall<ChangePasswordNewModel, ChangePasswordResponseModel>("SaveChangedPassword", viewModel, out errorMessage);
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

        /// <summary>
        /// UpdateUserProfileDetails
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UpdateUserProfileDetails(UserProfileModel viewModel)
        {
            try
            {
                ServiceCaller caller = new ServiceCaller("UserProfileDetailsApiController");
                string errorMessage = string.Empty;

                // commented by shubham  bhagat on 18-04-2019
                //if (ModelState.ContainsKey("Email"))
                //{
                //    ModelState.Remove("Email");
                //}

                // Added by shubham  bhagat on 18-04-2019
                if (ModelState.ContainsKey("Username"))
                {
                    ModelState.Remove("Username");
                }

                if (ModelState.IsValid)
                {
                    UserProfileDetailsResponseModel responseModel = caller.PostCall<UserProfileModel, UserProfileDetailsResponseModel>("UpdateUserProfileDetails", viewModel, out errorMessage);
                    if (!String.IsNullOrEmpty(errorMessage))
                    {
                        return Json(new { success = false, message = errorMessage });
                    }
                    else
                    {
                        KaveriSession.Current.FullName = viewModel.FirstName + " " + viewModel.LastName;
                        return Json(new { success = responseModel.Status, message = responseModel.ErrorMessage });
                    }
                }
                else
                {
                    string errorMsg = ModelState.FormatErrorMessage();
                    return Json(new { success = false, message = errorMsg, errorMessage });
                }
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Updating User Profile", URLToRedirect = "/Home/HomePage" });
            }
        }




        /// <summary>
        /// P Edit User Profile Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PEditUserProfileDetails()
        {
            try
            {
                ServiceCaller caller = new ServiceCaller("UserProfileDetailsApiController");
                UserProfileModel response = caller.GetCall<UserProfileModel>("EditUserProfileDetails", new { EncryptedID = KaveriSession.Current.UserID });


                string errorMessage = string.Empty;
                ServiceCaller caller2 = new ServiceCaller("CommonsApiController");
                int[] array = (int[])CommonEnum.MasterDropDownEnum.GetValues(typeof(CommonEnum.MasterDropDownEnum));
                EnumDropDownListModel dropdownmodel = new EnumDropDownListModel();
                dropdownmodel.DropdownListToFill = array;

                MasterDropDownModel masterModel = caller2.PostCall<EnumDropDownListModel, MasterDropDownModel>("FillMasterDropDownModel", dropdownmodel, out errorMessage);

                response.IdProofsTypeDropDown = masterModel.IdProofsTypeDropDown;
                response.CountryDropDown = masterModel.CountryDropDown;


                return PartialView(response);
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Edit User Profile View View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Check Mobile No Availability
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckMobileNoAvailability(string mobileNo)
        {

            string errorMessage = string.Empty;
            try
            {
                UserProfileModel objUserModel = new UserProfileModel();
                objUserModel.MobileNumber = mobileNo;
                objUserModel.UserID = Convert.ToInt32(UserID);


                ServiceCaller caller = new ServiceCaller("UserProfileDetailsApiController");

                Boolean IsMobileNoExist = caller.PostCall<UserProfileModel, bool>("CheckMobileNoAvailability", objUserModel, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success = false, errorMessage });
                }

                if (IsMobileNoExist)
                {
                    return Json(new { success = true, message = "Mobile No alredy exists." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while checking mobile number availability", URLToRedirect = "/Home/HomePage" });
            }
        }

    }
}