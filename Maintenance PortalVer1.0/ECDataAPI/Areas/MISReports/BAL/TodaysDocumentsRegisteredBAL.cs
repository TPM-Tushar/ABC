#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   TodaysDocumentsRegisteredBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.IndexIIReports;
using CustomModels.Models.MISReports.TodaysDocumentsRegistered;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class TodaysDocumentsRegisteredBAL : ITodaysDocumentsRegistered
    {
        ITodaysDocumentsRegistered todaysDocumentsRegisteredDAL = new TodaysDocumentsRegisteredDAL();


        /// <summary>
        /// Returns TodaysDocumentsRegisteredReqModel required to show TodaysDocumentsRegisteredView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public TodaysDocumentsRegisteredReqModel TodaysDocumentsRegisteredView(int OfficeID)
        {
            return todaysDocumentsRegisteredDAL.TodaysDocumentsRegisteredView(OfficeID);

        }

        /// <summary>
        /// Returns TodaysTotalDocsRegDetailsTable to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TodaysTotalDocsRegDetailsTable GetTodaysTotalDocumentsRegisteredDetails(TodaysDocumentsRegisteredReqModel model)
        {
            return todaysDocumentsRegisteredDAL.GetTodaysTotalDocumentsRegisteredDetails(model);

        }

    }
}