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
    public class RefundChallanController : Controller
    {

        ServiceCaller caller = null;
        string errormessage = string.Empty;
        
        [MenuHighlight]
        [HttpGet]
        public ActionResult RefundChallanView()
        {
            KaveriSession.Current.OrderID = 0;
            KaveriSession.Current.DECSROCode = 0;
            int OfficeID = KaveriSession.Current.OfficeID;
            long UserID = KaveriSession.Current.UserID;
            int LevelID = KaveriSession.Current.LevelID;

            caller = new ServiceCaller("RefundChallanAPIController");
            RefundChallanViewModel reqModel = caller.GetCall<RefundChallanViewModel>("RefundChallanView", new { officeID = OfficeID, LevelID = LevelID});

            return View(reqModel);
        }


        [HttpPost]
        public ActionResult LoadRefundChallanDetailsTable(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("RefundChallanAPIController");
                int DistrictID = Convert.ToInt32(formCollection["DroCode"]);
                int SROCode = Convert.ToInt32(formCollection["SroCode"]);

                int RoleID = KaveriSession.Current.RoleID;

              RefundChallanResultModel refundChallanResultModel = new RefundChallanResultModel();

                var result = caller.GetCall<RefundChallanResultModel>("LoadRefundChallanDetailsTable", new { DroCode = DistrictID, SROCode = SROCode, RoleID = RoleID });

                List<RefundChallanTableModel> reqModel = result.refundChallanTableList;

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading Refund Challan details table.", URLToRedirect = "/Home/HomePage" });
                }
                //else
                // {
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
                                errorMessage = "Please enter valid Search String "
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
                            //m.DROrderNumber.ToLower().Contains(searchValue.ToLower()) ||
                            m.ChallanEntryStatus.ToLower().Contains(searchValue.ToLower()) ||
                            m.DRApprovalStatus.ToLower().Contains(searchValue.ToLower())
                            ).ToList();
                    }
                }
            

                    var gridData = reqModel.Select(RefundChallanTableModel => new
                    {
                        SrNo = RefundChallanTableModel.SrNo,
                        DROName = RefundChallanTableModel.DROName,
                        SROName = RefundChallanTableModel.SROName,
                        DROrderNumber = RefundChallanTableModel.DROrderNumber,
                        DROrderDate = RefundChallanTableModel.DROrderDate,
                        InstrumentNumber = RefundChallanTableModel.InstrumentNumber,
                        InstrumentDate = (Convert.ToDateTime(RefundChallanTableModel.InstrumentDate)).ToShortDateString(),
                        ChallanAmount = RefundChallanTableModel.ChallanAmount,
                        RefundAmount = RefundChallanTableModel.RefundAmount,
                        PartyName = RefundChallanTableModel.PartyName,
                        PartyMobileNumber = RefundChallanTableModel.PartyMobileNumber,
                        ViewDROrder = RefundChallanTableModel.ViewBtn,
                        Action = RefundChallanTableModel.Action,
                        ChallanEntryStatus = RefundChallanTableModel.ChallanEntryStatus,
                        DRApprovalStatus = RefundChallanTableModel.DRApprovalStatus,

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
                        //IsSROrDRLogin  = result.IsSROrDRLogin,       
                    });
                    return JsonData;
               // }

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
        public ActionResult LoadAddNewRefundChallanView(long RowId = 0)
        {
            try
            {
                caller = new ServiceCaller("RefundChallanAPIController");
                
                RefundChallanViewModel refundChallanViewModel = caller.GetCall<RefundChallanViewModel>("RefundChallanAddEditDetails", new { RowId = RowId});
                
                if (refundChallanViewModel == null)
                {
                    if (RowId == 0)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Add New Refund Challan Details View.", URLToRedirect = "/Home/HomePage" });
                    }
                    else
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving existing Refund Challan Details View.", URLToRedirect = "/Home/HomePage" });
                    }
                }

                return View("RefundChallanAddEditDetails", refundChallanViewModel);

            }
            catch (Exception ex)
            {
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while loading Add New Refund Challan Details." });
            }

        }


        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult SaveRefundChallanDetails(RefundChallanViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.OfficeID = KaveriSession.Current.OfficeID;
                    viewModel.UserID = KaveriSession.Current.UserID;

                    DateTime DateOfInstrument = Convert.ToDateTime(viewModel.InstrumentDate);

                    if (viewModel.InstrumentNumber != viewModel.ReEnterInstrumentNumber)
                    {
                        return Json(new { success = false, message = "Challan Number and Re-Enter Challan Number must be same." });
                    }
                    
                    if(
                        viewModel.InstrumentNumber.Substring(0,2) != "CR" 
                       || Convert.ToInt32(viewModel.InstrumentNumber.Substring(2, 2)) != DateOfInstrument.Month 
                       || viewModel.InstrumentNumber.Substring(4, 2) != viewModel.InstrumentDate.Substring(8, 2)
                       )
                    {
                        if(
                            viewModel.InstrumentNumber.Substring(0,2) != "IG"
                           || Convert.ToInt32(viewModel.InstrumentNumber.Substring(2, 2)) != DateOfInstrument.Month
                           || viewModel.InstrumentNumber.Substring(4, 2) != viewModel.InstrumentDate.Substring(8, 2)
                           )
                        {
                            //return Json(new
                            //{
                            //    success = false,
                            //    message = "Please enter valid Challan number<br/>" +
                            //     "&nbsp &nbsp &nbsp or valid Challan date with proper month[<span style=\"color:red;\">MM</span>] and year[<span style=\"color:#26C726;\">YY</span>]<br/> " +
                            //     "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/><br/> " +
                            //     "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
                            //     "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
                            //     "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> "
                            //});

                            return Json(new
                            {
                                success = false,
                                message = "Please verify the challan number and challan date as per the format. <br/><br/>" +
                                 "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
                                 "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
                                 "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
                                 "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> "
                            });


                        }

                    }
                    

                    if (Math.Floor(viewModel.ChallanAmount) <= 0)
                    {
                        return Json(new { success = false, message = "Challan Amount should be > than Zero." });
                    }
                    if (Math.Floor(viewModel.RefundAmount) <= 0)
                    {
                        return Json(new { success = false, message = "Refund Amount should be > than Zero." });
                    }
                    if (viewModel.RefundAmount > viewModel.ChallanAmount)
                    {
                        return Json(new { success = false, message = "Refund Amount should be <= Challan Amount." });
                    }

                    DateTime ChallanDate = Convert.ToDateTime(DateOfInstrument);
                    DateTime ApplicationDateTime = Convert.ToDateTime(viewModel.ApplicationDateTime);
                    var IsApplicationDateGreater = DateTime.Compare(ChallanDate, ApplicationDateTime);

                    if (IsApplicationDateGreater > 0)
                    {
                        return Json(new { success = false, message = " Application Date should be >= Challan Date. " });
                    }

                    caller = new ServiceCaller("RefundChallanAPIController");

                    string errorMessage = string.Empty;
                    RefundChallanOrderResultModel responseModel = caller.PostCall<RefundChallanViewModel, RefundChallanOrderResultModel>("SaveRefundChallanDetails", viewModel, out errorMessage);

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
        public ActionResult UpdateRefundChallanDetails(RefundChallanViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.OfficeID = KaveriSession.Current.OfficeID;

                    DateTime DateOfInstrument = Convert.ToDateTime(viewModel.InstrumentDate);

                    if (viewModel.InstrumentNumber != viewModel.ReEnterInstrumentNumber)
                    {
                        return Json(new { success = false, message = "Challan Number and Re-Enter Challan Number must be same." });
                    }
                    
                    if(
                        viewModel.InstrumentNumber.Substring(0, 2) != "CR" 
                        || Convert.ToInt32(viewModel.InstrumentNumber.Substring(2, 2)) != DateOfInstrument.Month 
                        || viewModel.InstrumentNumber.Substring(4, 2) != viewModel.InstrumentDate.Substring(8, 2)
                       )
                    {
                        if(
                            viewModel.InstrumentNumber.Substring(0, 2) != "IG"
                            || Convert.ToInt32(viewModel.InstrumentNumber.Substring(2, 2)) != DateOfInstrument.Month
                            || viewModel.InstrumentNumber.Substring(4, 2) != viewModel.InstrumentDate.Substring(8, 2)
                           )
                        {
                            return Json(new
                            {
                                success = false,
                                //message = "Please enter valid Challan number. <br/> " +
                                // "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
                                // "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
                                // "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
                                // "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> "
                                message = "Please verify the challan number and challan date as per the format. <br/><br/>" +
                                 "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
                                 "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
                                 "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
                                 "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> "
                            });
                        }

                    }
                    
                    if (Math.Floor(viewModel.ChallanAmount) <= 0)
                    {
                        return Json(new { success = false, message = "Challan Amount should be > than Zero." });
                    }
                    if (Math.Floor(viewModel.RefundAmount) <= 0)
                    {
                        return Json(new { success = false, message = "Refund Amount should be > than Zero." });
                    }
                    if (viewModel.RefundAmount > viewModel.ChallanAmount)
                    {
                        return Json(new { success = false, message = "Refund Amount should be <= Challan Amount." });
                    }

                    DateTime ChallanDate = Convert.ToDateTime(DateOfInstrument);
                    DateTime ApplicationDateTime = Convert.ToDateTime(viewModel.ApplicationDateTime);
                    var IsApplicationDateGreater = DateTime.Compare(ChallanDate, ApplicationDateTime);

                    if (IsApplicationDateGreater > 0)
                    {
                        return Json(new { success = false, message = " Application Date should be >= Challan Date " });
                    }

                    caller = new ServiceCaller("RefundChallanAPIController");

                    string errorMessage = string.Empty;
                    RefundChallanOrderResultModel responseModel = caller.PostCall<RefundChallanViewModel, RefundChallanOrderResultModel>("UpdateRefundChallanDetails", viewModel, out errorMessage);

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

        
        [HttpGet]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult FinalizeRefundChallanDetails(long RowId)
        {
            try
            {
                int currentOrderID = KaveriSession.Current.OrderID;

                caller = new ServiceCaller("RefundChallanAPIController");

                string finalizeDECResp = caller.GetCall<string>("FinalizeRefundChallanDetails", new { RowId = RowId });


                if (finalizeDECResp == string.Empty)
                {
                    return Json(new { success = true, message = "This Refund Challan entry has been successfully Finalized." }, JsonRequestBehavior.AllowGet);
                }
                else if (finalizeDECResp == null || finalizeDECResp == "NULL")
                {
                    return Json(new { success = false, message = "Error Occured while finalizing Refund Challan Details. Please contact admin." }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { success = false, message = finalizeDECResp }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult ExportRefundChallanOrderDetailsToExcel(string SroID, string DistrictID, string SroName, string DroName)
        {

            try
            {
                if(SroID == "undefined")
                {
                    SroID = "0";
                }
                caller = new ServiceCaller("RefundChallanAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "Refund_Challan_Entry" + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                int RoleID = KaveriSession.Current.RoleID;
                var result  = caller.GetCall<RefundChallanResultModel>("LoadRefundChallanDetailsTable", new { DroCode = DistrictID, SROCode = SroID, RoleID = RoleID, IsExcel = true });

                RefundChallanResultModel refundChallanResultModel = new RefundChallanResultModel();
                refundChallanResultModel.refundChallanTableList = new List<RefundChallanTableModel>();
                refundChallanResultModel.refundChallanTableList = result.refundChallanTableList;


                if (refundChallanResultModel.refundChallanTableList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Refund Challan Note Details.", URLToRedirect = "/Home/HomePage" });
                }

                string excelHeader = string.Format("Refund Challan Entry Details");
                string createdExcelPath = CreateExcelOrderDetails(refundChallanResultModel.refundChallanTableList, fileName, excelHeader, DroName, SroName);
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


        private string CreateExcelOrderDetails(List<RefundChallanTableModel> refundChallanTableModelList, string fileName, string excelHeader, string DroName, string SroName = "NA")
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


        [HttpGet]
        public dynamic ViewBtnClickOrderTable(long RowId)
        {
            try
            {
                caller = new ServiceCaller("RefundChallanAPIController");
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

    }
}