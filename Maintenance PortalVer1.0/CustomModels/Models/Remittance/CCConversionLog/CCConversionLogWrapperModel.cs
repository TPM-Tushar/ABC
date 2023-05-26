using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.CCConversionLog
{
    public class CCConversionLogWrapperModel
    {
        public List<CCConversionLogDetails> CCConversionLogDetailsList { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date Required")]
        public String ToDate { get; set; }

        public bool IsErrorField { get; set; }

        public String ErrorMessageField { get; set; }

        //Added by Madhusoodan on 17-09-2020
        public List<SelectListItem> DocumentType { get; set; }
        
        [Display(Name = "Registration Type")]
        public int DocumentTypeID { get; set; }
    }

    public class CCConversionLogDetails
    {
        public int SrNo { get; set; }

        public int LogID { get; set; }

        public int UserID { get; set; }

        public String UserName { get; set; }

        public int SROCode { get; set; }

        public String FinalRegistrationNumber { get; set; }

        //Commented and Added by Madhusoodan on 29-09-2020
        //public DateTime LogDateTime { get; set; }
        public string LogDateTime { get; set; }

        public int DocumentID { get; set; }

        //Added by Madhusoodan on 07/10/2020 to add below column 
        public String IsConvertedUsingImgMagick { get; set; }

        //Added by Madhusoodan on 12-10-2020 to add below column
        public int CCID { get; set; }
    }

    public class CCConversionLogReqModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int startLen { get; set; }

        public int totalNum { get; set; }

        //Added by Madhusoodan on 18-09-2020
        public int DocumentTypeID { get; set; }

        //Added by Madhusoodan on 21-09-2020
        public bool DistinctLogs { get; set; }
    }

}
