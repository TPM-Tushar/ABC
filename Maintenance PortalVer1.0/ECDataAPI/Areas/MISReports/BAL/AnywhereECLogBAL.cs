#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   AnywhereECLogBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.AnywhereECLog;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class AnywhereECLogBAL : IAnywhereECLog
    {
        IAnywhereECLog misReportsDal = new AnywhereECLogDAL();

        public AnywhereECLogView AnywhereECLogView(int OfficeID)
        {
            return misReportsDal.AnywhereECLogView(OfficeID);

        }
        public AnywhereECLogResModel GetAnywhereECLogDetails(AnywhereECLogView model)
        {
            return misReportsDal.GetAnywhereECLogDetails(model);

        }
        public int GetAnywhereECLogTotalCount(AnywhereECLogView model)
        {
            return misReportsDal.GetAnywhereECLogTotalCount(model);

        }
    }
}