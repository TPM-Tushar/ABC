using CustomModels.Models.Remittance.OfficeWiseDiagnosticStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IOfficeWiseDiagnosticStatus
    {
        OfficeWiseDiagnosticStatusModel OfficeWiseDiagnosticStatusModelView();
        OfficeWiseDiagnosticStatusModel GetOfficeList(String OfficeType);
        OfficeWiseDiagnosticStatusListModel GetOfficeWiseDiagnosticStatusDetail(OfficeWiseDiagnosticStatusModel model);
        DiagnosticActionDetail GetActionDetail(DiagnosticActionDetail model);
        OfficeWiseDiagnosticStatusListModel ExportOfficeWiseDiagnosticStatusToExcel(OfficeWiseDiagnosticStatusModel model);
        OfficeWiseDiagnosticStatusListModel GetDiagnosticStatusDetailByActionType(OfficeWiseDiagnosticStatusModel model);

    }
}
