#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ExemptionDocumentBAL.cs
    * Author Name       :   Shubham Bhagat  
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.ExemptionDocument;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ExemptionDocumentBAL : IExemptionDocument
    {
        /// <summary>
        /// Exemption Document View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns> Exemption Document View Model</returns>
        public ExemptionDocumentModel ExemptionDocumentView(int OfficeID)
        {
            return new ExemptionDocumentDAL().ExemptionDocumentView(OfficeID);
        }

        ///// <summary>
        ///// Exemption Document Summary
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns>Exemption Document Summary Model</returns>
        //public ExemptionDocumentSummary ExemptionDocumentSummary(ExemptionDocumentModel model)
        //{
        //    return new ExemptionDocumentDAL().ExemptionDocumentSummary(model);
        //}

        /// <summary>
        /// Exemption Document Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Exemption Document Detail Model List</returns>
        public ExemptionDocumentDetailWrapper ExemptionDocumentDetail(ExemptionDocumentModel model)
        {
            return new ExemptionDocumentDAL().ExemptionDocumentDetail(model);
        }

        /// <summary>
        /// Exemption Document Total Count
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Exemption Document Total Count</returns>
        public int ExemptionDocumentTotalCount(ExemptionDocumentModel model)
        {
            return new ExemptionDocumentDAL().ExemptionDocumentTotalCount(model);
        }

        /// <summary>
        /// Get Sro Name
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns>return Sro Name</returns>
        public string GetSroName(int SROfficeID)
        {
            return new ExemptionDocumentDAL().GetSroName(SROfficeID);
        }
    }
}