#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsAPIController.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   API controller for Support Enclosure
*/
#endregion

using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Areas.SupportEnclosure.BAL;
using ECDataAPI.Areas.SupportEnclosure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.SupportEnclosure.Controllers
{
    public class SupportEnclosureDetailsAPIController : ApiController
    {
        ISupportEnclosureDetails balObject = null;


        [HttpGet]
        [Route("api/SupportEnclosureDetailsAPIController/SupportEnclosureDetails")]
        public IHttpActionResult SupportEnclosureDetails(int OfficeID)
        {
            try
            {
                balObject = new SupportEnclosureDetailsBAL();
                SupportEnclosureDetailsViewModel ViewModel = new SupportEnclosureDetailsViewModel();

                ViewModel = balObject.SupportEnclosureDetails(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/SupportEnclosureDetailsAPIController/GetSupportDocumentEnclosureTotalCount")]
        public IHttpActionResult GetSupportDocumentEnclosureTotalCount(SupportEnclosureDetailsViewModel model)
        {
            try
            {
                balObject = new SupportEnclosureDetailsBAL();
                int totalcount = balObject.GetSupportDocumentEnclosureTotalCount(model);
                return Ok(totalcount);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SupportEnclosureDetailsAPIController/GetSupportDocumentEnclosureTableData")]
        public IHttpActionResult GetSupportDocumentEnclosureTableData(SupportEnclosureDetailsViewModel model)
        {
            try
            {
                balObject = new SupportEnclosureDetailsBAL();
                SupportEnclosureDetailsResModel responseModel = new SupportEnclosureDetailsResModel();
                responseModel = balObject.GetSupportDocumentEnclosureTableData(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SupportEnclosureDetailsAPIController/GetSupportPartyEnclosureTotalCount")]
        public IHttpActionResult GetSupportPartyEnclosureTotalCount(SupportEnclosureDetailsViewModel model)
        {
            try
            {
                balObject = new SupportEnclosureDetailsBAL();
                int totalcount = balObject.GetSupportPartyEnclosureTotalCount(model);
                return Ok(totalcount);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SupportEnclosureDetailsAPIController/GetSupportPartyEnclosureTableData")]
        public IHttpActionResult GetSupportPartyEnclosureTableData(SupportEnclosureDetailsViewModel model)
        {
            try
            {
                balObject = new SupportEnclosureDetailsBAL();
                SupportEnclosureDetailsResModel responseModel = new SupportEnclosureDetailsResModel();
                responseModel = balObject.GetSupportPartyEnclosureTableData(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/SupportEnclosureDetailsAPIController/GetSupportDocumentEnclosure")]
        public IHttpActionResult GetSupportDocumentEnclosure(SupportEnclosureDetailsModel model)
        {
            try
            {
                balObject = new SupportEnclosureDetailsBAL();
                SupportEnclosureDetailsResModel responseModel = new SupportEnclosureDetailsResModel();
                responseModel = balObject.GetSupportDocumentEnclosureBytes(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
