using CustomModels.Models.PhotoThumb;
using ECDataAPI.Areas.PhotoThumb.BAL;
using ECDataAPI.Areas.PhotoThumb.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.PhotoThumb.Controllers
{
    public class PhotoThumbFailedApiController : ApiController
    {
        IPhotoThumbFailed balObject = null;

        [HttpGet]
        [Route("api/PhotoThumbFailedAPIController/PhotoThumbFailedView")]
        public IHttpActionResult PhotoThumbFailedView(int officeID)
        {
            try
            {
                balObject = new PhotoThumbFailedBAL();

                PhotoThumbFailedViewModel resModel = balObject.PhotoThumbFailedView(officeID);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/PhotoThumbFailedAPIController/PhotoThumbFailed")]
        public IHttpActionResult PhotoThumbFailed(int SROCode)
        {
            try
            {
                balObject = new PhotoThumbFailedBAL();

                PhotoThumbFailedTableModel resModel = balObject.PhotoThumbFailed(SROCode);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/PhotoThumbFailedAPIController/PhotoThumbFailedDetail")]
        public IHttpActionResult PhotoThumbFailedDetail(long PartyID, int SROCode, bool IsPhoto, bool IsThumb)
        {
            try
            {
                balObject = new PhotoThumbFailedBAL();

                PhotoThumbFailedTableModel resModel = balObject.PhotoThumbFailedDetail(PartyID, SROCode, IsPhoto, IsThumb);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}