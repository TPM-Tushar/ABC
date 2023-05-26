

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CustomModels.Models.SearchCertificate;
using GauriServicesApi.Interface;
using GauriServicesApi.BAL;

namespace GauriServicesApi.Controllers
{
    public class SearchCertificateApiController : ApiController
    {
        ISearchCertificate ObjBAL = new SearchCertificateBAL();

        [HttpGet]
        [Route("api/SearchCertificateApiController/SearchCertificate")]
        public IHttpActionResult SearchCertificate()
        {
            
            try
            {
                 SearchCertificateModel result = ObjBAL.SearchCertificate();
                return Ok(result);
            }
            catch (Exception)
            {

                throw ;
            }
        }

        [HttpPost]
        [Route("api/SearchCertificateApiController/GetCertificateDetails")]
        public IHttpActionResult GetCertificateDetails(CertificateModel ObjCertificate)
        {
            try
            {
                SearchCertificateModel result = ObjBAL.GetCertificateDetails(ObjCertificate);
                return Ok(result);
            }
            catch (Exception)
            {

                throw ;
            }
        }

        [HttpPost]
        [Route("api/SearchCertificateApiController/GetCertificateByteArray")]
        public IHttpActionResult GetCertificateByteArray(CertificateModel ObjCertificate)
        {
            try
            {
                SearchCertificateModel result = ObjBAL.GetCertificateByteArray(ObjCertificate);
                return Ok(result);
            }
            catch (Exception )
            {

                throw ;
            }
        }
    } 
}
