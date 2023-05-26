using CustomModels.Models.Remittance.BatchCompletionDetails;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//added by vijay on 16/02/2023

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class BatchCompletionDetailsBAL : IBatchCompletionDetails
    {
        BatchCompletionDetailsDAL batchCompletionDetailsDAL = new BatchCompletionDetailsDAL();

        public BatchCompletionDetailsReportModel BatchCompletionDetailsView()
        {
            return batchCompletionDetailsDAL.BatchCompletionDetailsView();
        }
     public BatchCompletionDetailsResultModel GetBatchCompletionDetails(BatchCompletionDetailsReportModel batchCompletionDetailsReportModel)
        {
            return batchCompletionDetailsDAL.GetBatchCompletionDetails(batchCompletionDetailsReportModel);   
        }

    }
}