using CustomModels.Models.MISReports.KOSPaymentStatusReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class KOSPaymentStatusReportBAL : IKOSPaymentStatusReport
    {
        IKOSPaymentStatusReport KOSPaymentStatusReportObj = new KOSPaymentStatusReportDAL();


        public KOSPaymentStatusRptViewModel KOSPaymentStatusReportReportView(int OfficeID)
        {
            return KOSPaymentStatusReportObj.KOSPaymentStatusReportReportView(OfficeID);

        }

        public KOSPaymentStatusRptResModel  KOSPaymentStatusReportDetails(KOSPaymentStatusRptViewModel model)
        {
            return KOSPaymentStatusReportObj.KOSPaymentStatusReportDetails(model);
        }


        public KOSPaymentStatusRptResTableModel LoadKOSPaymentStatusReportDataTable(KOSPaymentStatusRptViewModel model)
        {
            return KOSPaymentStatusReportObj.LoadKOSPaymentStatusReportDataTable(model);
        }

        public KOSPaymentStatusRptResModel PaymentPendingSince(KOSPaymentStatusRptViewModel model)
        {
            return KOSPaymentStatusReportObj.PaymentPendingSince(model);
        }



    }
}