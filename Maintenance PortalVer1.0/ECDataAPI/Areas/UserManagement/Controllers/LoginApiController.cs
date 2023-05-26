

#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   LoginApiController.cs
    * Author Name       :   ECDataPortal
    * Creation Date     :   14-04-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Api Controller for user registration related services to call Methods from BAL .
*/
#endregion



#region references
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.BAL;
using ECDataAPI.Areas.UserManagement.Interface; 
#endregion

namespace ECDataAPI.Controllers
{
    public class LoginApiController : ApiController
    {
        /// <summary>
        /// Api method returns LoginResponseModel object if logged in successfully.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/LoginApiController/UserLogin")]
        public IHttpActionResult UserLogin(LoginViewModel model)
        {
            try
            {
                    IUserDetail balObj = new UserDetailBAL();
                    LoginResponseModel result = balObj.UserLogin(model);
                        return Ok(result);
            }
            catch (Exception )
            {

                throw ;
            }

        }
    }
}
