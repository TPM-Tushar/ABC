#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   QueryExecutionStatusReportBAL.cs
    * Author Name       :   Pankaj Sakhare
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.QueryExecutionStatusReport;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class QueryExecutionStatusReportBAL : IQueryExecutionStatusReport
    {
        IQueryExecutionStatusReport dalObj = new QueryExecutionStatusReportDAL();

        /// <summary>
        /// QueryExecutionStatusReportView
        /// </summary>
        /// <returns>QueryExecutionStatusReportModel</returns>
        public QueryExecutionStatusReportModel QueryExecutionStatusReportView()
        {
            try
            {
                return dalObj.QueryExecutionStatusReportView();
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// GetQueryExecutionStatusReport
        /// </summary>
        /// <param name="QueryExecutionStatusReportModel"></param>
        /// <returns>QueryExecutionStatusReportResModel</returns>
        public QueryExecutionStatusReportResModel GetQueryExecutionStatusReport(QueryExecutionStatusReportModel viewModel)
        {
            try
            {
                return dalObj.GetQueryExecutionStatusReport(viewModel);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}