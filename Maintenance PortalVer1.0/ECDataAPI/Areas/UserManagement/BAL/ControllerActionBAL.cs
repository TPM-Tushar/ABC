#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ControllerActionBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for User Management module.
*/
#endregion

using CustomModels.Models.ControllerAction;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.DAL;
using ECDataAPI.Areas.UserManagement.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.BAL
{
    public class ControllerActionBAL: IControllerActionDetails
    {
        IControllerActionDetails dalObj = new ControllerActionDAL();

        /// <summary>
        /// Gets ControllerActionModel
        /// </summary>
        /// <param name="DataModel"></param>
        /// <returns></returns>
        /// 

        public ControllerActionViewModel ShowControllerActionData(ControllerActionViewModel viewModel)
        {
            return dalObj.ShowControllerActionData(viewModel);
        }


        /// <summary>
        /// Gets ControllerActionModel
        /// </summary>
        /// <param name="DataModel"></param>
        /// <returns></returns>
        /// 

        public ControllerActionModel GetControllerActionModel(ControllerActionDataModel DataModel)
        {
            return dalObj.GetControllerActionModel(DataModel);
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Insert NewControllerAction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ControllerActionModel InsertNewControllerAction(ControllerActionModel model)
        {

            return dalObj.InsertNewControllerAction(model);
        }
        #endregion

        /// <summary>
        /// Populates CAIDList
        /// </summary>
        /// <returns></returns>
        public List<ControllerActionModel> PopulateCAIDList(ControllerActionViewModel viewModel)
        {
            return dalObj.PopulateCAIDList(viewModel);
        }

        /// <summary>
        /// updates ControllerActionModel
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        public ControllerActionModel updateControllerActionModel(ControllerActionModel updateModel)
        {
            return dalObj.updateControllerActionModel(updateModel);
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Deletes ControllerData
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        public Boolean DeleteControllerData(String EncryptedID)
        {
            return dalObj.DeleteControllerData(EncryptedID);
        }
        #endregion

        /// <summary>
        /// Gets RoleData
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public List<SelectListItem> GetRoleData(int[] array = null)
        {
            return (dalObj.GetRoleData());
        }

        /// <summary>
        /// Gets ControllerList
        /// </summary>
        /// <param name="areaList"></param>
        /// <param name="areaName"></param>
        /// <returns></returns>
        public List<SelectListItem> GetControllerList(List<KaveriArea> areaList, string areaName = "")
        {
            return dalObj.GetControllerList(areaList, areaName);
        }

        /// <summary>
        /// Gets ActionList
        /// </summary>
        /// <param name="areaList"></param>
        /// <param name="areaName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public List<SelectListItem> GetActionList(List<KaveriArea> areaList, string areaName = "", string controllerName = "")
        {
            return dalObj.GetActionList(areaList, areaName,controllerName);
        }


        public ControllerActionModel GetRoleAuthView(string EncryptedID)
        {
            return dalObj.GetRoleAuthView(EncryptedID);
        }


        public ControllerActionModel UpdateRoleActionAuth(ControllerActionModel model)
        {
            return dalObj.UpdateRoleActionAuth(model);
        }

    
}
}
