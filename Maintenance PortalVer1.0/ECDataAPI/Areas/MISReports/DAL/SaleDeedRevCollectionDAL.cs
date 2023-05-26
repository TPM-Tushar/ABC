#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   SaleDeedRevCollectionDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   Raman Kalegaonkar
    * Last Modified On  :   30-07-2019
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.SaleDeedRevCollection;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class SaleDeedRevCollectionDAL : ISaleDeedRevCollection, IDisposable
    {
        KaveriEntities dbContext = null;
        KaigrSearchDB searchDBContext = null;

        /// <summary>
        /// Returns SaleDeedRevCollectionModel Required to show SaleDeedRevCollectionView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>

        public SaleDeedRevCollectionModel SaleDeedRevCollectionView(int OfficeID)
        {
            SaleDeedRevCollectionModel resModel = new SaleDeedRevCollectionModel();

            try
            {
                dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();
                SelectListItem selectListItem = new SelectListItem();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> SROfficeList = new List<SelectListItem>();
                string FirstRecord = "All";
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                resModel.SROfficeList = new List<SelectListItem>();
                resModel.DROfficeList = new List<SelectListItem>();
                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                string kaveriCode = Kaveri1Code.ToString();
                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);



                    sroNameItem = GetDefaultSelectListItem(SroName, kaveriCode);
                    droNameItem = GetDefaultSelectListItem(DroName, DroCode_string);
                    resModel.DROfficeList.Add(droNameItem);
                    resModel.SROfficeList.Add(sroNameItem);
                }
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    string DroCode_string = Convert.ToString(Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    resModel.DROfficeList.Add(droNameItem);
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, FirstRecord);
                }
                else
                {

                    SelectListItem select = new SelectListItem();
                    select.Text = "All";
                    select.Value = "0";
                    SROfficeList.Add(select);
                    resModel.SROfficeList = SROfficeList;
                    resModel.DROfficeList = objCommon.GetDROfficesList("All");
                    // resModel.Stamp5Date = Convert.ToString(DateTime.Now);




                }
                // Financial Year
                var finYearList = dbContext.USP_FINANCIAL_YEAR().ToList();

                resModel.FinancialYearList = dbContext.USP_FINANCIAL_YEAR().Select(i => new SelectListItem()
                {
                    Text = i.FYEAR,
                    Value = Convert.ToString(i.YEAR)
                }).ToList();


                List<SelectListItem> MonthList1 = DateTimeFormatInfo
                       .InvariantInfo
                       .MonthNames
                       .Select((monthName, index) => new SelectListItem
                       {
                           Value = (index + 1).ToString(),
                           Text = monthName
                       }).ToList();
                MonthList1.RemoveAt(12);
                MonthList1.RemoveRange(0, 3);

                List<SelectListItem> MonthList = new List<SelectListItem>();
                MonthList = DateTimeFormatInfo
                       .InvariantInfo
                       .MonthNames
                       .Select((monthName, index) => new SelectListItem
                       {
                           Value = (index + 1).ToString(),
                           Text = monthName
                       }).ToList();
                MonthList.RemoveAt(12);
                MonthList.RemoveRange(3, 9);

                resModel.MonthList = new List<SelectListItem>();

                resModel.MonthList.AddRange(MonthList1);

                resModel.MonthList.AddRange(MonthList);

                resModel.PropertyTypeList = objCommon.GetPropertyTypeList();
                //resModel.BuildTypeList = objCommon.GetBuildTypeList();
                resModel.PropertyValueList = objCommon.GetPropertyValueList();
                //  resModel.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
                List<DateTime> MaxDateTimeList = new List<DateTime>();
                MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
                //model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
               
                if (MaxDateTimeList != null)
                {
                    if (MaxDateTimeList.Count > 0)
                    {
                        resModel.ReportInfo = "Note : This report is based on pre processed data considered upto : " + MaxDateTimeList.Max();
                        resModel.MaxDate = MaxDateTimeList.Max();
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
        /// returns SelectList Item
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

        /// <summary>
        /// returns List of SaleDeedRevCollectionDetail to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public SaleDeedRevCollectionOuterModel GetSaleDeedRevCollectionDetails(SaleDeedRevCollectionModel model)
        {

            SaleDeedRevCollectionOuterModel SaleDeedRevCollectionOuterModel = new SaleDeedRevCollectionOuterModel();
            SaleDeedRevCollectionDetail ReportsDetails = null;
            List<SaleDeedRevCollectionDetail> ReportsDetailsList = new List<SaleDeedRevCollectionDetail>();

            try
            {
                dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();
                int financialYearID = model.FinacialYearID == null ? 0 : Convert.ToInt32(model.FinacialYearID);
                var TransactionList = searchDBContext.USP_RPT_SALEDEED_REGISTERED(model.DROfficeID, model.SROfficeID, financialYearID, model.PropertyTypeID, model.PropertyValueID).Skip(model.startLen).Take(model.totalNum).ToList();

                int counter = 1;
                decimal TotalRegFee = 0;
                decimal TotalStampDuty = 0;
                decimal TotalSum = 0;
                int TotalDocuments = 0;

                foreach (var item in TransactionList)
                {

                    ReportsDetails = new SaleDeedRevCollectionDetail();
                    ReportsDetails.SerialNo = counter++;
                    //ReportsDetails.DistrictName = string.IsNullOrEmpty(item.DistrictName) ? "null" : item.DistrictName;
                    //ReportsDetails.SROName = string.IsNullOrEmpty(item.SRONameE) ? "null" : item.SRONameE;

                    ReportsDetails.DocumentsRegistered = item.NO_OF_DOCUMENTS;

                    ReportsDetails.StampDuty = item.STAMPDUTY;
                    ReportsDetails.RegistrationFee = item.REGISTRATIONFEE;
                    decimal total = item.STAMPDUTY + item.REGISTRATIONFEE;
                    ReportsDetails.Total = total;
                    TotalRegFee = item.REGISTRATIONFEE + TotalRegFee;
                    TotalStampDuty = item.STAMPDUTY + TotalStampDuty;
                    TotalSum = total + TotalSum;
                    TotalDocuments = TotalDocuments + item.NO_OF_DOCUMENTS;
                    ReportsDetails.MonthName = item.MonthName;
                    ReportsDetailsList.Add(ReportsDetails);
                }

                SaleDeedRevCollectionOuterModel.SaleDeedRevCollList = ReportsDetailsList;
                SaleDeedRevCollectionOuterModel.TotalRegFee = TotalRegFee;
                SaleDeedRevCollectionOuterModel.TotalStampDuty = TotalStampDuty;
                SaleDeedRevCollectionOuterModel.TotalSum = TotalSum;
                SaleDeedRevCollectionOuterModel.TotalDocuments = TotalDocuments;
                SaleDeedRevCollectionOuterModel.SROCode = model.SROfficeID;
                //SaleDeedRevCollectionOuterModel.FinancialYear = Convert.ToInt32(dbContext.USP_FINANCIAL_YEAR().Where(x => x.YEAR == financialYearID).Select(x=>x.FYEAR));
                List<USP_FINANCIAL_YEAR_Result> lst = new List<USP_FINANCIAL_YEAR_Result>();
                lst = dbContext.USP_FINANCIAL_YEAR().ToList();
                foreach (var item in lst)
                {
                    if (item.YEAR == financialYearID)
                    {
                        SaleDeedRevCollectionOuterModel.FinancialYear = item.FYEAR;
                    }
                }
                //SaleDeedRevCollectionOuterModel.FinancialYear = Convert.ToInt32(Result.Where(x => x.YEAR == financialYearID).Select(x => x.FYEAR));
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
            return SaleDeedRevCollectionOuterModel;
        }

        /// <summary>
        /// Returns Total Count of GetSaleDeedRevCollectionDetails List
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public int GetSaleDeedRevCollectionDetailsTotalCount(SaleDeedRevCollectionModel model)
        {

            List<SaleDeedRevCollectionModel> indexIIReportsDetailsList = new List<SaleDeedRevCollectionModel>();
            int Count = 0;
            try
            {
                searchDBContext = new KaigrSearchDB();
                int financialYearID = model.FinacialYearID == null ? 0 : Convert.ToInt32(model.FinacialYearID);
                var TransactionList = searchDBContext.USP_RPT_SALEDEED_REGISTERED(model.DROfficeID, model.SROfficeID, financialYearID, model.PropertyTypeID, model.PropertyValueID).ToList();

                Count = TransactionList.Count;
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
            return Count;
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

