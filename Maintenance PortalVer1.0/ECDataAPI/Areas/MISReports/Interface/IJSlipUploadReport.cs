using CustomModels.Models.MISReports.JSlipUploadReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IJSlipUploadReport
    {
        JSlipUploadRptViewModel JSlipUploadReportView(int OfficeID);

        JSlipUploadRptResModel LoadJSlipUploadReportDataTable(JSlipUploadRptViewModel model);

    }
}
