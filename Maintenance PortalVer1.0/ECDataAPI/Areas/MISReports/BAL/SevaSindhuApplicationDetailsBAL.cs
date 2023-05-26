using CustomModels.Models.Remittance.MasterData;
using CustomModels.Models.MISReports.SevaSidhuApplicationDetails;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ECDataAPI.Areas.Remittance.DAL;

namespace ECDataAPI.Areas.MISReports.BAL
{
    //added by vijay on 01-03-2023
    public class SevaSindhuApplicationDetailsBAL:ISevaSindhuApplicationDetails
    {
        SevaSindhuApplicationDetailsDAL sevaSindhuApplicationDetailsDAL = new SevaSindhuApplicationDetailsDAL();
        public SevaSindhuApplicationDetailsReportModel SevaSindhuApplicationDetailsReportView(int OfficeID) 
        {
            return sevaSindhuApplicationDetailsDAL.SevaSindhuApplicationDetailsReportview(OfficeID);
        }
        public SevaSindhuApplicationDetailsResultModel GetSevaSindhuApplicationDetails(SevaSindhuApplicationDetailsReportModel reportModel)
        {
            return sevaSindhuApplicationDetailsDAL.GetSevaSindhuApplicationDetails(reportModel);

        }

        public SevaSindhuApplicationDetailsResultModel GetSevaSindhuApplicationDetails_For_TA(SevaSindhuApplicationDetailsReportModel reportModel)
        {
            return sevaSindhuApplicationDetailsDAL.GetSevaSindhuApplicationDetails_For_TA(reportModel);

        }
    }
}