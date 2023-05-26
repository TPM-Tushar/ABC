using CustomModels.Models.MISReports.DailyReceiptDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IDailyReceiptDetails
    {
        DailyReceiptDetailsViewModel DailyReceiptDetails(int OfficeID);
        int GetDailyReceiptsTotalCount(DailyReceiptDetailsViewModel model);
        DailyReceiptDetailsResModel GetDailyReceiptTableData(DailyReceiptDetailsViewModel model);

    }
}
