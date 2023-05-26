using CustomModels.Models.Common;
using CustomModels.Models.MISReports.OtherDepartmentImport;
using CustomModels.Models.MISReports.TransactionDetails;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using System.Xml.Serialization;
using static ECDataAPI.Common.ApiCommonEnum;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class OtherDepartmentImportDAL : IOtherDepartmentImport
    {
        KaveriEntities dbContext = null;

        public OtherDepartmentImportREQModel OtherDepartmentImportView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                OtherDepartmentImportREQModel resModel = new OtherDepartmentImportREQModel();

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                resModel.SROfficeList = new List<SelectListItem>();
                resModel.DistrictList = new List<SelectListItem>();
                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();

                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

          var ofcDetailsObj=      dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code ,x.LevelID}).FirstOrDefault();
                // commented and changed by shubham bhagat on 16-12-2019 at 10:18 am.
                //string kaveriCode = Kaveri1Code.ToString();

                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    // add anonymous here
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 16-12-2019 at 10:18 am.
                    //string DroCode_string = Convert.ToString(DroCode);
                    //resModel.SROfficeList = new List<SelectListItem>();
                    //resModel.DistrictList = new List<SelectListItem>();
                    // commented and changed by shubham bhagat on 16-12-2019 at 10:18 am.
                    //sroNameItem = new SelectListItem() { Text = SroName, Value = kaveriCode };//GetDefaultSelectListItem(SroName, kaveriCode);
                    //droNameItem = new SelectListItem() { Text = DroName, Value = DroCode_string }; //GetDefaultSelectListItem(DroName, DroCode_string);
                    //resModel.DistrictList.Add(droNameItem);
                    //resModel.SROfficeList.Add(sroNameItem);
                    resModel.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    resModel.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });
                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    // commented and changed by shubham bhagat on 16-12-2019 at 10:18 am.
                    //string DroCode_string = Convert.ToString(Kaveri1Code);                    
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //droNameItem = new SelectListItem() { Text = DroName, Value = DroCode_string };
                    //resModel.DistrictList.Add(droNameItem);
                    resModel.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");
                }
                else
                {
                    //SelectListItem select = new SelectListItem();
                    //select.Text = "All";
                    //select.Value = "0";
                    //SROfficeList.Add(select);
                    resModel.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                    resModel.DistrictList = objCommon.GetDROfficesList("All");
                }

                resModel.ReportList = new List<SelectListItem>();
                resModel.ReportList.Add(new SelectListItem() { Text = "ESwathu Import Report", Value = "eswathu" });
                resModel.ReportList.Add(new SelectListItem() { Text = "UPOR Import Report", Value = "upor" });
                resModel.ReportList.Add(new SelectListItem() { Text = "EAASTHI Import Report", Value = "eaasthi" });
                resModel.ReportList.Add(new SelectListItem() { Text = "Mojini Import Report", Value = "mojini" });
                //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
                resModel.ReportList.Add(new SelectListItem() { Text = "FRUITS", Value = "fruits" });

                return resModel;

                //var SROMasterList = dbContext.SROMaster.OrderBy(c => c.SRONameE).ToList();
                //resModel.SROfficeList.Add(new SelectListItem { Text = "Select", Value = "0" });
                //if (SROMasterList != null)
                //{
                //    if (SROMasterList.Count > 0)
                //    {
                //        foreach (var item in SROMasterList)
                //        {
                //            SelectListItem select = new SelectListItem()
                //            {
                //                Text = item.SRONameE,
                //                Value = item.SROCode.ToString()
                //            };
                //            resModel.SROfficeList.Add(select);
                //        }
                //    }
                //}

                //var DistrictMasterList = dbContext.DistrictMaster.OrderBy(c => c.DistrictNameE).ToList();
                //resModel.DistrictList.Add(new SelectListItem { Text = "Select", Value = "0" });
                //if (DistrictMasterList != null)
                //{
                //    if (DistrictMasterList.Count > 0)
                //    {
                //        foreach (var item in DistrictMasterList)
                //        {
                //            SelectListItem select = new SelectListItem()
                //            {
                //                Text = item.DistrictNameE,
                //                Value = item.DistrictCode.ToString()
                //            };
                //            resModel.DistrictList.Add(select);
                //        }
                //    }
                //}

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public OtherDepartmentImportWrapper OtherDepartmentImportDetails(OtherDepartmentImportREQModel model)
        {
            #region
            OtherDepartmentImportWrapper resModel = new OtherDepartmentImportWrapper();
            try
            {
                dbContext = new KaveriEntities();
                resModel.OtherDepartmentImportDetailList = new List<OtherDepartmentImportDetail>();
                OtherDepartmentImportDetail otherDepartmentImportDetail = null;
                //var result = dbContext.USP_RPT_DocumentReference(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();

                //if (result != null)
                //{
                //    if (result.Count() > 0)
                //    {
                //        int count = 1;

                switch (model.ReportName)
                {

                    //ESWATHU-0 
                    case "eswathu":
                        var result = dbContext.USP_RPT_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, 0).Skip(model.startLen).Take(model.totalNum).ToList();
                         
                        if (result != null)
                        {
                            if (result.Count() > 0)
                            {
                                int count = 1;

                                foreach (var item in result)
                                {
                                    otherDepartmentImportDetail = new OtherDepartmentImportDetail();
                                    otherDepartmentImportDetail.SerialNumber = count++;
                                    otherDepartmentImportDetail.OfficeName = String.IsNullOrEmpty(item.OFFICE_NAME) ? String.Empty : item.OFFICE_NAME;
                                    otherDepartmentImportDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.FINAL_REGISTRATION_NUMBER) ? String.Empty : item.FINAL_REGISTRATION_NUMBER;
                                    otherDepartmentImportDetail.PropertyID = String.IsNullOrEmpty(item.PROPERTY_IDENTIFICATION_NUMBER) ? String.Empty : item.PROPERTY_IDENTIFICATION_NUMBER;
                                    otherDepartmentImportDetail.ImportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.LogId + "','" + item.SROCode + "','eswathu','import')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
                                    otherDepartmentImportDetail.ExportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.LogId + "','" + item.SROCode + "','eswathu','export')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
                                    otherDepartmentImportDetail.ReferenceNumber = String.IsNullOrEmpty(item.REF_NO) ? String.Empty : item.REF_NO;
                                    otherDepartmentImportDetail.UploadDate = item.UPLOAD_DATE == null ? String.Empty : ((DateTime)item.UPLOAD_DATE).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    otherDepartmentImportDetail.DateofRegistration = item.REGISTRATION_DATE == null ? String.Empty : ((DateTime)item.REGISTRATION_DATE).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    otherDepartmentImportDetail.SketchNumber = String.Empty;

                                  resModel.OtherDepartmentImportDetailList.Add(otherDepartmentImportDetail);
                                }
                            }
                        }
                        break;

                    //UPOR-1
                    case "upor":
                        result = dbContext.USP_RPT_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, 1).Skip(model.startLen).Take(model.totalNum).ToList();
                        if (result != null)
                        {
                            if (result.Count() > 0)
                            {
                                int count = 1;

                                foreach (var item in result)
                                {
                                    otherDepartmentImportDetail = new OtherDepartmentImportDetail();
                                    otherDepartmentImportDetail.SerialNumber = count++;
                                    otherDepartmentImportDetail.OfficeName = String.IsNullOrEmpty(item.OFFICE_NAME) ? String.Empty : item.OFFICE_NAME;
                                    otherDepartmentImportDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.FINAL_REGISTRATION_NUMBER) ? String.Empty : item.FINAL_REGISTRATION_NUMBER;
                                    otherDepartmentImportDetail.PropertyID = String.IsNullOrEmpty(item.PROPERTY_IDENTIFICATION_NUMBER) ? String.Empty : item.PROPERTY_IDENTIFICATION_NUMBER;
                                    otherDepartmentImportDetail.ImportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.LogId + "','" + item.SROCode + "','upor','import')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
                                    otherDepartmentImportDetail.ExportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.LogId + "','" + item.SROCode + "','upor','export')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
                                    otherDepartmentImportDetail.ReferenceNumber = String.IsNullOrEmpty(item.REF_NO) ? String.Empty : item.REF_NO;
                                    otherDepartmentImportDetail.UploadDate = item.UPLOAD_DATE == null ? String.Empty : ((DateTime)item.UPLOAD_DATE).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    otherDepartmentImportDetail.DateofRegistration = item.REGISTRATION_DATE == null ? String.Empty : ((DateTime)item.REGISTRATION_DATE).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    otherDepartmentImportDetail.SketchNumber = String.Empty;

                                    resModel.OtherDepartmentImportDetailList.Add(otherDepartmentImportDetail);
                                }
                            }
                        }
                        break;

                    //Eaasthi-2
                    case "eaasthi":
                        result = dbContext.USP_RPT_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, 2).Skip(model.startLen).Take(model.totalNum).ToList();
                        if (result != null)
                        {
                            if (result.Count() > 0)
                            {
                                int count = 1;

                                foreach (var item in result)
                                {
                                    otherDepartmentImportDetail = new OtherDepartmentImportDetail();
                                    otherDepartmentImportDetail.SerialNumber = count++;
                                    otherDepartmentImportDetail.OfficeName = String.IsNullOrEmpty(item.OFFICE_NAME) ? String.Empty : item.OFFICE_NAME;
                                    otherDepartmentImportDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.FINAL_REGISTRATION_NUMBER) ? String.Empty : item.FINAL_REGISTRATION_NUMBER;
                                    otherDepartmentImportDetail.PropertyID = String.IsNullOrEmpty(item.PROPERTY_IDENTIFICATION_NUMBER) ? String.Empty : item.PROPERTY_IDENTIFICATION_NUMBER;
                                    otherDepartmentImportDetail.ImportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.LogId + "','" + item.SROCode + "','eaasthi','import')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
                                    otherDepartmentImportDetail.ExportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.LogId + "','" + item.SROCode + "','eaasthi','export')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
                                    otherDepartmentImportDetail.ReferenceNumber = String.IsNullOrEmpty(item.REF_NO) ? String.Empty : item.REF_NO;
                                    otherDepartmentImportDetail.UploadDate = item.UPLOAD_DATE == null ? String.Empty : ((DateTime)item.UPLOAD_DATE).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    otherDepartmentImportDetail.DateofRegistration = item.REGISTRATION_DATE == null ? String.Empty : ((DateTime)item.REGISTRATION_DATE).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    otherDepartmentImportDetail.SketchNumber = String.Empty;

                                    resModel.OtherDepartmentImportDetailList.Add(otherDepartmentImportDetail);
                                }
                            }
                        }
                        break;

                    case "mojini":
                        var mojiniresult = dbContext.USP_RPT_Mojini_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                        if (mojiniresult != null)
                        {
                            if (mojiniresult.Count() > 0)
                            {
                                int count = 1;

                                foreach (var item in mojiniresult)
                                {
                                    otherDepartmentImportDetail = new OtherDepartmentImportDetail();
                                    otherDepartmentImportDetail.SerialNumber = count++;
                                    otherDepartmentImportDetail.OfficeName = String.IsNullOrEmpty(item.OFFICE_NAME) ? String.Empty : item.OFFICE_NAME;
                                    otherDepartmentImportDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.FINAL_REGISTRATION_NUMBER) ? String.Empty : item.FINAL_REGISTRATION_NUMBER;
                                    otherDepartmentImportDetail.SketchNumber = String.IsNullOrEmpty(item.SKETCH_NUMBER) ? String.Empty : item.SKETCH_NUMBER;
                                    otherDepartmentImportDetail.ImportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.LogId + "','" + item.SROCode + "','mojini','import')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
                                    otherDepartmentImportDetail.ExportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.LogId + "','" + item.SROCode + "','mojini','export')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
                                    otherDepartmentImportDetail.ReferenceNumber = String.IsNullOrEmpty(item.REF_NO) ? String.Empty : item.REF_NO;
                                    otherDepartmentImportDetail.UploadDate = item.UPLOAD_DATE == null ? String.Empty : ((DateTime)item.UPLOAD_DATE).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    otherDepartmentImportDetail.DateofRegistration = item.REGISTRATION_DATE == null ? String.Empty : ((DateTime)item.REGISTRATION_DATE).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    otherDepartmentImportDetail.PropertyID = String.Empty;

                                    resModel.OtherDepartmentImportDetailList.Add(otherDepartmentImportDetail);
                                }
                            }
                        }
                        break;
                    //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
                    case "fruits":
                        //var mojiniresult = dbContext.USP_RPT_Mojini_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                        //var FRUITSData = (from FruitsRecv in dbContext.FRUITS_DATA_RECV_DETAILS
                        //                  where (FruitsRecv.KAIGREG_InsertDateTime > model.DateTime_FromDate &&
                        //                  FruitsRecv.KAIGREG_InsertDateTime < model.DateTime_ToDate)
                        //                  select new { FruitsRecv }).ToList();
                        var FRUITSData = dbContext.USP_RPT_FRUITS_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                        if (FRUITSData != null)
                        {
                            if (FRUITSData.Count() > 0)
                            {
                                int count = 1;

                                foreach (var item in FRUITSData)
                                {
                                    otherDepartmentImportDetail = new OtherDepartmentImportDetail();
                                    otherDepartmentImportDetail.SerialNumber = count++;
                                    otherDepartmentImportDetail.OfficeName = String.IsNullOrEmpty(item.SRONameE) ? "--" : item.SRONameE;
                                    otherDepartmentImportDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.FinalRegistrationNumber) ? "--" : item.FinalRegistrationNumber;
                                    otherDepartmentImportDetail.SketchNumber = String.IsNullOrEmpty(item.ReferenceNo) ? "--" : item.ReferenceNo;
                                    otherDepartmentImportDetail.PropertyID = String.IsNullOrEmpty(Convert.ToString(item.DocumentStatusCode)) ? "No Action Performed" : Convert.ToString((FruitsResponseCodes)item.DocumentStatusCode);
                                    //otherDepartmentImportDetail.ImportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.RequestID + "','" + item.SROCode + "','fruits','import')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
                                    otherDepartmentImportDetail.ImportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.RequestID + "','" + item.SROCode + "','fruits','import')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download</button>";
                                    if (item.DocumentStatusCode==(int)ApiCommonEnum.FruitsResponseCodes.Filed || item.DocumentStatusCode==(int)ApiCommonEnum.FruitsResponseCodes.Registered)
                                    {
                                        otherDepartmentImportDetail.BtnViewSummary = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=ViewTransXMLFun('" + item.RequestID + "','" + item.SROCode + "','fruits','import')><i style='padding-right:3%;' class='fa fa-eye'></i>View</button>"; 
                                    }
                                    else
                                    {
                                        otherDepartmentImportDetail.BtnViewSummary = "--";
                                    }
                                    otherDepartmentImportDetail.ArticleNameE = item.ArticleNameE ?? "--";
                                    if(string.IsNullOrEmpty(item.ArticleNameE))
                                    {
                                        TransactionDetails transactionDetails= FromXmlString(item.TransXML);
                                        if (transactionDetails.BankDetails.TransactionType == TransactionDetailsBankDetailsTransactionType.M)
                                            otherDepartmentImportDetail.ArticleNameE = "Mortgage without Possession";
                                        if (transactionDetails.BankDetails.TransactionType == TransactionDetailsBankDetailsTransactionType.R)
                                            otherDepartmentImportDetail.ArticleNameE = "Release of mortgage right";
                                    }
                                    if (Convert.ToBoolean(item.FormIIIExists))
                                    {
                                        //otherDepartmentImportDetail.ReferenceNumber = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=FormIIIDownloadFun('" + item.RequestID + "','" + item.SROCode + "','fruits','import')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                                        otherDepartmentImportDetail.ReferenceNumber = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=FormIIIDownloadFun('" + item.RequestID + "','" + item.SROCode + "','fruits','import')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download</button>";
                                    }
                                    else
                                    {
                                        otherDepartmentImportDetail.ReferenceNumber = "N.A";
                                    }
                                    if (Convert.ToBoolean(item.IsResponseUploaded))
                                    {
                                        //otherDepartmentImportDetail.ExportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.LogId + "','" + item.SROCode + "','fruits','export')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
                                        otherDepartmentImportDetail.ExportedXML = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.LogId + "','" + item.SROCode + "','fruits','export')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download</button>";
                                    }
                                    else
                                    {
                                        otherDepartmentImportDetail.ExportedXML = "--";
                                    }
                                    //otherDepartmentImportDetail.ReferenceNumber = String.IsNullOrEmpty(Convert.ToString(item.DocumentStatusCode)) ? "--" : Convert.ToString((FruitsResponseCodes)item.DocumentStatusCode);
                                    otherDepartmentImportDetail.UploadDate = item.UploadDateTime == null ? "--" : ((DateTime)item.UploadDateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    otherDepartmentImportDetail.DateofRegistration = item.Stamp5DateTime == null ? "--" : ((DateTime)item.Stamp5DateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    otherDepartmentImportDetail.ActionDate = item.ActionDateTime == null ? "--" : ((DateTime)item.ActionDateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    //otherDepartmentImportDetail.PropertyID = String.Empty;
                                    //Added by mayank on 05/01/2022
                                    otherDepartmentImportDetail.KaigrRegInsertDate= (item.KAIGREG_InsertDateTime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    //End of mayank comment on 05/01/2022
                                    resModel.OtherDepartmentImportDetailList.Add(otherDepartmentImportDetail);
                                }
                            }
                        }
                        break;

                }
                //        }
                //    }
                //}

                resModel.SelectedSRO = model.SROfficeID == 0 ? "All" : dbContext.SROMaster.Where(x => x.SROCode == model.SROfficeID).Select(x => x.SRONameE).FirstOrDefault();
                resModel.SelectedDRO = model.DistrictID == 0 ? "All" : dbContext.DistrictMaster.Where(x => x.DistrictCode == model.DistrictID).Select(x => x.DistrictNameE).FirstOrDefault();

                return resModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            //    Value = "eswathu"
            //    lue = "upor" });
            //     Value = "eaasthi"
            //    Value = "mojani" }
            //    if(model.ReportName.Equals("eswathu"))
            //if(model.ReportName.Equals("upor"))
            //if(model.ReportName.Equals("eswathu"))
            //if(model.ReportName.Equals("eswathu"))

            //"<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.GSCNo + "','" + item.SROCode + "','" + item.SId + "','" + item.SROName + "')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download as XML</button>";
            //switch (model.ReportName)
            //{

            //    //ESWATHU-0 
            //    case "eswathu":
            //        resModel.OtherDepartmentImportDetailList = new List<OtherDepartmentImportDetail>() {
            //                new OtherDepartmentImportDetail(){
            //                    SerialNumber=1,DateofRegistration="rgre",FinalRegistrationNumber="fgf",ImportedXML="fghdfgh",
            //                    OfficeName="regre",PropertyID="fghbfgdh",ReferenceNumber="hrth",UploadDate="ggrthtr"
            //                },
            //                new OtherDepartmentImportDetail{
            //                     SerialNumber=2,DateofRegistration="rgre",FinalRegistrationNumber="fgf",ImportedXML="fghdfgh",
            //                    OfficeName="regre",PropertyID="fghbfgdh",ReferenceNumber="hrth",UploadDate="ggrthtr"
            //                }
            //        };
            //        break;
            //    //UPOR-1
            //    case "upor":
            //        resModel.OtherDepartmentImportDetailList = new List<OtherDepartmentImportDetail>() {
            //                new OtherDepartmentImportDetail(){
            //                    SerialNumber=1,DateofRegistration="rgre",FinalRegistrationNumber="fgf",ImportedXML="fghdfgh",
            //                    OfficeName="regre",PropertyID="fghbfgdh",ReferenceNumber="hrth",UploadDate="ggrthtr"
            //                },
            //                new OtherDepartmentImportDetail{
            //                     SerialNumber=2,DateofRegistration="rgre",FinalRegistrationNumber="fgf",ImportedXML="fghdfgh",
            //                    OfficeName="regre",PropertyID="fghbfgdh",ReferenceNumber="hrth",UploadDate="ggrthtr"
            //                }
            //        };
            //        break;

            //    //Eaasthi-2
            //    case "eaasthi":
            //        resModel.OtherDepartmentImportDetailList = new List<OtherDepartmentImportDetail>() {
            //                new OtherDepartmentImportDetail(){
            //                    SerialNumber=1,DateofRegistration="rgre",FinalRegistrationNumber="fgf",ImportedXML="fghdfgh",
            //                    OfficeName="regre",PropertyID="fghbfgdh",ReferenceNumber="hrth",UploadDate="ggrthtr"
            //                },
            //                new OtherDepartmentImportDetail{
            //                     SerialNumber=2,DateofRegistration="rgre",FinalRegistrationNumber="fgf",ImportedXML="fghdfgh",
            //                    OfficeName="regre",PropertyID="fghbfgdh",ReferenceNumber="hrth",UploadDate="ggrthtr"
            //                }
            //        };
            //        break;

            //    case "mojani":
            //        resModel.OtherDepartmentImportDetailList = new List<OtherDepartmentImportDetail>() {
            //                new OtherDepartmentImportDetail(){
            //                    SerialNumber=1,DateofRegistration="rgre",FinalRegistrationNumber="fgf",ImportedXML="fghdfgh",
            //                    OfficeName="regre",PropertyID="fghbfgdh",ReferenceNumber="hrth",UploadDate="ggrthtr"
            //                },
            //                new OtherDepartmentImportDetail{
            //                     SerialNumber=2,DateofRegistration="rgre",FinalRegistrationNumber="fgf",ImportedXML="fghdfgh",
            //                    OfficeName="regre",PropertyID="fghbfgdh",ReferenceNumber="hrth",UploadDate="ggrthtr"
            //                }
            //        };
            //        break;  

            //}

            //return resModel;
            //resModel.OtherDepartmentImportDetailList = new List<OtherDepartmentImportDetail>() {
            //    new OtherDepartmentImportDetail(){
            //        SerialNumber=1,DateofRegistration="rgre",FinalRegistrationNumber="fgf",ImportedXML="fghdfgh",
            //        OfficeName="regre",PropertyID="fghbfgdh",ReferenceNumber="hrth",UploadDate="ggrthtr"
            //    },
            //    new OtherDepartmentImportDetail{
            //         SerialNumber=2,DateofRegistration="rgre",FinalRegistrationNumber="fgf",ImportedXML="fghdfgh",
            //        OfficeName="regre",PropertyID="fghbfgdh",ReferenceNumber="hrth",UploadDate="ggrthtr"
            //    }
            //};
            //return resModel;
            #endregion


        }

        public int OtherDepartmentImportCount(OtherDepartmentImportREQModel model)
        {
            //int count = 0;
            //switch (model.ReportName)
            //{
            //    case "eswathu": count = 2; break;
            //    case "upor": count = 2; break;
            //    case "eaasthi": count = 2; break;
            //    case "mojani": count = 2; break;
            //}
            //return count;
            try
            {
                dbContext = new KaveriEntities();
                // = null;
                //USP_RPT_Mojini_IMPORT_STATUS_Result mojinistatusResult = null;

                switch (model.ReportName)
                {
                    //ESWATHU-0 
                    case "eswathu":
                        var statusResult = dbContext.USP_RPT_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, 0).ToList();
                        if (statusResult != null)
                        {
                            return statusResult.Count();
                        }
                        break;

                    //UPOR-1
                    case "upor":
                        statusResult = dbContext.USP_RPT_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, 1).ToList();
                        if (statusResult != null)
                        {
                            return statusResult.Count();
                        }
                        break;

                    //Eaasthi-2
                    case "eaasthi":
                        statusResult = dbContext.USP_RPT_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, 2).ToList();
                        if (statusResult != null)
                        {
                            return statusResult.Count();
                        }
                        break;

                    case "mojini":
                        var result = dbContext.USP_RPT_Mojini_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
                        if (result != null)
                        {
                            return result.Count();
                        }
                        break;
                    //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
                    case "fruits":
                        var fruitsresult = dbContext.USP_RPT_FRUITS_IMPORT_STATUS(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
                        if (fruitsresult != null)
                        {
                            return fruitsresult.Count();
                        }
                        break;
                }

                //if (result != null)
                //{
                //    return result.Count();
                //}

                return 0;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public SelectListItem GetDefaultSelectListItem(string sTextValue, string sOptionValue)
        {
            return new SelectListItem
            {
                Text = sTextValue,
                Value = sOptionValue,
            };
        }

        public XMLResModel GetXMLContent(OtherDepartmentImportREQModel InputModel)
        {
            try
            {
                XMLResModel XMLResModel = new XMLResModel();
                dbContext = new KaveriEntities();
                //StringBuilder XMLContent = null;
                //String XMLContent = null;

                switch (InputModel.ReportName)
                {
                    //ESWATHU-0  RDPRd
                    case "eswathu":
                        if (InputModel.XMLType.Equals("import"))
                        {
                            //XMLContent = new StringBuilder(dbContext.RDPRXML.Where(i => i.LogId == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.OwnerDetails).FirstOrDefault());
                            XMLResModel.XMLString = dbContext.RDPRXML.Where(i => i.LogId == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.OwnerDetails).FirstOrDefault();
                        }
                        else if (InputModel.XMLType.Equals("export"))
                        {
                            XMLResModel.XMLString = dbContext.RDPRXML.Where(i => i.LogId == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.SentXMLDetails).FirstOrDefault();
                        }
                        break;

                    //UPOR-1 UPOR
                    case "upor":
                        if (InputModel.XMLType.Equals("import"))
                        {
                            XMLResModel.XMLString = dbContext.UPORXML.Where(i => i.LogId == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.OwnerDetails).FirstOrDefault();
                        }
                        else if (InputModel.XMLType.Equals("export"))
                        {
                            XMLResModel.XMLString = dbContext.UPORXML.Where(i => i.LogId == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.SentXMLDetails).FirstOrDefault();
                        }
                        break;

                    //Eaasthi-2 LBPASXML
                    case "eaasthi":
                        if (InputModel.XMLType.Equals("import"))
                        {
                            XMLResModel.XMLString = dbContext.LBPASXML.Where(i => i.LogId == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.OwnerDetails).FirstOrDefault();
                        }
                        else if (InputModel.XMLType.Equals("export"))
                        {
                            XMLResModel.XMLString = dbContext.LBPASXML.Where(i => i.LogId == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.SentXMLDetails).FirstOrDefault();
                        }
                        break;

                    // MojaniXML
                    case "mojini":
                        if (InputModel.XMLType.Equals("import"))
                        {
                            XMLResModel.XMLString = dbContext.MojaniXML.Where(i => i.LogId == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.ReceivedXMLDetails).FirstOrDefault();
                        }
                        else if (InputModel.XMLType.Equals("export"))
                        {
                            XMLResModel.XMLString = dbContext.MojaniXML.Where(i => i.LogId == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.SentXMLDetails).FirstOrDefault();
                        }
                        break;
                    //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
                    // FRUITSXML
                    case "fruits":
                        if (InputModel.XMLType.Equals("import"))
                        {
                            XMLResModel.XMLString = dbContext.FRUITS_DATA_RECV_DETAILS.Where(i => i.RequestID == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.TransXML).FirstOrDefault();
                        }
                        else if (InputModel.XMLType.Equals("export"))
                        {
                            XMLResModel.XMLString = dbContext.FRUITSXML.Where(i => i.LogId == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.SentXMLDetails).FirstOrDefault();
                        }
                        break;
                }
                //String XMLContent = dbContext.SAKALA_UploadFileDetails.Where(i => i.SID == InputModel.SId && i.SROCode == InputModel.SROCode && i.GSCNo == InputModel.GSCNo).Select(i => i.InputDataset).FirstOrDefault();
                //XMLResModel.XMLString = XMLContent;
                if (String.IsNullOrEmpty(XMLResModel.XMLString))
                    XMLResModel.XMLString = String.Empty;
                return XMLResModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration

        public XMLResModel FormIIIDownloadFun(OtherDepartmentImportREQModel InputModel)
        {
            try
            {
                XMLResModel XMLResModel = new XMLResModel();
                dbContext = new KaveriEntities();
                //StringBuilder XMLContent = null;
                //String XMLContent = null;

                switch (InputModel.ReportName)
                {

                    //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
                    // FRUITSXML
                    case "fruits":
                        if (InputModel.XMLType.Equals("import"))
                        {
                            XMLResModel.PDFString = dbContext.FRUITS_DATA_RECV_DETAILS.Where(i => i.RequestID == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.FormIIIData).FirstOrDefault();
                        }

                        break;
                }
                //String XMLContent = dbContext.SAKALA_UploadFileDetails.Where(i => i.SID == InputModel.SId && i.SROCode == InputModel.SROCode && i.GSCNo == InputModel.GSCNo).Select(i => i.InputDataset).FirstOrDefault();
                //XMLResModel.XMLString = XMLContent;
                if (String.IsNullOrEmpty(XMLResModel.PDFString))
                    XMLResModel.PDFString = String.Empty;
                else
                {
                    XMLResModel.PDFbyte = Convert.FromBase64String(XMLResModel.PDFString);
                }
                return XMLResModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public XMLResModel ViewTransXMLFun(OtherDepartmentImportREQModel InputModel)
        //{
        //    try
        //    {
        //        string xmldata = string.Empty;
        //        string sqlQuery = string.Empty;
        //        XMLResModel XMLResModel = new XMLResModel();
        //        dbContext = new KaveriEntities();
        //        //variables declared
        //        #region all declared variables

        //        string BHOOMIdistrictcode = string.Empty;
        //        string talukacode = string.Empty;
        //        string hoblicode = string.Empty;
        //        string hoblicodenotbhoomicode = string.Empty;

        //        string BhoomidistrictName = string.Empty;
        //        string BhoomitalukaName = string.Empty;
        //        string Bhoomihobliname = string.Empty;
        //        string BhoomiVillageName = string.Empty;
        //        string Kaverihobliname = string.Empty;
        //        string KaveriVillageName = string.Empty;
        //        //string relationshipName = string.Empty;
        //        //List<OwnerDetails> ownerList = new List<OwnerDetails>();
        //        #endregion
        //        //sqlQuery = @"select TOP 1 * from KAVERI.FRUITS_DATA_RECV_DETAILS where ReferenceNo = '" + FRUITSDetails.rowNum + "' ";
        //        //SqlDataReader reader = SqlObj.ExecuteReader(sqlQuery, CommandType.Text, null);
        //        //if (reader.HasRows)
        //        //{
        //        //DataTable objDtb = new DataTable();
        //        //objDtb.Load(reader);
        //        //foreach (DataRow row in objDtb.Rows)
        //        //{
        //        //// form 3 available and amount is less than or equal to 300000
        //        //string SroCode = Convert.ToString(row["SROCode"]);
        //        //string strXML = Convert.ToString(row["TransXML"]);
        //        string doubleQuoteStart = "\"";
        //        string doubleQuoteEnd = "\"";
        //        string dash = "-";

        //        //if (strXML.ToLower().Contains("“"))
        //        //{
        //        //    strXML = Regex.Replace(strXML, "“", doubleQuoteStart);
        //        //}

        //        //if (strXML.ToLower().Contains("”"))
        //        //{
        //        //    strXML = Regex.Replace(strXML, "”", doubleQuoteEnd);
        //        //}

        //        //if (strXML.ToLower().Contains("–"))
        //        //{
        //        //    strXML = Regex.Replace(strXML, "–", dash);
        //        //}

        //        //string IsfocConverted = IsFocConverter.GetIsfoc(strXML);
        //        //if (IsfocConverted.Contains("ISFOCCONVERROR"))
        //        //{
        //        //    string InvalidCharacters = getInvalidCharacterXML(row["TransXML"].ToString());
        //        //    MessageBox.Show(InvalidCharacters, "XML Conversion Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        //    return;
        //        //}
        //        //xmldata = Convert.ToString(row["TransXML"]);
        //        //xmldata = IsfocConverted;
        //        //xmldata = row["TransXML"].ToString();
        //        //XmlDocument xdoc = new XmlDocument();
        //        ////xdoc.LoadXml(xmldata);
        //        //TransactionDetails transactionDetails = FRUITSDetails.FromXmlString(xmldata);
        //        ////
        //        //TransactionDetails transactionDetails = FRUITSDetails.FromXmlString(xmldata);
        //        XMLResModel.XMLString = dbContext.FRUITS_DATA_RECV_DETAILS.Where(i => i.RequestID == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i => i.TransXML).FirstOrDefault();

        //        TransactionDetails transactionDetails = FromXmlString(XMLResModel.XMLString);



        //        string TempFileDirectory = @"C:\KAVERI-FRUITS_FORMIII";
        //        string TempFileDirectoryKarnatakaLogo = @"C:\KAVERI-FRUITS_FORMIII\Karnataka-Logo.png";
        //        if (!Directory.Exists(TempFileDirectory))
        //        {
        //            DirectoryInfo DI = Directory.CreateDirectory(TempFileDirectory);
        //            DI.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        //        }
        //        string KarnatakaLogo = "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";
        //        if (!File.Exists(@"C:\KAVERI-FRUITS_FORMIII\Karnataka-Logo.png"))
        //        {
        //            byte[] bytes = Convert.FromBase64String(KarnatakaLogo);
        //            File.WriteAllBytes(TempFileDirectoryKarnatakaLogo, bytes);
        //        }

        //        StringWriter stringwriter = new StringWriter(); // string writer for HTML text writer
        //        HtmlTextWriter writer = new HtmlTextWriter(stringwriter); // HTML text writer for creating HTML tags
        //                                                                  //Commented by mayank on 14-01-2021
        //                                                                  //StringWriter stringwriter = new StringWriter(); // string writer for HTML text writer
        //                                                                  //HtmlTextWriter writer = new HtmlTextWriter(stringwriter); // HTML text writer for creating HTML tags
        //                                                                  //writer.Write("<style>@media print { .noprint { visibility: hidden; }  </style>");
        //                                                                  //writer.Write("<button onclick='window.print()' class = 'noprint'>Print</button>");
        //                                                                  //table

        //        writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
        //        writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Border, "1");
        //        writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Bordercolor, "Black");
        //        writer.RenderBeginTag(HtmlTextWriterTag.Table);


        //        //writer.RenderBeginTag(HtmlTextWriterTag.Table);
        //        //T Body
        //        #region commneted by mayank
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");


        //        ////heading row
        //        //writer.RenderBeginTag(HtmlTextWriterTag.Tr);

        //        ////cell
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "5");
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Width, "878");

        //        //writer.RenderBeginTag(HtmlTextWriterTag.Td);
        //        //writer.RenderBeginTag(HtmlTextWriterTag.Br);
        //        //writer.RenderEndTag();
        //        ////para
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.P);

        //        ////strong start
        //        //writer.RenderBeginTag(HtmlTextWriterTag.Strong);

        //        ////image
        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Src, "data:image/gif;base64,R0lGODlhaQBoAPf/AGYaGa8DC6cJDrEHDcIABakNCMYBAKESDKESE80AAJkXFZMbGaEUGbQOD5YaJbUQB80AHJ0cHpAiJLYSF5UiIbcUEbwVA4wnKIMtLIkrKLIcF84TALgYJX80NbwbG7UgH74eF84WF70eI80UMHZANJkwMIw2OMEhH/4AL3ZCQOUWAHxAP7omI7snKcUmFMgoA7srHLYsKOYZFsQnKHRKSrwqM7gwMOUaMMszAMIvLsksK9ApLs4vGr40M8QyNr44LtQtMM0xNMU3K7o+KMM5N+UyA8M6PcwzSsg8Kb8+PM0xZb8/QuYzGcc9OrJMMM48PcQ+T+Q0NMpAQspFLLNLTcZEQNJFNv8vNJplMsFOQ+BCL8tNL8ZMSs1LTchPUptlZ9dNTM1PYsdVU+RNNMBaW+dLTLJmTtRXSMtZXbNmZeVNY9VZWctmMtNfXc5lTNdhUc9jY8tmY89nWcZpaM5pa8xngNVoaNlmaOZlSsxnmNNxYORoaMl0dbF/f+Vmf9lxaNxvbsd/V9F3aMJ7fNhydNR1dNZzf/9kZ85+ZORzZ85+fNl9a+lzddx9e81/muR/Z9eEcdmBgth/ksyJiOV+f/F6fMuNft+EesiNidiHheZ/mNmLfOCIiNqKjtaNjs2ZZ8ySlP9/fc2ZgMuajcyZmOeOjuSaY96UlduZhMuasdmbi+WZfN2amOiWktqemuSZmumYm/6YZeaYp+Oas+Oik+ifn8utrP+Xl96lpsuyl+Omot+kuNyvmc+ysOCtnv+XyeWqq/Cmptays96wreexl/Gsq9+4oOeyseOyveWyyvywl/Gzr87Lluy2tum7ueK+vv6ysuPAtM7MsPS3ucrMyf6yzO3AwfzEkfbAv+vFxvu+wObMsObIx+PMyf7Nl+bM4/HLzPfKzf7Nse/Rz/nRuf/LyuXY2f/M5PTW1P/L//bfteTlzv3X1//X5vTe3+Tm4/bi5P/k1f/i4P7mzvTn5/Xq3f3m4v/l/fLz8P/x8vz39f/+zf/+4f/7+f7//P///yH/C01TT0ZGSUNFOS4wFwAAAAttc09QTVNPRkZJQ0U5LjBCPKT1ACH/C01TT0ZGSUNFOS4wGAAAAAxjbVBQSkNtcDA3MTIAAAADSABzvAAh+QQBCgD/ACwAAAAAaQBoAAAI/wD/CRxIsKDBgwgT/gvAsKFDhwojSpxIsaLChwEeaNzIkeNDiyBDijzIkOOEBg9YEGjRIseVHDBbvvhgoUKDAQ8+jtzJ02BDjRM+bCFz61a5cv6O+lvar9++fuHKUSsaC42HChlzBujJNWSErAFEvPFUzt5Spkz7nV2rdq0/eOWG3YHRoCGDDl3zIqSQsYANOMTKuj0rz225Y2u7wWPrrx/cYYR+DGhQIILey/8kDChQQESjbvgaN1b7zuzSV2j93QpiWh6dZqKXmm3a2J4uIQEGBIhAAzPPCwI4e4CEdPDSXnu6LUVkeumqELD9veLRS61aeIqOlYPX9i2rHg8aNP+g4FvkgfAeBMmzbrwfsSBxkIqqtvbRFEX+5HnZYq5tv2Z0vEIJKYvF5o8oOehWgATlUcTXABp0gY1oanVzjHZn9UOOIpRQYg8ss6y1Bx50fLgHGcUtNYg48jRDSmGp9WMPJBNkhECDETGQExHcFLhUN7AM8sore5jmmCLwwELJLfidRUcoirwCBzl0pChPF8R00w+MZ812HRwFBCDACjgatEJwBcSxXnf+KMLFMfbIo0OK/dAhD3Z7cLGWF7f0sgUs/MDB5StuvALLMe+sBQ8l22TozylYTWACAGUKZEIBD0yAi3H+wNLFnc3wgJiT4vhDzx5ONEdHOO+4cScdZ/H/wwUx9oTTTKJMNRPEI4OhE8MDAxBRqQQFDNACOWs191YXsCi5B3NnKQLNUuXkUlhTWvajnDyj+jNPF86ss2aXg+yxx4/rjZZFmJY1uIBNVXBpTzPdLlWLG86Uc84jqC3FzTlpceqWY/TmQolyTMEibRz5IYkWP3MUK0B5COhGR3cfDoJwl25wl1STAoeMFjzcvBKdP9100Y04sNYyCJuNLUNZAZhRMMAAkKxVMizKlYOrWpTAViFjA4vMZoH9cNiPOHFwQ8fG9mw8TY035nUBZZLws9YrXZSj7SPcrBWXo8bZY8882hTDSivFYDOPsmx198pi5bhBRyvdyaOIlo2F/4OTAl1hgBPDhsWxiou8mGUk3Gd1wwscRnAwQZi53QTsBC0kUQguG7s1WzhblOhoM4N0W4huC/S0AkpecNrNHnRw05TJ/rklTyZc1KhBC010YUchriiDCyqd0NGFFC1kpIEYrMDdlt58i90FHaPmY4dGPYkQQAv6dBkwNG6UdczTxk0jhgYBaFDDIsMols+W8sSZrtkW6tEDQxPIkeJauCq6hyLDUIRp4GEEhuxEAekbh2jeQQm38GMVdIDFK8LmFmxwYQAT6AIntvENflhHWx0iRYe6oTXr8OMbzdhEFSajv6Ilq0Pv0Na1/DGOFhRAAyJxQG6w4R96vMlzgyCGx/8IQ4eTdAFOtDkLPJrhhiDwoA9dCAEdVmEOd8QIHrNoQgA8kAnGLWUdlKAD0tzijBodACSCe0ApBjO+4lhHHlpbSyZOcgZccAdmQOrCC3LAAzx4wQo+GAEiYEG2pcCDFTYICyviuBZQHAxmZ8nHK1BnkR4UIAnd8ZGAQmYPPQTgA7jwYHsU0cQQRCEIMsiBG7pwBFOobDDWsccpJjAAPdzDLcngUnv0IYYH0IwiGchNOGJ1DHodJTucKocPAsCFzhmHC1sIQhCYwANT8mAQoCBHEEjRHtF0QwgPCMP+RMYUdHxgAKmbiAYGUIi18KMbzejQKgbRBec5Q3udSJfA+kH/iWk6MQobiAIEOuHII+7zSwHIwTjJ2Q9O6GYimmlAPoh2mFesIj5uGYYANMAKchqyEdScgxo20IUccCEHW2iGj0SWiQZ4ABgeXYs8clAAwEWkAgXohAuXco959Ggtz5iACHgYU5Q94Zpb4EEXeBCFBDwCkjv1xzA+0AAKFlUXOYkIBToD1Qyxh1oaaIA2ivojHYTAiTLQAQ+YkIB+eZQ2uBhAEha6z3ws4QFnRAgJcFqLLh3jFRhqz0wb0FGyvmUPGzBlEU75giDUq6iNaIAVvMipfjjDgAjhwvbWAgtE7IESe7CqW7oQALeKbBvd2Bg0nuCFUPBgDJQIARwKlFoS/4Ysib28A1nzYQS8IkQDBRhG4waxjahEAhELjUQAuhDTbYjiGL3ohuLqoIdj6CAOzehCK5YiD1I0Ixev6B/RlGiEAWyKoUtpRo0OEkwRrCU7TdnSIxooUw34gLKDgYR3e6GIdTRGEVvg0CwUkYNGoUwUmvCFI6jxVn1owAPOXEo+wKELXWDDY/lIwgMYYBCaXmItkEBMfG9BOEN24QHDNJo/3kGHWbjBC13wb8p4MMVIRFEU9uiHn0ZACWlQo6sD80QAxMDIpdTDF67YhCc6EYwCtSJMBUlBRuiRRGIMojmNoO9SWjGA1r2VGpRIxhZOScI48EAG0eTBE0IQ4H4Mov8MW6BELnrx1sbEwAMnW4oqLmGMetCjG7WYkD/YMbkLEOSCPvBclMphoSplqAYPQIfATmiPd3CHG8foBhWUQYl3bAMEZw61DELAhCXIYxjQEMUsuMHNqFWarmd5RgCg0Jx+8EEWE23MMXQxUX1UoQFVE0gOAsCJgVkIGqQA7Fq08QA+dPUevojgOuDh6XeE4wPbgIdrZHBUUrtABnS4xWv6AQ0vOMIf3OhGOXrBDUXgQbwD+zUho4WMtTSjGBPtByweMLGBcEbSsXnjT7vkgwl0rz33MLMjPOgMaeg7AYrY0h60UAYkhEAGFkcuKY7Rj1skYAsxVITPzOHECK8FHQH/MMJaOvGKduSjHuAAxTPOwg6sEASD46WQvQuwBpGNT2is6gYdyiA0WLwWAkyQwZm3UAZE0G0PUyDFO1TaD3cUSpcD68cPIHwWa5TCE6PYRCOAQY9I9pY8/1hAA/RU1IgJOmSkEMcd+xEHUqTYH84YAw9GwIQRnBkGIUDEaJboBdnlpxuiYKgnGtCkfkzYF5PoBDTgNocBVI2mfZUNMUpzoW3A2wcVMBAsu+GGDhGjGa/wQgyXwo9jrCIIpAZBFECggm1urB9weAUxBDQIbpJTHgMYgrLyYY+DpwYYUP7HBwoAcDc/gh7DGAQl3NA5e0wADeRk4n4iMQUdgGLF73gH/zHo0MSAMvXMIfBCdrph6W5wgQdBoIR+H8upGlhAxIVkCzsK0AAyAatLVIAU3hUOjnYWDmUNRgMP9PQIs7AFXUBc0WUOdJAFTRACPKADe9AFG/AEZRACe7Bff3VWe3AMdgMrRmMIDWCCnJIPuSYPIvAAGIABYcEU5QArMtQm+wN6HsUNVhAEsEAKtUAHQSAKPWIPcgAGYRAFdGAuXVAGQQANdDAGLwJP+xEfiKBWJdZN/QAOD+ACs+EP+cAO5YANu+YJx/A++kAEA+AAdBAAS9AW5QAHSKEW9iBGSuQBRgBVlWYazrABW+AI2dFE4wAP3UAoXPAKeBAHlDAFQdAIQf9ADMfAA17gDPQAD6DwCHIwDFvAR1q2h+0RAC6QIuBQCpTwB3LACHEwCJLWD1zwAAowBw+AfYYkIJnGDaugZVIVi57zDrZIB4+QKPCAUo4gCsOACM00IHEwPc7QRF2wCl3wBEGACOIQBM3YC9tACnuwCq+gCHHABcXBDY/AIeLVFh8gAgbmD+jQDMOgCptAhqBAQYQQAAiQBQEACMkyfoowCMi1FqRQAWSQSZSgCL2wChCwAamwFGHUDeLSDaQACXtDBxAQOhe1BvDQCiFACVzwCOknDt0ACV3wItQmdAgDCRtQBoPQC4zTAyfgC47SD+YgDO7QDZMwc/7ACQUgAEn/UACZ4Cj8QC/DoEAZoggNsJNMoTdeQA+skAMbECLv8ArR0w/vsArCEA5x8AIuAALi0AuicA/d0Ajy4EQQEASsMgiwEEPWAQt05g9uAAFlQA/NcHtL4QUscAsDQw/YwA7ogAs0eQq+9Cu0UFmDQQcFAFNrsQ2KQA9WEAJjUBjbUB3+EDWGxAutEEVBAAF9wkHSwDNORE2U0AsKJCOl8Ra9UBiRYIHtYwuMMQctgIsSBg8siA5l5w+4cJM2xJIxhQcDkA2KAkEaGQJ0KQ+i4F9bkmyZtg1MdAPwVwZ6QAnUwAtB4AY5QAdKhwjtcwywsArUACP0AhVb4IeDAAnK0g+Z/6ABhNMdtHGe/vAMOUEXzjBekKQFAwCUs0gJ1cQJiwELfRBH8EAHIZADnUAKw5ALrgVqKhCdeEBqz7kHqNALCvN+PPB9hqQxePcEGzAFr4B1rPABKgiY6BYABQACD9Ce+TcYOdAACDgyrxACeCAP79ANEkpDr8dmzeAidOAMXRAEYyADqMQDORAK/UQJxOAIw0A6YyCjOeYPpKAIPeIMIDAFyRA3kfABWQiY3MAQMVAA9FdZpHVeZ0EPTOc1udAN4yAjx+AFU+CB0bAUuQABh+IM0LBUoAUdWNQK+kga0FCZcaYY9OAMw4A3lJADOeMWdNACvMJQ6lkAyYMLQOYWcf/QAJ6gKKTwAr4wL8TAelwTBG9WC6axDooYm3SwBd0ADSDgCmbBkKiBLUvYRHCQeP1Qgz0CTc4kBjawCjE1DB5qAwVQbDFFBxVgB9JDB6SgMIvRD+vQjM5gbRrTFr0ArPTSBV4QT1NAB0BKCqSQIoMwDPNQiKhyHccwCc0AB3KQlmdhAy1gWiGDC2KyBPVYZ4MAAsx1FuEAB9SQCqTANzpGCXHUD5DwDWfxDqSwH1zAZorwBNEETbGTr51wCqIRDmaweu5BraKAMNZRjnR5UI3RCfxGBg8ABzH1Hw/QAzLVDNLADblQHArYDdyQKMeQC7fUOEsFezSmVkHgBernFt3/0F/3kGmiwGAE0wsbxyX9oA8PMAP1MmEA5yhzEAAHQAZuyCmMUw4zMAFHg7Ki0ECtWqMn2Qxe0A33wA/wEA2N1gVLpQI3GlBT0HvdkAy+EDb8oAj7SgeK8IONoYDJtlL9kA0BwAPVtwmtkGt7MgAKcAcP8AHGAUZwAw9bUAAnyhT38A6wYIKYVg4fSQcWqgm5UAuIUK8X4gzT1AVRwANlIArNwA1OswVk6YwFlnvdQAlIJAqDCDOF8AE8UCD4ACRgcAatABqRVAUBIAE0wBDGhzLyxwOU8Aq1hggaahzb+A7OsHG6IgrkR35pMAh0oDHDgFrUKwploAgQFF6R+Jx0/yCEEIAK0roNlGAFxnuYxhEDLfCujncMZxACIfAGzRAa3AUDAYAB/xAmR4tuQkBqPOAHunQMPvABX7UUx+AMoOALgyAKYosIq0RcijDB1WsFprsOpIAIW8AN5KADmIoIZQBN1xpNXcAFLgAJdRCOEkQKcINyLgAyg5YIPMADicAO3YEOE/AAZBIDA6ALbDEIBvC542QPOpCbjcMz61AOimBlYIQIrMALAyJ0iMANzSAKsNcFAAIBISBBcbABXLAN5VAL1PgKW9AKlNAIeJCkrKCPw1AogeUPnSC7neMOpZBlsBCb6ZV8XFAAaMAmAEYJXTAPA9MFGtBOK0ZgdCAN0P+gCF3QQPCwDetRg6QABrywFNugCfuRA9PUn12gBzS2GPIwCCQyAttgD/DAKs7ZBW4AYFPQyGfRA1LAdl3KDvDgDu5gv0tRCPIoEDJIuPZWynAyGKsgAh+gD/9xo9e0B1twkdSmFtxAn70ADYXxDtBACVtgBaB2CFuQVmPWNY3xDuLQCCEACdP8Hy8Af9LJo3EQQ9xQAFLgVueZREzBWwWQTgsRae0BM/0wD0IwAV20BzzKBHEAe0HwCuPQanGABBtcIZ3QCNUUBU3UCJBACR3IBAWdJY0BDTwKG8nWC49gcV7ABFzABVZAZ0vwA0GAdXFzFvGAEgThAwOgZYvaFOX/IAITQA+koAPhkA7MQAre0Kps4DU5TZd1Rwq3gANskA7eEAR08AlCEL5CiAI4YAC3QA1xwAawcgtBHYc6oBbXUNX9wNT+0AwDgAMVO6JrwQoeShCkNQSLOhhBMACEUA5B8Al2txTUYACwQgolsE10nQCf4A9B8ATUkNOkEASgQK05jQM6kACHAApBkABxsA90MAIl0NM4EB8/Bh9sEAf6YARDEARFBQ9SsMs3NwBjRE63IAID4A7UAAp0EAdxQAdsAAd17dO3QAp0kADSFAc6cAW4kAOkQNlNTQqHAKwD3dRBgANXkAdVUoPSFL5M/QlxQJeuMAEz8AsdCw/rZGgE/0FT9vhWatGoXvBBXtMW6UAhpMAGQeAN/kAHOMDUoFAOJFYO1KrV0uTeuy0EnwBJ+2Ag9CACHrCh5ISxW2EQBeDLIWPfZ7EPMzAARAlJ9m3U1Aofq0EH1FDYpGDXuk0KGU7XpDDQQXAI8FF3qeEWXKABOABrxsEPVVAZB1Ejmccph33WSUEAA/BY3UEKOHAxqkEHOpDZ8hxwasHeTY0U5ZAAL+B7oucPhYDd2l1UyPcACCEFDxADyRQEOh2YHjABQMkmHacDt9CqanHYbIDWS6HljZEO1MAM0gRr6DoDPh5T9qBF/RbjAfBY5SDbbHAIdj1OdNAALIDH/BgO1GDoGf9+2AyG5uLGDIdeDulg34vuFnjrAWdOVsZwE/qLELzrAWyi20jxCXlw2Bi1FOEQBA2QBH6bFvGcFJDUVfEVMIMxDh8QA2JpWEtQAMGGEJShsF6VFHFAG+UQDt1B10HRv4YFSzEFDBPAASte7ORUDBqBFwqxBvy36ktR3QEjz/sQBBMwAVma7E0+SlsUBPsz0KVuHPLAAgHAYRLB7oInNiUAa3DIBpZElOK+W2LQACfABne3FFp9CLJdrYPRCbpBJhOREysVFfuEFnBAAA0gBit14vm+FtlgAyo+54J6C90+0Oe4z8CVA5QyETaUBDknNsDqFpF6ToWA7RW/FuPgBQ//0AIvkAcHfBa6DaxO0bJL0QZZZRFhskYCQ2JXHeyFIwL8lwnB+/LuACYfwAIFeBZuNN4myCac4KFoZxG+hOzUMtBjHgQ2niGrYdMiEAnymeyW1YYtcAJgPxgMniHlMCcVRBnuDhIFdNOGEW66LW4Lfgs6YNPbUwjW8NZnoQ/HAAfo8wFsfwvDBDME7yh2rTMtMAB3HhIsUAAzsPRtweP70w/UsFBubgAesHwfIAZ0kAms4AzjUIngcAysEAndyBAtIALx7XtsglEY3k2y0QSc0RPn9ASccuoDE9uPvxYEyAZ/DwNPXyM40RATUAMtEAQzkANwkO4DM9th7d451w+F/wAs3s0Tm0EGsMQGTA7w/b0axK7PSSFuQlgCOvACMyAEQZADnW0U/200cBALTN32xuHzAZD1APFP4ECCBQ3+W/MgQCl/DR2SKufQX7ggtxreouOvn8ONEjuGC1dOZESNHCWW8yaxYbkgcPx5ixOHpEN1EwIoOJhTJ8EHAxip7OgQThw6pPyRylhyZsOgHTc+Lcn0YQKjKuPkYBq04bQJBQ7sBJvzgcJBKiXuoyMyTpCKDlkujWpWZblwHOnQiZPSIxuLZp0NeCAg7GCDBRTu0CeXlNF+/dJKvKWj6i24IqXe+hSTskRScMopSRm0HDWzewoUEEFYdcEAAz7Qk+uWjf/Ku0EaxjHLViK1W78+Ne2HlGWQuiYl6vPS4EGP1c0H2pwQDOhD3G6xlGMjMqnbODpmdqzOOQidW6S8x85nQ2EX5+3/sQgQYI5HpuFIO7yFmxTv8A3phMvMLJlUImW2hqi5z6Ni4mvAPQcNC8AGbcxqTCK+/CnnkDj6Wsk7jMwizyNqbMsKqDni+8BBFf8B7IEkEovNnzzKaYyNIJaiQ4cvbPTIsf76CSlGRmwK4IEVVzwtgAb20Cq2u6yqK50BITPQOLNq+SA+DY7k8r3WKuCDwhL3OYlDZvrzZx+45LKlhSID6DLOfzQ4rYKyYsQTwzzNmsYGCOUE9J8JFKpgic2DrIytSTxv4QIEhQpgIdBAixzAsAm4uIYkRWPctJxAPFAoPjglJfWf1kRtoAdGQLFsz5VGqyQJ5QIoIIAWSsWVIEoHaKCAASzw4AQvxAhkElBIAQWTScTwYgYPBhU1vlpzpbagAhqoND6FHkiyVmkVGkDb+KolF6xafR0r1DcNK9eggAAAOw==");
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Src, "data:image/png;base64,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");
        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Src, TempFileDirectoryKarnatakaLogo);
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Height, "104");
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Width, "105");
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Height, "105");
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Alt, "govt-kar");


        //        //writer.RenderBeginTag(HtmlTextWriterTag.Img);
        //        //writer.RenderEndTag();

        //        ////strong end
        //        //writer.RenderEndTag();

        //        ////para end
        //        ////writer.RenderEndTag();

        //        ////para
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.P);

        //        ////text
        //        //writer.Write("<br/><b>Government of Karnataka</b><br/>");

        //        ////para end
        //        ////writer.RenderEndTag();

        //        ////para
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.P);

        //        ////text
        //        //writer.Write("<b>Department of Stamps and Registration</b><br/>");

        //        ////para end
        //        ////writer.RenderEndTag();



        //        ////para
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Align, "Left");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.P);
        //        //writer.RenderEndTag();

        //        //writer.RenderEndTag();

        //        ////text
        //        ////writer.Write("<b>Print Date:"+DateTime.Now+"</b>");


        //        ////row end
        //        ////writer.RenderEndTag();
        //        //writer.RenderEndTag();

        //        ////heading row
        //        //writer.RenderBeginTag(HtmlTextWriterTag.Tr);

        //        //writer.RenderBeginTag(HtmlTextWriterTag.Td);
        //        ////text
        //        ////writer.Write("<div style='max-width:1200px; margin:auto; background:white; padding:10px;' ><b></t>Print Date:" + DateTime.Now + "<p style ='display:inline; margin-left:9em;'> Ref : KAVERI - FRUITS Integration</b></p></div>");
        //        //writer.Write("<div style='max-width:1200px; margin:auto; background:white; padding:10px;' ><b></t>Print Date:" + DateTime.Now.ToString() + "<p style ='display:inline; margin-left:19em;'> Ref : KAVERI - FRUITS Integration</b></p></div>");
        //        //writer.RenderEndTag();
        //        //writer.RenderEndTag();
        //        //writer.Write("<hr>"); 
        //        #endregion
        //        //commented by mayank on 15-01-2021
        //        //writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
        //        //writer.Write(TransactionType=="M"? "Mortgage Deed": "Release Deed");
        //        //writer.RenderEndTag();
        //        //start
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-family: KNB-TTUmaEN;");

        //        writer.RenderBeginTag(HtmlTextWriterTag.Table);
        //        //writer.AddStyleAttribute(HtmlTextWriterStyle.FontStyle, "KNB-TTUmaEN");
        //        writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");


        //        //heading row
        //        writer.RenderBeginTag(HtmlTextWriterTag.Tr);

        //        //cell
        //        writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "5");
        //        writer.AddAttribute(HtmlTextWriterAttribute.Width, "878");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "x-large");

        //        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        //        //writer.RenderBeginTag(HtmlTextWriterTag.Br);
        //        //writer.RenderEndTag();
        //        //para
        //        writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
        //        //writer.Write("Mortgage Deed");
        //        writer.Write("<b>" + (transactionDetails.BankDetails.TransactionType == TransactionDetailsBankDetailsTransactionType.M ? "Mortgage Deed" : "Release Deed") + "</b>");
        //        writer.RenderEndTag(); writer.RenderEndTag(); writer.RenderEndTag();
        //        //end
        //        //heading
        //        int LandParcelCount = 0;
        //        foreach (TransactionDetailsBhoomi_TransactionDetails item in transactionDetails.bhoomi_LandParcelDetails)
        //        {
        //            //string sqlQuery1 = string.Empty;
        //            //DataTable XMLDetailstbl1 = new DataTable();
        //            ////sqlQuery1 = @"select top 1 DistrictNameE from kaveri.DistrictMaster where BhoomiDistrictCode = " + Convert.ToInt32(item.LandParcelDetails.BHOOMIdistrictcode) + " ";
        //            //sqlQuery1 = @"select * from kaveri.DistrictMaster where DistrictCode = (select DistrictCode from kaveri.SROMaster where SROCode=" + SroCode + ")";
        //            //SqlDataReader reader1 = SqlObj.ExecuteReader(sqlQuery1, CommandType.Text, null);
        //            //if (reader1.HasRows)
        //            //{
        //            //    XMLDetailstbl1.Load(reader1);

        //            //    BhoomidistrictName = (XMLDetailstbl1.Rows[0]["DistrictNameE"].ToString() + "(" + item.LandParcelDetails.BHOOMIdistrictcode + ")");
        //            //}
        //            //else
        //            //{
        //            //    BhoomidistrictName = "-";
        //            //}
        //            string DistrictName = (from district in dbContext.DistrictMaster
        //                                   where district.DistrictCode ==
        //                                   (dbContext.SROMaster.Where(m => m.SROCode == InputModel.XMLSROCODE).Select(m => m.DistrictCode).FirstOrDefault())
        //                                   select district.DistrictNameE).FirstOrDefault();

        //            //var Villagedetails = (from village in dbContext.bhoo
        //            //                      join hobli in dbContext.HobliMaster
        //            //                      on village.HobliCode equals hobli.HobliCode
        //            //                      where village.bh
        //            //can we refer BhoomiData

        //            //var VillageDetails= (from BhoomiMapping in dbContext.bho)

        //            //reader1.Close();

        //            //string sqlQuery2 = string.Empty;
        //            //DataTable XMLDetailstbl2 = new DataTable();
        //            //string bhoomitalukcode = string.Empty;
        //            //BHOOMIdistrictcode = Convert.ToString(item.LandParcelDetails.BHOOMIdistrictcode).Length == 2 ? Convert.ToString(item.LandParcelDetails.BHOOMIdistrictcode) : "0" + Convert.ToString(item.LandParcelDetails.BHOOMIdistrictcode);
        //            //bhoomitalukcode = Convert.ToString(item.LandParcelDetails.talukacode).Length == 2 ? BHOOMIdistrictcode + Convert.ToString(item.LandParcelDetails.talukacode) : BHOOMIdistrictcode + "0" + Convert.ToString(item.LandParcelDetails.talukacode);
        //            //sqlQuery2 = @"select top 1 TalukaName from kaveri.BhoomiTaluk where TalukaCode = '" + bhoomitalukcode + "'";
        //            //SqlDataReader reader2 = SqlObj.ExecuteReader(sqlQuery2, CommandType.Text, null);
        //            //if (reader2.HasRows)
        //            //{
        //            //    XMLDetailstbl2.Load(reader2);

        //            //    BhoomitalukaName = XMLDetailstbl2.Rows[0]["TalukaName"].ToString();
        //            //}
        //            //else
        //            //{

        //            //}
        //            //reader2.Close();

        //            string sqlQuery4 = string.Empty;
        //            //DataTable XMLDetailstbl4 = new DataTable();

        //            sqlQuery4 = @"select VM.VillageNameE,HM.HobliNameE,vm.BhoomiVillageName,hm.BhoomiHobliName,hm.BhoomiTalukName from kaveri.VillageMaster as VM,kaveri.HobliMaster as HM where vm.HobliCode = hm.HobliCode and vm.BhoomiDistrictCode = " + item.LandParcelDetails.BHOOMIdistrictcode + " and vm.BhoomiTalukCode = " + item.LandParcelDetails.talukacode + " and vm.BhoomiVillageCode = " + item.LandParcelDetails.villagecode + " and hm.BhoomiHobliCode = " + item.LandParcelDetails.hoblicode;
        //            //SqlDataReader reader4 = SqlObj.ExecuteReader(sqlQuery4, CommandType.Text, null);
        //            //if (reader4.HasRows)
        //            //{
        //            //    XMLDetailstbl4.Load(reader4);

        //            //    BhoomiVillageName = IsFocConverter.GetUnicode(XMLDetailstbl4.Rows[0]["BhoomiVillageName"].ToString());
        //            //    Bhoomihobliname = IsFocConverter.GetUnicode(XMLDetailstbl4.Rows[0]["BhoomiHobliName"].ToString());

        //            //    KaveriVillageName = IsFocConverter.GetUnicode(XMLDetailstbl4.Rows[0]["VillageNameE"].ToString());
        //            //    Kaverihobliname = IsFocConverter.GetUnicode(XMLDetailstbl4.Rows[0]["HobliNameE"].ToString());
        //            //}
        //            //else
        //            //{
        //            //    BhoomiVillageName = "-";
        //            //}
        //            //reader4.Close();

        //            writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#E6E3E6");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "left");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "4px");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "large");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "black");
        //            writer.RenderBeginTag(HtmlTextWriterTag.H4);
        //            writer.Write("<b>Land Parcel Details  " + ++LandParcelCount + "  of  " + transactionDetails.bhoomi_LandParcelDetails.Count() + "</b>");
        //            writer.RenderEndTag();
        //            //div 
        //            //Commented by mayank on 14-01-2021
        //            //writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#ebdfed");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "2px");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "5px");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginRight, "5px");
        //            //added by mayank font change
        //            //writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-family: KNB-TTUmaEN;");
        //            //writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-size: 20px;");

        //            writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-family:KNB-TTUmaEN; font-size:20px;");
        //            //writer.AddAttribute(HtmlTextWriterAttribute.Style, "");
        //            writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //            writer.Write("Bhoomi District : ");
        //            writer.Write(BhoomidistrictName);
        //            //writer.Write(item.LandParcelDetails.BHOOMIdistrictcode);
        //            writer.Write(",&nbsp;Bhoomi Taluk : ");
        //            writer.Write(BhoomitalukaName);
        //            //writer.Write(item.LandParcelDetails.talukacode);
        //            writer.Write("<br/>"); //break line
        //            writer.Write("Bhoomi Hobli : ");
        //            writer.Write(Bhoomihobliname);
        //            //writer.Write(item.LandParcelDetails.hoblicode);
        //            writer.Write(",&nbsp;Bhoomi Village : ");
        //            writer.Write(BhoomiVillageName);
        //            //writer.Write("<br/>");
        //            writer.Write("<br/>("); //break line
        //            writer.Write(" Kaveri Hobli : ");
        //            writer.Write(Kaverihobliname);
        //            //writer.Write(item.LandParcelDetails.hoblicode);
        //            writer.Write(",&nbsp; Kaveri Village : ");
        //            writer.Write(KaveriVillageName + " )");
        //            //writer.Write(item.LandParcelDetails.villagecode);
        //            writer.Write("<br/>"); //break line
        //            writer.Write("Land Code : ");
        //            //writer.Write(landcode);
        //            writer.Write(item.LandParcelDetails.landcode);
        //            //writer.Write(",&nbsp; Survey no.: ");
        //            //writer.Write(surveynumber);
        //            //writer.Write(",&nbsp; Hissa Number : ");
        //            //writer.Write(hissanumber);
        //            writer.Write("<br/>Property No. : ");
        //            writer.Write(item.LandParcelDetails.surveynumber + item.LandParcelDetails.surveynumbercharacter + item.LandParcelDetails.hissanumber);
        //            writer.Write("<br/>(Survey No. :" + item.LandParcelDetails.surveynumber + ",&nbsp;Survey No. Character :" + item.LandParcelDetails.surveynumbercharacter + ",&nbsp;Hissa No. : " + item.LandParcelDetails.hissanumber + "  )");
        //            writer.Write("<br/>"); //break line
        //            writer.Write("Acre : ");
        //            writer.Write(item.LandParcelDetails.transextents.ext_acre);
        //            writer.Write(",&nbsp; Gunta : ");
        //            writer.Write(item.LandParcelDetails.transextents.ext_gunta);
        //            writer.Write(",&nbsp; Gunta F.: ");
        //            writer.Write(item.LandParcelDetails.transextents.ext_fgunta);
        //            writer.Write("<br/>"); //break line
        //            writer.Write("Consideration Amount (in Rs): ");
        //            writer.Write(transactionDetails.BankDetails.considerationamount);
        //            writer.Write("<br/>"); //break line
        //            writer.RenderEndTag();
        //            //Owner details
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#E6E3E6");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "left");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "4px");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "large");
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "black");
        //            writer.RenderBeginTag(HtmlTextWriterTag.H4);
        //            writer.Write("<b>BHOOMI Owner Details : " + (transactionDetails.BankDetails.TransactionType == TransactionDetailsBankDetailsTransactionType.M ? " [Executant]" : " [Claimant]</b>"));
        //            writer.RenderEndTag();
        //            //writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-family:KNB-TTUmaEN; font-size:20px;");
        //            //writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-size: 15px;");

        //            writer.RenderBeginTag(HtmlTextWriterTag.Div);

        //            //for (int i = 0; i < ownerList.Count; i++)
        //            int i = 0;
        //            foreach (TransactionDetailsBhoomi_TransactionDetailsOwnerdetails ownerdetails in item.bhoomi_survey_owners)
        //            {
        //                //commented by mayank on 14-01-2021
        //                //writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#ebdfed");
        //                writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "2px");
        //                writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "5px");
        //                writer.AddStyleAttribute(HtmlTextWriterStyle.MarginRight, "5px");
        //                writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "large");
        //                writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //                string countofOwner = "Owner Details " + Convert.ToString(++i) + " of &nbsp;" + Convert.ToString(item.bhoomi_survey_owners.Count());
        //                writer.AddStyleAttribute(HtmlTextWriterStyle.FontStyle, "italic");
        //                writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "bold");
        //                // span tag
        //                writer.RenderBeginTag(HtmlTextWriterTag.Span);
        //                writer.Write(countofOwner);
        //                writer.RenderEndTag();
        //                writer.Write("<br/>"); //break line
        //                                       //second inner div
        //                writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "5px");
        //                writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-family: KNB-TTUmaEN;font-size:20px;");

        //                writer.RenderBeginTag(HtmlTextWriterTag.Div);

        //                writer.Write("Owner Name : ");
        //                writer.Write(ownerdetails.name);
        //                //writer.Write("<label style='font-family:KNB-TTUmaEN;'>" + ownerList[i].name + "</label>");
        //                writer.Write("<br/>"); //break line
        //                writer.Write("Address : ");
        //                writer.Write(ownerdetails.address);
        //                writer.Write("<br/>"); //break line
        //                writer.Write("Mobile No.: ");
        //                writer.Write(ownerdetails.mobilenumber);
        //                writer.Write("<br/>"); //break line
        //                                       //writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-family: KNB-TTUmaEN;");

        //                //writer.RenderBeginTag(HtmlTextWriterTag.);
        //                writer.Write("Relationship : ");
        //                //writer.Write("<label style='font-family:KNB-TTUmaEN;'>" + ownerList[i].relationship + "</label>");
        //                writer.Write(ownerdetails.relationship);
        //                //writer.RenderEndTag();

        //                writer.Write("<br/>"); //break line
        //                writer.Write("Relative Name : ");
        //                writer.Write(ownerdetails.relativename);
        //                writer.Write("<br/>"); //break line
        //                writer.Write("Owner No.:");
        //                writer.Write(ownerdetails.ownerno);
        //                writer.Write("<br/>"); //break line
        //                writer.Write("Main Owner No.:");
        //                writer.Write(ownerdetails.mainownerno);
        //                writer.Write("<br/>"); //break line
        //                writer.Write("Sex : ");
        //                writer.Write(ownerdetails.sex);
        //                writer.Write("<br/>"); //break line
        //                writer.Write("Age : ");
        //                writer.Write(ownerdetails.age);
        //                writer.Write("<br/>"); //break line
        //                writer.Write("EPIC : ");
        //                writer.Write(ownerdetails.EPIC);
        //                writer.Write("<br/>"); //break line
        //                writer.Write("PAN : ");
        //                writer.Write(ownerdetails.PAN);
        //                writer.Write("<br/>"); //break line
        //                writer.Write("<br/>"); //break line
        //                writer.Write("<br/>"); //break line
        //                writer.RenderEndTag();
        //                writer.RenderEndTag();

        //            }
        //            writer.RenderEndTag();
        //        }
        //        //


        //        //heading
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#E6E3E6");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "4px");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "large");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "black");
        //        writer.RenderBeginTag(HtmlTextWriterTag.H4);
        //        writer.Write("<b>Bank Details : " + (transactionDetails.BankDetails.TransactionType == TransactionDetailsBankDetailsTransactionType.M ? " [Claimant]" : " [Executant]</b>"));
        //        writer.RenderEndTag();
        //        //div
        //        //Commented by mayank on 14-01-2021
        //        //writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#ebdfed");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "2px");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "5px");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.MarginRight, "5px");
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-family: KNB-TTUmaEN;");
        //        writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-family:KNB-TTUmaEN; font-size:20px;");
        //        //writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-size: 15px;");

        //        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        writer.Write("Bank Name : ");
        //        writer.Write(transactionDetails.BankDetails.nameofbank);
        //        writer.Write(",&nbsp; Branch Name : ");
        //        writer.Write(transactionDetails.BankDetails.branchname);
        //        writer.Write("<br/>"); //break line
        //        writer.Write("Branch Address : ");
        //        writer.Write(transactionDetails.BankDetails.branchaddress);
        //        writer.Write("<br/>"); //break line
        //        writer.Write("Branch Mobile No. : ");
        //        writer.Write(transactionDetails.BankDetails.branchMobilenumber);
        //        writer.Write("<br/>"); //break line
        //        writer.Write("Branch EPIC : ");
        //        writer.Write(transactionDetails.BankDetails.branchEPIC);
        //        writer.Write("<br/>"); //break line
        //        writer.Write("Branch PAN : ");
        //        writer.Write(transactionDetails.BankDetails.branchPAN);
        //        writer.Write("<br/>"); //break line
        //                               //writer.Write("Type Of Transaction : ");
        //                               //writer.Write(TransactionType);
        //                               //writer.Write(",&nbsp; Amount : ");
        //                               //writer.Write(considerationamount);
        //        writer.RenderEndTag();

        //        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        writer.Write("<br/><br/><p><label style='color:red'>Note : </label> Above application details are recieved from FRUITS system and displayed as it is.</p>");
        //        writer.RenderEndTag();
        //        //webBrowser2.DocumentText = stringwriter.ToString();
        //        ////webBrowser2.DocumentText = IsFocConverter.GetUnicode(stringwriter.ToString());
        //        //this.ShowDialog();
        //        //}

        //        //}
        //        XMLResModel.HTMLString = stringwriter.ToString();
        //        XMLResModel.TransactionDetails = transactionDetails;
        //        return XMLResModel;

        //    }
        //    //catch (Exception ex)
        //    //{
        //    //    objCommon.WriteErrorLog(ex, "FRUITSFilingViewDetails");
        //    //    MessageBox.Show("Unable to show details " + ex.Message, "FRUITSFilingViewDetails_0001", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //}
        //    //finally
        //    //{
        //    //    SqlObj.Close();
        //    //}
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        public XMLResModel ViewTransXMLFun(OtherDepartmentImportREQModel InputModel)
        {
            try
            {
                string xmldata = string.Empty;
                string sqlQuery = string.Empty;
                XMLResModel XMLResModel = new XMLResModel();
                dbContext = new KaveriEntities();
              
                string doubleQuoteStart = "\"";
                string doubleQuoteEnd = "\"";
                string dash = "-";

               var FRUITSDetails= dbContext.FRUITS_DATA_RECV_DETAILS.Where(i => i.RequestID == InputModel.XMLLogID && i.SROCode == InputModel.XMLSROCODE).Select(i =>new { i.DocumentID, i.TransXML ,i.SROCode,i.ReferenceNo}).FirstOrDefault();
                XMLResModel.XMLString = FRUITSDetails.TransXML;
                TransactionDetails transactionDetails = FromXmlString(XMLResModel.XMLString);
                bool isRelationShipApplication = false;
                XMLResModel.TransactionDetails = transactionDetails;
                transactionDetails.ReferenceNo = FRUITSDetails.ReferenceNo;
                #region Old code for XML iteration
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(XMLResModel.XMLString);
                foreach (XmlNode node in xdoc.DocumentElement.ChildNodes)
                {
                    ////fetch data from xml node by node
                    #region fetch data from xml node by node
                    if (node.Name.ToLower() == "bhoomi_landparceldetails")
                    {
                        foreach (XmlNode locnode in node.ChildNodes)
                        {
                            if (locnode.Name.ToLower() == "bhoomi_transactiondetails")
                            {
                                foreach (XmlNode Sublocnode in locnode.ChildNodes)
                                {
                                    if (Sublocnode.Name.ToLower() == "bhoomi_survey_owners")
                                    {
                                        foreach (XmlNode innerlocnode in Sublocnode.ChildNodes)
                                        {
                                            if (innerlocnode.Name.ToLower() == "ownerdetails")
                                            {
                                                foreach (XmlNode innermostnode in innerlocnode.ChildNodes)
                                                {
                                                    if (innermostnode.Name.ToLower() == "relationship")
                                                    {
                                                        isRelationShipApplication = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                #endregion
                if (!isRelationShipApplication)
                {                            //writer.RenderBeginTag(HtmlTextWriterTag.);
                    //writer.Write("Bincom : ");
                    ////writer.Write("<label style='font-family:KNB-TTUmaEN;'>" + ownerList[i].relationship + "</label>");
                    ////writer.Write(ownerdetails.relationship);
                    ////Added or changed by mayank on 28/10/2021 for bincom field and relationship change
                    //writer.Write(ownerdetails.Bincom);
                    ////writer.RenderEndTag();
                    ///
                    foreach (TransactionDetailsBhoomi_TransactionDetails item in transactionDetails.bhoomi_LandParcelDetails)
                    {
                        foreach (TransactionDetailsBhoomi_TransactionDetailsOwnerdetails ownerdetails in item.bhoomi_survey_owners)
                        {
                           
                            ownerdetails.Relationship =Convert.ToString(ownerdetails.Bincom);
                            ownerdetails.IsRelationShip = false;

                        }
                    }
                }
                else
                {
                    //int indexOfTransactionDetails = Array.IndexOf(transactionDetails.bhoomi_LandParcelDetails, item);
                    //int indexOfTransactionDetails = Array.IndexOf(transactionDetails.bhoomi_LandParcelDetails, transactionDetails.);
                    //int indexOfOwnerDetails = Array.IndexOf(item.bhoomi_survey_owners, ownerdetails);
                    //string relationship = xdoc.DocumentElement.
                    //ChildNodes[1]. //bhoomi_landparceldetails
                    //ChildNodes[indexOfTransactionDetails].//bhoomi_transactiondetails
                    //ChildNodes[1].//bhoomi_survey_owners
                    //ChildNodes[indexOfOwnerDetails]//owner details
                    //["relationship"].InnerText;//relationship
                    //writer.Write("Relationship : ");
                    ////writer.Write("<label style='font-family:KNB-TTUmaEN;'>" + ownerList[i].relationship + "</label>");
                    ////writer.Write(ownerdetails.relationship);
                    ////Added or changed by mayank on 28/10/2021 for bincom field and relationship change
                    //writer.Write(relationship);
                    //writer.RenderEndTag();
                    //List<TransactionDetailsBhoomi_TransactionDetailsOwnerdetails[]> ownerdetail=  transactionDetails.bhoomi_LandParcelDetails.Select(m => m.bhoomi_survey_owners).ToList();

                    foreach (TransactionDetailsBhoomi_TransactionDetails item in transactionDetails.bhoomi_LandParcelDetails)
                    {
                        foreach (TransactionDetailsBhoomi_TransactionDetailsOwnerdetails ownerdetails in item.bhoomi_survey_owners)
                        {
                            int indexOfTransactionDetails = Array.IndexOf(transactionDetails.bhoomi_LandParcelDetails, item);
                            int indexOfOwnerDetails = Array.IndexOf(item.bhoomi_survey_owners, ownerdetails);
                            string relationship = xdoc.DocumentElement.
                            ChildNodes[1]. //bhoomi_landparceldetails
                            ChildNodes[indexOfTransactionDetails].//bhoomi_transactiondetails
                            ChildNodes[1].//bhoomi_survey_owners
                            ChildNodes[indexOfOwnerDetails]//owner details
                            ["relationship"].InnerText;//relationship
                            ownerdetails.Relationship = relationship;
                            ownerdetails.IsRelationShip = true;
                        }
                    }
                }
                    if (FRUITSDetails != null)
                    {
                        foreach (var item in transactionDetails.bhoomi_LandParcelDetails)
                        {
                            USP_GET_VILLAGE_FROM_DOCUMENTID_FRUITS_Result Jurisdiction = dbContext.USP_GET_VILLAGE_FROM_DOCUMENTID_FRUITS(FRUITSDetails.DocumentID, FRUITSDetails.SROCode, item.LandParcelDetails.landcode).FirstOrDefault();
                            if (Jurisdiction != null)
                            {
                                item.LandParcelDetails.KaveriDistrictName = Jurisdiction.DistrictNameE;
                                item.LandParcelDetails.KaveriTalukName = Jurisdiction.TalukNameE;
                                item.LandParcelDetails.KaveriHobliName = Jurisdiction.HobliNameE;
                                item.LandParcelDetails.KaveriVillageName = Jurisdiction.VillageNameE;
                            }
                        }
                    }
                    return XMLResModel;

            }
            catch(Exception)
            {
                throw;
            }
        }

        public static TransactionDetails FromXmlString(string xmlString)
        {
            try
            {
                var reader = new StringReader(xmlString);
                var serializer = new XmlSerializer(typeof(TransactionDetails));
                var instance = (TransactionDetails)serializer.Deserialize(reader);
                return instance;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}