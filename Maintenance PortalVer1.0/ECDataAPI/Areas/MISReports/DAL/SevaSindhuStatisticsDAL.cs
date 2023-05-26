/*
 * Author: Rushikesh Chaudhari
 * Class Name: SevaSindhuStatisticsDAL.cs
 */

using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomModels.Models.MISReports.SevaSidhuStatistics;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using System.Web.Mvc;
using System.Globalization;
using System.Data.Entity;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class SevaSindhuStatisticsDAL : ISevaSindhuStatistics, IDisposable
    {
        KaveriEntities dbContext = null;
        KaigrSearchDB searchDBContext = null;
        SevaSindhuStatisticsReportResModel ResultModel = new SevaSindhuStatisticsReportResModel();
        /// <summary>
        /// returns Seva Sindhu Statistics Report
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public SevaSindhuStatisticsReportModel SevaSindhuStatisticsReportView(int OfficeID)
        {
            SevaSindhuStatisticsReportModel resModel = new SevaSindhuStatisticsReportModel();
            ApiCommonFunctions objCommon = new ApiCommonFunctions();

            try
            {
                dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();
                SelectListItem selectListItem = new SelectListItem();
                string FirstRecord = "All";

                SelectListItem droNameItem = new SelectListItem();

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.fromDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate_Str = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                resModel.ArticleNameList = objCommon.GetRegistrationArticles(true);
                resModel.YearDropdown = objCommon.GetYearDropdown();
                resModel.MonthList = new List<SelectListItem>() { GetDefaultSelectListItem("Select", "0") };
                resModel.MonthList.AddRange(objCommon.GetMonthList());
                resModel.DROfficeList = new List<SelectListItem>();
                resModel.SROfficeList = new List<SelectListItem>();

                var officeMasterObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.LevelID, x.Kaveri1Code }).FirstOrDefault();

                if (officeMasterObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {

                    var srDetails = dbContext.SROMaster.Where(c => c.SROCode == officeMasterObj.Kaveri1Code).FirstOrDefault();
                    if (srDetails != null)
                        resModel.DROfficeList = (dbContext.DistrictMaster.Where(x => x.DistrictCode == srDetails.DistrictCode).OrderBy(c => c.DistrictNameE).Select(m => new SelectListItem { Value = m.DistrictCode.ToString(), Selected = true, Text = m.DistrictNameE }).ToList());
                    else
                        resModel.DROfficeList = new List<SelectListItem>();

                    resModel.SROfficeList = new List<SelectListItem>();
                    selectListItem = GetDefaultSelectListItem(srDetails.SRONameE, officeMasterObj.Kaveri1Code.ToString());
                    resModel.SROfficeList.Add(selectListItem);
                }
                else if (officeMasterObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == officeMasterObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    string DroCode_string = Convert.ToString(officeMasterObj.Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    resModel.DROfficeList.Add(droNameItem);
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(officeMasterObj.Kaveri1Code, FirstRecord);
                }
                else
                {
                    SelectListItem select = new SelectListItem();
                    //select.Text = "select";
                    //select.Value = "0";
                    //resModel.SROfficeList.Add(select);
                     resModel.DROfficeList = objCommon.GetDROfficesList("All");
                     //resModel.SROfficeList = objCommon.GetSROfficesList();
                     resModel.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });

                }

                SelectListItem item;

                //Get financial Year
                var finYearList = dbContext.USP_FINANCIAL_YEAR().ToList();
                resModel.FinYearList = new List<SelectListItem>();
                foreach (var finYear in finYearList)
                {
                    item = new SelectListItem();
                    item.Text = Convert.ToString(finYear.FYEAR);
                    item.Value = Convert.ToString(finYear.YEAR);
                    resModel.FinYearList.Add(item);
                }

                List<DateTime> MaxDateTimeList = new List<DateTime>();
                MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();

                if (MaxDateTimeList != null)
                {
                    if (MaxDateTimeList.Count > 0)
                    {
                        resModel.ReportInfo = "Note : This report is based on pre processed data considered upto : " + MaxDateTimeList.Max();
                        resModel.maxDate = MaxDateTimeList.Max();
                    }
                    else
                    {
                        resModel.ReportInfo = "";
                    }
                }
                else
                {
                    resModel.ReportInfo = "";
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }

            return resModel;

        }

        /// <summary>
        /// Returns  a selectList Item
        /// </summary>
        /// <param name="sTextValue"></param>
        /// <param name="sOptionValue"></param>
        /// <returns></returns>
        public SelectListItem GetDefaultSelectListItem(string sTextValue, string sOptionValue)
        {
            try
            {

                return new SelectListItem
                {
                    Text = sTextValue,
                    Value = sOptionValue,
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// returns SevaSindhuStatisticsReportDetailModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<SevaSindhuStatisticsReportDetailModel> SevaSindhuReportDetails(SevaSindhuStatisticsReportModel reportModel)
        {
            SevaSindhuStatisticsReportDetailModel resModel = null;
            List<SevaSindhuStatisticsReportDetailModel> SevaSindhuStatisticsDetailsList = new List<SevaSindhuStatisticsReportDetailModel>();
            List<SevaSindhuStatisticsReportDetailModel> Result = new List<SevaSindhuStatisticsReportDetailModel>();
            
            try
            {
                dbContext = new KaveriEntities();
                resModel = new SevaSindhuStatisticsReportDetailModel();
                string DroName = string.Empty;
                string SroName = string.Empty;

                //For excel download: To display DRO in Excel
                if (reportModel.isExcelDownload)
                {
                    DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == reportModel.DROfficeID).Select(x => x.DistrictNameE).FirstOrDefault();
                    SroName = dbContext.SROMaster.Where(c => c.SROCode == reportModel.SROfficeID).Select(x => x.SRONameE).FirstOrDefault();
                }

                //
                if (reportModel.SROfficeID != 0 && reportModel.DROfficeID != 0)
                {
                    var result = (from t1 in dbContext.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS
                                  join t2 in dbContext.SROMaster on t1.SROCode equals t2.SROCode
                                  where t1.DataReceivedDate != null
                                  && DbFunctions.TruncateTime(t1.DataReceivedDate) >= DbFunctions.TruncateTime(reportModel.fromDateTime)
                                  && DbFunctions.TruncateTime(t1.DataReceivedDate) <= DbFunctions.TruncateTime(reportModel.ToDate)
                                  && t2.SROCode == reportModel.SROfficeID
                                  && t2.DistrictCode == reportModel.DROfficeID

                                  group t1 by new { t1.SROCode, t2.SRONameE, ApplicationReceivedDate = DbFunctions.TruncateTime(t1.DataReceivedDate) } into g
                                  orderby g.Key.ApplicationReceivedDate
                                  select new
                                  {
                                      SRONameE = g.Key.SRONameE,
                                      ApplicationReceivedDate = DbFunctions.TruncateTime(g.Key.ApplicationReceivedDate),
                                      NoOfApplicationReceived = g.Count(x => x.DataReceivedDate != null),
                                      NoOfApplicationProcessed = g.Count(x => x.ApplicationAcceptDateTime != null),
                                      //NoOfApplicationRegistered = g.Count(x => x.ApplicationRegistrationDateTime != null),
                                      NoOfApplicationRegistered = g.Count(x => x.IsApplicationRegistered),
                                      NoOfApplicationRejected = g.Count(x => x.ApplicationRejectDateTime != null)
                                  }).ToList();

                    int count = 1;

                    foreach (var item in result)
                    {
                        resModel = new SevaSindhuStatisticsReportDetailModel();
                        resModel.SRNo = count++;
                        resModel.SROoffice = item.SRONameE;
                        string date = Convert.ToDateTime(item.ApplicationReceivedDate).ToString("dd/MM/yyyy");
                        resModel.Application_received_date = date;
                        resModel.No_of_Application_Received = item.NoOfApplicationReceived;//String.IsNullOrEmpty(item.No_of_Application_Received) ? "--" : Convert.ToDateTime(item.No_of_Application_Received).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.No_of_Application_Processed = item.NoOfApplicationProcessed;
                        resModel.No_of_Application_Registered = item.NoOfApplicationRegistered;
                        resModel.No_of_Application_Rejected = item.NoOfApplicationRejected;
                        resModel.DistrictName = DroName;
                        resModel.SROName = SroName;
                        SevaSindhuStatisticsDetailsList.Add(resModel);
                    }
                }
                else if (reportModel.SROfficeID == 0 && reportModel.DROfficeID != 0)
                {
                    var result = (from t1 in dbContext.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS
                                  join t2 in dbContext.SROMaster on t1.SROCode equals t2.SROCode
                                  where t1.DataReceivedDate != null
                                  && DbFunctions.TruncateTime(t1.DataReceivedDate) >= DbFunctions.TruncateTime(reportModel.fromDateTime)
                                  && DbFunctions.TruncateTime(t1.DataReceivedDate) <= DbFunctions.TruncateTime(reportModel.ToDate)
                                  && t2.DistrictCode == reportModel.DROfficeID

                                  group t1 by new 
                                  { 
                                      //t1.SROCode,
                                      //t2.SRONameE,
                                      ApplicationReceivedDate = DbFunctions.TruncateTime(t1.DataReceivedDate) 
                                  } into g
                                  orderby g.Key.ApplicationReceivedDate
                                  select new
                                  {
                                      //SRONameE = g.Key.SRONameE,
                                      
                                      ApplicationReceivedDate = DbFunctions.TruncateTime(g.Key.ApplicationReceivedDate),
                                      NoOfApplicationReceived = g.Count(x => x.DataReceivedDate != null),
                                      NoOfApplicationProcessed = g.Count(x => x.ApplicationAcceptDateTime != null),
                                      //NoOfApplicationRegistered = g.Count(x => x.ApplicationRegistrationDateTime != null),
                                      NoOfApplicationRegistered = g.Count(x => x.IsApplicationRegistered),
                                      NoOfApplicationRejected = g.Count(x => x.ApplicationRejectDateTime != null)
                                  }).ToList();

                    int count = 1;

                    foreach (var item in result)
                    {
                        resModel = new SevaSindhuStatisticsReportDetailModel();
                        resModel.SRNo = count++;
                        //resModel.SROoffice = item.SRONameE;
                        string date = Convert.ToDateTime(item.ApplicationReceivedDate).ToString("dd/MM/yyyy");
                        resModel.Application_received_date = date;
                        resModel.No_of_Application_Received = item.NoOfApplicationReceived;//String.IsNullOrEmpty(item.No_of_Application_Received) ? "--" : Convert.ToDateTime(item.No_of_Application_Received).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.No_of_Application_Processed = item.NoOfApplicationProcessed;
                        resModel.No_of_Application_Registered = item.NoOfApplicationRegistered;
                        resModel.No_of_Application_Rejected = item.NoOfApplicationRejected;
                        resModel.DistrictName = DroName;
                        resModel.SROName = SroName;
                        SevaSindhuStatisticsDetailsList.Add(resModel);
                    }
                }
                else
                {
                    var result = (from t1 in dbContext.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS
                                  join t2 in dbContext.SROMaster on t1.SROCode equals t2.SROCode
                                  where t1.DataReceivedDate != null
                                  && DbFunctions.TruncateTime(t1.DataReceivedDate) >= DbFunctions.TruncateTime(reportModel.fromDateTime)
                                  && DbFunctions.TruncateTime(t1.DataReceivedDate) <= DbFunctions.TruncateTime(reportModel.ToDate)
                           
                                  group t1 by new 
                                  { 
                                      //t1.SROCode,
                                      //t2.SRONameE,
                                      ApplicationReceivedDate = DbFunctions.TruncateTime(t1.DataReceivedDate) 
                                  } into g
                                  orderby g.Key.ApplicationReceivedDate
                                  select new
                                  {
                                      //SRONameE = g.Key.SRONameE,
                                      ApplicationReceivedDate = DbFunctions.TruncateTime(g.Key.ApplicationReceivedDate),
                                      NoOfApplicationReceived = g.Count(x => x.DataReceivedDate != null),
                                      NoOfApplicationProcessed = g.Count(x => x.ApplicationAcceptDateTime != null),
                                      //NoOfApplicationRegistered = g.Count(x => x.ApplicationRegistrationDateTime != null),
                                      NoOfApplicationRegistered = g.Count(x => x.IsApplicationRegistered),
                                      NoOfApplicationRejected = g.Count(x => x.ApplicationRejectDateTime != null)
                                  }).ToList();

                    int count = 1;

                    foreach (var item in result)
                    {
                        resModel = new SevaSindhuStatisticsReportDetailModel();
                        resModel.SRNo = count++;
                        //resModel.SROoffice = item.SRONameE;
                        string date = Convert.ToDateTime(item.ApplicationReceivedDate).ToString("dd/MM/yyyy");
                        resModel.Application_received_date = date;
                        resModel.No_of_Application_Received = item.NoOfApplicationReceived;//String.IsNullOrEmpty(item.No_of_Application_Received) ? "--" : Convert.ToDateTime(item.No_of_Application_Received).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.No_of_Application_Processed = item.NoOfApplicationProcessed;
                        resModel.No_of_Application_Registered = item.NoOfApplicationRegistered;
                        resModel.No_of_Application_Rejected = item.NoOfApplicationRejected;
                        resModel.DistrictName = DroName;
                        resModel.SROName = SroName;
                        SevaSindhuStatisticsDetailsList.Add(resModel);
                    }
                }
               
            }
            catch (Exception e)
            {
                throw;
            }
            return SevaSindhuStatisticsDetailsList;
        }


        /// <summary>
        /// returns YearWise SevaSindhuStatisticsReportDetailModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<SevaSindhuStatisticsReportDetailModel> SevaSindhuStatisticsReportDetailsYearWise(SevaSindhuStatisticsReportModel reportModel)
        {
            SevaSindhuStatisticsReportDetailModel resModel = null;
            List<SevaSindhuStatisticsReportDetailModel> SevaSindhuStatisticsDetailsList = new List<SevaSindhuStatisticsReportDetailModel>();
            List<SevaSindhuStatisticsReportDetailModel> Result = new List<SevaSindhuStatisticsReportDetailModel>();

            try
            {
                dbContext = new KaveriEntities();
                resModel = new SevaSindhuStatisticsReportDetailModel();

                int finYear = reportModel.selectedYear; //get financial year from db
                DateTime financialYearStart = new DateTime(finYear, 4, 1); //from 1 April to selectedYear
                DateTime financialYearEnd = new DateTime(finYear + 1, 3, 31); //to 31 March to selectedYR + 1

                var result = (from tab2 in (
                                    from tab1 in (
                                            from data in dbContext.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS
                                            where DbFunctions.TruncateTime(data.DataReceivedDate) >= DbFunctions.TruncateTime(financialYearStart)
                                                && DbFunctions.TruncateTime(data.DataReceivedDate) <= DbFunctions.TruncateTime(financialYearEnd)
                                            select new
                                            {
                                                data.RequestID,
                                                data.SROCode,
                                                data.DataReceivedDate,
                                                data.ApplicationAcceptDateTime,
                                                data.ApplicationRegistrationDateTime,
                                                data.ApplicationRejectDateTime
                                            }
                                    )
                                    select new
                                    {
                                        tab1.SROCode,
                                        tab1.RequestID,
                                        DataReceivd = (tab1.DataReceivedDate != null ? 1 : 0),
                                        AcceptedDt = (tab1.ApplicationAcceptDateTime != null ? 1 : 0),
                                        RegisteredDt = (tab1.ApplicationRegistrationDateTime != null ? 1 : 0),
                                        RejectedDt = (tab1.ApplicationRejectDateTime != null ? 1 : 0)
                                    }
                                )
                              group tab2 by new { tab2.SROCode } into g
                              select new
                              {
                                  SROCode = g.Key.SROCode,
                                  SROOffice = (from sro in dbContext.SROMaster where sro.SROCode == g.Key.SROCode select sro.SRONameE).FirstOrDefault(),
                                  ApplicationReceivedYear = finYear,
                                  NoOfApplicationReceived = g.Sum(x => x.DataReceivd),
                                  NoOfApplicationProcessed = g.Sum(x => x.AcceptedDt),
                                  NoOfApplicationRegistered = g.Sum(x => x.RegisteredDt),
                                  NoOfApplicationRejected= g.Sum(x => x.RejectedDt)
                              }).ToList();

                int count = 1;

                foreach (var item in result)
                {
                    resModel = new SevaSindhuStatisticsReportDetailModel();
                    resModel.SRNo = count++;
                    resModel.SROoffice = item.SROOffice;
                    int nextYear = item.ApplicationReceivedYear + 1;
                    string nextYearTrim = nextYear.ToString();
                    string finSelectedYear = item.ApplicationReceivedYear.ToString() + " - " + nextYearTrim.Substring(nextYearTrim.Length - 2);
                    resModel.Application_Received_Year = finSelectedYear;
                    resModel.No_of_Application_Received = item.NoOfApplicationReceived;//String.IsNullOrEmpty(item.No_of_Application_Received) ? "--" : Convert.ToDateTime(item.No_of_Application_Received).ToString("dd/MM/yyyy  HH:mm:ss");
                    resModel.No_of_Application_Processed = item.NoOfApplicationProcessed;
                    resModel.No_of_Application_Registered = item.NoOfApplicationRegistered;
                    resModel.No_of_Application_Rejected = item.NoOfApplicationRejected;

                    SevaSindhuStatisticsDetailsList.Add(resModel);
                }
            }
            catch (Exception e)
            {

            }
            return SevaSindhuStatisticsDetailsList;
        }
 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                    dbContext.Dispose();
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public List<SevaSindhuStatisticsReportDetailModel> LoadSevaSindhuStatisticsReportTblMonthWise(SevaSindhuStatisticsReportModel reportModel)
        {
            SevaSindhuStatisticsReportDetailModel resModel = null;
            List<SevaSindhuStatisticsReportDetailModel> SevaSindhuStatisticsDetailsList = new List<SevaSindhuStatisticsReportDetailModel>();
            List<SevaSindhuStatisticsReportDetailModel> Result = new List<SevaSindhuStatisticsReportDetailModel>();
            
            try
            {
                dbContext = new KaveriEntities();
                resModel = new SevaSindhuStatisticsReportDetailModel();

                string DroName = string.Empty;
                string SroName = string.Empty;

                //For excel download: To display DRO and SRO in Excel
                if (reportModel.isExcelDownload)
                {
                    DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == reportModel.DROfficeID).Select(x => x.DistrictNameE).FirstOrDefault();
                    SroName = dbContext.SROMaster.Where(c => c.SROCode == reportModel.SROfficeID).Select(x => x.SRONameE).FirstOrDefault();
                }


                int selectedYear = reportModel.selectedYear; //get financial year from db
                DateTime financialYearStart = new DateTime(selectedYear, 4, 1); //from 1 April to selectedYear
                DateTime financialYearEnd = new DateTime(selectedYear + 1, 3, 31); //to 31 March to selectedYR + 1
                
                // list of all months and years in the financial year 1 April to 31 March
                var allMonths = Enumerable.Range(0, 12)
                            .Select(i => financialYearStart.AddMonths(i))
                            .TakeWhile(date => date <= financialYearEnd)
                            .Select(date => new { Year = date.Year, Month = date.Month, MonthName = date.ToString("MMMM") });

                if (reportModel.SROfficeID != 0 && reportModel.DROfficeID != 0)
                {
                    //finding data from FromDate to ToDate of financial year
                    var query = (from t1 in dbContext.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS
                                 join t2 in dbContext.SROMaster on t1.SROCode equals t2.SROCode
                                 where t1.DataReceivedDate != null
                                     && t1.DataReceivedDate >= financialYearStart
                                     && t1.DataReceivedDate <= financialYearEnd
                                     && t2.SROCode == reportModel.SROfficeID
                                     && t2.DistrictCode == reportModel.DROfficeID

                                 group t1 by new
                                 {
                                     Month = t1.DataReceivedDate.Month,
                                     SRONameE = t2.SRONameE,
                                     Year = t1.DataReceivedDate.Year
                                 } into g
                                 orderby g.Key.Month
                                 select new
                                 {
                                     SROOffice = g.Key.SRONameE,
                                     Month = g.Key.Month,
                                     Year = g.Key.Year,
                                     NoOfApplicationReceived = g.Count(x => x.DataReceivedDate != null),
                                     NoOfApplicationProcessed = g.Count(x => x.ApplicationAcceptDateTime != null),
                                     //NoOfApplicationRegistered = g.Count(x => x.ApplicationRegistrationDateTime != null),
                                     NoOfApplicationRegistered = g.Count(x => x.IsApplicationRegistered),
                                     NoOfApplicationRejected = g.Count(x => x.ApplicationRejectDateTime != null)
                                 }).ToList();

                    // if found_print all the month from 1 April to 31 march with data and without data
                    // left join the data query with the all months list to include the months with no data
                    var result = (from month in allMonths
                                  join data in query on new { month.Month, month.Year } equals new { data.Month, Year = data.Year } into gj
                                  from subdata in gj.DefaultIfEmpty()
                                  select new
                                  {
                                      SROOffice = subdata?.SROOffice,
                                      Month = month.Month,
                                      MonthName = month.MonthName,
                                      Year = month.Year,
                                      NoOfApplicationReceived = subdata?.NoOfApplicationReceived ?? 0,
                                      NoOfApplicationProcessed = subdata?.NoOfApplicationProcessed ?? 0,
                                      NoOfApplicationRegistered = subdata?.NoOfApplicationRegistered ?? 0,
                                      NoOfApplicationRejected = subdata?.NoOfApplicationRejected ?? 0
                                  }).ToList();

                    int count = 1;

                    foreach (var item in result)
                    {
                        resModel = new SevaSindhuStatisticsReportDetailModel();
                        resModel.SRNo = count++;
                        resModel.SROoffice = item.SROOffice; //String.IsNullOrEmpty(item.No_of_Application_Received) ? "--" : Convert.ToDateTime(item.No_of_Application_Received).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.Application_Received_Month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Month) + "-" + item.Year;
                        resModel.No_of_Application_Received = item.NoOfApplicationReceived;//String.IsNullOrEmpty(item.No_of_Application_Received) ? "--" : Convert.ToDateTime(item.No_of_Application_Received).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.No_of_Application_Processed = item.NoOfApplicationProcessed;
                        resModel.No_of_Application_Registered = item.NoOfApplicationRegistered;
                        resModel.No_of_Application_Rejected = item.NoOfApplicationRejected;
                        resModel.DistrictName = DroName;
                        resModel.SROName = SroName;
                        SevaSindhuStatisticsDetailsList.Add(resModel);
                    }
                }
                else if(reportModel.SROfficeID == 0 && reportModel.DROfficeID != 0)
                {
                    //finding data from FromDate to ToDate of financial year
                    var query = (from t1 in dbContext.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS
                                 join t2 in dbContext.SROMaster on t1.SROCode equals t2.SROCode
                                 where t1.DataReceivedDate != null
                                     && t1.DataReceivedDate >= financialYearStart
                                     && t1.DataReceivedDate <= financialYearEnd
                                     && t2.DistrictCode == reportModel.DROfficeID

                                 group t1 by new
                                 {
                                     Month = t1.DataReceivedDate.Month,
                                     //SRONameE = t2.SRONameE,
                                     Year = t1.DataReceivedDate.Year
                                 } into g
                                 orderby g.Key.Month
                                 select new
                                 {
                                     //SROOffice = g.Key.SRONameE,
                                     Month = g.Key.Month,
                                     Year = g.Key.Year,
                                     NoOfApplicationReceived = g.Count(x => x.DataReceivedDate != null),
                                     NoOfApplicationProcessed = g.Count(x => x.ApplicationAcceptDateTime != null),
                                     //NoOfApplicationRegistered = g.Count(x => x.ApplicationRegistrationDateTime != null),
                                     NoOfApplicationRegistered = g.Count( x => x.IsApplicationRegistered),
                                     NoOfApplicationRejected = g.Count(x => x.ApplicationRejectDateTime != null)
                                 }).ToList();

                    // if found_print all the month from 1 April to 31 march with data and without data
                    // left join the data query with the all months list to include the months with no data
                    var result = (from month in allMonths
                                  join data in query on new { month.Month, month.Year } equals new { data.Month, Year = data.Year } into gj
                                  from subdata in gj.DefaultIfEmpty()
                                  select new
                                  {
                                      //SROOffice = subdata?.SROOffice,
                                      Month = month.Month,
                                      MonthName = month.MonthName,
                                      Year = month.Year,
                                      NoOfApplicationReceived = subdata?.NoOfApplicationReceived ?? 0,
                                      NoOfApplicationProcessed = subdata?.NoOfApplicationProcessed ?? 0,
                                      NoOfApplicationRegistered = subdata?.NoOfApplicationRegistered ?? 0,
                                      NoOfApplicationRejected = subdata?.NoOfApplicationRejected ?? 0
                                  }).ToList();

                    int count = 1;

                    foreach (var item in result)
                    {
                        resModel = new SevaSindhuStatisticsReportDetailModel();
                        resModel.SRNo = count++;
                        //resModel.SROoffice = item.SROOffice; //String.IsNullOrEmpty(item.No_of_Application_Received) ? "--" : Convert.ToDateTime(item.No_of_Application_Received).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.Application_Received_Month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Month) + "-" + item.Year;
                        resModel.No_of_Application_Received = item.NoOfApplicationReceived;//String.IsNullOrEmpty(item.No_of_Application_Received) ? "--" : Convert.ToDateTime(item.No_of_Application_Received).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.No_of_Application_Processed = item.NoOfApplicationProcessed;
                        resModel.No_of_Application_Registered = item.NoOfApplicationRegistered;
                        resModel.No_of_Application_Rejected = item.NoOfApplicationRejected;
                        resModel.DistrictName = DroName;
                        resModel.SROName = SroName;
                        SevaSindhuStatisticsDetailsList.Add(resModel);
                    }
                }
                else
                {
                    //finding data from FromDate to ToDate of financial year
                    var query = (from t1 in dbContext.INT_SEVASINDHU_MAR_DATA_RECV_DETAILS
                                 join t2 in dbContext.SROMaster on t1.SROCode equals t2.SROCode
                                 where t1.DataReceivedDate != null
                                     && t1.DataReceivedDate >= financialYearStart
                                     && t1.DataReceivedDate <= financialYearEnd
                                     
                                 group t1 by new
                                 {
                                     Month = t1.DataReceivedDate.Month,
                                     //SRONameE = t2.SRONameE,
                                     Year = t1.DataReceivedDate.Year
                                 } into g
                                 orderby g.Key.Month
                                 select new
                                 {
                                     //SROOffice = g.Key.SRONameE,
                                     Month = g.Key.Month,
                                     Year = g.Key.Year,
                                     NoOfApplicationReceived = g.Count(x => x.DataReceivedDate != null),
                                     NoOfApplicationProcessed = g.Count(x => x.ApplicationAcceptDateTime != null),
                                     //NoOfApplicationRegistered = g.Count(x => x.ApplicationRegistrationDateTime != null),
                                     NoOfApplicationRegistered = g.Count( x => x.IsApplicationRegistered),
                                     NoOfApplicationRejected = g.Count(x => x.ApplicationRejectDateTime != null)
                                 }).ToList();

                    // if found_print all the month from 1 April to 31 march with data and without data
                    // left join the data query with the all months list to include the months with no data
                    var result = (from month in allMonths
                                  join data in query on new { month.Month, month.Year } equals new { data.Month, Year = data.Year } into gj
                                  from subdata in gj.DefaultIfEmpty()
                                  select new
                                  {
                                      //SROOffice = subdata?.SROOffice,
                                      Month = month.Month,
                                      MonthName = month.MonthName,
                                      Year = month.Year,
                                      NoOfApplicationReceived = subdata?.NoOfApplicationReceived ?? 0,
                                      NoOfApplicationProcessed = subdata?.NoOfApplicationProcessed ?? 0,
                                      NoOfApplicationRegistered = subdata?.NoOfApplicationRegistered ?? 0,
                                      NoOfApplicationRejected = subdata?.NoOfApplicationRejected ?? 0
                                  }).ToList();

                    int count = 1;

                    foreach (var item in result)
                    {
                        resModel = new SevaSindhuStatisticsReportDetailModel();
                        resModel.SRNo = count++;
                        //resModel.SROoffice = item.SROOffice; //String.IsNullOrEmpty(item.No_of_Application_Received) ? "--" : Convert.ToDateTime(item.No_of_Application_Received).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.Application_Received_Month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Month) + "-" + item.Year;
                        resModel.No_of_Application_Received = item.NoOfApplicationReceived;//String.IsNullOrEmpty(item.No_of_Application_Received) ? "--" : Convert.ToDateTime(item.No_of_Application_Received).ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.No_of_Application_Processed = item.NoOfApplicationProcessed;
                        resModel.No_of_Application_Registered = item.NoOfApplicationRegistered;
                        resModel.No_of_Application_Rejected = item.NoOfApplicationRejected;
                        resModel.DistrictName = DroName;
                        resModel.SROName = SroName;
                        SevaSindhuStatisticsDetailsList.Add(resModel);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return SevaSindhuStatisticsDetailsList;
        }
    }
}