#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ARegisterAnalysisReportBAL.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :  BAL Layer for ARegister Analysis Report.

*/
#endregion

using CustomModels.Models.Remittance.ARegisterAnalysisReport;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Areas.Remittance.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class ARegisterAnalysisReportBAL : IARegisterAnalysisReport
    {
        ARegisterAnalysisReportDAL ARegisterAnalysisReportDAL = new ARegisterAnalysisReportDAL();
        public ARegisterAnalysisReportModel ARegisterAnalysisReportView(int officeID)
        {
            return ARegisterAnalysisReportDAL.ARegisterAnalysisReportView(officeID);
        }

        public ARegisterResultModel GetARegisterAnalysisReportDetails(ARegisterAnalysisReportModel aRegisterAnalysisReportModel)
        {
            return ARegisterAnalysisReportDAL.GetARegisterAnalysisReportDetails(aRegisterAnalysisReportModel);
        }

        public ARegisterSynchcheckResultModel GetSynchronizationCheckResult(ARegisterAnalysisReportModel aRegisterAnalysisReportModel)
        {
            return ARegisterAnalysisReportDAL.GetSynchronizationCheckResult(aRegisterAnalysisReportModel);
        }
    }
}