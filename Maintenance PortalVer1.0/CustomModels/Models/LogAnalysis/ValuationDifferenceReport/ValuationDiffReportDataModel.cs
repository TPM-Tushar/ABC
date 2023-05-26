using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.LogAnalysis.ValuationDifferenceReport
{
    public class ValuationDiffReportDataModel
    {
        public int TotalRecords { get; set; }

        public List<ValuationDiffReportRecordModel> ValuationDiffReportRecList { get; set; }
        public List<ValuationDiffRptDetailedRecordModel> ValuationDiffReportDetailedRecList { get; set; }

    }

    public class ValuationDiffReportRecordModel
    {
        public String SROName { get; set; }
        public String StampDutyRecovery { get; set; }
        public String ClickForDetails { get; set; }
        public String TansactionsDone { get; set; }
        public int TansactionsDoneForExcel { get; set; }
        public String SerialNo { get; set; }
        public String Registration_Fees_Recovery__Probable_ { get; set; }
        public String Total { get; set; }


        public Decimal Registration_Fees_Recovery__Probable_ForExcel { get; set; }

        public Decimal StampDutyRecoveryForExcel { get; set; }

        public Decimal TotalForExcel { get; set; }
        public Decimal TotalOccurencesTtl { get; set; }
        public Decimal DiffInStampDutyTotal { get; set; }
        public Decimal DiffInRegFeeTotal { get; set; }
        public Decimal TotalDiffTotal { get; set; }
        public ValuationDiffReportRecordModel()
        {
            this.SROName = string.Empty;
            this.StampDutyRecovery = string.Empty;
            this.ClickForDetails = string.Empty;
            this.TansactionsDone = string.Empty;
            this.SerialNo = string.Empty;
            this.Registration_Fees_Recovery__Probable_ = string.Empty;
            this.Total = string.Empty;
        }

    }

    public class ValuationDiffRptDetailedRecordModel
    {
        public String RegistrationDate { get; set; }
        public String FinalRegistrationNumber { get; set; }
        public Decimal RegisteredPerSquareFeetRate { get; set; }
        public Decimal GuidancePerSquareFeetRate { get; set; }
        public Decimal Measurement { get; set; }
        public Decimal Consideration { get; set; }
        public Decimal RegisteredGuidanceValue { get; set; }
        public Decimal PaidStampDuty { get; set; }
        public Decimal PayableStampDuty { get; set; }
        public Decimal StampDutyDifference { get; set; }
        public String Result { get; set; }
        public String ClickToViewDocument { get; set; }
        public String AreaName { get; set; }
        public String NatureOfDocument { get; set; }
        public Decimal RegFeePaid { get; set; }
        public String Registration_dump { get; set; }
        public decimal payableRegFee { get; set; }
        public decimal RegFeeDifference { get; set; }
        public decimal TotalDifference { get; set; }
        public decimal Measurement__Guntas { get; set; }
        public decimal Registered_Per_Gunta_Rate { get; set; }
        public String Apartment_Name { get; set; }
        public decimal Super_Builtup_Area_shown_in_Document { get; set; }
        public decimal Rate_as_per_G_V_notification_01_01_2019 { get; set; }
        public decimal Total_Value_on_Super_Builtup_Area { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal Market_Value_calculated_as_per_document_at_the_time_of_Registration { get; set; }
        public decimal Payable_Stamp_Duty { get; set; }
        
        public decimal TotalPaid { get; set; }
        public decimal Difference_between_the_Two { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 6-3-2020
        public decimal Registered_Per_Square_Feet_Rate { get; set; }
        public decimal Measurement__Square_Feet_ { get; set; }   



        public ValuationDiffRptDetailedRecordModel()
        {
          this.RegistrationDate = string.Empty;
          this.FinalRegistrationNumber = string.Empty;
          this.Result = string.Empty;
          this.ClickToViewDocument = string.Empty;
          this.AreaName = string.Empty;
          this.NatureOfDocument = string.Empty;
          this.Registration_dump = string.Empty;
          this.Apartment_Name = string.Empty;

        }

    }
}
