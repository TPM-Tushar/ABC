using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataUI.Helper
{
    public class MenuHelper
    {
        #region Properties

        public string SelectedMenu { get; set; }

        #endregion
    }

    public class SiteMenuItem : ISiteLink
    {
        #region Properties

        public Int32 MenuId { get; set; }
        public Int32 ParentId { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public Int16 VerticalLevel { get; set; }
        public Int16 Sequence { get; set; }
        public Int16 HorizontalSequence { get; set; }
        public int? ModuleID { get; set; }
        public bool IsHorizontalMenu { get; set; }
        #endregion
    }

}