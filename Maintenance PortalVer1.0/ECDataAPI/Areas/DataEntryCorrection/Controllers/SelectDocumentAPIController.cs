
using CustomModels.Models.DataEntryCorrection;
using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Areas.DataEntryCorrection.Interface;
using ECDataAPI.Areas.SupportEnclosure.BAL;
using ECDataAPI.Areas.SupportEnclosure.Interface;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.SupportEnclosure.Controllers
{
    public class SelectDocumentAPIController : ApiController
    {
        ISelectDocument balObject = null;

        //Added by Madhusoodan to check if Order file exists afer used has deleted to restrict user  in case if he hasn't uploaded it again
        [HttpGet]
        [Route("api/SelectDocumentAPIController/CheckOrderFileExists")]
        public IHttpActionResult CheckOrderFileExists(int currentOrderID)
        {
            try
            {
                balObject = new SelectDocumentBAL();

                bool isFileDeleted = balObject.CheckOrderFileExists(currentOrderID);

                return Ok(isFileDeleted);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/SelectDocumentAPIController/GetSelectDocumentView")]
        public IHttpActionResult GetSelectDocumentView(int OfficeID, bool isEditMode, int currentOrderID)
        {
            try
            {
                balObject = new SelectDocumentBAL();

                DataEntryCorrectionViewModel ViewModel = new DataEntryCorrectionViewModel();

                ViewModel = balObject.SelectDocumentTabView(OfficeID, isEditMode, currentOrderID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/SelectDocumentAPIController/LoadPropertyDetailsData")]
        public IHttpActionResult LoadPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            try
            {
                balObject = new SelectDocumentBAL();
                DataEntryCorrectionResultModel resModel = new DataEntryCorrectionResultModel();

                resModel = balObject.LoadPropertyDetailsData(dataEntryCorrectionViewModel);

                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Madhur on 27-7-21
        [HttpPost]
        [Route("api/SelectDocumentAPIController/LoadPreviousPropertyDetailsData")]
        public IHttpActionResult LoadPreviousPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            try
            {
                balObject = new SelectDocumentBAL();
                List<DataEntryCorrectionPreviousPropertyDetailModel> resModel = new List<DataEntryCorrectionPreviousPropertyDetailModel>();
                
                resModel = balObject.LoadPreviousPropertyDetailsData(dataEntryCorrectionViewModel);

                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by Madhur on 27-7-21

        [HttpGet]
        [Route("api/SelectDocumentAPIController/ViewBtnClickPreviousPropTable")]
        public IHttpActionResult ViewBtnClickPreviousPropTable(int OrderID)
        {
            try
            {
                balObject = new SelectDocumentBAL();
                string resModel = balObject.ViewBtnClickPreviousPropTable(OrderID);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 06/08/2021
        [HttpPost]
        [Route("api/SelectDocumentAPIController/SaveSection68Note")]
        public IHttpActionResult SaveSection68Note(DataEntryCorrectionViewModel decViewModel)
        {
            try
            {
                balObject = new SelectDocumentBAL();
                SelectDocumentResultModel sdResModel = new SelectDocumentResultModel();

                sdResModel = balObject.SaveSection68Note(decViewModel);

                return Ok(sdResModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 06/08/2021
        [HttpGet]
        [Route("api/SelectDocumentAPIController/LoadPreviousSec68Note")]
        public IHttpActionResult LoadPreviousSec68Note(int orderID, long propertyID, int officeID)
        {
            try
            {
                balObject = new SelectDocumentBAL();
                Section68NoteResultModel sec68NOteResModel = new Section68NoteResultModel();

                sec68NOteResModel = balObject.LoadPreviousSec68Note(orderID, propertyID, officeID);

                return Ok(sec68NOteResModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 13/08/2021 to load Delete button for Section 68 Note
        [HttpGet]
        [Route("api/SelectDocumentAPIController/DeleteSection68Note")]
        public IHttpActionResult DeleteSection68Note(int NoteID)
        {
            try
            {
                balObject = new SelectDocumentBAL();
                bool deleteResult = balObject.DeleteSection68Note(NoteID);

                return Ok(deleteResult);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 13/08/2021 to load finalize btn if Section 68 Note is added for Current Order ID
        [HttpGet]
        [Route("api/SelectDocumentAPIController/IsSection68NoteAddedForOrderID")]
        public IHttpActionResult IsSection68NoteAddedForOrderID(int currentOrderID)
        {
            try
            {
                balObject = new SelectDocumentBAL();
                bool isNoteAdded = balObject.IsSection68NoteAddedForOrderID(currentOrderID);

                return Ok(isNoteAdded);
            }

            catch (Exception ex)
            {
                throw;
            }
        }
		
		//Added by mayank on 12/08/2021
        [HttpGet]
        [Route("api/SelectDocumentAPIController/CheckifOrderNoteExist")]
        public IHttpActionResult CheckifOrderNoteExist(int OrderId, long PropertyID)
        {
            try
            {
                balObject = new SelectDocumentBAL();
                bool resModel = balObject.CheckifOrderNoteExist(OrderId, PropertyID);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
