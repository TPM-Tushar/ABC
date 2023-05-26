using CustomModels.Models.Remittance.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

//added by vijay on 16/02/2023

namespace CustomModels.Models.Remittance.BatchCompletionDetails
{
    public class BatchCompletionDetailsReportTableModel
    {
        public int SROCode { get; set; }
        public string SroName { get; set; }
        public string IsBatchComplete { get; set; }
        public string MaxDocID { get; set; }
        public string MaxToDocID { get; set;}
       public string RPT_DocReg_NoCLBatchDetails_MaxToDocID_Stamp5DateTime { get; set;}
        public string documentmaster_MaxDocID_Stamp5DateTime { get; set; }
        public string MarriageRegistration_MaxRegID_DateOfReg { get; set; }

        public string MaxRegistrationID { get; set; }

        public string L_Stamp5DateTime { get; set; }

        public string Is_verified { get; set; }
        public string Batchdatetime { get; set; }
        public long srNo { get; set; }

    }
    public class BatchCompletionDetailsResultModel
    {
        public List<BatchCompletionDetailsReportTableModel> BatchCompletionDetailsReportTableList;

    }
    public class BatchCompletionDetailsReportModel
    {
        /*
        [Display(Name = "Date")]
        [Required]
        public string TillDate { get; set; }
        */

        [Display(Name = "Document Type")]
        [Required]
        public int DocType { get; set; }    


    }
}
