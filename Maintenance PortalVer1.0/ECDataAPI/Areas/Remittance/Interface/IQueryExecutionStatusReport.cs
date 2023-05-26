#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IQueryExecutionStatusReport.cs
    * Author Name       :   Pankaj Sakhare
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.QueryExecutionStatusReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IQueryExecutionStatusReport
    {
        QueryExecutionStatusReportModel QueryExecutionStatusReportView();
        QueryExecutionStatusReportResModel GetQueryExecutionStatusReport(QueryExecutionStatusReportModel viewModel);
    }
}