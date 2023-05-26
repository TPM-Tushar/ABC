using CustomModels.Models.MISReports.FRUITSIntegration;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class FRUITSIntegrationAPIController : ApiController
    {
        IFRUITSIntegration fRUITSIntegrationBAL = new FRUITSIntegrationBAL();

        [HttpGet]
        [Route("api/FRUITSIntegrationAPI/KAVERIFRUITSIntegration")]
        public IHttpActionResult KAVERIFRUITSIntegration(int OfficeID)
        {
            try
            {
              return Ok(fRUITSIntegrationBAL.KAVERIFRUITSIntegration(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        [Route("api/FRUITSIntegrationAPI/GetFruitsRecvDetails")]
        public IHttpActionResult GetFruitsRecvDetails(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel)
        {
            try
            {
                return Ok(fRUITSIntegrationBAL.GetFRUITSRecvDetails(kaveriFruitsIntegrationViewModel));
            }
            catch (Exception)
            {

                throw;
            }   
        }

        [HttpPost]
        [Route("api/FRUITSIntegrationAPI/DownloadForm3")]
        public IHttpActionResult DownloadForm3(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel)
        {
            try
            {
                return Ok(fRUITSIntegrationBAL.DownloadForm3(kaveriFruitsIntegrationViewModel));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("api/FRUITSIntegrationAPI/DownloadTransXML")]
        public IHttpActionResult DownloadTransXML(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel)
        {
            try
            {
                return Ok(fRUITSIntegrationBAL.DownloadTransXML(kaveriFruitsIntegrationViewModel));
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
