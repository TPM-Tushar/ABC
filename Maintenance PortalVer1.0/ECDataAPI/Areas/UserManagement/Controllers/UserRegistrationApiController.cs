
#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   UserRegistrationApiController.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Api Controller for user registration related services to call Methods from BAL .
*/
#endregion



using CustomModels.Models.Common;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.BAL;
using ECDataAPI.Areas.UserManagement.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.UserManagement.Controllers
{
    public class UserRegistrationApiController : ApiController
    {
        IUserDetail balObj = null;


        /// <summary>
        /// Api method which provide service to SAVE(Register) user details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserRegistrationApiController/RegisterUser")]
        public IHttpActionResult RegisterUser(UserModel model)
        {
            try
            {
                     balObj = new UserDetailBAL();
                    UserActivationModel result = balObj.RegisterUser(model);
                        return Ok(result);
            }
            catch (Exception )
            {
                throw ;
                //var message = string.Format("Some technical problem occured :== "+ exception.Message);
                //HttpError err = new HttpError(message);
                ////   return Request.CreateResponse(HttpStatusCode.NotFound, err);

                //// return ResponseMessage( Request.CreateResponse( HttpStatusCode.BadRequest,err));

             //   return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, message));
            }
        }

        [HttpGet]
        [Route("api/UserRegistrationApiController/CheckUserNameAvailability")]
        public IHttpActionResult CheckUserNameAvailability(string encryptedID)
        {
            try
            {

                balObj = new UserDetailBAL();


                Boolean result = balObj.CheckUserNameAvailability(encryptedID);

                return Ok(result);
            }
            catch (Exception )
            {
                throw ;
            }

        }

        /// <summary>
        ///  Api method returns UserActivationModel object(to send email) if email address is registered and user is not Activated.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserRegistrationApiController/AccountActivation")]
        public IHttpActionResult AccountActivation(UserActivationModel model)
        {
            try
            {
                     balObj = new UserDetailBAL();
                    UserActivationModel result = balObj.AccountActivation(model);
                        return Ok(result);
            }
            catch (Exception )
            {
                throw ;
            }
        }




        /// <summary>
        ///  APi method returns UserActivationModel(to send email) object if email is correct in case of ForgotPassword.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserRegistrationApiController/ForgotPassword")]
        public IHttpActionResult ForgotPassword(UserActivationModel model)
        {
            try
            {
                     balObj = new UserDetailBAL();
                    UserActivationModel result = balObj.ForgotPassword(model);
                        return Ok(result);
            }
            catch (Exception )
            {
                throw ;
            }
        }


        /// <summary>
        /// Api method returns empty string if e-mail has been sent successfully.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserRegistrationApiController/SendEmail")]
        public IHttpActionResult SendEmail(EmailModel model)
        {
            try
            {
                     balObj = new UserDetailBAL();
                    string result = balObj.SendEmail(model);
                        return Ok(result);
            }
            catch (Exception )
            {
                throw ;
            }
        }


        /// <summary>
        /// Api method returns string messages depending on  user is activated or not.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UserRegistrationApiController/AccountActivationByLink")]
        public IHttpActionResult AccountActivationByLink(string Id)
        {
            try
            {
                     balObj = new UserDetailBAL();
                    string resultMessage = balObj.AccountActivationByLink(Id);
                        return Ok(resultMessage);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Api method returns string messages depending on password has been changed or Not.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/UserRegistrationApiController/ForgotPasswordByLink")]
        public IHttpActionResult ForgotPasswordByLink(ChangePasswordModel model)
        {

            try
            {
                     balObj = new UserDetailBAL();
                    string resultMessage = balObj.ForgotPasswordByLink(model);
                    return Ok(resultMessage);
            }
            catch (Exception )
            {
                throw ;
            }
        }


    }
}
