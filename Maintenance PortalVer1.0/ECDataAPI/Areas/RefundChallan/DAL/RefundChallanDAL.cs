

using CustomModels.Models.RefundChallan;
using ECDataAPI.Areas.RefundChallan.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web.Mvc;

namespace ECDataAPI.Areas.RefundChallan.DAL
{
    public class RefundChallanDAL : IRefundChallan
    {

        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService;


        // For Loading The SR and DR Codes & Names
        public RefundChallanViewModel RefundChallanView(int officeID, int LevelID) 
        {
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    RefundChallanViewModel refundChallanViewModel = new RefundChallanViewModel();

                    refundChallanViewModel.SROfficeList = new List<SelectListItem>();
                    refundChallanViewModel.DROfficeList = new List<SelectListItem>();

                    int kaveriCode = dbContext.MAS_OfficeMaster.Where(m => m.OfficeID == officeID).Select(m => m.Kaveri1Code).FirstOrDefault();

                    if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                    {
                        var SRODetails = dbContext.SROMaster.Where(x => x.SROCode == kaveriCode).Select(x => new { x.DistrictCode, x.SRONameE }).FirstOrDefault();
                        string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == SRODetails.DistrictCode).Select(x => x.DistrictNameE).FirstOrDefault();
                        refundChallanViewModel.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(SRODetails.DistrictCode) });
                        refundChallanViewModel.SROfficeList.Add(new SelectListItem() { Text = SRODetails.SRONameE, Value = kaveriCode.ToString() });
                        refundChallanViewModel.IsSRLogin = true;
                    }
                    else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                    {
                        string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == kaveriCode).Select(x => x.DistrictNameE).FirstOrDefault();
                        refundChallanViewModel.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(kaveriCode) });
                        refundChallanViewModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(kaveriCode, "All");
                        refundChallanViewModel.IsDRLogin = true;
                    }
                    else
                    {
                        refundChallanViewModel.IsSRLogin = false;
                        refundChallanViewModel.IsDRLogin = false;
                        refundChallanViewModel.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                        refundChallanViewModel.DROfficeList = objCommon.GetDROfficesList("All");
                    }
                    return refundChallanViewModel;
                }
            }
            catch (Exception)
            {
                throw;
            }
            //try
            //{
            //    RefundChallanViewModel refundChallanViewModel = new RefundChallanViewModel();

            //    refundChallanViewModel.SROfficeList = new List<SelectListItem>();
            //    refundChallanViewModel.DROfficeList = new List<SelectListItem>();
            //    using (dbContext = new KaveriEntities())
            //    {
            //        int kaveriCode = dbContext.MAS_OfficeMaster.Where(m => m.OfficeID == officeID).Select(m => m.Kaveri1Code).FirstOrDefault();

            //        if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
            //        {
            //            var SRODetails = dbContext.SROMaster.Where(x => x.SROCode == kaveriCode).Select(x => new { x.DistrictCode, x.SRONameE }).FirstOrDefault();
            //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == SRODetails.DistrictCode).Select(x => x.DistrictNameE).FirstOrDefault();
            //            refundChallanViewModel.SROCode = kaveriCode;
            //            refundChallanViewModel.SROName = SRODetails.SRONameE;
            //            refundChallanViewModel.DROCode = Convert.ToInt32(SRODetails.DistrictCode);
            //            refundChallanViewModel.DROName = DroName;
            //            refundChallanViewModel.IsSROrDRLogin = true;
            //        }
            //        else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
            //        {
            //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == kaveriCode).Select(x => x.DistrictNameE).FirstOrDefault();
            //            refundChallanViewModel.DROName = DroName;
            //            refundChallanViewModel.DROCode = kaveriCode;
            //            refundChallanViewModel.IsSROrDRLogin = true;
            //        }
            //        else
            //        {
            //            refundChallanViewModel.IsSROrDRLogin = false;
            //            refundChallanViewModel.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
            //            refundChallanViewModel.DROfficeList = objCommon.GetDROfficesList("All");
            //        }

            //        return refundChallanViewModel;
            //    }
            //}
            //catch (Exception)
            //{   
            //    throw;
            //}
        }


        // For Loading the Refund Challan Entry Table
        public RefundChallanResultModel LoadRefundChallanDetailsTable(int DROCode, int SROCode, int RoleID)
        {
            long SrNo = 0;
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    RefundChallanResultModel refundChallanResultModel = new RefundChallanResultModel();
                    refundChallanResultModel.refundChallanTableList = new List<RefundChallanTableModel>();

                    //List<RefundChallanTableModel> refundChallanTableList = new List<RefundChallanTableModel>();


                    List<USP_AMS_RefundChallanDetails_LIST_Result> refundChallanEntryList = dbContext.USP_AMS_RefundChallanDetails_LIST(DROCode, SROCode).ToList();

                    if (RoleID == (int)ApiCommonEnum.RoleDetails.DR)
                    {
                        refundChallanEntryList = refundChallanEntryList.Where(m => m.SROCode == 0).ToList();
                    }
                    else if(RoleID == (int)ApiCommonEnum.RoleDetails.SR)
                    {
                        refundChallanEntryList = refundChallanEntryList.Where(m => m.SROCode != 0).ToList();
                    }
                    

                    foreach (var refundChallanDetails in refundChallanEntryList)
                    {
                        SrNo++;
                        RefundChallanTableModel refundChallaTableModel = new RefundChallanTableModel();
                        
                        var DateOfInstrumentDate = ((Convert.ToDateTime(refundChallanDetails.InstrumentDate)).ToShortDateString()).ToString();

                        refundChallaTableModel.RowId = refundChallanDetails.RowId;
                        refundChallaTableModel.SrNo = SrNo;
                        refundChallaTableModel.DROName = refundChallanDetails.DRONAME;
                        refundChallaTableModel.SROName = refundChallanDetails.SROName;
                        refundChallaTableModel.InstrumentNumber = refundChallanDetails.InstrumentNumber;
                        refundChallaTableModel.InstrumentDate = DateOfInstrumentDate;
                        refundChallaTableModel.ChallanAmount = Convert.ToDecimal(refundChallanDetails.ChallanAmount);
                        refundChallaTableModel.RefundAmount = Convert.ToDecimal(refundChallanDetails.RefundAmount);
                        refundChallaTableModel.PartyName = refundChallanDetails.PartyName;
                        refundChallaTableModel.PartyMobileNumber = refundChallanDetails.PartyMobileNumber;


                        if (RoleID == (int)ApiCommonEnum.RoleDetails.DR || RoleID == (int)ApiCommonEnum.RoleDetails.SR)
                        {
                            if (refundChallanDetails.IsFinalized == false)
                            {
                                refundChallaTableModel.DROrderNumber = "";
                                refundChallaTableModel.DROrderDate = "";
                                refundChallaTableModel.ViewBtn = "";
                                refundChallaTableModel.Action = "<button class='btn btn-success' id='btnEdit'  OnClick='RefundChallanAddEditDetails(\"" + refundChallanDetails.RowId + "\")' data-toggle='tooltip'  title='Edit DR Order'>Edit</button>";
                                refundChallaTableModel.ChallanEntryStatus = "Not Submitted";
                                refundChallaTableModel.DRApprovalStatus = "";
                            }
                            else
                            {
                                refundChallaTableModel.ChallanEntryStatus = "Submitted for DR Approval";
                                refundChallaTableModel.Action = "<i class=\"fa fa-ban\" aria-hidden=\"true\" style=\"color: #065F8F\"></i>";

                                if (refundChallanDetails.IsDRApproved == true)
                                {
                                    refundChallaTableModel.DROrderNumber = refundChallanDetails.OrderNumber;
                                    refundChallaTableModel.DROrderDate = refundChallanDetails.OrderDate == null ? string.Empty : ((DateTime)refundChallanDetails.OrderDate).ToString("dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    refundChallaTableModel.ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + refundChallanDetails.RowId + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i>\"" + refundChallanDetails.FileName + "\"</a>";
                                    refundChallaTableModel.DRApprovalStatus = "Approved";
                                }
                                else if (refundChallanDetails.IsDRApproved == false)
                                {
                                    refundChallaTableModel.DROrderNumber = "";
                                    refundChallaTableModel.DROrderDate = "";
                                    refundChallaTableModel.ViewBtn = "";
                                    refundChallaTableModel.DRApprovalStatus = "Rejected";
                                }
                                else
                                {
                                    refundChallaTableModel.DRApprovalStatus = "In Process";
                                }   
                            }
                        }


                        else
                        {
                            refundChallaTableModel.Action = "<i class=\"fa fa-ban\" aria-hidden=\"true\" style=\"color: #065F8F\"></i>";

                            if (refundChallanDetails.IsFinalized == false)
                            {
                                refundChallaTableModel.DROrderNumber = "";
                                refundChallaTableModel.DROrderDate = "";
                                refundChallaTableModel.ViewBtn = "";
                                refundChallaTableModel.ChallanEntryStatus = "Not Submitted";
                                refundChallaTableModel.DRApprovalStatus = "";
                            }
                            else
                            {
                                refundChallaTableModel.ChallanEntryStatus = "Submitted for DR Approval";

                                if (refundChallanDetails.IsDRApproved == true)
                                {
                                    refundChallaTableModel.DROrderNumber = refundChallanDetails.OrderNumber;
                                    refundChallaTableModel.DROrderDate = refundChallanDetails.OrderDate == null ? string.Empty : ((DateTime)refundChallanDetails.OrderDate).ToString("dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    refundChallaTableModel.ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + refundChallanDetails.RowId + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i>\"" + refundChallanDetails.FileName + "\"</a>";
                                    refundChallaTableModel.DRApprovalStatus = "Approved";
                                }
                                else if (refundChallanDetails.IsDRApproved == false)
                                {
                                    refundChallaTableModel.DROrderNumber = "";
                                    refundChallaTableModel.DROrderDate = "";
                                    refundChallaTableModel.ViewBtn = "";
                                    refundChallaTableModel.DRApprovalStatus = "Rejected";
                                }
                                else
                                {
                                    refundChallaTableModel.DRApprovalStatus = "In Process";
                                }

                            }
                        }
                        
                        refundChallaTableModel.SROCode = Convert.ToInt16(refundChallanDetails.SROCode);
                        refundChallaTableModel.DROCode = Convert.ToInt16(refundChallanDetails.DROCode);

                        refundChallanResultModel.refundChallanTableList.Add(refundChallaTableModel);
                    }
                    return refundChallanResultModel;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        // For Loading the edit page of Refund Challan
        public RefundChallanViewModel RefundChallanAddEditDetails(long RowId) 
        {
            try
            {
                RefundChallanViewModel refundChallanViewModel = new RefundChallanViewModel();
                AMS_RefundChallanDetails refundChallanDetails = new AMS_RefundChallanDetails();
                AMS_RefundChallanUsageDetails refundChallanUsageDetails = new AMS_RefundChallanUsageDetails();
                
                using (dbContext = new KaveriEntities())
                {
                    refundChallanViewModel.ChallanPurposeList = new List<SelectListItem>();

                    //It Loads Previous Saved Data 
                    if (RowId != 0)
                    {
                        refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == RowId).FirstOrDefault();

                        refundChallanViewModel.RowId = refundChallanDetails.RowId;
                        refundChallanViewModel.InstrumentNumber = Convert.ToString(refundChallanDetails.InstrumentNumber);
                        refundChallanViewModel.ReEnterInstrumentNumber = Convert.ToString(refundChallanDetails.InstrumentNumber);

                        refundChallanViewModel.InstrumentDate = refundChallanDetails.InstrumentDate == null ? string.Empty : ((DateTime)refundChallanDetails.InstrumentDate).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        refundChallanViewModel.ChallanAmount = Convert.ToDecimal(refundChallanDetails.ChallanAmount);
                        refundChallanViewModel.RefundAmount = Convert.ToDecimal(refundChallanDetails.RefundAmount);
                        refundChallanViewModel.PartyName = refundChallanDetails.PartyName;

                        refundChallanViewModel.ApplicationDateTime = refundChallanDetails.ApplicationDateTime == null ? string.Empty : ((DateTime)refundChallanDetails.ApplicationDateTime).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        refundChallanViewModel.PartyMobileNumber = refundChallanDetails.PartyMobileNumber;
                        refundChallanViewModel.IsFinalized = refundChallanDetails.IsFinalized;
                        refundChallanViewModel.IsOrderInEditMode = true;
                        
                        List<int> ChallanPurposeCodeList = dbContext.AMS_RefundChallanUsageDetails.Where(x => x.RowId == RowId).Select(y => y.FeeRuleCode).ToList();

                        var RefundChallanPurposeList = dbContext.FeesRuleMaster.Select(m => new { m.FeeRuleCode, m.DescriptionE }).ToList();


                        foreach (var item in RefundChallanPurposeList)
                        {
                            SelectListItem objChallanPurpose = new SelectListItem();

                            if (ChallanPurposeCodeList.Contains(item.FeeRuleCode))
                                objChallanPurpose.Selected = true;

                            objChallanPurpose.Text = item.DescriptionE;
                            objChallanPurpose.Value = item.FeeRuleCode.ToString();

                            refundChallanViewModel.ChallanPurposeList.Add(objChallanPurpose);
                        }
                        
                    }

                    //It Loads new RefundChallan Entry for Saving Data
                    else
                    {
                        refundChallanViewModel.IsOrderInEditMode = false;
                        refundChallanViewModel.IsFinalized = false;

                        refundChallanViewModel.ChallanPurposeId = dbContext.FeesRuleMaster.Select(c => c.FeeRuleCode).ToArray();

                        List<SelectListItem> list = (from FRM in dbContext.FeesRuleMaster
                                                     select new SelectListItem()
                                                     {
                                                         Text = FRM.DescriptionE.ToString(),
                                                         Value = FRM.FeeRuleCode.ToString(),
                                                         Selected = false,
                                                     }).ToList();

                        refundChallanViewModel.ChallanPurposeList = list;
                    }
                    return refundChallanViewModel;
                }
            }
            catch (Exception ex)
            {
                //ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }



        // For Saving Refund Challan Details
        public RefundChallanOrderResultModel SaveRefundChallanDetails(RefundChallanViewModel refundChallanViewModel) 
        {
            RefundChallanOrderResultModel refundChallanResultModel = new RefundChallanOrderResultModel();
            try
            {
                //Added by BShivam on 31-03-2023 for double verification of challan payment details on khajane2 service
                string isChallanDoubleVerificationEnabled = ConfigurationManager.AppSettings["EnableKhajane2ChallanDBLVerification"];
                string erroMessage = null;


                if ((!string.IsNullOrEmpty(isChallanDoubleVerificationEnabled)) && (isChallanDoubleVerificationEnabled.ToLower() == "true"))
                {
                    bool isChallanVerified = ApiCommonFunctions.ChallanDoubleVerification(refundChallanViewModel.InstrumentNumber.Trim(),refundChallanViewModel.ChallanAmount,refundChallanViewModel.UserID, ref erroMessage);
               
                    if(!isChallanVerified)
                    {
                        refundChallanResultModel.ErrorMessage = erroMessage;
                        return refundChallanResultModel;
                    }
                }
                //Ended by BShivam on 31-03-2023

                
                using (dbContext = new KaveriEntities())
                {

                    //EcDataService.ECDataService ecDataService = new EcDataService.ECDataService();
                    //var IsChallanNoExists = ecDataService.CHECK_InstrumentDetails_Exists_Refund(refundChallanViewModel.InstrumentNumber);
                    
                    AMS_RefundChallanDetails refundChallanDetails = new AMS_RefundChallanDetails();

                    var ChallanNoExistsInDBList = dbContext.USP_AMS_CHECK_InstrumentDetails_Exists_Refund(refundChallanViewModel.InstrumentNumber, true).ToList();

                    if (ChallanNoExistsInDBList.Count != 0)
                    {
                        // This Checks that is Challan Exists in AMS_RefundChallanDetails table
                        var value = ChallanNoExistsInDBList.FirstOrDefault();

                        if (value.TableName == "AMS_BankInstrumentNumberDetails")
                        {
                            // This Checks is multiple Challans are present in AMS_BankInstrumentNumberDetails Table if present then show all 
                            if (ChallanNoExistsInDBList.Count > 1)
                            {
                                for (int index = 0; index < ChallanNoExistsInDBList.Count; index++)
                                {
                                    refundChallanResultModel.ErrorMessage += "Challan is Already used in " + ChallanNoExistsInDBList[index].OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                }
                            }
                            else
                            {
                                refundChallanResultModel.ErrorMessage = "Challan is Already used in " + value.OfficeName + ".";
                            }
                            return refundChallanResultModel;
                        }

                        // This Checks is Challan Exists in AMS_RefundChallanDetails table
                        else if (value.TableName == "AMS_RefundChallanDetails") 
                        {
                            // This Checks is multiple Challans are present in AMS_RefundChallanDetails Table if present then show all     
                            if (ChallanNoExistsInDBList.Count > 1)
                            {
                                for (int index = 0; index < ChallanNoExistsInDBList.Count; index++)
                                {
                                    if (ChallanNoExistsInDBList[index].IsDRApproved == null)
                                    {
                                        refundChallanResultModel.ErrorMessage += "Challan is applied for Refund at " + ChallanNoExistsInDBList[index].OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                    else if(ChallanNoExistsInDBList[index].IsDRApproved == true)
                                    {
                                        refundChallanResultModel.ErrorMessage += "Challan is Refunded at " + ChallanNoExistsInDBList[index].OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                    else
                                    {
                                        refundChallanResultModel.ErrorMessage += "Challan is Rejected for Refund at " + ChallanNoExistsInDBList[index].OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                }
                            }
                            else
                            {
                                if (value.IsDRApproved == null)
                                {
                                    refundChallanResultModel.ErrorMessage = "Challan is applied for Refund at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                }
                                else if(value.IsDRApproved == true)
                                {
                                    refundChallanResultModel.ErrorMessage = "Challan is Refunded at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                }
                                else
                                {
                                    refundChallanResultModel.ErrorMessage = "Challan is Rejected for Refund at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                }
                            }

                            return refundChallanResultModel;
                        }
                    }
                    
                    else
                    {
                        if (refundChallanViewModel.InstrumentNumber.Substring(0, 2) == "IG")
                        {
                            //Added By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.
                            ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();
                            var isDeptRefernceCodeExist = objService.GetChallanReferenceNoDetails(refundChallanViewModel.InstrumentNumber);

                            if (isDeptRefernceCodeExist == null)
                                throw new Exception("Exception occurred while getting challan Details from GetChallanReferenceNoDetails.");

                            if (!(isDeptRefernceCodeExist.IsDataExist))
                            {
                                refundChallanResultModel.ErrorMessage = "Challan reference number " + refundChallanViewModel.InstrumentNumber + " entered by User is not generated in Kaveri Online Services. Please verify Challan number.";
                                return refundChallanResultModel;
                            }
                            else if (isDeptRefernceCodeExist.IsDataExist)
                            {
                                if ((isDeptRefernceCodeExist.PaymentStatusCode.TrimEnd() != "10700066") && (isDeptRefernceCodeExist.PaymentStatusCode.TrimEnd() != "10700072"))
                                {
                                    refundChallanResultModel.ErrorMessage = "Payment is not successful for Challan reference number " + refundChallanViewModel.InstrumentNumber;
                                    return refundChallanResultModel;
                                }
                                else
                                {
                                    refundChallanResultModel = SaveOrUpdateRefundChallanDetails(refundChallanViewModel);
                                    return refundChallanResultModel;
                                }
                            }
                            //Added By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.

                            //Commented By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number
                            //ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();
                            //var isDeptRefernceCodeExist = objService.GetChallanReferenceNoDetails(refundChallanViewModel.InstrumentNumber);
                            //if (isDeptRefernceCodeExist.IsDataExist == true)
                            //{
                            //    if (isDeptRefernceCodeExist.StatusDesc == "Success")
                            //    {
                            //        refundChallanResultModel = SaveOrUpdateRefundChallanDetails(refundChallanViewModel);
                            //        return refundChallanResultModel;
                            //    }
                            //    else
                            //    {
                            //        refundChallanResultModel.ErrorMessage = "Payment Status is not Successfull for this Challan Number.";
                            //        return refundChallanResultModel;
                            //    }
                            //}
                            //else
                            //{
                            //    refundChallanResultModel.ErrorMessage = "This Challan number is not generated through Kaveri Online Services. Please verify Challan number.";
                            //    return refundChallanResultModel;
                            //}
                            //Commented By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number
                        }

                        else if (refundChallanViewModel.InstrumentNumber.Substring(0, 2) == "CR")
                        {
                            refundChallanResultModel = SaveOrUpdateRefundChallanDetails(refundChallanViewModel);
                            return refundChallanResultModel;
                        }
                    }
                }
                return refundChallanResultModel;
            }
            catch (DbEntityValidationException dbEx)
            {
                ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                refundChallanResultModel.ErrorMessage = "Entity Error Occured in saving Refund Challan Details.";
                return refundChallanResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }


        
        // For Updating Refund Challan Details
        public RefundChallanOrderResultModel UpdateRefundChallanDetails(RefundChallanViewModel refundChallanViewModel) 
        {
            RefundChallanOrderResultModel refundChallanResultModel = new RefundChallanOrderResultModel();
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    //EcDataService.ECDataService ecDataService = new EcDataService.ECDataService();
                    //var value1 = ecDataService.CHECK_InstrumentDetails_Exists_Refund(refundChallanViewModel.InstrumentNumber);
                    

                    AMS_RefundChallanDetails refundChallanDetails = new AMS_RefundChallanDetails();

                    var ChallanNoExistsInDBList = dbContext.USP_AMS_CHECK_InstrumentDetails_Exists_Refund(refundChallanViewModel.InstrumentNumber.Trim(), true).ToList();

                    if (ChallanNoExistsInDBList.Count != 0)
                    {
                        var value = ChallanNoExistsInDBList.FirstOrDefault();

                        // This Checks is Challan Exists in AMS_RefundChallanDetails table
                        if (value.TableName == "AMS_BankInstrumentNumberDetails") 
                        {
                            // This Checks is multiple Challans are present in AMS_BankInstrumentNumberDetails Table if present then show all 
                            if (ChallanNoExistsInDBList.Count > 1)
                            {
                                for (int index = 0; index < ChallanNoExistsInDBList.Count; index++)
                                {
                                    refundChallanResultModel.ErrorMessage += "Challan is Already used in " + ChallanNoExistsInDBList[index].OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                }
                            }
                            else
                            {
                                refundChallanResultModel.ErrorMessage = "Challan is Already used in " + value.OfficeName + ".";
                            }

                            return refundChallanResultModel;
                        }

                        else if (value.TableName == "AMS_RefundChallanDetails")
                        {
                            refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == refundChallanViewModel.RowId).FirstOrDefault();
                            
                            // For Updating The details in edit mode with not change in Challan Number when click update button
                            if (refundChallanViewModel.InstrumentNumber.Trim() == refundChallanDetails.InstrumentNumber.Trim()) 
                            {
                                if(refundChallanViewModel.InstrumentNumber.Substring(0, 2) == "IG")
                                {
                                    //Added By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.
                                    ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();
                                    var isDeptRefernceCodeExist = objService.GetChallanReferenceNoDetails(refundChallanViewModel.InstrumentNumber.Trim());

                                    if (isDeptRefernceCodeExist == null)
                                        throw new Exception("Exception occurred while getting challan Details from GetChallanReferenceNoDetails.");

                                    if (!(isDeptRefernceCodeExist.IsDataExist))
                                    {
                                        refundChallanResultModel.ErrorMessage = "Challan reference number " + refundChallanViewModel.InstrumentNumber + " entered by User is not generated in Kaveri Online Services. Please verify Challan number.";
                                        return refundChallanResultModel;
                                    }
                                    else if (isDeptRefernceCodeExist.IsDataExist)
                                    {
                                        if ((isDeptRefernceCodeExist.PaymentStatusCode.TrimEnd() != "10700066") && (isDeptRefernceCodeExist.PaymentStatusCode.TrimEnd() != "10700072"))
                                        {
                                            refundChallanResultModel.ErrorMessage = "Payment is not successful for Challan reference number " + refundChallanViewModel.InstrumentNumber;
                                            return refundChallanResultModel;
                                        }
                                        else
                                        {
                                            refundChallanResultModel = SaveOrUpdateRefundChallanDetails(refundChallanViewModel);
                                            return refundChallanResultModel;
                                        }
                                    }
                                    //Added By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.




                                    //Commented By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.
                                    //ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();
                                    //var isDeptRefernceCodeExist = objService.GetChallanReferenceNoDetails(refundChallanViewModel.InstrumentNumber.Trim());

                                    //if (isDeptRefernceCodeExist.IsDataExist == true)
                                    //{
                                    //    if (isDeptRefernceCodeExist.StatusDesc == "Success")
                                    //    {
                                    //        refundChallanResultModel = SaveOrUpdateRefundChallanDetails(refundChallanViewModel);
                                    //        return refundChallanResultModel;
                                    //    }
                                    //    else
                                    //    {
                                    //        refundChallanResultModel.ErrorMessage = "Payment Status is not Successfull for this Challan Number.";
                                    //        return refundChallanResultModel;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    refundChallanResultModel.ErrorMessage = "This Challan number is not generated through Kaveri Online Services. Please verify Challan number.";
                                    //    return refundChallanResultModel;
                                    //}
                                    //Commented By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.
                                }
                                else if(refundChallanViewModel.InstrumentNumber.Substring(0, 2) == "CR")
                                {
                                    refundChallanResultModel = SaveOrUpdateRefundChallanDetails(refundChallanViewModel);
                                    return refundChallanResultModel;    
                                }
                            }
                            else
                            {
                                
                                // Check Multiple Challan Entries present in  AMS_RefundChallanDetails Table
                                if (ChallanNoExistsInDBList.Count > 1)       
                                {
                                    for (int index = 0; index < ChallanNoExistsInDBList.Count; index++)
                                    {
                                        if (ChallanNoExistsInDBList[index].IsDRApproved == null)
                                        {
                                            refundChallanResultModel.ErrorMessage += "Challan is applied for Refund at " + ChallanNoExistsInDBList[index].OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                        }
                                        else if (ChallanNoExistsInDBList[index].IsDRApproved == true)
                                        {
                                            refundChallanResultModel.ErrorMessage += "Challan is Refunded at " + ChallanNoExistsInDBList[index].OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                        }
                                        else
                                        {
                                            refundChallanResultModel.ErrorMessage += "Challan is Rejected for Refund at " + ChallanNoExistsInDBList[index].OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                        }
                                    }
                                }
                                else
                                {
                                    if(value.IsDRApproved == null)
                                    {
                                        refundChallanResultModel.ErrorMessage = "Challan is applied for Refund at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                    else if(value.IsDRApproved == true)
                                    {
                                        refundChallanResultModel.ErrorMessage = "Challan is Refunded at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                    else
                                    {
                                        refundChallanResultModel.ErrorMessage = "Challan is Rejected for Refund at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                }
                            }
                        }
                    }

                    //For Updating the New Challan Number Entry in edit mode when changed in Challan Number
                    else
                    {
                        if (refundChallanViewModel.InstrumentNumber.Substring(0, 2) == "IG")
                        {

                            //Added By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.
                            ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();
                            var isDeptRefernceCodeExist = objService.GetChallanReferenceNoDetails(refundChallanViewModel.InstrumentNumber.Trim());

                            if (isDeptRefernceCodeExist == null)
                                throw new Exception("Exception occurred while getting challan Details from GetChallanReferenceNoDetails.");

                            if (!(isDeptRefernceCodeExist.IsDataExist))
                            {
                                refundChallanResultModel.ErrorMessage = "Challan reference number " + refundChallanViewModel.InstrumentNumber + " entered by User is not generated in Kaveri Online Services. Please verify Challan number.";
                                return refundChallanResultModel;
                            }
                            else if (isDeptRefernceCodeExist.IsDataExist)
                            {
                                if ((isDeptRefernceCodeExist.PaymentStatusCode.TrimEnd() != "10700066") && (isDeptRefernceCodeExist.PaymentStatusCode.TrimEnd() != "10700072"))
                                {
                                    refundChallanResultModel.ErrorMessage = "Payment is not successful for Challan reference number " + refundChallanViewModel.InstrumentNumber;
                                    return refundChallanResultModel;
                                }
                                else
                                {
                                    refundChallanResultModel = SaveOrUpdateRefundChallanDetails(refundChallanViewModel);
                                    return refundChallanResultModel;
                                }
                            }
                            //Added By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.



                            //Commented By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.
                            //ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();
                            //var isDeptRefernceCodeExist = objService.GetChallanReferenceNoDetails(refundChallanViewModel.InstrumentNumber.Trim());

                            //if (isDeptRefernceCodeExist.IsDataExist == true)
                            //{
                            //    if (isDeptRefernceCodeExist.StatusDesc == "Success")
                            //    {
                            //       refundChallanResultModel= SaveOrUpdateRefundChallanDetails(refundChallanViewModel);
                            //        return refundChallanResultModel;     
                            //    }
                            //    else
                            //    {
                            //        refundChallanResultModel.ErrorMessage = "Payment Status is not Successfull for this Challan Number.";
                            //        return refundChallanResultModel;
                            //    }
                            //}
                            //else
                            //{
                            //    refundChallanResultModel.ErrorMessage = "This Challan number is not generated through Kaveri Online Services. Please verify Challan number.";
                            //    return refundChallanResultModel;
                            //}
                            //Commented By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.

                        }
                        else if (refundChallanViewModel.InstrumentNumber.Substring(0, 2) == "CR")
                        {
                            refundChallanResultModel = SaveOrUpdateRefundChallanDetails(refundChallanViewModel);
                            return refundChallanResultModel;     
                        }
                    }

                    dbContext.SaveChanges();
                }

                return refundChallanResultModel;
            }
            catch (DbEntityValidationException dbEx)
            {
                ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                refundChallanResultModel.ErrorMessage = "Entity Error Occured in saving Refund Challan Details.";
                return refundChallanResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }



        public RefundChallanOrderResultModel SaveOrUpdateRefundChallanDetails(RefundChallanViewModel refundChallanViewModel)
        {
            RefundChallanOrderResultModel refundChallaResultModel = new RefundChallanOrderResultModel();
            try
            {
                AMS_RefundChallanDetails refundChallanDetails = new AMS_RefundChallanDetails();

                //This condition is For Updating Refund Challan Details
                if (refundChallanViewModel.RowId != 0)
                {
                    
                    refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(m => m.RowId == refundChallanViewModel.RowId).FirstOrDefault();
                    
                    //var title = ConfigurationManager.AppSettings["ChallanDateMonthLessThanSpecificMonth"];
                    //DateTime dtPreviousDate = DateTime.Now.AddMonths(-3);
                    //DateTime dtPreviousDate1 = DateTime.Now.AddMonths(-Convert.ToInt16(title));

                    refundChallanDetails.InstrumentNumber = refundChallanViewModel.InstrumentNumber.Trim();
                    refundChallanDetails.ChallanAmount = Convert.ToDecimal(refundChallanViewModel.ChallanAmount);
                    refundChallanDetails.InstrumentDate = Convert.ToDateTime(refundChallanViewModel.InstrumentDate.Trim());
                    refundChallanDetails.RefundAmount = Convert.ToDecimal(refundChallanViewModel.RefundAmount);
                    refundChallanDetails.PartyName = refundChallanViewModel.PartyName.Trim();
                    refundChallanDetails.ApplicationDateTime = Convert.ToDateTime(refundChallanViewModel.ApplicationDateTime.Trim());
                    refundChallanDetails.PartyMobileNumber = refundChallanViewModel.PartyMobileNumber.Trim();
                    
                    var refundChallanPurposeDetailsList = dbContext.AMS_RefundChallanUsageDetails.Where(x => x.RowId == refundChallanViewModel.RowId).ToList();
                    
                    //This is For Deleting previous entries of listbox and updating the new 
                    foreach (var deleteList in refundChallanPurposeDetailsList)
                    {
                        dbContext.AMS_RefundChallanUsageDetails.Remove(deleteList);
                        //dbContext.Entry(refundChallanDetails).State = System.Data.Entity.EntityState.Modified;
                    }

                    //this is for updating Challan Purpose list in AMS_RefundChallanUsageDetails table
                    for (int index = 0; index < refundChallanViewModel.ChallanPurposeId.Length; index++)
                    {
                        AMS_RefundChallanUsageDetails refundChallanUsageDetails = new AMS_RefundChallanUsageDetails();

                        var Id = Convert.ToInt32(dbContext.USP_GET_SEQID_AMS_GetSequenceRefundChallan(2).FirstOrDefault());
                        var ChallanPurposeId = refundChallanViewModel.ChallanPurposeId[index];
                        string ChallanPurposeDesc = dbContext.FeesRuleMaster.Where(x => x.FeeRuleCode == ChallanPurposeId).Select(m => m.DescriptionE).FirstOrDefault();

                        refundChallanUsageDetails.Id = Id;
                        refundChallanUsageDetails.RowId = refundChallanViewModel.RowId;
                        refundChallanUsageDetails.ChallanPurposeDescription = ChallanPurposeDesc;
                        refundChallanUsageDetails.FeeRuleCode = ChallanPurposeId;
                        dbContext.AMS_RefundChallanUsageDetails.Add(refundChallanUsageDetails);

                    }
                    refundChallaResultModel.ResponseMessage = "Details of Challan to be Refunded Updated Successfully.";
                    dbContext.Entry(refundChallanDetails).State = System.Data.Entity.EntityState.Modified;
                }

                

                //This condition is For Saving Refund Challan Details
                else
                {

                    int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == refundChallanViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                    var SRODetails = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => new { x.SROCode, x.DistrictCode, x.SRONameE }).FirstOrDefault();

                    var mas_OfficeMaster = (from OfficeMaster in dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == refundChallanViewModel.OfficeID)
                                            select new
                                            {
                                                OfficeMaster.LevelID,
                                                OfficeMaster.Kaveri1Code
                                            }).FirstOrDefault();


                    if (mas_OfficeMaster.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                    {
                        int SroCode = dbContext.SROMaster.Where(x => x.SROCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.SROCode).FirstOrDefault();
                        refundChallanViewModel.DROCode = Convert.ToInt16(SRODetails.DistrictCode);
                        refundChallanViewModel.SROCode = SRODetails.SROCode;
                    }
                    else
                    {
                        int DroCode = dbContext.DistrictMaster.Where(x => x.DistrictCode == mas_OfficeMaster.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault();
                        refundChallanViewModel.DROCode = DroCode;
                    }

                    //DateTime dtPreviousDate = DateTime.Now.AddMonths(-3);

                    var DefaultRoleId = dbContext.UMG_UserDetails.Where(x => x.UserID == refundChallanViewModel.UserID).Select(x => x.DefaultRoleID).FirstOrDefault();

                    if(DefaultRoleId == 1)
                    {
                        refundChallanDetails.SROUserId = refundChallanViewModel.UserID;
                    }
                    if(DefaultRoleId == 3)
                    {
                        refundChallanDetails.DROUserId = refundChallanViewModel.UserID;
                    }

                    var RowId = Convert.ToInt32(dbContext.USP_GET_SEQID_AMS_GetSequenceRefundChallan(1).FirstOrDefault());

                    refundChallanDetails.RowId = RowId;
                    refundChallanDetails.InstrumentNumber = refundChallanViewModel.InstrumentNumber.Trim();
                    refundChallanDetails.InstrumentDate = Convert.ToDateTime(refundChallanViewModel.InstrumentDate.Trim());
                    refundChallanDetails.ChallanAmount = Convert.ToDecimal(refundChallanViewModel.ChallanAmount);
                    refundChallanDetails.RefundAmount = Convert.ToDecimal(refundChallanViewModel.RefundAmount);
                    refundChallanDetails.PartyName = refundChallanViewModel.PartyName.Trim();
                    refundChallanDetails.ApplicationDateTime = Convert.ToDateTime(refundChallanViewModel.ApplicationDateTime.Trim());
                    refundChallanDetails.PartyMobileNumber = Convert.ToString(refundChallanViewModel.PartyMobileNumber.Trim());
                    refundChallanDetails.InsertDateTime = DateTime.Now;
                    refundChallanDetails.IsFinalized = false;
                
                    if (refundChallanViewModel.SROCode == 0)
                    {
                        refundChallanDetails.DROCode = Convert.ToInt32(refundChallanViewModel.DROCode);
                        refundChallanDetails.SROCode = 0;
                    }
                    else
                    {
                        refundChallanDetails.SROCode = Convert.ToInt32(refundChallanViewModel.SROCode);
                        refundChallanDetails.DROCode = Convert.ToInt32(refundChallanViewModel.DROCode);
                    }

                    //this is for saving Refund Purpose list Data in AMS_RefundChallanUsageDetails table
                    for (int index = 0; index < refundChallanViewModel.ChallanPurposeId.Length; index++)
                    {
                        AMS_RefundChallanUsageDetails refundChallanUsageDetails = new AMS_RefundChallanUsageDetails();

                        var Id = Convert.ToInt32(dbContext.USP_GET_SEQID_AMS_GetSequenceRefundChallan(2).FirstOrDefault());

                        var ChallanPurposeId = refundChallanViewModel.ChallanPurposeId[index];
                        string ChallanPurposeDesc = dbContext.FeesRuleMaster.Where(x => x.FeeRuleCode == ChallanPurposeId).Select(m => m.DescriptionE).FirstOrDefault();

                        refundChallanUsageDetails.Id = Id;
                        refundChallanUsageDetails.RowId = RowId;
                        refundChallanUsageDetails.ChallanPurposeDescription = ChallanPurposeDesc;
                        refundChallanUsageDetails.FeeRuleCode = ChallanPurposeId;
                        dbContext.AMS_RefundChallanUsageDetails.Add(refundChallanUsageDetails);
                    }

                    refundChallaResultModel.ResponseMessage = "Details of Challan to be Refunded Saved Successfully.";
                    dbContext.AMS_RefundChallanDetails.Add(refundChallanDetails);   
                }

                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        dbContext.SaveChanges();
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }

                return refundChallaResultModel;
            }

            catch (DbEntityValidationException dbEx)
            {
                ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                refundChallaResultModel.ErrorMessage = "Entity Error Occured in saving Refund Challan Details.";
                return refundChallaResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }

        
        // For Finalizing Refund Challan Details
        public string FinalizeRefundChallanDetails(long RowId)
        {
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    AMS_RefundChallanDetails refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == RowId).FirstOrDefault();

                    string finalizeSPResp;

                    if (refundChallanDetails != null)
                    {
                        refundChallanDetails.IsFinalized = true;
                        dbContext.Entry(refundChallanDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        finalizeSPResp = string.Empty;
                    }
                    else
                    {
                        finalizeSPResp = null;
                    }
                    return finalizeSPResp;
                }
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
        }

        
        public RefundChallanDROrderResultModel ViewBtnClickOrderTable(long RowId)
        {
            try
            {
                RefundChallanDROrderResultModel refundChallanDROrderResultModel = new RefundChallanDROrderResultModel();
                using (dbContext = new KaveriEntities())
                {
                    long Lng_FileID = Convert.ToInt64(RowId);
                    string filepath = dbContext.AMS_RefundChallanDetails.Where(y => y.RowId == RowId).Select(z => z.Relativepath).FirstOrDefault().ToString();
                    string MaintaincePortalVirtualSitePath = ConfigurationManager.AppSettings["MaintaincePortalVirtualSiteOrders"];

                    MaintaincePortalVirtualSitePath = MaintaincePortalVirtualSitePath + "//RefundChallan";

                    WebClient webClient = new WebClient();
                    Stream strm = webClient.OpenRead(new Uri(MaintaincePortalVirtualSitePath + "//" + filepath));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        strm.CopyTo(ms);
                        refundChallanDROrderResultModel.refundChallanApproveFileBytes = ms.ToArray();
                    }
                    return refundChallanDROrderResultModel;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        

    }
}