#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DiagnosticDataForGivenRegistrationDAL.cs
    * Author Name       :   Pankaj Sakhare
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.DiagnosticDataForGivenRegistration;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class DiagnosticDataForGivenRegistrationDAL : IDiagnosticDataForGivenRegistration
    {
        #region Properties
        KaveriEntities dbContext = new KaveriEntities();
        #endregion

        /// <summary>
        /// DownloadDiagnosticDataInsertScript
        /// </summary>
        /// <param name="model"></param>
        /// <returns>DownloadDiagnosticDataScript</returns>
        public DownloadDiagnosticDataScript DownloadDiagnosticDataInsertScript(DiagnosticDataForRegistrationModel model)
        {
            DownloadDiagnosticDataScript resModel = new DownloadDiagnosticDataScript();

            try
            {
                dbContext = new KaveriEntities();
                var result = dbContext.USP_GenerateDignosticData_ECDATA_GivenNO(model.RegistrationModuleCode, model.FinalRegistrationNumber);
                

                if(result != null)
                {                   
                    var script = result.ToArray();
                    resModel.FileContent = script;

                    //byte[] bytes = new byte[script.Length];
                    //for (int i = 0; i < script.Length; i++)
                    //{
                    //    bytes[i] = Byte.Parse(script[i]);
                    //}

                    //foreach (var r in script)
                    //{
                    //    Convert.ToByte(r);
                    //}

                    //var rr = Array.ConvertAll(script, Byte.Parse);
                }

            }
            catch (Exception ex)
            {
                if(ex.InnerException.Message == "No Record Found" )
                {
                    ApiExceptionLogs.LogError(ex);
                    resModel.ErrorMessage = "No Record Found";
                    return resModel;
                }
                ApiExceptionLogs.LogError(ex);
                return null;
            }
            finally
            {
                if (dbContext != null) dbContext.Dispose();
            }
            return resModel;
        }
    }
}