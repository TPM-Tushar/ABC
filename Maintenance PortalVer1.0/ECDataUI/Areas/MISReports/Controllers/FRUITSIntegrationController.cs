using CustomModels.Models.MISReports.FRUITSIntegration;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorization]
    public class FRUITSIntegrationController : Controller
    {
        ServiceCaller caller = null;

        // GET: MISReports/FRUITSIntegration
        [MenuHighlight]
        public ActionResult KAVERIFRUITSIntegration()
        {
            try
            {
                KaveriFruitsIntegrationViewModel model = new KaveriFruitsIntegrationViewModel();

                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.KaveriIntegrationReport;
                int OfficeID = KaveriSession.Current.OfficeID;

                caller = new ServiceCaller("FRUITSIntegrationAPI");

                model = caller.GetCall<KaveriFruitsIntegrationViewModel>("KAVERIFRUITSIntegration", new { OfficeID = OfficeID });
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Kaveri Integration View", URLToRedirect = "/Home/HomePage" });
            }
            //return View();
        }

        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {

                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "All" });
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetFruitsRecvDetails(FormCollection formCollection)
        {
            try
            {
                KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel = new KaveriFruitsIntegrationViewModel();
                KaveriFruitsIntegrationResultModel kaveriFruitsIntegrationResultModel = new KaveriFruitsIntegrationResultModel();
                caller = new ServiceCaller("FRUITSIntegrationAPI");
                kaveriFruitsIntegrationViewModel.SROCode = Convert.ToInt32(formCollection["SroID"]);
                kaveriFruitsIntegrationViewModel.DistrictID = Convert.ToInt32(formCollection["DistrictID"]);
                kaveriFruitsIntegrationViewModel.FinancialyearCode = Convert.ToInt32(formCollection["FinancialYear"]);
                kaveriFruitsIntegrationViewModel.MonthCode = Convert.ToInt32(formCollection["Month"]);
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                if (kaveriFruitsIntegrationViewModel.MonthCode == 0)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Please Select month to view FRUITS pending report." });
                }
                caller.HttpClient.Timeout = new TimeSpan(0, 4, 0);
                kaveriFruitsIntegrationResultModel = caller.PostCall<KaveriFruitsIntegrationViewModel, KaveriFruitsIntegrationResultModel>("GetFruitsRecvDetails", kaveriFruitsIntegrationViewModel);
                IEnumerable<KaveriFruitsIntegrationDetailModel> result = kaveriFruitsIntegrationResultModel.KaveriFruitsIntegrationDetailList;


                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;

                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting EC Daily Receipt Details." });
                }
                int totalCount = kaveriFruitsIntegrationResultModel.KaveriFruitsIntegrationDetailList.Count;
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
                        result = result.Where(m => m.ReferenceNo.ToString().ToLower().Contains(searchValue.ToLower()));
                        kaveriFruitsIntegrationResultModel.KaveriFruitsIntegrationDetailList = result.ToList();
                        kaveriFruitsIntegrationResultModel.TotalCount = result.Count();
                    }
                }
                var gridData = kaveriFruitsIntegrationResultModel.KaveriFruitsIntegrationDetailList.Select(KaveriFruitsIntegrationDetailModel => new
                {
                    SrNo = KaveriFruitsIntegrationDetailModel.Sno,
                    ReferenceNo = KaveriFruitsIntegrationDetailModel.ReferenceNo,
                    Form3 = KaveriFruitsIntegrationDetailModel.Form3,
                    OfficeName = KaveriFruitsIntegrationDetailModel.OfficeName,
                    TranXML = KaveriFruitsIntegrationDetailModel.TranXML,
                    AcknowledgementNo = KaveriFruitsIntegrationDetailModel.AcknowledgementNo,
                    DataReceivedDate = KaveriFruitsIntegrationDetailModel.DataReceivedDate,
                });

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                    //data = gridData.ToArray().ToList(),
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    status = "1",

                });
                JsonData.MaxJsonLength = Int32.MaxValue;

                return JsonData;
                //}
                //else
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request ID tempered.", URLToRedirect = "/Home/HomePage" });
                //}
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }

        }
        [HttpGet]
        public ActionResult DownloadFormIII(string ReferenceNo, int SroCode)
        {
            try
            {
                KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel = new KaveriFruitsIntegrationViewModel();
                kaveriFruitsIntegrationViewModel.ReferenceNo = ReferenceNo;
                kaveriFruitsIntegrationViewModel.SROCode = SroCode;
                caller = new ServiceCaller("FRUITSIntegrationAPI");
                KaveriFruitsIntegrationResultModel kaveriFruitsIntegrationResultModel = caller.PostCall<KaveriFruitsIntegrationViewModel, KaveriFruitsIntegrationResultModel>("DownloadForm3", kaveriFruitsIntegrationViewModel);
                return File(kaveriFruitsIntegrationResultModel.Form3, System.Net.Mime.MediaTypeNames.Application.Octet, ReferenceNo + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
            }
            catch (Exception)
            {
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
                throw;
            }
        }

        [HttpGet]
        public ActionResult DownloadTransXML(string ReferenceNo, int SroCode)
        {
            try
            {
                KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel = new KaveriFruitsIntegrationViewModel();
                kaveriFruitsIntegrationViewModel.ReferenceNo = ReferenceNo;
                kaveriFruitsIntegrationViewModel.SROCode = SroCode;
                caller = new ServiceCaller("FRUITSIntegrationAPI");
                KaveriFruitsIntegrationResultModel kaveriFruitsIntegrationResultModel = caller.PostCall<KaveriFruitsIntegrationViewModel, KaveriFruitsIntegrationResultModel>("DownloadTransXML", kaveriFruitsIntegrationViewModel);
                return File(kaveriFruitsIntegrationResultModel.Form3, System.Net.Mime.MediaTypeNames.Application.Octet, ReferenceNo + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xml");
            }
            catch (Exception)
            {
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
                throw;
            }
        }
    }
}