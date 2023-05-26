/*File Header
 * Project Id: 
 * Project Name: Kaveri Maintainance Portal
 * File Name: KaveriIntegrationBAL.cs
 * Author : Shubham Bhagat
 * Creation Date :14 Oct 2019
 * Desc : Call to DAL Layer
 * ECR No : 
*/

using CustomModels.Models.MISReports.PropertyWthoutImportBypassRDPR;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class PropertyWthoutImportBypassRDPRBAL : IPropertyWthoutImportBypassRDPR
    {
        /// <summary>
        /// Kaveri Integration View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns Kaveri Integration Model</returns>
        public ReportModel ReportView(int OfficeID)
        {
            return (new PropertyWthoutImportBypassRDPRDAL().ReportView(OfficeID));
        }

        /// <summary>
        /// Load Kaveri Integration Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns Kaveri Integration Wrapper Model</returns>
        public ReportWrapperModel LoadReportTable(ReportModel model)
        {
            return (new PropertyWthoutImportBypassRDPRDAL().LoadReportTable(model));
        }

        /// <summary>
        /// Other Table Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns KI Details Wrapper Model</returns>
        public ReportDetailsWrapperModel OtherTableDetailsBypassRDPR(ReportModel model)
        {
            return (new PropertyWthoutImportBypassRDPRDAL().OtherTableDetailsBypassRDPR(model));
        }
    }
}