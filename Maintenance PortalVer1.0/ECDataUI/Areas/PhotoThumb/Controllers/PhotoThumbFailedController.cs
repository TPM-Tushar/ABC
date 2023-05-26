using Aspose.Words;
using CustomModels.Models.PhotoThumb;
using ECDataUI.Common;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.PhotoThumb.Controllers
{
    public class PhotoThumbFailedController : Controller
    {
        private ServiceCaller caller;

        public ActionResult PhotoThumbFailedView()
        {
            try
            {
                caller = new ServiceCaller("PhotoThumbFailedAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                PhotoThumbFailedViewModel reqModel = caller.GetCall<PhotoThumbFailedViewModel>("PhotoThumbFailedView", new { OfficeID = OfficeID });
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
        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                string errormessage = string.Empty;
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
                return Json(new { SROfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult PhotoThumbFailed(FormCollection formCollection)
        {
            PhotoThumbFailedTableModel ResModel = new PhotoThumbFailedTableModel();
            try
            {
                long UserId = KaveriSession.Current.UserID;
                string errorMessage = string.Empty;
                caller = new ServiceCaller("PhotoThumbFailedAPIController");
                PhotoThumbReqModel ReqModel = new PhotoThumbReqModel();
                ReqModel.SROCode = Convert.ToInt32(formCollection["SROfficeID"]);


                ResModel = caller.GetCall<PhotoThumbFailedTableModel>("PhotoThumbFailed", new { SROCode = Convert.ToInt32(formCollection["SROfficeID"]) });

                if (ResModel.IsError == true || ResModel == null)
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
                        errorMessage = ResModel.ExError
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }




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
                        ResModel.FailedList = ResModel.FailedList.Where(m => m.PartyName.ToString().ToLower().Contains(searchValue.ToLower()) || (m.IsPhoto.ToString().ToLower().Contains("true") && (searchValue.ToLower().Contains("photo") || searchValue.ToLower().Contains("phot") || searchValue.ToLower().Contains("pho"))) || (m.IsThumb.ToString().ToLower().Contains("true") && (searchValue.ToLower().Contains("thumb") || searchValue.ToLower().Contains("thu") || searchValue.ToLower().Contains("thum")))).ToList();
                    }
                }

                Random rnd = new Random();
                int num = rnd.Next();

                string p_strPath = string.Format("{0}/{1}", Server.MapPath("~/Content/TempPhotoThumbFiles"), "ExcelReport" + num.ToString() + ".xlsx");

                var gridData = ResModel.FailedList.Select(PhotoThumbFailedListModel => new
                {
                    SNo = PhotoThumbFailedListModel.SNo,
                    FRN = PhotoThumbFailedListModel.FRN,
                    PartyName = PhotoThumbFailedListModel.PartyName,
                    Type = PhotoThumbFailedListModel.IsPhoto == true ? "Photo" : "Thumb",
                    Action = "<button class='btn btn-success' name='btnShow' id='btnShow' onclick='Detail(" + PhotoThumbFailedListModel.PartyID + "," + PhotoThumbFailedListModel.SROCode + ",\"" + PhotoThumbFailedListModel.IsPhoto + "\",\"" + PhotoThumbFailedListModel.IsThumb + "\")' style='padding: 1rem;'>Details</button>",
                    FileName = "ExcelReport" + num.ToString()
                });

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum;
                int skip = startLen;
                int totalCount = ResModel.FailedList.Count;
                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                    recordsTotal = totalCount,
                    status = "1",
                    recordsFiltered = totalCount
                });


                ExcelPackage excel = new ExcelPackage();

                // name of the sheet
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                // setting the properties
                // of the work sheet 
                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 20;

                // Setting the properties
                // of the first row
                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;

                // Header of the Excel sheet
                workSheet.Cells[1, 1].Value = "S.No";
                workSheet.Cells[1, 2].Value = "Final Registration Number";
                workSheet.Cells[1, 3].Value = "Party Name";
                workSheet.Cells[1, 4].Value = "Type";
                workSheet.Cells[1, 5].Value = "Date";
                workSheet.Cells[1, 6].Value = "Error Message";
                

                // Inserting the article data into excel
                // sheet by using the for each loop
                // As we have values to the first row 
                // we will start with second row
                int recordIndex = 2;

                foreach (PhotoThumbFailedListModel FL in ResModel.NonDistinctFailedList)
                {
                    workSheet.Cells[recordIndex, 1].Value = FL.SNo;
                    workSheet.Cells[recordIndex, 2].Value = FL.FRN;
                    workSheet.Cells[recordIndex, 3].Value = FL.PartyName;
                    workSheet.Cells[recordIndex, 4].Value = FL.IsPhoto == true ? "Photo" : "Thumb";
                    workSheet.Cells[recordIndex, 5].Value = Convert.ToDateTime(FL.Date).ToString("dd-MM-yyyy HH:mm:ss");
                    workSheet.Cells[recordIndex, 6].Value = FL.ErrorMessage;
                    workSheet.Cells[recordIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                    recordIndex++;
                }

                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();

                if (!Directory.Exists(Server.MapPath("~/Content/TempPhotoThumbFiles")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/TempPhotoThumbFiles"));
                }

                if (System.IO.File.Exists(p_strPath))
                    System.IO.File.Delete(p_strPath);

                // Create excel file on physical disk 
                FileStream objFileStrm = System.IO.File.Create(p_strPath);
                objFileStrm.Close();

                // Write content to excel file 
                System.IO.File.WriteAllBytes(p_strPath, excel.GetAsByteArray());
                //Close Excel package
                excel.Dispose();


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
                    errorMessage = ResModel.ExError + ". Check Exception Log."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }
        }



        public ActionResult PhotoThumbFailedDetail(FormCollection formCollection)
        {
            PhotoThumbFailedTableModel ResModel = new PhotoThumbFailedTableModel();
            try
            {
                long UserId = KaveriSession.Current.UserID;
                string errorMessage = string.Empty;
                caller = new ServiceCaller("PhotoThumbFailedAPIController");
                PhotoThumbFailedReqModel ReqModel = new PhotoThumbFailedReqModel();
                ReqModel.SROCode = Convert.ToInt32(formCollection["SROCode"]);
                ReqModel.PartyID = Convert.ToInt32(formCollection["PartyID"]);
                ReqModel.IsPhoto = Convert.ToBoolean(formCollection["IsPhoto"]);
                ReqModel.IsThumb = Convert.ToBoolean(formCollection["IsThumb"]);


                ResModel = caller.GetCall<PhotoThumbFailedTableModel>("PhotoThumbFailedDetail", new { PartyID = ReqModel.PartyID, SROCode = ReqModel.SROCode, IsPhoto = ReqModel.IsPhoto, IsThumb = ReqModel.IsThumb });

                if (ResModel.IsError == true || ResModel == null)
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
                        errorMessage = ResModel.ExError
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }




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
                        ResModel.FailedList = ResModel.FailedList.Where(m => m.Date.ToString().ToLower().Contains(searchValue.ToLower())).ToList();
                    }
                }



                var gridData = ResModel.FailedList.Select(PhotoThumbFailedListModel => new
                {
                    SNo = PhotoThumbFailedListModel.SNo,
                    CDNumber = PhotoThumbFailedListModel.CDNumber,
                    Date = Convert.ToDateTime(PhotoThumbFailedListModel.Date).ToLongDateString(),
                    Error = PhotoThumbFailedListModel.ErrorMessage,
                });

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum;
                int skip = startLen;
                int totalCount = ResModel.FailedList.Count;
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
                    errorMessage = ResModel.ExError + ". Check Exception Log."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }
        }
    }
}