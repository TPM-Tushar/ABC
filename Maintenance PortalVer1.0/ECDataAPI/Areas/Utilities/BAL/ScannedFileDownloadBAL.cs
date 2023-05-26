#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ScannedFileDownloadBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -22-10-2019
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion
using CustomModels.Models.Utilities.ScannedfileDownload;
using ECDataAPI.Areas.Utilities.Controllers;
using ECDataAPI.Areas.Utilities.DAL;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Utilities.BAL
{
    public class ScannedFileDownloadBAL : IScannedFileDownload
    {

        IScannedFileDownload misReportsDal = new ScannedFileDownloadDAL();

        /// <summary>
        /// returns Model which contains log table 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ScannedFileDownloadView ScannedFileDownloadView(int OfficeID)
        {
            return misReportsDal.ScannedFileDownloadView(OfficeID);

        }
        public ScannedFileDownloadView LoadScannedFileDownloadLogTable(long UserID)
        {
            return misReportsDal.LoadScannedFileDownloadLogTable(UserID);

        }

        /// <summary>
        /// returns Byte Array From ScannedFileDownloadDAL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ScannedFileDownloadResModel GetScannedFileByteArray(ScannedFileDownloadView ReqModel)
        {
            return misReportsDal.GetScannedFileByteArray(ReqModel);

        }

        /// <summary>
        /// returns Status (whether Scanned File id downloaded)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveScannedFileDownloadDetails(ScannedFileDownloadView ReqModel)
        {
            return misReportsDal.SaveScannedFileDownloadDetails(ReqModel);

        }

    }
}