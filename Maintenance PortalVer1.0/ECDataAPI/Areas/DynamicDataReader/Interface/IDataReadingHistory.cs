using CustomModels.Models.DynamicDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.DynamicDataReader.Interface
{
    public interface IDataReadingHistory
    {
        DataReadingHistoryResModel GetDataReadingHistoryReport(DataReadingHistoryModel model);

        DataReadingHistoryDetailModel GetDetailByQueryId(DataReadingHistoryDetailModel model);
    }
}
