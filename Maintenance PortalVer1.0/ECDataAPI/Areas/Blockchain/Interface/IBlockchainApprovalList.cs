
using CustomModels.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Blockchain.Interface
{
    public interface IBlockchainApprovalList
    {

        BlockchainViewModel BlockchainApprovalListView(int officeID, int LevelID, long UserID);
        List<BlockchainApprovalTableModel> LoadDetailsTable(int DroCode, int SROCode);
        BlockchainViewModel GetSroCodebyDistrict(int DroCode);

    }
}
