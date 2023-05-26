#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   ECCCSearchStatisticsBAL.cs
    * Author Name       :   Mayank Wankhede
    * Creation Date     :   14-07-2020
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   EC/CC search statistics BAL
*/
#endregion
using CustomModels.Models.MISReports.ECCCSearchStatistics;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ECCCSearchStatisticsBAL : IECCCSearchStatistics
    {
        IECCCSearchStatistics dalobj = new ECCCSearchStatisticsDAL();

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
                return dalobj.ECCCSearchStatisticsView(OfficeId);
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
            try
            {
                return dalobj.GetDetails(eCCCSearchStatisticsViewModel);
            }
            catch (Exception)
            {

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
                return dalobj.GetSroList(viewModel);
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
            try
            {
                return dalobj.GetSummary(eCCCSearchStatisticsViewModel);
            }
            catch (Exception)
            {

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
                return dalobj.GetSummaryDetailsforExcel(eCCCSearchStatisticsViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}