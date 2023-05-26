
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
    public class BhoomiMappingBAL : IBhoomiMapping
    {
        IBhoomiMapping BhoomiMappingDAL = new BhoomiMappingDAL();

        public BhoomiMappingViewModel BhoomiMappingView(int officeID, int LevelID, long UserID)
        {
            return BhoomiMappingDAL.BhoomiMappingView(officeID, LevelID, UserID);
        }


        public string Upload(BhoomiMappingUpdateModel model)
        {
            return BhoomiMappingDAL.Upload(model);
        }

    }
}