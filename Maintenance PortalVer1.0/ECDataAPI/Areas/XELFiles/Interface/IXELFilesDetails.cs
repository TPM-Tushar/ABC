using CustomModels.Models.XELFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.XELFiles.Interface
{
    public interface IXELFilesDetails
    {
        XELFilesViewModel GetJobsDetails(int OfficeID);
        XELFilesViewModel GetAuditSpecificationDetails(int OfficeID);

        int GetRegisteredJobsTotalCount(XELFilesViewModel model);
        RegisteredJobsListModel GetRegisteredJobsTableData(XELFilesViewModel model);

        XELFilesViewModel RegisterJobsDetails(XELFilesViewModel model);

        int GetAuditSpecificationDetailsTotalCount(XELFilesViewModel model);
        XELFilesResModel GetAuditSpecificationDetailsTableData(XELFilesViewModel model);

        XELLogViewModel GetXELLogView();

        XELLogViewModel LoadXELLogDetails(XELLogViewModel model);
        XELLogViewModel GetOfficeList(String OfficeType);

    }
}
