using CustomModels.Models.ChallanNoDataEntryCorrection;
using ECDataAPI.Areas.ChallanNoDataEntryCorrection.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.ChallanNoDataEntryCorrection.DAL
{
    public class ChallanNoDataEntryCorrectionDAL : IChallanNoDataEntryCorrection
    {
        #region Properties
        KaveriEntities dbContext = new KaveriEntities();
        ApiCommonFunctions apiCommonFunctions = new ApiCommonFunctions();
        #endregion


        // For Loading The SR and DR Codes & Names
        public ChallanDetailsModel ChallanNoDataEntryCorrectionView(int officeID)
        {
            ChallanDetailsModel model = new ChallanDetailsModel();
            try
            {
                model.StampType = new List<SelectListItem>();
                model.Date = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime date = DateTime.Now.Date;

            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw;
            }
            return model;
        }



        public ChallanDetailsModel GetChallanReportDetails(ChallanDetailsModel model)
        {
            ChallanDetailsModel resModel = new ChallanDetailsModel();
            List<ChallanDetailsDataTableModel> ChallanDetailsDataTableModelList = new List<ChallanDetailsDataTableModel>();
            List<long ?> DocumentIDList = new List<long ?>();

            int SrNo = 1;
            try
            {
                var AMS_BankIntrumentDetailsResult  = dbContext.RPT_GetInstrumentDetails_For_ChallanNoDataEntryCorrection(model.InstrumentNumber, model.Date, 0).ToList();
                
                if (AMS_BankIntrumentDetailsResult.Count() > 1)
                {
                    foreach (var item in AMS_BankIntrumentDetailsResult)
                    {
                        DocumentIDList.Add(item.DocumentID);
                    }
                    
                    var DocumentIDCount = DocumentIDList.Distinct().Count();

                    //Commented By ShivamB on 13-02-2023 for Duplicate Challan which Contain different DocumentID
                    //if(DocumentIDList.Count() != DocumentIDCount)
                    //Commented By ShivamB on 13-02-2023 for Duplicate Challan which Contain different DocumentID

                    if (DocumentIDCount != 1) //Added By ShivamB on 13-02-2023 for Duplicate Challan which Contain different DocumentID
                    {
                        model.message = "Duplicate challan Entry Exists," + System.Environment.NewLine + " Please contact system administrator.";
                        return model;
                    }
                     
                }
                
                if (AMS_BankIntrumentDetailsResult.Count() > 0)
                {
                        
                    var CNDECD_Result = dbContext.ChallanNumberDataEntryCorrection.Where(x => x.Old_BIND_InstrumentNumber == model.InstrumentNumber  && x.IsCentrallyupdated == null && x.CentralDBUpdateDateTime == null).OrderBy(x => x.ChallanCorrectionID).ToList();

                    string ChallanCorrectionIDArray  = null;

                    if (CNDECD_Result.Count > 0)
                    {
                        foreach(var item in CNDECD_Result)
                        {
                            ChallanCorrectionIDArray += item.ChallanCorrectionID.ToString() + ","; 
                        }
                        ChallanCorrectionIDArray = ChallanCorrectionIDArray.Substring(0, ChallanCorrectionIDArray.Length - 1);
                    }


                    
                    foreach (var item in AMS_BankIntrumentDetailsResult)
                    {
                        ChallanDetailsDataTableModel challanDetailObj = new ChallanDetailsDataTableModel();
                        challanDetailObj.SrNo = SrNo++;
                        challanDetailObj.RowId = item.RowId;
                        challanDetailObj.SROName = string.IsNullOrEmpty(item.SRONameE) ? "-" : item.SRONameE;
                        challanDetailObj.IsPayDoneAtDROffice = item.IsDRO ? "Yes" : "No";
                        challanDetailObj.DistrictName = item.IsDRO ? item.DistrictNameE : "-";
                        challanDetailObj.ChallanNumber = item.InstrumentNumber;
                        challanDetailObj.ChallanDate = item.InstrumentDate == null ? "" : ((DateTime)item.InstrumentDate).ToString("dd-MM-yyyy");
                        challanDetailObj.Amount = item.Amount;
                        challanDetailObj.IsStampPayment = item.StampTypeID == 0 ? "No" : "Yes";
                        challanDetailObj.IsReceiptPayment = item.ReceiptID > 0 ? "Yes" : "No";
                        challanDetailObj.ReceiptNumber = item.ReceiptNumber == null ? "-" : Convert.ToString(item.ReceiptNumber);
                        challanDetailObj.Receipt_StampPayDate = item.Receipt_StampDate == null ? "" : item.Receipt_StampDate.ToString();
                        challanDetailObj.ServiceName = item.ServiceName;
                        challanDetailObj.DocumentPendingNumber = item.DocumentPendingNumber;
                        challanDetailObj.FinalRegistrationNumber = item.FRN;
                        
                        if(CNDECD_Result.Count > 0)
                        {
                            var result = CNDECD_Result.FirstOrDefault();
                            
                             if (result.AnywhereECReceiptID != null && result.IsLocallyUpdated == true && result.LocalDBUpdateDateTime != null && result.IsCentrallyupdated == null && result.CentralDBUpdateDateTime == null)
                             {
                                 challanDetailObj.UpdateButton = "<button class='btn btn-success'  OnClick='updateChallanDetails(\"" + result.ChallanNumber + "" + "," + result.Old_BIND_InstrumentNumber + "," + result.ChallanDate + "," + result.SROCode + "," + result.DistrictCode + "," + ChallanCorrectionIDArray + "\")' data-toggle='tooltip'  title='Update Challan'>Update</button>";
                                 challanDetailObj.RemarkMessage = "<div style='font-size:15px;'><b>Remark : </b>Please click on update button.</div>";
                                 challanDetailObj.Reason = result.Reason;
                                 challanDetailObj.NewInstrumentNumber = result.ChallanNumber;
                                 challanDetailObj.ReEnterInstrumentNumber = result.ChallanNumber;
                                 if (result.ChallanDate != null)
                                 {
                                     challanDetailObj.NewInstrumentDate = Convert.ToDateTime(result.ChallanDate).ToString("dd/MM/yyyy");
                                     challanDetailObj.ReEnterInstrumentDate = Convert.ToDateTime(result.ChallanDate).ToString("dd/MM/yyyy");
                                 }
                             }
                             else if (result.IsCentrallyupdated == null && result.CentralDBUpdateDateTime == null)
                             {
                                 challanDetailObj.UpdateButton = "<button class='btn btn-success' disabled data-toggle='tooltip'  title='Update'>Update</button>";
                                 challanDetailObj.RemarkMessage = @"<div style='font-size:15px;'><b>Remark : </b>This challan is already saved and in process for update in respective SRO/DRO office.</div>";
                                 challanDetailObj.Reason = result.Reason;
                                 challanDetailObj.NewInstrumentNumber = result.ChallanNumber;
                                 challanDetailObj.ReEnterInstrumentNumber = result.ChallanNumber;
                                 if (result.ChallanDate != null)
                                 {
                                     challanDetailObj.NewInstrumentDate = Convert.ToDateTime(result.ChallanDate).ToString("dd/MM/yyyy");
                                     challanDetailObj.ReEnterInstrumentDate = Convert.ToDateTime(result.ChallanDate).ToString("dd/MM/yyyy");
                                 }
                             }
                            
                        }
                        ChallanDetailsDataTableModelList.Add(challanDetailObj);
                    }
                }

                //}
                resModel.challanDetailsDataTableList = ChallanDetailsDataTableModelList;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }

            return resModel;
        }



        public ChallanNoDataEntryCorrectionResponse SaveChallanDetails(ChallanDetailsModel model)
        {
            ChallanNoDataEntryCorrectionResponse resModel = new ChallanNoDataEntryCorrectionResponse();
            List<ChallanDetailsDataTableModel> ChallanDetailsDataTableModelList = new List<ChallanDetailsDataTableModel>();
            ChallanNoDataEntryCorrectionResponse response = new ChallanNoDataEntryCorrectionResponse();

            try
            {
                var ChallanExistsInChallanNoDataEntryCorrection = dbContext.ChallanNumberDataEntryCorrection.Where(x => x.ChallanNumber == model.NewInstrumentNumber && x.LocalDBUpdateDateTime == null && x.CentralDBUpdateDateTime == null && x.IsLocallyUpdated == null && x.IsCentrallyupdated == null).ToList();

                if(ChallanExistsInChallanNoDataEntryCorrection.Count() > 0 )
                {
                    response.ErrorMessage = "Challan is already saved and in process for update in respective SRO/DRO office. ";
                    return response;
                }

                var ChallanNoExistsInDBList = dbContext.USP_AMS_CHECK_InstrumentDetails_Exists_Refund(model.NewInstrumentNumber, true).ToList();
                
                if (ChallanNoExistsInDBList.Count > 0)
                {
                    // This Checks that is Challan Exists in AMS_RefundChallanDetails table
                    var value = ChallanNoExistsInDBList.FirstOrDefault();

                    if (value.TableName == "AMS_BankInstrumentNumberDetails")
                    {
                       // This Checks is multiple Challans are present in AMS_BankInstrumentNumberDetails Table if present then show all
                       for (int index = 0; index < ChallanNoExistsInDBList.Count; index++)
                       {
                           response.ErrorMessage += "Challan is Already used in " + ChallanNoExistsInDBList[index].OfficeName + " , This Challan No is not allowed for data entry correction.<br/>&nbsp &nbsp &nbsp &nbsp";
                       }   
                    }
       
                    // This Checks is Challan Exists in AMS_RefundChallanDetails table
                    else if (value.TableName == "AMS_RefundChallanDetails")
                    {
                        // This Checks is multiple Challans are present in AMS_RefundChallanDetails Table if present then show all     
                        for (int index = 0; index < ChallanNoExistsInDBList.Count; index++)
                        {

                            if (ChallanNoExistsInDBList[index].IsDRApproved == null)
                            {
                                response.ErrorMessage += "Challan is applied for Refund at " + ChallanNoExistsInDBList[index].OfficeName + ", This Challan No is not allowed for data entry correction.<br/>&nbsp &nbsp &nbsp &nbsp";
                            }
                            else if (ChallanNoExistsInDBList[index].IsDRApproved == true)
                            {
                                response.ErrorMessage += "Challan is Refunded at " + ChallanNoExistsInDBList[index].OfficeName + ", This Challan No is not allowed for data entry correction.<br/>&nbsp &nbsp &nbsp &nbsp";
                            }
                            else
                            {
                                //response.ErrorMessage += "Challan is Rejected for Refund at " + ChallanNoExistsInDBList[index].OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                response = SaveDetails(model);
                            }
                        }
                    }
                    return response;
                }

                else
                {
                    if (model.NewInstrumentNumber.Substring(0, 2) == "IG")
                    {
                        ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();
                        var isDeptRefernceCodeExist = objService.GetChallanReferenceNoDetails(model.NewInstrumentNumber);

                        if (isDeptRefernceCodeExist == null)
                            throw new Exception("Exception occurred while getting challan Details from GetChallanReferenceNoDetails.");

                        if (!(isDeptRefernceCodeExist.IsDataExist))
                        {
                            response.ErrorMessage = "Challan reference number " + model.NewInstrumentNumber + " entered by User is not generated in Kaveri Online Services.";
                            return response;
                        }
                        else if (isDeptRefernceCodeExist.IsDataExist)
                        {
                            if ((isDeptRefernceCodeExist.PaymentStatusCode.TrimEnd() != "10700066") && (isDeptRefernceCodeExist.PaymentStatusCode.TrimEnd() != "10700072"))
                            {
                                response.ErrorMessage = "Payment is not successful for Challan reference number " + model.NewInstrumentNumber;
                                return response;
                            }
                            else
                            {
                                response = SaveDetails(model);
                            }
                        }
                        
                    }
                    else if (model.NewInstrumentNumber.Substring(0, 2) == "CR")
                    {
                        response = SaveDetails(model);
                    }
                    
                    return response;
                }
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                response.ErrorMessage = "Exception Occured while saving the Challan Details";
                throw ex;
            }
        }


        public ChallanNoDataEntryCorrectionResponse SaveDetails(ChallanDetailsModel model)
        {
            ChallanNoDataEntryCorrectionResponse response = new ChallanNoDataEntryCorrectionResponse();
            List<ChallanDetailsDataTableModel> ChallanDetailsDataTableModelList = new List<ChallanDetailsDataTableModel>();
            ChallanDetailsResModel resModel = new ChallanDetailsResModel();
            
            string BIND_RowID_String = string.Empty;
            List<long?> DocumentIDList = new List<long?>();

            try
            {
                var CNDEC_Result = dbContext.ChallanNumberDataEntryCorrection.Where(x => x.Old_BIND_InstrumentNumber == model.InstrumentNumber && x.IsCentrallyupdated == null && x.CentralDBUpdateDateTime == null).OrderBy(x => x.ChallanCorrectionID).FirstOrDefault();
                
                if (CNDEC_Result != null)
                {
                    if (CNDEC_Result.AnywhereECReceiptID != null && CNDEC_Result.IsLocallyUpdated == true && CNDEC_Result.LocalDBUpdateDateTime != null)
                    {
                        response.ErrorMessage = "This challan is already saved and in process for update.";
                        return response;
                    }
                    else if (CNDEC_Result.IsLocallyUpdated == null && CNDEC_Result.LocalDBUpdateDateTime == null )
                    {
                        response.ErrorMessage = "This challan is already saved and in process for update.";
                        return response;
                    }
                }


                var Result_AMS_BIND = dbContext.RPT_GetInstrumentDetails_For_ChallanNoDataEntryCorrection(model.HiddenInstrumentNo, DateTime.Now.ToString(), 0).ToList();
                
                if (Result_AMS_BIND.Count() > 1)
                {
                    foreach (var item in Result_AMS_BIND)
                    {
                        DocumentIDList.Add(item.DocumentID);
                    }

                    var DocumentIDCount = DocumentIDList.Distinct().Count();

                    //Commented By ShivamB on 13-02-2023 for Duplicate Challan which Contain different DocumentID
                    //if (DocumentIDList.Count() != DocumentIDCount)
                    //Ended By ShivamB on 13-02-2023 for Duplicate Challan which Contain different DocumentID
                    if (DocumentIDCount != 1) //Added By ShivamB on 13-02-2023 for Duplicate Challan which Contain different DocumentID
                    {
                        response.ErrorMessage = "Duplicate challan Entry Exists," + System.Environment.NewLine + " Please contact system administrator.";
                        return response;
                    }

                    List<string> SourceOfReceiptList = new List<string>();
                    foreach (var item in Result_AMS_BIND)
                    {
                        if (item.SourceOfReceipt == "Online EC")
                            SourceOfReceiptList.Add(item.SourceOfReceipt);
                    }
                    
                    if(SourceOfReceiptList.Count > 0)
                    {
                        if(Result_AMS_BIND.Count != SourceOfReceiptList.Count)
                        {
                            response.ErrorMessage = "Duplicate challan Entry Exists," + System.Environment.NewLine + " Please contact system administrator.";
                            return response;
                        }
                    }
                    
                }
                
                if (Result_AMS_BIND.Count > 0 )
                {
                    foreach (var AMS_BIND_Result in Result_AMS_BIND)
                    {
                        ChallanNumberDataEntryCorrection challanNumberDataEntryCorrection = new ChallanNumberDataEntryCorrection();
                        
                        challanNumberDataEntryCorrection.ChallanCorrectionID = (dbContext.ChallanNumberDataEntryCorrection.Any() ? dbContext.ChallanNumberDataEntryCorrection.Max(x => x.ChallanCorrectionID) : 0) + 1;
                        
                        challanNumberDataEntryCorrection.UserID = model.UserID;

                        if (AMS_BIND_Result.IsDRO)
                            challanNumberDataEntryCorrection.DistrictCode = AMS_BIND_Result.DROCode;
                        else
                            challanNumberDataEntryCorrection.SROCode = AMS_BIND_Result.SROCode;

                        challanNumberDataEntryCorrection.ChallanNumber = model.NewInstrumentNumber;

                        if (!string.IsNullOrEmpty(model.NewInstrumentDate))
                            challanNumberDataEntryCorrection.ChallanDate = Convert.ToDateTime(model.NewInstrumentDate);

                        challanNumberDataEntryCorrection.Reason = model.Reason;
                        challanNumberDataEntryCorrection.ApplicationDate = DateTime.Now;
                        challanNumberDataEntryCorrection.BIND_RowID = AMS_BIND_Result.RowId;
                        challanNumberDataEntryCorrection.DocumentID = AMS_BIND_Result.DocumentID;
                        challanNumberDataEntryCorrection.Old_BIND_InstrumentNumber = AMS_BIND_Result.InstrumentNumber;
                        challanNumberDataEntryCorrection.Old_BIND_InstrumentDate = AMS_BIND_Result.InstrumentDate;
                        challanNumberDataEntryCorrection.ServiceType = AMS_BIND_Result.ServiceID;

                        if (AMS_BIND_Result.ServiceName == "Online EC")
                        {
                            var OnlineReceiptsResult = dbContext.OnlineReceipts.Where(x => x.ReceiptID == AMS_BIND_Result.ReceiptID && x.SROCode == AMS_BIND_Result.SROCode && x.InsrumentNumber == AMS_BIND_Result.InstrumentNumber).FirstOrDefault();

                            if(OnlineReceiptsResult == null)
                                throw new Exception("No Details found in OnlineReceipts table.");

                            challanNumberDataEntryCorrection.AnywhereECReceiptID = OnlineReceiptsResult.ReceiptID;
                            challanNumberDataEntryCorrection.AnywhereEC_InstrumentNumber = OnlineReceiptsResult.InsrumentNumber;
                            challanNumberDataEntryCorrection.AnywhereEC_InstrumentDate = OnlineReceiptsResult.InstrumentDate;
                            challanNumberDataEntryCorrection.IsLocallyUpdated = true;
                            challanNumberDataEntryCorrection.LocalDBUpdateDateTime = DateTime.Now;
                        }
                        else
                        {
                            if (AMS_BIND_Result.ReceiptID > 0)
                            {
                                var ReceiptPaymentDetailsResult = dbContext.ReceiptPaymentDetails.Where(x => x.ReceiptID == AMS_BIND_Result.ReceiptID && x.SROCode == AMS_BIND_Result.SROCode && x.DDNumber == AMS_BIND_Result.InstrumentNumber && x.PaymentModeID == 2).FirstOrDefault();

                                if (ReceiptPaymentDetailsResult == null)
                                    throw new Exception("No Details found in ReceiptPaymentDetails table.");

                                challanNumberDataEntryCorrection.ReceiptID = ReceiptPaymentDetailsResult.ReceiptID;
                                challanNumberDataEntryCorrection.ReceiptDDNumber = ReceiptPaymentDetailsResult.DDNumber;
                                challanNumberDataEntryCorrection.ReceiptDDDate = ReceiptPaymentDetailsResult.DDdate;
                                challanNumberDataEntryCorrection.ReceiptDescription = ReceiptPaymentDetailsResult.Description;
                            }
                            else if (AMS_BIND_Result.ServiceID > 0)
                            {
                                var StampDetailsResult = dbContext.StampDetails.Where(x => x.StampDetailsID == AMS_BIND_Result.StampDetailsID && x.SROCode == AMS_BIND_Result.SROCode && x.DDNumber == AMS_BIND_Result.InstrumentNumber ).FirstOrDefault();

                                if (StampDetailsResult == null)
                                    throw new Exception("No Details found in StampDetails table.");

                                challanNumberDataEntryCorrection.StampDetailsID = StampDetailsResult.StampDetailsID;
                                challanNumberDataEntryCorrection.StampDetailsDDNumber = StampDetailsResult.DDNumber;
                                challanNumberDataEntryCorrection.StampDetailsDDDate = StampDetailsResult.DDChalDate;
                                challanNumberDataEntryCorrection.StampDetailsDDChalNumber = StampDetailsResult.DDChalNumber;
                            }
                            else
                            {
                                throw new Exception("Exception occurred while saving challan details.");
                            }
                        }

                        dbContext.ChallanNumberDataEntryCorrection.Add(challanNumberDataEntryCorrection);

                    }

                    dbContext.SaveChanges();
                    response.ResponseMessage = "Challan Details saved successfully.";
                    return response;

                }
                else
                {
                    throw new Exception("No Details Found For Challan Number in AMS_BankInstrumentNumberDetails table.");
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Exception occurred while saving challan details.";
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                return response;
                throw ex;
            }
        }




        public ChallanNoDataEntryCorrectionResponse UpdateChallanDetails(string ChallanCorrectionID, string InstrumentNumber, string InstrumentDate, int SROCode,int DistrictCode)
        {
            ChallanNoDataEntryCorrectionResponse resModel = new ChallanNoDataEntryCorrectionResponse();
            List<ChallanDetailsDataTableModel> ChallanDetailsDataTableModelList = new List<ChallanDetailsDataTableModel>();
            ChallanNoDataEntryCorrectionResponse response = new ChallanNoDataEntryCorrectionResponse();
            TransactionScope transactionScope = null;
            try
            {
                if (SROCode > 0)
                {
                    string [] ChallanCorrectionIDArray = ChallanCorrectionID.Split(',').ToArray();
                    long[] ChallanCorrectionIDLongArray = ChallanCorrectionIDArray.Select(long.Parse).ToArray();


                    var CNDEC_Data = dbContext.ChallanNumberDataEntryCorrection
                                              .Where(x => x.ChallanNumber == InstrumentNumber && x.SROCode == SROCode  && ChallanCorrectionIDLongArray.Contains(x.ChallanCorrectionID)
                                              && x.AnywhereECReceiptID != null &&  x.IsLocallyUpdated == true && x.LocalDBUpdateDateTime != null && x.IsCentrallyupdated == null && x.CentralDBUpdateDateTime == null )
                                              .OrderBy(x => x.ChallanCorrectionID).ToList();

                    if(CNDEC_Data.Count > 0)
                    {
                        using (transactionScope = new TransactionScope())
                        {
                            foreach (var CNDEC_Result in CNDEC_Data)
                            {
                                 long RowID = Convert.ToInt64(CNDEC_Result.BIND_RowID);

                                if (CNDEC_Result.ChallanDate == null)
                                {
                                    var AMS_BIND = dbContext.AMS_BankInstrumentNumberDetails
                                                        .Where(x => x.InstrumentNumber == CNDEC_Result.Old_BIND_InstrumentNumber && x.SROCode == CNDEC_Result.SROCode && x.RowId == RowID)
                                                        .FirstOrDefault();

                                    if (AMS_BIND == null)
                                        throw new Exception("No Details found in AMS_BankInstrumentNumberDetails table.");

                                    if (AMS_BIND.SourceOfReceipt != "Online EC")
                                        throw new Exception("Exception occurred while updating Challan Details.");

                                    
                                     var OnlineReceiptsDetails = dbContext.OnlineReceipts.Where(x => x.ReceiptID == AMS_BIND.ReceiptID && x.SROCode == AMS_BIND.SROCode && x.InsrumentNumber == AMS_BIND.InstrumentNumber).FirstOrDefault();

                                     if (OnlineReceiptsDetails == null)
                                         throw new Exception("No Details found in OnlineReceipts table.");

                                     AMS_BIND.InstrumentNumber = CNDEC_Result.ChallanNumber;
                                     dbContext.Entry(AMS_BIND).State = System.Data.Entity.EntityState.Modified;
                                    

                                     OnlineReceiptsDetails.InsrumentNumber = CNDEC_Result.ChallanNumber;
                                     dbContext.Entry(OnlineReceiptsDetails).State = System.Data.Entity.EntityState.Modified;
                                    

                                     CNDEC_Result.IsCentrallyupdated = true;
                                     CNDEC_Result.CentralDBUpdateDateTime = DateTime.Now;
                                     dbContext.Entry(CNDEC_Result).State = System.Data.Entity.EntityState.Modified;
                                    
                                }
                                else
                                {
                                    var AMS_BIND = dbContext.AMS_BankInstrumentNumberDetails
                                                        .Where(x => x.InstrumentNumber == CNDEC_Result.Old_BIND_InstrumentNumber && x.SROCode == CNDEC_Result.SROCode && RowID == x.RowId).FirstOrDefault();
                                    
                                    if (AMS_BIND == null)
                                        throw new Exception("No Details found in AMS_BankInstrumentNumberDetails table.");

                                    if (AMS_BIND.SourceOfReceipt != "Online EC")
                                        throw new Exception("Exception occurred while updating Challan Details.");
                                   

                                    var OnlineReceiptsDetails = dbContext.OnlineReceipts.Where(x => x.ReceiptID == AMS_BIND.ReceiptID && x.SROCode == AMS_BIND.SROCode && x.InsrumentNumber == AMS_BIND.InstrumentNumber).FirstOrDefault();

                                    if (OnlineReceiptsDetails == null)
                                        throw new Exception("No Details found in OnlineReceipts table.");


                                    AMS_BIND.InstrumentNumber = CNDEC_Result.ChallanNumber;
                                    AMS_BIND.InstrumentDate = Convert.ToDateTime(CNDEC_Result.ChallanDate);
                                    dbContext.Entry(AMS_BIND).State = System.Data.Entity.EntityState.Modified;


                                    OnlineReceiptsDetails.InsrumentNumber = CNDEC_Result.ChallanNumber;
                                    OnlineReceiptsDetails.InstrumentDate = CNDEC_Result.ChallanDate;
                                    dbContext.Entry(OnlineReceiptsDetails).State = System.Data.Entity.EntityState.Modified;
                                    

                                    CNDEC_Result.IsCentrallyupdated = true;
                                    CNDEC_Result.CentralDBUpdateDateTime = DateTime.Now;
                                    dbContext.Entry(CNDEC_Result).State = System.Data.Entity.EntityState.Modified;
                                    
                                }
                                
                                
                            }
                            resModel.ResponseMessage = "Challan Details updated Successfully.";
                            dbContext.SaveChanges();
                            transactionScope.Complete();

                        }


                    }
                    else
                    {
                        throw new Exception("No Details found in ChallanNumberDataEntryCorrection table.");
                    }
                }
                else
                {
                    resModel.ErrorMessage = "Exception occurred while updating Challan Details.";
                } 
                return resModel;
            }
            catch (Exception ex)
            {
                transactionScope.Dispose();
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                resModel.ErrorMessage = "Exception occurred while updating Challan Details.";
                return resModel;
                throw ex;
            }
        }


        



    }
}