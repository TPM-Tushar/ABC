#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   PendingDocumentsSummaryDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion


using CustomModels.Models.MISReports.PendingDocumentsSummary;
using CustomModels.Security;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;
using ECDataAPI.Entity.KaveriEntities;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class PendingDocumentsSummaryDAL : IPendingDocsSummary
    {
        KaveriEntities dbContext = null;
        private String[] encryptedParameters = null;
        private Dictionary<String, String> decryptedParameters = null;


        /// <summary>
        /// Returns PendingDocSummaryViewModel Required to show PendingDocumentsSummaryView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>

        public PendingDocSummaryViewModel PendingDocumentsSummaryView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                PendingDocSummaryViewModel model = new PendingDocSummaryViewModel();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.SROfficeList = new List<SelectListItem>();
                model.DistrictList = new List<SelectListItem>();

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();
                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //string kaveriCode = Kaveri1Code.ToString();


                //if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                //{
                //    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                //    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                //    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                //    string DroCode_string = Convert.ToString(DroCode);

                //    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                //    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                //    model.DistrictList.Add(droNameItem);
                //    model.SROfficeList.Add(sroNameItem);
                //}
                //else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                //{
                //    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                //    string DroCode_string = Convert.ToString(Kaveri1Code);
                //    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                //    model.DistrictList.Add(droNameItem);
                //    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "Select");
                //}
                //else
                //{

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //SelectListItem select = new SelectListItem();
                //select.Text = "All";
                //select.Value = "0";
                //model.SROfficeList.Add(select);
                model.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                model.DistrictList = objCommon.GetDROfficesList("All");
                //}
                return model;

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

        //public PendingDocSummaryViewModel PendingDocumentsSummaryView(int OfficeID)
        //{
        //    try
        //    {
        //        dbContext = new KaveriEntities();
        //        PendingDocSummaryViewModel model = new PendingDocSummaryViewModel();
        //        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        //        SelectListItem sroNameItem = new SelectListItem();
        //        SelectListItem droNameItem = new SelectListItem();
        //        DateTime now = DateTime.Now;
        //        var startDate = new DateTime(now.Year, now.Month, 1);
        //        model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
        //        int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
        //        model.SROfficeList = new List<SelectListItem>();
        //        model.DistrictList = new List<SelectListItem>();
        //        string kaveriCode = Kaveri1Code.ToString();
        //        //if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
        //        //{
        //        //    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
        //        //    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
        //        //    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
        //        //    string DroCode_string = Convert.ToString(DroCode);

        //        //    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
        //        //    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //        //    model.DistrictList.Add(droNameItem);
        //        //    model.SROfficeList.Add(sroNameItem);
        //        //}
        //        //else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
        //        //{
        //        //    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
        //        //    string DroCode_string = Convert.ToString(Kaveri1Code);
        //        //    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //        //    model.DistrictList.Add(droNameItem);
        //        //    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "Select");
        //        //}
        //        //else
        //        //{
        //        SelectListItem select = new SelectListItem();
        //        select.Text = "All";
        //        select.Value = "0";
        //        model.SROfficeList.Add(select);
        //        model.DistrictList = objCommon.GetDROfficesList("All");
        //        //}
        //        return model;

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


        /// <summary>
        /// Returns PendingDocsSummaryResModel Required to show Pending Document Summary DataTable
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public PendingDocsSummaryResModel LoadPendingDocumentSummaryDataTable(PendingDocSummaryViewModel model)
        {
            PendingDocsSummaryResModel PendingDocSummaryResModel = new PendingDocsSummaryResModel();
            PendingDocsDatatableRecord PendingDocsSummaryTableRec = null;
            List<PendingDocsDatatableRecord> ReportsDetailsList = new List<PendingDocsDatatableRecord>();
            try
            {
                dbContext = new KaveriEntities();
                int counter = (model.startLen + 1); //To start Serial Number 
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                List<RPT_PendingDocumentsSummary_Result> PendingDocsList = new List<RPT_PendingDocumentsSummary_Result>();
                PendingDocsList = dbContext.RPT_PendingDocumentsSummary(model.DistrictID, model.SROfficeID, FromDate, ToDate).ToList();
                PendingDocSummaryResModel.TotalCount = PendingDocsList.Count();
                List<PendingDocsDatatableRecord> PendingDocsDatatableRecList = new List<PendingDocsDatatableRecord>();

                string EncryptedSROCODE, EncryptedColNameC, EncryptedColNameD, EncryptedSROName, EncryptedDistrict, EncryptedColNameE;

                foreach (RPT_PendingDocumentsSummary_Result item in PendingDocsList)
                {
                    PendingDocsSummaryTableRec = new PendingDocsDatatableRecord();
                    PendingDocsSummaryTableRec.SerialNo = counter++;
                    PendingDocsSummaryTableRec.District = string.IsNullOrEmpty(item.DistrictName) ? string.Empty : item.DistrictName;
                    PendingDocsSummaryTableRec.SRO = string.IsNullOrEmpty(item.SROName) ? string.Empty : item.SROName;
                    PendingDocsSummaryTableRec.NoOfDocsPresented = (item.Number_of_Docs_Presented == null) ? 0 : Convert.ToInt32(item.Number_of_Docs_Presented);
                    PendingDocsSummaryTableRec.NoOfDocsRegistered = (item.Number_Of_DocumentsRegistered == null) ? 0 : Convert.ToInt32(item.Number_Of_DocumentsRegistered);
                    PendingDocsSummaryTableRec.NoOfDocsKeptPending = (item.Number_of_current_Pending_Docs == null) ? 0 : Convert.ToInt32(item.Number_of_current_Pending_Docs);
                    
                    // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021
                    //PendingDocsSummaryTableRec.DocsNotRegdNotPending = PendingDocsSummaryTableRec.NoOfDocsPresented - PendingDocsSummaryTableRec.NoOfDocsRegistered - PendingDocsSummaryTableRec.NoOfDocsKeptPending;
                    PendingDocsSummaryTableRec.NoOfPendingLaterFinalizedDocs = (item.Number_of_Pending_later_finalized_Docs == null) ? 0 : Convert.ToInt32(item.Number_of_Pending_later_finalized_Docs);
                    // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021	

                    PendingDocSummaryResModel.TotalNoOfDocsPresented = PendingDocSummaryResModel.TotalNoOfDocsPresented + PendingDocsSummaryTableRec.NoOfDocsPresented;
                    PendingDocSummaryResModel.TotalNoOfDocsRegistered = PendingDocSummaryResModel.TotalNoOfDocsRegistered + PendingDocsSummaryTableRec.NoOfDocsRegistered;
                    PendingDocSummaryResModel.TotalNoOfDocsKeptPending = PendingDocSummaryResModel.TotalNoOfDocsKeptPending + PendingDocsSummaryTableRec.NoOfDocsKeptPending;
                    // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021
                    //PendingDocSummaryResModel.TotalDocsNotRegdNotPending = PendingDocSummaryResModel.TotalDocsNotRegdNotPending + PendingDocsSummaryTableRec.DocsNotRegdNotPending;
                    PendingDocSummaryResModel.TotalNumberOfPendingLaterFinalizedDocs = PendingDocSummaryResModel.TotalNumberOfPendingLaterFinalizedDocs + PendingDocsSummaryTableRec.NoOfPendingLaterFinalizedDocs;
                    // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021	

                    EncryptedSROCODE = URLEncrypt.EncryptParameters(new String[] { "SROCODE=" + item.SROCODE });
                    EncryptedColNameC = URLEncrypt.EncryptParameters(new String[] { "EncryptedColName=" + "C" });
                    EncryptedColNameD = URLEncrypt.EncryptParameters(new String[] { "EncryptedColName=" + "D" });
                    EncryptedSROName = URLEncrypt.EncryptParameters(new String[] { "SROName=" + item.SROName });
                    EncryptedDistrict = URLEncrypt.EncryptParameters(new String[] { "District=" + item.DistrictName });

                    //item.E_Swathu == 0 ? item.E_Swathu.ToString() : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetOtherTableDetails('" + "D" + "','" + item.SROCode + "');><i>" + item.E_Swathu + "</i></a>";

                    PendingDocsSummaryTableRec.Str_NoOfDocsKeptPending = item.Number_of_current_Pending_Docs == 0 ? "0" : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetPendingDocsSummaryDetails('" + EncryptedColNameC + "','" + EncryptedSROCODE + "','" + EncryptedSROName + "','" + EncryptedDistrict + "');><i>" + item.Number_of_current_Pending_Docs + "</i></a>";

                    // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021
                    
                    //PendingDocsSummaryTableRec.Str_DocsNotRegdNotPending = PendingDocsSummaryTableRec.DocsNotRegdNotPending == 0 ? "0" : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetPendingDocsDocsNotRegdNotPendingDetails('" + EncryptedColNameD + "','" + EncryptedSROCODE + "','" + EncryptedSROName + "','" + EncryptedDistrict + "');><i>" + PendingDocsSummaryTableRec.DocsNotRegdNotPending + "</i></a>";
                    EncryptedColNameE = URLEncrypt.EncryptParameters(new String[] { "EncryptedColName=" + "E" });
                    PendingDocsSummaryTableRec.Str_Number_of_Pending_later_finalized_Docs = item.Number_of_Pending_later_finalized_Docs == 0 ? "0" : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=GetPendingLaterFinalizedDocs('" + EncryptedColNameE + "','" + EncryptedSROCODE + "','" + EncryptedSROName + "','" + EncryptedDistrict + "');><i>" + item.Number_of_Pending_later_finalized_Docs + "</i></a>";
                    // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021	

                    PendingDocsDatatableRecList.Add(PendingDocsSummaryTableRec);
                }
                PendingDocSummaryResModel.IPendingDocsDatatableRecList = PendingDocsDatatableRecList;

                if (!string.IsNullOrEmpty(model.SearchValue))
                {
                    PendingDocSummaryResModel.IPendingDocsDatatableRecList = PendingDocSummaryResModel.IPendingDocsDatatableRecList.Where(m => m.District.ToString().Contains(model.SearchValue.ToLower()) ||
                      //m.District.ToLower().Contains(model.SearchValue.ToLower()) ||
                      m.SRO.ToLower().Contains(model.SearchValue.ToLower()) ||
                      m.NoOfDocsPresented.ToString().Contains(model.SearchValue.ToLower()) ||
                      m.NoOfDocsRegistered.ToString().Contains(model.SearchValue.ToLower()) ||
                      m.NoOfDocsKeptPending.ToString().Contains(model.SearchValue.ToLower()) ||
                      // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021        
                      //m.DocsNotRegdNotPending.ToString().Contains(model.SearchValue.ToLower())
                      m.NoOfPendingLaterFinalizedDocs.ToString().Contains(model.SearchValue.ToLower())
                      // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021			
                      );
                    PendingDocSummaryResModel.FilteredRecordsCount = PendingDocSummaryResModel.IPendingDocsDatatableRecList.Count();
                }

                return PendingDocSummaryResModel;
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


        /// <summary>
        /// Returns PendingDocsSummaryDetailsResModel Required to show Pending Document Details DataTable
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public PendingDocsSummaryDetailsResModel LoadPendingDocumentDetailsDataTable(PendingDocSummaryViewModel model)
        {
            PendingDocsSummaryDetailsResModel PendingDocSummaryResModel = new PendingDocsSummaryDetailsResModel();
            PendingDocsSummaryDetailsRecordModel PendingDocsSummaryTableRec = null;
            List<PendingDocsDatatableRecord> ReportsDetailsList = new List<PendingDocsDatatableRecord>();
            try
            {
                dbContext = new KaveriEntities();
                encryptedParameters = model.SROIDEncrypted.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");
                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int SROCode = Convert.ToInt32(decryptedParameters["SROCODE"].ToString().Trim());
                PendingDocSummaryResModel.DecryptedSROCode = SROCode;

                encryptedParameters = model.ColumnName.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");
                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                PendingDocSummaryResModel.DecryptedColumnName = decryptedParameters["EncryptedColName"].ToString().Trim();

                encryptedParameters = model.ENCDistrictName.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                PendingDocSummaryResModel.DecryptedDistrictName = decryptedParameters["District"].ToString().Trim();

                encryptedParameters = model.ENCSROName.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                PendingDocSummaryResModel.DecryptedSROName = decryptedParameters["SROName"].ToString().Trim();


                int counter = (model.startLen + 1); //To start Serial Number 
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                List<PendingDocsSummaryDetailsRecordModel> PendingDocsDatatableRecList = new List<PendingDocsSummaryDetailsRecordModel>();

                if (PendingDocSummaryResModel.DecryptedColumnName == "D")//Condition when DocumentsNotRegisteredAndNotPending Column Is Selected
                {
                    List<RPT_DocumentsNotRegisteredNotPendingDetails_Result> PendingDocsListWhenSearch = new List<RPT_DocumentsNotRegisteredNotPendingDetails_Result>();
                    List<RPT_DocumentsNotRegisteredNotPendingDetails_Result> PendingDocsList = new List<RPT_DocumentsNotRegisteredNotPendingDetails_Result>();
                    PendingDocsListWhenSearch = dbContext.RPT_DocumentsNotRegisteredNotPendingDetails(SROCode, FromDate, ToDate).ToList();

                    PendingDocSummaryResModel.TotalCount = PendingDocsListWhenSearch.Count();
                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        if (!model.IsExcel)
                        {
                            PendingDocsList = PendingDocsListWhenSearch.Skip(model.startLen).Take(model.totalNum).ToList();
                        }
                        else
                        {
                            PendingDocsList = PendingDocsListWhenSearch;
                        }
                    }
                    else
                    {
                        PendingDocsList = PendingDocsListWhenSearch;
                    }

                    foreach (RPT_DocumentsNotRegisteredNotPendingDetails_Result item in PendingDocsList)
                    {
                        PendingDocsSummaryTableRec = new PendingDocsSummaryDetailsRecordModel();
                        PendingDocsSummaryTableRec.SerialNo = counter++;
                        PendingDocsSummaryTableRec.SroName = string.IsNullOrEmpty(item.SROName) ? string.Empty : item.SROName;
                        PendingDocsSummaryTableRec.RegArticle = string.IsNullOrEmpty(item.ArticleName) ? string.Empty : item.ArticleName;

                        PendingDocsSummaryTableRec.DocumentNumber = (item.DocumentNumber == null) ? 0 : Convert.ToInt32(item.DocumentNumber);
                        PendingDocsSummaryTableRec.ConsiderationAmount = (item.ConsiderationAmount == null) ? string.Empty : Convert.ToDecimal(item.ConsiderationAmount).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        PendingDocsSummaryTableRec.ConsiderationAmount_Decimal = (item.ConsiderationAmount == null) ? 0 : Convert.ToDecimal(item.ConsiderationAmount);
                        PendingDocsSummaryTableRec.PresentDate = (item.PresentDateTime == null) ? string.Empty : Convert.ToDateTime(item.PresentDateTime).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        PendingDocsSummaryTableRec.WithdrawalDate = (item.WithdrawalDate == null) ? string.Empty : Convert.ToDateTime(item.WithdrawalDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        PendingDocsDatatableRecList.Add(PendingDocsSummaryTableRec);
                    }
                    PendingDocSummaryResModel.IPendingDocsSummaryRecordList = PendingDocsDatatableRecList;

                    if (!string.IsNullOrEmpty(model.SearchValue))
                    {
                        PendingDocSummaryResModel.IPendingDocsSummaryRecordList = PendingDocSummaryResModel.IPendingDocsSummaryRecordList.Where(m => m.SerialNo.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.SroName.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.DocumentNumber.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.ConsiderationAmount.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.RegArticle.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.PresentDate.ToString().Contains(model.SearchValue.ToLower()));
                        PendingDocSummaryResModel.FilteredRecordsCount = PendingDocSummaryResModel.IPendingDocsSummaryRecordList.Count();
                        PendingDocSummaryResModel.IPendingDocsSummaryRecordList= PendingDocSummaryResModel.IPendingDocsSummaryRecordList.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }
                else if (PendingDocSummaryResModel.DecryptedColumnName == "C")//Condition when PendingDocuments Column Is Selected
                {
                    List<RPT_PendingDocumentsDetails_Result> PendingDocsListWhenSearch = new List<RPT_PendingDocumentsDetails_Result>();
                    List<RPT_PendingDocumentsDetails_Result> PendingDocsList = new List<RPT_PendingDocumentsDetails_Result>();


                    PendingDocsListWhenSearch = dbContext.RPT_PendingDocumentsDetails(SROCode, FromDate, ToDate).ToList();
                    PendingDocSummaryResModel.TotalCount = PendingDocsListWhenSearch.Count();

                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        if (!model.IsExcel)
                        {
                            PendingDocsList = PendingDocsListWhenSearch.Skip(model.startLen).Take(model.totalNum).ToList();

                        }
                        else
                        {
                            PendingDocsList = PendingDocsListWhenSearch;
                        }
                    }
                    else
                    {
                        PendingDocsList = PendingDocsListWhenSearch;
                    }

                    foreach (RPT_PendingDocumentsDetails_Result item in PendingDocsList)
                    {
                        PendingDocsSummaryTableRec = new PendingDocsSummaryDetailsRecordModel();
                        PendingDocsSummaryTableRec.SerialNo = counter++;
                        PendingDocsSummaryTableRec.SroName = string.IsNullOrEmpty(item.SROName) ? string.Empty : item.SROName;
                        PendingDocsSummaryTableRec.DocumentID = item.DocumentID;
                        //PendingDocsSummaryTableRec.DocumentNumber = (item.PendingDocumentNumber == null) ? 0 : Convert.ToInt32(item.PendingDocumentNumber);
                        PendingDocsSummaryTableRec.FRN = string.IsNullOrEmpty(item.FRN) ? string.Empty : item.FRN;
                        PendingDocsSummaryTableRec.PendingDate = item.PendingDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        PendingDocsSummaryTableRec.ReasonOfPending = string.IsNullOrEmpty(item.ReasonOfPending) ? "" : item.ReasonOfPending;
                        PendingDocsSummaryTableRec.WhetherRegistered = item.WhetherRegistered == null ? false : true;
                        PendingDocsSummaryTableRec.PendingDocumentNumber = string.IsNullOrEmpty(item.PendingDocumentNumber) ? string.Empty : item.PendingDocumentNumber;
                        PendingDocsSummaryTableRec.FRN = string.IsNullOrEmpty(PendingDocsSummaryTableRec.FRN) ? string.Empty : PendingDocsSummaryTableRec.FRN;

                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 23-03-2021
                        PendingDocsSummaryTableRec.RegistrationDate = item.RegistrationDate == null ? "" : ((DateTime)item.RegistrationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 23-03-2021

                        if (item.PendingDocumentNumber != null || item.FRN != null)
                        {
                            // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021
                            // PendingDocsSummaryTableRec.PendingNumber = (string.IsNullOrEmpty(item.PendingDocumentNumber)?"-": item.PendingDocumentNumber) + "/ " + (string.IsNullOrEmpty(item.FRN) ? "-" : item.FRN);
                            PendingDocsSummaryTableRec.PendingNumber = (string.IsNullOrEmpty(item.PendingDocumentNumber) ? "-" : item.PendingDocumentNumber);
                            // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021
                        }
                        else
                        {
                            PendingDocsSummaryTableRec.PendingNumber = "";
                        }
                        
                        // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021
                        //PendingDocsSummaryTableRec.IsCleared = item.IsCleared == true ? true : false;
                        // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021

                        PendingDocsDatatableRecList.Add(PendingDocsSummaryTableRec);
                    }
                    PendingDocSummaryResModel.IPendingDocsSummaryRecordList = PendingDocsDatatableRecList;
                    if (!string.IsNullOrEmpty(model.SearchValue))
                    {
                       // PendingDocsList = dbContext.RPT_PendingDocumentsDetails(SROCode, FromDate, ToDate).Skip(model.startLen).Take(model.totalNum).ToList();

                        PendingDocSummaryResModel.IPendingDocsSummaryRecordList = PendingDocSummaryResModel.IPendingDocsSummaryRecordList.Where(m => m.SerialNo.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.SroName.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.DocumentID.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.DocumentNumber.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.FRN.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.PendingDate.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.ReasonOfPending.ToString().Contains(model.SearchValue.ToLower()));
                        PendingDocSummaryResModel.FilteredRecordsCount = PendingDocSummaryResModel.IPendingDocsSummaryRecordList.Count();
                        PendingDocSummaryResModel.IPendingDocsSummaryRecordList= PendingDocSummaryResModel.IPendingDocsSummaryRecordList.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }
                else if (PendingDocSummaryResModel.DecryptedColumnName == "E")//Condition when Number of Documents Pending then registered Column Is Selected
                {
                    List<RPT_PendingLaterFinalizedDocumentsDetails_Result> PendingLaterFinalizedDocumentsListWhenSearch = new List<RPT_PendingLaterFinalizedDocumentsDetails_Result>();
                    List<RPT_PendingLaterFinalizedDocumentsDetails_Result> PendingLaterFinalizedDocumentsList = new List<RPT_PendingLaterFinalizedDocumentsDetails_Result>();


                    PendingLaterFinalizedDocumentsListWhenSearch = dbContext.RPT_PendingLaterFinalizedDocumentsDetails(SROCode, FromDate, ToDate).ToList();
                    PendingDocSummaryResModel.TotalCount = PendingLaterFinalizedDocumentsListWhenSearch.Count();

                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        if (!model.IsExcel)
                        {
                            PendingLaterFinalizedDocumentsList = PendingLaterFinalizedDocumentsListWhenSearch.Skip(model.startLen).Take(model.totalNum).ToList();

                        }
                        else
                        {
                            PendingLaterFinalizedDocumentsList = PendingLaterFinalizedDocumentsListWhenSearch;
                        }
                    }
                    else
                    {
                        PendingLaterFinalizedDocumentsList = PendingLaterFinalizedDocumentsListWhenSearch;
                    }

                    foreach (RPT_PendingLaterFinalizedDocumentsDetails_Result item in PendingLaterFinalizedDocumentsList)
                    {
                        PendingDocsSummaryTableRec = new PendingDocsSummaryDetailsRecordModel();
                        PendingDocsSummaryTableRec.SerialNo = counter++;
                        PendingDocsSummaryTableRec.SroName = string.IsNullOrEmpty(item.SROName) ? string.Empty : item.SROName;
                        PendingDocsSummaryTableRec.DocumentID = item.DocumentID;
                        //PendingDocsSummaryTableRec.DocumentNumber = (item.PendingDocumentNumber == null) ? 0 : Convert.ToInt32(item.PendingDocumentNumber);
                        PendingDocsSummaryTableRec.FRN = string.IsNullOrEmpty(item.FRN) ? string.Empty : item.FRN;
                        PendingDocsSummaryTableRec.PendingDate = item.PendingDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        PendingDocsSummaryTableRec.ReasonOfPending = string.IsNullOrEmpty(item.ReasonOfPending) ? "" : item.ReasonOfPending;
                        PendingDocsSummaryTableRec.WhetherRegistered = item.WhetherRegistered == null ? false : true;
                        PendingDocsSummaryTableRec.PendingDocumentNumber = string.IsNullOrEmpty(item.PendingDocumentNumber) ? string.Empty : item.PendingDocumentNumber;
                        //PendingDocsSummaryTableRec.FRN = string.IsNullOrEmpty(PendingDocsSummaryTableRec.FRN) ? string.Empty : PendingDocsSummaryTableRec.FRN;
                        if (item.PendingDocumentNumber != null || item.FRN != null)
                        {
                            PendingDocsSummaryTableRec.PendingNumber = (string.IsNullOrEmpty(item.PendingDocumentNumber) ? "-" : item.PendingDocumentNumber) + " / " + (string.IsNullOrEmpty(item.FRN) ? "-" : item.FRN);
                        }
                        else
                        {
                            PendingDocsSummaryTableRec.PendingNumber = "";
                        }

                        // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021
                        //PendingDocsSummaryTableRec.IsCleared = item.IsCleared == true ? true : false;
                        // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021

                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 23-03-2021
                        PendingDocsSummaryTableRec.RegistrationDate = item.RegistrationDate == null ? "" : ((DateTime)item.RegistrationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 23-03-2021

                        PendingDocsDatatableRecList.Add(PendingDocsSummaryTableRec);
                    }
                    PendingDocSummaryResModel.IPendingDocsSummaryRecordList = PendingDocsDatatableRecList;
                    if (!string.IsNullOrEmpty(model.SearchValue))
                    {
                        // PendingDocsList = dbContext.RPT_PendingDocumentsDetails(SROCode, FromDate, ToDate).Skip(model.startLen).Take(model.totalNum).ToList();

                        PendingDocSummaryResModel.IPendingDocsSummaryRecordList = PendingDocSummaryResModel.IPendingDocsSummaryRecordList.Where(m => m.SerialNo.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.SroName.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.DocumentID.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.DocumentNumber.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.FRN.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.PendingDate.ToString().Contains(model.SearchValue.ToLower()) ||
                          m.ReasonOfPending.ToString().Contains(model.SearchValue.ToLower()));
                        PendingDocSummaryResModel.FilteredRecordsCount = PendingDocSummaryResModel.IPendingDocsSummaryRecordList.Count();
                        PendingDocSummaryResModel.IPendingDocsSummaryRecordList = PendingDocSummaryResModel.IPendingDocsSummaryRecordList.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }

                return PendingDocSummaryResModel;
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

    }
}