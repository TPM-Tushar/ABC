#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   IRegistrationNoVerificationDetails.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Registration No Verification Details.

*/
#endregion

using CustomModels.Models.Remittance.RegistrationNoVerificationDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    interface IRegistrationNoVerificationDetails
    {
        RegistrationNoVerificationDetailsModel RegistrationNoVerificationDetailsView(int officeID);

        List<RegistrationNoVerificationDetailsTableModel> GetRegistrationNoVerificationDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel);

        //Added by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 
        string RefreshPropertyAreaDetailsDetails(int SROfficeID);
        //Ended by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 


        RegistrationNoVerificationDetailsTableModel GetDocRegNoCLBatchDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel);
        RegistrationNoVerificationDetailsTableModel GetScannedFileDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel);

        RegistrationNoVerificationDetailsTableModel GetFinalRegistrationNumberDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel);

        RegistrationNoVerificationDetailsTableModel GetDateDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel);
    }
}
