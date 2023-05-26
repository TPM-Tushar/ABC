#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   QueryExecutionStatusReportController.cs
    * Author Name       :   Pankaj Sakhare	
    * Creation Date     :   13-10-2020
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Remittance module.
*/
#endregion

using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Models.Remittance.QueryExecutionStatusReport;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq.Dynamic;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]
    public class QueryExecutionStatusReportController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        /// <summary>
        /// QueryExecutionStatusReportView
        /// </summary>
        /// <returns>Query Execution Status Report View</returns>
        [MenuHighlight]
        [EventAuditLogFilter(Description = "Query Execution Status Report View")]
        public ActionResult QueryExecutionStatusReportView()
        {
            try
            {
               // KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DailyRevenueArticleWise;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("QueryExecutionStatusReportAPIController");
                QueryExecutionStatusReportModel result = caller.GetCall<QueryExecutionStatusReportModel>("QueryExecutionStatusReportView", new { OfficeID = OfficeID });
                return View(result);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Query Execution Status Report View", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// GetQueryExecutionStatusReport
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Get Detail to fill in datatable</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Query Execution Status Report")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetQueryExecutionStatusReport(FormCollection formCollection)
        {
            //if (!ModelState.IsValid)
            //{
            //    string errorMsg = ModelState.FormatErrorMessage();
            //    return Json(new { success = false, message = errorMsg });
            //}

            caller = new ServiceCaller("QueryExecutionStatusReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        
                string FromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string TopRows = formCollection["TopRows"];
                string SelectedDB = formCollection["SelectedDB"];
                string SelectedReplica = formCollection["ReplicaType"];
                int topRows = Convert.ToInt32(TopRows);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
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

                //top rows field validation
                System.Text.RegularExpressions.Regex regxForTopRow = new Regex(@"^[0-9][0-9]{0,9}$");
                Match match = regxForTopRow.Match((string)TopRows);
                if (!match.Success)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Invalid top rows value"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (string.IsNullOrEmpty(SelectedDB))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Database is required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (string.IsNullOrEmpty(SelectedReplica))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Replica is required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
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
                //database name validation
                if (SelectedDB != "ECDATA" && SelectedDB != "KAIGR_SEARCHDB" && SelectedDB != "PEN_DOCS" && SelectedDB != "ECDATA_DOCS")
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Select valid database"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (SelectedReplica != "PR" && SelectedReplica != "SR" && SelectedReplica != "CR")
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Select valid Replica"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);
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
                //else if ((toDate - frmDate).TotalDays > 180)//six months validation
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "Data of six months can be seen at a time"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;

                //}
                QueryExecutionStatusReportModel reqModel = new QueryExecutionStatusReportModel();
                QueryExecutionStatusReportResModel queryExecutionStatusReportResModel = new QueryExecutionStatusReportResModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = FromDate;
                reqModel.ToDate = ToDate;
                reqModel.TopRows = topRows;
                reqModel.ReplicaType = SelectedReplica;
                reqModel.DatabaseName = SelectedDB;

                //reqModel.SearchValue = searchValue;
                //reqModel.IsExcel = false;
                queryExecutionStatusReportResModel = caller.PostCall<QueryExecutionStatusReportModel, QueryExecutionStatusReportResModel>("GetQueryExecutionStatusReport", reqModel, out errorMessage);
                if(queryExecutionStatusReportResModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Query Execution Status Report Details." });
                }
                IEnumerable<QueryExecutionStatusReportDetailModel> result = queryExecutionStatusReportResModel.queryExecutionStatusReportDetailList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Query Execution Status Report Details." });
                }
                int totalCount = queryExecutionStatusReportResModel.queryExecutionStatusReportDetailList.Count;
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
                         //m.DatabaseName.ToLower().Contains(searchValue.ToLower()) ||
                         m.QuerySqlText.ToLower().Contains(searchValue.ToLower()) ||
                         //m.ObjectName.ToLower().Contains(searchValue.ToLower()) ||
                         //m.ReplicaName.ToLower().Contains(searchValue.ToLower()) ||
                         m.QueryPlanXML.ToLower().Contains(searchValue.ToLower()) ||
                         m.LastExecutionTime.ToLower().Contains(searchValue.ToLower()) ||
                         m.CountExecutions.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         m.LastExecutionTime.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         
                         m.last_elapsed_time.ToString().ToLower().Contains(searchValue.ToLower()) || 
                         m.last_logical_reads.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         m.last_logical_writes.ToString().ToLower().Contains(searchValue.ToLower()) || 
                         m.last_physical_reads.ToString().ToLower().Contains(searchValue.ToLower()) || 
                         m.last_worker_time.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        
                         m.max_elapsed_time.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         m.max_logical_reads.ToString().ToLower().Contains(searchValue.ToLower()) || 
                         m.max_logical_writes.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         m.max_physical_reads.ToString().ToLower().Contains(searchValue.ToLower()) || 
                         m.max_worker_time.ToString().ToLower().Contains(searchValue.ToLower()) || 

                         m.total_elapsed_time.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         m.total_logical_reads.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         m.total_logical_writes.ToString().ToLower().Contains(searchValue.ToLower()) || 
                         m.total_physical_reads.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         m.total_worker_time.ToString().ToLower().Contains(searchValue.ToLower()) 

                         //m.AvgTempdbSpaceUsed.ToString().ToLower().Contains(searchValue.ToLower()) || 
                         //m.LastTempdbSpaceUsed.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         //m.MaxQueryMaxUsedMemory8kPages.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         //m.AvgQueryMaxUsedMemory8kPages.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         //m.LastQueryMaxUsedMemory8kPages.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         //m.MaxRowCount.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         //m.AvgRowCount.ToString().ToLower().Contains(searchValue.ToLower()) ||
                         //m.LastRowCount.ToString().ToLower().Contains(searchValue.ToLower())

                        //result = result.Where(m=>m.SrNo.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        // m.DatabaseName.ToLower().Contains(searchValue.ToLower()) || m.QuerySqlText.ToLower().Contains(searchValue.ToLower()) || m.ReplicaName.ToLower().Contains(searchValue.ToLower()) ||
                        // m.QueryPlanXML.ToLower().Contains(searchValue.ToLower()) || m.LastExecutionTime.ToLower().Contains(searchValue.ToLower()) || m.CountExecutions.ToLower().Contains(searchValue.ToLower()) || 
                        // m.MaxDurationSeconds.ToLower().Contains(searchValue.ToLower()) || m.AvgDurationSeconds.ToLower().Contains(searchValue.ToLower()) || m.LastDurationSeconds.ToLower().Contains(searchValue.ToLower()) ||
                        // m.MaxCpuTimeSeconds.ToLower().Contains(searchValue.ToLower()) || m.AvgCpuTimeSeconds.ToLower().Contains(searchValue.ToLower()) || m.LastCpuTimeSeconds.ToLower().Contains(searchValue.ToLower()) ||
                        // m.MaxPhysicalIOReads.ToLower().Contains(searchValue.ToLower()) || m.AvgPhysicalIOReads.ToLower().Contains(searchValue.ToLower()) || m.LastPhysicalIOReads.ToLower().Contains(searchValue.ToLower()) ||
                        // m.MaxLogicalIOReads.ToLower().Contains(searchValue.ToLower()) || m.AvgLogicalIOReads.ToLower().Contains(searchValue.ToLower()) || m.LastLogicalIOReads.ToLower().Contains(searchValue.ToLower()) ||
                        // m.MaxLogicalIOWrites.ToLower().Contains(searchValue.ToLower()) || m.AvgLogicalIOWrites.ToLower().Contains(searchValue.ToLower()) || m.LastLogicalIOWrites.ToLower().Contains(searchValue.ToLower()) ||
                        // m.MaxTempdbSpaceUsed.ToLower().Contains(searchValue.ToLower()) || m.AvgTempdbSpaceUsed.ToLower().Contains(searchValue.ToLower()) || m.LastTempdbSpaceUsed.ToLower().Contains(searchValue.ToLower()) ||
                        // m.MaxQueryMaxUsedMemory8kPages.ToLower().Contains(searchValue.ToLower()) || m.AvgQueryMaxUsedMemory8kPages.ToLower().Contains(searchValue.ToLower()) || m.LastQueryMaxUsedMemory8kPages.ToLower().Contains(searchValue.ToLower()) ||
                        // m.MaxRowCount.ToLower().Contains(searchValue.ToLower()) || m.AvgRowCount.ToLower().Contains(searchValue.ToLower()) || m.LastRowCount.ToLower().Contains(searchValue.ToLower()) 

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
                    //DatabaseName = QueryExecutionStatusReportDetailModel.DatabaseName,
                    QuerySqlText = QueryExecutionStatusReportDetailModel.QuerySqlText,
                    Query = QueryExecutionStatusReportDetailModel.Query,
                    //ObjectName = QueryExecutionStatusReportDetailModel.ObjectName,
                    //ReplicaName = QueryExecutionStatusReportDetailModel.ReplicaName,
                    QueryPlanXML = QueryExecutionStatusReportDetailModel.QueryPlanXML,
                    QueryPlanXMLButton = QueryExecutionStatusReportDetailModel.QueryPlanXMLButton,
                    LastExecutionTime = QueryExecutionStatusReportDetailModel.LastExecutionTime,
                    CountExecutions = QueryExecutionStatusReportDetailModel.CountExecutions,

                    total_worker_time = QueryExecutionStatusReportDetailModel.total_worker_time,
                    max_worker_time = QueryExecutionStatusReportDetailModel.max_worker_time,
                    last_worker_time = QueryExecutionStatusReportDetailModel.last_worker_time,

                    total_elapsed_time = QueryExecutionStatusReportDetailModel.total_elapsed_time,
                    max_elapsed_time = QueryExecutionStatusReportDetailModel.max_elapsed_time,
                    last_elapsed_time = QueryExecutionStatusReportDetailModel.last_elapsed_time,

                    total_physical_reads = QueryExecutionStatusReportDetailModel.total_physical_reads,
                    max_physical_reads = QueryExecutionStatusReportDetailModel.max_physical_reads,
                    last_physical_reads = QueryExecutionStatusReportDetailModel.last_physical_reads,

                    total_logical_reads = QueryExecutionStatusReportDetailModel.total_logical_reads,
                    max_logical_reads = QueryExecutionStatusReportDetailModel.max_logical_reads,
                    last_logical_reads = QueryExecutionStatusReportDetailModel.last_logical_reads,

                    total_logical_writes = QueryExecutionStatusReportDetailModel.total_logical_writes,
                    max_logical_writes = QueryExecutionStatusReportDetailModel.max_logical_writes,
                    last_logical_writes = QueryExecutionStatusReportDetailModel.last_logical_writes


       

                    //Amount = ECDailyReceiptDetailsModel.Amount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                });

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = queryExecutionStatusReportResModel.TotalRecords,
                        status = "1",
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
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(), //gridData.ToArray(),
                        recordsTotal = queryExecutionStatusReportResModel.TotalRecords,
                        status = "1",
                        recordsFiltered = queryExecutionStatusReportResModel.TotalRecords,
                        //PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Query Execution Status Report Details." }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Excel
        /// <summary>
        /// ExportQueryStatusReportToExcel
        /// </summary>
        /// <param name="SelectedReplica"></param>
        /// <param name="SelectedDB"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="TopRows"></param>
        /// <returns></returns>
        [EventAuditLogFilter(Description = "Export Query Execution status Report To Excel")]
        public ActionResult ExportQueryStatusReportToExcel(string SelectedReplica, string SelectedDB, string FromDate, string ToDate, string TopRows)
        {
            try
            {
                caller = new ServiceCaller("QueryExecutionStatusReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("QueryExecutionStatusReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);
                int topRow = Convert.ToInt32(TopRows);

                QueryExecutionStatusReportModel model = new QueryExecutionStatusReportModel
                {
                    FromDate = FromDate,
                    ToDate = ToDate,
                    startLen = 0,
                    totalNum = 10,
                    IsExcel = true,
                    TopRows = topRow,
                    ReplicaType = SelectedReplica,
                    DatabaseName = SelectedDB
                };


                QueryExecutionStatusReportResModel ResModel = new QueryExecutionStatusReportResModel();

                model.IsExcel = true;

                ResModel = caller.PostCall<QueryExecutionStatusReportModel, QueryExecutionStatusReportResModel>("GetQueryExecutionStatusReport", model);
                if (ResModel.queryExecutionStatusReportDetailList == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Getting Query Execution Status Report Details..." }, JsonRequestBehavior.AllowGet);
                }


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("Query Execution Status Report ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, SelectedReplica, SelectedDB);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "QueryExecutionStatusReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        private string CreateExcel(QueryExecutionStatusReportResModel ResModel, string fileName, string excelHeader, string SelectedReplica, string SelectedDB)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                switch (SelectedReplica)
                {
                    case "PR":
                        SelectedReplica = "Primary";
                        break;
                    case "SR":
                        SelectedReplica = "Secondary";
                        break;
                    case "CR":
                        SelectedReplica = "Common";
                        break;
                }

                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Query Execution Status Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Replica : " + SelectedReplica;
                    workSheet.Cells[3, 1].Value = "Database : " + SelectedDB;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (ResModel.queryExecutionStatusReportDetailList.Count());
                    workSheet.Cells[1, 1, 1, 32].Merge = true;
                    workSheet.Cells[2, 1, 2, 32].Merge = true;
                    workSheet.Cells[3, 1, 3, 32].Merge = true;
                    workSheet.Cells[4, 1, 4, 32].Merge = true;
                    workSheet.Cells[5, 1, 5, 32].Merge = true;
                    workSheet.Cells[6, 1, 6, 32].Merge = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 10;
                    workSheet.Column(2).Width = 25;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    for (int i = 8; i <= 32; i++)
                    {
                        workSheet.Column(i).Width = 22;
                        workSheet.Cells[7, i].Style.WrapText = true;
                    }
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[7, 1].Value = "Sr No";
                    workSheet.Cells[7, 2].Value = "Datbase Name";
                    workSheet.Cells[7, 3].Value = "Sql Query";
                    workSheet.Cells[7, 4].Value = "Object Name";
                    workSheet.Cells[7, 5].Value = "Replica Name";
                    workSheet.Cells[7, 6].Value = "Query Plan XML";
                    workSheet.Cells[7, 7].Value = "Last Execution Time";
                    workSheet.Cells[7, 8].Value = "Count Executions";
                    workSheet.Cells[7, 9].Value = "Max Duration Seconds";
                    workSheet.Cells[7, 10].Value = "Avg Duration Seconds";
                    workSheet.Cells[7, 11].Value = "Last Duration Seconds";

                    workSheet.Cells[7, 12].Value = "Max Cpu Time Seconds";
                    workSheet.Cells[7, 13].Value = "Avg Cpu Time Seconds";
                    workSheet.Cells[7, 14].Value = "Last Cpu Time Seconds";
                    workSheet.Cells[7, 15].Value = "Max Physical IOReads";
                    workSheet.Cells[7, 16].Value = "Avg Physical IOReads";
                    workSheet.Cells[7, 17].Value = "Last Physical IOReads";
                    workSheet.Cells[7, 18].Value = "Max Logical IOReads";
                    workSheet.Cells[7, 19].Value = "Avg Logical IOReads";
                    workSheet.Cells[7, 20].Value = "Last Logical IOReads";
                    workSheet.Cells[7, 21].Value = "Max Logical IOWrites";

                    workSheet.Cells[7, 22].Value = "Avg Logical IOWrites";
                    workSheet.Cells[7, 23].Value = "Last Logical IOWrites";
                    workSheet.Cells[7, 24].Value = "Max Tempdb Space Used";
                    workSheet.Cells[7, 25].Value = "Avg Tempdb Space Used";
                    workSheet.Cells[7, 26].Value = "Last Tempdb Space Used";
                    workSheet.Cells[7, 27].Value = "Max Query Max Used Memory 8k Pages";
                    workSheet.Cells[7, 28].Value = "Avg Query Max Used Memory 8k Pages";
                    workSheet.Cells[7, 29].Value = "Last Query Max Used Memory 8k Pages";
                    workSheet.Cells[7, 30].Value = "Max Row Count";
                    workSheet.Cells[7, 31].Value = "Avg Row Count";
                    workSheet.Cells[7, 32].Value = "LastRowCount";


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.queryExecutionStatusReportDetailList)
                    {
                        //workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        for (int i = 1; i <= 32; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }

                        //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        //workSheet.Cells[rowIndex, 2].Value = items.DatabaseName;
                        workSheet.Cells[rowIndex, 2].Value = items.QuerySqlText;
                        //workSheet.Cells[rowIndex, 4].Value = items.ObjectName;
                        //workSheet.Cells[rowIndex, 5].Value = items.ReplicaName;
                        workSheet.Cells[rowIndex, 3].Value = items.QueryPlanXML;
                        workSheet.Cells[rowIndex, 4].Value = items.LastExecutionTime;
                        workSheet.Cells[rowIndex, 5].Value = items.CountExecutions;
                        //workSheet.Cells[rowIndex, 9].Value = items.MaxDurationSeconds;
                        //workSheet.Cells[rowIndex, 10].Value = items.AvgDurationSeconds;

                        workSheet.Cells[rowIndex, 6].Value = items.total_worker_time;
                        workSheet.Cells[rowIndex, 7].Value = items.max_worker_time;
                        workSheet.Cells[rowIndex, 8].Value = items.last_worker_time;
                        workSheet.Cells[rowIndex, 9].Value = items.total_elapsed_time;
                        workSheet.Cells[rowIndex, 10].Value = items.max_elapsed_time;
                        workSheet.Cells[rowIndex, 11].Value = items.last_elapsed_time;
                        workSheet.Cells[rowIndex, 12].Value = items.total_physical_reads;
                        workSheet.Cells[rowIndex, 13].Value = items.max_physical_reads;
                        workSheet.Cells[rowIndex, 14].Value = items.last_physical_reads;
                        workSheet.Cells[rowIndex, 15].Value = items.total_logical_reads;
                        workSheet.Cells[rowIndex, 16].Value = items.max_logical_reads;
                        workSheet.Cells[rowIndex, 17].Value = items.last_logical_reads;
                        workSheet.Cells[rowIndex, 18].Value = items.total_logical_writes;
                        workSheet.Cells[rowIndex, 19].Value = items.max_logical_writes;
                        workSheet.Cells[rowIndex, 20].Value = items.last_logical_writes;


                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                    }

                    //workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(rowIndex).Style.Font.Bold = true;
                    //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 20])
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
                    using (ExcelRange Rng = workSheet.Cells[7, 1, 7,20])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
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

        #region COMMENTED
        //[HttpPost]
        //[EventAuditLogFilter(Description = "Download query plan xml to sqlplan file")]
        //public  ActionResult DownloadQueryPlanXML(String Data)
        //{

        //    #region Log Server Exceptions
        //    CommonFunctions objCommon = new CommonFunctions();
        //    string fileName = string.Format("QueryPlanXML" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".sqlplan");
        //    if (string.IsNullOrEmpty(Data))
        //    {
        //        return Json(new { success = false, errorMessage = "No data found" }, JsonRequestBehavior.AllowGet);
        //    }
        //    string FilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //    //String filename = SroID + "_" + DateTime.Now.ToString().Replace(' ', '_').Replace(':', '_') + ".sql";
        //    //return File(data, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        //    ////
        //    FileInfo templateFile = GetFileInfo(FilePath);

        //    byte[] sqpPlan = System.IO.File.ReadAllBytes(Data);
        //    objCommon.DeleteFileFromTemporaryFolder(FilePath);
        //    return File(sqpPlan, System.Net.Mime.MediaTypeNames.Application.Octet, "QueryPlanXML" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".sqlplan");

        //    //directoryPath = System.IO.Path.Combine(directoryPath, DateTime.Now.Year.ToString(), DateTime.Now.Date.ToString("MMM"), DateTime.Now.Date.ToString("dd-MM-yyyy"));

        //    //string filePath = directoryPath + "/Log " + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
        //    //System.IO.DirectoryInfo info = System.IO.Directory.CreateDirectory(directoryPath);

        //    //using (System.IO.StreamWriter file = System.IO.File.AppendText(filePath))
        //    //{
        //    //    string format = "{0} : {1}";

        //    //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 100)));
        //    //    file.WriteLine(string.Format(format, "Timestamp: ", DateTime.Now.ToString("hh:mm:ss tt")));
        //    //    file.WriteLine(string.Format(format, "Message: ", exception.Message));
        //    //    file.WriteLine(string.Format(format, "Stack Trace: ", exception.StackTrace));

        //    //    Exception innerExp = GetInnerMostException(exception);
        //    //    if (innerExp != null)
        //    //    {
        //    //        file.WriteLine("--------Inner Exception--------");
        //    //        file.WriteLine();
        //    //        file.WriteLine(string.Format(format, "Message: ", innerExp.Message));
        //    //        file.WriteLine(string.Format(format, "Stack Trace: ", innerExp.StackTrace));
        //    //        file.WriteLine();
        //    //        innerExp = innerExp.InnerException;
        //    //    }

        //    //    file.Flush();
        //    //}

        //    #endregion


        //}
        #endregion
    }
}