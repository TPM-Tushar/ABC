using System.ComponentModel.DataAnnotations;

namespace CustomModels.Models.LogAnalysis.ECDataAuditDetails
{
    public class SROModificationDetailsViewModel
    {
        //[UIHint("Hidden")]
        public long EncryptedMasterID { get; set; }

        //[UIHint("Hidden")]
        public long EncryptedDetailsID { get; set; }

        public string SroName { get; set; }
        public string DateOfEvent { get; set; }
        public string ModificationType { get; set; }
        public string Loginname { get; set; }
        public string IPAddress { get; set; }
        public string HostName { get; set; }
        public bool qaws { get; set; }

        [Display(Name="Physical Address")]
        public string PhysicalAddress { get; set; }
        public string ApplicationName { get; set; }
        public string Statement { get; set; }
        public System.Collections.Generic.List<SROModificationDetailsViewModel> LstSROModificationDetails { get; set; }

    }
}