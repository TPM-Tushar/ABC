using CustomModels.Models.RefundChallan;
using ECDataAPI.Areas.RefundChallan.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.RefundChallan.DAL
{

    public class RefundChallanApproveDAL : IRefundChallanApprove
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService;

        public RefundChallanApproveViewModel RefundChallanApproveView(int officeID, int LevelID, long UserID)
        {
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    RefundChallanApproveViewModel refundChallanViewModel = new RefundChallanApproveViewModel();

                    refundChallanViewModel.SROfficeOrderList = new List<SelectListItem>();
                    refundChallanViewModel.DROfficeOrderList = new List<SelectListItem>();

                    int kaveriCode = dbContext.MAS_OfficeMaster.Where(m => m.OfficeID == officeID).Select(m => m.Kaveri1Code).FirstOrDefault();

                    if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                    {
                        var SRODetails = dbContext.SROMaster.Where(x => x.SROCode == kaveriCode).Select(x => new { x.DistrictCode, x.SRONameE }).FirstOrDefault();
                        string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == SRODetails.DistrictCode).Select(x => x.DistrictNameE).FirstOrDefault();
                        refundChallanViewModel.DROfficeOrderList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(SRODetails.DistrictCode) });
                        refundChallanViewModel.SROfficeOrderList.Add(new SelectListItem() { Text = SRODetails.SRONameE, Value = kaveriCode.ToString() });
                        refundChallanViewModel.IsSROrDRLogin = true;
                    }
                    else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                    {
                        string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == kaveriCode).Select(x => x.DistrictNameE).FirstOrDefault();
                        refundChallanViewModel.DROfficeOrderList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(kaveriCode) });
                        refundChallanViewModel.SROfficeOrderList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(kaveriCode, "All");
                        refundChallanViewModel.IsSROrDRLogin = true;
                    }
                    else
                    {
                        refundChallanViewModel.IsSROrDRLogin = false;
                        refundChallanViewModel.SROfficeOrderList.Add(new SelectListItem() { Text = "All", Value = "0" });
                        refundChallanViewModel.DROfficeOrderList = objCommon.GetDROfficesList("All");
                    }
                    return refundChallanViewModel;
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        

        public List<RefundChallanApproveTableModel> LoadRefundChallanApproveTable(int DROCode, int SROCode, int RoleID, bool IsExcel)
        {
            long SrNo = 0;
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    List<RefundChallanApproveTableModel> refundChallanApproveTableList = new List<RefundChallanApproveTableModel>();

                    List<USP_AMS_RefundChallanDetails_LIST_Result> refundChallanApproveList = dbContext.USP_AMS_RefundChallanDetails_LIST(DROCode, SROCode).ToList();

                    refundChallanApproveList = refundChallanApproveList.Where(m => m.IsFinalized == true).ToList();

                    foreach (var refundChallanDetails in refundChallanApproveList)
                    {
                        SrNo++;
                        RefundChallanApproveTableModel refundChallaOrderTableModel = new RefundChallanApproveTableModel();

                        refundChallaOrderTableModel.SrNo = SrNo;
                        refundChallaOrderTableModel.DROName = refundChallanDetails.DRONAME;
                        refundChallaOrderTableModel.SROName = refundChallanDetails.SROName;
                        refundChallaOrderTableModel.RowId = refundChallanDetails.RowId;
                        refundChallaOrderTableModel.InstrumentNumber = refundChallanDetails.InstrumentNumber;
                        refundChallaOrderTableModel.InstrumentDate = refundChallanDetails.InstrumentDate == null ? string.Empty : ((DateTime)refundChallanDetails.InstrumentDate).ToString("dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        refundChallaOrderTableModel.ChallanAmount = Convert.ToDecimal(refundChallanDetails.ChallanAmount);
                        refundChallaOrderTableModel.RefundAmount = Convert.ToDecimal(refundChallanDetails.RefundAmount);
                        refundChallaOrderTableModel.PartyName = refundChallanDetails.PartyName;
                        refundChallaOrderTableModel.PartyMobileNumber = refundChallanDetails.PartyMobileNumber;


                        if (RoleID == (int)ApiCommonEnum.RoleDetails.DR )
                        {
                            if (refundChallanDetails.IsFinalized == true)
                            {
                                if (refundChallanDetails.IsDRApproved == true)
                                {
                                    refundChallaOrderTableModel.DROrderNumber = refundChallanDetails.OrderNumber;
                                    refundChallaOrderTableModel.DROrderDate = refundChallanDetails.OrderDate == null ? string.Empty : ((DateTime)refundChallanDetails.OrderDate).ToString("dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                                    refundChallaOrderTableModel.ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + refundChallanDetails.RowId + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i>\"" + refundChallanDetails.FileName + "\"</a>";

                                    //refundChallaOrderTableModel.Action = " ";
                                    refundChallaOrderTableModel.Action = "<i class=\"fa fa-ban\" aria-hidden=\"true\" style=\"color: #065F8F\"></i>";
                                    refundChallaOrderTableModel.DRApprovalStatus = "Finalized (Approved)";
                                }
                                else if (refundChallanDetails.IsDRApproved == false)
                                {
                                    refundChallaOrderTableModel.DROrderNumber = "";
                                    refundChallaOrderTableModel.DROrderDate = "";
                                    refundChallaOrderTableModel.ViewBtn = "";
                                    //refundChallaOrderTableModel.Action = "";
                                    refundChallaOrderTableModel.Action = "<i class=\"fa fa-ban\" aria-hidden=\"true\" style=\"color: #065F8F\"></i>";
                                    refundChallaOrderTableModel.DRApprovalStatus = "Finalized (Rejected)";
                                }
                                else
                                {
                                    refundChallaOrderTableModel.DROrderNumber = refundChallanDetails.OrderNumber;
                                    refundChallaOrderTableModel.DROrderDate = refundChallanDetails.OrderDate == null ? string.Empty : ((DateTime)refundChallanDetails.OrderDate).ToString("dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                                    if (refundChallanDetails.RejectionReason == "")
                                    {
                                        refundChallaOrderTableModel.ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + refundChallanDetails.RowId + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i>\"" + refundChallanDetails.FileName + "\"</a>";
                                    }
                                    else
                                    {
                                        refundChallaOrderTableModel.ViewBtn = "";
                                    }

                                    refundChallaOrderTableModel.Action = "<button class='btn btn-success' id='btnEdit'  OnClick='RefundChallanApproveAddEditOrder(\"" + refundChallanDetails.RowId + "\")' data-toggle='tooltip'  title='Edit DR Order'>Edit</button>";

                                    refundChallaOrderTableModel.DRApprovalStatus = "Not Finalized";
                                }
                            }
                        }


                        else
                        {
                            if (refundChallanDetails.IsFinalized == true)
                            {
                                //refundChallaOrderTableModel.Action = "";
                                refundChallaOrderTableModel.Action = "<i class=\"fa fa-ban\" aria-hidden=\"true\" style=\"color: #065F8F\"></i>";
                                
                                if (refundChallanDetails.IsDRApproved == true)
                                {
                                    refundChallaOrderTableModel.DROrderNumber = refundChallanDetails.OrderNumber;
                                    refundChallaOrderTableModel.DROrderDate = refundChallanDetails.OrderDate == null ? string.Empty : ((DateTime)refundChallanDetails.OrderDate).ToString("dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                                    refundChallaOrderTableModel.ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + refundChallanDetails.RowId + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i>\"" + refundChallanDetails.FileName + "\"</a>";
                                    
                                    refundChallaOrderTableModel.DRApprovalStatus = "Finalized (Approved)";
                                }
                                else if (refundChallanDetails.IsDRApproved == false)
                                {
                                    refundChallaOrderTableModel.DROrderNumber = "";
                                    refundChallaOrderTableModel.DROrderDate = "";
                                    refundChallaOrderTableModel.ViewBtn = "";
                                    refundChallaOrderTableModel.DRApprovalStatus = "Finalized (Rejected)";
                                }
                                else
                                {
                                    refundChallaOrderTableModel.DROrderNumber = refundChallanDetails.OrderNumber;
                                    refundChallaOrderTableModel.DROrderDate = refundChallanDetails.OrderDate == null ? string.Empty : ((DateTime)refundChallanDetails.OrderDate).ToString("dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                                    if (refundChallanDetails.RejectionReason == "")
                                    {
                                        refundChallaOrderTableModel.ViewBtn = "<a href='javascript:void(0)' onClick='ViewBtnClickOrderTable(\"" + refundChallanDetails.RowId + "\")'><i class='fa fa-file-pdf-o' aria-hidden='true'></i>\"" + refundChallanDetails.FileName + "\"</a>";
                                    }
                                    else
                                    {
                                        refundChallaOrderTableModel.ViewBtn = "";
                                    }
                                    refundChallaOrderTableModel.DRApprovalStatus = "Not Finalized";
                                }
                            }
                        }


                        

                        refundChallaOrderTableModel.SROCode = Convert.ToInt16(refundChallanDetails.SROCode);
                        refundChallaOrderTableModel.DROCode = Convert.ToInt16(refundChallanDetails.DROCode);

                        refundChallanApproveTableList.Add(refundChallaOrderTableModel);
                    }

                    return refundChallanApproveTableList;
                }
                
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
        

        public RefundChallanApproveViewModel RefundChallanApproveAddEditOrder(long RowId, int OfficeID, int LevelID)
        {
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    RefundChallanApproveViewModel refundChallanViewModel = new RefundChallanApproveViewModel();

                    if (RowId != 0)
                    {
                        var result = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == RowId).FirstOrDefault();

                        refundChallanViewModel.RowId = result.RowId;
                        refundChallanViewModel.InstrumentNumber = Convert.ToString(result.InstrumentNumber);
                        refundChallanViewModel.InstrumentDate = result.InstrumentDate == null ? string.Empty : ((DateTime)result.InstrumentDate).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        refundChallanViewModel.DROrderNumber = result.OrderNumber;
                        refundChallanViewModel.DROrderNumberHidden = result.OrderNumber;
                        refundChallanViewModel.DROrderDate = result.OrderDate == null ? string.Empty : ((DateTime)result.OrderDate).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        refundChallanViewModel.ChallanAmount = Convert.ToDecimal(result.ChallanAmount);
                        refundChallanViewModel.RefundAmount = Convert.ToDecimal(result.RefundAmount);
                        refundChallanViewModel.PartyName = result.PartyName;
                        refundChallanViewModel.ApplicationDateTime = result.ApplicationDateTime == null ? string.Empty : ((DateTime)result.ApplicationDateTime).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        refundChallanViewModel.RejectionReason = result.RejectionReason;
                        refundChallanViewModel.RejectionReasonHidden = result.RejectionReason;
                        refundChallanViewModel.PartyMobileNumber = result.PartyMobileNumber;
                        refundChallanViewModel.IsFinalized = result.IsFinalized;
                        refundChallanViewModel.RelativeFilePath = result.Relativepath;
                        refundChallanViewModel.ExistingFileName = result.FileName;

                        if (result.OrderNumber == null && result.OrderDate == null && result.RejectionReason == null)
                        {
                            refundChallanViewModel.IsOrderInEditMode = false;
                        }
                        else
                        {
                            refundChallanViewModel.IsOrderInEditMode = true;
                        }

                        refundChallanViewModel.ChallanPurposeList = new List<SelectListItem>();

                        List<int> ChallanPurposeList = dbContext.AMS_RefundChallanUsageDetails.Where(x => x.RowId == RowId).Select(y => y.FeeRuleCode).ToList();

                        var RefundChallanPurposeList = dbContext.FeesRuleMaster.Select(m => new { m.FeeRuleCode, m.DescriptionE }).ToList();


                        foreach (var challanList in RefundChallanPurposeList)
                        {
                            SelectListItem objChallanPurpose = new SelectListItem();

                            if (ChallanPurposeList.Contains(challanList.FeeRuleCode))
                            {
                                objChallanPurpose.Selected = true;
                                objChallanPurpose.Disabled = true;
                                objChallanPurpose.Text = challanList.DescriptionE;
                                objChallanPurpose.Value = challanList.FeeRuleCode.ToString();
                                refundChallanViewModel.ChallanPurposeList.Add(objChallanPurpose);
                            }
                        }
                        //refundChallanViewModel.ChallanPurposeList = dbContext.AMS_RefundChallanUsageDetails.Where(x => x.RowId == RowId).Select(x => x.ChallanPurposeDescription).ToList();

                    }
                    else
                    {
                        refundChallanViewModel.IsOrderInEditMode = false;
                        refundChallanViewModel.IsFinalized = false;
                    }
                    return refundChallanViewModel;
                }
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
            
        }
        

        public RefundChallanOrderResultModel IsChallanNoExist(string InstrumentNumber, string InstrumentDate, long RowId)
        {
            try
            {
                using (dbContext = new KaveriEntities())
                {

                    RefundChallanOrderResultModel refundChallanResultModel = new RefundChallanOrderResultModel();
                    AMS_RefundChallanDetails refundChallanDetails = new AMS_RefundChallanDetails();
                    
                    var ChallanNoExistsInDBList = dbContext.USP_AMS_CHECK_InstrumentDetails_Exists_Refund(InstrumentNumber, true).ToList();

                    if (ChallanNoExistsInDBList.Count != 0)
                    {
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
                                return refundChallanResultModel;
                            }
                            else
                            {
                                refundChallanResultModel.ErrorMessage = "Challan is Already used in " + value.OfficeName + ".";
                                return refundChallanResultModel;
                            }

                        }
                        else if (value.TableName == "AMS_RefundChallanDetails")
                        {
                            var ChallanDetails = (from ARC in dbContext.AMS_RefundChallanDetails
                                                  where ARC.RowId != RowId && ARC.InstrumentNumber == InstrumentNumber
                                                  select ARC).ToList();
                            
                            if (ChallanDetails.Count == 0)
                            {
                                refundChallanResultModel.ResponseMessage = "Challan reference number " + InstrumentNumber + " is not used in KAVERI application and not captured for refund till now.";
                                return refundChallanResultModel;
                            }
                            else if (ChallanNoExistsInDBList.Count > 1)
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
                                if (InstrumentNumber.Substring(0, 2) == "IG")
                                {
                                    //Added By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.
                                    ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();
                                    var isDeptRefernceCodeExist = objService.GetChallanReferenceNoDetails(InstrumentNumber);

                                    if (isDeptRefernceCodeExist == null)
                                        throw new Exception("Exception occurred while getting challan Details from GetChallanReferenceNoDetails.");

                                    if (!(isDeptRefernceCodeExist.IsDataExist))
                                    {
                                        refundChallanResultModel.ErrorMessage = "Challan reference number " + InstrumentNumber + " entered by User is not generated in Kaveri Online Services. Please verify Challan number.";
                                        return refundChallanResultModel;
                                    }
                                    else if (isDeptRefernceCodeExist.IsDataExist)
                                    {
                                        if ((isDeptRefernceCodeExist.PaymentStatusCode.TrimEnd() != "10700066") && (isDeptRefernceCodeExist.PaymentStatusCode.TrimEnd() != "10700072"))
                                        {
                                            refundChallanResultModel.ErrorMessage = "Payment is not successful for Challan reference number " + InstrumentNumber;
                                            return refundChallanResultModel;
                                        }
                                        else
                                        {
                                            refundChallanResultModel.ResponseMessage = "Challan reference number " + InstrumentNumber + " is not used in KAVERI application and not captured for refund till now.";
                                            return refundChallanResultModel;
                                        }
                                    }
                                    //Added By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number.



                                    //Commented By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number
                                    //ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();

                                    //var isDeptRefernceCodeExist = objService.GetChallanReferenceNoDetails(InstrumentNumber);

                                    //if (isDeptRefernceCodeExist.IsDataExist == true)
                                    //{
                                    //    if (isDeptRefernceCodeExist.StatusDesc != "Success")
                                    //    {
                                    //        refundChallanResultModel.ErrorMessage = "Payment Status is not Successfull for this Challan Number.";
                                    //        return refundChallanResultModel;
                                    //    }
                                    //    else
                                    //    {
                                    //        refundChallanResultModel.ResponseMessage = "This Challan is not used in KAVERI application and not captured for refund till now.";
                                    //        return refundChallanResultModel;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    refundChallanResultModel.ErrorMessage = "This Challan number is not generated through Kaveri Online Services. Please verify Challan number.";
                                    //}
                                    //Commented By ShivamB on 02-02-2023 for checking paymentStatusCode of IG Challan Number

                                }
                                else if (InstrumentNumber.Substring(0, 2) == "CR")
                                {
                                    // This Checks is multiple Challans are present in AMS_RefundChallanDetails Table if present then show all     
                                    if (value.IsDRApproved == null)
                                    {
                                        refundChallanResultModel.ErrorMessage = "Challan is applied for Refund at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                    else if (value.IsDRApproved == true)
                                    {
                                        refundChallanResultModel.ErrorMessage = "Challan is Refunded at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                    else
                                    {
                                        refundChallanResultModel.ErrorMessage = "Challan is Rejeceted for Refund at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                    return refundChallanResultModel;
                                }
                            }
                        
                        }
                        
                        return refundChallanResultModel;
                    }
                    else
                    {
                        refundChallanResultModel.ResponseMessage = "Error While Getting Refund Challan Details.";
                        return refundChallanResultModel;
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }


        public bool CheckifOrderNoExist(string OrderNo, long RowId)
        {
            try
            {
                using(dbContext = new KaveriEntities())
                {
                    return dbContext.AMS_RefundChallanDetails.Where(x => x.OrderNumber == OrderNo && x.RowId != RowId).Any();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }


        public RefundChallanOrderResultModel SaveRefundChallanApproveDetails(RefundChallanApproveViewModel refundChallanViewModel)
        {

            RefundChallanOrderResultModel refundChallanResultModel = new RefundChallanOrderResultModel();
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    AMS_RefundChallanDetails refundChallanDetails = new AMS_RefundChallanDetails();

                    //This is for Edit Mode (IsInEditMode)

                    var ChallanNoExistsInDBList = dbContext.USP_AMS_CHECK_InstrumentDetails_Exists_Refund(refundChallanViewModel.InstrumentNumber.Trim(), true).ToList();

                    //Check is Challan Number which is come from Database is present in DB 
                    if (ChallanNoExistsInDBList.Count != 0)
                    {
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
                        else if (value.TableName == "AMS_RefundChallanDetails")
                        {
                            refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == refundChallanViewModel.RowId).FirstOrDefault();
                            
                            if (refundChallanViewModel.InstrumentNumber.Trim() == refundChallanDetails.InstrumentNumber.Trim())
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
                                            refundChallanResultModel = SaveApproveDetails(refundChallanViewModel);
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
                                    //        refundChallanResultModel = SaveApproveDetails(refundChallanViewModel);
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
                                    refundChallanResultModel = SaveApproveDetails(refundChallanViewModel);
                                    return refundChallanResultModel;
                                }
                            }
                            else
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
                                    if (value.IsDRApproved == null)
                                    {
                                        refundChallanResultModel.ErrorMessage = "Challan is applied for Refund at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                    else if (value.IsDRApproved == true)
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

                        dbContext.SaveChanges();

                    }
                    else
                    {
                        refundChallanResultModel.ErrorMessage = "Entity Error Occured in saving Refund Challan Order Details.";
                        return refundChallanResultModel;
                    }
                    
                    return refundChallanResultModel;
                }

                
            }
            catch (DbEntityValidationException dbEx)
            {
                ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                refundChallanResultModel.ErrorMessage = "Entity Error Occured in saving Refund Challan Order Details.";
                return refundChallanResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
            
        }


        public RefundChallanOrderResultModel SaveApproveDetails(RefundChallanApproveViewModel refundChallanViewModel)
        {
            RefundChallanOrderResultModel refundChallanResultModel = new RefundChallanOrderResultModel();
            try
            {
                AMS_RefundChallanDetails refundChallanDetails = new AMS_RefundChallanDetails();

                refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == refundChallanViewModel.RowId).FirstOrDefault();
                
                if(refundChallanDetails !=null)
                {
					//Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    #region 
                    //Added on 27-07-2022 For checking if DR Order pdf is exists or not in the folder path and if exists then save the details.

                    //string rootPath = ConfigurationManager.AppSettings["MaintaincePortalVirtualOrdersDirectoryPath"];
                    //rootPath = rootPath + "\\RefundChallan";

                    //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == refundChallanViewModel.OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                    //string districtTriLetter = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.ShortNameE).FirstOrDefault();
                    
                    //var insertDateTime = Convert.ToDateTime(refundChallanDetails.InsertDateTime);

                    //string finYear;
                    //int month = insertDateTime.Month;

                    //if (month > 3)
                    //{
                    //    finYear = Convert.ToString(insertDateTime.Year) + "-" + Convert.ToString(insertDateTime.AddYears(1).Year).Substring(2, 2);
                    //}
                    //else
                    //{
                    //    finYear = Convert.ToString(insertDateTime.Year - 1) + "-" + Convert.ToString(insertDateTime.Year).Substring(2, 2);
                    //}
                    //string fileName = "DROrder_" + refundChallanViewModel.RowId + ".pdf";

                    //var FilePath = rootPath + "\\" + districtTriLetter + "\\" + finYear + "\\" + refundChallanViewModel.RowId + "\\" + fileName;

                    
                    //if (System.IO.File.Exists(FilePath))
					//Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
					
                    if (System.IO.File.Exists(refundChallanViewModel.FilePath) || System.IO.File.Exists(refundChallanDetails.AbsoluteFilePath))  //System.IO.File.Exists(refundChallanDetails.AbsoluteFilePath) is Added by ShivamB to update DROrderNo and DROrderDate only on 07/09/2022
                    {
                        //refundChallanDetails.DROUserId = refundChallanViewModel.UserID;
                        refundChallanDetails.OrderNumber = refundChallanViewModel.DROrderNumber;
                        refundChallanDetails.OrderDate = Convert.ToDateTime(refundChallanViewModel.DROrderDate.Trim());
                        refundChallanDetails.ApproveDateTime = DateTime.Now;

                        if (refundChallanViewModel.FilePath != null)
                        {
                            refundChallanDetails.AbsoluteFilePath = refundChallanViewModel.FilePath.Trim();
                            refundChallanDetails.Relativepath = refundChallanViewModel.RelativeFilePath.Trim();
                            refundChallanDetails.FileName = refundChallanViewModel.ExistingFileName.Trim();
                            refundChallanDetails.RejectionReason = "";

                        }
                        refundChallanResultModel.ResponseMessage = "Details of Challan to be Refunded Saved Successfully.";
                        dbContext.Entry(refundChallanDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        refundChallanResultModel.ErrorMessage = "DR Order pdf is not uploaded in the directory folder path.";
                    }

                    //Added on 20/07-2022 For checking if DR Order pdf is exists or not in the folder path and if exists then save the details.
                    #endregion

                }
                else
                {
                    refundChallanResultModel.ErrorMessage = "Order Details Not found.";
                }
                
                return refundChallanResultModel;
            }
            catch (DbEntityValidationException dbEx)
            {
                ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                refundChallanResultModel.ErrorMessage = "Entity Error Occured in saving Refund Challan Order Details.";
                return refundChallanResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }


        public RefundChallanOrderResultModel SaveRefundChallanRejectionDetails(RefundChallanRejectionViewModel refundChallanViewModel)
        {

            RefundChallanOrderResultModel refundChallanResultModel = new RefundChallanOrderResultModel();
            try
            {
                using (dbContext = new KaveriEntities())
                {

                    AMS_RefundChallanDetails refundChallanDetails = new AMS_RefundChallanDetails();
                    
                    var ChallanNoExistsInDBList = dbContext.USP_AMS_CHECK_InstrumentDetails_Exists_Refund(refundChallanViewModel.InstrumentNumber.Trim(), true).ToList();

                    //Check is Challan Number which is come from Database is present in DB 
                    if (ChallanNoExistsInDBList.Count != 0)
                    {
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
                        else if (value.TableName == "AMS_RefundChallanDetails")
                        {

                            refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == refundChallanViewModel.RowId).FirstOrDefault();
                            
                            if (refundChallanDetails.InstrumentNumber.Trim() == refundChallanViewModel.InstrumentNumber.Trim())
                            {
                                if (refundChallanDetails.InstrumentNumber.Substring(0, 2) == "IG")
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
                                            refundChallanResultModel = SaveRejectionDetails(refundChallanViewModel);
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
                                    //        refundChallanResultModel = SaveRejectionDetails(refundChallanViewModel);
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

                                else if (refundChallanDetails.InstrumentNumber.Substring(0, 2) == "CR")
                                {
                                    refundChallanResultModel = SaveRejectionDetails(refundChallanViewModel);
                                    return refundChallanResultModel;
                                }
                            }
                            else
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
                                    else
                                    {
                                        refundChallanResultModel.ErrorMessage = "Challan is Rejected for Refund at " + value.OfficeName + ".<br/>&nbsp &nbsp &nbsp &nbsp";
                                    }
                                }

                                return refundChallanResultModel;
                            }
                        }
                    }
                    
                }
                return refundChallanResultModel;
            }

            catch (DbEntityValidationException dbEx)
            {
                ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                refundChallanResultModel.ErrorMessage = "Entity Error Occured in saving Order Details.";
                return refundChallanResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
            
        }


        public RefundChallanOrderResultModel SaveRejectionDetails(RefundChallanRejectionViewModel refundChallanViewModel)
        {

            RefundChallanOrderResultModel refundChallanResultModel = new RefundChallanOrderResultModel();

            try
            {
                AMS_RefundChallanDetails refundChallanDetails = new AMS_RefundChallanDetails();

                refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == refundChallanViewModel.RowId).FirstOrDefault();
                
                refundChallanDetails.OrderNumber = null;
                refundChallanDetails.OrderDate = null;
                refundChallanDetails.FileName = "";
                refundChallanDetails.Relativepath = "";
                refundChallanDetails.AbsoluteFilePath = "";
                refundChallanDetails.ApproveDateTime = DateTime.Now;
                refundChallanDetails.RejectionReason = refundChallanViewModel.RejectionReason;

                refundChallanResultModel.ResponseMessage = "Details of Challan to be Refunded Saved Successfully.";
                dbContext.Entry(refundChallanDetails).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();

                return refundChallanResultModel;
            }
            catch (DbEntityValidationException dbEx)
            {
                ApiCommonFunctions.WriteErrorLog(ApiCommonFunctions.GetDbEntityValidationExceptionMsgs(dbEx));
                refundChallanResultModel.ErrorMessage = "Entity Error Occured in saving Refund Challan Order Details.";
                return refundChallanResultModel;
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
            
        }

        
        public RefundChallanDROrderResultModel GenerateNewOrderID(int OfficeID, long RowId)
        {
            try
            {
                string rootPath = ConfigurationManager.AppSettings["MaintaincePortalVirtualOrdersDirectoryPath"];
                rootPath = rootPath + "\\RefundChallan";

                using (dbContext = new KaveriEntities())
                {
                    RefundChallanDROrderResultModel refundChallanDROrderResultModel = new RefundChallanDROrderResultModel();

                    int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                    string districtTriLetter = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.ShortNameE).FirstOrDefault();

                    //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    //string fileName = "DROrder_" + RowId + "_" ".pdf";
                    //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.

                    //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    string fileName = "DROrder_" + RowId + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".pdf";
                    //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.

                    #region 
                    //Added by Shivam B for upload DR Order pdf in Refund Challan for current financial year on 27/05/2022.
                    var DBDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == RowId).FirstOrDefault();
                    var insertDateTime = Convert.ToDateTime(DBDetails.InsertDateTime);
                    string finYear;
                    int month = insertDateTime.Month;

                    if ( month > 3)
                    {
                        finYear = Convert.ToString(insertDateTime.Year) + "-" + Convert.ToString(insertDateTime.AddYears(1).Year).Substring(2, 2);
                    }
                    else
                    {
                        finYear = Convert.ToString(insertDateTime.Year -1) + "-" + Convert.ToString(insertDateTime.Year).Substring(2, 2);
                    }
                    //string finYear = Convert.ToString(DateTime.Now.Year) + "-" + Convert.ToString(DateTime.Now.AddYears(1).Year).Substring(2, 2);
                    #endregion //Added by Shivam B for upload DR Order pdf in Refund Challan for current financial year on 27/05/2022.


                    #region
                    //Added on 28/05/2022 For Delete DR order pdf file. If it is already present in folder and no entry in database by Shivam B
                    //if (DBDetails.IsFinalized == true)
                    //{
                    //    if(DBDetails.IsDRApproved == null)
                    //    {
                    //        if (DBDetails.AbsoluteFilePath == "" && DBDetails.Relativepath == "" && DBDetails.FileName == "")
                    //        {
                    //            var FilePathToDelete = rootPath + "\\" + districtTriLetter + "\\" + finYear + "\\" + RowId + "\\" + fileName;
                    //            if (System.IO.File.Exists(FilePathToDelete))
                    //            {
                    //                System.IO.File.Delete(FilePathToDelete);
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion //Added on 28/05/2022 For Delete DR order pdf file. If it is already present in folder and no entry in database by Shivam B



                    refundChallanDROrderResultModel.RelativeFilePath = "\\" + districtTriLetter + "\\" + finYear + "\\" + RowId;
                    refundChallanDROrderResultModel.FileName = fileName;
                    refundChallanDROrderResultModel.rootPath = rootPath;
                   
                    return refundChallanDROrderResultModel;
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


        public string FinalizeApproveDROrder(long RowId, long UserId)
        {
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    string FinalizeDROrderResp;

                    AMS_RefundChallanDetails refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == RowId).FirstOrDefault();

                    if (refundChallanDetails != null)
                    {
                        if (refundChallanDetails.IsFinalized == true && refundChallanDetails.AbsoluteFilePath != null && refundChallanDetails.Relativepath !=null && refundChallanDetails.FileName != null && refundChallanDetails.IsDRApproved == null)
                        {

                            #region 
                            //Added on 20-07-2022 For checking if DR Order pdf is exists or not in the folder path and if exists then save the details.
                            string filePath = System.Configuration.ConfigurationManager.AppSettings["MaintaincePortalVirtualOrdersDirectoryPath"] + "\\RefundChallan\\" + refundChallanDetails.Relativepath;

                            
                            //string rootPath = ConfigurationManager.AppSettings["MaintaincePortalVirtualOrdersDirectoryPath"];
                            //rootPath = rootPath + "\\RefundChallan";

                            //string FilePath = rootPath + refundChallanDetails.Relativepath;

                            //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                            //string districtTriLetter = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.ShortNameE).FirstOrDefault();

                            ////var DBDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == RowId).FirstOrDefault();
                            //var insertDateTime = Convert.ToDateTime(refundChallanDetails.InsertDateTime);

                            //string finYear;
                            //int month = insertDateTime.Month;

                            //if (month > 3)
                            //{
                            //    finYear = Convert.ToString(insertDateTime.Year) + "-" + Convert.ToString(insertDateTime.AddYears(1).Year).Substring(2, 2);
                            //}
                            //else
                            //{
                            //    finYear = Convert.ToString(insertDateTime.Year - 1) + "-" + Convert.ToString(insertDateTime.Year).Substring(2, 2);
                            //}

                            //string fileName = "DROrder_" + RowId + ".pdf";

                            //var FilePath = rootPath + "\\" + districtTriLetter + "\\" + finYear + "\\" + RowId + "\\" + fileName;
                            if (System.IO.File.Exists(filePath))
                            {
                                //Added on 27/05/2022 for taking currentDateTime on click of finalize button in Refund Challan by ShivamB 
                                refundChallanDetails.ApproveDateTime = DateTime.Now;
                                //Added on 27/05/2022 for taking currentDateTime on click of finalize button in Refund Challan by ShivamB
                                refundChallanDetails.DROUserId = UserId;
                                refundChallanDetails.IsDRApproved = true;
                                dbContext.Entry(refundChallanDetails).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                                FinalizeDROrderResp = string.Empty;
                            }

                            //Added on 20-07-2022 For checking if DR Order pdf is exists or not in the folder path and if exists then save the details.
                            #endregion

                            else
                            {
                                FinalizeDROrderResp = "DR Order pdf is not uploaded in the directory folder path.";
                            }
                        }
                        else
                        {
                            FinalizeDROrderResp = null;
                        }
                        
                    }
                    else
                    {
                        FinalizeDROrderResp = null;
                    }
                    return FinalizeDROrderResp;
                }
            }
            catch (Exception ex)
            {
                //ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
            
        }


        public string FinalizeRejectDROrder(long RowId, long UserId)
        {
            try
            {
                using (dbContext = new KaveriEntities())
                {
                    int RowIdValue = Convert.ToInt16(RowId);
                    string FinalizeDROrderResp;

                    AMS_RefundChallanDetails refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == RowId).FirstOrDefault();

                    if (refundChallanDetails != null)
                    {
                        if (refundChallanDetails.IsFinalized == true && refundChallanDetails.RejectionReason != null )
                        {
                            //Added on 27/05/2022 for taking currentDateTime on click of finalize button in Refund Challan by ShivamB
                            refundChallanDetails.ApproveDateTime = DateTime.Now;
                            //Added on 27/05/2022 for taking currentDateTime on click of finalize button in Refund Challan by ShivamB
                            refundChallanDetails.DROUserId = UserId;
                            refundChallanDetails.IsDRApproved = false;
                            dbContext.Entry(refundChallanDetails).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            FinalizeDROrderResp = string.Empty;
                        }
                        else
                        {
                            FinalizeDROrderResp = null;
                        }
                    }
                    else
                    {
                        FinalizeDROrderResp = null;
                    }
                    return FinalizeDROrderResp;
                }
            }
            catch (Exception ex)
            {
                //ApiCommonFunctions.WriteErrorLog(ex.Message);
                throw ex;
            }
            
        }

        #region
        //Added on 27/05/2022 for delete DR order pdf on click of delete button in Refund Challan by ShivamB
        public RefundChallanOrderResultModel DeleteCurrentOrderFile(long RowId)
        {
            RefundChallanOrderResultModel refundChallanOrderResultModel = new RefundChallanOrderResultModel();

            try
            {
                dbContext = new KaveriEntities();

                AMS_RefundChallanDetails refundChallanDetails = dbContext.AMS_RefundChallanDetails.Where(x => x.RowId == RowId).FirstOrDefault();
                //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
				//RefundChallanOrderResultModel refundChallanOrderResultModel = new RefundChallanOrderResultModel();
				//Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
				
                if (refundChallanDetails != null)
                {
                    refundChallanDetails.AbsoluteFilePath = "";
                    refundChallanDetails.Relativepath = "";
                    refundChallanDetails.FileName = "";

                    dbContext.Entry(refundChallanDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                    //string relativeFilePath = refundChallanDetails.Relativepath;
                    //string filePathToDelete = System.Configuration.ConfigurationManager.AppSettings["MaintaincePortalVirtualOrdersDirectoryPath"] + "\\RefundChallan\\" + relativeFilePath;


                    //if (File.Exists(filePathToDelete))
                    //{
                    //    File.Delete(filePathToDelete);
                    //}
                    //Commented By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.

                    refundChallanOrderResultModel.ResponseMessage = "Order file Delete Successfully.";
                    return refundChallanOrderResultModel;
                }
                else
                {
                    refundChallanOrderResultModel.ErrorMessage = "Order Details Not found.";
                    return refundChallanOrderResultModel;
                }
            }
            catch (Exception ex)
            {
                ApiCommonFunctions.WriteErrorLog(ex.Message);
                //throw ex;

                //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
                refundChallanOrderResultModel.ErrorMessage = "Problem Occured while deleting the pdf";
                return refundChallanOrderResultModel;
                //Added By Shivam B on 19-08-2022 for Uploading DR Order Pdf Name with TimeStamp.
            }
        }
        #endregion  //Added on 27/05/2022 for delete DR order pdf on click of delete button in Refund Challan by ShivamB


    }
}