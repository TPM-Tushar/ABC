using CustomModels.Models.Remittance.ScheduleAllocationAnalysis;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class ScheduleAllocationAnalysisBAL : IScheduleAllocationAnalysis
    {   
        IScheduleAllocationAnalysis scheduleAllocationAnalysisDAL = new ScheduleAllocationAnalysisDAL();

        public ScheduleAllocationAnalysisResponseModel ScheduleAllocationAnalysisView(int OfficeID)
        {
            return scheduleAllocationAnalysisDAL.ScheduleAllocationAnalysisView(OfficeID);
        }

        public int GetScheduleAllocationAnalysisDetailsTotalCount(ScheduleAllocationAnalysisResponseModel model)
        {
            return scheduleAllocationAnalysisDAL.GetScheduleAllocationAnalysisDetailsTotalCount(model);

        }
        public ScheduleAllocationAnalysisResultModel GetScheduleAllocationAnalysisDetails(ScheduleAllocationAnalysisResponseModel model)
        {
            return scheduleAllocationAnalysisDAL.GetScheduleAllocationAnalysisDetails(model);
        }

        public string GetSroName(int SROfficeID)
        {
            return scheduleAllocationAnalysisDAL.GetSroName(SROfficeID);

        }
    }
}