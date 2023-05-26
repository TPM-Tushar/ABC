using CustomModels.Models.Remittance.FirmCentralization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    interface IFirmCentralization
    {
        FirmCentralizationModel FirmCentralizationView();
        FirmCentralizationResultModel GetFirmCentralizationDetails(FirmCentralizationModel firmCentralizationModel);
        FirmCentralizationResultModel GetFirmCentralizationLocalDetails(FirmCentralizationModel firmCentralizationModel);
        FirmCentralizationResultModel GetFirmCentralizationCentralDetails(FirmCentralizationModel firmCentralizationModel);
    }
}
