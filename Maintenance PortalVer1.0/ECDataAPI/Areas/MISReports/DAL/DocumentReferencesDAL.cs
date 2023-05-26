using CustomModels.Models.MISReports.DocumentReferences;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class DocumentReferencesDAL : IDocumentReferences
    {
        KaveriEntities dbContext = null;


        public DocumentReferencesREQModel DocumentReferencesView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                DocumentReferencesREQModel resModel = new DocumentReferencesREQModel();

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                resModel.SROfficeList = new List<SelectListItem>();
                resModel.DistrictList = new List<SelectListItem>();
                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //SelectListItem sroNameItem = new SelectListItem();
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
                    //sroNameItem = GetDefaultSelectListItem(SroName, kaveriCode);
                    //droNameItem = GetDefaultSelectListItem(DroName, DroCode_string);
                    //resModel.DistrictList.Add(droNameItem);
                    //resModel.SROfficeList.Add(sroNameItem);
                    resModel.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    resModel.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });


                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(Kaveri1Code);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //resModel.DistrictList.Add(droNameItem);
                    //resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "All");
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
        //public DocumentReferencesREQModel DocumentReferencesView(int OfficeID)
        //{
        //    try
        //    {
        //        dbContext = new KaveriEntities();
        //        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        //        DocumentReferencesREQModel resModel = new DocumentReferencesREQModel();

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

        public DocumentReferencesWrapper DocumentReferencesDetails(DocumentReferencesREQModel model)
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
                DocumentReferencesWrapper resModel = new DocumentReferencesWrapper();
                resModel.DocumentReferencesDetailList = new List<DocumentReferencesDetail>();
                DocumentReferencesDetail documentReferencesDetail = null;
                //var result = dbContext.USP_RPT_DocumentReference(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.startLen).Take(model.totalNum).ToList();
                var result = dbContext.USP_RPT_DocumentReference(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList();

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
                            documentReferencesDetail = new DocumentReferencesDetail();
                            documentReferencesDetail.SerialNumber = count++;

                            //documentScanAndDeliveryDetail.OfficeName = item.SROName ?? "";
                            documentReferencesDetail.OfficeName = String.IsNullOrEmpty(item.SROName) ? "" : item.SROName;
                            documentReferencesDetail.DocumentType = String.IsNullOrEmpty(item.Document_Type_Document_Marriage) ? String.Empty : item.Document_Type_Document_Marriage;
                            documentReferencesDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.FinalRegistrationNumber) ? String.Empty : item.FinalRegistrationNumber;
                            documentReferencesDetail.ReferenceText = String.IsNullOrEmpty(item.ReferenceText) ? String.Empty : item.ReferenceText;
                            documentReferencesDetail.ThroghType = String.IsNullOrEmpty(item.ThroghType) ? String.Empty : item.ThroghType;
                            documentReferencesDetail.RevenueOfficerNo_CourtNo = String.IsNullOrEmpty(item.RevenueOfficerNo_CourtNo) ? String.Empty : item.RevenueOfficerNo_CourtNo;
                            documentReferencesDetail.RevenueOfficerDate_CourtOrderDate = item.RevenueOfficerDate_CourtOrderDate == null ? String.Empty : item.RevenueOfficerDate_CourtOrderDate;
                            documentReferencesDetail.Date_of_Registration = item.Date_of_Registration == null ? String.Empty : item.Date_of_Registration.ToString();

                            resModel.DocumentReferencesDetailList.Add(documentReferencesDetail);
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

        public int DocumentReferencesCount(DocumentReferencesREQModel model)
        {
            try
            {
                dbContext = new KaveriEntities();
                var result = dbContext.USP_RPT_DocumentReference(model.DistrictID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
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
    }
}