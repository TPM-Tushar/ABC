#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   IARegisterAnalysisReport.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :  Interface for ARegister Analysis Report.

*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomModels.Models.Remittance.ARegisterAnalysisReport;

namespace ECDataAPI.Areas.Remittance.Interface
{
    interface IARegisterAnalysisReport
    {
        ARegisterAnalysisReportModel ARegisterAnalysisReportView(int officeID);
        ARegisterResultModel GetARegisterAnalysisReportDetails(ARegisterAnalysisReportModel aRegisterAnalysisReportModel);

        ARegisterSynchcheckResultModel GetSynchronizationCheckResult(ARegisterAnalysisReportModel aRegisterAnalysisReportModel);
    }
}
