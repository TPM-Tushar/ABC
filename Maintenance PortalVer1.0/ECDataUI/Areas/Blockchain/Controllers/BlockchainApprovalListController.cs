using CustomModels.Common;
using CustomModels.Models.Blockchain;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Blockchain.Controllers
{
    [KaveriAuthorization]
    public class BlockchainApprovalListController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;
        [MenuHighlight]
        public ActionResult BlockchainApprovalListView()
        {
            try
            {
                KaveriSession.Current.OrderID = 0;

                KaveriSession.Current.DECSROCode = 0;
                int OfficeID = KaveriSession.Current.OfficeID;
                int LevelID = KaveriSession.Current.LevelID;
                long UserID = KaveriSession.Current.UserID;
                caller = new ServiceCaller("BlockchainApprovalListAPIController");
                BlockchainViewModel reqModel = caller.GetCall<BlockchainViewModel>("BlockchainApprovalListView", new { officeID = OfficeID, LevelID = LevelID, UserID = UserID });


                return View(reqModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult GetSroCodebyDistrict(int DroCode)
        {
            try
            {
                caller = new ServiceCaller("BlockchainApprovalListAPIController");
                BlockchainViewModel resultModel = caller.GetCall<BlockchainViewModel>("GetSroCodebyDistrict", new { DroCode = DroCode });
                return Json(new { success = true, SroList = resultModel.SROfficeOrderList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Populate Data Table with approved list with respect to selected SRO and District")]
        public ActionResult LoadDetailsTable(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("BlockchainApprovalListAPIController");
                int DistrictID = Convert.ToInt32(formCollection["DroCode"]);
                int SROCode = Convert.ToInt32(formCollection["SroCode"]);

                List<BlockchainApprovalTableModel> reqModel = caller.GetCall<List<BlockchainApprovalTableModel>>("LoadDetailsTable", new { DroCode = DistrictID, SROCode = SROCode });

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading document details table", URLToRedirect = "/Home/HomePage" });

                }
                else
                {
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
                                    errorMessage = "Please enter valid Search String "
                                });
                                emptyData.MaxJsonLength = Int32.MaxValue;
                                return emptyData;
                            }

                        }
                        else
                        {
                            reqModel = reqModel.Where(m => m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                            m.NatureOfDocument.ToLower().Contains(searchValue.ToLower())).ToList();
                        }
                    }
                    var gridData = reqModel.Select(BlockchainApprovalTableModel => new
                    {

                        SNo = BlockchainApprovalTableModel.SNo,
                        FinalRegistrationNumber = BlockchainApprovalTableModel.FinalRegistrationNumber,
                        Stamp5DateTime = BlockchainApprovalTableModel.Stamp5DateTime,
                        RequestDate = BlockchainApprovalTableModel.RequestDate,
                        NatureOfDocument = BlockchainApprovalTableModel.NatureOfDocument,
                        ReasonDesc = BlockchainApprovalTableModel.ReasonDesc,
                        ApprovalDate = BlockchainApprovalTableModel.ApprovalDate,


                    });


                    int startLen = Convert.ToInt32(formCollection["start"]);
                    int totalNum = Convert.ToInt32(formCollection["length"]);
                    int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                    int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                    int skip = startLen;
                    int totalCount = reqModel.Count;
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

            }
            catch (Exception ex)
            {
                throw;
            }

        }



    }
}

