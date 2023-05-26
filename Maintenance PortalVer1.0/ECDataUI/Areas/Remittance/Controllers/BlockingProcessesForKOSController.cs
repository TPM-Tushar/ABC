using CustomModels.Models.Remittance.BlockingProcessesForKOS;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;

namespace ECDataUI.Areas.Remittance.Controllers
{
    public class BlockingProcessesForKOSController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        [MenuHighlightAttribute]
        public ActionResult GetBlockingProcessesForKOSView()
        {
            return View("GetBlockingProcessesForKOSView");
        }


        [HttpPost]
        [EventAuditLogFilter(Description = "Get Blocking Processes For KOS Report")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetBlockingProcessForKOSDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("BlockingProcessesForKOSAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });


                BlocingProcessesForKOSWrapperModel resModel = new BlocingProcessesForKOSWrapperModel();
                BlockingProcessesForKOSReqModel reqModel = new BlockingProcessesForKOSReqModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;



                resModel = caller.PostCall<BlockingProcessesForKOSReqModel, BlocingProcessesForKOSWrapperModel>("GetBlockingProcessForKOSDetails", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting blocking processes for KOS Details." });
                }
                IEnumerable<BlockingProcessesForKOSModel> result = resModel.blockingProcessesModelsList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting blocking processes for KOS Details." });
                }
                int totalCount = resModel.blockingProcessesModelsList.Count;

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
                         m.TEXT.ToLower().Contains(searchValue.ToLower()) || m.command.ToLower().Contains(searchValue.ToLower())
                         || m.blocking_session_id.ToLower().Contains(searchValue.ToLower())

                        );
                        totalCount = result.Count();
                    }
                }
                //  Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }


                var gridData = result.Select(BlockingProcessesDetailModel => new
                {
                    SrNo = BlockingProcessesDetailModel.SrNo,
                    session_id = BlockingProcessesDetailModel.session_id,
                    command = BlockingProcessesDetailModel.command,
                    blocking_session_id = BlockingProcessesDetailModel.blocking_session_id,
                    wait_type = BlockingProcessesDetailModel.wait_type,
                    wait_time = BlockingProcessesDetailModel.wait_time,
                    wait_resource = BlockingProcessesDetailModel.wait_resource,
                    TEXT = BlockingProcessesDetailModel.TEXT,
                    DateTime = BlockingProcessesDetailModel.DateTime,                   

                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(), //gridData.ToArray(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = resModel.TotalRecords,
                        ExcelDownloadBtn = ExcelDownloadBtn,

                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Blocking Processes for KOS" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadBlockingProcessForKOSDetails()
        {
            string errorMessage = string.Empty;
            caller = new ServiceCaller("BlockingProcessesForKOSAPIController");
            CommonFunctions objCommon = new CommonFunctions();
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            BlocingProcessesForKOSWrapperModel resModel = new BlocingProcessesForKOSWrapperModel();
            BlockingProcessesForKOSReqModel reqModel = new BlockingProcessesForKOSReqModel();
            reqModel.IsExcel = true;
            resModel = caller.PostCall<BlockingProcessesForKOSReqModel, BlocingProcessesForKOSWrapperModel>("GetBlockingProcessForKOSDetails", reqModel, out errorMessage);

            if (resModel == null)
            {
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting blocking processes details", URLToRedirect = "/Home/HomePage" });
            }

            string excelHeader = string.Format("Blocking Processes For KOS Report");
            string createdExcelPath = CreateExcel(resModel);

            byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
            objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
            return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "Blocking Processes for KOS_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
        }


        private string CreateExcel(BlocingProcessesForKOSWrapperModel ResModel)
        {
            string fileName = string.Format("BlockingProcessesForKOS" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Blocking Processes For KOS Report");
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1].Value = "Blocking Processes for KOS";

                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[3, 1].Value = "Total Records : " + (ResModel.blockingProcessesModelsList.Count());
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


                    foreach (var items in ResModel.blockingProcessesModelsList)
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