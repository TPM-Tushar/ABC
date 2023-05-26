#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IScannedFileUploadStatusReport.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.ScannedFileUploadStatusReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IScannedFileUploadStatusReport
    {
        ScannedFileUploadStatusRptReqModel GetScannedFileUploadStatusDetails(int OfficeID);
        ScannedFileUploadStatusRptResModel LoadScannedFileUploadStatusTable(ScannedFileUploadStatusRptReqModel model);


    }
}
