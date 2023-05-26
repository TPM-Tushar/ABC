using CustomModels.Models.Dashboard;
using ECDataAPI.Areas.Dashboard.BAL;
using ECDataAPI.Areas.Dashboard.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.Dashboard.Controllers
{
    public class DashboardAPIController : ApiController
    {
        IDashboard balObject = null;

        [HttpGet]
		
        [Route("api/DashboardAPIController/DashboardDetailsView")]
        [EventApiAuditLogFilter(Description = "Dashboard Details View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]

        public IHttpActionResult DashboardDetailsView(int OfficeID)
        {
            try
           {
               balObject = new DashboardBAL();
               DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();

        ViewModel = balObject.DashboardDetailsView(OfficeID);

        return Ok(ViewModel);
           }
           catch (Exception)
            {
               throw;
            }
        }



        [HttpGet]
        [Route("api/DashboardAPIController/DashboardSummaryView")]
        [EventApiAuditLogFilter(Description = "Dashboard Summary View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]

        public IHttpActionResult DashboardSummaryView(int OfficeID)
        {
            try
            {
                balObject = new DashboardBAL();
                DashboardSummaryModel ViewModel = new DashboardSummaryModel();

                ViewModel = balObject.DashboardSummaryView(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("api/DashboardAPIController/LoadDashboardSumaryTable")]

        [EventApiAuditLogFilter(Description = "Load Dashboard Sumary Table", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadDashboardSumaryTable(DashboardDetailsViewModel model)
        {
            try
            {
                balObject = new DashboardBAL();
                DashboardSummaryTblResModel responseModel = new DashboardSummaryTblResModel();

                responseModel = balObject.LoadDashboardSumaryTable(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/DashboardAPIController/LoadRevenueCollectedChartData")]

        [EventApiAuditLogFilter(Description = "Load Revenue Collected Chart Data", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadRevenueCollectedChartData(DashboardDetailsViewModel model)
        {
            try
            {
                balObject = new DashboardBAL();
                GraphTableResponseModel responseModel = new GraphTableResponseModel();
                responseModel = balObject.LoadRevenueCollectedChartData(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/DashboardAPIController/LoadDocumentRegisteredChartData")]

        [EventApiAuditLogFilter(Description = "Load Document Registered Chart Data", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadDocumentRegisteredChartData(DashboardDetailsViewModel model)
        {
            try
            {
                balObject = new DashboardBAL();
                GraphTableResponseModel responseModel = new GraphTableResponseModel();
                responseModel = balObject.LoadDocumentRegisteredChartData(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/DashboardAPIController/PopulateSurchargeCessBarChart")]

        [EventApiAuditLogFilter(Description = "Load Revenue Collected Chart Data", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult PopulateSurchargeCessBarChart(DashboardDetailsViewModel model)
        {
            try
            {
                balObject = new DashboardBAL();
                GraphTableResponseModel responseModel = new GraphTableResponseModel();
                responseModel = balObject.PopulateSurchargeCessBarChart(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("api/DashboardAPIController/LoadHighValPropChartData")]

        [EventApiAuditLogFilter(Description = "Load High Value Properties Chart Data", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadHighValPropChartData(DashboardDetailsViewModel model)
        {
            try
            {
                balObject = new DashboardBAL();
                GraphTableResponseModel responseModel = new GraphTableResponseModel();
                responseModel = balObject.LoadHighValPropChartData(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/DashboardAPIController/LoadRevenueTargetVsAchieved")]

        [EventApiAuditLogFilter(Description = "Load Revenue Target Vs Achieved", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadRevenueTargetVsAchieved(DashboardDetailsViewModel model)
        {
            try
            {
                balObject = new DashboardBAL();
                GraphTableResponseModel responseModel = new GraphTableResponseModel();
                responseModel = balObject.LoadRevenueTargetVsAchieved(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/DashboardAPIController/PopulateProgressChart")]

        [EventApiAuditLogFilter(Description = "Populate Progress Chart", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult PopulateProgressChart(DashboardDetailsViewModel model)
        {
            try
            {
                balObject = new DashboardBAL();
                GraphTableResponseModel responseModel = new GraphTableResponseModel();
                responseModel = balObject.PopulateProgressChart(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/DashboardAPIController/PopulateTiles")]

        [EventApiAuditLogFilter(Description = "Populate Tiles", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult PopulateTiles(TilesReqModel reqModel)
        {
            try
            {
                balObject = new DashboardBAL();
                DashboardSummaryModel responseModel = new DashboardSummaryModel();
                responseModel = balObject.PopulateTiles(reqModel);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/DashboardAPIController/LoadPopup")]
         
        public IHttpActionResult LoadPopup(DashboardPopupReqModel model)
        {
            try
            {
                balObject = new DashboardBAL();
                DashboardPopupViewModel responseModel = new DashboardPopupViewModel();
                responseModel = balObject.LoadPopup(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //ADDED BY SHUBHAM BHAGAT ON 1-4-2020        

        [HttpPost]
        [Route("api/DashboardAPIController/LoadHighValPropChartDataForDocs")]

        [EventApiAuditLogFilter(Description = "Load HighValue Prop Chart Data For Docs Registered", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadHighValPropChartDataForDocs(DashboardDetailsViewModel model)
        {
            try
            {
                balObject = new DashboardBAL();
                GraphTableResponseModel responseModel = new GraphTableResponseModel();
                responseModel = balObject.LoadHighValPropChartDataForDocs(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region ADDED BY SHUBHAM BHAGAT 09-04-2020
        [HttpPost]
        [Route("api/DashboardAPIController/NatureOfDocumentByRadioType")]
        [EventApiAuditLogFilter(Description = "Get Nature Of Document By Radio Type", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]

        public IHttpActionResult NatureOfDocumentByRadioType(NatureOfArticle_REQ_RES_Model model)
        {
            try
            {
                balObject = new DashboardBAL();
                NatureOfArticle_REQ_RES_Model ViewModel = new NatureOfArticle_REQ_RES_Model();

                ViewModel = balObject.NatureOfDocumentByRadioType(model);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
        [HttpPost]
        [Route("api/DashboardAPIController/PopulateAvgRegTime")]

        [EventApiAuditLogFilter(Description = "Populate Average Reg Time", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult PopulateAvgRegTime(TilesReqModel reqModel)
        {
            try
            {
                balObject = new DashboardBAL();
                DashboardSummaryModel responseModel = new DashboardSummaryModel();
                responseModel = balObject.PopulateAvgRegTime(reqModel);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        // BELOW CODE IS COMMENTED BY SHUBHAM BHAGAT ON  26-11-2020
        // BECAUSE WE ARE NOT PROVIDING PROGRESS CHART MONTH WISE LABLE CLICK FUNCTIONALITY NOW
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020                  

        //[HttpPost]
        //[Route("api/DashboardAPIController/PopulateProgressChartMonthWise")]

        //[EventApiAuditLogFilter(Description = "Populate Progress Chart Month Wise", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        //public IHttpActionResult PopulateProgressChartMonthWise(DashboardDetailsViewModel model)
        //{
        //    try
        //    {
        //        balObject = new DashboardBAL();
        //        GraphTableResponseModel responseModel = new GraphTableResponseModel();
        //        responseModel = balObject.PopulateProgressChartMonthWise(model);
        //        return Ok(responseModel);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
        // ABOVE CODE IS COMMENTED BY SHUBHAM BHAGAT ON  26-11-2020

    }
}
