using CustomModels.Models.PendingDocuments;
using ECDataAPI.Areas.PendingDocuments.BAL;
using ECDataAPI.Areas.PendingDocuments.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.PendingDocuments.DAL
{
    public class PendingDocumentsApiController: ApiController
    {
        IPendingDocuments balObject = null;

        [HttpGet]
        [Route("api/PendingDocumentsAPIController/PendingDocumentsView")]
        public IHttpActionResult PendingDocumentsView(int officeID)
        {
            try
            {
                balObject = new PendingDocumentsBAL();

                PendingDocumentsViewModel resModel = balObject.PendingDocumentsView(officeID);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }



        [HttpGet]
        [Route("api/PendingDocumentsAPIController/PendingDocumentsAvailaibility")]
        public IHttpActionResult PendingDocumentsAvailaibility(int SROCode)
        {
            try
            {
                balObject = new PendingDocumentsBAL();

                List<PendingDocumentsTableModel> resModel = balObject.PendingDocumentsAvailaibility(SROCode);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}