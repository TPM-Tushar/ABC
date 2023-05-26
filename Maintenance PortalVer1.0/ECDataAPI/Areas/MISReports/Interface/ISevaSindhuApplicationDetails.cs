using CustomModels.Models.MISReports.SevaSidhuApplicationDetails;
using ECDataAPI.Areas.MISReports.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    //added by vijay on 01-03-2023
    public interface ISevaSindhuApplicationDetails
    {
        SevaSindhuApplicationDetailsReportModel SevaSindhuApplicationDetailsReportView(int OfficeID);
        SevaSindhuApplicationDetailsResultModel GetSevaSindhuApplicationDetails(SevaSindhuApplicationDetailsReportModel reportModel);
        SevaSindhuApplicationDetailsResultModel GetSevaSindhuApplicationDetails_For_TA(SevaSindhuApplicationDetailsReportModel reportModel);
    }
}
