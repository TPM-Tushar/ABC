#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   OfficeDetailsApiController.cs
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
    public class OfficeDetailsApiController : ApiController
    {
        /// <summary>
        /// Gets All OfficeDetailsList
        /// </summary>
        IOfficeDetails objOfficeDetailsBAL = null;
        [HttpGet]
        [Route("api/OfficeDetailsApiController/GetAllOfficeDetailsList")]
        public IHttpActionResult GetAllOfficeDetailsList(String DistrictId)
        {
            try
            {
                objOfficeDetailsBAL = new OfficeDetailsBAL();
                return Ok(objOfficeDetailsBAL.GetAllOfficeDetailsList(Convert.ToInt32(DistrictId)));
            }
            catch (Exception )
            {

                throw ;
            }
        }

        /// <summary>
        /// Gets OfficeDetails
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeDetailsApiController/GetOfficeDetails")]
        public IHttpActionResult GetOfficeDetails(String EncryptedId)
        {
            try
            {
                objOfficeDetailsBAL = new OfficeDetailsBAL();
                return Ok(objOfficeDetailsBAL.GetOfficeDetails(EncryptedId));
            }
            catch (Exception )
            {

                throw ;
            }
        }

        //[HttpGet]
        //[Route("api/OfficeDetailsApiController/GetParentOfficeNameList")]
        //public IHttpActionResult GetParentOfficeNameList(int DistrictId)
        //{
        //    try
        //    {
        //        objOfficeDetailsBAL = new OfficeDetailsBAL();
        //        return Ok(objOfficeDetailsBAL.GetAllOfficeDetailsList(DistrictId));
        //    }
        //    catch (Exception e)
        //    {

        //        throw e;
        //    }
        //}

        /// <summary>
        /// Get ParentOfficeNameList
        /// </summary>
        /// <param name="OfficeTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeDetailsApiController/GetParentOfficeNameList")]
        public IHttpActionResult GetParentOfficeNameList(int OfficeTypeId)
        {
            try
            {
                objOfficeDetailsBAL = new OfficeDetailsBAL();
                return Ok(objOfficeDetailsBAL.GetAllOfficeDetailsList(OfficeTypeId));
            }
            catch (Exception )
            {

                throw ;
            }
        }

        /// <summary>
        /// Creates New Office
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OfficeDetailsApiController/CreateNewOffice")]
        [EventApiAuditLogFilter(Description = "Create New Office", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult CreateNewOffice(OfficeDetailsModel model)
        {
            try
            {
                objOfficeDetailsBAL = new OfficeDetailsBAL();
                return Ok(objOfficeDetailsBAL.CreateNewOffice(model));
            }
            catch (Exception )
            {

                throw ;
            }
        }

        /// <summary>
        /// Update Office
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OfficeDetailsApiController/UpdateOffice")]
        [EventApiAuditLogFilter(Description = "Update Office", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UpdateOffice(OfficeDetailsModel model)
        {
            try
            {
                objOfficeDetailsBAL = new OfficeDetailsBAL();
                return Ok(objOfficeDetailsBAL.UpdateOffice(model));
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Deletes Office
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OfficeDetailsApiController/DeleteOffice")]
        [EventApiAuditLogFilter(Description = "Delete Office", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DeleteOffice(OfficeDetailsModel model)
        {
            try
            {
                objOfficeDetailsBAL = new OfficeDetailsBAL();
                return Ok(objOfficeDetailsBAL.DeleteOffice(model.EncryptedId));
            }
            catch (Exception )
            {

                throw ;
            }
        }

        /// <summary>
        /// Loads OfficeDetailsGridData
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeDetailsApiController/LoadOfficeDetailsGridData")]
        public IHttpActionResult LoadOfficeDetailsGridData()
        {
            objOfficeDetailsBAL = new OfficeDetailsBAL();
            return Ok(objOfficeDetailsBAL.LoadOfficeDetailsGridData());
        }

        /// <summary>
        /// Gets Talukas By District ID
        /// </summary>
        /// <param name="districtID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeDetailsApiController/GetTalukasByDistrictID")]
        public IHttpActionResult GetTalukasByDistrictID(short districtID)
        {
            try
            {
                var result = ApiCommonFunctions.GetTalukasByDistrictID(districtID);
                objOfficeDetailsBAL = new OfficeDetailsBAL();
                return Ok(objOfficeDetailsBAL.GetTalukasByDistrictID(districtID));

            }
            catch (Exception )
            {
                throw ;
            }

        }

    }
}
