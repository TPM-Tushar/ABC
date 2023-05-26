using CustomModels.Models.Alerts;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.AnywhereEC.BAL;
using ECDataAPI.Areas.AnywhereEC.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.AnywhereEC.Controllers
{
    public class AnywhereECForgotPasswordApiController : ApiController
    {
        IAnywhereECForgotPassword balObj = null;


        [HttpGet]
        [Route("api/AnywhereECApiController/ForgotPasswordView")]
        public IHttpActionResult SaveNewPasswordView(int userid)
        {
            try
            {
                balObj = new AnywhereECForgotPasswordBAL();
                ChangePasswordNewModel result = balObj.SaveNewPasswordView(userid);
                return Ok(result);
            }
            catch(Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/AnywhereECApiController/SaveNewPassword")]
        public IHttpActionResult SaveNewPassword(ChangePasswordNewModel viewModel)
        {
            try
            {
                balObj = new AnywhereECForgotPasswordBAL();
                ChangePasswordResponseModel result = balObj.SaveNewPassword(viewModel);
                return Ok(result);

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}