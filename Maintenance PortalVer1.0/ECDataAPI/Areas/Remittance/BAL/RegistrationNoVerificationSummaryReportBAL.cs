#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationNoVerificationSummaryReportBAL.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL Layer for Registration No Verification Summary Report .

*/
#endregion

using CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class RegistrationNoVerificationSummaryReportBAL : IRegistrationNoVerificationSummaryReport
    {
        RegistrationNoVerificationSummaryReportDAL registrationNoVerificationDetailsDAL = new RegistrationNoVerificationSummaryReportDAL();
        public RegistrationNoVerificationSummaryReportModel RegistrationNoVerificationSummaryReportView()
        {
            return registrationNoVerificationDetailsDAL.RegistrationNoVerificationSummaryReportView();
        }
        public RegistrationNoVerificationSummaryResultModel GetSummaryReportDetails(RegistrationNoVerificationSummaryReportModel registrationNoVerificationSummaryReportModel)
        {
            return registrationNoVerificationDetailsDAL.GetSummaryReportDetails(registrationNoVerificationSummaryReportModel);
        }
    }
}