#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationScanningDetailsController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 11 Jan 2023
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Registration Scanning Details Report .

*/
#endregion

using CustomModels.Models.Remittance.RegistrationScanningDetails;
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
    public class RegistrationScanningDetailsController : Controller
    {
        ServiceCaller caller = null;
        // GET: Remittance/RegistrationScanningDetails
       
        [HttpGet,MenuHighlight]
        public ActionResult RegistrationScanningDetailsView(int DocumentTypeId = 0)
            {
            try
            {
                caller = new ServiceCaller("RegistrationScanningDetailsAPIController");
                RegistrationScanningDetailsModel reqModel = caller.GetCall<RegistrationScanningDetailsModel>("RegistrationScanningDetailsView", new { DocumentTypeId = DocumentTypeId });
				//Added By Tushar on 16 jan 2023
                if (DocumentTypeId == 0)
                {
                    return View(reqModel);
                }
                return Json(new { OfficeList = reqModel.SROfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
				//End By Tushar on 16 Jan 2023
            }
            catch(Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Registration Scanning Details View", URLToRedirect = "/Home/HomePage" });
            }
        }
        [HttpPost]
        public ActionResult GetRegistrationScanningDetails(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("RegistrationScanningDetailsAPIController");
                TimeSpan timeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = timeSpan;
                RegistrationScanningDetailsModel reqModel = new RegistrationScanningDetailsModel();
                RegistrationScanningDetailsResultModel resultModel = new RegistrationScanningDetailsResultModel();
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                var DocumentTypeId = formCollection["DocumentTypeId"];
                string SROfficeID = formCollection["SROfficeID"];
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                //Added By Tushar on 13 Jan 2023
                string ScanFilterValue = formCollection["ScanFilterValue"];
                //End By Tushar on 13 Jan 2023
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
                reqModel.DateTime_FromDate = Convert.ToDateTime(fromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);
                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                //Added By Tushar on 13 jan 2023
                reqModel.ScanFilterValue = ScanFilterValue;
                //End BY Tushar on 13 Jan 2023
                resultModel = caller.PostCall<RegistrationScanningDetailsModel, RegistrationScanningDetailsResultModel>("GetRegistrationScanningDetails", reqModel, out errorMessage);

                IEnumerable<RegistrationScanningDetailsTableModel> result = resultModel.registrationScanningDetailsTableModelsList;
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Registration Scanning Details." });
                }
                //Added By Tushar on 13 Jan 2023
            
                switch(reqModel.ScanFilterValue)
                {
                    case "SME":
                        {
                            result = result.Where(x => x.ScanMasterID.Contains("NO")).ToList();
                            break;
                        }
                    case "SFUDE":
                        {
                           
                            result = result.Where(x => x.ScannedFileUploadDetailsID.Contains("NO")).ToList();
                            break;
                        }
                    case "ICWE":
                        {
                            result = result.Where(x => x.IsCDWritten.Contains("NO")).ToList();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                //End By Tushar on 13 jan 2023
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
                   
                            result = result.Where(m =>m.CDNumber.ToLower().Contains(searchValue.ToLower())||
                                                     m.ScanMasterID.ToLower().Contains(searchValue.ToLower())||
                                                     m.ScannedFileUploadDetailsID.ToLower().Contains(searchValue.ToLower())||
                                                     m.IsCDWritten.ToLower().Contains(searchValue.ToLower())||
                                                     m.ScanFilePath.ToLower().Contains(searchValue.ToLower()) ||
                                                     m.MarriageCaseNo.ToLower().Contains(searchValue.ToLower()));
                
                        TotalCount = result.Count();
             

                    }
                }
                var gridData = result.Select(RegistrationNoVerificationSummaryTableModel => new
                {
                    srNo = RegistrationNoVerificationSummaryTableModel.srNo,
                    SROCode = RegistrationNoVerificationSummaryTableModel.SROCode,
                    MarriageCaseNo = RegistrationNoVerificationSummaryTableModel.MarriageCaseNo,
                    CDNumber = RegistrationNoVerificationSummaryTableModel.CDNumber,
                    ScanMasterEntry = RegistrationNoVerificationSummaryTableModel.ScanMasterID,
                    ScannedFileUploadDetailsEntry = RegistrationNoVerificationSummaryTableModel.ScannedFileUploadDetailsID,
                    IsCDWritten = RegistrationNoVerificationSummaryTableModel.IsCDWritten,
                    DateOfRegistration= RegistrationNoVerificationSummaryTableModel.DateOfRegistration_Date,
                    NoticeNo= RegistrationNoVerificationSummaryTableModel.MarriageCaseNo,
                    NoticeIssuedDate = RegistrationNoVerificationSummaryTableModel.DateOfRegistration_Date,
                    //Added By Tushar on 13 Jan 2023
                    ScanFilePath= RegistrationNoVerificationSummaryTableModel.ScanFilePath,
                    //End By Tushar on 13 Jan 2023
			        //Added By Tushar on 16 jan 2023
                    FirmNumber= RegistrationNoVerificationSummaryTableModel.FirmNumber,
                    DRCode= RegistrationNoVerificationSummaryTableModel.DRCode
                    //End By Tushar on 16 Jan 2023

                });

                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + reqModel.DocumentTypeId + "','" + reqModel.SROfficeID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
				//Coomented and Added BY Tushar on 13 Jan 2023
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + reqModel.DocumentTypeId + "','" + reqModel.SROfficeID + "','" + reqModel.ScanFilterValue + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,

                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = "Invalid To Date",

                    ExcelDownloadBtn = ExcelDownloadBtn,
        

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
             


                    });
                }

                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;


            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Registration Scanning Details View", URLToRedirect = "/Home/HomePage" });
            }

        }

        public ActionResult ExportRegistrationScanningDetailsToExcel(string SROfficeID,string DocumentTypeId, string FromDate, string ToDate,string ScanFilterValue)
        {
            try
            {
                caller = new ServiceCaller("RegistrationScanningDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("RegistrationScanningDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                RegistrationScanningDetailsModel reqModel = new RegistrationScanningDetailsModel();
                reqModel.DateTime_FromDate = Convert.ToDateTime(FromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);
                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                //Added By Tushar on 13 jan 2023
                reqModel.ScanFilterValue = ScanFilterValue;
                //End By Tushar on 13 jan 2023
                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;
                RegistrationScanningDetailsResultModel Result = caller.PostCall<RegistrationScanningDetailsModel, RegistrationScanningDetailsResultModel>("GetRegistrationScanningDetails", reqModel, out errorMessage);


                if (Result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
                List<RegistrationScanningDetailsTableModel> registrationNoVerificationSummaryTableModels = Result.registrationScanningDetailsTableModelsList;
                //Added By Tushar on 13 Jan 2023
                switch (reqModel.ScanFilterValue)
                {
                    case "SME":
                        {
                            registrationNoVerificationSummaryTableModels = registrationNoVerificationSummaryTableModels.Where(x => x.ScanMasterID.Contains("NO")).ToList();
                            break;
                        }
                    case "SFUDE":
                        {
                            registrationNoVerificationSummaryTableModels = registrationNoVerificationSummaryTableModels.Where(x => x.ScannedFileUploadDetailsID.Contains("NO")).ToList();
                            break;
                        }
                    case "ICWE":
                        {
                            registrationNoVerificationSummaryTableModels = registrationNoVerificationSummaryTableModels.Where(x => x.IsCDWritten.Contains("NO")).ToList();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                //End By Tushar on 13 jan 2023
                if (Result.registrationScanningDetailsTableModelsList != null && Result.registrationScanningDetailsTableModelsList.Count > 0)
                {
                    fileName = "RegistrationScanningDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("Registration Scanning Details");
                
                    createdExcelPath = CreateExcelForRegistrationScanningDetails(registrationNoVerificationSummaryTableModels, fileName, excelHeader, reqModel.DocumentTypeId);
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

        private string CreateExcelForRegistrationScanningDetails(List<RegistrationScanningDetailsTableModel> result, string fileName, string excelHeader,long DocumentTypeId)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                  
                    var workSheet = package.Workbook.Worksheets.Add("RegistrationScanningDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());


                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                    workSheet.Cells[4, 1, 4, 9].Merge = true;
                    workSheet.Cells[5, 1, 5, 9].Merge = true;
                    workSheet.Cells[6, 1, 6, 9].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 20;
                    workSheet.Column(3).Width = 35;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 25;
                    workSheet.Column(7).Width = 25;
                    workSheet.Column(8).Width = 25;
                    workSheet.Column(9).Width = 40;

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
                    if (DocumentTypeId != 4)
                    {
                        workSheet.Cells[7, 2].Value = "SROCode"; 
                    }else
                    {
                        workSheet.Cells[7, 2].Value = "DRCode";
                    }
                    if(DocumentTypeId == 2)
                    {
                        workSheet.Cells[7, 3].Value = "Marriage Case No";
                    }else if(DocumentTypeId == 4)
                    {
                        workSheet.Cells[7, 3].Value = "Firm Number";
                    }
                    else
                    {
                        workSheet.Cells[7, 3].Value = "Notice No";
                    }

                    if (DocumentTypeId == 2 || DocumentTypeId == 4)
                    {
                        workSheet.Cells[7, 4].Value = "Date Of Registration"; 
                    }else
                    {
                        workSheet.Cells[7, 4].Value = "Notice Issued Date";
                    }
                    workSheet.Cells[7, 5].Value = "CD Number";
                    workSheet.Cells[7, 6].Value = "Scan Master Entry";
                    workSheet.Cells[7, 7].Value = "Scanned File Upload Details Entry";
                    workSheet.Cells[7, 8].Value = "Is CD Written";
                    workSheet.Cells[7, 9].Value = "Scan File Path";


                    foreach (var items in result)
                    {
                        for (int i = 1; i < 10; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }
        



                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        if (DocumentTypeId != 4)
                        {
                            workSheet.Cells[rowIndex, 2].Value = items.SROCode;
                            workSheet.Cells[rowIndex, 3].Value = items.MarriageCaseNo;
                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 2].Value = items.DRCode;
                            workSheet.Cells[rowIndex, 3].Value = items.FirmNumber;
                        }
                    
                       // workSheet.Cells[rowIndex, 3].Value = items.MarriageCaseNo;
                        workSheet.Cells[rowIndex, 4].Value = items.DateOfRegistration_Date;
                        workSheet.Cells[rowIndex, 5].Value = items.CDNumber;
                        workSheet.Cells[rowIndex, 6].Value = items.ScanMasterID.Contains("YES")?"YES":"NO";
                        if (items.ScanMasterID.Contains("YES"))
                        {
                            workSheet.Cells[rowIndex, 6].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                        }else
                        {
                            workSheet.Cells[rowIndex, 6].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        }
                        workSheet.Cells[rowIndex, 7].Value = items.ScannedFileUploadDetailsID.Contains("YES") ? "YES" : "NO";
                        if (items.ScannedFileUploadDetailsID.Contains("YES"))
                        {
                            workSheet.Cells[rowIndex, 7].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 7].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        }
                        workSheet.Cells[rowIndex, 8].Value = items.IsCDWritten.Contains("YES") ? "YES" : "NO";
                        if (items.IsCDWritten.Contains("YES"))
                        {
                            workSheet.Cells[rowIndex, 8].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 8].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        }
                        workSheet.Cells[rowIndex, 9].Value = items.ScanFilePath;
                        workSheet.Cells[rowIndex, 9].Style.WrapText = true;


                        for (int i = 1; i < 10; i++)
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