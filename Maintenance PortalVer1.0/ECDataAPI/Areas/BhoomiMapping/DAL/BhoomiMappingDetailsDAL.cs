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

using CustomModels.Models.BhoomiMapping;
using ECDataAPI.Areas.BhoomiMapping.Interface;
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
using System.Data;

namespace ECDataAPI.Areas.BhoomiMapping.DAL
{
    public class BhoomiMappingDetailsDAL : IBhoomiMappingDetails
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        public BhoomiMappingViewModel BhoomiMappingDetailsView(int officeID, int LevelID, long UserID)
        {
            try
            {
                BhoomiMappingViewModel BhoomiMappingViewModel = new BhoomiMappingViewModel();
                BhoomiMappingViewModel.SROfficeOrderList = new List<SelectListItem>();
                dbContext = new KaveriEntities();
                var SroList = dbContext.SROMaster.OrderBy(t => t.SRONameE).Select(x => new { x.SRONameE, x.SROCode }).ToList();
                BhoomiMappingViewModel.SROfficeOrderList.Add(new SelectListItem { Text = "Select SRO", Value = "0" });

                foreach (var item in SroList)
                {
                    BhoomiMappingViewModel.SROfficeOrderList.Add(new SelectListItem { Text = item.SRONameE, Value = Convert.ToString(item.SROCode) });
                }
                return BhoomiMappingViewModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<BhoomiMappingTableModel> LoadDetailsTable(int SROCode)
        {
            int SNo = 0;
            try
            {
                dbContext = new KaveriEntities();

                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                List<BhoomiMappingTableModel> BMTM = new List<BhoomiMappingTableModel>();
                List<BhoomiMappingDetails> lstBm = new List<BhoomiMappingDetails>();



                if (SROCode != 0)
                {
                    lstBm = dbContext.BhoomiMappingDetails.Where(c => c.KaveriSROCode == SROCode).OrderBy(x => x.KaveriVillageName).ToList<BhoomiMappingDetails>();
                }
                else
                {
                    BMTM = null;
                    return BMTM;
                }
                
                foreach (BhoomiMappingDetails bm in lstBm)
                {
                    BhoomiMappingTableModel BMTM1 = new BhoomiMappingTableModel();
                    SNo++;
                    BMTM1.SrNo = SNo;
                    BMTM1.DistrictName = bm.DistrictNameE;                                       
                    BMTM1.KaveriSROCode = (bm.KaveriSROCode.ToString() == string.Empty) ? 0 :Convert.ToInt32(bm.KaveriSROCode);
                    BMTM1.KaveriSROName = bm.KaveriSROName;
                    BMTM1.KaveriVillageCode = (bm.KaveriVillageCode.ToString() == string.Empty) ? 0 : Convert.ToInt32(bm.KaveriVillageCode);
                    BMTM1.KaveriVillageName = bm.KaveriVillageName;
                    BMTM1.KaveriHobiCode = (bm.KaveriHobiCode.ToString() == string.Empty) ? 0 : Convert.ToInt32(bm.KaveriHobiCode);
                    BMTM1.KaveriHobiName = bm.KaveriHobiName;
                    BMTM1.BhoomiDistrictCode = (bm.BhoomiDistrictCode.ToString() == string.Empty) ? 0 : Convert.ToInt32(bm.BhoomiDistrictCode);
                    BMTM1.BhoomiTalukCode = (bm.BhoomiTalukCode == null) ? 0 : Convert.ToInt32(bm.BhoomiTalukCode);
                    BMTM1.BhoomiTalukName = bm.BhoomiTalukName;
                    BMTM1.BhoomiHobiCode = (bm.BhoomiHobiCode.ToString() == string.Empty) ? 0 : Convert.ToInt32(bm.BhoomiHobiCode);
                    BMTM1.BhoomiHobiName = bm.KaveriHobiName;
                    BMTM1.BhoomiVillageCode = (bm.BhoomiVillageCode.ToString() == string.Empty) ? 0 : Convert.ToInt32(bm.BhoomiVillageCode);
                    BMTM1.BhoomiVillageName = bm.BhoomiVillageName;

                    BMTM.Add(BMTM1);
                }

                return BMTM;

            }
            catch (Exception ex)
            {
                throw;
            }
        }




    }
}