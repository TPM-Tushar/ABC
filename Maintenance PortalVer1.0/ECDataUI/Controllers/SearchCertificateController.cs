

#region File Header
/*
    * Project Id        :   -
    * Project Name      :   GAURI
    * File Name         :   SearchCertificateController.cs
    * Author Name       :   Mayur Jive
    * Creation Date     :   07-10-2018
    * Last Modified By  :   -
    * Last Modified On  :   28-11-2018
    * Description       :   Controller for Search Certificate/License.
*/
#endregion


using GauriUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Mvc;
using CustomModels.Models.SearchCertificate;
using CaptchaLib;
using System.Web;
using GauriUI.Filters;

namespace GauriUI.Controllers
{
    public class SearchCertificateController : Controller
    {
        string certificateNum;
        string errorMessage = string.Empty;
        // GET: Certificate
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Action method for Load Search Certificate View
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchCertificate()
        {

            ServiceCaller caller = new ServiceCaller("SearchCertificateApiController");
            String EncryptedID = "ABC";

            SearchCertificateModel response = caller.GetCall<SearchCertificateModel>("SearchCertificate", new { EncryptedID = EncryptedID }, out errorMessage);
            return View(response);
        }

        /// <summary>
        /// Post call for getting Certificate Data
        /// </summary>
        /// <param name="ObjCertificate"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult SearchCertificate(SearchCertificateModel ObjCertificate)
        {
            certificateNum = ObjCertificate.CertificateNumber;
            if (ModelState.IsValid)
            {
                CertificateModel ObjModel = new CertificateModel();

                ObjModel.CertificateNumber = ObjCertificate.CertificateNumber;
                ObjModel.ServiceID = ObjCertificate.ServiceID;
                ObjModel.DepartmentID = ObjCertificate.DepartmentID;

                try
                {

                    ServiceCaller caller = new ServiceCaller("SearchCertificateApiController");
                    SearchCertificateModel response = caller.PostCall<CertificateModel, SearchCertificateModel>("GetCertificateDetails", ObjModel, out errorMessage);

                    if (response.errorMessage == null)
                    {
                        return View("GetCertificate", response);
                    }
                    else
                    {
                        string errorMsg = response.errorMessage;
                        return Json(new { success = false, responseMsg = errorMsg, errorMessage = errorMessage });
                    }
                }
                catch (Exception )
                {
                    throw ;
                }
            }
            else
            {
                string errorMsg = ModelState.FormatErrorMessageInString();
                return Json(new { success = false, responseMsg = errorMsg, errorMessage = errorMessage });
            }
        }

        public ActionResult GetCertificate()
        {
            return View();
        }


        [HttpPost]
        public ActionResult LoadCertificate(FormCollection formCollection)
        {
            CertificateModel model = null;
            String sSROoffice = string.Empty;

            try
            {
                String[] parameters = formCollection["SearchParameters"].Split('&');
                Dictionary<String, String> paramValues = new Dictionary<String, String>();
                string Message = string.Empty;
                //int value;
                model = new CertificateModel();
                string[] splitArr = null;
                foreach (var item in parameters)
                {
                    splitArr = item.Split('=');
                    if (!paramValues.ContainsKey(splitArr[0]))

                        paramValues.Add(splitArr[0], splitArr[1]);

                }
                string certificateNum = HttpUtility.UrlDecode(paramValues["CertificateNumber"]);
                string serviceID = HttpUtility.UrlDecode(paramValues["ServiceID"]);
                string deptID = HttpUtility.UrlDecode(paramValues["DepartmentID"]);

                model.CertificateNumber = certificateNum;
                model.ServiceID = Convert.ToInt16(serviceID);
                model.DepartmentID = Convert.ToInt16(deptID);

                ServiceCaller caller = new ServiceCaller("SearchCertificateApiController");
                SearchCertificateModel response = caller.PostCall<CertificateModel, SearchCertificateModel>("GetCertificateDetails", model, out errorMessage);

                List<SearchCertificateModel> ObjCertList = new List<SearchCertificateModel>();
                ObjCertList.Add(response);
                Int32 totalCount = ObjCertList.Count;

                if (totalCount == 0)
                {

                    var jsonData = new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        errorMessage = "Certificate details not found , Please try Again"
                    };
                    return Json(jsonData);
                }
                else
                {
                    var gridData = ObjCertList.Select(SearchCertificate => new
                    {
                        SearchCertificate.ApplicationDate,
                        SearchCertificate.NameOfEstablishment,
                        SearchCertificate.TypeOfEstablishment,
                        SearchCertificate.CertificateName,
                        SearchCertificate.IssuedOn,
                        SearchCertificate.ValidUpto,
                        SearchCertificate.AddressString,
                    }).ToArray();
                    var jsonData = new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = totalCount,
                        recordsFiltered = totalCount,
                        data = gridData.Skip(System.Convert.ToInt32(formCollection["start"])).Take(System.Convert.ToInt32(formCollection["length"])),
                    };

                    return Json(jsonData);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        public ActionResult GetCaptchaImage()
        {
            //return CaptchaLib.ControllerExtensions.Captcha(this,new MyCaptchaImage(),100,150,60);
            return ControllerExtensions.Captcha(this, new CaptchaImage(), 50, 280, 50);
        }

        /// <summary>
        /// View for displaying PDF document in div
        /// </summary>
        /// <param name="ObjCertificate"></param>
        /// <returns></returns>
        //public ActionResult DisplayCertificateView(SearchCertificateModel ObjCertificate)
        //{
        //    // SearchCertificateModel ObjCertificate = new SearchCertificateModel();

        //    return View("DisplayCertificateView", ObjCertificate);
        //}
        /// <summary>
        /// Getting Certificate PDF from DAL
        /// </summary>
        /// <param name = "CertificateNum" ></ param >
        /// < returns ></ returns >
        //[HttpGet]
        //public ActionResult GetCertificateByteArray(string CertificateNum)
        //{
        //    CertificateModel ObjCertificate = new CertificateModel();
        //    ObjCertificate.CertificateNumber = CertificateNum;
        //    try
        //    {
        //        ServiceCaller caller = new ServiceCaller("SearchCertificateApiController");
        //        SearchCertificateModel objFileDisplayModel = caller.PostCall<CertificateModel, SearchCertificateModel>("GetCertificateByteArray", ObjCertificate);

        //        //if (objFileDisplayModel.errorMessage != null)
        //        //{
        //        //    return View("DisplayCertificateView", ObjCertificate);
        //        //}

        //        //else
        //        //{
        //            Response.AddHeader("Content-Disposition", "inline; filename=" + "FirmCertificateFRM_" + objFileDisplayModel.FirmID + ".pdf");
        //            return File(objFileDisplayModel.fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf);
        //       // }

        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
    }
}