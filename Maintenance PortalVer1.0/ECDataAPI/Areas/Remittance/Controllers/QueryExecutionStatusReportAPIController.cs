#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   QueryExecutionStatusReportAPIController.cs
    * Author Name       :   Pankaj Sakhare
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.QueryExecutionStatusReport;
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
    public class QueryExecutionStatusReportAPIController : ApiController
    {
        IQueryExecutionStatusReport balObj = null;


        /// <summary>
        /// QueryExecutionStatusReportView
        /// </summary>
        /// <param name="officeid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/QueryExecutionStatusReportAPIController/QueryExecutionStatusReportView")]
        [EventApiAuditLogFilter(Description = "Get QueryExecutionStatusReportView model", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult QueryExecutionStatusReportView(int officeid)
        {
            try
            {
                balObj = new QueryExecutionStatusReportBAL();
                return Ok(balObj.QueryExecutionStatusReportView());
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GetQueryExecutionStatusReport
        /// </summary>
        /// <param name="QueryExecutionStatusReportModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/QueryExecutionStatusReportAPIController/GetQueryExecutionStatusReport")]
        [EventApiAuditLogFilter(Description = "Get Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetQueryExecutionStatusReport(QueryExecutionStatusReportModel viewModel)
        {
            try
            {
                balObj = new QueryExecutionStatusReportBAL();

                return Ok(balObj.GetQueryExecutionStatusReport(viewModel));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}