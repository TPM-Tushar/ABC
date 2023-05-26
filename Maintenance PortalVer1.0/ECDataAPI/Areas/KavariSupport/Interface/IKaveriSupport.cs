#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   KaveriSupportApiController.cs
    * Author Name       :   - Akash Patil
    * Creation Date     :   - 02-05-2019
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for kaveri support module.
*/
#endregion

#region References

using CustomModels.Models.KaveriSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace ECDataAPI.Areas.KavariSupport.Interface
{
    public interface IKaveriSupport
    {
        AppDeveloperViewModel RegisterTicketDetailsAndGenerateKeyPair(AppDeveloperViewModel viewModel);

        DecryptEnclosureModel NewDecryptAndSave(string zipFilePath,string TicketNumber ,out string DecryptFilepath);

        string IsTicketExists(string TicketNumber);

        AppDeveloperViewModel EncryptSQLPatchFile(AppDeveloperViewModel viewModel);

        AppDeveloperViewModel GetTicketRegistrationDetails();

        #region Listing for ticket details

        TicketDetailsListModel LoadTicketDetailsList();

        TicketDetailsListModel LoadPrivateKeyDetailsList();

        #endregion


    }
}
