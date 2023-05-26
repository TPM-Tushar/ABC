using CustomModels.Models.Remittance.DiagnosticDataForGivenRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IDiagnosticDataForGivenRegistration
    {
        DownloadDiagnosticDataScript DownloadDiagnosticDataInsertScript(DiagnosticDataForRegistrationModel model);
    }
}
