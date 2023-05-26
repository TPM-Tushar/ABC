using CustomModels.Models.Remittance.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CustomModels.Models.MISReports.SevaSidhuApplicationDetails
{
    //Added by vijay on 01-03-2023
    public class SevaSindhuApplicationDetailsReportModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int SROfficeID { get; set; }

        public int DROfficeID { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }

        public int OfficeID { get; set; }
        public int DocType { get; set; }
    }

    public class SevaSindhuApplicationDetailsReportTableModel
    {
        public int SRNO { get; set; }

        public string OfficeName { get; set; }

        public string ReferenceNo { get; set; }
        public int SROCode { get; set; }
        public string TransXML { get; set; }
        public string AknowledgementNo { get; set; }
        public System.DateTime DataReceivedDate { get; set; }
        public string AppointmentDateTime { get; set; }

        public System.DateTime AppointmentDate_Time { get; set; }

        public string AppointmentSlot { get; set; }
        public bool IsApplicationRegistered { get; set; }
        public Nullable<System.DateTime> ApplicationRegistrationDateTime { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public Nullable<long> RegistrationID { get; set; }
        public Nullable<long> NoticeID { get; set; }

        public string MaarigeType { get; set; }
        public string MarrigecaseNo { get; set; }
        public string MarriageRegistrationDate { get; set; }
        public string ApplicationRecivedDateTime { get; set; }
        public object MarriageTypeID { get; set; }
        public long RequestID { get; set; }
        public string ApplicationAcceptDateTime { get; set; }
        public string ApplicationRejectDateTime { get; set; }
        public byte? ApplicationStatusCode { get; set; }
        public string ApplicationStatus { get; set; }
        public string RejectionReason { get; set; }
    }

    public class SevaSindhuApplicationDetailsResultModel
    {
        public List<SevaSindhuApplicationDetailsReportTableModel> SevaSindhuApplicationDetailsReportTableList;


    }


}
