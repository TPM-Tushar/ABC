using CustomModels.Models.MISReports.OtherDepartmentStatus;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class OtherDepartmentStatusAPIController : ApiController
    {
        IOtherDepartmentStatus balObject = null;

        [HttpGet]
        [Route("api/OtherDepartmentStatusAPIController/OtherDepartmentStatusView")]
        //[EventApiAuditLogFilter(Description = "Sale Deed Rev Collection View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult OtherDepartmentStatusView(int OfficeID)
        {
            try
            {
                return Ok(new OtherDepartmentStatusBAL().OtherDepartmentStatusView(OfficeID));
                //balObject = new OtherDepartmentStatusBAL();
                //OtherDepartmentStatusModel responseModel = new OtherDepartmentStatusModel();
                //responseModel = balObject.OtherDepartmentStatusView(OfficeID);
                //return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

     
        [HttpPost]
        [Route("api/OtherDepartmentStatusAPIController/OtherDepartmentStatusDetails")]
        //[EventApiAuditLogFilter(Description = "Get Sale Deed Rev Collection Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult OtherDepartmentStatusDetails(OtherDepartmentStatusModel model)
        {
            try
            {
                balObject = new OtherDepartmentStatusBAL();                
                List<OtherDepartmentStatusDetailsModel> responseList = new List<OtherDepartmentStatusDetailsModel>();
                responseList = balObject.OtherDepartmentStatusDetails(model);
                return Ok(responseList);
            }
            catch (Exception)
            {
                throw;
            }
        }
                    
        [HttpPost]
        [Route("api/OtherDepartmentStatusAPIController/OtherDepartmentStatusDetailsTotalCount")]
        public IHttpActionResult OtherDepartmentStatusDetailsTotalCount(OtherDepartmentStatusModel model)
        {
            try
            {
                balObject = new OtherDepartmentStatusBAL();
                int totalcount = balObject.OtherDepartmentStatusDetailsTotalCount(model);
                return Ok(totalcount);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
