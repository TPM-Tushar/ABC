#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   SurchargeCessDetailsBAL.cs
    * Author Name       :   Shubham Bhagat  
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion


using CustomModels.Models.MISReports.SurchargeCessDetails;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class SurchargeCessDetailsBAL : ISurchargeCessDetails
    {
        //ISurchargeCessDetails misReportsDal = new SurchargeCessDetailsDAL();

        /// <summary>
        /// returns Surcharge Cess Details Model
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns Surcharge Cess Details Model</returns>
        public SurchargeCessDetailsModel SurchargeCessDetailsView(int OfficeID)
        {
            return new SurchargeCessDetailsDAL().SurchargeCessDetailsView(OfficeID);
        }

        /// <summary>
        /// returns List of surcharge Cess Detail Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns List of surcharge Cess Detail Model</returns>
        public SurchargeCessDetailWrapper SurchargeCessDetails(SurchargeCessDetailsModel model)
        {
            return new SurchargeCessDetailsDAL().SurchargeCessDetails(model);
        }

        /// <summary>
        /// returns Surcharge Cess Details Total Count
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns Surcharge Cess Details Total Count</returns>
        public int SurchargeCessDetailsTotalCount(SurchargeCessDetailsModel model)
        {
            return new SurchargeCessDetailsDAL().SurchargeCessDetailsTotalCount(model);
        }

        /// <summary>
        /// Returns SroName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns>Returns SroName</returns>
        public string GetSroName(int SROfficeID)
        {
            return new SurchargeCessDetailsDAL().GetSroName(SROfficeID);
        }
    }
}