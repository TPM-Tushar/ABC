#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   ECCCSearchStatisticsController.cs
    * Author Name       :   Mayank Wankhede
    * Creation Date     :   14-07-2020
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   EC/CC search statistics controller
*/
#endregion

using CustomModels.Models.MISReports.ECCCSearchStatistics;
using ECDataUI.Common;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ECDataUI.Filters;
using System.Drawing;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]
    public class ECCCSearchStatisticsController : Controller
    {
        ServiceCaller caller = null;
        // GET: MISReports/ECCCSearchStatistics
        public ActionResult Index()
        {
            return View();
        }

        //Added by mayank date 14-7-20
        //to get view page model populate and return to view page
        /// <summary>
        /// ECCCSearchStatisticsView
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [MenuHighlight]
        public ActionResult ECCCSearchStatisticsView()
        {
            try
            {
                ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel = new ECCCSearchStatisticsViewModel();
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("ECCCSearchStatisticsAPIController");
                eCCCSearchStatisticsViewModel = caller.GetCall<ECCCSearchStatisticsViewModel>("ECCCSearchStatisticsView", new { officeid = OfficeID });

                return View(eCCCSearchStatisticsViewModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        //Added by mayank date 14-7-20
        //to get sro list
        /// <summary>
        /// GetSroList
        /// </summary>
        /// <param name="DroCode"></param>
        /// <returns></returns>
        public ActionResult GetSroList(int DroCode)
        {
            try
            {
                ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel = new ECCCSearchStatisticsViewModel();
                eCCCSearchStatisticsViewModel.DROCode = DroCode;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("ECCCSearchStatisticsAPIController");
                eCCCSearchStatisticsViewModel = caller.PostCall<ECCCSearchStatisticsViewModel, ECCCSearchStatisticsViewModel>("GetSroList", eCCCSearchStatisticsViewModel);

                return Json(eCCCSearchStatisticsViewModel);
                //throw new Exception();
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        //Added by mayank date 14-7-20
        //to get Summary table data
        /// <summary>
        /// GetSummary
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult GetSummary(FormCollection formCollection)
        {
            try
            {
                ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel = new ECCCSearchStatisticsViewModel();
                ECCCSearchStatisticsResultModel eCCCSearchStatisticsResultModel = new ECCCSearchStatisticsResultModel();
                caller = new ServiceCaller("ECCCSearchStatisticsAPIController");
                if (CommonFunctions.ValidateId(formCollection["SroCode"]) &&
                    CommonFunctions.ValidateId(formCollection["DROCode"]) &&
                    CommonFunctions.ValidateId(formCollection["FinancialyearCode"]) &&
                    CommonFunctions.ValidateId(formCollection["MonthCode"]))
                {
                    eCCCSearchStatisticsViewModel.SROCode = Convert.ToInt32(formCollection["SROCode"]);
                    eCCCSearchStatisticsViewModel.DROCode = Convert.ToInt32(formCollection["DROCode"]);
                    eCCCSearchStatisticsViewModel.FinancialyearCode = Convert.ToInt32(formCollection["FinancialyearCode"]);
                    eCCCSearchStatisticsViewModel.MonthCode = Convert.ToInt32(formCollection["MonthCode"]);
                    int startLen = Convert.ToInt32(formCollection["start"]);
                    int totalNum = Convert.ToInt32(formCollection["length"]);
                    var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                    System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                    Match mtch = regx.Match((string)searchValue);
                    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                    eCCCSearchStatisticsResultModel = caller.PostCall<ECCCSearchStatisticsViewModel, ECCCSearchStatisticsResultModel>("GetSummaryApi", eCCCSearchStatisticsViewModel);
                    IEnumerable<ECCCSearchStatisticsSummaryModel> result = eCCCSearchStatisticsResultModel.SummaryList;


                    int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                    int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                    int skip = startLen;

                    if (result == null)
                    {
                        return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting EC Daily Receipt Details." });
                    }
                    int totalCount = eCCCSearchStatisticsResultModel.TotalSummaryRecords;
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
                            result = result.Where(m => m.MonthYear.ToString().ToLower().Contains(searchValue.ToLower()));
                            eCCCSearchStatisticsResultModel.SummaryList = result.ToList();
                            eCCCSearchStatisticsResultModel.TotalSummaryRecords = result.Count();
                        }
                    }
                    ////Sorting
                    //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    //{
                    //    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                    //    eCCCSearchStatisticsResultModel.SummaryList = result.ToList();
                    //}

                    var gridData = eCCCSearchStatisticsResultModel.SummaryList.Select(ECCCSearchStatisticsSummaryModel => new
                    {
                        SrNo = ECCCSearchStatisticsSummaryModel.SrNo,
                        MonthYear = ECCCSearchStatisticsSummaryModel.MonthYear,
                        TotalUserLogged = ECCCSearchStatisticsSummaryModel.TotalUserLogged,
                        TotalECSearched = ECCCSearchStatisticsSummaryModel.TotalECSearched,
                        TotalECSubmitted = ECCCSearchStatisticsSummaryModel.TotalECSubmitted,
                        TotalECSigned = ECCCSearchStatisticsSummaryModel.TotalECSigned,
                        TotalCCSearched = ECCCSearchStatisticsSummaryModel.TotalCCSearched,
                        TotalCCSubmitted = ECCCSearchStatisticsSummaryModel.TotalCCSubmitted,
                        TotalCCSigned = ECCCSearchStatisticsSummaryModel.TotalCCSigned,


                    });

                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        //data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        data = gridData.ToArray().ToList(),
                        recordsTotal = eCCCSearchStatisticsResultModel.TotalSummaryRecords,
                        status = "1",
                        DroName = eCCCSearchStatisticsResultModel.DroName,
                        SroName = eCCCSearchStatisticsResultModel.SroName,
                        FYName = eCCCSearchStatisticsResultModel.FinancialYearName,
                        recordsFiltered = eCCCSearchStatisticsResultModel.TotalSummaryRecords,
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    return JsonData;
                }
                else
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request ID tempered.", URLToRedirect = "/Home/HomePage" });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        //Added by mayank date 14-7-20
        //to get Detail table data
        /// <summary>
        /// GetDetails
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult GetDetails(FormCollection formCollection)
        {
            try
            {
                if (CommonFunctions.ValidateId(formCollection["SroCode"]) &&
                    CommonFunctions.ValidateId(formCollection["DROCode"]) &&
                    CommonFunctions.ValidateId(formCollection["FinancialyearCode"]) &&
                    CommonFunctions.ValidateId(formCollection["MonthCode"]))
                {
                    ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel = new ECCCSearchStatisticsViewModel();
                    ECCCSearchStatisticsResultModel eCCCSearchStatisticsResultModel = new ECCCSearchStatisticsResultModel();
                    caller = new ServiceCaller("ECCCSearchStatisticsAPIController");
                    eCCCSearchStatisticsViewModel.SROCode = Convert.ToInt32(formCollection["SROCode"]);
                    eCCCSearchStatisticsViewModel.DROCode = Convert.ToInt32(formCollection["DROCode"]);
                    eCCCSearchStatisticsViewModel.FinancialyearCode = Convert.ToInt32(formCollection["FinancialyearCode"]);
                    eCCCSearchStatisticsViewModel.MonthCode = Convert.ToInt32(formCollection["MonthCode"]);
                    eCCCSearchStatisticsViewModel.SearchBy = (SearchBy)Enum.Parse(typeof(SearchBy), Convert.ToString(formCollection["SearchByParameter"]));
                    int startLen = Convert.ToInt32(formCollection["start"]);
                    int totalNum = Convert.ToInt32(formCollection["length"]);
                    var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                    System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                    Match mtch = regx.Match((string)searchValue);
                    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                    eCCCSearchStatisticsResultModel = caller.PostCall<ECCCSearchStatisticsViewModel, ECCCSearchStatisticsResultModel>("GetDetailsApi", eCCCSearchStatisticsViewModel);
                    IEnumerable<ECCCSearchStatisticsDetailModel> result = eCCCSearchStatisticsResultModel.DetailsList;


                    int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                    int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                    int skip = startLen;

                    if (result == null)
                    {
                        return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting EC Daily Receipt Details." });
                    }
                    int totalCount = eCCCSearchStatisticsResultModel.TotalDetailRecords;
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
                            result = result.Where(m => m.MonthYear.ToString().ToLower().Contains(searchValue.ToLower()));
                            eCCCSearchStatisticsResultModel.DetailsList = result.ToList();
                            eCCCSearchStatisticsResultModel.TotalDetailRecords = result.Count();
                        }
                    }
                    ////Sorting
                    //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    //{
                    //    result = result.OrderBy(sortColumn + " " + sortColumnDir);

                    //    eCCCSearchStatisticsResultModel.SummaryList = result.ToList();
                    //}

                    var gridData = eCCCSearchStatisticsResultModel.DetailsList.Select(ECCCSearchStatisticsDetailModel => new
                    {
                        SrNo = ECCCSearchStatisticsDetailModel.SrNo,
                        MonthYear = ECCCSearchStatisticsDetailModel.MonthYear,
                        TotalUserLogged = ECCCSearchStatisticsDetailModel.TotalUserLogged,
                        TotalECSearched = ECCCSearchStatisticsDetailModel.TotalECSearched,
                        TotalECSubmitted = ECCCSearchStatisticsDetailModel.TotalECSubmitted,
                        TotalECSigned = ECCCSearchStatisticsDetailModel.TotalECSigned,
                        TotalCCSearched = ECCCSearchStatisticsDetailModel.TotalCCSearched,
                        TotalCCSubmitted = ECCCSearchStatisticsDetailModel.TotalCCSubmitted,
                        TotalCCSigned = ECCCSearchStatisticsDetailModel.TotalCCSigned,
                        //ADDED BY PANKAJ ON 11-06-2021
                        AnywhereTotalECSigned = ECCCSearchStatisticsDetailModel.AnywhereTotalECSigned,
                        LocalTotalECSigned = ECCCSearchStatisticsDetailModel.LocalTotalECSigned,
                        AnywhereTotalCCSigned = ECCCSearchStatisticsDetailModel.AnywhereTotalCCSigned,
                        LocalTotalCCSigned = ECCCSearchStatisticsDetailModel.LocalTotalCCSigned,


                    });

                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        //data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        data = gridData.ToArray().ToList(),
                        recordsTotal = eCCCSearchStatisticsResultModel.TotalDetailRecords,
                        status = "1",
                        DroName = eCCCSearchStatisticsResultModel.DroName,
                        SroName = eCCCSearchStatisticsResultModel.SroName,
                        FYName = eCCCSearchStatisticsResultModel.FinancialYearName,
                        MonthName = eCCCSearchStatisticsResultModel.MonthName,
                        TotalUserLogged = eCCCSearchStatisticsResultModel.DetailsList.Sum(m=>m.TotalUserLogged),
                        TotalECSearched = eCCCSearchStatisticsResultModel.DetailsList.Sum(m => m.TotalECSearched),
                        TotalECSubmitted = eCCCSearchStatisticsResultModel.DetailsList.Sum(m => m.TotalECSubmitted),
                        TotalECSigned = eCCCSearchStatisticsResultModel.DetailsList.Sum(m => m.TotalECSigned),
                        TotalCCSearched = eCCCSearchStatisticsResultModel.DetailsList.Sum(m => m.TotalCCSearched),
                        TotalCCSubmitted = eCCCSearchStatisticsResultModel.DetailsList.Sum(m => m.TotalCCSubmitted),
                        TotalCCSigned = eCCCSearchStatisticsResultModel.DetailsList.Sum(m => m.TotalCCSigned),
                        AnywhereTotalECSigned = eCCCSearchStatisticsResultModel.DetailsList.Sum(m => m.AnywhereTotalECSigned),
                        LocalTotalECSigned = eCCCSearchStatisticsResultModel.DetailsList.Sum(m => m.LocalTotalECSigned),
                        AnywhereTotalCCSigned = eCCCSearchStatisticsResultModel.DetailsList.Sum(m => m.AnywhereTotalCCSigned),
                        LocalTotalCCSigned = eCCCSearchStatisticsResultModel.DetailsList.Sum(m => m.LocalTotalCCSigned),
                        recordsFiltered = eCCCSearchStatisticsResultModel.TotalDetailRecords,
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    return JsonData;
                }
                else
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request ID tempered.", URLToRedirect = "/Home/HomePage" });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }


        //Added by mayank date 15-7-20
        //to get Excel file for summary and detail table
        /// <summary>
        /// ExportSummaryToExcel
        /// </summary>
        /// <param name="SROCode,DROCode,FinancialyearCode,MonthCode"></param>
        /// <returns></returns>
        public ActionResult ExportSummaryToExcel(int SROCode, int DROCode, int FinancialyearCode, int MonthCode,string SearchByParameter)
        {
            try
            {

                string fileName = string.Format("ECCCSummaryStatistics" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel = new ECCCSearchStatisticsViewModel();
                ECCCSearchStatisticsResultModel eCCCSearchStatisticsResultModel = new ECCCSearchStatisticsResultModel();
                caller = new ServiceCaller("ECCCSearchStatisticsAPIController");

                eCCCSearchStatisticsViewModel.SROCode = SROCode;
                eCCCSearchStatisticsViewModel.DROCode = DROCode;
                eCCCSearchStatisticsViewModel.FinancialyearCode = FinancialyearCode;
                eCCCSearchStatisticsViewModel.MonthCode = MonthCode;
                eCCCSearchStatisticsViewModel.SearchBy = (SearchBy)Enum.Parse(typeof(SearchBy), Convert.ToString(SearchByParameter));
                eCCCSearchStatisticsResultModel = caller.PostCall<ECCCSearchStatisticsViewModel, ECCCSearchStatisticsResultModel>("GetSummaryDetailsforExcelApi", eCCCSearchStatisticsViewModel);
                eCCCSearchStatisticsResultModel.SearchBy = eCCCSearchStatisticsViewModel.SearchBy;

                //if (eCCCSearchStatisticsResultModel.SummaryList == null)
                //{

                //    return Json(new { success = false, errorMessage = "Error Occured While Getting EC CC summary Details..." }, JsonRequestBehavior.AllowGet);

                //}



                string excelHeader = string.Format("EC/CC Summary and Detail Statistics");
                string createdExcelPath = CreateExcel(eCCCSearchStatisticsResultModel, fileName, excelHeader);

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "ECCCSumary" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        //Added by mayank date 15-7-20
        //to Create Excel file for summary and detail table
        /// <summary>
        /// CreateExcel
        /// </summary>
        /// <param name="ResModel,fileName,FinancialyearCode,excelHeader"></param>
        /// <returns></returns>
        private string CreateExcel(ECCCSearchStatisticsResultModel ResModel, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("EC/CC Summary Statistics");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    //workSheet.Cells[1, 1, 1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //workSheet.Cells[1, 1, 1, 9].Style.Fill.BackgroundColor.SetColor();

                    //workSheet.Cells[1,1,1,9].
                    //workSheet.Cells[1, 1]



                    workSheet.Cells[2, 1].Value = "District : " + ResModel.DroName;
                    workSheet.Cells[3, 1].Value = "SRO : " + ResModel.SroName;
                    workSheet.Cells[4, 1].Value = "Financial Year : " + ResModel.FinancialYearName;
                    workSheet.Cells[5, 1].Value = "Month : " + ResModel.MonthName;

                    workSheet.Cells[6, 1].Value = "Print Date Time : " + DateTime.Now;

                    //workSheet.Cells[6, 1].Value = "Total Records : " + (ResModel.SummaryList.Count());
                    workSheet.Cells[1, 1, 1, 13].Merge = true;
                    workSheet.Cells[2, 1, 2, 13].Merge = true;
                    workSheet.Cells[3, 1, 3, 13].Merge = true;
                    workSheet.Cells[4, 1, 4, 13].Merge = true;
                    workSheet.Cells[5, 1, 5, 13].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    //workSheet.Column(5).Width = 30;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 45;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;
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
                    workSheet.Row(9).Style.Font.Bold = true;
                    workSheet.Row(10).Style.Font.Bold = true;
                    workSheet.Row(11).Style.Font.Bold = true;
                    workSheet.Row(12).Style.Font.Bold = true;
                    workSheet.Row(13).Style.Font.Bold = true;
            
                    //workSheet.Cells[8, 1].Value = "EC/CC Summary Statistics";
                    //workSheet.Cells[8, 1, 8, 9].Merge = true;
                    //workSheet.Cells[8, 1].Style.Font.Bold = true;
                    //workSheet.Cells[8, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    ////Added by Madhusoodan on 10/08/2020
                    //workSheet.Cells[9, 4].Value = "EC Search Statistics";
                    //workSheet.Cells[9, 4, 9, 6].Merge = true;

                    //workSheet.Cells[9, 7].Value = "CC Search Statistics";
                    //workSheet.Cells[9, 7, 9, 9].Merge = true;

                    //workSheet.Cells[9, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Cells[9, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    //workSheet.Cells[9, 1, 10, 1].Merge = true;
                    //workSheet.Cells[9, 2, 10, 2].Merge = true;
                    //workSheet.Cells[9, 3, 10, 3].Merge = true;

                    //int rowIndex = 11;   //10
                    int rowIndex = 8;

                    //workSheet.Row(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Cells[9, 1].Value = "S.No";
                    ////workSheet.Cells[9, 2].Value = "Month/Year";

                    //if (ResModel.MonthName != "All")
                    //    workSheet.Cells[9, 2].Value = "Month";
                    //else
                    //    workSheet.Cells[9, 2].Value = "Year";

                    //workSheet.Cells[9, 3].Value = "Total User logged";

                    //workSheet.Cells[10, 4].Value = "Total EC Searched";

                    //workSheet.Cells[10, 5].Value = "Total EC Submitted";
                    //workSheet.Cells[10, 6].Value = "Total EC Signed";
                    //workSheet.Cells[10, 7].Value = "Total CC Searched";
                    //workSheet.Cells[10, 8].Value = "Total CC Submitted";
                    //workSheet.Cells[10, 9].Value = "Total CC Signed";
                    //workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(10).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[8, 8].Style.WrapText = true;

                    //foreach (var items in ResModel.SummaryList)
                    //{
                    //    workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";


                    //    //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                    //    workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                    //    workSheet.Cells[rowIndex, 2].Value = items.MonthYear;
                    //    workSheet.Cells[rowIndex, 3].Value = items.TotalUserLogged;
                    //    workSheet.Cells[rowIndex, 4].Value = items.TotalECSearched;

                    //    workSheet.Cells[rowIndex, 5].Value = items.TotalECSubmitted;
                    //    workSheet.Cells[rowIndex, 6].Value = items.TotalECSigned;

                    //    workSheet.Cells[rowIndex, 7].Value = items.TotalCCSearched;
                    //    workSheet.Cells[rowIndex, 8].Value = items.TotalCCSubmitted;
                    //    workSheet.Cells[rowIndex, 9].Value = items.TotalCCSigned;

                    //    workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                    //    workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //    workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    //    workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //    workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //    workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //    workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    //    rowIndex++;
                    //}
                    //Commented by mayank on 24-06-2021
                    //workSheet.Cells[rowIndex, 1].Value = "";
                    //workSheet.Cells[rowIndex, 2].Value = "";
                    //workSheet.Cells[rowIndex, 3].Value = "";
                    //workSheet.Cells[rowIndex, 4].Value = "";

                    //workSheet.Cells[rowIndex, 5].Value = "";
                    //workSheet.Cells[rowIndex, 6].Value = "";
                    //workSheet.Cells[rowIndex, 7].Value = "";
                    //workSheet.Cells[rowIndex, 8].Value = "Total";
                    //workSheet.Cells[rowIndex, 9].Value = ResModel.TotalSummaryRecords;

                    //Added by Madhusoodan on 10/08/2020 to align 'total' center
                    workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    //Commented by mayank on 24062021
                    //for (int i = 1; i <= 9; i++)
                    //    workSheet.Cells[rowIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(rowIndex).Style.Font.Bold = true;
                    //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    //commented by mayank on 24-06-2021
                    //using (ExcelRange Rng = workSheet.Cells[9, 1, (rowIndex), 9])
                    //{

                    //    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //}
                    //using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 9])
                    //{
                    //    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    //    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    //    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    //    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    //}

                    using (ExcelRange Rng = workSheet.Cells[9, 1, 9, 9])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    rowIndex = rowIndex + 4;    //4 rows gap for detail table
                    workSheet.Cells[rowIndex - 1, 1].Value = "EC/CC Detail Statistics";
                    workSheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Merge = true;
                    workSheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Style.Font.Bold = true;
                    workSheet.Cells[rowIndex - 1, 1].Style.Font.Bold = true;
                    workSheet.Cells[rowIndex - 1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    int newTableindex = rowIndex;

                    //Added by Madhusoodan on 10/08/2020
                    workSheet.Cells[rowIndex, 1].Value = "S.No";
                    if (ResModel.SearchBy==SearchBy.SearchDurationWise)
                    {
                        if (ResModel.MonthName == "All")
                            workSheet.Cells[rowIndex, 2].Value = "Month";
                        else
                            workSheet.Cells[rowIndex, 2].Value = "Date"; 
                    }
                    else
                    {
                        workSheet.Cells[rowIndex, 2].Value = "Office Name";

                    }

                    workSheet.Cells[rowIndex, 3].Value = "Total User logged";

                    workSheet.Cells[rowIndex, 4].Value = "Online EC Search ";
                    workSheet.Cells[rowIndex, 4, rowIndex, 6].Merge = true;

                    workSheet.Cells[rowIndex, 7].Value = "Online CC Search ";
                    workSheet.Cells[rowIndex, 7, rowIndex, 9].Merge = true;

                    workSheet.Cells[rowIndex, 10].Value = "AnyWhere EC Search";

                    workSheet.Cells[rowIndex, 11].Value = "Local EC Search";

                    workSheet.Cells[rowIndex, 12].Value = "AnyWhere CC Search";

                    workSheet.Cells[rowIndex, 13].Value = "Local CC Search";

                    workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[rowIndex, 1, rowIndex + 1, 1].Merge = true;
                    workSheet.Cells[rowIndex, 2, rowIndex + 1, 2].Merge = true;
                    workSheet.Cells[rowIndex, 3, rowIndex + 1, 3].Merge = true;

                    workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(rowIndex).Style.Font.Bold = true;
                    workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    rowIndex++;
                    ////

                    //workSheet.Cells[rowIndex, 2].Value = "Month/Year";

                    workSheet.Cells[rowIndex, 4].Value = "Total EC Searched";

                    workSheet.Cells[rowIndex, 5].Value = "Total EC Submitted";
                    workSheet.Cells[rowIndex, 6].Value = "Total EC Signed";
                    workSheet.Cells[rowIndex, 7].Value = "Total CC Searched";
                    workSheet.Cells[rowIndex, 8].Value = "Total CC Submitted";
                    workSheet.Cells[rowIndex, 9].Value = "Total CC Signed";
                    workSheet.Cells[rowIndex, 10].Value = "Total EC Signed";
                    workSheet.Cells[rowIndex, 11].Value = "Total EC Signed";
                    workSheet.Cells[rowIndex, 12].Value = "Total CC Signed";
                    workSheet.Cells[rowIndex, 13].Value = "Total CC Signed";
                    workSheet.Row(rowIndex).Style.Font.Bold = true;
                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    rowIndex++;
                    foreach (var items in ResModel.DetailsList)
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


                        //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.MonthYear;
                        workSheet.Cells[rowIndex, 3].Value = items.TotalUserLogged;
                        //if (ResModel.SearchBy == SearchBy.SearchOfficeWise)
                        //{
                        //    workSheet.Cells[rowIndex, 3].Value = "-";
                        //}
                        //else
                        //{
                        //    workSheet.Cells[rowIndex, 3].Value = items.TotalUserLogged;
                        //}

                        workSheet.Cells[rowIndex, 4].Value = items.TotalECSearched;

                        workSheet.Cells[rowIndex, 5].Value = items.TotalECSubmitted;
                        workSheet.Cells[rowIndex, 6].Value = items.TotalECSigned;

                        workSheet.Cells[rowIndex, 7].Value = items.TotalCCSearched;
                        workSheet.Cells[rowIndex, 8].Value = items.TotalCCSubmitted;
                        workSheet.Cells[rowIndex, 9].Value = items.TotalCCSigned;
                        workSheet.Cells[rowIndex, 10].Value = items.AnywhereTotalECSigned;
                        workSheet.Cells[rowIndex, 11].Value = items.LocalTotalECSigned;
                        workSheet.Cells[rowIndex, 12].Value = items.AnywhereTotalCCSigned;
                        workSheet.Cells[rowIndex, 13].Value = items.LocalTotalCCSigned;

                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                        workSheet.Cells[rowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                        rowIndex++;
                    }
                    workSheet.Cells[rowIndex, 1].Value = "";
                    workSheet.Cells[rowIndex, 2].Value = "Total";
                    workSheet.Cells[rowIndex, 3].Value = ResModel.DetailsList.Sum(m => m.TotalUserLogged);
                    workSheet.Cells[rowIndex, 4].Value = ResModel.DetailsList.Sum(m => m.TotalECSearched);

                    workSheet.Cells[rowIndex, 5].Value = ResModel.DetailsList.Sum(m => m.TotalECSubmitted);
                    workSheet.Cells[rowIndex, 6].Value = ResModel.DetailsList.Sum(m => m.TotalECSigned);
                    workSheet.Cells[rowIndex, 7].Value = ResModel.DetailsList.Sum(m => m.TotalCCSearched);
                    workSheet.Cells[rowIndex, 8].Value = ResModel.DetailsList.Sum(m => m.TotalCCSubmitted);
                    workSheet.Cells[rowIndex, 9].Value = ResModel.DetailsList.Sum(m => m.TotalCCSigned);
                    workSheet.Cells[rowIndex, 10].Value = ResModel.DetailsList.Sum(m => m.AnywhereTotalECSigned);
                    workSheet.Cells[rowIndex, 11].Value = ResModel.DetailsList.Sum(m => m.LocalTotalECSigned);
                    workSheet.Cells[rowIndex, 12].Value = ResModel.DetailsList.Sum(m => m.AnywhereTotalCCSigned);
                    workSheet.Cells[rowIndex, 13].Value = ResModel.DetailsList.Sum(m => m.LocalTotalCCSigned);

                    //Added by Madhusoodan on 10/08/2020 to align center
                    workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    for (int i = 1; i <= 13; i++)
                        workSheet.Cells[rowIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(rowIndex).Style.Font.Bold = true;
                    //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    using (ExcelRange Rng = workSheet.Cells[newTableindex, 1, (rowIndex), 13])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    //using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 1])
                    //{
                    //    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    //}
                    using (ExcelRange Rng = workSheet.Cells[newTableindex - 1, 1, newTableindex - 1, 13])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thick;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    }
                    //commented by mayank on 24-06-2021
                    //using (ExcelRange Rng = workSheet.Cells[8, 1, 8, 9])
                    //{
                    //    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    //    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    //    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    //    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    //}
                    using (ExcelRange Rng = workSheet.Cells[7, 1, 7, 9])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    package.SaveAs(templateFile);
                    //CreateExcelForDetails(package,"EC CC details",ResModel,templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelFilePath;
        }

        //Added by mayank date 15-7-20
        //to get file info
        /// <summary>
        /// GetFileInfo
        /// </summary>
        /// <param name="tempExcelFilePath"></param>
        /// <returns></returns>
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }

        #region Not in use
        //Added by mayank date 15-7-20
        //to create new excel sheet
        /// <summary>
        /// CreateExcelForDetails
        /// </summary>
        /// <param name="package,excelHeader,ResModel,templateFile"></param>
        /// <returns></returns>
        public void CreateExcelForDetails(ExcelPackage package, string excelHeader, ECCCSearchStatisticsResultModel ResModel, FileInfo templateFile)
        {
            #region Testcode
            var workbook = package.Workbook;
            var workSheet = package.Workbook.Worksheets.Add("EC CC Detail Statistics");
            workSheet.Cells.Style.Font.Size = 14;

            workSheet.Cells[1, 1].Value = excelHeader;


            workSheet.Cells[2, 1].Value = "Month : " + ResModel.MonthName;
            //workSheet.Cells[3, 1].Value = "SRO : " + ResModel.SroName;
            //workSheet.Cells[4, 1].Value = "Financial Year : " + ResModel.FinancialYearName;


            workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

            workSheet.Cells[4, 1].Value = "Total Records : " + (ResModel.DetailsList.Count());
            workSheet.Cells[1, 1, 1, 9].Merge = true;
            workSheet.Cells[2, 1, 2, 9].Merge = true;
            //workSheet.Cells[3, 1, 3, 9].Merge = true;
            //workSheet.Cells[4, 1, 4, 9].Merge = true;
            //workSheet.Cells[5, 1, 5, 9].Merge = true;

            workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            workSheet.Column(1).Width = 20;
            workSheet.Column(2).Width = 30;
            workSheet.Column(3).Width = 30;
            workSheet.Column(4).Width = 30;
            //workSheet.Column(5).Width = 30;
            workSheet.Column(5).Width = 40;
            workSheet.Column(6).Width = 30;
            workSheet.Column(7).Width = 30;
            workSheet.Column(8).Width = 45;
            workSheet.Column(9).Width = 30;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(2).Style.Font.Bold = true;
            workSheet.Row(3).Style.Font.Bold = true;
            workSheet.Row(4).Style.Font.Bold = true;
            workSheet.Row(5).Style.Font.Bold = true;
            workSheet.Row(6).Style.Font.Bold = true;
            workSheet.Row(7).Style.Font.Bold = true;
            int rowIndex = 8;
            workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            workSheet.Cells[7, 1].Value = "S.No";
            workSheet.Cells[7, 2].Value = "Month/Year";
            workSheet.Cells[7, 3].Value = "Total User logged";
            workSheet.Cells[7, 4].Value = "Total EC Searched";

            workSheet.Cells[7, 5].Value = "Total EC Submitted";
            workSheet.Cells[7, 6].Value = "Total EC Signed";
            workSheet.Cells[7, 7].Value = "Total CC Searched";
            workSheet.Cells[7, 8].Value = "Total CC Submitted";
            workSheet.Cells[7, 9].Value = "Total CC Signed";
            workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
            workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
            workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
            workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
            workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
            workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
            workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
            workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
            workSheet.Cells[7, 8].Style.WrapText = true;

            foreach (var items in ResModel.DetailsList)
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


                //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                workSheet.Cells[rowIndex, 2].Value = items.MonthYear;
                workSheet.Cells[rowIndex, 3].Value = items.TotalUserLogged;
                workSheet.Cells[rowIndex, 4].Value = items.TotalECSearched;


                workSheet.Cells[rowIndex, 5].Value = items.TotalECSigned;
                workSheet.Cells[rowIndex, 6].Value = items.TotalECSubmitted;
                workSheet.Cells[rowIndex, 7].Value = items.TotalCCSearched;
                workSheet.Cells[rowIndex, 8].Value = items.TotalCCSigned;
                workSheet.Cells[rowIndex, 9].Value = items.TotalCCSubmitted;
                workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                rowIndex++;
            }
            workSheet.Cells[rowIndex, 1].Value = "";
            workSheet.Cells[rowIndex, 2].Value = "";
            workSheet.Cells[rowIndex, 3].Value = "";
            workSheet.Cells[rowIndex, 4].Value = "";

            workSheet.Cells[rowIndex, 5].Value = "";
            workSheet.Cells[rowIndex, 6].Value = "";
            workSheet.Cells[rowIndex, 7].Value = "";
            workSheet.Cells[rowIndex, 8].Value = "Total";
            workSheet.Cells[rowIndex, 9].Value = ResModel.TotalDetailRecords;

            workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
            workSheet.Row(rowIndex).Style.Font.Bold = true;
            //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
            workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

            using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex), 9])
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
            using (ExcelRange Rng = workSheet.Cells[7, 1, 7, 9])
            {
                Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            }
            package.SaveAs(templateFile);
            #endregion
        }
        #endregion
    }
}