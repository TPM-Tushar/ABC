using CustomModels.Models.PendingDocuments;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.PendingDocuments.Controllers
{
    [KaveriAuthorization]
    public class PendingDocumentsController : Controller
    {

        private ServiceCaller caller;

        public ActionResult PendingDocumentsView()
        {
            try
            {
                caller = new ServiceCaller("PendingDocumentsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                PendingDocumentsViewModel reqModel = caller.GetCall<PendingDocumentsViewModel>("PendingDocumentsView", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured", URLToRedirect = "/Home/HomePage" });
            }

        }


        [HttpPost]
        public ActionResult PendingDocumentAvailaibility(FormCollection formCollection)
        {
            List<PendingDocumentsTableModel> ResModel = new List<PendingDocumentsTableModel>();
            try
            {
                long UserId = KaveriSession.Current.UserID;
                string errorMessage = string.Empty;
                caller = new ServiceCaller("PendingDocumentsAPIController");


                ResModel = caller.GetCall<List<PendingDocumentsTableModel>>("PendingDocumentsAvailaibility", new { SROCode = Convert.ToInt32(formCollection["SROfficeID"]) }, out errorMessage);

                if (ResModel == null)
                {
                    if (ResModel == null)
                    {
                        errorMessage = "Some error occured while loading table.";
                    }
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = errorMessage
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                var order = Request.Form.GetValues("columns[0][orderable]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
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
                                errorMessage = "Please enter valid Search String."
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }

                    }
                    else
                    {
                        ResModel = ResModel.Where(m => m.PendingNumber.ToString().ToLower().Contains(searchValue.ToLower()) || m.PendingReason.ToString().ToLower().Contains(searchValue.ToLower())).ToList();
                    }
                }



                var gridData = ResModel.Select(PendingDocumentsTableModel => new
                {
                    SNo = PendingDocumentsTableModel.SrNo,
                    PendingNumber = PendingDocumentsTableModel.PendingNumber,
                    PresentationDate = PendingDocumentsTableModel.PresentationDate,
                    SROName = PendingDocumentsTableModel.SROName,
                    PendingReason = PendingDocumentsTableModel.PendingReason,
                    DateOfPending = PendingDocumentsTableModel.DateOfPending
                });

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum;
                int skip = startLen;
                int totalCount = ResModel.Count;
                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                    recordsTotal = totalCount,
                    status = "1",
                    recordsFiltered = totalCount
                });

                return JsonData;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                var emptyData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = ex.Message
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }
        }




    }
}