#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   KaveriSupportApiController.cs
    * Author Name       :   - Akash Patil
    * Creation Date     :   - 02-05-2019
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for kaveri support module.
*/
#endregion


#region References
using CustomModels.Models.KaveriSupport;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.KavariSupport.BAL;
using ECDataAPI.Areas.KavariSupport.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http; 
#endregion

namespace ECDataAPI.Areas.KavariSupport.Controllers
{
    public class KaveriSupportApiController : ApiController
    {
        IKaveriSupport balObj = null;

        [HttpPost]
        [Route("api/KaveriSupportApiController/RegisterTicketDetailsAndGenerateKeyPair")]
        [EventApiAuditLogFilter(Description = "Register Ticket Details And Generate Key Pair")]

        public AppDeveloperViewModel RegisterTicketDetailsAndGenerateKeyPair(AppDeveloperViewModel viewModel)
        {
            try
            {
                balObj = new KaveriSupportBAL();

                viewModel = balObj.RegisterTicketDetailsAndGenerateKeyPair(viewModel);
                if (viewModel == null)
                    return null;
                else
                    return viewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/KaveriSupportApiController/DecryptEnclosureFile")]
        [EventApiAuditLogFilter(Description = "Decrypt Enclosure File")]

        public DecryptEnclosureModel DecryptEnclosureFile(AppDeveloperViewModel viewModel)
        {
            try
            {
                balObj = new KaveriSupportBAL();
                string path = string.Empty;

                DecryptEnclosureModel model = balObj.NewDecryptAndSave(viewModel.Filepath, viewModel.TicketNumber, out path);
                model.Filepath = path;
                return model;



            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        [Route("api/KaveriSupportApiController/IsTicketExists")]
        public string IsTicketExists(AppDeveloperViewModel model)
        {
            try
            {
                balObj = new KaveriSupportBAL();
                string response = balObj.IsTicketExists(model.TicketNumber);
                return response;
            }
            catch (Exception)
            {
                throw;
            }

        }


         [HttpPost]
        [Route("api/KaveriSupportApiController/EncryptSQLPatchFile")]
        [EventApiAuditLogFilter(Description = "Encrypt SQL Patch File")]

        public AppDeveloperViewModel EncryptSQLPatchFile(AppDeveloperViewModel viewModel)
        {
            try
            {
                balObj = new KaveriSupportBAL();
                string path = string.Empty;

                var response = balObj.EncryptSQLPatchFile(viewModel);
                return response;
            }
            catch (Exception )
            {
                throw;
            }

        }

        [HttpGet]
        [Route("api/KaveriSupportApiController/GetTicketRegistrationDetails")]
        public AppDeveloperViewModel GetTicketRegistrationDetails()
        {
            try
            {
                balObj = new KaveriSupportBAL();
                string path = string.Empty;

                var response = balObj.GetTicketRegistrationDetails();
                return response;
            }
            catch (Exception)
            {
                throw;
            }

        }





        #region Listing of Ticket Details

        [HttpGet]
        [Route("api/KaveriSupportApiController/LoadTicketDetailsList")]
        public IHttpActionResult LoadTicketDetailsList()
        {
            try
            {
                balObj = new KaveriSupportBAL();

                TicketDetailsListModel result = balObj.LoadTicketDetailsList();
                return Ok(result);

            }
            catch (Exception) { throw; }
        }


        


        [HttpGet]
        [Route("api/KaveriSupportApiController/LoadPrivateKeyDetailsList")]
        public IHttpActionResult LoadPrivateKeyDetailsList()
        {
            try
            {
                balObj = new KaveriSupportBAL();

                TicketDetailsListModel result = balObj.LoadPrivateKeyDetailsList();
                return Ok(result);

            }
            catch (Exception) { throw; }
        }

        #endregion

    }
}
