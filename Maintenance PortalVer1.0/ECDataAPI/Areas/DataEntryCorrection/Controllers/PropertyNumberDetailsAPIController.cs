
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
    public class PropertyNumberDetailsAPIController : ApiController
    {

        IPropertyNumberDetails balObject = null;

        [HttpGet]
        [Route("api/PropertyNumberDetailsAPIController/GetPropertyNoDetailsView")]
        public IHttpActionResult GetPropertyNoDetailsView(int OfficeID, int PropertyID, long OrderId)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();

                PropertyNumberDetailsViewModel ViewModel = new PropertyNumberDetailsViewModel();

                ViewModel = balObject.GetPropertyNoDetailsView(OfficeID, PropertyID, OrderId);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/PropertyNumberDetailsAPIController/LoadPropertyDetailsData")]
        public IHttpActionResult LoadPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();
                DataEntryCorrectionResultModel resModel = new DataEntryCorrectionResultModel();

                resModel = balObject.LoadPropertyDetailsData(dataEntryCorrectionViewModel);

                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/PropertyNumberDetailsAPIController/SelectBtnClick")]
        public IHttpActionResult SelectBtnClick(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();
                DataEntryCorrectionResultModel resModel = new DataEntryCorrectionResultModel();

                resModel = balObject.SelectBtnClick(dataEntryCorrectionViewModel);

                return Ok(resModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/PropertyNumberDetailsAPIController/AddUpdatePropertyNoDetails")]
        public IHttpActionResult AddUpdatePropertyNoDetails(PropertyNumberDetailsViewModel propertyNumberDetailsViewModel)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();

                AddPropertyNoDetailsResultModel addPropertyNoDetailsResultModel = new AddPropertyNoDetailsResultModel();

                addPropertyNoDetailsResultModel = balObject.AddUpdatePropertyNoDetails(propertyNumberDetailsViewModel);

                return Ok(addPropertyNoDetailsResultModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/PropertyNumberDetailsAPIController/UpdatePropertyNoDetails")]
        public IHttpActionResult UpdatePropertyNoDetails(PropertyNumberDetailsViewModel propertyNumberDetailsViewModel)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();

                AddPropertyNoDetailsResultModel addPropertyNoDetailsResultModel = new AddPropertyNoDetailsResultModel();

                addPropertyNoDetailsResultModel = balObject.UpdatePropertyNoDetails(propertyNumberDetailsViewModel);

                return Ok(addPropertyNoDetailsResultModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Added by Madhur 29-07-2021

        [HttpPost]
        [Route("api/PropertyNumberDetailsAPIController/EditBtnProperty")]
        public IHttpActionResult EditBtnProperty(int OrderID)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();
                EditbtnResultModel resModel = balObject.EditBtnProperty(OrderID);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/PropertyNumberDetailsAPIController/DeletePropertyNoDetails")]
        public IHttpActionResult DeletePropertyNoDetails(int KeyId)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();
                bool resModel = balObject.DeletePropertyNoDetails(KeyId);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/PropertyNumberDetailsAPIController/DeactivatePropertyNoDetails")]
        public IHttpActionResult DeactivatePropertyNoDetails(int KeyId, int OrderId)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();
                bool resModel = balObject.DeactivatePropertyNoDetails(KeyId,OrderId);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 18/08/2021 to activate PropertyNoDetails
        [HttpGet]
        [Route("api/PropertyNumberDetailsAPIController/ActivatePropertyNoDetails")]
        public IHttpActionResult ActivatePropertyNoDetails(int KeyId, int OrderId)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();
                bool resModel = balObject.ActivatePropertyNoDetails(KeyId, OrderId);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Shivam B on 10/05/2022 Populate SROList on DRO Change
        [HttpGet]
        [Route("api/PropertyNumberDetailsAPIController/GetSROListByDROCode")]
        public IHttpActionResult GetSROListByDROCode(int DroCode)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();
                PropertyNumberDetailsAddEditModel resModel = balObject.GetSROListByDROCode(DroCode);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }
        //Added by Shivam B on 10/05/2022 Populate SROList on DRO Change


        //Added by mayank on 16/08/2021
        [HttpGet]
        [Route("api/PropertyNumberDetailsAPIController/GetVillageBySROCode")]
        public IHttpActionResult GetVillageBySROCode(int SroCode)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();
                PropertyNumberDetailsAddEditModel resModel = balObject.GetVillageBySROCode(SroCode);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/PropertyNumberDetailsAPIController/GetHobliDetailsOnVillageSroCode")]
        public IHttpActionResult GetHobliDetailsOnVillageSroCode(long VillageCode, int SroCode)
        {
            try
            {
                balObject = new PropertyNumberDetailsBAL();
                PropertyNumberDetailsAddEditModel resModel = balObject.GetHobliDetailsOnVillageSroCode(VillageCode, SroCode);

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
