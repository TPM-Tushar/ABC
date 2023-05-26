using CustomModels.Models.MISReports.DocumentScanAndDeliveryReport;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class DocumentScanAndDeliveryDAL : IDocumentScanAndDelivery
    {
        KaveriEntities dbContext = null;

        public DocumentScanAndDeliveryREQModel DocumentScanAndDeliveryView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                DocumentScanAndDeliveryREQModel resModel = new DocumentScanAndDeliveryREQModel();

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                resModel.SROfficeList = new List<SelectListItem>();
                resModel.DistrictList = new List<SelectListItem>();

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                SelectListItem ItemAll = new SelectListItem() { Text = "All", Value = "0" };
                //SelectListItem droNameItem = new SelectListItem();
                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                //string kaveriCode = Kaveri1Code.ToString();

                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();

                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(DroCode);
                    //resModel.SROfficeList = new List<SelectListItem>();
                    //resModel.DistrictList = new List<SelectListItem>();
                    //sroNameItem = GetDefaultSelectListItem(SroName, ofcDetailsObj.kaveriCode);
                    //droNameItem = GetDefaultSelectListItem(DroName, DroCode_string);
                    //resModel.DistrictList.Add(droNameItem);
                    //resModel.SROfficeList.Add(sroNameItem);
                    resModel.DistrictList.Add(ItemAll);
                    resModel.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });
                    resModel.IsSr = true;
                    resModel.IsDr = false;
                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(ofcDetailsObj.Kaveri1Code);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //resModel.DistrictList.Add(droNameItem);
                    //resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");

                    resModel.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");
                    //resModel.SROfficeList.Add(ItemAll);
                    resModel.IsSr = false;
                    resModel.IsDr = true;


                }
                else
                {
                    //SelectListItem select = new SelectListItem();
                    //select.Text = "All";
                    //select.Value = "0";
                    //SROfficeList.Add(select);
                    resModel.SROfficeList = objCommon.getSROfficesList(true);
                    resModel.DistrictList = objCommon.GetDROfficesList("All");
                    resModel.IsSr = false;
                    resModel.IsDr = false;
                }
                    resModel.DocumentType = objCommon.GetDocumentType();
                    resModel.DocumentTypeID = 1;//For Document


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

        //public DocumentScanAndDeliveryREQModel DocumentScanAndDeliveryView(int OfficeID)
        //{
        //    try
        //    {
        //        dbContext = new KaveriEntities();
        //        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        //        DocumentScanAndDeliveryREQModel resModel = new DocumentScanAndDeliveryREQModel();

        //        DateTime now = DateTime.Now;
        //        var startDate = new DateTime(now.Year, now.Month, 1);
        //        resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

        //        resModel.SROfficeList = new List<SelectListItem>();
        //        resModel.DistrictList = new List<SelectListItem>();
        //        SelectListItem sroNameItem = new SelectListItem();
        //        SelectListItem droNameItem = new SelectListItem();

        //        short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
        //        int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
        //        string kaveriCode = Kaveri1Code.ToString();

        //        if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
        //        {
        //            string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
        //            int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(DroCode);
        //            //resModel.SROfficeList = new List<SelectListItem>();
        //            //resModel.DistrictList = new List<SelectListItem>();
        //            sroNameItem = GetDefaultSelectListItem(SroName, kaveriCode);
        //            droNameItem = GetDefaultSelectListItem(DroName, DroCode_string);
        //            resModel.DistrictList.Add(droNameItem);
        //            resModel.SROfficeList.Add(sroNameItem);
        //        }
        //        else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
        //        {
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

        //            string DroCode_string = Convert.ToString(Kaveri1Code);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            resModel.DistrictList.Add(droNameItem);
        //            resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "All");
        //        }
        //        else
        //        {
        //            //SelectListItem select = new SelectListItem();
        //            //select.Text = "All";
        //            //select.Value = "0";
        //            //SROfficeList.Add(select);
        //            resModel.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
        //            resModel.DistrictList = objCommon.GetDROfficesList("All");
        //        }

        //        return resModel;

        //        //var SROMasterList = dbContext.SROMaster.OrderBy(c => c.SRONameE).ToList();
        //        //resModel.SROfficeList.Add(new SelectListItem { Text = "Select", Value = "0" });
        //        //if (SROMasterList != null)
        //        //{
        //        //    if (SROMasterList.Count > 0)
        //        //    {
        //        //        foreach (var item in SROMasterList)
        //        //        {
        //        //            SelectListItem select = new SelectListItem()
        //        //            {
        //        //                Text = item.SRONameE,
        //        //                Value = item.SROCode.ToString()
        //        //            };
        //        //            resModel.SROfficeList.Add(select);
        //        //        }
        //        //    }
        //        //}

        //        //var DistrictMasterList = dbContext.DistrictMaster.OrderBy(c => c.DistrictNameE).ToList();
        //        //resModel.DistrictList.Add(new SelectListItem { Text = "Select", Value = "0" });
        //        //if (DistrictMasterList != null)
        //        //{
        //        //    if (DistrictMasterList.Count > 0)
        //        //    {
        //        //        foreach (var item in DistrictMasterList)
        //        //        {
        //        //            SelectListItem select = new SelectListItem()
        //        //            {
        //        //                Text = item.DistrictNameE,
        //        //                Value = item.DistrictCode.ToString()
        //        //            };
        //        //            resModel.DistrictList.Add(select);
        //        //        }
        //        //    }
        //        //}

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //            dbContext.Dispose();
        //    }
        //}

        public DocumentScanAndDeliveryWrapper DocumentScanAndDeliveryDetails(DocumentScanAndDeliveryREQModel model)
        {
            #region
            //DocumentScanAndDeliveryWrapper resModel = new DocumentScanAndDeliveryWrapper();
            //resModel.DocumentScanAndDeliveryDetailList = new List<DocumentScanAndDeliveryDetail>() {
            //    new DocumentScanAndDeliveryDetail(){
            //        SerialNumber=1,
            //        MyProperty1="gr",
            //        MyProperty2="greger",
            //        MyProperty3="gre",
            //        MyProperty4="gregre",
            //        MyProperty5="gfbdgf",
            //        MyProperty6="gbfghb",
            //        MyProperty7="nhgnfgh",
            //        MyProperty8="regregre",
            //        MyProperty9="fdsgsfdnfhnd",
            //        MyProperty10="nhgnfgh",
            //        MyProperty11="regregre",
            //        MyProperty12="fdsgsfdnfhnd"
            //    },
            //    new DocumentScanAndDeliveryDetail{
            //         SerialNumber=2,
            //        MyProperty1="rgre",
            //        MyProperty2="ysreyhtrjeyt",
            //        MyProperty3="rtyresyrty",
            //        MyProperty4="rtrwetare",
            //        MyProperty5="hgmfghm",
            //        MyProperty6="yukyumghfm",
            //        MyProperty7="yukyukuytk",
            //        MyProperty8="tretre",
            //        MyProperty9="retre",
            //        MyProperty10="nhgnfgh",
            //        MyProperty11="regregre",
            //        MyProperty12="fdsgsfdnfhnd"
            //    }
            //};
            #endregion

            try
            {
                dbContext = new KaveriEntities();
                DocumentScanAndDeliveryWrapper resModel = new DocumentScanAndDeliveryWrapper();
                resModel.DocumentScanAndDeliveryDetailList = new List<DocumentScanAndDeliveryDetail>();
                DocumentScanAndDeliveryDetail documentScanAndDeliveryDetail = null;
                //var result = dbContext.USP_RPT_DocumentScanAndDelivery(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                List<USP_RPT_DocumentScanAndDelivery_Result> result = new List<USP_RPT_DocumentScanAndDelivery_Result>();
                
              
                     result = dbContext.USP_RPT_DocumentScanAndDelivery(model.DistrictID, 0, model.DateTime_FromDate, model.DateTime_ToDate, "DR", model.DocumentTypeID).ToList();
               

                if (result != null)
                {
                    resModel.TotalRecords = result.Count;
                    if (!model.IsExcel)
                    {
                        if (string.IsNullOrEmpty(model.SearchValue))
                        {
                            result = result.Skip(model.startLen).Take(model.totalNum).ToList();
                        }
                    }
                    if (result.Count() > 0)
                    {
                        int count = 1;
                        foreach (var item in result)
                        {
                            documentScanAndDeliveryDetail = new DocumentScanAndDeliveryDetail();
                            documentScanAndDeliveryDetail.SerialNumber = count++;
                            //documentScanAndDeliveryDetail.OfficeName = item.SROName ?? "";
                            documentScanAndDeliveryDetail.OfficeName = String.IsNullOrEmpty(item.SROName) ? "" : item.SROName;
                            documentScanAndDeliveryDetail.DocumentType = String.IsNullOrEmpty(item.Document_Type_Document_Marriage) ? String.Empty : item.Document_Type_Document_Marriage;
                            documentScanAndDeliveryDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.FinalRegistrationNumber) ? String.Empty : item.FinalRegistrationNumber;
                            documentScanAndDeliveryDetail.LocalServerStoragePath = String.IsNullOrEmpty(item.Local_Server_Storage_Path) ? String.Empty : item.Local_Server_Storage_Path;
                            documentScanAndDeliveryDetail.FileUploadedToCentralServer = String.IsNullOrEmpty(item.File_Uploaded_to_Central_Server) ? String.Empty : item.File_Uploaded_to_Central_Server;
                            documentScanAndDeliveryDetail.StateDataCentreStoragePath = String.IsNullOrEmpty(item.State_Data_Centre_Storage_Path) ? String.Empty : item.State_Data_Centre_Storage_Path;
                            documentScanAndDeliveryDetail.SizeoftheFile = item.FileSize == null ? String.Empty : (item.FileSize/1024).ToString();
                            documentScanAndDeliveryDetail.DateofScan = (item.Date_of_Scan=="-") ? item.Date_of_Scan : Convert.ToDateTime(item.Date_of_Scan).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            documentScanAndDeliveryDetail.DateofUpload = (item.Date_of_Upload=="-") ? item.Date_of_Upload : item.Date_of_Upload;
                            documentScanAndDeliveryDetail.ArchivedinCD = String.IsNullOrEmpty(item.Archived_in_CD) ? String.Empty : item.Archived_in_CD;
                            documentScanAndDeliveryDetail.DocumentDeliveryDate = String.IsNullOrEmpty(item.Document_Delivery_Date) ? String.Empty : item.Document_Delivery_Date;
                            documentScanAndDeliveryDetail.DateofRegistration = item.Date_of_Registration == null ? String.Empty : ((item.Date_of_Registration=="-") ? item.Date_of_Registration :  Convert.ToDateTime(item.Date_of_Registration).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)  );
                            resModel.DocumentScanAndDeliveryDetailList.Add(documentScanAndDeliveryDetail);
                        }
                    }
                }

                resModel.SelectedSRO = model.SROfficeID == 0 ? "All" : dbContext.SROMaster.Where(x => x.SROCode == model.SROfficeID).Select(x => x.SRONameE).FirstOrDefault();
                resModel.SelectedDRO = model.DistrictID == 0 ? "All" : dbContext.DistrictMaster.Where(x => x.DistrictCode == model.DistrictID).Select(x => x.DistrictNameE).FirstOrDefault();
                return resModel;
            }
            catch (Exception) { throw; }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public int DocumentScanAndDeliveryCount(DocumentScanAndDeliveryREQModel model)
        {
            try
            {
                dbContext = new KaveriEntities(); var result = dbContext.USP_RPT_DocumentScanAndDelivery(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate,"DR",model.DocumentTypeID).ToList();
                if (result != null)
                {
                    return result.Count();
                }
                return 0;
            }
            catch (Exception) { throw; }
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
        public DocumentScanAndDeliveryWrapper DocumentScanAndDeliveryDetailsForSRO(DocumentScanAndDeliveryREQModel model)
        {
            #region
            //DocumentScanAndDeliveryWrapper resModel = new DocumentScanAndDeliveryWrapper();
            //resModel.DocumentScanAndDeliveryDetailList = new List<DocumentScanAndDeliveryDetail>() {
            //    new DocumentScanAndDeliveryDetail(){
            //        SerialNumber=1,
            //        MyProperty1="gr",
            //        MyProperty2="greger",
            //        MyProperty3="gre",
            //        MyProperty4="gregre",
            //        MyProperty5="gfbdgf",
            //        MyProperty6="gbfghb",
            //        MyProperty7="nhgnfgh",
            //        MyProperty8="regregre",
            //        MyProperty9="fdsgsfdnfhnd",
            //        MyProperty10="nhgnfgh",
            //        MyProperty11="regregre",
            //        MyProperty12="fdsgsfdnfhnd"
            //    },
            //    new DocumentScanAndDeliveryDetail{
            //         SerialNumber=2,
            //        MyProperty1="rgre",
            //        MyProperty2="ysreyhtrjeyt",
            //        MyProperty3="rtyresyrty",
            //        MyProperty4="rtrwetare",
            //        MyProperty5="hgmfghm",
            //        MyProperty6="yukyumghfm",
            //        MyProperty7="yukyukuytk",
            //        MyProperty8="tretre",
            //        MyProperty9="retre",
            //        MyProperty10="nhgnfgh",
            //        MyProperty11="regregre",
            //        MyProperty12="fdsgsfdnfhnd"
            //    }
            //};
            #endregion

            try
            {
                dbContext = new KaveriEntities();
                DocumentScanAndDeliveryWrapper resModel = new DocumentScanAndDeliveryWrapper();
                resModel.DocumentScanAndDeliveryDetailList = new List<DocumentScanAndDeliveryDetail>();
                DocumentScanAndDeliveryDetail documentScanAndDeliveryDetail = null;
                //var result = dbContext.USP_RPT_DocumentScanAndDelivery(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                //var result = dbContext.USP_RPT_DocumentScanAndDelivery(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, "SR").ToList();
                //0 for all SRO and for logins other than DR
                List<USP_RPT_DocumentScanAndDelivery_Result> result = new List<USP_RPT_DocumentScanAndDelivery_Result>();
                    if (model.IsSrLogin)
                {
                    result = dbContext.USP_RPT_DocumentScanAndDelivery(0, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, "SR",model.DocumentTypeID).ToList();
                }
                else
                {
                    result = dbContext.USP_RPT_DocumentScanAndDelivery(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, "SR", model.DocumentTypeID).ToList();
                }
                if (result != null)
                {

                    resModel.TotalRecords = result.Count;
                    if (!model.IsExcel)
                    {
                        if (string.IsNullOrEmpty(model.SearchValue))
                        {
                            result = result.Skip(model.startLen).Take(model.totalNum).ToList();
                        }
                    }


                    if (result.Count() > 0)
                    {
                        int count = 1;
                        foreach (var item in result)
                        {
                            documentScanAndDeliveryDetail = new DocumentScanAndDeliveryDetail();
                            documentScanAndDeliveryDetail.SerialNumber = count++;

                            //documentScanAndDeliveryDetail.OfficeName = item.SROName ?? "";
                            documentScanAndDeliveryDetail.OfficeName = String.IsNullOrEmpty(item.SROName) ? "" : item.SROName;
                            documentScanAndDeliveryDetail.DocumentType = String.IsNullOrEmpty(item.Document_Type_Document_Marriage) ? String.Empty : item.Document_Type_Document_Marriage;
                            documentScanAndDeliveryDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.FinalRegistrationNumber) ? String.Empty : item.FinalRegistrationNumber;
                            documentScanAndDeliveryDetail.LocalServerStoragePath = String.IsNullOrEmpty(item.Local_Server_Storage_Path) ? String.Empty : item.Local_Server_Storage_Path;
                            documentScanAndDeliveryDetail.FileUploadedToCentralServer = String.IsNullOrEmpty(item.File_Uploaded_to_Central_Server) ? String.Empty : item.File_Uploaded_to_Central_Server;
                            documentScanAndDeliveryDetail.StateDataCentreStoragePath = String.IsNullOrEmpty(item.State_Data_Centre_Storage_Path) ? String.Empty : item.State_Data_Centre_Storage_Path;
                            documentScanAndDeliveryDetail.SizeoftheFile = item.FileSize == null ? String.Empty : (item.FileSize / 1024).ToString();
                            documentScanAndDeliveryDetail.DateofScan = String.IsNullOrEmpty(item.Date_of_Scan) || item.Date_of_Scan == "-" ? String.Empty : Convert.ToDateTime(item.Date_of_Scan).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            documentScanAndDeliveryDetail.DateofUpload = String.IsNullOrEmpty(item.Date_of_Upload) ? String.Empty : item.Date_of_Upload;
                            documentScanAndDeliveryDetail.ArchivedinCD = String.IsNullOrEmpty(item.Archived_in_CD) ? String.Empty : item.Archived_in_CD;
                            documentScanAndDeliveryDetail.DocumentDeliveryDate = String.IsNullOrEmpty(item.Document_Delivery_Date) ? String.Empty : item.Document_Delivery_Date;
                            documentScanAndDeliveryDetail.DateofRegistration = (item.Date_of_Registration == null || item.Date_of_Registration =="-") ? "" : Convert.ToDateTime(item.Date_of_Registration).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                            resModel.DocumentScanAndDeliveryDetailList.Add(documentScanAndDeliveryDetail);
                        }
                    }
                }

                resModel.SelectedSRO = model.SROfficeID == 0 ? "All" : dbContext.SROMaster.Where(x => x.SROCode == model.SROfficeID).Select(x => x.SRONameE).FirstOrDefault();
                resModel.SelectedDRO = model.DistrictID == 0 ? "All" : dbContext.DistrictMaster.Where(x => x.DistrictCode == model.DistrictID).Select(x => x.DistrictNameE).FirstOrDefault();

                return resModel;
            }
            catch (Exception ex) { throw; }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

    }
}
