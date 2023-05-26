using CustomModels.Models.Common;
using CustomModels.Models.MISReports.OtherDepartmentImport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IOtherDepartmentImport
    {
        OtherDepartmentImportREQModel OtherDepartmentImportView(int OfficeID);
        OtherDepartmentImportWrapper OtherDepartmentImportDetails(OtherDepartmentImportREQModel model);
        int OtherDepartmentImportCount(OtherDepartmentImportREQModel model);
        XMLResModel GetXMLContent(OtherDepartmentImportREQModel model);
        //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
        XMLResModel FormIIIDownloadFun(OtherDepartmentImportREQModel model);

        XMLResModel ViewTransXMLFun(OtherDepartmentImportREQModel model);

    }
}
