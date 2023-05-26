using CustomModels.Models.XELFiles;
using ECDataAPI.Areas.XELFiles.BAL;
using ECDataAPI.Areas.XELFiles.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.XELFiles.Controllers
{
    public class XELFilesDetailsAPIController : ApiController
    {

        IXELFilesDetails balObject = null;


        [HttpGet]
        [Route("api/XELFilesDetailsAPIController/GetJobsDetails")]
        public IHttpActionResult GetJobsDetails(int OfficeID)
        {
            try
            {
                balObject = new XELFilesDetailsBAL();
                XELFilesViewModel ViewModel = new XELFilesViewModel();

                ViewModel = balObject.GetJobsDetails(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/XELFilesDetailsAPIController/GetAuditSpecificationDetails")]
        public IHttpActionResult GetAuditSpecificationDetails(int OfficeID)
        {
            try
            {
                balObject = new XELFilesDetailsBAL();
                XELFilesViewModel ViewModel = new XELFilesViewModel();

                ViewModel = balObject.GetAuditSpecificationDetails(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/XELFilesDetailsAPIController/GetRegisteredJobsTotalCount")]
        public IHttpActionResult GetRegisteredJobsTotalCount(XELFilesViewModel model)
        {
            try
            {
                balObject = new XELFilesDetailsBAL();
                int totalcount = balObject.GetRegisteredJobsTotalCount(model);
                return Ok(totalcount);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/XELFilesDetailsAPIController/GetRegisteredJobsTableData")]
        public IHttpActionResult GetRegisteredJobsTableData(XELFilesViewModel model)
        {
            try
            {
                balObject = new XELFilesDetailsBAL();
                RegisteredJobsListModel responseModel = new RegisteredJobsListModel();
                responseModel = balObject.GetRegisteredJobsTableData(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("api/XELFilesDetailsAPIController/RegisterJobsDetails")]
        public IHttpActionResult RegisterJobsDetails(XELFilesViewModel model)
        {
            try
            {
                balObject = new XELFilesDetailsBAL();
                XELFilesViewModel responseModel = new XELFilesViewModel();
                responseModel = balObject.RegisterJobsDetails(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/XELFilesDetailsAPIController/GetAuditSpecificationDetailsTotalCount")]
        public IHttpActionResult GetAuditSpecificationDetailsTotalCount(XELFilesViewModel model)
        {
            try
            {
                balObject = new XELFilesDetailsBAL();
                int totalcount = balObject.GetAuditSpecificationDetailsTotalCount(model);
                return Ok(totalcount);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/XELFilesDetailsAPIController/GetAuditSpecificationDetailsTableData")]
        public IHttpActionResult GetAuditSpecificationDetailsTableData(XELFilesViewModel model)
        {
            try
            {
                balObject = new XELFilesDetailsBAL();
                XELFilesResModel responseModel = new XELFilesResModel();
                responseModel = balObject.GetAuditSpecificationDetailsTableData(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/XELFilesDetailsAPIController/GetXELLogView")]
        public IHttpActionResult GetXELLogView()
        {
            try
            {
                balObject = new XELFilesDetailsBAL();


                XELLogViewModel ViewModel = new XELLogViewModel();

                ViewModel = balObject.GetXELLogView();

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        

              [HttpPost]
        [Route("api/XELFilesDetailsAPIController/LoadXELLogDetails")]
        public IHttpActionResult LoadXELLogDetails(XELLogViewModel model)
        {
            try
            {
                balObject = new XELFilesDetailsBAL();
                XELLogViewModel responseModel = new XELLogViewModel();
                responseModel = balObject.LoadXELLogDetails(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/XELFilesDetailsAPIController/GetOfficeList")]
        
        public IHttpActionResult GetOfficeList(String OfficeType)
        {
            try
            {
                balObject = new XELFilesDetailsBAL();
                XELLogViewModel responseModel = new XELLogViewModel();
                responseModel = balObject.GetOfficeList(OfficeType);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
