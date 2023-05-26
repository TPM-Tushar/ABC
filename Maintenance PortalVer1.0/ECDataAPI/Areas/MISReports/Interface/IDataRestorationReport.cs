#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   IDataRestorationReport.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for MIS Reports module.
	* ECR No			:	431
*/
#endregion
using CustomModels.Models.MISReports.DataRestorationReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IDataRestorationReport
    {
        DataRestorationReportViewModel DataRestorationReport(int OfficeID);

        DataRestorationPartialViewModel DataRestorationReportStatus(DataRestorationReportViewModel model);

        DataRestorationReportResModel InitiateDatabaseRestoration(DataRestorationReportReqModel model);
        
        DataRestorationReportResModel GenerateKeyAfterExpiration(DataRestorationReportReqModel model);

        DataRestorationReportResModel ApproveScript(DataRestorationReportReqModel model);

        DataRestorationPartialViewModel DataInsertionTable(DataRestorationReportReqModel model);

        DataRestorationReportResModel DownloadScriptPathVerify(DataRestorationReportReqModel model);

        DataRestorationReportResModel DownloadScriptForRectification(DataRestorationReportReqModel model);

        DataRestorationReportResModel SaveUplodedRectifiedScript(DataRestorationReportReqModel model);

        #region ADDED BY PANKAJ ON 16-07-2020
        DataRestorationReportResModel ConfirmDataInsertion(DataRestorationReportReqModel model);
        #endregion


        DataRestorationPartialViewModel LoadInitiateMasterTable(DataRestorationReportReqModel model);


        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
        AbortViewModel AbortView(String INIT_ID);

        AbortViewModel SaveAbortData(AbortViewModel model);
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

        
              DataRestorationPartialViewModel DataRestorationReportStatusForScript(DataRestorationReportViewModel model);
    }
}
