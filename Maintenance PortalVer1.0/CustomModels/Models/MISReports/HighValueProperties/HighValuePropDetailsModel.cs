using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.HighValueProperties
{
   public class HighValuePropDetailsModel
    {

        public int SerialNo { get; set; }
        public string FinYear { get; set; }
        public decimal SD { get; set; }
        public decimal RF { get; set; }
        public int DC { get; set; }

        public string str_SD { get; set; }

        public string str_RF { get; set; }
        public string MonthName { get; set; }

        public Decimal Total { get; set; }





    }
}
