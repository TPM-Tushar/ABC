using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Models.RefundChallan;
using System.Text.RegularExpressions;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Configuration;
using System.Globalization;
using System.Security;
using System.Text;

namespace ECDataUI.Areas.RefundChallan.Controllers
{
    [KaveriAuthorization]
    public class RefundChallanApproveController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;


        [MenuHighlight]
        [HttpGet]
        public ActionResult RefundChallanApproveView()
        {
            KaveriSession.Current.OrderID = 0;
            KaveriSession.Current.DECSROCode = 0;
            int OfficeID = KaveriSession.Current.OfficeID;
            int LevelID = KaveriSession.Current.LevelID;
            long UserID = KaveriSession.Current.UserID;
            caller = new ServiceCaller("RefundChallanApproveAPIController");
            RefundChallanApproveViewModel reqModel = caller.GetCall<RefundChallanApproveViewModel>("RefundChallanApproveView", new { officeID = OfficeID, LevelID = LevelID, UserID = UserID });

            return View(reqModel);
        }

        
        [HttpPost]
        public ActionResult LoadRefundChallanApproveTable(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("RefundChallanApproveAPIController");
                int DistrictID = Convert.ToInt32(formCollection["DroCode"]);
                int SROCode = Convert.ToInt32(formCollection["SroCode"]);


                int RoleID = KaveriSession.Current.RoleID;

                List<RefundChallanApproveTableModel> reqModel = caller.GetCall<List<RefundChallanApproveTableModel>>("LoadRefundChallanApproveTable", new { DroCode = DistrictID, SROCode = SROCode, RoleID = RoleID, IsExcel = false });


                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading Refund Challan Order details.", URLToRedirect = "/Home/HomePage" });

                }
                else
                {
                    var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                    System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                    Match mtch = regx.Match((string)searchValue);
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
                                    errorMessage = "Please enter valid Search String. "
                                });
                                emptyData.MaxJsonLength = Int32.MaxValue;
                                return emptyData;
                            }
                        }
                        else
                        {
                            reqModel = reqModel.Where(m => m.DROName.ToLower().Contains(searchValue.ToLower()) ||
                            m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                            m.PartyName.ToLower().Contains(searchValue.ToLower()) ||
                            m.InstrumentNumber.ToLower().Contains(searchValue.ToLower()) ||
                            ////m.DROrderNumber.ToLower().Contains(searchValue.ToLower()) ||
                            //m.ChallanEntryStatus.ToLower().Contains(searchValue.ToLower()) ||
                            m.DRApprovalStatus.ToLower().Contains(searchValue.ToLower())
                            ).ToList();
                        }
                    }


                    var gridData = reqModel.Select(RefundChallanApproveTableModel => new
                    {
                        SrNo = RefundChallanApproveTableModel.SrNo,
                        DROName = RefundChallanApproveTableModel.DROName,
                        SROName = RefundChallanApproveTableModel.SROName,
                        DROrderNumber = RefundChallanApproveTableModel.DROrderNumber,
                        DROrderDate = RefundChallanApproveTableModel.DROrderDate,
                        InstrumentNumber = RefundChallanApproveTableModel.InstrumentNumber,
                        InstrumentDate = (Convert.ToDateTime(RefundChallanApproveTableModel.InstrumentDate)).ToShortDateString(),
                        ChallanAmount = RefundChallanApproveTableModel.ChallanAmount,
                        RefundAmount = RefundChallanApproveTableModel.RefundAmount,
                        PartyName = RefundChallanApproveTableModel.PartyName,
                        PartyMobileNumber = RefundChallanApproveTableModel.PartyMobileNumber,
                        ViewDROrder = RefundChallanApproveTableModel.ViewBtn,
                        Action = RefundChallanApproveTableModel.Action,
                        DRApprovalStatus = RefundChallanApproveTableModel.DRApprovalStatus,
                    });


                    int startLen = Convert.ToInt32(formCollection["start"]);
                    int totalNum = Convert.ToInt32(formCollection["length"]);
                    int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                    int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                    int skip = startLen;
                    int totalCount = reqModel.Count;
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                    });
                    return JsonData;
                }

            }
            catch (Exception ex)
            {
                throw;
            }

        }


        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
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

        //[HttpPost, ValidateInput(false)]
        [HttpPost]
        public ActionResult LoadAddNewRefundChallanApproveView(long RowId = 0)
        {
            try
            {
                int OfficeID = KaveriSession.Current.OfficeID;
                int LevelID = KaveriSession.Current.LevelID;

                caller = new ServiceCaller("RefundChallanApproveAPIController");
                
                RefundChallanApproveViewModel refundChallanViewModel = caller.GetCall<RefundChallanApproveViewModel>("RefundChallanApproveAddEditOrder", new { RowId = RowId, OfficeID = OfficeID, LevelID = LevelID });

                if (refundChallanViewModel == null)
                {
                    if (RowId == 0)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving existing Refund Challan Order Details.", URLToRedirect = "/Home/HomePage" });
                    }
                    else
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving existing Refund Challan Order Details.", URLToRedirect = "/Home/HomePage" });
                    }
                }

                return View("RefundChallanApproveAddEditDetails", refundChallanViewModel);

            }
            catch (Exception ex)
            {
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while retreiving existing Refund Challan Order Details." });
            }

        }

        
        [HttpPost]
        public ActionResult IsChallanNoExists(string InstrumentNumber, string InstrumentDate, long RowId)
        {
            try
            {
                caller = new ServiceCaller("RefundChallanApproveAPIController");
                
                string errorMessage = string.Empty;

                RefundChallanOrderResultModel responseModel = caller.GetCall<RefundChallanOrderResultModel>("IsChallanNoExists", new { InstrumentNumber = InstrumentNumber, InstrumentDate = InstrumentDate, RowId = RowId }, out errorMessage);
                
                if (!String.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success = false, message = errorMessage });
                }

                if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                {
                    return Json(new { success = false, message = responseModel.ErrorMessage });
                }
                else
                {
                    return Json(new { success = true, message = responseModel.ResponseMessage });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpGet]
        public ActionResult CheckifOrderNoExist(string OrderNo, long RowId)
        {
            try
            {
                caller = new ServiceCaller("RefundChallanApproveAPIController");

                bool OrderNoExistResult = caller.GetCall<bool>("CheckifOrderNoExist", new { OrderNo = OrderNo, RowId = RowId });
                if (OrderNoExistResult)
                {
                    return Json(new { success = false, message = "DR Order No already exist. Please enter unique order no." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, message = "DR Order Number Not exist." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult SaveRefundChallanApproveDetails(RefundChallanApproveViewModel viewModel)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    int RoleID = KaveriSession.Current.RoleID;
                    viewModel.OfficeID = KaveriSession.Current.OfficeID;

                    DateTime newDate;
                    bool boolDate = DateTime.TryParse(DateTime.ParseExact(viewModel.DROrderDate, "dd/MM/yyyy", null).ToString(), out newDate);
                    DateTime minDate = new DateTime(2003, 1, 1);

                    DateTime ChallanDate = Convert.ToDateTime(viewModel.InstrumentDate);
                    DateTime DROrderDate = Convert.ToDateTime(viewModel.DROrderDate);
                    var result = DateTime.Compare(ChallanDate, DROrderDate);
                 
                    if (result > 0)
                    {
                        return Json(new { success = false, message = " DR Order Date should be >= Challan Date." });
                    }
                    
                    if (boolDate)
                    {
                        if (newDate < minDate)
                        {
                            return Json(new { success = false, errormsgType = 3, message = "Please provide valid DR order date." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    

                    caller = new ServiceCaller("RefundChallanApproveAPIController");

                    string errorMessage = string.Empty;
                    RefundChallanOrderResultModel responseModel = caller.PostCall<RefundChallanApproveViewModel, RefundChallanOrderResultModel>("SaveRefundChallanApproveDetails", viewModel, out errorMessage);


                    if (!String.IsNullOrEmpty(errorMessage))
                        return Json(new { success = false, message = errorMessage });


                    if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                        return Json(new { success = false, message = responseModel.ErrorMessage });
                    else
                        return Json(new { success = true, message = responseModel.ResponseMessage });
                }
                else
                {
                    String messages = String.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
                    return Json(new { success = false, message = messages });
                }
                
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult SaveRefundChallanRejectionDetails(RefundChallanRejectionViewModel viewModel)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    int RoleID = KaveriSession.Current.RoleID;

                    caller = new ServiceCaller("RefundChallanApproveAPIController");

                    string errorMessage = string.Empty;
                    RefundChallanOrderResultModel responseModel = caller.PostCall<RefundChallanRejectionViewModel, RefundChallanOrderResultModel>("SaveRefundChallanRejectionDetails", viewModel, out errorMessage);


                    if (!String.IsNullOrEmpty(errorMessage))
                        return Json(new { success = false, message = errorMessage });

                    if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                        return Json(new { success = false, message = responseModel.ErrorMessage });

                    else
                        return Json(new { success = true, message = responseModel.ResponseMessage });
                }
                else
                {
                    String messages = String.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
                    return Json(new { success = false, message = messages });
                }

            }

            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Upload PDF File")]
        public ActionResult UploadOrdersFile()
        {
            try
            {
                //string rootPath = ConfigurationManager.AppSettings["KaveriSupportPath"];
                //string rootPath = ConfigurationManager.AppSettings["MaintaincePortalVirtualOrdersDirectoryPath"];

                caller = new ServiceCaller("RefundChallanApproveAPIController");
                HttpPostedFileBase FileBase = null;
                string arr = Request.Params["filesArray"];
                string OrderNo = Request.Params["DROrderNumber"];
                string OrderDate = Request.Params["DROrderDate"];
                string InstrumentDate = Request.Params["InstrumentDate"];
                long RowId = Convert.ToInt64(Request.Params["RowId"]);

                DateTime ChallanDate = Convert.ToDateTime(InstrumentDate);
                DateTime DROrderDate = Convert.ToDateTime(OrderDate);
                var result = DateTime.Compare(ChallanDate, DROrderDate);

                if (result > 0)
                {
                    return Json(new { success = false, message = " DR Order Date should be >= Challan Date." });
                }

                if (string.IsNullOrWhiteSpace(OrderNo))
                {
                    return Json(new { success = false, message = "Please Enter Valid DR Order No." }, JsonRequestBehavior.AllowGet);
                }

                string[] filesArray = arr.Split(',');
                string FileName = string.Empty;
                if (Request.Files.Count > 0)
                {
                    if (Request.Files.Count >= 1)
                    {
                        foreach (string sFileName in Request.Files)
                        {
                            FileBase = Request.Files[sFileName];
                            int DblExtensions = FileBase.FileName.Count(f => f == '.');
                            if (!IsPDFValidExtension(Path.GetExtension(FileBase.FileName.ToLower())))
                            {
                                return Json(new { success = false, message = "Please upload PDF file only. Error in file name" + FileBase.FileName + " Kindly select file again." }, JsonRequestBehavior.AllowGet);
                            }

                            if (!isValidContentLength(FileBase.ContentLength))
                            {
                                return Json(new { success = false, message = "File size should be less than 2 MB. Kindly upload file again." + FileBase.FileName + " Kindly select file again." }, JsonRequestBehavior.AllowGet);

                            }
                        }

                        #region Added by mayank on 13/08/2021 to check if order no .
                        int OfficeID = KaveriSession.Current.OfficeID;
                        bool OrderNoExistResult = caller.GetCall<bool>("CheckifOrderNoExist", new { OrderNo = OrderNo, RowId = RowId });

                        if (OrderNoExistResult)
                        {
                            return Json(new { success = false, errormsgType = 3, message = " DR Order No already exist. Please enter unique order no." }, JsonRequestBehavior.AllowGet);
                        }

                        DateTime newDate;
                        bool boolDate = DateTime.TryParse(DateTime.ParseExact(OrderDate, "dd/MM/yyyy", null).ToString(), out newDate);
                        DateTime minDate = new DateTime(2003, 1, 1);
                        if (boolDate)
                        {
                            if (newDate < minDate)
                            {
                                return Json(new { success = false, errormsgType = 3, message = "Please provide valid DR order date." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            throw new Exception("Error Occured while Converting Date.");
                        }
                        #endregion


                        //int currentOrderID = KaveriSession.Current.OrderID;

                        RefundChallanDROrderResultModel dROrderFilePathResultModel = caller.GetCall<RefundChallanDROrderResultModel>("GenerateNewOrderID", new { OfficeID = OfficeID, RowId = RowId });


                        if (dROrderFilePathResultModel != null)
                        {
                            foreach (string sFileName in Request.Files)
                            {
                                FileBase = Request.Files[sFileName];

                                FileName = FileBase.FileName.Split('.')[0];

                                byte[] fileData = null;
                                string FileDataStr = string.Empty;
                                using (var binaryReader = new BinaryReader(Request.Files[sFileName].InputStream))
                                {
                                    fileData = binaryReader.ReadBytes(Request.Files[sFileName].ContentLength);

                                }

                                string completeReletiveFilePath = dROrderFilePathResultModel.RelativeFilePath + "\\" + dROrderFilePathResultModel.FileName;

                                string filePathToSave = dROrderFilePathResultModel.rootPath + completeReletiveFilePath;

                                if (fileData != null)
                                {

                                    if (!Directory.Exists(dROrderFilePathResultModel.RelativeFilePath))
                                        Directory.CreateDirectory(dROrderFilePathResultModel.rootPath + "\\" + dROrderFilePathResultModel.RelativeFilePath);

                                    System.IO.File.WriteAllBytes(filePathToSave, fileData);   //Write File

                                    var jsonResult = Json(new { success = true, message = "File uploaded and Saved Successfully.", filePath = filePathToSave, relativeFilePath = completeReletiveFilePath, orderFileName = dROrderFilePathResultModel.FileName});
                                    jsonResult.MaxJsonLength = int.MaxValue;

                                    return jsonResult;
                                }
                                else
                                {
                                    return Json(new { success = false, message = "Unable to save the selected file.", filePath = "" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Only one file is allowed." });
                    }
                }
                else
                {
                    return Json(new { success = true, message = "", FileDataStr = "" });
                }
                return Json(new { success = false, message = "Unable to Encrypt the selected file." });



            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return Json(new { success = false, message = e.GetBaseException().Message });
            }
        }

        
        private bool IsPDFValidExtension(string extension)
        {
            return (extension.Equals(".pdf") || extension.Equals(".PDF"));
        }


        private bool isValidContentLength(int ContentLength)
        {
            try
            {
                return ((ContentLength / 1024) / 1024) < 2;  //2 Mb
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }


        [HttpGet]
        public dynamic ViewBtnClickOrderTable(long RowId)
        {
            try
            {
                caller = new ServiceCaller("RefundChallanApproveAPIController");
                RefundChallanDROrderResultModel resModel = caller.GetCall<RefundChallanDROrderResultModel>("ViewBtnClickOrderTable", new { RowId = RowId });
                return File(resModel.refundChallanApproveFileBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);

                if (ex.Message.Contains("Could not find file"))
                {
                    return "Could not find file";
                }
                // Input string.
                const string input = "Due to some techinical problem this document cannot be viewed right now.Please try again later.";

                // Invoke GetBytes method.
                byte[] array = Encoding.ASCII.GetBytes(input);

                return File(array, "application/pdf");
            }

        }


        [HttpGet]
        public ActionResult ExportRefundChallanOrderDetailsToExcel(string SroID, string DistrictID, string SroName, string DroName)
        {

            try
            {
                caller = new ServiceCaller("RefundChallanApproveAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "Refund_Challan_Approve" + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                int RoleID = KaveriSession.Current.RoleID;

                List<RefundChallanApproveTableModel> ResModel = caller.GetCall<List<RefundChallanApproveTableModel>>("LoadRefundChallanApproveTable", new { DroCode = DistrictID, SROCode = SroID, RoleID = RoleID, IsExcel = true });


                if (ResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Refund Challan Note Details.", URLToRedirect = "/Home/HomePage" });
                }

                string excelHeader = string.Format("Refund Challan Approve Details.");

                string createdExcelPath = CreateExcelOrderDetails(ResModel, fileName, excelHeader, DroName, SroName);
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel.", URLToRedirect = "/Home/HomePage" });
            }
        }


        private string CreateExcelOrderDetails(List<RefundChallanApproveTableModel> refundChallanTableModelList, string fileName, string excelHeader, string DroName, string SroName = "NA")
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("DR Order Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + DroName;
                    workSheet.Cells[3, 1].Value = "SRO : " + SroName;
                    //workSheet.Cells[4, 1].Value = "Log Type : " + SelectedLogType;

                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (refundChallanTableModelList.Count());

                    workSheet.Cells[1, 1, 1, 11].Merge = true;
                    workSheet.Cells[2, 1, 2, 11].Merge = true;
                    workSheet.Cells[3, 1, 3, 11].Merge = true;
                    workSheet.Cells[4, 1, 4, 11].Merge = true;
                    workSheet.Cells[5, 1, 5, 11].Merge = true;
                    workSheet.Cells[6, 1, 6, 11].Merge = true;
                    //workSheet.Cells[6, 1, 6, 13].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 40;
                    workSheet.Column(7).Width = 40;
                    workSheet.Column(8).Width = 50;
                    workSheet.Column(9).Width = 40;
                    workSheet.Column(10).Width = 40;
                    workSheet.Column(11).Width = 40;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(6).Style.WrapText = true;


                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "DRO Name";
                    workSheet.Cells[7, 3].Value = "SRO Name";
                    workSheet.Cells[7, 4].Value = "DR Order Number";
                    workSheet.Cells[7, 5].Value = "DR Order Date(DD-MM-YYYY)";
                    workSheet.Cells[7, 6].Value = "Challan Number";
                    workSheet.Cells[7, 7].Value = "Challan Date (DD-MM-YYYY)";
                    workSheet.Cells[7, 8].Value = "Challan Amount (in Rs.)";
                    workSheet.Cells[7, 9].Value = "Refund Amount (in Rs.)";
                    workSheet.Cells[7, 10].Value = "Party Name";
                    workSheet.Cells[7, 11].Value = "Party Mobile Number";


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    ////workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";


                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    foreach (var items in refundChallanTableModelList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 10].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 11].Style.Font.Name = "KNB-TTUmaEN";

                        //workSheet.Cells[rowIndex, 1].Value = items.SNo;]

                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.DROName;
                        workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        workSheet.Cells[rowIndex, 4].Value = items.DROrderNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.DROrderDate;
                        workSheet.Cells[rowIndex, 6].Value = items.InstrumentNumber;
                        workSheet.Cells[rowIndex, 7].Value = items.InstrumentDate;
                        workSheet.Cells[rowIndex, 8].Value = items.ChallanAmount;
                        workSheet.Cells[rowIndex, 9].Value = items.RefundAmount;
                        workSheet.Cells[rowIndex, 10].Value = items.PartyName;
                        workSheet.Cells[rowIndex, 11].Value = items.PartyMobileNumber;



                        workSheet.Cells[rowIndex, 1].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 9].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 10].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 11].Style.WrapText = true;


                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 9].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        rowIndex++;
                    }

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 11])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
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


        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }


        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult FinalizeApproveDROrder(long RowId)
        {
            try
            {
                long UserId = KaveriSession.Current.UserID;
                int currentOrderID = KaveriSession.Current.OrderID;
                caller = new ServiceCaller("RefundChallanApproveAPIController");


                string DRfinalizeDECResp = caller.GetCall<string>("FinalizeApproveDROrder", new { RowId = RowId, UserId = UserId });


                if (DRfinalizeDECResp == string.Empty)
                {
                    return Json(new { success = true, message = "This Refund Challan entry has been successfully Finalized." }, JsonRequestBehavior.AllowGet);
                }
                else if (DRfinalizeDECResp == null || DRfinalizeDECResp == "NULL")
                {
                    return Json(new { success = false, message = "Error Occured while finalizing Refund Challan Order Details .Please contact admin." }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { success = false, message = DRfinalizeDECResp }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult FinalizeRejectDROrder(long RowId)
        {
            try
            {
                long UserId = KaveriSession.Current.UserID;
                int currentOrderID = KaveriSession.Current.OrderID;
                caller = new ServiceCaller("RefundChallanApproveAPIController");

                string DRfinalizeDECResp = caller.GetCall<string>("FinalizeRejectDROrder", new { RowId = RowId, UserId = UserId });

                if (DRfinalizeDECResp == string.Empty)
                {
                    return Json(new { success = true, message = "This Refund Challan entry has been successfully Finalized." }, JsonRequestBehavior.AllowGet);
                }
                else if (DRfinalizeDECResp == null || DRfinalizeDECResp == "NULL")
                {
                    return Json(new { success = false, message = "Error Occured while finalizing Refund Challan Order Details. Please contact admin." }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { success = false, message = DRfinalizeDECResp }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult DeleteCurrentOrderFile(long RowId)
        {
            try
            {
                RefundChallanApproveViewModel viewModel = new RefundChallanApproveViewModel();
                viewModel.RowId = RowId;
                if(RowId !=0)
                {
                    caller = new ServiceCaller("RefundChallanApproveAPIController");
                    RefundChallanOrderResultModel resultModel = caller.PostCall<RefundChallanApproveViewModel,RefundChallanOrderResultModel> ("DeleteCurrentOrderFile", viewModel);

                    if (string.IsNullOrEmpty(resultModel.ErrorMessage))
                        return Json(new { success = true, message = resultModel.ResponseMessage, RowId = RowId }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { success = false, message = resultModel.ErrorMessage, RowId = RowId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Error in deleting file. Please initiate the edit order process again.", RowId = RowId }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}