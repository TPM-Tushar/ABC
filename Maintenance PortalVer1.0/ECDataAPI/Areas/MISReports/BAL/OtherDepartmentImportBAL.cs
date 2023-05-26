using CustomModels.Models.Common;
using CustomModels.Models.MISReports.OtherDepartmentImport;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class OtherDepartmentImportBAL: IOtherDepartmentImport
    {
        public OtherDepartmentImportREQModel OtherDepartmentImportView(int OfficeID)
        {
            return (new OtherDepartmentImportDAL().OtherDepartmentImportView(OfficeID));
        }

        public OtherDepartmentImportWrapper OtherDepartmentImportDetails(OtherDepartmentImportREQModel model)
        {
            return (new OtherDepartmentImportDAL().OtherDepartmentImportDetails(model));
        }

        public int OtherDepartmentImportCount(OtherDepartmentImportREQModel model)
        {
            return (new OtherDepartmentImportDAL().OtherDepartmentImportCount(model));
        }

        public XMLResModel GetXMLContent(OtherDepartmentImportREQModel model)
        {
            return (new OtherDepartmentImportDAL().GetXMLContent(model));
        }
        //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration

        public XMLResModel FormIIIDownloadFun(OtherDepartmentImportREQModel model)
        {
            return (new OtherDepartmentImportDAL().FormIIIDownloadFun(model));
        }

        public XMLResModel ViewTransXMLFun(OtherDepartmentImportREQModel model)
        {
            return (new OtherDepartmentImportDAL().ViewTransXMLFun(model));
        }

    }
}