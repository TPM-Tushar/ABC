#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   IRegistrationNoVerificationSummaryReport.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :  Interface for Registration No Verification Summary Report .

*/
#endregion

using CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    interface IRegistrationNoVerificationSummaryReport
    {
        RegistrationNoVerificationSummaryReportModel RegistrationNoVerificationSummaryReportView();

        RegistrationNoVerificationSummaryResultModel GetSummaryReportDetails(RegistrationNoVerificationSummaryReportModel registrationNoVerificationSummaryReportModel);
    }
}
