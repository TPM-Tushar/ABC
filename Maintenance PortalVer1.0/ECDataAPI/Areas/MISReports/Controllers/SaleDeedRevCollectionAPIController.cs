#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   SaleDeedRevCollectionAPIController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.SaleDeedRevCollection;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class SaleDeedRevCollectionAPIController : ApiController
    {
        ISaleDeedRevCollection balObject = null;
        /// <summary>
        /// Returns SaleDeedRevCollectionModel Required to show SaleDeedRevCollectionView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SaleDeedRevCollectionAPIController/SaleDeedRevCollectionView")]
        [EventApiAuditLogFilter(Description = "Sale Deed Rev Collection View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult SaleDeedRevCollectionView(int OfficeID)
        {
            try
            {
                balObject = new SaleDeedRevCollectionBAL();
                SaleDeedRevCollectionModel responseModel = new SaleDeedRevCollectionModel();

                responseModel = balObject.SaleDeedRevCollectionView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns List of SaleDeedRevCollectionDetail to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SaleDeedRevCollectionAPIController/GetSaleDeedRevCollectionDetails")]
        [EventApiAuditLogFilter(Description = "Get Sale Deed Rev Collection Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetSaleDeedRevCollectionDetails(SaleDeedRevCollectionModel model)
        {
            try
            {
                balObject = new SaleDeedRevCollectionBAL();
                SaleDeedRevCollectionOuterModel saleDeedRevCollectionOuterModel = new SaleDeedRevCollectionOuterModel();
                saleDeedRevCollectionOuterModel = balObject.GetSaleDeedRevCollectionDetails(model);

                return Ok(saleDeedRevCollectionOuterModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Returns Total Count of GetSaleDeedRevCollectionDetails List
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SaleDeedRevCollectionAPIController/GetSaleDeedRevCollectionDetailsTotalCount")]        
        public IHttpActionResult GetSaleDeedRevCollectionDetailsTotalCount(SaleDeedRevCollectionModel model)
        {
            try
            {
                balObject = new SaleDeedRevCollectionBAL();
                int totalcount = balObject.GetSaleDeedRevCollectionDetailsTotalCount(model);
                return Ok(totalcount);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
