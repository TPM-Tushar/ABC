using CustomModels.Models.SROScriptManager;
using ECDataAPI.Areas.SROScriptManager.BAL;
using ECDataAPI.Areas.SROScriptManager.Interface;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.SROScriptManager.Controllers
{
    public class SROScriptManagerAPIController : ApiController
    {
        IScriptManager balObj = null;

        [HttpPost]
        [Route("api/SROScriptManagerAPIController/InsertInScriptManagerDetails")]
        [EventApiAuditLogFilter(Description = "Insert In Script Manager Details")]

        public SROScriptManagerModel InsertInScriptManagerDetails(SROScriptManagerModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();

                return balObj.InsertInScriptManagerDetails(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROScriptManagerAPIController/LoadScriptManagerTable")]
        [EventApiAuditLogFilter(Description = "Load Script Manager Table")]
        public ScriptManagerDetailWrapperModel LoadScriptManagerTable(SROScriptManagerModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                return balObj.LoadScriptManagerTable(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROScriptManagerAPIController/GetScriptDetails")]
        [EventApiAuditLogFilter(Description = "Get Script Details")]
        public SROScriptManagerModel GetScriptDetails(SROScriptManagerModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                return balObj.GetScriptDetails(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROScriptManagerAPIController/UpdateInScriptManagerDetails")]
        [EventApiAuditLogFilter(Description = "Update In Script Manager Details")]
        public SROScriptManagerModel UpdateInScriptManagerDetails(SROScriptManagerModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();

                return balObj.UpdateInScriptManagerDetails(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/SROScriptManagerAPIController/AppVersionDetails")]
        public IHttpActionResult AppVersionDetails(int OfficeID)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                AppVersionDetailsModel ViewModel = new AppVersionDetailsModel();

                ViewModel = balObj.AppVersionDetails(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROScriptManagerAPIController/LoadAppVersionDetails")]
        [EventApiAuditLogFilter(Description = "Load App Version Details")]
        public AppVersionDetailWrapperModel LoadAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                return balObj.LoadAppVersionDetails(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROScriptManagerAPIController/AddAppVersionDetails")]
        [EventApiAuditLogFilter(Description = "AddAppVersionDetails")]
        public AppVersionDetailsModel AddAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                return balObj.AddAppVersionDetails(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROScriptManagerAPIController/GetAppVersionDetails")]
        [EventApiAuditLogFilter(Description = "GetAppVersionDetails")]
        public AppVersionDetailsModel GetAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                return balObj.GetAppVersionDetails(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/SROScriptManagerAPIController/UpdateAppVersionDetails")]
        [EventApiAuditLogFilter(Description = "UpdateAppVersionDetails")]
        public AppVersionDetailsModel UpdateAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                return balObj.UpdateAppVersionDetails(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROScriptManagerAPIController/InsertInDROScriptManagerDetails")]
        [EventApiAuditLogFilter(Description = "Insert In DRO Script Manager Details")]

        public DROScriptManagerModel InsertInDROScriptManagerDetails(DROScriptManagerModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();

                return balObj.InsertInDROScriptManagerDetails(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROScriptManagerAPIController/LoadDROScriptManagerTable")]
        [EventApiAuditLogFilter(Description = "Load DRO Script Manager Table")]

        public DROScriptManagerDetailWrapperModel LoadDROScriptManagerTable(DROScriptManagerModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                return balObj.LoadDROScriptManagerTable(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROScriptManagerAPIController/GetDROScriptDetails")]
        [EventApiAuditLogFilter(Description = "Get DRO Script Details")]

        public DROScriptManagerModel GetDROScriptDetails(DROScriptManagerModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                return balObj.GetDROScriptDetails(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SROScriptManagerAPIController/UpdateInDROScriptManagerDetails")]
        [EventApiAuditLogFilter(Description = "Update In DRO Script Manager Details")]

        public DROScriptManagerModel UpdateInDROScriptManagerDetails(DROScriptManagerModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();

                return balObj.UpdateInDROScriptManagerDetails(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Omkar on 17-08-2020
        [HttpGet]
        [Route("api/SROScriptManagerAPIController/ApplyAppVersionView")]
        public IHttpActionResult ApplyAppVersionView()
        {
            try
            {
                balObj = new ScriptManagerBAL();
                ApplyAppVersionModel ViewModel = new ApplyAppVersionModel();

                ViewModel = balObj.ApplyAppVersionView();

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Omkar on 28-08-2020
        [HttpGet]
        [Route("api/SROScriptManagerAPIController/SRDRList")]
        public IHttpActionResult SRDRList()
        {
            try
            {
                balObj = new ScriptManagerBAL();
                ApplyAppVersionModel ViewModel = new ApplyAppVersionModel();

                ViewModel = balObj.SRDRList();

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Omkar on 09-09-2020
        [HttpGet]
        [Route("api/SROScriptManagerAPIController/DRList")]
        public IHttpActionResult DRList()
        {
            try
            {
                balObj = new ScriptManagerBAL();
                ApplyAppVersionModel ViewModel = new ApplyAppVersionModel();

                ViewModel = balObj.DRList();

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Added by Omkar on 18-08-2020

        [HttpPost]
        [Route("api/SROScriptManagerAPIController/ApplyAppVersion")]
        public IHttpActionResult ApplyAppVersion(ApplyAppVersionModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                ApplyAppVersionModel ViewModel = new ApplyAppVersionModel();

                ViewModel = balObj.ApplyAppVersion(viewModel);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by Omkar on 09-09-2020

        [HttpPost]
        [Route("api/SROScriptManagerAPIController/ApplyAppVersionDR")]
        public IHttpActionResult ApplyAppVersionDR(ApplyAppVersionModel viewModel)
        {
            try
            {
                balObj = new ScriptManagerBAL();
                ApplyAppVersionModel ViewModel = new ApplyAppVersionModel();

                ViewModel = balObj.ApplyAppVersionDR(viewModel);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
