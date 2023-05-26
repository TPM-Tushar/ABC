using CustomModels.Models.MenuHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Interface
{
    public interface IMenuHelper
    {
        List<MenuItems> PopulateMenu(Int16 roleID, long userId);
        MenuItems GetSubMenuDetails(int ParentMenuID, int RoleID);

    }
}
