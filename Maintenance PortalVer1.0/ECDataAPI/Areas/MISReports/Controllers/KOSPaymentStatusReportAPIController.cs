using CustomModels.Common;
using CustomModels.Models.MISReports.KOSPaymentStatusReport;
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
//using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.Controllers
{

    public class KOSPaymentStatusReportAPIController : ApiController
    {
        IKOSPaymentStatusReport balObject = null;
        
        [HttpGet]
        [Route("api/KOSPaymentStatusReportAPIController/KOSPaymentStatusReportView")]
        [EventApiAuditLogFilter(Description = "KOS Payment Status Report View")]
        public IHttpActionResult KOSPaymentStatusReportView(int OfficeID)
         {
            try
            {
                balObject = new KOSPaymentStatusReportBAL();
                KOSPaymentStatusRptViewModel ViewModel = new KOSPaymentStatusRptViewModel();

                ViewModel = balObject.KOSPaymentStatusReportReportView(OfficeID);

                
                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

       
        [HttpPost]
        [Route("api/KOSPaymentStatusReportAPIController/KOSPaymentStatusReportDetails")]
        [EventApiAuditLogFilter(Description = " KOS Payment status Datatable 1")]
        public IHttpActionResult KOSPaymentStatusReportDetails(KOSPaymentStatusRptViewModel model)
        {
            try
            {
                balObject = new KOSPaymentStatusReportBAL();
                KOSPaymentStatusRptResModel ResModel = new KOSPaymentStatusRptResModel();
                ResModel = balObject.KOSPaymentStatusReportDetails(model);

                return Ok(ResModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/KOSPaymentStatusReportAPIController/LoadKOSPaymentStatusReportDataTable")]
        [EventApiAuditLogFilter(Description = "KOS Payment Status Report Datatable 2")]
        public IHttpActionResult LoadKOSPaymentStatusReportDataTable(KOSPaymentStatusRptViewModel model)
        {
            try
            {
                balObject = new KOSPaymentStatusReportBAL();
                KOSPaymentStatusRptResTableModel ReqModel = new KOSPaymentStatusRptResTableModel();
                ReqModel = balObject.LoadKOSPaymentStatusReportDataTable(model);
                return Ok(ReqModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/KOSPaymentStatusReportAPIController/PaymentPendingSince")]
        [EventApiAuditLogFilter(Description = " Get Payment Pending Since days")]
        public IHttpActionResult PaymentPendingSince(KOSPaymentStatusRptViewModel model)
        {
            try
            {
                balObject = new KOSPaymentStatusReportBAL();
                KOSPaymentStatusRptResModel ResModel = new KOSPaymentStatusRptResModel();
                ResModel = balObject.PaymentPendingSince(model);

                return Ok(ResModel);
            }
            catch (Exception)
            {
                throw;
            }
        }



     
    }
}