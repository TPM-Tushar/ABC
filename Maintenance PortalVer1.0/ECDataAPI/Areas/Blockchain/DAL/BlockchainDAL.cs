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
    public class BlockchainDAL : IBlockchain
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        public BlockchainViewModel BlockchainView(int officeID, int LevelID, long UserID)
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
                    lstBc = dbContext.BlockchainRequestDetails.Where(c => (c.IsApproved == false || c.IsApproved == null)).ToList<BlockchainRequestDetails>();
                }

                else if (SROCode != 0)
                {
                    lstBc = dbContext.BlockchainRequestDetails.Where(c => c.SROCode == SROCode && (c.IsApproved == false || c.IsApproved == null)).ToList<BlockchainRequestDetails>();
                }
                else if (SROCode == 0 && DROCode != 0)
                {
                    var srList = dbContext.SROMaster.Where(x => x.DistrictCode == DROCode).Select(y => y.SROCode).ToList();
                    lstBc = dbContext.BlockchainRequestDetails.Where(c => (c.IsApproved == false || c.IsApproved == null) && srList.Contains(c.SROCode)).ToList<BlockchainRequestDetails>();
                }
                foreach (BlockchainRequestDetails bc in lstBc)
                {
                    BlockchainApprovalTableModel BCATM = new BlockchainApprovalTableModel();
                    SNo++;
                    BCATM.SNo = SNo;
                    BCATM.NatureOfDocument = dbContext.RegistrationArticles.Where(y => y.RegArticleCode == bc.DocumentMaster.RegArticleCode).Select(x => x.ArticleNameE).FirstOrDefault();
                    BCATM.SROCode = bc.DocumentMaster.SROCode;
                    BCATM.Stamp5DateTime = Convert.ToDateTime(bc.DocumentMaster.Stamp5DateTime).ToString("dd-MM-yyyy HH:mm:ss");
                    BCATM.FinalRegistrationNumber = bc.DocumentMaster.FinalRegistrationNumber;
                    BCATM.RequestDate = Convert.ToDateTime(bc.RequestDate).ToString("dd-MM-yyyy");
                    BCATM.DocumentID = bc.DocumentMaster.DocumentID;
                    BCATM.Action = "<input type='checkbox' name='chkbxBCA' value='" + bc.DocumentMaster.DocumentID + "_" + bc.DocumentMaster.SROCode + "' />";
                    BCATM.ReasonDesc = bc.BlockchainReasonDetails.Description;

                    BCATL.Add(BCATM);
                }

                return BCATL;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string Approval(List<string> appList)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            string status = string.Empty;
            long DocumentID;
            int SROCode;
            BlockchainRequestDetails UpdateList = new BlockchainRequestDetails();

            try
            {
                dbContext = new KaveriEntities();

                foreach (string s in appList)
                {
                    string[] a = s.Split('_');
                    DocumentID = Int64.Parse(a[0]);
                    SROCode = Int32.Parse(a[1]);

                    UpdateList = dbContext.BlockchainRequestDetails.Where(x => x.DocumentID == DocumentID && x.SROCode == SROCode).FirstOrDefault();

                    if (UpdateList != null)
                    {

                        UpdateList.IsApproved = true;
                        UpdateList.ApprovalDate = DateTime.Now;
                        dbContext.Entry(UpdateList).State = System.Data.Entity.EntityState.Modified;
                        status = "Successfull";
                    }
                    else
                    {
                        status = "Error";
                        count++;
                    }

                    if (status == "Error")
                    {
                        if (count == 1)
                        {
                            sb.Append("Error Occured while Updating for Final Registration Number: ");
                            sb.Append(UpdateList.DocumentMaster.FinalRegistrationNumber);
                        }
                        else
                        {
                            sb.Append(", " + UpdateList.DocumentMaster.FinalRegistrationNumber);
                        }
                    }
                }
                if (count == 0)
                {
                    sb.Append("All checked rows approved succesfully.");
                }
                dbContext.SaveChanges();
                return sb.ToString();


            }

            catch (Exception ex)
            {
                sb.Clear();

                sb.Append(ex.Message);

                return sb.ToString();
            }
        }

    }
}