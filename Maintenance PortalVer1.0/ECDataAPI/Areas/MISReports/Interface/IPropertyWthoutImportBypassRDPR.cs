/*File Header
 * Project Id: 
 * Project Name: Kaveri Maintainance Portal
 * File Name: IKaveriIntegration
 * Author : Shubham Bhagat
 * Creation Date : 14 Oct 2019
 * Desc : Contract  
 * ECR No : 
*/

using CustomModels.Models.MISReports.PropertyWthoutImportBypassRDPR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IPropertyWthoutImportBypassRDPR
    {
        ReportModel ReportView(int OfficeID);
        ReportWrapperModel LoadReportTable(ReportModel model);
        ReportDetailsWrapperModel OtherTableDetailsBypassRDPR(ReportModel model);
    }
}
