using CustomModels.Models.Remittance.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CustomModels.Models.Remittance.ChallanDataEntryCorrectionDetails
{
    //added by Vijay on 2-02-2023
    public class ChallanDataEntryCorrectionDetailsReportModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public int SROId { get; set;}
        public int LocallyUpdated { get; set; }
        public int CentrallyUpdated { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "Date Required")]
        public String FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "Date Required")]
        public String ToDate { get; set; }
        public DateTime DateTime_FromDate { get; set; }
        public DateTime DateTime_ToDate { get; set; }
    }
    public class ChallanDataEntryCorrectionDetailsReportTableModel
    {
        public long ChallanCorrectionID { get; set; }
        public string SroName { get; set; }
        public Nullable<int> SROCode { get; set; }
        public string ChallanNumber { get; set; }
        public string ChallanDate { get; set; }
        public string Reason { get; set; }
        public string ApplicationDate { get; set; }
        public long BIND_RowID { get; set; }
        public Nullable<long> DocumentID { get; set; }
        public string Old_BIND_InstrumentNumber { get; set; }
        public string Old_BIND_InstrumentDate { get; set; }
        public Nullable<long> ReceiptID { get; set; }
        public Nullable<long> StampDetailsID { get; set; }
        public Nullable<long> AnywhereECReceiptID { get; set; }
        public Nullable<int> ServiceType { get; set; }
        public string IsLocallyUpdated { get; set; }
        public string LocalDBUpdateDateTime { get; set; }
        public string IsCentrallyupdated { get; set; }
        public Nullable<System.DateTime> CentralDBUpdateDateTime { get; set; }
        public string ReceiptDDNumber { get; set; }
        public Nullable<System.DateTime> ReceiptDDDate { get; set; }
        public string ReceiptDescription { get; set; }
        public string StampDetailsDDNumber { get; set; }
        public Nullable<System.DateTime> StampDetailsDDDate { get; set; }
        public string StampDetailsDDChalNumber { get; set; }
        public string AnywhereEC_InstrumentNumber { get; set; }
        public Nullable<System.DateTime> AnywhereEC_InstrumentDate { get; set; }
        public long KAGIR_REG_UserID { get; set; }


    }

   



    public  class ChallanDataEntryCorrectionDetailsResultModel
    {
       public List<ChallanDataEntryCorrectionDetailsReportTableModel> CDECDDataTableList;
    }


}
