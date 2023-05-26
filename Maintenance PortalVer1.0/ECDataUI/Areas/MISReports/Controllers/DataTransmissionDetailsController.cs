using CustomModels.Models.MISReports.DataTransmissionDetails;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]

    public class DataTransmissionDetailsController : Controller
    {
        ServiceCaller caller = null;

        public ActionResult DataTransmissionDetailsView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DataTransmissionReport;               
                
                //DataTransReqModel dataTransReqModel = new DataTransReqModel();
                //dataTransReqModel.DataBaseList = new List<SelectListItem>();


                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("DataTransmissionDetailsAPIController");
                //TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                //caller.HttpClient.Timeout = objTimeSpan;
                DataTransReqModel reqModel = caller.GetCall<DataTransReqModel>("DataTransmissionDetailsView", new { OfficeID = OfficeID });
                reqModel.DataBaseList = new List<SelectListItem>();

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Anywhere Registration Statistics View", URLToRedirect = "/Home/HomePage" });
                }
                
                // ECDATA
                SelectListItem item1 = new SelectListItem() { Text = "ECDATA", Value = "ECDATA" };
                reqModel.DataBaseList.Add(item1);

                return View(reqModel);



            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Data Transmission Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadDataTransmissionDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("DataTransmissionDetailsAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects
                string dbname = formCollection["DBName"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                
                if (string.IsNullOrEmpty(SROOfficeListID))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select Any SRO."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                Regex regx1 = new Regex("^[0-9]*$");
                Match mtchDistrict = regx1.Match(SROOfficeListID);
                if (!mtchDistrict.Success)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select Any SRO."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

               int SROID = Convert.ToInt32(SROOfficeListID);

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                //int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                DataTransReqModel reqModel = new DataTransReqModel();
                reqModel.StartLen = startLen;
                reqModel.TotalNum = totalNum;
                reqModel.DBName = dbname;
                reqModel.SROID = SROID;
                reqModel.SearchValue = searchValue;


                DataTransWrapperModel resModel = caller.PostCall<DataTransReqModel, DataTransWrapperModel>("LoadDataTransmissionDetails", reqModel, out errorMessage);

                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Data Transmission Details" });
                }
                IEnumerable<DataTransDetailsModel> result = resModel.DataTransDetailsModelList;
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Data Transmission Details" });
                }
                int totalCount = resModel.DataTransDetailsModelList.Count;

                

                //Sorting
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
                    {//Modified by Raman on 28-06-2019
                        result = result.Where(m => m.TableName.ToLower().Contains(searchValue.ToLower()) ||
                        m.Count.ToString().Contains(searchValue.ToLower())
                         );
                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(DataTransDetailsModel => new
                {

                    SerialNumber = DataTransDetailsModel.SerialNumber,
                    TableName = DataTransDetailsModel.TableName,
                    rowCount = DataTransDetailsModel.Count,
                    //StampDuty = IndexIIReportsDetailsModel.StampDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    //RegistrationFee = IndexIIReportsDetailsModel.RegistrationFee.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    //Documents = IndexIIReportsDetailsModel.Documents,
                    //TotalAmount = IndexIIReportsDetailsModel.TotalAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))
                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + dbname +"','"+ SROID + "')><i style = 'padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as Excel</button>";


                var JsonData = Json(new
                {
                });

                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        //data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),                        
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = resModel.TotalRecords,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Data Transmission Details" });
            }
        }


        #region Excel
        public ActionResult ExportDataTransmissionDetailsToExcel(string DBName,string SROID)
        {
            try
            {
                caller = new ServiceCaller("DataTransmissionDetailsAPIController");
                //TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                //caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "DataTransmissionDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                DataTransReqModel model = new DataTransReqModel
                {
                    DBName = DBName,
                    SROID = Convert.ToInt32(SROID)
            };                              

                DataTransWrapperModel ResModel = new DataTransWrapperModel();       
                model.IsExcel = true;

                ResModel = caller.PostCall<DataTransReqModel, DataTransWrapperModel>("LoadDataTransmissionDetails", model, out errorMessage);
                if (ResModel == null ||  ResModel.DataTransDetailsModelList == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Getting Data Transmission Details.." }, JsonRequestBehavior.AllowGet);
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();
                //}
                string excelHeader = string.Format("Data Transmission Details");
                string createdExcelPath = CreateExcel(ResModel, DBName,fileName,excelHeader);
                // string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader);

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
               
        private string CreateExcel(DataTransWrapperModel ResModel,string DBName,string fileName,string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Data Transmission Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                   
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;                   
                    workSheet.Cells[3, 1].Value = "Total Records : " + (ResModel.DataTransDetailsModelList.Count());
                    workSheet.Cells[4, 1].Value = "SRO Name : " + ResModel.SroName;
                    workSheet.Cells[1, 1, 1, 15].Merge = true;
                    workSheet.Cells[2, 1, 2, 3].Merge = true;
                    workSheet.Cells[3, 1, 3, 3].Merge = true;
                    workSheet.Cells[4, 1, 4, 3].Merge = true;
                    
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                   
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    
                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[6, 1].Value = "S.No";
                    workSheet.Cells[6, 2].Value = "Table Name";
                    workSheet.Cells[6, 3].Value = "Row Count";
                   
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";

                    //workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.DataTransDetailsModelList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 1].Value = items.SerialNumber;
                        workSheet.Cells[rowIndex, 2].Value = items.TableName;
                        workSheet.Cells[rowIndex, 3].Value = items.Count;
                      
                        //workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                    }
                                                          
                    //workSheet.Row(rowIndex).Style.Font.Bold = true;

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex-1), 3])
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
                    //using (ExcelRange Rng = workSheet.Cells[7, 1, 7, 9])
                    //{
                    //    Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
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
        #endregion

    }
}