using CustomModels.Models.CDWrittenReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface ICDWrittenReport
    {
        CDWrittenReportViewModel CDWrittenReportView(int OfficeID);
        CDWrittenReportResModel LoadCDWrittenReportDataTable(CDWrittenReportViewModel model);


    }
}
