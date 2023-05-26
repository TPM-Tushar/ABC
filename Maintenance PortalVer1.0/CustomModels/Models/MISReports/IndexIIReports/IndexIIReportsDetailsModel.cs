using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.IndexIIReports
{
    //Added By Raman Kalegaonkar on 03-04-2019
    public class IndexIIReportsDetailsModel
    {

        public string FinalRegistrationNumber { get; set; }
        public string ArticleNameE { get; set; }
        public string Stamp5Datetime { get; set; }
        public string TotalArea { get; set; }
        public string Unit { get; set; }
        public string PropertyDetails { get; set; }
        public string Schedule { get; set; }
        public string Executant { get; set; }
        public string Claimant { get; set; }
        public string VillageNameE { get; set; }
        public decimal consideration { get; set; }
        public decimal marketvalue { get; set; }

        public decimal Total { get; set; }

        public string SroName { get; set; }

    }
}
