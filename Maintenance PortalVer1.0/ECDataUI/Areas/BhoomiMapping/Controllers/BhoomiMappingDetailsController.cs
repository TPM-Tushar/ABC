using CustomModels.Models.BhoomiMapping;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.BhoomiMapping.Controllers
{
    [KaveriAuthorization]
    public class BhoomiMappingDetailsController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;
        string FilePath { get; set; }
        [MenuHighlight]
        public ActionResult BhoomiMappingDetailsView()
        {
            try
            {
                KaveriSession.Current.OrderID = 0;

                KaveriSession.Current.DECSROCode = 0;
                int OfficeID = KaveriSession.Current.OfficeID;
                int LevelID = KaveriSession.Current.LevelID;
                long UserID = KaveriSession.Current.UserID;
                caller = new ServiceCaller("BhoomiMappingDetailsAPIController");
                BhoomiMappingViewModel reqModel = caller.GetCall<BhoomiMappingViewModel>("BhoomiMappingDetailsView", new { officeID = OfficeID, LevelID = LevelID, UserID = UserID });
                return View(reqModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult LoadDetailsTable(FormCollection formCollection)
        {
            try
            {
                int SROCode = Convert.ToInt32(formCollection["SroCode"]);
                caller = new ServiceCaller("BhoomiMappingDetailsAPIController");
                List<BhoomiMappingTableModel> reqModel = caller.GetCall<List<BhoomiMappingTableModel>>("LoadDetailsTable", new { SROCode = SROCode });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading bhoomi details table", URLToRedirect = "/Home/HomePage" });
                }
                else
                {
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
                            reqModel = reqModel.Where(m => m.KaveriVillageCode.ToString().ToLower().Contains(searchValue.ToLower()) ||
                            m.KaveriVillageName.ToString().ToLower().Contains(searchValue.ToLower())).ToList();
                        }
                    }


                    Random rnd = new Random();
                    int num = rnd.Next();

                    string p_strPath = string.Format("{0}/{1}", Server.MapPath("~/Content/BhoomiUploads"), "ExcelReport" + num.ToString() + ".xlsx");


                    var gridData = reqModel.Select(BhoomiMappingDetailsTableModel => new
                    {
                        SNo = BhoomiMappingDetailsTableModel.SrNo,
                        DistrictName = BhoomiMappingDetailsTableModel.DistrictName,
                        KaveriSROCode = BhoomiMappingDetailsTableModel.KaveriSROCode,
                        KaveriSROName = BhoomiMappingDetailsTableModel.KaveriSROName,
                        KaveriVillageCode = BhoomiMappingDetailsTableModel.KaveriVillageCode,
                        KaveriVillageName = BhoomiMappingDetailsTableModel.KaveriVillageName,
                        KaveriHobiCode = BhoomiMappingDetailsTableModel.KaveriHobiCode,
                        KaveriHobiName = BhoomiMappingDetailsTableModel.KaveriHobiName,
                        BhoomiDistrictCode = BhoomiMappingDetailsTableModel.BhoomiDistrictCode,
                        BhoomiTalukCode = BhoomiMappingDetailsTableModel.BhoomiTalukCode,
                        BhoomiTalukName = BhoomiMappingDetailsTableModel.BhoomiTalukName,
                        BhoomiHobiCode = BhoomiMappingDetailsTableModel.BhoomiHobiCode,
                        BhoomiHobiName = BhoomiMappingDetailsTableModel.BhoomiHobiName,
                        BhoomiVillageCode = BhoomiMappingDetailsTableModel.BhoomiVillageCode,
                        BhoomiVillageName = BhoomiMappingDetailsTableModel.BhoomiVillageName,
                        FileName = "ExcelReport" + num.ToString()




                    });


                    int startLen = Convert.ToInt32(formCollection["start"]);
                    int totalNum = Convert.ToInt32(formCollection["length"]);
                    int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                    int pageSize = totalNum;
                    int skip = startLen;
                    int totalCount = reqModel.Count;
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
                    workSheet.Cells[1, 2].Value = "DistrictName";
                    workSheet.Cells[1, 3].Value = "KaveriSROCode";
                    workSheet.Cells[1, 4].Value = "KaveriSROName";
                    workSheet.Cells[1, 5].Value = "KaveriVillageCode";
                    workSheet.Cells[1, 6].Value = "KaveriVillageName";
                    workSheet.Cells[1, 7].Value = "KaveriHobiCode";
                    workSheet.Cells[1, 8].Value = "KaveriHobiName";
                    workSheet.Cells[1, 9].Value = "BhoomiDistrictCode";
                    workSheet.Cells[1, 10].Value = "BhoomiTalukCode";
                    workSheet.Cells[1, 11].Value = "BhoomiTalukName";
                    workSheet.Cells[1, 12].Value = "BhoomiHobiCode";
                    workSheet.Cells[1, 13].Value = "BhoomiHobiName";
                    workSheet.Cells[1, 14].Value = "BhoomiVillageCode";
                    workSheet.Cells[1, 15].Value = "BhoomiVillageName";

                    // Inserting the article data into excel
                    // sheet by using the for each loop
                    // As we have values to the first row 
                    // we will start with second row
                    int recordIndex = 2;

                    foreach (BhoomiMappingTableModel BMTM in reqModel)
                    {
                        workSheet.Cells[recordIndex, 1].Value = BMTM.SrNo;
                        workSheet.Cells[recordIndex, 2].Value = BMTM.DistrictName;
                        workSheet.Cells[recordIndex, 3].Value = BMTM.KaveriSROCode;
                        workSheet.Cells[recordIndex, 4].Value = BMTM.KaveriSROName;
                        workSheet.Cells[recordIndex, 5].Value = BMTM.KaveriVillageCode;
                        workSheet.Cells[recordIndex, 6].Value = BMTM.KaveriVillageName;
                        workSheet.Cells[recordIndex, 7].Value = BMTM.KaveriHobiCode;
                        workSheet.Cells[recordIndex, 8].Value = BMTM.KaveriHobiName;
                        workSheet.Cells[recordIndex, 9].Value = BMTM.BhoomiDistrictCode;
                        workSheet.Cells[recordIndex, 10].Value = BMTM.BhoomiTalukCode;
                        workSheet.Cells[recordIndex, 11].Value = BMTM.BhoomiTalukName;
                        workSheet.Cells[recordIndex, 12].Value = BMTM.BhoomiHobiCode;
                        workSheet.Cells[recordIndex, 13].Value = BMTM.BhoomiHobiName;
                        workSheet.Cells[recordIndex, 14].Value = BMTM.BhoomiVillageCode;
                        workSheet.Cells[recordIndex, 15].Value = BMTM.BhoomiVillageName;
                        workSheet.Cells[recordIndex, 15].Style.Font.Name = "KNB-TTUmaEN";
                        recordIndex++;
                    }

                    workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();

                    if (!Directory.Exists(Server.MapPath("~/Content/BhoomiUploads")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/BhoomiUploads"));
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
            }
            catch (Exception ex)
            {
                throw;
            }

        }



        public string DeleteReport(string FileName)
        {
            string filename = FileName;
            try
            {
                DateTime CurrentDateTime = DateTime.Now;
                string fullName = string.Empty;
                long CurrentTimeStamp = (CurrentDateTime.Ticks - 621355968000000000) / 10000000;
                DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Server.MapPath("~/Content/BhoomiUploads/"));
                FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles();
                foreach (FileInfo foundFile in filesInDir)
                {
                    fullName = foundFile.FullName;
                    DateTime time = foundFile.CreationTime;
                    long timeStamp = (time.Ticks - 621355968000000000) / 10000000;

                    long total = CurrentTimeStamp - timeStamp;

                    if(total > 1799)
                    {
                        System.IO.File.Delete(fullName);
                        Console.WriteLine(fullName + "Deleted");
                    }
                }

                return "Successful";
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }

    }
}

