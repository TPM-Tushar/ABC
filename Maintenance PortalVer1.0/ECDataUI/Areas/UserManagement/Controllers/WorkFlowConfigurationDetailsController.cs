#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   WorkFlowConfigurationDetailsController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for User Management module.
*/
#endregion

using CustomModels.Models.UserManagement;
using ECDataUI.Common;
using ECDataUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.UserManagement.Controllers
{
    [KaveriAuthorizationAttribute]
    public class WorkFlowConfigurationDetailsController : Controller
    {
        ServiceCaller caller = new ServiceCaller("WorkFlowConfigurationDetailsApiController");
        string errorMessage = String.Empty;

        /// <summary>
        /// ShowWorkFlowConfigurationDetails
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowWorkFlowConfigurationDetails() {
            return View("ShowWorkFlowConfigurationDetails");
        }

        /// <summary>
        /// CreateWorkFlowConfiguration
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateWorkFlowConfiguration()
        {
            WorkFlowConfigurationModel objWorkFlowConfigurationModelView = new WorkFlowConfigurationModel();
            objWorkFlowConfigurationModelView = caller.GetCall<WorkFlowConfigurationModel>("GetWorkFlowConfigurationDetails", objWorkFlowConfigurationModelView);
            objWorkFlowConfigurationModelView.IsActive = true;
            return PartialView("CreateUpdateWorkFlowConfiguration", objWorkFlowConfigurationModelView);
        }

        /// <summary>
        /// UpdateWorkFlowConfiguration
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public ActionResult UpdateWorkFlowConfiguration(String EncryptedId)
        {
            WorkFlowConfigurationModel objWorkFlowConfigurationModelView = new WorkFlowConfigurationModel();
            objWorkFlowConfigurationModelView.EncryptedId = EncryptedId;
            WorkFlowConfigurationModel objConfigurationModel = caller.GetCall<WorkFlowConfigurationModel>("GetWorkFlowConfigurationModel", objWorkFlowConfigurationModelView);
            objConfigurationModel.IsForUpdate = true;
            return View("CreateUpdateWorkFlowConfiguration", objConfigurationModel);
        }

        //[HttpPost]
        //[EventAuditLogFilter(Description = "Update Work Flow configuration data")]
        //public ActionResult UpdateWorkFlowConfiguration(WorkFlowConfigurationModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Boolean Status = caller.PostCall<WorkFlowConfigurationModel, bool>("UpdateWorkFlowConfiguration", viewModel);
        //        if (Status)
        //            return Json(new { success = true, message = "Office Configuration Updated successfully" });
        //        else
        //            return Json(new { success = false, message = "Office Configuration Not Updated" });
        //    }
        //    else {
        //        string errorMsg = ModelState.FormatErrorMessageInString();
        //        return Json(new { success = false, message = errorMsg });

        //    }

        //}

        /// <summary>
        /// UpdateWorkFlowConfiguration
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Update Work Flow configuration data")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UpdateWorkFlowConfiguration(WorkFlowConfigurationModel viewModel)
        {
            // Added by Shubham Bhagat on 26-12-2018
            ModelState.Remove("FromRoleId");
            ModelState.Remove("ToRoleId");
            if (ModelState.IsValid)
            {
                // Added by Shubham Bhagat on 26-12-2018
                viewModel.FromRoleId = viewModel.FromRoleId_Hidden;
                viewModel.ToRoleId = viewModel.ToRoleId_Hidden;
                WorkFlowConfigurationResponseModel response = caller.PostCall<WorkFlowConfigurationModel, WorkFlowConfigurationResponseModel>("UpdateWorkFlowConfiguration", viewModel);
                if (response.IsRecordUpdated)
                    return Json(new { success = true, message = response.ResponseMessage });
                else
                    return Json(new { success = false, message = "Office Configuration Not Updated" });
            }
            else
            {
                string errorMsg = ModelState.FormatErrorMessageInString();
                return Json(new { success = false, message = errorMsg });

            }

        }

        /// <summary>
        /// DeleteWorkFlowConfiguration
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Delete Work Flow configuration ")]
        public ActionResult DeleteWorkFlowConfiguration(String EncryptedId)
        {
            WorkFlowConfigurationModel viewModel = new WorkFlowConfigurationModel();
            viewModel.EncryptedId = EncryptedId;
            Boolean Status = caller.PostCall<WorkFlowConfigurationModel, bool>("DeleteWorkFlowConfiguration", viewModel,out errorMessage);
            if (Status == true)
            {
                return Json(new { success = true, message = "Deleted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed failed" });
            }

        }

        /// <summary>
        /// CreateNewWorkFlowConfiguration
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Create New Work Flow configuration")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult CreateNewWorkFlowConfiguration(WorkFlowConfigurationModel ViewModel)
        {
            try
            {
                Boolean Status = false;
                String message = String.Empty;

                // Added by Shubham Bhagat on 17-12-2018
                ModelState.Remove("FromRoleId");
                ModelState.Remove("ToRoleId");
                if (ModelState.IsValid)
                {

                    // Added by Shubham Bhagat on 17-12-2018
                    ViewModel.FromRoleId = ViewModel.FromRoleId_Hidden;
                    ViewModel.ToRoleId = ViewModel.ToRoleId_Hidden;
                    Status = caller.PostCall<WorkFlowConfigurationModel, Boolean>("CreateNewWorkFlowConfiguration", ViewModel, out errorMessage);
                    if (Status)
                        message = "Office Configuration Added SuccessFully";
                    else
                        message = "Office Configuration Not Added";

                    return Json(new { success = true, message = message });

                }
                else {
                    string errorMsg = ModelState.FormatErrorMessageInString();

                    return Json(new { success = true, message = errorMsg });

                }


            }
            catch (Exception)
            {

                throw ;
            }


        }

        /// <summary>
        /// LoadWorkFlowConfigurationGridData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadWorkFlowConfigurationGridData()
        {
            try
            {
                WorkFlowConfigurationGridWrapperModel result = caller.GetCall<WorkFlowConfigurationGridWrapperModel>("LoadWorkFlowConfigurationGridData", null, out errorMessage);
                var JsonData = Json(new { status = true, data = result.dataArray, columns = result.ColumnArray });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception)
            {

                throw ;
            }
        }

    }
}