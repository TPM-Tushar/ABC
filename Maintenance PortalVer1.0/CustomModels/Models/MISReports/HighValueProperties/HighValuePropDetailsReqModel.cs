using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.HighValueProperties
{
    public class HighValuePropDetailsReqModel
    {
        public int RangeID { get; set; }
        public int FinYearListID { get; set; }

        public bool IsPdf { get; set; }
        public bool IsExcel { get; set; }


    }
}
