using CustomModels.Models.MISReports.ESignConsumptionReport;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Entity.KaveriEntities;
using ECDataUI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ESignConsumptionReportDAL : IESignConsumptionReport
    {
        public ESignConsumptionReportViewModel ESignConsumptionReportView()
        {
            try
            {
                ESignConsumptionReportViewModel viewModel = new ESignConsumptionReportViewModel();

                //var startDate = new DateTime(now.Year, now.Month, now.Day);
                //model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //model.ToDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                viewModel.FinancialYearList = getFinancialYearList();

                DateTime currentDate = DateTime.Now;
                var startDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                viewModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                viewModel.ToDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);



                return viewModel;

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public ESignTotalConsumptionResModel GetTotalESignConsumedCount(ESignConsumptionReportViewModel requestModel)
        {
            ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();

            try
            {
                //DateTime fromDate = Convert.ToDateTime(requestModel.FromDate);
                //DateTime toDate = Convert.ToDateTime(requestModel.ToDate);
                string applicationStatusType = string.Empty;           //This is set as empty string to get data for both methods from single method in PreRegAppDtlsServ

                ESignTotalConsumptionResModel resultModel = new ESignTotalConsumptionResModel();
                ESignConsumptionMonthWiseTableModel monthWiseESignCountList = null;
                objService.Timeout = 300000;  //5Min
                //DataSet eSignTotalConsumptionCountList = objService.GetESignConsumptionDetails(fromDate, toDate, applicationStatusType);
                DataSet eSignTotalConsumptionCountList = objService.GetESignConsumptionDetails(Convert.ToDateTime(requestModel.FromDate), Convert.ToDateTime(requestModel.ToDate), requestModel.FinancialYearCode, applicationStatusType);
                //Here second and third are of no use as in Serivce in if condition it is not used

                resultModel.ESignConsumptionMonthWiseTableList = new List<ESignConsumptionMonthWiseTableModel>();

                int totalESigncountForEC = 0;       //for financial year sum
                int totalESigncountForCC = 0;       //for financial year sum
                int totalESignCountForECCCFinYear = 0;

                foreach (DataTable dataTable in eSignTotalConsumptionCountList.Tables)
                {
                    foreach(DataRow dataRow in dataTable.Rows)
                    {
                        int totalESignCountForECCC = 0;     //for monthly sum
                        

                        monthWiseESignCountList = new ESignConsumptionMonthWiseTableModel();


                        if (!(dataRow["MonthName"] == DBNull.Value))
                        {
                            //monthWiseESignCountList.MonthAndYear = Convert.ToString(dataRow["MonthName"]);
                            //if (!requestModel.IsExcel && (requestModel.CurrentRoleID == Convert.ToInt32(CommonEnum.RoleDetails.TechnicalAdmin)))
                            //    monthWiseESignCountList.MonthAndYear = "<a href='javascript:void(0)' onClick='LoadMonthWiseDatatable(" + Convert.ToString(dataRow["MONTHID"]) + ", " + Convert.ToString(dataRow["Year"]) + ")'> " + Convert.ToString(dataRow["MonthName"]) + "</a>";
                            //else
                                monthWiseESignCountList.MonthAndYear = Convert.ToString(dataRow["MonthName"]);
                        }

                        if (!(dataRow["TotalESignConsumedForEC"] == DBNull.Value))
                        {
                            monthWiseESignCountList.TotalESignConsumedForEC = Convert.ToString(dataRow["TotalESignConsumedForEC"]);
                            totalESigncountForEC = totalESigncountForEC + Convert.ToInt32(dataRow["TotalESignConsumedForEC"]);
                            totalESignCountForECCC = totalESignCountForECCC + Convert.ToInt32(monthWiseESignCountList.TotalESignConsumedForEC);
                        }

                        if (!(dataRow["TotalESignConsumedForCC"] == DBNull.Value))
                        {
                            monthWiseESignCountList.TotalESignConsumedForCC = Convert.ToString(dataRow["TotalESignConsumedForCC"]);
                            totalESigncountForCC = totalESigncountForCC + Convert.ToInt32(dataRow["TotalESignConsumedForCC"]);
                            totalESignCountForECCC = totalESignCountForECCC + Convert.ToInt32(monthWiseESignCountList.TotalESignConsumedForCC);
                        }

                        monthWiseESignCountList.TotalESignConsumedMonthlyForECCC = totalESignCountForECCC;


                        totalESignCountForECCCFinYear = totalESignCountForECCCFinYear + totalESignCountForECCC;
                        resultModel.ESignConsumptionMonthWiseTableList.Add(monthWiseESignCountList);
                    }

                }
                resultModel.TotalESignConsumedForEC = totalESigncountForEC;
                resultModel.TotalESignConsumedForCC = totalESigncountForCC;
                resultModel.TotalESignConsumedForFinYear = totalESignCountForECCCFinYear;

                return resultModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ESignStatusDetailsResModel LoadESignDetailsDataTable(ESignConsumptionReportViewModel requestModel)
        {

            ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();

            try
            {
                DateTime fromDate = Convert.ToDateTime(requestModel.FromDate);
                DateTime toDate = Convert.ToDateTime(requestModel.ToDate);

                string applicationStatusType = requestModel.ApplicationStatusTypeID.ToString();

                ESignStatusDetailsResModel resultModel = new ESignStatusDetailsResModel();
                ESignStatusDetailsTableModel tableResultList = null;
                objService.Timeout = 300000;  //5 Min
                //DataSet eSignDetailsList = objService.GetESignConsumptionDetails(fromDate, toDate, applicationStatusType);
                //DataSet eSignDetailsList = objService.GetESignConsumptionDetails(0, requestModel.MonthCode, requestModel.FinancialYearCode, applicationStatusType, requestModel.StartLength, requestModel.TotalNum);  
                DataSet eSignDetailsList = objService.GetESignConsumptionDetails(Convert.ToDateTime(requestModel.FromDate), Convert.ToDateTime(requestModel.ToDate), requestModel.FinancialYearCode, applicationStatusType);
                //Here first parameter is of no use as in service in else condition it is not used


                resultModel.ESignStatusDetailsTableList = new List<ESignStatusDetailsTableModel>();

                int serialNoCounter = 1;

                foreach (DataTable dataTable in eSignDetailsList.Tables)
                {
                    foreach(DataRow dataRow in dataTable.Rows)
                    {
                        tableResultList = new ESignStatusDetailsTableModel();
                        tableResultList.SerialNo = serialNoCounter++;

                        if (!(dataRow["ApplicationNumber"] == DBNull.Value))
                        {
                            tableResultList.ApplicationNumber = Convert.ToString(dataRow["ApplicationNumber"]);
                        }
                        else
                        {
                            tableResultList.ApplicationNumber = "--";
                        }

                        if (!(dataRow["ApplicationType"] == DBNull.Value))
                        {
                            tableResultList.ApplicationType = Convert.ToString(dataRow["ApplicationType"]);
                        }
                        else
                        {
                            tableResultList.ApplicationType = "--";
                        }

                        if (!(dataRow["ApplicationDateTime"] == DBNull.Value))
                        {
                            tableResultList.ApplicationDate = Convert.ToString(dataRow["ApplicationDateTime"]);
                        }
                        else
                        {
                            tableResultList.ApplicationDate = "--";
                        }

                        if (!(dataRow["ESignReqDateTime"] == DBNull.Value))
                        {
                            tableResultList.ESignRequestDate = Convert.ToString(dataRow["ESignReqDateTime"]);
                        }
                        else
                        {
                            tableResultList.ESignRequestDate = "--";
                        }

                        if (!(dataRow["ESignReqTranNo"] == DBNull.Value))
                        {
                            tableResultList.ESignRequestTransactionNo = Convert.ToString(dataRow["ESignReqTranNo"]);
                        }
                        else
                        {
                            tableResultList.ESignRequestTransactionNo = "--";
                        }

                        if (!(dataRow["ESignRespDateTime"] == DBNull.Value))
                        {
                            tableResultList.ESignResponseDate = Convert.ToString(dataRow["ESignRespDateTime"]);
                        }
                        else
                        {
                            tableResultList.ESignResponseDate = "--";
                        }

                        if (!(dataRow["ESignRespTranNo"] == DBNull.Value))
                        {
                            tableResultList.ESignResponseTransactionNo = Convert.ToString(dataRow["ESignRespTranNo"]);
                        }
                        else
                        {
                            tableResultList.ESignResponseTransactionNo = "--";
                        }

                        if (!(dataRow["ESignRespCode"] == DBNull.Value))
                        {
                            tableResultList.ESignResponseCode = Convert.ToString(dataRow["ESignRespCode"]);
                        }
                        else
                        {
                            tableResultList.ESignResponseCode = "--";
                        }
                        if (!(dataRow["ESignRespStatus"] == DBNull.Value))
                        {
                            tableResultList.Status = Convert.ToString(dataRow["ESignRespStatus"]);
                        }
                        else
                        {
                            tableResultList.Status = "--";
                        }

                        if (!(dataRow["ApplicationStatus"] == DBNull.Value))
                        {
                            tableResultList.ApplicationStatus = Convert.ToString(dataRow["ApplicationStatus"]);
                        }
                        else
                        {
                            //tableResultList.ApplicationStatus = "--";
                            tableResultList.ApplicationStatus = "Document is not yet submitted.";
                        }

                        if (!(dataRow["SubmittedDateTime"] == DBNull.Value))
                        {
                            tableResultList.ApplicationSubmitDate = Convert.ToString(dataRow["SubmittedDateTime"]);
                        }
                        else
                        {
                            tableResultList.ApplicationSubmitDate = "--";
                        }

                        if (!(dataRow["RespErrorCode"] == DBNull.Value))
                        {
                            tableResultList.ResponseErrorCode = Convert.ToString(dataRow["RespErrorCode"]);
                        }
                        else
                        {
                            tableResultList.ResponseErrorCode = "--";
                        }

                        if (!(dataRow["RespErrorMessage"] == DBNull.Value))
                        {
                            tableResultList.ResponseErrorMessage = Convert.ToString(dataRow["RespErrorMessage"]);
                        }
                        else
                        {
                            tableResultList.ResponseErrorMessage = "--";
                        }

                        resultModel.ESignStatusDetailsTableList.Add(tableResultList);
                    }
                }


                resultModel.TotalCount = resultModel.ESignStatusDetailsTableList.Count();

                //tableResultList = new ESignStatusDetailsTableModel();
                //tableResultList.SerialNo = 1;
                //tableResultList.ApplicationNumber = "EC-123";
                //tableResultList.ApplicationType = "Online EC";
                //tableResultList.ApplicationDate = DateTime.Now.ToString();
                //tableResultList.ESignRequestDate = DateTime.Now.ToString();
                //tableResultList.ESignRequestTransactionNo = "ReqTran-1";
                //tableResultList.ESignResponseDate = DateTime.Now.ToString();
                //tableResultList.ESignResponseTransactionNo = "RespTran-1";
                //tableResultList.ESignResponseCode = "defr123";
                //tableResultList.Status = "Success";
                //tableResultList.ApplicationStatus = "Submitted";
                //tableResultList.ApplicationSubmitDate = DateTime.Now.ToString();
                //tableResultList.ResponseErrorCode = "NA";
                //tableResultList.ResponseErrorMessage = "NA";

                //resultModel.ESignStatusDetailsTableList.Add(tableResultList);

                //resultModel.TotalCount = resultModel.ESignStatusDetailsTableList.Count();

                return resultModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        
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
                            if (item.YEAR > 2019)
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

    }
}