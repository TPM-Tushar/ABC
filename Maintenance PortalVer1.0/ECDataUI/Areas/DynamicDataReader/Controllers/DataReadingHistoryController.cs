using CustomModels.Models.DynamicDataReader;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Configuration;

namespace ECDataUI.Areas.DynamicDataReader.Controllers
{
    [KaveriAuthorizationAttribute]
    public class DataReadingHistoryController : Controller
    {
        #region PROPERTIES
        private ServiceCaller caller = new ServiceCaller("DataReadingHistoryAPIController");
        #endregion

        /// <summary>
        /// DataReadingHistoryReportView
        /// </summary>
        /// <returns>view</returns>
        [HttpGet]
        [MenuHighlight]
        [EventAuditLogFilter(Description = "Data Reading History Report View")]
        public ActionResult DataReadingHistoryReportView()
        {
            try
            {
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.RemittanceXMLLog;
                int OfficeID = KaveriSession.Current.OfficeID;
                DataReadingHistoryModel model = new DataReadingHistoryModel();
                List<SelectListItem> databaseList = new List<SelectListItem>();
                databaseList.Add(new SelectListItem { Text = "Select", Value = "0" });
                databaseList.Add(new SelectListItem { Text = "ECDATA", Value = "ECDATA" });
                databaseList.Add(new SelectListItem { Text = "KAIGR_SEARCHDB", Value = "KAIGR_SEARCHDB" });
                databaseList.Add(new SelectListItem { Text = "PEN_DOCS", Value = "PEN_DOCS" });
                databaseList.Add(new SelectListItem { Text = "ECDATA_DOCS", Value = "ECDATA_DOCS" });

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                model.DatabaseList = databaseList;

                //model = caller.GetCall<DiagnosticDataForRegistrationModel>("", new { OfficeID = OfficeID });

                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// GetDataReadingHistoryReport
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Get Detail to fill in datatable</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Data Reading History Report")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetDataReadingHistoryReport(FormCollection formCollection)
        {
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        
                string FromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SelectedDB = formCollection["SelectedDB"];
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

                DataReadingHistoryModel reqModel = new DataReadingHistoryModel();
                DataReadingHistoryResModel dataReadingHistoryResModel = new DataReadingHistoryResModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = FromDate;
                reqModel.ToDate = ToDate;
                reqModel.DatabaseName = SelectedDB;
                reqModel.CurrentRoleID = KaveriSession.Current.RoleID;


                //reqModel.SearchValue = searchValue;
                //reqModel.IsExcel = false;
                dataReadingHistoryResModel = caller.PostCall<DataReadingHistoryModel, DataReadingHistoryResModel>("GetDataReadingHistoryReport", reqModel, out errorMessage);
                if (dataReadingHistoryResModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Query Analyser Report Details." });
                }
                IEnumerable<DataReadingHistoryDetailModel> result = dataReadingHistoryResModel.dataReadingHistoryDetailModels;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Query Analyser Report Details." });
                }
                int totalCount = dataReadingHistoryResModel.dataReadingHistoryDetailModels.Count;
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
                         m.DBName.ToLower().Contains(searchValue.ToLower()) || m.Date.ToString().ToLower().Contains(searchValue.ToLower()) || m.NoOfRows.ToString().ToLower().Contains(searchValue.ToLower()) || m.Purpose.ToLower().Contains(searchValue.ToLower())

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
                    QueryID = QueryExecutionStatusReportDetailModel.QueryID,
                    Purpose = QueryExecutionStatusReportDetailModel.Purpose,
                    DatabaseName = QueryExecutionStatusReportDetailModel.DBName,
                    Date = QueryExecutionStatusReportDetailModel.Date,
                    LoginName = QueryExecutionStatusReportDetailModel.LoginName,
                    DBUserName = QueryExecutionStatusReportDetailModel.DBUserName,
                    QueryResult = QueryExecutionStatusReportDetailModel.QueryResultButtons,
                    QuerySqlText = QueryExecutionStatusReportDetailModel.QueryText,
                    //NoOfRows = QueryExecutionStatusReportDetailModel.NoOfRows,

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
                        recordsTotal = dataReadingHistoryResModel.TotalRecords,
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
                        recordsTotal = dataReadingHistoryResModel.TotalRecords,
                        status = "1",
                        recordsFiltered = dataReadingHistoryResModel.TotalRecords,
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
                return Json(new { serverError = true, errorMessage = "Error occured while getting Data Reading History Report Details." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GetSSRSReportData
        /// </summary>
        /// <returns>view</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Query Analyser SSRS Report Data")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetSSRSReportData(FormCollection formCollection)
        {
            try
            {

                DataReadingHistoryDetailModel model = new DataReadingHistoryDetailModel();
                DataReadingHistoryDetailModel resModel = new DataReadingHistoryDetailModel();
                string QueryID = formCollection["QueryId"];
               
                model.QueryID = Convert.ToInt32(QueryID);
                string errorMessage = string.Empty;
                resModel = caller.PostCall<DataReadingHistoryDetailModel, DataReadingHistoryDetailModel>("GetDetailByQueryId", model, out errorMessage);
                if(resModel != null)
                {
                    model.QueryText = resModel.QueryText;
                    model.DBName = resModel.DBName;
                    model.UserID = ConfigurationManager.AppSettings["UserIDForSSRSReport"];
                    model.Password = ConfigurationManager.AppSettings["PasswordForSSRSReport"];
                }

                
              
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        #region Excel
        /// <summary>
        /// ExportDataReadingHistoryToExcel
        /// </summary>
        /// <param name="SelectedDB"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        [EventAuditLogFilter(Description = "Export Data Reading Histroy Report To Excel")]
        public ActionResult ExportDataReadingHistoryToExcel(string SelectedDB, string FromDate, string ToDate)
        {
            try
            {
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("QueryExecutionStatusReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);

                DataReadingHistoryModel model = new DataReadingHistoryModel
                {
                    DatabaseName = SelectedDB,
                    FromDate = frmDate.ToString(),
                    ToDate = toDate.ToString(),
                    IsExcel = true
                    
                };


                DataReadingHistoryResModel ResModel = new DataReadingHistoryResModel();

                ResModel = caller.PostCall<DataReadingHistoryModel, DataReadingHistoryResModel>("GetDataReadingHistoryReport", model);
                if (ResModel.dataReadingHistoryDetailModels == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Getting Data Reading History Report Details..." }, JsonRequestBehavior.AllowGet);
                }


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("Data Reading History Report ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, SelectedDB);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "DataReadingHistoryReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="SelectedDB"></param>
        /// <returns>string</returns>
        private string CreateExcel(DataReadingHistoryResModel ResModel, string fileName, string excelHeader, string SelectedDB)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {               

                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Data Reading History Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Database : " + SelectedDB;
                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (ResModel.dataReadingHistoryDetailModels.Count());
                    workSheet.Cells[1, 1, 1, 7].Merge = true;
                    workSheet.Cells[2, 1, 2, 7].Merge = true;
                    workSheet.Cells[3, 1, 3, 7].Merge = true;
                    workSheet.Cells[4, 1, 4, 7].Merge = true;
                    workSheet.Cells[5, 1, 5, 7].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 10;
                    workSheet.Column(2).Width = 25;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    //for (int i = 8; i <= 32; i++)
                    //{
                    //    workSheet.Column(i).Width = 22;
                    //    workSheet.Cells[7, i].Style.WrapText = true;
                    //}
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    //workSheet.Row(7).Style.Font.Bold = true;
                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[6, 1].Value = "Sr No";
                    workSheet.Cells[6, 2].Value = "Query ID";
                    workSheet.Cells[6, 3].Value = "Purpose";
                    workSheet.Cells[6, 4].Value = "Database Name";
                    workSheet.Cells[6, 5].Value = "Execution Date";
                    workSheet.Cells[6, 6].Value = "Login Name";
                    workSheet.Cells[6, 7].Value = "DB User Name";
                   


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.dataReadingHistoryDetailModels)
                    {
                        //workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        for (int i = 1; i <= 7; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }

                        //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.QueryID;
                        workSheet.Cells[rowIndex, 3].Value = items.Purpose;
                        workSheet.Cells[rowIndex, 4].Value = items.DBName;
                        workSheet.Cells[rowIndex, 5].Value = items.Date;
                        workSheet.Cells[rowIndex, 6].Value = items.LoginName;
                        workSheet.Cells[rowIndex, 7].Value = items.DBUserName;
                        //workSheet.Cells[rowIndex, 8].Value = items.CountExecutions;
                        

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

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 7])
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
                    using (ExcelRange Rng = workSheet.Cells[6, 1, 7, 7])
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
    }
}