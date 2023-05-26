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

using CustomModels.Models.Blockchain;
using ECDataAPI.Areas.Blockchain.BAL;
using ECDataAPI.Areas.Blockchain.Interface;

//using ECDataAPI.Areas.DataEntryCorrection.BAL;
//using ECDataAPI.Areas.DataEntryCorrection.Interface;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.Blockchain.Controllers
{
    public class BlockchainAPIController : ApiController
    {
        IBlockchain balObject = null;

        //Added by Madhusoodan on 05/08/2021
 
        [HttpGet]
        [Route("api/BlockchainAPIController/BlockchainView")]
        public IHttpActionResult BlockchainView(int officeID,int LevelID, long UserID)
        {
            try
            {
                balObject = new BlockchainBAL();

                BlockchainViewModel resModel = balObject.BlockchainView(officeID,LevelID,UserID);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/BlockchainAPIController/GetSroCodebyDistrict")]
        public IHttpActionResult GetSroCodebyDistrict(int DROCode)
        {
            try
            {
                balObject = new BlockchainBAL();

                BlockchainViewModel resModel = balObject.GetSroCodebyDistrict(DROCode);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }
        //Added by Madhur
        [HttpGet]
        [Route("api/BlockchainAPIController/LoadDetailsTable")]
        public IHttpActionResult LoadDetailsTable(int DroCode, int SROCode)
        {
            try
            {
                balObject = new BlockchainBAL();
                List<BlockchainApprovalTableModel> resModel = new List<BlockchainApprovalTableModel>();
                resModel = balObject.LoadDetailsTable(DroCode, SROCode);
                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/BlockchainAPIController/Approval")]
        public IHttpActionResult Approval(List<string> appList)
        {

            try
            {
                balObject = new BlockchainBAL();
                string resModel = balObject.Approval(appList);
                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
