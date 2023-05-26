#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ECDailyReceiptReportBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.ECDailyReceiptReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ECDailyReceiptReportBAL : IECDailyReceiptReport
    {
        IECDailyReceiptReport misReportsDal = new ECDailyReceiptReportDAL();

        public ECDailyReceiptRptView ECDailyReceiptDetails(int OfficeID)
        {
            return misReportsDal.ECDailyReceiptDetails(OfficeID);

        }
        public ECDailyReceiptRptResModel GetECDailyReceiptDetails(ECDailyReceiptRptView model)
        {
            return misReportsDal.GetECDailyReceiptDetails(model);

        }
        public int GetECDailyReceiptsTotalCount(ECDailyReceiptRptView model)
        {
            return misReportsDal.GetECDailyReceiptsTotalCount(model);

        }

    }
}