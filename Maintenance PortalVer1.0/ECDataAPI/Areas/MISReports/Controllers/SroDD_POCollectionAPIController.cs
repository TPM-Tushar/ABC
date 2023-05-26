#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   SroDD_POCollectionAPIController.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.SroDD_POCollection;
using ECDataAPI.Areas.MISReports.BAL;
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
    public class SroDD_POCollectionAPIController : ApiController
    {
        SroDD_POCollectionBAL balObject = null;

        /// <summary>
        /// Returns SroDD_POCollectionResponseModel required to show SroDD_POCollectionView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SroDD_POCollectionAPIController/SroDD_POCollectionView")]
        [EventApiAuditLogFilter(Description = "Sro DD PO Collection View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult SroDD_POCollectionView(int OfficeID)
        {
            try
            {
                balObject = new SroDD_POCollectionBAL();
                SroDD_POCollectionResponseModel responseModel = new SroDD_POCollectionResponseModel();

                responseModel = balObject.SroDD_POCollectionView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Returns List of SroDD_POCollectionDetailsModel Required to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SroDD_POCollectionAPIController/GetSroDD_POCollectionReportsDetails")]
        [EventApiAuditLogFilter(Description = "Get Sro DD PO Collection Reports Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetSroDD_POCollectionReportsDetails(SroDD_POCollectionResponseModel model)
        {
            try
            {
                balObject = new SroDD_POCollectionBAL();
                SroDD_POCollectionResponseModel responseModel = new SroDD_POCollectionResponseModel();
                List<SroDD_POCollectionDetailsModel> IndexIIReportsDetailsList = new List<SroDD_POCollectionDetailsModel>();
                IndexIIReportsDetailsList = balObject.GetSroDD_POCollectionReportsDetails(model);

                return Ok(IndexIIReportsDetailsList);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Returns Total Count GetSroDD_POCollectionReportsDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SroDD_POCollectionAPIController/GetSroDD_POCollectionReportsDetailsTotalCount")]
        public IHttpActionResult GetSroDD_POCollectionReportsDetailsTotalCount(SroDD_POCollectionResponseModel model)
        {
            try
            {
                balObject = new SroDD_POCollectionBAL();
                int totalCount = balObject.GetSroDD_POCollectionReportsDetailsTotalCount(model);
                return Ok(totalCount);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Returns SRoName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SroDD_POCollectionAPIController/GetSroName")]
        public IHttpActionResult GetSroName(int SROfficeID)
        {
            try
            {
                balObject = new SroDD_POCollectionBAL();
                string SroName = balObject.GetSroName(SROfficeID);
                return Ok(SroName);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
