using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.AnywhereECLog
{
    public class AnywhereECLogResModel
    {
        public List<AnywhereECLogDetailsModel> AnywhereECDetailsList { get; set; }
        public int TotalRecords { get; set; }

    }
    public class AnywhereECLogDetailsModel
    {
        public int SerialNo { get; set; }
        public string ApplicationNo { get; set; }
        public string SROfficeAppNo { get; set; }
        public string UserName { get; set; }
        public string Desc { get; set; }
        public string LogDateTime { get; set; }
        public string ApplicationFilingDate { get; set; }


    }
}
