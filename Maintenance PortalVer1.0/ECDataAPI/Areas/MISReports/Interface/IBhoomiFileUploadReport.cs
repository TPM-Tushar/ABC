using CustomModels.Models.MISReports.BhoomiFileUploadReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IBhoomiFileUploadReport
    {
        BhoomiFileUploadRptViewModel BhoomiFileUploadReportView(int OfficeID);
        BhoomiFileUploadRptResModel LoadBhoomiFileUploadReportDataTable(BhoomiFileUploadRptViewModel model);

    }
}
