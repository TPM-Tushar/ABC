#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   IndexIIReportsBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.IndexIIReports;
using CustomModels.Models.MISReports.RegistrationSummary;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class IndexIIReportsBAL : IIndexIIReports
    {
        IIndexIIReports misReportsDal = new IndexIIReportsDAL();
        /// <summary>
        /// returns IndexIIReports Response Model
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public IndexIIReportsResponseModel IndexIIReportsView(int OfficeID)
        {
            return misReportsDal.IndexIIReportsView(OfficeID);

        }


        /// <summary>
        /// returns List of IndexIIReportsDetailsModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<IndexIIReportsDetailsModel> GetIndexIIReportsDetails(IndexIIReportsResponseModel model)
        {
            return misReportsDal.GetIndexIIReportsDetails(model);

        }


        /// <summary>
        /// returns TolatCount of GetIndexIIReportsDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int GetIndexIIReportsDetailsTotalCount(IndexIIReportsResponseModel model)
        {
            return misReportsDal.GetIndexIIReportsDetailsTotalCount(model);

        }

        /// <summary>
        /// Returns SroName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns></returns>
        public string GetSroName(int SROfficeID)
        {
            return misReportsDal.GetSroName(SROfficeID);

        }

        public RegistrationSummaryREQModel DisplayScannedFile(RegistrationSummaryREQModel model)
        {
            return misReportsDal.DisplayScannedFile(model);
        }
    }
}