using CustomModels.Models.MISReports.ARegister;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorization]
    public class ARegisterController : Controller
    {
        ServiceCaller caller = new ServiceCaller("ARegisterAPIController");
        // GET: MISReports/ARegister
        [MenuHighlight]
        public ActionResult ARegisterView()
        {
            ARegisterViewModel viewModel = new ARegisterViewModel();
            viewModel.SROfficeList = new List<SelectListItem>();
            viewModel.ForDate = string.Empty;
            viewModel.UserID = KaveriSession.Current.UserID;
            viewModel.RoleID = KaveriSession.Current.RoleID;
            viewModel.LevelID = KaveriSession.Current.LevelID;
            viewModel.OfficeID = KaveriSession.Current.OfficeID;
            if (KaveriSession.Current.LevelID == Convert.ToInt16(CommonEnum.LevelDetails.SR))
            {
                ViewBag.IsSRLogin = true;
            }
            else
            {
                ViewBag.IsSRLogin = false;
            }
            ARegisterViewModel responseViewModel = caller.PostCall<ARegisterViewModel, ARegisterViewModel>("ARegisterView", viewModel);
            return View(responseViewModel);
        }

        [EventAuditLogFilter(Description = "Generate A Register Report")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult GenerateReport(FormCollection formCollection)
        {

            try
            {
                List<DateTime> SkipDateTimeList = new List<DateTime>();
                ARegisterViewModel viewModel = new ARegisterViewModel();

                viewModel.UserID = KaveriSession.Current.UserID;
                viewModel.RoleID = KaveriSession.Current.RoleID;
                viewModel.LevelID = KaveriSession.Current.LevelID;
                viewModel.OfficeID = KaveriSession.Current.OfficeID;

                viewModel.SROfficeID = Convert.ToInt32(formCollection["SROOfficeID"]);
                viewModel.ForDate = Convert.ToString(formCollection["ForDate"]);

                if (viewModel.SROfficeID == 0)
                {
                    return Json(new { success = false, message = "Please select SRO Code" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(viewModel.ForDate))
                {
                    return Json(new { success = false, message = "Please select Date" }, JsonRequestBehavior.AllowGet);
                }
                try
                {
                    viewModel.ForDate_DateTime = Convert.ToDateTime(viewModel.ForDate);
                    if (viewModel.ForDate_DateTime == DateTime.MinValue)
                        return Json(new { success = false, message = "Please select valid Date" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error Occured while converting date,Please contact admin" }, JsonRequestBehavior.AllowGet);
                }

                if (viewModel.ForDate_DateTime == DateTime.Today)
                {
                    if (DateTime.Now.Hour <= 18)
                    {
                        if (DateTime.Now.Minute < 30)
                        {
                            TimeSpan ts = new TimeSpan(18, 30,00);

                            return Json(new { success = false, message = "A Register report for today's date will be avaliable after 8:30 PM only,Please try again after sometime." }, JsonRequestBehavior.AllowGet);

                        }
                    }
                    //    #region Condition commented by mayank on 03/01/2022 uncomment before deployment
                    //    //TimeSpan ts = new TimeSpan(17, 30, 00);
                    //    ////if (viewModel.ForDate_DateTime.TimeOfDay < ts)
                    //    //if (DateTime.Now.TimeOfDay < ts)
                    //    //    //return Json(new { success = false, message = "A Register report for today's date will be avaliable after 5:30 PM only,Please try again after 5:30 PM." }, JsonRequestBehavior.AllowGet);
                    //    //    return Json(new { success = false, message = "A Register report for today's date will be generated after 5:30 PM only,Please try again after 5:30 PM." }, JsonRequestBehavior.AllowGet); 
                    //    #endregion

                    //}
                    //string SkipDateStr = (ConfigurationManager.AppSettings["SkipWeekendForAregister"]);
                    //if (!string.IsNullOrEmpty(SkipDateStr))
                    //{
                    //    List<string> SkipDateLst = SkipDateStr.Split(',').ToList();
                    //    foreach (string item in SkipDateLst)
                    //    {
                    //        string[] DateStr = item.Split('$');
                    //        if (DateStr.Length==3)
                    //        {
                    //            SkipDateTimeList.Add(new DateTime(Convert.ToInt32(DateStr[2]), Convert.ToInt32(DateStr[1]), Convert.ToInt32(DateStr[0]))); 
                    //        }
                    //    }
                    //}
                    ////if (SkipDateTimeList.Count > 0)
                    ////{
                    //if (!(SkipDateTimeList.Where(m => (m.Day == viewModel.ForDate_DateTime.Day &&
                    //                         m.Month == viewModel.ForDate_DateTime.Month &&
                    //                         m.Year == viewModel.ForDate_DateTime.Year)).Any()))
                    //{

                    //    if (viewModel.ForDate_DateTime.DayOfWeek == DayOfWeek.Sunday)
                    //    {
                    //        return Json(new { success = false, message = "2nd/4th saturday and sunday no report will be generated." }, JsonRequestBehavior.AllowGet);
                    //    }
                    //    if (viewModel.ForDate_DateTime.DayOfWeek == DayOfWeek.Saturday)
                    //    {

                    //        List<DateTime> DateTimeList = GetSecondFourthSaturday(viewModel.ForDate_DateTime);
                    //        if (DateTimeList.Where(m => (m.Day == viewModel.ForDate_DateTime.Day &&
                    //                                m.Month == viewModel.ForDate_DateTime.Month &&
                    //                                m.Year == viewModel.ForDate_DateTime.Year)).Any())
                    //        {
                    //            return Json(new { success = false, message = "2nd/4th saturday and sunday no report will be generated." }, JsonRequestBehavior.AllowGet);
                    //        }
                    //    }
                }
                ARegisterResultModel aRegisterResultModel = caller.PostCall<ARegisterViewModel, ARegisterResultModel>("GenerateReport", viewModel);

                return Json(new { success = aRegisterResultModel.ResponseStatus, message = aRegisterResultModel.ResponseMessage, messageGeneration = aRegisterResultModel.MessageforGeneration, URL = aRegisterResultModel.Url }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error Occured please contact admin", URL = string.Empty }, JsonRequestBehavior.AllowGet);

                throw;
            }
        }

        [HttpGet]
        public ActionResult ViewARegisterReport(string EncryptedFileID)
        {
            try
            {
                string[] encryptedParameters = EncryptedFileID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");
                Dictionary<String, String> decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                long FileID = Convert.ToInt64(decryptedParameters["FileID"].ToString().Trim());
                ARegisterResultModel resultmodel = caller.GetCall<ARegisterResultModel>("ViewARegisterReport", new { FileId = FileID });

                return File(resultmodel.AregisterFileBytes, "application/pdf");
            }
            catch (Exception)
            {
                const string input = "Due to some techinical problem this document cannot be viewed right now.Please try again later.";

                // Invoke GetBytes method.
                byte[] array = Encoding.ASCII.GetBytes(input);

                return File(array, "application/pdf");
                throw;
            }

        }

        private List<DateTime> GetSecondFourthSaturday(DateTime dateTime)
        {
            int month = dateTime.Month;
            int year = dateTime.Year;
            int firstday = 1;
            List<DateTime> SecondFourthSaturday = new List<DateTime>();
            DateTime FirstDate = new DateTime(year, month, firstday);
            SecondFourthSaturday.Add(new DateTime(year, month, (14 - ((int)FirstDate.DayOfWeek))));
            SecondFourthSaturday.Add(new DateTime(year, month, (28 - ((int)FirstDate.DayOfWeek))));
            return SecondFourthSaturday;
        }
    }
}