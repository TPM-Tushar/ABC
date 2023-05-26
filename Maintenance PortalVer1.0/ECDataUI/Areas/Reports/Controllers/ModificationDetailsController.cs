///*File Header
// * Project Id: 
// * Project Name: Kaveri Maintainance Portal
// * File Name: ModificationDetailsController.cs
// * Author :Harshit Gupta
// * Creation Date :17 Jan 2019
// * Desc : Provides methods for view and model interaction
// * ECR No : 
//*/
//#region References
//using CaptchaLib;
//using ECDataAuditDetails.BAL;
//using ECDataAuditDetails.Common;
//using ECDataAuditDetails.Models;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Web.Mvc; 
//#endregion
//namespace ECDataAuditDetails.Controllers
//{
//    public class ModificationDetailsController : Controller
//    {
//        #region User Variable & Objects
//        SROModificationDetailsBAL sROModificationDetailsBALObj = new SROModificationDetailsBAL();
//        CommonFunctions objCommon = new CommonFunctions();
//        string textSROName = "SRO Name";
//        string textDateOfEvent = "Date of Event";
//        string textModificationType = "Modification Type";
//        string textLoginName = "Login Name";
//        string textIPAddress = "IP Address";
//        string textHostName = "Host Name";
//        string textApplicationName = "Application Name";
//        string textPhysicalAddress = "MAC Address";
//        string textStatement = "Statement"; 
//        #endregion

//        /// <summary>
//        /// Returns View to input Details for View Modifications Audit Logs
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult SROModificationsDetailsHome()
//        {
//            SROModificationDetailsBAL modificationDetails = new SROModificationDetailsBAL();
//            SROModificationDetailsRequestModel reqModel = modificationDetails.GetSROModificationDetailsRequestModel();
//            return View(reqModel);
//        }

//        /// <summary>
//        /// Modal to show Masters details
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult GetModificationTypeDetails()
//        {
//            long MasterId = Convert.ToInt64(Request.Params["EncryptedMasterID"]);
//            long DetailsID = Convert.ToInt64(Request.Params["EncryptedDetailsID"]);
//            try
//            {
//                SROModificationDetailsBAL sROModificationDetailsBALObj = new SROModificationDetailsBAL();
//                SROModificationDetailsViewModel reqModel = new SROModificationDetailsViewModel();
//                reqModel.LstSROModificationDetails = sROModificationDetailsBALObj.GetModificationTypeDetails(MasterId, DetailsID);
//                return View("SROModificationTypeDetailsDetails", reqModel);
//            }
//            catch (Exception)
//            {
//                return Redirect("/ECDataAuditDetails/Error");
//            }
//        }

//        /// <summary>
//        /// Data Table Results are returned as Json Object to show details
//        /// </summary>
//        /// <param name="formCollection"></param>
//        /// <returns></returns>
//        [HttpPost]
//        public ActionResult GetModificationDetailsList(FormCollection formCollection)
//        {
//            string fromDate = formCollection["fromDate"];
//            string ToDate = formCollection["ToDate"];
//            string selectedOfc = formCollection["selectedOfc"];
//            string programs = formCollection["programs"];
//            string Captcha = formCollection["Captcha"];
//            string errorMessage = string.Empty;
//            #region Server Side Validation
//            if (string.IsNullOrEmpty(fromDate))
//            {
//                var emptyData = Json(new
//                {
//                    draw = formCollection["draw"],
//                    recordsTotal = 0,
//                    recordsFiltered = 0,
//                    data = "",
//                    status = false,
//                    errorMessage = "From Date required"

//                });

//                emptyData.MaxJsonLength = Int32.MaxValue;
//                return emptyData;
//            }
//            #region Validate Captcha
//            CaptchaLib.ValidateCaptchaAttribute obj = new ValidateCaptchaAttribute();
//            if (!obj.IsValid(Captcha))
//            {
//                var emptyData = Json(new
//                {
//                    draw = formCollection["draw"],
//                    recordsTotal = 0,
//                    recordsFiltered = 0,
//                    data = "",
//                    status = "0",
//                    errorMessage = "Captcha Entered is Invalid"
//                });
//                emptyData.MaxJsonLength = Int32.MaxValue;
//                return emptyData;

