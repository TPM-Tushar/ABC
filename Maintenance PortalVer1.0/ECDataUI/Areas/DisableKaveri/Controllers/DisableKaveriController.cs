using CustomModels.Models.DisableKaveri;
using ECDataUI.Common;
using ECDataUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.DisableKaveri.Controllers
{
    [KaveriAuthorization]
    public class DisableKaveriController : Controller
    {
        // GET: DisableKaveri/DisableKaveri
        ServiceCaller caller = null;
        [HttpGet]
        [MenuHighlight]
        // GET: DisableKaveri/DisableKaveri
        public ActionResult DisableKaveriView()
        {
            try
            {
                DisableKaveriViewModel reqModel = new DisableKaveriViewModel();

                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Disable Kaveri View", URLToRedirect = "/Home/HomePage" });
            }
        }

        public ActionResult DisableKaveriViewDetails()
        {
            try
            {
                caller = new ServiceCaller("DisableKaveriAPIController");

                DisableKaveriViewModel reqModel = caller.GetCall<DisableKaveriViewModel>("DisableKaveriView");
                //
                var gridData = reqModel.disableKaveriViewsList.Select(x => new
                {

                    srNo =x.SrNo,
                    OfficeName= x.OfficeName,
                    IsDisabled= x.IsDisabled,
                    DisableDate= x.DisableDate,
                    Kaveri1Code = x.Kaveri1Code

                });
                //
                var JsonData = Json(new
                {
                    //draw = formCollection["draw"],
                    data = gridData.ToArray().ToList(),
                    recordsTotal = reqModel.disableKaveriViewsList.Count,
                    recordsFiltered = reqModel.disableKaveriViewsList.Count,
                    status = "1",

                


                },JsonRequestBehavior.AllowGet);
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Disable Kaveri View", URLToRedirect = "/Home/HomePage" });
            }
        }
        [HttpPost]
        public ActionResult UpdateDisableKaveriDetails(int[] KaveriCode)
        {
           // bool Result = false;
            //string Message = "";
            try
            {
                DisableKaveriViewModel disableKaveriViewModel = new DisableKaveriViewModel();
                disableKaveriViewModel.KaveriCode = KaveriCode;
                caller = new ServiceCaller("DisableKaveriAPIController");
                UpdateDetailsModel Result = caller.PostCall<DisableKaveriViewModel, UpdateDetailsModel>("UpdateDisableKaveriDetails", disableKaveriViewModel);
                //if(Result)
                //{
                //    Message = "";
                //}
                var JsonData = Json(new
                {
                    status = "1",
                    Message = "The office(s) are disabled successfully.",
                    success = true,
                    serverError = false

                });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch(Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, success = false, Message = "Error occured while Updating Disable Kaveri Details" });
            }
        }
        
    }
}