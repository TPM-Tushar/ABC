using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MenuHelper
{
    public class LoadMenuModel
    {
        public Int32 ParentMenuId { get; set; }
        public List<Int32> SequenceExcludeList { get; set; }
        public List<Int32> SequenceIncludeList { get; set; }
        public short RoleID { get; set; }
        public long UserID { get; set; }
        public short OfficeID { get; set; }

        public int ModuleID { get; set; }
        //   public bool IsSideBarStatistics { get; set; }
        public string ModuleName { get; set; }

        public int MonthID { get; set; }
        public int YearID { get; set; }

    }
}
