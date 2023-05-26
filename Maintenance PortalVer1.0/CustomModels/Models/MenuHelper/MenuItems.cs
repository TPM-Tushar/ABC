using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MenuHelper
{
    public class MenuItems
    {
        public int IntMenuId { get; set; }
        public int? IntModuleId { get; set; }
        public string StrMenuName { get; set; }
        public int IntMenuParentId { get; set; }
        public short IntMenuLevel { get; set; }
        public bool BoolStatus { get; set; }
        public short IntMenuSeqNo { get; set; }
        public string strAreaName { get; set; }
        public string StrController { get; set; }
        public string StrAction { get; set; }
        public short HorizontalSequence { get; set; }
        public bool isMenuIDParameter { get; set; }
        public bool IsHorizontalMenu { get; set; }
        

        public List<SubModuleDetails> SubModuleList { get; set; } //added by akash
        public string ModuleIcon { get; set; }//added by akash

        // ADDED BY SHUBHAM BHAGAT ON 17-09-2019
        public string ModuleDescription { get; set; }//added by shubham

    }


    public class SubModuleDetails
    {

        public string SubModuleName { get; set; }
        public long Count { get; set; }
        public string SubModuleIcon { get; set; }
    }
}
