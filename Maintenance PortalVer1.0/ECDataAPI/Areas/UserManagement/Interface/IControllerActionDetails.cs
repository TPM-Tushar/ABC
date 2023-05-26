using CustomModels.Models.ControllerAction;
using CustomModels.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.Interface
{
    interface IControllerActionDetails
    {
        ControllerActionViewModel ShowControllerActionData(ControllerActionViewModel viewModel);
        List<ControllerActionModel> PopulateCAIDList(ControllerActionViewModel viewModel);
        ControllerActionModel InsertNewControllerAction(ControllerActionModel model);
        ControllerActionModel GetControllerActionModel(ControllerActionDataModel DataModel);
        ControllerActionModel updateControllerActionModel(ControllerActionModel updateModel);
        Boolean DeleteControllerData(String EncryptedID);
        List<SelectListItem> GetRoleData(int[] array = null);
        List<SelectListItem> GetControllerList(List<KaveriArea> areaList, String areaName = "");
        List<SelectListItem> GetActionList(List<KaveriArea> areaList, String areaName = "", String controllerName = "");

        ControllerActionModel UpdateRoleActionAuth(ControllerActionModel model);
        ControllerActionModel GetRoleAuthView(string EncryptedID);
    }
}
