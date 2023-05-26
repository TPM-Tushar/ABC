#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   SAKALAUpload_PendencyReportDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion


using CustomModels.Models.Common;
using CustomModels.Models.MISReports.SAKALAUpload_PendencyReport;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using ECDataUI.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    [KaveriAuthorizationAttribute]
    public class SAKALAUpload_PendencyReportDAL : ISAKALAUpload_PendencyReport
    {
        KaveriEntities dbContext = null;
        //private String[] encryptedParameters = null;
        //private Dictionary<String, String> decryptedParameters = null;


        /// <summary>
        /// Returns SAKALAUploadRptViewModel Required to show SAKALUploadReportView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public SAKALAUploadRptViewModel SAKALUploadReportView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                SAKALAUploadRptViewModel model = new SAKALAUploadRptViewModel();
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
        /// Returns SAKALAUploadRptResModel Required to LoadSakalaUploadReportDataTable
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public SAKALAUploadRptResModel LoadSakalaUploadReportDataTable(SAKALAUploadRptViewModel model)
        {
            SAKALAUploadRptResModel SakalaUploadRptResModel = new SAKALAUploadRptResModel();
            SakalaUploadRptRecModel SakalaUploadTableRec = null;
            List<SakalaUploadRptRecModel> ReportsDetailsList = new List<SakalaUploadRptRecModel>();
            try
            {
                dbContext = new KaveriEntities();
                int counter = (model.startLen + 1); //To start Serial Number 
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                List<USP_RPT_SAKALAUpoadReport_Result> PendingDocsList = new List<USP_RPT_SAKALAUpoadReport_Result>();
                PendingDocsList = dbContext.USP_RPT_SAKALAUpoadReport(model.DistrictID, model.SROfficeID, FromDate, ToDate).ToList();
                SakalaUploadRptResModel.TotalCount = PendingDocsList.Count();
                List<SakalaUploadRptRecModel> PendingDocsDatatableRecList = new List<SakalaUploadRptRecModel>();
                if (string.IsNullOrEmpty(model.SearchValue))
                {
                    if (!model.IsExcel)
                    {
                        PendingDocsList = PendingDocsList.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }
                foreach (USP_RPT_SAKALAUpoadReport_Result item in PendingDocsList)
                {
                    SakalaUploadTableRec = new SakalaUploadRptRecModel();
                    SakalaUploadTableRec.SerialNo = counter++;
                    SakalaUploadTableRec.OfficeName = String.IsNullOrEmpty(item.OfficeName) ? string.Empty : item.OfficeName;
                    SakalaUploadTableRec.RegNumPenNum = (string.IsNullOrEmpty(item.FinalRegistrationNumber) ? " - " : item.FinalRegistrationNumber.ToString()) + "  /  " + (string.IsNullOrEmpty(item.PendingNumber) ? "-" : item.PendingNumber);
                    SakalaUploadTableRec.GSCNumber = String.IsNullOrEmpty(item.GSCNo) ? string.Empty : item.GSCNo;
                    SakalaUploadTableRec.ApplicationStage = String.IsNullOrEmpty(item.ApplicationStage) ? string.Empty : item.ApplicationStage;
                    SakalaUploadTableRec.ExportedXML = string.IsNullOrEmpty(item.ExportedXML) ? string.Empty : item.ExportedXML;
                    SakalaUploadTableRec.ProcessingStatus = string.IsNullOrEmpty(item.ProcessingStatus) ? string.Empty : item.ProcessingStatus;
                    SakalaUploadTableRec.TransferDateTime = item.TransferDatetime == null ? string.Empty : Convert.ToDateTime(item.TransferDatetime).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    SakalaUploadTableRec.WhetherDocRegd = item.WhetherDocsRegd == null ? "NO" : Convert.ToDateTime(item.WhetherDocsRegd).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    SakalaUploadTableRec.WhetherDocDelivered = item.WhetherDocDelivered == null ? "NO" : Convert.ToDateTime(item.WhetherDocDelivered).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    SakalaUploadTableRec.WhetherDocPending = item.WhetherDocumentPending == null ? string.Empty : item.WhetherDocumentPending;
                    SakalaUploadTableRec.PendingReason = string.IsNullOrEmpty(item.ReasonOfpending) ? string.Empty : item.ReasonOfpending;
                    //SakalaUploadTableRec.ExportedXMLBtn = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + item.GSCNo+ "','" + item.SROCode + "','" +item.SId + "','" +item.SROName+ "')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download XML</button>";
                    SakalaUploadTableRec.ExportedXMLBtn = "<button type ='button' style='width:100%;' class='btn btn-group-md btn-success' onclick=XMLDownloadFun('" + (item.GSCNo == null ? "NA" : item.GSCNo) + "','" + item.SROCode + "','" + (item.SId == null ? "NA" : item.SId.Value.ToString()) + "','" + item.SROName + "')><i style='padding-right:3%;' class='fa fa-file-excel-o'></i>Download XML</button>";
                    PendingDocsDatatableRecList.Add(SakalaUploadTableRec);
                }

                SakalaUploadRptResModel.ISakalaUploadRecordList = PendingDocsDatatableRecList;

                if (!string.IsNullOrEmpty(model.SearchValue))
                {
                    SakalaUploadRptResModel.ISakalaUploadRecordList = SakalaUploadRptResModel.ISakalaUploadRecordList.Where(m => m.RegistrationNumber.ToString().ToLower().Contains(model.SearchValue.ToString().ToLower())).ToList();
                    SakalaUploadRptResModel.FilteredRecCount = SakalaUploadRptResModel.ISakalaUploadRecordList.Count();
                    SakalaUploadRptResModel.ISakalaUploadRecordList = SakalaUploadRptResModel.ISakalaUploadRecordList.Skip(model.startLen).Take(model.totalNum).ToList();
                }

                return SakalaUploadRptResModel;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public XMLResModel GetXMLContent(XMLInputForSAKALAUploadModel InputModel)
        {
            try
            {
                XMLResModel XMLResModel = new XMLResModel();
                dbContext = new KaveriEntities();

                String XMLContent = dbContext.SAKALA_UploadFileDetails.Where(i => i.SID == InputModel.SId && i.SROCode == InputModel.SROCode && i.GSCNo == InputModel.GSCNo).Select(i => i.InputDataset).FirstOrDefault();
                XMLResModel.XMLString = XMLContent;
                return XMLResModel;
            }
            catch (Exception ex)
            {
                throw;

            }

        }



    }
}