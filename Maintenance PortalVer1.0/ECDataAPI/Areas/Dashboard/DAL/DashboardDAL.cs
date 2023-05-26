using CustomModels.Models.Dashboard;
using ECDataAPI.Areas.Dashboard.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaigrSearchDB;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Dashboard.DAL
{
    public class DashboardDAL : IDashboard
    {
        #region Declarations
        KaigrSearchDB searchDBContext = null;
        private KaveriEntities dbContext;
        private static int LevelID;
        #endregion
        //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
        ////static string directoryPath = ConfigurationManager.AppSettings["KaveriUILogPath"];
        //static string directoryPath = ConfigurationManager.AppSettings["KaveriApiExceptionLogPath"];

        //string dashboardLogFilePath = directoryPath + "\\2020\\Aug\\DashboardTab2LogAPI.txt";
        ////string dashboardLogFilePath = directoryPath + "\\DashboardTab2Log.txt";
        //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

        #region Dashboard Tab1

        public DashboardSummaryModel DashboardSummaryView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                DashboardSummaryModel model = new DashboardSummaryModel();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID, x.DistrictID }).FirstOrDefault();
                //  var dsfg = (from mAS_OfficeMaster in dbContext.MAS_OfficeMaster join districtMaster in dbContext.DistrictMaster on mAS_OfficeMaster.DistrictID equals districtMaster.DistrictCode where mAS_OfficeMaster.OfficeID == OfficeID select new {d=districtMaster,o= mAS_OfficeMaster });
                //String district=dbContext.DistrictMaster.Where(x =>x.DistrictCode== ofcDetailsObj.Kaveri1Code).Select(x => new { x.DistrictNameE }).FirstOrDefault().ToString();

                if (ofcDetailsObj != null && ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    //string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    SelectListItem Item = new SelectListItem();
                    Item.Text = DroName;
                    Item.Value = ofcDetailsObj.Kaveri1Code.ToString();
                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Add(Item);
                }
                else
                {
                    model.DistrictList = objCommon.GetDROfficesList("State Wide View");
                }
                model.LevelId = ofcDetailsObj.LevelID;
                LevelID = ofcDetailsObj.LevelID;

                //Added by RamanK on 18-06-2020
                SelectListItem item;
                var finYearList = dbContext.USP_FINANCIAL_YEAR().ToList();
                model.FinYearList = new List<SelectListItem>();
                foreach (var finYear in finYearList)
                {
                    // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 14-08-2020 AT 10:23 AM
                    //item = new SelectListItem();
                    //item.Text = Convert.ToString(finYear.FYEAR);
                    //item.Value = Convert.ToString(finYear.YEAR);
                    //model.FinYearList.Add(item);

                    item = new SelectListItem();
                    if (finYear.YEAR != 2017)
                    {
                        item.Text = Convert.ToString(finYear.FYEAR);
                        item.Value = Convert.ToString(finYear.YEAR);
                        model.FinYearList.Add(item);
                    }
                    // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 14-08-2020 AT 10:23 AM
                }

                //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
                searchDBContext = new KaigrSearchDB();
                List<DateTime> MaxDateTimeList = new List<DateTime>();
                MaxDateTimeList = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList();
                //model.MaxDate = (from d in searchDBContext.RPT_ProcessingIterationMaster where d.IsValid == true select d.InsertDateTime).ToList().Max();
                if (MaxDateTimeList != null)
                {

                    if (MaxDateTimeList.Count > 0)
                    {
                        //if (model.Stamp5DateTime.Date == DateTime.Now.Date && model.ToDate.Date == DateTime.Now.Date)
                        //{
                        //    model.ReportInfo = "";
                        //}
                        //else
                        //{
                        model.ReportInfo = "Note: figures shown here are as per data processed upto: " + MaxDateTimeList.Max().ToString("dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);

                        //}
                        // model.MaxDate = MaxDateTimeList.Max();
                    }
                    else
                    {
                        model.ReportInfo = "";
                    }
                }
                else
                {
                    model.ReportInfo = "";
                }

                return model;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public GraphTableResponseModel LoadRevenueTargetVsAchieved(DashboardDetailsViewModel model)
        {
            GraphTableResponseModel returnModel = new GraphTableResponseModel();
            try
            {
                searchDBContext = new KaigrSearchDB();
                var returnValues = searchDBContext.USP_DB_GET_TARGET_ACHIEVED_FYWISE(model.DistrictCode, model.selectedType,model.FinYearId).ToList();
                int iRowCount = 0;

                if (returnValues != null)

                    iRowCount = returnValues.Count;
                //returnModel._RevenueTargetVsAchievedModel.Target = new int[iRownCount];
                //returnModel._RevenueTargetVsAchievedModel.Achieved = new int[iRownCount];

                if (model.toggleBtnId == 1)//For graph
                {
                    returnModel._RevenueTargetVsAchievedModel = new RevenueTargetVsAchievedModel();


                    returnModel._RevenueTargetVsAchievedModel.Target = new int[iRowCount];
                    returnModel._RevenueTargetVsAchievedModel.Achieved = new int[iRowCount];
                    returnModel._RevenueTargetVsAchievedModel.FinYears = new string[iRowCount];


                    //returnModel._RevenueTargetVsAchievedModel.FinYear = new int[8] { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017 };
                    for (int i = 0; i < iRowCount; i++)
                    {
                        returnModel._RevenueTargetVsAchievedModel.Achieved[i] = (returnValues[i].REVENUECOLLECTED == null) ? 0 : (int)returnValues[i].REVENUECOLLECTED;
                        //returnModel._RevenueTargetVsAchievedModel.Target[i] = (int)returnValues[i].REVENUETARGET;
                        returnModel._RevenueTargetVsAchievedModel.Target[i] = returnValues[i].REVENUETARGET == null ? 0 : (int)returnValues[i].REVENUETARGET;
                        returnModel._RevenueTargetVsAchievedModel.FinYears[i] = returnValues[i].FYEAR_STR;

                    }
                    returnModel._RevenueTargetVsAchievedModel.Lbl_Target = "Target";
                    returnModel._RevenueTargetVsAchievedModel.Lbl_Achieved = "Achieved";
                }

                //if (model.toggleBtnId == 2)//For Datatable
                //{
                //    returnModel._TableDataWrapper = new TableDataWrapper();
                //    List<TableData> tableDataList = new List<TableData>();
                //    List<ColumnArray> ColumnArrayList = new List<ColumnArray>();


                //    for (int i = 1; i <= 10; i++)
                //    {
                //        TableData tableData = new TableData();
                //        tableData.SrNo = i;
                //        tableData.Duration = "duration " + i;
                //        tableDataList.Add(tableData);
                //    }

                //    ColumnArrayList.Add(new ColumnArray { title = "Sr No", data = "SrNo" });
                //    ColumnArrayList.Add(new ColumnArray { title = "Duration", data = "Duration" });
                //    returnModel._TableDataWrapper.ColumnArray = ColumnArrayList.ToArray();
                //    returnModel._TableDataWrapper.TableDataArray = tableDataList.ToArray();
                //}
                if (model.toggleBtnId == 2)//For Datatable
                {
                    returnModel._TableDataWrapper = new TableDataWrapper();
                    List<TableDataRevTargetVsAchieved> tableDataList = new List<TableDataRevTargetVsAchieved>();
                    List<ColumnArray> ColumnArrayList = new List<ColumnArray>();

                    TableDataRevTargetVsAchieved tableData;

                    ColumnArrayList.Add(new ColumnArray { title = "Fin year", data = "Fin_Years" });
                    ColumnArrayList.Add(new ColumnArray { title = "Target", data = "RevTarget" });
                    ColumnArrayList.Add(new ColumnArray { title = "Achieved", data = "RevAchieved" });

                    foreach (var item in returnValues)
                    {
                        tableData = new TableDataRevTargetVsAchieved();
                        tableData.Fin_Years = (item.FYEAR_STR == null) ? string.Empty : Convert.ToString(item.FYEAR_STR);
                        tableData.RevTarget = (item.REVENUETARGET == null) ? string.Empty : Convert.ToDecimal(item.REVENUETARGET).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.RevAchieved = (item.REVENUECOLLECTED == null) ? string.Empty : Convert.ToDecimal(item.REVENUECOLLECTED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")); ;
                        tableDataList.Add(tableData);
                    }


                    returnModel._TableDataWrapper.ColumnArray = ColumnArrayList.ToArray();
                    returnModel._TableDataWrapper.tableDataRevTargetVsAchieved = tableDataList.ToArray();
                }
                return returnModel;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public GraphTableResponseModel PopulateProgressChart(DashboardDetailsViewModel model)
        {
            GraphTableResponseModel returnModel = new GraphTableResponseModel();
            try
            {
                searchDBContext = new KaigrSearchDB();
                var returnValues = searchDBContext.USP_DB_GET_FYWISE_PROGRESS(model.DistrictCode).ToList();

                int iRowCount = 0;

                if (returnValues != null)
                    iRowCount = returnValues.Count;
                if (model.toggleBtnId == 1)
                {
                    returnModel._ProgressChartModel = new ProgressChartModel();
                    returnModel._ProgressChartModel = new ProgressChartModel();
                    returnModel._ProgressChartModel.Documents = new int[iRowCount];
                    returnModel._ProgressChartModel.Revenue = new int[iRowCount];
                    returnModel._ProgressChartModel.FinYear = new string[iRowCount];

                    for (int i = 0; i < iRowCount; i++)
                    {
                        returnModel._ProgressChartModel.Documents[i] = (returnValues[i].NO_OF_DOCS_REGISTERED == null) ? 0 : (int)returnValues[i].NO_OF_DOCS_REGISTERED;
                        returnModel._ProgressChartModel.Revenue[i] = (returnValues[i].TOTAL_REVENUE == null) ? 0 : (int)returnValues[i].TOTAL_REVENUE;
                        returnModel._ProgressChartModel.FinYear[i] = (returnValues[i].FYEAR_STR == null) ? string.Empty : returnValues[i].FYEAR_STR;
                    }
                    returnModel._ProgressChartModel.Lbl_Documents = "Documents";
                    returnModel._ProgressChartModel.Lbl_Revenue = "Revenue";
                }

                if (model.toggleBtnId == 2)
                {
                    returnModel._TableDataWrapper = new TableDataWrapper();
                    List<TableDataProgressChart> tableDataList = new List<TableDataProgressChart>();
                    List<ColumnArray> ColumnArrayList = new List<ColumnArray>();

                    TableDataProgressChart tableData;

                    ColumnArrayList.Add(new ColumnArray { title = "Fin year", data = "REGFYEAR" });
                    ColumnArrayList.Add(new ColumnArray { title = "Documents", data = "NO_OF_DOCS_REGISTERED" });
                    ColumnArrayList.Add(new ColumnArray { title = "Revenue", data = "TOTAL_REVENUE" });

                    foreach (var item in returnValues)
                    {
                        tableData = new TableDataProgressChart();

                        tableData.NO_OF_DOCS_REGISTERED = (item.NO_OF_DOCS_REGISTERED == null) ? string.Empty : Convert.ToString(item.NO_OF_DOCS_REGISTERED);
                        tableData.REGFYEAR = (item.REGFYEAR == null) ? 0 : Convert.ToInt32(item.REGFYEAR);
                        tableData.TOTAL_REVENUE = (item.TOTAL_REVENUE == null) ? string.Empty : Convert.ToDecimal(item.TOTAL_REVENUE).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")); ;

                        tableDataList.Add(tableData);
                    }


                    returnModel._TableDataWrapper.ColumnArray = ColumnArrayList.ToArray();
                    returnModel._TableDataWrapper.TableDataArrayofProgressChart = tableDataList.ToArray();
                }

                return returnModel;
            }
            catch (Exception)
            {
                throw;
            }

        }

        //To Populate Tiles Total Revenue Collected etc.
        public DashboardSummaryModel PopulateTiles(TilesReqModel reqModel)
        {
            DashboardSummaryModel dashboardSummaryModel = new DashboardSummaryModel();
            try
            {
                searchDBContext = new KaigrSearchDB();
                List<RevenueCollectionModel> RevenueModelList = new List<RevenueCollectionModel>();
                RevenueCollectionWrapperModel revenueCollectedWrapper = new RevenueCollectionWrapperModel();
                dashboardSummaryModel.Tiles = new List<DashboardTileModel>();


                //Add parameter
                var TilesData = searchDBContext.USP_DB_GET_CURRENT_STATUS(reqModel.selectedType, reqModel.OfficeCode,reqModel.FinYearId).ToList();

                DashboardTileModel dashboardTileModel1 = new DashboardTileModel();
                DashboardTileModel dashboardTileModel2 = new DashboardTileModel();
                DashboardTileModel dashboardTileModel3 = new DashboardTileModel();
                DashboardTileModel dashboardTileModel4 = new DashboardTileModel();
                DashboardTileModel dashboardTileModel5 = new DashboardTileModel();
                DashboardTileModel dashboardTileModel6 = new DashboardTileModel();

                if (TilesData != null && TilesData.Count() != 0)
                {
                    string sDivisionString = "";
                    if (reqModel.selectedType == "M")
                        sDivisionString = "Day";
                    if (reqModel.selectedType == "D")
                        sDivisionString = "Hour";
                    if (reqModel.selectedType == "F")
                        sDivisionString = "Month";
                    foreach (var item in TilesData)
                    {
                        dashboardTileModel1.Amount = (item.TOTAL_REV_COLLECTED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        dashboardTileModel1.Title = "Total Revenue Collected";

                        if (item.TOTAL_REV_COLLECTED_WRT_LY == null)
                        {
                            dashboardTileModel1.DescPercentage = "";
                            dashboardTileModel1.Description = "-";
                        }
                        else
                        {
                            if (item.TOTAL_REV_COLLECTED_WRT_LY <= 0)
                            {
                                dashboardTileModel1.DescPercentage = Convert.ToDecimal(item.TOTAL_REV_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                                dashboardTileModel1.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                            }
                            else
                            {
                                dashboardTileModel1.DescPercentage = "+ " + Convert.ToDecimal(item.TOTAL_REV_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                                dashboardTileModel1.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");

                            }

                        }

                        dashboardTileModel2.Amount = (item.TOTAL_SD_COLLECTED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        dashboardTileModel2.Title = "Total Stamp Duty Collected";
                        if (item.TOTAL_SD_COLLECTED_WRT_LY == null)
                        {
                            dashboardTileModel2.DescPercentage = "";
                            dashboardTileModel2.Description = "-";
                        }
                        else
                        {

                            if (item.TOTAL_SD_COLLECTED_WRT_LY <= 0)
                            {
                                dashboardTileModel2.DescPercentage = Convert.ToDecimal(item.TOTAL_SD_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                                dashboardTileModel2.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                            }
                            else
                            {
                                dashboardTileModel2.DescPercentage = "+ " + Convert.ToDecimal(item.TOTAL_SD_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                                dashboardTileModel2.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");

                            }
                        }

                        dashboardTileModel3.Amount = item.TOTAL_RF_COLLECTED.ToString();
                        dashboardTileModel3.Title = "Total Registration fees Collected";

                        if (item.TOTAL_RF_COLLECTED_WRT_LY == null)
                        {
                            dashboardTileModel3.DescPercentage = "";
                            dashboardTileModel3.Description = "-";
                        }
                        else
                        {
                            if (item.TOTAL_RF_COLLECTED_WRT_LY <= 0)
                            {
                                dashboardTileModel3.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                                dashboardTileModel3.DescPercentage = Convert.ToDecimal(item.TOTAL_RF_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                            }
                            else
                            {
                                dashboardTileModel3.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                                dashboardTileModel3.DescPercentage = "+ " + Convert.ToDecimal(item.TOTAL_RF_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                            }
                        }

                        dashboardTileModel4.Amount = String.Format("{0:n0}", item.TOTAL_DOCS_REGISTERED);
                        dashboardTileModel4.Title = "Total Documents Registered";
                        if (item.TOTAL_DOCS_REGISTERED_WRT_LY == null)
                        {
                            dashboardTileModel4.DescPercentage = "";
                            dashboardTileModel4.Description = "-";
                        }
                        else
                        {
                            if (item.TOTAL_DOCS_REGISTERED_WRT_LY <= 0)
                            {
                                dashboardTileModel4.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                                dashboardTileModel4.DescPercentage = Convert.ToDecimal(item.TOTAL_DOCS_REGISTERED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                            }
                            else
                            {
                                dashboardTileModel4.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                                dashboardTileModel4.DescPercentage = "+ " + Convert.ToDecimal(item.TOTAL_DOCS_REGISTERED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                            }
                        }


                        //dashboardTileModel5.Amount = Convert.ToInt32((item.AVG_DOCS_REGISTERED_PER_MONTH)).ToString();
                        dashboardTileModel5.Amount = item.AVG_DOCS_REGISTERED_PER_MONTH == null ? "" : String.Format("{0:n0}", Convert.ToInt32(item.AVG_DOCS_REGISTERED_PER_MONTH.Value));
                        dashboardTileModel5.Title = "Avg Doc Registered / " + sDivisionString;
                        if (item.AVG_DOCS_REGISTERED_PER_MONTH_WRT_LY == null)
                        {
                            dashboardTileModel5.DescPercentage = "";
                            dashboardTileModel5.Description = "-";
                        }
                        else
                        {

                            if (item.AVG_DOCS_REGISTERED_PER_MONTH_WRT_LY <= 0)
                            {
                                dashboardTileModel5.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                                dashboardTileModel5.DescPercentage = Convert.ToDecimal(item.AVG_DOCS_REGISTERED_PER_MONTH_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                            }
                            else
                            {
                                dashboardTileModel5.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                                dashboardTileModel5.DescPercentage = "+ " + Convert.ToDecimal(item.AVG_DOCS_REGISTERED_PER_MONTH_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                            }
                        }

                        dashboardTileModel6.Amount = Convert.ToInt32((item.AVG_REVENUE_COLLECTED_PER_MONTH)).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        dashboardTileModel6.Title = "Average Revenue Collected / " + sDivisionString;
                        if (item.AVG_REVENUE_COLLECTED_PER_MONTH_WRT_LY == null)
                        {
                            dashboardTileModel6.DescPercentage = "";
                            dashboardTileModel6.Description = "-";
                        }
                        else
                        {
                            if (item.AVG_REVENUE_COLLECTED_PER_MONTH_WRT_LY <= 0)
                            {
                                dashboardTileModel6.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                                dashboardTileModel6.DescPercentage = Convert.ToDecimal(item.AVG_REVENUE_COLLECTED_PER_MONTH_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                            }
                            else
                            {
                                dashboardTileModel6.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                                dashboardTileModel6.DescPercentage = "+ " + Convert.ToDecimal(item.AVG_REVENUE_COLLECTED_PER_MONTH_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                            }
                        }

                        dashboardSummaryModel.Tiles.Add(dashboardTileModel1);
                        dashboardSummaryModel.Tiles.Add(dashboardTileModel2);
                        dashboardSummaryModel.Tiles.Add(dashboardTileModel3);
                        dashboardSummaryModel.Tiles.Add(dashboardTileModel4);
                        dashboardSummaryModel.Tiles.Add(dashboardTileModel5);
                        dashboardSummaryModel.Tiles.Add(dashboardTileModel6);
                    }
                    //  }
                }
                else
                {
                    dashboardTileModel1.Amount = "0.00";
                    dashboardTileModel1.Title = "Data Not Available";
                    dashboardTileModel1.Description = "Data Not Available";
                    dashboardTileModel1.DescPercentage = "";

                    dashboardTileModel2.Amount = "0.00";
                    dashboardTileModel2.Title = "Data Not Available";
                    dashboardTileModel2.DescPercentage = "";
                    dashboardTileModel2.Description = "Data Not Available";

                    dashboardTileModel3.Amount = "0.00";
                    dashboardTileModel3.Title = "Data Not Available";
                    dashboardTileModel3.DescPercentage = "";
                    dashboardTileModel3.Description = "Data Not Available";

                    dashboardTileModel4.Amount = "0.00";
                    dashboardTileModel4.Title = "Data Not Available";
                    dashboardTileModel4.DescPercentage = "";
                    dashboardTileModel4.Description = "Data Not Available";

                    dashboardTileModel5.Amount = "0.00";
                    dashboardTileModel5.Title = "Data Not Available";
                    dashboardTileModel5.DescPercentage = "";
                    dashboardTileModel5.Description = "Data Not Available";

                    dashboardTileModel6.Amount = "0.00";
                    dashboardTileModel6.Title = "Data Not Available";
                    dashboardTileModel6.DescPercentage = "";
                    dashboardTileModel6.Description = "Data Not Available";
                    dashboardSummaryModel.Tiles.Add(dashboardTileModel1);
                    dashboardSummaryModel.Tiles.Add(dashboardTileModel2);
                    dashboardSummaryModel.Tiles.Add(dashboardTileModel3);
                    dashboardSummaryModel.Tiles.Add(dashboardTileModel4);
                    dashboardSummaryModel.Tiles.Add(dashboardTileModel5);
                    dashboardSummaryModel.Tiles.Add(dashboardTileModel6);
                }
                dashboardSummaryModel.LevelId = LevelID;
                dashboardSummaryModel._RevenueCollectionWrapperModel = PopulateRevenueCollected(reqModel);
                dashboardSummaryModel.CurrentAchievementsModel = PopulateCurrentAchievements(reqModel);
                dashboardSummaryModel._ProgressBarTargetVsAchieved = PopulateProgressBarTargetVsAchieved(reqModel);
                //Add Parameter
                var startTimeIndications = searchDBContext.USP_DB_GET_OFFICESTARTTIME(reqModel.OfficeCode, reqModel.selectedType,reqModel.FinYearId).ToList();
                string TempOfficeName = string.Empty;
                string sContent = string.Empty;
                dashboardSummaryModel.StartTimeIndicationTop = string.Empty;
                dashboardSummaryModel.StartTimeIndicationBottom = string.Empty;
                foreach (var item in startTimeIndications)
                {
                    if (item.OFFICE_NAME.Contains("Banglore Development Authority"))
                    {
                        TempOfficeName = "BDA";
                    }
                    else if (item.OFFICE_NAME.Contains("Mysore Development Authority"))
                    {
                        TempOfficeName = "MDA";
                    }
                    else
                    {
                        TempOfficeName = item.OFFICE_NAME;
                    }

                    if (item.HIERARCHY == "T")
                    {
                        dashboardSummaryModel.StartTimeIndicationTop +=
                         @" <div class='row'><div class='col-md-6 col-sm-6 col-xs-12'><label for='inputEmail3' class='text-success control-label'>" + TempOfficeName + "</label></div><div class='col-md-6 col-sm-6 col-xs-12'><label class='text-success'>" + item.AVG_START_TIME + "</label></div></div>";

                    }
                    else
                    {
                        dashboardSummaryModel.StartTimeIndicationBottom +=
                         @" <div class='row'><div class='col-md-6 col-sm-6 col-xs-12'><label for='inputEmail3' class='text-danger control-label'>" + TempOfficeName + "</label></div><div class='col-md-6 col-sm-6 col-xs-12'><label class='text-danger'>" + item.AVG_START_TIME + "</label></div></div>";
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return dashboardSummaryModel;

        }

        //To display Top3 and Bottom3 Districts in Revenue collection RamanK
        public RevenueCollectionWrapperModel PopulateRevenueCollected(TilesReqModel reqModel)
        {
            try
            {
                searchDBContext = new KaigrSearchDB();
                RevenueCollectionWrapperModel RevenueCollectedWrapper = new RevenueCollectionWrapperModel();
                List<RevenueCollectionModel> RevenueModelList = new List<RevenueCollectionModel>();
                List<RevenueCollectionModel> RevenueModelUpperList = new List<RevenueCollectionModel>();
                List<RevenueCollectionModel> RevenueModelLowerList = new List<RevenueCollectionModel>();

                //Add Parameter
                //COMMENTED AND ADDED BY PANKAJ ON 06-10-2020
                decimal revColected = 0;
                //var ListRevenueModel = searchDBContext.USP_DB_GET_CURRENT_REVENUE(reqModel.OfficeCode, reqModel.selectedType, reqModel.FinYearId).ToList();
                var ListRevenueModel = searchDBContext.USP_DB_GET_CURRENT_REVENUE(reqModel.OfficeCode, reqModel.selectedType, reqModel.FinYearId).ToList();
                var _RevenueModelUpperList = ListRevenueModel.Where(x => x.HIERARCHY == "T").ToList();
                var _RevenueModelLowerList = ListRevenueModel.Where(x => x.HIERARCHY == "B").ToList();
                string TempOfficeName = string.Empty;
                RevenueCollectionModel RevenueModel;
                RevenueCollectedWrapper.LowerRevenueList = "";
                RevenueCollectedWrapper.UpperRevenueList = "";
                foreach (var item in _RevenueModelUpperList)
                {
                    RevenueModel = new RevenueCollectionModel();
                    RevenueModel.OfficeName = item.OFFICE_NAME == null ? string.Empty : item.OFFICE_NAME;
                    if (item.PERCENT_ACHIEVED == null)
                    {
                        RevenueModel.Percentage = string.Empty;
                    }
                    else
                    {
                        RevenueModel.Percentage = Convert.ToDecimal(item.PERCENT_ACHIEVED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "%";
                    }
                    if (item.REVENUECOLLECTED == null)
                    {
                        RevenueModel.ActualRevenueCollected = string.Empty;
                    }
                    else
                    {
                        //COMMENTED AND ADDED BY PANKAJ ON 06-10-2020
                        //RevenueModel.ActualRevenueCollected = Convert.ToDecimal(item.REVENUECOLLECTED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        revColected = Convert.ToDecimal(item.REVENUECOLLECTED);
                        revColected = revColected / 10000000;
                        //RevenueModel.ActualRevenueCollected = revColected.ToString("{0:0.00}");
                        RevenueModel.ActualRevenueCollected = string.Format("{0:n2}", revColected);

                    }

                    if (item.PERCENT_ACHIEVED == null)
                    {
                        RevenueModel.BarPercentage = string.Empty;
                    }
                    else
                    {
                        RevenueModel.BarPercentage = (item.PERCENT_ACHIEVED > 100) ? "100%" : Convert.ToDecimal(item.PERCENT_ACHIEVED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "%";
                    }
                    if (item.OFFICE_NAME.Contains("Banglore Development Authority"))
                    {
                        TempOfficeName = "BDA";
                    }
                    else if (item.OFFICE_NAME.Contains("Mysore Development Authority"))
                    {
                        TempOfficeName = "MDA";
                    }
                    else
                    {
                        TempOfficeName = item.OFFICE_NAME;
                    }

                    RevenueCollectedWrapper.UpperRevenueList +=

                    "<div class='row'><div class='col-md-4 col-sm-6 col-xs-12'><label id = 'lblRevenue_BOffice_0' for='inputEmail3' class='control-label'>" + TempOfficeName + "</label></div><div class='col-md-8 col-sm-6 col-xs-12'><div class='progress-custom tooltip'><span id = 'Revenue_B_ToolTip_0' class='tooltiptext'>" + RevenueModel.ActualRevenueCollected + "</span><div class='progress progress-xs'><div id = 'Revenue_B_0' class='progress-bar progress-bar-success progress-bar-striped' role='progressbar' aria-valuenow='" + RevenueModel.Percentage + "' aria-valuemin='0' aria-valuemax='100' style='width: " + RevenueModel.BarPercentage + "'></div></div><div class='progress-value'><span id = 'Revenue_B_Percentage_0' >" + RevenueModel.Percentage + "</span></div></div></div></div>";

                }


                foreach (var item in _RevenueModelLowerList)
                {
                    RevenueModel = new RevenueCollectionModel();
                    RevenueModel.OfficeName = item.OFFICE_NAME == null ? string.Empty : item.OFFICE_NAME;
                    if (item.PERCENT_ACHIEVED == null)
                    {
                        RevenueModel.Percentage = string.Empty;
                    }
                    else
                    {
                        RevenueModel.Percentage = Convert.ToDecimal(item.PERCENT_ACHIEVED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "%";
                    }
                    if (item.REVENUECOLLECTED == null)
                    {
                        RevenueModel.ActualRevenueCollected = string.Empty;
                    }
                    else
                    {
                        //COMMENTED AND ADDED BY PANKJA ON 06-10-2020
                        //RevenueModel.ActualRevenueCollected = Convert.ToDecimal(item.REVENUECOLLECTED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        revColected = Convert.ToDecimal(item.REVENUECOLLECTED);
                        revColected = revColected / 10000000;
                        // RevenueModel.ActualRevenueCollected = revColected.ToString("{0:0.00}");
                        RevenueModel.ActualRevenueCollected = string.Format("{0:n2}", revColected);
                    }

                    if (item.PERCENT_ACHIEVED == null)
                    {
                        RevenueModel.BarPercentage = string.Empty;
                    }
                    else
                    {
                        RevenueModel.BarPercentage = (item.PERCENT_ACHIEVED > 100) ? "100%" : Convert.ToDecimal(item.PERCENT_ACHIEVED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "%";
                    }


                    if (item.OFFICE_NAME.Contains("Banglore Development Authority"))
                    {
                        TempOfficeName = "BDA";
                    }
                    else if (item.OFFICE_NAME.Contains("Mysore Development Authority"))
                    {
                        TempOfficeName = "MDA";
                    }
                    else
                    {
                        TempOfficeName = item.OFFICE_NAME;
                    }


                    RevenueCollectedWrapper.LowerRevenueList +=

                  "<div class='row'><div class='col-md-4 col-sm-6 col-xs-12'><label id = 'lblRevenue_BOffice_0' for='inputEmail3' class='control-label'>" + TempOfficeName + "</label></div><div class='col-md-8 col-sm-6 col-xs-12'><div class='progress-custom tooltip'><span id = 'Revenue_B_ToolTip_0' class='tooltiptext'>" + RevenueModel.ActualRevenueCollected + "</span><div class='progress progress-xs'><div id = 'Revenue_B_0' class='progress-bar progress-bar-danger progress-bar-striped' role='progressbar' aria-valuenow='" + RevenueModel.Percentage + "' aria-valuemin='0' aria-valuemax='100' style='width: " + RevenueModel.BarPercentage + "'></div></div><div class='progress-value'><span id = 'Revenue_B_Percentage_0' >" + RevenueModel.Percentage + "</span></div></div></div></div>";

                }
                return RevenueCollectedWrapper;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //To Display Registration Highlights
        public CurrentAchievementsModel PopulateCurrentAchievements(TilesReqModel reqModel)
        {
            try
            {
                searchDBContext = new KaigrSearchDB();
                CurrentAchievementsModel currentAchievementsModel = new CurrentAchievementsModel();
                currentAchievementsModel.CurrentAchievementsList = new List<string>();
                //Add Parameter
                var CurrentAchievements = searchDBContext.USP_DB_Get_HIGHLIGHTS(reqModel.OfficeCode, reqModel.selectedType,reqModel.FinYearId).Where(x => x.IS_ACTIVE == true).ToList();
                String Achievement = string.Empty;
                foreach (var item in CurrentAchievements)
                {
                    Achievement = item.INFORMATION;
                    currentAchievementsModel.CurrentAchievementsList.Add(Achievement);
                }

                return currentAchievementsModel;
            }
            catch (Exception)
            {
                throw;
            }

        }

        //To Display Progress Bar, Achieved Vs Forecast
        public ProgressBarTargetVsAchieved PopulateProgressBarTargetVsAchieved(TilesReqModel reqModel)
        {
            try
            {
                searchDBContext = new KaigrSearchDB();
                Decimal TargetPer;
                Decimal AchievedPer;
                Decimal ForeCastPercentage;

                ProgressBarTargetVsAchieved progressBarTargetVsAchieved = new ProgressBarTargetVsAchieved();
                //Add Parameter
                var ProgressBarRes = searchDBContext.USP_DB_GET_TARGET_LINE(reqModel.OfficeCode,reqModel.FinYearId).FirstOrDefault();
                if (ProgressBarRes == null)
                    return progressBarTargetVsAchieved;

                if (ProgressBarRes == null)
                {
                    progressBarTargetVsAchieved.AchievedPercentage = "0%";
                    progressBarTargetVsAchieved.TargetPercentage = "0%";
                }
                else if (ProgressBarRes.FYEAR_TARGET == null || ProgressBarRes.FYEAR_ACHIEVEMENT == null || ProgressBarRes.FYEAR_FORCAST == null)
                {
                    progressBarTargetVsAchieved.AchievedPercentage = "0%";
                    progressBarTargetVsAchieved.TargetPercentage = "0%";
                    progressBarTargetVsAchieved.ForeCastPercentage = "0%";

                }
                else
                {
                    //
                    progressBarTargetVsAchieved.AchievedPercentage = Convert.ToDecimal(((ProgressBarRes.FYEAR_ACHIEVEMENT / ProgressBarRes.FYEAR_TARGET) * 100)).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "%";
                    progressBarTargetVsAchieved.TargetPercentage = 100 + "%";
                    progressBarTargetVsAchieved.ForeCastPercentage = Convert.ToDecimal(((ProgressBarRes.FYEAR_FORCAST / ProgressBarRes.FYEAR_TARGET) * 100)).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "%";
                    TargetPer = 100;
                    AchievedPer = Convert.ToDecimal((ProgressBarRes.FYEAR_ACHIEVEMENT / ProgressBarRes.FYEAR_TARGET) * 100);
                    ForeCastPercentage = Convert.ToDecimal(((ProgressBarRes.FYEAR_FORCAST / ProgressBarRes.FYEAR_TARGET) * 100));
                    if (ForeCastPercentage > TargetPer)
                    {
                        progressBarTargetVsAchieved.TargetPercentage_Bar = Convert.ToDecimal((TargetPer * 100) / ForeCastPercentage).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "%";
                        progressBarTargetVsAchieved.AchievedPercentage_Bar = Convert.ToDecimal((AchievedPer * 100) / ForeCastPercentage).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "%";

                        progressBarTargetVsAchieved.ForeCastPercentage_Bar = "100%";
                    }
                    else
                    {
                        progressBarTargetVsAchieved.TargetPercentage_Bar = progressBarTargetVsAchieved.TargetPercentage;
                        progressBarTargetVsAchieved.AchievedPercentage_Bar = progressBarTargetVsAchieved.AchievedPercentage;
                        progressBarTargetVsAchieved.ForeCastPercentage_Bar = progressBarTargetVsAchieved.ForeCastPercentage;
                    }
                }

                progressBarTargetVsAchieved.AchievedValue = (ProgressBarRes.FYEAR_ACHIEVEMENT == null) ? string.Empty : "Achieved : " + Convert.ToDecimal(ProgressBarRes.FYEAR_ACHIEVEMENT).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                progressBarTargetVsAchieved.TargetValue = (ProgressBarRes.FYEAR_TARGET == null) ? string.Empty : "Target : " + Convert.ToDecimal(ProgressBarRes.FYEAR_TARGET).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                progressBarTargetVsAchieved.ForeCastValue = (ProgressBarRes.FYEAR_FORCAST == null) ? string.Empty : "Forecast : " + Convert.ToDecimal(ProgressBarRes.FYEAR_FORCAST).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                //progressBarTargetVsAchieved.AchievedPercentage = "90%";
                //progressBarTargetVsAchieved.TargetPercentage = "90";
                //progressBarTargetVsAchieved.TargetPercentage_Bar ="80";
                //progressBarTargetVsAchieved.AchievedPercentage_Bar = "80%";

                return progressBarTargetVsAchieved;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public DashboardPopupViewModel LoadPopup(DashboardPopupReqModel reqModel)
        {
            DashboardPopupViewModel responseModel = new DashboardPopupViewModel();
            responseModel.PopupContentList = new List<DashboardPopupContent>();
            responseModel.RevenueModelList = new List<RevenueCollectionModel>();
            responseModel.AverageRegTimeDetailsModelList = new List<AverageRegTimeDetailsModel>();

            DashboardPopupContent contentObj = null;
            searchDBContext = new KaigrSearchDB();
            responseModel.PopupType = reqModel.PopupType;

            if (reqModel.PopupType == "T")
            {
                //Add Parameter
                var startTimeIndications = searchDBContext.USP_DB_GET_OFFICESTARTTIME_DETAILS(reqModel.SelectedOffice, reqModel.selectedType,reqModel.FinYearId).ToList();
                string sContent = string.Empty;

                responseModel.heading = "Avg Start Time Indications";
                foreach (var item in startTimeIndications)
                {
                    contentObj = new DashboardPopupContent();
                    contentObj.AVG_START_TIME = item.AVG_START_TIME;

                    contentObj.OFFICE_NAME = item.OFFICE_NAME;
                    responseModel.PopupContentList.Add(contentObj);
                }
            }

            if (reqModel.PopupType == "R")
            {
                responseModel.heading = "Revenue Details";
                //Add Parameter
                var ListRevenueModel = searchDBContext.USP_DB_GET_CURRENT_REVENUE_DETAILS(reqModel.SelectedOffice, reqModel.selectedType,reqModel.FinYearId).ToList();
                responseModel.RevenueModelList = new List<RevenueCollectionModel>();
                RevenueCollectionModel RevenueModel;
                foreach (var item in ListRevenueModel)
                {
                    RevenueModel = new RevenueCollectionModel();
                    RevenueModel.OfficeName = item.OFFICE_NAME == null ? string.Empty : item.OFFICE_NAME;
                    if (item.PERCENT_ACHIEVED == null)
                    {
                        RevenueModel.Percentage = string.Empty;
                    }
                    else
                    {
                        RevenueModel.Percentage = Convert.ToDecimal(item.PERCENT_ACHIEVED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "%";
                    }
                    if (item.REVENUECOLLECTED == null)
                    {
                        RevenueModel.ActualRevenueCollected = string.Empty;
                    }
                    else
                    {
                        RevenueModel.ActualRevenueCollected = Convert.ToDecimal(item.REVENUECOLLECTED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    }
                    responseModel.RevenueModelList.Add(RevenueModel);
                }

            }
            //ADDED BY PANKAJ SAKHARE ON 21-09-2020
            if(reqModel.PopupType == "ART")
            {
                responseModel.heading = "Average Registration Time Details";
                //Add Parameter
                var ListAvgRegTime = searchDBContext.USP_DB_GET_AVG_REGISTRASTION_TIME_FYWISE_DETAILS(reqModel.SelectedOffice, reqModel.selectedType, reqModel.FinYearId).ToList();
                responseModel.RevenueModelList = new List<RevenueCollectionModel>();
                AverageRegTimeDetailsModel averageRegTimeDetailsModel;
                
                foreach (var item in ListAvgRegTime)
                {
                    averageRegTimeDetailsModel = new AverageRegTimeDetailsModel();
                    averageRegTimeDetailsModel.DistrictName  = item.DistrictName == null ? string.Empty : item.DistrictName;
                    averageRegTimeDetailsModel.DocsRegistered = item.Docs_Registered == null ? string.Empty :((int) item.Docs_Registered).ToString();
                    averageRegTimeDetailsModel.ART = item.ART.ToString();
                    averageRegTimeDetailsModel.PercentART = item.PERCENT_ART == null ? string.Empty : ((decimal)item.PERCENT_ART).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " %";
                    responseModel.AverageRegTimeDetailsModelList.Add(averageRegTimeDetailsModel);
                }
            }

            return responseModel;
        }
        #endregion

        #region Dashboard Tab2
        public DashboardDetailsViewModel DashboardDetailsView(int OfficeID)
        {
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-DashboardDetailsView-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                dbContext = new KaveriEntities();
                DashboardDetailsViewModel model = new DashboardDetailsViewModel();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();
                model.SROfficeList = new List<SelectListItem>();
                model.DROfficeList = new List<SelectListItem>();
                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    model.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    model.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });
                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    model.DROfficeList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020
                    //model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "Select");
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "All");
                    // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020

                }
                else
                {
                    model.SROfficeList.Add(new SelectListItem() { Text = "All", Value = "0" });
                    model.DROfficeList = objCommon.GetDROfficesList("All");
                }

                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-DashboardDetailsView-Before objCommon.GetRegistrationArticlesTop10Wise(false)");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                model.NatuereOfDocsList = objCommon.GetRegistrationArticlesTop10Wise(false);
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-DashboardDetailsView-After objCommon.GetRegistrationArticlesTop10Wise(false)");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                LevelID = ofcDetailsObj.LevelID;

                // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 14-08-2020 AT 11:33 AM
                //int[] NatureOfDocsIDArray = { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 130, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 42, 43, 44, 45, 46, 47, 1, 107, 48, 49, 50, 51, 52, 53, 128, 129, 54, 55, 57, 58, 59, 60, 61, 62, 63, 64, 65, 56, 66, 67, 68, 69, 70, 71, 72, 73, 74, 131, 132, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 3, 97, 98, 99, 100, 4, 101, 102, 104, 105, 106, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 27, 119, 120, 121, 122, 123, 124, 125, 127, 126 };
                int[] NatureOfDocsIDArray = dbContext.RegistrationArticles.Select(x => x.RegArticleCode).ToArray();
                // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 14-08-2020 AT 11:33 AM
                model.NatureOfDocID = NatureOfDocsIDArray;

                //Added by RamanK on 18-06-2020
                SelectListItem item;
                var finYearList = dbContext.USP_FINANCIAL_YEAR().ToList();
                model.FinYearList = new List<SelectListItem>();
                foreach (var finYear in finYearList)
                {
                    // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 14-08-2020 AT 10:23 AM
                    //item = new SelectListItem();
                    //item.Text = Convert.ToString(finYear.FYEAR);
                    //item.Value = Convert.ToString(finYear.YEAR);
                    //model.FinYearList.Add(item);

                    item = new SelectListItem();
                    if (finYear.YEAR != 2017)
                    {
                        item.Text = Convert.ToString(finYear.FYEAR);
                        item.Value = Convert.ToString(finYear.YEAR);
                        model.FinYearList.Add(item);
                    }
                    // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 14-08-2020 AT 10:23 AM

                }
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-DashboardDetailsView-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                return model;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }
        public DashboardSummaryTblResModel LoadDashboardSumaryTable(DashboardDetailsViewModel model)
        {
            DashboardSummaryTblResModel DashboardSummarytblResModel = new DashboardSummaryTblResModel();
            //DayWiseModel dayWiseModel;
            //MonthWiseModel monthWiseModel;
            //FinYearWiseModel finYearWiseModel;
            //UptoDateWiseModel uptoDateWiseModel;
            //DashboardSummarytblResModel.DayWiseModelList = new List<DayWiseModel>();
            //DashboardSummarytblResModel.MonthWiseModel = new List<MonthWiseModel>();
            //DashboardSummarytblResModel.FinYearWiseModel = new List<FinYearWiseModel>();
            //DashboardSummarytblResModel.UptoDateWiseModel = new List<UptoDateWiseModel>();
            searchDBContext = new KaigrSearchDB();
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDashboardSumaryTable-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                //List<USP_DB_GET_SUMMARY_TABLE_DAYWISE_Result> DayWiseResult = searchDBContext.USP_DB_GET_SUMMARY_TABLE_DAYWISE(model.DROfficeID, model.SROfficeID, model.SNatureOfDocID).ToList();
                //List<USP_DB_GET_SUMMARY_TABLE_FYEARWISE_Result> FinYearWiseResult = searchDBContext.USP_DB_GET_SUMMARY_TABLE_FYEARWISE(model.DROfficeID, model.SROfficeID, model.SNatureOfDocID).ToList();
                //List<USP_DB_GET_SUMMARY_TABLE_MONTHWISE_Result> MonthWiseResult = searchDBContext.USP_DB_GET_SUMMARY_TABLE_MONTHWISE(model.DROfficeID, model.SROfficeID, model.SNatureOfDocID).ToList();
                //List<USP_DB_GET_SUMMARY_TABLE_UPTODATE_Result> UptoDateWiseResult = searchDBContext.USP_DB_GET_SUMMARY_TABLE_UPTODATE(model.DROfficeID, model.SROfficeID, model.SNatureOfDocID).ToList();


                List<DashboardSummaryRecData> DashboardSummaryRecList = new List<DashboardSummaryRecData>();
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDashboardSumaryTable-Before searchDBContext.USP_DB_GET_SUMMARY_TABLE_DAYWISE");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                USP_DB_GET_SUMMARY_TABLE_DAYWISE_Result DayWiseResult = searchDBContext.USP_DB_GET_SUMMARY_TABLE_DAYWISE(model.DROfficeID, model.SROfficeID, model.SNatureOfDocID).FirstOrDefault();
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDashboardSumaryTable-After searchDBContext.USP_DB_GET_SUMMARY_TABLE_DAYWISE");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDashboardSumaryTable-Before searchDBContext.USP_DB_GET_SUMMARY_TABLE_FYEARWISE");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                USP_DB_GET_SUMMARY_TABLE_FYEARWISE_Result FinYearWiseResult = searchDBContext.USP_DB_GET_SUMMARY_TABLE_FYEARWISE(model.DROfficeID, model.SROfficeID, model.SNatureOfDocID, model.FinYearId).FirstOrDefault();
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDashboardSumaryTable-After searchDBContext.USP_DB_GET_SUMMARY_TABLE_FYEARWISE");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDashboardSumaryTable-Before searchDBContext.USP_DB_GET_SUMMARY_TABLE_MONTHWISE");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                USP_DB_GET_SUMMARY_TABLE_MONTHWISE_Result MonthWiseResult = searchDBContext.USP_DB_GET_SUMMARY_TABLE_MONTHWISE(model.DROfficeID, model.SROfficeID, model.SNatureOfDocID).FirstOrDefault();
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDashboardSumaryTable-After searchDBContext.USP_DB_GET_SUMMARY_TABLE_MONTHWISE");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDashboardSumaryTable-Before searchDBContext.USP_DB_GET_SUMMARY_TABLE_UPTODATE");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                USP_DB_GET_SUMMARY_TABLE_UPTODATE_Result UptoDateWiseResult = searchDBContext.USP_DB_GET_SUMMARY_TABLE_UPTODATE(model.DROfficeID, model.SROfficeID, model.SNatureOfDocID).FirstOrDefault();
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDashboardSumaryTable-After searchDBContext.USP_DB_GET_SUMMARY_TABLE_UPTODATE");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020


                //ADDED BY PANKAJ SAKHARE ON 18-09-2020 FOR EXCEL CHECK
                if (!model.IsForExcel)
                {
                    DashboardSummaryRecData DashboardSummaryRecListItem;
                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_PRESENTED_TODAY) + Get_LT_GTSymbol(DayWiseResult.DOCS_PRESENTED_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_PRESENTED_YESTERDAY);
                    DashboardSummaryRecListItem.Description = "No. of Documents Presented";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_REGISTERED_TODAY) + Get_LT_GTSymbol(DayWiseResult.DOCS_REGISTERED_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_REGISTERED_YESTERDAY);
                    DashboardSummaryRecListItem.Description = "No. of Documents Registered";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_TODAY) + Get_LT_GTSymbol(DayWiseResult.DOCS_PENDING_WRT_YESTERDAY); ;
                    DashboardSummaryRecListItem.Yesterday = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_YESTERDAY); ;
                    DashboardSummaryRecListItem.Description = "No. of Documents Kept Pending";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_NEITHER_REGISTERED_NOR_PENDING_TODAY) + Get_LT_GTSymbol(DayWiseResult.NPNR_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_NEITHER_REGISTERED_NOR_PENDING_YESTERDAY);
                    DashboardSummaryRecListItem.Description = "Not Registered Not Pending";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = DayWiseResult.STAMP_DUTY_COLLECTED_TODAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(DayWiseResult.STAMP_DUTY_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = DayWiseResult.STAMP_DUTY_COLLECTED_YESTERDAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecListItem.Description = "Stamp Duty Collected (Rs. in Cr)";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = DayWiseResult.REGFEE_COLLECTD_TODAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(DayWiseResult.REG_FEE_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = DayWiseResult.REGFEE_COLLECTD_YESTERDAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecListItem.Description = "Registration fees Collected (Rs. in Cr)";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = DayWiseResult.TOTAL_COLLECTED_TODAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(DayWiseResult.TOTAL_COLLECTED_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = DayWiseResult.TOTAL_COLLECTED_YESTERDAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecListItem.Description = "Total Revenue Collected (Rs. in Cr)";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);
                    //From Shubham
                    //String.Format("{0:n0}", 9876);
                    DashboardSummaryRecList[0].CurrentMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_PRESENTED_CURR_MONTH) + Get_LT_GTSymbol(MonthWiseResult.NO_OF_DOCS_PRESENTED_WRT_PREV_MONTH);
                    DashboardSummaryRecList[0].PreviousMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_PRESENTED_PREV_MONTH);

                    // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 10-09-2020
                    //DashboardSummaryRecList[1].CurrentMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_REGISTERED_CURR_MONTH) + Get_LT_GTSymbol(MonthWiseResult.NO_OF_DOCUMENTS_REGISTERED_PREV_MONTH);
                    DashboardSummaryRecList[1].CurrentMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_REGISTERED_CURR_MONTH) + Get_LT_GTSymbol(MonthWiseResult.NO_OF_DOCS_REGISTERED_WRT_PREV_MONTH);
                    DashboardSummaryRecList[1].PreviousMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_REGISTERED_PREV_MONTH);

                    DashboardSummaryRecList[2].CurrentMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_CURR_MONTH) + Get_LT_GTSymbol(MonthWiseResult.NO_OF_DOCS_KEPT_PENDING_WRT_PREV_MONTH);
                    DashboardSummaryRecList[2].PreviousMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_PREV_MONTH);

                    DashboardSummaryRecList[3].CurrentMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_CURR_MONTH) + Get_LT_GTSymbol(MonthWiseResult.NO_OF_DOCS_NRNP_WRT_PREV_MONTH);
                    DashboardSummaryRecList[3].PreviousMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_PREV_MONTH);

                    DashboardSummaryRecList[4].CurrentMonth = MonthWiseResult.STAMPDUTY_LACS_CURR_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(MonthWiseResult.STAMPDUTY_WRT_PREV_MONTH);
                    DashboardSummaryRecList[4].PreviousMonth = MonthWiseResult.STAMPDUTY_LACS_PREV_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecList[5].CurrentMonth = MonthWiseResult.REGISTRATIONFEE_LACS_CURR_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(MonthWiseResult.REGISTRATIONFEE_WRT_PREV_MONTH);
                    DashboardSummaryRecList[5].PreviousMonth = MonthWiseResult.REGISTRATIONFEE_LACS_PREV_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    DashboardSummaryRecList[6].CurrentMonth = MonthWiseResult.TOTAL_LACS_CURR_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(MonthWiseResult.TOTAL_WRT_PREV_MONTH);
                    DashboardSummaryRecList[6].PreviousMonth = MonthWiseResult.TOTAL_LACS_PREV_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));


                    DashboardSummaryRecList[0].CurrentFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_PRESENTED_CURR_FYEAR) + Get_LT_GTSymbol(FinYearWiseResult.NO_OF_DOCS_PRESENTED_WRT_PREV_FYEAR);
                    //COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 21-09-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[0].PrevFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_PRESENTED_PREV_FYEAR)+"*";
                    DashboardSummaryRecList[0].PrevFinYear = String.Format("{0:n0}", "N.A.") + "*";
                    //DashboardSummaryRecList[0].PrevFinYear = "*";

                    DashboardSummaryRecList[1].CurrentFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_REGISTERED_CURR_FYEAR) + Get_LT_GTSymbol(FinYearWiseResult.NO_OF_DOCS_REGISTERED_WRT_PREV_FYEAR);
                    DashboardSummaryRecList[1].PrevFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_REGISTERED_PREV_FYEAR);

                    // COMMENTED AND CHANGED BY SHUBHAM BHAGAT 07-09-2020
                    //DashboardSummaryRecList[2].CurrentFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_PREV_FYEAR) + Get_LT_GTSymbol(FinYearWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_WRT_PREV_FYEAR); ;
                    DashboardSummaryRecList[2].CurrentFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_CURR_FYEAR) + Get_LT_GTSymbol(FinYearWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_WRT_PREV_FYEAR); ;
                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[2].PrevFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_PREV_FYEAR)+"*";
                    DashboardSummaryRecList[2].PrevFinYear = String.Format("{0:n0}", "N.A.") + "*";
                    DashboardSummaryRecList[3].CurrentFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_CURR_FYEAR) + Get_LT_GTSymbol(FinYearWiseResult.NO_OF_DOCS_NRNP_WRT_PREV_FYEAR); ;
                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[3].PrevFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_PREV_FYEAR);
                    DashboardSummaryRecList[3].PrevFinYear = String.Format("{0:n0}", "N.A.") + "*";

                    DashboardSummaryRecList[4].CurrentFinYear = FinYearWiseResult.STAMPDUTY_LACS_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(FinYearWiseResult.STAMPDUTY_WRT_PREV_FYEAR);
                    DashboardSummaryRecList[4].PrevFinYear = FinYearWiseResult.STAMPDUTY_LACS_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    DashboardSummaryRecList[5].CurrentFinYear = FinYearWiseResult.REGISTRATIONFEE_LACS_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(FinYearWiseResult.REGISTRATIONFEE_LACS_WRT_PREV_FYEAR);
                    DashboardSummaryRecList[5].PrevFinYear = FinYearWiseResult.REGISTRATIONFEE_LACS_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    DashboardSummaryRecList[6].CurrentFinYear = FinYearWiseResult.TOTAL_LACS_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(FinYearWiseResult.TOTAL_LACS_WRT_PREV_FYEAR);
                    DashboardSummaryRecList[6].PrevFinYear = FinYearWiseResult.TOTAL_LACS_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    //COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 21-09-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[0].UptoCurrentFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCUMENTS_PRESENTED_UPTO_UPTO_CURR_FYEAR)+"*";
                    DashboardSummaryRecList[0].UptoCurrentFinYear = String.Format("{0:n0}", "N.A.") + "*";
                    //DashboardSummaryRecList[0].UptoCurrentFinYear = "*";

                    DashboardSummaryRecList[0].UptoPrevFinYear = UptoDateWiseResult.NO_OF_DOCUMENTS_PRESENTED_UPTO_PREV_FYEAR.ToString();

                    //String.Format("{0:n0}", 9876);

                    DashboardSummaryRecList[1].UptoCurrentFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCUMENTS_REGISTERED_UPTO_CURR_FYEAR);
                    DashboardSummaryRecList[1].UptoPrevFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCUMENTS_REGISTERED_UPTO_PREV_FYEAR);

                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[2].UptoCurrentFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_UPTO_CURR_FYEAR)+"*";
                    DashboardSummaryRecList[2].UptoCurrentFinYear = String.Format("{0:n0}", "N.A.") + "*";
                    DashboardSummaryRecList[2].UptoPrevFinYear = UptoDateWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_UPTO_PREV_FYEAR.ToString();

                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[3].UptoCurrentFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_UPTO_CURR_FYEAR);
                    DashboardSummaryRecList[3].UptoCurrentFinYear = String.Format("{0:n0}", "N.A.") + "*";

                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[3].UptoPrevFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_UPTO_PREV_FYEAR);
                    DashboardSummaryRecList[3].UptoPrevFinYear = String.Format("{0:n0}", "N.A.")+"*";

                    DashboardSummaryRecList[4].UptoCurrentFinYear = UptoDateWiseResult.STAMPDUTY_LACS_UPTO_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecList[4].UptoPrevFinYear = UptoDateWiseResult.STAMPDUTY_LACS_UPTO_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    DashboardSummaryRecList[5].UptoCurrentFinYear = UptoDateWiseResult.REGISTRATIONFEE_LACS_UPTO_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecList[5].UptoPrevFinYear = UptoDateWiseResult.REGISTRATIONFEE_LACS_UPTO_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    //DashboardSummaryRecList[6].UptoCurrentFinYear = UptoDateWiseResult.TOTAL_LACS_UPTO_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(UptoDateWiseResult.TOTAL_LACS_WRT_PREV_FYEAR);
                    DashboardSummaryRecList[6].UptoCurrentFinYear = UptoDateWiseResult.TOTAL_LACS_UPTO_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecList[6].UptoPrevFinYear = UptoDateWiseResult.TOTAL_LACS_UPTO_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));


                    DashboardSummarytblResModel.IDashboardSummaryRecData = DashboardSummaryRecList;
                }
                //ADDED BY PANKAJ SAKHARE ON 18-09-2020
                else
                {
                    DashboardSummaryRecData DashboardSummaryRecListItem;
                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_PRESENTED_TODAY) + Get_LT_GTSymbol_ForExcel(DayWiseResult.DOCS_PRESENTED_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_PRESENTED_YESTERDAY);
                    DashboardSummaryRecListItem.Description = "No. of Documents Presented";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_REGISTERED_TODAY) + Get_LT_GTSymbol_ForExcel(DayWiseResult.DOCS_REGISTERED_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_REGISTERED_YESTERDAY);
                    DashboardSummaryRecListItem.Description = "No. of Documents Registered";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_TODAY) + Get_LT_GTSymbol_ForExcel(DayWiseResult.DOCS_PENDING_WRT_YESTERDAY); ;
                    DashboardSummaryRecListItem.Yesterday = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_YESTERDAY); ;
                    DashboardSummaryRecListItem.Description = "No. of Documents Kept Pending";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_NEITHER_REGISTERED_NOR_PENDING_TODAY) + Get_LT_GTSymbol_ForExcel(DayWiseResult.NPNR_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = String.Format("{0:n0}", DayWiseResult.NO_OF_DOCUMENTS_NEITHER_REGISTERED_NOR_PENDING_YESTERDAY);
                    DashboardSummaryRecListItem.Description = "Not Registered Not Pending";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = DayWiseResult.STAMP_DUTY_COLLECTED_TODAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol_ForExcel(DayWiseResult.STAMP_DUTY_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = DayWiseResult.STAMP_DUTY_COLLECTED_YESTERDAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecListItem.Description = "Stamp Duty Collected (Rs. in Cr)";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = DayWiseResult.REGFEE_COLLECTD_TODAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol_ForExcel(DayWiseResult.REG_FEE_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = DayWiseResult.REGFEE_COLLECTD_YESTERDAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecListItem.Description = "Registration fees Collected (Rs. in Cr)";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);

                    DashboardSummaryRecListItem = new DashboardSummaryRecData();
                    DashboardSummaryRecListItem.Today = DayWiseResult.TOTAL_COLLECTED_TODAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol_ForExcel(DayWiseResult.TOTAL_COLLECTED_WRT_YESTERDAY);
                    DashboardSummaryRecListItem.Yesterday = DayWiseResult.TOTAL_COLLECTED_YESTERDAY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecListItem.Description = "Total Revenue Collected (Rs. in Cr)";
                    DashboardSummaryRecList.Add(DashboardSummaryRecListItem);
                    //From Shubham
                    //String.Format("{0:n0}", 9876);
                    DashboardSummaryRecList[0].CurrentMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_PRESENTED_CURR_MONTH) + Get_LT_GTSymbol_ForExcel(MonthWiseResult.NO_OF_DOCS_PRESENTED_WRT_PREV_MONTH);
                    DashboardSummaryRecList[0].PreviousMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_PRESENTED_PREV_MONTH);

                    // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 10-09-2020
                    //DashboardSummaryRecList[1].CurrentMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_REGISTERED_CURR_MONTH) + Get_LT_GTSymbol(MonthWiseResult.NO_OF_DOCUMENTS_REGISTERED_PREV_MONTH);
                    DashboardSummaryRecList[1].CurrentMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_REGISTERED_CURR_MONTH) + Get_LT_GTSymbol_ForExcel(MonthWiseResult.NO_OF_DOCS_REGISTERED_WRT_PREV_MONTH);
                    DashboardSummaryRecList[1].PreviousMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_REGISTERED_PREV_MONTH);

                    DashboardSummaryRecList[2].CurrentMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_CURR_MONTH) + Get_LT_GTSymbol_ForExcel(MonthWiseResult.NO_OF_DOCS_KEPT_PENDING_WRT_PREV_MONTH);
                    DashboardSummaryRecList[2].PreviousMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_PREV_MONTH);

                    DashboardSummaryRecList[3].CurrentMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_CURR_MONTH) + Get_LT_GTSymbol_ForExcel(MonthWiseResult.NO_OF_DOCS_NRNP_WRT_PREV_MONTH);
                    DashboardSummaryRecList[3].PreviousMonth = String.Format("{0:n0}", MonthWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_PREV_MONTH);

                    DashboardSummaryRecList[4].CurrentMonth = MonthWiseResult.STAMPDUTY_LACS_CURR_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol_ForExcel(MonthWiseResult.STAMPDUTY_WRT_PREV_MONTH);
                    DashboardSummaryRecList[4].PreviousMonth = MonthWiseResult.STAMPDUTY_LACS_PREV_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecList[5].CurrentMonth = MonthWiseResult.REGISTRATIONFEE_LACS_CURR_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol_ForExcel(MonthWiseResult.REGISTRATIONFEE_WRT_PREV_MONTH);
                    DashboardSummaryRecList[5].PreviousMonth = MonthWiseResult.REGISTRATIONFEE_LACS_PREV_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    DashboardSummaryRecList[6].CurrentMonth = MonthWiseResult.TOTAL_LACS_CURR_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol_ForExcel(MonthWiseResult.TOTAL_WRT_PREV_MONTH);
                    DashboardSummaryRecList[6].PreviousMonth = MonthWiseResult.TOTAL_LACS_PREV_MONTH.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));


                    DashboardSummaryRecList[0].CurrentFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_PRESENTED_CURR_FYEAR) + Get_LT_GTSymbol_ForExcel(FinYearWiseResult.NO_OF_DOCS_PRESENTED_WRT_PREV_FYEAR);
                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[0].PrevFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_PRESENTED_PREV_FYEAR)+"*";
                    DashboardSummaryRecList[0].PrevFinYear = String.Format("{0:n0}", "N.A.") + "*";

                    //DashboardSummaryRecList[0].PrevFinYear = "*";
                    DashboardSummaryRecList[1].CurrentFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_REGISTERED_CURR_FYEAR) + Get_LT_GTSymbol_ForExcel(FinYearWiseResult.NO_OF_DOCS_REGISTERED_WRT_PREV_FYEAR);
                    DashboardSummaryRecList[1].PrevFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_REGISTERED_PREV_FYEAR);

                    // COMMENTED AND CHANGED BY SHUBHAM BHAGAT 07-09-2020
                    //DashboardSummaryRecList[2].CurrentFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_PREV_FYEAR) + Get_LT_GTSymbol(FinYearWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_WRT_PREV_FYEAR); ;
                    DashboardSummaryRecList[2].CurrentFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_CURR_FYEAR) + Get_LT_GTSymbol_ForExcel(FinYearWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_WRT_PREV_FYEAR); ;
                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[2].PrevFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_PREV_FYEAR)+"*";
                    DashboardSummaryRecList[2].PrevFinYear = String.Format("{0:n0}", "N.A.") + "*";

                    DashboardSummaryRecList[3].CurrentFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_CURR_FYEAR) + Get_LT_GTSymbol_ForExcel(FinYearWiseResult.NO_OF_DOCS_NRNP_WRT_PREV_FYEAR); ;
                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[3].PrevFinYear = String.Format("{0:n0}", FinYearWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_PREV_FYEAR);
                    DashboardSummaryRecList[3].PrevFinYear = String.Format("{0:n0}", "N.A.") + "*";

                    DashboardSummaryRecList[4].CurrentFinYear = FinYearWiseResult.STAMPDUTY_LACS_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol_ForExcel(FinYearWiseResult.STAMPDUTY_WRT_PREV_FYEAR);
                    DashboardSummaryRecList[4].PrevFinYear = FinYearWiseResult.STAMPDUTY_LACS_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    DashboardSummaryRecList[5].CurrentFinYear = FinYearWiseResult.REGISTRATIONFEE_LACS_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol_ForExcel(FinYearWiseResult.REGISTRATIONFEE_LACS_WRT_PREV_FYEAR);
                    DashboardSummaryRecList[5].PrevFinYear = FinYearWiseResult.REGISTRATIONFEE_LACS_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    DashboardSummaryRecList[6].CurrentFinYear = FinYearWiseResult.TOTAL_LACS_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol_ForExcel(FinYearWiseResult.TOTAL_LACS_WRT_PREV_FYEAR);
                    DashboardSummaryRecList[6].PrevFinYear = FinYearWiseResult.TOTAL_LACS_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[0].UptoCurrentFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCUMENTS_PRESENTED_UPTO_UPTO_CURR_FYEAR)+"*";
                    DashboardSummaryRecList[0].UptoCurrentFinYear = String.Format("{0:n0}", "N.A.") + "*";
                    //DashboardSummaryRecList[0].UptoCurrentFinYear = "*";
                    DashboardSummaryRecList[0].UptoPrevFinYear = UptoDateWiseResult.NO_OF_DOCUMENTS_PRESENTED_UPTO_PREV_FYEAR.ToString();

                    //String.Format("{0:n0}", 9876);

                    DashboardSummaryRecList[1].UptoCurrentFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCUMENTS_REGISTERED_UPTO_CURR_FYEAR);
                    DashboardSummaryRecList[1].UptoPrevFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCUMENTS_REGISTERED_UPTO_PREV_FYEAR);

                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[2].UptoCurrentFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_UPTO_CURR_FYEAR)+"*";
                    DashboardSummaryRecList[2].UptoCurrentFinYear = String.Format("{0:n0}", "N.A.") + "*";

                    DashboardSummaryRecList[2].UptoPrevFinYear = UptoDateWiseResult.NO_OF_DOCUMENTS_KEPT_PENDING_UPTO_PREV_FYEAR.ToString();

                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[3].UptoCurrentFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_UPTO_CURR_FYEAR);
                    DashboardSummaryRecList[3].UptoCurrentFinYear = String.Format("{0:n0}", "N.A.") + "*";

                    //COMMENTED AND CHANGED BY PANKAJ SAKHARE ON 12-10-2020 AFTER DISUCSSION WITH CHETAN SIR
                    //DashboardSummaryRecList[3].UptoPrevFinYear = String.Format("{0:n0}", UptoDateWiseResult.NO_OF_DOCS_NEIGHTER_REGISTERED_NOR_PENDING_UPTO_PREV_FYEAR);
                    DashboardSummaryRecList[3].UptoPrevFinYear = String.Format("{0:n0}", "N.A.") + "*";

                    DashboardSummaryRecList[4].UptoCurrentFinYear = UptoDateWiseResult.STAMPDUTY_LACS_UPTO_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecList[4].UptoPrevFinYear = UptoDateWiseResult.STAMPDUTY_LACS_UPTO_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    DashboardSummaryRecList[5].UptoCurrentFinYear = UptoDateWiseResult.REGISTRATIONFEE_LACS_UPTO_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecList[5].UptoPrevFinYear = UptoDateWiseResult.REGISTRATIONFEE_LACS_UPTO_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    //DashboardSummaryRecList[6].UptoCurrentFinYear = UptoDateWiseResult.TOTAL_LACS_UPTO_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + Get_LT_GTSymbol(UptoDateWiseResult.TOTAL_LACS_WRT_PREV_FYEAR);
                    DashboardSummaryRecList[6].UptoCurrentFinYear = UptoDateWiseResult.TOTAL_LACS_UPTO_CURR_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    DashboardSummaryRecList[6].UptoPrevFinYear = UptoDateWiseResult.TOTAL_LACS_UPTO_PREV_FYEAR.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));


                    DashboardSummarytblResModel.IDashboardSummaryRecData = DashboardSummaryRecList;
                }

                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDashboardSumaryTable-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                return DashboardSummarytblResModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }
        private string Get_LT_GTSymbol(Decimal Percentage)
        {
            try
            {
                if (Percentage != null)
                {
                    if (Percentage < 0)
                    {
                        return "<span style='color:Red;margin-left:10%;'> ↓ " + Math.Abs(Percentage).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " %</span>";

                    }
                    else if (Percentage > 0)
                    {
                        return "<span style='color:Green;margin-left:10%;'> ↑ " + Percentage.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " %</span>";
                    }
                    else
                    {
                        return " ";

                    }


                }
                else
                {
                    decimal percentage = 0;
                    return percentage.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    //return Percentage.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }



        public GraphTableResponseModel LoadRevenueCollectedChartData(DashboardDetailsViewModel model)
        {
            GraphTableResponseModel returnModel = new GraphTableResponseModel();
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadRevenueCollectedChartData-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                searchDBContext = new KaigrSearchDB();
                //var returnValues = dbContext.USP_MIS_REG_TREND_INPUT_0_COVER(0, 0, "0", "0", "0", "0", "0", 0, 0, "0", "0").ToList<USP_MIS_REG_TREND_INPUT_0_COVER_Result>(); //;.FirstOrDefault();
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadRevenueCollectedChartData-Before searchDBContext.USP_DB_GET_SALES_STASTICS_REVENUE_COLLECTED");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                var returnValues = searchDBContext.USP_DB_GET_SALES_STASTICS_REVENUE_COLLECTED(model.DistrictCode, model.SROfficeID).ToList(); //;.FirstOrDefault();
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadRevenueCollectedChartData-After searchDBContext.USP_DB_GET_SALES_STASTICS_REVENUE_COLLECTED");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                int ReturnValuesCnt = returnValues.Count();
                int RowCount = returnValues.Select(v => v.FYEAR).Distinct().Count();
                if (model.toggleBtnId == 1)
                {
                    returnModel._SalesStatisticsLineChartModel = new SalesStatisticsLineChartModel();

                    int Index = 0;
                    //if (iRownCount > 0)
                    //{
                    returnModel._SalesStatisticsLineChartModel.AgriGreaterThanTenLakhs = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.AgriLessThanTenLakhs = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.FaltsApartments = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.Lease = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.NonAgriGreaterThanTenLakhs = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.NonAgriLessThanTenLakhs = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.FinYear = new string[RowCount];

                    foreach (var item in returnValues)
                    {
                        returnModel._SalesStatisticsLineChartModel.AgriLessThanTenLakhs[Index] = item.AGRI_LT_10LAKHS.Value;
                        returnModel._SalesStatisticsLineChartModel.AgriGreaterThanTenLakhs[Index] = item.AGRI_GT_10LAKHS.Value;
                        returnModel._SalesStatisticsLineChartModel.FaltsApartments[Index] = item.APARTMENT.Value;
                        returnModel._SalesStatisticsLineChartModel.Lease[Index] = item.LEASE.Value;
                        returnModel._SalesStatisticsLineChartModel.NonAgriGreaterThanTenLakhs[Index] = item.NONAGRI_GT_10LAKHS.Value;
                        returnModel._SalesStatisticsLineChartModel.NonAgriLessThanTenLakhs[Index] = item.NONAGRI_LT_10LAKHS.Value;
                        returnModel._SalesStatisticsLineChartModel.FinYear[Index++] = item.FYEAR_TEXT;
                    }
                    //returnModel._SalesStatisticsLineChartModel.AgriGreaterThanTenLakhs = array;
                    //returnModel._SalesStatisticsLineChartModel.AgriLessThanTenLakhs = array1;
                    //returnModel._SalesStatisticsLineChartModel.FaltsApartments = array2;
                    //returnModel._SalesStatisticsLineChartModel.Lease = array3;
                    //returnModel._SalesStatisticsLineChartModel.NonAgriGreaterThanTenLakhs = array4;
                    //returnModel._SalesStatisticsLineChartModel.NonAgriLessThanTenLakhs = array5;
                    //returnModel._SalesStatisticsLineChartModel.FinYear = new int[8] { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017 };


                    //for (int i = 0; i < iRowCount; i++)
                    //{
                    //    returnModel.AgriGreaterThanTenLakhs[i] = (int)returnValues[i].AgriGreaterThanTenLakhs;
                    //    returnModel.AgriLessThanTenLakhs[i] = (int)returnValues[i].AgriLessThanTenLakhs;

                    //    returnModel.Lease[i] = (int)returnValues[i].Lease;
                    //    returnModel.NonAgriGreaterThanTenLakhs[i] = (String)returnValues[i].Time;
                    //    returnModel.NonAgriLessThanTenLakhs[i] = (String)returnValues[i].Time;

                    //    //    returnModel.PaidStampDuty = new int[] {
                    //    //     (int)returnValues[i].PaidStampDuty
                    //    //};

                    //    //    returnModel.RegistrationFee = new int[] {
                    //    //         (int)returnValues[i].RegistrationFee
                    //    //};

                    //}

                    //    }
                    //    else {

                    //    returnModel.NoOfDocsRegistered = new int[3];
                    //    returnModel.PaidStampDuty = new int[3];
                    //    returnModel.RegistrationFee = new int[3];
                    //    returnModel.Financialyear = new string[] { "NA", "NA", "NA" };

                    //}
                    returnModel._SalesStatisticsLineChartModel.Lbl_NonAgriLessThanTenLakhs = "Non Agri < 10Lakhs";
                    returnModel._SalesStatisticsLineChartModel.Lbl_AgriLessThanTenLakhs = "Agri < 10Lakhs";
                    returnModel._SalesStatisticsLineChartModel.Lbl_FaltsApartments = "Flats / Apartments";
                    returnModel._SalesStatisticsLineChartModel.Lbl_Lease = "Lease";
                    returnModel._SalesStatisticsLineChartModel.Lbl_NonAgriGreaterThanTenLakhs = "NonAgri > 10Lakhs";
                    returnModel._SalesStatisticsLineChartModel.Lbl_AgriGreaterThanTenLakhs = "Agri > 10Lakhs";
                }

                if (model.toggleBtnId == 2)
                {
                    returnModel._TableDataWrapper = new TableDataWrapper();
                    List<TableDataSalesStatisticsDocReg> tableDataList = new List<TableDataSalesStatisticsDocReg>();
                    List<ColumnArray> ColumnArrayList = new List<ColumnArray>();
                    int Index = 1;

                    //for (int i = 1; i <= 10; i++)
                    foreach (var item in returnValues)
                    {
                        TableDataSalesStatisticsDocReg tableData = new TableDataSalesStatisticsDocReg();
                        tableData.AgriLessThan10Lakhs = item.AGRI_LT_10LAKHS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.AgriGreaterThan10Lakhs = item.AGRI_GT_10LAKHS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.NonAgriGreaterThan10Lakhs = item.NONAGRI_GT_10LAKHS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.NonAgriLessThan10Lakhs = item.NONAGRI_LT_10LAKHS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.FlatsApartment = item.APARTMENT.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.Lease = item.LEASE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.SRNo = Index++;
                        tableData.FinYear = item.FYEAR_TEXT;
                        tableDataList.Add(tableData);
                    }

                    ColumnArrayList.Add(new ColumnArray { title = "Sr No", data = "SRNo" });
                    ColumnArrayList.Add(new ColumnArray { title = "Fin Year", data = "FinYear" });
                    ColumnArrayList.Add(new ColumnArray { title = "NonAgri < 10Lakhs", data = "NonAgriLessThan10Lakhs" });
                    ColumnArrayList.Add(new ColumnArray { title = "NonAgri > 10Lakhs", data = "NonAgriGreaterThan10Lakhs" });
                    ColumnArrayList.Add(new ColumnArray { title = "Agri < 10Lakhs", data = "AgriLessThan10Lakhs" });
                    ColumnArrayList.Add(new ColumnArray { title = "Agri > 10Lakhs", data = "AgriGreaterThan10Lakhs" });
                    ColumnArrayList.Add(new ColumnArray { title = "Flats / Apartments", data = "FlatsApartment" });
                    ColumnArrayList.Add(new ColumnArray { title = "Lease", data = "Lease" });
                    returnModel._TableDataWrapper.ColumnArray = ColumnArrayList.ToArray();
                    returnModel._TableDataWrapper.tableSalesStatisticsDocReg = tableDataList.ToArray();
                }

                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadRevenueCollectedChartData-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                return returnModel;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public GraphTableResponseModel PopulateSurchargeCessBarChart(DashboardDetailsViewModel model)
        {
            GraphTableResponseModel returnModel = new GraphTableResponseModel();
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-PopulateSurchargeCessBarChart-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                searchDBContext = new KaigrSearchDB();
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-PopulateSurchargeCessBarChart-Before searchDBContext.USP_DB_GET_SURCHARGE_CESS");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                var returnValues = searchDBContext.USP_DB_GET_SURCHARGE_CESS(model.DistrictCode, model.SROfficeID, model.SNatureOfDocID).ToList(); //;.FirstOrDefault();
                                                                                                                                                   // ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-PopulateSurchargeCessBarChart-After searchDBContext.USP_DB_GET_SURCHARGE_CESS");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                int ReturnCount = returnValues.Count();
                if (model.toggleBtnId == 1)
                {
                    //var returnValues = dbContext.USP_MIS_REG_TREND_INPUT_0_COVER(0, 0, "0", "0", "0", "0", "0", 0, 0, "0", "0").ToList<USP_MIS_REG_TREND_INPUT_0_COVER_Result>(); //;.FirstOrDefault();

                    decimal[] SurchargeArray;
                    decimal[] Total;
                    decimal[] CessArray;
                    String[] FinYearArray;
                    int Index = 0;
                    SurchargeArray = new decimal[ReturnCount];
                    CessArray = new decimal[ReturnCount];
                    Total = new decimal[ReturnCount];
                    FinYearArray = new string[ReturnCount];
                    foreach (var item in returnValues)
                    {
                        // CASTING DONE FROM DECIMAL NULLABLE TO DECIMAL BY SHUBHAM BHAGAT ON 06-10-2020
                        SurchargeArray[Index] = item.SURCHARGE.Value;
                        CessArray[Index] = item.CESS.Value;
                        FinYearArray[Index] = item.FYEAR_TEXT;
                        Total[Index] = item.CESS.Value + item.SURCHARGE.Value;
                        Index++;
                    }

                    returnModel._SurchargeAndCessBarChartModel = new SurchargeAndCessBarChartModel();
                    //int iRowCount = returnValues.Count;
                    int iRowCount = 6;
                    //if (iRownCount > 0)
                    //{
                    returnModel._SurchargeAndCessBarChartModel.SurchargeCollected = new decimal[ReturnCount];
                    returnModel._SurchargeAndCessBarChartModel.CessCollested = new decimal[ReturnCount];
                    returnModel._SurchargeAndCessBarChartModel.SurchargeCollected = SurchargeArray;
                    returnModel._SurchargeAndCessBarChartModel.CessCollested = CessArray;
                    returnModel._SurchargeAndCessBarChartModel.FinYear = FinYearArray;
                    returnModel._SurchargeAndCessBarChartModel.Total = Total;


                    returnModel._SurchargeAndCessBarChartModel.Lbl_CessCollected = "Cess Collected";
                    returnModel._SurchargeAndCessBarChartModel.Lbl_SurchargeCollected = "Surcharge Collected";
                    returnModel._SurchargeAndCessBarChartModel.Lbl_Total = "Total";
                }

                if (model.toggleBtnId == 2)
                {
                    returnModel._TableDataWrapper = new TableDataWrapper();
                    List<TableDataSurchargeCess> tableDataList = new List<TableDataSurchargeCess>();
                    List<ColumnArray> ColumnArrayList = new List<ColumnArray>();


                    //for (int i = 1; i <= 10; i++)
                    //{
                    //    TableData tableData = new TableData();
                    //    tableData.SrNo = i;
                    //    tableData.Duration = "duration " + i;
                    //    tableDataList.Add(tableData);

                    int Index = 1;
                    foreach (var item in returnValues)
                    {
                        // CASTING DONE FROM DECIMAL NULLABLE TO DECIMAL BY SHUBHAM BHAGAT ON 06-10-2020
                        TableDataSurchargeCess tableData = new TableDataSurchargeCess();
                        tableData.SRNo = Index++;
                        tableData.Surcharge = item.SURCHARGE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.Cess = item.CESS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.Fin_Years = item.FYEAR_TEXT;
                        tableData.Total = (item.SURCHARGE.Value + item.CESS.Value).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableDataList.Add(tableData);
                    }
                    ColumnArrayList.Add(new ColumnArray { title = "Sr No", data = "SRNo" });
                    ColumnArrayList.Add(new ColumnArray { title = "Fin Year", data = "Fin_Years" });
                    ColumnArrayList.Add(new ColumnArray { title = "Surcharge", data = "Surcharge" });
                    ColumnArrayList.Add(new ColumnArray { title = "Cess", data = "Cess" });
                    ColumnArrayList.Add(new ColumnArray { title = "Total", data = "Total" });
                    returnModel._TableDataWrapper.ColumnArray = ColumnArrayList.ToArray();
                    returnModel._TableDataWrapper.tableDataSurchargeAndCess = tableDataList.ToArray();
                }
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-PopulateSurchargeCessBarChart-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                return returnModel;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        // HIGH_VALUE_REVENUE_COLLECTED Chart and Datatable
        public GraphTableResponseModel LoadHighValPropChartData(DashboardDetailsViewModel model)//Line Chart 
        {
            #region Commented by shubham on 1-4-2020
            //GraphTableResponseModel returnModel = new GraphTableResponseModel();
            //try
            //{

            //    //var returnValues = dbContext.USP_MIS_REG_TREND_INPUT_0_COVER(0, 0, "0", "0", "0", "0", "0", 0, 0, "0", "0").ToList<USP_MIS_REG_TREND_INPUT_0_COVER_Result>(); //;.FirstOrDefault();
            //    //   var returnValues = dbContext.USP_MIS_REG_TREND_INPUT_0_COVER(model.DistrictID, model.RegistrationOfficeID, model.VillageID.ToString(), Deeds, model.NationalityID.ToString(), model.PropertyTypeID.ToString(), model.PropertyTypeSubListID.ToString(), model.ConsiderationListID, model.ConsiderationRange, model.fromDate, model.ToDate).ToList<USP_MIS_REG_TREND_INPUT_0_COVER_Result>(); //;.FirstOrDefault();
            //    if (model.toggleBtnId == 1)//For graph
            //    {
            //        returnModel._HighValPropLineChartModel = new HighValPropLineChartModel();
            //        int[] array = new int[10] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            //        int[] array1 = new int[10] { 25, 35, 40, 56, 68, 78, 80, 232, 123, 129 };
            //        int[] array2 = new int[10] { 54, 678, 798, 990, 44, 67, 70, 96, 90, 100 };
            //        int[] array3 = new int[10] { 105, 208, 680, 470, 560, 670, 750, 840, 40, 100 };
            //        int[] array4 = new int[10] { 108, 207, 308, 409, 500, 604, 705, 807, 908, 100 };
            //        int[] array5 = new int[10] { 10, 20, 30, 40, 550, 450, 710, 820, 940, 140 };

            //        //int iRowCount = returnValues.Count;
            //        int iRowCount = 6;
            //        //if (iRownCount > 0)
            //        //{
            //        returnModel._HighValPropLineChartModel.OneLakhToTenLakhs = new int[iRowCount];
            //        returnModel._HighValPropLineChartModel.TenLakhsToOneCrore = new int[iRowCount];
            //        returnModel._HighValPropLineChartModel.OneCroreToFiveCrore = new int[iRowCount];
            //        returnModel._HighValPropLineChartModel.FiveCroreToTenCrore = new int[iRowCount];
            //        returnModel._HighValPropLineChartModel.AboveTenCrore = new int[iRowCount];

            //        returnModel._HighValPropLineChartModel.OneLakhToTenLakhs = array;
            //        returnModel._HighValPropLineChartModel.TenLakhsToOneCrore = array1;
            //        returnModel._HighValPropLineChartModel.OneCroreToFiveCrore = array2;
            //        returnModel._HighValPropLineChartModel.FiveCroreToTenCrore = array3;
            //        returnModel._HighValPropLineChartModel.AboveTenCrore = array4;
            //        returnModel._HighValPropLineChartModel.FinYear = new int[8] { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017 };


            //        //for (int i = 0; i < iRowCount; i++)
            //        //{
            //        //    returnModel.AgriGreaterThanTenLakhs[i] = (int)returnValues[i].AgriGreaterThanTenLakhs;
            //        //    returnModel.AgriLessThanTenLakhs[i] = (int)returnValues[i].AgriLessThanTenLakhs;

            //        //    returnModel.Lease[i] = (int)returnValues[i].Lease;
            //        //    returnModel.NonAgriGreaterThanTenLakhs[i] = (String)returnValues[i].Time;
            //        //    returnModel.NonAgriLessThanTenLakhs[i] = (String)returnValues[i].Time;

            //        //    //    returnModel.PaidStampDuty = new int[] {
            //        //    //     (int)returnValues[i].PaidStampDuty
            //        //    //};

            //        //    //    returnModel.RegistrationFee = new int[] {
            //        //    //         (int)returnValues[i].RegistrationFee
            //        //    //};

            //        //}

            //        //    }
            //        //    else {

            //        //    returnModel.NoOfDocsRegistered = new int[3];
            //        //    returnModel.PaidStampDuty = new int[3];
            //        //    returnModel.RegistrationFee = new int[3];
            //        //    returnModel.Financialyear = new string[] { "NA", "NA", "NA" };

            //        //}
            //        returnModel._HighValPropLineChartModel.Lbl_OneLakhToTenLakhs = "One Lakh To Ten Lakhs";
            //        returnModel._HighValPropLineChartModel.Lbl_TenLakhsToOneCrore = "Ten Lakhs To One Crore";
            //        returnModel._HighValPropLineChartModel.Lbl_OneCroreToFiveCrore = "One Crore To Five Crore";
            //        returnModel._HighValPropLineChartModel.Lbl_FiveCroreToTenCrore = "Five Crore To Ten Crore";
            //        returnModel._HighValPropLineChartModel.Lbl_AboveTenCrore = "Above Ten Crore";
            //    }

            //    if (model.toggleBtnId == 2)//For Datatable
            //    {
            //        returnModel._TableDataWrapper = new TableDataWrapper();
            //        List<TableData> tableDataList = new List<TableData>();
            //        List<ColumnArray> ColumnArrayList = new List<ColumnArray>();


            //        for (int i = 1; i <= 10; i++)
            //        {
            //            TableData tableData = new TableData();
            //            tableData.SrNo = i;
            //            tableData.Duration = "duration " + i;
            //            tableDataList.Add(tableData);
            //        }

            //        ColumnArrayList.Add(new ColumnArray { title = "Sr No", data = "SrNo" });
            //        ColumnArrayList.Add(new ColumnArray { title = "Duration", data = "Duration" });
            //        returnModel._TableDataWrapper.ColumnArray = ColumnArrayList.ToArray();
            //        returnModel._TableDataWrapper.TableDataArray = tableDataList.ToArray();
            //    }

            //    return returnModel;
            //}
            //catch (Exception)
            //{
            //    throw;
            //} 
            #endregion

            GraphTableResponseModel returnModel = new GraphTableResponseModel();
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadHighValPropChartData-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                searchDBContext = new KaigrSearchDB();
                //var returnValues = dbContext.USP_MIS_REG_TREND_INPUT_0_COVER(0, 0, "0", "0", "0", "0", "0", 0, 0, "0", "0").ToList<USP_MIS_REG_TREND_INPUT_0_COVER_Result>(); //;.FirstOrDefault();
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadHighValPropChartData-Before searchDBContext.USP_DB_GET_HIGH_VALUE_REVENUE_COLLECTED");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                var returnValues = searchDBContext.USP_DB_GET_HIGH_VALUE_REVENUE_COLLECTED(model.DistrictCode, model.SROfficeID).ToList(); //;.FirstOrDefault();
                                                                                                                                           // ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadHighValPropChartData-After searchDBContext.USP_DB_GET_HIGH_VALUE_REVENUE_COLLECTED");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                int ReturnValuesCnt = returnValues.Count();
                int RowCount = returnValues.Select(v => v.FYEAR).Distinct().Count();
                //int rowCount= returnValues.Distinct()
                if (model.toggleBtnId == 1)
                {
                    returnModel._HighValPropLineChartModel = new HighValPropLineChartModel();


                    int Index = 0;
                    //if (iRownCount > 0)
                    //{
                    returnModel._HighValPropLineChartModel.OneLakhToTenLakhs = new decimal[RowCount];
                    returnModel._HighValPropLineChartModel.TenLakhsToOneCrore = new decimal[RowCount];
                    returnModel._HighValPropLineChartModel.OneCroreToFiveCrore = new decimal[RowCount];
                    returnModel._HighValPropLineChartModel.FiveCroreToTenCrore = new decimal[RowCount];
                    returnModel._HighValPropLineChartModel.AboveTenCrore = new decimal[RowCount];
                    returnModel._HighValPropLineChartModel.FinYear = new string[RowCount];

                    foreach (var item in returnValues)
                    {
                        returnModel._HighValPropLineChartModel.OneLakhToTenLakhs[Index] = item.C1_TO_10_LAKHS.Value;
                        returnModel._HighValPropLineChartModel.TenLakhsToOneCrore[Index] = item.C10_LAKHS_TO_1CRORE.Value;
                        returnModel._HighValPropLineChartModel.OneCroreToFiveCrore[Index] = item.C1CRORE_TO_5CRORE.Value;
                        returnModel._HighValPropLineChartModel.FiveCroreToTenCrore[Index] = item.C5CRORE_TO_10CRORE.Value;
                        returnModel._HighValPropLineChartModel.AboveTenCrore[Index] = item.ABOVE_10CRORE.Value;
                        returnModel._HighValPropLineChartModel.FinYear[Index++] = item.FYEAR_TEXT;
                    }
                    //returnModel._SalesStatisticsLineChartModel.AgriGreaterThanTenLakhs = array;
                    //returnModel._SalesStatisticsLineChartModel.AgriLessThanTenLakhs = array1;
                    //returnModel._SalesStatisticsLineChartModel.FaltsApartments = array2;
                    //returnModel._SalesStatisticsLineChartModel.Lease = array3;
                    //returnModel._SalesStatisticsLineChartModel.NonAgriGreaterThanTenLakhs = array4;
                    //returnModel._SalesStatisticsLineChartModel.NonAgriLessThanTenLakhs = array5;
                    //returnModel._SalesStatisticsLineChartModel.FinYear = new int[8] { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017 };


                    //for (int i = 0; i < iRowCount; i++)
                    //{
                    //    returnModel.AgriGreaterThanTenLakhs[i] = (int)returnValues[i].AgriGreaterThanTenLakhs;
                    //    returnModel.AgriLessThanTenLakhs[i] = (int)returnValues[i].AgriLessThanTenLakhs;

                    //    returnModel.Lease[i] = (int)returnValues[i].Lease;
                    //    returnModel.NonAgriGreaterThanTenLakhs[i] = (String)returnValues[i].Time;
                    //    returnModel.NonAgriLessThanTenLakhs[i] = (String)returnValues[i].Time;

                    //    //    returnModel.PaidStampDuty = new int[] {
                    //    //     (int)returnValues[i].PaidStampDuty
                    //    //};

                    //    //    returnModel.RegistrationFee = new int[] {
                    //    //         (int)returnValues[i].RegistrationFee
                    //    //};

                    //}

                    //    }
                    //    else {

                    //    returnModel.NoOfDocsRegistered = new int[3];
                    //    returnModel.PaidStampDuty = new int[3];
                    //    returnModel.RegistrationFee = new int[3];
                    //    returnModel.Financialyear = new string[] { "NA", "NA", "NA" };

                    //}

                    returnModel._HighValPropLineChartModel.Lbl_OneLakhToTenLakhs = "1 Lakh To 10 Lakhs";
                    returnModel._HighValPropLineChartModel.Lbl_TenLakhsToOneCrore = "10 Lakhs To 1 Crore";
                    returnModel._HighValPropLineChartModel.Lbl_OneCroreToFiveCrore = "1 Crore To 5 Crore";
                    returnModel._HighValPropLineChartModel.Lbl_FiveCroreToTenCrore = "5 Crore To 10 Crore";
                    returnModel._HighValPropLineChartModel.Lbl_AboveTenCrore = "Above 10 Crore";
                }

                if (model.toggleBtnId == 2)
                {
                    returnModel._TableDataWrapper = new TableDataWrapper();
                    List<TableData_HIGH_VALUE_REVENUE_COLLECTED> tableDataList = new List<TableData_HIGH_VALUE_REVENUE_COLLECTED>();
                    List<ColumnArray> ColumnArrayList = new List<ColumnArray>();
                    int Index = 1;

                    //for (int i = 1; i <= 10; i++)
                    foreach (var item in returnValues)
                    {
                        TableData_HIGH_VALUE_REVENUE_COLLECTED tableData = new TableData_HIGH_VALUE_REVENUE_COLLECTED();
                        tableData.OneLakhToTenLakhs = item.C1_TO_10_LAKHS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.TenLakhsToOneCrore = item.C10_LAKHS_TO_1CRORE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.OneCroreToFiveCrore = item.C1CRORE_TO_5CRORE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.FiveCroreToTenCrore = item.C5CRORE_TO_10CRORE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.AboveTenCrore = item.ABOVE_10CRORE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.SRNo = Index++;
                        //ADDED BY SHUBHAM BHAGAT ON 1-4-2020
                        tableData.FinYear = String.IsNullOrEmpty(item.FYEAR_TEXT) ? String.Empty : item.FYEAR_TEXT;
                        tableDataList.Add(tableData);
                    }

                    ColumnArrayList.Add(new ColumnArray { title = "Sr No", data = "SRNo" });
                    ColumnArrayList.Add(new ColumnArray { title = "Fin Year", data = "FinYear" });
                    ColumnArrayList.Add(new ColumnArray { title = "One Lakh To Ten Lakhs", data = "OneLakhToTenLakhs" });
                    ColumnArrayList.Add(new ColumnArray { title = "Ten Lakhs To One Crore", data = "TenLakhsToOneCrore" });
                    ColumnArrayList.Add(new ColumnArray { title = "One Crore To Five Crore", data = "OneCroreToFiveCrore" });
                    ColumnArrayList.Add(new ColumnArray { title = "Five Crore To Ten Crore", data = "FiveCroreToTenCrore" });
                    ColumnArrayList.Add(new ColumnArray { title = "Above Ten Crore", data = "AboveTenCrore" });

                    returnModel._TableDataWrapper.ColumnArray = ColumnArrayList.ToArray();
                    returnModel._TableDataWrapper.TableData_HIGH_VALUE_REVENUE_COLLECTED = tableDataList.ToArray();
                }
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadHighValPropChartData-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                return returnModel;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public GraphTableResponseModel LoadDocumentRegisteredChartData(DashboardDetailsViewModel model)
        {
            GraphTableResponseModel returnModel = new GraphTableResponseModel();
            try
            {

                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDocumentRegisteredChartData-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                searchDBContext = new KaigrSearchDB();
                //var returnValues = dbContext.USP_MIS_REG_TREND_INPUT_0_COVER(0, 0, "0", "0", "0", "0", "0", 0, 0, "0", "0").ToList<USP_MIS_REG_TREND_INPUT_0_COVER_Result>(); //;.FirstOrDefault();

                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDocumentRegisteredChartData-Before searchDBContext.USP_DB_GET_SALES_STASTICS_DOCS_REGISTERED");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                var returnValues = searchDBContext.USP_DB_GET_SALES_STASTICS_DOCS_REGISTERED(model.DistrictCode, model.SROfficeID).ToList(); //;.FirstOrDefault();
                                                                                                                                             // ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDocumentRegisteredChartData-After searchDBContext.USP_DB_GET_SALES_STASTICS_DOCS_REGISTERED");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                int ReturnValuesCnt = returnValues.Count();
                int RowCount = returnValues.Select(v => v.FYEAR).Distinct().Count();
                //int rowCount= returnValues.Distinct()
                if (model.toggleBtnId == 1)
                {
                    returnModel._SalesStatisticsLineChartModel = new SalesStatisticsLineChartModel();


                    int Index = 0;

                    returnModel._SalesStatisticsLineChartModel.AgriGreaterThanTenLakhs = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.AgriLessThanTenLakhs = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.FaltsApartments = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.Lease = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.NonAgriGreaterThanTenLakhs = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.NonAgriLessThanTenLakhs = new decimal[RowCount];
                    returnModel._SalesStatisticsLineChartModel.FinYear = new string[RowCount];

                    foreach (var item in returnValues)
                    {
                        returnModel._SalesStatisticsLineChartModel.AgriLessThanTenLakhs[Index] = item.AGRI_LT_10LAKHS.Value;
                        returnModel._SalesStatisticsLineChartModel.AgriGreaterThanTenLakhs[Index] = item.AGRI_GT_10LAKHS.Value;
                        returnModel._SalesStatisticsLineChartModel.FaltsApartments[Index] = item.APARTMENT.Value;
                        returnModel._SalesStatisticsLineChartModel.Lease[Index] = item.LEASE.Value;
                        returnModel._SalesStatisticsLineChartModel.NonAgriGreaterThanTenLakhs[Index] = item.NONAGRI_GT_10LAKHS.Value;
                        returnModel._SalesStatisticsLineChartModel.NonAgriLessThanTenLakhs[Index] = item.NONAGRI_LT_10LAKHS.Value;
                        returnModel._SalesStatisticsLineChartModel.FinYear[Index++] = item.FYEAR_TEXT;
                    }
                    //returnModel._SalesStatisticsLineChartModel.AgriGreaterThanTenLakhs = array;
                    //returnModel._SalesStatisticsLineChartModel.AgriLessThanTenLakhs = array1;
                    //returnModel._SalesStatisticsLineChartModel.FaltsApartments = array2;
                    //returnModel._SalesStatisticsLineChartModel.Lease = array3;
                    //returnModel._SalesStatisticsLineChartModel.NonAgriGreaterThanTenLakhs = array4;
                    //returnModel._SalesStatisticsLineChartModel.NonAgriLessThanTenLakhs = array5;
                    //returnModel._SalesStatisticsLineChartModel.FinYear = new int[8] { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017 };


                    //for (int i = 0; i < iRowCount; i++)
                    //{
                    //    returnModel.AgriGreaterThanTenLakhs[i] = (int)returnValues[i].AgriGreaterThanTenLakhs;
                    //    returnModel.AgriLessThanTenLakhs[i] = (int)returnValues[i].AgriLessThanTenLakhs;

                    //    returnModel.Lease[i] = (int)returnValues[i].Lease;
                    //    returnModel.NonAgriGreaterThanTenLakhs[i] = (String)returnValues[i].Time;
                    //    returnModel.NonAgriLessThanTenLakhs[i] = (String)returnValues[i].Time;

                    //    //    returnModel.PaidStampDuty = new int[] {
                    //    //     (int)returnValues[i].PaidStampDuty
                    //    //};

                    //    //    returnModel.RegistrationFee = new int[] {
                    //    //         (int)returnValues[i].RegistrationFee
                    //    //};

                    //}

                    //    }
                    //    else {

                    //    returnModel.NoOfDocsRegistered = new int[3];
                    //    returnModel.PaidStampDuty = new int[3];
                    //    returnModel.RegistrationFee = new int[3];
                    //    returnModel.Financialyear = new string[] { "NA", "NA", "NA" };

                    //}


                    returnModel._SalesStatisticsLineChartModel.Lbl_NonAgriLessThanTenLakhs = "Non Agri < 10Lakhs";
                    returnModel._SalesStatisticsLineChartModel.Lbl_AgriLessThanTenLakhs = "Agri < 10Lakhs";
                    returnModel._SalesStatisticsLineChartModel.Lbl_FaltsApartments = "Flats / Apartments";
                    returnModel._SalesStatisticsLineChartModel.Lbl_Lease = "Lease";
                    returnModel._SalesStatisticsLineChartModel.Lbl_NonAgriGreaterThanTenLakhs = "NonAgri > 10Lakhs";
                    returnModel._SalesStatisticsLineChartModel.Lbl_AgriGreaterThanTenLakhs = "Agri > 10Lakhs";
                }

                if (model.toggleBtnId == 2)
                {
                    returnModel._TableDataWrapper = new TableDataWrapper();
                    List<TableDataSalesStatisticsDocReg> tableDataList = new List<TableDataSalesStatisticsDocReg>();
                    List<ColumnArray> ColumnArrayList = new List<ColumnArray>();
                    int Index = 1;

                    //for (int i = 1; i <= 10; i++)
                    foreach (var item in returnValues)
                    {
                        TableDataSalesStatisticsDocReg tableData = new TableDataSalesStatisticsDocReg();

                        // COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-09-2020 AFTER DISCUSSION WITH SIR
                        //tableData.AgriLessThan10Lakhs = item.AGRI_LT_10LAKHS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        //tableData.AgriGreaterThan10Lakhs = item.AGRI_GT_10LAKHS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        //tableData.NonAgriGreaterThan10Lakhs = item.NONAGRI_GT_10LAKHS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        //tableData.NonAgriLessThan10Lakhs = item.NONAGRI_LT_10LAKHS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        //tableData.FlatsApartment = item.APARTMENT.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        //tableData.Lease = item.LEASE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                        tableData.AgriLessThan10Lakhs = item.AGRI_LT_10LAKHS.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.AgriGreaterThan10Lakhs = item.AGRI_GT_10LAKHS.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.NonAgriGreaterThan10Lakhs = item.NONAGRI_GT_10LAKHS.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.NonAgriLessThan10Lakhs = item.NONAGRI_LT_10LAKHS.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.FlatsApartment = item.APARTMENT.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.Lease = item.LEASE.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                        tableData.SRNo = Index++;
                        tableData.FinYear = item.FYEAR_TEXT;
                        tableDataList.Add(tableData);
                    }

                    ColumnArrayList.Add(new ColumnArray { title = "Sr No", data = "SRNo" });
                    ColumnArrayList.Add(new ColumnArray { title = "Fin Year", data = "FinYear" });
                    ColumnArrayList.Add(new ColumnArray { title = "NonAgri < 10Lakhs", data = "NonAgriLessThan10Lakhs" });
                    ColumnArrayList.Add(new ColumnArray { title = "NonAgri > 10Lakhs", data = "NonAgriGreaterThan10Lakhs" });
                    ColumnArrayList.Add(new ColumnArray { title = "Agri < 10Lakhs", data = "AgriLessThan10Lakhs" });
                    ColumnArrayList.Add(new ColumnArray { title = "Agri > 10Lakhs", data = "AgriGreaterThan10Lakhs" });
                    ColumnArrayList.Add(new ColumnArray { title = "Flats / Apartments", data = "FlatsApartment" });
                    ColumnArrayList.Add(new ColumnArray { title = "Lease", data = "Lease" });
                    returnModel._TableDataWrapper.ColumnArray = ColumnArrayList.ToArray();
                    returnModel._TableDataWrapper.tableSalesStatisticsDocReg = tableDataList.ToArray();
                }
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadDocumentRegisteredChartData-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                return returnModel;
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        //ADDED BY SHUBHAM BHAGAT ON 1-4-2020        
        // HIGH_VALUE_DOCS_REGISTERED Chart and Datatable
        public GraphTableResponseModel LoadHighValPropChartDataForDocs(DashboardDetailsViewModel model)//Line Chart 
        {
            GraphTableResponseModel returnModel = new GraphTableResponseModel();
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadHighValPropChartDataForDocs-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                searchDBContext = new KaigrSearchDB();
                //var returnValues = dbContext.USP_MIS_REG_TREND_INPUT_0_COVER(0, 0, "0", "0", "0", "0", "0", 0, 0, "0", "0").ToList<USP_MIS_REG_TREND_INPUT_0_COVER_Result>(); //;.FirstOrDefault();

                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadHighValPropChartDataForDocs-Before searchDBContext.USP_DB_GET_HIGH_VALUE_DOCS_REGISTERED");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                var returnValues = searchDBContext.USP_DB_GET_HIGH_VALUE_DOCS_REGISTERED(model.DistrictCode, model.SROfficeID).ToList(); //;.FirstOrDefault();
                                                                                                                                         // ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadHighValPropChartDataForDocs-After searchDBContext.USP_DB_GET_HIGH_VALUE_DOCS_REGISTERED");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                int ReturnValuesCnt = returnValues.Count();
                int RowCount = returnValues.Select(v => v.FYEAR).Distinct().Count();
                //int rowCount= returnValues.Distinct()
                if (model.toggleBtnId == 1)
                {
                    returnModel._HighValPropLineChartModel = new HighValPropLineChartModel();


                    int Index = 0;
                    //if (iRownCount > 0)
                    //{
                    returnModel._HighValPropLineChartModel.OneLakhToTenLakhs = new decimal[RowCount];
                    returnModel._HighValPropLineChartModel.TenLakhsToOneCrore = new decimal[RowCount];
                    returnModel._HighValPropLineChartModel.OneCroreToFiveCrore = new decimal[RowCount];
                    returnModel._HighValPropLineChartModel.FiveCroreToTenCrore = new decimal[RowCount];
                    returnModel._HighValPropLineChartModel.AboveTenCrore = new decimal[RowCount];
                    returnModel._HighValPropLineChartModel.FinYear = new string[RowCount];

                    foreach (var item in returnValues)
                    {
                        returnModel._HighValPropLineChartModel.OneLakhToTenLakhs[Index] = item.C1_TO_10_LAKHS.Value;
                        returnModel._HighValPropLineChartModel.TenLakhsToOneCrore[Index] = item.C10_LAKHS_TO_1CRORE.Value;
                        returnModel._HighValPropLineChartModel.OneCroreToFiveCrore[Index] = item.C1CRORE_TO_5CRORE.Value;
                        returnModel._HighValPropLineChartModel.FiveCroreToTenCrore[Index] = item.C5CRORE_TO_10CRORE.Value;
                        returnModel._HighValPropLineChartModel.AboveTenCrore[Index] = item.ABOVE_10CRORE.Value;
                        returnModel._HighValPropLineChartModel.FinYear[Index++] = item.FYEAR_TEXT;
                    }
                    //returnModel._SalesStatisticsLineChartModel.AgriGreaterThanTenLakhs = array;
                    //returnModel._SalesStatisticsLineChartModel.AgriLessThanTenLakhs = array1;
                    //returnModel._SalesStatisticsLineChartModel.FaltsApartments = array2;
                    //returnModel._SalesStatisticsLineChartModel.Lease = array3;
                    //returnModel._SalesStatisticsLineChartModel.NonAgriGreaterThanTenLakhs = array4;
                    //returnModel._SalesStatisticsLineChartModel.NonAgriLessThanTenLakhs = array5;
                    //returnModel._SalesStatisticsLineChartModel.FinYear = new int[8] { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017 };


                    //for (int i = 0; i < iRowCount; i++)
                    //{
                    //    returnModel.AgriGreaterThanTenLakhs[i] = (int)returnValues[i].AgriGreaterThanTenLakhs;
                    //    returnModel.AgriLessThanTenLakhs[i] = (int)returnValues[i].AgriLessThanTenLakhs;

                    //    returnModel.Lease[i] = (int)returnValues[i].Lease;
                    //    returnModel.NonAgriGreaterThanTenLakhs[i] = (String)returnValues[i].Time;
                    //    returnModel.NonAgriLessThanTenLakhs[i] = (String)returnValues[i].Time;

                    //    //    returnModel.PaidStampDuty = new int[] {
                    //    //     (int)returnValues[i].PaidStampDuty
                    //    //};

                    //    //    returnModel.RegistrationFee = new int[] {
                    //    //         (int)returnValues[i].RegistrationFee
                    //    //};

                    //}

                    //    }
                    //    else {

                    //    returnModel.NoOfDocsRegistered = new int[3];
                    //    returnModel.PaidStampDuty = new int[3];
                    //    returnModel.RegistrationFee = new int[3];
                    //    returnModel.Financialyear = new string[] { "NA", "NA", "NA" };

                    //}

                    returnModel._HighValPropLineChartModel.Lbl_OneLakhToTenLakhs = "1 Lakh To 10 Lakhs";
                    returnModel._HighValPropLineChartModel.Lbl_TenLakhsToOneCrore = "10 Lakhs To 1 Crore";
                    returnModel._HighValPropLineChartModel.Lbl_OneCroreToFiveCrore = "1 Crore To 5 Crore";
                    returnModel._HighValPropLineChartModel.Lbl_FiveCroreToTenCrore = "5 Crore To 10 Crore";
                    returnModel._HighValPropLineChartModel.Lbl_AboveTenCrore = "Above 10 Crore";
                }

                if (model.toggleBtnId == 2)
                {
                    returnModel._TableDataWrapper = new TableDataWrapper();
                    List<TableData_HIGH_VALUE_REVENUE_COLLECTED> tableDataList = new List<TableData_HIGH_VALUE_REVENUE_COLLECTED>();
                    List<ColumnArray> ColumnArrayList = new List<ColumnArray>();
                    int Index = 1;
                    //returnModel._HighValPropLineChartModel.OneLakhToTenLakhs = new decimal[RowCount];
                    //returnModel._HighValPropLineChartModel.TenLakhsToOneCrore = new decimal[RowCount];
                    //returnModel._HighValPropLineChartModel.OneCroreToFiveCrore = new decimal[RowCount];
                    //returnModel._HighValPropLineChartModel.FiveCroreToTenCrore = new decimal[RowCount];
                    ////returnModel._HighValPropLineChartModel.AboveTenCrore = new decimal[RowCount];
                    //returnModel._HighValPropLineChartModel.FinYear = new string[RowCount];

                    //for (int i = 1; i <= 10; i++)
                    foreach (var item in returnValues)
                    {
                        TableData_HIGH_VALUE_REVENUE_COLLECTED tableData = new TableData_HIGH_VALUE_REVENUE_COLLECTED();


                        // COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-09-2020 AFTER DISCUSSION WITH SIR
                        //tableData.OneLakhToTenLakhs = item.C1_TO_10_LAKHS.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        //tableData.TenLakhsToOneCrore = item.C10_LAKHS_TO_1CRORE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        //tableData.OneCroreToFiveCrore = item.C1CRORE_TO_5CRORE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        //tableData.FiveCroreToTenCrore = item.C5CRORE_TO_10CRORE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        //tableData.AboveTenCrore = item.ABOVE_10CRORE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                        tableData.OneLakhToTenLakhs = item.C1_TO_10_LAKHS.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.TenLakhsToOneCrore = item.C10_LAKHS_TO_1CRORE.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.OneCroreToFiveCrore = item.C1CRORE_TO_5CRORE.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.FiveCroreToTenCrore = item.C5CRORE_TO_10CRORE.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.AboveTenCrore = item.ABOVE_10CRORE.Value.ToString("N0", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        tableData.SRNo = Index++;
                        //ADDED BY SHUBHAM BHAGAT ON 1-4-2020
                        tableData.FinYear = String.IsNullOrEmpty(item.FYEAR_TEXT) ? String.Empty : item.FYEAR_TEXT;
                        tableDataList.Add(tableData);
                    }

                    ColumnArrayList.Add(new ColumnArray { title = "Sr No", data = "SRNo" });
                    ColumnArrayList.Add(new ColumnArray { title = "Fin Year", data = "FinYear" });
                    ColumnArrayList.Add(new ColumnArray { title = "One Lakh To Ten Lakhs", data = "OneLakhToTenLakhs" });
                    ColumnArrayList.Add(new ColumnArray { title = "Ten Lakhs To One Crore", data = "TenLakhsToOneCrore" });
                    ColumnArrayList.Add(new ColumnArray { title = "One Crore To Five Crore", data = "OneCroreToFiveCrore" });
                    ColumnArrayList.Add(new ColumnArray { title = "Five Crore To Ten Crore", data = "FiveCroreToTenCrore" });
                    ColumnArrayList.Add(new ColumnArray { title = "Above Ten Crore", data = "AboveTenCrore" });

                    returnModel._TableDataWrapper.ColumnArray = ColumnArrayList.ToArray();
                    returnModel._TableDataWrapper.TableData_HIGH_VALUE_REVENUE_COLLECTED = tableDataList.ToArray();
                }
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardDAL-LoadHighValPropChartDataForDocs-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                return returnModel;
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        #region ADDED BY SHUBHAM BHAGAT 09-04-2020
        public NatureOfArticle_REQ_RES_Model NatureOfDocumentByRadioType(NatureOfArticle_REQ_RES_Model model)
        {
            KaveriEntities dbContextForRegArtiList = null;
            try
            {
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                NatureOfArticle_REQ_RES_Model resModel = new NatureOfArticle_REQ_RES_Model();
                // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 14-08-2020 AT 11:33 AM
                //resModel.NatuereOfDocsList = objCommon.GetRegistrationArticles(false);
                resModel.NatuereOfDocsList = objCommon.GetRegistrationArticlesTop10Wise(false);
                //int[] NatureOfDocsIDArray_ALL = { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 130, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 42, 43, 44, 45, 46, 47, 1, 107, 48, 49, 50, 51, 52, 53, 128, 129, 54, 55, 57, 58, 59, 60, 61, 62, 63, 64, 65, 56, 66, 67, 68, 69, 70, 71, 72, 73, 74, 131, 132, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 3, 97, 98, 99, 100, 4, 101, 102, 104, 105, 106, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 27, 119, 120, 121, 122, 123, 124, 125, 127, 126 };
                //int[] NatureOfDocsIDArray_Top_10_Articles = { 106, 56, 69, 86, 98, 93, 59, 32, 125, 91 };
                dbContextForRegArtiList = new KaveriEntities();
                int[] NatureOfDocsIDArray_ALL = dbContextForRegArtiList.RegistrationArticles.Select(x => x.RegArticleCode).ToArray();

                int[] NatureOfDocsIDArray_Top_10_Articles = Array.ConvertAll(resModel.NatuereOfDocsList.Take(10).Select(x => x.Value).ToArray(), int.Parse);
                // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 14-08-2020 AT 11:33 AM
                int[] NatureOfDocsIDArray = model.RadioType == "ALL" ? NatureOfDocsIDArray_ALL : NatureOfDocsIDArray_Top_10_Articles;

                resModel.NatureOfDocID = NatureOfDocsIDArray;
                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContextForRegArtiList != null)
                {
                    dbContextForRegArtiList.Dispose();
                }
            }
        }

        #endregion
        #endregion

        //ADDED BY PANKAJ SAKHARE ON 18-09-2020
        private string Get_LT_GTSymbol_ForExcel(Decimal Percentage)
        {
            try
            {
                if (Percentage != null)
                {
                    if (Percentage < 0)
                    {
                        return " ↓ " + Math.Abs(Percentage).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " %";

                    }
                    else if (Percentage > 0)
                    {
                        return " ↑ " + Percentage.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + " %";
                    }
                    else
                    {
                        return " ";
                    }
                }
                else
                {
                    decimal percentage = 0;
                    return percentage.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    //return Percentage.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
        //For Dashboard tab1 Average Registration time
        public DashboardSummaryModel PopulateAvgRegTime(TilesReqModel reqModel)
        {
            DashboardSummaryModel dashboardSummaryModel = new DashboardSummaryModel();
            try
            {
                searchDBContext = new KaigrSearchDB();
                List<RevenueCollectionModel> RevenueModelList = new List<RevenueCollectionModel>();
                RevenueCollectionWrapperModel revenueCollectedWrapper = new RevenueCollectionWrapperModel();
                dashboardSummaryModel.Tiles = new List<DashboardTileModel>();

                #region commented code
                //Add parameter
                //var TilesData = searchDBContext.USP_DB_GET_CURRENT_STATUS(reqModel.selectedType, reqModel.OfficeCode, reqModel.FinYearId).ToList();

                //DashboardTileModel dashboardTileModel1 = new DashboardTileModel();
                //DashboardTileModel dashboardTileModel2 = new DashboardTileModel();
                //DashboardTileModel dashboardTileModel3 = new DashboardTileModel();
                //DashboardTileModel dashboardTileModel4 = new DashboardTileModel();
                //DashboardTileModel dashboardTileModel5 = new DashboardTileModel();
                //DashboardTileModel dashboardTileModel6 = new DashboardTileModel();

                //if (TilesData != null && TilesData.Count() != 0)
                //{
                //    string sDivisionString = "";
                //    if (reqModel.selectedType == "M")
                //        sDivisionString = "Day";
                //    if (reqModel.selectedType == "D")
                //        sDivisionString = "Hour";
                //    if (reqModel.selectedType == "F")
                //        sDivisionString = "Month";
                //    foreach (var item in TilesData)
                //    {
                //        dashboardTileModel1.Amount = (item.TOTAL_REV_COLLECTED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //        dashboardTileModel1.Title = "Total Revenue Collected";

                //        if (item.TOTAL_REV_COLLECTED_WRT_LY == null)
                //        {
                //            dashboardTileModel1.DescPercentage = "";
                //            dashboardTileModel1.Description = "-";
                //        }
                //        else
                //        {
                //            if (item.TOTAL_REV_COLLECTED_WRT_LY <= 0)
                //            {
                //                dashboardTileModel1.DescPercentage = Convert.ToDecimal(item.TOTAL_REV_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //                dashboardTileModel1.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                //            }
                //            else
                //            {
                //                dashboardTileModel1.DescPercentage = "+ " + Convert.ToDecimal(item.TOTAL_REV_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //                dashboardTileModel1.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");

                //            }

                //        }

                //        dashboardTileModel2.Amount = (item.TOTAL_SD_COLLECTED).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //        dashboardTileModel2.Title = "Total Stamp Duty Collected";
                //        if (item.TOTAL_SD_COLLECTED_WRT_LY == null)
                //        {
                //            dashboardTileModel2.DescPercentage = "";
                //            dashboardTileModel2.Description = "-";
                //        }
                //        else
                //        {

                //            if (item.TOTAL_SD_COLLECTED_WRT_LY <= 0)
                //            {
                //                dashboardTileModel2.DescPercentage = Convert.ToDecimal(item.TOTAL_SD_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //                dashboardTileModel2.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                //            }
                //            else
                //            {
                //                dashboardTileModel2.DescPercentage = "+ " + Convert.ToDecimal(item.TOTAL_SD_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                //                dashboardTileModel2.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");

                //            }
                //        }

                //        dashboardTileModel3.Amount = item.TOTAL_RF_COLLECTED.ToString();
                //        dashboardTileModel3.Title = "Total Registration fees Collected";

                //        if (item.TOTAL_RF_COLLECTED_WRT_LY == null)
                //        {
                //            dashboardTileModel3.DescPercentage = "";
                //            dashboardTileModel3.Description = "-";
                //        }
                //        else
                //        {
                //            if (item.TOTAL_RF_COLLECTED_WRT_LY <= 0)
                //            {
                //                dashboardTileModel3.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                //                dashboardTileModel3.DescPercentage = Convert.ToDecimal(item.TOTAL_RF_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //            }
                //            else
                //            {
                //                dashboardTileModel3.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                //                dashboardTileModel3.DescPercentage = "+ " + Convert.ToDecimal(item.TOTAL_RF_COLLECTED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                //            }
                //        }

                //        dashboardTileModel4.Amount = String.Format("{0:n0}", item.TOTAL_DOCS_REGISTERED);
                //        dashboardTileModel4.Title = "Total Documents Registered";
                //        if (item.TOTAL_DOCS_REGISTERED_WRT_LY == null)
                //        {
                //            dashboardTileModel4.DescPercentage = "";
                //            dashboardTileModel4.Description = "-";
                //        }
                //        else
                //        {
                //            if (item.TOTAL_DOCS_REGISTERED_WRT_LY <= 0)
                //            {
                //                dashboardTileModel4.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                //                dashboardTileModel4.DescPercentage = Convert.ToDecimal(item.TOTAL_DOCS_REGISTERED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //            }
                //            else
                //            {
                //                dashboardTileModel4.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                //                dashboardTileModel4.DescPercentage = "+ " + Convert.ToDecimal(item.TOTAL_DOCS_REGISTERED_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //            }
                //        }


                //        //dashboardTileModel5.Amount = Convert.ToInt32((item.AVG_DOCS_REGISTERED_PER_MONTH)).ToString();
                //        dashboardTileModel5.Amount = item.AVG_DOCS_REGISTERED_PER_MONTH == null ? "" : String.Format("{0:n0}", Convert.ToInt32(item.AVG_DOCS_REGISTERED_PER_MONTH.Value));
                //        dashboardTileModel5.Title = "Avg Doc Registered / " + sDivisionString;
                //        if (item.AVG_DOCS_REGISTERED_PER_MONTH_WRT_LY == null)
                //        {
                //            dashboardTileModel5.DescPercentage = "";
                //            dashboardTileModel5.Description = "-";
                //        }
                //        else
                //        {

                //            if (item.AVG_DOCS_REGISTERED_PER_MONTH_WRT_LY <= 0)
                //            {
                //                dashboardTileModel5.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                //                dashboardTileModel5.DescPercentage = Convert.ToDecimal(item.AVG_DOCS_REGISTERED_PER_MONTH_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //            }
                //            else
                //            {
                //                dashboardTileModel5.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                //                dashboardTileModel5.DescPercentage = "+ " + Convert.ToDecimal(item.AVG_DOCS_REGISTERED_PER_MONTH_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                //            }
                //        }

                //        dashboardTileModel6.Amount = Convert.ToInt32((item.AVG_REVENUE_COLLECTED_PER_MONTH)).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //        dashboardTileModel6.Title = "Average Revenue Collected / " + sDivisionString;
                //        if (item.AVG_REVENUE_COLLECTED_PER_MONTH_WRT_LY == null)
                //        {
                //            dashboardTileModel6.DescPercentage = "";
                //            dashboardTileModel6.Description = "-";
                //        }
                //        else
                //        {
                //            if (item.AVG_REVENUE_COLLECTED_PER_MONTH_WRT_LY <= 0)
                //            {
                //                dashboardTileModel6.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                //                dashboardTileModel6.DescPercentage = Convert.ToDecimal(item.AVG_REVENUE_COLLECTED_PER_MONTH_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //            }
                //            else
                //            {
                //                dashboardTileModel6.Description = (reqModel.selectedType == "F") ? " % wrt last fin year" : ((reqModel.selectedType == "M") ? " % wrt last Month" : " % wrt yesterday");
                //                dashboardTileModel6.DescPercentage = "+ " + Convert.ToDecimal(item.AVG_REVENUE_COLLECTED_PER_MONTH_WRT_LY).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                //            }
                //        }

                //        dashboardSummaryModel.Tiles.Add(dashboardTileModel1);
                //        dashboardSummaryModel.Tiles.Add(dashboardTileModel2);
                //        dashboardSummaryModel.Tiles.Add(dashboardTileModel3);
                //        dashboardSummaryModel.Tiles.Add(dashboardTileModel4);
                //        dashboardSummaryModel.Tiles.Add(dashboardTileModel5);
                //        dashboardSummaryModel.Tiles.Add(dashboardTileModel6);
                //    }
                //    //  }
                //}
                //else
                //{
                //    dashboardTileModel1.Amount = "0.00";
                //    dashboardTileModel1.Title = "Data Not Available";
                //    dashboardTileModel1.Description = "Data Not Available";
                //    dashboardTileModel1.DescPercentage = "";

                //    dashboardTileModel2.Amount = "0.00";
                //    dashboardTileModel2.Title = "Data Not Available";
                //    dashboardTileModel2.DescPercentage = "";
                //    dashboardTileModel2.Description = "Data Not Available";

                //    dashboardTileModel3.Amount = "0.00";
                //    dashboardTileModel3.Title = "Data Not Available";
                //    dashboardTileModel3.DescPercentage = "";
                //    dashboardTileModel3.Description = "Data Not Available";

                //    dashboardTileModel4.Amount = "0.00";
                //    dashboardTileModel4.Title = "Data Not Available";
                //    dashboardTileModel4.DescPercentage = "";
                //    dashboardTileModel4.Description = "Data Not Available";

                //    dashboardTileModel5.Amount = "0.00";
                //    dashboardTileModel5.Title = "Data Not Available";
                //    dashboardTileModel5.DescPercentage = "";
                //    dashboardTileModel5.Description = "Data Not Available";

                //    dashboardTileModel6.Amount = "0.00";
                //    dashboardTileModel6.Title = "Data Not Available";
                //    dashboardTileModel6.DescPercentage = "";
                //    dashboardTileModel6.Description = "Data Not Available";
                //    dashboardSummaryModel.Tiles.Add(dashboardTileModel1);
                //    dashboardSummaryModel.Tiles.Add(dashboardTileModel2);
                //    dashboardSummaryModel.Tiles.Add(dashboardTileModel3);
                //    dashboardSummaryModel.Tiles.Add(dashboardTileModel4);
                //    dashboardSummaryModel.Tiles.Add(dashboardTileModel5);
                //    dashboardSummaryModel.Tiles.Add(dashboardTileModel6);
                //}
                #endregion
                dashboardSummaryModel.LevelId = LevelID;
                //dashboardSummaryModel._RevenueCollectionWrapperModel = PopulateRevenueCollected(reqModel);
                //dashboardSummaryModel.CurrentAchievementsModel = PopulateCurrentAchievements(reqModel);
                //dashboardSummaryModel._ProgressBarTargetVsAchieved = PopulateProgressBarTargetVsAchieved(reqModel);
                //Add Parameter
                var resultList = searchDBContext.USP_DB_GET_TOP3BOTTOM3_AVG_REGISTRASTION_TIME_FYWISE(reqModel.OfficeCode, reqModel.selectedType, reqModel.FinYearId).ToList();
                string TempOfficeName = string.Empty;
                string sContent = string.Empty;
                dashboardSummaryModel.Top3AvgRegTime = string.Empty;
                dashboardSummaryModel.Bottom3AvgRegTime = string.Empty;
                foreach (var item in resultList)
                {
                    //if (item.OFFICE_NAME.Contains("Banglore Development Authority"))
                    //{
                    //    TempOfficeName = "BDA";
                    //}
                    //else if (item.OFFICE_NAME.Contains("Mysore Development Authority"))
                    //{
                    //    TempOfficeName = "MDA";
                    //}
                    //else
                    //{
                    //    TempOfficeName = item.OFFICE_NAME;
                    //}

                    if(item.seqNo < 4 )
                    //if (item.HIERARCHY == "T")
                    {
                        dashboardSummaryModel.Top3AvgRegTime +=
                         @" <div class='row'><div class='col-md-6 col-sm-6 col-xs-12'><label for='inputEmail3' class='text-success control-label'>" + item.OfficeName + "</label></div><div class='col-md-6 col-sm-6 col-xs-12'><label class='text-success'>" + item.ART + " minutes </label></div></div>";

                    }
                    else
                    {
                        dashboardSummaryModel.Bottom3AvgRegTime +=
                         @" <div class='row'><div class='col-md-6 col-sm-6 col-xs-12'><label for='inputEmail3' class='text-danger control-label'>" + item.OfficeName + "</label></div><div class='col-md-6 col-sm-6 col-xs-12'><label class='text-danger'>" + item.ART + " minutes </label></div></div>";
                    }
                }

                var avgRegTime = searchDBContext.USP_DB_GET_AVG_REGISTRASTION_TIME_FYWISE(reqModel.OfficeCode, reqModel.selectedType, reqModel.FinYearId).FirstOrDefault();
                dashboardSummaryModel.AVG_REGISTRASTION_TIME_FYWISE = avgRegTime == null ? string.Empty : ((int)avgRegTime).ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
            return dashboardSummaryModel;

        }

        // BELOW CODE IS COMMENTED BY SHUBHAM BHAGAT ON  26-11-2020
        // BECAUSE WE ARE NOT PROVIDING PROGRESS CHART MONTH WISE LABLE CLICK FUNCTIONALITY NOW
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        

        //public GraphTableResponseModel PopulateProgressChartMonthWise(DashboardDetailsViewModel model)
        //{
        //    GraphTableResponseModel returnModel = new GraphTableResponseModel();
        //    try
        //    {
        //        String[] Years=model.FinYear.Split('-');
        //        int finYear = Convert.ToInt32(Years[0]);
        //        searchDBContext = new KaigrSearchDB();
        //        var returnValues = searchDBContext.USP_DB_GET_MONTHWISE_PROGRESS(model.DistrictCode, finYear).ToList();

        //        int iRowCount = 0;

        //        if (returnValues != null)
        //            iRowCount = returnValues.Count;
        //        if (model.toggleBtnId == 1)
        //        {
        //            returnModel._ProgressChartModel = new ProgressChartModel();
        //            returnModel._ProgressChartModel = new ProgressChartModel();
        //            returnModel._ProgressChartModel.Documents = new int[iRowCount];
        //            returnModel._ProgressChartModel.Revenue = new int[iRowCount];
        //            returnModel._ProgressChartModel.Months = new string[iRowCount];

        //            for (int i = 0; i < iRowCount; i++)
        //            {
        //                returnModel._ProgressChartModel.Documents[i] = (returnValues[i].NO_OF_DOCS_REGISTERED == null) ? 0 : (int)returnValues[i].NO_OF_DOCS_REGISTERED;
        //                returnModel._ProgressChartModel.Revenue[i] = (returnValues[i].TOTAL_REVENUE == null) ? 0 : (int)returnValues[i].TOTAL_REVENUE;
        //                // COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 13-10-2020 
        //                returnModel._ProgressChartModel.Months[i] = (returnValues[i].MonthName == null) ? string.Empty : returnValues[i].MonthName;
        //                //returnModel._ProgressChartModel.FinYear[i] = (returnValues[i].FYEAR_STR == null) ? string.Empty :
        //                //    "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=DummyMethod('" + returnValues[i].FYEAR_STR + "')>" + returnValues[i].FYEAR_STR + "</a>";
        //            }
        //            returnModel._ProgressChartModel.Lbl_Documents = "Documents";
        //            returnModel._ProgressChartModel.Lbl_Revenue = "Revenue";
        //        }

        //        if (model.toggleBtnId == 2)
        //        {
        //            returnModel._TableDataWrapper = new TableDataWrapper();
        //            List<TableDataProgressChart> tableDataList = new List<TableDataProgressChart>();
        //            List<ColumnArray> ColumnArrayList = new List<ColumnArray>();

        //            TableDataProgressChart tableData;

        //            ColumnArrayList.Add(new ColumnArray { title = "Month", data = "MonthName" });
        //            ColumnArrayList.Add(new ColumnArray { title = "Documents", data = "NO_OF_DOCS_REGISTERED" });
        //            ColumnArrayList.Add(new ColumnArray { title = "Revenue", data = "TOTAL_REVENUE" });

        //            foreach (var item in returnValues)
        //            {
        //                tableData = new TableDataProgressChart();

        //                tableData.NO_OF_DOCS_REGISTERED = (item.NO_OF_DOCS_REGISTERED == null) ? string.Empty : Convert.ToString(item.NO_OF_DOCS_REGISTERED);
        //                tableData.MonthName = item.MonthName == null ? string.Empty : item.MonthName;
        //                tableData.TOTAL_REVENUE = (item.TOTAL_REVENUE == null) ? string.Empty : Convert.ToDecimal(item.TOTAL_REVENUE).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")); ;

        //                tableDataList.Add(tableData);
        //            }


        //            returnModel._TableDataWrapper.ColumnArray = ColumnArrayList.ToArray();
        //            returnModel._TableDataWrapper.TableDataArrayofProgressChart = tableDataList.ToArray();
        //        }

        //        return returnModel;
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
