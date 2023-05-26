#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   OfficeWiseDiagnosticStatusAPIController.cs
    * Author Name       :   Pankaj Sakhare
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.OfficeWiseDiagnosticStatus;
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
    public class OfficeWiseDiagnosticStatusAPIController : ApiController
    {
        IOfficeWiseDiagnosticStatus balObject = null;

        /// <summary>
        /// QueryExecutionStatusReportView
        /// </summary>
        /// <param name="officeid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeWiseDiagnosticStatusAPIController/OfficeWiseDiagnosticStatusView")]
        [EventApiAuditLogFilter(Description = "Get OfficeWiseDiagnosticStatusView model", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult QueryExecutionStatusReportView(int officeid)
        {
            try
            {
                balObject = new OfficeWiseDiagnosticStatusBAL();
                return Ok(balObject.OfficeWiseDiagnosticStatusModelView());
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OfficeWiseDiagnosticStatusAPIController/GetOfficeList")]
        [EventApiAuditLogFilter(Description = "Get Office List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetOfficeList(String OfficeType)
        {
            try
            {
                balObject = new OfficeWiseDiagnosticStatusBAL();
                //OfficeWiseDiagnosticStatusModel model = new OfficeWiseDiagnosticStatusModel();

                //model = balObject.GetOfficeList(OfficeType);
                return Ok(balObject.GetOfficeList(OfficeType));
                //return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GetOfficeWiseDiagnosticStatusDetail
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OfficeWiseDiagnosticStatusAPIController/GetOfficeWiseDiagnosticStatusDetail")]
        [EventApiAuditLogFilter(Description = "Get Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetOfficeWiseDiagnosticStatusDetail(OfficeWiseDiagnosticStatusModel viewModel)
        {
            try
            {
                balObject = new OfficeWiseDiagnosticStatusBAL();
                return Ok(balObject.GetOfficeWiseDiagnosticStatusDetail(viewModel));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GetActionDetail
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OfficeWiseDiagnosticStatusAPIController/GetActionDetail")]
        [EventApiAuditLogFilter(Description = "Get Action Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetActionDetail(DiagnosticActionDetail viewModel)
        {
            try
            {
                balObject = new OfficeWiseDiagnosticStatusBAL();
                return Ok(balObject.GetActionDetail(viewModel));
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/OfficeWiseDiagnosticStatusAPIController/ExportOfficeWiseDiagnosticStatusToExcel")]
        [EventApiAuditLogFilter(Description = "Export Office Wise Diagnostic Status To Excel", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult ExportOfficeWiseDiagnosticStatusToExcel(OfficeWiseDiagnosticStatusModel viewModel)
        {
            try
            {
                balObject = new OfficeWiseDiagnosticStatusBAL();
                return Ok(balObject.ExportOfficeWiseDiagnosticStatusToExcel(viewModel));
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// GetDiagnosticStatusDetailByActionType
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OfficeWiseDiagnosticStatusAPIController/GetDiagnosticStatusDetailByActionType")]
        [EventApiAuditLogFilter(Description = "Get Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetDiagnosticStatusDetailByActionType(OfficeWiseDiagnosticStatusModel viewModel)
        {
            try
            {
                balObject = new OfficeWiseDiagnosticStatusBAL();
                return Ok(balObject.GetDiagnosticStatusDetailByActionType(viewModel));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}