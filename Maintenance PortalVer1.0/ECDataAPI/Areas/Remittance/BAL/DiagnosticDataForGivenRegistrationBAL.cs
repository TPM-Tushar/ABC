#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DiagnosticDataForGivenRegistrationBAL.cs
    * Author Name       :   Pankaj Sakhare
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.DiagnosticDataForGivenRegistration;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class DiagnosticDataForGivenRegistrationBAL : IDiagnosticDataForGivenRegistration
    {
        IDiagnosticDataForGivenRegistration dalOBJ = new DiagnosticDataForGivenRegistrationDAL();


        /// <summary>
        /// DownloadDiagnosticDataInsertScript
        /// </summary>
        /// <param name="model"></param>
        /// <returns>DownloadDiagnosticDataScript</returns>
        public DownloadDiagnosticDataScript DownloadDiagnosticDataInsertScript(DiagnosticDataForRegistrationModel model)
        {
            return dalOBJ.DownloadDiagnosticDataInsertScript(model);
        }
    }
}