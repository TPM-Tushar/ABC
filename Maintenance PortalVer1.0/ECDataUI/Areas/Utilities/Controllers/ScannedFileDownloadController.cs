#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ScannedFileDownloadController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -22-10-2019
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.Utilities.ScannedfileDownload;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;



namespace ECDataUI.Areas.Utilities.Controllers
{
    [KaveriAuthorizationAttribute]
    public class ScannedFileDownloadController : Controller
    {
        ServiceCaller caller = null;

        /// <summary>
        /// Scanned File Download View
        /// </summary>
        /// <returns>returns view</returns>
        [HttpGet]
        public ActionResult ScannedFileDownloadView()
        {

            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ScannedFileDownload;
                caller = new ServiceCaller("ScannedFileDownloadAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                ScannedFileDownloadView reqModel = caller.GetCall<ScannedFileDownloadView>("ScannedFileDownloadView", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occurred while loading Scanned File Download View", URLToRedirect = "/Home/HomePage" });
                }
                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Some error occurred. Please try again.", URLToRedirect = "/Home/HomePage" });
            }
            //return View();

        }

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

        /// <summary>
        /// Loads ScannedFileDownload Status Datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadScannedFileDownloadLogTable(FormCollection formCollection)
        {
            caller = new ServiceCaller("ScannedFileDownloadAPIController");
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
                String errorMessage = String.Empty;
                long UserID = KaveriSession.Current.UserID;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;
                int totalCount = 0;
                //short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });
                #region Commented Validations
                //Validation For DR Login
                //if (LevelID == Convert.ToInt16(CommonEnum.LevelDetails.DR))
                //{
                //    //Validation for DR when user do not select any sro which is by default "Select"
                //    if ((SroId == 0))
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = false,
                //            errorMessage = "Please select any SRO"
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;
                //    }
                //}
                //else
                //{//Validations of Logins other than SR and DR

                //    if ((SroId == 0 && DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = false,
                //            errorMessage = "Please select any District"
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;
                //    }
                //    else if (SroId == 0 && DistrictId != 0)//when User selects DR but not SR which is by default "Select"
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = false,
                //            errorMessage = "Please select any SRO"
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;

                //    }
                //}

                #endregion

                //int totalCount = caller.GetCall<int>("GetScannedFileDownloadTotalCount");

                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}

                ScannedFileDownloadView ViewModel = caller.GetCall<ScannedFileDownloadView>("LoadScannedFileDownloadLogTable",new { UserID=UserID});
                IEnumerable<ScannedFileLogTableModel> result = ViewModel.ScannedFileDownloadList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting scanned file Download log." });
                }
                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
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
                        result = result.Where(m => m.FRN.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.SroName.ToLower().Contains(searchValue.ToLower()) ||
                        m.FileName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.Filepath.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.DownloadedBY.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.DownloadReason.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.DownloadDateTime.ToLower().Contains(searchValue.ToLower()));
                        totalCount = result.Count();
                    }
                }
                //  Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                //}

                var gridData = result.Select(ScannedFileLogTableModel => new
                {
                    FRN = ScannedFileLogTableModel.FRN,
                    SroName = ScannedFileLogTableModel.SroName,
                    FileName = ScannedFileLogTableModel.FileName,
                    Filepath = ScannedFileLogTableModel.Filepath,
                    DownloadedBY = ScannedFileLogTableModel.DownloadedBY,
                    DownloadReason = ScannedFileLogTableModel.DownloadReason,
                    DownloadDateTime = ScannedFileLogTableModel.DownloadDateTime
                });

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount
                        //PDFDownloadBtn = PDFDownloadBtn,
                        //ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = result.Count(),
                        status = "1",
                        recordsFiltered = result.Count()
                        //PDFDownloadBtn = PDFDownloadBtn,
                        //ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Scanned File Download Report." }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Validates Search Parameters
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ValidateParameters(int SROfficeID, int DROfficeID, long DocumentNumber, int BookTypeID, string FinancialYear, string DType,int MarriageTypeID,int DownloadReasonID)
        {
            try
            {
               
                if (DocumentNumber <= 0 )
                {
                    return Json(new { success = false, errorMessage = "Please enter Correct Document Number" }, JsonRequestBehavior.AllowGet);
                }
                //Commented By mayank on 24/Mar2022
                //else if (string.IsNullOrEmpty(DownloadReason))
                //{
                //    return Json(new { success = false, errorMessage = "Please enter Download Reason" }, JsonRequestBehavior.AllowGet);
                //}
                //if (string.IsNullOrEmpty(DownloadReason.Trim()))
                //{
                //    return Json(new { success = false, errorMessage = "Please enter valid Download Reason" }, JsonRequestBehavior.AllowGet);
                //}
                //if (DownloadReason.Trim().Length > 1000)//Character length validation for download reason
                //{
                //    return Json(new { success = false, errorMessage = "Download Reason length must be less than 1000 characters." }, JsonRequestBehavior.AllowGet);
                //}
                if(DownloadReasonID==0)
                {
                    return Json(new { success = false, errorMessage = "Please Select Download Reason" }, JsonRequestBehavior.AllowGet);
                }
                

                //System.Text.RegularExpressions.Regex regx = new Regex("^[a-zA-Z0-9-/., ]+$");
                //System.Text.RegularExpressions.Regex regxForDocumentNumber = new Regex("^[0-9]*$");
                //Match mtchDownloadReason = regx.Match(DownloadReason);

                //if (!mtchDownloadReason.Success)
                //{
                //    return Json(new { success = false, errorMessage = "Please enter valid Download Reason" }, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { success = true, errorMessage = "Error Occured while validating parameters" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, errorMessage = "Everything is valid" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Downloads the Scanned File
        /// </summary>
        /// <returns></returns>
        //public ActionResult DownloadScannedFile(int SROfficeID, int DROfficeID, long DocumentNumber, int BookTypeID, string FinancialYear, string DType,int DownloadReasonID,int DocumentTypeID,int MarriageTypeID)
        public ActionResult DownloadScannedFile(ScannedFileDownloadView ReqModel)
        {
            ScannedFileDownloadResModel ResModel = new ScannedFileDownloadResModel();
            try
            {
                long UserId=KaveriSession.Current.UserID;
                string errorMessage = string.Empty;
                caller = new ServiceCaller("ScannedFileDownloadAPIController");
                //ScannedFileDownloadView ReqModel = new ScannedFileDownloadView();
                //ReqModel.DownloadReason = DownloadReason.Trim();
                //Added By Tushar on 23 March 2022
                //ReqModel.ReasonID = DownloadReasonID;
                //ReqModel.DocumentTypeID = DocumentTypeID;
                ////End BY Tushar on 23 March 2022
                //ReqModel.SROfficeID = SROfficeID;
                //ReqModel.DROfficeID = DROfficeID;
                //ReqModel.DocumentNumber = DocumentNumber;
                //ReqModel.BookTypeID = BookTypeID;
                //ReqModel.FinancialYearStr = FinancialYear;
                ReqModel.UserID = KaveriSession.Current.UserID;
                //ReqModel.DType = DType;
                //string DType = "PDF";
                //ReqModel.MarriageTypeID = MarriageTypeID;

                if (ReqModel.SROfficeID <= 0)
                {
                    return Json(new { success = false, message = "Please select valid Office Details" }, JsonRequestBehavior.AllowGet);
                }
                if (ReqModel.DROfficeID <= 0)
                {
                    return Json(new { success = false, message = "Please select valid Office Details" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(ReqModel.FinancialYearStr))
                {
                    return Json(new { success = false, message = "Please select valid Financial Year" }, JsonRequestBehavior.AllowGet);

                }
                if (ReqModel.DocumentNumber <= 0 && ReqModel.DocumentTypeID == 1)
                {
                    return Json(new { success = false, message = "Please enter valid Document Number" }, JsonRequestBehavior.AllowGet);
                }
                if (ReqModel.DocumentNumber <= 0 && ReqModel.DocumentTypeID == 2)
                {
                    return Json(new { success = false, message = "Please enter valid Marriage Case No" }, JsonRequestBehavior.AllowGet);
                }
                if (ReqModel.DocumentNumber <= 0 && ReqModel.DocumentTypeID == 3)
                {
                    return Json(new { success = false, message = "Please enter valid Notice Number" }, JsonRequestBehavior.AllowGet);
                }
                
                if (string.IsNullOrEmpty(ReqModel.DType))
                {
                    return Json(new { success = false, message = "Please select Download type" }, JsonRequestBehavior.AllowGet);
                }
                if (ReqModel.BookTypeID == 0 && ReqModel.DocumentTypeID == 1)
                {
                    return Json(new { success = false, message = "Please select valid Book Type" }, JsonRequestBehavior.AllowGet);

                }
                if (ReqModel.MarriageTypeID == 0 && ReqModel.DocumentTypeID == 2)
                {
                    return Json(new { success = false, message = "Please select valid Marriage Type" }, JsonRequestBehavior.AllowGet);
                }
                if(ReqModel.NoticeTypeListID ==0 && ReqModel.DocumentTypeID == 3)
                {
                    return Json(new { success = false, message = "Please select valid Notice Type" }, JsonRequestBehavior.AllowGet);
                }
               
                //Commented By mayank on 24/Mar2022
                //else if (string.IsNullOrEmpty(DownloadReason))
                //{
                //    return Json(new { success = false, errorMessage = "Please enter Download Reason" }, JsonRequestBehavior.AllowGet);
                //}
                //if (string.IsNullOrEmpty(DownloadReason.Trim()))
                //{
                //    return Json(new { success = false, errorMessage = "Please enter valid Download Reason" }, JsonRequestBehavior.AllowGet);
                //}
                //if (DownloadReason.Trim().Length > 1000)//Character length validation for download reason
                //{
                //    return Json(new { success = false, errorMessage = "Download Reason length must be less than 1000 characters." }, JsonRequestBehavior.AllowGet);
                //}
                if (ReqModel.ReasonID == 0)
                {
                    return Json(new { success = false, message = "Please Select Download Reason" }, JsonRequestBehavior.AllowGet);
                }


                ResModel = caller.PostCall<ScannedFileDownloadView, ScannedFileDownloadResModel>("GetScannedFileByteArray", ReqModel, out errorMessage);
                string FileName= string.Format("ScannedFileDownload.enc");
                bool LogStatus = false;

                if (ResModel.IsError)
                {
                    //return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = ResModel.ErrorMessage, URLToRedirect = "/Utilities/ScannedFileDownload/ScannedFileDownloadView" });
                    return Json(new {message=ResModel.ErrorMessage, serverError = true,success=false }, JsonRequestBehavior.AllowGet);
                }

                //if (ResModel != null && ResModel.ScannedFileByteArray != null)
                //{
                //    string ScannedFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", FileName));
                //    FileInfo templateFile = GetFileInfo(ScannedFilePath);
                //    LogStatus = caller.PostCall<ScannedFileDownloadView, bool>("SaveScannedFileDownloadDetails", ReqModel);
                //}
                //else
                //{
                //    //return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "No data found for entered Document Number.", URLToRedirect = "/Utilities/ScannedFileDownload/ScannedFileDownloadView" });
                //    return Json(new { message = "No data found for entered Document Number.", serverError = true, success = false }, JsonRequestBehavior.AllowGet);

                //}

                //if (LogStatus)
                if (ResModel != null && ResModel.ScannedFileByteArray != null)
                {
                    Response.AppendCookie(new HttpCookie("fileDownload", "true") { Path = "/", HttpOnly = false });

                    if (ReqModel.DType == "PDF")
                    {
                        KaveriSession.Current.UserName = ResModel.UserName;
                        //----------------------- SAVE ENC FILE IN TEMPERORY FOLDER-------------------------
                        String fileNameWithExten = Path.GetFileName(ResModel.FileNameWithExt.Replace("*", "//"));
                        String encFilePath = HttpContext.Server.MapPath(Path.Combine("~/TempEncFilesDownload/", fileNameWithExten));
                        System.IO.File.WriteAllBytes(encFilePath, ResModel.ScannedFileByteArray); // Requires System.IO

                        //----------------------- CONVERT ENC TO TIFF-------------------------
                        ////-------------------------------------------------------------------------------------------- 

                        string filePath = string.Empty;
                        string fileName = string.Empty;
                        string watermark = string.Empty;
                        string finalFilePath = string.Empty;
                        string tempPdfFilePath = string.Empty;
                        string fileNameToProcess = string.Empty;
                        //string errorMessage = string.Empty;
                        Decrypt decryptor = new Decrypt();
                        // Tiff2Pdf tiffToPdfConverter = null;
                        PDFConversion objPDFConversion = null;
                        string decryptFilePath = HttpContext.Server.MapPath("~/TempEncFilesDownload/");
                        fileNameToProcess = ResModel.FileNameWithExt.Substring(0, ResModel.FileNameWithExt.IndexOf("."));// +@"-" + Environment.TickCount.ToString();
                        decryptor.encFilePath = encFilePath.ToString();
                        //decryptor.decFilePath = @decryptFilePath +@"\" + fileNameToProcess + ".tiff";
                        decryptor.decFilePath = @decryptFilePath + fileNameToProcess + ".tiff";
                        string TiffFileName = fileNameToProcess + ".tiff";
                        string TiffFilePath = @decryptFilePath + @"\" + fileNameToProcess + ".tiff";

                        try
                        {

                            if (decryptor.DecryptFile("fecdba9876543210", ref errorMessage) == false)
                            {
                                new CommonFunctions().DeleteFileFromTemporaryFolder(decryptor.decFilePath);
                                throw new Exception("File Decryption Failed for " + fileName.ToString() + errorMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogs.LogException(ex);
                            // Input string.
                            //const string input = "Due to some techinical problem this document cannot be viewed right now.Please try again later.";

                            //// Invoke GetBytes method.
                            //byte[] array = Encoding.ASCII.GetBytes(input);

                            //return File(array, "application/pdf");

                            return Json(new { message = "File Decryption Failed for " + fileName.ToString()+ " .Please contact administrator.", serverError = true, success = false }, JsonRequestBehavior.AllowGet);

                        }
                        #region Check for Compression Issue

                        if (!new CommonFunctions().CheckForCompressionIssue(decryptor.decFilePath, ref errorMessage))
                        {
                            try
                            {
                                tempPdfFilePath = @decryptFilePath + "Temp_" + fileNameToProcess + ".pdf";
                                objPDFConversion = new PDFConversion(@decryptor.decFilePath, tempPdfFilePath);

                            if (objPDFConversion.convertUsingImageMagick(watermark, ref errorMessage) == false)
                            {
                                throw new Exception("File conversion from Tiff to PDF Failed for " + fileName.ToString(), new Exception(errorMessage));
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogs.LogException(ex);
                            // Input string.
                            //const string input = "Due to some techinical problem this document cannot be viewed right now.Please try again later.";

                            //// Invoke GetBytes method.
                            //byte[] array = Encoding.ASCII.GetBytes(input);

                            //return File(array, "application/pdf");

                            return Json(new { message = "File conversion from Tiff to PDF Failed for " + fileName.ToString()+ " .Please contact administrator.", serverError = true, success = false }, JsonRequestBehavior.AllowGet);

                        }
                        //-------------------------Read PDF byte array from PDF file----------------------------------

                            try
                            {
                                byte[] pdfByteArray = System.IO.File.ReadAllBytes(tempPdfFilePath);
                                pdfByteArray = AddWaterMarkOnPDF("This File is downloaded from KAVERI Reports Portal on " + DateTime.Now.ToString() + " by " + ResModel.UserName + "            " + (ResModel.IsReferenceStringExist ? ResModel.ReferenceString : ""), pdfByteArray, ref errorMessage);
                                var JsonData = Json(new { message = "", serverError = false, success = true, FileContent = Convert.ToBase64String(pdfByteArray), FileName = (ResModel.FileNameWithoutExt + ".pdf") }, JsonRequestBehavior.AllowGet);
                                JsonData.MaxJsonLength = Int32.MaxValue;
                                return JsonData;
                            }
                            catch (Exception ex)
                            {
                                ExceptionLogs.LogException(ex);
                                return Json(new { message = "Due to some techinical problem this document cannot be viewed right now.Please contact administrator.", serverError = true, success = false }, JsonRequestBehavior.AllowGet);
                            }
                            finally
                            {
                                new CommonFunctions().DeleteFileFromTemporaryFolder(tempPdfFilePath);
                            }
                        }
                        else
                        {
                            string ENCcorruptedCCFileName = URLEncrypt.EncryptParameters(new string[] { "FileName=" + decryptor.decFilePath });
                            ENCcorruptedCCFileName = HttpUtility.UrlEncode(ENCcorruptedCCFileName);
                            string CompromisedTiffFilePath = System.Configuration.ConfigurationManager.AppSettings["ScannedCorruptedFileByteArrayPath"] + ENCcorruptedCCFileName;

                            return Json(new
                            {
                                serverError = false,
                                status = true,
                                CompromisedTiffFilePath = CompromisedTiffFilePath,
                                FinalRegistrationNumber = ResModel.FileNameWithoutExt,
                                ErrorDesc = "TiffConversionError",
                                TiffConversionError = true,
                                ReferenceString = ResModel.IsReferenceStringExist ? ResModel.ReferenceString : "",
                            }, JsonRequestBehavior.AllowGet);
                        }
                        #endregion
                    }
                    else
                    {
                        //return File(ResModel.ScannedFileByteArray, System.Net.Mime.MediaTypeNames.Application.Octet, ResModel.FileNameWithoutExt + ".enc");
                        //return Json(new { message = "", serverError = false, success = true,FileContent=Convert.ToBase64String(ResModel.ScannedFileByteArray ),FileName=(ResModel.FileNameWithoutExt+".enc")}, JsonRequestBehavior.AllowGet);
                        var JsonData = Json(new { message = "", serverError = false, success = true,FileContent=Convert.ToBase64String(ResModel.ScannedFileByteArray ),FileName=(ResModel.FileNameWithoutExt+".enc")}, JsonRequestBehavior.AllowGet);
                        JsonData.MaxJsonLength = Int32.MaxValue;
                        return JsonData;
                    }
                }
                else
                {
                    //return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing.", URLToRedirect = "/Utilities/ScannedFileDownload/ScannedFileDownloadView" });
                    //return Json(new { message = "Error occured while processing.", serverError = true, success = false }, JsonRequestBehavior.AllowGet);
                    return Json(new { message = "No data found for entered Document Number.", serverError = true, success = false }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                //return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Scanned File.", URLToRedirect = "/Utilities/ScannedFileDownload/ScannedFileDownloadView" });
                return Json(new { message = "Error occured while downloading Scanned File.Please contact administrator", serverError = true, success = false }, JsonRequestBehavior.AllowGet);
            }

        }


        //static string CalculateMD5(string filename)
        //{
        //    using (var md5 = MD5.Create())
        //    {
        //        using (var stream = File.OpenRead(filename))
        //        {
        //            var hash = md5.ComputeHash(stream);
        //            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        //        }
        //    }
        //}

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
        public ActionResult GetTIFFFileByteArray(string EncParams)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                string[] tempFileName = EncParams.Split('/');
                byte[] bytes;
                String filePath = string.Empty;
                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { tempFileName[0], tempFileName[1], tempFileName[2] });
                string FilePath = decryptedParameters["FileName"];
                bytes = System.IO.File.ReadAllBytes(FilePath);


                return File(bytes, "application/octet-stream");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult GetCorrectedPDFFileName(List<String> BaseImageList, string FinalRegistrationNumber, string ReferenceString)
        {
            try
            {
                CorrectedPdfModel CorrectedPDFModel = new CorrectedPdfModel();
                CorrectedPDFModel.BaseImageList = BaseImageList;
                CorrectedPDFModel.FinalRegistrationNumber = FinalRegistrationNumber;
                string FilePathWithEncFileName = string.Empty;
                string FileName = this.GetCorrectedPDFFileName(CorrectedPDFModel);
                var JsonData = new JsonResult();
                try
                {
                    string errorMessage = string.Empty;
                    byte[] pdfByteArray = System.IO.File.ReadAllBytes(FileName);
                    pdfByteArray = AddWaterMarkOnPDF("This File is downloaded from KAVERI Reports Portal on " + DateTime.Now.ToString() + " by " + KaveriSession.Current.UserName + "            " + (ReferenceString), pdfByteArray, ref errorMessage);
                    if (errorMessage == string.Empty)
                        JsonData = Json(new { message = "", serverError = false, success = true, FileContent = Convert.ToBase64String(pdfByteArray), FileName = (FinalRegistrationNumber + ".pdf") }, JsonRequestBehavior.AllowGet);
                    else
                        JsonData = Json(new { message = "ErrorCode 0001 " + System.Environment.NewLine + "Error Occured while reading file,Please contact admin", serverError = false, success = false, FileContent = string.Empty, FileName = (FinalRegistrationNumber + ".pdf") }, JsonRequestBehavior.AllowGet);

                    JsonData.MaxJsonLength = Int32.MaxValue;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    new CommonFunctions().DeleteFileFromTemporaryFolder(FileName);
                }
                return JsonData;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                var JsonData = Json(new { message = "ErrorCode 0002 " + System.Environment.NewLine + "Error Occured while reading file,Please contact admin", serverError = false, success = false, FileContent = string.Empty, FileName = (FinalRegistrationNumber + ".pdf") }, JsonRequestBehavior.AllowGet);
                return JsonData;
            }
        }

        public string GetCorrectedPDFFileName(CorrectedPdfModel CorrectedPDFModel)
        {
            #region Variable Declarations
            string errorMessage = string.Empty;
            string fileName = string.Empty;
            CommonFunctions objCommonFunctions = new CommonFunctions();
            string CorrectedPdfFilePath = string.Empty;
            string CorrectedTiffFilePath = string.Empty;
            System.Drawing.Bitmap bm = null;
            string CorrectedPdfFileName = CorrectedPDFModel.FinalRegistrationNumber + ".pdf";
            #endregion

            try
            {
                #region Variable declarations
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.SaveFlag;
                ImageCodecInfo encoderInfo = ImageCodecInfo.GetImageEncoders().First(i => i.MimeType == "image/tiff");
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.MultiFrame);
                string DirectoryPath = HttpContext.Server.MapPath(Path.Combine("~/TempTiffFileDownload/"));
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                string Corrected_TiffFileName = CorrectedPDFModel.FinalRegistrationNumber + ".tiff";
                CorrectedTiffFilePath = System.IO.Path.Combine(DirectoryPath, Corrected_TiffFileName);
                Bitmap firstImage = null;
                string finalFilePath = string.Empty;
                string CompromisedTIFFFileName = CorrectedPDFModel.FinalRegistrationNumber + ".pdf";
                string CorrectedTempPdfName = "CorrectedTemp_" + CorrectedPDFModel.FinalRegistrationNumber + ".pdf";
                string CompromisedTiffFilePath = System.IO.Path.Combine(DirectoryPath, CompromisedTIFFFileName);
                string CorrectedTempPdfPath = System.IO.Path.Combine(DirectoryPath, CorrectedTempPdfName);
                CorrectedPdfFilePath = System.IO.Path.Combine(DirectoryPath, CorrectedPdfFileName);
                #region Code To Save Corrected Multi Page TIFF FIle
                try
                {

                    using (MemoryStream ms1 = new MemoryStream())
                    {
                        using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(CorrectedPDFModel.BaseImageList[0].Replace("data:image/png;base64,", ""))))
                        {
                            Image.FromStream(ms).Save(ms1, ImageFormat.Tiff);
                            firstImage = (Bitmap)Image.FromStream(ms1);
                        }
                        firstImage.Save(CorrectedTiffFilePath, encoderInfo, encoderParameters); //throws Generic GDI+ error if the memory streams are not open when this is called
                    }

                    encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.FrameDimensionPage);

                    Bitmap imagePage;
                    for (int i = 1; i < CorrectedPDFModel.BaseImageList.Count; i++)
                    {
                        using (MemoryStream ms1 = new MemoryStream())
                        {
                            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(CorrectedPDFModel.BaseImageList[i].Replace("data:image/png;base64,", ""))))
                            {
                                Image.FromStream(ms).Save(ms1, ImageFormat.Tiff);
                                imagePage = (Bitmap)Image.FromStream(ms1);
                            }
                            firstImage.SaveAdd(imagePage, encoderParameters); //throws Generic GDI+ error if the memory streams are not open when this is called
                        }
                    }
                }
                catch (Exception ex)
                {
                    firstImage.Dispose();
                    throw;
                }
                finally
                {
                    encoderParameters.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.Flush);
                    firstImage.SaveAdd(encoderParameters);
                    objCommonFunctions.DeleteFileFromTemporaryFolder(CompromisedTiffFilePath);
                }
                #endregion

                #region Code to save Corrected PDF FILE
                try
                {

                    iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 0, 0, 0, 0);
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, new System.IO.FileStream(CorrectedTempPdfPath, System.IO.FileMode.Create));
                    bm = new System.Drawing.Bitmap(CorrectedTiffFilePath);
                    int total = bm.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
                    document.Open();
                    iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;
                    for (int k = 0; k < total; ++k)
                    {
                        bm.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, k);
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(bm, System.Drawing.Imaging.ImageFormat.Bmp);
                        img.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                        img.SetAbsolutePosition(0, 0);
                        cb.AddImage(img);
                        document.NewPage();
                    }
                    document.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    bm.Dispose();
                    objCommonFunctions.DeleteFileFromTemporaryFolder(CorrectedTiffFilePath);
                }
                #endregion
                return CorrectedTempPdfPath;
                #endregion
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                bm.Dispose();
            }
        }

        [HttpPost]
        public ActionResult ConvertToPDFUsingImageMagick(string FinalRegistrationNumber, string ReferenceString)
        {
            try
            {
                ImgMagickPDFConversionModel CorrectedPDFModel = new ImgMagickPDFConversionModel();
                ImgMagickPDFConversionRetModel CorrectedPDFRetModel = new ImgMagickPDFConversionRetModel();
                var JsonData = new JsonResult();
                String FilePathWithEncFileName = String.Empty;
                CorrectedPDFModel.FinalRegistrationNumber = FinalRegistrationNumber;
                CorrectedPDFRetModel = this.ConvertToPDFUsingImageMagick(CorrectedPDFModel);
                if (CorrectedPDFRetModel.Status)
                {
                    try
                    {
                        string errorMessage = string.Empty;
                        byte[] pdfByteArray = System.IO.File.ReadAllBytes(CorrectedPDFRetModel.CorrectedPDFFilePath);
                        pdfByteArray = AddWaterMarkOnPDF("This File is downloaded from KAVERI Reports Portal on " + DateTime.Now.ToString() + " by " + KaveriSession.Current.UserName + "            " + (ReferenceString), pdfByteArray, ref errorMessage);
                        if (errorMessage == string.Empty)
                            JsonData = Json(new { message = "", serverError = false, success = true, FileContent = Convert.ToBase64String(pdfByteArray), FileName = (FinalRegistrationNumber + ".pdf") }, JsonRequestBehavior.AllowGet);
                        else
                            JsonData = Json(new { message = "ErrorCode 0003 " + System.Environment.NewLine + "Error Occured while reading file,Please try again later", serverError = false, success = false, FileContent = string.Empty, FileName = (FinalRegistrationNumber + ".pdf") }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        new CommonFunctions().DeleteFileFromTemporaryFolder(CorrectedPDFRetModel.CorrectedPDFFilePath);
                    }
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    JsonData = Json(new { message = "ErrorCode 0004 " + System.Environment.NewLine + "Error Occured while reading file,Please try again later", serverError = false, success = false, FileContent = string.Empty, FileName = (FinalRegistrationNumber + ".pdf") }, JsonRequestBehavior.AllowGet);
                    return JsonData;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                var JsonData = Json(new { message = "ErrorCode 0005 " + System.Environment.NewLine + "Error Occured while reading file,Please try again later", serverError = false, success = false, FileContent = string.Empty, FileName = (FinalRegistrationNumber + ".pdf") }, JsonRequestBehavior.AllowGet);
                return JsonData;

            }
        }

        public ImgMagickPDFConversionRetModel ConvertToPDFUsingImageMagick(ImgMagickPDFConversionModel CorrectedPDFModel)
        {
            ImgMagickPDFConversionRetModel CorrectedPDFRetModel = new ImgMagickPDFConversionRetModel();
            string CorruptedTIFFFileName = CorrectedPDFModel.FinalRegistrationNumber;
            string CorrectedPDFFileName = CorruptedTIFFFileName + ".pdf";
            try
            {
                string errorMessage = "";
                string RootDirectory = HttpContext.Server.MapPath("~/TempEncFilesDownload/");
                string TempPDFFileName = CorrectedPDFModel.FinalRegistrationNumber + "_Temp";
                string CorruptedTIFFFilePath = Path.Combine(RootDirectory, CorruptedTIFFFileName + ".tiff");
                string CorrectedPdfFilePath = System.IO.Path.Combine(RootDirectory, CorruptedTIFFFileName + ".pdf");
                CommonFunctions objCommonFunctions = new CommonFunctions();
                PDFConversionUsingImageMagick objPDFConversion = new PDFConversionUsingImageMagick(CorruptedTIFFFilePath, CorrectedPdfFilePath);
                if (objPDFConversion.convertUsingImageMagick(string.Empty, ref errorMessage) == false)
                {
                    CorrectedPDFRetModel.Status = false;
                    CorrectedPDFRetModel.serverError = false;
                    CorrectedPDFRetModel.IsImgMagickConversionError = true;
                    CorrectedPDFRetModel.CorrectedPDFFilePath = CorrectedPdfFilePath;
                    CorrectedPDFRetModel.CorrectedPDFFileName = CorrectedPDFFileName;
                    return CorrectedPDFRetModel;
                }
                CorrectedPDFRetModel.CorrectedPDFFilePath = CorrectedPdfFilePath;
                CorrectedPDFRetModel.CorrectedPDFFileName = CorrectedPDFFileName;
                CorrectedPDFRetModel.IsImgMagickConversionError = false;
                CorrectedPDFRetModel.Status = true;
                CorrectedPDFRetModel.serverError = false;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
            }
            return CorrectedPDFRetModel;
        }

        #region Added by mayank  to add remark
        public byte[] AddWaterMarkOnPDF(string watermarkToEmbed, byte[] FileToWrite, ref string errorMessage)
        {

            using (MemoryStream outputms = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(FileToWrite))
                {
                    PdfSharp.Pdf.PdfDocument doc = PdfSharp.Pdf.IO.PdfReader.Open(ms, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify);// PdfDocumentOpenMode.Modify 

                    for (int i = 0; i < doc.PageCount; i++)
                    {

                        if (watermarkToEmbed.Length > 0)
                        {
                            PdfSharp.Pdf.PdfPage page = doc.Pages[i];
                            PdfSharp.Drawing.XGraphics gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page, PdfSharp.Drawing.XGraphicsPdfPageOptions.Append);
                            gfx.TranslateTransform(page.Width / 2, page.Height / 2);
                            gfx.TranslateTransform(-page.Width / 2, -(page.Height * 0.10));
                            PdfSharp.Drawing.XGraphicsPath path = new PdfSharp.Drawing.XGraphicsPath();

                            // Create a dimmed red brush
                            PdfSharp.Drawing.XBrush brush = new PdfSharp.Drawing.XSolidBrush(PdfSharp.Drawing.XColor.FromArgb(128, 255, 0, 0));
                            PdfSharp.Drawing.XFont font = new PdfSharp.Drawing.XFont("Times New Roman", 12, PdfSharp.Drawing.XFontStyle.Italic);

                            PdfSharp.Drawing.XPen pen = new PdfSharp.Drawing.XPen(PdfSharp.Drawing.XColor.FromArgb(128, 255, 0, 0), 0);

                            int intcnt = 0;
                            int substringLength = 100;

                            int position = 0;
                            position = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(watermarkToEmbed.Length) / substringLength) / 2);
                            position = -position * 10;

                            int offset = 0;
                            string watermark = string.Empty;
                            do
                            {
                                offset = watermarkToEmbed.Substring(intcnt).Length >= substringLength ? substringLength : watermarkToEmbed.Substring(intcnt).Length;
                                watermark = watermarkToEmbed.Substring(intcnt, offset);


                                PdfSharp.Drawing.XSize size = gfx.MeasureString(watermark, font);

                                if (!string.IsNullOrEmpty(watermark))
                                {
                                    path.AddString(watermark, font.FontFamily, font.Style, font.Size,
                                    new PdfSharp.Drawing.XPoint(((page.Width - size.Width) / 2), ((page.Height - size.Height) / 2) + position), PdfSharp.Drawing.XStringFormat.TopLeft);
                                }
                                position = position + 15;

                                intcnt = intcnt + (watermarkToEmbed.Substring(intcnt).Length >= substringLength ? substringLength : watermarkToEmbed.Substring(intcnt).Length);

                            } while (watermark != "");

                            gfx.DrawPath(pen, brush, path);
                        }
                    }
                    //ms2.Write(buffer, 0, buffer.Length);
                    doc.Save(outputms);
                    doc.Close();
                    errorMessage = string.Empty;
                }
                return outputms.GetBuffer();
            }
        }

        #endregion
        #region Added by mayank on 04/05/2022 to save Download log post completion of file download

        [HttpPost]
        public ActionResult SaveScannedFileDownloadDetails(ScannedFileDownloadView ReqModel)
        {
            try
            {
                ReqModel.UserID = KaveriSession.Current.UserID;
                caller = new ServiceCaller("ScannedFileDownloadAPIController");
                bool result = caller.PostCall<ScannedFileDownloadView, bool>("SaveScannedFileDownloadDetails", ReqModel);
                return Json(new { message = "ErrorCode 0006 " + System.Environment.NewLine + "Error Occured while saving log", serverError = false, success = false, FileContent = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        } 
        #endregion
    }
}