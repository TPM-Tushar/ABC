using CustomModels.Models.Remittance.BlockingProcessesForKOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    interface IBlockingProcessesForKOS
    {
        BlocingProcessesForKOSWrapperModel GetBlockingProcessForKOSDetails(BlockingProcessesForKOSReqModel model);
    }
}
