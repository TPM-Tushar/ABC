using CustomModels.Models.DataEntryCorrection;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.DataEntryCorrection.Controllers
{
    [KaveriAuthorization]
    public class PartyDetailsController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        public ActionResult LoadPartyDetailsTabView(long PartyID = 0)
        {
            try
            {
                caller = new ServiceCaller("PartyDetailsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;

                PartyDetailsViewModel reqModel = caller.GetCall<PartyDetailsViewModel>("GetPartyDetailsView", new { OfficeID = OfficeID, PartyID = PartyID });

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured in loading Party Details View", URLToRedirect = "/Home/HomePage" });
                }

                return View("LoadPartyDetailsTabView", reqModel);
            }
            catch (Exception ex)
            {
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while loading Party Details Tab" });
            }
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult AddUpdatePartyDetails(PartyDetailsViewModel partyDetailsViewModel)
        {
            string errorMessage = string.Empty;
            CommonFunctions objCommon = new CommonFunctions();

            ServiceCaller caller = new ServiceCaller("PartyDetailsAPIController");

            partyDetailsViewModel.OfficeID = KaveriSession.Current.OfficeID;
            partyDetailsViewModel.UserID = KaveriSession.Current.UserID;
            partyDetailsViewModel.IPAddress = objCommon.GetIPAddress();
            partyDetailsViewModel.SROfficeID = KaveriSession.Current.DECSROCode;

            //Check if OrderID is there in session
            if(KaveriSession.Current.OrderID == 0)
            {
                return Json(new { success = false, message = "Please initiate adding Party details process again." });
            }
            else
            {
                partyDetailsViewModel.OrderID = KaveriSession.Current.OrderID;
            }

            try
            {
                if (ModelState.IsValid)
                {

                    AddPartyDetailsResultModel responseModel = caller.PostCall<PartyDetailsViewModel, AddPartyDetailsResultModel>("AddUpdatePartyDetails", partyDetailsViewModel, out errorMessage);
                    
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

        //Check if it is in use ..
        //Added by Madhur 29-7-2021
        //[HttpPost]
        ////[ValidateAntiForgeryTokenOnAllPosts]
        ////[EventAuditLogFilter(Description = "Load Property details for Document By Document Number, Book Type and Financial Year")]
        //public ActionResult LoadPropertyDetailsPartyTabData(FormCollection formCollection)
        //{
        //    try
        //    {

        //        #region User Variables and Objects
        //        string SROOfficeID = formCollection["SROOfficeID"];
        //        string DROfficeID = formCollection["DROfficeID"];
        //        long DocNumber;
        //        if (string.IsNullOrEmpty(formCollection["DocumentNumber"].ToString()))
        //        {
        //            DocNumber = Convert.ToInt64(0);
        //        }
        //        else
        //        {
        //            var regex = "^[0-9]*$";
        //            var matchDocumentNo = Regex.Match(formCollection["DocumentNumber"].ToString(), regex, RegexOptions.IgnoreCase);
        //            if (!matchDocumentNo.Success)
        //                DocNumber = Convert.ToInt64(0);
        //            else
        //                DocNumber = Convert.ToInt64(formCollection["DocumentNumber"]);
        //        }
        //        string BookType = formCollection["BookTypeID"];
        //        string FinancialYear = formCollection["FinancialYear"];

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

        //            if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
        //            {
        //                var emptyData = Json(new
        //                {
        //                    draw = formCollection["draw"],
        //                    recordsTotal = 0,
        //                    recordsFiltered = 0,
        //                    data = "",
        //                    status = false,
        //                    errorMessage = "Please select any District"
        //                });
        //                emptyData.MaxJsonLength = Int32.MaxValue;
        //                return emptyData;
        //            }
        //            else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
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
        //            else if (DocNumber == 0)//when User selects DR but not SR which is by default "Select"
        //            {
        //                var emptyData = Json(new
        //                {
        //                    draw = formCollection["draw"],
        //                    recordsTotal = 0,
        //                    recordsFiltered = 0,
        //                    data = "",
        //                    status = false,
        //                    errorMessage = "Please Select Module to Procced"
        //                });
        //                emptyData.MaxJsonLength = Int32.MaxValue;
        //                return emptyData;

        //            }
        //            else if (BookType == "0")//when User selects DR but not SR which is by default "Select"
        //            {
        //                var emptyData = Json(new
        //                {
        //                    draw = formCollection["draw"],
        //                    recordsTotal = 0,
        //                    recordsFiltered = 0,
        //                    data = "",
        //                    status = false,
        //                    errorMessage = "Please Select Book Type to Procced"
        //                });
        //                emptyData.MaxJsonLength = Int32.MaxValue;
        //                return emptyData;

        //            }
        //        }

        //        if (string.IsNullOrEmpty(FinancialYear) || FinancialYear.Equals("Select"))
        //        {
        //            var emptyData = Json(new
        //            {
        //                draw = formCollection["draw"],
        //                recordsTotal = 0,
        //                recordsFiltered = 0,
        //                data = "",
        //                status = "0",
        //                errorMessage = "FinancialYear required"
        //            });
        //            emptyData.MaxJsonLength = Int32.MaxValue;
        //            return emptyData;
        //        }

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
        //        //reqModel.SROfficeID = decSROCode;
        //        // reqModel.BookTypeID = Convert.ToInt32(BookType);
        //        //reqModel.FinancialYearStr = FinancialYear;
        //        //reqModel.DROfficeID = Convert.ToInt32(DROfficeID);

        //        reqModel.DocumentNumber = decDocNo;
        //        reqModel.BookTypeID = decBookTypeID;
        //        reqModel.FinancialYearID = decFinancialYear;    
        //        reqModel.OfficeID = OfficeID;
        //        reqModel.SROfficeID = decSROCode;

        //        caller = new ServiceCaller("PartyDetailsAPIController");


        //        //To get records of indexII report table 
        //        DataEntryCorrectionResultModel resModel = caller.PostCall<DataEntryCorrectionViewModel, DataEntryCorrectionResultModel>("LoadPropertyDetailsPartyTabData", reqModel, out errorMessage);
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
        //            Select = DataEntryCorrectionViewModel.Select,
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
        //        return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Property Details" });
        //    }
        //}

        [HttpPost]
        public ActionResult SelectBtnPartyTabClick(FormCollection formCollection)
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
				reqModel.OrderID = KaveriSession.Current.OrderID;

                
                caller = new ServiceCaller("PartyDetailsAPIController");
                DataEntryCorrectionResultModel resModel = caller.PostCall<DataEntryCorrectionViewModel, DataEntryCorrectionResultModel>("SelectBtnPartyTabClick", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Party Details" });
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
                    PartyType = DataEntryCorrectionViewModel.PartyType,
                    FirstName = DataEntryCorrectionViewModel.FirstName,
                    MiddleName = DataEntryCorrectionViewModel.MiddleName,
                    LastName = DataEntryCorrectionViewModel.LastName,
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

        //[HttpGet]
        //public dynamic EditBtnClickOrderTable(string DROrderNumber)
        //{
        //    try
        //    {
        //        caller = new ServiceCaller("PartyDetailsAPIController");
        //        string reqModel = caller.GetCall<string>("EditBtnClickOrderTable", new { DROrderNumber = DROrderNumber });
        //        byte[] pdfByteArray = System.IO.File.ReadAllBytes(reqModel);
        //        return File(pdfByteArray, "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //Added by Madhur
        [HttpGet]
        public ActionResult DeletePartyDetails(long KeyId)
        {
            try
            {
                caller = new ServiceCaller("PartyDetailsAPIController");
                bool result= caller.GetCall<bool>("DeletePartyDetails", new { KeyId= KeyId });
                if (result)
                    return Json(new { serverError = false, success = true, Message = "Party Details Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { serverError = true, success = false, Message = "Error occured while deleting Party Details,Please contact admin" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult DeactivatePartyDetails(long KeyId)
        {
            try
            {
                caller = new ServiceCaller("PartyDetailsAPIController");
                int OrderID = KaveriSession.Current.OrderID;
                bool result= caller.GetCall<bool>("DeactivatePartyDetails", new { KeyId= KeyId ,OrderId= OrderID });
                if (result)
                    return Json(new { serverError = false, success = true, Message = "Party Details Deactivated Successfully" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { serverError = true, success = false, Message = "Error occured while deactivating Party Details,Please contact admin" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
    }
}