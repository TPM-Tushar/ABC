using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.TransactionDetails
{
    public partial class TransactionDetails
    {
        public string ReferenceNo { get; set; }
        
    }

    public partial class TransactionDetailsBhoomi_TransactionDetailsLandParcelDetails
    {
        public string KaveriVillageName { get; set; }
        public string KaveriTalukName { get; set; }
        public string KaveriHobliName { get; set; }
        public string KaveriDistrictName { get; set; }
    }

    public partial class TransactionDetailsBhoomi_TransactionDetailsOwnerdetails
    {
        public bool IsRelationShip { get; set; }

        public string Relationship { get; set; }
        //bhoomi_LandParcelDetails
        //bhoomi_survey_owners
        //relationship
        //public Dictionary<Dictionary<int,int>,string> relationship { get; set; }
    }
}
