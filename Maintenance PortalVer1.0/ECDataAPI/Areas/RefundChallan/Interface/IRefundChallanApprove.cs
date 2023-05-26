
using CustomModels.Models.RefundChallan;
using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECDataAPI.Areas.RefundChallan.Interface
{
    public interface IRefundChallanApprove
    {

        RefundChallanApproveViewModel RefundChallanApproveView(int officeID, int LevelID, long UserID);

        RefundChallanApproveViewModel RefundChallanApproveAddEditOrder(long RowId, int OfficeID, int LevelID);

        RefundChallanOrderResultModel SaveRefundChallanApproveDetails(RefundChallanApproveViewModel refundChallanModel);

        List<RefundChallanApproveTableModel> LoadRefundChallanApproveTable(int OfficeID, int SROCode, int RoleID, bool IsExcel);

        
        RefundChallanDROrderResultModel ViewBtnClickOrderTable(long RowId);

        RefundChallanDROrderResultModel GenerateNewOrderID(int OfficeID,long RowId);

        bool CheckifOrderNoExist(string OrderNo, long RowId);

        RefundChallanOrderResultModel IsChallanNoExist(string InstrumentNumber, string InstrumentDate, long RowId);

        string FinalizeApproveDROrder(long RowId, long UserId);

        string FinalizeRejectDROrder(long RowId, long UserId);

        RefundChallanOrderResultModel SaveRefundChallanRejectionDetails(RefundChallanRejectionViewModel refundChallanModel);
        
        RefundChallanOrderResultModel DeleteCurrentOrderFile(long RowId);
    }
}
