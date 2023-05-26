using CustomModels.Models.Common;
using CustomModels.Models.LogAnalysis.ValuationDifferenceReport;
using ECDataAPI.Areas.LogAnalysis.DAL;
using ECDataAPI.Areas.LogAnalysis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.LogAnalysis.BAL
{
    public class ValuationDifferenceReportBAL : IValuationDifferenceReport
    {

        public ValuationDiffReportViewModel ValuationDifferenceReportView(int OfficeID)
        {
            return new ValuationDifferenceReportDAL().ValuationDifferenceReportView(OfficeID);
        }
        public ValuationDiffReportDataModel GetValuationDiffRptData(ValuationDiffReportViewModel model)
        {
            return new ValuationDifferenceReportDAL().GetValuationDiffRptData(model);
        }
        public ValuationDiffReportDataModel GetValuationDiffDetailedData(ValuationDiffReportViewModel model)
        {
            return new ValuationDifferenceReportDAL().GetValuationDiffDetailedData(model);
        }

        public FileDisplayModel GetValuationDocument(ValuationDiffFileModel reqModel)
        {
            return new ValuationDifferenceReportDAL().GetValuationDocument(  reqModel);
        }
         

    }
}