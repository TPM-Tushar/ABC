#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   REMDaignosticsSummaryBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.REMDashboard;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class REMDaignosticsSummaryBAL : IREMDaignosticsSummary
    {

        IREMDaignosticsSummary misReportsDAL = new REMDaignosticsSummaryDAL();

        /// <summary>
        /// GetOfficeListSummary
        /// </summary>
        /// <returns></returns>
        public RemittanceOfficeListSummaryModel GetOfficeListSummary()
        {
            return misReportsDAL.GetOfficeListSummary();
        }
    }
}