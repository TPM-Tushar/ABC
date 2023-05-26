using CustomModels.Models.MISReports.DiskUtilization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IDiskUtilization
    {
        DiskUtilizationREQModel DiskUtilizationView(int OfficeID);

        DiskUtilizationWrapper DiskUtilizationDetails(DiskUtilizationREQModel model);
    }
}
