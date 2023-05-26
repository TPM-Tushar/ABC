#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ISurchargeCessDetails.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.SurchargeCessDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface ISurchargeCessDetails
    {
        SurchargeCessDetailsModel SurchargeCessDetailsView(int OfficeID);
        SurchargeCessDetailWrapper SurchargeCessDetails(SurchargeCessDetailsModel model);
        int SurchargeCessDetailsTotalCount(SurchargeCessDetailsModel model);
        string GetSroName(int OfficeID);
    }
}
