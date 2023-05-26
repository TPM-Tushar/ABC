#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   TodaysDocumentsRegisteredDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.IndexIIReports;
using CustomModels.Models.MISReports.TodaysDocumentsRegistered;
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
    public class TodaysDocumentsRegisteredDAL : ITodaysDocumentsRegistered, IDisposable
    {
        KaveriEntities dbContext = null;
        KaigrSearchDB searchDBContext = null;

        /// <summary>
        /// Returns TodaysDocumentsRegisteredReqModel required to show TodaysDocumentsRegisteredView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>

        public TodaysDocumentsRegisteredReqModel TodaysDocumentsRegisteredView(int OfficeID)
        {
            try
            {
                //string FirstRecord = "All";
                dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> SROfficeList = new List<SelectListItem>();
                TodaysDocumentsRegisteredReqModel model = new TodaysDocumentsRegisteredReqModel();
                model.SROfficeList = new List<SelectListItem>();
                model.DROfficeList = new List<SelectListItem>();

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();

                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();

                //string kaveriCode = Kaveri1Code.ToString();


                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(DroCode);
                    //model.SROfficeList = new List<SelectListItem>();
                    //model.DROfficeList = new List<SelectListItem>();
                    //sroNameItem = GetDefaultSelectListItem(SroName, ofcDetailsObj.kaveriCode);
                    //droNameItem = GetDefaultSelectListItem(DroName, DroCode_string);
                    //model.DROfficeList.Add(droNameItem);
                    //model.SROfficeList.Add(sroNameItem);
                    model.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    model.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });


                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.

                    //string DroCode_string = Convert.ToString(Kaveri1Code);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    //model.DROfficeList.Add(droNameItem);
                    //model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");

                    model.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");

                }
                else
                {
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //SelectListItem select = new SelectListItem();
                    //select.Text = "All";
                    //select.Value = "0";
                    //SROfficeList.Add(select);
                    //model.SROfficeList = SROfficeList;
                    model.DROfficeList = objCommon.GetDROfficesList("All");
                    model.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                    model.Stamp5Date = Convert.ToString(DateTime.Now);
                }

                model.ToDate_Str = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.Stamp5Date = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
                //List<DateTime> MaxDateTimeList = new List<DateTime>();
                //MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
                //model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
                //if (MaxDateTimeList != null)
                //{
                //    if (MaxDateTimeList.Count > 0)
                //    {
                //        //model.ReportInfo = "Note : This report is based on pre processed data considered upto : " + MaxDateTimeList.Max();
                //        model.MaxDate = MaxDateTimeList.Max();
                //    }
                //    else
                //    {
                //        model.ReportInfo = "";
                //    }
                //}
                //else
                //{
                //    model.ReportInfo = "";
                //}
                List<DateTime> MaxDateTimeList = new List<DateTime>();
                MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
                //model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
                if (MaxDateTimeList != null)
                {

                    if (MaxDateTimeList.Count > 0)
                    {
                        if (model.Stamp5DateTime.Date == DateTime.Now.Date && model.ToDate.Date == DateTime.Now.Date)
                        {
                            model.ReportInfo = "";
                        }
                        else
                        {
                            model.ReportInfo = "Note : This report is based on pre processed data considered upto : " + MaxDateTimeList.Max();

                        }
                        model.MaxDate = MaxDateTimeList.Max();
                    }
                    else
                    {
                        model.ReportInfo = "";
                    }
                }
                else
                {
                    model.ReportInfo = "";
                }

                //Added by Madhusoodan on 28-08-2020
                model.DocumentType = objCommon.GetDocumentType();

                //Added by mayank on 13/09/2021 for Firm registered report
                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    model.DocumentType.RemoveAt(4);
                }
                //End

                model.DocumentTypeID = 1; //For by default Document
                model.isDRO = 0;  //By default zero

                return model;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
                if (searchDBContext != null)
                {
                    searchDBContext.Dispose();
                }
            }
        }
        //public TodaysDocumentsRegisteredReqModel TodaysDocumentsRegisteredView(int OfficeID)
        //{
        //    try
        //    {
        //        string FirstRecord = "All";
        //        dbContext = new KaveriEntities();
        //        searchDBContext = new KaigrSearchDB();
        //        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        //        List<SelectListItem> SROfficeList = new List<SelectListItem>();
        //        TodaysDocumentsRegisteredReqModel model = new TodaysDocumentsRegisteredReqModel();
        //        SelectListItem sroNameItem = new SelectListItem();
        //        SelectListItem droNameItem = new SelectListItem();
        //        model.SROfficeList = new List<SelectListItem>();
        //        model.DROfficeList = new List<SelectListItem>();
        //        short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
        //        int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
        //        string kaveriCode = Kaveri1Code.ToString();
        //        if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
        //        {
        //            string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
        //            int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(DroCode);
        //            model.SROfficeList = new List<SelectListItem>();
        //            model.DROfficeList = new List<SelectListItem>();
        //            sroNameItem = GetDefaultSelectListItem(SroName, kaveriCode);
        //            droNameItem = GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DROfficeList.Add(droNameItem);
        //            model.SROfficeList.Add(sroNameItem);
        //        }
        //        else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
        //        {
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

        //            string DroCode_string = Convert.ToString(Kaveri1Code);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DROfficeList.Add(droNameItem);
        //            model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, FirstRecord);
        //        }
        //        else
        //        {
        //            SelectListItem select = new SelectListItem();
        //            select.Text = "All";
        //            select.Value = "0";
        //            SROfficeList.Add(select);
        //            model.SROfficeList = SROfficeList;
        //            model.DROfficeList = objCommon.GetDROfficesList("All");
        //            model.Stamp5Date = Convert.ToString(DateTime.Now);

        //        }

        //        model.ToDate_Str = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        DateTime now = DateTime.Now;
        //        var startDate = new DateTime(now.Year, now.Month, 1);
        //        model.Stamp5Date = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        //model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
        //        //List<DateTime> MaxDateTimeList = new List<DateTime>();
        //        //MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
        //        //model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
        //        //if (MaxDateTimeList != null)
        //        //{
        //        //    if (MaxDateTimeList.Count > 0)
        //        //    {
        //        //        //model.ReportInfo = "Note : This report is based on pre processed data considered upto : " + MaxDateTimeList.Max();
        //        //        model.MaxDate = MaxDateTimeList.Max();
        //        //    }
        //        //    else
        //        //    {
        //        //        model.ReportInfo = "";
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    model.ReportInfo = "";
        //        //}
        //        List<DateTime> MaxDateTimeList = new List<DateTime>();
        //        MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
        //        //model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
        //        if (MaxDateTimeList != null)
        //        {

        //            if (MaxDateTimeList.Count > 0)
        //            {
        //                if (model.Stamp5DateTime.Date == DateTime.Now.Date && model.ToDate.Date == DateTime.Now.Date)
        //                {
        //                    model.ReportInfo = "";
        //                }
        //                else
        //                {
        //                    model.ReportInfo = "Note : This report is based on pre processed data considered upto : " + MaxDateTimeList.Max();

        //                }
        //                model.MaxDate = MaxDateTimeList.Max();
        //            }
        //            else
        //            {
        //                model.ReportInfo = "";
        //            }
        //        }
        //        else
        //        {
        //            model.ReportInfo = "";
        //        }
        //        return model;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //        if (searchDBContext != null)
        //        {
        //            searchDBContext.Dispose();
        //        }
        //    }
        //}

        /// <summary>
        /// Returns TodaysTotalDocsRegDetailsTable to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TodaysTotalDocsRegDetailsTable GetTodaysTotalDocumentsRegisteredDetails(TodaysDocumentsRegisteredReqModel model)
         {
            try
            {
                int count = 1;
                TodaysTotalDocsRegDetailsTable tableModel = new TodaysTotalDocsRegDetailsTable();
                TodaysDocumentsRegisteredDetailsModel TodaysDocsRegDetails = null;
                List<TodaysDocumentsRegisteredDetailsModel> TodaysDocumentsRegisteredDetailsList = new List<TodaysDocumentsRegisteredDetailsModel>();

                //dbContext = new KaveriEntities();
                //var todaysDocRegList = dbContext.USP_RPT_TODAYS_TOTAL_DOCUMENTS_REGISTERED_SUMMARY(model.DROfficeID, model.SROfficeID, model.Stamp5DateTime,model.ToDate).ToList();
                searchDBContext = new KaigrSearchDB();
                //Added by Madhusoodan on 29-04-2020
                //Added model.isDRO & model.DocumentTypeID

                int i = 0;
                if (model.SpecialSP == "true")
                {
                    var todaysDocRegList = searchDBContext.USP_RPT_TODAYS_TOTAL_DOCUMENTS_REGISTERED_SUMMARY_TODAYPRESENTED(model.DROfficeID, model.SROfficeID, model.Stamp5DateTime, model.ToDate, model.isDRO, model.DocumentTypeID).ToList().OrderBy(x => x.DistrictName);//.OrderByDescending(x=>x.SRONAMEE);
                    var todaysDocRegGroupBy = todaysDocRegList.GroupBy(x => x.DistrictName).Select(c => new { DName = c.Key, DCount = c.Count() });
                    foreach (var item in todaysDocRegList)
                    {
                        TodaysDocsRegDetails = new TodaysDocumentsRegisteredDetailsModel();
                        TodaysDocsRegDetails.RegistrationFee = item.REGISTRATIONFEE;
                        TodaysDocsRegDetails.SRNo = count++;

                        if (item.SRONAMEE != null)
                            TodaysDocsRegDetails.SROName = item.SRONAMEE;
                        else
                            TodaysDocsRegDetails.SROName = "-";

                        TodaysDocsRegDetails.StampDuty = item.STAMPDUTY;

                        TodaysDocsRegDetails.Total = item.STAMPDUTY + item.REGISTRATIONFEE;
                        TodaysDocsRegDetails.Documents = Convert.ToInt32(item.NO_OF_DOCUMENTS);
                        TodaysDocsRegDetails.str_StampDuty = item.STAMPDUTY.ToString("F");
                        TodaysDocsRegDetails.str_RegistrationFee = item.REGISTRATIONFEE.ToString("F");
                        TodaysDocsRegDetails.str_Total = (item.STAMPDUTY + item.REGISTRATIONFEE).ToString("F");
                        TodaysDocsRegDetails.District = string.IsNullOrEmpty(item.DistrictName) ? "" : item.DistrictName;
                        //Logic for merging District Column for SROs of same District
                        if (i == 0)
                        {
                            i = todaysDocRegGroupBy.Where(x => x.DName == item.DistrictName).Select(c => c.DCount).FirstOrDefault();
                            TodaysDocsRegDetails.DistrictNameInExcel = "<td style='text-align:center; vertical-align:middle;' rowspan=" + i + "><label>" + (string.IsNullOrEmpty(item.DistrictName) ? "" : item.DistrictName) + "</label></td>";
                            i--;
                        }
                        else
                        {
                            i--;
                        }
                        TodaysDocsRegDetails.RegistrationFee = item.REGISTRATIONFEE;
                        TodaysDocsRegDetails.StampDuty = item.STAMPDUTY;
                        TodaysDocsRegDetails.Total = item.REGISTRATIONFEE + item.STAMPDUTY;
                        TodaysDocumentsRegisteredDetailsList.Add(TodaysDocsRegDetails);
                        tableModel.TotalDocuments = tableModel.TotalDocuments + Convert.ToInt32(item.NO_OF_DOCUMENTS);
                        tableModel.TotalRegFee = tableModel.TotalRegFee + item.REGISTRATIONFEE;
                        tableModel.TotalStampDuty = tableModel.TotalStampDuty + item.STAMPDUTY;
                        tableModel.Total = tableModel.Total + TodaysDocsRegDetails.Total;
                    }
                }

                else
                {
                    var todaysDocRegList = searchDBContext.USP_RPT_TODAYS_TOTAL_DOCUMENTS_REGISTERED_SUMMARY(model.DROfficeID, model.SROfficeID, model.Stamp5DateTime, model.ToDate, model.isDRO, model.DocumentTypeID).ToList().OrderBy(x => x.DistrictName);//.OrderByDescending(x=>x.SRONAMEE);
                    var todaysDocRegGroupBy = todaysDocRegList.GroupBy(x => x.DistrictName).Select(c => new { DName = c.Key, DCount = c.Count() });
                    foreach (var item in todaysDocRegList)
                    {
                        TodaysDocsRegDetails = new TodaysDocumentsRegisteredDetailsModel();
                        TodaysDocsRegDetails.RegistrationFee = item.REGISTRATIONFEE;
                        TodaysDocsRegDetails.SRNo = count++;

                        if (item.SRONAMEE != null)
                            TodaysDocsRegDetails.SROName = item.SRONAMEE;
                        else
                            TodaysDocsRegDetails.SROName = "-";

                        TodaysDocsRegDetails.StampDuty = item.STAMPDUTY;

                        TodaysDocsRegDetails.Total = item.STAMPDUTY + item.REGISTRATIONFEE;
                        TodaysDocsRegDetails.Documents = Convert.ToInt32(item.NO_OF_DOCUMENTS);
                        TodaysDocsRegDetails.str_StampDuty = item.STAMPDUTY.ToString("F");
                        TodaysDocsRegDetails.str_RegistrationFee = item.REGISTRATIONFEE.ToString("F");
                        TodaysDocsRegDetails.str_Total = (item.STAMPDUTY + item.REGISTRATIONFEE).ToString("F");
                        TodaysDocsRegDetails.District = string.IsNullOrEmpty(item.DistrictName) ? "" : item.DistrictName;
                        //Logic for merging District Column for SROs of same District
                        if (i == 0)
                        {
                            i = todaysDocRegGroupBy.Where(x => x.DName == item.DistrictName).Select(c => c.DCount).FirstOrDefault();
                            TodaysDocsRegDetails.DistrictNameInExcel = "<td style='text-align:center; vertical-align:middle;' rowspan=" + i + "><label>" + (string.IsNullOrEmpty(item.DistrictName) ? "" : item.DistrictName) + "</label></td>";
                            i--;
                        }
                        else
                        {
                            i--;
                        }
                        TodaysDocsRegDetails.RegistrationFee = item.REGISTRATIONFEE;
                        TodaysDocsRegDetails.StampDuty = item.STAMPDUTY;
                        TodaysDocsRegDetails.Total = item.REGISTRATIONFEE + item.STAMPDUTY;
                        TodaysDocumentsRegisteredDetailsList.Add(TodaysDocsRegDetails);
                        tableModel.TotalDocuments = tableModel.TotalDocuments + Convert.ToInt32(item.NO_OF_DOCUMENTS);
                        tableModel.TotalRegFee = tableModel.TotalRegFee + item.REGISTRATIONFEE;
                        tableModel.TotalStampDuty = tableModel.TotalStampDuty + item.STAMPDUTY;
                        tableModel.Total = tableModel.Total + TodaysDocsRegDetails.Total;
                    }
                }
                
                //var todaysDocRegList = searchDBContext.USP_RPT_TODAYS_TOTAL_DOCUMENTS_REGISTERED_SUMMARY(model.DROfficeID, model.SROfficeID, model.Stamp5DateTime, model.ToDate, model.isDRO, model.DocumentTypeID).ToList().OrderBy(x => x.DistrictName);//.OrderByDescending(x=>x.SRONAMEE);
                //var todaysDocRegGroupBy = todaysDocRegList.GroupBy(x => x.DistrictName).Select(c => new { DName = c.Key, DCount = c.Count() });



                tableModel.TotalRegFee = tableModel.TotalRegFee;
                tableModel.TotalStampDuty = tableModel.TotalStampDuty;
                tableModel.Total = tableModel.Total;
                tableModel.str_TotalRegFee = tableModel.TotalRegFee.ToString("F");
                tableModel.str_TotalStampDuty = tableModel.TotalStampDuty.ToString("F");
                tableModel.str_Total = tableModel.Total.ToString("F");
                tableModel.TotalNoOfRecords = TodaysDocumentsRegisteredDetailsList.Count();
                tableModel.GenerationDateTime = Convert.ToString(DateTime.Now);
                tableModel.TodaysTotalDocsRegTableList = TodaysDocumentsRegisteredDetailsList;
                //tableModel.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
                List<DateTime> MaxDateTimeList = new List<DateTime>();
                MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
                //model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
                if (MaxDateTimeList != null)
                {

                    if (MaxDateTimeList.Count > 0)
                    {
                        if (model.Stamp5DateTime.Date == DateTime.Now.Date && model.ToDate.Date == DateTime.Now.Date)
                        {
                            tableModel.ReportInfo = "";
                        }
                        else
                        {
                            tableModel.ReportInfo = "Note : This report is based on pre processed data considered upto : " + MaxDateTimeList.Max();

                        }
                        tableModel.MaxDate = MaxDateTimeList.Max();
                    }
                    else
                    {
                        tableModel.ReportInfo = "";
                    }
                }
                else
                {
                    tableModel.ReportInfo = "";
                }
                Dictionary<int, int> DistrictWiseSROCountDict = new Dictionary<int, int>();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                DistrictWiseSROCountDict = objCommon.GetDistrictWiseSROCountDictionary(model.DROfficeID);
                tableModel.DistrictWiseSRODict = DistrictWiseSROCountDict;
                tableModel.DistrictWiseSRODictForSingleDistrict = objCommon.GetDistrictWiseSROCountDictForSingleDistrict(model.DROfficeID);
                return tableModel;

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
        }


        /// <summary>
        /// Returns SelectList Item
        /// </summary>
        /// <param name="sTextValue"></param>
        /// <param name="sOptionValue"></param>
        /// <returns></returns>
        public SelectListItem GetDefaultSelectListItem(string sTextValue, string sOptionValue)
        {
            return new SelectListItem
            {
                Text = sTextValue,
                Value = sOptionValue,
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
                if (searchDBContext != null)
                {
                    searchDBContext.Dispose();
                }
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}