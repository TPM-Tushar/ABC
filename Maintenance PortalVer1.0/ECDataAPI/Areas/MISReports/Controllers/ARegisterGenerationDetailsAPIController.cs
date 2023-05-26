using CustomModels.Models.MISReports.ARegisterGenerationDetails;
using ECDataAPI.Areas.MISReports.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{

    public class ARegisterGenerationDetailsAPIController : ApiController
    {
        ARegisterGenerationDetailsBAL balObject = null;
        // GET: MISReports/ARegisterGenerationDetailsAPI
        [HttpGet]
        [Route("api/ARegisterGenerationDetailsAPIController/ARegisterGenerationDetailsView")]
        public IHttpActionResult ARegisterGenerationDetailsView(int OfficeID)
        {
            balObject = new ARegisterGenerationDetailsBAL();
            ARegisterGenerationDetailsModel responseModel = new ARegisterGenerationDetailsModel();

            responseModel = balObject.ARegisterGenerationDetailsView(OfficeID);

            return Ok(responseModel);
        }
        [HttpPost]
        [Route("api/ARegisterGenerationDetailsAPIController/GetARegisterGenerationReportsDetails")]
        public IHttpActionResult GetARegisterGenerationReportsDetails(ARegisterGenerationDetailsModel model)
        {
            try
            {
                balObject = new ARegisterGenerationDetailsBAL();
                ARegisterGenerationDetailsModel responseModel = new ARegisterGenerationDetailsModel();
                List<ARegisterGenerationDetailsTableModel> ReportsDetailsList = new List<ARegisterGenerationDetailsTableModel>();
               ReportsDetailsList = balObject.GetARegisterGenerationReportsDetails(model);
                return Ok(ReportsDetailsList);
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }
    }
}