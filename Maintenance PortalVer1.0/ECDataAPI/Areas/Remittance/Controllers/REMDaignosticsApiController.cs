#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   REMDaignosticsApiController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Remittance module.
*/
#endregion


using CustomModels.Models.Remittance.REMDashboard;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class REMDaignosticsApiController : ApiController
    {
        IREMDaignostics balObject = null;

        /// <summary>
        /// RemittanceDiagnosticsDetailsView
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/REMDaignosticsApiController/RemittanceDiagnosticsDetailsView")]
        [EventApiAuditLogFilter(Description = "Remittance Diagnostics Details View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult RemittanceDiagnosticsDetailsView(string EncryptedID)
        {
            try
            {
                balObject = new REMDaignosticsBAL();
                RemitanceDiagnosticsDetailsReqModel responseModel = new RemitanceDiagnosticsDetailsReqModel();

                responseModel = balObject.RemittanceDiagnosticsDetailsView(EncryptedID);

                return Ok(responseModel);
            }
            catch (Exception )
            {
                throw;
            }
        }


        //[HttpGet]
        //[Route("api/REMDiagnisticsController/BankTransactionDetailsList")]       
        //public IHttpActionResult BankTransactionDetailsList(String EncryptedID)
        //{
        //    try
        //    {
        //        balObject = new MISReportsBAL();
        //        List<BankTransactionDetailsResponseModel> BankTransactionDetailsResponseModelList = new List<BankTransactionDetailsResponseModel>();
        //        BankTransactionDetailsResponseModelList = balObject.BankTransactionDetailsList(EncryptedID);
        //        return Ok(BankTransactionDetailsResponseModelList);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        /// <summary>
        /// GetBankTransactionDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/REMDaignosticsApiController/GetBankTransactionDetailsList")]
        [EventApiAuditLogFilter(Description = "Get Bank Transaction Details List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetBankTransactionDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            try
            {
                //balObject = new MISReportsBAL();
                // List<BankTransactionDetailsResponseModel> BankTransactionDetailsResponseModelList = new List<BankTransactionDetailsResponseModel>();
                //BankTransactionDetailsResponseModelList = balObject.BankTransactionDetailsList(EncryptedID);
                balObject = new REMDaignosticsBAL();
                List<BankTransactionDetailsResponseModel> BankTransactionDetailsResponseModelList = new List<BankTransactionDetailsResponseModel>();
                BankTransactionDetailsResponseModelList = balObject.GetBankTransactionDetailsList(WrapperModel);

                return Ok(BankTransactionDetailsResponseModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GetRemittanceDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/REMDaignosticsApiController/GetRemittanceDetailsList")]
        [EventApiAuditLogFilter(Description = "Get Remittance Details List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetRemittanceDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            try
            {
                balObject = new REMDaignosticsBAL();
                List<RemittanceDetailsResponseModel> BankTransactionDetailsResponseModelList = new List<RemittanceDetailsResponseModel>();
                BankTransactionDetailsResponseModelList = balObject.GetRemittanceDetailsList(WrapperModel);
                return Ok(BankTransactionDetailsResponseModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GetChallanDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/REMDaignosticsApiController/GetChallanDetailsList")]
        [EventApiAuditLogFilter(Description = "Get Challan Details List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetChallanDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            try
            {
                balObject = new REMDaignosticsBAL();
                List<ChallanDetailsResponseModel> ChallanDetailsResponseModelList = new List<ChallanDetailsResponseModel>();
                ChallanDetailsResponseModelList = balObject.GetChallanDetailsList(WrapperModel);
                return Ok(ChallanDetailsResponseModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// GetDoubleVerificationDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/REMDaignosticsApiController/GetDoubleVerificationDetailsList")]
        [EventApiAuditLogFilter(Description = "Get Double Verification Details List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetDoubleVerificationDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            try
            {
                balObject = new REMDaignosticsBAL();
                List<DoubleVerificationDetailsResponseModel> DoubleVerificationDetailsResponseModelList = new List<DoubleVerificationDetailsResponseModel>();
                DoubleVerificationDetailsResponseModelList = balObject.GetDoubleVerificationDetailsList(WrapperModel);
                return Ok(DoubleVerificationDetailsResponseModelList);
            }
            catch (Exception )
            {
                throw;
            }
        }

        /// <summary>
        /// GetBankTransactionAmountDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/REMDaignosticsApiController/GetBankTransactionAmountDetailsList")]
        [EventApiAuditLogFilter(Description = "Get Bank Transaction Amount Details List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetBankTransactionAmountDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            try
            {
                balObject = new REMDaignosticsBAL();
                List<BankTransactionAmountDetailsResponseModel> DoubleVerificationDetailsResponseModelList = new List<BankTransactionAmountDetailsResponseModel>();
                DoubleVerificationDetailsResponseModelList = balObject.GetBankTransactionAmountDetailsList(WrapperModel);
                return Ok(DoubleVerificationDetailsResponseModelList);
            }
            catch (Exception )
            {
                throw;
            }
        }


        /// <summary>
        /// GetChallanMatrixTransactionDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/REMDaignosticsApiController/GetChallanMatrixTransactionDetailsList")]
        [EventApiAuditLogFilter(Description = "Get Challan Matrix Transaction Details List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetChallanMatrixTransactionDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {
            try
            {
                balObject = new REMDaignosticsBAL();
                List<ChallanMatrixTransactionDetailsResponseModel> ChallanMatrixTransactionDetailsList = new List<ChallanMatrixTransactionDetailsResponseModel>();
                ChallanMatrixTransactionDetailsList = balObject.GetChallanMatrixTransactionDetailsList(WrapperModel);
                return Ok(ChallanMatrixTransactionDetailsList);
            }
            catch (Exception )
            {
                throw;
            }
        }
    }
}
