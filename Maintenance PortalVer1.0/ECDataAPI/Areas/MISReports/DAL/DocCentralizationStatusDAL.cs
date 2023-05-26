#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DocCentralizationStatusDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.DocCentralizationStatus;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECDataAPI.Entity.KaveriEntities;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class DocCentralizationStatusDAL : IDocCentralizationStatus
    {
        private KaveriEntities dbContext;
        KaigrSearchDB searchDBContext = null;


        /// <summary>
        /// returns HighValueProperties Request Model
        /// </summary>
        /// <returns></returns>
        public DocCentrStatusReqModel DocCentralizationStatusView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                bool IsAllSelected=true;
                searchDBContext = new KaigrSearchDB();
                ApiCommonFunctions ObjCommom = new ApiCommonFunctions();
                List<SelectListItem> SROList = new List<SelectListItem>();
                DocCentrStatusReqModel model = new DocCentrStatusReqModel();
                model.SROfficeList = ObjCommom.getSROfficesList(IsAllSelected);
                model.Date = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
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
        /// returns DocCentrStatusResModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DocCentrStatusResModel LoadDocCentralizationStatusDataTable(DocCentrStatusReqModel model)
        {
            try
            {
                //dbContext.Database.CommandTimeout = 200;
                int SerialNo = 1;
                DocCentrStatusResModel resModel = new DocCentrStatusResModel();
                List<DocCentrStatusDetailsModel> detailsList = new List<DocCentrStatusDetailsModel>();
                dbContext = new KaveriEntities();
                searchDBContext = new KaigrSearchDB();
                List<USP_RPT_REGISTRATION_STATUS_Result> RegistrationStatusList = new List<USP_RPT_REGISTRATION_STATUS_Result>();
                int TotalDocsCentlzdToday = 0;
                int totalDocsRegdPreviouslyCrtlzdToday = 0;
                DateTime Date = Convert.ToDateTime(model.Date);
                RegistrationStatusList = dbContext.USP_RPT_REGISTRATION_STATUS(model.SROID, Date).OrderByDescending(x=>x.LAST_DOCUMENT_CENTRALIZED_DATETIME).ToList();
                List<DocCentrStatusDetailsModel> DocCentralizationList = new List<DocCentrStatusDetailsModel>();
                DocCentrStatusDetailsModel DetailsModelObj = null;

                foreach (var RegStatRecord in RegistrationStatusList)
                {
                    DetailsModelObj = new DocCentrStatusDetailsModel();
                    DetailsModelObj.SROCode = RegStatRecord.SROCODE;
                    DetailsModelObj.SROName = string.IsNullOrEmpty(RegStatRecord.SRONameE) ? "" : Convert.ToString(RegStatRecord.SRONameE);
                    DetailsModelObj.DocsCentlzdToday = RegStatRecord.SAMEDAY_REGISTERED_SAMEDAY_CENTRALIZED == null ? 0 : Convert.ToInt32(RegStatRecord.SAMEDAY_REGISTERED_SAMEDAY_CENTRALIZED);
                    DetailsModelObj.DocsRegdPreviouslyCrtlzdToday = RegStatRecord.PreviousDay_REGISTERED_SAMEDAY_CENTRALIZED == null ? 0 : Convert.ToInt32(RegStatRecord.PreviousDay_REGISTERED_SAMEDAY_CENTRALIZED) ;
                    DetailsModelObj.SerialNo = SerialNo++;
                    DetailsModelObj.LatestCentralizationDate = (RegStatRecord.LAST_DOCUMENT_CENTRALIZED_DATETIME == null) ? string.Empty : RegStatRecord.LAST_DOCUMENT_CENTRALIZED_DATETIME.ToString("dd/MM/yyyy HH:mm:ss",CultureInfo.InvariantCulture);
                    TotalDocsCentlzdToday = TotalDocsCentlzdToday + DetailsModelObj.DocsCentlzdToday;
                    totalDocsRegdPreviouslyCrtlzdToday = totalDocsRegdPreviouslyCrtlzdToday + DetailsModelObj.DocsRegdPreviouslyCrtlzdToday;
                    DocCentralizationList.Add(DetailsModelObj);
                }
                resModel.TotalDocsCentralized = TotalDocsCentlzdToday;
                resModel.TotalDocsRegdPreviously = totalDocsRegdPreviouslyCrtlzdToday;
                resModel.DetailsList = DocCentralizationList;
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