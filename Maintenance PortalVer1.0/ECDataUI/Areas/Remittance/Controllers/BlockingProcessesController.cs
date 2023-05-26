using CustomModels.Models.Remittance.BlockingProcesses;
using ECDataUI.Common;
using ECDataUI.Filters;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
    public class BlockingProcessesController : Controller
    {

        ServiceCaller caller = null;
        string errormessage = string.Empty;
        // GET: Remittance/BlockingProcesses
       

        [MenuHighlightAttribute]

        public ActionResult GetBlockingProcessesView()
        {

            return View("BlockingProcessesView");
        }
        public ActionResult DownloadBlockingProcessDetails()
        {

            caller = new ServiceCaller("BlockingProcessesAPIController");
                CommonFunctions objCommon = new CommonFunctions();
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            BlockingProcessWrapperModel reqModel = caller.GetCall<BlockingProcessWrapperModel>("GetBlockingProcessDetails");
            if (reqModel == null)
            {
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Valuation Difference View", URLToRedirect = "/Home/HomePage" });
            }

            string excelHeader = string.Format("Valuation Analysis Report");
            string createdExcelPath = CreateExcel(reqModel);
             
            byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
            objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
            return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "Blocking Processes_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");


        }


        private string CreateExcel(BlockingProcessWrapperModel ResModel)
        {
                string fileName = string.Format("BlockingProcesses" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Valuation Analysis Report");
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1].Value = "Blocking Processes";
                     
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[3, 1].Value = "Total Records : " + (ResModel.BlockingProcessList.Count());
                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                 

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 40;
                    workSheet.Column(4).Width = 40;
                    //workSheet.Column(5).Width = 30;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 35;
                    workSheet.Column(8).Width = 53;
                    

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
              
                    workSheet.Row(5).Style.Font.Bold = true;
                   
                    

                    int rowIndex = 6;
                    //workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Row(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[5, 1].Value = "session_id ";
                    workSheet.Cells[5, 2].Value = "command";
                    workSheet.Cells[5, 3].Value = "blocking_session_id";
                    workSheet.Cells[5, 4].Value = "wait type";
                    workSheet.Cells[5, 5].Value = "wait time";
                    workSheet.Cells[5, 6].Value = "wait resource";
                    workSheet.Cells[5, 7].Value = "TEXT";
                    workSheet.Cells[5, 8].Value = "DateTime";
                                    

                  

                    //workSheet.Cells[7, 12].Value = "Click to view document";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    foreach (var items in ResModel.BlockingProcessList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";
                       


                        
                
                        workSheet.Cells[rowIndex, 1].Value = items.session_id;
                        workSheet.Cells[rowIndex, 2].Value = items.command;
                        workSheet.Cells[rowIndex, 3].Value = items.blocking_session_id; // items.RegisteredPerSquareFeetRate;
                        workSheet.Cells[rowIndex, 4].Value = items.wait_type;

                        //workSheet.Cells[rowIndex, 5].Value = items.AppNo;
                        workSheet.Cells[rowIndex, 5].Value = items.wait_time;
                        workSheet.Cells[rowIndex, 6].Value = items.wait_resource;
                        workSheet.Cells[rowIndex, 7].Value = items.TEXT;
                        workSheet.Cells[rowIndex, 8].Value = items.DateTime.ToString();
                       
                        //workSheet.Cells[rowIndex, 12].Value = items.ClickToViewDocument;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                      
                        rowIndex++;
                    }
                    using (ExcelRange Rng = workSheet.Cells[5, 1, (rowIndex - 1), 8])
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
                    //using (ExcelRange Rng = workSheet.Cells[7, 1, 7, 10])
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


        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }
    }
}