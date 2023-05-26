using CustomModels.Models.MISReports.IncomeTaxReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class IncomeTaxReportBAL : IIncomeTaxReport
    {

        IIncomeTaxReport incomeTaxReportDAL = new IncomeTaxReportDAL();

        public IncomeTaxReportResponseModel IncomeTaxReportView(int OfficeID)
        {
            return incomeTaxReportDAL.IncomeTaxReportView(OfficeID);
        }

        public IncomeTaxReportResultModel GetIncomeTaxReportDetails(IncomeTaxReportResponseModel model)
        {
            return incomeTaxReportDAL.GetIncomeTaxReportDetails(model);
        }

        public int GetIncomeTaxReportDetailsTotalCount(IncomeTaxReportResponseModel model)
        {
            return incomeTaxReportDAL.GetIncomeTaxReportDetailsTotalCount(model);

        }

        public string GetSroName(int SROfficeID)
        {
            return incomeTaxReportDAL.GetSroName(SROfficeID);

        }


        
    }
}