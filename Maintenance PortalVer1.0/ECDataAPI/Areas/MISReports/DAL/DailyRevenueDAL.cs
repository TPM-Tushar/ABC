#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DailyRevenueDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.DailyRevenue;
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
    public class DailyRevenueDAL : IDailyRevenue, IDisposable
    {
        KaveriEntities dbContext = null;
        KaigrSearchDB searchDBContext = null;

        /// <summary>
        /// returns Daily Revenue Report
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public DailyRevenueReportReqModel DailyRevenueReport(int OfficeID)
        {
            DailyRevenueReportReqModel resModel = new DailyRevenueReportReqModel();

            try
            {
                dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();
                SelectListItem selectListItem = new SelectListItem();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                string FirstRecord = "All";

                SelectListItem droNameItem = new SelectListItem();

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.fromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate_Str = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //True added by Raman Kalegaonkar on 25-03-2020
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
                    //int FirstDistrict = 11;//Bagalkot
                    //resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(FirstDistrict, FirstRecord);
                    SelectListItem select = new SelectListItem();
                    select.Text = "All";
                    select.Value = "0";
                    resModel.SROfficeList.Add(select);
                    resModel.DROfficeList = objCommon.GetDROfficesList("All");
                }
                
                //Added by RamanK on 22-07-2019
                SelectListItem item;
                var finYearList = dbContext.USP_FINANCIAL_YEAR().ToList();
                resModel.FinYearList = new List<SelectListItem>();
                foreach (var finYear in finYearList)
                {
                    item = new SelectListItem();
                    item.Text = Convert.ToString(finYear.FYEAR);
                    item.Value = Convert.ToString(finYear.YEAR);
                    resModel.FinYearList.Add(item);

                }

                //resModel.maxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
                List<DateTime> MaxDateTimeList = new List<DateTime>();
                MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
                //model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
               
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
        /// returns DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<DailyRevenueReportDetailModel> DailyRevenueReportDetails(DailyRevenueReportReqModel model)
         {

            DailyRevenueReportDetailModel indexIIReportsDetails = null;
            List<DailyRevenueReportDetailModel> indexIIReportsDetailsList = new List<DailyRevenueReportDetailModel>();
            try
            {
                decimal TotalRegFee = 0;
                decimal TotalStampDuty = 0;
                decimal TotalSum = 0;
                int TotalDocuments = 0;
                searchDBContext = new KaigrSearchDB();
                var RevenueList = searchDBContext.USP_RPT_DAILY_REVENUE_RANGEWISE(model.DROfficeID, model.SROfficeID, model.ArticleID, model.fromDateTime, model.ToDate).ToList();

                int i = 1;
                foreach (var item in RevenueList)
                {
                    indexIIReportsDetails = new DailyRevenueReportDetailModel();
                    indexIIReportsDetails.SRNo = i++;
                    if (!string.IsNullOrEmpty(item.ARTICLENAMEE))
                    {
                        indexIIReportsDetails.ArticleName = item.ARTICLENAMEE;
                    }
                    else
                    {
                        indexIIReportsDetails.ArticleName = "null";
                    }
                    if (item.STAMPDUTY != null)
                    {
                        indexIIReportsDetails.StampDuty = Convert.ToDecimal(item.STAMPDUTY);
                    }
                    else
                    {
                        indexIIReportsDetails.StampDuty = 0;
                    }
                    if (item.REGISTRATIONFEE != null)
                    {
                        indexIIReportsDetails.RegistrationFee = Convert.ToDecimal(item.REGISTRATIONFEE);
                    }
                    else
                    {
                        indexIIReportsDetails.RegistrationFee = 0;
                    }

                    if (item.NO_OF_DOCUMENTS != null)
                    {
                        indexIIReportsDetails.Documents = Convert.ToInt32(item.NO_OF_DOCUMENTS);
                    }
                    else
                    {
                        indexIIReportsDetails.Documents = 0;
                    }
                    indexIIReportsDetails.TotalAmount = indexIIReportsDetails.StampDuty + indexIIReportsDetails.RegistrationFee;

                    TotalStampDuty = TotalStampDuty + indexIIReportsDetails.StampDuty;

                    TotalRegFee = TotalRegFee + indexIIReportsDetails.RegistrationFee;

                    TotalSum = TotalSum + indexIIReportsDetails.TotalAmount;
                    TotalDocuments = TotalDocuments + indexIIReportsDetails.Documents;

                    //Added by Madhusoodan on 11-05-2020
                    indexIIReportsDetails.districtName = string.IsNullOrEmpty(item.DISTRICT_NAME) ? string.Empty : item.DISTRICT_NAME;
                    indexIIReportsDetails.officeName = string.IsNullOrEmpty(item.OFFICE_NAME) ? string.Empty : item.OFFICE_NAME;
                    
                    indexIIReportsDetailsList.Add(indexIIReportsDetails);

                    
                }
                if (model.IsPDF == true || model.IsExcel == true)
                {

                    indexIIReportsDetails = new DailyRevenueReportDetailModel();
                    indexIIReportsDetails.SRNo = 0;

                    indexIIReportsDetails.ArticleName = "Total";
                    indexIIReportsDetails.StampDuty = TotalStampDuty;

                    indexIIReportsDetails.RegistrationFee = TotalRegFee;
                    indexIIReportsDetails.Documents = TotalDocuments;
                    indexIIReportsDetails.TotalAmount = TotalSum;

                    indexIIReportsDetailsList.Add(indexIIReportsDetails);

                }


                //List<DateTime> MaxDateTimeList = new List<DateTime>();
                //MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
                ////model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();

                //if (MaxDateTimeList != null)
                //{
                //    if (MaxDateTimeList.Count > 0)
                //    {
                //        resModel.ReportInfo = "Note : This report is based on pre processed data considered upto : " + MaxDateTimeList.Max();
                //    }
                //    else
                //    {
                //        resModel.ReportInfo = "";
                //    }
                //}
                //else
                //{
                //    resModel.ReportInfo = "";
                //}


            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }

            return indexIIReportsDetailsList;


        }

        /// <summary>
        /// returns TotalCount of DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DailyRevenueReportDetailsTotalCount(DailyRevenueReportReqModel model)
        {

            List<DailyRevenueReportDetailModel> indexIIReportsDetailsList = new List<DailyRevenueReportDetailModel>();
            int ResultCount = 0;
            try
            {
                searchDBContext = new KaigrSearchDB();
                var RevenueList = searchDBContext.USP_RPT_DAILY_REVENUE_RANGEWISE(model.DROfficeID, model.SROfficeID, model.ArticleID, model.fromDateTime, model.ToDate).ToList();
                ResultCount = RevenueList == null ? 0 : RevenueList.Count();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }

            return ResultCount;
        }

        /// <summary>
        /// returns DayWise DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<DailyRevenueReportDetailModel> DailyRevenueReportDetailsDayWise(DailyRevenueReportReqModel model)
        {

            DailyRevenueReportDetailModel indexIIReportsDetails = null;
            List<DailyRevenueReportDetailModel> indexIIReportsDetailsList = new List<DailyRevenueReportDetailModel>();
            try
            {
                decimal TotalRegFee = 0;
                decimal TotalStampDuty = 0;
                decimal TotalSum = 0;
                int TotalDocuments = 0;
                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                string sMonthName = objCommon.GetMonthList().Where(x => x.Value == model.selectedMonth.ToString()).Select(c => c.Text).FirstOrDefault();

                searchDBContext = new KaigrSearchDB();
                var RevenueList = searchDBContext.USP_RPT_DAILY_REVENUE_DAYWISE(model.DROfficeID, model.SROfficeID, model.ArticleID, model.selectedMonth, model.selectedYear).ToList();
                int i = 1;
                foreach (var item in RevenueList)
                {
                    indexIIReportsDetails = new DailyRevenueReportDetailModel();
                    indexIIReportsDetails.SRNo = i++;
                    if (!string.IsNullOrEmpty(item.ARTICLENAMEE))
                    {
                        indexIIReportsDetails.ArticleName = item.ARTICLENAMEE;
                    }
                    else
                    {
                        indexIIReportsDetails.ArticleName = "null";
                    }
                    indexIIReportsDetails.StampDuty = item.STAMPDUTY;
                    indexIIReportsDetails.RegistrationFee = item.REGISTRATIONFEE;
                    indexIIReportsDetails.Documents = item.NO_OF_DOCUMENTS;
                    if (!string.IsNullOrEmpty(item.registrationDate))
                    {
                        indexIIReportsDetails.DateValue = Convert.ToDateTime(item.registrationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); 
                    }
                    else
                    {
                        indexIIReportsDetails.DateValue = "null";
                    }
                    indexIIReportsDetails.SelectedMonthName = sMonthName;// used in excel or pdf
                    indexIIReportsDetails.TotalAmount = item.STAMPDUTY + item.REGISTRATIONFEE;

                    TotalRegFee = TotalRegFee + item.REGISTRATIONFEE;
                    TotalStampDuty = TotalStampDuty + item.STAMPDUTY;
                    TotalSum = TotalSum + indexIIReportsDetails.TotalAmount;
                    TotalDocuments = TotalDocuments + item.NO_OF_DOCUMENTS;
                    indexIIReportsDetailsList.Add(indexIIReportsDetails);
                }
                if (model.IsExcel == true || model.IsPDF == true)
                {
                    indexIIReportsDetails = new DailyRevenueReportDetailModel();
                    indexIIReportsDetails.ArticleName = "Total";
                    indexIIReportsDetails.StampDuty = TotalStampDuty;
                    indexIIReportsDetails.RegistrationFee = TotalRegFee;
                    indexIIReportsDetails.Documents = TotalDocuments;
                    //   indexIIReportsDetails.DateValue = item.registrationDate;
                    indexIIReportsDetails.SelectedMonthName = sMonthName;// used in excel or pdf
                    indexIIReportsDetails.TotalAmount = TotalSum;
                    indexIIReportsDetailsList.Add(indexIIReportsDetails);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
            return indexIIReportsDetailsList;
        }

        /// <summary>
        /// returns DayWise DailyRevenueReportDetailsTotalCount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DailyRevenueReportDetailsTotalCountDayWise(DailyRevenueReportReqModel model)
        {
            List<DailyRevenueReportDetailModel> indexIIReportsDetailsList = new List<DailyRevenueReportDetailModel>();
            //KaveriEntities dbContext = null;
            //List<USP_INDEX2_DETAILS_Result> Result = null;
            //long Amount = Convert.ToInt64(model.Amount);
            //try
            //{
            //    dbContext = new KaveriEntities();
            //    Result = dbContext.USP_INDEX2_DETAILS(model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, model.NatureOfDocumentID, Amount).ToList();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            //finally
            //{
            //    if (dbContext != null)
            //        dbContext.Dispose();
            //}
            //return Result.Count();
            return 0;
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

        /// <summary>
        /// returns DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<DailyRevenueReportDetailModel> LoadDailyRevenueReportTblMonthWise(DailyRevenueReportReqModel model)
        {

            DailyRevenueReportDetailModel DailyRevMonthWiseRptDetails = null;
            List<DailyRevenueReportDetailModel> DailyRevMonthWiseRptList = new List<DailyRevenueReportDetailModel>();
            try
            {
                decimal TotalRegFee = 0;
                decimal TotalStampDuty = 0;
                decimal TotalSum = 0;
                int TotalDocuments = 0;
                searchDBContext = new KaigrSearchDB();
                var RevenueList = searchDBContext.USP_RPT_DAILY_REVENUE_MONTHWISE(model.DROfficeID, model.SROfficeID, model.ArticleID, model.selectedYear).ToList();
                int i = 1;
                foreach (var item in RevenueList)
                {
                    DailyRevMonthWiseRptDetails = new DailyRevenueReportDetailModel();
                    DailyRevMonthWiseRptDetails.SRNo = i++;
                    if (!string.IsNullOrEmpty(item.ARTICLENAMEE))
                    {
                        DailyRevMonthWiseRptDetails.ArticleName = item.ARTICLENAMEE;
                    }
                    else
                    {
                        DailyRevMonthWiseRptDetails.ArticleName = "";
                    }
                    DailyRevMonthWiseRptDetails.StampDuty = item.STAMPDUTY;

                    DailyRevMonthWiseRptDetails.RegistrationFee = item.REGISTRATIONFEE;
                    DailyRevMonthWiseRptDetails.Documents = item.NO_OF_DOCUMENTS;
                    DailyRevMonthWiseRptDetails.TotalAmount = item.STAMPDUTY + item.REGISTRATIONFEE;

                    DailyRevMonthWiseRptDetails.SelectedMonthName = string.IsNullOrEmpty(item.registrationMonth) ? "" : item.registrationMonth;

                    TotalStampDuty = TotalStampDuty + item.STAMPDUTY;

                    TotalRegFee = TotalRegFee + item.REGISTRATIONFEE;

                    TotalSum = TotalSum + DailyRevMonthWiseRptDetails.TotalAmount;
                    TotalDocuments = TotalDocuments + item.NO_OF_DOCUMENTS;

                    DailyRevMonthWiseRptList.Add(DailyRevMonthWiseRptDetails);

                }
                if (model.IsPDF == true || model.IsExcel == true)
                {

                    DailyRevMonthWiseRptDetails = new DailyRevenueReportDetailModel();
                    DailyRevMonthWiseRptDetails.SRNo = 0;

                    DailyRevMonthWiseRptDetails.ArticleName = "Total";
                    DailyRevMonthWiseRptDetails.StampDuty = TotalStampDuty;

                    DailyRevMonthWiseRptDetails.RegistrationFee = TotalRegFee;
                    DailyRevMonthWiseRptDetails.Documents = TotalDocuments;
                    DailyRevMonthWiseRptDetails.TotalAmount = TotalSum;

                    DailyRevMonthWiseRptList.Add(DailyRevMonthWiseRptDetails);

                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
            return DailyRevMonthWiseRptList;
        }


        public List<DailyRevenueReportDetailModel> LoadDailyRevenueReportTblDocWise(DailyRevenueReportReqModel model)
        {

            DailyRevenueReportDetailModel DailyRevMonthWiseRptDetails = null;
            List<DailyRevenueReportDetailModel> DailyRevMonthWiseRptList = new List<DailyRevenueReportDetailModel>();
            try
            {
                decimal TotalPurchaseVal = 0;
                decimal TotalStampDuty = 0;
                decimal TotalRegistrationFee = 0;
                decimal Total_Total_StampDuty_RegistrationFee = 0;


                int TotalDocuments = 0;
                searchDBContext = new KaigrSearchDB();
                //var RevenueList = searchDBContext.USP_RPT_DAILY_REVENUE_DOCUMENTWISE(model.DROfficeID, model.SROfficeID, model.ArticleID, model.selectedYear).Skip(model.startLen).Take(model.totalNum).ToList();
                var RevenueList = searchDBContext.USP_RPT_DAILY_REVENUE_DOCUMENTWISE(model.DROfficeID, model.SROfficeID, model.ArticleID, model.selectedYear).ToList();
                int i = 1;
                foreach (var item in RevenueList)
                {
                    DailyRevMonthWiseRptDetails = new DailyRevenueReportDetailModel();
                    DailyRevMonthWiseRptDetails.SRNo = i++;
                    DailyRevMonthWiseRptDetails.FinancialYear = String.IsNullOrEmpty(item.FinancialYear) ? "" : item.FinancialYear;
                    DailyRevMonthWiseRptDetails.ArticleName = String.IsNullOrEmpty(item.RegArticleName) ? "" : item.RegArticleName;
                    DailyRevMonthWiseRptDetails.SROName = String.IsNullOrEmpty(item.SROName) ? "" : item.SROName;
                    DailyRevMonthWiseRptDetails.FinalRegistrationNumber = String.IsNullOrEmpty(item.FinalRegistrationNumber) ? "" : item.FinalRegistrationNumber;
                    DailyRevMonthWiseRptDetails.RegistrationDate = item.RegistrationDate == null ? "" : ((DateTime)item.RegistrationDate).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    DailyRevMonthWiseRptDetails.PurchaseValue = item.PurchaseValue == null ? 0 : (decimal)item.PurchaseValue;
                    DailyRevMonthWiseRptDetails.StampDuty = item.STAMPDUTY;
                    DailyRevMonthWiseRptDetails.RegistrationFee = item.REGISTRATIONFEE;
                    DailyRevMonthWiseRptDetails.Total_StampDuty_RegiFee = DailyRevMonthWiseRptDetails.StampDuty + DailyRevMonthWiseRptDetails.RegistrationFee;

                    TotalPurchaseVal = TotalPurchaseVal + DailyRevMonthWiseRptDetails.PurchaseValue;
                    TotalStampDuty = TotalStampDuty + DailyRevMonthWiseRptDetails.StampDuty;
                    TotalRegistrationFee = TotalRegistrationFee + DailyRevMonthWiseRptDetails.RegistrationFee;
                    Total_Total_StampDuty_RegistrationFee = Total_Total_StampDuty_RegistrationFee + DailyRevMonthWiseRptDetails.Total_StampDuty_RegiFee;

                    DailyRevMonthWiseRptList.Add(DailyRevMonthWiseRptDetails);

                }
                if (model.IsPDF == true || model.IsExcel == true)
                {

                    DailyRevMonthWiseRptDetails = new DailyRevenueReportDetailModel();
                    DailyRevMonthWiseRptDetails.SRNo = 0;
                    DailyRevMonthWiseRptDetails.FinancialYear = String.Empty;
                    DailyRevMonthWiseRptDetails.ArticleName = String.Empty;
                    DailyRevMonthWiseRptDetails.RegistrationDate = "Total";
                    DailyRevMonthWiseRptDetails.SROName = string.Empty;
                    DailyRevMonthWiseRptDetails.FinalRegistrationNumber = string.Empty;
                    DailyRevMonthWiseRptDetails.PurchaseValue = TotalPurchaseVal;
                    DailyRevMonthWiseRptDetails.StampDuty = TotalStampDuty;
                    DailyRevMonthWiseRptDetails.RegistrationFee = TotalRegistrationFee;
                    DailyRevMonthWiseRptDetails.Total_StampDuty_RegiFee = Total_Total_StampDuty_RegistrationFee;


                    DailyRevMonthWiseRptList.Add(DailyRevMonthWiseRptDetails);

                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
            return DailyRevMonthWiseRptList;
        }


        public int DailyRevenueReportDetailsTotalCountDocWise(DailyRevenueReportReqModel model)
        {
            searchDBContext = new KaigrSearchDB();
            try
            {
                var Result = searchDBContext.USP_RPT_DAILY_REVENUE_DOCUMENTWISE(model.DROfficeID, model.SROfficeID, model.ArticleID, model.selectedYear).ToList();
                if (Result != null)
                    return Result.Count();
                else
                    return 0;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
            //return 0;
        }


    }
}