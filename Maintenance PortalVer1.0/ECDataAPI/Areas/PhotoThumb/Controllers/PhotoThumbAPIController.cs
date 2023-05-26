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
    public class PhotoThumbApiController : ApiController
    {
        IPhotoThumb balObject = null;

        [HttpGet]
        [Route("api/PhotoThumbAPIController/PhotoThumbView")]
        public IHttpActionResult PhotoThumbView(int officeID)
        {
            try
            {
                balObject = new PhotoThumbBAL();

                PhotoThumbViewModel resModel = balObject.PhotoThumbView(officeID);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/PhotoThumbAPIController/PhotoThumbAvailaibility")]
        public IHttpActionResult PhotoThumbAvailaibility(int SROCode, long DocumentNumber, int BookTypeID,string fyear)
        {
            try
            {
                balObject = new PhotoThumbBAL();

                PhotoThumbTableModel resModel = balObject.PhotoThumbAvailaibility(SROCode, DocumentNumber, BookTypeID,fyear);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}