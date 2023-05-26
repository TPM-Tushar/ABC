using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.ChallanMatrixXMLLog
{
    public class ChallanMatrixWrapperModel
    {
        public String OfficeType { get; set; }

        [Display(Name = "Office Name")]
        public int OfficeTypeID { get; set; }

        public List<SelectListItem> OfficeTypeList { get; set; }

        public List<ChallanMatrixXMLLogDetail> ChallanMatrixXMLLogDetailList { get; set; }

        public bool IsErrorField { get; set; }

        public string ErrorMessageField { get; set; }
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }


        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 18-05-2019 

        [Display(Name = "Request text")]
        public String Request { get; set; }

        [Display(Name = "Response text")]
        public String Response { get; set; }

    }
    public class ChallanMatrixXMLLogDetail
    {
        #region For Datatable
        public long RequestXMLID { get; set; }
        public String SROCode { get; set; }
        public String RequestDateTime { get; set; }
        public String ResponseDateTime { get; set; }
        public String IsExceptionInRequest { get; set; }
        public String RequestExceptionDetails { get; set; }
        public String IsExceptionInResponse { get; set; }
        public String ResponseExceptionDetails { get; set; }
        //public bool IsDRO { get; set; }
        //public String DROCode { get; set; }
        public String DownloadXmlBtn { get; set; }

        #endregion
    }
    public class ChallanMatrixLogRequestModel
    {
        public String OfficeType { get; set; }
        public String OfficeTypeID { get; set; }
        public bool IsDRO { get; set; }
        public int SROCode { get; set; }
        public String RequestXMLID { get; set; }
        public DateTime Datetime_FromDate { get; set; }
        public DateTime Datetime_ToDate { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 18-05-2019 

        [Display(Name = "Request")]
        public String Request { get; set; }

        [Display(Name = "Response")]
        public String Response { get; set; }
    }

    public class FileDownloadModel
    {
        public bool IsErrorField { get; set; }

        public string SErrorMsgField { get; set; }

        public string DownloadFileNameField { get; set; }

        public byte[] FileContentField { get; set; }
    }
}
