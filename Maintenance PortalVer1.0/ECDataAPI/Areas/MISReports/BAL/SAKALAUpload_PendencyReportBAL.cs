#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   SAKALAUpload_PendencyReportBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.Common;
using CustomModels.Models.MISReports.SAKALAUpload_PendencyReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class SAKALAUpload_PendencyReportBAL : ISAKALAUpload_PendencyReport
    {
        ISAKALAUpload_PendencyReport misReportsDal = new SAKALAUpload_PendencyReportDAL();

        public SAKALAUploadRptViewModel SAKALUploadReportView(int OfficeID)
        {
            return misReportsDal.SAKALUploadReportView(OfficeID);

        }
        public SAKALAUploadRptResModel LoadSakalaUploadReportDataTable(SAKALAUploadRptViewModel model)
        {
            return misReportsDal.LoadSakalaUploadReportDataTable(model);

        }
        public XMLResModel GetXMLContent(XMLInputForSAKALAUploadModel InputModel)
        {

            return misReportsDal.GetXMLContent(InputModel);

        }

    }
}