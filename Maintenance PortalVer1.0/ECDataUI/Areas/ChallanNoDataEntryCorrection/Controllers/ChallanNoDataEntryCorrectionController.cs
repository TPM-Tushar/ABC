using CustomModels.Models.ChallanNoDataEntryCorrection;
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

namespace ECDataUI.Areas.ChallanNoDataEntryCorrection.Controllers
{
    [KaveriAuthorization]
    public class ChallanNoDataEntryCorrectionController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;



        [HttpGet]
        [MenuHighlight]
        [EventAuditLogFilter(Description = "Challan Details View")]
        public ActionResult ChallanNoDataEntryCorrectionView()
        {
            try
            {
                string MinChallanMonth = System.Configuration.ConfigurationManager.AppSettings["MinChallanMonthOfChallanNoDataEntryCorrection"];
                string MinChallanYear = System.Configuration.ConfigurationManager.AppSettings["MinChallanYearOfChallanNoDataEntryCorrection"];

                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ECDailyReceiptDetails;
                int OfficeID = KaveriSession.Current.OfficeID;

                caller = new ServiceCaller("ChallanNoDataEntryCorrectionAPIController");

                ChallanDetailsModel reqModel = caller.GetCall<ChallanDetailsModel>("ChallanNoDataEntryCorrectionView", new { officeID = OfficeID});
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Challan Details Report View", URLToRedirect = "/Home/HomePage" });
                }

                

                return View(reqModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        [EventAuditLogFilter(Description = "Get Challan Details Report")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetChallanReportDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("ChallanNoDataEntryCorrectionAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                string MinChallanMonth = System.Configuration.ConfigurationManager.AppSettings["MinChallanMonthOfChallanNoDataEntryCorrection"];
                string MinChallanYear = System.Configuration.ConfigurationManager.AppSettings["MinChallanYearOfChallanNoDataEntryCorrection"];
                string MinChallanYearTwoDigit = MinChallanYear.Substring(2, 2); 
                #region User Variables and Objects        

                string InstrumentNumber = formCollection["InstrumentNumber"];
                

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int SROOfficeListID = Convert.ToInt32(formCollection["SROOfficeListID"]);

                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });


                

                if (string.IsNullOrEmpty(InstrumentNumber))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Challan Number is required."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                Regex regex = new Regex(@"^[CR]{2}[(0-9)]{16}$");
                Regex reg = new Regex(@"^[IG]{2}[(0-9)]{16}$");
                if (!regex.IsMatch(InstrumentNumber))
                {
                    if(!reg.IsMatch(InstrumentNumber))
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Challan Number is not valid."
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }
                

                int year = DateTime.Now.Year % 100;
                string InstrumentNumberYearSTR = InstrumentNumber.Substring(4, 2);
                if (Convert.ToInt32(InstrumentNumberYearSTR) > year)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Challan Number is not valid."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                

                ChallanDetailsModel resModel = new ChallanDetailsModel();
                
                resModel.startLen = startLen;
                resModel.totalNum = totalNum; 
                resModel.Date = DateTime.Now.ToString();
                resModel.InstrumentNumber = InstrumentNumber;
                resModel.NewInstrumentNumber = "";
                resModel.NewInstrumentDate = "";
                resModel.ReEnterInstrumentNumber = "";


                string MonthName = getMonthName(Convert.ToInt32(MinChallanMonth));
                int InstrumentNumberYear = Convert.ToInt32(InstrumentNumber.Substring(4, 2));
                if (InstrumentNumberYear < Convert.ToInt32(MinChallanYearTwoDigit))
                {
                    return Json(new
                    {
                        serverError = false,
                        errorMessage = "Challan generated before " + MonthName + " " + MinChallanYear + " is not allowed for challan data entry correction "
                    });
                }
                else if (Convert.ToInt32(InstrumentNumber.Substring(4, 2)) <= Convert.ToInt32(MinChallanYearTwoDigit) && Convert.ToInt32(InstrumentNumber.Substring(2, 2)) < Convert.ToInt32(MinChallanMonth))
                {

                    return Json(new
                    {
                        serverError = false,
                        errorMessage = "Challan generated before " + MonthName + " " + MinChallanYear + " is not allowed for challan data entry correction "
                    });
                }


                resModel = caller.PostCall<ChallanDetailsModel, ChallanDetailsModel>("GetChallanReportDetails", resModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Challan Details." });
                }
                else if(resModel.message != null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = resModel.message });
                }
                IEnumerable<ChallanDetailsDataTableModel> result = resModel.challanDetailsDataTableList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Challan Details." });
                }
                else if(result.Count()< 1)
                {
                    return Json(new { serverError = false, success = false, errorMessage = "No data available for this challan number." });
                }
                int totalCount = resModel.challanDetailsDataTableList.Count;
                


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
                         m.SROName.ToLower().Contains(searchValue.ToLower()) || m.ChallanNumber.ToLower().Contains(searchValue.ToLower())
                         || m.DistrictName.ToLower().Contains(searchValue.ToLower())

                        );
                        totalCount = result.Count();
                    }
                }
                

                var gridData = result.Select(item => new
                {
                    SrNo = item.SrNo,
                    SROName = item.SROName,
                    IsPayDoneAtDROffice = item.IsPayDoneAtDROffice,
                    DistrictName = item.DistrictName,
                    ChallanNumber = item.ChallanNumber,
                    ChallanDate = item.ChallanDate,
                    Amount = item.Amount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    IsStampPayment = item.IsStampPayment,
                    IsReceiptPayment = item.IsReceiptPayment,
                    ReceiptNumber = item.ReceiptNumber,
                    Receipt_StampPayDate = item.Receipt_StampPayDate,
                    ServiceName = item.ServiceName,
                    DocumentPendingNumber = item.DocumentPendingNumber,
                    FinalRegistrationNumber = item.FinalRegistrationNumber,
                    NewInstrumentNumber = item.NewInstrumentNumber,
                    ReEnterInstrumentNumber = item.ReEnterInstrumentNumber,
                    NewInstrumentDate = item.NewInstrumentDate,
                    ReEnterInstrumentDate = item.ReEnterInstrumentDate,
                    Reason = item.Reason,
                });

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = totalCount,
                        UpdateBtn = result.Select(x => x.UpdateButton).FirstOrDefault(),
                        RemarkMessage = result.Select(x => x.RemarkMessage).FirstOrDefault(),
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(), //gridData.ToArray(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = resModel.TotalRecords,
                        UpdateBtn = result.Select(x => x.UpdateButton).FirstOrDefault(),
                        RemarkMessage = result.Select(x => x.RemarkMessage).FirstOrDefault(),
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Challan Details." }, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpPost]
        public ActionResult SaveChallanDetails(ChallanDetailsModel challanDetailsModel)
        {
            try
            {
                List<string> listStrLineElements = challanDetailsModel.HiddenInstrumentNo.Split(',').ToList();

                int ChallanMonth =  Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MinChallanMonthOfChallanNoDataEntryCorrection"]);
                string ChallanMonthString = System.Configuration.ConfigurationManager.AppSettings["MinChallanMonthOfChallanNoDataEntryCorrection"];
                int ChallanYear = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MinChallanYearOfChallanNoDataEntryCorrection"]);
                string ChallanYearTwoDigit = Convert.ToString(ChallanYear).Substring(2, 2);
                DateTime OldChallDate;
                int oldChallanDateMonth =-1;
                int oldChallanDateYear = -1;

                if (ModelState.IsValid)
                {
                    challanDetailsModel.OfficeID = KaveriSession.Current.OfficeID;
                    challanDetailsModel.LevelID = KaveriSession.Current.LevelID;
                    challanDetailsModel.UserID = KaveriSession.Current.UserID;
                    challanDetailsModel.InstrumentNoList = listStrLineElements;
                    string NewInstrumentNumber = challanDetailsModel.NewInstrumentNumber.Trim();
                    string ReEnterInstrumentNumber = challanDetailsModel.ReEnterInstrumentNumber.Trim();
                    string Reason = challanDetailsModel.Reason;

                    if(string.IsNullOrEmpty(NewInstrumentNumber))
                        return Json(new { success = false, message = "New Challan Number is mandatory." });

                    if (string.IsNullOrEmpty(ReEnterInstrumentNumber))
                        return Json(new { success = false, message = "Re-Enter Challan Number is mandatory." });
                    
                    if (NewInstrumentNumber != ReEnterInstrumentNumber)
                        return Json(new { success = false, message = "New Challan Number and Re-Enter challan Number must be same." });

                    if (!string.IsNullOrEmpty(challanDetailsModel.Date))
                    {
                        OldChallDate = Convert.ToDateTime(challanDetailsModel.Date);
                        oldChallanDateMonth = Convert.ToInt32(String.Format("{0: MM}", OldChallDate));
                        oldChallanDateYear = Convert.ToInt32(OldChallDate.ToString("yy"));
                    }

                    
                    if (challanDetailsModel.IsChallanDateSelected)
                    {
                        
                        if (string.IsNullOrEmpty(challanDetailsModel.NewInstrumentDate))
                            return Json(new { success = false, message = "New Challan Date is mandatory." });


                        if (string.IsNullOrEmpty(challanDetailsModel.ReEnterInstrumentDate))
                            return Json(new { success = false, message = "Re-Enter challan Date is mandatory." });

                        
                        string NewChallanDate = challanDetailsModel.NewInstrumentDate.Trim();
                        DateTime InstrumentDate = Convert.ToDateTime(NewChallanDate);
                        int month = Convert.ToInt32(String.Format("{0: MM}", InstrumentDate));
                        int year = Convert.ToInt32(InstrumentDate.ToString("yy"));



                        int isDateSame = DateTime.Compare(Convert.ToDateTime(challanDetailsModel.NewInstrumentDate).Date, Convert.ToDateTime(challanDetailsModel.ReEnterInstrumentDate).Date);
                        if (isDateSame != 0)
                            return Json(new { success = false, message = "New Challan Date and Re-Enter challan Date must be same." });


                        int IsReceipt_StampPayDateGreater = DateTime.Compare(InstrumentDate.Date, Convert.ToDateTime(challanDetailsModel.Receipt_StampPayDate).Date);
                        if (IsReceipt_StampPayDateGreater > 0)
                            return Json(new { success = false, message = "New Challan Date must be less than Receipt/stamp payment date." });


                        int InstrumentDateGreater = DateTime.Compare(Convert.ToDateTime(InstrumentDate).Date, DateTime.Today.Date);
                        if (InstrumentDateGreater > 0)
                            return Json(new { success = false, message = "New Challan Date must be less than today's Date." });


                        if (NewInstrumentNumber.Substring(0, 2) == "CR" && ReEnterInstrumentNumber.Substring(0, 2) == "CR")
                        {
                            Regex regex = new Regex(@"^[CR]{2}[(0-9)]{16}$");
                            if (!regex.IsMatch(NewInstrumentNumber) || Convert.ToInt32(NewInstrumentNumber.Substring(2, 2)) != month || Convert.ToInt32(NewInstrumentNumber.Substring(4, 2)) != year)
                            {
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
                        else if (NewInstrumentNumber.Substring(0, 2) == "IG" && ReEnterInstrumentNumber.Substring(0, 2) == "IG")
                        {
                            Regex regex = new Regex(@"^[IG]{2}[(0-9)]{16}$");
                            if (!regex.IsMatch(NewInstrumentNumber) || Convert.ToInt32(NewInstrumentNumber.Substring(2, 2)) != month || Convert.ToInt32(NewInstrumentNumber.Substring(4, 2)) != year)
                            {
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
                        else
                        {
                            return Json(new { success = false, message = "Challan Number must be start with CR or IG." });
                        }

                        if (InstrumentDate.Year < ChallanYear)
                            return Json(new { success = false, message = "Challan date year must be greater than " + ChallanYear.ToString() + "" });

                    }
                    else
                    {
                        if (NewInstrumentNumber.Substring(0, 2) == "CR" && ReEnterInstrumentNumber.Substring(0, 2) == "CR")
                        {
                            Regex regex = new Regex(@"^[CR]{2}[(0-9)]{16}$");
                            if (!regex.IsMatch(NewInstrumentNumber) || Convert.ToInt32(NewInstrumentNumber.Substring(2, 2)) != oldChallanDateMonth || Convert.ToInt32(NewInstrumentNumber.Substring(4, 2)) != oldChallanDateYear)
                            {
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
                        else if (NewInstrumentNumber.Substring(0, 2) == "IG" && ReEnterInstrumentNumber.Substring(0, 2) == "IG")
                        {
                            Regex regex = new Regex(@"^[IG]{2}[(0-9)]{16}$");
                            if (!regex.IsMatch(NewInstrumentNumber) || Convert.ToInt32(NewInstrumentNumber.Substring(2, 2)) != oldChallanDateMonth || Convert.ToInt32(NewInstrumentNumber.Substring(4, 2)) != oldChallanDateYear)
                            {
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
                        else
                        {
                            return Json(new { success = false, message = "Challan Number must be start with CR or IG." });
                        }
                    }

                    string MonthName = getMonthName(ChallanMonth);
                    int InstrumentNumberYear = Convert.ToInt32(NewInstrumentNumber.Substring(4, 2));
                    int ChallanDigit = Convert.ToInt32(ChallanYearTwoDigit);
                    if (InstrumentNumberYear < Convert.ToInt32(ChallanYearTwoDigit))
                    {
                        return Json(new
                        {
                            serverError = false,
                            message = "Challan generated before " + MonthName + " " + ChallanYear + " is not allowed for challan data entry correction "
                        });
                    }
                    else if (Convert.ToInt32(NewInstrumentNumber.Substring(4, 2)) <= Convert.ToInt32(ChallanYearTwoDigit) && Convert.ToInt32(NewInstrumentNumber.Substring(2, 2)) < ChallanMonth)
                    {
                        return Json(new
                        {
                            serverError = false,
                            message = "Challan generated before " + MonthName + " " + ChallanYear + " is not allowed for challan data entry correction "
                        });
                    }
                    //if (Convert.ToInt32(NewInstrumentNumber.Substring(4, 2)) > Convert.ToInt32(Convert.ToDateTime(challanDetailsModel.Receipt_StampPayDate).ToString("yy")))
                    //{
                    //    return Json(new
                    //    {
                    //        success = false,
                    //        message = "Please verify the challan number as per the format. <br/><br/>" +
                    //                  "New Challan Number < Reciept/Stamp Payment Date Year<br/><br/>" +
                    //                  "CR03<span style=\"color:red;\">22</span>123456789123" +
                    //                  " < 06/03/20<span style=\"color:red;\">22</span>"
                    //    });
                    //}
                    //else if (Convert.ToInt32(NewInstrumentNumber.Substring(4, 2)) == Convert.ToInt32(Convert.ToDateTime(challanDetailsModel.Receipt_StampPayDate).ToString("yy")))
                    //{
                    //    if (Convert.ToInt32(NewInstrumentNumber.Substring(2, 2)) > Convert.ToInt32(Convert.ToDateTime(challanDetailsModel.Receipt_StampPayDate).ToString("MM")))
                    //        return Json(new
                    //        {
                    //            success = false,
                    //            message = "Please verify the challan number as per the format. <br/><br/>" +
                    //                      "New Challan Number < Reciept/Stamp Payment Date Month<br/><br/>" +
                    //                      "CR<span style=\"color:red;\">03</span>22123456789123" +
                    //                      " < 06/<span style=\"color:red;\">03</span>/2022"
                    //        });
                    //}


                    if (string.IsNullOrEmpty(Reason))
                        return Json(new { success = false, message = "Reason is mandatory." });

                    if (Reason.Length > 250)
                        return Json(new { success = false, message = "Reason should be maximum 250 characters." });
                    

                    Regex regx = new Regex("[#$<>]");
                    Match requestMatch = regx.Match((string)Reason);
                    if (!string.IsNullOrEmpty(Reason))
                    {
                        if (requestMatch.Success)
                            return Json(new { success = false, message = "Please enter valid Reason." });
                    }
                    

                    caller = new ServiceCaller("ChallanNoDataEntryCorrectionAPIController");

                    ChallanNoDataEntryCorrectionResponse responseModel = caller.PostCall<ChallanDetailsModel, ChallanNoDataEntryCorrectionResponse>("SaveChallanDetails", challanDetailsModel);

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
        public ActionResult UpdateChallanDetails(string ChallanDetails)
        {
            try
            {
                
                var ChallanDetailArray =   ChallanDetails.Split(',').ToArray();

                string ChallanNumber = ChallanDetailArray[0].ToString();
                string OldChallanNo = ChallanDetailArray[1].ToString();
                string ChallanDate = ChallanDetailArray[2].ToString();
                string SROCodeString = ChallanDetailArray[3].ToString();
                string DistrictCodeString = ChallanDetailArray[4].ToString();
                string ChallanCorrectionID = string.Empty;

                for (int i = 5; i< ChallanDetailArray.Length; i++)
                {
                    ChallanCorrectionID += ChallanDetailArray[i].ToString() +",";
                }
                ChallanCorrectionID = ChallanCorrectionID.Substring(0, ChallanCorrectionID.Length - 1);


                int SROCode = -1;
                int DistrictCode = -1;

                if (!string.IsNullOrEmpty(SROCodeString))
                    SROCode = Convert.ToInt32(SROCodeString);

                
                if (!string.IsNullOrEmpty(DistrictCodeString))
                    DistrictCode = Convert.ToInt16(DistrictCodeString);

                string InstrumentNumber = ChallanNumber.Trim();
                string IDate = ChallanDate.Trim();



                

                if (string.IsNullOrEmpty(ChallanDate))
                {
                    if (string.IsNullOrEmpty(InstrumentNumber) || SROCode < 0 || string.IsNullOrEmpty(ChallanCorrectionID) )
                        return Json(new { success = false, message = "Exception occurred, while updating challan details." });


                    if (InstrumentNumber.Substring(0, 2) == "CR")
                    {
                        int a = Convert.ToInt32(DateTime.Now.ToString("yy"));
                        Regex regex = new Regex(@"^[CR]{2}[(0-9)]{16}$");
                        if (!regex.IsMatch(InstrumentNumber) )
                        {
                            return Json(new { success = false, message = "Exception occurred, Invalid Challan Number." });
                        }
                    }
                    else if (InstrumentNumber.Substring(0, 2) == "IG")
                    {
                        Regex regex = new Regex(@"^[IG]{2}[(0-9)]{16}$");
                        if (!regex.IsMatch(InstrumentNumber))
                        {
                            return Json(new { success = false, message = "Exception occurred, Invalid Challan Number." });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Exception occurred, Invalid Challan Number." });
                    }

                }
                else
                {

                    DateTime InstrumentDate = Convert.ToDateTime(IDate);
                    int month = Convert.ToInt32(String.Format("{0: MM}", InstrumentDate));
                    int year = Convert.ToInt32(InstrumentDate.ToString("yy"));


                    if (month > 12 || month < 1 || InstrumentDate.Year > DateTime.Now.Year || InstrumentDate.Day < 1 || InstrumentDate.Day > 31)
                        return Json(new { success = false, message = "Exception occurred, Invalid Challan Date." });


                    if (string.IsNullOrEmpty(InstrumentNumber) || string.IsNullOrEmpty(ChallanDate) || SROCode < 0 || string.IsNullOrEmpty(ChallanCorrectionID)  )
                        return Json(new { success = false, message = "Exception occurred, while updating challan details." });


                    if (InstrumentNumber.Substring(0, 2) == "CR")
                    {
                        Regex regex = new Regex(@"^[CR]{2}[(0-9)]{16}$");
                        if (!regex.IsMatch(InstrumentNumber) || Convert.ToInt32(InstrumentNumber.Substring(2, 2)) != month || Convert.ToInt32(InstrumentNumber.Substring(4, 2)) != year)
                        {
                            return Json(new { success = false, message = "Exception occurred, Invalid Challan Number." });
                        }
                    }
                    else if (InstrumentNumber.Substring(0, 2) == "IG")
                    {
                        Regex regex = new Regex(@"^[IG]{2}[(0-9)]{16}$");
                        if (!regex.IsMatch(InstrumentNumber) || Convert.ToInt32(InstrumentNumber.Substring(2, 2)) != month || Convert.ToInt32(InstrumentNumber.Substring(4, 2)) != year)
                        {
                            return Json(new { success = false, message = "Exception occurred, Invalid Challan Number." });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Exception occurred, Invalid Challan Number." });
                    }
                    

                }

                
                caller = new ServiceCaller("ChallanNoDataEntryCorrectionAPIController");

                ChallanNoDataEntryCorrectionResponse responseModel = caller.GetCall<ChallanNoDataEntryCorrectionResponse>("UpdateChallanDetails", new { ChallanCorrectionID = ChallanCorrectionID, InstrumentNumber = InstrumentNumber,InstrumentDate = IDate, SROCode= SROCode, DistrictCode = DistrictCode });
                
                if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                {
                    return Json(new { success = false, message = responseModel.ErrorMessage });
                }
                else
                {
                    return Json(new { success = true, message = responseModel.ResponseMessage });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }

        }

        
        

        public string getMonthName(int month)
        {
            string MonthName = string.Empty;
            if (month == 01 || month == 1)
                return "January";
            else if (month == 02 || month == 2)
                return "February";
            else if (month == 03 || month == 3)
                return "March";
            else if (month == 04 || month == 4)
                return "April";
            else if (month == 05 || month == 5)
                return "May";
            else if (month == 06 || month == 6)
                return "June";
            else if (month == 07 || month == 7)
                return "July";
            else if (month == 08 || month == 8)
                return "August";
            else if (month == 09 || month == 9)
                return "September";
            else if (month == 10 )
                return "October";
            else if (month == 11 )
                return "November";
            else 
                return "December";
            
        }

    }
    

}