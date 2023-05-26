using CustomModels.Models.MISReports.PendingDocumentsSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IPendingDocsSummary
    {
        PendingDocSummaryViewModel PendingDocumentsSummaryView(int OfficeID);
        PendingDocsSummaryResModel LoadPendingDocumentSummaryDataTable(PendingDocSummaryViewModel model);

        PendingDocsSummaryDetailsResModel LoadPendingDocumentDetailsDataTable(PendingDocSummaryViewModel model);
    }
}
