using CustomModels.Models.DataEntryCorrection;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.DataEntryCorrection.Controllers
{
    [KaveriAuthorization]
    public class SelectDocumentController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        //Added by Madhusoodan to check if OrderID is in session before loading Select Document tab
        //CheckOrderInSession
        [HttpGet]
        public ActionResult CheckOrderInSession()
        {
            try
            {
                caller = new ServiceCaller("SelectDocumentAPIController");
                //Added by Madhusoodan on 12/08/2021 to restrict user if order is not added or it is notin edit mode then don't load select document tab

                int currentOrderID = KaveriSession.Current.OrderID;

                if (currentOrderID != 0)
                {
                        
                    bool isOrderFileExists = caller.GetCall<bool>("CheckOrderFileExists", new { currentOrderID = currentOrderID });

                    if (isOrderFileExists)
                        return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                    else
                    return Json(new { success = false, message = " Please upload DR Order Document to continue." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = " Please upload DR order first." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occured while loading Select Document Tab" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult LoadSelectDocumentTabView(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("SelectDocumentAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                
                //Added by Madhusoodan on 09*/08/2021 to populate Search parameters in edit mode
                bool isEditMode = false;

                if (KaveriSession.Current.IsEditMode)
                {
                    isEditMode = true;
                }
                int currentOrderID = KaveriSession.Current.OrderID;

                //Added by Madhusoodan on 12/08/2021 to restrict user if order is not added or it is notin edit mode then don't load select document tab
                if (currentOrderID != 0)
                {
                    //DataEntryCorrectionViewModel reqModel = caller.GetCall<DataEntryCorrectionViewModel>("GetSelectDocumentView", new { OfficeID = OfficeID });
                    DataEntryCorrectionViewModel reqModel = caller.GetCall<DataEntryCorrectionViewModel>("GetSelectDocumentView", new { OfficeID = OfficeID, isEditMode = isEditMode, currentOrderID = currentOrderID });
                    if (reqModel == null)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Data Entry Correction View", URLToRedirect = "/Home/HomePage" });
                    }
                    //return View(reqModel);

                    //After saving Note add IsEdit mode true in session to load search parameterss
                    if (reqModel.DocumentNumber != null )
                    {
                        reqModel.IsDocumentSearched = true;
                    }
                   
                    KaveriSession.Current.IsEditMode = true;
                    
                    return View("LoadSelectDocumentTabView", reqModel);
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Please either add a new order or edit an existing order to open Select Document Tab." });
                }
            
                
            }
            catch (Exception ex)
            {
                //return Json(new { serverError = true, success = false, errorMessage = "Error occured while loading Select Document Tab" });
                return Json(new { success = false, errorMessage = "Error occured while loading Select Document Tab" });
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
        [EventAuditLogFilter(Description = "Load Property details for Document By Document Number, Book Type and Financial Year")]
        public ActionResult LoadPropertyDetailsData(FormCollection formCollection)
        {
            try
            {

                #region User Variables and Objects
                string SROOfficeID = formCollection["SROOfficeID"];
                string DROfficeID = formCollection["DROfficeID"];
                long DocNumber;
                if (string.IsNullOrEmpty(formCollection["DocumentNumber"].ToString()))
                {
                    DocNumber = Convert.ToInt64(0);
                }
                else
                {
                    var regex = "^[0-9]*$";
                    var matchDocumentNo = Regex.Match(formCollection["DocumentNumber"].ToString(), regex, RegexOptions.IgnoreCase);
                    if (!matchDocumentNo.Success)
                        DocNumber = Convert.ToInt64(0);
                    else
                        DocNumber = Convert.ToInt64(formCollection["DocumentNumber"]);
                }
                string BookType = formCollection["BookTypeID"];
                string FinancialYear = formCollection["FinancialYear"];

                //long DocNumber = Convert.ToInt64(DocumentNumber);

                int SroId = Convert.ToInt32(SROOfficeID);
                int DroId = Convert.ToInt32(DROfficeID);

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

                //Validation For DR Login
                if (LevelID == Convert.ToInt16(ECDataUI.Common.CommonEnum.LevelDetails.DR))
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
                            errorMessage = "Please select any SRO"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }
                else
                {//Validations of Logins other than SR and DR

                    if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any District"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                    else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any SRO"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                    else if (DocNumber == 0)//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please Select Module to Procced"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                    else if (BookType == "0")//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please Select Book Type to Procced"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                }

                if (string.IsNullOrEmpty(FinancialYear) || FinancialYear.Equals("Select"))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "FinancialYear required"
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

                //Added by Madhusoodan on 09*/08/2021
                int OrderID = KaveriSession.Current.OrderID;

                DataEntryCorrectionViewModel reqModel = new DataEntryCorrectionViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.DocumentNumber = DocNumber;
                reqModel.SROfficeID = SroId;
                reqModel.BookTypeID = Convert.ToInt32(BookType);
                reqModel.FinancialYearStr = FinancialYear;
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);
                reqModel.OfficeID = OfficeID;
                //Added by Madhusoodan on 09*/08/2021
                reqModel.OrderID = OrderID;


                caller = new ServiceCaller("SelectDocumentAPIController");

                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}

                //To get records of indexII report table 
                DataEntryCorrectionResultModel resModel = caller.PostCall<DataEntryCorrectionViewModel, DataEntryCorrectionResultModel>("LoadPropertyDetailsData", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting property details" });
                }
                int totalCount = resModel.DataEntryCorrectionPropertyDetailList.Count;
                IEnumerable<DataEntryCorrectionPropertyDetailModel> result = resModel.DataEntryCorrectionPropertyDetailList;

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
                        result = result.Where(m => m.PropertyDetails.ToLower().Contains(searchValue.ToLower()) ||
                        m.PropertyDetails.ToLower().Contains(searchValue.ToLower()) ||
                        m.ExecutionDateTime.ToLower().Contains(searchValue.ToLower()));
                    }
                }

                var gridData = result.Select(DataEntryCorrectionViewModel => new
                {
                    Sno = DataEntryCorrectionViewModel.SerialNo,
                    //Added by Madhusoodan on 06/08/2021
                    SelectButton = DataEntryCorrectionViewModel.Select,
                    PNDTabButton = DataEntryCorrectionViewModel.ButtonPropertyNoDetails,
                    PartyTabButton = DataEntryCorrectionViewModel.ButtonPartyDetails,
                    //
                    ScheduleDescription = DataEntryCorrectionViewModel.ScheduleDescription,
                    ExecutionDate = DataEntryCorrectionViewModel.ExecutionDateTime,
                    NatureOfDocument = DataEntryCorrectionViewModel.NatureOfDocument,
                    Executant = DataEntryCorrectionViewModel.Executant,
                    Claimant = DataEntryCorrectionViewModel.Claimant,
                    CDNumber = DataEntryCorrectionViewModel.CDNumber,
                    PageCount = DataEntryCorrectionViewModel.PageCount,
                    FinalRegistrationNumber = DataEntryCorrectionViewModel.FinalRegistrationNo,
                    DocumentID = DataEntryCorrectionViewModel.DocumentID,
                    PropertyID = DataEntryCorrectionViewModel.PropertyID

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

                //Added by Madhusoodan on 02/08/2021 to save Document No, BookID and Financial Year in Session for other tabs
                KaveriSession.Current.DECDocumentNumber = reqModel.DocumentNumber;
                KaveriSession.Current.DECBookTypeID = reqModel.BookTypeID;
                KaveriSession.Current.DECFinancialYear = Int32.Parse(reqModel.FinancialYearStr.Split('-')[0]);
                KaveriSession.Current.DECSROCode = reqModel.SROfficeID;
                //
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Property Details data" });
            }
        }

        //Added by Madhur on 27-7-21

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Load Property details for Document By Document Number, Book Type and Financial Year")]
        public ActionResult LoadPreviousPropertyDetailsData(FormCollection formCollection)
        {
            try
            {

                #region User Variables and Objects
                string SROOfficeID = formCollection["SROOfficeID"];
                string DROfficeID = formCollection["DROfficeID"];

                string DocID = formCollection["DocumentID"];
                string PropID = formCollection["PropertyID"];

                //Commented and added by Madhusoodan on 09/08/2021
                //long DocNumber;
                //if (string.IsNullOrEmpty(formCollection["DocumentNumber"].ToString()))
                //{
                //    DocNumber = Convert.ToInt64(0);
                //}
                //else
                //{
                //    var regex = "^[0-9]*$";
                //    var matchDocumentNo = Regex.Match(formCollection["DocumentNumber"].ToString(), regex, RegexOptions.IgnoreCase);
                //    if (!matchDocumentNo.Success)
                //        DocNumber = Convert.ToInt64(0);
                //    else
                //        DocNumber = Convert.ToInt64(formCollection["DocumentNumber"]);
                //}
                //string BookType = formCollection["BookTypeID"];
                //string FinancialYear = formCollection["FinancialYear"];

                //long DocNumber = Convert.ToInt64(DocumentNumber);


                int SroId = Convert.ToInt32(SROOfficeID);
                int DroId = Convert.ToInt32(DROfficeID);
                

                long DocumentID = Convert.ToInt64(DocID);
                long PropertyID = Convert.ToInt64(PropID);


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


                //Commented by Madhusoodan on 09/08/2021
                //if (string.IsNullOrEmpty(FinancialYear) || FinancialYear.Equals("Select"))
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "FinancialYear required"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                #endregion




                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                DataEntryCorrectionViewModel reqModel = new DataEntryCorrectionViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                //reqModel.DocumentNumber = DocNumber;
                reqModel.SROfficeID = SroId;
                //reqModel.BookTypeID = Convert.ToInt32(BookType);
                //reqModel.FinancialYearStr = FinancialYear;
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);
                reqModel.OfficeID = OfficeID;
                reqModel.DocumentID = DocumentID;
                reqModel.PropertyID = PropertyID;
                //Added by Madhusoodan on 13/08/2021 to load Delete button for Section 68 Note
                reqModel.OrderID = KaveriSession.Current.OrderID;

                caller = new ServiceCaller("SelectDocumentAPIController");
                List<DataEntryCorrectionPreviousPropertyDetailModel> resModel = caller.PostCall<DataEntryCorrectionViewModel, List<DataEntryCorrectionPreviousPropertyDetailModel>>("LoadPreviousPropertyDetailsData", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting property details" });
                }
                int totalCount = resModel.Count;
                IEnumerable<DataEntryCorrectionPreviousPropertyDetailModel> result = resModel;

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
                        result = result.Where(m => m.Section68Note.ToLower().Contains(searchValue.ToLower()) ||
                        m.DROrderNumber.ToLower().Contains(searchValue.ToLower()));
                    }
                }

                var gridData = result.Select(DataEntryCorrectionViewModel => new
                {
                    Sno = DataEntryCorrectionViewModel.SerialNo,
                    DROrderNumber = DataEntryCorrectionViewModel.DROrderNumber,
                    //Added by Madhusoodan on 11/08/2021 to add Order Upload Date column
                    OrderUploadDate = DataEntryCorrectionViewModel.OrderUploadDate,
                    OrderDate = DataEntryCorrectionViewModel.OrderDate,
                    Section68Note = DataEntryCorrectionViewModel.Section68Note,
                    ViewDocument = DataEntryCorrectionViewModel.ViewDocument,

                    //Added by Madhusoodan on 13/08/2021 to load Delete button for Section 68 Note
                    DeleteNoteBtn = DataEntryCorrectionViewModel.Section68NoteDeleteBtn,
                });


                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        //EncrytptedDocumetID = resModel.EncrytptedDocumetID,
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
                        data = gridData.Skip(skip).Take(pageSize).ToArray(),
                        //EncrytptedDocumetID = resModel.EncrytptedDocumetID,
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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting previous DR orders for selected property." });
            }
        }

        [HttpGet]
        public dynamic ViewBtnClickPreviousPropTable(int OrderID)
        {
            try
            {
                caller = new ServiceCaller("SelectDocumentAPIController");
                string reqModel = caller.GetCall<string>("ViewBtnClickPreviousPropTable", new { OrderID = OrderID });
                byte[] pdfByteArray = System.IO.File.ReadAllBytes(reqModel);
                return File(pdfByteArray, "application/pdf");
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

        //Added by Madhur on 27-7-21

        //Added by Madhusoodan on 02/08/2021 to save DocumentID in DEC_DROrderMaster table
        [HttpPost]
        //[HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult SaveSectionNote(FormCollection formCollection)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                caller = new ServiceCaller("SelectDocumentAPIController");

                DataEntryCorrectionViewModel decViewModel = new DataEntryCorrectionViewModel();
                decViewModel.OfficeID = KaveriSession.Current.OfficeID;
                decViewModel.OrderID = KaveriSession.Current.OrderID;

                decViewModel.UserID = KaveriSession.Current.UserID;
                decViewModel.IPAddress = objCommon.GetIPAddress();

                //decViewModel.DocumentID = currentDocumentID;
                decViewModel.DocumentID = Convert.ToInt64(formCollection["DocumentID"]);
                decViewModel.PropertyID = Convert.ToInt64(formCollection["PropertyID"]);
                decViewModel.Section68NoteForProperty = formCollection["Section68NoteForProperty"];

                decViewModel.SROfficeID = KaveriSession.Current.DECSROCode;

                if (decViewModel.OfficeID != 0 && decViewModel.DocumentID != 0 && decViewModel.PropertyID != 0 && decViewModel.OrderID != 0)
                {
                    SelectDocumentResultModel sdReqModel = caller.PostCall<DataEntryCorrectionViewModel, SelectDocumentResultModel>("SaveSection68Note", decViewModel, out errorMessage);
                    
                    if (sdReqModel == null)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Data Entry Correction View", URLToRedirect = "/Home/HomePage" });
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(errorMessage))
                            return Json(new { serverError = false, success = false, message = sdReqModel.ErrorMessage });

                        if (!string.IsNullOrEmpty(sdReqModel.ErrorMessage))
                        {
                            return Json(new { serverError = false, success = false, message = sdReqModel.ErrorMessage });
                        }
                        else
                        {
                            return Json(new { serverError = false, success = true, message = sdReqModel.ResponseMessage });
                        }
                    }
                }
                else
                {
                    return Json(new { serverError = false, success = false, errorMessage = "Error in saving Document details. Plese try again." });
                }
            }
            catch (Exception)
            {
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while selecting Document Details." });
            }
        }

        //LoadPreviousSec68Note
        [HttpGet]
        public ActionResult LoadPreviousSecNote(int currentPropertyID = 0)
        {
            try
            {
                caller = new ServiceCaller("SelectDocumentAPIController");
                //long PropertyID = Convert.ToInt64(formCollection["PropertyID"]);
                long PropertyID = currentPropertyID;
                int OrderID = KaveriSession.Current.OrderID;
                int officeID = KaveriSession.Current.OfficeID;
                
                if (OrderID != 0 && PropertyID != 0)
                {
                    Section68NoteResultModel sec68NoteResModel = caller.GetCall<Section68NoteResultModel>("LoadPreviousSec68Note", new { orderID = OrderID, propertyID = PropertyID, officeID = officeID });

                    if (sec68NoteResModel == null)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading previously added Section 68(2) Note. Please try again.", URLToRedirect = "/Home/HomePage" });
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(sec68NoteResModel.ErrorMessage))
                            //return Json(new { serverError = false, success = false, message = sec68NoteResModel.ErrorMessage, PreviousSection68Note = string.Empty });
                            return Json(new { serverError = false, success = false, message = sec68NoteResModel.ErrorMessage, PreviousSection68Note = string.Empty }, JsonRequestBehavior.AllowGet);

                        if (!string.IsNullOrEmpty(sec68NoteResModel.ResponseMessage))
                        {
                            //return Json(new { serverError = false, success = true, message = sec68NoteResModel.ResponseMessage, PreviousSection68Note = sec68NoteResModel.previousSection68Note });
                            return Json(new { serverError = false, success = true, message = sec68NoteResModel.ResponseMessage, PreviousSection68Note = sec68NoteResModel.previousSection68Note, IsUpdateButton = true, Sec68NotePreparedPart = sec68NoteResModel.Sec68NotePreparedPart }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            //return Json(new { serverError = false, success = true, message = "", PreviousSection68Note = string.Empty });
                            return Json(new { serverError = false, success = true, message = "", PreviousSection68Note = sec68NoteResModel.previousSection68Note, IsUpdateButton = false, Sec68NotePreparedPart = sec68NoteResModel.Sec68NotePreparedPart }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    //return Json(new { serverError = false, success = false, errorMessage = "Error occured in loading previous Section 68 Note. Plese try again.", PreviousSection68Note = string.Empty });
                    return Json(new { serverError = false, success = false, errorMessage = "Error occured in loading previous Section 68(2) Note. Plese try again.", PreviousSection68Note = string.Empty }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //return Json(new { serverError = true, success = false, errorMessage = "Error occured while loading previously added Section 68 Note.", PreviousSection68Note = string.Empty });
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while loading previously added Section 68(2) Note.", PreviousSection68Note = string.Empty }, JsonRequestBehavior.AllowGet);
            }

        }

        //Added by Madhur on 07/08/2021 for PND Tab in Popup
        //Changed by mayank on 16/08/2021
        public ActionResult LoadInsertUpdateDeleteView(string TableViewNamee,long PropertyID)
        {
            try
            {
                string TableViewName = TableViewNamee;
                if (TableViewName == "PropertyNumberDetails")
                {
                    return RedirectToAction("LoadpropertyNumberDetailsTabView", "PropertyNumberDetails", new { area = "DataEntryCorrection",PropertyID=PropertyID });

                }
                else
                {
                    return RedirectToAction("LoadPartyDetailsTabView", "PartyDetails", new { area = "DataEntryCorrection" });
                }

            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Insert update View", URLToRedirect = "/Home/HomePage" });
            }
        }


        //Not in use. Can be removed
        //Added by Madhusoodan on 08/08/2021
        //[HttpPost]
        //[ValidateAntiForgeryTokenOnAllPosts]
        //[EventAuditLogFilter(Description = "Load EC Report for Document By SROCode and DOcuentID")]
        //public ActionResult LoadECReport(FormCollection formCollection)
        //{
        //    try
        //    {

        //        #region User Variables and Objects
        //        string SROOfficeID = formCollection["SROOfficeID"];
        //        string DROfficeID = formCollection["DROfficeID"];

        //        long DocumentID = Convert.ToInt64(formCollection["DocumentID"]);

        //        //long DocNumber;
        //        //if (string.IsNullOrEmpty(formCollection["DocumentNumber"].ToString()))
        //        //{
        //        //    DocNumber = Convert.ToInt64(0);
        //        //}
        //        //else
        //        //{
        //        //    var regex = "^[0-9]*$";
        //        //    var matchDocumentNo = Regex.Match(formCollection["DocumentNumber"].ToString(), regex, RegexOptions.IgnoreCase);
        //        //    if (!matchDocumentNo.Success)
        //        //        DocNumber = Convert.ToInt64(0);
        //        //    else
        //        //        DocNumber = Convert.ToInt64(formCollection["DocumentNumber"]);
        //        //}
        //        //string BookType = formCollection["BookTypeID"];
        //        //string FinancialYear = formCollection["FinancialYear"];

        //        //long DocNumber = Convert.ToInt64(DocumentNumber);

        //        int SroId = Convert.ToInt32(SROOfficeID);
        //        int DroId = Convert.ToInt32(DROfficeID);

        //        var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
        //        System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
        //        Match mtch = regx.Match((string)searchValue);

        //        var JsonData = Json(new
        //        {
        //            draw = formCollection["draw"],
        //            recordsTotal = 0,
        //            recordsFiltered = 0,
        //            data = "",
        //            status = "0",
        //            errorMessage = "Invalid To Date"
        //        });

        //        CommonFunctions objCommon = new CommonFunctions();
        //        string errorMessage = String.Empty;
        //        #endregion

        //        #region Server Side Validation              


        //        caller = new ServiceCaller("CommonsApiController");
        //        short OfficeID = KaveriSession.Current.OfficeID;
        //        short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

        //        //Validation For DR Login
        //        if (LevelID == Convert.ToInt16(ECDataUI.Common.CommonEnum.LevelDetails.DR))
        //        {
        //            //Validation for DR when user do not select any sro which is by default "Select"
        //            if ((SroId == 0))
        //            {
        //                var emptyData = Json(new
        //                {
        //                    draw = formCollection["draw"],
        //                    recordsTotal = 0,
        //                    recordsFiltered = 0,
        //                    data = "",
        //                    status = false,
        //                    errorMessage = "Please select any SRO"
        //                });
        //                emptyData.MaxJsonLength = Int32.MaxValue;
        //                return emptyData;
        //            }
        //        }
        //        else
        //        {//Validations of Logins other than SR and DR

        //            //if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
        //            //{
        //            //    var emptyData = Json(new
        //            //    {
        //            //        draw = formCollection["draw"],
        //            //        recordsTotal = 0,
        //            //        recordsFiltered = 0,
        //            //        data = "",
        //            //        status = false,
        //            //        errorMessage = "Please select any District"
        //            //    });
        //            //    emptyData.MaxJsonLength = Int32.MaxValue;
        //            //    return emptyData;
        //            //}
        //            //else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
        //            //{
        //            //    var emptyData = Json(new
        //            //    {
        //            //        draw = formCollection["draw"],
        //            //        recordsTotal = 0,
        //            //        recordsFiltered = 0,
        //            //        data = "",
        //            //        status = false,
        //            //        errorMessage = "Please select any SRO"
        //            //    });
        //            //    emptyData.MaxJsonLength = Int32.MaxValue;
        //            //    return emptyData;

        //            //}
        //            //else if (DocNumber == 0)//when User selects DR but not SR which is by default "Select"
        //            //{
        //            //    var emptyData = Json(new
        //            //    {
        //            //        draw = formCollection["draw"],
        //            //        recordsTotal = 0,
        //            //        recordsFiltered = 0,
        //            //        data = "",
        //            //        status = false,
        //            //        errorMessage = "Please Select Module to Procced"
        //            //    });
        //            //    emptyData.MaxJsonLength = Int32.MaxValue;
        //            //    return emptyData;

        //            //}
        //            //else if (BookType == "0")//when User selects DR but not SR which is by default "Select"
        //            //{
        //            //    var emptyData = Json(new
        //            //    {
        //            //        draw = formCollection["draw"],
        //            //        recordsTotal = 0,
        //            //        recordsFiltered = 0,
        //            //        data = "",
        //            //        status = false,
        //            //        errorMessage = "Please Select Book Type to Procced"
        //            //    });
        //            //    emptyData.MaxJsonLength = Int32.MaxValue;
        //            //    return emptyData;

        //            //}
        //        }

        //        //if (string.IsNullOrEmpty(FinancialYear) || FinancialYear.Equals("Select"))
        //        //{
        //        //    var emptyData = Json(new
        //        //    {
        //        //        draw = formCollection["draw"],
        //        //        recordsTotal = 0,
        //        //        recordsFiltered = 0,
        //        //        data = "",
        //        //        status = "0",
        //        //        errorMessage = "FinancialYear required"
        //        //    });
        //        //    emptyData.MaxJsonLength = Int32.MaxValue;
        //        //    return emptyData;
        //        //}

        //        #endregion


        //        int startLen = Convert.ToInt32(formCollection["start"]);
        //        int totalNum = Convert.ToInt32(formCollection["length"]);
        //        int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
        //        int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
        //        int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

        //        DataEntryCorrectionViewModel reqModel = new DataEntryCorrectionViewModel();
        //        reqModel.startLen = startLen;
        //        reqModel.totalNum = totalNum;
        //        //reqModel.DocumentNumber = DocNumber;
        //        reqModel.SROfficeID = SroId;
        //        //reqModel.BookTypeID = Convert.ToInt32(BookType);
        //        //reqModel.FinancialYearStr = FinancialYear;
        //        reqModel.DROfficeID = Convert.ToInt32(DROfficeID);
        //        reqModel.OfficeID = OfficeID;
        //        reqModel.DocumentID = DocumentID;


        //        caller = new ServiceCaller("SelectDocumentAPIController");
        //        //int totalCount = caller.PostCall<DataEntryCorrectionViewModel, int>("GetSupportDocumentEnclosureTotalCount", reqModel, out errorMessage);

        //        //if (searchValue != null && searchValue != "")
        //        //{
        //        //    reqModel.startLen = 0;
        //        //    reqModel.totalNum = totalCount;
        //        //}

        //        //To get records of indexII report table 
        //        DataEntryCorrectionResultModel resModel = caller.PostCall<DataEntryCorrectionViewModel, DataEntryCorrectionResultModel>("LoadECReport", reqModel, out errorMessage);
        //        if (resModel == null)
        //        {
        //            return Json(new { success = false, errorMessage = "Error occured while getting EC Report" });
        //        }
        //        int totalCount = resModel.DataEntryCorrectionPropertyDetailList.Count;
        //        IEnumerable<DataEntryCorrectionPropertyDetailModel> result = resModel.DataEntryCorrectionPropertyDetailList;

        //        if (searchValue != null && searchValue != "")
        //        {
        //            reqModel.startLen = 0;
        //            reqModel.totalNum = totalCount;
        //        }

        //        //Sorting
        //        //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
        //        //{
        //        //    result = result.OrderBy(sortColumn + " " + sortColumnDir);
        //        //}
        //        if (!string.IsNullOrEmpty(searchValue))
        //        {
        //            if (mtch.Success)
        //            {
        //                if (!string.IsNullOrEmpty(searchValue))
        //                {

        //                    var emptyData = Json(new
        //                    {
        //                        draw = formCollection["draw"],
        //                        recordsTotal = 0,
        //                        recordsFiltered = 0,
        //                        data = "",
        //                        status = false,
        //                        errorMessage = "Please enter valid Search String "
        //                    });
        //                    emptyData.MaxJsonLength = Int32.MaxValue;
        //                    return emptyData;
        //                }

        //            }
        //            else
        //            {
        //                result = result.Where(m => m.PropertyDetails.ToLower().Contains(searchValue.ToLower()) ||
        //                m.PropertyDetails.ToLower().Contains(searchValue.ToLower()) ||
        //                m.ExecutionDateTime.ToLower().Contains(searchValue.ToLower()));
        //            }
        //        }

        //        var gridData = result.Select(DataEntryCorrectionViewModel => new
        //        {
        //            Sno = DataEntryCorrectionViewModel.SerialNo,
        //            //Added by Madhusoodan on 06/08/2021
        //            SelectButton = DataEntryCorrectionViewModel.Select,
        //            PNDTabButton = DataEntryCorrectionViewModel.ButtonPropertyNoDetails,
        //            PartyTabButton = DataEntryCorrectionViewModel.ButtonPartyDetails,
        //            //
        //            FinalRegistrationNumber = DataEntryCorrectionViewModel.FinalRegistrationNo,
        //            RegistrationDate = DataEntryCorrectionViewModel.RegistrationDate,
        //            ExecutionDate = DataEntryCorrectionViewModel.ExecutionDateTime,
        //            NatureOfDocument = DataEntryCorrectionViewModel.NatureOfDocument,
        //            PropertyDetails = DataEntryCorrectionViewModel.PropertyDetails,
        //            VillageName = DataEntryCorrectionViewModel.VillageName,
        //            TotalArea = DataEntryCorrectionViewModel.TotalArea,
        //            ScheduleDescription = DataEntryCorrectionViewModel.ScheduleDescription,
        //            Marketvalue = DataEntryCorrectionViewModel.Marketvalue,
        //            Consideration = DataEntryCorrectionViewModel.Consideration,
        //            Claimant = DataEntryCorrectionViewModel.Claimant,
        //            Executant = DataEntryCorrectionViewModel.Executant,
        //            //Added by Madhusoodan on 02/08/2021s
        //            DocumentID = DataEntryCorrectionViewModel.DocumentID,
        //            PropertyID = DataEntryCorrectionViewModel.PropertyID

        //        });


        //        if (searchValue != null && searchValue != "")
        //        {
        //            JsonData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
        //                //EncrytptedDocumetID = resModel.EncrytptedDocumetID,
        //                recordsTotal = totalCount,
        //                status = "1",
        //                recordsFiltered = totalCount
        //            });

        //        }
        //        else
        //        {
        //            JsonData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                data = gridData.ToArray(),
        //                //EncrytptedDocumetID = resModel.EncrytptedDocumetID,
        //                recordsTotal = totalCount,
        //                status = "1",
        //                recordsFiltered = totalCount,
        //            });
        //        }
        //        JsonData.MaxJsonLength = Int32.MaxValue;

        //        //Added by Madhusoodan on 02/08/2021 to save Document No, BookID and Financial Year in Session for other tabs
        //        KaveriSession.Current.DECDocumentNumber = reqModel.DocumentNumber;
        //        KaveriSession.Current.DECBookTypeID = reqModel.BookTypeID;
        //        KaveriSession.Current.DECFinancialYear = Int32.Parse(reqModel.FinancialYearStr.Split('-')[0]);
        //        KaveriSession.Current.DECSROCode = reqModel.SROfficeID;
        //        //
        //        return JsonData;
        //    }
        //    catch (Exception e)
        //    {
        //        ExceptionLogs.LogException(e);
        //        return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Property Details data" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //Added by Madhusoodan on 13/08/2021 to load Delete button for Section 68 Note
        [HttpGet]
        public ActionResult DeleteSectionNote(long NoteID)
        {
            try
            {
                caller = new ServiceCaller("SelectDocumentAPIController");
                bool deleteResult = caller.GetCall<bool>("DeleteSection68Note", new { NoteID = NoteID });
                if (deleteResult)
                    return Json(new { serverError = false, success = true, Message = "Section 68(2) Note Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { serverError = true, success = false, Message = "Error occured while deleting Section 68(2) Note. Please try again" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 13/08/2021 to lkoad finalize btn if Section 68 Note is added for Current Order ID
        [HttpGet]
        public ActionResult IsSectionNoteAddedForOrderID()
        {
            try
            {
                caller = new ServiceCaller("SelectDocumentAPIController");

                int currentOrderID = KaveriSession.Current.OrderID;
                bool showFinalizeButton = false;
                if (currentOrderID != 0)
                {
                    //If section 68 note is added for current orderID then load Finalize Button

                    showFinalizeButton = caller.GetCall<bool>("IsSection68NoteAddedForOrderID", new { currentOrderID = currentOrderID });

                    return Json(new { serverError = false, success = true, message = "", showFinalizeButton = showFinalizeButton }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { serverError = true, success = false, message = "Error occured while loading Finalize Order Button", showFinalizeButton = showFinalizeButton }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
		
		//Added by mayank on 12/08/2021
        [HttpGet]
        public ActionResult CheckifOrderNoteExist(long PropertyID)
        {
            try
            {
                long OrderId = KaveriSession.Current.OrderID;
                caller = new ServiceCaller("SelectDocumentAPIController");
                bool result = caller.GetCall<bool>("CheckifOrderNoteExist", new { OrderId = OrderId, PropertyID = PropertyID });
                if (result)
                    return Json(new { success = true, Message = "" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, Message = "Please Add Section 68(2) Note for Selected Property " }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}