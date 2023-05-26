using CustomModels.Models.Remittance.BlockingProcesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
   public interface IBlockingProcesses
    {
        BlockingProcessWrapperModel GetBlockingProcessDetails();

    }
}
