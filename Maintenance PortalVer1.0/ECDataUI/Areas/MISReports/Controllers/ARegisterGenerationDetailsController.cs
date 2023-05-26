using CustomModels.Models.MISReports.ARegisterGenerationDetails;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]
    public class ARegisterGenerationDetailsController : Controller
    {
        ServiceCaller caller = null;
        // GET: MISReports/ARegisterGenerationDetails
        [MenuHighlight]
        public ActionResult ARegisterGenerationDetailsView()
        {
            try
            {
                caller = new ServiceCaller("ARegisterGenerationDetailsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                ARegisterGenerationDetailsModel reqModel = caller.GetCall<ARegisterGenerationDetailsModel>("ARegisterGenerationDetailsView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving ARegister GenerationDetails View", URLToRedirect = "/Home/HomePage" });
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
       [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetARegisterGenerationReportsDetails(FormCollection formCollection)
         {
            try
            {
                caller = new ServiceCaller("ARegisterGenerationDetailsAPIController");
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROOfficeListID = formCollection["SROfficeID"];
                string DROfficeID = formCollection["DROfficeID"];
                //
                if(Convert.ToDateTime(fromDate) < new DateTime(2022,03,31))
                {
                    return Json(new { success = false, errorMessage = "A Register Report Generation details not available." }, JsonRequestBehavior.AllowGet);
                }
                    if (Convert.ToDateTime(ToDate) > DateTime.Today)
                {
                    return Json(new{ success = false, errorMessage = "Please select To Date less than or equal to today's date" },JsonRequestBehavior.AllowGet);
                }
                if (Convert.ToDateTime(fromDate) > Convert.ToDateTime(ToDate))
                {
                    return Json(new { success = false, errorMessage = "From Date can not be larger than To Date" }, JsonRequestBehavior.AllowGet);
                }
                    //
                    int SroId = Convert.ToInt32(SROOfficeListID);
                int DroId = Convert.ToInt32(DROfficeID);
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                int pageSize = totalNum;
                int skip = startLen;
                //
                var NGReportVal = formCollection["NGReport"];

                //
                String errorMessage = String.Empty;

                ARegisterGenerationDetailsModel reqModel = new ARegisterGenerationDetailsModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = fromDate;
                reqModel.ToDate = ToDate;
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);
                reqModel.DateTime_FromDate = Convert.ToDateTime(fromDate);
                reqModel.DateTime_ToDate = Convert.ToDateTime(ToDate);
                if (NGReportVal == "0")
                {
                    reqModel.NGFile = false;
                }
                else
                {
                    reqModel.NGFile = true;
                }

                IEnumerable<ARegisterGenerationDetailsTableModel> result = caller.PostCall<ARegisterGenerationDetailsModel, List<ARegisterGenerationDetailsTableModel>>("GetARegisterGenerationReportsDetails", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting A Register Report Generation details." });
                 

                }
                int TotalCount=result.Count();
                var gridData = result.Select(ARegisterGenerationDetailsTableModel => new
                {

                    srNo = ARegisterGenerationDetailsTableModel.srNo,
                    DistrictName = ARegisterGenerationDetailsTableModel.DistrictName,
                    SroName = ARegisterGenerationDetailsTableModel.SroName,
                    Receipt_Date = ARegisterGenerationDetailsTableModel.Receipt_Date,
                    IsReceiptsSynchronized = ARegisterGenerationDetailsTableModel.IsReceiptsSynchronized,
                    File_Gen_Date = ARegisterGenerationDetailsTableModel.File_Gen_Date,
                    File_Path = ARegisterGenerationDetailsTableModel.File_Path,

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
                        recordsFiltered= TotalCount,
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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting A Register Report Generation details." });
            }

        }

    }
}