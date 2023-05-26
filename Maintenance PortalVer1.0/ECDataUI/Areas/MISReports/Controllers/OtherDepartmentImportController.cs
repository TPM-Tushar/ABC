using CustomModels.Models.MISReports.OtherDepartmentImport;
using CustomModels.Models.Common;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.Web.UI;
using CustomModels.Models.MISReports.TransactionDetails;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]
    public class OtherDepartmentImportController : Controller
    {
        ServiceCaller caller = null;

        [EventAuditLogFilter(Description = "Other Department Import View")]
        public ActionResult OtherDepartmentImportView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.OtherDepartmentImport;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("OtherDepartmentImportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                OtherDepartmentImportREQModel reqModel = caller.GetCall<OtherDepartmentImportREQModel>("OtherDepartmentImportView", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Other Department Import View", URLToRedirect = "/Home/HomePage" });
                }
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Other Department Import View", URLToRedirect = "/Home/HomePage" });
            }
        }

        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "All" });
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }

        [EventAuditLogFilter(Description = "Other Department Import Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult OtherDepartmentImportDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("OtherDepartmentImportAPIController");
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
                string STRSroID = formCollection["SroID"];
                string STRDistrictID = formCollection["DistrictID"];
                string STRReportNameID = formCollection["ReportNameID"];

                int INTSroId = Convert.ToInt32(STRSroID);
                int INTDistrictId = Convert.ToInt32(STRDistrictID);


                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                //int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                                    //short OfficeID = KaveriSession.Current.OfficeID;
                                    //short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });

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

                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);

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

                // 30 days validation or 1 months vaidation
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

                //else
                //{
                //    #region Validation For Allowing Date range between only Current Financial year(Validation for From Date)
                //    DateTime CurrentDate = DateTime.Now;
                //    int CMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                //    int CYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));

                //    int FromDateyear = Convert.ToInt32(frmDate.ToString("yyyy"));
                //    int FromDateMonth = Convert.ToInt32(frmDate.ToString("MM"));
                //    if (CMonth > 3)
                //    {
                //        if (FromDateyear < CYear)
                //        {
                //            var emptyData = Json(new
                //            {
                //                draw = formCollection["draw"],
                //                recordsTotal = 0,
                //                recordsFiltered = 0,
                //                data = "",
                //                status = false,
                //                errorMessage = "Records of current financial year only can be seen at a time"
                //            });
                //            emptyData.MaxJsonLength = Int32.MaxValue;
                //            return emptyData;

                //        }
                //        else
                //        {
                //            if (FromDateyear > CYear)
                //            {
                //                if (FromDateyear == CYear + 1)
                //                {
                //                    if (FromDateMonth > 3)
                //                    {
                //                        var emptyData = Json(new
                //                        {
                //                            draw = formCollection["draw"],
                //                            recordsTotal = 0,
                //                            recordsFiltered = 0,
                //                            data = "",
                //                            status = false,
                //                            errorMessage = "Records of current financial year only can be seen at a time"
                //                        });
                //                        emptyData.MaxJsonLength = Int32.MaxValue;
                //                        return emptyData;

                //                    }

                //                }
                //                else
                //                {
                //                    var emptyData = Json(new
                //                    {
                //                        draw = formCollection["draw"],
                //                        recordsTotal = 0,
                //                        recordsFiltered = 0,
                //                        data = "",
                //                        status = false,
                //                        errorMessage = "Records of current financial year only can be seen at a time"
                //                    });
                //                    emptyData.MaxJsonLength = Int32.MaxValue;
                //                    return emptyData;
                //                }
                //            }
                //            else if (FromDateyear == CYear)
                //            {
                //                if (FromDateMonth < 4)
                //                {
                //                    var emptyData = Json(new
                //                    {
                //                        draw = formCollection["draw"],
                //                        recordsTotal = 0,
                //                        recordsFiltered = 0,
                //                        data = "",
                //                        status = false,
                //                        errorMessage = "Records of current financial year only can be seen at a time"
                //                    });
                //                    emptyData.MaxJsonLength = Int32.MaxValue;
                //                    return emptyData;

                //                }
                //            }



                //        }

                //    }
                //    else if (CMonth <= 3)
                //    {
                //        if (FromDateyear < CYear)
                //        {
                //            if (FromDateyear == CYear - 1)
                //            {
                //                if (FromDateMonth < 4)
                //                {
                //                    var emptyData = Json(new
                //                    {
                //                        draw = formCollection["draw"],
                //                        recordsTotal = 0,
                //                        recordsFiltered = 0,
                //                        data = "",
                //                        status = false,
                //                        errorMessage = "Records of current financial year only can be seen at a time"
                //                    });
                //                    emptyData.MaxJsonLength = Int32.MaxValue;
                //                    return emptyData;

                //                }

                //            }
                //            else
                //            {
                //                var emptyData = Json(new
                //                {
                //                    draw = formCollection["draw"],
                //                    recordsTotal = 0,
                //                    recordsFiltered = 0,
                //                    data = "",
                //                    status = false,
                //                    errorMessage = "Records of current financial year only can be seen at a time"
                //                });
                //                emptyData.MaxJsonLength = Int32.MaxValue;
                //                return emptyData;


                //            }

                //        }

                //    }
                //    #endregion
                //}

                OtherDepartmentImportREQModel reqModel = new OtherDepartmentImportREQModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = FromDate;
                reqModel.ToDate = ToDate;
                reqModel.SROfficeID = INTSroId;
                reqModel.DistrictID = INTDistrictId;
                reqModel.DateTime_FromDate = frmDate;
                reqModel.DateTime_ToDate = toDate;
                reqModel.ReportName = STRReportNameID;

                int totalCount = caller.PostCall<OtherDepartmentImportREQModel, int>("OtherDepartmentImportCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                OtherDepartmentImportWrapper ResModel = caller.PostCall<OtherDepartmentImportREQModel, OtherDepartmentImportWrapper>("OtherDepartmentImportDetails", reqModel, out errorMessage);

                IEnumerable<OtherDepartmentImportDetail> result = ResModel.OtherDepartmentImportDetailList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Document References Details." });
                }

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                // Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                //}

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
                          m.OfficeName.ToLower().Contains(searchValue.ToLower()) ||
                          m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                          m.PropertyID.ToLower().Contains(searchValue.ToLower()) ||
                          //m.ImportedXML.ToLower().Contains(searchValue.ToLower()) ||
                          //m.ExportedXML.ToLower().Contains(searchValue.ToLower()) ||
                          m.ReferenceNumber.ToLower().Contains(searchValue.ToLower()) ||
                          m.UploadDate.ToLower().Contains(searchValue.ToLower()) ||
                          m.DateofRegistration.ToLower().Contains(searchValue.ToLower()) ||
                          m.SketchNumber.ToLower().Contains(searchValue.ToLower())
                         //m.WhetherDocumentRegistered.ToLower().Contains(searchValue.ToLower())
                         );

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(OtherDepartmentImportDetail => new
                {
                    SerialNo = OtherDepartmentImportDetail.SerialNumber,
                    OfficeName = OtherDepartmentImportDetail.OfficeName,
                    FinalRegistrationNumber = OtherDepartmentImportDetail.FinalRegistrationNumber,
                    PropertyID = OtherDepartmentImportDetail.PropertyID,
                    ImportedXML = OtherDepartmentImportDetail.ImportedXML,
                    ExportedXML = OtherDepartmentImportDetail.ExportedXML,
                    ReferenceNumber = OtherDepartmentImportDetail.ReferenceNumber,
                    UploadDate = OtherDepartmentImportDetail.UploadDate,
                    DateofRegistration = OtherDepartmentImportDetail.DateofRegistration,
                    SketchNumber = OtherDepartmentImportDetail.SketchNumber,
                    //Added by mayank on 02/09/2021 for Kaveri-FRUITS Integration
                    ArticleNameE = OtherDepartmentImportDetail.ArticleNameE,
                    ActionDate = OtherDepartmentImportDetail.ActionDate,
                    BtnViewSummary = OtherDepartmentImportDetail.BtnViewSummary,
                    //Added by mayank on 05/01/2022
                    KaigrRegInsertDate = OtherDepartmentImportDetail.KaigrRegInsertDate,
                    //End of mayank Comment on 05/01/2022
                    //WhetherDocumentRegistered = OtherDepartmentImportDetail.WhetherDocumentRegistered
                });

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";

                // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 31-12-2020
                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + INTDistrictId + "','" + INTSroId + "','" + FromDate + "','" + FromDate + "','" + STRReportNameID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + INTDistrictId + "','" + INTSroId + "','" + FromDate + "','" + ToDate + "','" + STRReportNameID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 31-12-2020

                //var JsonData = "";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
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
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        //commented on 22-11-2019
                        //status = "1",
                        recordsFiltered = totalCount,
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
                return Json(new { serverError = true, errorMessage = "Error occured while getting Other Department Import Detail." }, JsonRequestBehavior.AllowGet);
            }
        }


        #region Excel

        [EventAuditLogFilter(Description = "Export Other Department Import To Excel")]
        public ActionResult ExportOtherDepartmentImportToExcel(string FromDate, string ToDate, string SroID, string DistrictID, string ReportName)
        {
            try
            {
                caller = new ServiceCaller("OtherDepartmentImportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                // changed by Shubham bhagat on 14-12-2019 on 3:48 pm 
                //string fileName = string.Format("OtherDepartmentImport.xlsx");
                string fileName = ReportName.ToUpper() + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);

                OtherDepartmentImportREQModel model = new OtherDepartmentImportREQModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SroID),
                    DistrictID = Convert.ToInt32(DistrictID),
                    startLen = 0,
                    totalNum = 10,
                    ReportName = ReportName
                };

                OtherDepartmentImportWrapper ResModel = new OtherDepartmentImportWrapper();
                //caller = new ServiceCaller("AnywhereECLogAPIController");
                //caller.HttpClient.Timeout = objTimeSpan;
                int totalCount = caller.PostCall<OtherDepartmentImportREQModel, int>("OtherDepartmentImportCount", model);
                model.totalNum = totalCount;
                //COMMENTED by shubham on 22-11-2019
                //model.IsExcel = true;
                ResModel = caller.PostCall<OtherDepartmentImportREQModel, OtherDepartmentImportWrapper>("OtherDepartmentImportDetails", model, out errorMessage);
                if (ResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Other Department Import Details", URLToRedirect = "/Home/HomePage" });
                    //return Json(new { success = false, errorMessage = "Error Occured While Getting Document Scan And Delivery Details" }, JsonRequestBehavior.AllowGet);
                }
                if (ResModel.OtherDepartmentImportDetailList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Other Department Import Details", URLToRedirect = "/Home/HomePage" });
                    //return Json(new { success = false, errorMessage = "Error Occured While Getting Document Scan And Delivery Details" }, JsonRequestBehavior.AllowGet);
                }
                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();
                //}
                string excelHeader = string.Format(ReportName.ToUpper() + " Import Between ({0} and {1})", FromDate, ToDate);

                ResModel.ReportName = ReportName;

                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader);
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

        private string CreateExcel(OtherDepartmentImportWrapper ResModel, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Other Department Import");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + ResModel.SelectedDRO;
                    workSheet.Cells[3, 1].Value = "SRO : " + ResModel.SelectedSRO;
                    //workSheet.Cells[4, 1].Value = "Log Type : " + SelectedLogType;

                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (ResModel.OtherDepartmentImportDetailList.Count());

                    workSheet.Cells[1, 1, 1, 7].Merge = true;
                    workSheet.Cells[2, 1, 2, 7].Merge = true;
                    workSheet.Cells[3, 1, 3, 7].Merge = true;
                    workSheet.Cells[4, 1, 4, 7].Merge = true;
                    workSheet.Cells[5, 1, 5, 7].Merge = true;
                    //workSheet.Cells[6, 1, 6, 13].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    if (ResModel.ReportName.Equals("mojini"))
                    {
                        workSheet.Column(1).Width = 20;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 30;
                        workSheet.Column(4).Width = 40;
                        workSheet.Column(5).Width = 50;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(7).Width = 40;
                        //workSheet.Column(8).Width = 30;
						//added by mayank on 02/09/2021 for FRUITS
                    }
                    else if (ResModel.ReportName.Equals("fruits"))
                    {
                        workSheet.Column(1).Width = 20;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 30;
                        workSheet.Column(4).Width = 40;
                        workSheet.Column(5).Width = 50;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(7).Width = 40;
                        workSheet.Column(8).Width = 40;
                        workSheet.Column(9).Width = 40;
                        workSheet.Cells[1, 1, 1, 9].Merge = true;
                        workSheet.Cells[2, 1, 2, 9].Merge = true;
                        workSheet.Cells[3, 1, 3, 9].Merge = true;
                        workSheet.Cells[4, 1, 4, 9].Merge = true;
                        workSheet.Cells[5, 1, 5, 9].Merge = true;
                    }
                    else
                    {
                        workSheet.Column(1).Width = 20;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 30;
                        workSheet.Column(4).Width = 40;
                        workSheet.Column(5).Width = 50;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(7).Width = 40;
                    }


                    ////workSheet.Column(10).Width = 30;
                    ////workSheet.Column(11).Width = 30;
                    ////workSheet.Column(12).Width = 30;
                    ////workSheet.Column(13).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    // //workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.WrapText = true;

                    if (ResModel.ReportName.Equals("mojini"))
                    {
                        workSheet.Cells[7, 1].Value = "S No.";
                        workSheet.Cells[7, 2].Value = "Office Name";
                        workSheet.Cells[7, 3].Value = "Final Registration Number";
                        workSheet.Cells[7, 4].Value = "Sketch Number";
                        workSheet.Cells[7, 5].Value = "Reference Number";
                        workSheet.Cells[7, 6].Value = "Upload Date";
                        //workSheet.Cells[7, 7].Value = "Whether Document Registered";
                        workSheet.Cells[7, 7].Value = "Registration Date";
						//added by mayank on 02/09/2021 for FRUITS
                    }
                    else if (ResModel.ReportName.Equals("fruits"))
                    {
                        workSheet.Cells[7, 1].Value = "S No.";
                        workSheet.Cells[7, 2].Value = "Office Name";
                        workSheet.Cells[7, 3].Value = "Final Registration Number";
                        workSheet.Cells[7, 4].Value = "Article Name";
                        workSheet.Cells[7, 5].Value = "Action";
                        workSheet.Cells[7, 6].Value = "Reference Number";
                        workSheet.Cells[7, 7].Value = "Upload Date";
                        //workSheet.Cells[7, 7].Value = "Whether Document Registered";
                        workSheet.Cells[7, 8].Value = "Registration Date";
                        workSheet.Cells[7, 9].Value = "Data Received(SRO) Date";
                    }
                    else
                    {
                        workSheet.Cells[7, 1].Value = "S No.";
                        workSheet.Cells[7, 2].Value = "Office Name";
                        workSheet.Cells[7, 3].Value = "Final Registration Number";
                        workSheet.Cells[7, 4].Value = "Property Identification Number";
                        workSheet.Cells[7, 5].Value = "Reference Number";
                        workSheet.Cells[7, 6].Value = "Upload Date";
                        workSheet.Cells[7, 7].Value = "Registration Date";
                    }

                    ////workSheet.Cells[7, 10].Value = "Date of Upload";
                    ////workSheet.Cells[7, 11].Value = "Archived in CD";
                    ////workSheet.Cells[7, 12].Value = "Document Delivery Date";
                    ////workSheet.Cells[7, 13].Value = "Date of Registration";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    ////workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";


                    foreach (var items in ResModel.OtherDepartmentImportDetailList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        //added by mayank on 02/09/2021 for FRUITS
						workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";
                        ////workSheet.Cells[rowIndex, 10].Style.Font.Name = "KNB-TTUmaEN";
                        ////workSheet.Cells[rowIndex, 11].Style.Font.Name = "KNB-TTUmaEN";
                        ////workSheet.Cells[rowIndex, 12].Style.Font.Name = "KNB-TTUmaEN";
                        ////workSheet.Cells[rowIndex, 13].Style.Font.Name = "KNB-TTUmaEN";

                        if (ResModel.ReportName.Equals("mojini"))
                        {
                            workSheet.Cells[rowIndex, 1].Value = items.SerialNumber;
                            workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                            workSheet.Cells[rowIndex, 3].Value = items.FinalRegistrationNumber;
                            workSheet.Cells[rowIndex, 4].Value = items.SketchNumber;
                            workSheet.Cells[rowIndex, 5].Value = items.ReferenceNumber;
                            workSheet.Cells[rowIndex, 6].Value = items.UploadDate;
                            //workSheet.Cells[rowIndex, 7].Value = items.WhetherDocumentRegistered;
                            workSheet.Cells[rowIndex, 7].Value = items.DateofRegistration;
                       //added by mayank on 02/09/2021 for FRUITS
					    }
                        else if (ResModel.ReportName.Equals("fruits"))
                        {
                            workSheet.Cells[rowIndex, 1].Value = items.SerialNumber;
                            workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                            workSheet.Cells[rowIndex, 3].Value = items.FinalRegistrationNumber;
                            workSheet.Cells[rowIndex, 4].Value = items.ArticleNameE;
                            workSheet.Cells[rowIndex, 5].Value = items.PropertyID;
                            workSheet.Cells[rowIndex, 6].Value = items.SketchNumber;
                            workSheet.Cells[rowIndex, 7].Value = items.UploadDate;
                            workSheet.Cells[rowIndex, 8].Value = items.DateofRegistration;
                            workSheet.Cells[rowIndex, 9].Value = items.KaigrRegInsertDate;
                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 1].Value = items.SerialNumber;
                            workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                            workSheet.Cells[rowIndex, 3].Value = items.FinalRegistrationNumber;
                            workSheet.Cells[rowIndex, 4].Value = items.PropertyID;
                            workSheet.Cells[rowIndex, 5].Value = items.ReferenceNumber;
                            workSheet.Cells[rowIndex, 6].Value = items.UploadDate;
                            workSheet.Cells[rowIndex, 7].Value = items.DateofRegistration;
                        }
                        //workSheet.Cells[rowIndex, 10].Value = items.DateofUpload;
                        //workSheet.Cells[rowIndex, 11].Value = items.ArchivedinCD;
                        //workSheet.Cells[rowIndex, 12].Value = items.DocumentDeliveryDate;
                        //workSheet.Cells[rowIndex, 13].Value = items.DateofRegistration;

                        workSheet.Cells[rowIndex, 1].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                     //added by mayank on 02/09/2021 for FRUITS
					    if (ResModel.ReportName.Equals("fruits"))
                        {
                            workSheet.Cells[rowIndex, 8].Style.WrapText = true;
                            workSheet.Cells[rowIndex, 9].Style.WrapText = true;
                        }
                        //workSheet.Cells[rowIndex, 8].Style.WrapText = true;
                        //workSheet.Cells[rowIndex, 9].Style.WrapText = true;
                        ////workSheet.Cells[rowIndex, 10].Style.WrapText = true;
                        ////workSheet.Cells[rowIndex, 11].Style.WrapText = true;
                        ////workSheet.Cells[rowIndex, 12].Style.WrapText = true;
                        ////workSheet.Cells[rowIndex, 13].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        ////workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        ////Function that passes the current row and adds the column details 
                        ////AddSubRowsForCurrentRow(out row,out workSheet);
                    }

                    if (ResModel.ReportName.Equals("mojini"))
                    {
                        using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 7])
                       //added by mayank on 02/09/2021 for FRUITS
					    {

                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                    }
                    else if (ResModel.ReportName.Equals("fruits"))
                    {
                        using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 9])
                        {

                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                    }
                    else
                    {
                        using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 7])
                        {

                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
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


        public ActionResult ExportToXML(String LogId, String SROCode, String ReportName, string XMLType)
        {
            try
            {
                caller = new ServiceCaller("OtherDepartmentImportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                OtherDepartmentImportREQModel InputModel = new OtherDepartmentImportREQModel();
                InputModel.XMLLogID = Convert.ToInt32(LogId);
                InputModel.XMLSROCODE = Convert.ToInt32(SROCode);
                InputModel.ReportName = ReportName;
                InputModel.XMLType = XMLType;

                XMLResModel XMLResModel = new XMLResModel();
                ////InputModel.GSCNo = GSCNo;
                ////InputModel.SROCode = SROCode;
                ////InputModel.SId = SId;
                XMLResModel = caller.PostCall<OtherDepartmentImportREQModel, XMLResModel>("GetXMLContent", InputModel);
                byte[] XMLByteArray = Encoding.Unicode.GetBytes(CommonFunctions.PrettyXml(XMLResModel.XMLString));
                return File(XMLByteArray, System.Net.Mime.MediaTypeNames.Application.Octet, ReportName.ToUpper() + "_" + XMLType + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xml");
                //return File(XMLByteArray, System.Net.Mime.MediaTypeNames.Application.Octet, "OtherDepartmentImport" + "_" + SROName + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xml");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading XML for Other Department Import Report", URLToRedirect = "/MISReports/OtherDepartmentImport/OtherDepartmentImportView" });
            }
        }


        //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
        [HttpGet]
        public ActionResult FormIIIDownloadFun(String LogId, String SROCode, String ReportName, string XMLType)
        {
            try
            {
                OtherDepartmentImportREQModel InputModel = new OtherDepartmentImportREQModel();
                InputModel.XMLLogID = Convert.ToInt32(LogId);
                InputModel.XMLSROCODE = Convert.ToInt32(SROCode);
                InputModel.ReportName = ReportName;
                InputModel.XMLType = XMLType; ;
                caller = new ServiceCaller("OtherDepartmentImportAPIController");
                XMLResModel XMLResModel = new XMLResModel();
                XMLResModel = caller.PostCall<OtherDepartmentImportREQModel, XMLResModel>("FormIIIDownloadFun", InputModel);
                return File(XMLResModel.PDFbyte, System.Net.Mime.MediaTypeNames.Application.Octet, ReportName.ToUpper() + XMLType + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
        [HttpGet]
        public ActionResult ViewTransXMLFun(String LogId, String SROCode, String ReportName, string XMLType)
        {
            try
            {
                OtherDepartmentImportREQModel InputModel = new OtherDepartmentImportREQModel();
                InputModel.XMLLogID = Convert.ToInt32(LogId);
                InputModel.XMLSROCODE = Convert.ToInt32(SROCode);
                InputModel.ReportName = ReportName;
                InputModel.XMLType = XMLType; ;
                caller = new ServiceCaller("OtherDepartmentImportAPIController");
                XMLResModel XMLResModel = new XMLResModel();
                XMLResModel = caller.PostCall<OtherDepartmentImportREQModel, XMLResModel>("ViewTransXMLFun", InputModel);
                //TransactionDetails transactionDetails = FromXmlString(XMLResModel.XMLString);


                return View("FRUITSSummaryView", XMLResModel);
                //return Json(new { serverError = false, success = true,HtmlString=XMLResModel.HTMLString, errorMessage = string.Empty},JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }


        }

        
    }
}