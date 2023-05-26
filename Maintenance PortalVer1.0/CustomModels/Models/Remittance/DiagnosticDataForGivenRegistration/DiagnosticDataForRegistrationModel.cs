using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.DiagnosticDataForGivenRegistration
{
    public class DiagnosticDataForRegistrationModel
    {
        [Display(Name = "Registration Module")]
        public string RegistrationModuleCode { get; set; }
        public List<SelectListItem> RegistrationModuleList { get; set; }
       
        public int startLen { get; set; }
        public int totalNum { get; set; }

        [Required(ErrorMessage = "Final Registration No is mandatory.")]
        [Display(Name = "Final Registration Number")]
        public string FinalRegistrationNumber { get; set; }
    }

    public class DownloadDiagnosticDataScript
    {
        public byte[] FileContentField { get; set; }
        public string[] FileContent { get; set; }

        public int SROCodeForFileNameDownload { get; set; }
        public bool IsFileFetchedSuccesfully { get; set; }
        public string ErrorMessage { get; set; }

    }
}
