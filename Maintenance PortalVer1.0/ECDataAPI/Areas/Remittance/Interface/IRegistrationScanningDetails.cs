#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   IRegistrationScanningDetails.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Registration Scanning Details Report .

*/
#endregion


using CustomModels.Models.Remittance.RegistrationScanningDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    interface IRegistrationScanningDetails
    {
        RegistrationScanningDetailsModel RegistrationScanningDetailsView(int DocumentTypeId);

        RegistrationScanningDetailsResultModel GetRegistrationScanningDetails(RegistrationScanningDetailsModel registrationScanningDetailsModel);
    }
}
