using CustomModels.Models.XELFiles;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.XELFiles.Controllers
{
      [KaveriAuthorizationAttribute]
    public class XELFilesDetailsController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;
        [MenuHighlightAttribute]
        public ActionResult RegisterJobs()
        {
            try
            {

                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.XELFiles;
                caller = new ServiceCaller("XELFilesDetailsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                XELFilesViewModel reqModel = caller.GetCall<XELFilesViewModel>("GetJobsDetails", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading XEL Files Details View", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading XEL Files Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        [MenuHighlightAttribute]
        public ActionResult AuditSpecficationDetails()
        {
            try
            {

                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.AuditSpecificationDetails;
                caller = new ServiceCaller("XELFilesDetailsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                XELFilesViewModel reqModel = caller.GetCall<XELFilesViewModel>("GetAuditSpecificationDetails", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading Audit Specification Details View", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading Audit Specification Details Details View", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// Get SRO Office List By District ID
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <returns>returns SRO Office list</returns>
        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadRegisteredJobsTableData(FormCollection formCollection)
        {
            try
            {

                #region User Variables and Objects

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
                string errorMessage = String.Empty;
                #endregion

                #region Server Side Validation              


                caller = new ServiceCaller("CommonsApiController");
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

                #endregion




                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                XELFilesViewModel reqModel = new XELFilesViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;

                caller = new ServiceCaller("XELFilesDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;



                int totalCount = caller.PostCall<XELFilesViewModel, int>("GetRegisteredJobsTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //To get records of indexII report table 
                RegisteredJobsListModel resModel = caller.PostCall<XELFilesViewModel, RegisteredJobsListModel>("GetRegisteredJobsTableData", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Registered Jobs Details" });
                }
                IEnumerable<RegisteredJobsModel> result = resModel.RegisteredJobsModelLst;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }


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
                        result = result.Where(m => m.SROOfficeName.ToLower().Contains(searchValue.ToLower()) ||
                        m.RegisteredDateTime.ToLower().Contains(searchValue.ToLower()) ||
                        m.Description.ToLower().Contains(searchValue.ToLower()) ||
                        m.CompletedDateTime.ToString().ToLower().Contains(searchValue.ToLower()));

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(SupportEnclosureDetailsModel => new
                {
                    JobID = SupportEnclosureDetailsModel.JobID,
                    OfficeName = SupportEnclosureDetailsModel.SROOfficeName,
                    SupportEnclosureDetailsModel.FromYear,
                    SupportEnclosureDetailsModel.ToYear,
                    SupportEnclosureDetailsModel.FromMonth,
                    SupportEnclosureDetailsModel.ToMonth,
                    SupportEnclosureDetailsModel.RegisteredDateTime,
                    SupportEnclosureDetailsModel.CompletedDateTime,
                    SupportEnclosureDetailsModel.IsJobCompleted,
                    SupportEnclosureDetailsModel.Description,
                    SupportEnclosureDetailsModel.OfficeType
                });


                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
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
                    });
                }
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Registered Jobs Details" });
            }
        }


        /// <summary>
        /// POST call for Ticket registration.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Register Jobs")]
        public ActionResult RegisterJobsDetails(XELFilesViewModel viewModel)
        {
            string errorMessage = string.Empty;
            //viewModel. = false;

            try
            {
                #region Validations
                if (viewModel.SROfficeID == 0)
                    return Json(new { success = false, message = "SRO Name is required" });
                if (String.IsNullOrEmpty(viewModel.OfficeType))
                    return Json(new { success = false, message = "Office Type is required" });

                if (viewModel.OfficeType == "DRO" || viewModel.OfficeType == "SRO")
                { }
                else
                {
                    return Json(new { success = false, message = "Invalid Office Type" });


                }

                if (viewModel.FromYearID == 0)
                    return Json(new { success = false, message = "FromYear is required" });

                if (viewModel.ToYearID == 0)
                    return Json(new { success = false, message = "To Year is required" });

                if (viewModel.FromMonthID == 0)
                    return Json(new { success = false, message = "From Month is required" });

                if (viewModel.ToMonthID == 0)
                    return Json(new { success = false, message = "To Month is required" });

                if (viewModel.ToMonthID > DateTime.Now.Month)
                    return Json(new { success = false, message = "'To Month' should not be greater than current month ( " + DateTime.Now.Date.ToString("MMM") + " )" });

                if (viewModel.FromMonthID > DateTime.Now.Month)
                    return Json(new { success = false, message = "'From Month' should not be greater than current month ( " + DateTime.Now.Date.ToString("MMM") + " )" });



                if (viewModel.ToYearID < viewModel.FromYearID)
                    return Json(new { success = false, message = "'From Year' should be less than 'To Year'" });

                if (viewModel.ToYearID == viewModel.FromYearID)
                {
                    if (viewModel.ToMonthID < viewModel.FromMonthID)
                        return Json(new { success = false, message = "'From Month' should be less than 'To Month'" });
                }

                ModelState.Remove("FromDate");
                ModelState.Remove("ToDate");
                #endregion

                if (ModelState.IsValid)
                {
                    ServiceCaller caller = new ServiceCaller("XELFilesDetailsAPIController");
                    XELFilesViewModel responseModel = caller.PostCall<XELFilesViewModel, XELFilesViewModel>("RegisterJobsDetails", viewModel, out errorMessage);
                    if (!String.IsNullOrEmpty(errorMessage))
                        return Json(new { success = false, message = errorMessage });


                    if (!responseModel.IsInserted && !string.IsNullOrEmpty(responseModel.ErrorMessage))
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
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return Json(new { success = false, message = e.GetBaseException().Message });

            }
        }

        //XELFilesDetailsAPIController
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadAuditSpecificationDetails(FormCollection formCollection)
        {
            try
            {

                #region User Variables and Objects
                string fromDate = formCollection["fromDate"];
                string ToDate = formCollection["ToDate"];
                string SROOfficeID = formCollection["SROOfficeCode"];
                string OfficeType = formCollection["OfficeType"];

                int SroId = Convert.ToInt32(SROOfficeID);

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                string Amount = formCollection["Amount"];
                if (Amount == "")
                {
                    Amount = "0";
                }
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
                String errorMessage = String.Empty;
                #endregion

                #region Server Side Validation              


                caller = new ServiceCaller("CommonsApiController");
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

                //Validation For DR Login
                if (LevelID == Convert.ToInt16(CommonEnum.LevelDetails.DR))
                {
                    //Validation for DR when user do not select any sro which is by default "Select"
                    if ((SroId == 0))
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select SRO Name"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }
                else
                {//Validations of Logins other than SR and DR

                    if (SroId == 0)//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select SRO Name"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                }

                if (string.IsNullOrEmpty(fromDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date is required"
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
                        errorMessage = "To Date is required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    DateTime DateTime_FromDate = Convert.ToDateTime(fromDate);
                    DateTime DateTime_Todate = Convert.ToDateTime(ToDate);

                    if ((DateTime_Todate - DateTime_FromDate).TotalDays > 180)
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = "0",
                            errorMessage = "You can only see records of six months."
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }
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
                        errorMessage = "From Date can not be greater than To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                XELFilesViewModel reqModel = new XELFilesViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = fromDate;
                reqModel.ToDate = ToDate;
                reqModel.SROfficeID = SroId;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DateTime_FromDate = frmDate;
                reqModel.OfficeType = OfficeType;
                caller = new ServiceCaller("XELFilesDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;


                int totalCount = caller.PostCall<XELFilesViewModel, int>("GetAuditSpecificationDetailsTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //To get records of indexII report table 
                XELFilesResModel resModel = caller.PostCall<XELFilesViewModel, XELFilesResModel>("GetAuditSpecificationDetailsTableData", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Rescanning Details" });
                }
                IEnumerable<XELFilesModel> result = resModel.xelFilesModellST;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }


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
                        result = result.Where(m => m.EventTime.ToLower().Contains(searchValue) ||
                        m.LoginName.ToLower().Contains(searchValue.ToLower()) ||
                        m.ServerName.ToLower().Contains(searchValue.ToLower()) ||
                        m.DatabaseName.ToString().Contains(searchValue.ToLower()) ||
                        m.ApplicationName.ToString().Contains(searchValue.ToLower()) ||
                        m.Statement.ToString().Contains(searchValue.ToLower()) ||
                        m.OfficeName.ToString().Contains(searchValue.ToLower()) ||
                        m.OfficeType.ToString().Contains(searchValue.ToLower()) ||
                        m.HostName.ToString().Contains(searchValue.ToLower()));

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(AuditSpecificationModel => new
                {
                    AuditSpecificationModel.SrNo,
                    AuditSpecificationModel.EventTime,
                    AuditSpecificationModel.LoginName,
                    AuditSpecificationModel.ServerName,
                    AuditSpecificationModel.DatabaseName,
                    AuditSpecificationModel.ApplicationName,
                    AuditSpecificationModel.Statement,
                    AuditSpecificationModel.OfficeName,
                    AuditSpecificationModel.OfficeType,
                    AuditSpecificationModel.HostName
                });
                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";


                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
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
                        PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Rescanning Details" });
            }
        }


        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeID"></param>
        /// <returns>returns excel file</returns>
        public ActionResult ExportToExcel(string FromDate, string ToDate, string SROOfficeID, string SelectedSRO, string OfficeType)
        {
            try
            {
                caller = new ServiceCaller("XELFilesDetailsAPIController");
                string fileName = string.Format("XELFile.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                XELFilesViewModel reqModel = new XELFilesViewModel
                {

                    FromDate = FromDate,
                    ToDate = ToDate,
                    SROfficeID = Convert.ToInt32(SROOfficeID),
                    startLen = 0,
                    totalNum = 10
                };

                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}

                reqModel.OfficeType = OfficeType;
                int totalCount = caller.PostCall<XELFilesViewModel, int>("GetAuditSpecificationDetailsTotalCount", reqModel, out errorMessage);
                reqModel.totalNum = totalCount;
                XELFilesResModel resModel = caller.PostCall<XELFilesViewModel, XELFilesResModel>("GetAuditSpecificationDetailsTableData", reqModel, out errorMessage);

                List<XELFilesModel> objListItemsToBeExported = resModel.xelFilesModellST;

                if (objListItemsToBeExported == null)
                {

                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);

                }


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("Database Server Audit Trails Between ( " + FromDate + " and " + ToDate + " )");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = string.Empty;

                createdExcelPath = CreateXELFileExcel(resModel, fileName, excelHeader, SelectedSRO,OfficeType);


                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "XELAuditTrails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });

            }

        }

        /// <summary>
        /// Create Excel
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateXELFileExcel(XELFilesResModel Model, string fileName, string excelHeader, string SelectedSRO, string OfficeType)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("XEL Files Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                  //  workSheet.Cells[2, 1].Value = "SRO : " + SelectedSRO;
                    workSheet.Cells[2, 1].Value = OfficeType+" : " + SelectedSRO;
                    workSheet.Cells[3, 1].Value = "Total Records : " + Model.xelFilesModellST.Count();
                    workSheet.Cells[1, 1, 1, 7].Merge = true;
                    workSheet.Cells[2, 1, 2, 7].Merge = true;
                    workSheet.Cells[3, 1, 3, 7].Merge = true;
                    workSheet.Cells[4, 1, 4, 7].Merge = true;

                    //workSheet.Column(6).Style.WrapText = true;
                    //workSheet.Column(9).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 15;
                    workSheet.Column(2).Width = 50;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 40;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 60;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Column(9).Style.WrapText = true;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;


                    int rowIndex = 6;
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[5, 1].Value = "Sr No";
                    workSheet.Cells[5, 2].Value = "Office Name";
                    workSheet.Cells[5, 3].Value = "Server Name";
                    workSheet.Cells[5, 4].Value = "Database Name";
                    workSheet.Cells[5, 5].Value = "Login Name";
                    workSheet.Cells[5, 6].Value = "Host Name";
                    workSheet.Cells[5, 7].Value = "Application Name";
                    workSheet.Cells[5, 8].Value = "Event Time";
                    workSheet.Cells[5, 9].Value = "Statement";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";


                    foreach (var items in Model.xelFilesModellST)
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

                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                        workSheet.Cells[rowIndex, 3].Value = items.ServerName;
                        workSheet.Cells[rowIndex, 4].Value = items.DatabaseName;
                        workSheet.Cells[rowIndex, 5].Value = items.LoginName;
                        workSheet.Cells[rowIndex, 6].Value = items.HostName;
                        workSheet.Cells[rowIndex, 7].Value = items.ApplicationName;
                        workSheet.Cells[rowIndex, 8].Value = items.EventTime;
                        workSheet.Cells[rowIndex, 9].Value = items.Statement;


                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                        //workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        //workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        rowIndex++;

                    }


                    workSheet.Row(rowIndex).Style.Font.Bold = true;




                    using (ExcelRange Rng = workSheet.Cells[5, 1, (rowIndex), 9])
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
        /// GetFileInfo
        /// </summary>
        /// <param name="tempExcelFilePath"></param>
        /// <returns>returns file info</returns>
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }




        [EventAuditLogFilter(Description = "XEL Log View")]
        public ActionResult GetXELLogView()
        {
            try
            {
                // ask shubham what to set here
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.XELLog;

                caller = new ServiceCaller("XELFilesDetailsAPIController");

                XELLogViewModel reqModel = caller.GetCall<XELLogViewModel>("GetXELLogView");

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving XEL Log View", URLToRedirect = "/Home/HomePage" });

                }
                return View("XELLogView", reqModel);



            }
            catch (Exception)
            {
                throw;

            }
        }


        [EventAuditLogFilter(Description = "Loads EC Daily Receipt Details Datatable")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult LoadXELLogDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("XELFilesDetailsAPIController");


            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects               
                string FromDate = formCollection["FromDate"];
                string OfficeType = formCollection["OfficeType"];
                string ToDate = formCollection["ToDate"];
                string SroID = formCollection["SroID"];
                string TableID = formCollection["TableID"];
                int SroId = Convert.ToInt32(SroID);
                int iTableID = Convert.ToInt32(TableID);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                //   int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                XELLogViewModel reqModel = new XELLogViewModel();

                if ((iTableID == 0))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select any table"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (string.IsNullOrEmpty(FromDate))
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
                else if (string.IsNullOrEmpty(ToDate))
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
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);
                //  bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);

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
                else if (!boolToDate)
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


                else if (frmDate > toDate)
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

                reqModel.dtFromDate = frmDate;

                reqModel.dtToDate = toDate;
                //else if ((toDate - frmDate).TotalDays > 180)//six months validation by RamanK on 20-09-2019
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "Data of six months can be seen at a time"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;

                //}
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = FromDate;
                reqModel.ToDate = ToDate;
                reqModel.SROfficeID = SroId;
                reqModel.TableID = iTableID;
                reqModel.OfficeType = OfficeType;
                int totalCount = 0;

                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}

                XELLogViewModel responseModel = caller.PostCall<XELLogViewModel, XELLogViewModel>("LoadXELLogDetails", reqModel, out errorMessage);

                IEnumerable<XELLogDetailsModel> result = responseModel.XELLogDetailsModelList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting XEL Log Details." });
                }
                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = result.Count();
                }
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

                        m.sroName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.AbsolutePath.ToString().ToLower().Contains(searchValue.ToLower()) ||

                        m.fileName.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.IsSuccessfullUpload.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.IsFileReadSuccessful.ToString().ToLower().Contains(searchValue.ToLower()) ||

m.TransmissionInitateDateTime.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.TransmissionCompleteDateTime.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.Year.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.Month.ToString().ToLower().Contains(searchValue.ToLower()) ||

m.FileReadDateTime.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.EventStartDate.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.EventEndDate.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.sExceptionType.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.InnerExceptionMsg.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.ExceptionMsg.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.ExceptionStackTrace.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.ExceptionMethodName.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.LogDate.ToString().ToLower().Contains(searchValue.ToLower()) ||
m.SchedulerName.ToString().ToLower().Contains(searchValue.ToLower())
                        );



                    }
                }
                totalCount = result.Count();

                ////  Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                //}

                var gridData = result.Select(x => new
                {
                    SrNo = x.SrNo,
                    sroCode = x.sroName,

                    AbsolutePath = x.AbsolutePath,
                    fileName = x.fileName,
                    IsSuccessfullUpload = x.IsSuccessfullUpload,
                    TransmissionInitateDateTime = x.TransmissionInitateDateTime,
                    TransmissionCompleteDateTime = x.TransmissionCompleteDateTime,
                    Year = x.Year,
                    Month = x.Month,
                    FileSize = x.FileSize,
                    FileReadDateTime = x.FileReadDateTime,
                    IsFileReadSuccessful = x.IsFileReadSuccessful,
                    EventStartDate = x.EventStartDate,
                    EventEndDate = x.EventEndDate,
                    sExceptionType = x.sExceptionType,
                    InnerExceptionMsg = x.InnerExceptionMsg,
                    ExceptionMsg = x.ExceptionMsg,
                    ExceptionStackTrace = x.ExceptionStackTrace,
                    ExceptionMethodName = x.ExceptionMethodName,
                    LogDate = x.LogDate,
                    SchedulerName = x.SchedulerName
                });

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting xel log Details." }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult GetOfficeList(String OfficeType)
        {
            XELLogViewModel model = new XELLogViewModel();

            try
            {
                #region Server Side Validation
                // ADDED BY SHUBHAM BHAGAT ON 21-05-2019 AT 6:36 PM
                if (String.IsNullOrEmpty(OfficeType))
                {
                    return Json(new { errorMessage = "Please select office type." }, JsonRequestBehavior.AllowGet);
                }
                #endregion
                caller = new ServiceCaller("XELFilesDetailsAPIController");

                model = caller.GetCall<XELLogViewModel>("GetOfficeList", new { OfficeType });

                //JSON with JsonRequestBehavior.AllowGet is working fine when KaveriAuthorizationAttribute is not added. 
                return Json(new { OfficeList = model.SROfficeList }, JsonRequestBehavior.AllowGet);
                //return Json(new { OfficeList = model.OfficeTypeList });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { errorMessage = "Error occured while processing your request." }, JsonRequestBehavior.AllowGet);

            }
        }

    }
}
