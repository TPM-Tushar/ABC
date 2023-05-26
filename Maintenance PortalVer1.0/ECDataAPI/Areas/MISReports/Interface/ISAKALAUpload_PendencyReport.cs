using CustomModels.Models.Common;
using CustomModels.Models.MISReports.SAKALAUpload_PendencyReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface ISAKALAUpload_PendencyReport
    {
        SAKALAUploadRptViewModel SAKALUploadReportView(int OfficeID);

        SAKALAUploadRptResModel LoadSakalaUploadReportDataTable(SAKALAUploadRptViewModel model);
        XMLResModel GetXMLContent(XMLInputForSAKALAUploadModel InputModel);



    }
}
