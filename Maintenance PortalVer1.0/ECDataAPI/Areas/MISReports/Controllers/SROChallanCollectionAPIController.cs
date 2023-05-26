using CustomModels.Models.MISReports.SROChallanCollection;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class SROChallanCollectionAPIController : ApiController
    {
        SROChallanCollectionBAL balObject = null;

  
        [HttpGet]
        [Route("api/SROChallanCollectionAPIController/SROChallanCollectionView")]
        [EventApiAuditLogFilter(Description = "Sro Challan Collection View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult SROChallanCollectionView(int OfficeID)
        {
            try
            {
                balObject = new SROChallanCollectionBAL();
                SROChallanCollectionResponseModel responseModel = new SROChallanCollectionResponseModel();

                responseModel = balObject.SROChallanCollectionView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROChallanCollectionAPIController/GetSROChallanCollectionReportsDetails")]
        [EventApiAuditLogFilter(Description = "Get Sro Challan Collection Reports Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetSROChallanCollectionReportsDetails(SROChallanCollectionResponseModel model)
        {
            try
            {
                balObject = new SROChallanCollectionBAL();
                SROChallanCollectionResponseModel responseModel = new SROChallanCollectionResponseModel();
                List<SROChallanCollectionDetailsModel> IndexIIReportsDetailsList = new List<SROChallanCollectionDetailsModel>();
                IndexIIReportsDetailsList = balObject.GetSROChallanCollectionReportsDetails(model);

                return Ok(IndexIIReportsDetailsList);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("api/SROChallanCollectionAPIController/GetSROChallanCollectionReportsDetailsTotalCount")]
        public IHttpActionResult GetSROChallanCollectionReportsDetailsTotalCount(SROChallanCollectionResponseModel model)
        {
            try
            {
                balObject = new SROChallanCollectionBAL();
                int totalCount = balObject.GetSROChallanCollectionReportsDetailsTotalCount(model);
                return Ok(totalCount);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpGet]
        [Route("api/SROChallanCollectionAPIController/GetSroName")]
        public IHttpActionResult GetSroName(int SROfficeID)
        {
            try
            {
                balObject = new SROChallanCollectionBAL();
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