using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport;
using CustomModels.Models.Remittance.MasterData;

namespace ECDataAPI.Areas.Remittance.BAL


{
    public class MasterDataBAL:IMasterData
    {
        MasterDataDAL masterDataDAL = new MasterDataDAL();

        public MasterDataResultModel MasterDataResult(MasterDataReportModel masterDataReportModel)
        {
            return masterDataDAL.GetMasterData(masterDataReportModel);
                
        }
        public MasterDataReportModel MasterDataReportview()
        {
            return masterDataDAL.MasterDataReportView();
        }


    }
}