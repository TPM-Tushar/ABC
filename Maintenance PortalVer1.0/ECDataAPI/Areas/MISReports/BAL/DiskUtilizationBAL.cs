using CustomModels.Models.MISReports.DiskUtilization;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class DiskUtilizationBAL: IDiskUtilization
    {
        public DiskUtilizationREQModel DiskUtilizationView(int OfficeID)
        {
            return (new DiskUtilizationDAL().DiskUtilizationView(OfficeID));
        }

        public DiskUtilizationWrapper DiskUtilizationDetails(DiskUtilizationREQModel model)
        {
            return (new DiskUtilizationDAL().DiskUtilizationDetails(model));
        }
    }
}