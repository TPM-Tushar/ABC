#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   OfficeUserDetailsController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for User Management module.
*/
#endregion

using CustomModels.Models.Common;
using CustomModels.Models.UserManagement;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Models.UserRegistration;
using ECDataUI.Session;
using System.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.UserManagement.Controllers
{
    [KaveriAuthorizationAttribute]
    public class OfficeUserDetailsController : Controller
    {
        string errorMessage = string.Empty;
        ServiceCaller caller = null;

        /// <summary>
        /// ViewOfficeUserDetails
        /// </summary>
        /// <returns>return view</returns>
        public ActionResult ViewOfficeUserDetails()
        {
            try
            {
                // Added BY SB on 8-04-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.UserDetail;

                //Added by mayank for DR user manager
                if (KaveriSession.Current.RoleID == (int)ECDataUI.Common.CommonEnum.RoleDetails.DR)
                    ViewBag.IsNotDrLogin = false;
                else
                    ViewBag.IsNotDrLogin = true;

                return View("ViewOfficeUserDetails");
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// OfficeUserRegistration
        /// </summary>
        /// <returns>return office user view</returns>
        public ActionResult OfficeUserRegistration()
        {

            try
            {
                caller = new ServiceCaller("CommonsApiController");
                int[] array = (int[])CommonEnum.MasterDropDownEnum.GetValues(typeof(CommonEnum.MasterDropDownEnum));
                EnumDropDownListModel dropdownmodel = new EnumDropDownListModel();
                dropdownmodel.DropdownListToFill = array;

                MasterDropDownModel masterModel = caller.PostCall<EnumDropDownListModel, MasterDropDownModel>("FillMasterDropDownModel", dropdownmodel, out errorMessage);
                OfficeUserDetailsModel model = new OfficeUserDetailsModel();
                caller = new ServiceCaller("OfficeUserDetailsApiController");
                model = caller.GetCall<OfficeUserDetailsModel>("GetOfficeUserDetailsList", null, out errorMessage);
                model.IdProofsTypeDropDown = masterModel.IdProofsTypeDropDown;
                model.CountryDropDown = masterModel.CountryDropDown;
                model.CountryID = (int)CommonEnum.Countries.India;
                model.IsActive = true;
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// OfficeUserRegistration
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns>returns office user added message</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Office User Registration")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult OfficeUserRegistration(OfficeUserDetailsModel viewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(viewModel.OfficeID))
                {
                    if (viewModel.OfficeID == "0")
                        return Json(new { success = false, errorMessage = "Office is required." });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Office is required." });
                }

                if (!string.IsNullOrEmpty(viewModel.LevelID))
                {
                    if (viewModel.LevelID == "0")
                        return Json(new { success = false, errorMessage = "Level is required." });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Level is required." });
                }

                if (!string.IsNullOrEmpty(viewModel.RoleID))
                {
                    if (viewModel.RoleID == "0")
                        return Json(new { success = false, errorMessage = "Role is required." });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Role is required." });
                }

                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");

                if (ModelState.IsValid)
                {
                    // For Audit Log
                    viewModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;

                    viewModel.IsForIDProofForPartner = false;

                    caller = new ServiceCaller("OfficeUserDetailsApiController");
                    UserActivationModel response = caller.PostCall<OfficeUserDetailsModel, UserActivationModel>("RegisterUser", viewModel);

                    if (response.IsSuccessfullyInserted)
                    {
                        return Json(new { success = true, message = "Office User Details Added Successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Office User Details Not Added", errorMessage = response.ResponseMessage });
                    }
                }
                else
                {
                    // string errorMsg = ModelState.FormatErrorMessage();
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

                caller = new ServiceCaller("OfficeUserDetailsApiController");
                Boolean IsFirmNameExist = caller.GetCall<Boolean>("CheckUserNameAvailability", new { encryptedID }, out errorMessage);

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

                return Json(new { success = false}, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// UpdateOfficeUser
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns>returns office user data to edit</returns>
        [HttpGet]
        public ActionResult UpdateOfficeUser(String EncryptedId)
        {
            try
            {
                OfficeUserDetailsModel objOfficeUserModel = new OfficeUserDetailsModel();
                MasterDropDownModel masterModel = new MasterDropDownModel();
                caller = new ServiceCaller("CommonsApiController");
                int[] array = (int[])CommonEnum.MasterDropDownEnum.GetValues(typeof(CommonEnum.MasterDropDownEnum));
                EnumDropDownListModel dropdownmodel = new EnumDropDownListModel();
                dropdownmodel.DropdownListToFill = array;
                masterModel = caller.PostCall<EnumDropDownListModel, MasterDropDownModel>("FillMasterDropDownModel", dropdownmodel, out errorMessage);


                caller = new ServiceCaller("OfficeUserDetailsApiController");
                objOfficeUserModel = caller.GetCall<OfficeUserDetailsModel>("GetUserDetails", new { EncryptedId });


                #region Added by mayank for User Manager on 26/08/2021
                bool isDRLogin = false;
                int RoleID = KaveriSession.Current.RoleID;
                if (RoleID == (int)ECDataUI.Common.CommonEnum.RoleDetails.DR)
                {
                    isDRLogin = true;
                }
                objOfficeUserModel.isDrLogin = isDRLogin; 
                #endregion
                objOfficeUserModel.IdProofsTypeDropDown = masterModel.IdProofsTypeDropDown;
                objOfficeUserModel.CountryDropDown = masterModel.CountryDropDown;
                objOfficeUserModel.CountryID = (int)CommonEnum.Countries.India;

                objOfficeUserModel.IsForUpdate = true;
                return View("OfficeUserRegistration", objOfficeUserModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// UpdateOfficeUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns update message</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Update office user data")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UpdateOfficeUser(OfficeUserDetailsModel model)
        {
            try
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");

                if (KaveriSession.Current.RoleID == (int)ECDataUI.Common.CommonEnum.RoleDetails.DR)
                {
                    //ModelState.Remove
                }

                if (ModelState.IsValid)
                {
                    #region Validation by shubham Bhagat on 5-4-2019
                    if (!String.IsNullOrEmpty(model.LevelID))
                    {
                        if (model.LevelID == "0")
                            return Json(new { success = false, message = "Level is required." });
                        //else
                        //{
                        //    int levelID;
                        //    bool levelFlag=int.TryParse(model.LevelID, out levelID);
                        //    if (!levelFlag)
                        //    { return Json(new { success = false, message = "Please select levelID." }); }
                        //}
                    }
                    else
                    {
                        return Json(new { success = false, message = "Level is required." });
                    }
                    if (!String.IsNullOrEmpty(model.RoleID))
                    {
                        if (model.RoleID == "0")
                            return Json(new { success = false, message = "Role is required." });
                        //else
                        //{
                        //    int roleID;
                        //    bool roleFlag = int.TryParse(model.RoleID, out roleID);
                        //    if (!roleFlag)
                        //    { return Json(new { success = false, message = "Please select RoleID." }); }
                        //}
                    }
                    else
                    {
                        return Json(new { success = false, message = "Role is required." });
                    }
                    if (!String.IsNullOrEmpty(model.OfficeID))
                    {
                        if (model.OfficeID == "0")
                            return Json(new { success = false, message = "Office is required." });
                        //else
                        //{
                        //    int officeID;
                        //    bool officeFlag = int.TryParse(model.OfficeID, out officeID);
                        //    if (!officeFlag)
                        //    { return Json(new { success = false, message = "Please select OfficeID." }); }
                        //}
                    }
                    else
                    {
                        return Json(new { success = false, message = "Office is required." });
                    }

                    #endregion

                    caller = new ServiceCaller("OfficeUserDetailsApiController");
                    // For Audit Log
                    model.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                    CommonFunctions commonOBJ = new CommonFunctions();
                    model.UserIPAddress = commonOBJ.GetIPAddress();
                    UserActivationModel response = caller.PostCall<OfficeUserDetailsModel, UserActivationModel>("UpdateOfficeUser", model);
                    if (response.IsUpdatedSuccessfully == true)
                    {
                        return Json(new { success = true, message = response.ResponseMessage });
                    }
                    else
                    {
                        return Json(new { success = false, message = response.ResponseMessage });
                    }
                }
                else
                {
                    //return Json(new { success = false, message = "Updation failed" });
                    //string errorMsg = ModelState.FormatErrorMessage();
                    string errorMsg = ModelState.FormatErrorMessageInString();

                    return Json(new { success = false, message = errorMsg });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { success = false, message = "Error occured while processing your request." });

          
            }

        }

        //public ActionResult UpdateOfficeUser(OfficeUserDetailsModel model)
        //{
        //    try
        //    {
        //        ModelState.Remove("Password");
        //        ModelState.Remove("ConfirmPassword");

        //        if (ModelState.IsValid)
        //        {
        //            caller = new ServiceCaller("OfficeUserDetailsApiController");
        //            // For Audit Log
        //            model.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
        //            Boolean Status = caller.PostCall<OfficeUserDetailsModel, bool>("UpdateOfficeUser", model);
        //            if (Status == true)
        //            {
        //                return Json(new { success = true, message = "Office User Details Updated successfully" });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = "Office User Details Not Updated" });
        //            }
        //        }
        //        else {
        //            //return Json(new { success = false, message = "Updation failed" });
        //            //string errorMsg = ModelState.FormatErrorMessage();
        //            string errorMsg = ModelState.FormatErrorMessageInString();

        //            return Json(new { success = false, message = errorMsg });
        //        }
        //    }
        //    catch (Exception ) {
        //        throw ;
        //    }

        //}

        // Uncommented on 02-12-2019 by Shubham Bhagat because of User Management Changes
        /// <summary>
        /// DeleteOfficeUser
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns>return delete message</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Delete office user ")]
        public ActionResult DeleteOfficeUser(String EncryptedId)
        {
            try
            {
                caller = new ServiceCaller("OfficeUserDetailsApiController");
                OfficeUserDetailsModel viewModel = new OfficeUserDetailsModel();
                viewModel.EncryptedId = EncryptedId;
                Boolean Status = caller.PostCall<OfficeUserDetailsModel, bool>("DeleteOfficeUser", viewModel, out errorMessage);
                if (Status == true)
                {
                    return Json(new { success = true, message = "Deleted successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Deletion failed failed" });
                }
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return Json(new { success = false, message = "Error occured while processing your request." });
            }

        }

        /// <summary>
        /// LoadOfficeUserDetailsGridData
        /// </summary>
        /// <returns>return office user list</returns>
        [HttpPost]
        public ActionResult LoadOfficeUserDetailsGridData()
        {
            try
            {
                caller = new ServiceCaller("OfficeUserDetailsApiController");
                //Added by mayank on 09/08/2021
                int OfficeID = KaveriSession.Current.OfficeID;
                int LevelID = KaveriSession.Current.LevelID;
                //UserGridWrapperModel result = caller.GetCall<UserGridWrapperModel>("LoadOfficeUserDetailsGridData", null, out errorMessage);
                UserGridWrapperModel result = caller.GetCall<UserGridWrapperModel>("LoadOfficeUserDetailsGridData", new { OfficeID = OfficeID, LevelID = LevelID }, out errorMessage);

                var JsonData = Json(new { status = true, data = result.dataArray, columns = result.ColumnArray });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return Json(new { errorMessage = "Error occured while getting office user list." }, JsonRequestBehavior.AllowGet);
            }
        }

        // Uncommented on 02-12-2019 by Shubham Bhagat because of User Management Changes
        /// <summary>
        /// GetOfficeDetailsInfo
        /// </summary>
        /// <param name="officeDetailId"></param>
        /// <returns>returns office details info</returns>
        [HttpGet]
        public ActionResult GetOfficeDetailsInfo(String officeDetailId)
        {
            try
            {
                caller = new ServiceCaller("OfficeUserDetailsApiController");
                OfficeUserDetailsModel userResponseModel = new OfficeUserDetailsModel();
                userResponseModel = caller.GetCall<OfficeUserDetailsModel>("GetOfficeDetailsInfo", new { officeDetailId }, out errorMessage);
                return Json(new { userResponseModel.Office_OfficeType, userResponseModel.Office_ShortName, userResponseModel.Office_District, userResponseModel.RoleDropDown, userResponseModel.LevelDetailsDropDown }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { errorMessage = "Error occured while getting office detail info." }, JsonRequestBehavior.AllowGet);
            }
        }

        // Below coda all working fine
        //// Added by Shubham Bhagat on 8-4-2019
        //[HttpGet]
        //public ActionResult GetRoleListByLevel(String LevelID)
        //{
        //    caller = new ServiceCaller("OfficeUserDetailsApiController");
        //    OfficeUserDetailsModel userResponseModel = new OfficeUserDetailsModel();
        //    userResponseModel = caller.GetCall<OfficeUserDetailsModel>("GetRoleListByLevel", new { LevelID }, out errorMessage);
        //    return Json(new { userResponseModel.Office_OfficeType, userResponseModel.Office_ShortName, userResponseModel.Office_District, userResponseModel.RoleDropDown, userResponseModel.LevelDetailsDropDown }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// GetRoleListByLevel
        /// </summary>
        /// <param name="LevelID"></param>
        /// <returns>return role list by level</returns>
        // Added by Shubham Bhagat on 8-4-2019
        [HttpGet]
        public ActionResult GetRoleListByLevel(String LevelID)
        {
            try
            {
                caller = new ServiceCaller("OfficeUserDetailsApiController");
                OfficeUserDetailsModel userResponseModel = new OfficeUserDetailsModel();
                userResponseModel = caller.GetCall<OfficeUserDetailsModel>("GetRoleListByLevel", new { LevelID }, out errorMessage);
                return Json(new { userResponseModel.RoleDropDown, userResponseModel.OfficeNamesDropDown }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return Json(new { errorMessage="Error occured while getting role list." }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}