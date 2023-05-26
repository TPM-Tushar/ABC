#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   PendingDocumentsSummaryBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.PendingDocumentsSummary;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class PendingDocumentsSummaryBAL : IPendingDocsSummary
    {
        IPendingDocsSummary misReportsDal = new PendingDocumentsSummaryDAL();

        /// <summary>
        /// Returns PendingDocSummaryViewModel Required to show Pending Documents Summary View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public PendingDocSummaryViewModel PendingDocumentsSummaryView(int OfficeID)
        {
            return misReportsDal.PendingDocumentsSummaryView(OfficeID);

        }


        /// <summary>
        /// Returns PendingDocsSummaryResModel Required to show Pending Document Summary Data Table
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public PendingDocsSummaryResModel LoadPendingDocumentSummaryDataTable(PendingDocSummaryViewModel model)
        {
            return misReportsDal.LoadPendingDocumentSummaryDataTable(model);

        }
        //public int GetECDailyReceiptsTotalCount(ECDailyReceiptRptView model)
        //{
        //    return misReportsDal.GetECDailyReceiptsTotalCount(model);

        //}


        /// <summary>
        /// Returns PendingDocSummaryViewModel Required to show Pending Document Details DataTable
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public PendingDocsSummaryDetailsResModel LoadPendingDocumentDetailsDataTable(PendingDocSummaryViewModel model)
        {
            return misReportsDal.LoadPendingDocumentDetailsDataTable(model);

        }
    }
}