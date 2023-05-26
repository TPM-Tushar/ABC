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
    public class BlockchainApprovalListAPIController : ApiController
    {
        IBlockchainApprovalList balObject = null;

 
        [HttpGet]
        [Route("api/BlockchainApprovalListAPIController/BlockchainApprovalListView")]
        public IHttpActionResult BlockchainApprovalListView(int officeID,int LevelID, long UserID)
        {
            try
            {
                balObject = new BlockchainApprovalListBAL();

                BlockchainViewModel resModel = balObject.BlockchainApprovalListView(officeID,LevelID,UserID);

                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/BlockchainApprovalListAPIController/GetSroCodebyDistrict")]
        public IHttpActionResult GetSroCodebyDistrict(int DROCode)
        {
            try
            {
                balObject = new BlockchainApprovalListBAL();

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
        [Route("api/BlockchainApprovalListAPIController/LoadDetailsTable")]
        public IHttpActionResult LoadDetailsTable(int DroCode, int SROCode)
        {
            try
            {
                balObject = new BlockchainApprovalListBAL();
                List<BlockchainApprovalTableModel> resModel = new List<BlockchainApprovalTableModel>();
                resModel = balObject.LoadDetailsTable(DroCode, SROCode);
                return Ok(resModel);
            }

            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
