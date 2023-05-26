using CustomModels.Models.MISReports.DocumentScanAndDeliveryReport;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]
    public class DocumentScanAndDeliveryController : Controller
    {
        ServiceCaller caller = null;

        [EventAuditLogFilter(Description = "Document Scan And Delivery View")]
        public ActionResult DocumentScanAndDeliveryView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DocumentScanandDelivery;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("DocumentScanAndDeliveryAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                DocumentScanAndDeliveryREQModel reqModel = caller.GetCall<DocumentScanAndDeliveryREQModel>("DocumentScanAndDeliveryView", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Document Scan And Delivery View", URLToRedirect = "/Home/HomePage" });
                }
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Document Scan And Delivery View", URLToRedirect = "/Home/HomePage" });
            }
        }

        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "All" });
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }

        [EventAuditLogFilter(Description = "Document Scan And Delivery Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult DocumentScanAndDeliveryDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("DocumentScanAndDeliveryAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {

                #region User Variables and Objects    
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                string FromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SroID = formCollection["SroID"];
                string DistrictID = formCollection["DistrictID"];
                string DocumentTypeID = formCollection["DocumentTypeId"];
                int SroId = Convert.ToInt32(SroID);
                int DistrictId = Convert.ToInt32(DistrictID);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum;
                int skip = startLen;
                //Validations of Logins other than SR and DR
                //if ((SroId == 0 && DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select any District"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //else if (SroId == 0 && DistrictId != 0)//when User selects DR but not SR which is by default "Select"
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select any SRO"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;

                //}

                if (string.IsNullOrEmpty(FromDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (string.IsNullOrEmpty(ToDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "To Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                //bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);

                if (!boolFrmDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid From Date"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (!boolToDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (frmDate > toDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "From Date can not be larger than To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (String.IsNullOrEmpty(DocumentTypeID))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Please Enter any Document Type"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;

                }
                else if (Convert.ToInt32(DocumentTypeID)==0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Please Enter any Document Type"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;

                }
                //30 days validation or 1 months vaidation
                else if ((toDate - frmDate).TotalDays > 30)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "You can only see records of one month."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                DocumentScanAndDeliveryREQModel reqModel = new DocumentScanAndDeliveryREQModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = FromDate;
                reqModel.ToDate = ToDate;
                reqModel.SROfficeID = SroId;
                reqModel.DistrictID = DistrictId;
                reqModel.DateTime_FromDate = frmDate;
                reqModel.DateTime_ToDate = toDate;
                reqModel.SearchValue = searchValue;
                reqModel.DocumentTypeID = Convert.ToInt32(DocumentTypeID);

                DocumentScanAndDeliveryWrapper ResModel = caller.PostCall<DocumentScanAndDeliveryREQModel, DocumentScanAndDeliveryWrapper>("DocumentScanAndDeliveryDetails", reqModel, out errorMessage);
                if (ResModel == null || ResModel.DocumentScanAndDeliveryDetailList == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Document Scan And Delivery Detail." });
                }

                IEnumerable<DocumentScanAndDeliveryDetail> result = ResModel.DocumentScanAndDeliveryDetailList;
                int totalCount = result.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (mtch.Success)
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
                    else
                    {
                        result = result.Where(m =>
                          m.OfficeName.ToLower().Contains(searchValue.ToLower()) ||
                          m.DocumentType.ToLower().Contains(searchValue.ToLower()) ||
                          m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                          m.LocalServerStoragePath.ToLower().Contains(searchValue.ToLower()) ||
                          m.FileUploadedToCentralServer.ToLower().Contains(searchValue.ToLower()) ||
                          m.StateDataCentreStoragePath.ToLower().Contains(searchValue.ToLower()) ||
                          m.SizeoftheFile.Contains(searchValue.ToLower()) ||
                          m.DateofScan.Contains(searchValue.ToLower()) ||
                          m.DateofUpload.Contains(searchValue.ToLower()) ||
                          m.ArchivedinCD.Contains(searchValue.ToLower()) ||
                          m.DocumentDeliveryDate.Contains(searchValue.ToLower()) ||
                          m.DateofRegistration.Contains(searchValue.ToLower())
                        );

                        totalCount = result.Count();
                    }
                }

                //var gridData = result.Select(DocumentScanAndDeliveryDetail => new
                //{
                //    SerialNo = DocumentScanAndDeliveryDetail.SerialNumber,
                //    OfficeName = DocumentScanAndDeliveryDetail.OfficeName,
                //    DocumentType = DocumentScanAndDeliveryDetail.DocumentType,
                //    FinalRegistrationNumber = DocumentScanAndDeliveryDetail.FinalRegistrationNumber,
                //    LocalServerStoragePath = DocumentScanAndDeliveryDetail.LocalServerStoragePath,
                //    FileUploadedToCentralServer = DocumentScanAndDeliveryDetail.FileUploadedToCentralServer,
                //    StateDataCentreStoragePath = DocumentScanAndDeliveryDetail.StateDataCentreStoragePath,
                //    SizeoftheFile = DocumentScanAndDeliveryDetail.SizeoftheFile,
                //    DateofScan = DocumentScanAndDeliveryDetail.DateofScan,
                //    DateofUpload = DocumentScanAndDeliveryDetail.DateofUpload,
                //    ArchivedinCD = DocumentScanAndDeliveryDetail.ArchivedinCD,
                //    DocumentDeliveryDate = DocumentScanAndDeliveryDetail.DocumentDeliveryDate,
                //    DateofRegistration = DocumentScanAndDeliveryDetail.DateofRegistration
                //});

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DistrictId + "','" + SroId + "','" + FromDate + "','" + ToDate + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                //var JsonData = "";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = result.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = ResModel.TotalRecords,
                        //commented on 22-11-2019
                        //status = "1",
                        recordsFiltered = totalCount,
                        //PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = result.ToArray(),
                        recordsTotal = ResModel.TotalRecords,
                        //commented on 22-11-2019
                        //status = "1",
                        recordsFiltered = ResModel.TotalRecords,
                        //PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Document Scan and delivery." }, JsonRequestBehavior.AllowGet);
            }
        }

        [EventAuditLogFilter(Description = "Document Scan And Delivery Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult DocumentScanAndDeliveryDetailsForSRO(FormCollection formCollection)
        {
            caller = new ServiceCaller("DocumentScanAndDeliveryAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {

                #region User Variables and Objects    
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                string FromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SroID = formCollection["SroID"];
                string DistrictID = formCollection["DistrictID"];
                string DocumentTypeId = formCollection["DocumentTypeId"];

                int SroId = Convert.ToInt32(SroID);
                int DistrictId = Convert.ToInt32(DistrictID);
                string IsSrLogin = formCollection["IsSrLogin"];


                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum;
                int skip = startLen;

                //Validations of Logins other than SR and DR
                //if ((SroId == 0 && DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select any District"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //else if (SroId == 0 && DistrictId != 0)//when User selects DR but not SR which is by default "Select"
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select any SRO"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;

                //}

                if (string.IsNullOrEmpty(FromDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (string.IsNullOrEmpty(ToDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "To Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                if (!boolFrmDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid From Date"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (!boolToDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (frmDate > toDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "From Date can not be larger than To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if ((toDate - frmDate).TotalDays > 30)
                {
                    //30 days validation or 1 months vaidation
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "You can only see records of one month."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (String.IsNullOrEmpty(DocumentTypeId))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Please Enter any Document Type"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;

                }
                else if (Convert.ToInt32(DocumentTypeId) == 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Please Enter any Document Type"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;

                }



                DocumentScanAndDeliveryREQModel reqModel = new DocumentScanAndDeliveryREQModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = FromDate;
                reqModel.ToDate = ToDate;
                reqModel.SROfficeID = SroId;
                reqModel.DistrictID = DistrictId;
                reqModel.DateTime_FromDate = frmDate;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DocumentTypeID = Convert.ToInt32(DocumentTypeId);
                if (IsSrLogin == "True")
                {
                    reqModel.IsSrLogin = true;
                }
                reqModel.SearchValue = searchValue;

                DocumentScanAndDeliveryWrapper ResModel = caller.PostCall<DocumentScanAndDeliveryREQModel, DocumentScanAndDeliveryWrapper>("DocumentScanAndDeliveryDetailsForSRO", reqModel, out errorMessage);
                if (ResModel == null || ResModel.DocumentScanAndDeliveryDetailList == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Document Scan And Delivery Detail." });
                }
                IEnumerable<DocumentScanAndDeliveryDetail> result = ResModel.DocumentScanAndDeliveryDetailList;
                int totalCount = result.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (mtch.Success)
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
                    else
                    {
                        result = result.Where(m =>
                          m.OfficeName.ToLower().Contains(searchValue.ToLower()) ||
                          m.DocumentType.ToLower().Contains(searchValue.ToLower()) ||
                          m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                          m.LocalServerStoragePath.ToLower().Contains(searchValue.ToLower()) ||
                          m.FileUploadedToCentralServer.ToLower().Contains(searchValue.ToLower()) ||
                          m.StateDataCentreStoragePath.ToLower().Contains(searchValue.ToLower()) ||
                          m.SizeoftheFile.Contains(searchValue.ToLower()) ||
                          m.DateofScan.Contains(searchValue.ToLower()) ||
                          m.DateofUpload.Contains(searchValue.ToLower()) ||
                          m.ArchivedinCD.Contains(searchValue.ToLower()) ||
                          m.DocumentDeliveryDate.Contains(searchValue.ToLower()) ||
                          m.DateofRegistration.Contains(searchValue.ToLower())
                        );

                        totalCount = result.Count();
                    }
                }

                //var gridData = result.Select(DocumentScanAndDeliveryDetail => new
                //{
                //    SerialNo = DocumentScanAndDeliveryDetail.SerialNumber,
                //    OfficeName = DocumentScanAndDeliveryDetail.OfficeName,
                //    DocumentType = DocumentScanAndDeliveryDetail.DocumentType,
                //    FinalRegistrationNumber = DocumentScanAndDeliveryDetail.FinalRegistrationNumber,
                //    LocalServerStoragePath = DocumentScanAndDeliveryDetail.LocalServerStoragePath,
                //    FileUploadedToCentralServer = DocumentScanAndDeliveryDetail.FileUploadedToCentralServer,
                //    StateDataCentreStoragePath = DocumentScanAndDeliveryDetail.StateDataCentreStoragePath,
                //    SizeoftheFile = DocumentScanAndDeliveryDetail.SizeoftheFile,
                //    DateofScan = DocumentScanAndDeliveryDetail.DateofScan,
                //    DateofUpload = DocumentScanAndDeliveryDetail.DateofUpload,
                //    ArchivedinCD = DocumentScanAndDeliveryDetail.ArchivedinCD,
                //    DocumentDeliveryDate = DocumentScanAndDeliveryDetail.DocumentDeliveryDate,
                //    DateofRegistration = DocumentScanAndDeliveryDetail.DateofRegistration
                //});

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DistrictId + "','" + SroId + "','" + FromDate + "','" + ToDate + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = result.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = ResModel.TotalRecords,
                        //commented on 22-11-2019
                        //status = "1",
                        recordsFiltered = totalCount,
                        //PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = result.ToArray(),
                        recordsTotal = ResModel.TotalRecords,
                        //commented on 22-11-2019
                        //status = "1",
                        recordsFiltered = ResModel.TotalRecords,
                        //PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Document Scan and delivery." }, JsonRequestBehavior.AllowGet);
            }
        }
        #region Excel

        [EventAuditLogFilter(Description = "Export Document Scan To Excel")]
        public ActionResult ExportDocumentScanToExcel(string FromDate, string ToDate, string SroID, string DistrictID, string Type, bool IsSROLogin,string DocumentType,string DocumentTypeId)
        {
            try
            {
                caller = new ServiceCaller("DocumentScanAndDeliveryAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "DocumentScanAndDelivery" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);

                DocumentScanAndDeliveryREQModel model = new DocumentScanAndDeliveryREQModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SroID),
                    DistrictID = Convert.ToInt32(DistrictID),
                    startLen = 0,
                    totalNum = 10,
                    DocumentTypeID= Convert.ToInt32(DocumentTypeId)
                };
                model.IsExcel = true;
                model.IsSrLogin = IsSROLogin;
                DocumentScanAndDeliveryWrapper ResModel = new DocumentScanAndDeliveryWrapper();
                //caller = new ServiceCaller("AnywhereECLogAPIController");
                //caller.HttpClient.Timeout = objTimeSpan;
                //int totalCount = caller.PostCall<DocumentScanAndDeliveryREQModel, int>("DocumentScanAndDeliveryCount", model);
                //model.totalNum = totalCount;
                //COMMENTED by shubham on 22-11-2019
                //model.IsExcel = true;
                if (Type == "SR")
                {
                    ResModel = caller.PostCall<DocumentScanAndDeliveryREQModel, DocumentScanAndDeliveryWrapper>("DocumentScanAndDeliveryDetailsForSro", model, out errorMessage);
                }
                else
                {
                    ResModel = caller.PostCall<DocumentScanAndDeliveryREQModel, DocumentScanAndDeliveryWrapper>("DocumentScanAndDeliveryDetails", model, out errorMessage);
                }

                if (ResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Document Scan And Delivery Details", URLToRedirect = "/Home/HomePage" });
                    //return Json(new { success = false, errorMessage = "Error Occured While Getting Document Scan And Delivery Details" }, JsonRequestBehavior.AllowGet);
                }

                if (ResModel.DocumentScanAndDeliveryDetailList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Document Scan And Delivery Details", URLToRedirect = "/Home/HomePage" });
                    //return Json(new { success = false, errorMessage = "Error Occured While Getting Document Scan And Delivery Details" }, JsonRequestBehavior.AllowGet);
                }
                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();
                //}
                string createdExcelPath = string.Empty;
                string excelHeader = string.Format("Document Scan And Delivery Report ({0} and {1})", FromDate, ToDate);
                if (Type == "SR")
                {
                    createdExcelPath = CreateExcelForSR(ResModel, fileName, excelHeader, DocumentType);
                }
                else if (Type == "DR")
                {
                    createdExcelPath = CreateExcelForDR(ResModel, fileName, excelHeader);
                }
                // string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader);

                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();
                //}
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
        private string CreateExcelForDR(DocumentScanAndDeliveryWrapper ResModel, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Document Scan And Delivery");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    //workSheet.Cells[2, 1].Value = "District : " + ResModel.SelectedDRO;
                    workSheet.Cells[2, 1].Value = "DRO : " + ResModel.SelectedDRO;
                    //workSheet.Cells[4, 1].Value = "Log Type : " + SelectedLogType;

                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (ResModel.DocumentScanAndDeliveryDetailList.Count());

                    workSheet.Cells[1, 1, 1, 13].Merge = true;
                    workSheet.Cells[2, 1, 2, 13].Merge = true;
                    workSheet.Cells[3, 1, 3, 13].Merge = true;
                    workSheet.Cells[4, 1, 4, 13].Merge = true;
                    workSheet.Cells[5, 1, 5, 13].Merge = true;
                    //workSheet.Cells[6, 1, 6, 13].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 50;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 40;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 35;
                    workSheet.Column(12).Width = 30;
                    workSheet.Column(13).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;

                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(6).Style.WrapText = true;

                    workSheet.Cells[6, 1].Value = "Serial No.";
                    workSheet.Cells[6, 2].Value = "Office Name";
                    workSheet.Cells[6, 3].Value = "Document Type";
                    workSheet.Cells[6, 4].Value = "Registration Number";
                    workSheet.Cells[6, 5].Value = "Local Server Storage Path";
                    workSheet.Cells[6, 6].Value = "File Uploaded to Central Server [Yes/No]";
                    workSheet.Cells[6, 7].Value = "State Data Centre Storage Path";
                    workSheet.Cells[6, 8].Value = "Size of the File (KB)";
                    workSheet.Cells[6, 9].Value = "Date of Scan";
                    workSheet.Cells[6, 10].Value = "Date of Upload";
                    workSheet.Cells[6, 11].Value = "Archived in CD Number";
                    workSheet.Cells[6, 12].Value = "Document Delivery Date";
                    workSheet.Cells[6, 13].Value = "Registration Date";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";


                    foreach (var items in ResModel.DocumentScanAndDeliveryDetailList)
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
                        workSheet.Cells[rowIndex, 10].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 11].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 12].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 13].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 1].Value = items.SerialNumber;
                        workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                        workSheet.Cells[rowIndex, 3].Value = items.DocumentType;
                        workSheet.Cells[rowIndex, 4].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.LocalServerStoragePath;
                        workSheet.Cells[rowIndex, 6].Value = items.FileUploadedToCentralServer;
                        workSheet.Cells[rowIndex, 7].Value = items.StateDataCentreStoragePath;
                        workSheet.Cells[rowIndex, 8].Value = items.SizeoftheFile;
                        workSheet.Cells[rowIndex, 9].Value = items.DateofScan;
                        workSheet.Cells[rowIndex, 10].Value = items.DateofUpload;
                        workSheet.Cells[rowIndex, 11].Value = items.ArchivedinCD;
                        workSheet.Cells[rowIndex, 12].Value = items.DocumentDeliveryDate;
                        workSheet.Cells[rowIndex, 13].Value = items.DateofRegistration;

                        workSheet.Cells[rowIndex, 1].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 9].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 10].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 11].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 12].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 13].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 13])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    //using (ExcelRange Rng = workSheet.Cells[8, 6, (rowIndex - 1), 8])
                    //{
                    //    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    //}
                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return ExcelFilePath;
        }
        private string CreateExcelForSR(DocumentScanAndDeliveryWrapper ResModel, string fileName, string excelHeader,string DocumentType)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {

                if (KaveriSession.Current.RoleID == Convert.ToInt32(CommonEnum.RoleDetails.SR))
                {
                    //create a new ExcelPackage
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        var workbook = package.Workbook;
                        var workSheet = package.Workbook.Worksheets.Add("Document Scan And Delivery");
                        workSheet.Cells.Style.Font.Size = 14;

                        workSheet.Cells[1, 1].Value = excelHeader;
                        //workSheet.Cells[2, 1].Value = "District : " + ResModel.SelectedDRO;
                        workSheet.Cells[2, 1].Value = "SRO : " + ResModel.SelectedSRO;
                        workSheet.Cells[3, 1].Value = "DocumentType : " + DocumentType;
                        //workSheet.Cells[4, 1].Value = "Log Type : " + SelectedLogType;

                        workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                        workSheet.Cells[5, 1].Value = "Total Records : " + (ResModel.DocumentScanAndDeliveryDetailList.Count());

                        workSheet.Cells[1, 1, 1, 13].Merge = true;
                        workSheet.Cells[2, 1, 2, 13].Merge = true;
                        workSheet.Cells[3, 1, 3, 13].Merge = true;
                        workSheet.Cells[4, 1, 4, 13].Merge = true;
                        workSheet.Cells[5, 1, 5, 13].Merge = true;
                        workSheet.Cells[6, 1, 6, 13].Merge = true;

                        workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Column(1).Width = 20;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 30;
                        workSheet.Column(4).Width = 40;
                        workSheet.Column(5).Width = 50;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(7).Width = 40;
                        workSheet.Column(8).Width = 30;
                        workSheet.Column(9).Width = 30;
                        workSheet.Column(10).Width = 30;
                        workSheet.Column(11).Width = 35;
                        workSheet.Column(12).Width = 30;
                        workSheet.Column(13).Width = 30;

                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Row(2).Style.Font.Bold = true;
                        workSheet.Row(3).Style.Font.Bold = true;
                        workSheet.Row(4).Style.Font.Bold = true;
                        workSheet.Row(5).Style.Font.Bold = true;
                        workSheet.Row(6).Style.Font.Bold = true;

                        int rowIndex = 8;
                        workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(7).Style.WrapText = true;

                        workSheet.Cells[7, 1].Value = "Serial No.";
                        workSheet.Cells[7, 2].Value = "Office Name";
                        workSheet.Cells[7, 3].Value = "Document Type";
                        workSheet.Cells[7, 4].Value = "Registration Number";
                        workSheet.Cells[7, 5].Value = "Local Server Storage Path";
                        workSheet.Cells[7, 6].Value = "File Uploaded to Central Server [Yes/No]";
                        workSheet.Cells[7, 7].Value = "State Data Centre Storage Path";
                        workSheet.Cells[7, 8].Value = "Size of the File (KB)";
                        workSheet.Cells[7, 9].Value = "Date of Scan";
                        workSheet.Cells[7, 10].Value = "Date of Upload";
                        workSheet.Cells[7, 11].Value = "Archived in CD Number";
                        workSheet.Cells[7, 12].Value = "Document Delivery Date";
                        workSheet.Cells[7, 13].Value = "Registration Date";

                        workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Row(8).Style.Font.Bold = true;

                        foreach (var items in ResModel.DocumentScanAndDeliveryDetailList)
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
                            workSheet.Cells[rowIndex, 10].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 11].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 12].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 13].Style.Font.Name = "KNB-TTUmaEN";


                            workSheet.Cells[rowIndex, 1].Value = items.SerialNumber;
                            workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                            workSheet.Cells[rowIndex, 3].Value = items.DocumentType;
                            workSheet.Cells[rowIndex, 4].Value = items.FinalRegistrationNumber;
                            workSheet.Cells[rowIndex, 5].Value = items.LocalServerStoragePath;
                            workSheet.Cells[rowIndex, 6].Value = items.FileUploadedToCentralServer;
                            workSheet.Cells[rowIndex, 7].Value = items.StateDataCentreStoragePath;
                            workSheet.Cells[rowIndex, 8].Value = items.SizeoftheFile;
                            workSheet.Cells[rowIndex, 9].Value = items.DateofScan;
                            workSheet.Cells[rowIndex, 10].Value = items.DateofUpload;
                            workSheet.Cells[rowIndex, 11].Value = items.ArchivedinCD;
                            workSheet.Cells[rowIndex, 12].Value = items.DocumentDeliveryDate;
                            workSheet.Cells[rowIndex, 13].Value = items.DateofRegistration;

                            workSheet.Cells[rowIndex, 1].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 8].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 9].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 10].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 11].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 12].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 13].Style.WrapText = true;

                            workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                            workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            rowIndex++;
                            //Function that passes the current row and adds the column details 
                            //AddSubRowsForCurrentRow(out row,out workSheet);
                        }

                        using (ExcelRange Rng = workSheet.Cells[8, 1, (rowIndex - 1), 13])
                        {

                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                        //using (ExcelRange Rng = workSheet.Cells[8, 6, (rowIndex - 1), 8])
                        //{
                        //    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        //}
                        package.SaveAs(templateFile);
                    }

                }
                else
                {
                    //create a new ExcelPackage
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        var workbook = package.Workbook;
                        var workSheet = package.Workbook.Worksheets.Add("Document Scan And Delivery");
                        workSheet.Cells.Style.Font.Size = 14;

                        workSheet.Cells[1, 1].Value = excelHeader;
                        //workSheet.Cells[2, 1].Value = "District : " + ResModel.SelectedDRO;
                        workSheet.Cells[2, 1].Value = "District : " + ResModel.SelectedDRO;
                        workSheet.Cells[3, 1].Value = "SRO : " + ResModel.SelectedSRO;
                        workSheet.Cells[4, 1].Value = "Document Type : " + DocumentType;
                        workSheet.Cells[5, 1].Value = "Print Date Time : " + DateTime.Now;
                        workSheet.Cells[6, 1].Value = "Total Records : " + (ResModel.DocumentScanAndDeliveryDetailList.Count());

                        workSheet.Cells[1, 1, 1, 13].Merge = true;
                        workSheet.Cells[2, 1, 2, 13].Merge = true;
                        workSheet.Cells[3, 1, 3, 13].Merge = true;
                        workSheet.Cells[4, 1, 4, 13].Merge = true;
                        workSheet.Cells[5, 1, 5, 13].Merge = true;
                        workSheet.Cells[6, 1, 6, 13].Merge = true;//workSheet.Cells[6, 1, 6, 13].Merge = true;

                        workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Column(1).Width = 20;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 30;
                        workSheet.Column(4).Width = 40;
                        workSheet.Column(5).Width = 50;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(7).Width = 40;
                        workSheet.Column(8).Width = 30;
                        workSheet.Column(9).Width = 30;
                        workSheet.Column(10).Width = 30;
                        workSheet.Column(11).Width = 35;
                        workSheet.Column(12).Width = 30;
                        workSheet.Column(13).Width = 30;

                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Row(2).Style.Font.Bold = true;
                        workSheet.Row(3).Style.Font.Bold = true;
                        workSheet.Row(4).Style.Font.Bold = true;
                        workSheet.Row(5).Style.Font.Bold = true;
                        workSheet.Row(6).Style.Font.Bold = true;
                        workSheet.Row(7).Style.Font.Bold = true;
                        workSheet.Row(8).Style.Font.Bold = true;

                        int rowIndex = 9;
                        workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(8).Style.WrapText = true;

                        workSheet.Cells[8, 1].Value = "Serial No.";
                        workSheet.Cells[8, 2].Value = "Office Name";
                        workSheet.Cells[8, 3].Value = "Document Type";
                        workSheet.Cells[8, 4].Value = "Registration Number";
                        workSheet.Cells[8, 5].Value = "Local Server Storage Path";
                        workSheet.Cells[8, 6].Value = "File Uploaded to Central Server [Yes/No]";
                        workSheet.Cells[8, 7].Value = "State Data Centre Storage Path";
                        workSheet.Cells[8, 8].Value = "Size of the File (KB)";
                        workSheet.Cells[8, 9].Value = "Date of Scan";
                        workSheet.Cells[8, 10].Value = "Date of Upload";
                        workSheet.Cells[8, 11].Value = "Archived in CD Number";
                        workSheet.Cells[8, 12].Value = "Document Delivery Date";
                        workSheet.Cells[8, 13].Value = "Registration Date";

                        workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(8).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        foreach (var items in ResModel.DocumentScanAndDeliveryDetailList)
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
                            workSheet.Cells[rowIndex, 10].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 11].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 12].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 13].Style.Font.Name = "KNB-TTUmaEN";


                            workSheet.Cells[rowIndex, 1].Value = items.SerialNumber;
                            workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                            workSheet.Cells[rowIndex, 3].Value = items.DocumentType;
                            workSheet.Cells[rowIndex, 4].Value = items.FinalRegistrationNumber;
                            workSheet.Cells[rowIndex, 5].Value = items.LocalServerStoragePath;
                            workSheet.Cells[rowIndex, 6].Value = items.FileUploadedToCentralServer;
                            workSheet.Cells[rowIndex, 7].Value = items.StateDataCentreStoragePath;
                            workSheet.Cells[rowIndex, 8].Value = items.SizeoftheFile;
                            workSheet.Cells[rowIndex, 9].Value = items.DateofScan;
                            workSheet.Cells[rowIndex, 10].Value = items.DateofUpload;
                            workSheet.Cells[rowIndex, 11].Value = items.ArchivedinCD;
                            workSheet.Cells[rowIndex, 12].Value = items.DocumentDeliveryDate;
                            workSheet.Cells[rowIndex, 13].Value = items.DateofRegistration;

                            workSheet.Cells[rowIndex, 1].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 8].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 9].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 10].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 11].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 12].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 13].Style.WrapText = true;

                            workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                            workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            rowIndex++;
                            //Function that passes the current row and adds the column details 
                            //AddSubRowsForCurrentRow(out row,out workSheet);
                        }

                        using (ExcelRange Rng = workSheet.Cells[8, 1, (rowIndex - 1), 13])
                        {

                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                        //using (ExcelRange Rng = workSheet.Cells[8, 6, (rowIndex - 1), 8])
                        //{
                        //    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        //}
                        package.SaveAs(templateFile);
                    }

                }

            }
            catch (Exception)
            {
                throw;
            }

            return ExcelFilePath;
        }

        /// <summary>
        /// Get File Info
        /// </summary>
        /// <param name="tempExcelFilePath"></param>
        /// <returns>returns file info</returns>
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }
        #endregion

        //#region Download report in PDF
        ///// <summary>
        ///// Export Report To PDF
        ///// </summary>
        ///// <param name="FromDate"></param>
        ///// <param name="ToDate"></param>
        ///// <param name="SroID"></param>
        ///// <param name="DistrictID"></param>
        ///// <param name="SelectedDist"></param>
        ///// <param name="SelectedSRO"></param>
        ///// <param name="SelectedLogType"></param>
        ///// <returns>returns pdf file</returns>
        //[EventAuditLogFilter(Description = "Export Anywhere EC Log to PDF")]
        //public ActionResult ExportAnywhereECRptToPDF(string FromDate, string ToDate, string SroID, string DistrictID, string LogTypeID, string SelectedDist, string SelectedSRO, string SelectedLogType)
        //{
        //    try
        //    {
        //        CommonFunctions objCommon = new CommonFunctions();
        //        string errorMessage = string.Empty;

        //        AnywhereECLogView model = new AnywhereECLogView
        //        {

        //            FromDate = FromDate,
        //            ToDate = ToDate,
        //            SROfficeID = Convert.ToInt32(SroID),
        //            startLen = 0,
        //            totalNum = 10,
        //            DistrictID = Convert.ToInt32(DistrictID),
        //            LogTypeID = Convert.ToInt32(LogTypeID)
        //        };

        //        List<AnywhereECLogDetailsModel> objListItemsToBeExported = new List<AnywhereECLogDetailsModel>();

        //        caller = new ServiceCaller("AnywhereECLogAPIController");
        //        TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
        //        caller.HttpClient.Timeout = objTimeSpan;

        //        int totalCount = caller.PostCall<AnywhereECLogView, int>("GetAnywhereECLogTotalCount", model, out errorMessage);
        //        model.totalNum = totalCount;
        //        AnywhereECLogResModel ResModel = new AnywhereECLogResModel();
        //        // To get total records of Anywhere EC Log
        //        ResModel = caller.PostCall<AnywhereECLogView, AnywhereECLogResModel>("GetAnywhereECLogDetails", model, out errorMessage);
        //        objListItemsToBeExported = ResModel.AnywhereECDetailsList;
        //        if (objListItemsToBeExported == null)
        //        {
        //            return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
        //        }

        //        //string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.pdf",  DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
        //        string fileName = string.Format("AnywhereECLog.pdf");
        //        string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //        string pdfHeader = string.Format("Anywhere EC Log Between (" + FromDate + " and " + ToDate + ")");

        //        //To get SRONAME
        //        // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

        //        //Create Temp PDF File
        //        // byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader, SROName);
        //        byte[] pdfBytes = CreatePDFFile(ResModel, fileName, pdfHeader, SelectedSRO, SelectedDist, SelectedLogType);

        //        return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "AnywhereECLog_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");

        //    }
        //    catch (Exception e)
        //    {
        //        ExceptionLogs.LogException(e);
        //        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
        //    }
        //}


        //private byte[] CreatePDFFile(AnywhereECLogResModel ResModel, string fileName, string pdfHeader, string SelectedSRO, string SelectedDist, string SelectedLogType)
        //{
        //    string folderPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/"));

        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }
        //    string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //    try
        //    {
        //        byte[] pdfBytes = null;

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (Document doc = new Document(PageSize.A4.Rotate(), 35, 10, 10, 25))
        //            {
        //                using (PdfWriter writer = PdfWriter.GetInstance(doc, ms))
        //                {

        //                    //  string Info = string.Format("Print Date Time : {0}   Total Records : {1}  SRO Name : {2}", DateTime.Now.ToString(), SROName);
        //                    doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
        //                    var headerTextFont = FontFactory.GetFont("Arial", 15, new BaseColor(0, 128, 255));
        //                    doc.Open();
        //                    Paragraph addHeading = new Paragraph(pdfHeader, headerTextFont)
        //                    {
        //                        Alignment = 1,
        //                    };
        //                    //Paragraph Info = new Paragraph(Info, redListTextFont)
        //                    //{
        //                    //    Alignment = 1,
        //                    //};
        //                    Paragraph addSpace = new Paragraph(" ")
        //                    {
        //                        Alignment = 1
        //                    };
        //                    var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
        //                    //var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(128,191,255));
        //                    var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(94, 154, 214));


        //                    var titleChunk = new Chunk("    Print Date Time : ", blackListTextFont);
        //                    var totalChunk = new Chunk("Total Records: ", blackListTextFont);
        //                    var SroNameChunk = new Chunk("    SRO : ", blackListTextFont);
        //                    var DistrictChunk = new Chunk("District : ", blackListTextFont);
        //                    var LogTypeChunk = new Chunk("Log Type : ", blackListTextFont);


        //                    var descriptionChunk = new Chunk(DateTime.Now.ToString() + "       ", redListTextFont);
        //                    string count = ResModel.AnywhereECDetailsList.Count().ToString();
        //                    var countChunk = new Chunk(count, redListTextFont);
        //                    var District = new Chunk(SelectedDist + "      ", redListTextFont);
        //                    var SRO = new Chunk(SelectedSRO + "      ", redListTextFont);
        //                    var LogType = new Chunk(SelectedLogType + "      ", redListTextFont);



        //                    var titlePhrase = new Phrase(titleChunk)
        //                    {
        //                        descriptionChunk
        //                    };
        //                    var totalPhrase = new Phrase(totalChunk)
        //                    {
        //                        countChunk
        //                    };

        //                    var SROPhrase = new Phrase(SroNameChunk)
        //                    {
        //                        SRO
        //                    };
        //                    var DistrictPhrase = new Phrase(DistrictChunk)
        //                    {
        //                        District
        //                    };
        //                    var LogTypePhrase = new Phrase(LogTypeChunk)
        //                    {
        //                        LogType
        //                    };
        //                    var FontItalic = FontFactory.GetFont("Arial", 10, 2, new BaseColor(94, 94, 94));
        //                    Paragraph NotePara = new Paragraph("Note : This report is based on pre processed data considered upto : ", FontItalic);
        //                    NotePara.Alignment = Element.ALIGN_RIGHT;

        //                    doc.Add(addHeading);
        //                    doc.Add(addSpace);
        //                    doc.Add(DistrictPhrase);
        //                    doc.Add(SROPhrase);
        //                    doc.Add(LogTypePhrase);
        //                    doc.Add(titlePhrase);
        //                    //doc.Add(SroNamePhrase);
        //                    doc.Add(totalPhrase);
        //                    //doc.Add(addSpace);
        //                    //doc.Add(NotePara);
        //                    doc.Add(addSpace);
        //                    doc.Add(ReportTable(ResModel));
        //                    doc.Close();
        //                }
        //                pdfBytes = AddpageNumber(ms.ToArray());
        //            }

        //        }
        //        return pdfBytes;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        ////To get PdfPtable of Anywhere EC Log report 
        ///// <summary>
        ///// Report Table
        ///// </summary>
        ///// <param name="ResModel"></param>
        ///// <returns>returns pdf table</returns>
        //private PdfPTable ReportTable(AnywhereECLogResModel ResModel)
        //{
        //    string SerialNumber = "Serial Number";
        //    string ApplicationNumber = "Application Number";
        //    string SROfficeApplicationNumber = "SR Office Application Number";
        //    string UserName = "User Name";
        //    string Description = "Description";
        //    string LogDateTime = "Log Date Time";
        //    string ApplicationFilingDate = "Application Filing Date";


        //    try
        //    {
        //        string[] col = { SerialNumber, ApplicationNumber, SROfficeApplicationNumber, ApplicationFilingDate, UserName, Description, LogDateTime };
        //        PdfPTable table = new PdfPTable(7)
        //        {
        //            WidthPercentage = 100
        //        };
        //        // table.DefaultCell.FixedHeight = 500f;

        //        string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
        //        string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
        //        BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        //        iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 14);

        //        //to repeat Headers
        //        table.HeaderRows = 1;
        //        // then set the column's __relative__ widths
        //        table.SetWidths(new Single[] { 4, 7, 8, 5, 6, 6, 6 });
        //        /*
        //        * by default tables 'collapse' on surrounding elements,
        //        * so you need to explicitly add spacing
        //        */
        //        //table.SpacingBefore = 10;

        //        // PdfPCell cell = null;
        //        PdfPCell cell1 = null;
        //        PdfPCell cell2 = null;
        //        PdfPCell cell3 = null;
        //        PdfPCell cell4 = null;
        //        PdfPCell cell5 = null;
        //        PdfPCell cell6 = null;
        //        PdfPCell cell7 = null;

        //        for (int i = 0; i < col.Length; ++i)
        //        {
        //            PdfPCell cell = new PdfPCell(new Phrase(col[i]))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;

        //            table.AddCell(cell);

        //        }

        //        foreach (var items in ResModel.AnywhereECDetailsList)
        //        {


        //            cell1 = new PdfPCell(new Phrase(items.SerialNo.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell1.BackgroundColor = BaseColor.WHITE;


        //            cell2 = new PdfPCell(new Phrase(items.ApplicationNo, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell2.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell2.BackgroundColor = BaseColor.WHITE;

        //            cell3 = new PdfPCell(new Phrase(items.SROfficeAppNo.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell3.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell3.BackgroundColor = BaseColor.WHITE;

        //            cell4 = new PdfPCell(new Phrase(items.UserName, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell4.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell4.BackgroundColor = BaseColor.WHITE;

        //            cell5 = new PdfPCell(new Phrase(items.Desc, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell5.BackgroundColor = BaseColor.WHITE;

        //            cell5.HorizontalAlignment = Element.ALIGN_LEFT;


        //            cell6 = new PdfPCell(new Phrase(items.LogDateTime.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell6.BackgroundColor = BaseColor.WHITE;
        //            cell6.HorizontalAlignment = Element.ALIGN_CENTER;

        //            cell7 = new PdfPCell(new Phrase(items.ApplicationFilingDate.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell7.BackgroundColor = BaseColor.WHITE;

        //            cell7.HorizontalAlignment = Element.ALIGN_CENTER;

        //            table.AddCell(cell1);
        //            table.AddCell(cell2);
        //            table.AddCell(cell3);
        //            table.AddCell(cell7);
        //            table.AddCell(cell4);
        //            table.AddCell(cell5);
        //            table.AddCell(cell6);
        //        }

        //        PdfPCell bottomCell = null;
        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public byte[] AddpageNumber(byte[] inputArray)
        //{
        //    byte[] pdfBytes = null;
        //    CommonFunctions objCommon = new CommonFunctions();
        //    iTextSharp.text.Font fntrow = objCommon.DefineNormaFont("Times New Roman", 12);

        //    using (MemoryStream stream = new MemoryStream())
        //    {

        //        PdfReader reader = new PdfReader(inputArray);
        //        using (PdfStamper stamper = new PdfStamper(reader, stream))
        //        {
        //            int pages = reader.NumberOfPages;
        //            for (int i = 1; i <= pages; i++)
        //            {
        //                ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_MIDDLE, new Phrase("Page " + i.ToString() + " of " + pages, fntrow), 420f, 16f, 0);
        //            }
        //        }
        //        pdfBytes = stream.ToArray();
        //    }

        //    return pdfBytes;

        //}

        //#endregion
    }
}
