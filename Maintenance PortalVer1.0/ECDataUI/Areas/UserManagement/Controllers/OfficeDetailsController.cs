#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   OfficeDetailsController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for User Management module.
*/
#endregion

using ECDataUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Models.UserManagement;
using ECDataUI.Filters;
using ECDataUI.Session;
using System.Text.RegularExpressions;

namespace ECDataUI.Areas.UserManagement.Controllers
{
   [KaveriAuthorizationAttribute]
    public class OfficeDetailsController : Controller
    {
        ServiceCaller caller = new ServiceCaller("OfficeDetailsApiController");
        string errorMessage = String.Empty;

        /// <summary>
        /// Show Office View
        /// </summary>
        /// <returns>View</returns>
        public ActionResult ShowOfficeView()
        {
            // Added BY SB on 8-04-2019 to active link clicked
            KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.OfficeDetail;
            return View("ShowOfficeView");
        }

        //public ActionResult GetParentOfficeNameList(int DistrictId)
        //{
        //    try
        //    {
        //        OfficeDetailsModel objSRofficeModelView = new OfficeDetailsModel();
        //        objSRofficeModelView.DistrictId = DistrictId;
        //        OfficeDetailsModel objMISDetailsModel = caller.GetCall<OfficeDetailsModel>("GetParentOfficeNameList", objSRofficeModelView, out errorMessage);
        //        if (objMISDetailsModel.ParentOfficeList != null)
        //        {
        //            return Json(new { success = true, office = objMISDetailsModel.ParentOfficeList }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Invalid District Code." }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        /// <summary>
        /// GetParentOfficeNameList
        /// </summary>
        /// <param name="OfficeTypeId"></param>
        /// <returns>returns parent office list</returns>
        public ActionResult GetParentOfficeNameList(int OfficeTypeId)
        {
            try
            {
                OfficeDetailsModel objSRofficeModelView = new OfficeDetailsModel();
                objSRofficeModelView.OfficeTypeId = OfficeTypeId;
                OfficeDetailsModel objMISDetailsModel = caller.GetCall<OfficeDetailsModel>("GetParentOfficeNameList", objSRofficeModelView, out errorMessage);


                if (objMISDetailsModel.ParentOfficeList != null)
                {
                    return Json(new { success = true, office = objMISDetailsModel.ParentOfficeList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Invalid OfficeType Code." }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception)
            {

                throw ;
            }
        }

        /// <summary>
        /// CreateNewOffice
        /// </summary>
        /// <returns>return view</returns>
        public ActionResult CreateNewOffice()
        {
            OfficeDetailsModel objSROfficeModelView = new OfficeDetailsModel();
            OfficeDetailsModel objobjSROfficeModel = caller.GetCall<OfficeDetailsModel>("GetAllOfficeDetailsList", objSROfficeModelView);
            // Commented by shubham bhagat on 10-4-2019 requirement change
            //objobjSROfficeModel.TalukaList = new List<SelectListItem>();
            return PartialView("CreateUpdateNewOffice", objobjSROfficeModel);
        }

        /// <summary>
        /// UpdateOffice
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns>returns office data to edit</returns>
        public ActionResult UpdateOffice(String EncryptedId)
        {
            if (String.IsNullOrEmpty(EncryptedId))
            {
                EncryptedId = KaveriSession.Current.EncryptedID;
            }
            else
            {
                KaveriSession.Current.EncryptedID = EncryptedId;
            }
          
            OfficeDetailsModel objSROfficeModelView = new OfficeDetailsModel();
            objSROfficeModelView.EncryptedId = EncryptedId;
            OfficeDetailsModel objOfficeModel = caller.GetCall<OfficeDetailsModel>("GetOfficeDetails", objSROfficeModelView);
            objOfficeModel.IsForUpdate = true;
            #region Commented by shubham bhagat on 10-4-2019 requirement change
            //objOfficeModel.displayTalukaListHidden = true;
            #endregion
            return View("CreateUpdateNewOffice", objOfficeModel);
        }

        /// <summary>
        /// UpdateOffice
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns>update Office message</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "update office")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UpdateOffice(OfficeDetailsModel viewModel)
        {
            if (viewModel.OfficeTypeId == (int)CommonEnum.OfficeTypes.SRO)
            {
                // ModelState.Remove("TalukaId");
                #region Commented by shubham bhagat on 10-4-2019 requirement change
                //if (viewModel.TalukaId <= 0)
                //{
                //    return Json(new { success = false, message = "Taluka is required " });
                //}
                #endregion
            }

            if (ModelState.IsValid)
            {

                viewModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                CommonFunctions commonOBJ = new CommonFunctions();
                viewModel.UserIPAddress = commonOBJ.GetIPAddress();
                if (viewModel.IsAnyFirmRegisteredForCurrentOffice == false)
                {
                    OfficeDetailsModel officeDetailsResponseModel = caller.PostCall<OfficeDetailsModel, OfficeDetailsModel>("UpdateOffice", viewModel);
                    return Json(new { success = officeDetailsResponseModel.ResponseStatus, message = officeDetailsResponseModel.ResponseMessage });
                }
                else { return Json(new { success = false, message = "Unable to update office details" }); }
            }
            else
            {

                string errorMsg = ModelState.FormatErrorMessageInString();

                return Json(new { success = false, message = errorMsg });

            }



        }

        /// <summary>
        /// DeleteOffice
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns>Delete Office message</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Delete office")]
        public ActionResult DeleteOffice(String EncryptedId)
        {
            OfficeDetailsModel viewModel = new OfficeDetailsModel();
            viewModel.EncryptedId = EncryptedId;
            Boolean Status = caller.PostCall<OfficeDetailsModel, bool>("DeleteOffice", viewModel);
            if (Status == true)
            {
                return Json(new { success = true, message = "Office Deleted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed failed" });
            }

        }

        [HttpPost]
        [EventAuditLogFilter(Description = "Create new office")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult CreateNewOffice(OfficeDetailsModel ViewModel)
        {
            try
            {
              //  Boolean Status = false;
                String message = String.Empty;
                ViewModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                OfficeDetailsModel objSRofficeModelView = new OfficeDetailsModel();
                OfficeDetailsModel officeDetailResponseModel = null;

                #region Commented by shubham bhagat on 10-4-2019 requirement change
                //if (ViewModel.OfficeTypeId == (int)CommonEnum.OfficeTypes.SRO)
                //{
                //    // ModelState.Remove("TalukaId");
                //    if (ViewModel.TalukaId <= 0)
                //    {
                //        return Json(new { success = false, message = "Taluka is required " });
                //    }
                //}
                #endregion


                if (ModelState.IsValid)
                {
                    officeDetailResponseModel = caller.PostCall<OfficeDetailsModel, OfficeDetailsModel>("CreateNewOffice", ViewModel, out errorMessage);
                    if (officeDetailResponseModel.ResponseStatus)
                        message = officeDetailResponseModel.ResponseMessage;
                    else
                        message = officeDetailResponseModel.ResponseMessage;

                    return Json(new { success = officeDetailResponseModel.ResponseStatus,  message });

                }
                else {

                    string errorMsg = ModelState.FormatErrorMessageInString();

                    return Json(new { success = false, message = errorMsg });

                }


            }
            catch (Exception )
            {

                throw ;
            }


        }

        /// <summary>
        /// LoadOfficeDetailsGridData
        /// </summary>
        /// <returns>returns office list</returns>
        [HttpPost]
        public ActionResult LoadOfficeDetailsGridData()
        {
            try
            {
                OfficeGridWrapperModel result = caller.GetCall<OfficeGridWrapperModel>("LoadOfficeDetailsGridData", null, out errorMessage);
                var JsonData = Json(new { status = true, data = result.dataArray, columns = result.ColumnArray });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception )
            {

                throw ;
            }
        }


        /// <summary>
        /// GetTalukasByDistrictID
        /// </summary>
        /// <param name="districtID"></param>
        /// <returns>returns Taluka by district id</returns>
        [HttpGet]
        public ActionResult GetTalukasByDistrictID(short districtID)
        {
            // ArrayList villageList = caller.GetCall<ArrayList>("GetVillagesByDistrictID", new { districtID = districtID });

            // objFirmDetailsModel.EncryptedID = URLEncrypt.EncryptParameters(new String[] { "OfficeID=" + KaveriSession.Current.OfficeID, "FirmID=" + KaveriSession.Current.FirmID, "TokenID=" + KaveriSession.Current.KioskTokenID, "FirmApplicationTypeID=" + KaveriSession.Current.FirmApplicationTypeID });

            try
            {
                List<SelectListItem> talukasList = caller.GetCall<List<SelectListItem>>("GetTalukasByDistrictID", new { districtID }, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success = false, errorMessage }, JsonRequestBehavior.AllowGet);
                }

                if (talukasList != null)
                {


                    return Json(new { success = true, Talukas = talukasList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, response = "Invalid districtID.", errorMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }




    }
}