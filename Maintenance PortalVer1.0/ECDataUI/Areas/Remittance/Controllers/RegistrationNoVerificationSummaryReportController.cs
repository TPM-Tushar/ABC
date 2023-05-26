#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationNoVerificationSummaryReportController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Registration No Verification Summary Report .

*/
#endregion

using CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport;
using ECDataUI.Common;
using ECDataUI.Filters;
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
    public class RegistrationNoVerificationSummaryReportController : Controller
    {
        ServiceCaller caller = null;
        [HttpGet]
        [MenuHighlight]
        // GET: Remittance/RegistrationNoVerificationSummaryReport
        public ActionResult RegistrationNoVerificationSummaryReportView()
        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationSummaryReportAPIController");
                RegistrationNoVerificationSummaryReportModel reqModel = caller.GetCall<RegistrationNoVerificationSummaryReportModel>("RegistrationNoVerificationSummaryReportView");
                return View(reqModel);

            }catch(Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Summary Report Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        public ActionResult GetSummaryReportDetails(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationSummaryReportAPIController");
                TimeSpan timeSpan = new TimeSpan(0,30,0);
                caller.HttpClient.Timeout = timeSpan;
                RegistrationNoVerificationSummaryReportModel reqModel = new RegistrationNoVerificationSummaryReportModel();
                RegistrationNoVerificationSummaryResultModel resultModel = new RegistrationNoVerificationSummaryResultModel();
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                var DocumentTypeId = formCollection["DocumentTypeId"];
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
              
                int pageSize = totalNum;
                int skip = startLen;
                String errorMessage = String.Empty;
                //Validations
                if (string.IsNullOrEmpty(DocumentTypeId) || DocumentTypeId == "0")
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
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                reqModel.DateTime_Date = Convert.ToDateTime(fromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);
                resultModel = caller.PostCall<RegistrationNoVerificationSummaryReportModel, RegistrationNoVerificationSummaryResultModel>("GetSummaryReportDetails", reqModel, out errorMessage);

                IEnumerable<RegistrationNoVerificationSummaryTableModel> result = resultModel.registrationNoVerificationSummaryTableList;
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Summary Report Details." });
                }

                int TotalCount = result.Count();
                //Added By Tushar on 28 Dec 2022
                var IsRecordFilter = false;
                //End By Tushar on 28 Dec 2022

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
                        if (reqModel.DocumentTypeId == 4)
                        {
                            result = result.Where(m =>
                                                     m.DistrictName.ToLower().Contains(searchValue.ToLower()));
                        }
                        else
                        {
                            result = result.Where(m =>
                                                    m.SROName.ToLower().Contains(searchValue.ToLower()));
                        }
                        TotalCount = result.Count();
                        //Added By Tushar on 28 Dec 2022
                        IsRecordFilter = true;
                        //End By Tushar on 28 Dec 2022

                    }
                }
                var gridData = result.Select(RegistrationNoVerificationSummaryTableModel => new
                    {
                        srNo = RegistrationNoVerificationSummaryTableModel.srNo,
                        M_M = RegistrationNoVerificationSummaryTableModel.M_M,
                        CNP_LNP = RegistrationNoVerificationSummaryTableModel.CNP_LNP,
                        CP_LNP = RegistrationNoVerificationSummaryTableModel.CP_LNP,
                        LP_CNP = RegistrationNoVerificationSummaryTableModel.LP_CNP,
                        L_Additional = RegistrationNoVerificationSummaryTableModel.L_Additional,
                        L_Missing = RegistrationNoVerificationSummaryTableModel.L_Missing,
                        SM_M = RegistrationNoVerificationSummaryTableModel.SM_M,
                        SROName = RegistrationNoVerificationSummaryTableModel.SROName,
                        Is_Duplicate = RegistrationNoVerificationSummaryTableModel.Is_Duplicate,

                    //Added By Tushar on 28 Dec 2022
                    DistrictName = RegistrationNoVerificationSummaryTableModel.DistrictName,
                    FirmResult_LA_CNA_Count = RegistrationNoVerificationSummaryTableModel.FirmResult_LA_CNA_Count,
                    FirmResult_CA_LNA_Count = RegistrationNoVerificationSummaryTableModel.FirmResult_CA_LNA_Count,
                    FirmResult_FN_Miss_Count = RegistrationNoVerificationSummaryTableModel.FirmResult_FN_Miss_Count,
                    FirmResult_SC_LA_CNA_Count = RegistrationNoVerificationSummaryTableModel.FirmResult_SC_LA_CNA_Count,
                    FirmResult_SC_CA_LNA_Count = RegistrationNoVerificationSummaryTableModel.FirmResult_SC_CA_LNA_Count,
                    FirmResult_SC_FN_Miss_Count = RegistrationNoVerificationSummaryTableModel.FirmResult_SC_FN_Miss_Count
                    //End By Tushar on 28 Dec 2022


                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + reqModel.DocumentTypeId + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,

                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid To Date",

                    ExcelDownloadBtn = ExcelDownloadBtn,
                    //Added By Tushar on 28 Dec 2022
                    IsRecordFilter = IsRecordFilter,
                    //End By Tushar on 28 Dec 2022

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

                            ExcelDownloadBtn = ExcelDownloadBtn,
                             //Added By Tushar on 28 Dec 2022
                            IsRecordFilter = IsRecordFilter,
                            //End By Tushar on 28 Dec 2022
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

                           ExcelDownloadBtn = ExcelDownloadBtn,
                            //Added By Tushar on 28 Dec 2022
                            IsRecordFilter = IsRecordFilter,
                            //End By Tushar on 28 Dec 2022


                        });
                    }

                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
             
            
            }
            catch(Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Summary Report Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        public ActionResult ExportSummaryReportToExcel(string fromDate, string ToDate,string DocumentTypeId)
        {
            try
            {
                caller = new ServiceCaller("RegistrationNoVerificationSummaryReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("SummaryReport.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                RegistrationNoVerificationSummaryReportModel reqModel = new RegistrationNoVerificationSummaryReportModel();
                reqModel.DateTime_Date = Convert.ToDateTime(fromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);

                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;
                RegistrationNoVerificationSummaryResultModel Result = caller.PostCall<RegistrationNoVerificationSummaryReportModel, RegistrationNoVerificationSummaryResultModel>("GetSummaryReportDetails", reqModel, out errorMessage);


                if (Result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
                List<RegistrationNoVerificationSummaryTableModel> registrationNoVerificationSummaryTableModels = Result.registrationNoVerificationSummaryTableList;

                if (Result.registrationNoVerificationSummaryTableList != null && Result.registrationNoVerificationSummaryTableList.Count > 0)
                {
                    fileName = "SummaryReportDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("Summary Report");
					//Added By Tushar on 28 Dec 2022
                    if (reqModel.DocumentTypeId == 4)
                    {
                        createdExcelPath = CreateExcelForFirmSummaryReportDetails(registrationNoVerificationSummaryTableModels, fileName, excelHeader);
                    }
                    else
                    {
                        createdExcelPath = CreateExcelForSummaryReportDetails(registrationNoVerificationSummaryTableModels, fileName, excelHeader);
                    }
					//End By Tushar on 28 Dec 2022
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


        private string CreateExcelForSummaryReportDetails(List<RegistrationNoVerificationSummaryTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("SummaryReportDetails");
                   
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[6, 3, 6, 10].Style.Font.Size = 18;
                    //workSheet.Cells[6, 3, 6, 10].Style.Font.Italic = true;

                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());


                    workSheet.Cells[1, 1, 1, 10].Merge = true;
                    workSheet.Cells[2, 1, 2, 10].Merge = true;
                    workSheet.Cells[3, 1, 3, 10].Merge = true;
                    workSheet.Cells[4, 1, 4, 10].Merge = true;
                    workSheet.Cells[5, 1, 5, 10].Merge = true;
                    workSheet.Cells[6, 1, 6, 2].Merge = true;
                    workSheet.Cells[6, 3, 6, 6].Merge = true;
                    workSheet.Cells[6, 7, 6, 10].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;

                    for (int i = 1; i < 11; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment =OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

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
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    //
                    workSheet.Cells[6, 3, 6, 6].Value = "Transactional Data Statistics";
                    workSheet.Cells[6, 7, 6, 10].Value = "Scan Document Data Statistics";

                    //

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "SRO Name";
          
                    workSheet.Cells[7, 3].Value = "MisMatch In Final Registration Number";
                    workSheet.Cells[7, 4].Value = "Record Removed From Local Database";
                    workSheet.Cells[7, 5].Value = "Additional Records In Local Database";
                    workSheet.Cells[7, 6].Value = "Duplicate Records";
                
                    workSheet.Cells[7, 7].Value = "Central Scan Document Not Present Local Present";

                    workSheet.Cells[7, 8].Value = "Central and Local Scan Document Not Present";
                    workSheet.Cells[7, 9].Value = "Central Scan Document Present Local Not Present";
                    workSheet.Cells[7, 10].Value = "Scan File Mismatch";




                    foreach (var items in result)
                    {
                        for (int i = 1; i < 11; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";

                        }


                            workSheet.Cells[rowIndex, 1].Value = items.srNo;
                            workSheet.Cells[rowIndex, 2].Value = items.SROName;
                            workSheet.Cells[rowIndex, 3].Value = items.M_M;

                            workSheet.Cells[rowIndex, 4].Value = items.L_Missing;
                            workSheet.Cells[rowIndex, 5].Value = items.L_Additional;
                            workSheet.Cells[rowIndex, 6].Value = items.Is_Duplicate;
                            workSheet.Cells[rowIndex, 7].Value = items.LP_CNP;
                            workSheet.Cells[rowIndex, 8].Value = items.CNP_LNP;
                            workSheet.Cells[rowIndex, 9].Value = items.CP_LNP;
                            workSheet.Cells[rowIndex, 10].Value = items.SM_M; 
                       



                        for (int i = 1; i < 11; i++)
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

        //Added By Tushar on 28 Dec 2022
        private string CreateExcelForFirmSummaryReportDetails(List<RegistrationNoVerificationSummaryTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("SummaryReportDetails");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[6, 3, 6, 8].Style.Font.Size = 18;
                 

                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());


                    workSheet.Cells[1, 1, 1, 8].Merge = true;
                    workSheet.Cells[2, 1, 2, 8].Merge = true;
                    workSheet.Cells[3, 1, 3, 8].Merge = true;
                    workSheet.Cells[4, 1, 4, 8].Merge = true;
                    workSheet.Cells[5, 1, 5, 8].Merge = true;
                    workSheet.Cells[6, 1, 6, 2].Merge = true;
                    workSheet.Cells[6, 3, 6, 5].Merge = true;
                    workSheet.Cells[6, 6, 6, 8].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                  

                    for (int i = 1; i < 11; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

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
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                 
                    workSheet.Cells[6, 3, 6, 5].Value = "Transactional Data Statistics";
                    workSheet.Cells[6, 6, 6, 8].Value = "Scan Document Data Statistics";

                  

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "District Name";

                    workSheet.Cells[7, 3].Value = "Central Firm Data Not Present Local Present";
                    workSheet.Cells[7, 4].Value = "Central Firm Data Present Local Not Present";
                    workSheet.Cells[7, 5].Value = "MisMatch In Firm Number";

                    workSheet.Cells[7, 6].Value = "Central Scan Document Not Present Local Present";
                    workSheet.Cells[7, 7].Value = "Central Scan Document Present Local Not Present";
                    workSheet.Cells[7, 8].Value = "MisMatch In Scan File Name";
                




                    foreach (var items in result)
                    {
                        for (int i = 1; i < 11; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";

                        }


                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.DistrictName;
                        workSheet.Cells[rowIndex, 3].Value = items.FirmResult_LA_CNA_Count;

                        workSheet.Cells[rowIndex, 4].Value = items.FirmResult_CA_LNA_Count;
                        workSheet.Cells[rowIndex, 5].Value = items.FirmResult_FN_Miss_Count;
                        workSheet.Cells[rowIndex, 6].Value = items.FirmResult_SC_CA_LNA_Count;
                        workSheet.Cells[rowIndex, 7].Value = items.FirmResult_SC_LA_CNA_Count;
                        workSheet.Cells[rowIndex, 8].Value = items.FirmResult_SC_FN_Miss_Count;
                    




                        for (int i = 1; i < 11; i++)
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
        //End By Tushar on 28 Dec 2022


        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }
    }
}