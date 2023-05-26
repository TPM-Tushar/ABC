using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomModels.Models.Remittance.MasterData;
using System.Web.Http;
using System.Web.Http.Results;

using ECDataAPI.Areas.Remittance.BAL;
using CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class MasterDataAPIController:ApiController
    {
        IMasterData balObj = null;
        [HttpGet]
        [Route("api/MasterDataAPIController/MasterDataView")]

        public IHttpActionResult MasterDataReportView()
        {
            try
            
            {
                balObj = new MasterDataBAL();
                MasterDataReportModel masterDataReportModel = new MasterDataReportModel();
                masterDataReportModel = balObj.MasterDataReportview();
                return Ok(masterDataReportModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        
        [HttpPost]
       [Route("api/MasterDataAPIController/GetMasterData")]

        public IHttpActionResult GetMasterData(MasterDataReportModel MasterDataReportModel)
        {
            try
            {
                balObj =new MasterDataBAL();
                MasterDataResultModel villageMasterResultModel = new MasterDataResultModel();
                villageMasterResultModel = balObj.MasterDataResult(MasterDataReportModel);
                return Ok(villageMasterResultModel);

            }
            catch (Exception ex)
            {
                throw;
            }

        }






    }
}