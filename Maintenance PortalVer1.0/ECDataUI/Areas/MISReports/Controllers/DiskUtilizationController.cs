
using CustomModels.Models.MISReports.DiskUtilization;
using ECDataUI.Common;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    public class DiskUtilizationController : Controller
    {

        ServiceCaller caller = null;

        //[EventAuditLogFilter(Description = "Document References View")]
        public ActionResult DiskUtilizationView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DiskUtilizationReport;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("DiskUtilizationAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                DiskUtilizationREQModel reqModel = caller.GetCall<DiskUtilizationREQModel>("DiskUtilizationView", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Document References View", URLToRedirect = "/Home/HomePage" });
                }
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Document References View", URLToRedirect = "/Home/HomePage" });
            }
        }


        //[EventAuditLogFilter(Description = "Document References Details")]
        //[ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult DiskUtilizationDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("DiskUtilizationAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {

                #region User Variables and Objects    
                string serverIdStr = formCollection["ServerId"];
                int ServerId = Convert.ToInt32(serverIdStr);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;


                DiskUtilizationREQModel reqModel = new DiskUtilizationREQModel();
                reqModel.ServerId = ServerId;
                reqModel.TotalNum = totalNum;
                reqModel.SearchValue = searchValue;

                DiskUtilizationWrapper ResModel = caller.PostCall<DiskUtilizationREQModel, DiskUtilizationWrapper>("DiskUtilizationDetails", reqModel, out errorMessage);

                if (ResModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Document References Details." });
                }

                if (ResModel.isError == true)
                {
                    return Json(new { serverError = true, success = false, errorMessage = ResModel.sErrorMessage });
                }

                IEnumerable<DiskUtilizationDetail> result = ResModel.DiskUtilizationDetailList;
                int totalCount = result.Count();

                
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
                          m.DriveName.ToLower().Contains(searchValue.ToLower()) ||
                          m.DriveType.ToLower().Contains(searchValue.ToLower()) ||
                          m.FileSystem.ToLower().Contains(searchValue.ToLower()) ||
                          m.TotalSpace.ToLower().Contains(searchValue.ToLower()) ||
                          m.UsedSpace.ToLower().Contains(searchValue.ToLower()) ||
                          m.FreeSpace.ToLower().Contains(searchValue.ToLower())
                        );

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(DocumentReferencesDetail => new
                {
                    SerialNo = DocumentReferencesDetail.SerialNumber,
                    DriveName = DocumentReferencesDetail.DriveName,
                    DriveType = DocumentReferencesDetail.DriveType,
                    FileSystem = DocumentReferencesDetail.FileSystem,
                    TotalSpace = DocumentReferencesDetail.TotalSpace,
                    UsedSpace = DocumentReferencesDetail.UsedSpace,
                    FreeSpace = DocumentReferencesDetail.FreeSpace,
                    FreeSpacePercentage = DocumentReferencesDetail.FreeSpacePercentage
                });

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = ResModel.TotalRecords,
                        //commented on 22-11-2019
                        //status = "1",
                        recordsFiltered = totalCount
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
                        recordsTotal = ResModel.TotalRecords,
                        //commented on 22-11-2019
                        //status = "1",
                        recordsFiltered = ResModel.TotalRecords
                        //PDFDownloadBtn = PDFDownloadBtn,
                        //ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Document References." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}