
using CustomModels.Models.DataEntryCorrection;
using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Areas.DataEntryCorrection.Interface;
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
    public class PartyDetailsAPIController : ApiController
    {
        
        IPartyDetails balObject = null;

        [HttpGet]
        [Route("api/PartyDetailsAPIController/GetPartyDetailsView")]
        public IHttpActionResult GetPartyDetailsView(int OfficeID, int PartyID)
        {
            try
            {
                balObject = new PartyDetailsBAL();

                PartyDetailsViewModel ViewModel = new PartyDetailsViewModel();

                ViewModel = balObject.GetPartyDetailsView(OfficeID, PartyID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/PartyDetailsAPIController/AddUpdatePartyDetails")]
        public IHttpActionResult AddUpdatePartyDetails(PartyDetailsViewModel partyDetailsViewModel)
        {
            try
            {
                balObject = new PartyDetailsBAL();

                AddPartyDetailsResultModel addPartyDetailsResultModel = new AddPartyDetailsResultModel();

                addPartyDetailsResultModel = balObject.AddUpdatePartyDetails(partyDetailsViewModel);

                return Ok(addPartyDetailsResultModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Added by Madhur 29-07-2021
        [HttpPost]
        [Route("api/PartyDetailsAPIController/LoadPropertyDetailsPartyTabData")]
        public IHttpActionResult LoadPropertyDetailsPartyTabData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            try
            {
                balObject = new PartyDetailsBAL();
                DataEntryCorrectionResultModel resModel = new DataEntryCorrectionResultModel();

                resModel = balObject.LoadPropertyDetailsPartyTabData(dataEntryCorrectionViewModel);

                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/PartyDetailsAPIController/SelectBtnPartyTabClick")]
        public IHttpActionResult SelectBtnPartyTabClick(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            try
            {
                balObject = new PartyDetailsBAL();
                DataEntryCorrectionResultModel resModel = new DataEntryCorrectionResultModel();

                resModel = balObject.SelectBtnPartyTabClick(dataEntryCorrectionViewModel);

                return Ok(resModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/PartyDetailsAPIController/EditSaveBtnClickOrderTable")]
        public IHttpActionResult EditBtnClickOrderTable(string DROrderNumber)
        {
            try
            {
                balObject = new PartyDetailsBAL();
                string resModel = balObject.EditBtnClickOrderTable(DROrderNumber);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhur
        [HttpGet]
        [Route("api/PartyDetailsAPIController/DeletePartyDetails")]
        public IHttpActionResult DeletePartyDetails(long KeyId)
        {
            try
            {
                balObject = new PartyDetailsBAL();
                bool resModel = balObject.DeletePartyDetails(KeyId);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/PartyDetailsAPIController/DeactivatePartyDetails")]
        public IHttpActionResult DeactivatePartyDetails(long KeyId,int OrderId)
        {
            try
            {
                balObject = new PartyDetailsBAL();
                bool resModel = balObject.DeactivatePartyDetails(KeyId, OrderId);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }


        //Added by Madhur 29-07-2021
    }
}
