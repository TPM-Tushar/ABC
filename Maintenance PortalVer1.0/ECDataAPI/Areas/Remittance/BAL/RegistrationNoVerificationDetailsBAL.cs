#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationNoVerificationDetailsBAL.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL Layer for Registration No Verification Details.

*/
#endregion

using CustomModels.Models.Remittance.RegistrationNoVerificationDetails;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class RegistrationNoVerificationDetailsBAL : IRegistrationNoVerificationDetails
    {
        RegistrationNoVerificationDetailsDAL registrationNoVerificationDetailsDAL = new RegistrationNoVerificationDetailsDAL();
        public RegistrationNoVerificationDetailsModel RegistrationNoVerificationDetailsView(int officeID)
        {
            return registrationNoVerificationDetailsDAL.RegistrationNoVerificationDetailsView(officeID);
        }

        public List<RegistrationNoVerificationDetailsTableModel> GetRegistrationNoVerificationDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            return registrationNoVerificationDetailsDAL.GetRegistrationNoVerificationDetails(registrationNoVerificationDetailsModel);
        }

        //Added by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 
        public string RefreshPropertyAreaDetailsDetails(int SROfficeID)
        {
            return registrationNoVerificationDetailsDAL.RefreshPropertyAreaDetailsDetails(SROfficeID);
        }
        //Ended by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 


        public RegistrationNoVerificationDetailsTableModel GetDocRegNoCLBatchDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            return registrationNoVerificationDetailsDAL.GetDocRegNoCLBatchDetails(registrationNoVerificationDetailsModel);
        }
        //Added By Tushar on 7 Nov 2022
        public RegistrationNoVerificationDetailsTableModel GetScannedFileDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            return registrationNoVerificationDetailsDAL.GetScannedFileDetails(registrationNoVerificationDetailsModel);
        }
        public RegistrationNoVerificationDetailsTableModel GetFinalRegistrationNumberDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            return registrationNoVerificationDetailsDAL.GetFinalRegistrationNumberDetails(registrationNoVerificationDetailsModel);
        }
        //End By Tushar on 7 Nov 2022
        //Added By Tushar on on 8 feb
        public RegistrationNoVerificationDetailsTableModel GetDateDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            return registrationNoVerificationDetailsDAL.GetDateDetails(registrationNoVerificationDetailsModel);
        
        }
        //End By Tushar on on 8 feb
    }
}