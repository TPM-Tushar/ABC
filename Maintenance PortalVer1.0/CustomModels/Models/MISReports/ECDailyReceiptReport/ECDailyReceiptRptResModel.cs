using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.ECDailyReceiptReport
{
    public class ECDailyReceiptRptResModel
    {
        public List<ECDailyReceiptDetailsModel> DailyReceiptDetailsList { get; set; }
        public Decimal TotalAmount { get; set; }
        public int TotalRecords { get; set; }


    }

    public class ECDailyReceiptDetailsModel
    {
        public int SrNo { get; set; }
        public string ReceiptNo { get; set; }
        public string AppNo { get; set; }
        public string SrOfficeAppNo { get; set; }
        public string AppName { get; set; }
        public string IssuedBy { get; set; }
        public string PeriodOfSearch { get; set; }
        public string ReceiptType { get; set; }
        public string ReceiptDate { get; set; }


        public Decimal Amount { get; set; }

        //Added By ShivamB for view this columns in the ECDailyRecieptDetails Grid Table on 07-09-2022
        public string ModeOfPayment { get; set; }
        public string ChallanNumber { get; set; }
        //Added By ShivamB for view this columns in the ECDailyRecieptDetails Grid Table on 07-09-2022

    }
}
