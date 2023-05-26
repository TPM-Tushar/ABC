using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.SaleDeedRevCollection
{
   public class SaleDeedRevCollectionOuterModel
    {
        public List<SaleDeedRevCollectionDetail> SaleDeedRevCollList { get; set; }
        public decimal TotalRegFee { get; set; }
        public decimal TotalStampDuty { get; set; }
        public decimal TotalSum { get; set; }
        public int TotalDocuments { get; set; }

        public int SROCode { get; set; }

        public string FinancialYear { get; set; }


    }
}
