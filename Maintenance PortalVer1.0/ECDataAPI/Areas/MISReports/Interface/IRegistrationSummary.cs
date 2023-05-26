using CustomModels.Models.MISReports.RegistrationSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IRegistrationSummary
    {
        //IndexIIReportsResponseModel IndexIIReportsView(IndexIIReportsRequestModel model);
        RegistrationSummaryRESModel RegistrationSummaryView(int OfficeID);
        List<RegistrationSummaryDetailModel> GetRegistrationSummaryDetails(RegistrationSummaryRESModel model);
        int GetRegistrationSummaryDetailsTotalCount(RegistrationSummaryRESModel model);
        string GetSroName(int OfficeID);
        RegistrationSummaryREQModel DisplayScannedFile(RegistrationSummaryREQModel model);
    }
}
