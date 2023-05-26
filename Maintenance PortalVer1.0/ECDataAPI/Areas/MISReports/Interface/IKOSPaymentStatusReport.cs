using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomModels.Models.MISReports.KOSPaymentStatusReport;
using ECDataAPI.Areas.MISReports.DAL;

namespace ECDataAPI.Areas.MISReports.Interface
{
      interface IKOSPaymentStatusReport
    {
        
        KOSPaymentStatusRptViewModel KOSPaymentStatusReportReportView(int OfficeID);

        KOSPaymentStatusRptResModel KOSPaymentStatusReportDetails(KOSPaymentStatusRptViewModel model);

        KOSPaymentStatusRptResTableModel LoadKOSPaymentStatusReportDataTable(KOSPaymentStatusRptViewModel model);

        KOSPaymentStatusRptResModel PaymentPendingSince(KOSPaymentStatusRptViewModel model);




    }
}