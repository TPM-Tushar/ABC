using CustomModels.Models.Remittance.MarriageAnalysisReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    interface IMarriageAnalysisReport
    {
        MarriageAnalysisReportModel MarriageAnalysisReportModelView(int officeID);

        List<MarriageAnalysisReportTableModel> GetMarriageAnalysisReportsDetails(MarriageAnalysisReportModel reportModel);
    }
}