//            }
//            #endregion
//            if (string.IsNullOrEmpty(ToDate))
//            {
//                var emptyData = Json(new
//                {
//                    draw = formCollection["draw"],
//                    recordsTotal = 0,
//                    recordsFiltered = 0,
//                    data = "",
//                    status = "0",
//                    errorMessage = "To Date required"

//                });
//                emptyData.MaxJsonLength = Int32.MaxValue;
//                return emptyData;

//            }
//            if (string.IsNullOrEmpty(programs))
//            {
//                var emptyData = Json(new
//                {
//                    draw = formCollection["draw"],
//                    recordsTotal = 0,
//                    recordsFiltered = 0,
//                    data = "",

//                    status = "0",
//                    errorMessage = "Select a program"

//                });
//                emptyData.MaxJsonLength = Int32.MaxValue;
//                return emptyData;

//            }
//            #endregion
//            DateTime frmDate,toDate;
//            bool boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
//            bool boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
//            #region Date Validation
//            if (!boolFrmDate)
//            {
//                var emptyData = Json(new
//                {
//                    draw = formCollection["draw"],
//                    recordsTotal = 0,
//                    recordsFiltered = 0,
//                    data = "",

//                    status = "0",
//                    errorMessage = "Invalid From Date"

//                });
//                emptyData.MaxJsonLength = Int32.MaxValue;
//                return emptyData;
//            }
//            if (!boolToDate)
//            {
//                var emptyData = Json(new
//                {
//                    draw = formCollection["draw"],
//                    recordsTotal = 0,
//                    recordsFiltered = 0,
//                    data = "",
//                    status = "0",
//                    errorMessage = "Invalid To Date"
//                });
//                emptyData.MaxJsonLength = Int32.MaxValue;
//                return emptyData;
//            }
//            if (frmDate > toDate)
//            {
//                var emptyData = Json(new
//                {
//                    draw = formCollection["draw"],
//                    recordsTotal = 0,
//                    recordsFiltered = 0,
//                    data = "",
//                    status = "0",
//                    errorMessage = "From Date can not be larger than To Date"

//                });
//                emptyData.MaxJsonLength = Int32.MaxValue;
//                return emptyData;
//            }
//            #endregion
//            ECDataAuditDetailsRequestModel model = new ECDataAuditDetailsRequestModel
//            {
//                Datetime_FromDate = frmDate,
//                Datetime_ToDate = toDate,
//                OfficeID = Convert.ToInt32(selectedOfc),
//                programs = programs
//            };
//            var result = sROModificationDetailsBALObj.GetModificationDetailsList(model);
//            if(result.Count==0)
//            {
//                var emptyData = Json(new
//                {
//                    draw = formCollection["draw"],
//                    recordsTotal = 0,
//                    recordsFiltered = 0,
//                    data = "",
//                    status = false,
//                    errorMessage = "No results Found For the Current Input!Please try again"

//                });
//                emptyData.MaxJsonLength = Int32.MaxValue;
//                return emptyData;
//            }
//            var JsonData = Json(new
//            {
//                draw = formCollection["draw"],
//                data = result.ToArray(),
//                recordsTotal = result.Count,
//                status = "1",
//                recordsFiltered = result.Count,

//            });
//            JsonData.MaxJsonLength = Int32.MaxValue;
//            return JsonData;
//        }

