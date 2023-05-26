using CustomModels.Models.DataEntryCorrection;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.DataEntryCorrection.Controllers
{

    [KaveriAuthorization]
    public class ReScanningApplicationController : Controller
    {
        private ServiceCaller caller;

        //***********************************************************************************************************************************************************************************


        [MenuHighlight]
        public ActionResult ReScanningApplicationView()
        {
            try
            {
                caller = new ServiceCaller("ReScanningApplicationAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                //int OfficeID = 48;
                ReScanningApplicationViewModel reqModel = caller.GetCall<ReScanningApplicationViewModel>("ReScanningApplicationView", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured", URLToRedirect = "/Home/HomePage" });
            }

        }


        //***********************************************************************************************************************************************************************************


        [HttpGet]
        public ActionResult btnShowClick(int DRO, int SRO, string OrderNo, string Date, string DocNo, string DType, string BType, string FYear, int NPC)
        {
            try
            {
                #region validation
                var regex = "^[a-zA-Z0-9 _./-]*$";
                var matchOrderNumber = Regex.Match(OrderNo, regex, RegexOptions.IgnoreCase);
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                long tempDocNum = 0;
                
                
                //Added by Madhur on 24/08/2022
                //start
                int tdays = (dt - Convert.ToDateTime(Date)).Days;
                //end

                if (!matchOrderNumber.Success || OrderNo == "" || OrderNo.Length < 10 || OrderNo.Length > 100)
                {
                    if (OrderNo.Length <= 0)
                    {

                        return Json(new { serverError = false, status = "NotFoundFile", Message = "Please enter Order Number." }, JsonRequestBehavior.AllowGet);

                    }
                    else if (OrderNo.Length < 10)
                    {

                        return Json(new { serverError = false, status = "NotFoundFile", Message = "Order number's minimum length should be 10 characters." }, JsonRequestBehavior.AllowGet);

                    }
                    else if (OrderNo.Length > 100)
                    {

                        return Json(new { serverError = false, status = "NotFoundFile", Message = "Order number's maximum length should not be more than 100 characters." }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {

                        return Json(new { serverError = false, status = "NotFoundFile", Message = "Please enter valid Order Number." }, JsonRequestBehavior.AllowGet);

                    }
                }

                else if (Date == "")
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "Please select Order Date." }, JsonRequestBehavior.AllowGet);
                }

                //Added by Madhur on 24/08/2022
                //start
                else if(tdays < 0)
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "Future dates are not allowed. Please check." }, JsonRequestBehavior.AllowGet);

                }
                //end
                //Commented by Madhur on 24-08-2022
                //else if ((dt - Convert.ToDateTime(Date)).TotalDays >= 15)
                //{
                //    return Json(new { serverError = false, status = "NotFoundFile", Message = "Order date cannot be older than 2 weeks." }, JsonRequestBehavior.AllowGet);
                //}

                else if (DType == "" || (DType != "MARR" && DType != "DOC"))
                {
                    return Json(new { serverError = false, errorMessage = "Please select Document Type." }, JsonRequestBehavior.AllowGet);

                }
                else if (DocNo == "")
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "Please enter Document Number." }, JsonRequestBehavior.AllowGet);

                }
                else if (DType == "DOC" && (!Int64.TryParse(DocNo, out tempDocNum) || DocNo == "0"))
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "Please enter valid Document Number." }, JsonRequestBehavior.AllowGet);

                }
                else if ((BType == "0") && DType == "DOC")
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "Please Select Book Type." }, JsonRequestBehavior.AllowGet);

                }

                else if ((FYear == "0") && DType == "DOC")
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "Please Select Financial Year." }, JsonRequestBehavior.AllowGet);

                }

                string CPC = PCount(SRO, DocNo, FYear, BType);

                if (NPC == 0 || NPC > 999)
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "Please enter valid Page Count (Max Allowed Value:999)." }, JsonRequestBehavior.AllowGet);

                }
                else if (CPC != "Not Available" && NPC != -1 && NPC < Convert.ToInt32(CPC))
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "New Page Count must be equal or greater to Current Page Count." }, JsonRequestBehavior.AllowGet);

                }
                //foreach(char c in OrderNo)
                //{
                //    if((int)c==32)
                //    {
                //        return Json(new { serverError = false, status = "NotFoundFile", Message = "Order number cannot have spaces." }, JsonRequestBehavior.AllowGet);

                //    }
                //}

                #endregion

                caller = new ServiceCaller("ReScanningApplicationAPIController");

                ReScanningApplicationReqModel req = new ReScanningApplicationReqModel();
                req.SROCode = SRO;
                req.DROCode = DRO;
                req.OrderNo = OrderNo;
                req.Date = Date;
                req.DocNo = DocNo;
                req.DType = DType;
                req.FinancialYearStr = FYear;
                req.BookTypeStr = BType;
                FileUploaderRescanningApplication.FileUploaderSoapClient FU = new FileUploaderRescanningApplication.FileUploaderSoapClient();
                int DocumentTypeID = (DType == "MARR") ? 2 : 1;
                try
                {
                    if (DocumentTypeID == 1)
                    {
                        if ((Convert.ToInt64(DocNo).ToString().Length > 5))
                            return Json(new { serverError = false, status = "NotFoundFile", Message = "Please check document number" }, JsonRequestBehavior.AllowGet);
                        req.DocNo = Convert.ToInt64(req.DocNo).ToString();

                    }
                }
                catch (Exception ex)
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "Please check document number" }, JsonRequestBehavior.AllowGet);
                }

                DetailModel responseModel = caller.GetCall<DetailModel>("btnShowClick", new { DRO = DRO, SRO = SRO, OrderNo = OrderNo, Date = Date, DocNo = DocNo, DType = DType, FYear = FYear, BType = BType });


                if (responseModel.DocID == -1111)
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "This document cannot be enabled for rescaning as it is a FRUITS document." }, JsonRequestBehavior.AllowGet);
                }
                else if (responseModel.DocID <= 0)
                {
                    return Json(new { serverError = false, status = "NotFoundFile", Message = "No document available for given details. Please check details." }, JsonRequestBehavior.AllowGet);
                }

                string FRN = "";
                if (DocumentTypeID == 1)
                    FRN = caller.GetCall<string>("GetFRN", new { SROCode = SRO, BType = BType, FYear = FYear, DocNo = DocNo });
                else
                    FRN = DocNo;
                //FRN = responseModel.TriLetter + "-" + req.BookTypeStr + "-" + Convert.ToInt64(req.DocNo).ToString("D5") + "-" + req.FinancialYearStr;

                int isAvailable = FU.IsFileExistAndNotRescanningEnabledMP(responseModel.DocID, DocumentTypeID, SRO);




                if (isAvailable == 1)
                {
                    return Json(new { serverError = false, status = "Found", OnClickData = "upFunct(" + req.DROCode + "," + req.SROCode + ",'" + req.DocNo + "','" + req.OrderNo + "','" + req.Date + "','" + req.DType + "'," + responseModel.DocID + ",'" + responseModel.TriLetter + "','" + FRN + "','" + NPC + "','" + "false" + "')" }, JsonRequestBehavior.AllowGet);

                }
                else if (isAvailable == 0)
                {
                    return Json(new { serverError = false, status = FRN, Message = "Something went wrong while trying to fetch previous scanned file details. Please try again." }, JsonRequestBehavior.AllowGet);
                }
                else if (isAvailable == 2)
                {
                    return Json(new { serverError = false, status = FRN, Message = "Scanned document is not available at specified path. Please check." }, JsonRequestBehavior.AllowGet);
                }
                else if (isAvailable == 3)
                {
                    //return Json(new { serverError = false, status = FRN, Message = "Re-Scan Order is already uploaded and is in the active state, please proceed with Re-scanning the document" }, JsonRequestBehavior.AllowGet);
                    return Json(new { serverError = false, status = FRN, Message = "Re-Scan Order is already uploaded, please proceed with Re-scanning the document." }, JsonRequestBehavior.AllowGet);
                }
                else if (isAvailable == 4)
                {
                    //Added by Madhur on 24-08-2022

                    //string errorMessage = string.Empty;
                    //req.DocID = responseModel.DocID;
                    //req.IPAddress = new CommonFunctions().GetIPAddress();
                    //req.UserID = KaveriSession.Current.UserID;
                    //req.isMissingDocument = true;
                    //req.FileName = "-";
                    //req.FilePath = "-";
                    //int MD = caller.PostCall<ReScanningApplicationReqModel, int>("Upload", req, out errorMessage);

                    //End

                    return Json(new { serverError = false, status = "Found", OnClickData = "upFunct(" + req.DROCode + "," + req.SROCode + ",'" + req.DocNo + "','" + req.OrderNo + "','" + req.Date + "','" + req.DType + "'," + responseModel.DocID + ",'" + responseModel.TriLetter + "','" + FRN + "','" + NPC+ "','" + "true" + "')" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { serverError = false, status = FRN, Message = "Something went wrong while trying to fetch scanned file details. Please try again." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { serverError = true, status = "NotFoundFile", errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }


        //***********************************************************************************************************************************************************************************


        private bool IsPDFValidExtension(string extension)
        {
            return (extension.Equals(".pdf") || extension.Equals(".PDF"));
        }


        //***********************************************************************************************************************************************************************************


        private bool isValidContentLength(int ContentLength)
        {
            try
            {
                return ((ContentLength / 1024) / 1024) < 10;  //5 Mb
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }


        //***********************************************************************************************************************************************************************************


        [HttpPost]
        public ActionResult Upload()
        {
            ReScanningApplicationReqModel reqModel = new ReScanningApplicationReqModel();

            try
            {
                HttpPostedFileBase fileBase = null;
                caller = new ServiceCaller("ReScanningApplicationAPIController");
                fileBase = Request.Files["File"];
                reqModel.SROCode = Convert.ToInt32(Request.Params["SRO"]);
                reqModel.DROCode = Convert.ToInt32(Request.Params["DRO"]);
                reqModel.DocNo = Request.Params["DocNo"];
                reqModel.OrderNo = Request.Params["OrderNo"].Trim();
                reqModel.Date = Request.Params["OrderDT"];
                reqModel.DType = Request.Params["DType"];
                reqModel.DocID = Convert.ToInt64(Request.Params["DocID"]);
                reqModel.TriLetter = Request.Params["Tri"];
                reqModel.IPAddress = new CommonFunctions().GetIPAddress();
                reqModel.UserID = KaveriSession.Current.UserID;
                reqModel.NPC = Convert.ToInt32(Request.Params["NPC"]);

                //Added by madhur on 24-08-2022
                reqModel.isMissingDocument = (Request.Params["isMissingDocument"].ToString() == "true") ? true : false ;
                string FRN = Request.Params["FRN"];
                string errorMessage = "";






                int DblExtensions = fileBase.FileName.Count(f => f == '.');
                if (!IsPDFValidExtension(Path.GetExtension(fileBase.FileName.ToLower())))
                {
                    return Json(new { success = false, message = "Please upload PDF file only. Error in file name" + fileBase.FileName + " Kindly select file again." }, JsonRequestBehavior.AllowGet);
                }

                if (!isValidContentLength(fileBase.ContentLength))
                {
                    return Json(new { success = false, message = "File size should be less than 10 MB. Kindly upload file again." + fileBase.FileName + " Kindly select file again." }, JsonRequestBehavior.AllowGet);

                }




                reqModel.FileName = reqModel.DocNo.ToString() + "_" + reqModel.SROCode + ".pdf";

                byte[] fileData = null;
                string FileDataStr = string.Empty;
                using (var binaryReader = new BinaryReader(fileBase.InputStream))
                {
                    fileData = binaryReader.ReadBytes(fileBase.ContentLength);
                }

                if (reqModel.DType == "DOC")
                {
                    reqModel.DocNo = Convert.ToInt64(reqModel.DocNo).ToString();
                    reqModel.FilePath = System.Configuration.ConfigurationManager.AppSettings["ReScanningApplicationUploadFolder"] + "/" + reqModel.TriLetter + "/" + FRN;
                }
                else
                {
                    reqModel.FilePath = System.Configuration.ConfigurationManager.AppSettings["ReScanningApplicationUploadFolder"] + "/" + reqModel.TriLetter + "/" + reqModel.DocNo;
                }
                if (fileData != null)
                {

                    if (!Directory.Exists(reqModel.FilePath))
                    {
                        Directory.CreateDirectory(reqModel.FilePath);
                    }

                    if (!System.IO.File.Exists(reqModel.FilePath + "/" + reqModel.FileName))
                    {
                        reqModel.FilePath = reqModel.FilePath + "/" + reqModel.FileName;


                        using (FileStream fs = System.IO.File.Create(reqModel.FilePath))
                        {

                            fs.Write(fileData, 0, fileData.Length);
                        }

                    }
                    else
                    {
                        return Json(new { serverError = false, status = "Failed", Message = "File already exists for given document details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { serverError = false, status = "Failed", Message = "Error in reading file. Please try again" }, JsonRequestBehavior.AllowGet);
                }


                reqModel.isUploaded = 0;




                int isSuccess = caller.PostCall<ReScanningApplicationReqModel, int>("Upload", reqModel, out errorMessage);
                //ReScanningApplicationResModel responseModel = caller.GetCall<ReScanningApplicationResModel>("Upload", new { req = reqModel });
                if (isSuccess != 1)
                {
                    if (System.IO.File.Exists(reqModel.FilePath))
                    {
                        System.IO.File.Delete(reqModel.FilePath);
                    }
                    return Json(new { serverError = true, status = "Failed", Message = "Upload Failed. Please Try Again." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { serverError = false, status = "Success", Message = "Rescan Order Successfully Uploaded for Document:" + FRN }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(reqModel.FilePath))
                {
                    System.IO.File.Delete(reqModel.FilePath);
                }
                return Json(new { serverError = true, status = "Failed", Message = "Upload Failed. Please Try Again." }, JsonRequestBehavior.AllowGet);

            }
        }

        //***********************************************************************************************************************************************************************************


        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                string errormessage = string.Empty;
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
                return Json(new { SROfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }


        //***********************************************************************************************************************************************************************************


        [HttpPost]
        public ActionResult LoadDocDetailsTable(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("ReScanningApplicationAPIController");
                int DistrictID = Convert.ToInt32(formCollection["DroCode"]);
                int SROCode = Convert.ToInt32(formCollection["SroCode"]);
                bool isDRLogin = false;
                int OfficeID = KaveriSession.Current.RoleID;
                List<ReScanningApplicationOrderTableModel> reqModel = caller.GetCall<List<ReScanningApplicationOrderTableModel>>("LoadDocDetailsTable", new { DroCode = DistrictID });
                //if (RoleID == (int)ECDataUI.Common.CommonEnum.RoleDetails.DR)
                //{
                //    isDRLogin = true;
                //}
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

                            reqModel = reqModel.Where(m => m.RegistrationNumber.ToLower().Contains(searchValue.ToLower())).ToList();
                        }
                    }
                    var gridData = reqModel.Select(ReScanningApplicationOrderTableModel => new
                    {
                        SNo = ReScanningApplicationOrderTableModel.SNo,
                        DROName = ReScanningApplicationOrderTableModel.DistrictName,
                        EnteredBY = ReScanningApplicationOrderTableModel.EnteredBY,
                        EnteryDate = ReScanningApplicationOrderTableModel.EntryDate,
                        SROName = ReScanningApplicationOrderTableModel.SROName,
                        DROrderNumber = ReScanningApplicationOrderTableModel.DROrderNumber.Trim(),
                        OrderDate = ReScanningApplicationOrderTableModel.OrderDate,
                        RegistrationNumber = ReScanningApplicationOrderTableModel.RegistrationNumber,
                        ViewBtn = ReScanningApplicationOrderTableModel.ViewBtn,
                        DocTypeID = ReScanningApplicationOrderTableModel.DocTypeID,
                        isActive = ReScanningApplicationOrderTableModel.IsActive == true ? "Yes" : "No",
                        isRescanCompleted = ReScanningApplicationOrderTableModel.isReScanCompleted == true ? "Yes" : "No",

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


        //***********************************************************************************************************************************************************************************


        [HttpGet]
        public string ViewBtnClickOrderTable(string path)
        {
            try
            {




                byte[] fileBytes = System.IO.File.ReadAllBytes(@path);



                string str = path.Split('/').Last();
                //Added by shivam b on 04/05/2022 for Virtual Directory on Section 68 Note Data Entry
                //caller = new ServiceCaller("ReScanningApplicationAPIController");
                //DROrderFilePathResultModel reqModel = caller.GetCall<DROrderFilePathResultModel>("ViewBtnClickOrderTable", new {path});
                if (!Directory.Exists(Server.MapPath("~/Content/ReScanOrderPDF")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/ReScanOrderPDF"));
                }

                string NewPath = Server.MapPath("~/Content/ReScanOrderPDF") + @"/" + str;

                if (System.IO.File.Exists(NewPath))
                {
                    System.IO.File.Delete(NewPath);
                }

                System.IO.File.WriteAllBytes(path, fileBytes);


                //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, str);
                return NewPath;
                //Added by shivam b on 04/05/2022 for Virtual Directory on Section 68 Note Data Entry

                //byte[] pdfByteArray = System.IO.File.ReadAllBytes(reqModel);
                //return File(pdfByteArray, "application/pdf");
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);

                //if (ex.Message.Contains("Could not find file"))
                //{

                //TempData["name"] = "Bill";
                //return ViewBag;
                //return Content("<script language='javascript' type='text/javascript'>alert('Thanks for Feedback!');</script>");
                return "Could not find file.";
                //}
                // Input string.
                //const string input = "Due to some techinical problem this document cannot be viewed right now.Please try again later.";

                //// Invoke GetBytes method.
                //byte[] array = Encoding.ASCII.GetBytes(input);

                //return File(array, "application/pdf");
            }


        }


        //***********************************************************************************************************************************************************************************


        [HttpGet]
        public dynamic Download(string path)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@path);



            string str = path.Split('/').Last();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, str);
        }

        //***********************************************************************************************************************************************************************************


        [HttpGet]
        public string PCount(int SROCode, string DocNo, string Fyear, string BType)
        {
            try
            {
                caller = new ServiceCaller("ReScanningApplicationAPIController");
                string pCount = caller.GetCall<string>("PCount", new { SROCode = SROCode, DocNo = DocNo, Fyear = Fyear, BType = BType });
                return pCount;
            }
            catch (Exception e)
            {
                return "Not Available";
            }
        }









    }
}