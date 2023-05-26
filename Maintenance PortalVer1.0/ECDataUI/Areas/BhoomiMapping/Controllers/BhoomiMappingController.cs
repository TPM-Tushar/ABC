using CustomModels.Models.BhoomiMapping;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.BhoomiMapping.Controllers
{
    //[KaveriAuthorization]
    public class BhoomiMappingController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;
        [MenuHighlight]
        public ActionResult BhoomiMappingView()
        {
            try
            {
                KaveriSession.Current.OrderID = 0;

                KaveriSession.Current.DECSROCode = 0;
                int OfficeID = KaveriSession.Current.OfficeID;
                int LevelID = KaveriSession.Current.LevelID;
                long UserID = KaveriSession.Current.UserID;
                caller = new ServiceCaller("BhoomiMappingAPIController");
                BhoomiMappingViewModel reqModel = caller.GetCall<BhoomiMappingViewModel>("BhoomiMappingView", new { officeID = OfficeID, LevelID = LevelID, UserID = UserID });


                return View(reqModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public string Upload(FormCollection formCollection)
        {
            try
            {
                BhoomiMappingUpdateModel bhoomiMappingUpdateModel = new BhoomiMappingUpdateModel();
                bhoomiMappingUpdateModel.SROCode = Int32.Parse(formCollection["SROCode"]);
                string errorMessage = string.Empty;
                bhoomiMappingUpdateModel.FileName = Request.Files["ExcelFile"].FileName;
                bhoomiMappingUpdateModel.Extension = System.IO.Path.GetExtension(Request.Files["ExcelFile"].FileName).ToLower();
                string[] validFileTypes = { ".xls", ".xlsx" };
                string[] validColumnNames = { "KaveriVillageCode", "KaveriSROCode", "BhoomiDistrictCode", "BhoomiTalukCode", "BhoomiHobiCode", "BhoomiVillageCode" };
                DataTable dt = new DataTable();
                DataRow dr = null;
                bhoomiMappingUpdateModel.FullPath = string.Format("{0}/{1}", Server.MapPath("~/Content/BhoomiUploads"), Request.Files["ExcelFile"].FileName);


                if (!Directory.Exists(bhoomiMappingUpdateModel.FullPath))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/BhoomiUploads"));
                }


                if (validFileTypes.Contains(bhoomiMappingUpdateModel.Extension))
                {

                    if (System.IO.File.Exists(bhoomiMappingUpdateModel.FullPath))
                    {
                        System.IO.File.Delete(bhoomiMappingUpdateModel.FullPath);
                    }
                    //Upload Excel File temporarily in "~/Content/BhoomiUploads" folder
                    Request.Files["ExcelFile"].SaveAs(bhoomiMappingUpdateModel.FullPath);

                    //create a new Excel package in a memorystream
                    byte[] bytes = System.IO.File.ReadAllBytes(bhoomiMappingUpdateModel.FullPath);
                    using (MemoryStream stream = new MemoryStream(bytes))
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];

                        //check if the worksheet is completely empty
                        if (worksheet.Dimension == null)
                        {
                            return "Empty_Excel_Error";
                        }

                        int TotalRows = worksheet.Dimension.End.Row;
                        int allowedRows = Int32.Parse(ConfigurationManager.AppSettings["BhoomiMappingAllowedNumRows"]);
                        if (TotalRows > allowedRows)
                        {
                            errormessage = allowedRows.ToString() + ".Row_Exceed_Error";
                            return errormessage;
                        }

                        //Column Name validation
                        for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                        {
                            var excelCell = worksheet.Cells[1, j].Value;
                            if (!validColumnNames.Contains(excelCell))
                            {
                                return "Col_Mismatch_error";
                            }
                            else
                            {
                                switch (excelCell)
                                {
                                    case "KaveriVillageCode":
                                        bhoomiMappingUpdateModel.KaveriVillageCode = j;
                                        dt.Columns.Add(excelCell.ToString(), typeof(int));
                                        break;
                                    case "KaveriSROCode":
                                        bhoomiMappingUpdateModel.KaveriSROCode = j;
                                        dt.Columns.Add(excelCell.ToString(), typeof(int));

                                        break;
                                    case "BhoomiDistrictCode":
                                        bhoomiMappingUpdateModel.BhoomiDistrictCode = j;
                                        dt.Columns.Add(excelCell.ToString(), typeof(int));
                                        break;
                                    case "BhoomiTalukCode":
                                        bhoomiMappingUpdateModel.BhoomiTalukCode = j;
                                        dt.Columns.Add(excelCell.ToString(), typeof(int));
                                        break;
                                    case "BhoomiHobiCode":
                                        bhoomiMappingUpdateModel.BhoomiHobiCode = j;
                                        dt.Columns.Add(excelCell.ToString(), typeof(int));
                                        break;
                                    case "BhoomiVillageCode":
                                        bhoomiMappingUpdateModel.BhoomiVillageCode = j;
                                        dt.Columns.Add(excelCell.ToString(), typeof(int));
                                        break;

                                }
                            }
                        }
                        try
                        {
                            //SRO match validation
                            for (int j = worksheet.Dimension.Start.Row + 1; j <= worksheet.Dimension.End.Row; j++)
                            {
                                if (Int32.Parse(worksheet.Cells[j, bhoomiMappingUpdateModel.KaveriSROCode].Value.ToString()) != bhoomiMappingUpdateModel.SROCode)
                                {
                                    return "SRO_error";
                                }
                            }

                            //Create DataTable from excel to send to DAL

                            for (int j = worksheet.Dimension.Start.Row + 1; j <= worksheet.Dimension.End.Row; j++)
                            {
                                dr = dt.NewRow();
                                dr["KaveriVillageCode"] = worksheet.Cells[j, bhoomiMappingUpdateModel.KaveriVillageCode].Value;
                                dr["KaveriSROCode"] = worksheet.Cells[j, bhoomiMappingUpdateModel.KaveriSROCode].Value;
                                dr["BhoomiDistrictCode"] = worksheet.Cells[j, bhoomiMappingUpdateModel.BhoomiDistrictCode].Value;
                                dr["BhoomiTalukCode"] = worksheet.Cells[j, bhoomiMappingUpdateModel.BhoomiTalukCode].Value;
                                dr["BhoomiHobiCode"] = worksheet.Cells[j, bhoomiMappingUpdateModel.BhoomiHobiCode].Value;
                                dr["BhoomiVillageCode"] = worksheet.Cells[j, bhoomiMappingUpdateModel.BhoomiVillageCode].Value;
                                dt.Rows.Add(dr);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Object reference not set to an instance of an object."))
                            {
                                return "Null_Value_Error";
                            }
                        }
                        bhoomiMappingUpdateModel.ExcelTable = dt;
                    }
                }
                else
                {
                    ViewBag.Error = "Please Select Files in .xls, .xlsx format";

                }
                caller = new ServiceCaller("BhoomiMappingAPIController");
                string resultModel = caller.PostCall<BhoomiMappingUpdateModel, string>("Upload", bhoomiMappingUpdateModel, out errormessage);

                if (resultModel == "Success")
                {
                    System.IO.File.Delete(bhoomiMappingUpdateModel.FullPath);
                    return "Success";
                }
                else
                {
                    return resultModel;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cannot set Column") || ex.Message.Contains("to be null"))
                {
                    return "Null_Value_Error";
                }
                else
                {
                    throw;
                }
            }

        }
    }
}

