
using ECDataAPI.EcDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECDataAPI.Areas.RefundChallan.DAL;
using ECDataAPI.Areas.RefundChallan.Interface;
using CustomModels.Models.RefundChallan;

namespace ECDataAPI.Areas.RefundChallan.BAL
{
    public class RefundChallanBAL : IRefundChallan
    {
        IRefundChallan refundChallanDAL = new RefundChallanDAL();

        public RefundChallanViewModel RefundChallanView(int officeID, int LevelID)
        {
            return refundChallanDAL.RefundChallanView(officeID, LevelID);
        }

        public RefundChallanResultModel LoadRefundChallanDetailsTable(int DroCode, int SROCode, int RoleID)
        {
            return refundChallanDAL.LoadRefundChallanDetailsTable(DroCode, SROCode, RoleID);
        }

        public RefundChallanViewModel RefundChallanAddEditDetails(long RowId)
        {
            return refundChallanDAL.RefundChallanAddEditDetails(RowId);
        }

        public RefundChallanOrderResultModel SaveRefundChallanDetails(RefundChallanViewModel refundChallanViewModel)
        {
            return refundChallanDAL.SaveRefundChallanDetails(refundChallanViewModel);
        }

        public RefundChallanOrderResultModel UpdateRefundChallanDetails(RefundChallanViewModel refundChallanViewModel)
        {
            return refundChallanDAL.UpdateRefundChallanDetails(refundChallanViewModel);
        }
        
        public string FinalizeRefundChallanDetails(long RowId)
        {
            return refundChallanDAL.FinalizeRefundChallanDetails(RowId);
        }

        public RefundChallanDROrderResultModel ViewBtnClickOrderTable(long RowId)
        {
            return refundChallanDAL.ViewBtnClickOrderTable(RowId);
        }



    }
}