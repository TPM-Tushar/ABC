using CustomModels.Models.Remittance.XELFileStorageDetails;
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]
    public class XELFileStorageDetailsController : Controller
    {
        ServiceCaller caller = null;

        [MenuHighlight]
        [EventAuditLogFilter(Description = "XEL File Storage View")]
        public ActionResult XELFileStorageView()
        {
            try
            {
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DailyReceiptsDetails;
                caller = new ServiceCaller("XELFileStorageDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                int OfficeID = KaveriSession.Current.OfficeID;
                XELFileStorageViewModel reqModel = caller.GetCall<XELFileStorageViewModel>("XELFileStorageView", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving XEL File Storage View", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving XEL File Storage View", URLToRedirect = "/Home/HomePage" });
            }

        }

        [EventAuditLogFilter(Description = "XEL File Office List")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult XELFileOfficeList(FormCollection formCollection)
        {
            caller = new ServiceCaller("XELFileStorageDetailsAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects    

                string OfficeType = formCollection["OfficeType"];
                if (String.IsNullOrEmpty(OfficeType))
                {
                    return Json(new { serverError = false, success = false, errorMessage = "Please select office type." });
                }

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


                XELFileStorageViewModel reqModel = new XELFileStorageViewModel()
                {
                    OfficeType = OfficeType
                };





                XELFileStorageWrapperModel ResModel = caller.PostCall<XELFileStorageViewModel, XELFileStorageWrapperModel>("XELFileOfficeList", reqModel, out errorMessage);


                IEnumerable<XELFileOfficeListRESModel> result = ResModel.XELFileOfficeListRESModelList;


                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting XEL file storage details." });
                }
                int totalCount = result.Count();

                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}

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
                          m.NoOfFiles.ToLower().Contains(searchValue.ToLower()) ||
                          m.TotalSizeOnDiskInMB.ToString().Contains(searchValue.ToLower()) ||
                          m.LastCentralizedOn.ToLower().Contains(searchValue.ToLower())
                        );

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(XELFileOfficeListRESModel => new
                {
                    SerialNo = XELFileOfficeListRESModel.SerialNo,
                    OfficeName = XELFileOfficeListRESModel.OfficeName,
                    NoOfFiles = XELFileOfficeListRESModel.NoOfFiles,
                    TotalSizeOnDiskInMB = XELFileOfficeListRESModel.TotalSizeOnDiskInMB,
                    LastCentralizedOn = XELFileOfficeListRESModel.LastCentralizedOn
                });

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + OfficeType + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                //var JsonData = "";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
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
                        data = gridData.ToArray(),
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
                return Json(new { serverError = true, errorMessage = "Error occured while getting XEL file storage details." }, JsonRequestBehavior.AllowGet);
            }
        }


        [EventAuditLogFilter(Description = "XELFile List Office Wise")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult XELFileListOfficeWise(FormCollection formCollection)
        {
            caller = new ServiceCaller("XELFileStorageDetailsAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects    

                string SROCode = formCollection["SROCode"];
                string OfficeType = formCollection["OfficeType"];
                if (String.IsNullOrEmpty(OfficeType))
                {
                    return Json(new { serverError = false, success = false, errorMessage = "Please select office type." });
                }
                if (String.IsNullOrEmpty(SROCode))
                {
                    return Json(new { serverError = false, success = false, errorMessage = "Error occured while getting XEL File List Office Wise." });
                }

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


                XELFileStorageViewModel reqModel = new XELFileStorageViewModel()
                {
                    SROCode = SROCode,
                    OfficeType = OfficeType,
                    StartLen = startLen,
                    TotalNum = totalNum,
                    SearchValue = searchValue
                };

                XELFileStorageWrapperModel ResModel = caller.PostCall<XELFileStorageViewModel, XELFileStorageWrapperModel>("XELFileListOfficeWise", reqModel, out errorMessage);

                IEnumerable<XELFileListOfficeWiseRESModel> result = ResModel.XELFileListOfficeWiseRESModelList;


                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting XEL File List Office Wise." });
                }
                int totalCount = result.Count();

                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}

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
                          m.FileName.ToLower().Contains(searchValue.ToLower()) ||
                          m.FileSizeInMB.ToString().Contains(searchValue.ToLower()) ||
                          m.FileDateTime.ToString().Contains(searchValue.ToLower()) ||
                          m.FilePath.ToLower().Contains(searchValue.ToLower()) ||
                          m.EventStartDate.ToLower().Contains(searchValue.ToLower()) ||
                          m.EventEndDate.ToLower().Contains(searchValue.ToLower()) ||
                          m.FileReadDateTime.ToLower().Contains(searchValue.ToLower())
                        );

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(XELFileListOfficeWiseRESModel => new
                {
                    SerialNo = XELFileListOfficeWiseRESModel.SerialNo,
                    FileName = XELFileListOfficeWiseRESModel.FileName,
                    FileSizeInMB = XELFileListOfficeWiseRESModel.FileSizeInMB,
                    FileDateTime = XELFileListOfficeWiseRESModel.FileDateTime,
                    FilePath = XELFileListOfficeWiseRESModel.FilePath,
                    EventStartDate = XELFileListOfficeWiseRESModel.EventStartDate,
                    EventEndDate = XELFileListOfficeWiseRESModel.EventEndDate,
                    FileReadDateTime = XELFileListOfficeWiseRESModel.FileReadDateTime
                });

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFunFileList('" + SROCode + "','" + OfficeType + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                //var JsonData = "";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
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
                        data = gridData.ToArray(),
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
                return Json(new { serverError = true, errorMessage = "Error occured while getting XEL File List Office Wise." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RootDirectoryTable(String officeType)

        {
            caller = new ServiceCaller("XELFileStorageDetailsAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                String errorMessage = String.Empty;
                XELFileStorageViewModel reqModel = new XELFileStorageViewModel()
                {
                    OfficeType = officeType
                };

                XELFileStorageViewModel ResModel = caller.PostCall<XELFileStorageViewModel, XELFileStorageViewModel>("RootDirectoryTable", reqModel, out errorMessage);
                if (ResModel == null)
                {
                    ResModel = new XELFileStorageViewModel()
                    {
                        RootDirInfoModelList = new List<RootDirInfoModel>()
                    };
                    return View(ResModel);
                    //return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error in getting Root directory information.", URLToRedirect = "/Home/HomePage" });
                    //return Json(new { serverError = true, success = false, errorMessage = "Error in getting Root directory information." }, JsonRequestBehavior.AllowGet);
                }
                else
                    return View(ResModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error in getting Root directory information.", URLToRedirect = "/Home/HomePage" });
            }

        }

        #region Excel

        [EventAuditLogFilter(Description = "Export XEL File Count Office wise Report to EXCEL")]
        public ActionResult ExportXELFileCountOfficewiseToExcel(string OfficeType)
        {
            try
            {
                caller = new ServiceCaller("XELFileStorageDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("XELFileCountOfficeWise" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                XELFileStorageViewModel model = new XELFileStorageViewModel
                {
                    OfficeType = OfficeType,
                    IsExcel = true
                    //SearchValue=" "//In search value space is sent to fetch all details
                };

                XELFileStorageWrapperModel ResModel = new XELFileStorageWrapperModel();
                //model.IsExcel = true;
                ResModel = caller.PostCall<XELFileStorageViewModel, XELFileStorageWrapperModel>("XELFileOfficeList", model, out errorMessage);
                if (ResModel.XELFileOfficeListRESModelList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                    //return Json(new { success = false, errorMessage = "Error Occured While Getting XEL File Office List..." }, JsonRequestBehavior.AllowGet);
                }
                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = "XEL File count office wise";//String.Empty;//string.Format("Anywhere EC Log ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader);
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

        private string CreateExcel(XELFileStorageWrapperModel ResModel, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("XEL File count office wise");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    //workSheet.Cells[2, 1].Value = "District : " + SelectedDist;
                    //workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;
                    //workSheet.Cells[4, 1].Value = "Log Type : " + SelectedLogType;

                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "Total Records : " + (ResModel.XELFileOfficeListRESModelList.Count());
                    workSheet.Cells[1, 1, 1, 5].Merge = true;
                    workSheet.Cells[2, 1, 2, 5].Merge = true;
                    workSheet.Cells[3, 1, 3, 5].Merge = true;
                    workSheet.Cells[4, 1, 4, 5].Merge = true;
                    //workSheet.Cells[5, 1, 5, 7].Merge = true;
                    //workSheet.Cells[6, 1, 6, 7].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    //workSheet.Column(6).Width = 45;
                    //workSheet.Column(7).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    //workSheet.Row(6).Style.Font.Bold = true;
                    //workSheet.Row(8).Style.Font.Bold = true;


                    int rowIndex = 6;
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[5, 1].Value = "Serial No.";
                    workSheet.Cells[5, 2].Value = "Office Name";
                    workSheet.Cells[5, 3].Value = "No. of files";
                    workSheet.Cells[5, 4].Value = "Total Size on disk (MB)";
                    workSheet.Cells[5, 5].Value = "Last Centralized on";
                    //workSheet.Cells[5, 6].Value = "Description";
                    //workSheet.Cells[5, 7].Value = "Log Date Time";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";


                    foreach (var items in ResModel.XELFileOfficeListRESModelList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                        workSheet.Cells[rowIndex, 3].Value = items.NoOfFiles;
                        workSheet.Cells[rowIndex, 4].Value = items.TotalSizeOnDiskInMB;
                        workSheet.Cells[rowIndex, 5].Value = items.LastCentralizedOn;
                        //workSheet.Cells[rowIndex, 6].Value = items.Desc;
                        //workSheet.Cells[rowIndex, 7].Value = items.LogDateTime;
                        //workSheet.Cells[rowIndex, 8].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }


                    using (ExcelRange Rng = workSheet.Cells[5, 1, (rowIndex - 1), 5])
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

        [EventAuditLogFilter(Description = "Export XEL File List Office wise Report to EXCEL")]
        public ActionResult ExportXELFileListOfficewiseToExcel(string SROCode, string OfficeType)
        {
            try
            {
                caller = new ServiceCaller("XELFileStorageDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("XELFileListOfficeWise" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                XELFileStorageViewModel model = new XELFileStorageViewModel
                {
                    SROCode = SROCode,
                    OfficeType = OfficeType,
                    IsExcel = true
                    //SearchValue=" "//In search value space is sent to fetch all details
                };

                XELFileStorageWrapperModel ResModel = new XELFileStorageWrapperModel();
                //model.IsExcel = true;
                ResModel = caller.PostCall<XELFileStorageViewModel, XELFileStorageWrapperModel>("XELFileListOfficeWise", model, out errorMessage);
                if (ResModel.XELFileListOfficeWiseRESModelList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                    //return Json(new { success = false, errorMessage = "Error Occured While Getting XEL File Office List..." }, JsonRequestBehavior.AllowGet);
                }
                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = "XEL File List office wise";//String.Empty;//string.Format("Anywhere EC Log ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcelFileList(ResModel, fileName, excelHeader);
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

        private string CreateExcelFileList(XELFileStorageWrapperModel ResModel, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("XEL File List office wise");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    //workSheet.Cells[2, 1].Value = "District : " + SelectedDist;
                    //workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;
                    //workSheet.Cells[4, 1].Value = "Log Type : " + SelectedLogType;

                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "Total Records : " + (ResModel.XELFileListOfficeWiseRESModelList.Count());
                    workSheet.Cells[1, 1, 1, 8].Merge = true;
                    workSheet.Cells[2, 1, 2, 8].Merge = true;
                    workSheet.Cells[3, 1, 3, 8].Merge = true;
                    workSheet.Cells[4, 1, 4, 8].Merge = true;
                    //workSheet.Cells[5, 1, 5, 7].Merge = true;
                    //workSheet.Cells[6, 1, 6, 7].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    //workSheet.Row(6).Style.Font.Bold = true;
                    //workSheet.Row(8).Style.Font.Bold = true;


                    int rowIndex = 6;
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[5, 1].Value = "Serial No.";
                    workSheet.Cells[5, 2].Value = "File Name";
                    workSheet.Cells[5, 3].Value = "File Size (MB)";
                    workSheet.Cells[5, 4].Value = "File Create Date time";
                    workSheet.Cells[5, 5].Value = "Path";
                    workSheet.Cells[5, 6].Value = "Event Start Date Time";
                    workSheet.Cells[5, 7].Value = "Event End Date Time";
                    workSheet.Cells[5, 8].Value = "FIle Read Date Time";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in ResModel.XELFileListOfficeWiseRESModelList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.FileName;
                        workSheet.Cells[rowIndex, 3].Value = items.FileSizeInMB;
                        workSheet.Cells[rowIndex, 4].Value = items.FileDateTime;
                        workSheet.Cells[rowIndex, 5].Value = items.FilePath;
                        workSheet.Cells[rowIndex, 6].Value = items.EventStartDate;
                        workSheet.Cells[rowIndex, 7].Value = items.EventEndDate;
                        workSheet.Cells[rowIndex, 8].Value = items.FileReadDateTime;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }


                    using (ExcelRange Rng = workSheet.Cells[5, 1, (rowIndex - 1), 8])
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

        #endregion


        public ActionResult XELFileDownloadPathVerify(String path)

        {
            caller = new ServiceCaller("XELFileStorageDetailsAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                String errorMessage = String.Empty;
                XELFileStorageViewModel reqModel = new XELFileStorageViewModel()
                {
                    FileDownloadPath = path
                };

                XELFileStorageViewModel ResModel = caller.PostCall<XELFileStorageViewModel, XELFileStorageViewModel>("XELFileDownloadPathVerify", reqModel, out errorMessage);
                if (ResModel == null)
                {
                    return Json(new { serverError = false, success = false, errorMessage = "Error in getting Xel file." }, JsonRequestBehavior.AllowGet);

                    //return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error in getting Root directory information.", URLToRedirect = "/Home/HomePage" });
                    //return Json(new { serverError = true, success = false, errorMessage = "Error in getting Root directory information." }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { serverError = false, success = true, IsFileExistAtDownloadPath = ResModel.IsFileExistAtDownloadPath }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, success = false, errorMessage = "Error in getting Xel file." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult XELFileDownload(string path,String OfficeType, String OfficeCodeSelected)
        {
            try
            {
                caller = new ServiceCaller("XELFileStorageDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                string errorMessage = string.Empty;

                XELFileStorageViewModel model = new XELFileStorageViewModel
                {
                    FileDownloadPath = path
                };

                XELFileStorageWrapperModel ResModel = new XELFileStorageWrapperModel();
                //String fileName = (OfficeType == "SR" ? "SRO" : "DRO") + "_" + OfficeCodeSelected + "_" + Path.GetFileName(path.Replace('*', '\\').Replace('$', ' '));
                String fileName = Path.GetFileName(path.Replace('*', '\\').Replace('$', ' '));
                ResModel = caller.PostCall<XELFileStorageViewModel, XELFileStorageWrapperModel>("XELFileDownload", model, out errorMessage);
                if (ResModel == null)
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading XEL File", URLToRedirect = "/Home/HomePage" });
                else if (ResModel.FileContentField == null)
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading XEL File", URLToRedirect = "/Home/HomePage" });
                else
                    return File(ResModel.FileContentField, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

    }
}