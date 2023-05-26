#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsAPIController.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   API controller for Support Enclosure
*/
#endregion

using CustomModels.Models.BhoomiMapping;
using ECDataAPI.Areas.BhoomiMapping.BAL;
using ECDataAPI.Areas.BhoomiMapping.Interface;

//using ECDataAPI.Areas.DataEntryCorrection.BAL;
//using ECDataAPI.Areas.DataEntryCorrection.Interface;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.BhoomiMapping.Controllers
{
    public class BhoomiMappingDetailsAPIController : ApiController
    {
        IBhoomiMappingDetails balObject = null;


        [HttpGet]
        [Route("api/BhoomiMappingDetailsAPIController/BhoomiMappingDetailsView")]
        public IHttpActionResult BhoomiMappingDetailsView(int officeID, int LevelID, long UserID)
        {
            try
            {
                balObject = new BhoomiMappingDetailsBAL();

                BhoomiMappingViewModel resModel = balObject.BhoomiMappingDetailsView(officeID, LevelID, UserID);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/BhoomiMappingDetailsAPIController/LoadDetailsTable")]
        public IHttpActionResult LoadDetailsTable(int SROCode) 
        {
            try
            {
                balObject = new BhoomiMappingDetailsBAL();

                List<BhoomiMappingTableModel> resModel = balObject.LoadDetailsTable(SROCode);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}
