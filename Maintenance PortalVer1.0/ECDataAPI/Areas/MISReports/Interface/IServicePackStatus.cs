#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IServicePackStatus.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.ServicePackStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IServicePackStatus
    {
        ServicePackStatusModel ServicePackStatusView(int OfficeID);

        List<ServicePackStatusDetails> ServicePackStatusDetails(ServicePackStatusModel model);

        int ServicePackStatusTotalCount(ServicePackStatusModel model);
    }
}
