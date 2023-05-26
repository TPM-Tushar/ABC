#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   RefundChallanApproveAPIController.cs
    * Author Name       :   Shivam B
    * Creation Date     :   08-Mar-2012
    * Last Modified By  :   Shivam B
    * Last Modified On  :   07-Apr-2022
    * Description       :   API controller for Refund Challan Approve
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
    public class RefundChallanApproveAPIController : ApiController
    {
        IRefundChallanApprove balObject = null;

        [HttpGet]
        [Route("api/RefundChallanApproveAPIController/RefundChallanApproveView")]
        public IHttpActionResult RefundChallanApproveView(int officeID, int LevelID, long UserID)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                RefundChallanApproveViewModel resModel = balObject.RefundChallanApproveView(officeID, LevelID, UserID);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/RefundChallanApproveAPIController/LoadRefundChallanApproveTable")]
        public IHttpActionResult LoadRefundChallanApproveTable(int DroCode, int SROCode, int RoleID, bool IsExcel)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                List<RefundChallanApproveTableModel> resModel = new List<RefundChallanApproveTableModel>();
                resModel = balObject.LoadRefundChallanApproveTable(DroCode, SROCode, RoleID, IsExcel);
                return Ok(resModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/RefundChallanApproveAPIController/RefundChallanApproveAddEditOrder")]
        public IHttpActionResult RefundChallanApproveAddEditOrder(long RowId, int OfficeID, int LevelID)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                RefundChallanApproveViewModel resModel = balObject.RefundChallanApproveAddEditOrder(RowId, OfficeID, LevelID);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        
        [HttpGet]
        [Route("api/RefundChallanApproveAPIController/IsChallanNoExists")]
        public IHttpActionResult IsChallanNoExist(string InstrumentNumber, string InstrumentDate, long RowId)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                RefundChallanOrderResultModel resModel = new RefundChallanOrderResultModel();
                resModel = balObject.IsChallanNoExist(InstrumentNumber, InstrumentDate, RowId);
                return Ok(resModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/RefundChallanApproveAPIController/CheckifOrderNoExist")]
        public IHttpActionResult CheckifOrderNoExist(string OrderNo, long RowId)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                bool resModel = balObject.CheckifOrderNoExist(OrderNo, RowId);
                return Ok(resModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/RefundChallanApproveAPIController/SaveRefundChallanApproveDetails")]
        public IHttpActionResult SaveRefundChallanApproveDetails(RefundChallanApproveViewModel refundChallanModel)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                RefundChallanOrderResultModel resModel = new RefundChallanOrderResultModel();
                resModel = balObject.SaveRefundChallanApproveDetails(refundChallanModel);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("api/RefundChallanApproveAPIController/GenerateNewOrderID")]
        public IHttpActionResult GenerateNewOrderID(int OfficeID,  long RowId)
        {
            try
            {
                return Ok(new RefundChallanApproveBAL().GenerateNewOrderID(OfficeID,  RowId));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/RefundChallanApproveAPIController/ViewBtnClickOrderTable")]
        public IHttpActionResult ViewBtnClickOrderTable(long RowId)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                RefundChallanDROrderResultModel resModel = balObject.ViewBtnClickOrderTable(RowId);
                return Ok(resModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/RefundChallanApproveAPIController/FinalizeApproveDROrder")]
        public IHttpActionResult FinalizeApproveDROrder(long RowId, long UserId)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                string FinalizeDROrderResp = balObject.FinalizeApproveDROrder(RowId, UserId);
                return Ok(FinalizeDROrderResp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/RefundChallanApproveAPIController/FinalizeRejectDROrder")]
        public IHttpActionResult FinalizeRejectDROrder(long RowId, long UserID)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                string FinalizeDROrderResp = balObject.FinalizeRejectDROrder(RowId, UserID);
                return Ok(FinalizeDROrderResp);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/RefundChallanApproveAPIController/SaveRefundChallanRejectionDetails")]
        public IHttpActionResult SaveRefundChallanRejectionDetails(RefundChallanRejectionViewModel refundChallanModel)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                RefundChallanOrderResultModel resModel = new RefundChallanOrderResultModel();
                resModel = balObject.SaveRefundChallanRejectionDetails(refundChallanModel);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("api/RefundChallanApproveAPIController/DeleteCurrentOrderFile")]
        public IHttpActionResult DeleteCurrentOrderFile(RefundChallanApproveViewModel viewmodel)
        {
            try
            {
                balObject = new RefundChallanApproveBAL();
                
                RefundChallanOrderResultModel resultModel = balObject.DeleteCurrentOrderFile(viewmodel.RowId);

                return Ok(resultModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
