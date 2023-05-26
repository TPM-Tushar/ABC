
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
    public class IsVillageMatchingBAL : IIsVillageMatching
    {
        IIsVillageMatching IsVillageMatchingDAL = new IsVillageMatchingDAL();

        public List<IsVillageMatchingViewTableModel> IsVillageMatchingView(int officeID, int LevelID, long UserID)
        {
            return IsVillageMatchingDAL.IsVillageMatchingView(officeID, LevelID, UserID);
        }
    }
}