//        #region PDF
//        /// <summary>
//        /// Following Method returns PDF File Result
//        /// </summary>
//        /// <param name="FromDate"></param>
//        /// <param name="ToDate"></param>
//        /// <param name="SelectedOfc"></param>
//        /// <param name="Programs"></param>
//        /// <param name="OfficeName"></param>
//        /// <returns></returns>
//        public ActionResult ExportModificationDetailsLogList(string FromDate, string ToDate, string SelectedOfc, string Programs, string OfficeName)
//        {
//                try
//                {
//                    DateTime frmDate,toDate;
//                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
//                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
//                ECDataAuditDetailsRequestModel model = new ECDataAuditDetailsRequestModel
//                {
//                    Datetime_FromDate = frmDate,
//                    Datetime_ToDate = toDate,
//                    OfficeID = Convert.ToInt32(SelectedOfc),
//                    OfficeName = OfficeName,
//                    programs = Programs
//                };
//                List<SROModificationDetailsViewModel> objListItemsToBeExported = new List<SROModificationDetailsViewModel>();
//                objListItemsToBeExported = sROModificationDetailsBALObj.GetModificationDetailsAllList(model);
//                string fileName = string.Format("{0}{1}_{2}_{3}.pdf", OfficeName, DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
//                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
//                string pdfHeader = string.Format("SRO Modification Details Log (Between {0} and {1})", FromDate, ToDate);
//                //Create Temp PDF File
//                string createdPDFPath = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader);
//                //Then Encrypt it
//                string passwordProtectedPDFFilePath = AddPasswordToPDF(createdPDFPath, filepath);
//                //Delete Temp File
//                objCommon.DeleteFileFromTemporaryFolder(createdPDFPath);
//                byte[] pdfBinary = System.IO.File.ReadAllBytes(passwordProtectedPDFFilePath);
//                //Delete Password Protected File Too
//                objCommon.DeleteFileFromTemporaryFolder(passwordProtectedPDFFilePath);
//                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
//            }
//            catch (Exception)
//            {
//                return Redirect("/ECDataAuditDetails/Error");
//            }
//        }

//        /// <summary>
//        /// Addes Password to the Already Created PDF
//        /// </summary>
//        /// <param name="tempFilePathOfFileTobePasswordProtected"></param>
//        /// <param name="DownloadableFilePath"></param>
//        /// <returns></returns>
//        private string AddPasswordToPDF(string tempFilePathOfFileTobePasswordProtected,string DownloadableFilePath)
//        {
//            try
//            {
//                using (Stream input = new FileStream(tempFilePathOfFileTobePasswordProtected, FileMode.Open, FileAccess.Read, FileShare.Read))
//                {
//                    using (Stream output = new FileStream(DownloadableFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
//                    {
//                        PdfReader reader = new PdfReader(input);
//                        PdfEncryptor.Encrypt(reader, output, true, "test", "secret", PdfWriter.ALLOW_SCREENREADERS);
//                    }
//                }
//                return DownloadableFilePath;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        /// <summary>
//        /// Created PDF iterating over a list of objects
//        /// </summary>
//        /// <param name="objListItemsToBeExported"></param>
//        /// <param name="fileName"></param>
//        /// <param name="pdfHeader"></param>
//        /// <returns></returns>
//        private string CreatePDFFile(List<SROModificationDetailsViewModel> objListItemsToBeExported, string fileName, string pdfHeader)
//        {
//            //Temporary PDF
//            string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", "Temp"+fileName));
//            try
//            {
//                //Create PDF
//                using (Stream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None))
//                {
//                    using (Document doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10))
//                    {
//                        #region Prepare Header and Styling for PDF
//                        //Take a custom header font
//                        var headerTextFont = FontFactory.GetFont("Arial", 15, new BaseColor(0, 128, 255));
//                        PdfWriter writer = PdfWriter.GetInstance(doc, fs);
//                        doc.Open();
//                        //Align it to center
//                        Paragraph addHeading = new Paragraph(pdfHeader, headerTextFont)
//                        {
//                            Alignment = 1,
//                        };
//                        //Add a blank Space
//                        Paragraph addSpace = new Paragraph(" ")
//                        {
//                            Alignment = 1
//                        };
//                        var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
//                        var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(255, 51, 51));
//                        var titleChunk = new Chunk("Log Creation DateTime : ", blackListTextFont);
//                        var descriptionChunk = new Chunk(DateTime.Now.ToString(), redListTextFont);
//                        var phrase = new Phrase(titleChunk)
//                        {
//                            descriptionChunk
//                        };
//                        doc.Add(addHeading);
//                        doc.Add(addSpace);
//                        doc.Add(phrase); 
//                        #endregion
//                        //Table Data
//                        doc.Add(_stateTable(objListItemsToBeExported));
//                        doc.Close();
//                    }
//                }
//                return filepath;
//            }
//            catch (Exception)
//            {
//                return "";
//            }
//        }

