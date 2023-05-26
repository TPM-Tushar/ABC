using CustomModels.Models.Remittance.ChallanDetailsReport;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class ChallanDetailsBAL : IChallanDetails
    {
        IChallanDetails dalOBJ = new ChallanDetailsDAL();

        public ChallanDetailsModel ChallanDetailsReportView()
        {
            return dalOBJ.ChallanDetailsReportView();
        }

        public ChallanDetailsResModel GetChallanReportDetails(ChallanDetailsModel model)
        {
            return dalOBJ.GetChallanReportDetails(model);
        }
    }
}