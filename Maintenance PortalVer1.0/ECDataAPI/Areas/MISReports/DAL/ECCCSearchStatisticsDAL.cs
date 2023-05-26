using CustomModels.Models.MISReports.ECCCSearchStatistics;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Entity.KaveriEntities;
//using ECDataAPI.Entity.KaigrOnlineEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECDataAPI.Common;
using ECDataAPI.AnywhereCCService;
using System.Data;
using System.Globalization;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ECCCSearchStatisticsDAL : IECCCSearchStatistics
    {
        ApiCommonFunctions common = new ApiCommonFunctions();

        //Added by mayank date 14-7-20
        //to get view page model populate and return to view page
        /// <summary>
        /// ECCCSearchStatisticsView
        /// </summary>
        /// <param name=""></param>
        /// <returns>ECCCSearchStatisticsViewModel</returns>
        public ECCCSearchStatisticsViewModel ECCCSearchStatisticsView(int OfficeId)
        {
            try
            {
                ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel = new ECCCSearchStatisticsViewModel();
                eCCCSearchStatisticsViewModel.DROfficeList = getDroList();
                eCCCSearchStatisticsViewModel.SROfficeList = GetDefaultSroList();

                // BELOW CODE IS COMMENTED AND CHANGED ON 10-02-2021
                eCCCSearchStatisticsViewModel.FinancialYearList = getFinancialYearList();
                //eCCCSearchStatisticsViewModel.FinancialYearList = new List<SelectListItem> {
                //                                                        new SelectListItem{ Value = "0", Text = "Select" },
                //                                                        new SelectListItem{ Value = "2021", Text = "2021" },
                //                                                        new SelectListItem{ Value = "2020", Text = "2020" },
                //                                                        new SelectListItem{ Value = "2019", Text = "2019" },
                //                                                        new SelectListItem{ Value = "2018", Text = "2018" },
                //                                                        new SelectListItem{ Value = "2017", Text = "2017" },
                //                                                    };
                // ABOVE CODE IS COMMENTED AND CHANGED ON 10-02-2021

                eCCCSearchStatisticsViewModel.MonthList = getMonthList();
                eCCCSearchStatisticsViewModel.DroName = " ";
                eCCCSearchStatisticsViewModel.SroName = " ";
                eCCCSearchStatisticsViewModel.FinancialYearName = " ";
                eCCCSearchStatisticsViewModel.MonthName = " ";

                return eCCCSearchStatisticsViewModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Added by mayank date 14-7-20
        //to get Dro list data
        /// <summary>
        /// getDroList
        /// </summary>
        /// <param name=""></param>
        /// <returns>List<SelectListItem></returns>
        public List<SelectListItem> getDroList()
        {
            try
            {
                List<SelectListItem> DroList = new List<SelectListItem>();
                DroList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                KaveriEntities dbcontext = null;
                using (dbcontext = new KaveriEntities())
                {
                    var DroVarList = (from dro in dbcontext.DistrictMaster
                                      select new
                                      {
                                          dro.DistrictNameE,
                                          dro.DistrictCode
                                      }).ToList();
                    if (DroVarList != null)//
                    {
                        foreach (var item in DroVarList)
                        {
                            //DroList.Add(new SelectListItem { Value = item.DistrictCode.ToString(), Text = item.DistrictNameE });
                            DroList.Add(new SelectListItem { Value = item.DistrictCode.ToString(), Text = item.DistrictNameE ?? string.Empty });
                        }
                    }
                }
                return DroList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Added by mayank date 14-7-20
        //to get default sro list
        /// <summary>
        /// GetSroList
        /// </summary>
        /// <param name=""></param>
        /// <returns>List<SelectListItem></returns>
        public List<SelectListItem> GetDefaultSroList()
        {
            try
            {
                List<SelectListItem> SroList = new List<SelectListItem>();
                SroList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                return SroList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Added by mayank date 14-7-20
        //to get financial year list data
        /// <summary>
        /// getFinancialYearList
        /// </summary>
        /// <param name=""></param>
        /// <returns>List<SelectListItem></returns>
        public List<SelectListItem> getFinancialYearList()
        {
            try
            {
                List<SelectListItem> Financialyearlist = new List<SelectListItem>();
                Financialyearlist.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
                KaveriEntities dbcontext = null;
                using (dbcontext = new KaveriEntities())
                {
                    var financialyearlist = dbcontext.USP_FINANCIAL_YEAR().ToList();
                    if (financialyearlist != null)
                    {
                        foreach (var item in financialyearlist)
                        {
                            //Financialyearlist.Add(new SelectListItem { Value = item.YEAR.ToString(), Text = item.FYEAR });
                            Financialyearlist.Add(new SelectListItem { Value = item.YEAR.ToString(), Text = item.FYEAR ?? string.Empty });
                        }
                    }
                    return Financialyearlist;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        //Added by mayank date 14-7-20
        //to get Month list data
        /// <summary>
        /// getMonthList
        /// </summary>
        /// <param name=""></param>
        /// <returns>List<SelectListItem></returns>
        public List<SelectListItem> getMonthList()
        {
            try
            {
                List<SelectListItem> Monthlist = new List<SelectListItem>();
                Monthlist.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                Monthlist.Insert(1, new SelectListItem { Value = "4", Text = "April" });
                Monthlist.Insert(2, new SelectListItem { Value = "5", Text = "May" });
                Monthlist.Insert(3, new SelectListItem { Value = "6", Text = "June" });
                Monthlist.Insert(4, new SelectListItem { Value = "7", Text = "July" });
                Monthlist.Insert(5, new SelectListItem { Value = "8", Text = "August" });
                Monthlist.Insert(6, new SelectListItem { Value = "9", Text = "September" });
                Monthlist.Insert(7, new SelectListItem { Value = "10", Text = "October" });
                Monthlist.Insert(8, new SelectListItem { Value = "11", Text = "November" });
                Monthlist.Insert(9, new SelectListItem { Value = "12", Text = "December" });
                Monthlist.Insert(10, new SelectListItem { Value = "1", Text = "January" });
                Monthlist.Insert(11, new SelectListItem { Value = "2", Text = "February" });
                Monthlist.Insert(12, new SelectListItem { Value = "3", Text = "March" });

                return Monthlist;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Added by mayank date 14-7-20
        //to get Summary table data
        /// <summary>
        /// GetSummary
        /// </summary>
        /// <param name="eCCCSearchStatisticsViewModel"></param>
        /// <returns>ECCCSearchStatisticsResultModel</returns>
        public ECCCSearchStatisticsResultModel GetSummary(ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel)
        {
            ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();
            int FinancialyearCode = 0;
            try
            {
                //KaigrSearchDB kaigrSearchDBContext = new KaigrSearchDB();
                KaveriEntities dbcontext = new KaveriEntities();
                ECCCSearchStatisticsResultModel eCCCSearchStatisticsResultModel = new ECCCSearchStatisticsResultModel();
                eCCCSearchStatisticsResultModel.SummaryList = new List<ECCCSearchStatisticsSummaryModel>();
                DataSet Summary = null;

                // ADDED BY SHUBHAM BHAGAT ON 22-03-2021
                if (eCCCSearchStatisticsViewModel.MonthCode < 4 && eCCCSearchStatisticsViewModel.MonthCode != 0)
                    FinancialyearCode = eCCCSearchStatisticsViewModel.FinancialyearCode + 1;
                else
                    FinancialyearCode = eCCCSearchStatisticsViewModel.FinancialyearCode;
                //To get data from PreRegCCService
                Summary = objService.GetECCCSummaryData(eCCCSearchStatisticsViewModel.DROCode, eCCCSearchStatisticsViewModel.SROCode, eCCCSearchStatisticsViewModel.MonthCode, FinancialyearCode);


                if (Summary != null)
                {
                    ECCCSearchStatisticsSummaryModel eCCSearchStatisticsSummaryModel = new ECCCSearchStatisticsSummaryModel();

                    if (eCCCSearchStatisticsViewModel.DROCode != 0)
                        eCCCSearchStatisticsResultModel.DroName = (from dro in dbcontext.DistrictMaster where dro.DistrictCode == eCCCSearchStatisticsViewModel.DROCode select dro.DistrictNameE).FirstOrDefault();
                    else
                        eCCCSearchStatisticsResultModel.DroName = "All";
                    if (eCCCSearchStatisticsViewModel.SROCode != 0)
                        eCCCSearchStatisticsResultModel.SroName = (from dro in dbcontext.SROMaster where dro.SROCode == eCCCSearchStatisticsViewModel.SROCode select dro.SRONameE).FirstOrDefault();
                    else
                        eCCCSearchStatisticsResultModel.SroName = "All";
                    if (eCCCSearchStatisticsViewModel.FinancialyearCode != 0)
                        // BELOW CODE IS COMMENTED AND CHANGED ON 10-02-2021
                        eCCCSearchStatisticsResultModel.FinancialYearName = Convert.ToString(dbcontext.USP_FINANCIAL_YEAR().ToList().Where(m => m.YEAR == eCCCSearchStatisticsViewModel.FinancialyearCode).Select(m => m.FYEAR).FirstOrDefault());
                        //eCCCSearchStatisticsResultModel.FinancialYearName = eCCCSearchStatisticsViewModel.FinancialyearCode.ToString();
                    // ABOVE CODE IS COMMENTED AND CHANGED ON 10-02-2021

                    int i = 0;

                    foreach (DataTable dataTable in Summary.Tables)
                    {
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            eCCSearchStatisticsSummaryModel.SrNo = i + 1;

                            if (eCCCSearchStatisticsViewModel.MonthCode != 0)
                            {
                                if (!(dataRow["moNthname"] == DBNull.Value))
                                    eCCSearchStatisticsSummaryModel.MonthYear = Convert.ToString(dataRow["moNthname"]);
                                else
                                    eCCSearchStatisticsSummaryModel.MonthYear = "--";
                            }
                            else
                            {
                                if (!(dataRow["fyear"] == DBNull.Value))
                                    eCCSearchStatisticsSummaryModel.MonthYear = Convert.ToString(dataRow["fyear"]);
                                else
                                    eCCSearchStatisticsSummaryModel.MonthYear = "--";
                            }

                            if (!(dataRow["KOS_USER_LOGGED"] == DBNull.Value))
                                eCCSearchStatisticsSummaryModel.TotalUserLogged = Convert.ToInt32(dataRow["KOS_USER_LOGGED"]);
                            else
                                eCCSearchStatisticsSummaryModel.TotalUserLogged = 0;

                            if (!(dataRow["KOS_EC_SEARCHED"] == DBNull.Value))
                                eCCSearchStatisticsSummaryModel.TotalECSearched = Convert.ToInt32(dataRow["KOS_EC_SEARCHED"]);
                            else
                                eCCSearchStatisticsSummaryModel.TotalECSearched = 0;

                            if (!(dataRow["KOS_EC_SIGNED"] == DBNull.Value))
                                eCCSearchStatisticsSummaryModel.TotalECSigned = Convert.ToInt32(dataRow["KOS_EC_SIGNED"]);
                            else
                                eCCSearchStatisticsSummaryModel.TotalECSigned = 0;

                            if (!(dataRow["KOS_EC_SUBMITTED"] == DBNull.Value))
                                eCCSearchStatisticsSummaryModel.TotalECSubmitted = Convert.ToInt32(dataRow["KOS_EC_SUBMITTED"]);
                            else
                                eCCSearchStatisticsSummaryModel.TotalECSubmitted = 0;

                            if (!(dataRow["KOS_CC_SEARCHED"] == DBNull.Value))
                                eCCSearchStatisticsSummaryModel.TotalCCSearched = Convert.ToInt32(dataRow["KOS_CC_SEARCHED"]);
                            else
                                eCCSearchStatisticsSummaryModel.TotalCCSearched = 0;

                            if (!(dataRow["KOS_CC_SIGNED"] == DBNull.Value))
                                eCCSearchStatisticsSummaryModel.TotalCCSigned = Convert.ToInt32(dataRow["KOS_CC_SIGNED"]);
                            else
                                eCCSearchStatisticsSummaryModel.TotalCCSigned = 0;

                            if (!(dataRow["KOS_CC_SUBMITTED"] == DBNull.Value))
                                eCCSearchStatisticsSummaryModel.TotalCCSubmitted = Convert.ToInt32(dataRow["KOS_CC_SUBMITTED"]);
                            else
                                eCCSearchStatisticsSummaryModel.TotalCCSubmitted = 0;

                            eCCCSearchStatisticsResultModel.SummaryList.Add(eCCSearchStatisticsSummaryModel);
                        }
                    }
                    eCCCSearchStatisticsResultModel.TotalSummaryRecords = Summary.Tables.Count;

                    //ADDED BY PANKAJ ON 14-06-2021
                    //Commented by mayank on 26032021 for ECCC Statistics
                    //var details = kaigrSearchDBContext.USP_RPT_CONSOLIDATED_SUMMARY_ALLFYEAR(eCCCSearchStatisticsViewModel.DROCode, eCCCSearchStatisticsViewModel.SROCode).ToList();
                    //kaigrSearchDBContext.USP_RPT_OTH_ECCCCOUNT_SUMMARY_MONTHWISE()
                    //var details = kaigrSearchDBContext.USP_RPT_CONSOLIDATED_SUMMARY_ALLMONTH(eCCCSearchStatisticsViewModel.DROCode, eCCCSearchStatisticsViewModel.SROCode,eCCCSearchStatisticsViewModel.FinancialyearCode).ToList();

                    #region Commented by mayank content 23-06-2021 for ECCC Statistics


                    //foreach (var item in details)
                    //{
                    //    eCCCSearchStatisticsResultModel.SummaryList.ElementAt(0).AnywhereTotalECSigned += item.REG_COUNT_AnywhereEC;
                    //    eCCCSearchStatisticsResultModel.SummaryList.ElementAt(0).LocalTotalECSigned += item.REG_COUNT_LocalEC;
                    //    eCCCSearchStatisticsResultModel.SummaryList.ElementAt(0).AnywhereTotalCCSigned += item.REG_COUNT_AnywhereCC;
                    //    eCCCSearchStatisticsResultModel.SummaryList.ElementAt(0).LocalTotalCCSigned += item.REG_COUNT_LocalCC;
                    //   //item.REG_COUNT_AnywhereEC
                    //} 
                    #endregion

                    //var details = dbcontext.(eCCCSearchStatisticsViewModel.DROCode, eCCCSearchStatisticsViewModel.SROCode, FinancialyearCode).FirstOrDefault();
                    //if (details != null)
                    //{
                    //    eCCCSearchStatisticsResultModel.SummaryList.ElementAt(0).AnywhereTotalECSigned += details.REG_COUNT_AnywhereEC;
                    //    eCCCSearchStatisticsResultModel.SummaryList.ElementAt(0).LocalTotalECSigned += details.REG_COUNT_LocalEC;
                    //    eCCCSearchStatisticsResultModel.SummaryList.ElementAt(0).AnywhereTotalCCSigned += details.REG_COUNT_AnywhereCC;
                    //    eCCCSearchStatisticsResultModel.SummaryList.ElementAt(0).LocalTotalCCSigned += details.REG_COUNT_LocalCC;
                    //}
                    //else
                    //{
                    //    throw new Exception("No details recieved from KaigrSearch");
                    //}
                    return eCCCSearchStatisticsResultModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                ApiExceptionLogs.LogError(ex);
                throw;
            }
        }


        //Added by mayank date 14-7-20
        //to get sro list
        /// <summary>
        /// GetSroList
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns>ECCCSearchStatisticsViewModel</returns>
        public ECCCSearchStatisticsViewModel GetSroList(ECCCSearchStatisticsViewModel viewModel)
        {
            try
            {
                if (viewModel.DROCode != 0)
                    viewModel.SROfficeList = common.GetSroListOnDro(viewModel.DROCode);
                else
                {
                    viewModel.SROfficeList = new List<SelectListItem>();
                    viewModel.SROfficeList.Add(new SelectListItem { Value = "0", Text = "All" });
                }
                return viewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by mayank date 14-7-20
        //to get Detail table data
        /// <summary>
        /// GetDetails
        /// </summary>
        /// <param name="eCCCSearchStatisticsViewModel"></param>
        /// <returns>ECCCSearchStatisticsResultModel</returns>
        public ECCCSearchStatisticsResultModel GetDetails(ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel)
        {
            ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();
            objService.Timeout = 500000;
            int FinancialyearCode = 0;
            try
            {
                KaveriEntities dbcontext = new KaveriEntities();

                ECCCSearchStatisticsResultModel eCCCSearchStatisticsResultModel = new ECCCSearchStatisticsResultModel();
                eCCCSearchStatisticsResultModel.DetailsList = new List<ECCCSearchStatisticsDetailModel>();
                ECCCSearchStatisticsDetailModel eCCCSearchStatisticsDetailModel = null;

                DataSet Details = null;

                #region Commented by mayank on 05-07-2021
                //// ADDED BY SHUBHAM BHAGAT ON 22-03-2021
                //if (eCCCSearchStatisticsViewModel.MonthCode < 4 && eCCCSearchStatisticsViewModel.MonthCode != 0)
                //    FinancialyearCode  = eCCCSearchStatisticsViewModel.FinancialyearCode + 1;
                //else
                //    FinancialyearCode = eCCCSearchStatisticsViewModel.FinancialyearCode; 
                #endregion

                FinancialyearCode = eCCCSearchStatisticsViewModel.FinancialyearCode;
                //To get data from PreRegCCService

                // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020
                //Details = objService.GetECCCDetailsData(eCCCSearchStatisticsViewModel.SROCode, eCCCSearchStatisticsViewModel.MonthCode, eCCCSearchStatisticsViewModel.FinancialyearCode);
                //Details = objService.GetECCCDetailsData(eCCCSearchStatisticsViewModel.SROCode, eCCCSearchStatisticsViewModel.MonthCode, FinancialyearCode, eCCCSearchStatisticsViewModel.DROCode);
                bool isSearchDurationWise=eCCCSearchStatisticsViewModel.SearchBy == SearchBy.SearchDurationWise;
                Details = objService.GetECCCDetailsData(eCCCSearchStatisticsViewModel.SROCode, eCCCSearchStatisticsViewModel.MonthCode, FinancialyearCode, eCCCSearchStatisticsViewModel.DROCode, isSearchDurationWise);
                // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020

                if (Details != null)
                {

                    eCCCSearchStatisticsResultModel.TotalSummaryRecords = 1;
                    if (eCCCSearchStatisticsViewModel.DROCode != 0)
                        eCCCSearchStatisticsResultModel.DroName = (from dro in dbcontext.DistrictMaster where dro.DistrictCode == eCCCSearchStatisticsViewModel.DROCode select dro.DistrictNameE).FirstOrDefault();
                    else
                        eCCCSearchStatisticsResultModel.DroName = "All";
                    if (eCCCSearchStatisticsViewModel.SROCode != 0)
                        eCCCSearchStatisticsResultModel.SroName = (from dro in dbcontext.SROMaster where dro.SROCode == eCCCSearchStatisticsViewModel.SROCode select dro.SRONameE).FirstOrDefault();
                    else
                        eCCCSearchStatisticsResultModel.SroName = "All";
                    if (eCCCSearchStatisticsViewModel.FinancialyearCode != 0)
                        // BELOW CODE IS COMMENTED AND CHANGED ON 10-02-2021
                        eCCCSearchStatisticsResultModel.FinancialYearName = Convert.ToString(dbcontext.USP_FINANCIAL_YEAR().ToList().Where(m => m.YEAR == eCCCSearchStatisticsViewModel.FinancialyearCode).Select(m => m.FYEAR).FirstOrDefault());
                        // eCCCSearchStatisticsResultModel.FinancialYearName = eCCCSearchStatisticsViewModel.FinancialyearCode.ToString();
                    // ABOVE CODE IS COMMENTED AND CHANGED ON 10-02-2021

                    eCCCSearchStatisticsResultModel.MonthName = GetMonthName(eCCCSearchStatisticsViewModel.MonthCode);

                    int i = 1;

                    foreach (DataTable dataTable in Details.Tables)
                    {

                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            eCCCSearchStatisticsDetailModel = new ECCCSearchStatisticsDetailModel();
                            eCCCSearchStatisticsDetailModel.SrNo = i++;

                            if (eCCCSearchStatisticsViewModel.SearchBy == SearchBy.SearchDurationWise)
                            {
                                if (eCCCSearchStatisticsViewModel.MonthCode != 0)
                                {
                                    if (!(dataRow["SelDate"] == DBNull.Value))
                                        // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 22-12-2020
                                        //eCCCSearchStatisticsDetailModel.MonthYear = Convert.ToString(dataRow["SelDate"]);
                                        eCCCSearchStatisticsDetailModel.MonthYear = Convert.ToDateTime(Convert.ToString(dataRow["SelDate"])).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 22-12-2020
                                    else
                                        eCCCSearchStatisticsDetailModel.MonthYear = "--";
                                }
                                else
                                {
                                    if (!(dataRow["MonthName"] == DBNull.Value))
                                        eCCCSearchStatisticsDetailModel.MonthYear = Convert.ToString(dataRow["MonthName"]);
                                    else
                                        eCCCSearchStatisticsDetailModel.MonthYear = "--";
                                }
                            }
                            else
                            {
                                //if (eCCCSearchStatisticsViewModel.MonthCode == 0)
                                //{
                                //    if (!(dataRow["SRONAME"] == DBNull.Value))
                                //        eCCCSearchStatisticsDetailModel.MonthYear = Convert.ToString(dataRow["SRONAME"]);
                                //    else
                                //        eCCCSearchStatisticsDetailModel.MonthYear = "--";
                                //}
                                //else
                                //{
                                //    if (!(dataRow["OFFICENAME"] == DBNull.Value))
                                //        eCCCSearchStatisticsDetailModel.MonthYear = Convert.ToString(dataRow["OFFICENAME"]);
                                //    else
                                //        eCCCSearchStatisticsDetailModel.MonthYear = "--";
                                //}
                                if (!(dataRow["SRONAME"] == DBNull.Value))
                                    eCCCSearchStatisticsDetailModel.MonthYear = Convert.ToString(dataRow["SRONAME"]);
                                else
                                    eCCCSearchStatisticsDetailModel.MonthYear = "--";

                                if (!(dataRow["SROCODE"] == DBNull.Value))
                                    eCCCSearchStatisticsDetailModel.SroCode = Convert.ToInt32(dataRow["SROCODE"]);
                                else
                                    eCCCSearchStatisticsDetailModel.SroCode = 0;


                            }

                           

                            if (!(dataRow["KOS_USER_LOGGED"] == DBNull.Value))
                                eCCCSearchStatisticsDetailModel.TotalUserLogged = Convert.ToInt32(dataRow["KOS_USER_LOGGED"]);
                            else
                                eCCCSearchStatisticsDetailModel.TotalUserLogged = 0;

                            if (!(dataRow["KOS_EC_SEARCHED"] == DBNull.Value))
                                eCCCSearchStatisticsDetailModel.TotalECSearched = Convert.ToInt32(dataRow["KOS_EC_SEARCHED"]);
                            else
                                eCCCSearchStatisticsDetailModel.TotalECSearched = 0;

                            if (!(dataRow["KOS_EC_SIGNED"] == DBNull.Value))
                                eCCCSearchStatisticsDetailModel.TotalECSigned = Convert.ToInt32(dataRow["KOS_EC_SIGNED"]);
                            else
                                eCCCSearchStatisticsDetailModel.TotalECSigned = 0;

                            if (!(dataRow["KOS_EC_SUBMITTED"] == DBNull.Value))
                                eCCCSearchStatisticsDetailModel.TotalECSubmitted = Convert.ToInt32(dataRow["KOS_EC_SUBMITTED"]);
                            else
                                eCCCSearchStatisticsDetailModel.TotalECSubmitted = 0;

                            if (!(dataRow["KOS_CC_SEARCHED"] == DBNull.Value))
                                eCCCSearchStatisticsDetailModel.TotalCCSearched = Convert.ToInt32(dataRow["KOS_CC_SEARCHED"]);
                            else
                                eCCCSearchStatisticsDetailModel.TotalCCSearched = 0;

                            if (!(dataRow["KOS_CC_SIGNED"] == DBNull.Value))
                                eCCCSearchStatisticsDetailModel.TotalCCSigned = Convert.ToInt32(dataRow["KOS_CC_SIGNED"]);
                            else
                                eCCCSearchStatisticsDetailModel.TotalCCSigned = 0;

                            if (!(dataRow["KOS_CC_SUBMITTED"] == DBNull.Value))
                                eCCCSearchStatisticsDetailModel.TotalCCSubmitted = Convert.ToInt32(dataRow["KOS_CC_SUBMITTED"]);
                            else
                                eCCCSearchStatisticsDetailModel.TotalCCSubmitted = 0;

                            eCCCSearchStatisticsResultModel.DetailsList.Add(eCCCSearchStatisticsDetailModel);
                        }
                    }


                    if (eCCCSearchStatisticsViewModel.SearchBy == SearchBy.SearchDurationWise)
                    {
                        if (eCCCSearchStatisticsViewModel.MonthCode == 0)
                        {
                            //var kaigrSearchDetails = kaigrSearchDBContext.USP_RPT_OTH_ECCCCOUNT_DETAIL_MONTHWISE(eCCCSearchStatisticsViewModel.DROCode, eCCCSearchStatisticsViewModel.SROCode, FinancialyearCode).ToList();
                            var AnyWhereLocalECCCDetails = dbcontext.USP_RPT_ECCCCOUNT_OTH_DETAIL_MONTHWISE(eCCCSearchStatisticsViewModel.DROCode, eCCCSearchStatisticsViewModel.SROCode, FinancialyearCode).ToList();
                            if (AnyWhereLocalECCCDetails != null)
                            {
                                if (eCCCSearchStatisticsResultModel.DetailsList.Count == 0)
                                {
                                    eCCCSearchStatisticsResultModel.DetailsList = new List<ECCCSearchStatisticsDetailModel>();
                                    ECCCSearchStatisticsDetailModel defaulteCCCSearchStatisticsDetailModel = new ECCCSearchStatisticsDetailModel();
                                    defaulteCCCSearchStatisticsDetailModel.MonthYear = "---";
                                    eCCCSearchStatisticsResultModel.DetailsList.Add(defaulteCCCSearchStatisticsDetailModel);
                                }
                                if (AnyWhereLocalECCCDetails.Count == eCCCSearchStatisticsResultModel.DetailsList.Count)
                                {
                                    //for (int j = 0; j < AnyWhereLocalECCCDetails.Count; j++)
                                    //{
                                    //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).AnywhereTotalECSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_AnywhereEC;
                                    //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).AnywhereTotalCCSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_AnywhereCC;
                                    //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).LocalTotalECSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_LocalEC;
                                    //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).LocalTotalCCSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_LocalCC;

                                    //}
                                    ECCCSearchStatisticsDetailModel DetailModel = null;
                                    AnyWhereLocalECCCDetails.ForEach(m =>
                                    {
                                        DetailModel = eCCCSearchStatisticsResultModel.DetailsList.Where(t => t.MonthYear == m.MonthYear).FirstOrDefault();
                                        DetailModel.AnywhereTotalECSigned = m.REG_COUNT_AnywhereEC;
                                        DetailModel.AnywhereTotalCCSigned = m.REG_COUNT_AnywhereCC;
                                        DetailModel.LocalTotalECSigned = m.REG_COUNT_LocalEC;
                                        DetailModel.LocalTotalCCSigned = m.REG_COUNT_LocalCC;
                                    });
                                }
                                else if (eCCCSearchStatisticsResultModel.DetailsList.Count == 0)
                                {
                                    eCCCSearchStatisticsResultModel.DetailsList = new List<ECCCSearchStatisticsDetailModel>();
                                    eCCCSearchStatisticsResultModel.DetailsList.Add(new ECCCSearchStatisticsDetailModel());
                                }
                                else
                                {
                                    throw new Exception("Row mismatch found in KaigrOnline and KaigrSearch");
                                }
                            }
                            else
                            {
                                throw new Exception("Data not Received from SP");
                            }
                        }
                        else
                        {
                            //change to daywise
                            //var kaigrSearchDetails = kaigrSearchDBContext.USP_RPT_OTH_ECCCCOUNT_DETAIL_DAYWISE(eCCCSearchStatisticsViewModel.DROCode, eCCCSearchStatisticsViewModel.SROCode, FinancialyearCode,eCCCSearchStatisticsViewModel.MonthCode).ToList();
                            var AnyWhereLocalECCCDetails = dbcontext.USP_RPT_ECCCCOUNT_OTH_DETAIL_DAYWISE(eCCCSearchStatisticsViewModel.DROCode, eCCCSearchStatisticsViewModel.SROCode, FinancialyearCode, eCCCSearchStatisticsViewModel.MonthCode).ToList();
                            if (AnyWhereLocalECCCDetails != null)
                            {
                                if (eCCCSearchStatisticsResultModel.DetailsList.Count == 0)
                                {
                                    eCCCSearchStatisticsResultModel.DetailsList = new List<ECCCSearchStatisticsDetailModel>();
                                    ECCCSearchStatisticsDetailModel defaulteCCCSearchStatisticsDetailModel = new ECCCSearchStatisticsDetailModel();
                                    defaulteCCCSearchStatisticsDetailModel.MonthYear = "---";
                                    eCCCSearchStatisticsResultModel.DetailsList.Add(defaulteCCCSearchStatisticsDetailModel);
                                }
                                if (AnyWhereLocalECCCDetails.Count == eCCCSearchStatisticsResultModel.DetailsList.Count)
                                {
                                    //for (int j = 0; j < AnyWhereLocalECCCDetails.Count; j++)
                                    //{
                                    //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).AnywhereTotalECSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_AnywhereEC;
                                    //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).AnywhereTotalCCSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_AnywhereCC;
                                    //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).LocalTotalECSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_LocalEC;
                                    //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).LocalTotalCCSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_LocalCC;
                                    //}
                                    ECCCSearchStatisticsDetailModel DetailModel = null;
                                    AnyWhereLocalECCCDetails.ForEach(m =>
                                    {
                                       m.MonthYear= Convert.ToDateTime(Convert.ToString(m.MonthYear)).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                        DetailModel = eCCCSearchStatisticsResultModel.DetailsList.Where(t => t.MonthYear == m.MonthYear).FirstOrDefault();
                                        DetailModel.AnywhereTotalECSigned = m.REG_COUNT_AnywhereEC;
                                        DetailModel.AnywhereTotalCCSigned = m.REG_COUNT_AnywhereCC;
                                        DetailModel.LocalTotalECSigned = m.REG_COUNT_LocalEC;
                                        DetailModel.LocalTotalCCSigned = m.REG_COUNT_LocalCC;
                                    });
                                }

                                else
                                {
                                    throw new Exception("Row mismatch found in KaigrOnline and KaigrSearch");
                                }
                            }
                            else
                            {
                                throw new Exception("Data not Received from SP");
                            }
                        }
                    }
                    else
                    {
                        var AnyWhereLocalECCCDetails = dbcontext.USP_RPT_ECCCCOUNT_OTH_DETAIL_OFFICEWISE(eCCCSearchStatisticsViewModel.DROCode, eCCCSearchStatisticsViewModel.SROCode, FinancialyearCode, eCCCSearchStatisticsViewModel.MonthCode).ToList();
                        if (AnyWhereLocalECCCDetails != null)
                        {
                            if (eCCCSearchStatisticsResultModel.DetailsList.Count == 0)
                            {
                                eCCCSearchStatisticsResultModel.DetailsList = new List<ECCCSearchStatisticsDetailModel>();
                                ECCCSearchStatisticsDetailModel defaulteCCCSearchStatisticsDetailModel = new ECCCSearchStatisticsDetailModel();
                                defaulteCCCSearchStatisticsDetailModel.MonthYear = "---";
                                eCCCSearchStatisticsResultModel.DetailsList.Add(defaulteCCCSearchStatisticsDetailModel);
                            }
                            if (AnyWhereLocalECCCDetails.Count == eCCCSearchStatisticsResultModel.DetailsList.Count)
                            {
                                //for (int j = 0; j < AnyWhereLocalECCCDetails.Count; j++)
                                //{
                                //    //eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).TotalUserLogged = "--";
                                //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).AnywhereTotalECSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_AnywhereEC;
                                //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).AnywhereTotalCCSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_AnywhereCC;
                                //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).LocalTotalECSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_LocalEC;
                                //    eCCCSearchStatisticsResultModel.DetailsList.ElementAt(j).LocalTotalCCSigned = AnyWhereLocalECCCDetails.ElementAt(j).REG_COUNT_LocalCC;
                                //}
                                ECCCSearchStatisticsDetailModel DetailModel = null;
                                AnyWhereLocalECCCDetails.ForEach(m =>
                                {
                                    DetailModel = eCCCSearchStatisticsResultModel.DetailsList.Where(t => t.SroCode == m.SROCode).FirstOrDefault();
                                    DetailModel.AnywhereTotalECSigned = m.REG_COUNT_AnywhereEC;
                                    DetailModel.AnywhereTotalCCSigned = m.REG_COUNT_AnywhereCC;
                                    DetailModel.LocalTotalECSigned = m.REG_COUNT_LocalEC;
                                    DetailModel.LocalTotalCCSigned = m.REG_COUNT_LocalCC;
                                });
                            }

                            else
                            {
                                throw new Exception("Row mismatch found in KaigrOnline and KaigrSearch");
                            }
                        }
                        else
                        {
                            throw new Exception("Data not Received from SP");
                        }
                    }
                    eCCCSearchStatisticsResultModel.TotalDetailRecords = eCCCSearchStatisticsResultModel.DetailsList.Count;

                    return eCCCSearchStatisticsResultModel;
                }
                return null;

            }
            catch (Exception ex)
            {
                ApiExceptionLogs.LogError(ex);
                throw;
            }

        }

        //Added by mayank date 14-7-20
        //to get month name on month code
        /// <summary>
        /// GetMonthName
        /// </summary>
        /// <param name="monthCode"></param>
        /// <returns>string</returns>
        public string GetMonthName(int monthCode)
        {
            try
            {
                List<string> monthList = new List<string>();
                monthList.Add("All");
                monthList.Add("January");
                monthList.Add("February");
                monthList.Add("March");
                monthList.Add("April");
                monthList.Add("May");
                monthList.Add("June");
                monthList.Add("July");
                monthList.Add("August");
                monthList.Add("September");
                monthList.Add("October");
                monthList.Add("November");
                monthList.Add("December");
                return monthList.ElementAt(monthCode);
            }
            catch (Exception ex)
            {
                ApiExceptionLogs.LogError(ex);
                throw;
            }
        }

        //Added by mayank date 15-7-20
        //to get Summary and Detail table data for excel
        /// <summary>
        /// GetDetails
        /// </summary>
        /// <param name="eCCCSearchStatisticsViewModel"></param>
        /// <returns>ECCCSearchStatisticsResultModel</returns>
        public ECCCSearchStatisticsResultModel GetSummaryDetailsforExcel(ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel)
        {
            try
            {
                ECCCSearchStatisticsResultModel ResModel = new ECCCSearchStatisticsResultModel();
                //commeneted by mayank on 24-06-2021
                //ECCCSearchStatisticsResultModel SummaryModel = GetSummary(eCCCSearchStatisticsViewModel);
                ECCCSearchStatisticsResultModel DetailModel = GetDetails(eCCCSearchStatisticsViewModel);
                ResModel.SummaryList = DetailModel.SummaryList;
                ResModel.DetailsList = DetailModel.DetailsList;
                ResModel.SroName = DetailModel.SroName;
                ResModel.DroName = DetailModel.DroName;
                ResModel.FinancialYearName = DetailModel.FinancialYearName;
                ResModel.MonthName = DetailModel.MonthName;
                //ResModel.TotalSummaryRecords = SummaryModel.TotalSummaryRecords;
                ResModel.TotalDetailRecords = DetailModel.TotalDetailRecords;
                return ResModel;
            }
            catch (Exception ex)
            {
                ApiExceptionLogs.LogError(ex);
                throw;
            }

        }
    }
}