#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   NotReadableDocAPIController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Not Readable Documents Details.

*/
#endregion

using CustomModels.Models.Remittance.NotReadableDoc;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class NotReadableDocAPIController : ApiController
    {
        //INotReadableDoc balObj = null;
        INotReadableDoc balObj = null;
        [HttpGet]
        [Route("api/NotReadableDocAPIController/NotReadableDocView")]
        // GET: Remittance/NotReadableDocAPI
        public IHttpActionResult NotReadableDocView(int OfficeID)
        {
            try
            {
                NotReadableDocModel responseModel = new NotReadableDocModel();
                balObj = new NotReadableDocBAL();
                responseModel = balObj.NotReadableDocView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/NotReadableDocAPIController/GetNotReadableDocDetails")]
        public IHttpActionResult GetNotReadableDocDetails(NotReadableDocModel notReadableDocModel)
        {
            try
            {
                balObj = new NotReadableDocBAL();
                NotReadableDocResultModel notReadableDocResultModel = new NotReadableDocResultModel();
                notReadableDocResultModel = balObj.GetNotReadableDocDetails(notReadableDocModel);
                return Ok(notReadableDocResultModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}