using CustomModels.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.Interface
{
    interface IMenuDetails
    {
        MenuDetailsResponseModel AddMenu(MenuDetailsModel menuDetailsModel);
        IEnumerable<MenuDetailsModel> RetriveMenu();
        MenuDetailsModel EditMenu(String EncryptedID);
        MenuDetailsResponseModel UpdateMenu(MenuDetailsModel menuDetailsModel);
        MenuDetailsResponseModel DeleteMenu(String EncryptedID, long UserIdForActivityLogFromSession, string IPAddress);
        //List<SelectListItem> GetRoleList();
        List<SelectListItem> GetMenuList(int menuId = 0);
        MenuDetailsModel AddMenu();
        List<SelectListItem> GetFirstChildMenuDetailsList(int parentId, int menuId = 0);
        List<SelectListItem> GetSecondChildMenuDetailsList(int firstChildMenuDetailsId, int menuId = 0);


        MenuDetailsModel MenuActionMapping(String EncryptedID);
        MenuDetailsModel ControllerList(MenuDetailsModel menuDetailsModel);
        MenuDetailsModel ActionList(MenuDetailsModel menuDetailsModel);
        MenuDetailsModel MapMenuToAction(MenuDetailsModel menuDetailsModel);
        MenuDetailsModel MapUnmapMenuActionButton(MenuDetailsModel menuDetailsModel);
        MenuDetailsModel UnmapMenuToAction(MenuDetailsModel menuDetailsModel);

    }
}
