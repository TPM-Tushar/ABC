using CustomModels.Models.RefundChallan;
using ECDataAPI.Areas.RefundChallan.DAL;
using ECDataAPI.Areas.RefundChallan.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.RefundChallan.BAL
{
    public class RefundChallanApproveBAL : IRefundChallanApprove
    {
        IRefundChallanApprove refundChallanApproveDAL = new RefundChallanApproveDAL();

        public RefundChallanApproveViewModel RefundChallanApproveView(int officeID, int LevelID, long UserID)
        {
            return refundChallanApproveDAL.RefundChallanApproveView(officeID, LevelID, UserID);
        }

        public List<RefundChallanApproveTableModel> LoadRefundChallanApproveTable(int DroCode, int SROCode, int RoleID, bool IsExcel)
        {
            return refundChallanApproveDAL.LoadRefundChallanApproveTable(DroCode, SROCode, RoleID, IsExcel);
        }

        public RefundChallanApproveViewModel RefundChallanApproveAddEditOrder(long RowId, int OfficeID, int LevelID)
        {
            return refundChallanApproveDAL.RefundChallanApproveAddEditOrder(RowId, OfficeID, LevelID);
        }
        

        public RefundChallanOrderResultModel IsChallanNoExist(string InstrumentNumber, string InstrumentDate, long RowId)
        {
            return refundChallanApproveDAL.IsChallanNoExist(InstrumentNumber, InstrumentDate, RowId);
        }

        public bool CheckifOrderNoExist(string OrderNo, long RowId)
        {
            return refundChallanApproveDAL.CheckifOrderNoExist(OrderNo, RowId);
        }

        public RefundChallanOrderResultModel SaveRefundChallanApproveDetails(RefundChallanApproveViewModel refundChallanModel)
        {
            return refundChallanApproveDAL.SaveRefundChallanApproveDetails(refundChallanModel);
        }

        public RefundChallanDROrderResultModel GenerateNewOrderID(int OfficeID, long RowId)
        {
            return refundChallanApproveDAL.GenerateNewOrderID(OfficeID,  RowId);
        }

        public RefundChallanDROrderResultModel ViewBtnClickOrderTable(long RowId)
        {
            return refundChallanApproveDAL.ViewBtnClickOrderTable(RowId);
        }

        public string FinalizeApproveDROrder(long RowId, long UserId)
        {
            return refundChallanApproveDAL.FinalizeApproveDROrder(RowId, UserId);
        }

        public string FinalizeRejectDROrder(long RowId, long UserId)
        {
            return refundChallanApproveDAL.FinalizeRejectDROrder(RowId, UserId);
        }

        public RefundChallanOrderResultModel SaveRefundChallanRejectionDetails(RefundChallanRejectionViewModel refundChallanModel)
        {
            return refundChallanApproveDAL.SaveRefundChallanRejectionDetails(refundChallanModel);
        }

        public RefundChallanOrderResultModel DeleteCurrentOrderFile(long RowId)
        {
            return refundChallanApproveDAL.DeleteCurrentOrderFile(RowId);
        }

    }
}