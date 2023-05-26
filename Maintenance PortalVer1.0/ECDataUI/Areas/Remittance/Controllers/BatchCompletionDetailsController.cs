using CustomModels.Models.Remittance.BatchCompletionDetails;
using ECDataUI.Common;
using ECDataUI.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

//added by vijay on 16/02/2023
namespace ECDataUI.Areas.Remittance.Controllers
{

    [KaveriAuthorization]

    public class BatchCompletionDetailsController : Controller

    {

        ServiceCaller caller = null;
        [HttpGet]
        [MenuHighlight]
        public ActionResult BatchCompletionDetailsView()
        {
            try
            {
                caller = new ServiceCaller("BatchCompletionDetailsAPIController");
                BatchCompletionDetailsReportModel reqModel = caller.GetCall<BatchCompletionDetailsReportModel>("BatchCompletionDetailsView");

                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Summary Report Details View", URLToRedirect = "/Home/HomePage" });
            }
        }
        [HttpPost]


        public ActionResult GetBatchCompletionDetails(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("BatchCompletionDetailsAPIController");

                BatchCompletionDetailsResultModel resultModel = new BatchCompletionDetailsResultModel();
                string errorMessage = string.Empty;

                BatchCompletionDetailsReportModel reportModel = new BatchCompletionDetailsReportModel();
                
                //reportModel.TillDate = formCollection["FromDate"];

                var DocType = formCollection["DocType"];
                //var TableId = formCollection["TableId"];
                reportModel.DocType = Convert.ToInt32(DocType);

                resultModel = caller.PostCall<BatchCompletionDetailsReportModel, BatchCompletionDetailsResultModel>("GetBatchCompletionDetails", reportModel, out errorMessage);
                IEnumerable<BatchCompletionDetailsReportTableModel> result = resultModel.BatchCompletionDetailsReportTableList;
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var IsRecordFilter = false;
                int TotalCount = result.Count();
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);

                int pageSize = totalNum;
                int skip = startLen;
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Batch Completion  Details." });
                }
                
                /*
                if (string.IsNullOrEmpty(reportModel.TillDate))
                {
                    return Json(new { success = false, errorMessage = "please enter Date" });
                }
                */

                if (reportModel.DocType == -1)
                {
                    return Json(new { success = false, errorMessage = "Please Select Document Type.." });
                }

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
                        //bool isDigitPresent = searchValue.All(c => char.IsDigit(c));

                        searchValue = searchValue.Trim();
                        {
                            //result = result.Where(m => (m.SROCode == int.Parse(searchValue)) || (m.SroName.ToLower().Contains(searchValue.ToLower())));
                            
                            //Updated by Rushikesh on 27 Feb 2023
                            result = result.Where(obj => (obj.SroName.ToLower().Contains(searchValue.ToLower()) || obj.SROCode.ToString().Equals(searchValue)));
                            //End by Rushikesh on 27 Feb 2023
                        }

                        TotalCount = result.Count();
                        IsRecordFilter = true;
                    }

                }

                var gridData = result.Select(batchCompletionDetailsReportTableModel => new
                {
                    //Added by Rushikesh on 27 Feb 2023
                    srNo = batchCompletionDetailsReportTableModel.srNo,
                    //End by Rushikesh on 27 Feb 2023

                    SROCode = batchCompletionDetailsReportTableModel.SROCode,
                    SroName = batchCompletionDetailsReportTableModel.SroName,

                    //Added by Rushikesh on 27 Feb 2023
                    L_Stamp5DateTime = batchCompletionDetailsReportTableModel.L_Stamp5DateTime,

                    BatchDateTime=batchCompletionDetailsReportTableModel.Batchdatetime,
                    Isverified=batchCompletionDetailsReportTableModel.Is_verified,



                    //End by Rushikesh on 27 Feb 2023

                    /*
                    RPT_DocReg_NoCLBatchDetails_MaxToDocID = batchCompletionDetailsReportTableModel.MaxToDocID,
                    documentmaster_MaxDocID = batchCompletionDetailsReportTableModel.MaxDocID,
                    DataStatus = batchCompletionDetailsReportTableModel.IsBatchComplete,
                    MaxBatchStamp5Datetime = batchCompletionDetailsReportTableModel.RPT_DocReg_NoCLBatchDetails_MaxToDocID_Stamp5DateTime,
                    MaxDMStamp5Datetime = batchCompletionDetailsReportTableModel.documentmaster_MaxDocID_Stamp5DateTime,
                    

                    MaxMRDateofReg = batchCompletionDetailsReportTableModel.MarriageRegistration_MaxRegID_DateOfReg,
                    MaxRegID = batchCompletionDetailsReportTableModel.MaxRegistrationID,
                    */
                });
                ;
                ;


                //  String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:150%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + reportModel.TableId + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = "",
                    //  ExcelDownloadBtn = ExcelDownloadBtn,
                    recordsFiltered = 0,
                    recordsTotal = 0,

                    status = "0",
                    IsRecordFilter = IsRecordFilter,


                });
                if (searchValue != null && searchValue != "")
                {

                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalNum,
                        recordsFiltered = TotalCount,
                        status = "1",
                        //  ExcelDownloadBtn = ExcelDownloadBtn,
                        IsRecordFilter = IsRecordFilter,
                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalNum,
                        recordsFiltered = TotalCount,
                        status = "1",
                        //     ExcelDownloadBtn = ExcelDownloadBtn,
                        IsRecordFilter = IsRecordFilter,
                    });
                }

                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Batch Completion Details View", URLToRedirect = "/Home/HomePage" });

            }

        }

    }
}