using CustomModels.Models.Remittance.ChallanDataEntryCorrectionDetails;
using CustomModels.Models.Remittance.MasterData;
using ECDataUI.Common;
using ECDataUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Linq;
using System.Text.RegularExpressions;
using Aspose.Words.Tables;
using CustomModels.Models.MISReports.PropertyWthoutImportBypassRDPR;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorization]

    //added by vijay on 01/02/23
    public class ChallanDataEntryCorrectionDetailsController:Controller
    {
        ServiceCaller caller = null;


        [HttpGet]
        [MenuHighlight]

        public ActionResult ChallanDataEntryCorrectionDetailsView()
        {
            try
            {

                caller = new ServiceCaller("ChallanDataEntryCorrectionDetailsAPIController");
                    ChallanDataEntryCorrectionDetailsReportModel reportModel = new ChallanDataEntryCorrectionDetailsReportModel();

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                reportModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                reportModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                reportModel = caller.GetCall<ChallanDataEntryCorrectionDetailsReportModel>("ChallanDataEntryCorrectionDetailsView");

                return View(reportModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Summary Report Details View", URLToRedirect = "/Home/HomePage" });
            }



        }


        [HttpPost]

        public ActionResult GetCDECorrectionDetailsData(FormCollection formCollection)

        {
            caller = new ServiceCaller("ChallanDataEntryCorrectionDetailsAPIController");

            ChallanDataEntryCorrectionDetailsResultModel resultModel = new ChallanDataEntryCorrectionDetailsResultModel();
            string errorMessage = string.Empty;

            ChallanDataEntryCorrectionDetailsReportModel reportModel = new ChallanDataEntryCorrectionDetailsReportModel();
            var SROId = formCollection["SRO"];
            reportModel.SROId = Convert.ToInt32(SROId);



            var LocallyUpdated = formCollection["IsLocalluUpdated"];
            var CentrallyllyUpdated = formCollection["IsCentrallyUpdated"];

            if(LocallyUpdated!= null)
            {
                reportModel.LocallyUpdated =1;

            }
            if (CentrallyllyUpdated != null)
            {
                reportModel.CentrallyUpdated = 1;
            }
            /*
            if (LocallyUpdated ==null && CentrallyllyUpdated==null)
            {

                var json = new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    status = false,
                    data = "",
                    errorMessage = "please select atleast one checkbox"
                };
                return Json(json, JsonRequestBehavior.AllowGet);


            }
            */

            string FromDate = formCollection["FromDate"];

            string ToDate = formCollection["ToDate"];

            
            if (FromDate == "" || ToDate=="")
            {
                return Json(new { success = false, errorMessage = "Please Select Date.." });
            }


            reportModel.DateTime_FromDate = Convert.ToDateTime(FromDate);
            reportModel.DateTime_ToDate = Convert.ToDateTime(ToDate);

           

            resultModel = caller.PostCall<ChallanDataEntryCorrectionDetailsReportModel, ChallanDataEntryCorrectionDetailsResultModel>("GetCDECorrectionDetailsData", reportModel, out errorMessage);

            IEnumerable<ChallanDataEntryCorrectionDetailsReportTableModel> result = resultModel.CDECDDataTableList;

            


           var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
            Match mtch = regx.Match((string)searchValue);
            var IsRecordFilter = false;
            int TotalCount = result.Count();
            int startLen = Convert.ToInt32(formCollection["start"]);
            int totalNum = Convert.ToInt32(formCollection["length"]);


     

            // var TableId = formCollection["TableId"];
            //reportModel.TableId = Convert.ToInt32(TableId);

  

            int pageSize = totalNum;
            int skip = startLen;

            if (result == null)
            {
                return Json(new { success = false, errorMessage = "Error occured while getting Summary Report Details." });
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
                    bool isDigitPresent = searchValue.All(c => char.IsDigit(c));

                    searchValue = searchValue.Trim();
                    result = result.Where(m => (m.ChallanNumber.Contains(searchValue)) || m.Old_BIND_InstrumentNumber.Contains(searchValue));




                    TotalCount = result.Count();
                    IsRecordFilter = true;
                }
            }
            
              var gridData = result.Select(model => new
            {
               // SROCode = model.SROCode,

                SRONAME=model.SroName,
                ApplicationDate =model.ApplicationDate,
                OldChallanNumber =model.Old_BIND_InstrumentNumber,
                OldChallanDate=model.Old_BIND_InstrumentDate,
                NewChallanNumber=model.ChallanNumber,
                NewChallanDate= model.ChallanDate,
                Reason=model.Reason,
                IsLocallyUpdated=model.IsLocallyUpdated,
                IsCentrallyUpdated=model.IsCentrallyupdated,










            });;
            ;
            ;


            // String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:150%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + reportModel.TableId + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

            var JsonData = Json(new
            {
                draw = formCollection["draw"],
                data = "",
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
                    IsRecordFilter = IsRecordFilter,
                });
            }

            JsonData.MaxJsonLength = Int32.MaxValue;
            return JsonData;


        }


    }
}