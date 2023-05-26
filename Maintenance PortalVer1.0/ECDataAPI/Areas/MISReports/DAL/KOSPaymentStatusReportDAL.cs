#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   KOSPaymentStatusReportDAL.cs
    * Author Name       :   Omkar Chitnis
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for KOS Payment Status Report module.
*/
#endregion


using CustomModels.Models.Common;
using CustomModels.Models.MISReports.KOSPaymentStatusReport;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using ECDataAPI.Entity;
using ECDataUI.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Security;
using ECDataUI.Common;
using CustomModels.Common;
using System.Data;
//using ECDataAPI.Entity.KaigrOnlineEntities;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class KOSPaymentStatusReportDAL : IKOSPaymentStatusReport
    {

        KaveriEntities dbContext = null;
        //KAIGR_ONLINEEntities kAIGR_Onilne_etities = null;
        int status = -1;

        public KOSPaymentStatusRptViewModel KOSPaymentStatusReportReportView(int OfficeID)
        {
            try
            {
                KOSPaymentStatusRptViewModel model = new KOSPaymentStatusRptViewModel();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                string FirstRecord = "All";
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, now.Day);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                using (dbContext=new KaveriEntities())
                {

                    short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                    int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                    string kaveriCode = Kaveri1Code.ToString();

                    model.SROfficeList = new List<SelectListItem>();
                    model.DROfficeList = new List<SelectListItem>();

                    if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                    {

                        string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                        int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                        string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                        string DroCode_string = Convert.ToString(DroCode);

                        sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                        droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                        model.DROfficeList.Add(droNameItem);
                        model.SROfficeList.Add(sroNameItem);

                    }
                    else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                    {
                        string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                        string DroCode_string = Convert.ToString(Kaveri1Code);
                        droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                        model.DROfficeList.Add(droNameItem);
                        model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, FirstRecord);
                    }
                    else
                    {

                        SelectListItem select = new SelectListItem();
                        select.Text = "All";
                        select.Value = "0";
                        model.SROfficeList.Add(select);
                        model.DROfficeList = objCommon.GetDROfficesList("All");

                    } 
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //if (dbContext != null && kAIGR_Onilne_etities != null)
                //{
                //    kAIGR_Onilne_etities.Dispose();
                //    dbContext.Dispose();

                //}
                if (dbContext != null)
                {
                    
                    dbContext.Dispose();

                }

            }
        }

        //Added by Madhusoodan on 31/07/2020
        //To get data from PreRegCCService method: KOSPaymentStatusList
        public KOSPaymentStatusRptResModel KOSPaymentStatusReportDetails(KOSPaymentStatusRptViewModel model)
        {
            ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();

            try
            {
                dbContext = new KaveriEntities();
                KOSPaymentStatusRptResModel resModel = new KOSPaymentStatusRptResModel();
                KOSPaymentStatusDetailsModel ReportsDetails = null;
                resModel.KOSPaymentStatusDetailsList = new List<KOSPaymentStatusDetailsModel>();

                //dbContext = new KaveriEntities();
                using (dbContext = new KaveriEntities())
                {
                    if (model.DROfficeID != 0)
                        resModel.DistrictName = dbContext.DistrictMaster.Where(m => m.DistrictCode == model.DROfficeID).Select(m => m.DistrictNameE).FirstOrDefault();
                    else
                        resModel.DistrictName = "All";

                    if (model.SROfficeID != 0)
                        resModel.SROName = dbContext.SROMaster.Where(m => m.SROCode == model.SROfficeID).Select(m => m.SRONameE).FirstOrDefault();
                    else
                        resModel.SROName = "All";
                }
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                int ApplicationTypeId = model.ApplicationTypeId;
                int counter = (model.startLen + 1); //To start Serial Number 

                DataSet KOSPaymentStatusList = objService.KOSPaymentStatusList(model.ApplicationTypeId, FromDate, ToDate,model.DROfficeID,model.SROfficeID);

                resModel.TotalCount = KOSPaymentStatusList.Tables.Count;

                resModel.KOSPaymentStatusDetailsList = new List<KOSPaymentStatusDetailsModel>();


                foreach (DataTable dataTable in KOSPaymentStatusList.Tables)
                {
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        ReportsDetails = new KOSPaymentStatusDetailsModel();

                        ReportsDetails.SerialNo = counter++;

                        #region Application Type
                        //For Application Type
                        if (model.ApplicationTypeId == 1)
                        {
                            ReportsDetails.ApplicationTypeId = " EC Application";
                        }
                        else if (model.ApplicationTypeId == 2)
                        {
                            ReportsDetails.ApplicationTypeId = " CC Application";

                        }
                        else if (model.ApplicationTypeId == 3)
                        {
                            ReportsDetails.ApplicationTypeId = " Document Registration";

                        }
                        else if (model.ApplicationTypeId == 4)
                        {
                            ReportsDetails.ApplicationTypeId = " Direct Payment";

                        }
                        else
                        {
                            ReportsDetails.ApplicationTypeId = "All";
                        }
                        #endregion

                        #region Assign values if not null

                        if (!(dataRow["SUBMITTED"] == DBNull.Value))
                        {
                            ReportsDetails.TotalPaymentReqSubmitted = Convert.ToString(dataRow["SUBMITTED"]);
                        }
                        else
                        {
                            ReportsDetails.TotalPaymentReqSubmitted = "--";
                        }
                        
                        if (!(dataRow["SUCCEED"] == DBNull.Value))
                        {
                            ReportsDetails.TotalPaymentsSuccessful = Convert.ToInt32(dataRow["SUCCEED"]) + " (" + (dataRow["SUCCEED_PERCENT"]) + "%" + ")";
                        }
                        else
                        {
                            ReportsDetails.TotalPaymentsSuccessful = "--";
                        }


                        if (!(dataRow["FAILED"] == DBNull.Value))
                        {                            
                            ReportsDetails.TotalPaymentsFailed = Convert.ToInt32(dataRow["FAILED"]) + "(" + (dataRow["FAILED_PERCENT"]) + "%" + ")";
                        }
                        else
                        {
                            ReportsDetails.TotalPaymentsFailed = "--";
                        }

                        if (!(dataRow["EXPIRED"] == DBNull.Value))
                        {
                            ReportsDetails.TotalPaymentsExpired = Convert.ToInt32(dataRow["EXPIRED"]) + " (" + (dataRow["EXPIRED_PERCENT"]) + "%" + ")";
                        }
                        else
                        {
                            ReportsDetails.TotalPaymentsExpired = "--";
                        }


                        if (!(dataRow["PENDING"] == DBNull.Value))
                        {
                            ReportsDetails.TotalPaymentsPending = Convert.ToInt32(dataRow["PENDING"]) + " (" + (dataRow["PENDING_PERCENT"]) + "%" + ")";
                        }
                        else
                        {
                            ReportsDetails.TotalPaymentsPending = "--";
                        }

                        if (!(dataRow["PAYMENT_PENDING_MORETHAN_THREEDAYS"] == DBNull.Value))
                        {
                            ReportsDetails.PaymentPendingSince = Convert.ToString(dataRow["PAYMENT_PENDING_MORETHAN_THREEDAYS"]);
                        }
                        else
                        {
                            ReportsDetails.PaymentPendingSince = "--";
                        }


                        if (!(dataRow["EARLIEST_PAYMENT_PENDING_SINCE"] == DBNull.Value))
                        {
                            ReportsDetails.LongestPaymentPendingSince = Convert.ToDateTime(dataRow["EARLIEST_PAYMENT_PENDING_SINCE"]).ToString("dd/MM/yyyy") + " (" + (dataRow["DAYS"]) + "  Days" + ")";
                            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-12-2020 
                            ReportsDetails.LongestPaymentPendingSinceDays = (dataRow["DAYS"].ToString());
                            ReportsDetails.LongestPaymentPendingSinceDate = Convert.ToDateTime(dataRow["EARLIEST_PAYMENT_PENDING_SINCE"]).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);                                                       
                            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-12-2020 
                        }
                        else
                        {
                            ReportsDetails.LongestPaymentPendingSince = "--";
                        }


                        #region ADDED BY SHUBHAM BHAGAT ON 11-12-2020
                        if (!(dataRow["NO_STATUS"] == DBNull.Value))
                        {
                            ReportsDetails.PaymentWithNoStatus = Convert.ToInt32(dataRow["NO_STATUS"]) + " (" + (dataRow["NO_STATUS_PERCENT"]) + "%" + ")";
                        }
                        else
                        {
                            ReportsDetails.PaymentWithNoStatus = "--";
                        }
                        #endregion

                        #region Average Payment Realization
                        // Logic To get AvgPaymentRealizationSpan


                        if (!(dataRow["AVG_PAYMENT_SPAN"] == DBNull.Value))
                        {
                            int? minsdiff = Convert.ToInt32(dataRow["AVG_PAYMENT_SPAN"]);

                            if (minsdiff < 60)
                            {
                                ReportsDetails.AvgPaymentRealizationSpan = minsdiff + " Minute(s)";
                            }
                            else if (minsdiff > 60 && minsdiff < (60 * 24))
                            {
                                ReportsDetails.AvgPaymentRealizationSpan = ((minsdiff / 60 + " Hour(s)"));
                            }
                            else if (minsdiff > (60 * 24))
                            {
                                ReportsDetails.AvgPaymentRealizationSpan = (minsdiff / (60 * 24) + " Day(s)");
                            }
                        }
                        else
                        {
                            ReportsDetails.AvgPaymentRealizationSpan = "--";

                        }
                        #endregion

                        #endregion

                        resModel.KOSPaymentStatusDetailsList.Add(ReportsDetails);

                    }
                }
                return resModel;
                
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //if (dbContext != null && kAIGR_Onilne_etities != null)
                //{
                //    dbContext.Dispose();
                //}
                if (dbContext != null )
                {
                    dbContext.Dispose();
                }

            }
            
        }
        //Addition ends here

        //Added by Madhusoodan on 31/07/2020  
        //To get data from PreRegCCService method: LoadKOSPaymentStatusDataTable
        public KOSPaymentStatusRptResTableModel LoadKOSPaymentStatusReportDataTable(KOSPaymentStatusRptViewModel model)
        {
            ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();

            try
            {

                dbContext = new KaveriEntities();
                KOSPaymentStatusRptResTableModel resModel = new KOSPaymentStatusRptResTableModel();
                KOSPaymentStatusDetailsTableModel ReportsDetails = null;
                resModel.KOSPaymentStatusDetailsTableList = new List<KOSPaymentStatusDetailsTableModel>();

                using (dbContext = new KaveriEntities())
                {
                    if (model.DROfficeID != 0)
                        resModel.DistrictName = dbContext.DistrictMaster.Where(m => m.DistrictCode == model.DROfficeID).Select(m => m.DistrictNameE).FirstOrDefault();
                    else
                        resModel.DistrictName = "All";

                    if (model.SROfficeID != 0)
                        resModel.SROName = dbContext.SROMaster.Where(m => m.SROCode == model.SROfficeID).Select(m => m.SRONameE).FirstOrDefault();
                    else
                        resModel.SROName = "All";
                }

                    DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                int ApplicationTypeId = model.ApplicationTypeId;
                status = model.status;
                int counter = (model.startLen + 1); //To start Serial Number 

                //LoadKOSPaymentStatusDataTable
                // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
                // BELOW CODE IS COMMENTED AND ADDED
                // DataSet KOSPaymentStatusList = objService.LoadKOSPaymentStatusDataTable(model.ApplicationTypeId, FromDate, ToDate, status);
                DataSet KOSPaymentStatusList = objService.LoadKOSPaymentStatusDataTable(model.ApplicationTypeId, FromDate, ToDate, status, Convert.ToInt32(model.Days),model.DROfficeID,model.SROfficeID);
                // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
                resModel.KOSPaymentStatusDetailsTableList = new List<KOSPaymentStatusDetailsTableModel>();

                foreach (DataTable dataTable in KOSPaymentStatusList.Tables)
                {
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        ReportsDetails = new KOSPaymentStatusDetailsTableModel();
                        ReportsDetails.SerialNo = counter++;

                        #region Assign values if not null

                        if (!(dataRow["OFFICE_NAME"] == DBNull.Value))
                        {
                            ReportsDetails.OfficeName = Convert.ToString(dataRow["OFFICE_NAME"]);
                        }
                        else
                        {
                            ReportsDetails.OfficeName = "--";
                        }

                        if (!(dataRow["APPLICATION_TYPE"] == DBNull.Value))
                        {
                            ReportsDetails.ApplicationType =  Convert.ToString(dataRow["APPLICATION_TYPE"]);
                        }
                        else
                        {
                            ReportsDetails.ApplicationType = "--";
                        }

                        if (!(dataRow["APPLICATION_NUMBER"] == DBNull.Value))
                        {
                            ReportsDetails.ApplicationNumber = Convert.ToString(dataRow["APPLICATION_NUMBER"]);
                        }
                        else
                        {
                            ReportsDetails.ApplicationNumber = "--";
                        }

                        if (!(dataRow["TransactionDateTime"] == DBNull.Value))
                        {
                            ReportsDetails.TransactionDate = Convert.ToString(dataRow["TransactionDateTime"]);
                        }
                        else
                        {
                            ReportsDetails.TransactionDate = "--";
                        }

                        if (!(dataRow["CHALLAN_REF_NUMBER"] == DBNull.Value))
                        {
                            ReportsDetails.ChallanReferencenNumber = Convert.ToString(dataRow["CHALLAN_REF_NUMBER"]);
                        }
                        else
                        {
                            ReportsDetails.ChallanReferencenNumber = "--";
                        }
                         
                        if (!(dataRow["PAYMENT_STATUS"] == DBNull.Value))
                        {
                            ReportsDetails.PaymentStatus = Convert.ToString(dataRow["PAYMENT_STATUS"]);
                        }
                        else
                        {
                            ReportsDetails.PaymentStatus = "--";
                        }


                        #region Payment Reazlied
                        // Logic To get PaymentRealized

                        if (!(dataRow["MINSDIFF"] == DBNull.Value))
                        {
                            int? minsdiff = Convert.ToInt32(dataRow["MINSDIFF"]);

                            if (minsdiff < 60)
                            {
                                ReportsDetails.PaymentRealizedInDays = minsdiff + " Minute(s)";
                            }
                            else if (minsdiff > 60 && minsdiff < (60 * 24))
                            {
                                ReportsDetails.PaymentRealizedInDays = (minsdiff / 60 + " Hour(s)");
                            }
                            else if (minsdiff > (60 * 24))
                            {
                                ReportsDetails.PaymentRealizedInDays = (minsdiff / (60 * 24) + " Day(s)");
                            }
                        }
                        else
                        {
                            ReportsDetails.PaymentRealizedInDays = "--";
                        }

                        #endregion

                        #endregion

                        resModel.KOSPaymentStatusDetailsTableList.Add(ReportsDetails);


                    }
                }
                resModel.TotalCount = resModel.KOSPaymentStatusDetailsTableList.Count;
                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }

            }

        }

        //Added by Madhusoodan on 31/07/2020
        //To get data from PreRegCCService method: PendingPaymentStatus
        public KOSPaymentStatusRptResModel PaymentPendingSince(KOSPaymentStatusRptViewModel model)
        {
            ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService objService = new PreRegApplicationDetailsService.ApplicationDetailsService();

            try
            {
                dbContext = new KaveriEntities();
                KOSPaymentStatusRptResModel resModel = new KOSPaymentStatusRptResModel();
                KOSPaymentStatusDetailsModel ReportsDetails = null;
              
                resModel.KOSPaymentStatusDetailsList = new List<KOSPaymentStatusDetailsModel>();

                //dbContext = new KaveriEntities();
                using (dbContext = new KaveriEntities())
                {
                    if (model.DROfficeID != 0)
                        resModel.DistrictName = dbContext.DistrictMaster.Where(m => m.DistrictCode == model.DROfficeID).Select(m => m.DistrictNameE).FirstOrDefault();
                    else
                        resModel.DistrictName = "All";

                    if (model.SROfficeID != 0)
                        resModel.SROName = dbContext.SROMaster.Where(m => m.SROCode == model.SROfficeID).Select(m => m.SRONameE).FirstOrDefault();
                    else
                        resModel.SROName = "All";
                }
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                int ApplicationTypeId = model.ApplicationTypeId;
                int days = model.paymentPendingsince;
                int counter = (model.startLen + 1); //To start Serial Number 

                DataSet result = objService.PendingPaymentStatus(model.ApplicationTypeId, FromDate, ToDate, days,model.DROfficeID,model.SROfficeID);

                resModel.KOSPaymentStatusDetailsList = new List<KOSPaymentStatusDetailsModel>();

                foreach (DataTable dataTable in result.Tables)
                {
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        ReportsDetails = new KOSPaymentStatusDetailsModel();

                        ReportsDetails.SerialNo = counter++;

                        if (!(dataRow["Days"] == DBNull.Value))
                        {
                            ReportsDetails.PaymentPendingSince = Convert.ToString(dataRow["Days"]);
                        }
                        else
                        {
                            ReportsDetails.PaymentPendingSince = "--";
                        }
                    }
                }
  
                resModel.KOSPaymentStatusDetailsList.Add(ReportsDetails);
                
                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }

            }

        }
    }
}