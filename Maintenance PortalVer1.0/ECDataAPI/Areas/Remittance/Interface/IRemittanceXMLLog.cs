#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IRemittanceXMLLog.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.RemittanceXMLLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IRemittanceXMLLog
    {
        RemittXMLLogModel GetOfficeList(String OfficeType);
        RemittXMLLogModel RemittanceXMLLogDetails(REMRequestXMLLogModel model);
        FileDownloadModel DownloadREMLogXml(REMRequestXMLLogModel model);
    }
}
