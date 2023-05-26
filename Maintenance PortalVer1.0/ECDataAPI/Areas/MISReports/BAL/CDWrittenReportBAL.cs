#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   CDWrittenReportBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.CDWrittenReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class CDWrittenReportBAL : ICDWrittenReport
    {
        ICDWrittenReport misReportsDal = new CDWrittenReportDAL();

        /// <summary>
        /// Returns PendingDocSummaryViewModel Required to show Pending Documents Summary View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public CDWrittenReportViewModel CDWrittenReportView(int OfficeID)
        {
            return misReportsDal.CDWrittenReportView(OfficeID);

        }
        public CDWrittenReportResModel LoadCDWrittenReportDataTable(CDWrittenReportViewModel model)
        {
            return misReportsDal.LoadCDWrittenReportDataTable(model);

        }



    }
}