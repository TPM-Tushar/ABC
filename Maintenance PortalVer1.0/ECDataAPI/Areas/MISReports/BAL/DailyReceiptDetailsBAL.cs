#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DailyReceiptDetailsBAL.cs
    * Author Name       :   Raman Kalegaonkar  
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion



using CustomModels.Models.MISReports.DailyReceiptDetails;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class DailyReceiptDetailsBAL: IDailyReceiptDetails
    {
        IDailyReceiptDetails misReportsDal = new DailyReceiptDetailsDAL();

        /// <summary>
        /// Returns DailyReceiptDetailsViewModel Required to show DailyReceiptDetails View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>

        public DailyReceiptDetailsViewModel DailyReceiptDetails(int OfficeID)
        {
            return misReportsDal.DailyReceiptDetails(OfficeID);

        }

        /// <summary>
        /// Returns Total Count of DailyReceiptDetails List
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public int GetDailyReceiptsTotalCount(DailyReceiptDetailsViewModel model)
        {
            return misReportsDal.GetDailyReceiptsTotalCount(model);

        }

        /// <summary>
        /// Returns DailyReceiptDetailsResModel required to display Data Grid
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public DailyReceiptDetailsResModel GetDailyReceiptTableData(DailyReceiptDetailsViewModel model)
        {
            return misReportsDal.GetDailyReceiptTableData(model);

        }
    }
}