using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.TodaysDocumentsRegistered
{
    public class TodaysDocumentsRegisteredDetailsModel
    {
        public int SRNo { get; set; }
        public string SROName { get; set; }
        public int Documents { get; set; }
        public decimal RegistrationFee { get; set; }
        public decimal StampDuty { get; set; }
        public decimal Total { get; set; }

        public string str_RegistrationFee { get; set; }
        public string str_StampDuty { get; set; }

        public string str_Total { get; set; }
        public string District { get; set; }

        public string DistrictNameInExcel { get; set; }



    }
}
