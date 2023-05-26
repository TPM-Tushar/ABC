using CustomModels.Models.BhoomiMapping;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.BhoomiMapping.Controllers
{
    [KaveriAuthorization]
    public class IsVillageMatchingController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;
        [MenuHighlight]
        public ActionResult IsVillageMatchingView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IsVillageMatching(FormCollection formCollection)
        {
            try
            {
                var str = formCollection["str"];
                int OfficeID = KaveriSession.Current.OfficeID;
                int LevelID = KaveriSession.Current.LevelID;
                long UserID = KaveriSession.Current.UserID;
                caller = new ServiceCaller("IsVillageMatchingAPIController");
                List<IsVillageMatchingViewTableModel> reqModel = caller.GetCall<List<IsVillageMatchingViewTableModel>>("IsVillageMatchingView", new { officeID = OfficeID, LevelID = LevelID, UserID = UserID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading Village Matching Table.", URLToRedirect = "/Home/HomePage" });
                }
                else
                {
                    var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault() is null ? "" : Request.Form.GetValues("search[value]").FirstOrDefault();
                    Regex regx = new Regex("/^[^<>] +$/");
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
                            reqModel = reqModel.Where(m => m.SROName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                            m.IsVillageMatching.ToString().ToLower().Contains(searchValue.ToLower())).ToList();
                        }
                    }
                    var gridData = reqModel.Select(IsVillageMatchingViewTableModel => new
                    {
                        SNo = IsVillageMatchingViewTableModel.SrNo,
                        SROName = IsVillageMatchingViewTableModel.SROName,
                        IsVillageMatching = IsVillageMatchingViewTableModel.IsVillageMatching


                    });


                    int startLen = Convert.ToInt32(formCollection["start"]);
                    int totalNum = Convert.ToInt32(formCollection["length"]);
                    int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                    int pageSize = totalNum;
                    int skip = startLen;
                    int totalCount = reqModel.Count;
                    var JsonData = Json(new
                    {
                        draw = 0,
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount
                    });
                    return JsonData;
                }

                //return View(reqModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

