#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IDailyRevenue.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.DailyRevenue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
   public interface IDailyRevenue
    {
        DailyRevenueReportReqModel DailyRevenueReport(int OfficeID);
        List<DailyRevenueReportDetailModel> DailyRevenueReportDetails(DailyRevenueReportReqModel model);
        int DailyRevenueReportDetailsTotalCount(DailyRevenueReportReqModel model);


        List<DailyRevenueReportDetailModel> DailyRevenueReportDetailsDayWise(DailyRevenueReportReqModel model);
        int DailyRevenueReportDetailsTotalCountDayWise(DailyRevenueReportReqModel model);

        List<DailyRevenueReportDetailModel> LoadDailyRevenueReportTblMonthWise(DailyRevenueReportReqModel model);
        int DailyRevenueReportDetailsTotalCountDocWise(DailyRevenueReportReqModel model);
        List<DailyRevenueReportDetailModel> LoadDailyRevenueReportTblDocWise(DailyRevenueReportReqModel model);


    }
}
