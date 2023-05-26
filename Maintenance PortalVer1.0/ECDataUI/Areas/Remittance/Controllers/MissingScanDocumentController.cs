using CustomModels.Models.Remittance.MissingSacnDocument;
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
    public class MissingScanDocumentController : Controller
    {
        ServiceCaller caller = null;
        [HttpGet]
        [MenuHighlight]
        // GET: Remittance/MissingSacnDocument
        public ActionResult MissingScanDocumentView()
        {

           try
            {
                caller = new ServiceCaller("MissingScanDocumentAPIController");

                MissingScanDocumentModel reqModel = caller.GetCall<MissingScanDocumentModel>("MissingScanDocumentView");
                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Missing Sacn Document Details View", URLToRedirect = "/Home/HomePage" });
            }
        }
        [HttpPost]
        public ActionResult GetMissingScanDocumentDetails(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("MissingScanDocumentAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                MissingScanDocumentModel reqModel = new MissingScanDocumentModel();
                MissingScanDocumentResModel ResultModel = new MissingScanDocumentResModel();
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROfficeID = formCollection["SROfficeID"];
                var DocumentTypeId = formCollection["DocumentTypeId"];
           
                var C_NA_L_A = formCollection["C_NA_L_A"];
                var C_NA_L_NA = formCollection["C_NA_L_NA"];
                var C_A_L_NA = formCollection["C_A_L_NA"];
              
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                //Validation
                if (string.IsNullOrEmpty(SROfficeID))
                {
                    return Json(new { success = false, errorMessage = "Please Select SRO Name" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(DocumentTypeId) || DocumentTypeId == "0")
                {
                    return Json(new { success = false, errorMessage = "Please Select Document Type" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(fromDate))
                {
                    return Json(new { success = false, errorMessage = "Please Select From Date" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    return Json(new { success = false, errorMessage = "Please Select To Date" }, JsonRequestBehavior.AllowGet);
                }
                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                reqModel.DateTime_Date = Convert.ToDateTime(fromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                reqModel.DocumentTypeId = Convert.ToInt32(DocumentTypeId);

                if (C_NA_L_A == "4")
                {
                    reqModel.IsErrorTypecheck = true;
                    reqModel.ErrorCode = Convert.ToInt32(C_NA_L_A);
                }
                else if (C_NA_L_NA == "5")
                {
                    reqModel.IsErrorTypecheck = true;
                    reqModel.ErrorCode = Convert.ToInt32(C_NA_L_NA);
                }
                else if (C_A_L_NA == "6")
                {
                    reqModel.IsErrorTypecheck = true;
                    reqModel.ErrorCode = Convert.ToInt32(C_A_L_NA);
                }
                else
                {
                    reqModel.IsErrorTypecheck = false;
                    reqModel.ErrorCode = 0;
                }
        
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
               

                int pageSize = totalNum;
                int skip = startLen;
                String errorMessage = String.Empty;
                // For Excel download
                var SroCodeEx = reqModel.SROfficeID;
                var DateTime_DateEx = reqModel.DateTime_Date;
                var DateTime_ToDateEX = reqModel.DateTime_ToDate;
                var DocumentTypeIdEx = reqModel.DocumentTypeId;
  
                var IsErrorTypecheckEx = reqModel.IsErrorTypecheck;
                var ErrorCodeEx = reqModel.ErrorCode;
                //End for excel download


                ResultModel = caller.PostCall<MissingScanDocumentModel, MissingScanDocumentResModel>("GetMissingScanDocumentDetails", reqModel, out errorMessage);
                IEnumerable<MissingScanDocumentTableModel> result = ResultModel.MissingScanDocumentTableModelList;
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Missing Scan Document Details." });


                }
                int TotalCount = result.Count();
                //



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
                        m.SRO_Name.ToLower().Contains(searchValue.ToLower()) ||
                        m.C_Stamp5DateTime.ToLower().Contains(searchValue.ToLower()) ||
                        m.C_FRN.ToLower().Contains(searchValue.ToLower()) ||
                        m.C_ScannedFileName.ToLower().Contains(searchValue.ToLower()) ||
                        //m.L_Stamp5DateTime.ToLower().Contains(searchValue.ToLower()) ||
                        m.L_FRN.ToLower().Contains(searchValue.ToLower()) ||
                        m.L_ScannedFileName.ToLower().Contains(searchValue.ToLower()) ||

                        m.C_CDNumber.ToLower().Contains(searchValue.ToLower()) ||
                  
                    
                        m.L_CDNumber.ToLower().Contains(searchValue.ToLower()));
                        TotalCount = result.Count();
                    }
                }


                //
                var gridData = result.Select(MissingScanDocumentTableModel => new
                {

                    srNo = MissingScanDocumentTableModel.srNo,

                   
                    SRO_Name = MissingScanDocumentTableModel.SRO_Name,
                    Central_Stamp5DateTime = MissingScanDocumentTableModel.C_Stamp5DateTime,
                    Central_FinalRegistrationNumber = MissingScanDocumentTableModel.C_FRN,
                    Central_ScannedFileName = MissingScanDocumentTableModel.C_ScannedFileName,
                    Local_Stamp5DateTime = MissingScanDocumentTableModel.L_Stamp5DateTime,
                    Local_FinalRegistrationNumber = MissingScanDocumentTableModel.L_FRN,
                    Local_ScannedFileName = MissingScanDocumentTableModel.L_ScannedFileName,
                   
                    Central_CDNumber = MissingScanDocumentTableModel.C_CDNumber,
                    Local_CDNumber = MissingScanDocumentTableModel.L_CDNumber
          
                });
 

              
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + SroCodeEx + "','" + fromDate + "','" + ToDate + "','" + DocumentTypeIdEx + "','" + IsErrorTypecheckEx + "','" + ErrorCodeEx + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

            
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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Missing Scan Document Details." });
            }

        }


        public ActionResult ExportMissingScanDocumentDetailsToExcel(int SroCode, string fromDate, string ToDate, int DocumentTypeId,string IsErrorTypecheck, string ErrorCode)
        {
            try
            {
                caller = new ServiceCaller("MissingScanDocumentAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("MissingScanDocumentDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                MissingScanDocumentModel reqModel = new MissingScanDocumentModel();
                MissingScanDocumentResModel ResultModel = new MissingScanDocumentResModel();
                reqModel.SROfficeID = SroCode;
                reqModel.DateTime_Date = Convert.ToDateTime(fromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                reqModel.DocumentTypeId = DocumentTypeId;
       
                //
                reqModel.IsErrorTypecheck = Convert.ToBoolean(IsErrorTypecheck);
                reqModel.ErrorCode = Convert.ToInt32(ErrorCode);
                //
                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;

                ResultModel = caller.PostCall<MissingScanDocumentModel, MissingScanDocumentResModel>("GetMissingScanDocumentDetails", reqModel, out errorMessage);
                List<MissingScanDocumentTableModel> Result = ResultModel.MissingScanDocumentTableModelList;
                if (Result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
           
             
                    fileName = "MissingScanDocumentDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("Missing Scan Document Details");
                   
                    //createdExcelPath = CreateExcelForMissingScanDocumentDetails(Result, fileName, excelHeader);
                      createdExcelPath = CreateExcelForMissingScanDocumentDetails(Result, fileName, excelHeader, reqModel.DocumentTypeId);

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

        private string CreateExcelForMissingScanDocumentDetails(List<MissingScanDocumentTableModel> result, string fileName, string excelHeader,int DocumentTypeId)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("MissingScanDocumentDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());


                    workSheet.Cells[1, 1, 1, 10].Merge = true;
                    workSheet.Cells[2, 1, 2, 10].Merge = true;
                    workSheet.Cells[3, 1, 3, 10].Merge = true;
                    workSheet.Cells[4, 1, 4, 10].Merge = true;
                    workSheet.Cells[5, 1, 5, 10].Merge = true;
                    workSheet.Cells[6, 1, 6, 10].Merge = true;


                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 10;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 40;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 40;
                    workSheet.Column(8).Width = 40;
                    workSheet.Column(9).Width = 30;

                    workSheet.Column(10).Width = 30;
           

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
                    workSheet.Cells[7, 2].Value = "SRO Name";
                    if (DocumentTypeId == 1)
                    {
                        workSheet.Cells[7, 3].Value = "Central_FinalRegistrationNumber";
                        workSheet.Cells[7, 4].Value = "Central_ScannedFileName";
                        workSheet.Cells[7, 5].Value = "Central_Stamp5DateTime";
                        workSheet.Cells[7, 6].Value = "Central_CDNumber";
                        workSheet.Cells[7, 7].Value = "Local_FinalRegistrationNumber";
                        workSheet.Cells[7, 8].Value = "Local_ScannedFileName";
                        workSheet.Cells[7, 9].Value = "Local_Stamp5DateTime";
                        workSheet.Cells[7, 10].Value = "Local_CDNumber"; 
                    }else
                    {
                        workSheet.Cells[7, 3].Value = "Central_MarriageCaseNo";
                        workSheet.Cells[7, 4].Value = "Central_ScannedFileName";
                        workSheet.Cells[7, 5].Value = "Central_DateOfRegistration";
                        workSheet.Cells[7, 6].Value = "Central_CDNumber";
                        workSheet.Cells[7, 7].Value = "Local_MarriageCaseNo";
                        workSheet.Cells[7, 8].Value = "Local_ScannedFileName";
                        workSheet.Cells[7, 9].Value = "Local_DateOfRegistration";
                        workSheet.Cells[7, 10].Value = "Local_CDNumber";
                    }
        
          



                    foreach (var items in result)
                    {
                        for (int i = 1; i < 11; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }


              
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SRO_Name;
                        workSheet.Cells[rowIndex, 3].Value = items.C_FRN;
                        workSheet.Cells[rowIndex, 4].Value = items.C_ScannedFileName;
                        workSheet.Cells[rowIndex, 5].Value = items.C_Stamp5DateTime;
                 
                        workSheet.Cells[rowIndex, 6].Value = items.C_CDNumber;
                        workSheet.Cells[rowIndex, 7].Value = items.L_FRN;
                        workSheet.Cells[rowIndex, 8].Value = items.L_ScannedFileName;
                        workSheet.Cells[rowIndex, 9].Value = items.L_Stamp5DateTime;
                        workSheet.Cells[rowIndex, 10].Value = items.L_CDNumber;
            





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

        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }
    }
}