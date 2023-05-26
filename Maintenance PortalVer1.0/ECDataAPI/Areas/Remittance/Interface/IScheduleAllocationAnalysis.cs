using CustomModels.Models.Remittance.ScheduleAllocationAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    public interface IScheduleAllocationAnalysis
    {
        ScheduleAllocationAnalysisResponseModel ScheduleAllocationAnalysisView(int officeID);
        int GetScheduleAllocationAnalysisDetailsTotalCount(ScheduleAllocationAnalysisResponseModel model);

        ScheduleAllocationAnalysisResultModel GetScheduleAllocationAnalysisDetails(ScheduleAllocationAnalysisResponseModel reqModel);

        string GetSroName(int SROfficeID);
    }
}
