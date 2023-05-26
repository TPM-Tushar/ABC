using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.PendingDocumentsSummary
{
    public class PendingDocsSummaryResModel
    {
        public List<PendingDocsDatatableRecord> PendingDocsDatatableRecList { get; set; }
        public IEnumerable<PendingDocsDatatableRecord> IPendingDocsDatatableRecList { get; set; }

        public int TotalCount { get; set; }
        public int FilteredRecordsCount { get; set; }

        // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021

        //public int TotalDocsNotRegdNotPending { get; set; }
        public int TotalNumberOfPendingLaterFinalizedDocs { get; set; }
        // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021

        public int TotalNoOfDocsPresented { get; set; }
        public int TotalNoOfDocsRegistered { get; set; }
        public int TotalNoOfDocsKeptPending { get; set; }
    }
}
