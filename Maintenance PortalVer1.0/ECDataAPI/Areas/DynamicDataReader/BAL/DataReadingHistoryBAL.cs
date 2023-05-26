using CustomModels.Models.DynamicDataReader;
using ECDataAPI.Areas.DynamicDataReader.DAL;
using ECDataAPI.Areas.DynamicDataReader.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.DynamicDataReader.BAL
{
    public class DataReadingHistoryBAL : IDataReadingHistory
    {
        IDataReadingHistory dalOBJ = new DataReadingHistoryDAL();

        public DataReadingHistoryResModel GetDataReadingHistoryReport(DataReadingHistoryModel model)
        {
            return dalOBJ.GetDataReadingHistoryReport(model);
        }

        public DataReadingHistoryDetailModel GetDetailByQueryId(DataReadingHistoryDetailModel model)
        {
            return dalOBJ.GetDetailByQueryId(model);
        }
    }
}