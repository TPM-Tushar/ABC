using CustomModels.Models.Remittance.BlockingProcessesForKOS;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class BlockingProcessesForKOSBAL : IBlockingProcessesForKOS
    {
        IBlockingProcessesForKOS dalOBJ = new BlockingProcessesForKOSDAL();
        public BlocingProcessesForKOSWrapperModel GetBlockingProcessForKOSDetails(BlockingProcessesForKOSReqModel model)
        {
            return dalOBJ.GetBlockingProcessForKOSDetails(model);
        }
    }
}