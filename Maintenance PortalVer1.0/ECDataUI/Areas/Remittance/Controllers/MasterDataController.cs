using CustomModels.Models.MISReports.PropertyWthoutImportBypassRDPR;
using CustomModels.Models.Remittance.MasterData;
using CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport;
using ECDataUI.Common;
using ECDataUI.Filters;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using static iTextSharp.text.pdf.AcroFields;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorization]

    public class MasterDataController : Controller
    {
        // GET: Remittance/MasterData
        ServiceCaller caller = null;
        [HttpGet]
        [MenuHighlight]
        public ActionResult MasterDataReportView()
        {
            try
            {
                caller = new ServiceCaller("MasterDataAPIController");
                MasterDataReportModel reqModel = caller.GetCall<MasterDataReportModel>("MasterDataView");

                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Summary Report Details View", URLToRedirect = "/Home/HomePage" });
            }
        }
        [HttpPost]

        public ActionResult GetMasterData(FormCollection formCollection)

        {
            caller = new ServiceCaller("MasterDataAPIController");

            MasterDataResultModel resultModel = new MasterDataResultModel();
            string errorMessage = string.Empty;

            MasterDataReportModel reportModel = new MasterDataReportModel();
            var TableId = formCollection["TableId"];
            reportModel.TableId = Convert.ToInt32(TableId);

            resultModel = caller.PostCall<MasterDataReportModel, MasterDataResultModel>("GetMasterData", reportModel, out errorMessage);
            IEnumerable<MasterDataReportTableModel> result = resultModel.MasterDataTableList;
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
                return Json(new { success = false, errorMessage = "Error occured while getting Summary Report Details." });
            }
            if (string.IsNullOrEmpty(TableId) || TableId == "0")
            {
                return Json(new { success = false, errorMessage = "Please select Table Name" }, JsonRequestBehavior.AllowGet);
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
                    switch (reportModel.TableId)
                    {
                        case 1:
                            if (isDigitPresent)
                            {
                                result = result.Where(m => m.VillageCode == int.Parse(searchValue));
                            }
                            else
                            {
                                result = result.Where(m =>

                                                      (m.VillageNameE.ToLower().Contains(searchValue.ToLower())
                                                      ));
                            }

                            break;
                        case 2:
                            result = result.Where(m =>
                                                 ((m.HobliNameE.ToLower().Contains(searchValue.ToLower())
                                                 || (m.ShortNameE.ToLower()).Contains(searchValue.ToLower()))));
                            break;
                        case 3:
                            result = result.Where(m =>
                                                (m.DistrictNameE.ToLower().Contains(searchValue.ToLower()))
                                                || (m.KaveriVillageName.ToLower()).Contains(searchValue.ToLower())
                                                 || m.KaveriHobiName.ToLower().Contains(searchValue.ToLower()
                                                ));
                            break;
                        case 4:
                            result = result.Where(m => (m.DistrictNameE.ToLower().Contains(searchValue.ToLower())) || (m.ShortNameE.ToLower().
                            Contains(searchValue.ToLower())));

                            break;
                        case 5:

                            result = result.Where(m => m.SROCode == int.Parse(searchValue));
                            break;
                        case 6:

                            result = result.Where((m => m.SRONameE.ToLower().Contains(searchValue.ToLower()) || (m.ShortNameE.ToLower().
                            Contains(searchValue.ToLower()) || (m.SROCode == int.Parse(searchValue)))));
                            break;
                        case 7:

                            if (isDigitPresent)
                            {
                                result = result.Where(m => m.KaveriCode == int.Parse(searchValue));

                            }
                            else
                            {

                                result = result.Where((m => (m.OfficeName.ToLower().Contains(searchValue.ToLower())) || (m.ShortName.ToLower().
                               Contains(searchValue.ToLower()))));
                            }
                            break;
                        case 8:
                            if (isDigitPresent)
                            {
                                result = result.Where(m => (m.VillageID == int.Parse(searchValue)) || (m.OfficeID == int.Parse(searchValue)) ||
                               (m.TalukaID == int.Parse(searchValue))
                                );

                            }
                            else
                            {
                                result = result.Where(m => (m.VillageName.ToLower().Contains(searchValue.ToLower())));

                            }
                            break;
                        case 9:
                            if (isDigitPresent)
                            {
                                result = result.Where(m => (m.HobliID == int.Parse(searchValue)) || (m.TalukaID == int.Parse(searchValue))

                                );

                            }
                            else
                            {
                                result = result.Where(m => (m.HobliName.ToLower().Contains(searchValue.ToLower())));
                            }
                            break;





                    }




                    TotalCount = result.Count();
                    IsRecordFilter = true;
                }
            }

            var gridData = result.Select(masterDataReportTableModel => new
            {
                SrNo = masterDataReportTableModel.SrNo,
                VillageCode = masterDataReportTableModel.VillageCode,
                SROCode = masterDataReportTableModel.SROCode,
                HobliCode = masterDataReportTableModel.HobliCode,
                CensusCode = masterDataReportTableModel.CensusCode,
                TalukCode = masterDataReportTableModel.TalukCode,
                VillageNameK = masterDataReportTableModel.VillageNameK,
                VillageNameE = masterDataReportTableModel.VillageNameE,
                IsUrban = masterDataReportTableModel.IsUrban,
                BhoomiTalukCode = masterDataReportTableModel.BhoomiTalukCode,
                BhoomiVillageCode = masterDataReportTableModel.BhoomiVillageCode,
                BhoomiVillageName = masterDataReportTableModel.BhoomiVillageName,

                //hoblimaster
                HobliNameK = masterDataReportTableModel.HobliNameK,
                HobliNameE = masterDataReportTableModel.HobliNameE,
                ShortNameK = masterDataReportTableModel.ShortNameK,
                ShortNameE = masterDataReportTableModel.ShortNameE,
                BhoomiHobliCode = masterDataReportTableModel.BhoomiHobliCode,
                BhoomiHobliName = masterDataReportTableModel.BhoomiHobliName,
                //bhoomi
                BhoomiMappingID = masterDataReportTableModel.BhoomiMappingID,
                KaveriSROCode = masterDataReportTableModel.KaveriSROCode,
                KaveriSROName = masterDataReportTableModel.KaveriSROName,
                KaveriVillageCode = masterDataReportTableModel.KaveriVillageCode,
                KaveriVillageName = masterDataReportTableModel.KaveriVillageName,
                kaveriHobliCode = masterDataReportTableModel.KaveriHobiCode,
                KaveriHobliName = masterDataReportTableModel.KaveriHobiName,
                BhoomiDistrictCode = masterDataReportTableModel.BhoomiDistrictCode,
                BhoomiTalukName = masterDataReportTableModel.BhoomiTalukName,
                BhoomiHobiName = masterDataReportTableModel.BhoomiHobiName,

                IsUpdated = masterDataReportTableModel.IsUpdated,


                //district master
                DistrictCode = masterDataReportTableModel.DistrictCode,
                DistrictNameK = masterDataReportTableModel.DistrictNameK,
                DistrictNameE = masterDataReportTableModel.DistrictNameE,
                DIGCode = masterDataReportTableModel.DIGCode,

                //villagemastermergingmapping
                ID = masterDataReportTableModel.ID,
                MergedVillageCode = masterDataReportTableModel.MergedVillageCode,


                //sroMAster
                SRONameK = masterDataReportTableModel.SRONameK,
                SRONameE = masterDataReportTableModel.SRONameE,
                GetBhoomiData = masterDataReportTableModel.GetBhoomiData,
                IsVillageMatching = masterDataReportTableModel.IsVillageMatching,




                #region Added by vijay on 20-1-2023 for MAS_OfficeMaster
                OfficeID = masterDataReportTableModel.OfficeID,
                OfficeTypeID = masterDataReportTableModel.OfficeTypeID,
                OfficeName = masterDataReportTableModel.OfficeName,
                OfficeNameR = masterDataReportTableModel.OfficeNameR,
                ShortName = masterDataReportTableModel.ShortName,
                ShortNameR = masterDataReportTableModel.ShortNameR,
                DistrictID = masterDataReportTableModel.DistrictID,
                ParentOfficeID = masterDataReportTableModel.ParentOfficeID,
                KaveriCode = (short?)masterDataReportTableModel.KaveriCode,
                BhoomiCensusCode = masterDataReportTableModel.BhoomiCensusCode,
                AnyWhereRegEnabled = masterDataReportTableModel.AnyWhereRegEnabled,
                OfficeAddress = masterDataReportTableModel.OfficeAddress,
                Landline = masterDataReportTableModel.Landline,
                Mobile = masterDataReportTableModel.Mobile,
                OnlineBookingEnabled = masterDataReportTableModel.OnlineBookingEnabled,
                #endregion

                //20-23
                VillageID = masterDataReportTableModel.VillageID,
                HobliID = masterDataReportTableModel.HobliID,
                TalukaID = masterDataReportTableModel.TalukaID,
                VillageName = masterDataReportTableModel.VillageName,
                VillageNameR = masterDataReportTableModel.VillageNameR,
                UPORTownID = masterDataReportTableModel.UPORTownID,
                //20-23

                HobliName = masterDataReportTableModel.HobliName,
                HobliNameR = masterDataReportTableModel.HobliNameR,











            });
            ;
            ;


            String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:150%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + reportModel.TableId + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

            var JsonData = Json(new
            {
                draw = formCollection["draw"],
                data = "",
                ExcelDownloadBtn = ExcelDownloadBtn,
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
                    ExcelDownloadBtn = ExcelDownloadBtn,
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
                    ExcelDownloadBtn = ExcelDownloadBtn,
                    IsRecordFilter = IsRecordFilter,
                });
            }

            JsonData.MaxJsonLength = Int32.MaxValue;
            return JsonData;


        }

        public ActionResult ExportSummaryReportToExcel(string TableId)
        {
            try
            {
                caller = new ServiceCaller("MasterDataAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("MasterDataReport.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                MasterDataReportModel reqModel = new MasterDataReportModel();

                reqModel.TableId = Convert.ToInt32(TableId);

                string excelHeader = string.Empty;
                string message = string.Empty;
                string createdExcelPath = string.Empty;
                MasterDataResultModel Result = caller.PostCall<MasterDataReportModel, MasterDataResultModel>("GetMasterData", reqModel, out errorMessage);


                if (Result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }
                List<MasterDataReportTableModel> masterDataReportTableModel = Result.MasterDataTableList;

                if (Result.MasterDataTableList != null && Result.MasterDataTableList.Count > 0)
                {
                    fileName = "MasterData_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("Master Data Report");

                    switch (reqModel.TableId)
                    {
                        case 1:
                            createdExcelPath = CreateExcelForVillageMaster(masterDataReportTableModel, fileName, excelHeader);
                            break;
                        case 2:
                            createdExcelPath = CreateExcelForHobliMaster(masterDataReportTableModel, fileName, excelHeader);
                            break;
                        case 3:
                            createdExcelPath = CreateExcelForBhoomiMapping(masterDataReportTableModel, fileName, excelHeader);
                            break;
                        case 4:
                            createdExcelPath = CreateExcelForDistrictMaster(masterDataReportTableModel, fileName, excelHeader);
                            break;
                        case 5:
                            createdExcelPath = CreateExcelForVMasterMergingMapping(masterDataReportTableModel, fileName, excelHeader);
                            break;
                        case 6:
                            createdExcelPath = CreateExcelForSROMaster(masterDataReportTableModel, fileName, excelHeader);
                            break;
                        case 7:
                            createdExcelPath = CreateExcelForMAS_OfficeMaster(masterDataReportTableModel, fileName, excelHeader);
                            break;
                        case 8:
                            createdExcelPath = CreateExcelForMAS_Villages(masterDataReportTableModel, fileName, excelHeader);
                            break;
                        case 9:
                            createdExcelPath = CreateExcelForMAS_Hoblis(masterDataReportTableModel, fileName, excelHeader);
                            break;

                    }
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

        private string CreateExcelForVillageMaster(List<MasterDataReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("VillageMaster");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 10].Style.Font.Size = 14;



                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;

                    for (int i = 1; i < 12; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;


                    workSheet.Cells[1, 1].Value = "VillageCode";
                    workSheet.Cells[1, 2].Value = "SROCode";

                    workSheet.Cells[1, 3].Value = "HobliCode";
                    workSheet.Cells[1, 4].Value = "CensusCode";
                    workSheet.Cells[1, 5].Value = "TalukCode";
                    workSheet.Cells[1, 6].Value = "VillageNameK";
                    workSheet.Cells[1, 7].Value = "VillageNameE";

                    workSheet.Cells[1, 8].Value = "IsUrban";
                    workSheet.Cells[1, 9].Value = "BhoomiTalukCode";
                    //changed by vijay on 06-02-2023
                    workSheet.Cells[1, 10].Value = "BhoomiVillageCode";
                    //changed by vijay on 06-02-2023

                    workSheet.Cells[1, 11].Value = "BhoomiVillageName";


                    foreach (var items in result)
                    {
                        for (int i = 1; i < 12; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";

                        }


                        workSheet.Cells[rowIndex, 1].Value = items.VillageCode;
                        workSheet.Cells[rowIndex, 2].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 3].Value = items.HobliCode;

                        workSheet.Cells[rowIndex, 4].Value = items.CensusCode;
                        workSheet.Cells[rowIndex, 5].Value = items.TalukCode;
                        workSheet.Cells[rowIndex, 6].Value = items.VillageNameK;
                        workSheet.Cells[rowIndex, 7].Value = items.VillageNameE;
                        workSheet.Cells[rowIndex, 8].Value = items.IsUrban;
                        workSheet.Cells[rowIndex, 9].Value = items.BhoomiTalukCode;
                        workSheet.Cells[rowIndex, 10].Value = items.BhoomiVillageCode;
                        workSheet.Cells[rowIndex, 11].Value = items.BhoomiVillageName;




                        for (int i = 1; i < 12; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
        private string CreateExcelForHobliMaster(List<MasterDataReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("HobliMaster");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 8].Style.Font.Size = 14;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;

                    for (int i = 1; i < 9; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;
                    workSheet.Cells[1, 1].Value = "HobliCode";
                    workSheet.Cells[1, 2].Value = "TalukCode";
                    workSheet.Cells[1, 3].Value = "HobliNameK";
                    workSheet.Cells[1, 4].Value = "HobliNameE";
                    workSheet.Cells[1, 5].Value = "ShortNameK";
                    workSheet.Cells[1, 6].Value = "ShortNameE";
                    workSheet.Cells[1, 7].Value = "BhoomiHobliCode";
                    workSheet.Cells[1, 8].Value = "BhoomiHobliName";






                    foreach (var items in result)
                    {
                        for (int i = 1; i < 9; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }


                        workSheet.Cells[rowIndex, 1].Value = items.HobliCode;
                        workSheet.Cells[rowIndex, 2].Value = items.TalukCode;
                        workSheet.Cells[rowIndex, 3].Value = items.HobliNameK;
                        workSheet.Cells[rowIndex, 4].Value = items.HobliNameE;
                        workSheet.Cells[rowIndex, 5].Value = items.ShortNameK;
                        workSheet.Cells[rowIndex, 6].Value = items.ShortNameE;
                        workSheet.Cells[rowIndex, 7].Value = items.BhoomiHobliCode;
                        workSheet.Cells[rowIndex, 8].Value = items.BhoomiHobliName;

                        for (int i = 1; i < 9; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
        private string CreateExcelForBhoomiMapping(List<MasterDataReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("BhoomiMappingDetails");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 17].Style.Font.Size = 14;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;
                    workSheet.Column(12).Width = 30;
                    workSheet.Column(13).Width = 30;
                    workSheet.Column(14).Width = 30;
                    workSheet.Column(15).Width = 30;
                    workSheet.Column(16).Width = 30;
                    workSheet.Column(17).Width = 30;


                    for (int i = 1; i < 18; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;

                    workSheet.Cells[1, 1].Value = "BhoomiMappingID";
                    workSheet.Cells[1, 2].Value = "DistrictCode";

                    workSheet.Cells[1, 3].Value = "DistrictNameE";
                    workSheet.Cells[1, 4].Value = "KaveriSROCode";
                    workSheet.Cells[1, 5].Value = "KaveriSROName";
                    workSheet.Cells[1, 6].Value = "KaveriVillageCode";


                    workSheet.Cells[1, 7].Value = "KaveriVillageName";

                    workSheet.Cells[1, 8].Value = "KaveriHobiCode";
                    workSheet.Cells[1, 9].Value = "KaveriHobiName";
                    workSheet.Cells[1, 10].Value = "BhoomiDistrictCode";
                    workSheet.Cells[1, 11].Value = "BhoomiTalukCode";
                    workSheet.Cells[1, 12].Value = "BhoomiTalukName";
                    workSheet.Cells[1, 13].Value = "BhoomiHobiCode";
                    workSheet.Cells[1, 14].Value = "BhoomiHobiName";
                    workSheet.Cells[1, 15].Value = "BhoomiVillageCode";
                    workSheet.Cells[1, 16].Value = "BhoomiVillageName";
                    workSheet.Cells[1, 17].Value = "IsUpdated";
                    foreach (var items in result)
                    {
                        for (int i = 1; i < 18; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }

                        workSheet.Cells[rowIndex, 1].Value = items.BhoomiMappingID;
                        workSheet.Cells[rowIndex, 2].Value = items.DistrictCode;
                        workSheet.Cells[rowIndex, 3].Value = items.DistrictNameE;
                        workSheet.Cells[rowIndex, 4].Value = items.KaveriSROCode;
                        workSheet.Cells[rowIndex, 5].Value = items.KaveriSROName;
                        workSheet.Cells[rowIndex, 6].Value = items.KaveriVillageCode;
                        workSheet.Cells[rowIndex, 7].Value = items.KaveriVillageName;
                        workSheet.Cells[rowIndex, 8].Value = items.KaveriHobiCode;
                        workSheet.Cells[rowIndex, 9].Value = items.KaveriHobiName;
                        workSheet.Cells[rowIndex, 10].Value = items.BhoomiDistrictCode;
                        workSheet.Cells[rowIndex, 11].Value = items.BhoomiTalukCode;
                        workSheet.Cells[rowIndex, 12].Value = items.BhoomiTalukName;
                        workSheet.Cells[rowIndex, 13].Value = items.BhoomiHobiCode;
                        workSheet.Cells[rowIndex, 14].Value = items.BhoomiHobiName;
                        workSheet.Cells[rowIndex, 15].Value = items.BhoomiVillageCode;
                        workSheet.Cells[rowIndex, 16].Value = items.BhoomiVillageName;
                        workSheet.Cells[rowIndex, 17].Value = items.IsUpdated;



                        for (int i = 1; i < 18; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
        private string CreateExcelForDistrictMaster(List<MasterDataReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("DistrictMaster");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 7].Style.Font.Size = 14;


                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;


                    for (int i = 1; i < 8; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;

                    workSheet.Cells[1, 1].Value = "DistrictCode";
                    workSheet.Cells[1, 2].Value = "DistrictNameE";

                    workSheet.Cells[1, 3].Value = "DistrictNameK";
                    workSheet.Cells[1, 4].Value = "ShortNameE ";
                    workSheet.Cells[1, 5].Value = "ShortNameK";
                    workSheet.Cells[1, 6].Value = "DIGCode";

                    workSheet.Cells[1, 7].Value = "BhoomiDistrictCode";

                    foreach (var items in result)
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }
                        workSheet.Cells[rowIndex, 1].Value = items.DistrictCode;
                        workSheet.Cells[rowIndex, 2].Value = items.DistrictNameE;
                        workSheet.Cells[rowIndex, 3].Value = items.DistrictNameK;
                        workSheet.Cells[rowIndex, 4].Value = items.ShortNameE;
                        workSheet.Cells[rowIndex, 5].Value = items.ShortNameK;
                        workSheet.Cells[rowIndex, 6].Value = items.DIGCode;
                        workSheet.Cells[rowIndex, 7].Value = items.BhoomiDistrictCode;


                        for (int i = 1; i < 8; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
        private string CreateExcelForVMasterMergingMapping(List<MasterDataReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("VillageMasterVillagesMergingMapping");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 4].Style.Font.Size = 14;


                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;



                    for (int i = 1; i < 5; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;

                    workSheet.Cells[1, 1].Value = "ID";
                    workSheet.Cells[1, 2].Value = "SROCode";

                    workSheet.Cells[1, 3].Value = "VillageCode";
                    workSheet.Cells[1, 4].Value = "MergedVillageCode";



                    foreach (var items in result)
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }

                        workSheet.Cells[rowIndex, 1].Value = items.ID;
                        workSheet.Cells[rowIndex, 2].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 3].Value = items.VillageCode;
                        workSheet.Cells[rowIndex, 4].Value = items.MergedVillageCode;

                        for (int i = 1; i < 5; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
        private string CreateExcelForSROMaster(List<MasterDataReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("SROMaster");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 8].Style.Font.Size = 14;





                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;


                    for (int i = 1; i < 9; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;
                    workSheet.Cells[1, 1].Value = "SROCode";
                    workSheet.Cells[1, 2].Value = "DistrictCode";

                    workSheet.Cells[1, 3].Value = "SRONameK";
                    workSheet.Cells[1, 4].Value = "SRONameE";
                    workSheet.Cells[1, 5].Value = "ShortnameK";
                    workSheet.Cells[1, 6].Value = "ShortNameE";
                    workSheet.Cells[1, 7].Value = "GetBhoomiData";
                    workSheet.Cells[1, 8].Value = "IsVillageMatching";



                    foreach (var items in result)
                    {
                        for (int i = 1; i < 9; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }

                        workSheet.Cells[rowIndex, 1].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 2].Value = items.DistrictCode;
                        workSheet.Cells[rowIndex, 3].Value = items.SRONameK;
                        workSheet.Cells[rowIndex, 4].Value = items.SRONameE;
                        workSheet.Cells[rowIndex, 5].Value = items.ShortNameK;
                        workSheet.Cells[rowIndex, 6].Value = items.ShortNameE;
                        workSheet.Cells[rowIndex, 7].Value = items.GetBhoomiData;
                        workSheet.Cells[rowIndex, 8].Value = items.IsVillageMatching;

                        for (int i = 1; i < 9; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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


        #region Added by vijay on 20-01-2023 for MAS_OfficeMaster 
        private string CreateExcelForMAS_OfficeMaster(List<MasterDataReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("MAS_Office_Master");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 15].Style.Font.Size = 14;


                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;
                    workSheet.Column(12).Width = 60;
                    workSheet.Column(13).Width = 30;
                    workSheet.Column(14).Width = 30;
                    workSheet.Column(15).Width = 30;

                    for (int i = 1; i < 16; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;
                    workSheet.Cells[1, 1].Value = "OfficeID";
                    workSheet.Cells[1, 2].Value = "OfficeTypeID";

                    workSheet.Cells[1, 3].Value = "OfficeName";
                    workSheet.Cells[1, 4].Value = "OfficeNameR";
                    workSheet.Cells[1, 5].Value = "ShortName";
                    workSheet.Cells[1, 6].Value = "ShortNameR";
                    workSheet.Cells[1, 7].Value = "DistrictID";
                    workSheet.Cells[1, 8].Value = "ParentOfficeID";
                    workSheet.Cells[1, 9].Value = "KaveriCode";
                    workSheet.Cells[1, 10].Value = "BhoomiCensusCode";
                    workSheet.Cells[1, 11].Value = "AnyWhereRegEnabled";
                    workSheet.Cells[1, 12].Value = "OfficeAddress";
                    workSheet.Cells[1, 13].Value = "Landline";
                    workSheet.Cells[1, 14].Value = "Mobile";
                    workSheet.Cells[1, 15].Value = "OnlineBookingEnabled";



                    foreach (var items in result)
                    {
                        for (int i = 1; i < 16; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }

                        workSheet.Cells[rowIndex, 1].Value = items.OfficeID;
                        workSheet.Cells[rowIndex, 2].Value = items.OfficeTypeID;
                        workSheet.Cells[rowIndex, 3].Value = items.OfficeName;
                        workSheet.Cells[rowIndex, 4].Value = items.OfficeNameR;
                        workSheet.Cells[rowIndex, 5].Value = items.ShortName;
                        workSheet.Cells[rowIndex, 6].Value = items.ShortNameR;
                        workSheet.Cells[rowIndex, 7].Value = items.DistrictID;
                        workSheet.Cells[rowIndex, 8].Value = items.ParentOfficeID;
                        workSheet.Cells[rowIndex, 9].Value = (short?)items.KaveriCode;
                        workSheet.Cells[rowIndex, 10].Value = items.BhoomiCensusCode;
                        workSheet.Cells[rowIndex, 11].Value = items.AnyWhereRegEnabled;
                        workSheet.Cells[rowIndex, 12].Value = items.OfficeAddress;
                        workSheet.Cells[rowIndex, 13].Value = items.Landline;
                        workSheet.Cells[rowIndex, 14].Value = items.Mobile;
                        workSheet.Cells[rowIndex, 15].Value = items.OnlineBookingEnabled;

                        for (int i = 1; i < 16; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
        #endregion

        #region Added by vijay on 20-01-2023 for MAS_Villages
        private string CreateExcelForMAS_Villages(List<MasterDataReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("MAS_Villages");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 13].Style.Font.Size = 14;



                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 40;
                    workSheet.Column(11).Width = 40;
                    workSheet.Column(12).Width = 30;
                    workSheet.Column(13).Width = 30;





                    for (int i = 1; i < 13; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;

                    workSheet.Cells[1, 1].Value = "VillageID";
                    workSheet.Cells[1, 2].Value = "OfficeID";
                    workSheet.Cells[1, 3].Value = "HobliID";
                    workSheet.Cells[1, 4].Value = "TalukaID";
                    workSheet.Cells[1, 5].Value = "VillageName";
                    workSheet.Cells[1, 6].Value = "VillageNameR";
                    workSheet.Cells[1, 7].Value = "IsUrban";
                    workSheet.Cells[1, 8].Value = "CensusCode";
                    workSheet.Cells[1, 9].Value = "BhoomiTalukCode";
                    workSheet.Cells[1, 10].Value = "BhoomiVillageCode";
                    workSheet.Cells[1, 11].Value = "BhoomiVillageName";
                    workSheet.Cells[1, 12].Value = "BhoomiDistrictCode";
                    workSheet.Cells[1, 13].Value = "UPORTownID";



                    foreach (var items in result)
                    {
                        for (int i = 1; i < 13; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }

                        workSheet.Cells[rowIndex, 1].Value = items.VillageID;
                        workSheet.Cells[rowIndex, 2].Value = items.OfficeID;
                        workSheet.Cells[rowIndex, 3].Value = items.HobliID;
                        workSheet.Cells[rowIndex, 4].Value = items.TalukaID;
                        workSheet.Cells[rowIndex, 5].Value = items.VillageName;
                        workSheet.Cells[rowIndex, 6].Value = items.VillageNameR;
                        workSheet.Cells[rowIndex, 7].Value = items.IsUrban;
                        workSheet.Cells[rowIndex, 8].Value = items.CensusCode;
                        workSheet.Cells[rowIndex, 9].Value = (short?)items.BhoomiTalukCode;
                        workSheet.Cells[rowIndex, 10].Value = items.BhoomiVillageCode;
                        workSheet.Cells[rowIndex, 11].Value = items.BhoomiVillageName;
                        workSheet.Cells[rowIndex, 12].Value = items.BhoomiDistrictCode;
                        workSheet.Cells[rowIndex, 13].Value = items.UPORTownID;


                        for (int i = 1; i < 13; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
        #endregion
        #region Added by vijay on 20-01-2023 for MAS_Hoblis
        private string CreateExcelForMAS_Hoblis(List<MasterDataReportTableModel> result, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("MAS_Hoblis");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 9].Style.Font.Size = 14;



                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;





                    for (int i = 1; i < 10; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 2;

                    workSheet.Cells[1, 1].Value = "HobliID";
                    workSheet.Cells[1, 2].Value = "TalukaID";
                    workSheet.Cells[1, 3].Value = "HobliName";
                    workSheet.Cells[1, 4].Value = "HobliNameR";
                    workSheet.Cells[1, 5].Value = "BhoomiHobliCode";
                    workSheet.Cells[1, 6].Value = "BhoomiHobliName";
                    workSheet.Cells[1, 7].Value = "BhoomiTalukCode";
                    workSheet.Cells[1, 8].Value = "BhoomiTalukName";
                    workSheet.Cells[1, 9].Value = "BhoomiDistrictCode";



                    foreach (var items in result)
                    {
                        for (int i = 1; i < 10; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }

                        workSheet.Cells[rowIndex, 1].Value = items.HobliID;
                        workSheet.Cells[rowIndex, 2].Value = items.TalukaID;
                        workSheet.Cells[rowIndex, 3].Value = items.HobliName;
                        workSheet.Cells[rowIndex, 4].Value = items.HobliNameR;
                        workSheet.Cells[rowIndex, 5].Value = items.BhoomiHobliCode;
                        workSheet.Cells[rowIndex, 6].Value = items.BhoomiHobliName;
                        workSheet.Cells[rowIndex, 7].Value = (short?)items.BhoomiTalukCode;
                        workSheet.Cells[rowIndex, 8].Value = items.BhoomiTalukName;
                        workSheet.Cells[rowIndex, 9].Value = items.BhoomiDistrictCode;


                        for (int i = 1; i < 10; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
        #endregion



        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }
    }



}

