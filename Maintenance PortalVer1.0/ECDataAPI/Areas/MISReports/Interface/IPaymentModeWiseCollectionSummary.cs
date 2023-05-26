using CustomModels.Models.MISReports.PaymmentModeWiseCollectionSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IPaymentModeWiseCollectionSummary
    {
        PaymmentModeWiseCollectionSummaryView PaymentModeWiseCollectionSummaryView(int OfficeID);
        PaymentModeWiseCollectionSummaryResModel GetPaymentModeWiseRPTTableData(PaymmentModeWiseCollectionSummaryView model);


    }
}
