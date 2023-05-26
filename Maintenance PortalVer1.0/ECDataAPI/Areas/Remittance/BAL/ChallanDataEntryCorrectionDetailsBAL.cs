using CustomModels.Models.Remittance.ChallanDataEntryCorrectionDetails;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class ChallanDataEntryCorrectionDetailsBAL : IChallanDataEntryCorrectionDetails
    {

        ChallanDataEntryCorrectionDetailsDAL dalObj = new ChallanDataEntryCorrectionDetailsDAL();


        public ChallanDataEntryCorrectionDetailsReportModel ChallanDataEntryCorrectionDetailsview()
        {
            return dalObj.ChallanDataEntryCorrectionDetailsView();
        }

        public ChallanDataEntryCorrectionDetailsResultModel GetCDECorrectionDetailsData(ChallanDataEntryCorrectionDetailsReportModel reportModel)
        {
            return dalObj.GetCDECorrectionDetailsData(reportModel);

        }

    }
}