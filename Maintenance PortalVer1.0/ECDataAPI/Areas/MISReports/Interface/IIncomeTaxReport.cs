using CustomModels.Models.MISReports.IncomeTaxReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IIncomeTaxReport
    {
        IncomeTaxReportResponseModel IncomeTaxReportView(int officeID);

        IncomeTaxReportResultModel GetIncomeTaxReportDetails(IncomeTaxReportResponseModel reqModel);

        int GetIncomeTaxReportDetailsTotalCount(IncomeTaxReportResponseModel model);
        string GetSroName(int OfficeID);


    }
}