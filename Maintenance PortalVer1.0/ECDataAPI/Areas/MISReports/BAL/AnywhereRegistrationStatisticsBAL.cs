#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   AnywhereRegistrationStatisticsBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.AnywhereRegistrationStatistics;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class AnywhereRegistrationStatisticsBAL : IAnywhereRegistrationStatistics
    {
        IAnywhereRegistrationStatistics misReportsDal = new AnywhereRegistrationStatisticsDAL();

        public AnywhereRegStatViewModel AnywhereRegistrationStatisticsView(int OfficeID)
        {
            return misReportsDal.AnywhereRegistrationStatisticsView(OfficeID);

        }
        public AnywhereRegStatResModel GetAnywhereRegStatDetails(AnywhereRegStatViewModel model)
        {
            return misReportsDal.GetAnywhereRegStatDetails(model);

        }
        //public int GetECDailyReceiptsTotalCount(ECDailyReceiptRptView model)
        //{
        //    return misReportsDal.GetECDailyReceiptsTotalCount(model);

        //}
    }
}