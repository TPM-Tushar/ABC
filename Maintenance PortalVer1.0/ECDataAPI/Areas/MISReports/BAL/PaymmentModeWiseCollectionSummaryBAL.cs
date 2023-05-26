using CustomModels.Models.MISReports.PaymmentModeWiseCollectionSummary;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class PaymmentModeWiseCollectionSummaryBAL : IPaymentModeWiseCollectionSummary
    {
        IPaymentModeWiseCollectionSummary misReportsDal = new PaymmentModeWiseCollectionSummaryDAL();

        public PaymmentModeWiseCollectionSummaryView PaymentModeWiseCollectionSummaryView(int OfficeID)
        {
            return misReportsDal.PaymentModeWiseCollectionSummaryView(OfficeID);

        }

        public PaymentModeWiseCollectionSummaryResModel GetPaymentModeWiseRPTTableData(PaymmentModeWiseCollectionSummaryView model)
        {
            return misReportsDal.GetPaymentModeWiseRPTTableData(model);

        }
    }
}