/*File Header
 * Project Id: KARIGR [ IN-KA-IGR-02-05 ]
 * Project Name: Kaveri Maintainance Portal
 * File Name: ServicePackDetailsController.cs
 * Author :Harshit Gupta
 * Creation Date :17 May 2019
 * Desc : Provides methods for view and model interaction
 * ECR No : 300
*/
#region References
using CustomModels.Models.ServicePackDetails;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using Ionic.Zip;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
#endregion

namespace ECDataUI.Areas.ServicePack.Controllers
{
    [KaveriAuthorization]
    public class ServicePackDetailsController : Controller
    {
        #region User Variables & Objects
        string errorMessage = string.Empty;
        ServiceCaller caller = null;
        #endregion

        #region Service Pack Insert Form
        /// <summary>
        /// Add Service Pack Details Home
        /// </summary>
        /// <returns></returns>
        public ActionResult ServicePackDetailsHome()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.AddServicePackDetails;
                ServicePackViewModel servicePackViewModel = new ServicePackViewModel();
                caller = new ServiceCaller("ServicePackApiController");
                servicePackViewModel.SelectedValueReleaseType = "Select";
                servicePackViewModel.ReleaseTypeDropDown = caller.GetCall<List<SelectListItem>>("GetReleaseTypeList");
                if (servicePackViewModel.ReleaseTypeDropDown == null)
                    throw new Exception("Unable to fetch Release Type details.");
                ViewBag.ReleaseTypeDD = servicePackViewModel.ReleaseTypeDropDown;
                servicePackViewModel.EncryptedID = string.Empty;
                ViewBag.ChangeTypeDD = caller.GetCall<List<SelectListItem>>("GetChangesTypeList");
                return View("AddServicePackDetailsHome", servicePackViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region List Service Pack Details - GET
        /// <summary>
        /// List Service Pack Details Home For SW Support
        /// </summary>
        /// <returns></returns>
        public ActionResult AddServicePackDetailsHome()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ServicePackDetails;
                return View("ListServicePackDetails");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ApproveServicePackList()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ServicePacksDetails;
                return View("ListApproveServicePack");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ReleasedServicePackList()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ReleasedServicePacks;
                return View("ListReleasedServicePack");

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region CDAC Support Role Actions
        #region Deactivate SP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeactivateServicePack(string id)
        {
            #region decryptedParameters
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            encryptedParameters = id.Split('/');
            if (encryptedParameters.Length != 3)
                throw new SecurityException("URL is Tampered");
            ServiceCaller caller = new ServiceCaller("ServicePackApiController");
            string retVal = string.Empty;
            decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            String[] parameters = decryptedParameters["ID"].Split('$');
            Int32 masterID = Convert.ToInt32(parameters[0].ToString());
            #endregion
            try
            {
                if (masterID != 0)
                {
                    retVal = caller.GetCall<string>("DeactivateServicePackDetailsEntry", new { masterID = masterID }, out errorMessage);
                }
                else
                {
                    return Json(new { serverError = false, success = false, message = "X Invalid Input." }, JsonRequestBehavior.AllowGet);
                }
                if (retVal.Equals(string.Empty))
                    return Json(new { serverError = false, success = true, message = "✔ Service Pack Entry deactivated Successfully!" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { serverError = false, success = false, message = retVal }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Added by shubham bhagat on 18-09-2019
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, success = false, message = "Some Error occurred. Please contact helpdesk." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Activate SP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActivateServicePackDetailsEntry(string id)
        {
            #region decryptedParameters
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            encryptedParameters = id.Split('/');
            if (encryptedParameters.Length != 3)
                throw new SecurityException("URL is Tampered");
            ServiceCaller caller = new ServiceCaller("ServicePackApiController");
            string retVal = string.Empty;
            decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            String[] parameters = decryptedParameters["ID"].Split('$');
            Int32 masterID = Convert.ToInt32(parameters[0].ToString());
            #endregion
            try
            {
                if (masterID != 0)
                {
                    retVal = caller.GetCall<string>("ActivateServicePackDetailsEntry", new { masterID = masterID }, out errorMessage);
                }
                else
                {
                    return Json(new { serverError = false, success = false, message = "X Invalid Input." }, JsonRequestBehavior.AllowGet);
                }
                if (retVal.Equals(string.Empty))
                    return Json(new { serverError = false, success = true, message = "✔ Service Pack Entry Activated Successfully!" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { serverError = false, success = false, message = retVal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Added by shubham bhagat on 18-09-2019
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, success = false, message = "Some Error occurred. Please contact helpdesk." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion 

        #region List SP Details & Its Actions
        /// <summary>
        /// Get Service Pack Details List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetServicePackDetailsList(FormCollection formCollection)
        {
            bool IsRequestForApprovalsList = Convert.ToBoolean(formCollection["IsRequestForApprovalsList"]);
            bool IsRequestForReleasedServiceList = Convert.ToBoolean(formCollection["IsRequestForReleasedServicePacksList"]);

            caller = new ServiceCaller("ServicePackApiController");
            try
            {
                #region User Variables and Objects
                var searchValue = Request.Form.GetValues("search[value]");
                String errorMessage = String.Empty;
                ServicePackViewModel obj = new ServicePackViewModel();
                #endregion

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum;
                int skip = startLen;
                //Added and Changed by mayank on 14/09/2021 for Support Exe Release
                int RoleID = KaveriSession.Current.RoleID;

                var result = caller.GetCall<List<ServicePackViewModel>>("GetServicePackDetailsList", new { IsRequestForApprovalsList, IsRequestForReleasedServiceList ,RoleID});

                var gridData = result.Select(ServicePackViewModel => new
                {
                    //Added by shubham bhagat on 18-09-2019 
                    //ReferenceID = ServicePackViewModel.ServicePackDetails.SpID,
                    ReferenceID = ServicePackViewModel.SerialNo,
                    ReleaseType = ServicePackViewModel.SoftwareReleaseType.TypeName,
                    //Added by shubham bhagat on 18-09-2019 
                    //ActiveDeactiveMenu = ServicePackViewModel.ServicePackDetails.IsActive == true ? "<div class='text-center'><span style='cursor:pointer;font-size:16px;color:red;' class='fa fa-ban' title='Click here to deactivate Service Pack' onClick ='DeactivateServicePack(\"" + ServicePackViewModel.EncryptedID + "\")' /><div>" : "<div class='text-center'><span style='cursor:pointer;font-size:16px;color:green;' class='fa fa-toggle-on' title='Click here to Activate Service Pack' onClick ='ActivateServicePack(\"" + ServicePackViewModel.EncryptedID + "\")' /><div>",
                    ActiveDeactiveMenu = ServicePackViewModel.ServicePackDetails.IsActive == true ? "<div class='text-center'><span style='cursor:pointer;font-size:16px;color:green;' class='fa fa-toggle-on' title='Click here to deactivate Service Pack' onClick ='DeactivateServicePack(\"" + ServicePackViewModel.EncryptedID + "\")' /><div>" : "<div class='text-center'><span style='cursor:pointer;font-size:16px;color:red;' class='fa fa-ban' title='Click here to Activate Service Pack' onClick ='ActivateServicePack(\"" + ServicePackViewModel.EncryptedID + "\")' /><div>",
                    EditServicePackIfNotReleased = ServicePackViewModel.ServicePackDetails.IsReleased == false ? "<div class='text-center'><span style='cursor:pointer;' class='fa fa-pencil' title='Click here to update details' onClick ='EditServicePackDetails(\"" + ServicePackViewModel.EncryptedID + "\")' /><div>" : "-",
                    IsTestOrFinal = ServicePackViewModel.ServicePackDetails.IsTestFinal,
                    MajorVersion = ServicePackViewModel.ServicePackDetails.Major,
                    MinorVersion = ServicePackViewModel.ServicePackDetails.Minor,
                    SPDescription = ServicePackViewModel.ServicePackDetails.Description,
                    InstallationProc = ServicePackViewModel.ServicePackDetails.InstallationProcedure,
                    ChangesList = ServicePackViewModel.ChangesTable,
                    ServicePackID = ServicePackViewModel.ServicePackDetails.SpID,
                    EncryptedId = ServicePackViewModel.EncryptedID,

                    //VirtualPath = ServicePackViewModel.ServicePackDetails.IsActive == true ? "<div class='text-center'><span style='cursor:pointer;font-size:16px;color:blue;' class='fa fa-download' title='Click here to downlaod ServicePack' onClick ='DownloadServicePack(\"" + ServicePackViewModel.ServicePackDetails.FileServerVPath + "\")' /><div>" : "<div class='text-center'>-<div>",
                    VirtualPath = ServicePackViewModel.ServicePackDetails.IsActive == true ? "<div class='text-center'><span style='cursor:pointer;font-size:16px;color:blue;' class='fa fa-download' title='Click here to download Service Pack' onClick ='DownloadServicePack(\"" + (ServicePackViewModel.ServicePackDetails.FileServerVPath == "ForseeableRestriction" ? "ForseeableRestriction" : ServicePackViewModel.EncryptedID) + "\")' /><div>" : "<div class='text-center'>-<div>",

                    ServicePackViewModel.ReleaseDetails,
                    ServicePackStatus = ServicePackViewModel.ServicePackDetails.IsReleased == true ? "Release" : "In Process",
                    ServicePackReleaseDateTime = ServicePackViewModel.ServicePackReleaseDate,

                    // Added by 05-10-2019
                    ApproveBTn = ServicePackViewModel.SoftwareReleaseType.TypeName != "Support Release" ? ( ServicePackViewModel.ServicePackDetails.IsReleased  ? "<div class='text-center' style='font-size:100%;color:green;'>Approved Service Pack</div>" :
                    "<div class='text-center'><span style='cursor:pointer;font-size:16px;color:blue;' class='fa fa-check' title='Click here to Approve Service Pack' onClick ='ApproveServicePack(\"" + ServicePackViewModel.EncryptedID + "\")' /></div>"):"-",
					//Added By Tushar on 2 Jan 2023 for Upload DateTime
                    ServicePackAddedDateTime = ServicePackViewModel.ServicePackAddedDateTime,
					//End By Tushar on 2 Jan 2023
                });
                if (result.Count() == 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "No results Found For the Current Input! Please try again"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                var jsonData = new
                {
                    draw = formCollection["draw"],
                    recordsTotal = result.Count(),
                    recordsFiltered = result.Count(),
                    data = gridData.Skip(Convert.ToInt32(formCollection["start"])).Take(Convert.ToInt32(formCollection["length"])),
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Action Method TO Download Service Pack
        /// </summary>
        /// <param name="VirtualPath"></param>
        /// <returns></returns>
        public ActionResult DownloadServicePackFile(string VirtualPath)
        {
            try
            {
                // Decrypt Service pack ID
                Dictionary<String, String> decryptedParameters = null;
                String[] encryptedParameters = null;

                encryptedParameters = VirtualPath.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int servicePackID = Convert.ToInt32(decryptedParameters["ID"].ToString().Trim());

                caller = new ServiceCaller("ServicePackApiController");
                DownloadResponseModel downloadResponseModel = caller.GetCall<DownloadResponseModel>("DownloadServicePackFile", new { servicePackID });
                if (downloadResponseModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file.", URLToRedirect = "/Home/HomePage" });
                }
                else
                {
                    if (!String.IsNullOrEmpty(downloadResponseModel.errorMsg))
                    {
                        // ADDED BY SHUBHAM BHAGAT ON 11-10-2019 
                        //return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = downloadResponseModel.errorMsg, URLToRedirect = "/Home/HomePage" });
                        if (KaveriSession.Current.RoleID == (int)CommonEnum.RoleDetails.CDACSupportTeam)
                        {
                            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = downloadResponseModel.errorMsg, URLToRedirect = "/ServicePack/ServicePackDetails/AddServicePackDetailsHome" });
                        }
                        else if (KaveriSession.Current.RoleID == (int)CommonEnum.RoleDetails.AIGRComp)
                        {
                            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = downloadResponseModel.errorMsg, URLToRedirect = "/ServicePack/ServicePackDetails/ApproveServicePackList" });
                        }
                        else if (KaveriSession.Current.RoleID == (int)CommonEnum.RoleDetails.SystemIntegrator)
                        {
                            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = downloadResponseModel.errorMsg, URLToRedirect = "/ServicePack/ServicePackDetails/ReleasedServicePackList" });
                        }
                        else
                            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = downloadResponseModel.errorMsg, URLToRedirect = "/Home/HomePage" });

                    }
                    else
                    {
                        return File(downloadResponseModel.FileByte, "application/zip", downloadResponseModel.Filename);
                    }
                }

                // web api call which will return byte array and file name
                //string FileName = Path.GetFileName(VirtualPath);
                //byte[] FileArr = null;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file.", URLToRedirect = "/Home/HomePage" });
            }
        }
        #endregion

        #region Update Service Pack Details
        #region GET Details
        /// <summary>
        /// Get Service Pack Details On Edit Call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditServicePackDetails(string id)
        {
            try
            {
                ServicePackViewModel servicePackViewModel = new ServicePackViewModel();
                caller = new ServiceCaller("ServicePackApiController");
                #region decryptedParameters
                Dictionary<string, string> decryptedParameters = null;
                String[] encryptedParameters = null;
                encryptedParameters = id.Split('/');
                if (encryptedParameters == null)
                    throw new SecurityException("URL is Tampered.");
                if (encryptedParameters.Length != 3)
                    throw new SecurityException("URL is Tampered");
                decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                String[] parameters = decryptedParameters["ID"].Split('$');
                int servicePackID = Convert.ToInt16(parameters[0].ToString());
                #endregion
                servicePackViewModel = caller.GetCall<ServicePackViewModel>("GetServicePackDetails", new { servicePackID }, out errorMessage);
                if (servicePackViewModel == null)
                    throw new Exception("Unable to fetch Service Pack details.");
                servicePackViewModel.ReleaseTypeDropDown = caller.GetCall<List<SelectListItem>>("GetReleaseTypeListForEdit", new { servicePackID }, out errorMessage);
                if (servicePackViewModel.ReleaseTypeDropDown == null)
                    throw new Exception("Unable to fetch Release Type details.");
                servicePackViewModel.SelectedValueReleaseType = (from sub in servicePackViewModel.ReleaseTypeDropDown
                                                                 where Convert.ToInt32(sub.Value) == servicePackViewModel.SoftwareReleaseType.TypeID
                                                                 select sub.Text).First();
                if (servicePackViewModel.SelectedValueReleaseType == "EXE/DLL")
                {
                    servicePackViewModel.ReleaseTypeDropDown.Add(new SelectListItem() { Text = "Service Pack", Value = "2" });
                }
                else if (servicePackViewModel.SelectedValueReleaseType == "Service Pack")
                {
                    servicePackViewModel.ReleaseTypeDropDown.Add(new SelectListItem() { Text = "EXE/DLL", Value = "1" });
                }
                ViewBag.ReleaseTypeDD = servicePackViewModel.ReleaseTypeDropDown;
                ViewBag.ChangeTypeDD = caller.GetCall<List<SelectListItem>>("GetChangesTypeList");

                //Added by shubham bhagat on 18-09-2019
                //servicePackViewModel.ModificationTypeUpdateIds = new List<ModificationTypeUpdateIdModel>();
                //ModificationTypeUpdateIdModel modificationTypeUpdateIdModel = null;
                //foreach (var item in servicePackViewModel.servicePackChagesDetails)
                //{
                //    modificationTypeUpdateIdModel = new ModificationTypeUpdateIdModel()
                //    {
                //        Id =item.SpFixedID,
                //        Value =item.ChangeType.ToString()
                //    };
                //    servicePackViewModel.ModificationTypeUpdateIds.Add(modificationTypeUpdateIdModel);
                //}

                servicePackViewModel.FilePath = Path.GetFileName(servicePackViewModel.ServicePackDetails.FileServerFPath);
                return View("EditServicePackDetailsHome", servicePackViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region POST Details
        /// <summary>
        /// Update Service Pack Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateServicePackDetails()
        {
            try
            {
                ServicePackViewModel servicePackViewModelObj = new ServicePackViewModel();
                servicePackViewModelObj.ServicePackDetails = new CustomModels.Models.ServicePackDetails.ServicePackDetail();
                servicePackViewModelObj.ServicePackChangesDetails = new CustomModels.Models.ServicePackDetails.ServicePackChangesDetailsModel();
                servicePackViewModelObj.SoftwareReleaseType = new CustomModels.Models.ServicePackDetails.SoftwareReleaseTypes();
                HttpPostedFileBase file = null;

                //Added by shubham bhagat on 18-09-2019
                bool checkFileValidation = Convert.ToBoolean(Request.Params["IsFileToUpdateID"]);
                servicePackViewModelObj.IsFileToUpdate = checkFileValidation;

                servicePackViewModelObj.ServicePackDetails.SpID = Convert.ToInt32(Request.Params["SpID"]);

                //Added by shubham bhagat on 18-09-2019
                //servicePackViewModelObj.ServicePackDetails.Major = Convert.ToInt32(Request.Params["Major"]);
                //servicePackViewModelObj.ServicePackDetails.Minor = Convert.ToInt32(Request.Params["Minor"]);

                string MajorStr = Request.Params["Major"].ToString();
                string MinorStr = Request.Params["Minor"].ToString();
                Int32 MajorInt32 = 0;
                Int32 MinorInt32 = 0;
                if (!Int32.TryParse(MajorStr, out MajorInt32))
                {
                    return Json(new { success = false, errormsgType = 11, message = "Please Enter valid Major version." });
                }
                if (!Int32.TryParse(MinorStr, out MinorInt32))
                {
                    return Json(new { success = false, errormsgType = 12, message = "Please Enter valid Minor version." });
                }

                // Major version should be between 1-99
                if (MajorInt32 > 99)
                {
                    return Json(new { success = false, errormsgType = 13, message = "Please Enter Major version between 1 and 99." });
                }
                // Minor version should be between 1-9
                if (MinorInt32 > 9)
                {
                    return Json(new { success = false, errormsgType = 14, message = "Please Enter Minor version between 1 and 9." });
                }

                servicePackViewModelObj.ServicePackDetails.Major = MajorInt32;
                servicePackViewModelObj.ServicePackDetails.Minor = MinorInt32;
                //Added by shubham bhagat on 18-09-2019 above

                //servicePackViewModelObj.ServicePackChangesDetails.ChangeType = Convert.ToInt32(Request.Params["ChangesType"]);

                //-----------------------------------------------------------------------------------------------------------
                //Added by shubham bhagat on 18-09-2019
                //Find out and commented on 25-09-2019
                /* IsTestOrFinal value is not updating correctly because directly request param is assigned into
                String variable(i.e IsTestFinal) and the bool variable is by default having false value that
                is updating in database(i.e IsTestOrFinal) */
                //servicePackViewModelObj.ServicePackDetails.IsTestFinal = Request.Params["FinalTestValue"];
                servicePackViewModelObj.ServicePackDetails.IsTestOrFinal = Convert.ToBoolean(Request.Params["FinalTestValue"]);

                //-----------------------------------------------------------------------------------------------------------


                //servicePackViewModelObj.ServicePackDetails.IsActive = Convert.ToBoolean(Request.Params["IsActiveValue"]);
                servicePackViewModelObj.SoftwareReleaseType.TypeID = Convert.ToInt16(Request.Params["ReleaseType"]);
                int GridCount = Convert.ToInt16(Request.Params["GridCount"]);
                if (GridCount == 0)
                    return Json(new { success = false, errormsgType = 6, message = "Changes/Modification Type Details Cannot Be Empty" });
                servicePackViewModelObj.ServicePackDetails.Description = Request.Params["ServicePackDescription"];
                servicePackViewModelObj.ServicePackDetails.InstallationProcedure = Request.Params["InstallationProcedure"];

                // ADDED BY SHUBHAM BHAGAT ON 09-10-2019 BELOW 
                if (String.IsNullOrEmpty(servicePackViewModelObj.ServicePackDetails.Description))
                {
                    return Json(new { success = false, errormsgType = 1, message = "Description is Required." });
                }
                else if (String.IsNullOrEmpty(servicePackViewModelObj.ServicePackDetails.Description.Trim()))
                {
                    return Json(new { success = false, errormsgType = 1, message = "Description is Required." });
                }
                else
                {
                    var regex = @"^[a-zA-Z0-9-/., ]+$";
                    Match m = Regex.Match(servicePackViewModelObj.ServicePackDetails.Description, regex);
                    if (!m.Success)
                    {
                        return Json(new { success = false, errormsgType = 1, message = "Invalid Description." });
                    }
                }
                if (String.IsNullOrEmpty(servicePackViewModelObj.ServicePackDetails.InstallationProcedure))
                {
                    return Json(new { success = false, errormsgType = 1, message = "Installation Procedure is Required." });
                }
                else if (String.IsNullOrEmpty(servicePackViewModelObj.ServicePackDetails.InstallationProcedure.Trim()))
                {
                    return Json(new { success = false, errormsgType = 1, message = "Installation Procedure is Required." });
                }
                else
                {
                    var regex = @"^[a-zA-Z0-9-/., ]+$";
                    Match m = Regex.Match(servicePackViewModelObj.ServicePackDetails.InstallationProcedure, regex);
                    if (!m.Success)
                    {
                        return Json(new { success = false, errormsgType = 1, message = "Invalid Installation Procedure." });
                    }
                }
                // ADDED BY SHUBHAM BHAGAT ON 09-10-2019 ABOVE

                if (string.IsNullOrEmpty(Request.Params["BugsList"]))
                    return Json(new { success = false, errormsgType = 6, message = "Changes/Modification Type Details Cannot Be Empty" });
                Dictionary<string, int> obj = new Dictionary<string, int>();
                JToken contourManifest = JObject.Parse(Request.Params["BugsList"]);
                for (int i = 0; i < GridCount; i++)
                {
                    JToken features = contourManifest.SelectToken(i.ToString().Trim() ?? string.Empty);
                    //if (features[0].ToArray().Count() == 0)
                    //{
                    //    return Json(new { success = false, errormsgType = 10, message = "❌ One of the Values in the Modification Details are empty." });
                    //}
                    var ModificationTypeDesc = features[0].ToString().Trim();
                    // ADDED BY SHUBHAM BHAGAT ON 04-10-2019
                    if (String.IsNullOrEmpty(ModificationTypeDesc))
                    {
                        return Json(new { success = false, errormsgType = 10, message = "❌ One of the Values in the Modification Details are empty." });
                    }
                    else if (ModificationTypeDesc.Length >= 500)
                    {
                        return Json(new { success = false, errormsgType = 6, message = "Changes/Modification Types length must be less than 500 characters." });
                    }
                    obj[Convert.ToString(features[0])] = Convert.ToInt16(features[1]);
                }
                //bool s=obj.ContainsKey(String.Empty);
                if (obj.ContainsKey(String.Empty))
                {
                    return Json(new { success = false, errormsgType = 10, message = "❌ One of the Values in the Modification Details are empty." });
                }
                servicePackViewModelObj.WebGridListValues = obj;
                foreach (string sFileName in Request.Files)
                {
                    file = Request.Files[sFileName];
                }
                //Added by shubham bhagat on 18-09-2019
                //int DblExtensions = file.FileName.Split('.').Length - 1;
                int DblExtensions = 0;
                if (file != null)
                    DblExtensions = file.FileName.Split('.').Length - 1;

                ServiceCaller caller = new ServiceCaller("ServicePackApiController");

                #region Server Side Validation

                //Check Whether Major and Minor Already exist in conjunction
                //Changed by Omkar on 17-09-2020 
                bool spMajorMinorAlreadyExists = caller.GetCall<bool>("CheckIfServicePackVersionAlreadyExists", new { majorVersion = servicePackViewModelObj.ServicePackDetails.Major, minorVersion = servicePackViewModelObj.ServicePackDetails.Minor, releaseType = servicePackViewModelObj.ReleaseTypeDropDown }, out errorMessage);
                string versionAlreadyExistsMsg = string.Format("Service Pack Version With Major: {0},Minor: {1} already exists.Please check version details.", servicePackViewModelObj.ServicePackDetails.Major, servicePackViewModelObj.ServicePackDetails.Minor);
                if (spMajorMinorAlreadyExists)
                    return Json(new { success = false, errormsgType = 9, message = versionAlreadyExistsMsg });

                #region Service Pack File
                //Added by shubham bhagat on 18-09-2019
                if (file != null)
                {
                    if (file.ContentLength < 0)
                        return Json(new { success = false, errormsgType = 1, message = "Please Upload Service Pack." });

                    if (!IsValidExtension(Path.GetExtension(file.FileName.ToLower())))
                    {
                        return Json(new { success = false, errormsgType = 2, message = "Please upload zip file only. Error in file name - " + file.FileName + " Kindly upload file again." });
                    }
                    else if (!IsValidContentLength(file.ContentLength))
                    {
                        // COMMENTED AND CHAGED BY OMKAR ON 12-08-2020 
                        return Json(new { success = false, errormsgType = 3, message = "Each File size should be less than 150 MB. Kindly upload file again." });
                        //return Json(new { success = false, errormsgType = 3, message = "Each File size should be less than 20 MB. Kindly upload file again." });
                    }
                    else if (DblExtensions > 1)
                    {
                        return Json(new { success = false, errormsgType = 4, message = "Cannot Upload Files because it is either not a supported File type or because the file has been damaged(for example,it was sent as an attachment and wasn't correctly decoded)!!" });
                    }
                }
                #endregion

                if (servicePackViewModelObj.ServicePackDetails.Major < 1 || servicePackViewModelObj.ServicePackDetails.Minor < 0)
                    return Json(new { success = false, errormsgType = 5, message = "Invalid Version Input" });


                #endregion

                //Added by shubham bhagat on 18-09-2019
                if (file != null)
                {
                    servicePackViewModelObj.IsFileUploadedSuccessfully = true;
                    //string rootPath = ConfigurationManager.AppSettings["ServicePackDocPath"];
                    //if (!Directory.Exists(rootPath))
                    //    Directory.CreateDirectory(rootPath);
                    //string systemGeneratedFileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", "").Replace("/", "") + Path.GetExtension(file.FileName); //Date Followed With Time  
                    //servicePackViewModelObj.ServicePackDetails.FileServerFPath = rootPath + "\\" + systemGeneratedFileName;
                    //string absolutePath = servicePackViewModelObj.ServicePackDetails.FileServerFPath.Substring(rootPath.Length).Replace('\\', '/').Insert(0, "~/ServicePacks/");
                    //servicePackViewModelObj.ServicePackDetails.FileServerVPath = absolutePath;
                    ////servicePackViewModelObj.ServicePackDetails.ServicePackUploadedBy = KaveriSession.Current.UserID;
                    byte[] fileData = null;

                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(file.ContentLength);
                    }
                    servicePackViewModelObj.FileData = fileData;
                    servicePackViewModelObj.FileExtension = Path.GetExtension(file.FileName);

                    //using (FileStream stream = new FileStream(servicePackViewModelObj.ServicePackDetails.FileServerFPath, FileMode.Create))
                    //{
                    //    stream.Write(fileData, 0, fileData.Length);
                    //    stream.Close();
                    //}
                }
                //Added by shubham bhagat on 18-09-2019
                servicePackViewModelObj.ServicePackDetails.ServicePackUploadedBy = KaveriSession.Current.UserID;

                string retVal = caller.PostCall<ServicePackViewModel, string>("UpdateServicePackDetails", servicePackViewModelObj, out errorMessage);
                if (string.IsNullOrEmpty(retVal))
                    return Json(new { success = true, serverError = false, message = "✔ Service Pack Details Updated Successfully!" });
                else
                    return Json(new { success = false, serverError = false, errormsgType = 8, message = "❌ Error While Updating Service Pack Details" });
            }
            catch (Exception ex)
            {
                //Added by shubham bhagat on 18-09-2019
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, serverError = true, message = "Some Error occurred. Please contact helpdesk." });
            }
        }
        #endregion

        #region Delete Modification Details From Table
        /// <summary>
        /// Delete Change Details on Edit View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ServicePackChangesDetailsEntry(string id)
        {
            ServiceCaller caller = new ServiceCaller("ServicePackApiController");
            string retVal = string.Empty;
            #region decryptedParameters
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            encryptedParameters = id.Split('/');
            if (encryptedParameters.Length != 3)
                throw new SecurityException("URL is Tampered");
            decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            String[] parameters = decryptedParameters["ID"].Split('$');
            Int32 masterID = Convert.ToInt32(parameters[0].ToString());
            #endregion
            try
            {
                if (masterID != 0)
                {
                    retVal = caller.GetCall<string>("DeleteServicePackDetailsEntry", new { masterID = masterID }, out errorMessage);
                }
                else
                {
                    return Json(new { serverError = false, success = false, message = "X Invalid Input." }, JsonRequestBehavior.AllowGet);
                }
                if (retVal.Equals(string.Empty))
                    return Json(new { serverError = false, success = true, message = "✔ Service Pack Changes Details Entry deleted Successfully!" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { serverError = false, success = false, message = retVal }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Added by shubham bhagat on 18-09-2019
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, success = false, message = "Some Error occurred. Please contact helpdesk." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #endregion

        #region Insert Service Pack Details
        /// <summary>
        /// Add Service Pack Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveServicePackDetails()
        {
            try
            {
                #region Model Interceptions
                ServicePackViewModel servicePackViewModelObj = new ServicePackViewModel();
                servicePackViewModelObj.ServicePackDetails = new CustomModels.Models.ServicePackDetails.ServicePackDetail();
                servicePackViewModelObj.ServicePackChangesDetails = new CustomModels.Models.ServicePackDetails.ServicePackChangesDetailsModel();
                servicePackViewModelObj.SoftwareReleaseType = new CustomModels.Models.ServicePackDetails.SoftwareReleaseTypes();
                HttpPostedFileBase file = null;

                //Added by shubham bhagat on 18-09-2019
                // Added validation
                //servicePackViewModelObj.ServicePackDetails.Major = Convert.ToInt32(Request.Params["Major"]);
                //servicePackViewModelObj.ServicePackDetails.Minor = Convert.ToInt32(Request.Params["Minor"]);

                string MajorStr = Request.Params["Major"].ToString();
                string MinorStr = Request.Params["Minor"].ToString();
                Int32 MajorInt32 = 0;
                Int32 MinorInt32 = 0;
                if (!Int32.TryParse(MajorStr, out MajorInt32))
                {
                    return Json(new { success = false, errormsgType = 10, message = "Please Enter valid Major version." });
                }
                if (!Int32.TryParse(MinorStr, out MinorInt32))
                {
                    return Json(new { success = false, errormsgType = 11, message = "Please Enter valid Minor version." });
                }

                // Major version should be between 1-99
                if (MajorInt32 > 99)
                {
                    return Json(new { success = false, errormsgType = 12, message = "Please Enter Major version between 1 and 99." });
                }
                // Minor version should be between 1-9
                if (MinorInt32 > 9)
                {
                    return Json(new { success = false, errormsgType = 13, message = "Please Enter Minor version between 1 and 9." });
                }

                servicePackViewModelObj.ServicePackDetails.Major = MajorInt32;
                servicePackViewModelObj.ServicePackDetails.Minor = MinorInt32;

                //Added by shubham bhagat on 18-09-2019 above
                // Changed below code by Shubham Bhagat on 10-10-2019
                //servicePackViewModelObj.ServicePackChangesDetails.ChangeType = Request.Params["ChangesType"] == null ? 0 : Convert.ToInt32(Request.Params["ChangesType"]);
                servicePackViewModelObj.ServicePackChangesDetails.ChangeType = String.IsNullOrEmpty(Request.Params["ChangesType"]) ? 0 : Convert.ToInt32(Request.Params["ChangesType"]);

                servicePackViewModelObj.ServicePackDetails.IsTestOrFinal = Convert.ToBoolean(Request.Params["FinalTestValue"]);
                //servicePackViewModelObj.ServicePackDetails.IsActive = Convert.ToBoolean(Request.Params["IsActiveValue"]);
                servicePackViewModelObj.SoftwareReleaseType.TypeID = Convert.ToInt16(Request.Params["ReleaseType"]);
                servicePackViewModelObj.ServicePackDetails.Description = Request.Params["ServicePackDescription"];
                servicePackViewModelObj.ServicePackDetails.InstallationProcedure = Request.Params["InstallationProcedure"];
                servicePackViewModelObj.BugsListValues = Request.Params["BugsList"];
                servicePackViewModelObj.EnhancementListValues = Request.Params["EnhancementList"];
                servicePackViewModelObj.SupportAnalysisListValues = Request.Params["SupportAnalysisList"];

                // ADDED BY SHUBHAM BHAGAT ON 09-10-2019 BELOW 
                if (String.IsNullOrEmpty(servicePackViewModelObj.ServicePackDetails.Description))
                {
                    return Json(new { success = false, errormsgType = 1, message = "Description is Required." });
                }
                else if (String.IsNullOrEmpty(servicePackViewModelObj.ServicePackDetails.Description.Trim()))
                {
                    return Json(new { success = false, errormsgType = 1, message = "Description is Required." });
                }
                //Commneted by mayank on 06/12/2021
                //else
                //{
                //    var regex = @"^[a-zA-Z0-9-/., ]+$";
                //    Match m = Regex.Match(servicePackViewModelObj.ServicePackDetails.Description, regex);
                //    if (!m.Success)
                //    {
                //        return Json(new { success = false, errormsgType = 1, message = "Invalid Description." });
                //    }
                //}
                if (String.IsNullOrEmpty(servicePackViewModelObj.ServicePackDetails.InstallationProcedure))
                {
                    return Json(new { success = false, errormsgType = 1, message = "Installation Procedure is Required." });
                }
                else if (String.IsNullOrEmpty(servicePackViewModelObj.ServicePackDetails.InstallationProcedure.Trim()))
                {
                    return Json(new { success = false, errormsgType = 1, message = "Installation Procedure is Required." });
                }
                //Commneted by mayank on 06/12/2021
                //else
                //{
                //    var regex = @"^[a-zA-Z0-9-/., ]+$";
                //    Match m = Regex.Match(servicePackViewModelObj.ServicePackDetails.InstallationProcedure, regex);
                //    if (!m.Success)
                //    {
                //        return Json(new { success = false, errormsgType = 1, message = "Invalid Installation Procedure." });
                //    }
                //}
                // ADDED BY SHUBHAM BHAGAT ON 09-10-2019 ABOVE

                #endregion
                foreach (string sFileName in Request.Files)
                {
                    file = Request.Files[sFileName];
                }
                int DblExtensions = file.FileName.Split('.').Length - 1;
                ServiceCaller caller = new ServiceCaller("ServicePackApiController");

                #region Server Side Validation
                //Check Whether Major and Minor Already exist in conjunction
                //Changed by Omkar on 17-09-2020 
                //Added and Changed by mayank on 14/09/2021 for Support Exe Release
                //bool spMajorMinorAlreadyExists = caller.GetCall<bool>("CheckIfServicePackVersionAlreadyExists", new { majorVersion = servicePackViewModelObj.ServicePackDetails.Major, minorVersion = servicePackViewModelObj.ServicePackDetails.Minor, releaseType = servicePackViewModelObj.ServicePackDetails.IsTestOrFinal }, out errorMessage);
                //string versionAlreadyExistsMsg = string.Format("Service Pack Version With Major: {0},Minor: {1} already exists.Please check version details.", servicePackViewModelObj.ServicePackDetails.Major, servicePackViewModelObj.ServicePackDetails.Minor);
                //if (spMajorMinorAlreadyExists)
                //    return Json(new { success = false, errormsgType = 9, message = versionAlreadyExistsMsg });
                //Added and Changed by mayank on 14/09/2021 for Support Exe Release removed version check after discussion with shaila ma'am and aparna ma'am
                ////if (servicePackViewModelObj.SoftwareReleaseType.TypeID!=3)
                ////{
                ////    bool spMajorMinorAlreadyExists = caller.GetCall<bool>("CheckIfServicePackVersionAlreadyExists", new { majorVersion = servicePackViewModelObj.ServicePackDetails.Major, minorVersion = servicePackViewModelObj.ServicePackDetails.Minor, releaseType = servicePackViewModelObj.ServicePackDetails.IsTestOrFinal }, out errorMessage);
                ////    string versionAlreadyExistsMsg = string.Format("Service Pack Version With Major: {0},Minor: {1} already exists.Please check version details.", servicePackViewModelObj.ServicePackDetails.Major, servicePackViewModelObj.ServicePackDetails.Minor);
                ////    if (spMajorMinorAlreadyExists)
                ////        return Json(new { success = false, errormsgType = 9, message = versionAlreadyExistsMsg }); 
                ////}
                //END
                #region Service Pack File Validations

                if (file.ContentLength < 0)
                    return Json(new { success = false, errormsgType = 1, message = "Please Upload Service Pack." });

                if (!IsValidExtension(Path.GetExtension(file.FileName.ToLower())))
                {
                    return Json(new { success = false, errormsgType = 2, message = "Please upload zip file only. Error in file name -" + file.FileName + " Kindly upload file again." });
                }
                else if (!IsValidContentLength(file.ContentLength))
                {
                    // COMMENTED AND CHAGED BY OMKAR ON 12-08-2020 
                    return Json(new { success = false, errormsgType = 3, message = "Each File size should be less than 150 MB. Kindly upload file again." });
                    //return Json(new { success = false, errormsgType = 3, message = "Each File size should be less than 20 MB. Kindly upload file again." });
                }
                //Commneted by mayank on 06/12/2021
                //else if (DblExtensions > 1)
                //{
                //    return Json(new { success = false, errormsgType = 4, message = "Cannot Upload Files because it is either not a supported File type or because the file has been damaged(for example,it was sent as an attachment and wasn't correctly decoded)!!" });
                //}
                #endregion

                if (servicePackViewModelObj.ServicePackDetails.Major < 1 || servicePackViewModelObj.ServicePackDetails.Minor < 0)
                    return Json(new { success = false, errormsgType = 5, message = "Invalid Version Input" });

                // ADDED BY SHUBHAM BHAGAT ON 04-10-2019 Below and commented
                //remove the "|" and check 
                //if (string.IsNullOrEmpty(servicePackViewModelObj.BugsListValues.Replace("|", "")) && string.IsNullOrEmpty(servicePackViewModelObj.EnhancementListValues.Replace("|", "")))
                //if (string.IsNullOrEmpty(servicePackViewModelObj.BugsListValues.Replace("|", "").Trim()) || string.IsNullOrEmpty(servicePackViewModelObj.EnhancementListValues.Replace("|", "").Trim()))

                String[] BugsListValuesArray = String.IsNullOrEmpty(servicePackViewModelObj.BugsListValues) ? null : servicePackViewModelObj.BugsListValues.Split('|');
                String[] EnhancementListValuesArray = String.IsNullOrEmpty(servicePackViewModelObj.EnhancementListValues) ? null : servicePackViewModelObj.EnhancementListValues.Split('|');
                String[] SupportAnalysisListValuesArray = String.IsNullOrEmpty(servicePackViewModelObj.SupportAnalysisListValues) ? null : servicePackViewModelObj.SupportAnalysisListValues.Split('|');

                if (servicePackViewModelObj.ServicePackChangesDetails.ChangeType == 0)
                {
                    if (BugsListValuesArray == null && EnhancementListValuesArray == null && SupportAnalysisListValuesArray == null)
                    {
                        return Json(new { success = false, errormsgType = 6, message = "Please add Change/Modification Types." });
                    }
                }
                else
                {
                    if (BugsListValuesArray == null && EnhancementListValuesArray == null && SupportAnalysisListValuesArray == null)
                    {
                        return Json(new { success = false, errormsgType = 6, message = "Please add Change/Modification Types." });
                    }
                }

                if (BugsListValuesArray != null)
                {
                    foreach (var item in BugsListValuesArray)
                    {
                        if (String.IsNullOrEmpty(item.Trim()))
                            return Json(new { success = false, errormsgType = 6, message = "Changes/Modification Types are not mentioned" });
                        //Added by shubham bhagat on 07-10-2019
                        else if (item.Length >= 500)
                        {
                            return Json(new { success = false, errormsgType = 6, message = "Changes/Modification Types length must be less than 500 characters." });
                        }
                    }
                }

                if (EnhancementListValuesArray != null)
                {
                    foreach (var item in EnhancementListValuesArray)
                    {
                        if (String.IsNullOrEmpty(item.Trim()))
                            return Json(new { success = false, errormsgType = 6, message = "Changes/Modification Types are not mentioned" });
                        //Added by shubham bhagat on 07-10-2019
                        else if (item.Length >= 500)
                        {
                            return Json(new { success = false, errormsgType = 6, message = "Changes/Modification Types length must be less than 500 characters." });
                        }
                    }
                }

                if (SupportAnalysisListValuesArray != null)
                {
                    foreach (var item in SupportAnalysisListValuesArray)
                    {
                        if (String.IsNullOrEmpty(item.Trim()))
                            return Json(new { success = false, errormsgType = 6, message = "Changes/Modification Types are not mentioned" });
                        //Added by shubham bhagat on 07-10-2019
                        else if (item.Length >= 500)
                        {
                            return Json(new { success = false, errormsgType = 6, message = "Changes/Modification Types length must be less than 500 characters." });
                        }
                    }
                }
                // ADDED BY SHUBHAM BHAGAT ON 04-10-2019 Above
                #endregion

                //string rootPath = ConfigurationManager.AppSettings["ServicePackDocPath"];
                //if (!Directory.Exists(rootPath))
                //    Directory.CreateDirectory(rootPath);

                //Create FileName 
                //string systemGeneratedFileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", "").Replace("/", "") + Path.GetExtension(file.FileName); //Date Followed With Time  
                //servicePackViewModelObj.ServicePackDetails.FileServerFPath = rootPath + "\\" + systemGeneratedFileName;
                //string absolutePath = servicePackViewModelObj.ServicePackDetails.FileServerFPath.Substring(rootPath.Length).Replace('\\', '/').Insert(0, "~/ServicePacks/");
                //servicePackViewModelObj.ServicePackDetails.FileServerVPath = absolutePath;
                servicePackViewModelObj.ServicePackDetails.ServicePackUploadedBy = KaveriSession.Current.UserID;

                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileData = binaryReader.ReadBytes(file.ContentLength);
                }
                servicePackViewModelObj.FileData = fileData;
                servicePackViewModelObj.FileExtension = Path.GetExtension(file.FileName);

                //using (FileStream stream = new FileStream(servicePackViewModelObj.ServicePackDetails.FileServerFPath, FileMode.Create))
                //{
                //    stream.Write(fileData, 0, fileData.Length);
                //    stream.Close();
                //}

                string retVal = caller.PostCall<ServicePackViewModel, string>("AddServicePackDetails", servicePackViewModelObj, out errorMessage);
                if (string.IsNullOrEmpty(retVal))
                    return Json(new { success = true, serverError = false, message = "✔ Service Pack Details Added Successfully!" });
                else
                    return Json(new { success = false, serverError = false, errormsgType = 8, message = "❌ Error While Adding Service Pack Details" });
            }
            catch (Exception ex)
            {
                //Added by shubham bhagat on 18-09-2019
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, serverError = true, message = "Some Error occurred. Please contact helpdesk." });
            }
        }

        private bool IsValidExtension(string extension)
        {
            return (extension.Equals(".zip") || extension.Equals(".Zip"));
        }

        private bool IsValidContentLength(int ContentLength)
        {
            // COMMENTED AND CHAGED BY OMKAR ON 12-08-2020 
            //return (ContentLength < 41943040);  //40 Mb
            //Added by mayank on 06-12-2021
            return (ContentLength < 157286400);  //40 Mb
            //return (ContentLength < 20971520);  //20 Mb
        }
        #endregion
        #endregion

        #region AIGR_COMP Role Actions
        /// <summary>
        /// View That Returns Service Pack Details In Modal For AIGR_COMP
        /// </summary>
        /// <returns></returns>
        public ActionResult GetServicePackDetailsForApprovalAndRelease()
        {
            string spIDEncrypted = Convert.ToString(Request.Params["EncryptedID"]);
            #region decryptedParameters
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            encryptedParameters = spIDEncrypted.Split('/');
            if (encryptedParameters.Length != 3)
                throw new SecurityException("URL is Tampered");
            ServiceCaller caller = new ServiceCaller("ServicePackApiController");

            decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            String[] parameters = decryptedParameters["ID"].Split('$');
            Int32 servicePackID = Convert.ToInt32(parameters[0].ToString());
            #endregion
            ServicePackViewModel servicePackViewModel = new ServicePackViewModel();
            try
            {
                servicePackViewModel = caller.GetCall<ServicePackViewModel>("GetServicePackDetails", new { servicePackID }, out errorMessage);
                return PartialView("ReleaseNotesDetails", servicePackViewModel);
            }
            catch (Exception ex)
            {
                //Added by shubham bhagat on 18-09-2019
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = "Some Error occurred. Please contact helpdesk." }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Update Service Pack Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReleaseNotes()
        {
            try
            {

                string spIDEncrypted = Convert.ToString(Request.Params["SpID"]);

                #region decryptedParameters
                Dictionary<string, string> decryptedParameters = null;
                String[] encryptedParameters = null;
                encryptedParameters = spIDEncrypted.Split('/');
                if (encryptedParameters.Length != 3)
                    throw new SecurityException("URL is Tampered");
                ServiceCaller caller = new ServiceCaller("ServicePackApiController");

                decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                String[] parameters = decryptedParameters["ID"].Split('$');
                Int32 servicePackID = Convert.ToInt32(parameters[0].ToString());
                #endregion
                //Validate Date Input 
                if (string.IsNullOrEmpty(Convert.ToString(Request.Params["ReleaseDate"])))
                {
                    return Json(new { success = false, serverError = false, errorType = 1, message = "❌ Release Date Is Required" });
                }

                ReleaseDetails releaseDetailsModelObj = new ReleaseDetails
                {
                    ReleaseNotes = Convert.ToString(Request.Params["ReleaseNotes"]).Trim(' '),
                    ReleaseDateTime = Convert.ToDateTime(Request.Params["ReleaseDate"]),
                    ServicePackID = servicePackID,
                    ServicePackReleasedBy = KaveriSession.Current.UserID
                };

                // ADDED BY SHUBHAM BHAGAT ON 07-10-2019
                if (!String.IsNullOrEmpty(releaseDetailsModelObj.ReleaseNotes))
                {
                    if (releaseDetailsModelObj.ReleaseNotes.Length >= 500)
                    {
                        return Json(new { success = false, serverError = false, errorType = 2, message = "❌ Release notes length must be less than 500 characters." });
                    }

                    var regex = @"^[a-zA-Z0-9-/., ]+$";
                    Match m = Regex.Match(releaseDetailsModelObj.ReleaseNotes, regex);
                    if (!m.Success)
                    {
                        return Json(new { success = false, serverError = false, errorType = 2, message = "❌ Invalid Release Notes." });
                    }
                }

                string retVal = caller.PostCall<ReleaseDetails, string>("SaveReleaseNotesDetails", releaseDetailsModelObj, out errorMessage);
                if (string.IsNullOrEmpty(retVal))
                    return Json(new { success = true, serverError = false, message = "✔ Service Pack Released Successfully!" });
                else
                    return Json(new { success = false, serverError = false, message = "❌ Something went wrong while releasing service pack.Please contact helpdesk." });
            }
            catch (Exception ex)
            {
                //Added by shubham bhagat on 18-09-2019
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, serverError = true, message = "Some Error occurred. Please contact helpdesk." });
            }
        }
        #endregion

        [HttpGet]
        public ActionResult CheckIfFileExists(string VirtualPath)
        {
            try
            {
                // Decrypt Service pack ID
                Dictionary<String, String> decryptedParameters = null;
                String[] encryptedParameters = null;

                encryptedParameters = VirtualPath.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int servicePackID = Convert.ToInt32(decryptedParameters["ID"].ToString().Trim());

                caller = new ServiceCaller("ServicePackApiController");
                DownloadResponseModel downloadResponseModel = caller.GetCall<DownloadResponseModel>("CheckIfFileExists", new { servicePackID });
                if (downloadResponseModel == null)
                {
                    return Json(new { success = false, message = "Error occured while downloading file." }, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file.", URLToRedirect = "/Home/HomePage" });
                }
                else
                {
                    bool IsFileExist = String.IsNullOrEmpty(downloadResponseModel.errorMsg) ?    true: false;
                    return Json(new { success = IsFileExist, message = downloadResponseModel.errorMsg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { success = false, message = "Error occured while downloading file." }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file.", URLToRedirect = "/Home/HomePage" });
            }
        }


    }
}