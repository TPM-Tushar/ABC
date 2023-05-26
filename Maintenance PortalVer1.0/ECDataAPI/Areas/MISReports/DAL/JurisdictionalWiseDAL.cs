#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   JurisdictionalWiseDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.JurisdictionalWise;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECDataAPI.Entity.KaveriEntities;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class JurisdictionalWiseDAL : IJurisdictionalWise
    {
        KaveriEntities dbContext = null;
        KaigrSearchDB searchDBContext = null;

        /// <summary>
        /// Jurisdictional Wise View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>Jurisdictional Wise View model</returns>
        public JurisdictionalWiseModel JurisdictionalWiseView(int OfficeID)
        {
            JurisdictionalWiseModel resModel = new JurisdictionalWiseModel();
            try
            {
                dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                resModel.SROfficeList = new List<SelectListItem>();
                resModel.SROfficeList.Add(new SelectListItem { Text = "All", Value = "0" });

                //List<SelectListItem> SROMasterList = dbContext.SROMaster.Where(x => x.SROCode > 0).OrderBy(x => x.SRONameE).Select(i => new SelectListItem()
                //{
                //    Text = i.SRONameE,
                //    Value = i.SROCode.ToString()
                //}).ToList();

                List<SelectListItem> SROMasterList = dbContext.USP_RPT_JURISDICTIONAL_SRO().Select(i => new SelectListItem()
                {
                    Text = i.SRONameE,
                    Value = i.SROCode.ToString()
                }).ToList();

                resModel.SROfficeList.AddRange(SROMasterList);
                // resModel.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
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


                return resModel;

                // Commented due to requirement change on 18-07-2019 at 4:10 PM by Shubham Bhagat
                //resModel.DROfficeList = new List<SelectListItem>();
                //string kaveriCode = Kaveri1Code.ToString();
                //if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                //{
                //    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                //    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                //    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                //    string DroCode_string = Convert.ToString(DroCode);
                //    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                //    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                //    //resModel.DROfficeList.Add(droNameItem);
                //    resModel.SROfficeList.Add(sroNameItem);
                //}
                //else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                //{
                //    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                //    string DroCode_string = Convert.ToString(Kaveri1Code);
                //    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                //    //resModel.DROfficeList.Add(droNameItem);
                //    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "Select");
                //}
                //else
                //{
                //    SelectListItem select = new SelectListItem();
                //    select.Text = "Select";
                //    select.Value = "0";
                //    resModel.SROfficeList.Add(select);
                //    //resModel.DROfficeList = objCommon.GetDROfficesList("Select");
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

        /// <summary>
        /// Jurisdictional Wise Summary
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Jurisdictional Wise Summary Model</returns>
        //public JurisdictionalWiseSummary JurisdictionalWiseSummary(JurisdictionalWiseModel model)
        //{
        //    try
        //    {
        //        JurisdictionalWiseSummary jurisdictionalWiseSummary = new JurisdictionalWiseSummary();
        //        searchDBContext = new KaigrSearchDB();
        //        USP_RPT_JurisdictionalWiseSummary_Result result = searchDBContext.USP_RPT_JurisdictionalWiseSummary(model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).FirstOrDefault();
        //        int count = 1;
        //        if (result != null)
        //        {
        //            jurisdictionalWiseSummary.SerialNo = count;
        //            jurisdictionalWiseSummary.JurisdictionalOffice = String.IsNullOrEmpty(result.JSRONameE) ? String.Empty : result.JSRONameE;
        //            jurisdictionalWiseSummary.SROName = String.IsNullOrEmpty(result.SRONameE) ? String.Empty : result.SRONameE;

        //            if (result.NO_OF_DOCUMENTS == null)
        //                jurisdictionalWiseSummary.Documents = 0;
        //            else
        //                jurisdictionalWiseSummary.Documents = Convert.ToInt32(result.NO_OF_DOCUMENTS);
        //            jurisdictionalWiseSummary.RegistrationFees = result.REGISTRATIONFEE;
        //            jurisdictionalWiseSummary.StumpDuty = result.STAMPDUTY;
        //            jurisdictionalWiseSummary.Total = result.Total == null ? 0 : (decimal)result.Total;
        //        }
        //        return jurisdictionalWiseSummary;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (searchDBContext != null)
        //            searchDBContext.Dispose();
        //    }
        //}

        /// <summary>
        /// Jurisdictional Wise Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Jurisdictional Wise Detail Model List</returns>
        public JurisdictionalWiseDetailWrapper JurisdictionalWiseDetail(JurisdictionalWiseModel model)
        {
            try
            {
                JurisdictionalWiseDetailWrapper jurisdictionalWiseDetailWrapper = new JurisdictionalWiseDetailWrapper();
                jurisdictionalWiseDetailWrapper.JurisdictionalWiseDetailList = new List<JurisdictionalWiseDetail>();
                JurisdictionalWiseDetail jurisdictionalWiseDetail = null;
                searchDBContext = new KaigrSearchDB();
                //List<ECDataAPI.Entity.KaigrSearchDB.USP_RPT_JurisdictionalWiseDetail_Result> result = searchDBContext.USP_RPT_JurisdictionalWiseDetail(model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.StartLen).Take(model.TotalNum).ToList();
                List<ECDataAPI.Entity.KaigrSearchDB.USP_RPT_JurisdictionalWiseDetail_Result> result = searchDBContext.USP_RPT_JurisdictionalWiseDetail(model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList();

                if (result != null)
                {



                    jurisdictionalWiseDetailWrapper.TotalRecords = result.Count;
                    if (!model.IsExcel)
                    {
                        if (string.IsNullOrEmpty(model.SearchValue))
                        {
                            result = result.Skip(model.StartLen).Take(model.TotalNum).ToList();
                        }
                    }


                    int count = 1;
                    foreach (var item in result)
                    {
                        jurisdictionalWiseDetail = new JurisdictionalWiseDetail();
                        jurisdictionalWiseDetail.SerialNo = count++;
                        jurisdictionalWiseDetail.JurisdictionalOffice = String.IsNullOrEmpty(item.JSRONameE) ? String.Empty : item.JSRONameE;
                        jurisdictionalWiseDetail.SROName = String.IsNullOrEmpty(item.SRONameE) ? String.Empty : item.SRONameE;
                        jurisdictionalWiseDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.FINALREGISTRATIONNUMBER) ? String.Empty : item.FINALREGISTRATIONNUMBER;
                        //jurisdictionalWiseDetail.RegistrationFees = item.REGISTRATIONFEE == null ? 0 : (decimal)item.REGISTRATIONFEE;
                        //jurisdictionalWiseDetail.StumpDuty = item.STAMPDUTY == null ? 0 : (decimal)item.STAMPDUTY;
                        jurisdictionalWiseDetail.RegistrationFees = item.REGISTRATIONFEE;
                        jurisdictionalWiseDetail.StumpDuty = item.STAMPDUTY;
                        jurisdictionalWiseDetail.Total = item.Total == null ? 0 : (decimal)item.Total;
                        jurisdictionalWiseDetailWrapper.JurisdictionalWiseDetailList.Add(jurisdictionalWiseDetail);
                    }
                }
                return jurisdictionalWiseDetailWrapper;
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
        }

        /// <summary>
        /// Jurisdictional Wise Total Count
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Jurisdictional Wise Total Count</returns>
        public int JurisdictionalWiseTotalCount(JurisdictionalWiseModel model)
        {
            List<ECDataAPI.Entity.KaigrSearchDB.USP_RPT_JurisdictionalWiseDetail_Result> Result = null;
            try
            {
                searchDBContext = new KaigrSearchDB();
                Result = searchDBContext.USP_RPT_JurisdictionalWiseDetail(model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
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
        }

        /// <summary>
        /// Returns SroName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns> Returns SroName</returns>
        public string GetSroName(int SROfficeID)
        {
            string SroName = String.Empty;
            try
            {
                dbContext = new KaveriEntities();
                SroName = dbContext.SROMaster.Where(x => x.SROCode == SROfficeID).Select(x => x.SRONameE).FirstOrDefault();
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
            return SroName;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}