using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.DisableKaveri
{
   public class DisableKaveriViewModel
    {
       // public string OfficeName { get; set; }
        public List<DisableKaveriViewDetails> disableKaveriViewsList { get; set; }
        public int[] KaveriCode { get; set; }
    }
    public class DisableKaveriViewDetails
    {
        public int SrNo { get; set; }
        public string OfficeName { get; set; }
        
        public string IsDisabled { get; set; }
        public string DisableDate { get; set; }
        public int Kaveri1Code  { get; set; }
    }
    public class UpdateDetailsModel
    {
        public bool IsKaveriUpdate { get; set; }
    }
    public class MenuDisabledOfficeIDModel
    {
        public string DisabledOfficeID { get; set; }
    }

}
