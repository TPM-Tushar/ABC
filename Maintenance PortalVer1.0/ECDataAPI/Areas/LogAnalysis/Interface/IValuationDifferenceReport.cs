using CustomModels.Models.Common;
using CustomModels.Models.LogAnalysis.ValuationDifferenceReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.LogAnalysis.Interface
{
    interface IValuationDifferenceReport
    {
        ValuationDiffReportDataModel GetValuationDiffRptData(ValuationDiffReportViewModel model);
        ValuationDiffReportDataModel GetValuationDiffDetailedData(ValuationDiffReportViewModel model);
        FileDisplayModel GetValuationDocument(ValuationDiffFileModel reqModel);

    }
}
