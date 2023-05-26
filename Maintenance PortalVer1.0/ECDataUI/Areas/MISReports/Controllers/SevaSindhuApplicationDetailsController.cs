using CustomModels.Models.MISReports.PropertyWthoutImportBypassRDPR;
using CustomModels.Models.MISReports.SevaSidhuApplicationDetails;
using CustomModels.Models.Remittance.ChallanDataEntryCorrectionDetails;
using CustomModels.Models.Remittance.MasterData;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

//added by vijay on 01-03-2023

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorization]
    public class SevaSindhuApplicationDetailsController : Controller
    {
        ServiceCaller caller = null;
        int OfficeID = KaveriSession.Current.OfficeID;

        [MenuHighlight]
        [HttpGet]
        public ActionResult SevaSindhuApplicationDetailsReportView()
        {

            try
            {
                caller = new ServiceCaller("SevaSindhuApplicationDetailsAPIController");

                SevaSindhuApplicationDetailsReportModel reportmodel = new SevaSindhuApplicationDetailsReportModel();
                reportmodel.OfficeID = OfficeID;

                SevaSindhuApplicationDetailsReportModel reqModel = caller.GetCall<SevaSindhuApplicationDetailsReportModel>("SevaSindhuApplicationDetailsReportView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Seva Sindhu application Details Report View", URLToRedirect = "/Home/HomePage" });
            }
        }


        [HttpPost]

        public ActionResult GetSevaSindhuApplicationDetails(FormCollection formCollection)

        {
            caller = new ServiceCaller("SevaSindhuApplicationDetailsAPIController");

            SevaSindhuApplicationDetailsResultModel resultModel = new SevaSindhuApplicationDetailsResultModel();
            string errorMessage = string.Empty;
            SevaSindhuApplicationDetailsReportModel reportModel = new SevaSindhuApplicationDetailsReportModel();
            var SROId = formCollection["SROfficeID"];
            var DROfficeID = formCollection["DROfficeID"];

            reportModel.SROfficeID = Convert.ToInt32(SROId);
            reportModel.DROfficeID = Convert.ToInt32(DROfficeID);

            //reportmodel.OfficeID = OfficeID;


            //var LocallyUpdated = formCollection["IsLocalluUpdated"];
            //var CentrallyllyUpdated = formCollection["IsCentrallyUpdated"];




            string FromDate = formCollection["FromDate"];

            string ToDate = formCollection["ToDate"];

            if (FromDate.Equals("") || ToDate.Equals(""))
            {
                return Json(new { success = false, errorMessage = "Please enter Date." });


            }


            reportModel.FromDate = Convert.ToDateTime(FromDate);
            reportModel.ToDate = Convert.ToDateTime(ToDate);

            if (reportModel.FromDate > reportModel.ToDate)
            {
                return Json(new { success = false, errorMessage = "The 'From' date cannot be greater than the 'To' date" });

            }


            else
            {
                resultModel = caller.PostCall<SevaSindhuApplicationDetailsReportModel, SevaSindhuApplicationDetailsResultModel>("GetSevaSindhuApplicationDetails", reportModel, out errorMessage);

            }

            IEnumerable<SevaSindhuApplicationDetailsReportTableModel> result = resultModel.SevaSindhuApplicationDetailsReportTableList;




            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
            System.Text.RegularExpressions.Match mtch = regx.Match((string)searchValue);
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
                return Json(new { success = false, errorMessage = "Error occured while Seva Sindhu Application Details." });

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

                    searchValue = searchValue.Trim();
                    result = result.Where(m => (m.AknowledgementNo.Contains(searchValue)) || m.OfficeName.Contains(searchValue));




                    TotalCount = result.Count();
                    IsRecordFilter = true;
                }
            }

            var gridData = result.Select(model => new
            {
                // SROCode = model.SROCode,

                SrNo = model.SRNO,
                RefNo = model.ReferenceNo,
                Akg = model.AknowledgementNo,
                RegDate = model.MarriageRegistrationDate,
                MarrigeType = model.MarriageTypeID,
                CaseNo = model.MarrigecaseNo,
                AppRecivedDate = model.ApplicationRecivedDateTime,
                OfficeName = model.OfficeName,
                
                AppointmentDateTime = model.AppointmentDateTime,
                ApplicationStatus=model.ApplicationStatus,

                RejectionReason= model.RejectionReason,
                AcceptDateTime=model.ApplicationAcceptDateTime,
                RejectDateTime=model.ApplicationRejectDateTime,







            }); ; ;
            ;
            ;
            String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type='button' style='width:150%;' class='btn btn-group-md btn-success' onclick='EXCELDownloadFun(\"" + reportModel.FromDate + "\",\"" + reportModel.ToDate + "\",\"" + reportModel.SROfficeID + "\")'><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";



            var JsonData = Json(new
            {
                draw = formCollection["draw"],
                data = "",
                recordsFiltered = 0,
                recordsTotal = 0,
                ExcelDownloadBtn = ExcelDownloadBtn,

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
                    ExcelDownloadBtn = ExcelDownloadBtn,

                });
            }
            else
            {
                JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                    recordsTotal = totalNum,
                    ExcelDownloadBtn = ExcelDownloadBtn,

                    recordsFiltered = TotalCount,
                    status = "1",
                    IsRecordFilter = IsRecordFilter,
                });
            }

            JsonData.MaxJsonLength = Int32.MaxValue;
         
            return JsonData;


        }






        public ActionResult ExportReportToExcel(string FromDate, string ToDate, string SROfficeID)
        {
            try
            {


                SevaSindhuApplicationDetailsResultModel resultModel = new SevaSindhuApplicationDetailsResultModel();
                string errorMessage = string.Empty;
                SevaSindhuApplicationDetailsReportModel reportmodel = new SevaSindhuApplicationDetailsReportModel();
                reportmodel.SROfficeID = Convert.ToInt32(SROfficeID);
                caller = new ServiceCaller("SevaSindhuApplicationDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("SevaSindhuApplicationDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();








                reportmodel.FromDate = Convert.ToDateTime(FromDate);
                reportmodel.ToDate = Convert.ToDateTime(ToDate);

                //  reqModel.TableId = Convert.ToInt32(TableId);

                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;

                if (KaveriSession.Current.RoleID == (int)CommonEnum.RoleDetails.TechnicalAdmin)
                {
                    resultModel = caller.PostCall<SevaSindhuApplicationDetailsReportModel, SevaSindhuApplicationDetailsResultModel>("GetSevaSindhuApplicationDetails_For_TA", reportmodel, out errorMessage);

                }
                else
                {

                    resultModel = caller.PostCall<SevaSindhuApplicationDetailsReportModel, SevaSindhuApplicationDetailsResultModel>("GetSevaSindhuApplicationDetails", reportmodel, out errorMessage);
                }

                if (resultModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
                //   List<SevaSindhuApplicationDetailsReportTableModel> result = resultModel.SevaSindhuApplicationDetailsReportTableList;

                IEnumerable<SevaSindhuApplicationDetailsReportTableModel> result = resultModel.SevaSindhuApplicationDetailsReportTableList;

                if (resultModel.SevaSindhuApplicationDetailsReportTableList != null && resultModel.SevaSindhuApplicationDetailsReportTableList.Count > 0)
                {
                    fileName = "SevaSindhuApplicationDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("Seva SindhuApplication Details_");



                    if (OfficeID != 18)

                        createdExcelPath = CreateExcelForSevaSindhu(result, fileName, excelHeader);
                    else
                        createdExcelPath = CreateExcelForSevaSindhu_TA(result, fileName, excelHeader);




                }
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);


                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }



        private string CreateExcelForSevaSindhu(IEnumerable<SevaSindhuApplicationDetailsReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("SevaSindhuApplicationDetails");

                          workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[6, 3, 6, 10].Style.Font.Size = 18;
                    //workSheet.Cells[6, 3, 6, 10].Style.Font.Italic = true;

                    workSheet.Cells[1, 1].Value = excelHeader;


                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());



                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 8].Style.Font.Size = 14;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 50;
                    workSheet.Column(6).Width = 50;
                    workSheet.Column(7).Width = 50;
                    workSheet.Column(8).Width= 50;
                    workSheet.Column(9).Width = 70;
                    workSheet.Column(10).Width = 70;
                    workSheet.Column(11).Width = 70;
                    workSheet.Column(12).Width = 70;
                    workSheet.Column(12).Width = 70;
                    workSheet.Column(13).Width = 100;

                    for (int i = 1; i < 13; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }


                    







                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;
                    workSheet.Cells[1, 1].Value = "Sr.No.";
                    workSheet.Cells[1, 2].Value = "Office Name";
                    workSheet.Cells[1, 3].Value = "Reference No.";
                    workSheet.Cells[1, 4].Value = "Aknowledgement Number";
                    workSheet.Cells[1, 5].Value = "Application Received Date Time";
                    workSheet.Cells[1, 6].Value = "Appointment Date Time";
                    workSheet.Cells[1, 7].Value = "Marriage Type";
                    workSheet.Cells[1, 8].Value = "Marriage Case No.";
                    workSheet.Cells[1, 9].Value = "Marriage Registration Date";
                    workSheet.Cells[1, 10].Value = "Application Status";
                    workSheet.Cells[1, 11].Value = "Acceptance Date Time";
                    workSheet.Cells[1, 12].Value = "Rejection Date Time";
                    workSheet.Cells[1, 13].Value = "Rejection Reason";









                    foreach (var items in result)
                    {
                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }


                        workSheet.Cells[rowIndex, 1].Value = items.SRNO;
                        workSheet.Cells[rowIndex, 1].Value = items.SRNO;
                        workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                        workSheet.Cells[rowIndex, 3].Value = items.ReferenceNo;
                        workSheet.Cells[rowIndex, 4].Value = items.AknowledgementNo;
                        workSheet.Cells[rowIndex, 5].Value = items.ApplicationRecivedDateTime;
                        workSheet.Cells[rowIndex, 6].Value = items.AppointmentDateTime;
                        workSheet.Cells[rowIndex, 7].Value = items.MaarigeType;
                        workSheet.Cells[rowIndex, 8].Value = items.MarrigecaseNo;
                        workSheet.Cells[rowIndex, 9].Value = items.ApplicationRegistrationDateTime;
                        workSheet.Cells[rowIndex, 10].Value = items.ApplicationStatus;
                        workSheet.Cells[rowIndex, 11].Value = items.ApplicationAcceptDateTime;
                        workSheet.Cells[rowIndex, 12].Value = items.ApplicationRejectDateTime;
                        workSheet.Cells[rowIndex, 13].Value = items.RejectionReason;

                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[1, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        }

                        rowIndex++;
                    }


                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }

        private string CreateExcelForSevaSindhu_TA(IEnumerable<SevaSindhuApplicationDetailsReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("SevaSindhuApplicationDetails");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 8].Style.Font.Size = 14;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 40;
                    workSheet.Column(4).Width = 500;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 40;
                    workSheet.Column(7).Width = 40;
                    workSheet.Column(8).Width = 40;
                    workSheet.Column(9).Width = 40;
                    workSheet.Column(10).Width = 45;
                    workSheet.Column(11).Width = 40;
                    workSheet.Column(12).Width = 40;
                    workSheet.Column(13).Width = 40;
                    workSheet.Column(14).Width = 45;
                    workSheet.Column(15).Width = 40;
                    workSheet.Column(16).Width = 100;
                    workSheet.Column(17).Width = 30;

                    for (int i = 1; i < 18; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;
                    workSheet.Cells[1, 1].Value = "RequestID";
                    workSheet.Cells[1, 2].Value = "ReferenceNo";
                    workSheet.Cells[1, 3].Value = "SROCode";
                    workSheet.Cells[1, 4].Value = "TransXML";
                    workSheet.Cells[1, 5].Value = "AknowledgementNo";
                    workSheet.Cells[1, 6].Value = "DataReceivedDate";
                    workSheet.Cells[1, 7].Value = "AppointmentDateTime";
                    workSheet.Cells[1, 8].Value = "AppointmentSlot";
                    workSheet.Cells[1, 9].Value = "IsApplicationRegistered";
                    workSheet.Cells[1, 10].Value = "ApplicationRegistrationDateTime";
                    workSheet.Cells[1, 11].Value = "FinalRegistrationNumber";
                    workSheet.Cells[1, 12].Value = "RegistrationID";
                    workSheet.Cells[1, 13].Value = "NoticeID";
                    workSheet.Cells[1, 14].Value = "ApplicationAcceptDateTime";
                    workSheet.Cells[1, 15].Value = "ApplicationRejectDateTime";
                    workSheet.Cells[1, 16].Value = "RejectionReason";
                    workSheet.Cells[1, 17].Value = "ApplicationStatusCode";






                    foreach (var items in result)
                    {
                        for (int i = 1; i < 18; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }


                        workSheet.Cells[rowIndex, 1].Value = items.RequestID;
                        workSheet.Cells[rowIndex, 2].Value = items.ReferenceNo;
                        workSheet.Cells[rowIndex, 3].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 4].Value = items.TransXML;
                        workSheet.Cells[rowIndex, 5].Value = items.AknowledgementNo;
                        workSheet.Cells[rowIndex, 6].Value = items.DataReceivedDate.ToString();
                        if (items.AppointmentDateTime == null)
                        {
                            workSheet.Cells[rowIndex, 7].Value = "NULL";

                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 7].Value = items.AppointmentDateTime.ToString();

                        }

                        workSheet.Cells[rowIndex, 8].Value = items.AppointmentSlot;
                        workSheet.Cells[rowIndex, 9].Value = items.IsApplicationRegistered;
                        if (items.ApplicationRegistrationDateTime == null)
                        {
                            workSheet.Cells[rowIndex, 10].Value = "NULL";

                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 10].Value = items.ApplicationRegistrationDateTime.ToString();
                        }
                        workSheet.Cells[rowIndex, 11].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 12].Value = items.RegistrationID;
                        workSheet.Cells[rowIndex, 13].Value = items.NoticeID;
                        if (items.ApplicationAcceptDateTime == null)
                        {
                            workSheet.Cells[rowIndex, 14].Value = "NULL";

                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 14].Value = items.ApplicationAcceptDateTime.ToString();

                        }
                        if (items.ApplicationRejectDateTime == null)
                        {
                            workSheet.Cells[rowIndex, 15].Value = "NULL";

                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 15].Value = items.ApplicationRejectDateTime.ToString();

                        }
                        workSheet.Cells[rowIndex, 16].Value = items.RejectionReason;
                        workSheet.Cells[rowIndex, 17].Value = items.ApplicationStatusCode;

                        for (int i = 1; i < 18; i++)

                        {
                            if (i == 4)
                            {
                                workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                            }
                            else
                                workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        }

                        for (int i = 1; i < 18; i++)
                        {
                            workSheet.Cells[1, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        }

                        rowIndex++;
                    }


                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }






    }
}