//        /// <summary>
//        /// Writes Table Content to PDF
//        /// </summary>
//        /// <param name="objListItemsToBeExported"></param>
//        /// <returns></returns>
//        private PdfPTable _stateTable(List<SROModificationDetailsViewModel> objListItemsToBeExported)
//        {
//            string[] col = { textSROName, textDateOfEvent, textModificationType, textLoginName, textIPAddress, textHostName, textApplicationName, textPhysicalAddress, textStatement };
//            PdfPTable table = new PdfPTable(9)
//            {
//                /*
//                * default table width => 80%
//                */
//                WidthPercentage = 100
//            };
//            //to repeat Headers on each page
//            table.HeaderRows = 1;
//            // then set the column's __relative__ widths
//            table.SetWidths(new Single[] { 3, 3, 2, 2, 2, 2, 3, 3, 6 });
//            /*
//            * by default tables 'collapse' on surrounding elements,
//            * so you need to explicitly add spacing
//            */
//            for (int i = 0; i < col.Length; ++i)
//            {
//                PdfPCell cell = new PdfPCell(new Phrase(col[i]))
//                {
//                    BackgroundColor = new BaseColor(204, 255, 255)
//                };
//                table.AddCell(cell);
//            }
//            foreach (var items in objListItemsToBeExported)
//            {
//                table.AddCell(items.SroName);
//                table.AddCell(items.DateOfEvent);
//                table.AddCell(items.ModificationType);
//                table.AddCell(items.Loginname);
//                table.AddCell(items.IPAddress);
//                table.AddCell(items.HostName);
//                table.AddCell(items.ApplicationName);
//                table.AddCell(items.PhysicalAddress);
//                table.AddCell(items.Statement);
//            }
//            return table;
//        }
//        #endregion

//        #region Excel
//        /// <summary>
//        /// Following Methods returns Excel File Result
//        /// </summary>
//        /// <param name="FromDate"></param>
//        /// <param name="ToDate"></param>
//        /// <param name="SelectedOfc"></param>
//        /// <param name="Programs"></param>
//        /// <param name="OfficeName"></param>
//        /// <returns></returns>
//        public ActionResult ExportModificationDetailsLogListToExcel(string FromDate, string ToDate, string SelectedOfc, string Programs, string OfficeName)
//        {
//            try
//            {
//                string fileName = string.Format("{0}{1}_{2}_{3}.xlsx", OfficeName, DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
//                DateTime frmDate, toDate;
//                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out  frmDate);
//                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
//                ECDataAuditDetailsRequestModel model = new ECDataAuditDetailsRequestModel
//                {
//                    Datetime_FromDate = frmDate,
//                    Datetime_ToDate = toDate,
//                    OfficeID = Convert.ToInt32(SelectedOfc),
//                    programs = Programs
//                };
//                List<SROModificationDetailsViewModel> objListItemsToBeExported = new List<SROModificationDetailsViewModel>();
//                objListItemsToBeExported = sROModificationDetailsBALObj.GetModificationDetailsAllList(model);
//                string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/",fileName));
//                string excelHeader = string.Format("SRO Modification Details Log (Between {0} and {1})", FromDate, ToDate);
//                string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader);
//                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
//                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
//                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
//            }
//            catch (IOException)
//            {
//                return Redirect("/ECDataAuditDetails/Error");
//            }
//            catch (Exception)
//            {
//                return Redirect("/ECDataAuditDetails/Error");
//            }
//        }

