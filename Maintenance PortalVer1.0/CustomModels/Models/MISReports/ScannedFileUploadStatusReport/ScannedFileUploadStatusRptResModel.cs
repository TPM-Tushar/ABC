using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.ScannedFileUploadStatusReport
{
    public class ScannedFileUploadStatusRptResModel
    {
       public List<ScannedFileUploadStatusDetailsModel> ScannedFileList { get; set; }
        public String GenerationDateTime { get; set; }
        public String PDFDownloadBtn { get; set; }
        public String EXCELDownloadBtn { get; set; }


    }

    public class ScannedFileUploadStatusDetailsModel
    {
        public int SrNo { get; set; }
        public string SubRegistrarOffice { get; set; }
        public String LastUploadDateTime { get; set; }
        public String FilePendingForDays { get; set; }

        //Added by Madhusoodan on 29-04-2020
        public String RegistrationModule { get; set; }
        public string NoOfFiles { get; set; }
        public string FileSize { get; set; }
    }
}
