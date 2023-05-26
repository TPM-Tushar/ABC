using CustomModels.Models.MenuHelper;
using ECDataAPI.BAL;
using ECDataAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Controllers
{
    public class MenuHelperApiController : ApiController
    {
        IMenuHelper InterfaceReference = new MenuHelperBAL();

        [HttpGet]
        [Route("api/MenuHelperApiController/PopulateMenu")]
        public IHttpActionResult PopulateMenu(Int16 roleID, long userId)
        {
            try
            {
                List<MenuItems> MenuList = InterfaceReference.PopulateMenu(roleID, userId);
                return Ok(MenuList);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        [Route("api/MenuHelperApiController/GetSubMenuDetails")]
        public IHttpActionResult GetSubMenuDetails(int ParentMenuID, int RoleID)
        {
            try
            {
                MenuItems obj = InterfaceReference.GetSubMenuDetails(ParentMenuID, RoleID);
                return Ok(obj);
            }
            catch (Exception)
            {

                throw;
            }
        }



    }
}
