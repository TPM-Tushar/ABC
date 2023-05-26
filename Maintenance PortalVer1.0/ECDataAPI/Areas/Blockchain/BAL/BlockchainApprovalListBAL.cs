#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsBAL.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   BAL for Support Enclosure
*/
#endregion

using CustomModels.Models.Blockchain;
using ECDataAPI.Areas.Blockchain.DAL;
using ECDataAPI.Areas.Blockchain.Interface;
using ECDataAPI.EcDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Blockchain.BAL
{
    public class BlockchainApprovalListBAL : IBlockchainApprovalList
    {
        IBlockchainApprovalList BlockchainApprovalListDAL = new BlockchainApprovalListDAL();

        public BlockchainViewModel BlockchainApprovalListView(int officeID, int LevelID, long UserID)
        {
            return BlockchainApprovalListDAL.BlockchainApprovalListView(officeID, LevelID, UserID);
        }


        public BlockchainViewModel GetSroCodebyDistrict(int DroCode)
        {
            return BlockchainApprovalListDAL.GetSroCodebyDistrict(DroCode);
        }

        public List<BlockchainApprovalTableModel> LoadDetailsTable(int DroCode, int SROCode)
        {
            return BlockchainApprovalListDAL.LoadDetailsTable(DroCode, SROCode);
        }


    }
}