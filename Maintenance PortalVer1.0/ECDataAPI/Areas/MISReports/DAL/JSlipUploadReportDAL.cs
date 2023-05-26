#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   JSlipUploadReportDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.JSlipUploadReport;
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
    public class JSlipUploadReportDAL : IJSlipUploadReport
    {
        KaveriEntities dbContext = null;

        /// <summary>
        /// Returns JSlipUploadRptViewModel Required to show JSlipUploadReportView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public JSlipUploadRptViewModel JSlipUploadReportView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                JSlipUploadRptViewModel model = new JSlipUploadRptViewModel();
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
        /// Returns JSlipUploadRptResModel Required to show JSlip Upload Report DataTable
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JSlipUploadRptResModel LoadJSlipUploadReportDataTable(JSlipUploadRptViewModel model)
        {
            JSlipUploadRptResModel bhoomiFileUploadRptResModel = new JSlipUploadRptResModel();
            JSlipUploadRptRecModel JSlipUploadRec = null;
            try
            {
                dbContext = new KaveriEntities();
                int counter = (model.startLen + 1); //To start Serial Number 
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                List<USP_RPT_JSlipUploadReport_Result> JSlipUploadRecList = new List<USP_RPT_JSlipUploadReport_Result>();
                JSlipUploadRecList = dbContext.USP_RPT_JSlipUploadReport(model.DistrictID, model.SROfficeID, FromDate, ToDate).ToList();
                bhoomiFileUploadRptResModel.TotalCount = JSlipUploadRecList.Count();
                List<JSlipUploadRptRecModel> JSlipRecList = new List<JSlipUploadRptRecModel>();
                if (string.IsNullOrEmpty(model.SearchValue))
                {
                    if (!model.IsExcel)
                    {
                        JSlipUploadRecList = JSlipUploadRecList.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }
                foreach(USP_RPT_JSlipUploadReport_Result item in JSlipUploadRecList)
                {
                    JSlipUploadRec = new JSlipUploadRptRecModel();
                    JSlipUploadRec.SerialNo = counter++;
                    JSlipUploadRec.OfficeName = String.IsNullOrEmpty(item.OfficeName) ? string.Empty : item.OfficeName;
                    JSlipUploadRec.FileName = String.IsNullOrEmpty(item.FileName) ? string.Empty : item.FileName;
                    JSlipUploadRec.TotalRecords = (item.NoOfRecords == null) ? 0 : Convert.ToInt32(item.NoOfRecords);
                    JSlipUploadRec.DocNumberList = String.IsNullOrEmpty(item.DocumentNumberList) ? string.Empty : item.DocumentNumberList;
                    JSlipUploadRec.AdditionalDocs = String.IsNullOrEmpty(item.AdditionalDocuments) ? string.Empty : item.AdditionalDocuments;
                    JSlipUploadRec.UploadDateTime = item.UploadTime == null ? string.Empty: Convert.ToDateTime(item.UploadTime).ToString("dd/MM/yyyy",CultureInfo.InvariantCulture);
                    JSlipRecList.Add(JSlipUploadRec);
                }

                bhoomiFileUploadRptResModel.IJSlipUploadRecList = JSlipRecList;

                if (!string.IsNullOrEmpty(model.SearchValue))
                {
                    bhoomiFileUploadRptResModel.IJSlipUploadRecList = bhoomiFileUploadRptResModel.IJSlipUploadRecList.Where(m => m.TotalRecords.ToString().Contains(model.SearchValue.ToString().ToLower())
                      //m.District.ToLower().Contains(model.SearchValue.ToLower()) ||

                      );
                    bhoomiFileUploadRptResModel.FilteredRecCount = bhoomiFileUploadRptResModel.IJSlipUploadRecList.Count();
                    bhoomiFileUploadRptResModel.IJSlipUploadRecList= bhoomiFileUploadRptResModel.IJSlipUploadRecList.Skip(model.startLen).Take(model.totalNum).ToList();
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