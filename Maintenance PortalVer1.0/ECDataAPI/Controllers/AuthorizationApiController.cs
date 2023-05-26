#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   AuthorizationApiController.cs
    * Author Name       :   
    * Creation Date     :   14-04-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   
*/
#endregion

#region references
using ECDataAPI.BAL;
using ECDataAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
#endregion


namespace ECDataAPI.Controllers
{
    public class AuthorizationApiController : ApiController
    {
        ICommonInterface InterfaceReference = new CommonBAL();

        //Added by Avinash
        /// <summary>
        /// Validation role has permission or not
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="area"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns>This Method Returns boolean after Authorization of Particular User</returns>
        

        [HttpGet]
        [System.Web.Http.Route("api/AuthorizationApi/CheckUserPermissionForRole")]
        public bool CheckUserPermissionForRole(short roleID, string area, string controller, string action)
        {
            return InterfaceReference.CheckUserPermissionForRole(roleID, area, controller, action);
        } 

    }
}





