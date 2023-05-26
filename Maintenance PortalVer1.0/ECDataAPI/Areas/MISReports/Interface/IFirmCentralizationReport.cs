using CustomModels.Models.MISReports.FirmCentralizationReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IFirmCentralizationReport
    {
        FirmCentralizationReportViewModel FirmCentralizationReportView(FirmCentralizationReportViewModel firmCentralizationReportViewModel);
        FirmCentralizationReportResultModel GetFirmCentralizationDetails(FirmCentralizationReportViewModel firmCentralizationReportViewModel);
    }
}
