#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   JurisdictionalWiseBAL.cs
    * Author Name       :   Shubham Bhagat  
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.JurisdictionalWise;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class JurisdictionalWiseBAL : IJurisdictionalWise
    {
        /// <summary>
        /// Jurisdictional Wise View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>Jurisdictional Wise View Model</returns>
        public JurisdictionalWiseModel JurisdictionalWiseView(int OfficeID)
        {
            return new JurisdictionalWiseDAL().JurisdictionalWiseView(OfficeID);
        }

        ///// <summary>
        ///// Jurisdictional Wise Summary
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns>Jurisdictional Wise Summary Model</returns>
        //public JurisdictionalWiseSummary JurisdictionalWiseSummary(JurisdictionalWiseModel model)
        //{
        //    return new JurisdictionalWiseDAL().JurisdictionalWiseSummary(model);
        //}

        /// <summary>
        /// Jurisdictional Wise Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Jurisdictional Wise Detail Model list</returns>
        public JurisdictionalWiseDetailWrapper JurisdictionalWiseDetail(JurisdictionalWiseModel model)
        {
            return new JurisdictionalWiseDAL().JurisdictionalWiseDetail(model);
        }

        /// <summary>
        /// Jurisdictional Wise Total Count 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Jurisdictional Wise Total Count</returns>
        public int JurisdictionalWiseTotalCount(JurisdictionalWiseModel model)
        {
            return new JurisdictionalWiseDAL().JurisdictionalWiseTotalCount(model);
        }

        /// <summary>
        /// Get Sr oName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns>Get Sro Name</returns>
        public string GetSroName(int SROfficeID)
        {
            return new JurisdictionalWiseDAL().GetSroName(SROfficeID);
        }       
    }
}