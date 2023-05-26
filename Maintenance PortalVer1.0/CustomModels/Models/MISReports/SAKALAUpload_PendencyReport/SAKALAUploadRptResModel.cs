using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.SAKALAUpload_PendencyReport
{
    public class SAKALAUploadRptResModel
    {
        public List<SakalaUploadRptRecModel> SakalaUploadRecordList { get; set; }
        public IEnumerable<SakalaUploadRptRecModel> ISakalaUploadRecordList { get; set; }
        public int TotalCount { get; set; }
        public int FilteredRecCount { get; set; }
    }

    public class SakalaUploadRptRecModel
    {
        public int SerialNo { get; set; }
        public string District { get; set; }
        public string SRO { get; set; }
        public string OfficeName { get; set; }

        public String RegistrationNumber { get; set; }
        public String PendingNumber { get; set; }

        public String GSCNumber { get; set; }
        public String ApplicationStage { get; set; }

        public String ExportedXML { get; set; }
        public String ExportedXMLBtn { get; set; }

        public String ProcessingStatus { get; set; }

        public String TransferDateTime { get; set; }
        public String WhetherDocRegd { get; set; }
        public String WhetherDocDelivered { get; set; }
        public String WhetherDocPending { get; set; }
        public String PendingReason { get; set; }

        public String RegNumPenNum { get; set; }





    }
}
