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
    public class PropertyNumberDetailsController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        [HttpGet]
        public ActionResult LoadpropertyNumberDetailsTabView(long PropertyID) //Check if Property ID is required or not
        {
            try
            {
                caller = new ServiceCaller("PropertyNumberDetailsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                long OrderId = KaveriSession.Current.OrderID;
                //Changed by mayank on 16/08/2021
                //PropertyNumberDetailsViewModel reqModel = caller.GetCall<PropertyNumberDetailsViewModel>("GetPropertyNoDetailsView", new { OfficeID = OfficeID, PropertyID = PropertyID });
                PropertyNumberDetailsViewModel reqModel = caller.GetCall<PropertyNumberDetailsViewModel>("GetPropertyNoDetailsView", new { OfficeID = OfficeID, PropertyID = PropertyID, OrderId = OrderId });

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured in loading Property Number Details View", URLToRedirect = "/Home/HomePage" });
                }

                return View("LoadpropertyNumberDetailsTabView", reqModel);
            }
            catch (Exception ex)
            {
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while loading Property Number Details Tab" });
            }
        }

        //Check if it is in use ..
        //[HttpPost]
        ////[ValidateAntiForgeryTokenOnAllPosts]
        ////[EventAuditLogFilter(Description = "Load Property details for Document By Document Number, Book Type and Financial Year")]
        //public ActionResult LoadPropertyDetailsData(FormCollection formCollection)
        //{
        //    try
        //    {

        //        #region User Variables and Objects
        //        //Commented by Madhusoodan on 02/08/2021 to remove hardcoded values

        //        //string SROOfficeID = formCollection["SROOfficeID"];
        //        //string DROfficeID = formCollection["DROfficeID"];
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

        //        //int SroId = Convert.ToInt32(SROOfficeID);
        //        //int DroId = Convert.ToInt32(DROfficeID);

        //        var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
        //        System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
        //        Match mtch = regx.Match((string)searchValue);

        //        //var JsonData = Json(new
        //        //{
        //        //    draw = formCollection["draw"],
        //        //    recordsTotal = 0,
        //        //    recordsFiltered = 0,
        //        //    data = "",
        //        //    status = "0",
        //        //    errorMessage = "Invalid To Date"
        //        //});

        //        var JsonData = Json(new { });

        //        CommonFunctions objCommon = new CommonFunctions();
        //        string errorMessage = String.Empty;
        //        #endregion

        //        #region Server Side Validation              


        //        caller = new ServiceCaller("CommonsApiController");
        //        short OfficeID = KaveriSession.Current.OfficeID;
        //        short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

        //        //Validation For DR Login
        //        //if (LevelID == Convert.ToInt16(ECDataUI.Common.CommonEnum.LevelDetails.DR))
        //        //{
        //        //    //Validation for DR when user do not select any sro which is by default "Select"
        //        //    if ((SroId == 0))
        //        //    {
        //        //        var emptyData = Json(new
        //        //        {
        //        //            draw = formCollection["draw"],
        //        //            recordsTotal = 0,
        //        //            recordsFiltered = 0,
        //        //            data = "",
        //        //            status = false,
        //        //            errorMessage = "Please select any SRO"
        //        //        });
        //        //        emptyData.MaxJsonLength = Int32.MaxValue;
        //        //        return emptyData;
        //        //    }
        //        //}
        //        //else
        //        //{//Validations of Logins other than SR and DR

        //        //    if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
        //        //    {
        //        //        var emptyData = Json(new
        //        //        {
        //        //            draw = formCollection["draw"],
        //        //            recordsTotal = 0,
        //        //            recordsFiltered = 0,
        //        //            data = "",
        //        //            status = false,
        //        //            errorMessage = "Please select any District"
        //        //        });
        //        //        emptyData.MaxJsonLength = Int32.MaxValue;
        //        //        return emptyData;
        //        //    }
        //        //    else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
        //        //    {
        //        //        var emptyData = Json(new
        //        //        {
        //        //            draw = formCollection["draw"],
        //        //            recordsTotal = 0,
        //        //            recordsFiltered = 0,
        //        //            data = "",
        //        //            status = false,
        //        //            errorMessage = "Please select any SRO"
        //        //        });
        //        //        emptyData.MaxJsonLength = Int32.MaxValue;
        //        //        return emptyData;

        //        //    }
        //        //    else if (DocNumber == 0)//when User selects DR but not SR which is by default "Select"
        //        //    {
        //        //        var emptyData = Json(new
        //        //        {
        //        //            draw = formCollection["draw"],
        //        //            recordsTotal = 0,
        //        //            recordsFiltered = 0,
        //        //            data = "",
        //        //            status = false,
        //        //            errorMessage = "Please Select Module to Procced"
        //        //        });
        //        //        emptyData.MaxJsonLength = Int32.MaxValue;
        //        //        return emptyData;

        //        //    }
        //        //    else if (BookType == "0")//when User selects DR but not SR which is by default "Select"
        //        //    {
        //        //        var emptyData = Json(new
        //        //        {
        //        //            draw = formCollection["draw"],
        //        //            recordsTotal = 0,
        //        //            recordsFiltered = 0,
        //        //            data = "",
        //        //            status = false,
        //        //            errorMessage = "Please Select Book Type to Procced"
        //        //        });
        //        //        emptyData.MaxJsonLength = Int32.MaxValue;
        //        //        return emptyData;

        //        //    }
        //        //}

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

        //        long? decDocNo = KaveriSession.Current.DECDocumentNumber;
        //        int decBookTypeID = KaveriSession.Current.DECBookTypeID;
        //        int decFinancialYear = KaveriSession.Current.DECFinancialYear;
        //        int decSROCode = KaveriSession.Current.DECSROCode;

        //        if (decDocNo == null || decDocNo == 0 || decBookTypeID == 0 || decFinancialYear == 0 || decSROCode == 0)
        //        {
        //            return Json(new { success = false, errorMessage = "Error occured while getting property details. Please try again." });
        //        }

        //        DataEntryCorrectionViewModel reqModel = new DataEntryCorrectionViewModel();
        //        reqModel.startLen = startLen;
        //        reqModel.totalNum = totalNum;
        //        //reqModel.DocumentNumber = DocNumber;
        //        //reqModel.SROfficeID = SroId;
        //        //reqModel.BookTypeID = Convert.ToInt32(BookType);
        //        //reqModel.DROfficeID = Convert.ToInt32(DROfficeID);
        //        reqModel.DocumentNumber = decDocNo;
        //        reqModel.BookTypeID = decBookTypeID;
        //        reqModel.FinancialYearID = decFinancialYear;
        //        reqModel.OfficeID = OfficeID;
        //        reqModel.SROfficeID = decSROCode;

        //        caller = new ServiceCaller("PropertyNumberDetailsAPIController");

        //        //if (searchValue != null && searchValue != "")
        //        //{
        //        //    reqModel.startLen = 0;
        //        //    reqModel.totalNum = totalCount;
        //        //}

        //        //To get records of EC Report table 
        //        DataEntryCorrectionResultModel resModel = caller.PostCall<DataEntryCorrectionViewModel, DataEntryCorrectionResultModel>("LoadPropertyDetailsData", reqModel, out errorMessage);
        //        if (resModel == null)
        //        {
        //            return Json(new { success = false, errorMessage = "Error occured while getting property details" });
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
        //            Select= DataEntryCorrectionViewModel.Select,
        //            Sno = DataEntryCorrectionViewModel.SerialNo,
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
        //            DocumentID = DataEntryCorrectionViewModel.DocumentID,
        //            PropertyID = DataEntryCorrectionViewModel.PropertyID
        //        });


        //        if (searchValue != null && searchValue != "")
        //        {
        //            JsonData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
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
        //                recordsTotal = totalCount,
        //                status = "1",
        //                recordsFiltered = totalCount,
        //            });
        //        }
        //        JsonData.MaxJsonLength = Int32.MaxValue;
        //        return JsonData;
        //    }
        //    catch (Exception e)
        //    {
        //        ExceptionLogs.LogException(e);
        //        return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Support Enclosure Details" });
        //    }
        //}


        [HttpPost]
        public ActionResult SelectBtnClick(FormCollection formCollection)
        {
            try
            {
                string errorMessage = String.Empty;
                string DocumentID = formCollection["DocumentID"];
                string PropertyID = formCollection["PropertyID"];
                int OfficeID = KaveriSession.Current.OfficeID;
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                DataEntryCorrectionViewModel reqModel = new DataEntryCorrectionViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.OfficeID = OfficeID;
                reqModel.DocumentID = Int64.Parse(DocumentID);
                reqModel.PropertyID = Int64.Parse(PropertyID);
                reqModel.SROfficeID = KaveriSession.Current.DECSROCode;

                //Added on 05/08/2021 for Deactivate btn
                reqModel.OrderID = KaveriSession.Current.OrderID;

                caller = new ServiceCaller("PropertyNumberDetailsAPIController");
                DataEntryCorrectionResultModel resModel = caller.PostCall<DataEntryCorrectionViewModel, DataEntryCorrectionResultModel>("SelectBtnClick", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting property details" });
                }
                int totalCount = resModel.DataEntryCorrectionPropertyDetailList.Count;
                IEnumerable<DataEntryCorrectionPropertyDetailModel> result = resModel.DataEntryCorrectionPropertyDetailList;


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
                    DROrderNumber = DataEntryCorrectionViewModel.DROrderNumber,
                    OrderDate = DataEntryCorrectionViewModel.OrderDate,
                    Village = DataEntryCorrectionViewModel.Village,
                    CurrentPropertyType = DataEntryCorrectionViewModel.CurrentPropertyType,
                    CurrentNumber = DataEntryCorrectionViewModel.CurrentNumber,
                    OldPropertyType = DataEntryCorrectionViewModel.OldPropertyType,
                    OldNumber = DataEntryCorrectionViewModel.OldNumber,
                    Survey_No = DataEntryCorrectionViewModel.Survey_No,
                    Surnoc = DataEntryCorrectionViewModel.Surnoc,
                    Hissa_No = DataEntryCorrectionViewModel.Hissa_No,
                    CorrectionNote = DataEntryCorrectionViewModel.CorrectionNote,
                    Action = DataEntryCorrectionViewModel.Action,
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
                    });
                }
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Support Enclosure Details" });
            }
        }


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult AddUpdatePropertyNoDetails(PropertyNumberDetailsViewModel propertyNumberDetailsViewModel)
        {
            string errorMessage = string.Empty;
            CommonFunctions objCommon = new CommonFunctions();

            ServiceCaller caller = new ServiceCaller("PropertyNumberDetailsAPIController");

            propertyNumberDetailsViewModel.OfficeID = KaveriSession.Current.OfficeID;
            propertyNumberDetailsViewModel.UserID = KaveriSession.Current.UserID;
            propertyNumberDetailsViewModel.IPAddress = objCommon.GetIPAddress();
            propertyNumberDetailsViewModel.SROfficeID = KaveriSession.Current.DECSROCode;


            //Added by Shivam B on 10/05/2022 For Validation of Sub Registrar 

            if ((propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.SroCode == 0))//when user do not select Sub Registrar which are by default "Select"
            {
                return Json(new { success = false, message = "Please Select Sub Registrar" });
            }
            if ((propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.VillageID == 0))//when user do not select Sub Registrar which are by default "Select"
            {
                return Json(new { success = false, message = "Please Select Village/Area" });
            }
            //Added by Shivam B on 10/05/2022 For Validation of Sub Registrar


            //Check if OrderID is there in session
            if (KaveriSession.Current.OrderID == 0)
            {
                return Json(new { success = false, message = "Please initiate adding Party details process again." });
            }
            else
            {
                propertyNumberDetailsViewModel.OrderID = KaveriSession.Current.OrderID;
            }

            try
            {
                if (propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.OldPropertyTypeID == "0")
                {
                    ModelState.Remove("PropertyNumberDetailsAddEditModel.OldNumber");
                    ModelState.Remove("PropertyNumberDetailsAddEditModel.OldPropertyTypeID");
                }


                if (propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentPropertyTypeID == 1)
                {
                    ModelState.Remove("PropertyNumberDetailsAddEditModel.CurrentNumber");
                }
                else
                {
                    ModelState.Remove("PropertyNumberDetailsAddEditModel.CurrentSurveyNumber");
                    ModelState.Remove("PropertyNumberDetailsAddEditModel.CurrentSurveyNoChar");
                    ModelState.Remove("PropertyNumberDetailsAddEditModel.CurrentHissaNumber");
                }
                if (propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentPropertyTypeID == 0)
                {
                    ModelState.Remove("PropertyNumberDetailsAddEditModel.CurrentPropertyTypeID");
                }
                if (ModelState.IsValid)
                {
                    //if (propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentHissaNumber == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentNumber == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNoChar == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNumber == 0 && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.OldNumber == null)
                    //{

                    //    String messages = String.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
                    //    return Json(new { success = false, message = "All the fields are empty. Please try again." });
                    //}

                    //else if (propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentHissaNumber == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentNumber == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNoChar == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNumber.ToString() == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.OldNumber == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.VillageID != null)
                    //{

                    //    String messages = String.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
                    //    return Json(new { success = false, message = "Add more fields to continue." });
                    //}

                    //else
                    //{

                    AddPropertyNoDetailsResultModel responseModel = caller.PostCall<PropertyNumberDetailsViewModel, AddPropertyNoDetailsResultModel>("AddUpdatePropertyNoDetails", propertyNumberDetailsViewModel, out errorMessage);

                    if (!String.IsNullOrEmpty(errorMessage))
                        return Json(new { success = false, message = errorMessage });


                    if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                    {
                        return Json(new { success = false, message = responseModel.ErrorMessage });
                    }
                    else
                    {
                        return Json(new { success = true, message = responseModel.ResponseMessage });
                    }
                    //}
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


        //Added by Madhur 29-7-2021
        //[HttpGet]
        //public ActionResult EditBtnProperty(int OrderID)
        //{
        //    try
        //    {
        //        var JsonData = Json(new { });
        //        caller = new ServiceCaller("PropertyNumberDetailsAPIController");
        //        EditbtnResultModel reqModel = caller.PostCall<int, EditbtnResultModel>("EditBtnProperty", OrderID);
        //        if (reqModel != null)
        //        {
        //            JsonData = Json(new
        //            {

        //                data = reqModel
        //            });
        //            return JsonData;
        //        }
        //        else
        //        {
        //            JsonData = Json(new
        //            {

        //                data = "Error"
        //            });
        //            return JsonData;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //Added by Madhur
        [HttpGet]
        public ActionResult DeletePropertyNoDetails(long KeyId)
        {
            try
            {
                caller = new ServiceCaller("PropertyNumberDetailsAPIController");
                bool result = caller.GetCall<bool>("DeletePropertyNoDetails", new { KeyId = KeyId });
                if (result)
                    return Json(new { serverError = false, success = true, Message = "Property Number Details Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { serverError = true, success = false, Message = "Error occured while deleting Property Number Details,Please contact admin" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                throw;
            }
        }
        ////Added by Madhur 29-7-2021


        ////Added by mayank on 10/08/2021 to Update PropertyNo details
        //public ActionResult UpdatePropertyNoDetails(PropertyNumberDetailsViewModel propertyNumberDetailsViewModel)
        //{
        //    string errorMessage = string.Empty;
        //    CommonFunctions objCommon = new CommonFunctions();

        //    ServiceCaller caller = new ServiceCaller("PropertyNumberDetailsAPIController");

        //    propertyNumberDetailsViewModel.OfficeID = KaveriSession.Current.OfficeID;
        //    propertyNumberDetailsViewModel.UserID = KaveriSession.Current.UserID;
        //    propertyNumberDetailsViewModel.IPAddress = objCommon.GetIPAddress();
        //    propertyNumberDetailsViewModel.SROfficeID = KaveriSession.Current.DECSROCode;

        //    //Check if OrderID is there in session
        //    if (KaveriSession.Current.OrderID == 0)
        //    {
        //        return Json(new { success = false, message = "Please initiate adding Party details process again." });
        //    }
        //    else
        //    {
        //        propertyNumberDetailsViewModel.OrderID = KaveriSession.Current.OrderID;
        //    }

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            //if (propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentHissaNumber == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentNumber == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNoChar == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNumber == 0 && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.OldNumber == null)
        //            //{

        //            //    String messages = String.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
        //            //    return Json(new { success = false, message = "All the fields are empty. Please try again." });
        //            //}

        //            //else if (propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentHissaNumber == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentNumber == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNoChar == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.CurrentSurveyNumber.ToString() == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.OldNumber == null && propertyNumberDetailsViewModel.PropertyNumberDetailsAddEditModel.VillageID != null)
        //            //{

        //            //    String messages = String.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
        //            //    return Json(new { success = false, message = "Add more fields to continue." });
        //            //}

        //            //else
        //            //{

        //                AddPropertyNoDetailsResultModel responseModel = caller.PostCall<PropertyNumberDetailsViewModel, AddPropertyNoDetailsResultModel>("UpdatePropertyNoDetails", propertyNumberDetailsViewModel, out errorMessage);

        //                if (!String.IsNullOrEmpty(errorMessage))
        //                    return Json(new { success = false, message = errorMessage });


        //                if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
        //                {
        //                    return Json(new { success = false, message = responseModel.ErrorMessage });
        //                }
        //                else
        //                {
        //                    return Json(new { success = true, message = responseModel.ResponseMessage });
        //                }
        //            //}
        //        }
        //        else
        //        {
        //            String messages = String.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
        //            return Json(new { success = false, message = messages });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogs.LogException(ex);
        //        return Json(new { success = false, message = ex.GetBaseException().Message });
        //    }
        //}

        [HttpGet]
        public ActionResult DeactivatePropertyNoDetails(long KeyId)
        //public ActionResult DeactivatePropertyNoDetails(PropertyNumberDetailsAddEditModel PropertyNumberDetailsAddEditModel)
        {
            try
            {
                //long KeyId = Convert.ToInt64(formCollection["KeyId"]);
                caller = new ServiceCaller("PropertyNumberDetailsAPIController");
                int OrderId = KaveriSession.Current.OrderID;
                bool result = caller.GetCall<bool>("DeactivatePropertyNoDetails", new { KeyId = KeyId, OrderId = OrderId });
                if (result)
                    return Json(new { serverError = false, success = true, Message = "Property Number Details Deactivated Successfully" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { serverError = true, success = false, Message = "Error occured while dectivating Property Number Details,Please contact admin" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added by Madhusoodan on 18/08/2021 to activate PropertyNoDetails
        [HttpGet]
        public ActionResult ActivatePropertyNoDetails(long KeyId)
        {
            try
            {
                caller = new ServiceCaller("PropertyNumberDetailsAPIController");
                int OrderId = KaveriSession.Current.OrderID;

                bool result = caller.GetCall<bool>("ActivatePropertyNoDetails", new { KeyId = KeyId, OrderId = OrderId });
                if (result)
                    return Json(new { serverError = false, success = true, Message = "Property Number Details Activated Successfully" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { serverError = true, success = false, Message = "Error occured while activating Property Number Details,Please contact admin" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                throw;
            }
        }



        //Added by Shivam B on 10/05/2022 Populate SROList on DRO Change
        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DroCode)
        {
            try
            {
                caller = new ServiceCaller("PropertyNumberDetailsAPIController");
                PropertyNumberDetailsAddEditModel propertyNumberDetailsAddEditModel = caller.GetCall<PropertyNumberDetailsAddEditModel>("GetSROListByDROCode", new { DroCode = DroCode });
                return Json(new { serverError = false, success = true, SROOfficeList = propertyNumberDetailsAddEditModel.SROfficeList }, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }
        //Added by Shivam B on 10/05/2022 Populate SROList on DRO Change


        [HttpGet]
        public ActionResult GetVillageBySROCode(int SroCode)
        {
            try
            {
                caller = new ServiceCaller("PropertyNumberDetailsAPIController");
                PropertyNumberDetailsAddEditModel propertyNumberDetailsAddEditModel = caller.GetCall<PropertyNumberDetailsAddEditModel>("GetVillageBySROCode", new { SroCode = SroCode });
                return Json(new { serverError = false, success = true, VillageList = propertyNumberDetailsAddEditModel.VillageList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult GetHobliDetailsOnVillageSroCode(long VillageCode, int SroCode)
        {
            try
            {
                caller = new ServiceCaller("PropertyNumberDetailsAPIController");
                PropertyNumberDetailsAddEditModel propertyNumberDetailsAddEditModel = caller.GetCall<PropertyNumberDetailsAddEditModel>("GetHobliDetailsOnVillageSroCode", new { VillageCode = VillageCode, SroCode = SroCode });
                return Json(new { serverError = false, success = true, HobliName = propertyNumberDetailsAddEditModel.HobliName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}