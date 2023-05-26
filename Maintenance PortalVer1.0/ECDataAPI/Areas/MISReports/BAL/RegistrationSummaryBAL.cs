using CustomModels.Models.MISReports.RegistrationSummary;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class RegistrationSummaryBAL: IRegistrationSummary
    {
        IRegistrationSummary misReportsDal = new RegistrationSummaryDAL();

        /// <summary>
        /// returns IndexIIReports Response Model
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public RegistrationSummaryRESModel RegistrationSummaryView(int OfficeID)
        {
            return misReportsDal.RegistrationSummaryView(OfficeID);

        }


        /// <summary>
        /// returns List of IndexIIReportsDetailsModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<RegistrationSummaryDetailModel> GetRegistrationSummaryDetails(RegistrationSummaryRESModel model)
        {
            return misReportsDal.GetRegistrationSummaryDetails(model);

        }

        /// <summary>
        /// returns TolatCount of GetIndexIIReportsDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int GetRegistrationSummaryDetailsTotalCount(RegistrationSummaryRESModel model)
        {
            return misReportsDal.GetRegistrationSummaryDetailsTotalCount(model);

        }

        /// <summary>
        /// Returns SroName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns></returns>
        public string GetSroName(int SROfficeID)
        {
            return misReportsDal.GetSroName(SROfficeID);

        }

        public RegistrationSummaryREQModel DisplayScannedFile(RegistrationSummaryREQModel model)
        {
            return misReportsDal.DisplayScannedFile(model);
        }
    }
}