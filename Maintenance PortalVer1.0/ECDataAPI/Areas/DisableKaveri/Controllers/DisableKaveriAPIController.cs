using CustomModels.Models.DisableKaveri;
using ECDataAPI.Areas.DisableKaveri.BAL;
using ECDataAPI.Areas.DisableKaveri.Interface;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.DisableKaveri.Controllers
{
    public class DisableKaveriAPIController : ApiController
    {
        // GET: DisableKaveri/DisableKaveriAPI
        IDisableKaveri balObj = null;
        [HttpGet]
        [Route("api/DisableKaveriAPIController/DisableKaveriView")]
        public IHttpActionResult DisableKaveriView()
        {
            try
            {
                DisableKaveriViewModel responseModel = new DisableKaveriViewModel();
                balObj = new DisableKaveriBAL();
                responseModel = balObj.DisableKaveriView();

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/DisableKaveriAPIController/UpdateDisableKaveriDetails")]
        public IHttpActionResult UpdateDisableKaveriDetails(DisableKaveriViewModel disableKaveriViewModel)
        {
            try
            {
                UpdateDetailsModel responseModel = new UpdateDetailsModel();
                balObj = new DisableKaveriBAL();
                responseModel = balObj.UpdateDisableKaveriDetails(disableKaveriViewModel);

                return Ok(responseModel);
            }
            catch (Exception)

            {
                throw;
            }
        }

        //Added By Tushar on 5 apr 2023
        [HttpGet]
        [Route("api/DisableKaveriAPIController/GetMenuDisabledOfficeID")]
        public IHttpActionResult GetMenuDisabledOfficeID(MenuDisabledOfficeIDModel menuDisabledOfficeIDModel)
        {
            try
            {
                MenuDisabledOfficeIDModel responseModel = new MenuDisabledOfficeIDModel();
                balObj = new DisableKaveriBAL();
                responseModel = balObj.GetMenuDisabledOfficeID(menuDisabledOfficeIDModel);

                return Ok(responseModel);
            }
            catch (Exception)

            {
                throw;
            }
        }
        //End By Tushar on 5 Apr 2023
    }
}