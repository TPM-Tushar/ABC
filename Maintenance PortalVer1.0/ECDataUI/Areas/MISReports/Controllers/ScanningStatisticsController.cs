using CustomModels.Models.MISReports.ScanningStatistics;
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

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorization]
    public class ScanningStatisticsController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;
        [HttpGet]
        [MenuHighlight]
        // GET: MISReports/ScanningStatistics
        public ActionResult ScanningStatisticsView()
        {
            try
            {
                caller = new ServiceCaller("ScanningStatisticsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                ScanningStatisticsReqModel reqModel = caller.GetCall<ScanningStatisticsReqModel>("ScanningStatisticsView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Scanning Statistics View", URLToRedirect = "/Home/HomePage" });
            }
        }
        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetScanningStatisticsDetails(FormCollection formCollection)
        {
            try

            {
                caller = new ServiceCaller("ScanningStatisticsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                ScanningStatisticsReqModel reqModel = new ScanningStatisticsReqModel();
                ScanningStatisticsResModel resultModel = new ScanningStatisticsResModel();
                string SROfficeID = formCollection["SROfficeID"];
         
                string DistrictID = formCollection["DistrictID"];
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);




                //Validation
                if (string.IsNullOrEmpty(SROfficeID) || SROfficeID == "0")
                {
                    return Json(new { success = false, errorMessage = "Please select SRO Name" }, JsonRequestBehavior.AllowGet);
                }else if(string.IsNullOrEmpty(DistrictID) || DistrictID == "0")
                {
                    return Json(new { success = false, errorMessage = "Please select District" }, JsonRequestBehavior.AllowGet);
                }

                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                reqModel.DateTime_FromDate = Convert.ToDateTime(fromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                reqModel.DROfficeID = Convert.ToInt32(DistrictID);
        

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                //var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = totalNum;
                int skip = startLen;
                String errorMessage = String.Empty;
                // For Excel download
                var SroCodeEx = reqModel.SROfficeID;
                var DROfficeIDEx = reqModel.DROfficeID;
                var FromDateEX = fromDate;
                var ToDateEX = ToDate;
                //End for excel download
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                //To get records
                resultModel = caller.PostCall<ScanningStatisticsReqModel, ScanningStatisticsResModel>("GetScanningStatisticsDetails", reqModel, out errorMessage);

                IEnumerable<ScanningStatisticsTableModel> result = resultModel.scanningStatisticsTableModelsList;
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Scanning Statistics Details." });


                }

                int TotalCount = result.Count();

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
                        result = result.Where(m => m.RegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.DistrictName.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                        m.DocType.ToLower().Contains(searchValue.ToLower()) ||
                        m.ScannedPagecount.ToLower().Contains(searchValue.ToLower()) ||
                        m.DateOfRegistration.ToLower().Contains(searchValue.ToLower()));
                        TotalCount = result.Count();
                    }
                }


                var gridData = result.Select(ScanningStatisticsTableModel => new
                {

                    srNo = ScanningStatisticsTableModel.srNo,

                    RegistrationNumber = ScanningStatisticsTableModel.RegistrationNumber,
                    DistrictName = ScanningStatisticsTableModel.DistrictName,
                    SROName = ScanningStatisticsTableModel.SROName,
                    DateOfRegistration = ScanningStatisticsTableModel.DateOfRegistration,
                    ScannedPagecount = ScanningStatisticsTableModel.ScannedPagecount,
                    ScanDate = ScanningStatisticsTableModel.ScanDate,
                    DocType = ScanningStatisticsTableModel.DocType

                });



                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + FromDateEX + "','" + ToDateEX + "','" + DROfficeIDEx + "','" + SroCodeEx + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";


                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = "Invalid To Date",

                    ExcelDownloadBtn = ExcelDownloadBtn

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

                        ExcelDownloadBtn = ExcelDownloadBtn

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

                        ExcelDownloadBtn = ExcelDownloadBtn


                    });
                }

                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;


            }

            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Scanning Statistics Details." });
            }
        }

        public ActionResult ExportScanningStatisticsDetailsToExcel(string FromDate, string ToDate, string SroCode, string DROfficeID)
        {
            try
            {
                caller = new ServiceCaller("ScanningStatisticsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("ScanningStatisticsDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                ScanningStatisticsReqModel reqModel = new ScanningStatisticsReqModel();
                reqModel.SROfficeID = Convert.ToInt32(SroCode);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);
                reqModel.DateTime_FromDate = Convert.ToDateTime(FromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);

                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;
                ScanningStatisticsResModel Result = caller.PostCall<ScanningStatisticsReqModel, ScanningStatisticsResModel>("GetScanningStatisticsDetails", reqModel, out errorMessage);


                if (Result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
                List<ScanningStatisticsTableModel> scanningStatisticsTableModels = Result.scanningStatisticsTableModelsList;

                if (Result.scanningStatisticsTableModelsList != null && Result.scanningStatisticsTableModelsList.Count > 0)
                {
                    fileName = "ScanningStatisticsDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("Scanning Statistics Detail");
                    createdExcelPath = CreateExcelForScanningStatisticsDetails(scanningStatisticsTableModels, fileName, excelHeader);
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


        private string CreateExcelForScanningStatisticsDetails(List<ScanningStatisticsTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("ScanningStatisticsDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());


                    workSheet.Cells[1, 1, 1, 8].Merge = true;
                    workSheet.Cells[2, 1, 2, 8].Merge = true;
                    workSheet.Cells[3, 1, 3, 8].Merge = true;
                    workSheet.Cells[4, 1, 4, 8].Merge = true;
                    workSheet.Cells[5, 1, 5, 8].Merge = true;
                    workSheet.Cells[6, 1, 6, 8].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 25;
                    workSheet.Column(7).Width = 25;
                    workSheet.Column(8).Width = 25;


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
                    workSheet.Cells[7, 2].Value = "District Name";
                    workSheet.Cells[7, 3].Value = "SRO Name";
                    workSheet.Cells[7, 4].Value = "Registration Number";
                    workSheet.Cells[7, 5].Value = "Date Of Registration";
                    workSheet.Cells[7, 6].Value = "Scanned Page count";
                    workSheet.Cells[7, 7].Value = "Scan Date";
                    workSheet.Cells[7, 8].Value = "DocType";



                    foreach (var items in result)
                    {
                        for (int i = 1; i < 9; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }




                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.DistrictName;
                        workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        workSheet.Cells[rowIndex, 4].Value = items.RegistrationNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.DateOfRegistration;
                        workSheet.Cells[rowIndex, 6].Value = items.ScannedPagecount;
                        workSheet.Cells[rowIndex, 7].Value = items.ScanDate;
                        workSheet.Cells[rowIndex, 8].Value = items.DocType;


                        for (int i = 1; i < 9; i++)
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

        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }

    }
}