using CustomModels.Models.Remittance.BlockingProcesses;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class BlockingProcessesBAL : IBlockingProcesses
    {
        IBlockingProcesses dalOBJ = new BlockingProcessesDAL();
        public BlockingProcessWrapperModel GetBlockingProcessDetails()
        {
            return dalOBJ.GetBlockingProcessDetails();
        }
    }
}