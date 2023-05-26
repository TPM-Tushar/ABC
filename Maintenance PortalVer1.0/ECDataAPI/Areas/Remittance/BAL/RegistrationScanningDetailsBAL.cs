#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationScanningDetailsBAL.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL Layer for Registration Scanning Details Report .

*/
#endregion


using CustomModels.Models.Remittance.RegistrationScanningDetails;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class RegistrationScanningDetailsBAL : IRegistrationScanningDetails
    {
        RegistrationScanningDetailsDAL registrationScanningDetailsDAL = new RegistrationScanningDetailsDAL();

     public RegistrationScanningDetailsModel RegistrationScanningDetailsView(int DocumentTypeId)
        {
            return registrationScanningDetailsDAL.RegistrationScanningDetailsView(DocumentTypeId);
        }
      public  RegistrationScanningDetailsResultModel GetRegistrationScanningDetails(RegistrationScanningDetailsModel registrationScanningDetailsModel)
        {
            return registrationScanningDetailsDAL.GetRegistrationScanningDetails(registrationScanningDetailsModel);
        }
    }
}