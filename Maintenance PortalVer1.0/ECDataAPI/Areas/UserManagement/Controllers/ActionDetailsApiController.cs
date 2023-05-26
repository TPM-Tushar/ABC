#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ActionDetailsApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for User Management module.
*/
#endregion

using CustomModels.Models.ControllerAction;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.BAL;
using ECDataAPI.Areas.UserManagement.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.UserManagement.Controllers
{


    public class ActionDetailsApiController : ApiController
    {


        /// <summary>
        /// Returns List of ControllerActionModel to populate GridView
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ActionDetailsApiController/ShowControllerActionData")]
        public IHttpActionResult ShowControllerActionData(ControllerActionViewModel model)
        {
            try
            {
                IControllerActionDetails balObj = new ControllerActionBAL();
                ControllerActionViewModel CadModel = new ControllerActionViewModel();
                CadModel = balObj.ShowControllerActionData(model);
                return Ok(CadModel);
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Returns List of ControllerActionModel to populate GridView
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ActionDetailsApiController/GetControllerActionDetails")]
        public IHttpActionResult GetControllerActionDetails(ControllerActionViewModel viewModel)
        {
            try
            {
                IControllerActionDetails balObj = new ControllerActionBAL();
                List<ControllerActionModel> CadModel = new List<ControllerActionModel>();
                CadModel = balObj.PopulateCAIDList(  viewModel);
                return Ok(CadModel);
            }
            catch (Exception )
            {

                throw ;
            }
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Inserts ControllerActionModel
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ActionDetailsApiController/InsertControllerActionData")]
        [EventApiAuditLogFilter(Description = "Inserted new controller action", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult InsertControllerActionData(ControllerActionModel viewModel)
        {
            try
            {
                IControllerActionDetails balObj = new ControllerActionBAL();
                ControllerActionModel returnValue = balObj.InsertNewControllerAction(viewModel);
                return Ok(returnValue);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        /// <summary>
        /// Gets ControllerActionModel
        /// </summary>
        /// <param name="DataModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ActionDetailsApiController/GetControllerActionModel")]
        public IHttpActionResult GetControllerActionModel(ControllerActionDataModel DataModel)
        {
            try
            {
                IControllerActionDetails balObj = new ControllerActionBAL();
                ControllerActionModel returnValue = new ControllerActionModel();
                returnValue = balObj.GetControllerActionModel(DataModel);
                return Ok(returnValue);
            }
            catch (Exception )
            {

                throw ;
            }
        }
        /// <summary>
        /// returns UpdateControllerAction Data
        /// </summary>
        /// <param name="updationModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ActionDetailsApiController/UpdateControllerActionData")]
        [EventApiAuditLogFilter(Description = "Updated controller action", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UpdateControllerActionData(ControllerActionModel updationModel)
        {
            try
            {
                IControllerActionDetails balObj = new ControllerActionBAL();
                ControllerActionModel returnValue = balObj.updateControllerActionModel(updationModel);
                return Ok(returnValue);
            }
            catch (Exception )
            {

                throw ;
            }
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Deletes ControllerActionData
        /// </summary>
        /// <param name="modelForDelete"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ActionDetailsApiController/DeleteControllerActionData")]
        [EventApiAuditLogFilter(Description = "Deleted new controller action", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DeleteControllerActionData(ControllerActionModel modelForDelete)
        {
            try
            {
                IControllerActionDetails balObj = new ControllerActionBAL();
                Boolean returnValue;
                returnValue = balObj.DeleteControllerData(modelForDelete.EncryptedID);
                return Ok(returnValue);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        /// <summary>
        /// returns ControllerActionLists
        /// </summary>
        /// <param name="DataModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ActionDetailsApiController/GetControllerActionLists")]
        public IHttpActionResult GetControllerActionLists(ControllerActionDataModel DataModel)
        {
            try
            {
                IControllerActionDetails balObj = new ControllerActionBAL();
                ControllerActionModel returnValue = new ControllerActionModel();
              
                if(DataModel.ControllerName!=null)
                    returnValue.ActionList = balObj.GetActionList(DataModel.AreaList, DataModel.AreaName, DataModel.ControllerName);
                else
                    returnValue.ControllerList = balObj.GetControllerList(DataModel.AreaList, DataModel.AreaName);
                return Ok(returnValue);
            }
            catch (Exception )
            {

                throw ;
            }
        }

        /// <summary>
        /// returns ControllerActionLists
        /// </summary>
        /// <param name="DataModel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ActionDetailsApiController/GetRoleAuthView")]
        public IHttpActionResult GetRoleAuthView(string EncryptedID)
        {
            try
            {
                IControllerActionDetails balObj = new ControllerActionBAL();
                ControllerActionModel returnValue = new ControllerActionModel();

           
                    returnValue = balObj.GetRoleAuthView(EncryptedID);
                
                return Ok(returnValue);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// returns ControllerActionLists
        /// </summary>
        /// <param name="DataModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ActionDetailsApiController/UpdateRoleActionAuth")]
        public IHttpActionResult UpdateRoleActionAuth(ControllerActionModel model)
        {
            try
            {
                IControllerActionDetails balObj = new ControllerActionBAL();
                ControllerActionModel returnValue = new ControllerActionModel();


                returnValue = balObj.UpdateRoleActionAuth(model);

                return Ok(returnValue);
            }
            catch (Exception)
            {

                throw;
            }
        }

        




    }


}
