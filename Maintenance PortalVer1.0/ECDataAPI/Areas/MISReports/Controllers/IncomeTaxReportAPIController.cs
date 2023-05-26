using CustomModels.Models.MISReports.IncomeTaxReport;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{

    public class IncomeTaxReportAPIController : ApiController
    {

        IIncomeTaxReport balObject = null;

        [HttpGet]
        [Route("api/IncomeTaxReportAPIController/IncomeTaxReportView")]
        public IHttpActionResult IncomeTaxReportView(int officeID)
        {
            try
            {
                balObject = new IncomeTaxReportBAL();
                IncomeTaxReportResponseModel resModel = balObject.IncomeTaxReportView(officeID);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/IncomeTaxReportAPIController/GetIncomeTaxReportDetailsTotalCount")]
        public IHttpActionResult GetIncomeTaxReportDetailsTotalCount(IncomeTaxReportResponseModel model)
        {
            try
            {
                balObject = new IncomeTaxReportBAL();
                int totalCount = balObject.GetIncomeTaxReportDetailsTotalCount(model);
                return Ok(totalCount);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/IncomeTaxReportAPIController/GetIncomeTaxReportDetails")]
        [EventApiAuditLogFilter(Description = "Get Index II Reports Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetIncomeTaxReportDetails(IncomeTaxReportResponseModel model)
        {
            try
            {
                balObject = new IncomeTaxReportBAL();
                IncomeTaxReportResponseModel responseModel = new IncomeTaxReportResponseModel();
                //List<IncomeTaxReportDetailsModel> IncomeTaxReportDetailsList = new List<IncomeTaxReportDetailsModel>();
                IncomeTaxReportResultModel incomeTaxReportResultModel = new IncomeTaxReportResultModel();
                incomeTaxReportResultModel = balObject.GetIncomeTaxReportDetails(model);

                return Ok(incomeTaxReportResultModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/IncomeTaxReportAPIController/GetSroName")]
        public IHttpActionResult GetSroName(int SROfficeID)
        {
            try
            {
                balObject = new IncomeTaxReportBAL();
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