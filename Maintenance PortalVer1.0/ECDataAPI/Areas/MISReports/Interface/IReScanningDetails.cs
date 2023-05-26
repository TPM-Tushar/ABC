using CustomModels.Models.MISReports.ReScanningDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IReScanningDetails
    {

        ReScanningDetailsViewModel ReScanningDetails(int OfficeID);
        int GetReScanningTotalCount(ReScanningDetailsViewModel model);
        ReScanningDetailsResModel GetReScanningTableData(ReScanningDetailsViewModel model);

    }
}
