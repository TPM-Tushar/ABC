using CustomModels.Models.DynamicDataReader;
using ECDataAPI.Areas.DynamicDataReader.BAL;
using ECDataAPI.Areas.DynamicDataReader.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.DynamicDataReader.Controllers
{
    public class DataReadingHistoryAPIController : ApiController
    {
        IDataReadingHistory balObject = null;

        /// <summary>
        /// GetQueryAnalyserReport
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DataReadingHistoryAPIController/GetDataReadingHistoryReport")]
        [EventApiAuditLogFilter(Description = "Get Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetDataReadingHistoryReport(DataReadingHistoryModel viewModel)
        {
            try
            {
                balObject = new DataReadingHistoryBAL();
                return Ok(balObject.GetDataReadingHistoryReport(viewModel));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GetDetailByQueryId
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DataReadingHistoryAPIController/GetDetailByQueryId")]
        [EventApiAuditLogFilter(Description = "Get Detail By Query Id", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetDetailByQueryId(DataReadingHistoryDetailModel model)
        {
            try
            {
                balObject = new DataReadingHistoryBAL();
                return Ok(balObject.GetDetailByQueryId(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}