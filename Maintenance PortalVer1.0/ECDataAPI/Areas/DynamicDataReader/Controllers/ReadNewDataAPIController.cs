using CustomModels.Models.DynamicDataReader;
using ECDataAPI.Areas.DynamicDataReader.BAL;
using ECDataAPI.Areas.DynamicDataReader.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.DynamicDataReader.Controllers
{
    public class ReadNewDataAPIController : ApiController
    {
        IReadNewData balObject = null;


        /// <summary>
        /// SaveNewQuerySearchParameter
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ReadNewDataAPIController/SaveNewQuerySearchParameter")]
        [EventApiAuditLogFilter(Description = "Save New Query Search Parameter", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult SaveNewQuerySearchParameter(ReadNewDataModel viewModel)
        {
            try
            {
                balObject = new ReadNewDataBAL();
                return Ok(balObject.SaveNewQuerySearchParameter(viewModel));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}