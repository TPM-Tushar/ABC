#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   BhoomiFileUploadReportDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.BhoomiFileUploadReport;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class BhoomiFileUploadReportDAL : IBhoomiFileUploadReport
    {
        KaveriEntities dbContext = null;
        private String[] encryptedParameters = null;
        private Dictionary<String, String> decryptedParameters = null;


        /// <summary>
        /// Returns PendingDocSummaryViewModel Required to show PendingDocumentsSummaryView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public BhoomiFileUploadRptViewModel BhoomiFileUploadReportView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                BhoomiFileUploadRptViewModel model = new BhoomiFileUploadRptViewModel();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                model.SROfficeList = new List<SelectListItem>();
                model.DistrictList = new List<SelectListItem>();
                string kaveriCode = Kaveri1Code.ToString();
                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);

                    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    model.DistrictList.Add(droNameItem);
                    model.SROfficeList.Add(sroNameItem);
                }
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    model.DistrictList.Add(droNameItem);
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "All");
                }
                else
                {
                    SelectListItem select = new SelectListItem();
                    select.Text = "All";
                    select.Value = "0";
                    model.SROfficeList.Add(select);
                    model.DistrictList = objCommon.GetDROfficesList("All");
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
            }
        }


        /// <summary>
        /// Returns BhoomiFileUploadRptResModel Required to Load BhoomiFileUploadReportDataTable
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public BhoomiFileUploadRptResModel LoadBhoomiFileUploadReportDataTable(BhoomiFileUploadRptViewModel model)
        {
            BhoomiFileUploadRptResModel bhoomiFileUploadRptResModel = new BhoomiFileUploadRptResModel();
            BhoomiFileUploadRptRecModel bhoomiFileUploadTableRec = null;
            List<BhoomiFileUploadRptRecModel> ReportsDetailsList = new List<BhoomiFileUploadRptRecModel>();
            try
            {
                dbContext = new KaveriEntities();
                int counter = (model.startLen + 1); //To start Serial Number 
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                List<USP_RPT_BhoomiFileUploadReport_Result> PendingDocsList = new List<USP_RPT_BhoomiFileUploadReport_Result>();
                PendingDocsList = dbContext.USP_RPT_BhoomiFileUploadReport(model.DistrictID, model.SROfficeID, FromDate, ToDate).ToList();
                bhoomiFileUploadRptResModel.TotalCount = PendingDocsList.Count();
                List<BhoomiFileUploadRptRecModel> PendingDocsDatatableRecList = new List<BhoomiFileUploadRptRecModel>();
                if (string.IsNullOrEmpty(model.SearchValue))
                {
                    if (!model.IsExcel)
                    {
                        PendingDocsList = PendingDocsList.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }
                foreach(USP_RPT_BhoomiFileUploadReport_Result item in PendingDocsList)
                {
                    bhoomiFileUploadTableRec = new BhoomiFileUploadRptRecModel();
                    bhoomiFileUploadTableRec.SerialNo = counter++;
                    bhoomiFileUploadTableRec.OfficeName = String.IsNullOrEmpty(item.OfficeName) ? string.Empty : item.OfficeName;
                    bhoomiFileUploadTableRec.RegistrationNumber = String.IsNullOrEmpty(item.RegistrationNumber) ? string.Empty : item.RegistrationNumber;
                    bhoomiFileUploadTableRec.SurveyNumber = String.IsNullOrEmpty(item.SurveyNumber) ? string.Empty : item.SurveyNumber;
                    bhoomiFileUploadTableRec.ReferenceNumber = String.IsNullOrEmpty(item.ReferenceNumber) ? string.Empty : item.ReferenceNumber;
                    bhoomiFileUploadTableRec.UploadDate = item.UploadDate == null ? string.Empty : (Convert.ToDateTime(item.UploadDate)).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    bhoomiFileUploadTableRec.DateofRegistration = item.DateOfRegistration == null ? string.Empty : (Convert.ToDateTime(item.DateOfRegistration)).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    PendingDocsDatatableRecList.Add(bhoomiFileUploadTableRec);
                }

                bhoomiFileUploadRptResModel.IBhoomiFileUploadRecordList = PendingDocsDatatableRecList;

                if (!string.IsNullOrEmpty(model.SearchValue))
                {
                    bhoomiFileUploadRptResModel.IBhoomiFileUploadRecordList = bhoomiFileUploadRptResModel.IBhoomiFileUploadRecordList.Where(m => m.RegistrationNumber.ToString().ToLower().Contains(model.SearchValue.ToString().ToLower())
                      //m.District.ToLower().Contains(model.SearchValue.ToLower()) ||

                      ).ToList();
                    bhoomiFileUploadRptResModel.FilteredRecCount = bhoomiFileUploadRptResModel.IBhoomiFileUploadRecordList.Count();
                    bhoomiFileUploadRptResModel.IBhoomiFileUploadRecordList= bhoomiFileUploadRptResModel.IBhoomiFileUploadRecordList.Skip(model.startLen).Take(model.totalNum).ToList();
                }
                return bhoomiFileUploadRptResModel;
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