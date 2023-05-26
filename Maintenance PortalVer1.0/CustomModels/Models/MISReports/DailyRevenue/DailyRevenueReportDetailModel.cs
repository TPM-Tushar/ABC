using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.DailyRevenue
{
    public class DailyRevenueReportDetailModel
    {
        public int SRNo { get; set; }
        public string ArticleName { get; set; }
        public int Documents { get; set; }
        public decimal RegistrationFee { get; set; }
        public decimal StampDuty { get; set; }

        public string DateValue { get; set; }
        public string SelectedMonthName { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalsStampDuty { get; set; }
        public decimal TotalRegFee { get; set; }
        public decimal TotalSum { get; set; }

        public string SROName { get; set; }

        // Added by sb on 09-12-2019 
        public String FinancialYear { get; set; }

        public String FinalRegistrationNumber { get; set; }

        public String RegistrationDate { get; set; }

        public decimal PurchaseValue { get; set; }

        public decimal Total_StampDuty_RegiFee { get; set; }

        //Added by Madhusoodan on 11-05-2020
        public string districtName { get; set; }
        public string officeName { get; set; }
        
    }
}
