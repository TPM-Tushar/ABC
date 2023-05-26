using CustomModels.Models.DataEntryCorrection;
using ECDataAPI.Areas.DataEntryCorrection.BAL;
using ECDataAPI.Areas.DataEntryCorrection.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.DataEntryCorrection.Controllers
{
    public class ReScanningApplicationApiController : ApiController
    {
        IReScanningApplication balObject = null;

        [HttpGet]
        [Route("api/ReScanningApplicationAPIController/ReScanningApplicationView")]
        public IHttpActionResult ReScanningApplicationView(int officeID)
        {
            try
            {
                balObject = new ReScanningApplicationBAL();

                ReScanningApplicationViewModel resModel = balObject.ReScanningApplicationView(officeID);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/ReScanningApplicationAPIController/btnShowClick")]
        public IHttpActionResult btnShowClick(int DRO, int SRO, string OrderNo, string Date, string DocNo, string DType, string BType, string FYear)
        {
            try
            {
                balObject = new ReScanningApplicationBAL();

                DetailModel resModel = balObject.btnShowClick(DRO, SRO, OrderNo, Date, DocNo, DType, BType, FYear);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }



        [HttpGet]
        [Route("api/ReScanningApplicationAPIController/GetFRN")]
        public IHttpActionResult GetFRN(int SROCode, string BType, string FYear, string DocNo)
        {
            try
            {
                balObject = new ReScanningApplicationBAL();

                string resModel = balObject.GetFRN(SROCode, BType, FYear, DocNo);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/ReScanningApplicationAPIController/ViewBtnClickOrderTable")]
        public IHttpActionResult ViewBtnClickOrderTable(string path)
        {
            try
            {
                balObject = new ReScanningApplicationBAL();
                DROrderFilePathResultModel resModel = balObject.ViewBtnClickOrderTable(path);

                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }






        [HttpPost]
        [Route("api/ReScanningApplicationAPIController/Upload")]
        public IHttpActionResult Upload(ReScanningApplicationReqModel req)
        {
            try
            {
                balObject = new ReScanningApplicationBAL();

                int resModel = balObject.Upload(req);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/ReScanningApplicationAPIController/PCount")]
        public IHttpActionResult PCount(int SROCode, string DocNo, string Fyear, string BType)
        {
            try
            {
                balObject = new ReScanningApplicationBAL();
                string resModel = balObject.PCount(SROCode, DocNo, Fyear, BType);
                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/ReScanningApplicationAPIController/LoadDocDetailsTable")]
        public IHttpActionResult LoadDocDetailsTable(int DroCode)
        {
            try
            {
                balObject = new ReScanningApplicationBAL();
                List<ReScanningApplicationOrderTableModel> resModel = new List<ReScanningApplicationOrderTableModel>();
                resModel = balObject.LoadDocDetailsTable(DroCode);
                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }


    }
}