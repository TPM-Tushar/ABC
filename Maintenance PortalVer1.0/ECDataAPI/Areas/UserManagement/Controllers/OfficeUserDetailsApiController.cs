#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   OfficeUserDetailsApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for User Management module.
*/
#endregion

using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.BAL;
using ECDataAPI.Areas.UserManagement.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.UserManagement.Controllers
{
    public class OfficeUserDetailsApiController : ApiController
    {
        IOfficeUser balObj = null;

        /// <summary>
        /// Registers User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("api/OfficeUserDetailsApiController/RegisterUser")]
        [EventApiAuditLogFilter(Description = "Office user registered", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult RegisterUser(OfficeUserDetailsModel model)
        {
            try
            {
                balObj = new OfficeUserDetailsBAL();

                UserActivationModel result = balObj.RegisterUser(model);
                return Ok(result);
            }
            catch (Exception )
            {
                throw ;

            }
        }

        /// <summary>
        /// Checks UserNameAvailability
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeUserDetailsApiController/CheckUserNameAvailability")]
        public IHttpActionResult CheckUserNameAvailability(string EncryptedId)
        {
            try
            {
                balObj = new OfficeUserDetailsBAL();
                Boolean result = balObj.CheckUserNameAvailability(EncryptedId);

                return Ok(result);
            }
            catch (Exception )
            {
                throw ;
            }

        }

        /// <summary>
        /// Updates Office User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OfficeUserDetailsApiController/UpdateOfficeUser")]
        [EventApiAuditLogFilter(Description = "Office user updated", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UpdateOfficeUser(OfficeUserDetailsModel model)
        {
            try
            {
                balObj = new OfficeUserDetailsBAL();
                return Ok(balObj.UpdateOfficeUser(model));
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public IHttpActionResult UpdateOfficeUser(OfficeUserDetailsModel model)
        //{
        //    try
        //    {
        //        balObj = new OfficeUserDetailsBAL();
        //        return Ok(balObj.UpdateOfficeUser(model));
        //    }
        //    catch (Exception )
        //    {

        //        throw ;
        //    }
        //}

        /// <summary>
        /// Deletes Office User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OfficeUserDetailsApiController/DeleteOfficeUser")]
        [EventApiAuditLogFilter(Description = "Office user deleted", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DeleteOfficeUser(OfficeUserDetailsModel model)
        {
            try
            {
                balObj = new OfficeUserDetailsBAL();
                return Ok(balObj.DeleteOfficeUser(model.EncryptedId));
            }
            catch (Exception )
            {

                throw ;
            }
        }

        /// <summary>
        /// Gets UserDetails
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeUserDetailsApiController/GetUserDetails")]
        public IHttpActionResult GetUserDetails(string EncryptedId)
        {
            try
            {
                balObj = new OfficeUserDetailsBAL();
                return Ok(balObj.GetUserDetails(EncryptedId));
            }
            catch (Exception )
            {

                throw ;
            }
        }


        /// <summary>
        /// Load Office UserDetailsGridData
        /// </summary>
        /// <returns></returns>
        #region Old COde commented by mayank on 06-08-2021
        //[HttpGet]
        //[Route("api/OfficeUserDetailsApiController/LoadOfficeUserDetailsGridData")]
        //public IHttpActionResult LoadOfficeUserDetailsGridData()
        //{
        //    try
        //    {
        //        balObj = new OfficeUserDetailsBAL();
        //        return Ok(balObj.LoadOfficeUserDetailsGridData());
        //    }
        //    catch (Exception )
        //    {

        //        throw ;
        //    }
        //} 
        #endregion
        #region New Code changed by mayank on 06-08-2021
        [HttpGet]
        [Route("api/OfficeUserDetailsApiController/LoadOfficeUserDetailsGridData")]
        public IHttpActionResult LoadOfficeUserDetailsGridData(int officeID, int LevelID)
        {
            try
            {
                balObj = new OfficeUserDetailsBAL();
                return Ok(balObj.LoadOfficeUserDetailsGridData(officeID, LevelID));
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        /// <summary>
        /// Gets OfficeUserDetailsList
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeUserDetailsApiController/GetOfficeUserDetailsList")]
        public IHttpActionResult GetOfficeUserDetailsList()
        {
            try
            {
                balObj = new OfficeUserDetailsBAL();
                return Ok(balObj.GetOfficeUserDetailsList());
            }
            catch (Exception )
            {

                throw ;
            }
        }

        /// <summary>
        /// Gets OfficeDetailsInfo
        /// </summary>
        /// <param name="officeDetailId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeUserDetailsApiController/GetOfficeDetailsInfo")]
        public IHttpActionResult GetOfficeDetailsInfo(String officeDetailId)
        {
            try
            {
                balObj = new OfficeUserDetailsBAL();
                return Ok(balObj.GetOfficeDetailsInfo(officeDetailId));
            }
            catch (Exception )
            {

                throw ;
            }
        }

        /// <summary>
        /// Gets RoleListByLevel
        /// </summary>
        /// <param name="LevelID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeUserDetailsApiController/GetRoleListByLevel")]
        public IHttpActionResult GetRoleListByLevel(String LevelID)
        {
            try
            {
                balObj = new OfficeUserDetailsBAL();
                return Ok(balObj.GetRoleListByLevel(LevelID));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
