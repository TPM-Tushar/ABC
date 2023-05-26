using CustomModels.Models.Remittance.ChallanDetailsReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IChallanDetails
    {
        ChallanDetailsModel ChallanDetailsReportView();

        ChallanDetailsResModel GetChallanReportDetails(ChallanDetailsModel model);
    }
}