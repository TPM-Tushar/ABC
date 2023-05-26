using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.DailyReceiptDetails
{
    public class DailyReceiptDetailsResModel
    {
        public List<DailyReceiptDetailsModel> DailyReceiptsDetailsList { get; set; }
        public decimal TotalAmountSum { get; set; }
        public int TotalRecords { get; set; }
        public Decimal TotalAmount { get; set; }

    }
    public class DailyReceiptDetailsModel
    {
        public int SerialNo { get; set; }
        public String DocumentNo { get; set; }
        public String ArticleName { get; set; }
        public String FRN { get; set; }
        public int ReceiptNo { get; set; }
        public String DateOfReceipt { get; set; }

        public String Description { get; set; }
        public Decimal Amount { get; set; }
        public String MarriageCaseNumber { get; set; }
        public int NoticeNumber { get; set; }
        public String DescriptionEnglish { get; set; }

        public String PresentDateTime { get; set; }

        //public String DDChallanNo { get; set; }
        public String ChallanNo { get; set; }


        public Decimal AmountPaid { get; set; }

        public string StampType { get; set; }
        public string SroName { get; set; }

    }

}
