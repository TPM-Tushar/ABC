#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   REMDaignosticsDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Remittance  module.
*/
#endregion


using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Linq;
using System.Collections.Generic;
using Security;
using System.Security;
using System.Web.Mvc;
using CustomModels.Models.Remittance.REMDashboard;

namespace ECDataAPI.Areas.Remittance.DAL
{
    class REMDaignosticsDAL : IREMDaignostics
    {
        #region Properties
        private Dictionary<String, String> decryptedParameters = null;
        private String[] encryptedParameters = null;
        #endregion

        /// <summary>
        /// RemittanceDiagnosticsDetailsView
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        public RemitanceDiagnosticsDetailsReqModel RemittanceDiagnosticsDetailsView(string EncryptedID)
        {
            KaveriEntities dbContext = null;

            try
            {
                int TransactionStatus;
                bool isDro = false;
                int OfficeID;
                dbContext = new KaveriEntities();

                List<SelectListItem> TransactionStatusList = new List<SelectListItem>();
                RemitanceDiagnosticsDetailsReqModel model = new RemitanceDiagnosticsDetailsReqModel();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> SROList = new List<SelectListItem>();
                List<SelectListItem> DROList = new List<SelectListItem>();

                if (!string.IsNullOrEmpty(EncryptedID) && EncryptedID != "0")
                {
                    encryptedParameters = EncryptedID.Split('/');
                    if (!(encryptedParameters.Length == 3))
                        throw new SecurityException("URL Tempered");
                    decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    isDro = Convert.ToBoolean(decryptedParameters["IsDro"].ToString().Trim());
                    OfficeID = Convert.ToInt16(decryptedParameters["OfficeID"].ToString().Trim());
                    TransactionStatus = Convert.ToInt16(decryptedParameters["TransactionStatus"].ToString().Trim());
                    model.IsForwardedFromSummaryLink = true;
                    DateTime FromDate = new DateTime();
                    if (isDro)
                    {
                        var AMS_Remittance_Office_Status = (from aMS_Remittance_Office_Status in dbContext.AMS_Remittance_Office_Status.Where(x => x.DROCODE == OfficeID && x.IsDRO==true)
                                                            select new
                                                            {
                                                                aMS_Remittance_Office_Status.RemitStartDate
                                                            }).FirstOrDefault();
                        FromDate = Convert.ToDateTime(AMS_Remittance_Office_Status.RemitStartDate);
                    }
                    else
                    {
                        var AMS_Remittance_Office_Status = (from aMS_Remittance_Office_Status in dbContext.AMS_Remittance_Office_Status.Where(x => x.SROCode == OfficeID)
                                                            select new
                                                            {
                                                                aMS_Remittance_Office_Status.RemitStartDate
                                                            }).FirstOrDefault();
                        FromDate = Convert.ToDateTime(AMS_Remittance_Office_Status.RemitStartDate);

                    }
                    model.IsDRO = isDro;
                   
                    string fromDate = Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    model.FromDate = fromDate;

                    model.DROOfficeList = objCommon.GetDROfficesList();
                    model.SROOfficeList = objCommon.GetSROfficesList();


                }
                else
                {
                    TransactionStatus = (short)Common.ApiCommonEnum.TransactionStatus.All;
                    OfficeID = 0;
                    model.IsForwardedFromSummaryLink = false;
                    model.FromDate = "01/04/2019";
                    model.SROOfficeList = objCommon.GetSROfficesList();
                    model.DROOfficeList = objCommon.GetDROfficesList();

                }

                if (EncryptedID != "0")
                {
                    model.TransactionStatusID = TransactionStatus;
                    if (isDro)
                    {
                        model.DROOfficeID = OfficeID;
                    }
                    else
                    {
                        model.SROOfficeID = OfficeID;
                    }
                }
                else
                {
                    model.TransactionStatusID = Convert.ToInt32(Common.ApiCommonEnum.TransactionStatus.All);

                }
                TransactionStatusList.Insert(0, objCommon.GetDefaultSelectListItem("All", Convert.ToString((short)Common.ApiCommonEnum.TransactionStatus.All)));
                TransactionStatusList.Insert(1, objCommon.GetDefaultSelectListItem("Receipts Not Submitted for Remittence", Convert.ToString((short)Common.ApiCommonEnum.TransactionStatus.ReceiptsNotSubmittedForRemittance)));
                TransactionStatusList.Insert(2, objCommon.GetDefaultSelectListItem("Receipts Not Remitted", Convert.ToString((short)Common.ApiCommonEnum.TransactionStatus.ReceiptsNotRemitted)));
                TransactionStatusList.Insert(3, objCommon.GetDefaultSelectListItem("Challan Not Generated", Convert.ToString((short)Common.ApiCommonEnum.TransactionStatus.ChallanNotGenerated)));
                TransactionStatusList.Insert(4, objCommon.GetDefaultSelectListItem("Bank Reconcealation Pending", Convert.ToString((short)Common.ApiCommonEnum.TransactionStatus.BankReconcealationPending)));


                model.TransactionStatusList = TransactionStatusList;
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary>
        /// GetBankTransactionDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<BankTransactionDetailsResponseModel> GetBankTransactionDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {

            List<BankTransactionDetailsResponseModel> List = new List<BankTransactionDetailsResponseModel>();
            BankTransactionDetailsResponseModel obj = null;
            KaveriEntities dbContext = null;
            try
            {

                dbContext = new KaveriEntities();
                if (WrapperModel.IsDro==1)
                {
                    var TransactionList = dbContext.USP_REM_BankTransactionDetailList_DRO(WrapperModel.Datetime_FromDate, WrapperModel.Datetime_ToDate, WrapperModel.DROOfficeID, WrapperModel.TransactionStatus).ToList();
                    if (TransactionList != null)
                    {
                        foreach (var item in TransactionList)
                        {
                            obj = new BankTransactionDetailsResponseModel();
                            obj.TransactionID = item.TransactionID.ToString();
                            obj.EncryptedID = URLEncrypt.EncryptParameters(new String[] {
                                                                    "TransactionID="+item.TransactionID
                                                                                });

                            if (item.InstrumentBankIFSCCode != null)
                                obj.InstrumentBankIFSCCode = item.InstrumentBankIFSCCode;
                            else
                                obj.InstrumentBankIFSCCode = "null";

                            if (item.InstrumentBankMICRCode != null)
                                obj.InstrumentBankMICRCode = item.InstrumentBankMICRCode.ToString();
                            else
                                obj.InstrumentBankMICRCode = "null";

                            if (item.InstrumentNumber != null)
                                obj.InstrumentNumber = item.InstrumentNumber;
                            else
                                obj.InstrumentNumber = "null";

                            if (item.DROCode != null)
                                obj.DROCode = item.DROCode + "";
                            else
                            {
                                obj.DROCode = "null";
                                obj.SROCode = "";
                            }
                            if (item.IsReceipt != null)
                                obj.IsReceipt = item.IsReceipt.ToString();
                            else
                                obj.IsReceipt = "null";

                            if (item.DocumentID != null)
                                obj.DocumentID = item.DocumentID.ToString();
                            else
                                obj.DocumentID = "null";

                            if (item.DateOfUpdate != null)
                                obj.DateOfUpdate = item.DateOfUpdate.ToString();
                            else
                                obj.DateOfUpdate = "null";

                            if (item.StampTypeID != null)
                                obj.StampTypeID = item.StampTypeID;
                            else
                                obj.StampTypeID = "null";

                            if (item.ReceiptPaymentMode != null)
                                obj.ReceiptPaymentMode = item.ReceiptPaymentMode.ToString();
                            else
                                obj.ReceiptPaymentMode = "null";

                            if (item.ReceiptNumber != null)
                                obj.ReceiptNumber = item.ReceiptNumber.ToString();
                            else
                                obj.ReceiptNumber = "null";

                            if (item.InstrumentBankName != null)
                                obj.InstrumentBankName = item.InstrumentBankName;
                            else
                                obj.InstrumentBankName = "null";

                            if (item.Receipt_StampDate != null)
                                obj.Receipt_StampDate = item.Receipt_StampDate.ToString();
                            else
                                obj.Receipt_StampDate = "null";

                            if (item.TotalAmount != null)
                                obj.TotalAmount = item.TotalAmount.ToString();
                            else
                                obj.TotalAmount = "null";

                            if (item.InstrumentDate != null)
                                obj.InstrumentDate = item.InstrumentDate.ToString();
                            else
                                obj.InstrumentDate = "null";

                            if (item.ReceiptID != null)
                                obj.ReceiptID = item.ReceiptID.ToString();
                            else
                                obj.ReceiptID = "null";

                            if (item.StampDetailsID != null)
                                obj.StampDetailsID = item.StampDetailsID.ToString();
                            else
                                obj.StampDetailsID = "null";

                            if (item.SourceOfReceipt == null)
                                obj.SourceOfReceipt = "null";
                            else
                                obj.SourceOfReceipt = item.SourceOfReceipt;


                            if (item.DROCode != null)
                                obj.DROCode = item.DROCode.ToString();
                            else
                            {
                                obj.DROCode = "null";
                                obj.SROCode = "";
                            }

                            obj.IsDRO = item.IsDRO.ToString();

                            obj.InsertDateTime = item.InsertDateTime.HasValue ? Convert.ToDateTime(item.InsertDateTime.Value).ToString() : "null";

                            obj.TransactionIDRedirectBtn = "<a href='#' onclick=GetOtherTableDetails('" + obj.EncryptedID + "');>" + item.TransactionID + "</a>";
                            List.Add(obj);
                        }
                    }

                }
                else
                { 
                var TransactionList = dbContext.USP_REM_BankTransactionDetailList_SRO(WrapperModel.Datetime_FromDate, WrapperModel.Datetime_ToDate, WrapperModel.SROOfficeID, WrapperModel.TransactionStatus).ToList();
                    if (TransactionList != null)
                    {
                        foreach (var item in TransactionList)
                        {
                            obj = new BankTransactionDetailsResponseModel();
                            obj.TransactionID = item.TransactionID.ToString();
                            obj.EncryptedID = URLEncrypt.EncryptParameters(new String[] {
                                                                    "TransactionID="+item.TransactionID
                                                                                });

                            if (item.InstrumentBankIFSCCode != null)
                                obj.InstrumentBankIFSCCode = item.InstrumentBankIFSCCode;
                            else
                                obj.InstrumentBankIFSCCode = "null";

                            if (item.InstrumentBankMICRCode != null)
                                obj.InstrumentBankMICRCode = item.InstrumentBankMICRCode.ToString();
                            else
                                obj.InstrumentBankMICRCode = "null";

                            if (item.InstrumentNumber != null)
                                obj.InstrumentNumber = item.InstrumentNumber;
                            else
                                obj.InstrumentNumber = "null";

                            if (item.SROCODE != null)
                                obj.SROCode = item.SROCODE;
                            else
                                obj.SROCode = "null";

                            if (item.IsReceipt != null)
                                obj.IsReceipt = item.IsReceipt.ToString();
                            else
                                obj.IsReceipt = "null";

                            if (item.DocumentID != null)
                                obj.DocumentID = item.DocumentID.ToString();
                            else
                                obj.DocumentID = "null";

                            if (item.DateOfUpdate != null)
                                obj.DateOfUpdate = item.DateOfUpdate.ToString();
                            else
                                obj.DateOfUpdate = "null";

                            if (item.StampTypeID != null)
                                obj.StampTypeID = item.StampTypeID;
                            else
                                obj.StampTypeID = "null";

                            if (item.ReceiptPaymentMode != null)
                                obj.ReceiptPaymentMode = item.ReceiptPaymentMode.ToString();
                            else
                                obj.ReceiptPaymentMode = "null";

                            if (item.ReceiptNumber != null)
                                obj.ReceiptNumber = item.ReceiptNumber.ToString();
                            else
                                obj.ReceiptNumber = "null";

                            if (item.InstrumentBankName != null)
                                obj.InstrumentBankName = item.InstrumentBankName;
                            else
                                obj.InstrumentBankName = "null";

                            if (item.Receipt_StampDate != null)
                                obj.Receipt_StampDate = item.Receipt_StampDate.ToString();
                            else
                                obj.Receipt_StampDate = "null";

                            if (item.TotalAmount != null)
                                obj.TotalAmount = item.TotalAmount.ToString();
                            else
                                obj.TotalAmount = "null";

                            if (item.InstrumentDate != null)
                                obj.InstrumentDate = item.InstrumentDate.ToString();
                            else
                                obj.InstrumentDate = "null";

                            if (item.ReceiptID != null)
                                obj.ReceiptID = item.ReceiptID.ToString();
                            else
                                obj.ReceiptID = "null";

                            if (item.StampDetailsID != null)
                                obj.StampDetailsID = item.StampDetailsID.ToString();
                            else
                                obj.StampDetailsID = "null";

                            if (item.SourceOfReceipt == null)
                                obj.SourceOfReceipt = "null";
                            else
                                obj.SourceOfReceipt = item.SourceOfReceipt;


                            if (item.DROCode != null)
                                obj.DROCode = item.DROCode.ToString();
                            else
                                obj.DROCode = "null";


                            obj.IsDRO = item.IsDRO.ToString();

                            obj.InsertDateTime = item.InsertDateTime.HasValue ? Convert.ToDateTime(item.InsertDateTime.Value).ToString() : "null";

                            obj.TransactionIDRedirectBtn = "<a href='#' onclick=GetOtherTableDetails('" + obj.EncryptedID + "');>" + item.TransactionID + "</a>";
                            List.Add(obj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return List;
        }

        /// <summary>
        /// GetRemittanceDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<RemittanceDetailsResponseModel> GetRemittanceDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {

            List<RemittanceDetailsResponseModel> List = new List<RemittanceDetailsResponseModel>();
            RemittanceDetailsResponseModel obj = null;
            KaveriEntities dbContext = null;
            try
            {

                dbContext = new KaveriEntities();

                var Result = (from PaymentTransactionDetails in dbContext.AMS_REG_PaymentTransDetails.Where(x => x.TransactionID == WrapperModel.TransactionID)

                              select new
                              {
                                  PaymentTransactionDetails.DDOCode,
                                  PaymentTransactionDetails.DeptReferenceCode,
                                  PaymentTransactionDetails.ID,
                                  PaymentTransactionDetails.IPAdd,
                                  PaymentTransactionDetails.PaymentStatusCode,
                                  PaymentTransactionDetails.RemitterName,
                                  PaymentTransactionDetails.StatusCode,
                                  PaymentTransactionDetails.StatusDescription,
                                  PaymentTransactionDetails.TransactionDateTime,
                                  PaymentTransactionDetails.TransactionID,
                                  PaymentTransactionDetails.TransactionStatus,
                                  PaymentTransactionDetails.UIRNumber,
                                  PaymentTransactionDetails.UserID,
                                  PaymentTransactionDetails.InsertDateTime
                              }).ToList();

                if (Result != null)
                {
                    foreach (var item in Result)
                    {
                        obj = new RemittanceDetailsResponseModel();
                        if (item.DDOCode != null)
                            obj.DDOCode = item.DDOCode;
                        else
                            obj.DDOCode = "null";

                        if (item.DeptReferenceCode != null)
                            obj.DeptReferenceCode = item.DeptReferenceCode;
                        else
                            obj.DeptReferenceCode = "null";

                        obj.ID = item.ID.ToString();//Non Nullable

                        if (item.PaymentStatusCode == null)
                            obj.PaymentStatusCode = "null";
                        else
                            obj.PaymentStatusCode = item.PaymentStatusCode;

                        if (item.RemitterName != null)
                            obj.RemitterName = item.RemitterName;
                        else
                            obj.RemitterName = "null";

                        if (item.StatusCode != null)
                            obj.StatusCode = item.StatusCode;
                        else
                            obj.StatusCode = "null";

                        if (item.StatusDescription != null)
                            obj.StatusDesc = item.StatusDescription;
                        else
                            obj.StatusDesc = "null";

                        if (item.TransactionDateTime != null)
                            obj.TransactionDateTime = item.TransactionDateTime.ToString();
                        else
                            obj.TransactionDateTime = "null";


                        obj.TransactionID = item.TransactionID.ToString();

                        if (item.TransactionStatus != null)
                            obj.TransactionStatus = item.TransactionStatus.ToString();
                        else
                            obj.TransactionStatus = "null";

                        if (item.UIRNumber != null)
                            obj.UIRNumber = item.UIRNumber;
                        else
                            obj.UIRNumber = "null";

                        if (item.UserID != null)
                            obj.UserID = item.UserID.ToString();
                        else
                            obj.UserID = "null";

                        if (item.IPAdd != null)
                            obj.IPAdd = item.IPAdd;
                        else
                            obj.IPAdd = "null";

                        if (item.InsertDateTime == null)
                            obj.InsertDateTime = "null";
                        else
                            obj.InsertDateTime = Convert.ToDateTime(item.InsertDateTime).ToString();

                        List.Add(obj);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return List;
        }

        /// <summary>
        /// GetChallanDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<ChallanDetailsResponseModel> GetChallanDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {

            List<ChallanDetailsResponseModel> List = new List<ChallanDetailsResponseModel>();
            ChallanDetailsResponseModel obj = null;
            KaveriEntities dbContext = null;
            try
            {

                dbContext = new KaveriEntities();
                var Result = (from ChallanDetails in dbContext.AMS_ChallanMatrixDetails.Where(x => x.DepartmentRefNumber == WrapperModel.DeptRefNo)
                              select new
                              {
                                  ChallanDetails.BatchID,
                                  ChallanDetails.CardType,
                                  ChallanDetails.CCNumber,
                                  ChallanDetails.ChallanAmount,
                                  ChallanDetails.ChallanDate,
                                  ChallanDetails.ChallanExpiryDate,
                                  ChallanDetails.ChallanID,
                                  ChallanDetails.ChallanRefNum,
                                  ChallanDetails.ChallanRequestID,
                                  ChallanDetails.ChallanTotalAmount,
                                  ChallanDetails.DepartmentRefNumber,
                                  ChallanDetails.InstrmntDate,
                                  ChallanDetails.InstrmntNumber,
                                  ChallanDetails.MICRCode,
                                  ChallanDetails.PaymentMode,
                                  ChallanDetails.RemitterName,
                                  ChallanDetails.RmtncAgencyBank,
                                  ChallanDetails.InsertDateTime
                              }).ToList();

                if (Result != null)
                {
                    foreach (var item in Result)
                    {
                        obj = new ChallanDetailsResponseModel();

                        obj.BatchID = (string.IsNullOrEmpty(item.BatchID)) ? "null" : item.BatchID;

                        if (item.CardType == null)
                            obj.CardType = "null";
                        else
                            obj.CardType = item.CardType;

                        if (item.CCNumber == null)
                            obj.CCNumber = "null";
                        else
                            obj.CCNumber = item.CCNumber;

                        obj.ChallanAmount = item.ChallanAmount.ToString();

                        if (item.ChallanDate != null)
                            obj.ChallanDate = item.ChallanDate.ToString();
                        else
                            obj.ChallanDate = "null";

                        if (item.ChallanExpiryDate != null)
                            obj.ChallanExpiryDate = item.ChallanExpiryDate.ToString();
                        else
                            obj.ChallanExpiryDate = "null";

                        obj.ChallanID = item.ChallanID.ToString();

                        if (item.ChallanRefNum != null)
                            obj.ChallanRefNum = item.ChallanRefNum;
                        else
                            obj.ChallanRefNum = "null";

                        obj.ChallanRequestID = item.ChallanRequestID.ToString();

                        obj.ChallanTotalAmount = item.ChallanTotalAmount.ToString();

                        if (item.DepartmentRefNumber != null)
                            obj.DepartmentRefNumber = item.DepartmentRefNumber;
                        else
                            obj.DepartmentRefNumber = "null";

                        if (item.InstrmntDate == null)
                            obj.InstrmntDate = "null";
                        else
                            obj.InstrmntDate = item.InstrmntDate.ToString();

                        if (item.InstrmntNumber == null)
                            obj.InstrmntNumber = "null";
                        else
                            obj.InstrmntNumber = item.InstrmntNumber;

                        if (item.MICRCode == null)
                            obj.MICRCode = "null";
                        else
                            obj.MICRCode = item.MICRCode;

                        if (item.PaymentMode != null)
                            obj.PaymentMode = item.PaymentMode;
                        else
                            obj.PaymentMode = "null";

                        if (item.RemitterName != null)
                            obj.RemitterName = item.RemitterName;
                        else
                            obj.RemitterName = "null";

                        if (item.RmtncAgencyBank != null)
                            obj.RmtncAgencyBank = item.RmtncAgencyBank;
                        else
                            obj.RmtncAgencyBank = "null";

                        if (item.InsertDateTime == null)
                            obj.InsertDateTime = "null";
                        else
                            obj.InsertDateTime = Convert.ToDateTime(item.InsertDateTime).ToString();

                        List.Add(obj);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return List;
        }

        /// <summary>
        /// GetDoubleVerificationDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<DoubleVerificationDetailsResponseModel> GetDoubleVerificationDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {

            List<DoubleVerificationDetailsResponseModel> List = new List<DoubleVerificationDetailsResponseModel>();
            DoubleVerificationDetailsResponseModel obj = null;
            KaveriEntities dbContext = null;
            try
            {

                dbContext = new KaveriEntities();
                var result = dbContext.AMS_REG_PaymentDblVerificationLog.Where(x => x.ChallanRefNumber == WrapperModel.ChallanRefNumber).ToList();

                if (result != null)
                {
                    foreach (var item in result)
                    {
                        obj = new DoubleVerificationDetailsResponseModel();

                        obj.ID = item.ID.ToString();

                        if (item.ChallanRefNumber != null)
                            obj.ChallanRefNumber = item.ChallanRefNumber;
                        else
                            obj.ChallanRefNumber = "null";

                        if (item.BankTransactionNumber == null)
                        {
                            obj.BankTransactionNumber = "null";
                        }
                        else
                        {
                            obj.BankTransactionNumber = item.BankTransactionNumber;

                        }
                        if (item.BankName == null)
                        {
                            obj.BankName = "null";
                        }
                        else
                        {
                            obj.BankName = item.BankName;

                        }
                        if (item.PaymentMode == null)
                        {
                            obj.PaymentMode = "null";
                        }
                        else
                        {
                            obj.PaymentMode = item.PaymentMode;

                        }
                        if (item.PaymentStatusCode == null)
                        {
                            obj.PaymentStatusCode = "null";
                        }
                        else
                        {
                            obj.PaymentStatusCode = item.PaymentStatusCode;

                        }
                        if (item.TransactionTimeStamp == null)
                        {
                            obj.TransactionTimeStamp = "null";
                        }
                        else
                        {
                            obj.TransactionTimeStamp = item.TransactionTimeStamp.ToString();


                        }
                        if (item.PaidAmount != null)
                            obj.PaidAmount = item.PaidAmount.ToString();
                        else
                            obj.PaidAmount = "null";

                        if (item.UserID == null)
                        {
                            obj.UserID = "null";
                        }
                        else
                        {
                            obj.UserID = item.UserID.ToString();
                        }

                        if (item.IPAdd != null)
                            obj.IPAdd = item.IPAdd;
                        else
                            obj.IPAdd = "null";

                        if (item.TransactionID != null)
                            obj.TransactionID = item.TransactionID.ToString();
                        else
                            obj.TransactionID = "null";

                        if (item.ServiceStatusCode != null)
                            obj.ServiceStatusCode = item.ServiceStatusCode;
                        else
                            obj.ServiceStatusCode = "null";

                        if (item.ServiceStatusDesc != null)
                            obj.ServiceStatusDesc = item.ServiceStatusDesc;
                        else
                            obj.ServiceStatusDesc = "null";

                        if (item.SchedulerID == null)
                        {
                            obj.SchedulerID = "null";
                        }
                        else
                        {
                            obj.SchedulerID = item.SchedulerID.ToString();
                        }

                        obj.InsertDateTime = item.InsertDateTime.HasValue ? item.InsertDateTime.Value.ToString() : "null";

                        if (item.ChallanRefNumber != null)
                            obj.ChallanRefNumber = item.ChallanRefNumber;
                        else
                            obj.ChallanRefNumber = "null";

                        List.Add(obj);
                    }
                }
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return List;
        }

        /// <summary>
        /// GetBankTransactionAmountDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<BankTransactionAmountDetailsResponseModel> GetBankTransactionAmountDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {

            List<BankTransactionAmountDetailsResponseModel> List = new List<BankTransactionAmountDetailsResponseModel>();
            BankTransactionAmountDetailsResponseModel obj = null;
            KaveriEntities dbContext = null;
            try
            {

                dbContext = new KaveriEntities();
                int SROCode = WrapperModel.SROOfficeID;
                var result = (from bankTransactionamountDetails in dbContext.AMS_BankTransactionAmountDetails
                              where bankTransactionamountDetails.Srocode == SROCode && bankTransactionamountDetails.TransactionID == WrapperModel.TransactionID
                              select new
                              {
                                  bankTransactionamountDetails.ID,
                                  bankTransactionamountDetails.TransactionID,
                                  bankTransactionamountDetails.Amount,
                                  bankTransactionamountDetails.FeesRuleCode,
                                  bankTransactionamountDetails.Srocode,
                                  bankTransactionamountDetails.DeptSubPurposeID,
                                  bankTransactionamountDetails.InsertDateTime
                              });
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        obj = new BankTransactionAmountDetailsResponseModel();

                        obj.ID = item.ID.ToString();

                        if (item.TransactionID == null)
                        {
                            obj.TransactionID = " ";
                        }
                        else
                        {
                            obj.TransactionID = item.TransactionID.ToString();
                        }

                        if (item.Amount != null)
                            obj.Amount = item.Amount.ToString();
                        else
                            obj.Amount = "null";

                        if (item.Srocode != null)
                            obj.SROCode = item.Srocode.ToString();
                        else
                            obj.SROCode = "null";

                        if (item.DeptSubPurposeID == null)
                        {
                            obj.DeptSubPurpooseID = "null";
                        }
                        else
                        {
                            obj.DeptSubPurpooseID = item.DeptSubPurposeID.ToString();
                        }

                        if (item.FeesRuleCode != null)
                            obj.FeesRuleCode = item.FeesRuleCode.ToString();
                        else
                            obj.FeesRuleCode = "null";

                        if (item.InsertDateTime == null)
                        {
                            obj.InsertDateTime = "null";
                        }
                        else
                        {
                            obj.InsertDateTime = Convert.ToDateTime(item.InsertDateTime).ToString();

                        }
                        List.Add(obj);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return List;
        }

        /// <summary>
        /// GetChallanMatrixTransactionDetailsList
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<ChallanMatrixTransactionDetailsResponseModel> GetChallanMatrixTransactionDetailsList(RemittanceDiagnosticsDetialsWrapperModel WrapperModel)
        {

            List<ChallanMatrixTransactionDetailsResponseModel> List = new List<ChallanMatrixTransactionDetailsResponseModel>();
            ChallanMatrixTransactionDetailsResponseModel obj = null;
            KaveriEntities dbContext = null;
            try
            {

                dbContext = new KaveriEntities();
                var user = dbContext.AMS_ChallanMatrixDetails.Where(x => x.DepartmentRefNumber == WrapperModel.DeptRefNo).FirstOrDefault();
                if (user != null)
                {
                    long ChallanReqID = user.ChallanRequestID;
                    var result = (from ChallanMatrixTransDetails in dbContext.AMS_ChallanMatrixTransDetails.Where(x => x.ChallanRequestID == ChallanReqID && x.IsDRO ==( (WrapperModel.IsDro == 1) ? true : false ) )
                                  select new
                                  {
                                      ChallanMatrixTransDetails.ChallanRequestID,
                                      ChallanMatrixTransDetails.TransactionDateTime,
                                      ChallanMatrixTransDetails.SroCode,
                                      ChallanMatrixTransDetails.DDOCode,
                                      ChallanMatrixTransDetails.RemittanceBankName,
                                      ChallanMatrixTransDetails.ReceiptDate,
                                      ChallanMatrixTransDetails.UIRNumber,
                                      ChallanMatrixTransDetails.TransactionStatus,
                                      ChallanMatrixTransDetails.StatusCode,
                                      ChallanMatrixTransDetails.StatusDescription,
                                      ChallanMatrixTransDetails.UserID,
                                      ChallanMatrixTransDetails.IPAdd,
                                      ChallanMatrixTransDetails.BatchID,
                                      ChallanMatrixTransDetails.FirstPrintdate,
                                      ChallanMatrixTransDetails.RequestPaymentMode,
                                      ChallanMatrixTransDetails.IsDRO,
                                      ChallanMatrixTransDetails.DROCode,
                                      ChallanMatrixTransDetails.SchedulerID,
                                      ChallanMatrixTransDetails.InsertDateTime

                                  }).ToList();

                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            obj = new ChallanMatrixTransactionDetailsResponseModel();


                            obj.ChallanReqID = item.ChallanRequestID.ToString();


                            if (item.TransactionDateTime != null)
                                obj.TransactionDateTime = item.TransactionDateTime.ToString();
                            else
                                obj.TransactionDateTime = "null";

                            if (item.SroCode != null)
                                obj.SroCode = item.SroCode.ToString();
                            else
                                obj.SroCode = "null";

                            if (item.DDOCode != null)
                                obj.DDOCode = item.DDOCode;
                            else
                                obj.DDOCode = "null";

                            if (item.RemittanceBankName != null)
                                obj.RemittanceBankName = item.RemittanceBankName;
                            else
                                obj.RemittanceBankName = "null";

                            if (item.ReceiptDate != null)
                                obj.ReceiptDate = item.ReceiptDate.ToString();
                            else
                                obj.ReceiptDate = "null";

                            if (item.UIRNumber != null)
                                obj.UIRNumber = item.UIRNumber;
                            else
                                obj.UIRNumber = "null";

                            if (item.TransactionStatus != null)
                                obj.TransactionStatus = item.TransactionStatus.ToString();
                            else
                                obj.TransactionStatus = "null";

                            if (item.StatusCode != null)
                                obj.StatusCode = item.StatusCode;
                            else
                                obj.StatusCode = "null";

                            if (item.StatusDescription != null)
                                obj.StatusDesc = item.StatusDescription;
                            else
                                obj.StatusDesc = "null";

                            if (item.UserID == null)
                            {
                                obj.UserID = "null";
                            }
                            else
                            {
                                obj.UserID = item.UserID.ToString();

                            }
                            if (item.IPAdd != null)
                                obj.IPAddress = item.IPAdd;
                            else
                                obj.IPAddress = "null";

                            if (item.BatchID == null)
                            {
                                obj.BatchID = "null";
                            }
                            else
                            {
                                obj.BatchID = item.BatchID;

                            }
                            if (item.FirstPrintdate == null)
                            {
                                obj.FirstPrintDate = "null";
                            }
                            else
                            {
                                obj.FirstPrintDate = item.FirstPrintdate.ToString();
                            }

                            if (item.RequestPaymentMode == null)
                            {
                                obj.ReqPaymentMode = "null";
                            }
                            else
                            {
                                obj.ReqPaymentMode = item.RequestPaymentMode;
                            }


                            obj.IsDro = item.IsDRO.ToString();

                            if (item.DROCode == null)
                            {
                                obj.DroCode = "null";
                            }
                            else
                            {
                                obj.DroCode = item.DROCode.ToString();

                            }

                            if (item.SchedulerID == null)
                            {
                                obj.SchedulerID = "null";
                            }
                            else
                            {
                                obj.SchedulerID = item.SchedulerID.ToString();

                            }

                            if (item.InsertDateTime != null)
                                obj.InsertDateTime = Convert.ToDateTime(item.InsertDateTime).ToString();
                            else
                                obj.InsertDateTime = "null";

                            List.Add(obj);
                        }
                    }
                }
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return List;
        }

    }
}
