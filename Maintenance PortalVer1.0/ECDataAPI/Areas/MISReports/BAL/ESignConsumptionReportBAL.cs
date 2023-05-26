using CustomModels.Models.MISReports.ESignConsumptionReport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ESignConsumptionReportBAL : IESignConsumptionReport
    {
        
        IESignConsumptionReport objDAL = null;

        public ESignConsumptionReportViewModel ESignConsumptionReportView()
        {
            try
            {
                objDAL = new ESignConsumptionReportDAL();

                return objDAL.ESignConsumptionReportView();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ESignTotalConsumptionResModel GetTotalESignConsumedCount(ESignConsumptionReportViewModel requestModel)
        {
            try
            {
                objDAL = new ESignConsumptionReportDAL();
                return objDAL.GetTotalESignConsumedCount(requestModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ESignStatusDetailsResModel LoadESignDetailsDataTable(ESignConsumptionReportViewModel requestModel)
        {
            try
            {
                objDAL = new ESignConsumptionReportDAL();

                return objDAL.LoadESignDetailsDataTable(requestModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}