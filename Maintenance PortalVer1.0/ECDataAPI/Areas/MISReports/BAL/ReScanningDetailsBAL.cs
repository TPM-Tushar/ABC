using CustomModels.Models.MISReports.ReScanningDetails;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ReScanningDetailsBAL : IReScanningDetails
    {
        IReScanningDetails rescanningDal = new ReScanningDetailsDAL();
        public ReScanningDetailsResModel GetReScanningTableData(ReScanningDetailsViewModel model)
        {
            return rescanningDal.GetReScanningTableData(model);
        }

        public int GetReScanningTotalCount(ReScanningDetailsViewModel model)
        {
            return rescanningDal.GetReScanningTotalCount(model);
        }

        public ReScanningDetailsViewModel ReScanningDetails(int OfficeID)
        {
            return rescanningDal.ReScanningDetails(OfficeID);
        }
    }
}