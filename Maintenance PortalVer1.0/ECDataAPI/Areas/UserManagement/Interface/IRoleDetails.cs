using CustomModels.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.Interface
{
    interface IRoleDetails
    {
        IEnumerable<RoleDetailsModel> RoleDetailsList(short userLoggedInRole);
        RoleDetailsResponseModel AddRoleDetails(RoleDetailsModel roleDetailsModel);
        RoleDetailsModel EditRoleDetails(String EncryptedID);
        RoleDetailsResponseModel UpdateRoleDetails(RoleDetailsModel roleDetailsModel);
       RoleDetailsResponseModel DeleteRoleDetails(String EncryptedID,long UserIdForActivityLogFromSession);

        RoleDetailsModel RoleMenuMapping(String EncryptedID,short RoleIDFromSession);
        RoleDetailsModel FirstChildMenuList(RoleDetailsModel roleDetailsModel);
        RoleDetailsModel SecondChildMenuList(RoleDetailsModel roleDetailsModel);
        RoleDetailsModel GetMapUnmapButtonForSecondChildMenu(RoleDetailsModel roleDetailsModel);

        RoleDetailsModel FirstChildList_SecondChildList_BeforeParentUnmap(RoleDetailsModel roleDetailsModel);
        RoleDetailsModel MapParentMenu(RoleDetailsModel roleDetailsModel);
        RoleDetailsModel UnmapParentMenu(RoleDetailsModel roleDetailsModel);
        RoleDetailsModel UnmapParentMenuAndSubMenu(RoleDetailsModel roleDetailsModel);

        RoleDetailsModel MapFirstChildMenu(RoleDetailsModel roleDetailsModel);
        RoleDetailsModel SecondChildList_BeforeFirstChildUnmap(RoleDetailsModel roleDetailsModel);
        RoleDetailsModel UnmapFirstChildMenu(RoleDetailsModel roleDetailsModel);
        RoleDetailsModel UnmapFirstChildMenuAndSubMenu(RoleDetailsModel roleDetailsModel);

        RoleDetailsModel MapSecondChildMenu(RoleDetailsModel roleDetailsModel);
        RoleDetailsModel UnmapSecondChildMenu(RoleDetailsModel roleDetailsModel);
        List<SelectListItem> GetLevelList();

    }
}
