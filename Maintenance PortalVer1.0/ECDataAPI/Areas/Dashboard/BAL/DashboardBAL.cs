using CustomModels.Models.Dashboard;
using ECDataAPI.Areas.Dashboard.DAL;
using ECDataAPI.Areas.Dashboard.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Dashboard.BAL
{
    public class DashboardBAL : IDashboard
    {
        IDashboard misReportsDal = new DashboardDAL();

        public DashboardDetailsViewModel DashboardDetailsView(int OfficeID)
        {
            return misReportsDal.DashboardDetailsView(OfficeID);
        }
        public DashboardSummaryModel DashboardSummaryView(int OfficeID)
        {
            return misReportsDal.DashboardSummaryView(OfficeID);
        }

        public DashboardSummaryTblResModel LoadDashboardSumaryTable(DashboardDetailsViewModel model)
        {
            return misReportsDal.LoadDashboardSumaryTable(model);
        }


        public GraphTableResponseModel LoadRevenueCollectedChartData(DashboardDetailsViewModel model)
        {
            return misReportsDal.LoadRevenueCollectedChartData(model);
        }
        public GraphTableResponseModel LoadDocumentRegisteredChartData(DashboardDetailsViewModel model)
        {
                return misReportsDal.LoadDocumentRegisteredChartData(model);
        }
        public GraphTableResponseModel PopulateSurchargeCessBarChart(DashboardDetailsViewModel model)
        {
            return misReportsDal.PopulateSurchargeCessBarChart(model);
        }
        public GraphTableResponseModel LoadHighValPropChartData(DashboardDetailsViewModel model)
        {
            return misReportsDal.LoadHighValPropChartData(model);
        }

        public GraphTableResponseModel LoadRevenueTargetVsAchieved(DashboardDetailsViewModel model)
        {
            return misReportsDal.LoadRevenueTargetVsAchieved(model);
        }

        public GraphTableResponseModel PopulateProgressChart(DashboardDetailsViewModel model)
        {
            return misReportsDal.PopulateProgressChart(model);
        }
        public DashboardSummaryModel PopulateTiles(TilesReqModel reqModel)
        {
            return misReportsDal.PopulateTiles(reqModel);
        }
        public DashboardPopupViewModel LoadPopup(DashboardPopupReqModel reqModel)
        {
            return misReportsDal.LoadPopup(reqModel);
        }

        //ADDED BY SHUBHAM BHAGAT ON 1-4-2020        
        public GraphTableResponseModel LoadHighValPropChartDataForDocs(DashboardDetailsViewModel model)
        {
            return misReportsDal.LoadHighValPropChartDataForDocs(model);
        }


        #region ADDED BY SHUBHAM BHAGAT 09-04-2020
        public NatureOfArticle_REQ_RES_Model NatureOfDocumentByRadioType(NatureOfArticle_REQ_RES_Model model)
        {
            return misReportsDal.NatureOfDocumentByRadioType(model);
        }

        #endregion

        //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
        public DashboardSummaryModel PopulateAvgRegTime(TilesReqModel reqModel)
        {
            return misReportsDal.PopulateAvgRegTime(reqModel);
        }

        // BELOW CODE IS COMMENTED BY SHUBHAM BHAGAT ON  26-11-2020
        // BECAUSE WE ARE NOT PROVIDING PROGRESS CHART MONTH WISE LABLE CLICK FUNCTIONALITY NOW
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020 
        //public GraphTableResponseModel PopulateProgressChartMonthWise(DashboardDetailsViewModel model)
        //{
        //    return misReportsDal.PopulateProgressChartMonthWise(model);
        //}
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
        // ABOVE CODE IS COMMENTED BY SHUBHAM BHAGAT ON  26-11-2020

    }
}