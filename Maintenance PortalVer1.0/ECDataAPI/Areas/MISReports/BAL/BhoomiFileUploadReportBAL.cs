#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   BhoomiFileUploadReportBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.BhoomiFileUploadReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class BhoomiFileUploadReportBAL : IBhoomiFileUploadReport
    {
        IBhoomiFileUploadReport misReportsDal = new BhoomiFileUploadReportDAL();

        public BhoomiFileUploadRptViewModel BhoomiFileUploadReportView(int OfficeID)
        {
            return misReportsDal.BhoomiFileUploadReportView(OfficeID);

        }
        public BhoomiFileUploadRptResModel LoadBhoomiFileUploadReportDataTable(BhoomiFileUploadRptViewModel model)
        {
            return misReportsDal.LoadBhoomiFileUploadReportDataTable(model);

        }
    }
}