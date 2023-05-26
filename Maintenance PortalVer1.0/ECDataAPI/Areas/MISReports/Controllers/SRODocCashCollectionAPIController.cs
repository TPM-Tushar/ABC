#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   SRODocCashCollectionAPIController.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.SRODocCashCollection;
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
    public class SRODocCashCollectionAPIController : ApiController
    {
        SRODocCashCollectionBAL balObject = null;
        /// <summary>
        /// Returns SRODocCashCollectionResponseModel to show SRODocCashCollectionView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SRODocCashCollectionAPIController/SRODocCashCollectionView")]
        [EventApiAuditLogFilter(Description = "SRO Doc Cash Collection View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult SRODocCashCollectionView(int OfficeID)
        {
            try
            {
                balObject = new SRODocCashCollectionBAL();
                SRODocCashCollectionResponseModel responseModel = new SRODocCashCollectionResponseModel();

                responseModel = balObject.SRODocCashCollectionView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Returns List of SRODocCashDetailsModel required to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SRODocCashCollectionAPIController/GetSRODocCashCollectionReportsDetails")]
        [EventApiAuditLogFilter(Description = "Get SRO Doc Cash Collection Reports Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetSRODocCashCollectionReportsDetails(SRODocCashCollectionResponseModel model)
        {
            try
            {
                balObject = new SRODocCashCollectionBAL();
                SRODocCashCollectionResponseModel responseModel = new SRODocCashCollectionResponseModel();
                List<SRODocCashDetailsModel> IndexIIReportsDetailsList = new List<SRODocCashDetailsModel>();
                IndexIIReportsDetailsList = balObject.GetSRODocCashCollectionReportsDetails(model);

                return Ok(IndexIIReportsDetailsList);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Returns totalCount of List of SRODocCashDetailsModel 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SRODocCashCollectionAPIController/GetSRODocCashCollectionReportsDetailsTotalCount")]
        public IHttpActionResult GetSRODocCashCollectionReportsDetailsTotalCount(SRODocCashCollectionResponseModel model)
        {
            try
            {
                balObject = new SRODocCashCollectionBAL();
                int totalCount = balObject.GetSRODocCashCollectionReportsDetailsTotalCount(model);
                return Ok(totalCount);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
