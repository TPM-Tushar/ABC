#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   NotReadableDocController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Not Readable Documents Details.

*/
#endregion

using CustomModels.Models.Remittance.NotReadableDoc;
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

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorization]
    public class NotReadableDocController : Controller
    {
        ServiceCaller caller = null;
        [HttpGet]
        [MenuHighlight]
        // GET: Remittance/NotReadableDoc
        public ActionResult NotReadableDocView()
        {
            try
            {
                caller = new ServiceCaller("NotReadableDocAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                NotReadableDocModel reqModel = caller.GetCall<NotReadableDocModel>("NotReadableDocView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Not Readable Document View", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        public ActionResult GetNotReadableDocDetails(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("NotReadableDocAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                NotReadableDocModel reqModel = new NotReadableDocModel();
                NotReadableDocResultModel resultModel  = new NotReadableDocResultModel();
                string SROfficeID = formCollection["SROfficeID"];
                //Added By Tushar on 14 Nov 2022
                string DistrictID = formCollection["DistrictID"];
                //End By Tushar on 14 Nov 2022
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);




                //Validation
                if (string.IsNullOrEmpty(SROfficeID))
                {
                    return Json(new { success = false, errorMessage = "Please select SRO Name" }, JsonRequestBehavior.AllowGet);
                }
            
                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                //Added By Tushar on 14 Nov 2022
                reqModel.DROfficeID = Convert.ToInt32(DistrictID);
                //End By Tushar on 14 Nov 2022

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                //var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = totalNum;
                int skip = startLen;
                String errorMessage = String.Empty;
                // For Excel download
                var SroCodeEx = reqModel.SROfficeID;
                var DROfficeIDEx = reqModel.DROfficeID;
                //End for excel download

                //To get records
                resultModel = caller.PostCall<NotReadableDocModel, NotReadableDocResultModel>("GetNotReadableDocDetails", reqModel, out errorMessage);

                IEnumerable<NotReadableDocTableModel> result = resultModel.notReadableDocTableModelList;
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Not Readable Document Details." });


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
                        m.LogDateTime.ToLower().Contains(searchValue.ToLower()) ||
                        m.CDNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.LogBy.ToLower().Contains(searchValue.ToLower()) ||
                        m.Document_Type.ToLower().Contains(searchValue.ToLower()));
                        TotalCount = result.Count();
                    }
                }

                
                var gridData = result.Select(NotReadableDocTableModel => new
                {

                    srNo = NotReadableDocTableModel.srNo,

                    Registration_Number = NotReadableDocTableModel.RegistrationNumber,
                    Log_Date = NotReadableDocTableModel.LogDateTime,
                    CD_Number = NotReadableDocTableModel.CDNumber,
                    Document_Type = NotReadableDocTableModel.Document_Type,
                    Logged_By = NotReadableDocTableModel.LogBy,
                    //Added By Tushar on 20 Dec 2022
                    Stamp5DateTime = NotReadableDocTableModel.Stamp5DateTime
                    //End By Tushar on 20 Dec 2022
                });
        

                
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + SroCodeEx + "','" + DROfficeIDEx + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

              
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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Not Readable Document Details." });
            }
        }

        public ActionResult ExportNotReadableDocToExcel(string SROfficeID,string DROfficeID)
        {
            try
            {
                caller = new ServiceCaller("NotReadableDocAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("NotReadableDocDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                NotReadableDocModel reqModel = new NotReadableDocModel();
                reqModel.SROfficeID = Convert.ToInt32(SROfficeID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);
              
                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;
                NotReadableDocResultModel Result = caller.PostCall<NotReadableDocModel, NotReadableDocResultModel>("GetNotReadableDocDetails", reqModel, out errorMessage);


                if (Result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
                List<NotReadableDocTableModel> notReadableDocTableModels = Result.notReadableDocTableModelList;

                if (Result.notReadableDocTableModelList != null && Result.notReadableDocTableModelList.Count > 0)
                {
                    fileName = "NotReadableDocDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("Not Readable Document Detail");
                    createdExcelPath = CreateExcelForNotReadableDocDetails(notReadableDocTableModels, fileName, excelHeader);
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


        private string CreateExcelForNotReadableDocDetails(List<NotReadableDocTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("NotReadableDocDetails");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());


                    workSheet.Cells[1, 1, 1, 7].Merge = true;
                    workSheet.Cells[2, 1, 2, 7].Merge = true;
                    workSheet.Cells[3, 1, 3, 7].Merge = true;
                    workSheet.Cells[4, 1, 4, 7].Merge = true;
                    workSheet.Cells[5, 1, 5, 7].Merge = true;
                    workSheet.Cells[6, 1, 6, 7].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 25;
                    workSheet.Column(7).Width = 30;

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
                    workSheet.Cells[7, 2].Value = "Registration Number";
                    //workSheet.Cells[7, 3].Value = "Log Date";
                    workSheet.Cells[7, 3].Value = "Registration DateTime";
                    workSheet.Cells[7, 4].Value = "CD Number";
                    workSheet.Cells[7, 5].Value = "Document Type";
                    workSheet.Cells[7, 6].Value = "Logged By";
                    //Added By Tushar on 20 Dec 2022
                    workSheet.Cells[7, 7].Value = "Log Date";
                    //End By Tushar on 20 Dec 2022


                    foreach (var items in result)
                    {
                        for(int i=1;i<8;i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }
                        //workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                    

                      
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.RegistrationNumber;
                        //workSheet.Cells[rowIndex, 3].Value = items.LogDateTime;
                        workSheet.Cells[rowIndex, 3].Value = items.Stamp5DateTime;
                        workSheet.Cells[rowIndex, 4].Value = items.CDNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.Document_Type;
                        workSheet.Cells[rowIndex, 6].Value = items.LogBy;
                        workSheet.Cells[rowIndex, 7].Value = items.LogDateTime;



                        for (int i = 1; i < 8; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }

                        //workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                     
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
        //Added By Tushar on 14 Nov 2022
        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                string errormessage = string.Empty;
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "All" }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }
        //End By Tushar on 14 Nov 2022
    }
}