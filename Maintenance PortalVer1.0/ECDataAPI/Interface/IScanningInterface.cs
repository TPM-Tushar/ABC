#region references
using CustomModels.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace ECDataAPI.Interface
{
   public interface IScanningInterface
    {
        string InsertScanDetails(ScanDetails model);
    }
}
