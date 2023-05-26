#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   RefundChallanAPIController.cs
    * Author Name       :   Shivam B
    * Creation Date     :   15-Feb-2012
    * Last Modified By  :   Shivam B
    * Last Modified On  :   07-Apr-2022
    * Description       :   API controller for Refund Challan
*/
#endregion

using CustomModels.Models.RefundChallan;
using ECDataAPI.Areas.RefundChallan.BAL;
using ECDataAPI.Areas.RefundChallan.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.RefundChallan.Controllers
{
    public class RefundChallanAPIController : ApiController
    {
        IRefundChallan balObject = null;


        [HttpGet]
        [Route("api/RefundChallanAPIController/RefundChallanView")]
        public IHttpActionResult RefundChallanView(int officeID, int LevelID)
        {
            try
            {
                balObject = new RefundChallanBAL();
                RefundChallanViewModel resModel = balObject.RefundChallanView(officeID, LevelID);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/RefundChallanAPIController/LoadRefundChallanDetailsTable")]
        public IHttpActionResult LoadRefundChallanDetailsTable(int DroCode, int SROCode, int RoleID)
        {
            try
            {
                balObject = new RefundChallanBAL();
                // List<RefundChallanTableModel> resModel = new List<RefundChallanTableModel>();
                RefundChallanResultModel resModel = new RefundChallanResultModel();
                resModel = balObject.LoadRefundChallanDetailsTable(DroCode, SROCode, RoleID);
                return Ok(resModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/RefundChallanAPIController/RefundChallanAddEditDetails")]
        public IHttpActionResult RefundChallanAddEditDetails(long RowId)
        {
            try
            {
                balObject = new RefundChallanBAL();
                RefundChallanViewModel resModel = balObject.RefundChallanAddEditDetails(RowId);
                return Ok(resModel);
            }

            catch (Exception)
            {
                throw;
            }
        }

        
        [HttpPost]
        [Route("api/RefundChallanAPIController/SaveRefundChallanDetails")]
        public IHttpActionResult SaveRefundChallanDetails(RefundChallanViewModel refundChallanViewModel)
        {
            try
            {
                balObject = new RefundChallanBAL();
                RefundChallanOrderResultModel resModel = new RefundChallanOrderResultModel();
                resModel = balObject.SaveRefundChallanDetails(refundChallanViewModel);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/RefundChallanAPIController/UpdateRefundChallanDetails")]
        public IHttpActionResult UpdateRefundChallanDetails(RefundChallanViewModel refundChallanViewModel)
        {
            try
            {
                balObject = new RefundChallanBAL();
                RefundChallanOrderResultModel resModel = new RefundChallanOrderResultModel();
                resModel = balObject.UpdateRefundChallanDetails(refundChallanViewModel);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        

        [HttpGet]
        [Route("api/RefundChallanAPIController/FinalizeRefundChallanDetails")]
        public IHttpActionResult FinalizeRefundChallanDetails(long RowId)
        {
            try
            {
                balObject = new RefundChallanBAL();
                string finalizeDECResp = balObject.FinalizeRefundChallanDetails(RowId);
                return Ok(finalizeDECResp);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpGet]
        [Route("api/RefundChallanAPIController/ViewBtnClickOrderTable")]
        public IHttpActionResult ViewBtnClickOrderTable(long RowId)
        {
            try
            {
                balObject = new RefundChallanBAL();
                RefundChallanDROrderResultModel resModel = balObject.ViewBtnClickOrderTable(RowId);
                return Ok(resModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
