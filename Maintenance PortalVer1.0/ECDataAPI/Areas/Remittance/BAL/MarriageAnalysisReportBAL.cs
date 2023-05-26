
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   MarriageAnalysisReportBAL.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL Layer for Marriage Analysis Report.

*/

using CustomModels.Models.Remittance.MarriageAnalysisReport;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class MarriageAnalysisReportBAL : IMarriageAnalysisReport
    {
        MarriageAnalysisReportDAL marriageAnalysisReportDAL = new MarriageAnalysisReportDAL();

        public List<MarriageAnalysisReportTableModel> GetMarriageAnalysisReportsDetails(MarriageAnalysisReportModel reportModel)
        {
            return marriageAnalysisReportDAL.GetMarriageAnalysisReportsDetails(reportModel);
        }

        public MarriageAnalysisReportModel MarriageAnalysisReportModelView(int officeID)
        {
           return marriageAnalysisReportDAL.MarriageAnalysisReportModelView(officeID);
        }

    }
}