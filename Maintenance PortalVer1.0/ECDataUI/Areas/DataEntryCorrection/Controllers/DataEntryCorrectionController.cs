using CustomModels.Common;
using CustomModels.Models.DataEntryCorrection;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.DataEntryCorrection.Controllers
{
    [KaveriAuthorization]
    public class DataEntryCorrectionController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        public ActionResult DataEntryCorrectionView()
        {

            //Added by Madhusoodan on 05/08/2021 to clear all previous session properties of DEC
            KaveriSession.Current.OrderID = 0;
            //Commented by Madhusoodan on 10/08/2021 (Not required now)
            //KaveriSession.Current.DECDocumentNumber = 0;
            //KaveriSession.Current.DECFinancialYear = 0;
            //KaveriSession.Current.DECBookTypeID = 0;
            KaveriSession.Current.DECSROCode = 0;
            int OfficeID = KaveriSession.Current.OfficeID;
            int LevelID = KaveriSession.Current.LevelID;
            //Added by mayank District enabled for DEC
            long UserID = KaveriSession.Current.UserID;
            caller = new ServiceCaller("DataEntryCorrectionAPIController");
            //Added by mayank District enabled for DEC
            DataEntryCorrectionViewModel reqModel = caller.GetCall<DataEntryCorrectionViewModel>("DataEntryCorrectionView", new { officeID = OfficeID, LevelID = LevelID, UserID = UserID });
            //Commented by Madhusoodan on 10/08/2021 (Not required in this method)
            //int OfficeID = KaveriSession.Current.OfficeID;

            //DataEntryCorrectionViewModel reqModel = new DataEntryCorrectionViewModel();

            return View(reqModel);
        }


        public ActionResult AddEditOrdersView(int OrderID = 0)
        {
            try
            {

                DataEntryCorrectionOrderViewModel decOrderViewModel = new DataEntryCorrectionOrderViewModel();
                caller = new ServiceCaller("DataEntryCorrectionAPIController");
                //Added by Madhusoodan on 08/08/2021
                int orderID = KaveriSession.Current.OrderID;
                int OfficeID = KaveriSession.Current.OfficeID;

                if (orderID > 0)
                {
                    //checkif order details present in table and populate data in textboxes
                    decOrderViewModel = caller.GetCall<DataEntryCorrectionOrderViewModel>("AddEditOrderDetails", new { OrderID = orderID });

                    if (decOrderViewModel == null)
                    {
                        if (orderID == 0)
                        {
                            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Add New Order View", URLToRedirect = "/Home/HomePage" });
                        }
                        else
                        {
                            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving existing Order View", URLToRedirect = "/Home/HomePage" });
                        }
                    }

                }
                // else load empty view for new entry

                return View("AddEditOrderDetails", decOrderViewModel);

            }
            catch (Exception ex)
            {
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while loading Partial View" });
            }

        }

        //For add new Order btn click
        //[HttpGet]
        [HttpPost]
        //public ActionResult LoadAddNewOrderView(FormCollection formCollection)
        //public ActionResult LoadAddNewOrderView()
        public ActionResult LoadAddNewOrderView(int orderID = 0)
        {
            try
            {
                //Keep OrderID in session (For edit mode)
                KaveriSession.Current.OrderID = orderID;
                int OfficeID = KaveriSession.Current.OfficeID;

                caller = new ServiceCaller("DataEntryCorrectionAPIController");

                //Set isEdit Mode
                if (orderID > 0)
                    KaveriSession.Current.IsEditMode = true;
                else
                    KaveriSession.Current.IsEditMode = false;

                DataEntryCorrectionOrderViewModel decResModel = caller.GetCall<DataEntryCorrectionOrderViewModel>("AddEditOrderDetails", new { orderID = orderID, OfficeID = OfficeID });

                if (decResModel == null)
                {
                    if (orderID == 0)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Add New Order View", URLToRedirect = "/Home/HomePage" });
                    }
                    else
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving existing Order View", URLToRedirect = "/Home/HomePage" });
                    }
                }


                return View("AddEditOrderDetails", decResModel);

            }
            catch (Exception ex)
            {
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while loading Add New Order Tab" });
            }

        }

        //Added by Madhusoodan on 26/07/2021 to load different tabs
        public ActionResult LoadInsertUpdateDeleteView(FormCollection formCollection)
        {
            try
            {
                string TableViewName = formCollection["TableViewName"];
                int OrderID = Convert.ToInt32(formCollection["OrderID"]);

                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("DataEntryCorrectionAPIController");

                KaveriSession.Current.OrderID = OrderID;
                if (TableViewName == "SelectDocument")
                {
                    //added by mayank on 06/12/2021 for DEC change request
                    KaveriSession.Current.IsEditMode = true;
                }
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                DataEntryCorrectionViewModel reqModel = caller.GetCall<DataEntryCorrectionViewModel>("LoadInsertUpdateDeleteView", new { OfficeID = OfficeID });

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Insert update View", URLToRedirect = "/Home/HomePage" });
                }

                switch (TableViewName)
                {
                    case "AddEditOrder": return RedirectToAction("AddEditOrdersView", "DataEntryCorrection", new { area = "DataEntryCorrection" });

                    //case "SelectDocument": return RedirectToAction("AddSelectDocumentView", "SelectDocument", new { area = "DataEntryCorrection", documentID = DocumentID, SROCode = SROCode });

                    case "SelectDocument": return RedirectToAction("LoadSelectDocumentTabView", "SelectDocument", new { area = "DataEntryCorrection" });

                        //Commented by Madhusoodan on 10/08/2021 (As these 2 tabs are removed and added in Popup)
                        //case "PropertyNumberDetails": return RedirectToAction("LoadpropertyNumberDetailsTabView", "PropertyNumberDetails", new { area = "DataEntryCorrection" });
                        ////case "PropertyNumberDetails": return RedirectToAction("LoadpropertyNumberDetailsTabView", "DataEntryCorrection", new { area = "DataEntryCorrection" });

                        ////Change below as per requirement
                        //case "PartyDetails": return RedirectToAction("LoadPartyDetailsTabView", "PartyDetails", new { area = "DataEntryCorrection" });

                }
                return View(reqModel);

            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Insert update View", URLToRedirect = "/Home/HomePage" });
            }
        }

        //Added by Madhur
        [HttpPost]
        public ActionResult LoadDocDetailsTable(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("DataEntryCorrectionAPIController");
                int DistrictID = Convert.ToInt32(formCollection["DroCode"]);
                int SROCode = Convert.ToInt32(formCollection["SroCode"]);
                bool isDRLogin = false;
                int RoleID = KaveriSession.Current.RoleID;
                List<DataEntryCorrectionOrderTableModel> reqModel = caller.GetCall<List<DataEntryCorrectionOrderTableModel>>("LoadDocDetailsTable", new { DroCode = DistrictID, SROCode = SROCode, RoleID = RoleID, IsExcel = false });
                if (RoleID == (int)ECDataUI.Common.CommonEnum.RoleDetails.DR)
                {
                    isDRLogin = true;
                }
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading document details table", URLToRedirect = "/Home/HomePage" });

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
                                    errorMessage = "Please enter valid Search String "
                                });
                                emptyData.MaxJsonLength = Int32.MaxValue;
                                return emptyData;
                            }

                        }
                        else
                        {
                            //reqModel = reqModel.Where(m => m.RegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                            //m.DROrderNumber.ToLower().Contains(searchValue.ToLower()) ||
                            //m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                            //m.Section68Note.ToLower().Contains(searchValue.ToLower()) ||
                            //m.OrderDate.ToLower().Contains(searchValue.ToLower()) ||
                            //m.DistrictName.ToLower().Contains(searchValue.ToLower())).ToList();

                            reqModel = reqModel.Where(m => m.RegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                            m.DROrderNumber.ToLower().Contains(searchValue.ToLower()) ||
                            m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                            m.Section68Note.ToLower().Contains(searchValue.ToLower()) ||
                            m.Status.ToLower().Contains(searchValue.ToLower()) ||
                            m.OrderDate.ToLower().Contains(searchValue.ToLower())).ToList();
                        }
                    }
                    var gridData = reqModel.Select(DataEntryCorrectionOrderTableModel => new
                    {
                        Select = DataEntryCorrectionOrderTableModel.Select,
                        SNo = DataEntryCorrectionOrderTableModel.SNo,
                        DROName = DataEntryCorrectionOrderTableModel.DistrictName,
                        EnteredBY = DataEntryCorrectionOrderTableModel.EnteredBY,
                        SROName = DataEntryCorrectionOrderTableModel.SROName,
                        DROrderNumber = DataEntryCorrectionOrderTableModel.DROrderNumber,
                        OrderDate = DataEntryCorrectionOrderTableModel.OrderDate,
                        Section68Note = DataEntryCorrectionOrderTableModel.Section68Note,
                        RegistrationNumber = DataEntryCorrectionOrderTableModel.RegistrationNumber,
                        Status = DataEntryCorrectionOrderTableModel.Status,
                        ViewBtn = DataEntryCorrectionOrderTableModel.ViewBtn,
                        Action = DataEntryCorrectionOrderTableModel.Action

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
                        isDrLogin = isDRLogin
                    });
                    return JsonData;
                }

            }
            catch (Exception ex)
            {
                throw;
            }

        }


        //Added by Madhur
        [HttpGet]
        public dynamic ViewBtnClickOrderTable(int OrderID)
        {
            try
            {
                //Added by shivam b on 04/05/2022 for Virtual Directory on Section 68 Note Data Entry
                caller = new ServiceCaller("DataEntryCorrectionAPIController");
                DROrderFilePathResultModel reqModel = caller.GetCall<DROrderFilePathResultModel>("ViewBtnClickOrderTable", new { OrderID = OrderID });
                return File(reqModel.DataEntryCorrectionFileBytes, "application/pdf");
                //Added by shivam b on 04/05/2022 for Virtual Directory on Section 68 Note Data Entry

                //byte[] pdfByteArray = System.IO.File.ReadAllBytes(reqModel);
                //return File(pdfByteArray, "application/pdf");
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

        //Not in use Can be removed
        //Added by Madhur
        //[HttpGet]
        //public dynamic EditBtnClickOrderTable(string DROrderNumber)
        //{
        //    try
        //    {
        //        caller = new ServiceCaller("DataEntryCorrectionAPIController");
        //        string reqModel = caller.GetCall<string>("EditBtnClickOrderTable", new { DROrderNumber = DROrderNumber });
        //        byte[] pdfByteArray = System.IO.File.ReadAllBytes(reqModel);
        //        return File(pdfByteArray, "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //Modified by Madhusoodan on 12/08/2021
        //Added by Madhur
        [HttpGet]
        //public dynamic DeleteBtnClickOrderTable(string DROrderNumber)
        public ActionResult DeleteDECOrder(int orderID)
        {
            try
            {
                caller = new ServiceCaller("DataEntryCorrectionAPIController");
                bool isOrderDeleted = caller.GetCall<bool>("DeleteDECOrder", new { orderID = orderID });

                if (isOrderDeleted)
                    return Json(new { success = true, message = "DR Order details has been reset." }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, message = "Failed to delete DR Order. Please try again." }, JsonRequestBehavior.AllowGet);

                //return isOrderDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhur
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Upload PDF File")]
        public ActionResult UploadOrdersFile()
        {
            try
            {
                //string rootPath = ConfigurationManager.AppSettings["KaveriSupportPath"];

                caller = new ServiceCaller("DataEntryCorrectionAPIController");
                HttpPostedFileBase FileBase = null;
                string arr = Request.Params["filesArray"];
                string OrderNo = Request.Params["OrderNo"];
                string OrderDate = Request.Params["OrderDate"];


                if (string.IsNullOrWhiteSpace(OrderNo))
                {
                    return Json(new { success = false, message = "Please Enter Valid Order No." }, JsonRequestBehavior.AllowGet);
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
                                return Json(new { success = false, message = "File size should be less than 5 MB. Kindly upload file again." + FileBase.FileName + " Kindly select file again." }, JsonRequestBehavior.AllowGet);

                            }
                            //else if (DblExtensions > 1)
                            //{
                            //    return Json(new { success = false, errormsgType = 3, message = "Cannot Upload Files because it is either not a supported File type or because the file has been damaged(for example,it was sent as an attachment and wasn't correctly decoded)!!" }, JsonRequestBehavior.AllowGet);
                            //}
                            //if (!IsValidContentLength(FileBase.ContentLength))
                            //{
                            //    return Json(new { success = false, message = "File size should be less than 6 MB. Kindly upload file again." }, JsonRequestBehavior.AllowGet);
                            //}
                        }

                        //Added by Madhusoodan on 11/08/2021 (To take Order ID to save file)

                        //string filePathToSave = @"F:\OrderPdf\file" + DateTime.Now.Ticks + ".pdf";
                        #region Added by mayank on 13/08/2021 to check if order no .
                        int OfficeID = KaveriSession.Current.OfficeID;
                        bool OrderNoExistResult = caller.GetCall<bool>("CheckifOrderNoExist", new { OrderNo = OrderNo, OrderID = KaveriSession.Current.OrderID, OfficeID = OfficeID });
                        if (OrderNoExistResult)
                        {
                            return Json(new { success = false, errormsgType = 3, message = "Order No already exist.Please enter unique order no" }, JsonRequestBehavior.AllowGet);

                        }

                        DateTime newDate;
                        bool boolDate = DateTime.TryParse(DateTime.ParseExact(OrderDate, "dd/MM/yyyy", null).ToString(), out newDate);
                        DateTime minDate = new DateTime(2003, 1, 1);
                        if (boolDate)
                        {
                            if (newDate < minDate)
                            {
                                return Json(new { success = false, errormsgType = 3, message = "Please provide valid order date" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            throw new Exception("Error Occured while Converting Date");
                        }
                        #endregion

                        //int OfficeID = KaveriSession.Current.OfficeID;

                        int currentOrderID = KaveriSession.Current.OrderID;

                        DROrderFilePathResultModel dROrderFilePathResultModel = caller.GetCall<DROrderFilePathResultModel>("GenerateNewOrderID", new { OfficeID = OfficeID, currentOrderID = currentOrderID });

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

                                    ////Added by mayank for pdf file check on 23/08/2021 not in use
                                    //Document pdf = new Document();
                                    //PdfWriter.GetInstance(pdf, new MemoryStream(fileData));
                                    //try
                                    //{
                                    //    pdf.Open();
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    return Json(new { success = false, errormsgType = 3, message = "Please Provide PDF file only." }, JsonRequestBehavior.AllowGet);

                                    //}
                                }

                                string completeReletiveFilePath = dROrderFilePathResultModel.RelativeFilePath + "\\" + dROrderFilePathResultModel.FileName;

                                string filePathToSave = dROrderFilePathResultModel.rootPath + completeReletiveFilePath;

                                if (fileData != null)
                                {
                                    //Added by shivam b on 04/05/2022 for Virtual Directory for Section68 Note Data Entry
                                    if (!Directory.Exists(dROrderFilePathResultModel.RelativeFilePath))
                                        Directory.CreateDirectory(dROrderFilePathResultModel.rootPath + "\\" + dROrderFilePathResultModel.RelativeFilePath);
                                    
                                        System.IO.File.WriteAllBytes(filePathToSave, fileData);   //Write File   
                                    


                                    ////Added by mayank for pdf file check on 23/08/2021 not in use
                                    //try
                                    //{
                                    //    FileInfo file = new FileInfo(filePathToSave);
                                    //    FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                                    //}
                                    //catch (Exception)
                                    //{
                                    //    return Json(new { success = false, errormsgType = 3, message = "Please Provide PDF file only." }, JsonRequestBehavior.AllowGet);

                                    //}
                                    var jsonResult = Json(new { success = true, message = "File uploaded and Saved Successfully.", filePath = filePathToSave, relativeFilePath = completeReletiveFilePath, orderFileName = dROrderFilePathResultModel.FileName, orderID = dROrderFilePathResultModel.NewOrderID });
                                    jsonResult.MaxJsonLength = int.MaxValue;
                                    return jsonResult;
                                }
                                else
                                {
                                    return Json(new { success = false, message = "Unable to save the selected file.", filePath = "" });
                                }

                                //if (!string.IsNullOrEmpty(partialfilePathToSave))
                                //{
                                //    //return Json(new { success = true, message = "File uploaded and Saved Successfully.", FileDataStr = FileDataStr });

                                //    //Commented and added by Madhusoodan on 27/07/2021 to send byte array of file to DAL and save file by using OrderID
                                //    //var jsonResult = Json(new { success = true, message = "File uploaded and Saved Successfully.", filePath = filePath });
                                //    var jsonResult = Json(new { success = true, message = "File uploaded and Saved Successfully.", filePath = partialfilePathToSave });
                                //    jsonResult.MaxJsonLength = int.MaxValue;
                                //    return jsonResult;
                                //}
                                //else
                                //{
                                //    return Json(new { success = false, message = "Unable to Encrypt the selected file.", filePath = "" });
                                //}
                            }
                        }
                        //


                    }
                    else
                    {
                        return Json(new { success = false, message = "Only one file is allowed" });
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



        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult SaveOrderDetails(DataEntryCorrectionOrderViewModel viewModel)
        {
            string errorMessage = string.Empty;
            caller = new ServiceCaller("DataEntryCorrectionAPIController");
            viewModel.IsInsertedSuccessfully = false;

            try
            {
                #region Input Validations
                string enteredOrderNumber = viewModel.OrderNo;

                if (enteredOrderNumber == null)
                {
                    return Json(new { success = false, message = "Please enter Order Number." });
                }

                if (viewModel.OrderDate == null || viewModel.OrderDate == string.Empty)
                {
                    return Json(new { success = false, message = "Please enter Order Date." });
                }


                #endregion

                viewModel.IPAddress = new CommonFunctions().GetIPAddress();
                viewModel.UserID = KaveriSession.Current.UserID;
                viewModel.OfficeID = KaveriSession.Current.OfficeID;

                //Added by Madhusoodan on 16/08/2021 (To take Order ID to save file)
                //If OrderID is zero user is only updating Order No or Order Date but not file. For this case OrderID is taken from Session (As it is edit mode)
                if (viewModel.OrderID == 0)
                {
                    viewModel.OrderID = KaveriSession.Current.OrderID;
                }


                if (ModelState.IsValid)
                {

                    DataEntryCorrectionOrderResultModel responseModel = caller.PostCall<DataEntryCorrectionOrderViewModel, DataEntryCorrectionOrderResultModel>("SaveOrderDetails", viewModel, out errorMessage);
                    if (!String.IsNullOrEmpty(errorMessage))
                        return Json(new { success = false, message = errorMessage });


                    if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                    {
                        return Json(new { success = false, message = responseModel.ErrorMessage });
                    }
                    else
                    {
                        //if Order details are saved sucessfully then add OrderID in Session
                        KaveriSession.Current.OrderID = responseModel.OrderID;

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


        //Added by Madhusoodan on 10/08/2021 (Delete current Order file)
        //[HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryTokenOnAllPosts]
        //[HttpGet]
        [HttpPost]
        public ActionResult DeleteCurrentOrderFile()
        {
            try
            {
                int orderID = KaveriSession.Current.OrderID;
                DataEntryCorrectionOrderViewModel viewModel = new DataEntryCorrectionOrderViewModel();
                viewModel.OrderID = orderID;
                if (orderID != 0)
                {
                    caller = new ServiceCaller("DataEntryCorrectionAPIController");

                    DataEntryCorrectionOrderResultModel resultModel = caller.PostCall<DataEntryCorrectionOrderViewModel, DataEntryCorrectionOrderResultModel>("DeleteCurrentOrderFile", viewModel);

                    if (string.IsNullOrEmpty(resultModel.ErrorMessage))
                        return Json(new { success = true, message = resultModel.ResponseMessage, currentOrderID = orderID }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { success = false, message = resultModel.ErrorMessage, currentOrderID = orderID }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Error in deleting file. Please initiate the edit order process again.", currentOrderID = orderID }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }

        }
        //Addition ends here

        [HttpGet]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult FinalizeDECOrder()
        {
            try
            {
                int currentOrderID = KaveriSession.Current.OrderID;
                caller = new ServiceCaller("DataEntryCorrectionAPIController");

                if (currentOrderID != 0)
                {
                    //call API Controller

                    string finalizeDECResp = caller.GetCall<string>("FinalizeDECOrder", new { OrderID = currentOrderID });

                    //if (finalizeDECResp == "NULL")
                    if (finalizeDECResp == string.Empty)
                    {
                        return Json(new { success = true, message = "This DR order has been saved successfully. Correction note is appended in EC Search Report." }, JsonRequestBehavior.AllowGet);
                    }
                    else if (finalizeDECResp == null || finalizeDECResp == "NULL")
                    {
                        return Json(new { success = false, message = "Error Occured while finalizing Order .Please contact admin" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { success = false, message = finalizeDECResp }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Please initiate the Section 68(2) Note Data Entry first." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Added by Madhusoodan on 20/08/2021 to show Index II report for finalized orders
        //LoadIndexIIData

        [HttpPost]
        public ActionResult LoadIndexIIDetails(FormCollection formCollection)
        {
            try
            {
                string errorMessage = String.Empty;
                int OrderID = Convert.ToInt32(formCollection["OrderID"]);
                //string PropertyID = formCollection["PropertyID"];
                int OfficeID = KaveriSession.Current.OfficeID;
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                //DataEntryCorrectionViewModel reqModel = new DataEntryCorrectionViewModel();
                //reqModel.startLen = startLen;
                //reqModel.totalNum = totalNum;
                //reqModel.OfficeID = OfficeID;
                //reqModel.DocumentID = Int64.Parse(DocumentID);
                //reqModel.PropertyID = Int64.Parse(PropertyID);
                //reqModel.SROfficeID = KaveriSession.Current.DECSROCode;

                //Added on 05/08/2021 for Deactivate btn
                //reqModel.OrderID = KaveriSession.Current.OrderID;

                caller = new ServiceCaller("DataEntryCorrectionAPIController");

                DataEntryCorrectionResultModel resModel = caller.GetCall<DataEntryCorrectionResultModel>("LoadIndexIIDetails", new { OrderID = OrderID });
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Index II details." });
                }
                int totalCount = resModel.IndexIIReportList.Count;
                IEnumerable<IndexIIReportsDetailsModel> result = resModel.IndexIIReportList;


                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);


                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = "Invalid To Date"
                });

                CommonFunctions objCommon = new CommonFunctions();

                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}

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
                    {
                        result = result.Where(m => m.PropertyDetails.ToLower().Contains(searchValue.ToLower()) ||
                        m.PropertyDetails.ToLower().Contains(searchValue.ToLower()) ||
                        m.Stamp5Datetime.ToLower().Contains(searchValue.ToLower()));
                    }
                }

                var gridData = result.Select(DataEntryCorrectionViewModel => new
                {
                    Sno = DataEntryCorrectionViewModel.Sno,
                    FinalRegistrationNumber = DataEntryCorrectionViewModel.FinalRegistrationNumber,
                    VillageNameE = DataEntryCorrectionViewModel.VillageNameE,
                    Stamp5Datetime = DataEntryCorrectionViewModel.Stamp5Datetime,
                    PropertyDetails = DataEntryCorrectionViewModel.PropertyDetails,
                    ArticleNameE = DataEntryCorrectionViewModel.ArticleNameE,
                    marketvalue = DataEntryCorrectionViewModel.marketvalue,
                    consideration = DataEntryCorrectionViewModel.consideration,
                    Claimant = DataEntryCorrectionViewModel.Claimant,
                    Executant = DataEntryCorrectionViewModel.Executant,
                    CDNumber = DataEntryCorrectionViewModel.CDNumber,
                    PageCount = DataEntryCorrectionViewModel.PageCount,
                    Description = DataEntryCorrectionViewModel.PropertyDetails,
                    Section68Note = DataEntryCorrectionViewModel.Section68Note,
                });


                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount
                    });

                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        finalRegistrationNo = resModel.IndexIIFinalRegistrationNo,
                        SroName = resModel.IndexIISroName,
                    });
                }
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while loading Index II Details" });
            }
        }


        private bool isValidContentLength(int ContentLength)
        {
            try
            {
                return ((ContentLength / 1024) / 1024) < 5;  //5 Mb
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        [HttpGet]
        public ActionResult GetSroCodebyDistrict(int DroCode)
        {
            try
            {
                caller = new ServiceCaller("DataEntryCorrectionAPIController");
                DataEntryCorrectionViewModel resultModel = caller.GetCall<DataEntryCorrectionViewModel>("GetSroCodebyDistrict", new { DroCode = DroCode });
                return Json(new { success = true, SroList = resultModel.SROfficeOrderList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        #region Added by mayank for Excel download

        [HttpGet]
        public ActionResult ExportOrderDetailsToExcel(string SroID, string DistrictID, string SroName, string DroName)
        {
            try
            {
                caller = new ServiceCaller("DataEntryCorrectionAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "DR_Order_Details" + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                int RoleID = KaveriSession.Current.RoleID;
                //string SroName = "-";
                //string DroName = "-";
                List<DataEntryCorrectionOrderTableModel> ResModel = caller.GetCall<List<DataEntryCorrectionOrderTableModel>>("LoadDocDetailsTable", new { DroCode = DistrictID, SROCode = SroID, RoleID = RoleID, IsExcel = true });
                if (ResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Section 68(2) Note Details", URLToRedirect = "/Home/HomePage" });
                }

                string excelHeader = string.Format("Finalised DR Order Details");
                int SroCode = Convert.ToInt32(SroID);
                int DistrictCode = Convert.ToInt32(SroID);
                //if (SroCode== 0)
                //     SroName = "All";
                //if (DistrictCode == 0)
                //     DroName = "All";
                //if (DistrictCode != 0 && SroCode == 0)
                //{
                //    if (ResModel.Count>0)
                //    {
                //        DroName = ResModel.FirstOrDefault().DistrictName;
                //        SroName = "All"; 
                //    }
                //}
                //if (DistrictCode != 0 && SroCode != 0)
                //{

                //        DroName = ResModel.FirstOrDefault().DistrictName;
                //        SroName = ResModel.FirstOrDefault().SROName; 

                //}


                string createdExcelPath = CreateExcelOrderDetails(ResModel, fileName, excelHeader, DroName, SroName);
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

        private string CreateExcelOrderDetails_withnewcolumn(List<DataEntryCorrectionOrderTableModel> dataEntryCorrectionOrderTableModelList, string fileName, string excelHeader, string DroName, string SroName)
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
                    workSheet.Cells[5, 1].Value = "Total Records : " + (dataEntryCorrectionOrderTableModelList.Count());

                    workSheet.Cells[1, 1, 1, 8].Merge = true;
                    workSheet.Cells[2, 1, 2, 8].Merge = true;
                    workSheet.Cells[3, 1, 3, 8].Merge = true;
                    workSheet.Cells[4, 1, 4, 8].Merge = true;
                    workSheet.Cells[5, 1, 5, 8].Merge = true;
                    workSheet.Cells[6, 1, 6, 8].Merge = true;
                    //workSheet.Cells[6, 1, 6, 13].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 50;
                    workSheet.Column(8).Width = 40;

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


                    workSheet.Cells[7, 1].Value = "S No.";
                    workSheet.Cells[7, 2].Value = "DRO Name";
                    workSheet.Cells[7, 3].Value = "SRO Name";
                    workSheet.Cells[7, 4].Value = "Entered By";
                    workSheet.Cells[7, 5].Value = "DR Order Number";
                    workSheet.Cells[7, 6].Value = "Order Date";
                    workSheet.Cells[7, 7].Value = "Section 68(2) Note";
                    workSheet.Cells[7, 8].Value = "Registration Number";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    ////workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";


                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    foreach (var items in dataEntryCorrectionOrderTableModelList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = items.SNo;
                        workSheet.Cells[rowIndex, 2].Value = items.DistrictName;
                        workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        workSheet.Cells[rowIndex, 4].Value = items.EnteredBY;
                        workSheet.Cells[rowIndex, 5].Value = items.DROrderNumber;
                        workSheet.Cells[rowIndex, 6].Value = items.OrderDate;
                        workSheet.Cells[rowIndex, 7].Value = items.Section68NoteForExcel;
                        workSheet.Cells[rowIndex, 8].Value = items.RegistrationNumber;

                        workSheet.Cells[rowIndex, 1].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        rowIndex++;
                    }

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 8])
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

        private string CreateExcelOrderDetails(List<DataEntryCorrectionOrderTableModel> dataEntryCorrectionOrderTableModelList, string fileName, string excelHeader, string DroName, string SroName)
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
                    workSheet.Cells[5, 1].Value = "Total Records : " + (dataEntryCorrectionOrderTableModelList.Count());

                    workSheet.Cells[1, 1, 1, 8].Merge = true;
                    workSheet.Cells[2, 1, 2, 8].Merge = true;
                    workSheet.Cells[3, 1, 3, 8].Merge = true;
                    workSheet.Cells[4, 1, 4, 8].Merge = true;
                    workSheet.Cells[5, 1, 5, 8].Merge = true;
                    workSheet.Cells[6, 1, 6, 8].Merge = true;
                    //workSheet.Cells[6, 1, 6, 13].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 50;
                    workSheet.Column(6).Width = 40;
                    workSheet.Column(7).Width = 50;
                    workSheet.Column(8).Width = 40;

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


                    workSheet.Cells[7, 1].Value = "S No.";
                    workSheet.Cells[7, 2].Value = "DRO Name";
                    workSheet.Cells[7, 3].Value = "SRO Name";
                    workSheet.Cells[7, 4].Value = "Order Uploaded By";
                    workSheet.Cells[7, 5].Value = "DR Order Number";
                    workSheet.Cells[7, 6].Value = "Order Date";
                    workSheet.Cells[7, 7].Value = "Section 68(2) Note";
                    workSheet.Cells[7, 8].Value = "Registration Number";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    ////workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";


                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    foreach (var items in dataEntryCorrectionOrderTableModelList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = items.SNo;
                        workSheet.Cells[rowIndex, 2].Value = items.DistrictName;
                        workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        workSheet.Cells[rowIndex, 4].Value = items.EnteredBY;
                        workSheet.Cells[rowIndex, 5].Value = items.DROrderNumber;
                        workSheet.Cells[rowIndex, 6].Value = items.OrderDate;
                        workSheet.Cells[rowIndex, 7].Value = items.Section68NoteForExcel;
                        workSheet.Cells[rowIndex, 8].Value = items.RegistrationNumber;

                        workSheet.Cells[rowIndex, 1].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        rowIndex++;
                    }

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 8])
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

        [HttpGet]
        public ActionResult ExportindexIIDetailsToExcel(String OrderID)
        {
            try
            {
                caller = new ServiceCaller("DataEntryCorrectionAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "DR_Order_Details" + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                int RoleID = KaveriSession.Current.RoleID;
                //string SroName = "-";
                //string DroName = "-";
                DataEntryCorrectionResultModel resModel = caller.GetCall<DataEntryCorrectionResultModel>("LoadIndexIIDetails", new { OrderID = OrderID });
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Index II details." });
                }
                string excelHeader = string.Format("Index II Entry for Document No: " + resModel.IndexIIFinalRegistrationNo + " of " + resModel.IndexIISroName + " SRO");

                //if (SroCode== 0)
                //     SroName = "All";
                //if (DistrictCode == 0)
                //     DroName = "All";
                //if (DistrictCode != 0 && SroCode == 0)
                //{
                //    if (ResModel.Count>0)
                //    {
                //        DroName = ResModel.FirstOrDefault().DistrictName;
                //        SroName = "All"; 
                //    }
                //}
                //if (DistrictCode != 0 && SroCode != 0)
                //{

                //        DroName = ResModel.FirstOrDefault().DistrictName;
                //        SroName = ResModel.FirstOrDefault().SROName; 

                //}


                string createdExcelPath = CreateExcelIndexIIDetails(resModel, fileName, excelHeader);
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

        private string CreateExcelIndexIIDetails(DataEntryCorrectionResultModel dataEntryCorrectionResultModel, string fileName, string excelHeader)
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
                    //workSheet.Cells[2, 1].Value = "District : " + DroName;
                    //workSheet.Cells[3, 1].Value = "SRO : " + SroName;
                    //workSheet.Cells[4, 1].Value = "Log Type : " + SelectedLogType;

                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (dataEntryCorrectionResultModel.IndexIIReportList.Count);

                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                    workSheet.Cells[4, 1, 4, 9].Merge = true;
                    //workSheet.Cells[5, 1, 5, 9].Merge = true;
                    //workSheet.Cells[6, 1, 6, 9].Merge = true;
                    //workSheet.Cells[6, 1, 6, 13].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 90;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 20;
                    workSheet.Column(8).Width = 20;
                    workSheet.Column(9).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    //workSheet.Row(5).Style.Font.Bold = true;
                    //workSheet.Row(6).Style.Font.Bold = true;
                    //workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(6).Style.WrapText = true;


                    workSheet.Cells[7, 1].Value = "S No.";
                    workSheet.Cells[7, 2].Value = "Property Description";
                    workSheet.Cells[7, 3].Value = "Execution Date";
                    workSheet.Cells[7, 4].Value = "Nature of Document";
                    workSheet.Cells[7, 5].Value = "Executant";
                    workSheet.Cells[7, 6].Value = "Claimant";
                    workSheet.Cells[7, 7].Value = "CD Number";
                    workSheet.Cells[7, 8].Value = "Page Count";
                    workSheet.Cells[7, 9].Value = "Final Registration Number";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    ////workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";


                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    foreach (var items in dataEntryCorrectionResultModel.IndexIIReportList)
                    {
                        if (!string.IsNullOrEmpty(items.Section68Note))
                        {
                            string Section68Note = string.Empty;
                            string Section68NoteHTML = string.Empty;

                            string[] Section68NoteList = items.Section68Note.Split(new string[] { "#*#" }, StringSplitOptions.None);

                            foreach (string str in Section68NoteList)
                            {
                                Section68Note += str + System.Environment.NewLine;
                            }

                            //if (!string.IsNullOrEmpty(Section68Note))
                            //{
                            //    Section68NoteHTML = "<table border='1' style='font-family: KNB-TTUmaEN;'><tbody><tr><td style='text-align:left;'><label style='color: black;font-size:medium'><b>Note: </b></label><br><span style='color: black'><b>" + Section68Note + "</b></span></td></tr></tbody></table>";
                            //}
                            workSheet.Cells[rowIndex, 1, rowIndex + 1, 1].Merge = true;
                            workSheet.Cells[rowIndex, 3, rowIndex + 1, 3].Merge = true;
                            workSheet.Cells[rowIndex, 4, rowIndex + 1, 4].Merge = true;
                            workSheet.Cells[rowIndex, 5, rowIndex + 1, 5].Merge = true;
                            workSheet.Cells[rowIndex, 6, rowIndex + 1, 6].Merge = true;
                            workSheet.Cells[rowIndex, 7, rowIndex + 1, 7].Merge = true;
                            workSheet.Cells[rowIndex, 8, rowIndex + 1, 8].Merge = true;
                            workSheet.Cells[rowIndex, 9, rowIndex + 1, 9].Merge = true;
                            workSheet.Cells[rowIndex + 1, 2].Value = "Note : " + System.Environment.NewLine + Section68Note;
                            workSheet.Cells[rowIndex + 1, 2].Style.Font.Bold = true;
                            workSheet.Cells[rowIndex + 1, 2].Style.Font.Size = 11;
                            workSheet.Cells[rowIndex + 1, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            workSheet.Cells[rowIndex + 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            workSheet.Cells[rowIndex + 1, 1].Style.WrapText = true;
                            workSheet.Cells[rowIndex + 1, 2].Style.WrapText = true;
                            workSheet.Cells[rowIndex + 1, 3].Style.WrapText = true;
                            workSheet.Cells[rowIndex + 1, 4].Style.WrapText = true;
                            workSheet.Cells[rowIndex + 1, 5].Style.WrapText = true;
                            workSheet.Cells[rowIndex + 1, 6].Style.WrapText = true;
                            workSheet.Cells[rowIndex + 1, 7].Style.WrapText = true;
                            workSheet.Cells[rowIndex + 1, 8].Style.WrapText = true;
                            workSheet.Cells[rowIndex + 1, 9].Style.WrapText = true;

                            workSheet.Cells[rowIndex + 1, 1].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex + 1, 2].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex + 1, 3].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex + 1, 4].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex + 1, 5].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex + 1, 6].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex + 1, 7].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex + 1, 8].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex + 1, 9].Style.Font.Name = "KNB-TTUmaEN";


                        }

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";



                        workSheet.Cells[rowIndex, 1].Value = items.Sno;
                        workSheet.Cells[rowIndex, 2].Value = items.PropertyDescription;
                        workSheet.Cells[rowIndex, 3].Value = items.Stamp5Datetime;
                        workSheet.Cells[rowIndex, 4].Value = items.ArticleNameE;
                        workSheet.Cells[rowIndex, 5].Value = items.Executant;
                        workSheet.Cells[rowIndex, 6].Value = items.Claimant;
                        workSheet.Cells[rowIndex, 7].Value = items.CDNumber;
                        workSheet.Cells[rowIndex, 8].Value = items.PageCount;
                        workSheet.Cells[rowIndex, 9].Value = items.FinalRegistrationNumber;

                        //workSheet.Cells[rowIndex,1].Merge.


                        workSheet.Cells[rowIndex, 1].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 9].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        if (!string.IsNullOrEmpty(items.Section68Note))
                        {
                            rowIndex = rowIndex + 2;
                        }
                    }

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 9])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
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
        #endregion

        
        
    }

}

