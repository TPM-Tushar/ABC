using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomModels.Models.MISReports.FirmCentralizationReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class FirmCentralizationReportBAL : IFirmCentralizationReport
    {
        IFirmCentralizationReport objDal = new FirmCentralizationReportDAL();
        public FirmCentralizationReportViewModel FirmCentralizationReportView(FirmCentralizationReportViewModel firmCentralizationReportViewModel)
        {
            try
            {
                return objDal.FirmCentralizationReportView(firmCentralizationReportViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FirmCentralizationReportResultModel GetFirmCentralizationDetails(FirmCentralizationReportViewModel firmCentralizationReportViewModel)
        {
            try
            {
                return objDal.GetFirmCentralizationDetails(firmCentralizationReportViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}