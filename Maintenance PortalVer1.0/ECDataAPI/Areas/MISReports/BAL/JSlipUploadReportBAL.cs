#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   JSlipUploadReportBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.JSlipUploadReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class JSlipUploadReportBAL : IJSlipUploadReport
    {
        IJSlipUploadReport misReportsDal = new JSlipUploadReportDAL();

        public JSlipUploadRptViewModel JSlipUploadReportView(int OfficeID)
        {
            return misReportsDal.JSlipUploadReportView(OfficeID);

        }
        public JSlipUploadRptResModel LoadJSlipUploadReportDataTable(JSlipUploadRptViewModel model)
        {
            return misReportsDal.LoadJSlipUploadReportDataTable(model);

        }
    }
}