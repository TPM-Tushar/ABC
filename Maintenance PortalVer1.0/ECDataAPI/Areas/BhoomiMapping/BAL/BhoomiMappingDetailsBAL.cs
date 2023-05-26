
using CustomModels.Models.BhoomiMapping;
using ECDataAPI.Areas.BhoomiMapping.DAL;
using ECDataAPI.Areas.BhoomiMapping.Interface;
using ECDataAPI.EcDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.BhoomiMapping.BAL
{
    public class BhoomiMappingDetailsBAL : IBhoomiMappingDetails
    {
        IBhoomiMappingDetails BhoomiMappingDAL = new BhoomiMappingDetailsDAL();

        public BhoomiMappingViewModel BhoomiMappingDetailsView(int officeID, int LevelID, long UserID)
        {
            return BhoomiMappingDAL.BhoomiMappingDetailsView(officeID, LevelID, UserID);
        }


        public List<BhoomiMappingTableModel> LoadDetailsTable(int SROCode)
        {
            return BhoomiMappingDAL.LoadDetailsTable(SROCode);
        }

    }
}