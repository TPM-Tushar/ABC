#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   DailyReceiptDetailsAPIController.cs
    * Author Name       :   Raman Kalegaonkar 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion



using CustomModels.Models.MISReports.DailyReceiptDetails;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class DailyReceiptDetailsAPIController : ApiController
    {
        IDailyReceiptDetails balObject = null;
        /// <summary>
        /// Returns DailyReceiptDetailsViewModel Required to show DailyReceiptDetailsViewModel
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/DailyReceiptDetailsAPIController/DailyReceiptDetails")]

        public IHttpActionResult DailyReceiptDetails(int OfficeID)
        {
            try
            {
                balObject = new DailyReceiptDetailsBAL();
                DailyReceiptDetailsViewModel ViewModel = new DailyReceiptDetailsViewModel();

                ViewModel = balObject.DailyReceiptDetails(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns Total Count of DailyReceiptDetails List
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DailyReceiptDetailsAPIController/GetDailyReceiptsTotalCount")]
        public IHttpActionResult GetDailyReceiptsTotalCount(DailyReceiptDetailsViewModel model)
        {
            try
            {
                balObject = new DailyReceiptDetailsBAL();
                int totalcount = balObject.GetDailyReceiptsTotalCount(model);
                return Ok(totalcount);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns Daily Receipt Details Table Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DailyReceiptDetailsAPIController/GetDailyReceiptTableData")]
        public IHttpActionResult GetDailyReceiptTableData(DailyReceiptDetailsViewModel model)
        {
            try
            {
                balObject = new DailyReceiptDetailsBAL();
                DailyReceiptDetailsResModel responseModel = new DailyReceiptDetailsResModel();
                responseModel = balObject.GetDailyReceiptTableData(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
