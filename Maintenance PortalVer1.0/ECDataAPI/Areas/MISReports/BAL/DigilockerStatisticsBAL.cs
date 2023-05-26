using CustomModels.Models.MISReports.DigilockerStatistics;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class DigilockerStatisticsBAL : IDigilockerStatistics
    {
        IDigilockerStatistics digilockerStatisticsDAL = new DigilockerStatisticsDAL();

        public DigiLockerStatisticsViewModel DigilockerStatisticsView(int OfficeID)
        {
            return digilockerStatisticsDAL.DigilockerStatisticsView(OfficeID);
        }
        
        public DigilockerStatisticsResponseModel DigilockerStatisticsReportDetails(DigiLockerStatisticsViewModel model)
        {
            return digilockerStatisticsDAL.DigilockerStatisticsReportDetails(model);
        }
        

    }
}