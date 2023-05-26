#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DataRestorationReportBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
	* ECR No			:	431
*/
#endregion

using CustomModels.Models.MISReports.DataRestorationReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class DataRestorationReportBAL : IDataRestorationReport
    {

        /// <summary>
        /// DataRestorationReport
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns DataRestorationReportViewModel Model</returns>
        public DataRestorationReportViewModel DataRestorationReport(int OfficeID)
        {
            return new DataRestorationReportDAL().DataRestorationReport(OfficeID);
        }

        /// <summary>
        /// DataRestorationReportStatus
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationPartialViewModel Model</returns>
        public DataRestorationPartialViewModel DataRestorationReportStatus(DataRestorationReportViewModel model)
        {
            return new DataRestorationReportDAL().DataRestorationReportStatus(model);
        }

        /// <summary>
        /// InitiateDatabaseRestoration
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel InitiateDatabaseRestoration(DataRestorationReportReqModel model)
        {
            return new DataRestorationReportDAL().InitiateDatabaseRestoration(model);
        }

        /// <summary>
        /// GenerateKeyAfterExpiration
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel GenerateKeyAfterExpiration(DataRestorationReportReqModel model)
        {
            return new DataRestorationReportDAL().GenerateKeyAfterExpiration(model);
        }

        /// <summary>
        /// ApproveScript
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel ApproveScript(DataRestorationReportReqModel model)
        {
            return new DataRestorationReportDAL().ApproveScript(model);
        }

        /// <summary>
        /// DataInsertionTable
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationPartialViewModel Model</returns>
        public DataRestorationPartialViewModel DataInsertionTable(DataRestorationReportReqModel model)
        {
            return new DataRestorationReportDAL().DataInsertionTable(model);
        }
        
        /// <summary>
        /// DownloadScriptPathVerify
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel DownloadScriptPathVerify(DataRestorationReportReqModel model)
        {
            return new DataRestorationReportDAL().DownloadScriptPathVerify(model);
        }
        
        /// <summary>
        /// DownloadScriptForRectification
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel DownloadScriptForRectification(DataRestorationReportReqModel model)
        {
            return new DataRestorationReportDAL().DownloadScriptForRectification(model);
        }

        /// <summary>
        /// SaveUplodedRectifiedScript
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel SaveUplodedRectifiedScript(DataRestorationReportReqModel model)
        {
            return new DataRestorationReportDAL().SaveUplodedRectifiedScript(model);
        }

        #region ADDED BY PANKAJ ON 15-07-2020
        /// <summary>
        /// ConfirmDataInsertion
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        public DataRestorationReportResModel ConfirmDataInsertion(DataRestorationReportReqModel model)
        {
            return new DataRestorationReportDAL().ConfirmDataInsertion(model);
        }

        ////2nd method for partial view
        //public DataRestorationReportResModel GetConfirmationButtonMessage(DataRestorationReportReqModel model)
        //{
        //    return new DataRestorationReportDAL().GetConfirmationButtonMessage(model);
        //}
        #endregion

        #region ADDED BY SHUBHAM BHAGAT ON 23-07-2020
        /// <summary>
        /// LoadInitiateMasterTable
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationPartialViewModel Model</returns>
        public DataRestorationPartialViewModel LoadInitiateMasterTable(DataRestorationReportReqModel model)
        {
            return new DataRestorationReportDAL().LoadInitiateMasterTable(model);
        }
        #endregion


        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
        /// <summary>
        /// AbortView
        /// </summary>
        /// <param name="INIT_ID"></param>
        /// <returns>returns AbortViewModel Model</returns>
        public AbortViewModel AbortView(String INIT_ID)
        {
            return new DataRestorationReportDAL().AbortView(INIT_ID);
        }

        /// <summary>
        /// SaveAbortData
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns AbortViewModel Model</returns>
        public AbortViewModel SaveAbortData(AbortViewModel model)
        {
            return new DataRestorationReportDAL().SaveAbortData(model);
        }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

        public DataRestorationPartialViewModel DataRestorationReportStatusForScript(DataRestorationReportViewModel model)
        {
            return new DataRestorationReportDAL().DataRestorationReportStatusForScript(model);
        }
    }
}