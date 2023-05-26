#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ECDataAuditDetailsApiController.cs
    * Author Name       :   Harshit
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Log Analysis module.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CustomModels.Models.LogAnalysis.ECDataAuditDetails;
using ECDataAPI.Areas.LogAnalysis.BAL;
using ECDataAPI.Areas.LogAnalysis.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;

namespace ECDataAPI.Areas.LogAnalysis.Controllers
{

    public class ECDataAuditDetailsApiController : ApiController
    {
        IECDataAuditDetails balObject = null;


        /// <summary>
        /// returns request model of ECDataAuditDetails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ECDataAuditDetailsApiController/ECDataAuditDetailsRequestModel")]
        [EventApiAuditLogFilter(Description = "ECData Audit Details Request Model", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult ECDataAuditDetailsRequestModel()
        {
            try
            {
                balObject = new ECDataAuditDetails();
                ECDataAuditDetailsRequestModel responseModel = new ECDataAuditDetailsRequestModel();

                responseModel = balObject.ECDataAuditDetailsRequestModel();

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// returns ECDataAuditDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ECDataAuditDetailsApiController/ECDataAuditDetailsList")]
        [EventApiAuditLogFilter(Description = "ECData Audit Details List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult ECDataAuditDetailsList(ECDatatAuditDetailsWrapperModel WrapperModel)
        {
            try
            {
                balObject = new ECDataAuditDetails();
                List<ECDataAuditDetailsResponseModel> ECDataAuditDetailsResponseModelList = new List<ECDataAuditDetailsResponseModel>();
                ECDataAuditDetailsResponseModelList = balObject.ECDataAuditDetailsList(WrapperModel);
                return Ok(ECDataAuditDetailsResponseModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns Officewise Modification  occurences list
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ECDataAuditDetailsApiController/GetOfficeWiseModificationOccurences")]
        [EventApiAuditLogFilter(Description = "Get Office Wise Modification Occurences", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetOfficeWiseModificationOccurences(ECDatatAuditDetailsWrapperModel WrapperModel)
        {
            try
            {
                balObject = new ECDataAuditDetails();
                List < OfficeModificationOccurenceModel > OfficeWiseOccurance =new List<OfficeModificationOccurenceModel>();
                OfficeWiseOccurance = balObject.GetOfficeWiseModificationOccurences(WrapperModel);
                return Ok(OfficeWiseOccurance);
            }
            catch (Exception)
            {
                throw;

            }
        }


        /// <summary>
        /// Returns TotalCount of ECDataAuditDetailsList 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ECDataAuditDetailsApiController/ECDataAuditDetailsListTotalCount")]
        public IHttpActionResult ECDataAuditDetailsListTotalCount(ECDataAuditDetailsRequestModel model)
        {
            try
            {
                balObject = new ECDataAuditDetails();
                int totalCount = balObject.ECDataAuditDetailsListTotalCount(model);
                return Ok(totalCount);
            }
            catch (Exception)
            {
                throw;

            }
        }

        /// <summary>
        /// Returns GetECDataAuditDetailsList For PDF
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Added by Raman 28-03-2019

        [HttpPost]
        [Route("api/ECDataAuditDetailsApiController/GetECDataAuditDetailsListForPDF")]
        [EventApiAuditLogFilter(Description = "Get ECData Audit Details List For PDF", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetECDataAuditDetailsListForPDF(ECDataAuditDetailsRequestModel model)
        {
            try
            {
                balObject = new ECDataAuditDetails();
                List<ECDataAuditDetailsResponseModel> objListItemsToBeExported = new List<ECDataAuditDetailsResponseModel>();
                objListItemsToBeExported = balObject.GetECDataAuditDetailsListForPDF(model);
                return Ok(objListItemsToBeExported);
            }
            catch (Exception)
            {
                throw;

            }
        }

        /// <summary>
        /// Returns MasterTableList for PDF
        /// </summary>
        /// <param name="wrapperModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ECDataAuditDetailsApiController/MasterTablesListForPDF")]
        [EventApiAuditLogFilter(Description = "Master Tables List For PDF", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult MasterTablesListForPDF(WrapperModelForDescPDF wrapperModel)
        {
            try
            {
                balObject = new ECDataAuditDetails();
                List<MasterTableModel> objMasterTableDataList = new List<MasterTableModel>();
                objMasterTableDataList = balObject.MasterTablesListForPDF(wrapperModel);
                return Ok(objMasterTableDataList);
            }
            catch (Exception)
            {
                throw;

            }
        }
               
    }
}
