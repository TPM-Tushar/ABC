#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IIntegrationCallExceptions.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.IntegrationCallExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IIntegrationCallExceptions
    {
        IntegrationCallExceptionsModel GetOfficeList(String OfficeType);
        IEnumerable<IntegrationCallExceptionsModel> GetExceptionsDetails(String OfficeType, String OfficeTypeID);
    }
}
