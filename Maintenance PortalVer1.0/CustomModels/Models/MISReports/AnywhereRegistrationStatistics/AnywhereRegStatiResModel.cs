using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.AnywhereRegistrationStatistics
{
    public class AnywhereRegStatResModel
    {
        public List<AnywhereRegStatDetailsModel> AnywhereRegStatList {get; set;}
        public int[,] AnywhereRegStatArray { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public Dictionary<int,string> SRODictionary { get; set; }
        public List<SelectListItem> SROList { get; set; }
        public int TDWidth { get; set; }

    }

    public class AnywhereRegStatDetailsModel
    {
        public string Jurisdiction { get; set; }
        public string RegOffice { get; set; }
        public string RegSROCode { get; set; }
        public string SROCode { get; set; }
        public string DocCount { get; set; }
        public int TotalDocsInJurdn { get; set; }


    }

    public class AnywhereRegStatTableDetails
    {
        List<int> DocCount { get; set; }

    }

}
