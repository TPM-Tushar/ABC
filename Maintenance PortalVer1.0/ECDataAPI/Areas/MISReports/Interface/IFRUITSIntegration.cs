using CustomModels.Models.MISReports.FRUITSIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IFRUITSIntegration
    {
        KaveriFruitsIntegrationViewModel KAVERIFRUITSIntegration(int OfficeID);

        KaveriFruitsIntegrationResultModel GetFRUITSRecvDetails(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel);
        KaveriFruitsIntegrationResultModel DownloadForm3(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel);
        KaveriFruitsIntegrationResultModel DownloadTransXML(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel);
    }
}
