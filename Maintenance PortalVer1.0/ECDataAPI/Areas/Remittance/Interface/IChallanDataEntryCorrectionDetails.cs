using CustomModels.Models.Remittance.ChallanDataEntryCorrectionDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
   public interface IChallanDataEntryCorrectionDetails
    {
        ChallanDataEntryCorrectionDetailsReportModel ChallanDataEntryCorrectionDetailsview();
        ChallanDataEntryCorrectionDetailsResultModel GetCDECorrectionDetailsData(ChallanDataEntryCorrectionDetailsReportModel reportModel);


    }
}
