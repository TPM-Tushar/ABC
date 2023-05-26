#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IIndexIIReports.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.IndexIIReports;
using CustomModels.Models.MISReports.RegistrationSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IIndexIIReports
    {

        //IndexIIReportsResponseModel IndexIIReportsView(IndexIIReportsRequestModel model);
        IndexIIReportsResponseModel IndexIIReportsView(int OfficeID);
        List<IndexIIReportsDetailsModel> GetIndexIIReportsDetails(IndexIIReportsResponseModel model);
        int GetIndexIIReportsDetailsTotalCount(IndexIIReportsResponseModel model);
        string GetSroName(int OfficeID);

        RegistrationSummaryREQModel DisplayScannedFile(RegistrationSummaryREQModel model);
    }
}
