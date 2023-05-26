#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IChallanMatrixXMLLog.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.ChallanMatrixXMLLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IChallanMatrixXMLLog
    {
        ChallanMatrixWrapperModel GetOfficeList(String OfficeType);
        ChallanMatrixWrapperModel ChallanMatrixDetails(ChallanMatrixLogRequestModel model);
        FileDownloadModel DownloadChallanMatrixXML(ChallanMatrixLogRequestModel model);
    }
}
