/*File Header
 * Project Id: KARIGR [ IN-KA-IGR-02-05 ]
 * Project Name: Kaveri Maintainance Portal
 * File Name: ServicePackApiController.cs
 * Author :Harshit Gupta
 * Creation Date :17 May 2019
 * Desc : Service 
 * ECR No : 300
*/


#region References
using CustomModels.Models.ServicePackDetails;
using ECDataAPI.Areas.ServicePack.BAL;
using ECDataAPI.Areas.ServicePack.Interface;
using System;
using System.Collections.Generic;
using System.Web.Http; 
#endregion

namespace ECDataAPI.Areas.ServicePack.Controller
{
    public class ServicePackApiController : ApiController
    {
        IServicePack objServicePackBAL = null;

        /// <summary>
        /// Get Release Type List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackApiController/GetReleaseTypeList")]
        public IHttpActionResult GetReleaseTypeList()
        {
            objServicePackBAL = new ServicePackBAL();
            List<System.Web.Mvc.SelectListItem> releaseTypeList = objServicePackBAL.GetReleaseTypeList();
            return Ok(releaseTypeList);
        }

        /// <summary>
        /// Get Changes Type List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackApiController/GetChangesTypeList")]
        public IHttpActionResult GetChangesTypeList()
        {
            objServicePackBAL = new ServicePackBAL();
            List<System.Web.Mvc.SelectListItem> changesTypesList = objServicePackBAL.GetChangesTypesList();
            return Ok(changesTypesList);
        }

        /// <summary>
        ///  Get Changes Type List For Edit Call
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackApiController/GetChangesTypeListForEditCall")]
        public IHttpActionResult GetChangesTypeListForEditCall(int servicePackID)
        {
            objServicePackBAL = new ServicePackBAL();
            List<System.Web.Mvc.SelectListItem> changesTypesList = objServicePackBAL.GetChangesTypeListForEditCall(servicePackID);
            return Ok(changesTypesList);
        }

        /// <summary>
        /// Get Release Type List For Edit
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackApiController/GetReleaseTypeListForEdit")]
        public IHttpActionResult GetReleaseTypeListForEdit(int servicePackID)
        {
            objServicePackBAL = new ServicePackBAL();
            List<System.Web.Mvc.SelectListItem> releaseTypesList = objServicePackBAL.GetReleaseTypeListForEdit(servicePackID);
            return Ok(releaseTypesList);
        }

        /// <summary>
        /// Add Service Pack Details
        /// </summary>
        /// <param name="spDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ServicePackApiController/AddServicePackDetails")]
        public IHttpActionResult AddServicePackDetails(ServicePackViewModel spDetailsModel)
        {
            try
            {
                objServicePackBAL = new ServicePackBAL();
                string retVal = objServicePackBAL.AddServicePackDetails(spDetailsModel);
                return Ok(retVal);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update Service Pack Details
        /// </summary>
        /// <param name="spDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ServicePackApiController/UpdateServicePackDetails")]
        public IHttpActionResult UpdateServicePackDetails(ServicePackViewModel spDetailsModel)
        {
            try
            {
                objServicePackBAL = new ServicePackBAL();
                string retVal = objServicePackBAL.UpdateServicePackDetails(spDetailsModel);
                return Ok(retVal);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete Service Pack Details Entry
        /// </summary>
        /// <param name="masterID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackApiController/DeleteServicePackDetailsEntry")]
        public IHttpActionResult DeleteServicePackDetailsEntry(int masterID)
        {
            try
            {
                objServicePackBAL = new ServicePackBAL();
                string retVal = objServicePackBAL.DeleteServicePackDetailsEntry(masterID);
                return Ok(retVal);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Service Pack Details List
        /// </summary>
        /// <param name="IsRequestForApprovalsList"></param>
        /// <param name="IsRequestForReleasedServiceList"></param>
        /// <returns></returns>
        /// //Added and Changed by mayank on 14/09/2021 for Support Exe Release
        [HttpGet]
        [Route("api/ServicePackApiController/GetServicePackDetailsList")]
        public IHttpActionResult GetServicePackDetailsList(bool IsRequestForApprovalsList, bool IsRequestForReleasedServiceList,int RoleID)
        {
            try
            {
                objServicePackBAL = new ServicePackBAL();
                List<ServicePackViewModel> servicePackDetailsResponseModelList = new List<ServicePackViewModel>();
                servicePackDetailsResponseModelList = objServicePackBAL.GetServicePackDetailsList(IsRequestForApprovalsList, IsRequestForReleasedServiceList,  RoleID);
                return Ok(servicePackDetailsResponseModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Service Pack Details
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackApiController/GetServicePackDetails")]
        public IHttpActionResult GetServicePackDetails(int servicePackID)
        {
            try
            {
                objServicePackBAL = new ServicePackBAL();
                ServicePackViewModel servicePackModel = objServicePackBAL.GetServicePackDetails(servicePackID);
                return Ok(servicePackModel);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Deactivate Service Pack Details Entry
        /// </summary>
        /// <param name="masterID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackApiController/DeactivateServicePackDetailsEntry")]
        public IHttpActionResult DeactivateServicePackDetailsEntry(int masterID)
        {
            try
            {
                objServicePackBAL = new ServicePackBAL();
                string retVal = objServicePackBAL.DeactivateServicePackDetailsEntry(masterID);
                return Ok(retVal);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Save Release Notes Details
        /// </summary>
        /// <param name="releaseDetailsModelObj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ServicePackApiController/SaveReleaseNotesDetails")]
        public IHttpActionResult SaveReleaseNotesDetails(ReleaseDetails releaseDetailsModelObj)
        {
            try
            {
                objServicePackBAL = new ServicePackBAL();
                string retVal = objServicePackBAL.SaveReleaseNotesDetails(releaseDetailsModelObj);
                return Ok(retVal);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Activate Service Pack Details Entry
        /// </summary>
        /// <param name="masterID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackApiController/ActivateServicePackDetailsEntry")]
        public IHttpActionResult ActivateServicePackDetailsEntry(int masterID)
        {
            try
            {
                objServicePackBAL = new ServicePackBAL();
                string retVal = objServicePackBAL.ActivateServicePackDetailsEntry(masterID);
                return Ok(retVal);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check If Service Pack Version Already Exists
        /// </summary>
        /// <param name="majorVersion"></param>
        /// <param name="minorVersion"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackApiController/CheckIfServicePackVersionAlreadyExists")]
        //Changed by Omkar on 17-09-2020 

        public IHttpActionResult CheckIfServicePackVersionAlreadyExists(int majorVersion, int minorVersion, bool releaseType)
        {
            try
            {
                objServicePackBAL = new ServicePackBAL();
            bool retVal =  objServicePackBAL.CheckIfServicePackVersionAlreadyExists(majorVersion, minorVersion, releaseType);
                return Ok(retVal);
           }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Download Service Pack File
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackApiController/DownloadServicePackFile")]
        public IHttpActionResult DownloadServicePackFile(int servicePackID)
        {
            DownloadResponseModel responseModel = new ServicePackBAL().DownloadServicePackFile( servicePackID);
                       return Ok(responseModel);
        }

     
        [HttpGet]
        [Route("api/ServicePackApiController/CheckIfFileExists")]
        public IHttpActionResult CheckIfFileExists(int servicePackID)
        {
            DownloadResponseModel responseModel = new ServicePackBAL().CheckIfFileExists(servicePackID);
            return Ok(responseModel);
        }
    }
}
