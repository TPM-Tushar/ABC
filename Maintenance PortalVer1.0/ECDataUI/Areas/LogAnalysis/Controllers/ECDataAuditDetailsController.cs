#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ECDataAuditDetailsController.cs
    * Author Name       :   Harshit	
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Log Analysis module.
*/
#endregion

using CustomModels.Models.LogAnalysis.ECDataAuditDetails;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using iTextSharp.text;
using iTextSharp.text.pdf;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;
//using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ECDataUI.Areas.LogAnalysis.Controllers
{

    [KaveriAuthorizationAttribute]
    public class ECDataAuditDetailsController : Controller
    {
        #region User Variables & Objects
        //CommonFunctions objCommon = new CommonFunctions();
        ServiceCaller caller = null;
        string txtSROName = "SRO Name";
        string txtDateOfModification = "Date Of Modification";
        string txtModificationArea = "Modification Area";
        string txtModificationType = "Modification Type";
        string txtModificationDescription = "Modification Description";
        //string txtColumnName = "Column Name";
        //string txtPrevValue = "Previous Value";
        //string txtModifiedValue = "Modified Value";
        string txtIPAddress = "IP Address";
        string txtHostName = "Host Name";
        string txtApplicationName = "Application Name";
        string txtFRN = "Final Registration Number";
        string[] col = { "Column Name", "Previous Value", "Modified Value" };
        #endregion

        /// <summary>
        /// Get Captcha Image
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCaptchaImage()
        {
            try
            {
                return CaptchaLib.ControllerExtensions.Captcha(this, new CaptchaLib.CaptchaImage(), 100, 220, 70);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request", URLToRedirect = "/Home/HomePage" });
            }

        }

        /// <summary>
        /// ECData Audit Details View
        /// </summary>
        /// <returns>return view</returns>
        [EventAuditLogFilter(Description = "ECData Audit Details View")]
        [MenuHighlightAttribute]
        public ActionResult ECDataAuditDetailsView()
        {

            try
            {
                caller = new ServiceCaller("ECDataAuditDetailsApiController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                ECDataAuditDetailsRequestModel reqModel = caller.GetCall<ECDataAuditDetailsRequestModel>("ECDataAuditDetailsRequestModel");
                // Added BY SB on 2-04-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.EcAuditDetails;
                return View(reqModel);
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request", URLToRedirect = "/Home/HomePage" });
            }
        }



        /// <summary>
        /// Get ECData Audit Details List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns ecdata audit details list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get ECData Audit Details List")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetECDataAuditDetailsList(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects
                string fromDate = formCollection["fromDate"];
                string ToDate = formCollection["ToDate"];
                string selectedOfc = formCollection["selectedOfc"];
                string PopulateOccurances = formCollection["PopulateOccurances"];
                //string Captcha = formCollection["Captcha"];
                string programs = formCollection["programs"];
                // Added by raman
                string selectedOfficeName = formCollection["selectedOfficeName"];
                CommonFunctions objCommon = new CommonFunctions();
                //IECDataAuditDetails ecDataBAL = new ECDataAuditDetailsBAL();
                String errorMessage = String.Empty;
                #endregion

                #region Server Side Validation
                if (string.IsNullOrEmpty(fromDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date required"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "To Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (string.IsNullOrEmpty(programs))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",

                        status = "0",
                        errorMessage = "Select a program"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                //if (PopulateOccurances == "1")
                //{
                //    CaptchaLib.ValidateCaptchaAttribute obj = new ValidateCaptchaAttribute();
                //    if (!obj.IsValid(Captcha))
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = "0",
                //            errorMessage = "Captcha Entered is Invalid"
                //        });


                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;

                //    }
                //}
                #endregion

                #region  For IP address to be written in controller
                //string ipAddressOfUser = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //if (string.IsNullOrEmpty(ipAddressOfUser))
                //{
                //    ipAddressOfUser = Request.ServerVariables["REMOTE_ADDR"];
                //}
                //string[] userMachineName = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
                //string machineNameOfUser = userMachineName[0].ToString();
                #endregion

                DateTime frmDate, toDate;
                bool boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                bool boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                #region Validate date Inputs
                if (!boolFrmDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid From Date"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (!boolToDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
                if (frmDate > toDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "From Date can not be larger than To Date"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                ECDataAuditDetailsRequestModel model = new ECDataAuditDetailsRequestModel
                {
                    Datetime_FromDate = frmDate,
                    Datetime_ToDate = toDate,
                    OfficeID = Convert.ToInt32(selectedOfc),
                    programs = programs
                };
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                caller = new ServiceCaller("ECDataAuditDetailsApiController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                ECDatatAuditDetailsWrapperModel wrapperModel = new ECDatatAuditDetailsWrapperModel();
                wrapperModel.Datetime_FromDate = model.Datetime_FromDate;
                wrapperModel.Datetime_ToDate = model.Datetime_ToDate;
                wrapperModel.programs = model.programs;
                wrapperModel.OfficeID = model.OfficeID;
                wrapperModel.StartLength = startLen;
                wrapperModel.TotalNum = totalNum;



                #region Office-Wise Occurrance
                List<OfficeModificationOccurenceModel> OfficeWiseOccurance = null;
                long TotModfctnCount = 0;
                if (PopulateOccurances == "1")  
                {
                    OfficeWiseOccurance = caller.PostCall<ECDatatAuditDetailsWrapperModel, List<OfficeModificationOccurenceModel>>("GetOfficeWiseModificationOccurences", wrapperModel, out errorMessage);
                    //OfficeWiseOccurance = ecDataBAL.GetOfficeWiseModificationOccurences(model);
                    TotModfctnCount = OfficeWiseOccurance.Sum(x => x.NoOfOccurances);
                }
                else
                {
                    OfficeWiseOccurance = new List<OfficeModificationOccurenceModel>();
                }


                // var result = ecDataBAL.GetECDataAuditDetailsList(model, startLen, totalNum);
                var result = caller.PostCall<ECDatatAuditDetailsWrapperModel, List<ECDataAuditDetailsResponseModel>>("ECDataAuditDetailsList", wrapperModel, out errorMessage);
                int totalCount = caller.PostCall<ECDataAuditDetailsRequestModel, int>("ECDataAuditDetailsListTotalCount", model, out errorMessage);

                //  int totalCount = ecDataBAL.GetECDataAuditDetailsList(model);

                if (result.Count == 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "No results Found For the Current Input ! Please try again"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                var gridData = result.Select(ECDataAuditDetailsResponseModel => new
                {
                    SRONAME = ECDataAuditDetailsResponseModel.SRONAME,
                    FRN = ECDataAuditDetailsResponseModel.FRN,
                    DATEOFMODIFICATION = ECDataAuditDetailsResponseModel.DATEOFMODIFICATION,
                    MODIFICATION_AREA = ECDataAuditDetailsResponseModel.MODIFICATION_AREA,
                    MODIFICATION_TYPE = ECDataAuditDetailsResponseModel.MODIFICATION_TYPE,
                    MODIFICATION_DESCRIPTION = ECDataAuditDetailsResponseModel.MODIFICATION_DESCRIPTION,
                    APPLICATION_NAME = ECDataAuditDetailsResponseModel.APPLICATION_NAME,
                    IPADRESS = ECDataAuditDetailsResponseModel.IPADRESS,
                    hostname = ECDataAuditDetailsResponseModel.hostname,
                });

                String PDFDownloadBtn = "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + fromDate + "','" + ToDate + "','" + selectedOfc + "','" + programs + "','" + selectedOfficeName + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = gridData.ToArray(),
                    recordsTotal = totalCount,
                    status = "1",
                    OfficeWiseOccurance,
                    TotModfctnCount,
                    recordsFiltered = totalCount,
                    PDFDownloadBtn= PDFDownloadBtn
                });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                var emptyData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }

        }





        //[HttpPost]
        //public ActionResult GetECDataAuditDetailsList(FormCollection formCollection)
        //{
        //    try
        //    {
        //        #region User Variables and Objects
        //        string fromDate = formCollection["fromDate"];
        //        string ToDate = formCollection["ToDate"];
        //        string selectedOfc = formCollection["selectedOfc"];
        //        string PopulateOccurances = formCollection["PopulateOccurances"];
        //        //string Captcha = formCollection["Captcha"];
        //        string programs = formCollection["programs"];
        //        //CommonFunctions objCommon = new CommonFunctions();
        //        //IECDataAuditDetails ecDataBAL = new ECDataAuditDetailsBAL();
        //        String errorMessage = String.Empty;
        //        #endregion

        //        #region Server Side Validation
        //        if (string.IsNullOrEmpty(fromDate))
        //        {
        //            var emptyData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                recordsTotal = 0,
        //                recordsFiltered = 0,
        //                data = "",
        //                status = false,
        //                errorMessage = "From Date required"

        //            });
        //            emptyData.MaxJsonLength = Int32.MaxValue;
        //            return emptyData;
        //        }
        //        if (string.IsNullOrEmpty(ToDate))
        //        {
        //            var emptyData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                recordsTotal = 0,
        //                recordsFiltered = 0,
        //                data = "",
        //                status = "0",
        //                errorMessage = "To Date required"
        //            });
        //            emptyData.MaxJsonLength = Int32.MaxValue;
        //            return emptyData;
        //        }
        //        if (string.IsNullOrEmpty(programs))
        //        {
        //            var emptyData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                recordsTotal = 0,
        //                recordsFiltered = 0,
        //                data = "",

        //                status = "0",
        //                errorMessage = "Select a program"

        //            });
        //            emptyData.MaxJsonLength = Int32.MaxValue;
        //            return emptyData;
        //        }
        //        //if (PopulateOccurances == "1")
        //        //{
        //        //    CaptchaLib.ValidateCaptchaAttribute obj = new ValidateCaptchaAttribute();
        //        //    if (!obj.IsValid(Captcha))
        //        //    {
        //        //        var emptyData = Json(new
        //        //        {
        //        //            draw = formCollection["draw"],
        //        //            recordsTotal = 0,
        //        //            recordsFiltered = 0,
        //        //            data = "",
        //        //            status = "0",
        //        //            errorMessage = "Captcha Entered is Invalid"
        //        //        });


        //        //        emptyData.MaxJsonLength = Int32.MaxValue;
        //        //        return emptyData;

        //        //    }
        //        //}
        //        #endregion

        //        #region  For IP address to be written in controller
        //        string ipAddressOfUser = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //        if (string.IsNullOrEmpty(ipAddressOfUser))
        //        {
        //            ipAddressOfUser = Request.ServerVariables["REMOTE_ADDR"];
        //        }
        //        string[] userMachineName = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
        //        string machineNameOfUser = userMachineName[0].ToString();
        //        #endregion

        //        DateTime frmDate, toDate;
        //        bool boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
        //        bool boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

        //        #region Validate date Inputs
        //        if (!boolFrmDate)
        //        {
        //            var emptyData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                recordsTotal = 0,
        //                recordsFiltered = 0,
        //                data = "",
        //                status = "0",
        //                errorMessage = "Invalid From Date"

        //            });
        //            emptyData.MaxJsonLength = Int32.MaxValue;
        //            return emptyData;
        //        }
        //        if (!boolToDate)
        //        {
        //            var emptyData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                recordsTotal = 0,
        //                recordsFiltered = 0,
        //                data = "",
        //                status = "0",
        //                errorMessage = "Invalid To Date"
        //            });
        //            emptyData.MaxJsonLength = Int32.MaxValue;
        //            return emptyData;
        //        }
        //        bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
        //        if (frmDate > toDate)
        //        {
        //            var emptyData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                recordsTotal = 0,
        //                recordsFiltered = 0,
        //                data = "",
        //                status = "0",
        //                errorMessage = "From Date can not be larger than To Date"

        //            });
        //            emptyData.MaxJsonLength = Int32.MaxValue;
        //            return emptyData;
        //        } 
        //        #endregion

        //        ECDataAuditDetailsRequestModel model = new ECDataAuditDetailsRequestModel
        //        {
        //            Datetime_FromDate = frmDate,
        //            Datetime_ToDate = toDate,
        //            OfficeID = Convert.ToInt32(selectedOfc),
        //            programs = programs
        //        };

        //        int startLen = Convert.ToInt32(formCollection["start"]);
        //        int totalNum = Convert.ToInt32(formCollection["length"]);
        //        //string EncryptedID = URLEncrypt.EncryptParameters(new string[]
        //        //                            {
        //        //                            "Datetime_FromDate=" + model.Datetime_FromDate,
        //        //                            "Datetime_ToDate="+model.Datetime_ToDate,
        //        //                            "programs="+model.programs,
        //        //                            "OfficeID="+model.OfficeID,
        //        //                            "startLen=" + startLen,
        //        //                            "totalNum=" + totalNum
        //        //                            });
        //        //Paging Size (10,20,50,100)    
        //        int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
        //        int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;




        //        caller = new ServiceCaller("ECDataAuditDetailsApiController");
        //        ECDatatAuditDetailsWrapperModel wrapperModel = new ECDatatAuditDetailsWrapperModel();
        //        wrapperModel.Datetime_FromDate = model.Datetime_FromDate;
        //        wrapperModel.Datetime_ToDate = model.Datetime_ToDate;
        //        wrapperModel.programs = model.programs;
        //        wrapperModel.OfficeID = model.OfficeID;
        //        wrapperModel.StartLength = startLen;
        //        wrapperModel.TotalNum = totalNum;


        //        #region Office-Wise Occurrance

        //        List<OfficeModificationOccurenceModel> OfficeWiseOccurance = null;
        //        long TotModfctnCount = 0;
        //        if (PopulateOccurances == "1")
        //        {
        //            //  OfficeWiseOccurance = caller.GetCall< List<OfficeModificationOccurenceModel>>("GetOfficeWiseModificationOccurences", new { EncryptedID = EncryptedID }, out errorMessage);
        //            OfficeWiseOccurance = caller.PostCall<ECDatatAuditDetailsWrapperModel, List<OfficeModificationOccurenceModel>>("GetOfficeWiseModificationOccurences", wrapperModel, out errorMessage);
        //            //OfficeWiseOccurance = ecDataBAL.GetOfficeWiseModificationOccurences(model);
        //            TotModfctnCount = OfficeWiseOccurance.Sum(x => x.NoOfOccurances);
        //        }
        //        else
        //        {
        //            OfficeWiseOccurance = new List<OfficeModificationOccurenceModel>();
        //        }


        //        var result = caller.PostCall<ECDatatAuditDetailsWrapperModel, List<ECDataAuditDetailsResponseModel>>("ECDataAuditDetailsList", wrapperModel, out errorMessage);

        //        //int totalCount = ecDataBAL.ECDataAuditDetailsList(model);
        //        int totalCount=caller.PostCall<ECDataAuditDetailsRequestModel, int>("ECDataAuditDetailsListTotalCount", model, out errorMessage);
        //        if (result.Count == 0)
        //        {
        //            var emptyData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                recordsTotal = 0,
        //                recordsFiltered = 0,
        //                data = "",
        //                status = false,
        //                errorMessage = "No results Found For the Current Input!Please try again"
        //            });
        //            emptyData.MaxJsonLength = Int32.MaxValue;
        //            return emptyData;
        //        } 
        //        #endregion

        //        var gridData = result.Select(ECDataAuditDetailsResponseModel => new
        //        {
        //            SRONAME = ECDataAuditDetailsResponseModel.SRONAME,
        //            FRN = ECDataAuditDetailsResponseModel.FRN,
        //            DATEOFMODIFICATION = ECDataAuditDetailsResponseModel.DATEOFMODIFICATION,
        //            MODIFICATION_AREA = ECDataAuditDetailsResponseModel.MODIFICATION_AREA,
        //            MODIFICATION_TYPE = ECDataAuditDetailsResponseModel.MODIFICATION_TYPE,
        //            MODIFICATION_DESCRIPTION = ECDataAuditDetailsResponseModel.MODIFICATION_DESCRIPTION,
        //            APPLICATION_NAME = ECDataAuditDetailsResponseModel.APPLICATION_NAME,
        //            IPADRESS = ECDataAuditDetailsResponseModel.IPADRESS,
        //            hostname = ECDataAuditDetailsResponseModel.hostname,
        //        });


        //        var JsonData = Json(new
        //        {
        //            draw = formCollection["draw"],
        //            //data = gridData.ToArray(),
        //            data = result.Skip(skip).Take(pageSize).ToList(),
        //            recordsTotal = totalCount,
        //            status = "1",
        //            OfficeWiseOccurance,
        //            TotModfctnCount,
        //            recordsFiltered = totalCount,
        //        });
        //        JsonData.MaxJsonLength = Int32.MaxValue;
        //        return JsonData;
        //    }
        //    catch (Exception ex)
        //    {
        //        return Redirect("/ECDataAuditDetails/Error");
        //    }
        //}

        //public ActionResult Error()
        //{
        //    return View();
        //}

        #region PDF
        /// <summary>
        /// Export ECData Modification Info To PDF
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SelectedOfc"></param>
        /// <param name="Programs"></param>
        /// <param name="OfficeName"></param>
        /// <returns>returns pdf</returns>
        [EventAuditLogFilter(Description = "Export ECData Modification Info To PDF")]
        public ActionResult ExportECDataModificationInfoToPDF(string FromDate, string ToDate, string SelectedOfc, string Programs, string OfficeName)
        {

            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                ECDataAuditDetailsRequestModel model = new ECDataAuditDetailsRequestModel
                {
                    Datetime_FromDate = frmDate,
                    Datetime_ToDate = toDate,
                    OfficeID = Convert.ToInt32(SelectedOfc),
                    OfficeName = OfficeName,
                    programs = Programs
                };
                List<ECDataAuditDetailsResponseModel> objListItemsToBeExported = new List<ECDataAuditDetailsResponseModel>();

                // IECDataAuditDetails ecDataBAL = new ECDataAuditDetailsBAL();

                // objListItemsToBeExported = ecDataBAL.GetECDataAuditDetailsListForPDF(model);
             
                caller = new ServiceCaller("ECDataAuditDetailsApiController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                objListItemsToBeExported = caller.PostCall<ECDataAuditDetailsRequestModel, List<ECDataAuditDetailsResponseModel>>("GetECDataAuditDetailsListForPDF", model, out errorMessage);

                string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.pdf", OfficeName, DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                string pdfHeader = string.Format("EC Data Modification Details Log (Between {0} and {1})", FromDate, ToDate);
                //Create Temp PDF File
                string createdPDFPath = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader);
                if (string.IsNullOrEmpty(createdPDFPath))
                {
                    throw new Exception();
                    //return Redirect("/ECDataAuditDetails/Error");
                }   //Then Encrypt it
                string passwordProtectedPDFFilePath = AddPasswordToPDF(createdPDFPath, filepath);
                if (string.IsNullOrEmpty(passwordProtectedPDFFilePath))
                {
                    throw new Exception();
                    //return Redirect("/ECDataAuditDetails/Error");
                }
                //Delete Temp File
                objCommon.DeleteFileFromTemporaryFolder(createdPDFPath);
                byte[] pdfBinary = System.IO.File.ReadAllBytes(passwordProtectedPDFFilePath);
                //Delete Password Protected File Too
                objCommon.DeleteFileFromTemporaryFolder(passwordProtectedPDFFilePath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request", URLToRedirect = "/Home/HomePage" });
            }
            
        }

        /// <summary>
        /// Add Password To PDF
        /// </summary>
        /// <param name="tempFilePathOfFileTobePasswordProtected"></param>
        /// <param name="DownloadableFilePath"></param>
        /// <returns>add password to pdf</returns>
        private string AddPasswordToPDF(string tempFilePathOfFileTobePasswordProtected, string DownloadableFilePath)
        {
            try
            {
                using (Stream input = new FileStream(tempFilePathOfFileTobePasswordProtected, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (Stream output = new FileStream(DownloadableFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        PdfReader reader = new PdfReader(input);
                        PdfEncryptor.Encrypt(reader, output, true, "Pa55w0rd@", "secret", PdfWriter.ALLOW_SCREENREADERS);
                    }
                }
                return DownloadableFilePath;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// Create PDF File
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="pdfHeader"></param>
        /// <returns>create pdf file</returns>
        private string CreatePDFFile(List<ECDataAuditDetailsResponseModel> objListItemsToBeExported, string fileName, string pdfHeader)
        {
            string folderPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/"));

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", "Temp" + fileName));
            try
            {
                //Create PDF
                // Server.Map.getpath;
                //if (!Path.GetDirectory.Exists)
                //{
                //    Path.CreateDirectory(filepath)
                // }


           

                using (Stream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (Document doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10))
                    {
                        var headerTextFont = FontFactory.GetFont("Arial", 15, new BaseColor(0, 128, 255));
                        PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                        doc.Open();
                        Paragraph addHeading = new Paragraph(pdfHeader, headerTextFont)
                        {
                            Alignment = 1,
                        };
                        Paragraph addSpace = new Paragraph(" ")
                        {
                            Alignment = 1
                        };
                        var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
                        var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(255, 51, 51));

                        var titleChunk = new Chunk("DateTime : ", blackListTextFont);
                        var totalChunk = new Chunk("Total Records: ", blackListTextFont);
                        var descriptionChunk = new Chunk(DateTime.Now.ToString(), redListTextFont);
                        string count = objListItemsToBeExported.Count().ToString();
                        var countChunk = new Chunk(count, redListTextFont);

                        var titlePhrase = new Phrase(titleChunk)
                        {
                            descriptionChunk
                        };
                        var totalPhrase = new Phrase(totalChunk)
                        {
                            countChunk
                        };
                        doc.Add(addHeading);
                        doc.Add(addSpace);
                        doc.Add(titlePhrase);
                        doc.Add(addSpace);
                        doc.Add(totalPhrase);
                        //Table Data
                        doc.Add(_stateTable(objListItemsToBeExported));
                        //doc.Close();
                    }
                }
                return filepath;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// _state Table
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <returns></returns>
        private PdfPTable _stateTable(List<ECDataAuditDetailsResponseModel> objListItemsToBeExported)
        {
            try
            {
                string[] col = { txtSROName, txtFRN, txtDateOfModification, txtModificationArea, txtModificationType, txtModificationDescription, txtApplicationName, txtIPAddress, txtHostName };
                PdfPTable table = new PdfPTable(9)
                {
                    /*
                    * default table width => 80%
                    */
                    WidthPercentage = 100
                };
                string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
                string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);

                //to repeat Headers
                table.HeaderRows = 1;
                // then set the column's __relative__ widths
                table.SetWidths(new Single[] { 2, 2, 1, 2, 1, 8, 2, 2, 1 });
                /*
                * by default tables 'collapse' on surrounding elements,
                * so you need to explicitly add spacing
                */
                //table.SpacingBefore = 10;
                for (int i = 0; i < col.Length; ++i)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col[i]))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    table.AddCell(cell);
                }
                foreach (var items in objListItemsToBeExported)
                {
                    table.AddCell(new Phrase(items.SRONAME, tableContentFont));
                    table.AddCell(new Phrase(items.FRN, tableContentFont));
                    table.AddCell(new Phrase(items.DATEOFMODIFICATION, tableContentFont));
                    table.AddCell(new Phrase(items.MODIFICATION_AREA, tableContentFont));
                    table.AddCell(new Phrase(items.MODIFICATION_TYPE, tableContentFont));
                    table.AddCell(GetModificationDescForPDF(items.LogID, items.LogTypeID, items.SROCODE, items.ITEMID));
                    table.AddCell(new Phrase(items.APPLICATION_NAME, tableContentFont));
                    table.AddCell(new Phrase(items.IPADRESS, tableContentFont));
                    table.AddCell(new Phrase(items.hostname, tableContentFont));
                }
                return table;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// Get Modification Desc For PDF
        /// </summary>
        /// <param name="logID"></param>
        /// <param name="logTypeID"></param>
        /// <param name="sROCODE"></param>
        /// <param name="iTEMID"></param>
        /// <returns></returns>
        private PdfPTable GetModificationDescForPDF(long logID, int logTypeID, int sROCODE, long iTEMID)
        {
            try
            {
                string errorMessage = string.Empty;
                List<MasterTableModel> objMasterTableDataList = new List<MasterTableModel>();
                PdfPTable table = new PdfPTable(3)
                {
                    WidthPercentage = 100
                };
                string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
                string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);

                // IECDataAuditDetails ecDataBAL = new ECDataAuditDetailsBAL();
                //objMasterTableDataList = ecDataBAL.MasterTablesListForPDF(logID, logTypeID, sROCODE, iTEMID);

                WrapperModelForDescPDF wrapperModel = new WrapperModelForDescPDF();

                wrapperModel.logID = logID;
                wrapperModel.logTypeID = logTypeID;
                wrapperModel.sROCODE = sROCODE;
                wrapperModel.iTEMID = iTEMID;

                caller = new ServiceCaller("ECDataAuditDetailsApiController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                objMasterTableDataList = caller.PostCall<WrapperModelForDescPDF, List<MasterTableModel>>("MasterTablesListForPDF", wrapperModel, out errorMessage);


                if (objMasterTableDataList.Count() == 0)
                {

                }
                else
                {
                    table.SetWidths(new Single[] { 2, 1, 3 });
                    for (int i = 0; i < col.Length; ++i)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(col[i]))
                        {
                            BackgroundColor = new BaseColor(143, 172, 204)
                        };
                        table.AddCell(cell);
                    }

                    foreach (var masterTableListItems in objMasterTableDataList)
                    {
                        table.AddCell(new Phrase(masterTableListItems.ColumnName, tableContentFont));
                        table.AddCell(new Phrase(masterTableListItems.PrevValue, tableContentFont));
                        table.AddCell(new Phrase(masterTableListItems.ModifiedValue, tableContentFont));
                    }
                }
                return table;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return null;
            }
        }

        #endregion

        #region Excel
        //public ActionResult ExportECDataModificationInfoToExcel(string FromDate, string ToDate, string SelectedOfc, string Programs, string OfficeName)
        //{
        //    try
        //    {
        //        string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.xlsx", OfficeName, DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
        //        DateTime frmDate, toDate;
        //        DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
        //        DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
        //        ECDataAuditDetailsRequestModel model = new ECDataAuditDetailsRequestModel
        //        {
        //            Datetime_FromDate = frmDate,
        //            Datetime_ToDate = toDate,
        //            OfficeID = Convert.ToInt32(SelectedOfc),
        //            programs = Programs
        //        };
        //        List<ECDataAuditDetailsResponseModel> objListItemsToBeExported = new List<ECDataAuditDetailsResponseModel>();
        //        IECDataAuditDetails ecDataBAL = new ECDataAuditDetailsBAL();
        //        objListItemsToBeExported = ecDataBAL.GetECDataAuditDetailsListForPDF(model);
        //        string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //        if(string.IsNullOrEmpty(clientDownloadableExcelFilePath))
        //            return Redirect("/ECDataAuditDetails/Error");
        //        string excelHeader = string.Format("EC Data Modification Details Log (Between {0} and {1})", FromDate, ToDate);
        //        string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader);
        //        if (string.IsNullOrEmpty(createdExcelPath))
        //            return Redirect("/ECDataAuditDetails/Error");
        //        byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
        //        objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
        //        return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        //    }
        //    catch (IOException)
        //    {
        //        return Redirect("/ECDataAuditDetails/Error");
        //    }
        //    catch (Exception)
        //    {
        //        return Redirect("/ECDataAuditDetails/Error");
        //    }
        //}

        //private string CreateExcel(List<ECDataAuditDetailsResponseModel> objListItemsToBeExported, string fileName, string excelHeader)
        //{
        //    string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //    FileInfo templateFile = GetFileInfo(ExcelFilePath);
        //    try
        //    {
        //        //create a new ExcelPackage
        //        using (ExcelPackage package = new ExcelPackage())
        //        {
        //            //Lock the workbook totally
        //            var workbook = package.Workbook;
        //            workbook.View.ShowSheetTabs = false;
        //            workbook.Protection.SetPassword("test");
        //            var workSheet = package.Workbook.Worksheets.Add("EC Data Logs");
        //            workSheet.Cells[1, 3].Value = excelHeader;
        //            workSheet.Cells[2, 3].Value = "Log Creation DateTime :" + DateTime.Now;
        //            //Add Headers
        //            workSheet.Cells[5, 1].Value = txtSROName;
        //            workSheet.Cells[5, 2].Value = txtFRN;
        //            workSheet.Cells[5, 3].Value = txtDateOfModification;
        //            workSheet.Cells[5, 4].Value = txtModificationArea;
        //            workSheet.Cells[5, 5].Value = txtModificationType;
        //            workSheet.Cells[5, 6].Value = txtModificationDescription;
        //            workSheet.Cells[5, 7].Value = txtApplicationName;
        //            workSheet.Cells[5, 8].Value = txtIPAddress;
        //            workSheet.Cells[5, 9].Value = txtHostName;
        //            using (ExcelRange Rng = workSheet.Cells[5, 1, 5, 9])
        //            {
        //                Rng.Style.Font.Bold = true;
        //                Rng.Style.Font.Color.SetColor(Color.Black);
        //                Rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                Rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
        //            }
        //            workSheet.Row(1).Style.Font.Bold = true;
        //            workSheet.Row(5).Style.Font.Bold = true;
        //            int row = 6;
        //            foreach (var items in objListItemsToBeExported)
        //            {
        //                workSheet.Cells[row, 1].Value = items.SRONAME;
        //                workSheet.Cells[row, 2].Value = items.FRN;
        //                workSheet.Cells[row, 3].Value = items.DATEOFMODIFICATION;
        //                workSheet.Cells[row, 4].Value = items.MODIFICATION_AREA;
        //                workSheet.Cells[row, 5].Value = items.MODIFICATION_TYPE;
        //                workSheet.Cells[row, 6].Value = items.ModificationDescForPDF;
        //                workSheet.Cells[row, 7].Value = items.APPLICATION_NAME;
        //                workSheet.Cells[row, 8].Value = items.IPADRESS;
        //                workSheet.Cells[row, 9].Value = items.hostname;
        //                row++;
        //                //Function that passes the current row and adds the column details 
        //                //AddSubRowsForCurrentRow(out row,out workSheet);
        //            }
        //            workSheet.Cells["A:XFD"].AutoFitColumns();
        //            workSheet.Protection.SetPassword("Pa55w0rd@");
        //            package.Encryption.Algorithm = EncryptionAlgorithm.AES192;
        //            package.SaveAs(templateFile, "Pa55w0rd@");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return ExcelFilePath;
        //}

        //private void AddSubRowsForCurrentRow(out int row, out ExcelWorksheet workSheet)
        //{
        //    workSheet.Cells[5, 1].Value = txtSROName;
        //    workSheet.Cells[5, 2].Value = txtFRN;
        //    workSheet.Cells[5, 3].Value = txtDateOfModification;
        //}

        //public static FileInfo GetFileInfo(string tempExcelFilePath)
        //{
        //    var fi = new FileInfo(tempExcelFilePath);
        //    return fi;
        //}
        #endregion
    }
}