#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsDAL.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   DAL layer for Support Enclosure
*/
#endregion

using CustomModels.Models.Blockchain;
using ECDataAPI.Areas.Blockchain.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Security;
using System.Text;
using System.IO;
using ECDataUI.Session;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Transactions;

namespace ECDataAPI.Areas.Blockchain.DAL
{
    public class BlockchainApprovalListDAL : IBlockchainApprovalList
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        public BlockchainViewModel BlockchainApprovalListView(int officeID, int LevelID, long UserID)
        {
            try
            {
                BlockchainViewModel blockchainViewModel = new BlockchainViewModel();
                blockchainViewModel.SROfficeOrderList = new List<SelectListItem>();
                blockchainViewModel.DROfficeOrderList = new List<SelectListItem>();
                blockchainViewModel.SROfficeOrderList.Add(new SelectListItem() { Text = "All", Value = "0" });
                blockchainViewModel.DROfficeOrderList = objCommon.GetDROfficesList("All");

                return blockchainViewModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public BlockchainViewModel GetSroCodebyDistrict(int DroCode)
        {
            try
            {
                BlockchainViewModel dataEntryCorrectionViewModel = new BlockchainViewModel();
                dataEntryCorrectionViewModel.SROfficeOrderList = new List<SelectListItem>();
                dbContext = new KaveriEntities();
                var SroList = dbContext.SROMaster.Where(m => m.DistrictCode == DroCode).OrderBy(t => t.SRONameE).Select(x => new { x.SRONameE, x.SROCode }).ToList();
                dataEntryCorrectionViewModel.SROfficeOrderList.Add(new SelectListItem { Text = "All", Value = "0" });

                foreach (var item in SroList)
                {
                    dataEntryCorrectionViewModel.SROfficeOrderList.Add(new SelectListItem { Text = item.SRONameE, Value = Convert.ToString(item.SROCode) });
                }
                return dataEntryCorrectionViewModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public List<BlockchainApprovalTableModel> LoadDetailsTable(int DROCode, int SROCode)
        {
            int SNo = 0;
            try
            {
                dbContext = new KaveriEntities();

                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                List<BlockchainApprovalTableModel> BCATL = new List<BlockchainApprovalTableModel>();
                List<BlockchainRequestDetails> lstBc = new List<BlockchainRequestDetails>();

                if (SROCode == 0 && DROCode == 0)
                {
                    lstBc = dbContext.BlockchainRequestDetails.Where(c => (c.IsApproved == true)).ToList<BlockchainRequestDetails>();
                }

                else if (SROCode != 0)
                {
                    lstBc = dbContext.BlockchainRequestDetails.Where(c => c.SROCode == SROCode && (c.IsApproved == true)).ToList<BlockchainRequestDetails>();
                }
                else if (SROCode == 0 && DROCode != 0)
                {
                    var srList = dbContext.SROMaster.Where(x => x.DistrictCode == DROCode).Select(y => y.SROCode).ToList();
                    lstBc = dbContext.BlockchainRequestDetails.Where(c => (c.IsApproved == true) && srList.Contains(c.SROCode)).ToList<BlockchainRequestDetails>();
                }
                foreach (BlockchainRequestDetails bc in lstBc)
                {
                    BlockchainApprovalTableModel BCATM = new BlockchainApprovalTableModel();
                    SNo++;
                    BCATM.SNo = SNo;
                    BCATM.NatureOfDocument = dbContext.RegistrationArticles.Where(y => y.RegArticleCode == bc.DocumentMaster.RegArticleCode).Select(x => x.ArticleNameE).FirstOrDefault();

                    BCATM.Stamp5DateTime = Convert.ToDateTime(bc.DocumentMaster.Stamp5DateTime).ToString("dd-MM-yyyy HH:mm:ss");
                    BCATM.FinalRegistrationNumber = bc.DocumentMaster.FinalRegistrationNumber;
                    BCATM.RequestDate = Convert.ToDateTime(bc.RequestDate).ToString("dd-MM-yyyy");
                    BCATM.ReasonDesc = bc.BlockchainReasonDetails.Description;
                    BCATM.ApprovalDate = Convert.ToDateTime(bc.ApprovalDate).ToString("dd-MM-yyyy");

                    BCATL.Add(BCATM);
                }

                return BCATL;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}