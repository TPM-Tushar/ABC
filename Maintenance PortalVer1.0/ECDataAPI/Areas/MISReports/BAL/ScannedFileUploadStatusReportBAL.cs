#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ScannedFileUploadStatusReportBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.ScannedFileUploadStatusReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ScannedFileUploadStatusReportBAL : IScannedFileUploadStatusReport
    {
        IScannedFileUploadStatusReport ScannedFileUploadStatusdalObj = new ScannedFileUploadStatusReportDAL();
        public ScannedFileUploadStatusRptReqModel GetScannedFileUploadStatusDetails(int OfficeID)
        {
            return ScannedFileUploadStatusdalObj.GetScannedFileUploadStatusDetails(OfficeID);

        }

        /// <summary>
        /// Returns Scanned File Upload Status View to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ScannedFileUploadStatusRptResModel LoadScannedFileUploadStatusTable(ScannedFileUploadStatusRptReqModel model)
        {
            return ScannedFileUploadStatusdalObj.LoadScannedFileUploadStatusTable(model);

        }
    }
}