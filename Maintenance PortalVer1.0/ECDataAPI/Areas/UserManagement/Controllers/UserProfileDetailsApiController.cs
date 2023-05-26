#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   UserProfileDetailsApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for User Management module.
*/
#endregion


using CustomModels.Models.Alerts;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.BAL;
//using KaveriUI.Models.UserRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.Controllers
{
    public class UserProfileDetailsApiController : ApiController
    {
        UserProfileDetailsBAL BALObj = null;

        /// <summary>
        /// Edits UserProfileDetails
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UserProfileDetailsApiController/EditUserProfileDetails")]
        public IHttpActionResult EditUserProfileDetails(string EncryptedID)
        {
            try
            {
                BALObj = new UserProfileDetailsBAL();
                UserProfileModel result = BALObj.EditUserProfileDetails(EncryptedID);
                return Ok(result);
            }
            catch (Exception)
            {
                throw ;
            }

        }

        /// <summary>
        /// Update UserProfileDetails
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserProfileDetailsApiController/UpdateUserProfileDetails")]
        public IHttpActionResult UpdateUserProfileDetails(UserProfileModel viewModel)
        {
            try
            {
                BALObj = new UserProfileDetailsBAL();
                UserProfileDetailsResponseModel result = BALObj.UpdateUserProfileDetails(viewModel);
                return Ok(result);

            }
            catch (Exception )
            {
                throw ;
            }

        }

        /// <summary>
        /// Save ChangedPassword
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserProfileDetailsApiController/SaveChangedPassword")]
        public IHttpActionResult SaveChangedPassword(ChangePasswordNewModel viewModel)
        {
            try
            {
                BALObj = new UserProfileDetailsBAL();
                ChangePasswordResponseModel result = BALObj.SaveChangedPassword(viewModel);
                return Ok(result);

            }
            catch (Exception )
            {
                throw ;
            }

        }

        /// <summary>
        /// Check Mobile No Availability
        /// </summary>
        /// <param name="objUserModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserProfileDetailsApiController/CheckMobileNoAvailability")]
        public IHttpActionResult CheckMobileNoAvailability(UserProfileModel objUserModel)
        {
            try
            {

                BALObj = new UserProfileDetailsBAL();
                Boolean result = BALObj.CheckMobileNoAvailability(objUserModel);
                return Ok(result);
            }
            catch (Exception )
            {
                throw ;
            }

        }

    }
}