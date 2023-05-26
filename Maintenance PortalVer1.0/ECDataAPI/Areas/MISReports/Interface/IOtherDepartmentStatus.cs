using CustomModels.Models.MISReports.OtherDepartmentStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
   public interface IOtherDepartmentStatus
    {
        OtherDepartmentStatusModel OtherDepartmentStatusView(int OfficeID);
        List<OtherDepartmentStatusDetailsModel> OtherDepartmentStatusDetails(OtherDepartmentStatusModel model);

        int OtherDepartmentStatusDetailsTotalCount(OtherDepartmentStatusModel model);
    }
}
