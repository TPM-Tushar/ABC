#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   OfficeWiseDiagnosticStatusController.cs
    * Author Name       :   Pankaj Sakhare 
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Remittance module.
*/
#endregion


using CustomModels.Models.Remittance.OfficeWiseDiagnosticStatus;
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
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]
    public class OfficeWiseDiagnosticStatusController : Controller
    {
        #region PROPERTIES
        private ServiceCaller caller = new ServiceCaller("OfficeWiseDiagnosticStatusAPIController");
        #endregion

        /// <summary>
        /// Office Wise Diagnostic Status View
        /// </summary>
        /// <returns>view</returns>
        [HttpGet]
        [MenuHighlight]
        [EventAuditLogFilter(Description = "Office Wise Status View")]
        public ActionResult OfficeWiseDiagnosticStatusView()
        {
            try
            {
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.RemittanceXMLLog;
                int OfficeID = KaveriSession.Current.OfficeID;
                OfficeWiseDiagnosticStatusModel model = new OfficeWiseDiagnosticStatusModel();
                model = caller.GetCall<OfficeWiseDiagnosticStatusModel>("OfficeWiseDiagnosticStatusView", new { OfficeID = OfficeID });

                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Get Office List
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns>Office List</returns>
        [HttpGet]
        public ActionResult GetOfficeList(String OfficeType)
        {
            #region Server Side Validation
            if (String.IsNullOrEmpty(OfficeType))
            {
                return Json(new { errorMessage = "Please select office type." }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            OfficeWiseDiagnosticStatusModel model = new OfficeWiseDiagnosticStatusModel();
            model = caller.GetCall<OfficeWiseDiagnosticStatusModel>("GetOfficeList", new { OfficeType });

            return Json(new { OfficeList = model.OfficeTypeList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetQueryExecutionStatusReport
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>detail report</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Office Wise Diagnostic Status Detail")]
        //[ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetOfficeWiseDiagnosticStatusDetail(FormCollection formCollection)
        {
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        

                string Date = formCollection["Date"];

                string SelectedStatus = formCollection["SelectedStatus"];
                //string Office = formCollection["OfficeName"];
                //string SelectedOfficeType = formCollection["SelectedOfficeType"];


                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                DateTime toDate;
                bool boolDate = false;

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });


                if (string.IsNullOrEmpty(Date))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }


                boolDate = DateTime.TryParse(DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);

                if (!boolDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid Date"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }


                OfficeWiseDiagnosticStatusModel reqModel = new OfficeWiseDiagnosticStatusModel();
                OfficeWiseDiagnosticStatusListModel resModel = new OfficeWiseDiagnosticStatusListModel();
                //QueryExecutionStatusReportResModel resModel = new QueryExecutionStatusReportResModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                //reqModel.FromDate = FromDate;
                //reqModel.ToDate = ToDate;
                reqModel.Date = Date;
                reqModel.Status = Int32.Parse(SelectedStatus);
                //reqModel.OfficeTypeID = Int32.Parse(Office);
                //reqModel.ActionId = Int32.Parse(SelectedAction);
                //if (SelectedOfficeType.ToLower().Equals("dro"))
                //{
                //    reqModel.IsDRO = true;
                //}

                //reqModel.SearchValue = searchValue;
                //reqModel.IsExcel = false;
                resModel = caller.PostCall<OfficeWiseDiagnosticStatusModel, OfficeWiseDiagnosticStatusListModel>("GetOfficeWiseDiagnosticStatusDetail", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Office Wise Diagnostic Status Details." });
                }
                IEnumerable<OfficeWiseDiagnosticStatusDetailModel> result = resModel.OfficeWiseDiagnosticStatusList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Office Wise Diagnostic Status Details." });
                }
                int totalCount = resModel.OfficeWiseDiagnosticStatusList.Count;
                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
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
                        result = result.Where(m => /*m.SrNo.ToString().ToLower().Contains(searchValue.ToLower()) ||*/
                         m.OfficeName.ToLower().Contains(searchValue.ToLower()) || m.DiagnosticDate.ToLower().Contains(searchValue.ToLower())

                        );
                        totalCount = result.Count();
                    }
                }
                //  Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }

                var gridData = result.Select(QueryExecutionStatusReportDetailModel => new
                {
                    SrNo = QueryExecutionStatusReportDetailModel.SrNo,
                    OfficeName = QueryExecutionStatusReportDetailModel.OfficeName,
                    DiagnosticDate = QueryExecutionStatusReportDetailModel.DiagnosticDate,
                    AllActionStatus = QueryExecutionStatusReportDetailModel.AllActionCellHtml,
                    DataFileSize = QueryExecutionStatusReportDetailModel.DataFileSize == 0 ? (Object)@"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>" : QueryExecutionStatusReportDetailModel.DataFileSize,
                    //LogFileSize = QueryExecutionStatusReportDetailModel.LogFileSize == 0 ? (Object)@"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>" : QueryExecutionStatusReportDetailModel.LogFileSize,
                    LogFileSize = string.IsNullOrEmpty(QueryExecutionStatusReportDetailModel.LogFileSize) ? (Object)@"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>" : QueryExecutionStatusReportDetailModel.LogFileSize,
                    //DBDiskSpace = QueryExecutionStatusReportDetailModel.DBDiskSpace == 0 ? (Object)@"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>" : QueryExecutionStatusReportDetailModel.DBDiskSpace,
                    DBDiskSpace = QueryExecutionStatusReportDetailModel.DBDiskSpace == string.Empty ? (Object)@"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>" : QueryExecutionStatusReportDetailModel.DBDiskSpace,
                    //SecureCodeHTML = QueryExecutionStatusReportDetailModel.SecureCodeHtml,
                    
                    TimeZoneStatus = QueryExecutionStatusReportDetailModel.TimeZoneCCellHtml,
                    DBCCStatus = QueryExecutionStatusReportDetailModel.DBCCCCellHtml,
                    //   DBCCErrorDesc = QueryExecutionStatusReportDetailModel.DBCCErrorDesc,
                    //   DBCCOutput = QueryExecutionStatusReportDetailModel.DBCCOutput,

                    ConstraintIntegrityStatus = QueryExecutionStatusReportDetailModel.ConstraintIntegrityCellHtml,
                    //  ConstraintIntegrityErrorDesc = QueryExecutionStatusReportDetailModel.ConstraintIntegrityErrorDesc,
                    //  ConstraintIntegrityOutput = QueryExecutionStatusReportDetailModel.ConstraintIntegrityOutput,

                    AuditEventStatus = QueryExecutionStatusReportDetailModel.AuditEventCellHtml,
                    //  AuditEventErrorDesc = QueryExecutionStatusReportDetailModel.AuditEventErrorDesc,
                    // AuditEventOutput = QueryExecutionStatusReportDetailModel.AuditEventOutput,

                    Optimizer1Status = QueryExecutionStatusReportDetailModel.Optimizer1CellHtml,
                    // Optimizer1ErrorDesc = QueryExecutionStatusReportDetailModel.Optimizer1ErrorDesc,
                    //Optimizer1Output = QueryExecutionStatusReportDetailModel.Optimizer1Output,

                    Optimizer2Status = QueryExecutionStatusReportDetailModel.Optimizer2CellHtml,
                    //Optimizer2ErrorDesc=QueryExecutionStatusReportDetailModel.Optimizer2ErrorDesc,
                    //Optimizer2Output=QueryExecutionStatusReportDetailModel.Optimizer2Output,

                    LastFullBackupStatus = QueryExecutionStatusReportDetailModel.LastFullBackupCellHtml,
                    //LastFullBackupErrorDesc=QueryExecutionStatusReportDetailModel.LastFullBackupErrorDesc,
                    //LastFullBackupOutput=QueryExecutionStatusReportDetailModel.LastFullBackupOutput,

                    LastDiffBackupStatus = QueryExecutionStatusReportDetailModel.LastDiffBackupCCellHtml,
                    //LastDiffBackupErrorDesc=QueryExecutionStatusReportDetailModel.LastDiffBackupErrorDesc,
                    //LastDiffBackupOutput=QueryExecutionStatusReportDetailModel.LastDiffBackupOutput,

                });

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        tilesData = resModel.TilesData
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(), //gridData.ToArray(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = resModel.TotalRecords,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        tilesData = resModel.TilesData

                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Office Wise Diagnostic Status Details." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GetActionDetail
        /// </summary>
        /// <param name="ActionId"></param>
        /// <param name="DetailId"></param>
        /// <param name="MasterId"></param>
        /// <returns>ErrorMessage or Output</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Action Detail")]
        //[ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetActionDetail(string ActionId, string DetailId, string MasterId)
        {
            try
            {
                if (CommonFunctions.ValidateId(ActionId) && CommonFunctions.ValidateId(DetailId) && CommonFunctions.ValidateId(MasterId))
                {
                    DiagnosticActionDetail reqModel = new DiagnosticActionDetail();
                    DiagnosticActionDetail resModel = new DiagnosticActionDetail();

                    reqModel.ActionId = Convert.ToInt32(ActionId);
                    reqModel.DetailId = Convert.ToInt32(DetailId);
                    reqModel.MasterId = Convert.ToInt32(MasterId);

                    resModel = caller.PostCall<DiagnosticActionDetail, DiagnosticActionDetail>("GetActionDetail", reqModel);

                    if (resModel == null)
                    {
                        return Json(new { serverError = true, errorMessage = "Error occured while getting Action Details." }, JsonRequestBehavior.AllowGet);
                    }


                    var JsonData = Json(new
                    {
                        data = resModel.ActionDetail,

                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    return JsonData;
                }
                else
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request ID tempered.", URLToRedirect = "/Home/HomePage" });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Action Details." }, JsonRequestBehavior.AllowGet);
            }

        }


        #region Excel
        [EventAuditLogFilter(Description = "Export Office Wise Diagnostic Status To Excel")]
        public ActionResult ExportOfficeWiseDiagnosticStatusToExcel(string Status, string txtDate)
        {
            try
            {
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("OfficeWiseDiagnosticStatus" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime date;
                DateTime.TryParse(DateTime.ParseExact(txtDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out date);

                int status = Convert.ToInt32(Status);

                OfficeWiseDiagnosticStatusModel model = new OfficeWiseDiagnosticStatusModel
                {
                    Status = status,
                    Date = txtDate,
                };

                OfficeWiseDiagnosticStatusListModel ResModel = new OfficeWiseDiagnosticStatusListModel();


                ResModel = caller.PostCall<OfficeWiseDiagnosticStatusModel, OfficeWiseDiagnosticStatusListModel>("ExportOfficeWiseDiagnosticStatusToExcel", model);
                if (ResModel.OfficeWiseDiagnosticStatusList == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Getting Office Wise Diagnostic Status Details..." }, JsonRequestBehavior.AllowGet);
                }


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("Office Wise Diagnostic Status ({0})", date);
                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, Status);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "OfficeWiseDiagnosticStatus" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create Excel
        /// </summary>
        /// <param name="ResModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SelectedReplica"></param>
        /// <param name="SelectedDB"></param>
        /// <returns>string</returns>
        private string CreateExcel(OfficeWiseDiagnosticStatusListModel ResModel, string fileName, string excelHeader, string SelectedStatus)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                string statusstring = string.Empty;
                if (SelectedStatus.Equals("0"))
                {
                    statusstring = "All Offices";
                }
                else if (SelectedStatus.Equals("1"))
                {
                    statusstring = "All Checks Successful";
                }
                else if (SelectedStatus.Equals("2"))
                {
                    statusstring = "Issues Found";
                }
                else if (SelectedStatus.Equals("3"))
                {
                    statusstring = "Not Available";
                }
                else if (SelectedStatus.Equals("4"))
                {
                    statusstring = "Available";
                }

                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Office Wise Diagnostic Status");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    //workSheet.Cells[2, 1].Value = "Office Type : " + OfficeType;
                    //workSheet.Cells[3, 1].Value = "Office Name : " + OfficeName;
                    workSheet.Cells[4, 1].Value = "Status : " + statusstring;
                    workSheet.Cells[5, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[6, 1].Value = "Total Records : " + (ResModel.OfficeWiseDiagnosticStatusList.Count());
                    workSheet.Cells[1, 1, 1, 16].Merge = true;
                    workSheet.Cells[2, 1, 2, 16].Merge = true;
                    workSheet.Cells[3, 1, 3, 16].Merge = true;
                    workSheet.Cells[4, 1, 4, 16].Merge = true;
                    workSheet.Cells[5, 1, 5, 16].Merge = true;
                    workSheet.Cells[6, 1, 6, 16].Merge = true;
                    workSheet.Cells[7, 1, 7, 16].Merge = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 10;
                    workSheet.Column(2).Width = 25;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    for (int i = 8; i <= 16; i++)
                    {
                        workSheet.Column(i).Width = 30;
                        workSheet.Cells[7, i].Style.WrapText = true;
                    }
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
                    workSheet.Cells[8, 1].Value = "Sr No";
                    workSheet.Cells[8, 2].Value = "Office Name";
                    workSheet.Cells[8, 3].Value = "Execution Date";
                    workSheet.Cells[8, 4].Value = "Data File Size (MB)";
                    workSheet.Cells[8, 5].Value = "Log File Size (MB)";
                    workSheet.Cells[8, 6].Value = "DB Disk Space (GB)";
                    workSheet.Cells[8, 7].Value = "Secure Code Check";
                    workSheet.Cells[8, 8].Value = "All Action Status";
                    workSheet.Cells[8, 9].Value = "Time Zone Check";
                    workSheet.Cells[8, 10].Value = "Database Consistency Check";
                    workSheet.Cells[8, 11].Value = "Constraint Integrity Check";
                    workSheet.Cells[8, 12].Value = "Audit Event Check";
                    workSheet.Cells[8, 13].Value = "Optimizer 1 Check";
                    workSheet.Cells[8, 14].Value = "Optimizer 2 Check";
                    workSheet.Cells[8, 15].Value = "Last Full Backup Verification";
                    workSheet.Cells[8, 16].Value = "Last Differential Backup Verification";

                    //workSheet.Cells[7, 12].Value = "Max Cpu Time Seconds";
                    //workSheet.Cells[7, 13].Value = "Avg Cpu Time Seconds";
                    //workSheet.Cells[7, 14].Value = "Last Cpu Time Seconds";
                    //workSheet.Cells[7, 15].Value = "Max Physical IOReads";
                    //workSheet.Cells[7, 16].Value = "Avg Physical IOReads";




                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.OfficeWiseDiagnosticStatusList)
                    {
                        //workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        for (int i = 1; i <= 16; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }

                        //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                        workSheet.Cells[rowIndex, 3].Value = items.DiagnosticDate;
                        workSheet.Cells[rowIndex, 4].Value = items.DataFileSize;
                        workSheet.Cells[rowIndex, 5].Value = items.LogFileSize;
                        workSheet.Cells[rowIndex, 6].Value = items.DBDiskSpace;
                        workSheet.Cells[rowIndex, 7].Value = items.SecureCodeHtml;
                        workSheet.Cells[rowIndex, 8].Value = items.AllActionCellHtml;
                        workSheet.Cells[rowIndex, 9].Value = items.TimeZoneCCellHtml;
                        workSheet.Cells[rowIndex, 10].Value = items.DBCCCCellHtml;
                        workSheet.Cells[rowIndex, 11].Value = items.ConstraintIntegrityCellHtml;
                        workSheet.Cells[rowIndex, 12].Value = items.AuditEventCellHtml;
                        workSheet.Cells[rowIndex, 13].Value = items.Optimizer1CellHtml;
                        workSheet.Cells[rowIndex, 14].Value = items.Optimizer2CellHtml;
                        workSheet.Cells[rowIndex, 15].Value = items.LastFullBackupCellHtml;
                        workSheet.Cells[rowIndex, 16].Value = items.LastDiffBackupCCellHtml;


                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 9].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 10].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 11].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 12].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 13].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 14].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 15].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 16].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                    }

                    //workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(rowIndex).Style.Font.Bold = true;
                    //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    using (ExcelRange Rng = workSheet.Cells[8, 1, (rowIndex - 1), 16])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 1])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    }
                    using (ExcelRange Rng = workSheet.Cells[8, 1, 8, 16])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
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

        /// <summary>
        /// GetFileInfo
        /// </summary>
        /// <param name="tempExcelFilePath"></param>
        /// <returns>FileInfo</returns>
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }

        #endregion

        [HttpPost]
        [EventAuditLogFilter(Description = "Get Office Wise Diagnostic Status Detail by Action Id")]
        //[ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetDiagnosticStatusDetailByActionType(FormCollection formCollection)
        {
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        

                string Date = formCollection["Date"];

                string SelectedStatus = formCollection["SelectedStatus"];
                string ActionID = formCollection["ActionId"];
               

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                DateTime toDate;
                bool boolDate = false;

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });


                if (string.IsNullOrEmpty(Date))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }


                boolDate = DateTime.TryParse(DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);

                if (!boolDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid Date"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }


                OfficeWiseDiagnosticStatusModel reqModel = new OfficeWiseDiagnosticStatusModel();
                OfficeWiseDiagnosticStatusListModel resModel = new OfficeWiseDiagnosticStatusListModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.ActionIDForDetailTable = Convert.ToInt32(ActionID);
                reqModel.Date = Date;
                reqModel.Status = Int32.Parse(SelectedStatus);

                //reqModel.IsExcel = false;
                resModel = caller.PostCall<OfficeWiseDiagnosticStatusModel, OfficeWiseDiagnosticStatusListModel>("GetDiagnosticStatusDetailByActionType", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Office Wise Diagnostic Status Details." });
                }
                IEnumerable<OfficeWiseDiagnosticStatusDetailModel> result = resModel.OfficeWiseDiagnosticStatusList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Office Wise Diagnostic Status Details." });
                }
                int totalCount = resModel.OfficeWiseDiagnosticStatusList.Count;

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
                        result = result.Where(m => /*m.SrNo.ToString().ToLower().Contains(searchValue.ToLower()) ||*/
                         m.OfficeName.ToLower().Contains(searchValue.ToLower()) || m.DiagnosticDate.ToLower().Contains(searchValue.ToLower())

                        );
                        totalCount = result.Count();
                    }
                }
                //  Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }

                var gridData = result.Select(QueryExecutionStatusReportDetailModel => new
                {
                    SrNo = QueryExecutionStatusReportDetailModel.SrNo,
                    OfficeName = QueryExecutionStatusReportDetailModel.OfficeName,
                    DiagnosticDate = QueryExecutionStatusReportDetailModel.DiagnosticDate,
                    AllActionStatus = QueryExecutionStatusReportDetailModel.AllActionCellHtml,
                    DataFileSize = QueryExecutionStatusReportDetailModel.DataFileSize == 0 ? (Object)@"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>" : QueryExecutionStatusReportDetailModel.DataFileSize,
                    //LogFileSize = QueryExecutionStatusReportDetailModel.LogFileSize == 0 ? (Object)@"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>" : QueryExecutionStatusReportDetailModel.LogFileSize,
                    LogFileSize = string.IsNullOrEmpty(QueryExecutionStatusReportDetailModel.LogFileSize) ? (Object)@"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>" : QueryExecutionStatusReportDetailModel.LogFileSize,
                    //DBDiskSpace = QueryExecutionStatusReportDetailModel.DBDiskSpace == 0 ? (Object)@"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>" : QueryExecutionStatusReportDetailModel.DBDiskSpace,
                    DBDiskSpace = string.IsNullOrEmpty(QueryExecutionStatusReportDetailModel.DBDiskSpace) ? (Object)@"<i class='fa fa-minus fa-lg' style='color: lightslategray;' aria-hidden='true'></i>" : QueryExecutionStatusReportDetailModel.DBDiskSpace,
                    //SecureCodeHTML = QueryExecutionStatusReportDetailModel.SecureCodeHtml,
                  
                    TimeZoneStatus = QueryExecutionStatusReportDetailModel.TimeZoneCCellHtml,
                    DBCCStatus = QueryExecutionStatusReportDetailModel.DBCCCCellHtml,
                   
                    ConstraintIntegrityStatus = QueryExecutionStatusReportDetailModel.ConstraintIntegrityCellHtml,

                    AuditEventStatus = QueryExecutionStatusReportDetailModel.AuditEventCellHtml,

                    Optimizer1Status = QueryExecutionStatusReportDetailModel.Optimizer1CellHtml,

                    Optimizer2Status = QueryExecutionStatusReportDetailModel.Optimizer2CellHtml,

                    LastFullBackupStatus = QueryExecutionStatusReportDetailModel.LastFullBackupCellHtml,

                    LastDiffBackupStatus = QueryExecutionStatusReportDetailModel.LastDiffBackupCCellHtml,


                });

       
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        tilesData = resModel.TilesData
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(), //gridData.ToArray(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = resModel.TotalRecords,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        tilesData = resModel.TilesData

                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Office Wise Diagnostic Status Details." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}