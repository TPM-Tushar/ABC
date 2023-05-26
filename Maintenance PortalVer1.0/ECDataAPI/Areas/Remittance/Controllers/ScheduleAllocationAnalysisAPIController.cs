using CustomModels.Models.Remittance.ScheduleAllocationAnalysis;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;

using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class ScheduleAllocationAnalysisAPIController : ApiController
    {
        IScheduleAllocationAnalysis balObject = null;

        [HttpGet]
        [Route("api/ScheduleAllocationAnalysisAPIController/ScheduleAllocationAnalysisView")]
        public IHttpActionResult ScheduleAllocationAnalysisView(int officeID)
        {
            try
            {
                balObject = new ScheduleAllocationAnalysisBAL();
                ScheduleAllocationAnalysisResponseModel resModel = balObject.ScheduleAllocationAnalysisView(officeID);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/ScheduleAllocationAnalysisAPIController/GetScheduleAllocationAnalysisDetailsTotalCount")]
        public IHttpActionResult GetScheduleAllocationAnalysisDetailsTotalCount(ScheduleAllocationAnalysisResponseModel model)
        {
            try
            {
                balObject = new ScheduleAllocationAnalysisBAL();
                int totalCount = balObject.GetScheduleAllocationAnalysisDetailsTotalCount(model);
                return Ok(totalCount);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/ScheduleAllocationAnalysisAPIController/GetScheduleAllocationAnalysisDetails")]
        [EventApiAuditLogFilter(Description = "Get Schedule Allocation Analysis Reports Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetScheduleAllocationAnalysisDetails(ScheduleAllocationAnalysisResponseModel model)
        {
            try
            {
                balObject = new ScheduleAllocationAnalysisBAL();
                ScheduleAllocationAnalysisResponseModel responseModel = new ScheduleAllocationAnalysisResponseModel();
                //List<IncomeTaxReportDetailsModel> IncomeTaxReportDetailsList = new List<IncomeTaxReportDetailsModel>();
                ScheduleAllocationAnalysisResultModel resultModel = new ScheduleAllocationAnalysisResultModel();
                resultModel = balObject.GetScheduleAllocationAnalysisDetails(model);

                return Ok(resultModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/ScheduleAllocationAnalysisAPIController/GetSroName")]
        public IHttpActionResult GetSroName(int SROfficeID)
        {
            try
            {
                balObject = new ScheduleAllocationAnalysisBAL();
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