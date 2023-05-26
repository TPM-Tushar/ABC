using CustomModels.Models.MISReports.OtherDepartmentStatus;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class OtherDepartmentStatusBAL: IOtherDepartmentStatus
    {
        IOtherDepartmentStatus misReportsDal = new OtherDepartmentStatusDAL();
       

        public OtherDepartmentStatusModel OtherDepartmentStatusView(int OfficeID)
        {
            return misReportsDal.OtherDepartmentStatusView(OfficeID);
        }
     
        public List<OtherDepartmentStatusDetailsModel> OtherDepartmentStatusDetails(OtherDepartmentStatusModel model)
        {
            return misReportsDal.OtherDepartmentStatusDetails(model);
        }
       
        public int OtherDepartmentStatusDetailsTotalCount(OtherDepartmentStatusModel model)
        {
            return misReportsDal.OtherDepartmentStatusDetailsTotalCount(model);
        }
    }
}