#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   MarriageAnalysisReportController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :    Controller for Marriage Analysis Report.

*/
#endregion
using CustomModels.Models.Remittance.MarriageAnalysisReport;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorization]
    public class MarriageAnalysisReportController : Controller
    {
        // GET: Remittance/MarriageAnalysisReport
        ServiceCaller caller = null;
        [MenuHighlight]
        [HttpGet]
        public ActionResult MarriageAnalysisReportView()
        {
           
            try
            {
                caller = new ServiceCaller("MarriageAnalysisReportAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                MarriageAnalysisReportModel reqModel = caller.GetCall<MarriageAnalysisReportModel>("MarriageAnalysisReportView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Marriage Analysis Report View", URLToRedirect = "/Home/HomePage" });
            }
        }


        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                string errormessage = string.Empty;
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "All" }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetMarriageAnalysisReportsDetails(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("MarriageAnalysisReportAPIController");
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROOfficeListID = formCollection["SROfficeID"];
                string DROfficeID = formCollection["DROfficeID"];
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                int pageSize = totalNum;
                int skip = startLen;
                //
                var BRIDEPersonID = formCollection["BRIDEPersonID"];
                var BRIDEGroomPersonID = formCollection["BRIDEGroomPersonID"];
                var WitnessCount = formCollection["WitnessCount"];
                var ReceiptCount = formCollection["ReceiptCount"];

                //
                String errorMessage = String.Empty;

                MarriageAnalysisReportModel reqModel = new MarriageAnalysisReportModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = fromDate;
                reqModel.ToDate = ToDate;
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);
                reqModel.DateTime_FromDate = Convert.ToDateTime(fromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                if(BRIDEPersonID == "0" && BRIDEGroomPersonID == "0")
                {
                    reqModel.BRIDEPersonID = true;
                    reqModel.BRIDEGroomPersonID = true;
                }
               else if (BRIDEPersonID == "0")
                {
                    reqModel.BRIDEPersonID = true;
                }else if(BRIDEGroomPersonID =="0")
                {
                    reqModel.BRIDEGroomPersonID = true;
                }else if(WitnessCount == "0")
                {
                    reqModel.WitnessCount = true;
                }else if(ReceiptCount == "0")
                {
                    reqModel.ReceiptCount = true;
                }
                else
                {
                    reqModel.BRIDEPersonID = false;
                    reqModel.BRIDEGroomPersonID = false;
                    reqModel.WitnessCount = false;
                    reqModel.ReceiptCount = false;
                }

                IEnumerable<MarriageAnalysisReportTableModel> result = caller.PostCall<MarriageAnalysisReportModel, List<MarriageAnalysisReportTableModel>>("GetMarriageAnalysisReportsDetails", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Marriage Analysis Reports details." });


                }
                int TotalCount = result.Count();
                var gridData = result.Select(MarriageAnalysisReportTableModel => new
                {

                    srNo = MarriageAnalysisReportTableModel.srNo,
                   
                    SroName = MarriageAnalysisReportTableModel.SroName,
                    MarriageCaseNo = MarriageAnalysisReportTableModel.MarriageCaseNo,
                    RegistrationID = MarriageAnalysisReportTableModel.RegistrationID,
                    Bride = MarriageAnalysisReportTableModel.Bride,
                    BrideGroom = MarriageAnalysisReportTableModel.BrideGroom,
                    BRIDEPersonID =MarriageAnalysisReportTableModel.BRIDEPersonID,
                    BRIDEGroomPersonID =MarriageAnalysisReportTableModel.BRIDEGroomPersonID
                });
                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = "Invalid To Date"
                });
                if (searchValue != null && searchValue != "")
                {

                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalNum,
                        recordsFiltered = TotalCount,
                        status = "1",

                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalNum,
                        recordsFiltered = TotalCount,
                        status = "1",

                    });
                }

                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;


            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Marriage Analysis Reports details." });
            }
        }

        }
}