//        /// <summary>
//        /// Creates Excel By Iterating over a list of objects
//        /// </summary>
//        /// <param name="objListItemsToBeExported"></param>
//        /// <param name="fileName"></param>
//        /// <param name="excelHeader"></param>
//        /// <returns></returns>
//        private string CreateExcel(List<SROModificationDetailsViewModel> objListItemsToBeExported, string fileName,string excelHeader)
//        {
//            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/",fileName));
//            FileInfo templateFile= GetFileInfo(ExcelFilePath);
//            try
//            {
//                using (ExcelPackage package = new ExcelPackage(templateFile))
//                {
//                    #region  Lock the workbook totally
//                    var workbook = package.Workbook;
//                    //workbook.Protection.LockWindows = true;
//                    //workbook.Protection.LockStructure = true;
//                    //workbook.View.SetWindowSize(350, 725, 18500, 6000);
//                    //workbook.View.ShowHorizontalScrollBar = false;
//                    //workbook.View.ShowVerticalScrollBar = false;
//                    workbook.View.ShowSheetTabs = false;
//                    //Set a password for the workbookprotection
//                    workbook.Protection.SetPassword("test");
//                    #endregion
//                    //Create a sheet named Logs
//                    var workSheet = package.Workbook.Worksheets.Add("Logs");
//                    #region Add Headers
//                    workSheet.Cells[1, 3].Value = excelHeader;
//                    workSheet.Cells[2, 3].Value = "Log Creation DateTime :" + DateTime.Now;
//                    workSheet.Cells[5,1].Value = textSROName;
//                    workSheet.Cells[5,2].Value = textDateOfEvent;
//                    workSheet.Cells[5,3].Value = textModificationType;
//                    workSheet.Cells[5,4].Value = textLoginName;
//                    workSheet.Cells[5,5].Value = textIPAddress;
//                    workSheet.Cells[5,6].Value = textHostName;
//                    workSheet.Cells[5,7].Value = textApplicationName;
//                    workSheet.Cells[5,8].Value = textPhysicalAddress;
//                    workSheet.Cells[5,9].Value = textStatement;
//                    //Add Color to the Header
//                    using (ExcelRange Rng = workSheet.Cells[5, 1, 5, 9])
//                    {
//                        Rng.Style.Font.Bold = true;
//                        Rng.Style.Font.Color.SetColor(Color.Black);
//                        Rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                        Rng.Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
//                    }
//                    #endregion

//                    workSheet.Row(1).Style.Font.Bold = true;
//                    workSheet.Row(5).Style.Font.Bold = true;
//                    int row = 6;
//                    foreach (var items in objListItemsToBeExported)
//                    {
//                        workSheet.Cells[row, 1].Value = items.SroName;
//                        workSheet.Cells[row, 2].Value = items.DateOfEvent;
//                        workSheet.Cells[row, 3].Value = items.ModificationType;
//                        workSheet.Cells[row, 4].Value = items.Loginname;
//                        workSheet.Cells[row, 5].Value = items.IPAddress;
//                        workSheet.Cells[row, 6].Value = items.HostName;
//                        workSheet.Cells[row, 7].Value = items.ApplicationName;
//                        workSheet.Cells[row, 8].Value = items.PhysicalAddress;
//                        workSheet.Cells[row,9].Value = items.Statement;
//                        row++;
//                    }
//                    workSheet.Cells["A:XFD"].AutoFitColumns();
//                    workSheet.Protection.SetPassword("test");
//                    package.Encryption.Algorithm = EncryptionAlgorithm.AES192;   //For the answers we want a little bit stronger encryption
//                    package.SaveAs(templateFile, "test");
//                }
//            }
//            catch (Exception)
//            {
//            }
//            finally
//            {
//            }
//            return ExcelFilePath;
//        }

//        /// <summary>
//        /// Returns a File Info
//        /// </summary>
//        /// <param name="tempExcelFilePath"></param>
//        /// <returns></returns>
//        public static FileInfo GetFileInfo(string tempExcelFilePath)
//        {
//            var fi = new FileInfo(tempExcelFilePath);
//            return fi;
//        }
//        #endregion


//    }
//}