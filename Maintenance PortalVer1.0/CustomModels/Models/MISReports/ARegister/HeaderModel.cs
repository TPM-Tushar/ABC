using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aregister.Models
{
    public class HeaderModel
    {
        public string PreviousWorkingDate { get; set; }
        public double StampDuty { get; set; }

        public double RegistrationFees { get; set; }

        public double MutationFees { get; set; }

        public double MarriageFees { get; set; }

        public double TotalDutyFees { get; set; }

        public string TreasuryRemitanceDate { get; set; }

        public double ScanningFees { get; set; }

        public string IGRRemittedDate{ get; set; }

        public string RecieptDate { get; set; }

        public string SroName { get; set; }
    }
}