using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataUI.Helper
{
    public interface ISiteLink
    {
        int MenuId { get; }
        int ParentId { get; }
        string Text { get; }
        string Url { get; }
        Int16 VerticalLevel { get; }
        Int16 Sequence { get; }
        Int16 HorizontalSequence { get; }
        int? ModuleID { get; }
        bool IsHorizontalMenu { get; }
    }
}
