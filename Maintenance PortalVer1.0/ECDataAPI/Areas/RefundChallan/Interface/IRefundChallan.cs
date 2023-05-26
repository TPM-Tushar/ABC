
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
    public interface IRefundChallan
    {
        
        RefundChallanViewModel RefundChallanView(int officeID, int LevelID);

        RefundChallanViewModel RefundChallanAddEditDetails(long RowId);

        RefundChallanOrderResultModel SaveRefundChallanDetails(RefundChallanViewModel dataEntryCorrectionOrderViewModel);

        RefundChallanOrderResultModel UpdateRefundChallanDetails(RefundChallanViewModel dataEntryCorrectionOrderViewModel);


        RefundChallanResultModel LoadRefundChallanDetailsTable(int OfficeID, int SROCode, int RoleID);

        RefundChallanDROrderResultModel ViewBtnClickOrderTable(long RowId);

        string FinalizeRefundChallanDetails(long RowId);


    }
}
