#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   HighValuePropertiesDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.HighValueProperties;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
//using ECDataAPI.Entity.K;
using ECDataAPI.Entity.KaigrSearchDB;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ECDataAPI.Areas.MISReports.DAL
{
    public class HighValuePropertiesDAL : IHighValueProperties, IDisposable
    {
        private KaveriEntities dbContext;
        KaigrSearchDB searchDBContext = null;


        /// <summary>
        /// returns HighValueProperties Request Model
        /// </summary>
        /// <returns></returns>
        public HighValuePropertiesReqModel HighValuePropertiesView()
        {
            try
            {
                dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();

                List<SelectListItem> RangeList = new List<SelectListItem>();
                HighValuePropertiesReqModel model = new HighValuePropertiesReqModel();
                RangeList = GetRangeList();
                model.RangeList = RangeList;
                var finYearList = dbContext.USP_FINANCIAL_YEAR().ToList();

                model.FinYearList = dbContext.USP_FINANCIAL_YEAR().Select(i => new SelectListItem()
                {
                    Text = i.FYEAR,
                    Value = Convert.ToString(i.YEAR)
                }).ToList();

                List<DateTime> MaxDateTimeList = new List<DateTime>();
                MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();

                if (MaxDateTimeList != null)
                {
                    if (MaxDateTimeList.Count > 0)
                    {
                        model.ReportInfo = "Note : This report is based on pre processed data considered upto : " + MaxDateTimeList.Max();
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
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
        }

        /// <summary>
        /// returns HighValuePropDetailsResponseModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public HighValuePropDetailsResModel GetHighValuePropertyDetails(HighValuePropDetailsReqModel model)
        {
            try
            {
                //dbContext.Database.CommandTimeout = 200;
                HighValuePropDetailsResModel resModel = new HighValuePropDetailsResModel();
                List<HighValuePropDetailsModel> detailsList = new List<HighValuePropDetailsModel>();
                HighValuePropDetailsModel detailsModel = null;
                dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();
                int index = 1;
                int year = Convert.ToInt32(model.FinYearListID);
                decimal totalStampDuty = 0;
                decimal totalRegFee = 0;
                int totalDocsReg = 0;

                List<ECDataAPI.Entity.KaigrSearchDB.USP_RPT_HIGHVALUE_PROPERTIES_Result> list = new List<ECDataAPI.Entity.KaigrSearchDB.USP_RPT_HIGHVALUE_PROPERTIES_Result>();

                var HighValuePropertiesList = list;

                if (model.RangeID == Convert.ToInt32(ApiCommonEnum.RangeList.UptoTenLackhs))
                {
                    HighValuePropertiesList = searchDBContext.USP_RPT_HIGHVALUE_PROPERTIES(year, 0, 1000000).ToList();
                }
                else if (model.RangeID == Convert.ToInt32(ApiCommonEnum.RangeList.TenLackhsToOneCrore))
                {
                    HighValuePropertiesList = searchDBContext.USP_RPT_HIGHVALUE_PROPERTIES(year, 1000000, 10000000).ToList();
                }
                else if (model.RangeID == Convert.ToInt32(ApiCommonEnum.RangeList.OneCroreToFiveCrores))
                {
                    HighValuePropertiesList = searchDBContext.USP_RPT_HIGHVALUE_PROPERTIES(year, 10000000, 50000000).ToList();
                }
                else if (model.RangeID == Convert.ToInt32(ApiCommonEnum.RangeList.FiveCroresToTenCrores))
                {
                    HighValuePropertiesList = searchDBContext.USP_RPT_HIGHVALUE_PROPERTIES(year, 50000000, 100000000).ToList();
                }
                else if (model.RangeID == Convert.ToInt32(ApiCommonEnum.RangeList.MoreThanTenCrores))
                {
                    HighValuePropertiesList = searchDBContext.USP_RPT_HIGHVALUE_PROPERTIES(year, 100000000, 1000000000).ToList();
                }
                foreach (var item in HighValuePropertiesList)
                {
                    detailsModel = new HighValuePropDetailsModel();
                    detailsModel.SerialNo = index++;
                    if (item.FinancialYear != null)
                    {
                        detailsModel.FinYear = Convert.ToString(item.FinancialYear);
                    }
                    else
                    {
                        detailsModel.FinYear = "null";
                    }
                    detailsModel.SD = item.STAMPDUTY;
                    totalStampDuty = totalStampDuty + item.STAMPDUTY;


                    detailsModel.RF = item.REGISTRATIONFEE;
                    totalRegFee = totalRegFee + item.REGISTRATIONFEE;
                    detailsModel.str_RF = item.REGISTRATIONFEE.ToString("F");
                    detailsModel.str_SD = item.STAMPDUTY.ToString("F");
                    detailsModel.SD = item.STAMPDUTY;
                    detailsModel.RF = item.REGISTRATIONFEE;
                    //if (item.DocsRegistered != null)
                    //{
                    detailsModel.DC = item.DocsRegistered;
                    totalDocsReg = totalDocsReg + item.DocsRegistered;
                    //}
                    //else
                    //{
                    //    detailsModel.DC = 0;
                    //}

                    if (item.MonthName != null)
                    {
                        detailsModel.MonthName = item.MonthName;
                    }
                    else
                    {
                        detailsModel.MonthName = "null";

                    }

                    detailsModel.Total = item.REGISTRATIONFEE + item.STAMPDUTY;
                    detailsList.Add(detailsModel);

                }

                detailsModel = new HighValuePropDetailsModel();
                detailsModel.FinYear = "";
                detailsModel.MonthName = "Total";
                detailsModel.SD = totalStampDuty;
                detailsModel.RF = totalRegFee;
                detailsModel.DC = Convert.ToInt32(totalDocsReg);
                detailsModel.Total = totalStampDuty + totalRegFee;
                detailsList.Add(detailsModel);


                List<ECDataAPI.Entity.KaveriEntities.USP_FINANCIAL_YEAR_Result> lst = new List<ECDataAPI.Entity.KaveriEntities.USP_FINANCIAL_YEAR_Result>();

                lst = dbContext.USP_FINANCIAL_YEAR().ToList();

                foreach (var item in lst)
                {

                    if (item.YEAR == model.FinYearListID)
                    {
                        resModel.FinancialYear = item.FYEAR;

                    }

                }
                resModel.RangeList = detailsList;
                resModel.GenerationDateTime = DateTime.Now;
                //resModel.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
                List<DateTime> MaxDateTimeList = new List<DateTime>();
                MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
                        
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
                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }



        public List<SelectListItem> GetRangeList()
        {
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> RangeList = new List<SelectListItem>();
                SelectListItem range = new SelectListItem();
                SelectListItem List = new SelectListItem();
                RangeList.Add(objCommon.GetDefaultSelectListItem("Upto 10 Lakhs", Convert.ToInt32(ApiCommonEnum.RangeList.UptoTenLackhs).ToString()));
                RangeList.Add(objCommon.GetDefaultSelectListItem("10 Lakhs to 1 Crore", Convert.ToInt32(ApiCommonEnum.RangeList.TenLackhsToOneCrore).ToString()));
                RangeList.Add(objCommon.GetDefaultSelectListItem("1 Crore to 5 Crores", Convert.ToInt32(ApiCommonEnum.RangeList.OneCroreToFiveCrores).ToString()));
                RangeList.Add(objCommon.GetDefaultSelectListItem("5 Crores to 10 Crores", Convert.ToInt32(ApiCommonEnum.RangeList.FiveCroresToTenCrores).ToString()));
                RangeList.Add(objCommon.GetDefaultSelectListItem("More than 10 Crores", Convert.ToInt32(ApiCommonEnum.RangeList.MoreThanTenCrores).ToString()));
                return RangeList;

            }
            catch (Exception)
            {
                throw;
            }

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

    }
}