using CustomModels.Models.MISReports.ESignConsumptionReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IESignConsumptionReport
    {
        ESignConsumptionReportViewModel ESignConsumptionReportView();

        ESignTotalConsumptionResModel GetTotalESignConsumedCount(ESignConsumptionReportViewModel requestModel);

        ESignStatusDetailsResModel LoadESignDetailsDataTable(ESignConsumptionReportViewModel requestModel);

    }
}
