using CustomModels.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Dashboard.Interface
{
    interface IDashboard
    {
        DashboardDetailsViewModel DashboardDetailsView(int OfficeID);
        DashboardSummaryTblResModel LoadDashboardSumaryTable(DashboardDetailsViewModel model);
        GraphTableResponseModel LoadRevenueCollectedChartData(DashboardDetailsViewModel model);
        GraphTableResponseModel PopulateSurchargeCessBarChart(DashboardDetailsViewModel model);
        GraphTableResponseModel LoadHighValPropChartData(DashboardDetailsViewModel model);
        GraphTableResponseModel LoadRevenueTargetVsAchieved(DashboardDetailsViewModel model);
        GraphTableResponseModel PopulateProgressChart(DashboardDetailsViewModel model);
        DashboardSummaryModel DashboardSummaryView(int OfficeID);
        DashboardSummaryModel PopulateTiles(TilesReqModel reqModel);
        DashboardPopupViewModel LoadPopup(DashboardPopupReqModel reqModel);
        GraphTableResponseModel LoadDocumentRegisteredChartData(DashboardDetailsViewModel model);

        //ADDED BY SHUBHAM BHAGAT ON 1-4-2020        
        GraphTableResponseModel LoadHighValPropChartDataForDocs(DashboardDetailsViewModel model);
        #region ADDED BY SHUBHAM BHAGAT 09-04-2020

        NatureOfArticle_REQ_RES_Model NatureOfDocumentByRadioType(NatureOfArticle_REQ_RES_Model model);
        #endregion

        //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
        DashboardSummaryModel PopulateAvgRegTime(TilesReqModel reqModel);

        // BELOW CODE IS COMMENTED BY SHUBHAM BHAGAT ON  26-11-2020
        // BECAUSE WE ARE NOT PROVIDING PROGRESS CHART MONTH WISE LABLE CLICK FUNCTIONALITY NOW
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
        //GraphTableResponseModel PopulateProgressChartMonthWise(DashboardDetailsViewModel model);
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
        // ABOVE CODE IS COMMENTED BY SHUBHAM BHAGAT ON  26-11-2020

    }
}
