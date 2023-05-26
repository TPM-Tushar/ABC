#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationNoVerificationDetailsController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Registration No Verification Details.

*/
#endregion


using CustomModels.Models.Remittance.RegistrationNoVerificationDetails;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
   [KaveriAuthorization]
    public class RegistrationNoVerificationDetailsController : Controller
    {
        // GET: Remittance/RegistrationNoVerificationDetails
        ServiceCaller caller = null;
        [HttpGet]
        [MenuHighlight]
        public ActionResult RegistrationNoVerificationDetailsView()

        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationDetailsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                RegistrationNoVerificationDetailsModel reqModel = caller.GetCall<RegistrationNoVerificationDetailsModel>("RegistrationNoVerificationDetailsView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Registration No Verification Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        public ActionResult GetRegistrationNoVerificationDetails(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                RegistrationNoVerificationDetailsModel reqModel = new RegistrationNoVerificationDetailsModel();
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROfficeID = formCollection["SROfficeID"];
                var DocumentTypeId = formCollection["DocumentTypeId"];
                var DateNullCheck = formCollection["DateNullCheck"];
                //Added By Tushar on 14 Oct 2022
                var FRNCheck = formCollection["FRNCheck"];
                var SFNCheck = formCollection["SFNCheck"];
                var IsRefresh = formCollection["IsRefresh"];

                //End By Tushar on 14 Oct 2022
                //Added By Tushar on 1 Nov 2022
                var FileNACheck = formCollection["FileNACheck"];
                var CNullCheck = formCollection["CNull"];
                var LNullCheck = formCollection["LNull"];
                //End By Tushar on 1 Nov 2022
                //Added By Tushar on 2 Nov 2022
                var MisMatch = formCollection["MisMatch"];
                var Deleted = formCollection["Deleted"];
                var Added = formCollection["Added"];
                var C_NA_L_A = formCollection["C_NA_L_A"];
                var C_NA_L_NA = formCollection["C_NA_L_NA"];
                var C_A_L_NA = formCollection["C_A_L_NA"];
                var SM_M = formCollection["SM_M"];
                //End By Tushar on 2 Nov 2022
                //Added By Tushar on 29 Nov 2022
                var IsDuplicate = formCollection["IsDuplicate"];
                //End By Tushar on 29 Nov 2022
                //Added By tushar on 3 jan 2023
                var PropertyAreaDetailsErrorType = formCollection["PropertyAreaDetailsErrorType"];
                //End By Tushar on 3 Jan 2023

                //Added By Rushikesh on 6 Feb 2023
                var DateDetailsErrorType = formCollection["DateDetailsErrorType"];
                //End By Rushikesh on 6 Feb 2023
                //Added By Rushikesh 8 Feb 2023
                var DateErrorType_DateDetails = formCollection["DateErrorType_DateDetails"];
                //End By Rushikesh 8 Feb 2023

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                int pageSize = 0;
                int skip = 0;
                String errorMessage = String.Empty;
                // For Excel download
                var SroCodeEx =0 ;
                var DateTime_DateEx = new DateTime();
                var DateTime_ToDateEX = new DateTime();
                var DocumentTypeIdEx = 0;
                var IsFRNCheckEx = false;
                var IsSFNCheckEx = false;
                var IsRefreshEx = false;
                var IsDateNullEx = false;
                var IsErrorTypecheckEx = false;
                var ErrorCodeEx = 0;
                var IsDuplicateEx = false;
                //End for excel download
                //
                var IsFileNAEX = false;
                var IsCNullEx = false;
                var IsLNullEx = false;
                // 
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                var IsPropertyAreaDetailsErrorType = false;

                //Added by Rushikesh 6 Feb 2023
                var IsDateDetailsErrorType = false;
                //End by Rushikesh 6 Feb 2023

                var RefreshMessage = string.Empty;
                var DateErrorType_DateDetailsEx = false;
               
                //added by rushikesh 10 feb 2023
                if ((DateDetailsErrorType != null) && (string.IsNullOrEmpty(DocumentTypeId) || DocumentTypeId == "0"))
                {
                    return Json(new { success = false, errorMessage = "Please select Document Type" }, JsonRequestBehavior.AllowGet);
                }
                if ((DateErrorType_DateDetails != null) && (string.IsNullOrEmpty(DocumentTypeId) || DocumentTypeId == "0"))
                {
                    return Json(new { success = false, errorMessage = "Please select Document Type" }, JsonRequestBehavior.AllowGet);
                }
                //end by rushikesh 10 feb 2023

                //Updated by Rushikesh 10 Feb 2023
                if ((PropertyAreaDetailsErrorType == null && DateDetailsErrorType == null && DateErrorType_DateDetails == null))
                {
                //Validation
                if (string.IsNullOrEmpty(SROfficeID))
                {
                    return Json(new { success = false, errorMessage = "Please select SRO Name" }, JsonRequestBehavior.AllowGet);
                }
                if ((string.IsNullOrEmpty(DocumentTypeId) || DocumentTypeId == "0"))
                {
                    return Json(new { success = false, errorMessage = "Please select Document Type" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(fromDate))
                {
                    return Json(new { success = false, errorMessage = "Please select From Date" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    return Json(new { success = false, errorMessage = "Please select To Date" }, JsonRequestBehavior.AllowGet);
                }
                //
                if (DateNullCheck == "NullCheck")
                    reqModel.IsDateNull = true;
                else
                    reqModel.IsDateNull = false;
                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                reqModel.DateTime_Date = Convert.ToDateTime(fromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);
                //Added By Tushar on 14 Oct 2022
                if (FRNCheck == "FRN")
                {
                    reqModel.IsFRNCheck = true;
                }
                else
                {
                    reqModel.IsFRNCheck = false;
                }
                if (SFNCheck == "SFN")
                {
                    reqModel.IsSFNCheck = true;
                }
                else
                {
                    reqModel.IsSFNCheck = false;
                }
                if (IsRefresh == "true")
                {
                    reqModel.IsRefresh = true;
                }
                else
                    reqModel.IsRefresh = false;
                //End By Tushar on 14 Oct 2022
                //Added By Tushar on 1 Nov 2022
                if(FileNACheck == "FileNA")
                {
                    reqModel.IsFileNA = true;
                }
                else
                {
                    reqModel.IsFileNA = false;
                }
                if (CNullCheck == "CNull")
                {
                    reqModel.IsCNull = true;
                }else
                {
                    reqModel.IsCNull = false;
                }
                if (LNullCheck == "LNull")
                {
                    reqModel.IsLNull = true;
                }
                else
                {
                    reqModel.IsLNull = false;
                }
                //End By Tushar on 1 Nov 2022
                //Added By Tushar on 2 Nov 2022
                if(MisMatch == "1")
                {
                    reqModel.IsErrorTypecheck = true;
                    reqModel.ErrorCode = Convert.ToInt32(MisMatch);
                }else if(Deleted == "2")
                {
                    reqModel.IsErrorTypecheck = true;
                    reqModel.ErrorCode = Convert.ToInt32(Deleted);
                }else if(Added == "3")
                {
                    reqModel.IsErrorTypecheck = true;
                    reqModel.ErrorCode = Convert.ToInt32(Added);
                }else if(C_NA_L_A == "4")
                {
                    reqModel.IsErrorTypecheck = true;
                    reqModel.ErrorCode = Convert.ToInt32(C_NA_L_A);
                }else if(C_NA_L_NA == "5")
                {
                    reqModel.IsErrorTypecheck = true;
                    reqModel.ErrorCode = Convert.ToInt32(C_NA_L_NA);
                }else if(C_A_L_NA =="6")
                {
                    reqModel.IsErrorTypecheck = true;
                    reqModel.ErrorCode = Convert.ToInt32(C_A_L_NA);
                }
                else if (SM_M == "7")
                {
                    reqModel.IsErrorTypecheck = true;
                    reqModel.ErrorCode = Convert.ToInt32(SM_M);
                }else
                {
                    reqModel.IsErrorTypecheck = false;
                    reqModel.ErrorCode = 0;
                }
                //End By Tushar on 2 Nov 2022
                //Added By Tushar on 29 Nov 2022
                if (IsDuplicate == "IsDuplicate")
                {
                    reqModel.IsDuplicate = true;
                }
                else
                    reqModel.IsDuplicate = false;
                //End By Tushar on 29 nov 2022
//int startLen = Convert.ToInt32(formCollection["start"]);
                    //int totalNum = Convert.ToInt32(formCollection["length"]);
                    // var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                    /*Commented and added By Tushar on 3 jan 2023
                    int pageSize = totalNum;
                    int skip = startLen;
                    String errorMessage = String.Empty;
                    // For Excel download
                    var SroCodeEx = reqModel.SROfficeID;
                    var DateTime_DateEx = reqModel.DateTime_Date;
                    var DateTime_ToDateEX = reqModel.DateTime_ToDate;
                    var DocumentTypeIdEx = reqModel.DocumentTypeId;
                    var IsFRNCheckEx = reqModel.IsFRNCheck;
                    var IsSFNCheckEx = reqModel.IsSFNCheck;
                    var IsRefreshEx = reqModel.IsRefresh;
                    var IsDateNullEx = reqModel.IsDateNull;
                    var IsErrorTypecheckEx = reqModel.IsErrorTypecheck;
                    var ErrorCodeEx = reqModel.ErrorCode;
                    var IsDuplicateEx = reqModel.IsDuplicate;
                    //End for excel download
                    //
                    var IsFileNAEX = reqModel.IsFileNA;
                    var IsCNullEx = reqModel.IsCNull;
                    var IsLNullEx = reqModel.IsLNull;
                    // 
                    */
                    pageSize = totalNum;
                     skip = startLen;
            
                    // For Excel download
                     SroCodeEx = reqModel.SROfficeID;
                     DateTime_DateEx = reqModel.DateTime_Date;
                     DateTime_ToDateEX = reqModel.DateTime_ToDate;
                     DocumentTypeIdEx = reqModel.DocumentTypeId;
                     IsFRNCheckEx = reqModel.IsFRNCheck;
                     IsSFNCheckEx = reqModel.IsSFNCheck;
                     IsRefreshEx = reqModel.IsRefresh;
                     IsDateNullEx = reqModel.IsDateNull;
                     IsErrorTypecheckEx = reqModel.IsErrorTypecheck;
                     ErrorCodeEx = reqModel.ErrorCode;
                     IsDuplicateEx = reqModel.IsDuplicate;
                    //End for excel download
                    //
                     IsFileNAEX = reqModel.IsFileNA;
                     IsCNullEx = reqModel.IsCNull;
                     IsLNullEx = reqModel.IsLNull;
                    // 
                }
                //Added by Rushikesh 6 Feb 2023
                else if(DateDetailsErrorType != null && DateErrorType_DateDetails == null)
                {
                    reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                    pageSize = totalNum;
                    skip = startLen;
                    reqModel.IsDateDetailsErrorType = true;
                    IsDateDetailsErrorType = true;
                    reqModel.IsErrorTypecheck = true;
                    reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);
                    reqModel.ErrorCode = Convert.ToInt32(DateDetailsErrorType);
                    reqModel.DateTime_Date = Convert.ToDateTime(fromDate);
                    reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                    ErrorCodeEx = reqModel.ErrorCode;
                    DocumentTypeIdEx = reqModel.DocumentTypeId;
                    IsErrorTypecheckEx = reqModel.IsErrorTypecheck;
                    ErrorCodeEx = reqModel.ErrorCode;
                    SroCodeEx = reqModel.SROfficeID;
                }
                //End by rushikesh 6 Feb 2023
                //Added by Rushikesh 8 Feb 2023
                else if (DateErrorType_DateDetails != null)
                {
                    reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                    pageSize = totalNum;
                    skip = startLen;
                    //reqModel.IsDateDetailsErrorType = true;
                    //IsDateDetailsErrorType = true;
                    reqModel.IsErrorTypecheck = true;
                    reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);
                    reqModel.ErrorCode = Convert.ToInt32(DateErrorType_DateDetails);
                    reqModel.DateTime_Date = Convert.ToDateTime(fromDate);
                    reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                    ErrorCodeEx = reqModel.ErrorCode;
                    DocumentTypeIdEx = reqModel.DocumentTypeId;
                    IsErrorTypecheckEx = reqModel.IsErrorTypecheck;
                    ErrorCodeEx = reqModel.ErrorCode;
                    reqModel.IsDateErrorType_DateDetails = true;
                    SroCodeEx = reqModel.SROfficeID;

                    DateErrorType_DateDetailsEx = reqModel.IsDateErrorType_DateDetails;
                }
                //End by Rushikesh 8 Feb 2023
                else
                {
                    reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                    pageSize = totalNum;
                    skip = startLen;
                    reqModel.IsPropertyAreaDetailsErrorType = true;
                    IsPropertyAreaDetailsErrorType = true;
                    reqModel.ErrorCode = Convert.ToInt32(PropertyAreaDetailsErrorType);
                    ErrorCodeEx = reqModel.ErrorCode;
                }
				//End By Tushar on 3 Jan 2023
                IEnumerable<RegistrationNoVerificationDetailsTableModel> result = caller.PostCall<RegistrationNoVerificationDetailsModel, List<RegistrationNoVerificationDetailsTableModel>>("GetRegistrationNoVerificationDetails", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Registration No Verification Details." });


                }
                int TotalCount = result.Count();
                //

                //Added By Tushar on 5 jan 2023
                RefreshMessage = result.Select(c => c.RefreshMessage).FirstOrDefault();
                //End By Tushar on 5 jan 2023


                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (mtch.Success)
                    {
                        if (!string.IsNullOrEmpty(searchValue))
                        {

                            var emptyData = Json(new
                            {
                                draw = formCollection["draw"],
                                recordsTotal = 0,
                                recordsFiltered = 0,
                                data = "",
                                status = false,
                                errorMessage = "Please enter valid Search String "
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }

                    }
                    else
                    {
                        result = result.Where(m =>
                        // m.SROCode.ToLower().Contains(searchValue.ToLower()) ||
                        m.C_Stamp5DateTime.ToLower().Contains(searchValue.ToLower()) ||
                        m.C_FRN.ToLower().Contains(searchValue.ToLower()) ||
                        m.C_ScannedFileName.ToLower().Contains(searchValue.ToLower()) ||
                        m.L_Stamp5DateTime.ToLower().Contains(searchValue.ToLower()) ||
                        m.L_FRN.ToLower().Contains(searchValue.ToLower()) ||
                        m.L_ScannedFileName.ToLower().Contains(searchValue.ToLower()) ||

                        m.C_CDNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.L_CDNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.L_ScanDate.ToLower().Contains(searchValue.ToLower()) ||
                        m.BatchDateTime.ToLower().Contains(searchValue.ToLower()) ||
                        m.C_ScanFileUploadDateTime.ToLower().Contains(searchValue.ToLower()) ||
                        m.ErrorType.ToLower().Contains(searchValue.ToLower()));
                        TotalCount = result.Count();
                    }
                }


                //
                var gridData = result.Select(RegistrationNoVerificationDetailsTableModel => new
                {

                    srNo = RegistrationNoVerificationDetailsTableModel.srNo,

                    DocumentID = RegistrationNoVerificationDetailsTableModel.DocumentID,
                    SROCode = RegistrationNoVerificationDetailsTableModel.SROCode,
                    C_Stamp5DateTime = RegistrationNoVerificationDetailsTableModel.C_Stamp5DateTime,
                    C_FRN = RegistrationNoVerificationDetailsTableModel.C_FRN,
                    C_ScannedFileName = RegistrationNoVerificationDetailsTableModel.C_ScannedFileName,
                    L_Stamp5DateTime = RegistrationNoVerificationDetailsTableModel.L_Stamp5DateTime,
                    L_FRN = RegistrationNoVerificationDetailsTableModel.L_FRN,
                    L_ScannedFileName = RegistrationNoVerificationDetailsTableModel.L_ScannedFileName,
                    BatchID = RegistrationNoVerificationDetailsTableModel.BatchID,
                    C_CDNumber = RegistrationNoVerificationDetailsTableModel.C_CDNumber,
                    L_CDNumber = RegistrationNoVerificationDetailsTableModel.L_CDNumber,
                    ErrorType = RegistrationNoVerificationDetailsTableModel.ErrorType,
                    DocumentTypeID = RegistrationNoVerificationDetailsTableModel.DocumentTypeID,
                    BatchDateTime = RegistrationNoVerificationDetailsTableModel.BatchDateTime,
                    //Added By Tushar on 14 Oct 2022
                    C_ScanFileUploadDateTime = RegistrationNoVerificationDetailsTableModel.C_ScanFileUploadDateTime,
                    L_ScanDate = RegistrationNoVerificationDetailsTableModel.L_ScanDate,
                    //End By Tushar on 14 Oct 2022
                    //Added By Tushar on 29 Nov 2022
                    L_StartTime = RegistrationNoVerificationDetailsTableModel.L_StartTime,
                    L_EndTime = RegistrationNoVerificationDetailsTableModel.L_EndTime,
                    L_Filesize = RegistrationNoVerificationDetailsTableModel.L_Filesize,
                    L_Pages = RegistrationNoVerificationDetailsTableModel.L_Pages,
                    L_Checksum = RegistrationNoVerificationDetailsTableModel.L_Checksum,
                    IsDuplicate = RegistrationNoVerificationDetailsTableModel.IsDuplicate,
                    //End By Tushar on 29 Nov 2022
                    //Added By Tushar on 3 Jan 2023
                    PropertyID = RegistrationNoVerificationDetailsTableModel.PropertyID,
                    VillageCode= RegistrationNoVerificationDetailsTableModel.VillageCode,
                    TotalArea = RegistrationNoVerificationDetailsTableModel.TotalArea,
                    MeasurementUnit = RegistrationNoVerificationDetailsTableModel.MeasurementUnit,
                    C_PropertyID = RegistrationNoVerificationDetailsTableModel.C_PropertyID,
                    C_SROCode = RegistrationNoVerificationDetailsTableModel.C_SROCode,
                    C_VillageCode = RegistrationNoVerificationDetailsTableModel.C_VillageCode,
                    C_TotalArea = RegistrationNoVerificationDetailsTableModel.C_TotalArea,
                    C_DocumentID = RegistrationNoVerificationDetailsTableModel.C_DocumentID,
                    C_MeasurementUnit = RegistrationNoVerificationDetailsTableModel.C_MeasurementUnit,
                    P_DocumentID = RegistrationNoVerificationDetailsTableModel.DocumentID,
                    P_BatchID = RegistrationNoVerificationDetailsTableModel.BatchID,
                    P_ErrorType = RegistrationNoVerificationDetailsTableModel.ErrorType,
                    L_SROCode = RegistrationNoVerificationDetailsTableModel.SROCode,
                    //End By Tushar on 3 Jan 2023

                    //Add by Rushikesh on 6 Feb 2023
                    L_Stamp5DateTime_1 = RegistrationNoVerificationDetailsTableModel.L_Stamp5DateTime_1,
                    L_Stamp1DateTime = RegistrationNoVerificationDetailsTableModel.L_Stamp1DateTime,
                    L_Stamp2DateTime = RegistrationNoVerificationDetailsTableModel.L_Stamp2DateTime,
                    L_Stamp3DateTime = RegistrationNoVerificationDetailsTableModel.L_Stamp3DateTime,
                    L_Stamp4DateTime = RegistrationNoVerificationDetailsTableModel.L_Stamp4DateTime,
                    L_PresentDateTime = RegistrationNoVerificationDetailsTableModel.L_PresentDateTime,
                    L_ExecutionDateTime = RegistrationNoVerificationDetailsTableModel.L_ExecutionDateTime,
                    L_DateOfStamp = RegistrationNoVerificationDetailsTableModel.L_DateOfStamp,
                    L_WithdrawalDate = RegistrationNoVerificationDetailsTableModel.L_WithdrawalDate,
                    L_RefusalDate = RegistrationNoVerificationDetailsTableModel.L_RefusalDate,

                    //Central
                    C_Stamp1DateTime = RegistrationNoVerificationDetailsTableModel.C_Stamp1DateTime,
                    C_Stamp2DateTime = RegistrationNoVerificationDetailsTableModel.C_Stamp2DateTime,
                    C_Stamp3DateTime = RegistrationNoVerificationDetailsTableModel.C_Stamp3DateTime,
                    C_Stamp4DateTime = RegistrationNoVerificationDetailsTableModel.C_Stamp4DateTime,
                    C_PresentDateTime = RegistrationNoVerificationDetailsTableModel.C_PresentDateTime,
                    C_ExecutionDateTime = RegistrationNoVerificationDetailsTableModel.C_ExecutionDateTime,
                    C_DateOfStamp = RegistrationNoVerificationDetailsTableModel.C_DateOfStamp,
                    C_WithdrawalDate = RegistrationNoVerificationDetailsTableModel.C_WithdrawalDate,
                    C_RefusalDate = RegistrationNoVerificationDetailsTableModel.C_RefusalDate,
                    //End by Rushikesh on 6 Feb 2023

                    //Added By Rushikesh 9 Feb 2023
                    TableName = RegistrationNoVerificationDetailsTableModel.TableName,
                    ReceiptID=RegistrationNoVerificationDetailsTableModel.ReceiptID,
                    L_DateOfReceipt = RegistrationNoVerificationDetailsTableModel.L_DateOfReceipt,
                    C_DateOfReceipt = RegistrationNoVerificationDetailsTableModel.C_DateOfReceipt,
                    StampDetailsID = RegistrationNoVerificationDetailsTableModel.StampDetailsID,
                    L_DDChalDate = RegistrationNoVerificationDetailsTableModel.L_DDChalDate,
                    C_DDChalDate = RegistrationNoVerificationDetailsTableModel.C_DDChalDate,
                    L_StampPaymentDate = RegistrationNoVerificationDetailsTableModel.L_StampPaymentDate,
                    C_StampPaymentDate = RegistrationNoVerificationDetailsTableModel.C_StampPaymentDate,
                    L_DateOfReturn = RegistrationNoVerificationDetailsTableModel.L_DateOfReturn,
                    C_DateOfReturn = RegistrationNoVerificationDetailsTableModel.C_DateOfReturn,
                    PartyID = RegistrationNoVerificationDetailsTableModel.PartyID,
                    L_AdmissionDate = RegistrationNoVerificationDetailsTableModel.L_AdmissionDate,
                    C_AdmissionDate = RegistrationNoVerificationDetailsTableModel.C_AdmissionDate,
                    
                    //End By Rushikesh 9 Feb 2023

                });
                //Added By Tushar on 19 Oct 2022

                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFunForTable('" + SroCodeEx + "','" + fromDate + "','" + ToDate + "','" + DocumentTypeIdEx + "','" + IsFRNCheckEx + "','" + IsSFNCheckEx + "','" + IsRefreshEx + "','" + IsDateNullEx + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFunForTable('" + SroCodeEx + "','" + fromDate + "','" + ToDate + "','" + DocumentTypeIdEx + "','" + IsFRNCheckEx + "','" + IsSFNCheckEx + "','" + IsRefreshEx + "','" + IsDateNullEx + "','" + IsFileNAEX + "','" + IsCNullEx + "','" + IsLNullEx + "','" + IsErrorTypecheckEx + "','" + ErrorCodeEx + "','" + IsDuplicateEx + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
				//Commented and Added By Tushar on 3 Jan 2023
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFunForTable('" + SroCodeEx + "','" + fromDate + "','" + ToDate + "','" + DocumentTypeIdEx + "','" + IsFRNCheckEx + "','" + IsSFNCheckEx + "','" + IsRefreshEx + "','" + IsDateNullEx + "','" + IsFileNAEX + "','" + IsCNullEx + "','" + IsLNullEx + "','" + IsErrorTypecheckEx + "','" + ErrorCodeEx + "','" + IsDuplicateEx + "','" + IsPropertyAreaDetailsErrorType + "','" + IsDateDetailsErrorType + "','" + DateErrorType_DateDetailsEx + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                //End By Tushar on 19 Oct 2022
                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = "Invalid To Date",
                    //Added By Tushar on 19 Oct 2022
                    ExcelDownloadBtn = ExcelDownloadBtn,
                    //End By Tushar on 19 Oct 2022
                    //Added By Tushar on 3 Jan 2023
                    IsPropertyAreaDetailsErrorType = IsPropertyAreaDetailsErrorType,
                    RefreshMessage = RefreshMessage
                    //End By Tushar on 3 jan 2023
                });
                if (searchValue != null && searchValue != "")
                {

                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalNum,
                        recordsFiltered = TotalCount,
                        status = "1",
                        //Added By Tushar on 19 Oct 2022
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        //End By Tushar on 19 Oct 2022
                        //Added By Tushar on 3 Jan 2023
                        IsPropertyAreaDetailsErrorType = IsPropertyAreaDetailsErrorType,
                        RefreshMessage = RefreshMessage
                        //End Buy Tushar on 3 jan 2023
                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalNum,
                        recordsFiltered = TotalCount,
                        status = "1",
                        //Added By Tushar on 19 Oct 2022
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        //End By Tushar on 19 Oct 2022
                        //Added By Tushar on 3 Jan 2023
                        IsPropertyAreaDetailsErrorType = IsPropertyAreaDetailsErrorType,
                        RefreshMessage = RefreshMessage
                        //End Buy Tushar on 3 jan 2023

                    });
                }

                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;


            }

            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Registration No Verification Details." });
            }

        }



        //Added by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 
        [HttpGet]
        public ActionResult RefreshPropertyAreaDetails(string SROfficeID)
        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                
                
                if (string.IsNullOrEmpty(SROfficeID))
                {
                    return Json(new { success = false, message = "Please select SRO Name" }, JsonRequestBehavior.AllowGet);
                }
              
                int SROffice_ID = Convert.ToInt32(SROfficeID);
                
                if(SROffice_ID == 0)
                {
                    return Json(new { success = false, message = "Please select SRO Name rather than All" }, JsonRequestBehavior.AllowGet);
                }

                string result = caller.GetCall<string>("RefreshPropertyAreaDetailsDetails", new { SROfficeID = SROffice_ID });
                
                if (result == null || result == "NULL")
                {
                    return Json(new { success = false, message = "Error occured while refreshing property area details. Please contact admin." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, message = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while refreshing property area details." });
            }

        }
        //Added by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 


        // [HttpPost]
        public ActionResult ExportDocRegNoCLBatchDetailsToExcel(string SROfficeID,string FromDate,string ToDate)
        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("DocReg_NoCLBatchDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                RegistrationNoVerificationDetailsModel reqModel = new RegistrationNoVerificationDetailsModel();
                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                reqModel.DateTime_Date = Convert.ToDateTime(FromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;
                RegistrationNoVerificationDetailsTableModel Result = caller.PostCall<RegistrationNoVerificationDetailsModel, RegistrationNoVerificationDetailsTableModel>("GetDocRegNoCLBatchDetails", reqModel, out errorMessage);


                if (Result == null)
                {
                    //Commented and Added by BShivam on 24-04-2023 to show the error message and redirect to the RegistrationNoVerificationDetails View page
                    //return Json(new { success = false, errorMessage = "Error Occured While Processing..." }); 
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Processing...", URLToRedirect = "/Remittance/RegistrationNoVerificationDetails/RegistrationNoVerificationDetailsView" });
                    //Commented and Added by BShivam on 24-04-2023
                }
                List<RPT_DocReg_NoCLBatchDetailsTable> DocReg_NoCLBatchDetailsResult = Result.RPT_DocReg_NoCLBatchDetailsExcelSheet;

                //Added by ShivamB on 24-04-2023 to check is DocRegBatchDetails list count is zero.
                if (DocReg_NoCLBatchDetailsResult.Count == 0)
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "No data found for document registration batch details.", URLToRedirect = "/Remittance/RegistrationNoVerificationDetails/RegistrationNoVerificationDetailsView" });
                //Ended by ShivamB on 24-04-2023

                if (Result.RPT_DocReg_NoCLBatchDetailsExcelSheet != null && Result.RPT_DocReg_NoCLBatchDetailsExcelSheet.Count > 0)
                {
                    fileName = "DocReg_NoCLBatchDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("DocReg_NoCLBatch Report Detail");
                    createdExcelPath = CreateExcelForDocReg_NoCLBatchDetails(DocReg_NoCLBatchDetailsResult, fileName, excelHeader);
                }
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);


                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }


        private string CreateExcelForDocReg_NoCLBatchDetails(List<RPT_DocReg_NoCLBatchDetailsTable> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("DocReg_NoCLBatchDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;
                   

                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                   
                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());
             

                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                    workSheet.Cells[4, 1, 4, 9].Merge = true;
                    workSheet.Cells[5, 1, 5, 9].Merge = true;
                    workSheet.Cells[6, 1, 6, 9].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 45;
                    workSheet.Column(7).Width = 35;
                    workSheet.Column(8).Width = 20;
                    workSheet.Column(9).Width = 20;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "BatchID";
                    workSheet.Cells[7, 3].Value = "SROCode";
                    workSheet.Cells[7, 4].Value = "FromDocumentID";
                    workSheet.Cells[7, 5].Value = "ToDocumentID";
                    workSheet.Cells[7, 6].Value = "DocumentTypeID";
                    workSheet.Cells[7, 7].Value = "BatchDateTime";
                    workSheet.Cells[7, 8].Value = "IsVerified";
                    workSheet.Cells[7, 9].Value = "IsMismatchFound";


                    foreach (var items in result)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";

                        //workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.BatchID;
                        if (items.SROCode == null)
                        {
                            workSheet.Cells[rowIndex, 3].Value = "NULL"; 
                        }else
                        {
                            workSheet.Cells[rowIndex, 3].Value = items.SROCode;
                        }
                        if (items.FromDocumentID == null)
                        {
                            workSheet.Cells[rowIndex, 4].Value = "NULL"; 
                        }else
                        {
                            workSheet.Cells[rowIndex, 4].Value = items.FromDocumentID;
                        }
                        if(items.ToDocumentID == null)
                        {
                            workSheet.Cells[rowIndex, 5].Value = "NULL";
                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 5].Value = items.ToDocumentID;
                        }
                        if (items.DocumentTypeID == null)
                        {
                            workSheet.Cells[rowIndex, 6].Value = "NULL"; 
                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 6].Value = items.DocumentTypeID;
                        }
                        workSheet.Cells[rowIndex, 7].Value = items.BatchDateTime.ToString();
                        if (items.IsVerified == null)
                        {
                            workSheet.Cells[rowIndex, 8].Value = "NULL"; 
                        }else
                        {
                            workSheet.Cells[rowIndex, 8].Value = items.IsVerified;
                        }
                        if (items.IsMismatchFound == null)
                        {
                            workSheet.Cells[rowIndex, 9].Value = "NULL"; 
                        }else
                        {
                            workSheet.Cells[rowIndex, 9].Value = items.IsMismatchFound;
                        }


                      
                        workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
               
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        rowIndex++;
                     
                    }
                 
             
                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }

        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }
        //public ActionResult ExportRegistrationNoVerificationDetailsToExcel(int SroCode, string fromDate, string ToDate, int DocumentTypeId,string IsFRNCheck, string IsSFNCheck, string IsRefresh,string IsDateNull)
        //public ActionResult ExportRegistrationNoVerificationDetailsToExcel(int SroCode, string fromDate, string ToDate, int DocumentTypeId, string IsFRNCheck, string IsSFNCheck, string IsRefresh, string IsDateNull,string IsFileNA,string IsCNull,string IsLNull,string IsErrorTypecheck,string ErrorCode,string IsDuplicate)
		//Commented and Added By Tushar on 3 jan 2023
        public ActionResult ExportRegistrationNoVerificationDetailsToExcel(int SroCode, string fromDate, string ToDate, int DocumentTypeId, string IsFRNCheck, string IsSFNCheck, string IsRefresh, string IsDateNull, string IsFileNA, string IsCNull, string IsLNull, string IsErrorTypecheck, string ErrorCode, string IsDuplicate,string IsPropertyAreaDetailsErrorType, string IsDateDetailsErrorType,string IsDateErrorType_DateDetails)
        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("RegistrationNoVerificationDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                RegistrationNoVerificationDetailsModel reqModel = new RegistrationNoVerificationDetailsModel();
                reqModel.SROfficeID = SroCode;
                reqModel.DateTime_Date = Convert.ToDateTime(fromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                reqModel.DocumentTypeId = DocumentTypeId;
                reqModel.IsFRNCheck =Convert.ToBoolean(IsFRNCheck);
                reqModel.IsSFNCheck = Convert.ToBoolean(IsSFNCheck);
                reqModel.IsRefresh = Convert.ToBoolean(IsRefresh);
                reqModel.IsDateNull = Convert.ToBoolean(IsDateNull);
                //
                reqModel.IsFileNA = Convert.ToBoolean(IsFileNA);
                reqModel.IsCNull = Convert.ToBoolean(IsCNull);
                reqModel.IsLNull = Convert.ToBoolean(IsLNull);
                //
                reqModel.IsErrorTypecheck = Convert.ToBoolean(IsErrorTypecheck);
                reqModel.ErrorCode = Convert.ToInt32(ErrorCode);
                reqModel.IsDuplicate = Convert.ToBoolean(IsDuplicate);
                //
                //Added BY Tushar on 3 jan 2023
                reqModel.IsPropertyAreaDetailsErrorType = Convert.ToBoolean(IsPropertyAreaDetailsErrorType);
                //End By Tushar on 3 Jan 2023
                reqModel.IsDateDetailsErrorType = Convert.ToBoolean(IsDateDetailsErrorType);
                //Added by rushikesh 13 Feb 2023
                reqModel.IsDateErrorType_DateDetails = Convert.ToBoolean(IsDateErrorType_DateDetails);
                //end by rushikesh
                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;

               List< RegistrationNoVerificationDetailsTableModel> Result = caller.PostCall<RegistrationNoVerificationDetailsModel, List<RegistrationNoVerificationDetailsTableModel>>("GetRegistrationNoVerificationDetails", reqModel, out errorMessage);

                if (Result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
                //List<RegistrationNoVerificationDetailsExcelResp> DocReg_NoCLBatchDetailsResult = Result.RegistrationNoVerificationDetailsTableExcelSheet;

                if (Result != null && Result.Count > 0)
                {
                    fileName = "RegistrationNoVerificationDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("Registration No Verification Details");
                    //createdExcelPath = CreateExcelForRegistrationNoVerificationDetails(Result, fileName, excelHeader);
                    // createdExcelPath = CreateExcelForRegistrationNoVerificationDetails(Result, fileName, excelHeader, reqModel.IsFRNCheck, reqModel.IsSFNCheck, reqModel.IsDateNull);
                    //createdExcelPath = CreateExcelForRegistrationNoVerificationDetails(Result, fileName, excelHeader, reqModel.IsFRNCheck, reqModel.IsSFNCheck, reqModel.IsDateNull,reqModel.IsFileNA,reqModel.IsCNull,reqModel.IsLNull);
                    //Commented and Added By Tushar on 3 Jan 2023
                    if (reqModel.IsPropertyAreaDetailsErrorType)
                    {
                      
                        fileName = "RegistrationNoVerification_PropertyAreaDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                        excelHeader = string.Format("Registration No Verification Property Area Details");
                        createdExcelPath = CreateExcelForPropertyAreaDetails(Result, fileName, excelHeader, reqModel.ErrorCode);
                    }
                    //added by rushikesh 13 feb 2023
                    else if (reqModel.IsDateErrorType_DateDetails)
                    {
                        fileName = "RegistrationNoVerification_DateDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                        excelHeader = string.Format("Registration No Verification Date Details");
                        createdExcelPath = CreateExcelForDateErrorType_DateDetails(Result, fileName, excelHeader, reqModel.ErrorCode);
                    }

                    else if (reqModel.IsDateDetailsErrorType)
                    {
                        fileName = "RegistrationNoVerification_DateDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                        excelHeader = string.Format("Registration No Verification Date Details");
                        createdExcelPath = CreateExcelForDateDetails(Result, fileName, excelHeader, reqModel.ErrorCode);
                    }
                    //end by rushikesh
                    else
                    {
                        createdExcelPath = CreateExcelForRegistrationNoVerificationDetails(Result, fileName, excelHeader, reqModel.IsFRNCheck, reqModel.IsSFNCheck, reqModel.IsDateNull, reqModel.IsFileNA, reqModel.IsCNull, reqModel.IsLNull);
                    }
                    //End By Tushar on 3 jan 2023
                }
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);


                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }
           
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }
        //Updated by rushikesh 13 Feb 2023
        private string CreateExcelForRegistrationNoVerificationDetails(List<RegistrationNoVerificationDetailsTableModel> result, string fileName, string excelHeader,bool IsFRNCheck,bool IsSFNCheck,bool IsDateNull,bool IsFileNACheck,bool IsCNull,bool IsLNull)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("RegistrationNoVerificationDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());

                    workSheet.Cells[5, 1].Value = "IsFRNCheck : " + IsFRNCheck;
                    workSheet.Cells[5, 2].Value = "IsSFNCheck : " + IsSFNCheck;
                    workSheet.Cells[5, 3].Value = "IsDateNull : " + IsDateNull;
                    workSheet.Cells[5, 4].Value = "IsFileNACheck :" + IsFileNACheck;
                    workSheet.Cells[5, 5].Value = "C_NA & L_A :" + IsCNull;
                    workSheet.Cells[5, 6].Value = "L_NA & C_A :" + IsLNull;

                    workSheet.Cells[5, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[5, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[5, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[5, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[1, 1, 1, 23].Merge = true;
                    workSheet.Cells[2, 1, 2, 23].Merge = true;
                    workSheet.Cells[3, 1, 3, 23].Merge = true;
                    workSheet.Cells[4, 1, 4, 23].Merge = true;
                    workSheet.Cells[5, 6, 5, 23].Merge = true;
                    workSheet.Cells[6, 1, 6, 23].Merge = true;
               

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 45;
                    workSheet.Column(7).Width = 35;
                    workSheet.Column(8).Width = 40;
                    workSheet.Column(9).Width = 40;

                    workSheet.Column(10).Width = 40;
                    workSheet.Column(11).Width = 40;
                    workSheet.Column(12).Width = 40;
                    workSheet.Column(13).Width = 40;
                    workSheet.Column(14).Width = 40;
                    workSheet.Column(15).Width = 40;
                    workSheet.Column(16).Width = 40;
                    workSheet.Column(17).Width = 40;
                    workSheet.Column(18).Width = 40;
                    workSheet.Column(19).Width = 40;
                    workSheet.Column(20).Width = 40;
                    workSheet.Column(21).Width = 40;
                    workSheet.Column(22).Width = 40;
                    workSheet.Column(23).Width = 40;
                   
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "DocumentID";
                    workSheet.Cells[7, 3].Value = "SROCode";
                    workSheet.Cells[7, 4].Value = "C_Stamp5DateTime";
                    workSheet.Cells[7, 5].Value = "L_Stamp5DateTime";

                    // workSheet.Cells[7, 5].Value = "L_Stamp5DateTime_1";
                    workSheet.Cells[7, 6].Value = "C_FinalRegistrationNumber";
                    workSheet.Cells[7, 7].Value = "L_FinalRegistrationNumber";
                    workSheet.Cells[7, 8].Value = "C_ScannedFileName";
                    workSheet.Cells[7, 9].Value = "L_ScannedFileName";
                    workSheet.Cells[7, 10].Value = "C_CDNumber";
                    workSheet.Cells[7, 11].Value = "L_CDNumber";
                    workSheet.Cells[7, 12].Value = "C_ScanDate";
                    workSheet.Cells[7, 13].Value = "L_ScanDate";
                    /*
                    workSheet.Cells[7, 14].Value = "C_Stamp1DateTime";
                    workSheet.Cells[7, 15].Value = "L_Stamp1DateTime";
                    workSheet.Cells[7, 16].Value = "C_Stamp2DateTime";
                    workSheet.Cells[7, 17].Value = "L_Stamp2DateTime";
                    workSheet.Cells[7, 18].Value = "C_Stamp3DateTime";
                    workSheet.Cells[7, 19].Value = "L_Stamp3DateTime";
                    workSheet.Cells[7, 20].Value = "C_Stamp4DateTime";
                    workSheet.Cells[7, 21].Value = "L_Stamp4DateTime";
                    workSheet.Cells[7, 22].Value = "C_PresentDateTime";
                    workSheet.Cells[7, 23].Value = "L_PresentDateTime";
                    workSheet.Cells[7, 24].Value = "C_ExecutionDateTime";
                    workSheet.Cells[7, 25].Value = "L_ExecutionDateTime";
                    workSheet.Cells[7, 26].Value = "C_DateOfStamp";
                    workSheet.Cells[7, 27].Value = "L_DateOfStamp";
                    workSheet.Cells[7, 28].Value = "C_WithdrawalDate";
                    workSheet.Cells[7, 29].Value = "L_WithdrawalDate";

                    workSheet.Cells[7, 30].Value = "C_RefusalDate";
                    workSheet.Cells[7, 31].Value = "L_Refusal";
                    */
                    //Added By Tushar on 29 Nov 2022
                    workSheet.Cells[7, 14].Value = "L_StartTime";
                    workSheet.Cells[7, 15].Value = "L_EndTime";
                    workSheet.Cells[7, 16].Value = "L_Filesize";
                    workSheet.Cells[7, 17].Value = "L_Pages";
                    workSheet.Cells[7, 18].Value = "L_Checksum";
                    workSheet.Cells[7, 19].Value = "IsDuplicate";
                    //End By Tushar on 29 Nov 2022
                    workSheet.Cells[7, 20].Value = "BatchID";

                    workSheet.Cells[7, 21].Value = "Error Description";
                    workSheet.Cells[7, 22].Value = "DocumentTypeID";
                    workSheet.Cells[7, 23].Value = "BatchDateTime";

                    foreach (var items in result)
                    {
                        for (int i = 1; i < 24; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }
                

                        //workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.DocumentID;
                        workSheet.Cells[rowIndex, 3].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 4].Value = items.C_Stamp5DateTime;
                        workSheet.Cells[rowIndex, 5].Value = items.L_Stamp5DateTime;
                        //workSheet.Cells[rowIndex, 5].Value = items.L_Stamp5DateTime_1;
                        workSheet.Cells[rowIndex, 6].Value = items.C_FRN;
                        workSheet.Cells[rowIndex, 7].Value = items.L_FRN;
                        workSheet.Cells[rowIndex, 8].Value = items.C_ScannedFileName;
                        workSheet.Cells[rowIndex, 9].Value = items.L_ScannedFileName;
                        workSheet.Cells[rowIndex, 10].Value = items.C_CDNumber;
                        workSheet.Cells[rowIndex, 11].Value = items.L_CDNumber;
                        workSheet.Cells[rowIndex, 12].Value = items.C_ScanFileUploadDateTime;
                        workSheet.Cells[rowIndex, 13].Value = items.L_ScanDate;
                      /*
                        workSheet.Cells[rowIndex, 14].Value = items.C_Stamp1DateTime;
                        workSheet.Cells[rowIndex, 15].Value = items.L_Stamp1DateTime;
                        workSheet.Cells[rowIndex, 16].Value = items.C_Stamp2DateTime;
                        workSheet.Cells[rowIndex, 17].Value = items.L_Stamp2DateTime;
                        workSheet.Cells[rowIndex, 18].Value = items.C_Stamp3DateTime;
                        workSheet.Cells[rowIndex, 19].Value = items.L_Stamp3DateTime;
                        workSheet.Cells[rowIndex, 20].Value = items.C_Stamp4DateTime;
                        workSheet.Cells[rowIndex, 21].Value = items.L_Stamp4DateTime;
                        workSheet.Cells[rowIndex, 22].Value = items.C_PresentDateTime;
                        workSheet.Cells[rowIndex, 23].Value = items.L_PresentDateTime;
                        workSheet.Cells[rowIndex, 24].Value = items.C_ExecutionDateTime;
                        workSheet.Cells[rowIndex, 25].Value = items.L_ExecutionDateTime;
                        workSheet.Cells[rowIndex, 26].Value = items.C_DateOfStamp;
                        workSheet.Cells[rowIndex, 27].Value = items.L_DateOfStamp;
                        workSheet.Cells[rowIndex, 28].Value = items.C_WithdrawalDate;
                        workSheet.Cells[rowIndex, 29].Value = items.L_WithdrawalDate;
                        workSheet.Cells[rowIndex, 30].Value = items.C_RefusalDate;
                        workSheet.Cells[rowIndex, 31].Value = items.L_RefusalDate;
                      */
                        workSheet.Cells[rowIndex, 14].Value = items.L_StartTime;
                        workSheet.Cells[rowIndex, 15].Value = items.L_EndTime;
                        workSheet.Cells[rowIndex, 16].Value = items.L_Filesize;
                        workSheet.Cells[rowIndex, 17].Value = items.L_Pages;
                        workSheet.Cells[rowIndex, 18].Value = items.L_Checksum;
                       
                        workSheet.Cells[rowIndex, 19].Value = items.IsDuplicate;
                        workSheet.Cells[rowIndex, 20].Value = items.BatchID;
                        workSheet.Cells[rowIndex, 21].Value = items.ErrorType;
                        workSheet.Cells[rowIndex, 22].Value = items.DocumentTypeID;
                        workSheet.Cells[rowIndex, 23].Value = items.BatchDateTime;

                        for (int i = 1; i < 24; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        rowIndex++;

                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }

      
        public ActionResult ExportScannedFileDetailsToExcel(string SROfficeID, string DocumentTypeId)
        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("ScannedFileDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                RegistrationNoVerificationDetailsModel reqModel = new RegistrationNoVerificationDetailsModel();
                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);
               
                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;
                RegistrationNoVerificationDetailsTableModel Result = caller.PostCall<RegistrationNoVerificationDetailsModel, RegistrationNoVerificationDetailsTableModel>("GetScannedFileDetails", reqModel, out errorMessage);


                if (Result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
                List<ScannedFileDetails> scannedFileDetails = Result.scannedFileDetailsList;

                if (Result.scannedFileDetailsList != null && Result.scannedFileDetailsList.Count > 0)
                {
                    fileName = "ScannedFileDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("Scanned File Detail");
                    createdExcelPath = CreateExcelForScannedFileDetails(scannedFileDetails, fileName, excelHeader);
                }
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);


                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        private string CreateExcelForScannedFileDetails(List<ScannedFileDetails> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("ScannedFileDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());


                    workSheet.Cells[1, 1, 1, 5].Merge = true;
                    workSheet.Cells[2, 1, 2, 5].Merge = true;
                    workSheet.Cells[3, 1, 3, 5].Merge = true;
                    workSheet.Cells[4, 1, 4, 5].Merge = true;
                    workSheet.Cells[5, 1, 5, 5].Merge = true;
                    workSheet.Cells[6, 1, 6, 5].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 40;
        

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "SROCode";
                    workSheet.Cells[7, 3].Value = "SROName";
                    workSheet.Cells[7, 4].Value = "Count";
                    //workSheet.Cells[7, 4].Value = "SROName";
                    workSheet.Cells[7, 5].Value = "scannedFileName";
             


                    foreach (var items in result)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
             

                        //workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 3].Value = items.SroName;
                        workSheet.Cells[rowIndex, 4].Value = items.CountD;
                        workSheet.Cells[rowIndex, 5].Value = items.ScannedFileName;




                        workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
              
                        rowIndex++;

                    }


                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }

        public ActionResult ExportFinalRegistrationNumberDetailsToExcel(string SROfficeID, string DocumentTypeId)
        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("FinalRegistrationNumberDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                RegistrationNoVerificationDetailsModel reqModel = new RegistrationNoVerificationDetailsModel();
                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);

                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;
                RegistrationNoVerificationDetailsTableModel Result = caller.PostCall<RegistrationNoVerificationDetailsModel, RegistrationNoVerificationDetailsTableModel>("GetFinalRegistrationNumberDetails", reqModel, out errorMessage);


                if (Result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
                List<DocumentMasterFRN> documentMasterFRNs = Result.DocumentMasterFRNList;

                if (Result.DocumentMasterFRNList != null && Result.DocumentMasterFRNList.Count > 0)
                {
                    //fileName = "FinalRegistrationNumberDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    if(reqModel.DocumentTypeId ==1)
                    {
                        fileName = "FinalRegistrationNumberDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                        excelHeader = string.Format("Final Registration Number Details");
                    }else
                    {
                        fileName = "MarriageCaseNoDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                        excelHeader = string.Format("Marriage Case No Details");
                    }
                    
                    createdExcelPath = CreateExcelForFinalRegistrationNumberDetails(documentMasterFRNs, fileName, excelHeader, reqModel.DocumentTypeId);
                }
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);


                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        private string CreateExcelForFinalRegistrationNumberDetails(List<DocumentMasterFRN> result, string fileName, string excelHeader,int DocumentTypeId)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("FinalRegistrationNumberDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());


                    workSheet.Cells[1, 1, 1, 5].Merge = true;
                    workSheet.Cells[2, 1, 2, 5].Merge = true;
                    workSheet.Cells[3, 1, 3, 5].Merge = true;
                    workSheet.Cells[4, 1, 4, 5].Merge = true;
                    workSheet.Cells[5, 1, 5, 5].Merge = true;
                    workSheet.Cells[6, 1, 6, 5].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 40;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "SROCode";
                    workSheet.Cells[7, 3].Value = "SROName";
                    workSheet.Cells[7, 4].Value = "Count";
                    //workSheet.Cells[7, 4].Value = "SROName";
                    if(DocumentTypeId == 1)
                    {
                        workSheet.Cells[7, 5].Value = "Final Registration Number";
                    }else
                    {
                        workSheet.Cells[7, 5].Value = "Marriage Case No";
                    }
                   



                    foreach (var items in result)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";


                        //workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 3].Value = items.SroName;
                        workSheet.Cells[rowIndex, 4].Value = items.Count;
                        if(items.FinalRegistrationNumber != null)
                            workSheet.Cells[rowIndex, 5].Value = items.FinalRegistrationNumber;
                        else
                        {
                            workSheet.Cells[rowIndex, 5].Value = "NULL";
                            //workSheet.Cells[rowIndex, 8].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        }
                         




                        workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        rowIndex++;

                    }


                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }


        //Added by Rushikesh 13 Feb 2023
       private String CreateExcelForDateErrorType_DateDetails(List<RegistrationNoVerificationDetailsTableModel> result, string fileName, string excelHeader, int ErrorCode)
       {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("FinalRegistrationNumberDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());


                    workSheet.Cells[1, 1, 1, 22].Merge = true;
                    workSheet.Cells[2, 1, 2, 22].Merge = true;
                    workSheet.Cells[3, 1, 3, 22].Merge = true;
                    workSheet.Cells[4, 1, 4, 22].Merge = true;
                    workSheet.Cells[5, 1, 5, 22].Merge = true;
                    workSheet.Cells[6, 1, 6, 22].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 40;

                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;
                    workSheet.Column(12).Width = 30;
                    workSheet.Column(13).Width = 30;
                    workSheet.Column(14).Width = 30;
                    workSheet.Column(15).Width = 30;
                    workSheet.Column(16).Width = 30;
                    workSheet.Column(17).Width = 30;
                    workSheet.Column(18).Width = 30;
                    workSheet.Column(19).Width = 30;
                    workSheet.Column(20).Width = 30;
                    workSheet.Column(21).Width = 30;
                    


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "SROCode";
                   
                    workSheet.Cells[7, 3].Value = "DocumentID";
                    workSheet.Cells[7, 4].Value = "TableName";
                    workSheet.Cells[7, 5].Value = "ReceiptID";
                    workSheet.Cells[7, 6].Value = "L_DateOfReceipt";
                    workSheet.Cells[7, 7].Value = "C_DateOfReceipt";
                    workSheet.Cells[7, 8].Value = "StampDetailsID";
                    workSheet.Cells[7, 9].Value = "L_DateOfStamp";
                    workSheet.Cells[7, 10].Value = "C_DateOfStamp";
                    workSheet.Cells[7, 11].Value = "L_DDChalDate";
                    workSheet.Cells[7, 12].Value = "C_DDChalDate";
                    workSheet.Cells[7, 13].Value = "L_StampPaymentDate";
                    workSheet.Cells[7, 14].Value = "C_StampPaymentDate";
                    workSheet.Cells[7, 15].Value = "L_DateOfReturn";
                    workSheet.Cells[7, 16].Value = "C_DateOfReturn";
                    workSheet.Cells[7, 17].Value = "PartyID";
                    workSheet.Cells[7, 18].Value = "L_AdmissionDate";
                    workSheet.Cells[7, 19].Value = "C_AdmissionDate";
                    workSheet.Cells[7, 20].Value = "BatchID";
                    workSheet.Cells[7, 21].Value = "ErrorType";


                    foreach (var items in result)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";



                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SROCode;
                       
                        workSheet.Cells[rowIndex, 3].Value = items.DocumentID;
                        workSheet.Cells[rowIndex, 4].Value = items.TableName;
                        workSheet.Cells[rowIndex, 5].Value = items.ReceiptID;
                        workSheet.Cells[rowIndex, 6].Value = items.L_DateOfReceipt;
                        workSheet.Cells[rowIndex, 7].Value = items.C_DateOfReceipt;
                        workSheet.Cells[rowIndex, 8].Value = items.StampDetailsID;
                        workSheet.Cells[rowIndex, 9].Value = items.L_DateOfStamp;
                        workSheet.Cells[rowIndex, 10].Value = items.C_DateOfStamp;
                        workSheet.Cells[rowIndex, 11].Value = items.L_DDChalDate;
                        workSheet.Cells[rowIndex, 12].Value = items.C_DDChalDate;
                        workSheet.Cells[rowIndex, 13].Value = items.L_StampPaymentDate;
                        workSheet.Cells[rowIndex, 14].Value = items.C_StampPaymentDate;
                        workSheet.Cells[rowIndex, 15].Value = items.L_DateOfReturn;
                        workSheet.Cells[rowIndex, 16].Value = items.C_DateOfReturn;
                        workSheet.Cells[rowIndex, 17].Value = items.PartyID;
                        workSheet.Cells[rowIndex, 18].Value = items.L_AdmissionDate;
                        workSheet.Cells[rowIndex, 19].Value = items.C_AdmissionDate;
                        workSheet.Cells[rowIndex, 20].Value = items.BatchID;
                        workSheet.Cells[rowIndex, 21].Value = items.ErrorType;


                        for (int i = 1; i < 22; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        rowIndex++;

                    }


                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }
        //End by rushikesh 13 Feb 2023
        
        //Added by rushikesh 13 Feb 2023 
        private String CreateExcelForDateDetails(List<RegistrationNoVerificationDetailsTableModel> result, string fileName, string excelHeader, int ErrorCode)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("RegistrationNoVerificationDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());

                    workSheet.Cells[5, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[5, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[5, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[5, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[1, 1, 1, 23].Merge = true;
                    workSheet.Cells[2, 1, 2, 23].Merge = true;
                    workSheet.Cells[3, 1, 3, 23].Merge = true;
                    workSheet.Cells[4, 1, 4, 23].Merge = true;
                    workSheet.Cells[5, 6, 5, 23].Merge = true;
                    workSheet.Cells[6, 1, 6, 23].Merge = true;


                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 45;
                    workSheet.Column(7).Width = 35;
                    workSheet.Column(8).Width = 40;
                    workSheet.Column(9).Width = 40;

                    workSheet.Column(10).Width = 40;
                    workSheet.Column(11).Width = 40;
                    workSheet.Column(12).Width = 40;
                    workSheet.Column(13).Width = 40;
                    workSheet.Column(14).Width = 40;
                    workSheet.Column(15).Width = 40;
                    workSheet.Column(16).Width = 40;
                    workSheet.Column(17).Width = 40;
                    workSheet.Column(18).Width = 40;
                    workSheet.Column(19).Width = 40;
                    workSheet.Column(20).Width = 40;
                    workSheet.Column(21).Width = 40;
                    workSheet.Column(22).Width = 40;
                    workSheet.Column(23).Width = 40;
                    workSheet.Column(24).Width = 40;
                    workSheet.Column(25).Width = 40;
                    workSheet.Column(26).Width = 40;
                    workSheet.Column(27).Width = 40;
                    workSheet.Column(28).Width = 40;
                    workSheet.Column(29).Width = 40;
                    workSheet.Column(30).Width = 40;
                    workSheet.Column(31).Width = 40;
                    workSheet.Column(32).Width = 40;
                    workSheet.Column(33).Width = 40;
                    workSheet.Column(34).Width = 40;
                    workSheet.Column(35).Width = 40;
                    workSheet.Column(36).Width = 40;
                    workSheet.Column(37).Width = 40;
                    workSheet.Column(38).Width = 40;
                    workSheet.Column(39).Width = 40;
                    workSheet.Column(40).Width = 40;
                    workSheet.Column(41).Width = 40;
                    workSheet.Column(42).Width = 40;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "DocumentID";
                    workSheet.Cells[7, 3].Value = "SROCode";
                    workSheet.Cells[7, 4].Value = "C_Stamp5DateTime";
                    workSheet.Cells[7, 5].Value = "L_Stamp5DateTime_1";
                    workSheet.Cells[7, 6].Value = "L_Stamp5DateTime";
                    workSheet.Cells[7, 7].Value = "C_FinalRegistrationNumber";
                    workSheet.Cells[7, 8].Value = "L_FinalRegistrationNumber";
                    workSheet.Cells[7, 9].Value = "C_ScannedFileName";
                    workSheet.Cells[7, 10].Value = "L_ScannedFileName";
                    workSheet.Cells[7, 11].Value = "C_CDNumber";
                    workSheet.Cells[7, 12].Value = "L_CDNumber";
                    workSheet.Cells[7, 13].Value = "C_ScanDate";
                    workSheet.Cells[7, 14].Value = "L_ScanDate";
                    workSheet.Cells[7, 15].Value = "C_Stamp1DateTime";
                    workSheet.Cells[7, 16].Value = "L_Stamp1DateTime";
                    workSheet.Cells[7, 17].Value = "C_Stamp2DateTime";
                    workSheet.Cells[7, 18].Value = "L_Stamp2DateTime";
                    workSheet.Cells[7, 19].Value = "C_Stamp3DateTime";
                    workSheet.Cells[7, 20].Value = "L_Stamp3DateTime";
                    workSheet.Cells[7, 21].Value = "C_Stamp4DateTime";
                    workSheet.Cells[7, 22].Value = "L_Stamp4DateTime";
                    workSheet.Cells[7, 23].Value = "C_PresentDateTime";
                    workSheet.Cells[7, 24].Value = "L_PresentDateTime";
                    workSheet.Cells[7, 25].Value = "C_ExecutionDateTime";
                    workSheet.Cells[7, 26].Value = "L_ExecutionDateTime";
                    workSheet.Cells[7, 27].Value = "C_DateOfStamp";
                    workSheet.Cells[7, 28].Value = "L_DateOfStamp";
                    workSheet.Cells[7, 29].Value = "C_WithdrawalDate";
                    workSheet.Cells[7, 30].Value = "L_WithdrawalDate";

                    workSheet.Cells[7, 31].Value = "C_RefusalDate";
                    workSheet.Cells[7, 32].Value = "L_Refusal";

                    //Added By Tushar on 29 Nov 2022
                    workSheet.Cells[7, 33].Value = "L_StartTime";
                    workSheet.Cells[7, 34].Value = "L_EndTime";
                    workSheet.Cells[7, 35].Value = "L_Filesize";
                    workSheet.Cells[7, 36].Value = "L_Pages";
                    workSheet.Cells[7, 37].Value = "L_Checksum";
                   
                    workSheet.Cells[7, 38].Value = "IsDuplicate";
                    //End By Tushar on 29 Nov 2022
                    workSheet.Cells[7, 39].Value = "BatchID";

                    workSheet.Cells[7, 40].Value = "Error Description";
                    workSheet.Cells[7, 41].Value = "DocumentTypeID";
                    workSheet.Cells[7, 42].Value = "BatchDateTime";

                    foreach (var items in result)
                    {
                        for (int i = 1; i < 43; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }


                        //workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.DocumentID;
                        workSheet.Cells[rowIndex, 3].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 4].Value = items.C_Stamp5DateTime;
                        workSheet.Cells[rowIndex, 5].Value = items.L_Stamp5DateTime_1;
                        workSheet.Cells[rowIndex, 6].Value = items.L_Stamp5DateTime;
                        workSheet.Cells[rowIndex, 7].Value = items.C_FRN;
                        workSheet.Cells[rowIndex, 8].Value = items.L_FRN;
                        workSheet.Cells[rowIndex, 9].Value = items.C_ScannedFileName;
                        workSheet.Cells[rowIndex, 10].Value = items.L_ScannedFileName;
                        workSheet.Cells[rowIndex, 11].Value = items.C_CDNumber;
                        workSheet.Cells[rowIndex, 12].Value = items.L_CDNumber;
                        workSheet.Cells[rowIndex, 13].Value = items.C_ScanFileUploadDateTime;
                        workSheet.Cells[rowIndex, 14].Value = items.L_ScanDate;
                        workSheet.Cells[rowIndex, 15].Value = items.C_Stamp1DateTime;
                        workSheet.Cells[rowIndex, 16].Value = items.L_Stamp1DateTime;
                        workSheet.Cells[rowIndex, 17].Value = items.C_Stamp2DateTime;
                        workSheet.Cells[rowIndex, 18].Value = items.L_Stamp2DateTime;
                        workSheet.Cells[rowIndex, 19].Value = items.C_Stamp3DateTime;
                        workSheet.Cells[rowIndex, 20].Value = items.L_Stamp3DateTime;
                        workSheet.Cells[rowIndex, 21].Value = items.C_Stamp4DateTime;
                        workSheet.Cells[rowIndex, 22].Value = items.L_Stamp4DateTime;
                        workSheet.Cells[rowIndex, 23].Value = items.C_PresentDateTime;
                        workSheet.Cells[rowIndex, 24].Value = items.L_PresentDateTime;
                        workSheet.Cells[rowIndex, 25].Value = items.C_ExecutionDateTime;
                        workSheet.Cells[rowIndex, 26].Value = items.L_ExecutionDateTime;
                        workSheet.Cells[rowIndex, 27].Value = items.C_DateOfStamp;
                        workSheet.Cells[rowIndex, 28].Value = items.L_DateOfStamp;
                        workSheet.Cells[rowIndex, 29].Value = items.C_WithdrawalDate;
                        workSheet.Cells[rowIndex, 30].Value = items.L_WithdrawalDate;
                        workSheet.Cells[rowIndex, 31].Value = items.C_RefusalDate;
                        workSheet.Cells[rowIndex, 32].Value = items.L_RefusalDate;
                        workSheet.Cells[rowIndex, 33].Value = items.L_StartTime;
                        workSheet.Cells[rowIndex, 34].Value = items.L_EndTime;
                        workSheet.Cells[rowIndex, 35].Value = items.L_Filesize;
                        workSheet.Cells[rowIndex, 36].Value = items.L_Pages;
                        workSheet.Cells[rowIndex, 37].Value = items.L_Checksum;
                        
                        workSheet.Cells[rowIndex, 38].Value = items.IsDuplicate;
                        workSheet.Cells[rowIndex, 39].Value = items.BatchID;
                        workSheet.Cells[rowIndex, 40].Value = items.ErrorType;
                        workSheet.Cells[rowIndex, 41].Value = items.DocumentTypeID;
                        workSheet.Cells[rowIndex, 42].Value = items.BatchDateTime;

                        for (int i = 1; i < 43; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        rowIndex++;

                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }
        //End by rushikesh 13 Feb 2023

        //Added By Tushar on 3 Jan 2023
        private string CreateExcelForPropertyAreaDetails(List<RegistrationNoVerificationDetailsTableModel> result, string fileName, string excelHeader, int ErrorCode)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("RegistrationNoVerification_PropertyAreaDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());

                    workSheet.Cells[1, 1, 1, 15].Merge = true;
                    workSheet.Cells[2, 1, 2, 15].Merge = true;
                    workSheet.Cells[3, 1, 3, 15].Merge = true;
                    workSheet.Cells[4, 1, 4, 15].Merge = true;
                    workSheet.Cells[5, 1, 5, 15].Merge = true;
                    workSheet.Cells[6, 1, 6, 15].Merge = true;


                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;

                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;
                    workSheet.Column(12).Width = 30;
                    workSheet.Column(13).Width = 30;
                    workSheet.Column(14).Width = 30;
                    workSheet.Column(15).Width = 30;
                    

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "C_SROCode";
                    workSheet.Cells[7, 3].Value = "L_SROCode";
                    workSheet.Cells[7, 4].Value = "C_PropertyID";
                    workSheet.Cells[7, 5].Value = "L_PropertyID";
                    workSheet.Cells[7, 6].Value = "C_VillageCode";
                    workSheet.Cells[7, 7].Value = "L_VillageCode";
                    workSheet.Cells[7, 8].Value = "C_TotalArea";
                    workSheet.Cells[7, 9].Value = "L_TotalArea";
                    workSheet.Cells[7, 10].Value = "C_MeasurementUnit";
                    workSheet.Cells[7, 11].Value = "L_MeasurementUnit";
                    workSheet.Cells[7, 12].Value = "C_DocumentID";
                    workSheet.Cells[7, 13].Value = "L_DocumentID";
                    workSheet.Cells[7, 14].Value = "BatchID";
                    workSheet.Cells[7, 15].Value = "Error Description";
     

                    foreach (var items in result)
                    {
                        for (int i = 1; i < 16; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }


                        
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.C_SROCode;
                        workSheet.Cells[rowIndex, 3].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 4].Value = items.C_PropertyID;
                        workSheet.Cells[rowIndex, 5].Value = items.PropertyID;
                        workSheet.Cells[rowIndex, 6].Value = items.C_VillageCode;
                        workSheet.Cells[rowIndex, 7].Value = items.VillageCode;
                        workSheet.Cells[rowIndex, 8].Value = items.C_TotalArea;
                        workSheet.Cells[rowIndex, 9].Value = items.TotalArea;
                        workSheet.Cells[rowIndex, 10].Value = items.C_MeasurementUnit;
                        workSheet.Cells[rowIndex, 11].Value = items.MeasurementUnit;
                        workSheet.Cells[rowIndex, 12].Value = items.C_DocumentID;    
                        workSheet.Cells[rowIndex, 13].Value = items.DocumentID;
                        workSheet.Cells[rowIndex, 14].Value = items.BatchID;
                        workSheet.Cells[rowIndex, 15].Value = items.ErrorType;
               

                        for (int i = 1; i < 16; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        rowIndex++;

                    }


                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }
        //End By Tushar on 3 jan 2023

        //Added By Tushar on 8 feb 2023
        public ActionResult ExportDateDetailsToExcel(string SROfficeID)
        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("DateDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                RegistrationNoVerificationDetailsModel reqModel = new RegistrationNoVerificationDetailsModel();
                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                //reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);

                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;
                RegistrationNoVerificationDetailsTableModel Result = caller.PostCall<RegistrationNoVerificationDetailsModel, RegistrationNoVerificationDetailsTableModel>("GetDateDetails", reqModel, out errorMessage);


                if (Result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
                List<RPT_DocReg_DateDetailsList> RPT_DocReg_DateDetailsList = Result.RPT_DocReg_DateDetailsList;

                if (Result.RPT_DocReg_DateDetailsList != null && Result.RPT_DocReg_DateDetailsList.Count > 0)
                {
                    //fileName = "FinalRegistrationNumberDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                   
                        fileName = "RPT_DocReg_DateDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                        excelHeader = string.Format("RPT_DocReg_DateDetails");
                    
                    

                    createdExcelPath = CreateExcelForDateDetails(RPT_DocReg_DateDetailsList, fileName, excelHeader);
                }
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);


                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }
        //End By Tushar on 8 Feb 2023

        private string CreateExcelForDateDetails(List<RPT_DocReg_DateDetailsList> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("FinalRegistrationNumberDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());


                    workSheet.Cells[1, 1, 1, 22].Merge = true;
                    workSheet.Cells[2, 1, 2, 22].Merge = true;
                    workSheet.Cells[3, 1, 3, 22].Merge = true;
                    workSheet.Cells[4, 1, 4, 22].Merge = true;
                    workSheet.Cells[5, 1, 5, 22].Merge = true;
                    workSheet.Cells[6, 1, 6, 22].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 40;

                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;
                    workSheet.Column(12).Width = 30;
                    workSheet.Column(13).Width = 30;
                    workSheet.Column(14).Width = 30;
                    workSheet.Column(15).Width = 30;
                    workSheet.Column(16).Width = 30;
                    workSheet.Column(17).Width = 30;
                    workSheet.Column(18).Width = 30;
                    workSheet.Column(19).Width = 30;
                    workSheet.Column(20).Width = 30;
                    workSheet.Column(21).Width = 30;
                    workSheet.Column(22).Width = 30;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "SROCode";
                    workSheet.Cells[7, 3].Value = "ID";
                    workSheet.Cells[7, 4].Value = "DocumentID";
                    workSheet.Cells[7, 5].Value = "TableName";
                    workSheet.Cells[7, 6].Value = "ReceiptID";
                    workSheet.Cells[7, 7].Value = "L_DateOfReceipt";
                    workSheet.Cells[7, 8].Value = "C_DateOfReceipt";
                    workSheet.Cells[7, 9].Value = "StampDetailsID";
                    workSheet.Cells[7, 10].Value = "L_DateOfStamp";
                    workSheet.Cells[7, 11].Value = "C_DateOfStamp";
                    workSheet.Cells[7, 12].Value = "L_DDChalDate";
                    workSheet.Cells[7, 13].Value = "C_DDChalDate";
                    workSheet.Cells[7, 14].Value = "L_StampPaymentDate";
                    workSheet.Cells[7, 15].Value = "C_StampPaymentDate";
                    workSheet.Cells[7, 16].Value = "L_DateOfReturn";
                    workSheet.Cells[7, 17].Value = "C_DateOfReturn";
                    workSheet.Cells[7, 18].Value = "PartyID";
                    workSheet.Cells[7, 19].Value = "L_AdmissionDate";
                    workSheet.Cells[7, 20].Value = "C_AdmissionDate";
                    workSheet.Cells[7, 21].Value = "BatchID";
                    workSheet.Cells[7, 22].Value = "ErrorType";
                

                    foreach (var items in result)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";


                       
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 3].Value = items.ID;
                        workSheet.Cells[rowIndex, 4].Value = items.DocumentID;
                        workSheet.Cells[rowIndex, 5].Value = items.TableName;
                        workSheet.Cells[rowIndex, 6].Value = items.ReceiptID;
                        workSheet.Cells[rowIndex, 7].Value = items.L_DateOfReceipt;
                        workSheet.Cells[rowIndex, 8].Value = items.C_DateOfReceipt;
                        workSheet.Cells[rowIndex, 9].Value = items.StampDetailsID;
                        workSheet.Cells[rowIndex, 10].Value = items.L_DateOfStamp;
                        workSheet.Cells[rowIndex, 11].Value = items.C_DateOfStamp;
                        workSheet.Cells[rowIndex, 12].Value = items.L_DDChalDate;
                        workSheet.Cells[rowIndex, 13].Value = items.C_DDChalDate;
                        workSheet.Cells[rowIndex, 14].Value = items.L_StampPaymentDate;
                        workSheet.Cells[rowIndex, 15].Value = items.C_StampPaymentDate;
                        workSheet.Cells[rowIndex, 16].Value = items.L_DateOfReturn;
                        workSheet.Cells[rowIndex, 17].Value = items.C_DateOfReturn;
                        workSheet.Cells[rowIndex, 18].Value = items.PartyID;
                        workSheet.Cells[rowIndex, 19].Value = items.L_AdmissionDate;
                        workSheet.Cells[rowIndex, 20].Value = items.C_AdmissionDate;
                        workSheet.Cells[rowIndex, 21].Value = items.BatchID;
                        workSheet.Cells[rowIndex, 22].Value = items.ErrorType;


                        for (int i = 1; i < 23; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        rowIndex++;

                    }


                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }
    }
}