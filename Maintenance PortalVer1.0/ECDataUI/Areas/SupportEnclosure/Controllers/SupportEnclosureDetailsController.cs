#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsBAL.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   UI Controller for Support Enclosure
*/
#endregion

using CustomModels.Models.SupportEnclosure;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.SupportEnclosure.Controllers
{
    [KaveriAuthorizationAttribute]
    public class SupportEnclosureDetailsController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        /// <summary>
        /// SupportEnclosureDetails View Load
        /// </summary>
        /// <returns></returns>
        public ActionResult SupportEnclosureDetails()
        {
            try
            {

                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DownloadEnclosure;
                caller = new ServiceCaller("SupportEnclosureDetailsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                SupportEnclosureDetailsViewModel reqModel = caller.GetCall<SupportEnclosureDetailsViewModel>("SupportEnclosureDetails", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Support Enclosure Details View", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Support Enclosure Details View", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// Get SRO Office List By District ID
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <returns>returns SRO Office list</returns>
        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Load SupportEnclosure for Document By Document Number, Book Type and Financial Year
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Load SupportEnclosure for Document By Document Number, Book Type and Financial Year")]
        public ActionResult LoadSupportDocumentEnclosureTableData(FormCollection formCollection)
        {
            try
            {

                #region User Variables and Objects
                string SROOfficeID = formCollection["SROOfficeID"];
                string DROfficeID = formCollection["DROfficeID"];
                long DocNumber;
                if (string.IsNullOrEmpty(formCollection["DocumentNumber"].ToString()))
                {
                    DocNumber = Convert.ToInt64(0);
                }
                else
                {
                    var regex = "^[0-9]*$";
                    var matchDocumentNo = Regex.Match(formCollection["DocumentNumber"].ToString(), regex, RegexOptions.IgnoreCase);
                    if (!matchDocumentNo.Success)
                        DocNumber = Convert.ToInt64(0);
                    else
                        DocNumber = Convert.ToInt64(formCollection["DocumentNumber"]);
                }
                string BookType = formCollection["BookTypeID"];
                string FinancialYear = formCollection["FinancialYear"];

                //long DocNumber = Convert.ToInt64(DocumentNumber);

                int SroId = Convert.ToInt32(SROOfficeID);
                int DroId = Convert.ToInt32(DROfficeID);

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = "Invalid To Date"
                });

                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = String.Empty;
                #endregion

                #region Server Side Validation              


                caller = new ServiceCaller("CommonsApiController");
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

                //Validation For DR Login
                if (LevelID == Convert.ToInt16(CommonEnum.LevelDetails.DR))
                {
                    //Validation for DR when user do not select any sro which is by default "Select"
                    if ((SroId == 0))
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any SRO"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }
                else
                {//Validations of Logins other than SR and DR

                    if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any District"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                    else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any SRO"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                    else if (DocNumber == 0)//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please Select Module to Procced"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                    else if (BookType == "0")//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please Select Book Type to Procced"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                }

                if (string.IsNullOrEmpty(FinancialYear) || FinancialYear.Equals("Select"))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "FinancialYear required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                #endregion




                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                SupportEnclosureDetailsViewModel reqModel = new SupportEnclosureDetailsViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.DocumentNumber = DocNumber;
                reqModel.SROfficeID = SroId;
                reqModel.BookTypeID = Convert.ToInt32(BookType);
                reqModel.FinancialYearStr = FinancialYear;
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);

                caller = new ServiceCaller("SupportEnclosureDetailsAPIController");
                int totalCount = caller.PostCall<SupportEnclosureDetailsViewModel, int>("GetSupportDocumentEnclosureTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //To get records of indexII report table 
                SupportEnclosureDetailsResModel resModel = caller.PostCall<SupportEnclosureDetailsViewModel, SupportEnclosureDetailsResModel>("GetSupportDocumentEnclosureTableData", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Support Enclosure Details" });
                }
                IEnumerable<SupportEnclosureDetailsModel> result = resModel.SupportEnclosureDetailsList;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }


                //Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                //}
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
                        result = result.Where(m => m.DocumentNo.ToLower().Contains(searchValue.ToLower()) ||
                        m.DocumentNo.ToLower().Contains(searchValue.ToLower()) ||
                        m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROOffice.ToString().Contains(searchValue) ||
                        m.UploadDateTime.ToString().ToLower().Contains(searchValue.ToLower()));
                    }
                }

                var gridData = result.Select(SupportEnclosureDetailsModel => new
                {
                    SrNo = SupportEnclosureDetailsModel.SerialNo,
                    OfficeName = SupportEnclosureDetailsModel.SROOffice,
                    SupportEnclosureDetailsModel.DocumentID,
                    DocumentNumber = SupportEnclosureDetailsModel.DocumentNo == "0" ? "NA" : SupportEnclosureDetailsModel.DocumentNo,
                    SupportEnclosureDetailsModel.FinalRegistrationNumber,
                    SupportEnclosureDetailsModel.SupportDocumentTypeID,
                    SupportEnclosureDetailsModel.SupportDocumentType,
                    SupportEnclosureDetailsModel.UploadDateTime,
                    SupportEnclosureDetailsModel.FileName,
                    DownloadEnclosureButton = "<button type ='button' class='btn btn-group-md btn-primary' onclick=DownLoadEnclosureFile('" + EncryptedFilePathDetails(SupportEnclosureDetailsModel.FilePath) + "','" + EncryptedFilePathDetails(SupportEnclosureDetailsModel.FileName) + "')>Download file</button>"
                });


                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount
                    });

                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                    });
                }
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Support Enclosure Details" });
            }
        }


        /// <summary>
        /// Load SupportEnclosure for Party By Document Number, Book Type and Financial Year
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Load SupportEnclosure for Party By Document Number, Book Type and Financial Year")]
        public ActionResult LoadSupportPartyEnclosureTableData(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects
                string SROOfficeID = formCollection["SROOfficeID"];
                string DROfficeID = formCollection["DROfficeID"];
                long DocNumber;
                if (string.IsNullOrEmpty(formCollection["DocumentNumber"].ToString()))
                {
                    DocNumber = Convert.ToInt64(0);
                }
                else
                {
                    var regex = @"^[0-9]*$";
                    var matchDocumentNo = Regex.Match(formCollection["DocumentNumber"].ToString(), regex, RegexOptions.IgnoreCase);
                    if (!matchDocumentNo.Success)
                        DocNumber = Convert.ToInt64(0);
                    else
                        DocNumber = Convert.ToInt64(formCollection["DocumentNumber"]);
                }
                string BookType = formCollection["BookTypeID"];
                string FinancialYear = formCollection["FinancialYear"];


                int SroId = Convert.ToInt32(SROOfficeID);
                int DroId = Convert.ToInt32(DROfficeID);

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = "Invalid To Date"
                });

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                #region Server Side Validation              


                caller = new ServiceCaller("CommonsApiController");
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

                //Validation For DR Login
                if (LevelID == Convert.ToInt16(CommonEnum.LevelDetails.DR))
                {
                    //Validation for DR when user do not select any sro which is by default "Select"
                    if ((SroId == 0))
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any SRO"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }
                else
                {//Validations of Logins other than SR and DR

                    if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any District"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                    else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any SRO"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                    else if (DocNumber == 0)//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please Select Module to Procced"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                    else if (BookType == "0")//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please Select Book Type to Procced"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                }

                if (string.IsNullOrEmpty(FinancialYear) || FinancialYear.Equals("Select"))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "FinancialYear required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                #endregion




                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                SupportEnclosureDetailsViewModel reqModel = new SupportEnclosureDetailsViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.DocumentNumber = DocNumber;
                reqModel.SROfficeID = SroId;
                reqModel.BookTypeID = Convert.ToInt32(BookType);
                reqModel.FinancialYearStr = FinancialYear;
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);

                caller = new ServiceCaller("SupportEnclosureDetailsAPIController");
                int totalCount = caller.PostCall<SupportEnclosureDetailsViewModel, int>("GetSupportPartyEnclosureTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //To get records of indexII report table 
                SupportEnclosureDetailsResModel resModel = caller.PostCall<SupportEnclosureDetailsViewModel, SupportEnclosureDetailsResModel>("GetSupportPartyEnclosureTableData", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Support Enclosure Details" });
                }
                IEnumerable<SupportEnclosureDetailsModel> result = resModel.SupportEnclosureDetailsList;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }


                //Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                //}
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
                        result = result.Where(m => m.DocumentNo.ToLower().Contains(searchValue.ToLower()) ||
                        m.DocumentNo.ToLower().Contains(searchValue.ToLower()) ||
                        m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROOffice.ToString().Contains(searchValue) ||
                        m.PartyName.ToString().Contains(searchValue) ||
                        m.UploadDateTime.ToString().ToLower().Contains(searchValue.ToLower()));

                    }
                }

                var gridData = result.Select(SupportEnclosureModel => new
                {
                    SrNo = SupportEnclosureModel.SerialNo,
                    OfficeName = SupportEnclosureModel.SROOffice,
                    SupportEnclosureModel.PartyID,
                    SupportEnclosureModel.PartyName,
                    SupportEnclosureModel.DocumentID,
                    DocumentNumber = SupportEnclosureModel.DocumentNo == "0" ? "NA" : SupportEnclosureModel.DocumentNo,
                    SupportEnclosureModel.FinalRegistrationNumber,
                    SupportEnclosureModel.SupportDocumentTypeID,
                    SupportEnclosureModel.SupportDocumentType,
                    SupportEnclosureModel.UploadDateTime,
                    SupportEnclosureModel.FileName,
                    DownloadEnclosureButton = "<button type ='button' class='btn btn-group-md btn-primary' onclick=DownLoadEnclosureFile('" + EncryptedFilePathDetails(SupportEnclosureModel.FilePath) + "','" + EncryptedFilePathDetails(SupportEnclosureModel.FileName) + "')>Download file</button>"
                });


                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount
                    });

                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                    });
                }
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Support Enclosure Details" });
            }
        }

        /// <summary>
        /// Encrypted File Path Details
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string EncryptedFilePathDetails(string filePath)
        {
            string encryptedfilePath = string.Empty;
            try
            {
                encryptedfilePath = URLEncrypt.EncryptParameters(new string[] { "EnclosureFilePath=" + filePath });
                encryptedfilePath = HttpUtility.UrlEncode(encryptedfilePath);
                return encryptedfilePath;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// DownLoad Selected Enclosure File
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public ActionResult DownLoadEnclosureFile(string Path, string FileName)
        {
            try
            {
                string errorMessage = String.Empty;
                string enclosureFilePath = string.Empty;
                string fileName = string.Empty;
                #region Decrypting FilePath
                Dictionary<string, string> decryptedParameters = null;
                string[] tempAppId = Path.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { tempAppId[0], tempAppId[1], tempAppId[2] });
                enclosureFilePath = Convert.ToString(decryptedParameters["EnclosureFilePath"]);
                if (string.IsNullOrEmpty(enclosureFilePath))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting FilePath for downloading Enclosure.", URLToRedirect = "/Home/HomePage" });
                }
                #endregion

                decryptedParameters = null;
                tempAppId = FileName.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { tempAppId[0], tempAppId[1], tempAppId[2] });
                fileName = Convert.ToString(decryptedParameters["EnclosureFilePath"]);
                if (string.IsNullOrEmpty(fileName))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting FilePath for downloading Enclosure.", URLToRedirect = "/Home/HomePage" });
                }

                SupportEnclosureDetailsModel model = new SupportEnclosureDetailsModel();
                model.FilePath = enclosureFilePath;
                model.FileName = fileName;


                caller = new ServiceCaller("SupportEnclosureDetailsAPIController");
                SupportEnclosureDetailsResModel resModel = caller.PostCall<SupportEnclosureDetailsModel, SupportEnclosureDetailsResModel>("GetSupportDocumentEnclosure", model, out errorMessage);

                if (resModel == null || !string.IsNullOrEmpty(errorMessage))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file from Server.", URLToRedirect = "/Home/HomePage" });
                }
                else if (resModel.IsError)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = resModel.ErrorMessage, URLToRedirect = "/Home/HomePage" });
                }
                else
                {
                    HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    return File(resModel.EnclosureFileContent, "text/plain");
                }
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file.", URLToRedirect = "/Home/HomePage" });
            }
        }

    }
}