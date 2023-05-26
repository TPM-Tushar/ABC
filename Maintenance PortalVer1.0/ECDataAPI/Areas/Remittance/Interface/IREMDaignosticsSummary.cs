#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IREMDaignosticsSummary.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.REMDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
   public interface IREMDaignosticsSummary
    {
        RemittanceOfficeListSummaryModel GetOfficeListSummary();
    }
}
