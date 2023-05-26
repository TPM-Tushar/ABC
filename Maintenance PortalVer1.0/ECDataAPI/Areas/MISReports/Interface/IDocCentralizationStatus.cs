#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IDocCentralizationStatus.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.DocCentralizationStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IDocCentralizationStatus
    {
        DocCentrStatusReqModel DocCentralizationStatusView(int OfficeID);
        DocCentrStatusResModel LoadDocCentralizationStatusDataTable(DocCentrStatusReqModel model);
    }
}
