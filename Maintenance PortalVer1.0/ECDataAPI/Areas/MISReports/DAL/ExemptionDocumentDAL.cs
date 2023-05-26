#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ExemptionDocumentDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.ExemptionDocument;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity;
using ECDataAPI.Entity.KaigrSearchDB;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ExemptionDocumentDAL : IExemptionDocument
    {
        KaveriEntities dbContext = null;
        KaigrSearchDB searchDBContext = null;
        /// <summary>
        /// Exemption Document View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns ExemptionDocument Model</returns>
        public ExemptionDocumentModel ExemptionDocumentView(int OfficeID)
        {
            ExemptionDocumentModel resModel = new ExemptionDocumentModel();
            try
            {
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                searchDBContext = new KaigrSearchDB();

                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                // short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                // int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                resModel.SROfficeList = new List<SelectListItem>();
                resModel.SROfficeList.Add(new SelectListItem { Text = "Select", Value = "0" });
                resModel.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();

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
                return resModel;


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
                //    resModel.DROfficeList.Add(droNameItem);
                //    resModel.SROfficeList.Add(sroNameItem);
                //}
                //else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                //{
                //    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                //    string DroCode_string = Convert.ToString(Kaveri1Code);
                //    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                //    resModel.DROfficeList.Add(droNameItem);
                //    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "Select");
                //}
                //else
                //{
                //    SelectListItem select = new SelectListItem();
                //    select.Text = "Select";
                //    select.Value = "0";
                //    resModel.SROfficeList.Add(select);
                //    resModel.DROfficeList = objCommon.GetDROfficesList("Select");
                //}
                //return resModel;
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

        ///// <summary>
        ///// Exemption Document Summary
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns>returns ExemptionDocumentSummary model</returns>
        //public ExemptionDocumentSummary ExemptionDocumentSummary(ExemptionDocumentModel model)
        //{
        //    //return new ExemptionDocumentSummary()
        //    //{
        //    //    SerialNo = 1,
        //    //    SROName = "sro name",
        //    //    JurisdictionalOffice = "Jurisdictional Office",
        //    //    Documents = 1,
        //    //    StumpDuty = 1,
        //    //    RegistrationFees = 1,
        //    //    Total = 1
        //    //};

        //    try
        //    {
        //        ExemptionDocumentSummary exemptionDocumentSummary = new ExemptionDocumentSummary();
        //        dbContext = new KaveriEntities();
        //        USP_RPT_ExemptedDocumentsSummary_Result result = dbContext.USP_RPT_ExemptedDocumentsSummary(model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).FirstOrDefault();
        //        int count = 1;
        //        if (result != null)
        //        {
        //            exemptionDocumentSummary.SerialNo = count;
        //            exemptionDocumentSummary.JurisdictionalOffice = String.IsNullOrEmpty(result.JSRONameE) ? String.Empty : result.JSRONameE;
        //            exemptionDocumentSummary.SROName = String.IsNullOrEmpty(result.SRONameE) ? String.Empty : result.SRONameE;
        //            exemptionDocumentSummary.Documents = result.NO_OF_DOCUMENTS == null ? 0 : (int)result.NO_OF_DOCUMENTS;
        //            exemptionDocumentSummary.RegistrationFees = result.REGISTRATIONFEE == null ? 0 : (decimal)result.REGISTRATIONFEE;
        //            exemptionDocumentSummary.STAMPDUTY_BEFORE_EXEMPTION = result.STAMPDUTY_BEFORE_EXEMPTION == null ? 0 : (decimal)result.STAMPDUTY_BEFORE_EXEMPTION;
        //            exemptionDocumentSummary.EXEMPTION_GIVEN = result.EXEMPTION_GIVEN;
        //            exemptionDocumentSummary.STAMPDUTY_AFTER_EXEMPTION = result.STAMPDUTY_AFTER_EXEMPTION;
        //            exemptionDocumentSummary.Total = result.Total == null ? 0 : (decimal)result.Total;
        //        }
        //        return exemptionDocumentSummary;
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
        /// Exemption Document Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns ExemptionDocumentDetailWrapper model</returns>
        public ExemptionDocumentDetailWrapper ExemptionDocumentDetail(ExemptionDocumentModel model)
        {
            #region DUMMY DATA
            //ExemptionDocumentDetailWrapper exemptionDocumentDetailWrapper = new ExemptionDocumentDetailWrapper();
            //exemptionDocumentDetailWrapper.ExemptionDocumentDetailList = new List<ExemptionDocumentDetail>();
            //ExemptionDocumentDetail exemptionDocumentDetail = new ExemptionDocumentDetail();
            //exemptionDocumentDetail.SerialNo = 1;
            //exemptionDocumentDetail.JurisdictionalOffice = "Jurisdictional Office";
            //exemptionDocumentDetail.SROName = "SROName";
            //exemptionDocumentDetail.FinalRegistrationNumber = "Final registration number";
            //exemptionDocumentDetail.StumpDuty = 1;
            //exemptionDocumentDetail.RegistrationFees = 1;
            //exemptionDocumentDetail.Total = 1;

            //exemptionDocumentDetailWrapper.ExemptionDocumentDetailList.Add(exemptionDocumentDetail);
            //return exemptionDocumentDetailWrapper;
            #endregion

            try
            {
                ExemptionDocumentDetailWrapper exemptionDocumentDetailWrapper = new ExemptionDocumentDetailWrapper();
                exemptionDocumentDetailWrapper.ExemptionDocumentDetailList = new List<ExemptionDocumentDetail>();
                ExemptionDocumentDetail exemptionDocumentDetail = null;
                dbContext = new KaveriEntities();
                List<USP_RPT_ExemptedDocumentsDetail_Result> result = dbContext.USP_RPT_ExemptedDocumentsDetail(model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).Skip(model.StartLen).Take(model.TotalNum).ToList();
                if (result != null)
                {
                    int count = 1;
                    foreach (var item in result)
                    {
                        exemptionDocumentDetail = new ExemptionDocumentDetail();
                        exemptionDocumentDetail.SerialNo = count++;
                        //exemptionDocumentDetail.JurisdictionalOffice = String.IsNullOrEmpty(item.JSRONameE) ? String.Empty : item.JSRONameE;
                        exemptionDocumentDetail.SROName = String.IsNullOrEmpty(item.SRONAME) ? String.Empty : item.SRONAME;
                        exemptionDocumentDetail.FinalRegistrationNumber = String.IsNullOrEmpty(item.FinalRegistrationNumber) ? String.Empty : item.FinalRegistrationNumber;
                        exemptionDocumentDetail.RegistrationFees = item.RegistrationFees;// item.REGISTRATIONFEE == null ? 0 : (decimal)item.REGISTRATIONFEE;
                        exemptionDocumentDetail.STAMPDUTY_BEFORE_EXEMPTION = item.StampDutyBeforeRegistration!=null ? Convert.ToDecimal(item.StampDutyBeforeRegistration) : 0;// item.STAMPDUTY == null ? 0 : (decimal)item.STAMPDUTY;
                        exemptionDocumentDetail.EXEMPTION_GIVEN = item.EXEMPTION_GIVEN;
                        exemptionDocumentDetail.STAMPDUTY_AFTER_EXEMPTION = item.StampDutyAfterExemption != null ? Convert.ToDecimal(item.StampDutyAfterExemption) : 0;
                        exemptionDocumentDetail.Total = item.Total == null ? 0 : (decimal)item.Total;
                        exemptionDocumentDetailWrapper.ExemptionDocumentDetailList.Add(exemptionDocumentDetail);
                    }
                }
                return exemptionDocumentDetailWrapper;
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
        /// Exemption Document Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns ExemptionDocument Total Count</returns>
        public int ExemptionDocumentTotalCount(ExemptionDocumentModel model)
        {
            //return 1;
            List<USP_RPT_ExemptedDocumentsDetail_Result> Result = null;
            try
            {
                dbContext = new KaveriEntities();
                Result = dbContext.USP_RPT_ExemptedDocumentsDetail(model.SROfficeID, model.DateTime_FromDate, model.DateTime_ToDate).ToList();
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