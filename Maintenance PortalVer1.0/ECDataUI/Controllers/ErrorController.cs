using CustomModels.Models.Common;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// Any General Unhandled Error Page
        /// </summary>
        /// <returns></returns>
       
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Page Not Found Error View
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound() 
        {
            return View();
        }

        /// <summary>
        /// Show Session Expire Message
        /// </summary>
        /// <returns></returns>
        public ActionResult SessionExpire()
        {
            bool requestHeader = Request.IsAjaxRequest();
            Session.Abandon();
            KaveriSession.Current.EndSession();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //added by soujanya 05-10-2017 and commented cookies.add
            Response.Cookies.Clear();
            Response.Cookies["KaveriSessionID"].Expires = DateTime.Now.AddYears(-30);
            // Response.Cookies.Add(new HttpCookie("KaveriSessionID", ""));
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.Clear();
            // Added by Tushar on 19 April 2022 for DeleteExpiredSessions
            Response.StatusCode = 401;
            ViewData["IsAJAXRequest"] = requestHeader.ToString();
            return View();
        }

        public ActionResult ScanError()
        {
            return View();
        }

        /// <summary>
        /// Method for checking valid roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UnAuthorized()
        {
            return View();
        }


        //public ActionResult ShowErrorMessage(ErrorPageModel errorModel)
        //{

        //    return View(errorModel);
        //}

        public ActionResult ShowErrorMessage(string message, string URLToRedirect)
        {
            //string path = Request.UrlReferrer.ToString();

            ViewBag.Message = message;
            ViewBag.URLToRedirect = URLToRedirect;

            return View();
        }



        public ActionResult ShowErrorMessageWithoutBack(string message, string URLToRedirect)
        {
            //string path = Request.UrlReferrer.ToString();

            ViewBag.Message = message;
            ViewBag.URLToRedirect = URLToRedirect;

            return View();
        }

        /// <summary>
        /// If Seesion Expired, method is called which returns message as Session Expired.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SessionExpired()
        {
            
           
            return View();
        }


        
    }
}
