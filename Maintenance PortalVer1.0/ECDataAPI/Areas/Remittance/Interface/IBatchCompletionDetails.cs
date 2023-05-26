using CustomModels.Models.Remittance.BatchCompletionDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//added by vijay on 16/02/2023

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IBatchCompletionDetails
    {
        BatchCompletionDetailsReportModel BatchCompletionDetailsView();
        BatchCompletionDetailsResultModel GetBatchCompletionDetails(BatchCompletionDetailsReportModel batchCompletionDetailsReportModel);

    }
}
