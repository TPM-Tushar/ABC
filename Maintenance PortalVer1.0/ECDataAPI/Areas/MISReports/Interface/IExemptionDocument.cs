#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IExemptionDocument.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.ExemptionDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IExemptionDocument
    {
        ExemptionDocumentModel ExemptionDocumentView(int OfficeID);

        //ExemptionDocumentSummary ExemptionDocumentSummary(ExemptionDocumentModel model);

        ExemptionDocumentDetailWrapper ExemptionDocumentDetail(ExemptionDocumentModel model);

        int ExemptionDocumentTotalCount(ExemptionDocumentModel model);

        string GetSroName(int OfficeID);
    }
}
