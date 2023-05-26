#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   SurchargeCessDetailsDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.SurchargeCessDetails;
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
    public class SurchargeCessDetailsDAL : ISurchargeCessDetails
    {
        KaveriEntities dbContext = null;

        /// <summary>
        /// Surcharge Cess Details View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>Surcharge Cess Details View model</returns>

        public SurchargeCessDetailsModel SurchargeCessDetailsView(int OfficeID)
        {
            SurchargeCessDetailsModel resModel = new SurchargeCessDetailsModel();
            try
            {
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                // Added Aricle dropdown
                resModel.NatureOfDocumentList = objCommon.GetNatureOfDocumentList();

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();

                resModel.SROfficeList = new List<SelectListItem>();
                resModel.DROfficeList = new List<SelectListItem>();
                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //string kaveriCode = Kaveri1Code.ToString();
                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(DroCode);
                    //sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    resModel.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    resModel.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });
                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //string DroCode_string = Convert.ToString(Kaveri1Code);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    resModel.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");
                }
                else
                {
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //SelectListItem select = new SelectListItem();
                    //select.Text = "All";
                    //select.Value = "0";
                    //resModel.SROfficeList.Add(select);
                    resModel.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                    resModel.DROfficeList = objCommon.GetDROfficesList("All");
                }

                //// Added Aricle dropdown
                //resModel.NatureOfDocumentList = new List<SelectListItem>();
                //resModel.NatureOfDocumentList.Add(new SelectListItem { Text = "Select", Value = "0" });
                //resModel.NatureOfDocumentList = dbContext.RegistrationArticles.Select(i => new SelectListItem()
                //{
                //    Text = i.ArticleNameE,
                //    Value = i.RegArticleCode.ToString()
                //}).ToList();
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
            return resModel;
        }

        //public SurchargeCessDetailsModel SurchargeCessDetailsView(int OfficeID)
        //{
        //    SurchargeCessDetailsModel resModel = new SurchargeCessDetailsModel();
        //    try
        //    {
        //        dbContext = new KaveriEntities();
        //        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        //        // Added Aricle dropdown
        //        resModel.NatureOfDocumentList = objCommon.GetNatureOfDocumentList();
        //        SelectListItem sroNameItem = new SelectListItem();
        //        SelectListItem droNameItem = new SelectListItem();
        //        DateTime now = DateTime.Now;
        //        var startDate = new DateTime(now.Year, now.Month, 1);
        //        resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

        //        short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
        //        int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

        //        resModel.SROfficeList = new List<SelectListItem>();
        //        resModel.DROfficeList = new List<SelectListItem>();
        //        string kaveriCode = Kaveri1Code.ToString();
        //        if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
        //        {
        //            string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
        //            int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(DroCode);

        //            sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            resModel.DROfficeList.Add(droNameItem);
        //            resModel.SROfficeList.Add(sroNameItem);
        //        }
        //        else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
        //        {
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

        //            string DroCode_string = Convert.ToString(Kaveri1Code);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            resModel.DROfficeList.Add(droNameItem);
        //            resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "All");
        //        }
        //        else
        //        {
        //            SelectListItem select = new SelectListItem();
        //            select.Text = "All";
        //            select.Value = "0";
        //            resModel.SROfficeList.Add(select);
        //            resModel.DROfficeList = objCommon.GetDROfficesList("All");
        //        }

        //        //// Added Aricle dropdown
        //        //resModel.NatureOfDocumentList = new List<SelectListItem>();
        //        //resModel.NatureOfDocumentList.Add(new SelectListItem { Text = "Select", Value = "0" });
        //        //resModel.NatureOfDocumentList = dbContext.RegistrationArticles.Select(i => new SelectListItem()
        //        //{
        //        //    Text = i.ArticleNameE,
        //        //    Value = i.RegArticleCode.ToString()
        //        //}).ToList();
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
        //    return resModel;
        //}

        /// <summary>
        /// Surcharge Cess Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Surcharge Cess Details wrapper model</returns>
        public SurchargeCessDetailWrapper SurchargeCessDetails(SurchargeCessDetailsModel model)
        {
            #region DUMMY DATA
            //SurchargeCessDetailWrapper surchargeCessDetailWrapper = new SurchargeCessDetailWrapper();
            //surchargeCessDetailWrapper.SurchargeCessDetailList = new List<SurchargeCessDetail>();

            //SurchargeCessDetail surchargeCessDetail = new SurchargeCessDetail();
            //surchargeCessDetail.SerialNo = 1;
            //surchargeCessDetail.FinalRegistrationNumber = "Final registration number";
            //surchargeCessDetail.PropertyDetails = "property details";
            //surchargeCessDetail.VillageNameE = "Vilage name";
            //surchargeCessDetail.Executant = "executant";
            //surchargeCessDetail.Claimant = "claimant";
            //surchargeCessDetail.PropertyValue = 1;
            //surchargeCessDetail.GovtDuty = 1;
            //surchargeCessDetail.AdditionalDuty = 1;
            //surchargeCessDetail.CessDuty = 1;
            //surchargeCessDetail.TotalStumpDuty = 1;
            //surchargeCessDetail.PaidStumpDuty = 1;
            //surchargeCessDetail.RegisteredDatetime = DateTime.Now.ToString();
            //surchargeCessDetail.ArticleNameE = "article name";

            //surchargeCessDetailWrapper.SurchargeCessDetailList.Add(surchargeCessDetail);
            //return surchargeCessDetailWrapper; 
            #endregion

            SurchargeCessDetailWrapper surchargeCessDetailWrapper = new SurchargeCessDetailWrapper();
            surchargeCessDetailWrapper.SurchargeCessDetailList = new List<SurchargeCessDetail>();
            SurchargeCessDetail surchargeCessDetail = null;
            try
            {
                dbContext = new KaveriEntities();
                //var TransactionList = dbContext.USP_RPT_SURCHARGE_CESS_DETAILS_BIFURCATED(model.DROfficeID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, model.NatureOfDocumentID).Skip(model.startLen).Take(model.totalNum).ToList();
                var TransactionList = dbContext.USP_RPT_SURCHARGE_CESS_DETAILS_BIFURCATED(model.DROfficeID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, model.NatureOfDocumentID).ToList();
                int count = 1;
                decimal Total_GovtDuty = 0;
                decimal Total_THREEPERCENT_GOVTDUTY = 0;
                decimal Total_TWOPERCENT_GOVTDUTY = 0;
                decimal Total_CessDuty = 0;
                decimal Total_TotalStumpDuty = 0;
                decimal Total_PaidStumpDuty = 0;
                decimal TotalOfTotal = 0;
                decimal PropertyValue = 0;

                if (TransactionList != null)
                {

                    surchargeCessDetailWrapper.TotalRecords = TransactionList.Count;
                    if (!model.IsExcel)
                    {
                        if (string.IsNullOrEmpty(model.SearchValue))
                        {
                            TransactionList = TransactionList.Skip(model.startLen).Take(model.totalNum).ToList();
                        }
                    }


                    foreach (var item in TransactionList)
                    {
                        surchargeCessDetail = new SurchargeCessDetail();

                        surchargeCessDetail.SerialNo = count++;
                        surchargeCessDetail.SroName = string.IsNullOrEmpty(item.SROName) ? "" : item.SROName;
                        surchargeCessDetail.ArticleNameE = String.IsNullOrEmpty(item.Article_Name) ? String.Empty : item.Article_Name;
                        //surchargeCessDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.Final_Registration_Number) ? String.Empty : item.Final_Registration_Number;
                        //surchargeCessDetail.PropertyDetails = String.IsNullOrEmpty(item.Property_Details) ? String.Empty : item.Property_Details;
                        // surchargeCessDetail.VillageNameE = String.IsNullOrEmpty(item.Village_Name) ? String.Empty : item.Village_Name;
                        //surchargeCessDetail.Executant = String.IsNullOrEmpty(item.Executant) ? String.Empty : item.Executant;
                        //surchargeCessDetail.Claimant = String.IsNullOrEmpty(item.Claimant) ? String.Empty : item.Claimant;
                        surchargeCessDetail.PropertyValue = String.IsNullOrEmpty(item.PropertyValue) ? 0 : Convert.ToDecimal(item.PropertyValue);
                        surchargeCessDetail.GovtDuty = String.IsNullOrEmpty(item.Govt_Duty) ? 0 : Convert.ToDecimal(item.Govt_Duty);
                        surchargeCessDetail.THREEPERCENT_GOVTDUTY = String.IsNullOrEmpty(item.THREEPERCENT_GOVTDUTY) ? 0 : Convert.ToDecimal(item.THREEPERCENT_GOVTDUTY);
                        surchargeCessDetail.TWOPERCENT_GOVTDUTY = String.IsNullOrEmpty(item.TWOPERCENT_GOVTDUTY) ? 0 : Convert.ToDecimal(item.TWOPERCENT_GOVTDUTY);
                        surchargeCessDetail.CessDuty = String.IsNullOrEmpty(item.CESS_Duty) ? 0 : Convert.ToDecimal(item.CESS_Duty);
                        surchargeCessDetail.TotalStumpDuty = String.IsNullOrEmpty(item.Total_StampDuty) ? 0 : Convert.ToDecimal(item.Total_StampDuty);
                        //surchargeCessDetail.PaidStumpDuty = String.IsNullOrEmpty(item.Paid_StampDuty) ? 0 : Convert.ToDecimal(item.Paid_StampDuty);
                        //surchargeCessDetail.RegisteredDatetime = item.Registered_DateTime == null ? String.Empty : item.Registered_DateTime.ToString();

                        //surchargeCessDetail.Total = surchargeCessDetail.PropertyValue + surchargeCessDetail.GovtDuty + surchargeCessDetail.AdditionalDuty + surchargeCessDetail.CessDuty + surchargeCessDetail.TotalStumpDuty + surchargeCessDetail.PaidStumpDuty;
                        surchargeCessDetailWrapper.SurchargeCessDetailList.Add(surchargeCessDetail);

                        Total_GovtDuty = Total_GovtDuty + surchargeCessDetail.GovtDuty;
                        Total_TWOPERCENT_GOVTDUTY = Total_TWOPERCENT_GOVTDUTY + surchargeCessDetail.TWOPERCENT_GOVTDUTY;
                        Total_THREEPERCENT_GOVTDUTY = Total_THREEPERCENT_GOVTDUTY + surchargeCessDetail.THREEPERCENT_GOVTDUTY;
                        Total_CessDuty = Total_CessDuty + surchargeCessDetail.CessDuty;
                        Total_TotalStumpDuty = Total_TotalStumpDuty + surchargeCessDetail.TotalStumpDuty;
                        Total_PaidStumpDuty = Total_PaidStumpDuty + surchargeCessDetail.PaidStumpDuty;
                        TotalOfTotal = TotalOfTotal + surchargeCessDetail.Total;
                        PropertyValue = PropertyValue + surchargeCessDetail.PropertyValue;
                    }
                    surchargeCessDetailWrapper.Total_GovtDuty = Total_GovtDuty;
                    surchargeCessDetailWrapper.Total_TWOPERCENT_GOVTDUTY = Total_TWOPERCENT_GOVTDUTY;
                    surchargeCessDetailWrapper.Total_THREEPERCENT_GOVTDUTY = Total_THREEPERCENT_GOVTDUTY;
                    surchargeCessDetailWrapper.Total_CessDuty = Total_CessDuty;
                    surchargeCessDetailWrapper.Total_TotalStumpDuty = Total_TotalStumpDuty;
                    surchargeCessDetailWrapper.Total_PaidStumpDuty = Total_PaidStumpDuty;
                    surchargeCessDetailWrapper.TotalOfTotal = TotalOfTotal;
                    surchargeCessDetailWrapper.Total_PropertyValue = PropertyValue;
                }
                return surchargeCessDetailWrapper;
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
        /// Surcharge Cess Details Total Count
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Surcharge Cess Details Total Count</returns>
        public int SurchargeCessDetailsTotalCount(SurchargeCessDetailsModel model)
        {
            //return 1;
            List<SurchargeCessDetail> surchargeCessDetailList = new List<SurchargeCessDetail>();
            List<USP_RPT_SURCHARGE_CESS_DETAILS_BIFURCATED_Result> Result = null;
            try
            {
                dbContext = new KaveriEntities();
                Result = dbContext.USP_RPT_SURCHARGE_CESS_DETAILS_BIFURCATED(model.DROfficeID, model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate, model.NatureOfDocumentID).ToList();
                return Result.Count();
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
        /// Returns SroName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns> Returns SroName</returns>
        public string GetSroName(int SROfficeID)
        {
            SurchargeCessDetailsModel resModel = new SurchargeCessDetailsModel();
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
                {
                    dbContext.Dispose();
                }
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