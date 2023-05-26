#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IAnywhereRegistrationStatistics.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.AnywhereRegistrationStatistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IAnywhereRegistrationStatistics
    {
        AnywhereRegStatViewModel AnywhereRegistrationStatisticsView(int OfficeID);
        AnywhereRegStatResModel GetAnywhereRegStatDetails(AnywhereRegStatViewModel model);
    }
}
