#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   JSlipUploadReportController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.JSlipUploadReport;
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

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]

    public class JSlipUploadReportController : Controller
    {
        ServiceCaller caller = null;

        /// <summary>
        /// BhoomiFileUploadReportView
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "JSlip Upload Report View")]
        public ActionResult JSlipUploadReportView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.JSlipUploadReport;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("JSlipUploadReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                JSlipUploadRptViewModel reqModel = caller.GetCall<JSlipUploadRptViewModel>("JSlipUploadReportView", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving JSlip Upload Report View", URLToRedirect = "/Home/HomePage" });
                }
                return View(reqModel);
            }
            catch (Exception)
            {
                throw;

            }
        }

        /// <summary>
        /// Get  Datatable
        /// </summary>
        /// <param name="ReqModel"></param>
        /// <returns>View</returns>
        [EventAuditLogFilter(Description = "Loads JSlip Upload Report Datatable")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult LoadJSlipUploadReportDataTable(FormCollection formCollection)
        {
            caller = new ServiceCaller("JSlipUploadReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        
                string FromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SroID = formCollection["SroID"];
                string DistrictID = formCollection["DistrictID"];
                int SroId = Convert.ToInt32(SroID);
                int DistrictId = Convert.ToInt32(DistrictID);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                String errorMessage = String.Empty;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });

                ////Validation For DR Login
                //if (LevelID == Convert.ToInt16(CommonEnum.LevelDetails.DR))
                //{
                //    //Validation for DR when user do not select any sro which is by default "Select"
                //    if ((SroId == 0))
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = false,
                //            errorMessage = "Please select any SRO"
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;
                //    }
                //}
                //else
                //{//Validations of Logins other than SR and DR

                //    if ((DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = false,
                //            errorMessage = "Please select any District"
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;
                //    }
                //    //else
                //    // if (SroId == 0 && DistrictId != 0)//when User selects DR but not SR which is by default "Select"
                //    //{
                //    //    var emptyData = Json(new
                //    //    {
                //    //        draw = formCollection["draw"],
                //    //        recordsTotal = 0,
                //    //        recordsFiltered = 0,
                //    //        data = "",
                //    //        status = false,
                //    //        errorMessage = "Please select any SRO"
                //    //    });
                //    //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    //    return emptyData;

                //    //}
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
                else if ((toDate - frmDate).TotalDays > 30)//six months validation by RamanK on 20-09-2019
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Data of One month can be seen at a time"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;

                }

                System.Text.RegularExpressions.Regex regxForSROID_and_DistrictID = new Regex("^[0-9]*$");
                Match mtchSRO = regxForSROID_and_DistrictID.Match(SroID);
                Match mtchDistrict = regxForSROID_and_DistrictID.Match(DistrictID);

                if (!mtchDistrict.Success)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid District" }, JsonRequestBehavior.AllowGet);

                }
                if (!mtchSRO.Success)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid SRO Office" }, JsonRequestBehavior.AllowGet);

                }
                JSlipUploadRptViewModel reqModel = new JSlipUploadRptViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.SROfficeID = SroId;
                reqModel.DistrictID = DistrictId;
                reqModel.FromDate = FromDate;
                reqModel.ToDate = ToDate;
                reqModel.SearchValue = searchValue;

                JSlipUploadRptResModel JSlipUploadReportResModel = caller.PostCall<JSlipUploadRptViewModel, JSlipUploadRptResModel>("LoadJSlipUploadReportDataTable", reqModel, out errorMessage);
                if (JSlipUploadReportResModel == null || JSlipUploadReportResModel.IJSlipUploadRecList == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting JSlip upload report." });
                }

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

                }


                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    JSlipUploadReportResModel.IJSlipUploadRecList = JSlipUploadReportResModel.IJSlipUploadRecList.OrderBy(sortColumn + " " + sortColumnDir);
                }

                //var gridData = PendingDocsResModel.IPendingDocsDatatableRecList.Select(PendingDocsDatatableRecord => new
                //{
                //    SerialNo = PendingDocsDatatableRecord.SerialNo,
                //    District = PendingDocsDatatableRecord.District,
                //    SRO = PendingDocsDatatableRecord.SRO,
                //    NoOfDocsPresented = PendingDocsDatatableRecord.NoOfDocsPresented,
                //    NoOfDocsRegistered = PendingDocsDatatableRecord.NoOfDocsRegistered,
                //    Str_NoOfDocsKeptPending = PendingDocsDatatableRecord.Str_NoOfDocsKeptPending,
                //    Str_DocsNotRegdNotPending = PendingDocsDatatableRecord.Str_DocsNotRegdNotPending
                //});

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //  String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = JSlipUploadReportResModel.IJSlipUploadRecList.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = JSlipUploadReportResModel.IJSlipUploadRecList.ToArray(),
                        recordsTotal = JSlipUploadReportResModel.TotalCount,
                        status = "1",
                        recordsFiltered = JSlipUploadReportResModel.FilteredRecCount,
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
                        data = JSlipUploadReportResModel.IJSlipUploadRecList.ToArray(),
                        recordsTotal = JSlipUploadReportResModel.TotalCount,
                        status = "1",
                        recordsFiltered = JSlipUploadReportResModel.TotalCount,
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
                return Json(new { serverError = true, errorMessage = "Error occured while getting Bhoomi File Upload Report." }, JsonRequestBehavior.AllowGet);
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

        #region Excel
        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="SroID"></param>
        /// <param name="SelectedSRO"></param>
        /// <param name="SelectedDistrict"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Export Bhoomi File Upload Report to EXCEL")]
        public ActionResult ExportJSlipUploadRptToExcel(String FromDate, String ToDate, int SROCode, int DistrictCode, string SelectedDistrict, string SelectedSRO)
        {
            try
            {
                caller = new ServiceCaller("JSlipUploadReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("JSlipUploadReport.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                //DateTime frmDate, toDate;
                //DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                //DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);

                JSlipUploadRptViewModel reqModel = new JSlipUploadRptViewModel
                {

                    SROfficeID = Convert.ToInt32(SROCode),
                    DistrictID = Convert.ToInt32(DistrictCode),
                    FromDate = FromDate,
                    ToDate = ToDate,
                    startLen = 0,
                    totalNum = 10,
                };

                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}

                JSlipUploadRptResModel ResModel = new JSlipUploadRptResModel();
               
                reqModel.IsExcel = true;
                ResModel = caller.PostCall<JSlipUploadRptViewModel, JSlipUploadRptResModel>("LoadJSlipUploadReportDataTable", reqModel, out errorMessage);

                if (ResModel == null || ResModel.IJSlipUploadRecList == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Getting Jslip Upload Report Datatable..." }, JsonRequestBehavior.AllowGet);
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("JslipUploadReport ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, SelectedDistrict, SelectedSRO);

                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "BhoomiFileUploadReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel for Jslip Upload Report", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create Excel
        /// </summary>
        /// <param name="ResModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SelectedDistrict"></param>
        /// <param name="SelectedSRO"></param>
        /// <returns>returns excel file path</returns>
        private string CreateExcel(JSlipUploadRptResModel ResModel, string fileName, string excelHeader, string SelectedDistrict, string SelectedSRO)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("CD Written Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
                    workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (ResModel.IJSlipUploadRecList.Count());
                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                    workSheet.Cells[4, 1, 4, 9].Merge = true;
                    workSheet.Cells[5, 1, 5, 9].Merge = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    //workSheet.Column(5).Width = 30;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 40;
                    workSheet.Column(8).Width = 35;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;
                    workSheet.Column(12).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "S. No.";
                    workSheet.Cells[7, 2].Value = "Office Name";
                    workSheet.Cells[7, 3].Value = "File Name";
                    workSheet.Cells[7, 4].Value = "Total Records";
                    workSheet.Cells[7, 5].Value = "Document Number List";
                    workSheet.Cells[7, 6].Value = "Additional Documents";
                    workSheet.Cells[7, 7].Value = "Upload Date Time";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Cells[7, 6].Style.WrapText = true;

                    workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.IJSlipUploadRecList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                        workSheet.Cells[rowIndex, 3].Value = items.FileName;
                        workSheet.Cells[rowIndex, 4].Value = items.TotalRecords;
                        workSheet.Cells[rowIndex, 5].Value = items.DocNumberList;
                        workSheet.Cells[rowIndex, 6].Value = items.AdditionalDocs;
                        workSheet.Cells[rowIndex, 7].Value = items.UploadDateTime;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        rowIndex++;
                    }


                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 7])
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
    }
}