#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ECDailyReceiptReportDAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.ECDailyReceiptReport;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECDataAPI.Entity.KaveriEntities;


namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ECDailyReceiptReportDAL : IECDailyReceiptReport
    {
        KaveriEntities dbContext = null;
        public ECDailyReceiptRptView ECDailyReceiptDetails(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                ECDailyReceiptRptView model = new ECDailyReceiptRptView();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //SelectListItem sroNameItem = new SelectListItem();
                //SelectListItem droNameItem = new SelectListItem();

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                //int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();

                model.SROfficeList = new List<SelectListItem>();
                model.DistrictList = new List<SelectListItem>();

                // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                //string kaveriCode = Kaveri1Code.ToString();
                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();

                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    // string DroCode_string = Convert.ToString(DroCode);
                    //sroNameItem = objCommon.GetDefaultSelectListItem(SroName, ofcDetailsObj.kaveriCode);
                    //droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    model.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });
                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    // string DroCode_string = Convert.ToString(ofcDetailsObj.Kaveri1Code);
                    // droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);

                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "Select");
                }
                else
                {
                    // commented and changed by shubham bhagat on 18-12-2019 at 2:58 am.
                    //SelectListItem select = new SelectListItem();
                    //select.Text = "Select";
                    //select.Value = "0";
                    model.SROfficeList.Add(new SelectListItem() { Text = "Select", Value = "0" });
                    model.DistrictList = objCommon.GetDROfficesList("Select");
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


        //public ECDailyReceiptRptView ECDailyReceiptDetails(int OfficeID)
        //{
        //    try
        //    {
        //        dbContext = new KaveriEntities();
        //        ECDailyReceiptRptView model = new ECDailyReceiptRptView();
        //        ApiCommonFunctions objCommon = new ApiCommonFunctions();
        //        SelectListItem sroNameItem = new SelectListItem();
        //        SelectListItem droNameItem = new SelectListItem();
        //        DateTime now = DateTime.Now;
        //        var startDate = new DateTime(now.Year, now.Month, 1);
        //        model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
        //        int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
        //        model.SROfficeList = new List<SelectListItem>();
        //        model.DistrictList = new List<SelectListItem>();
        //        string kaveriCode = Kaveri1Code.ToString();
        //        if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
        //        {
        //            string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
        //            int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(DroCode);

        //            sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DistrictList.Add(droNameItem);
        //            model.SROfficeList.Add(sroNameItem);
        //        }
        //        else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
        //        {
        //            string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
        //            string DroCode_string = Convert.ToString(Kaveri1Code);
        //            droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
        //            model.DistrictList.Add(droNameItem);
        //            model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "Select");
        //        }
        //        else
        //        {
        //            SelectListItem select = new SelectListItem();
        //            select.Text = "Select";
        //            select.Value = "0";
        //            model.SROfficeList.Add(select);
        //            model.DistrictList = objCommon.GetDROfficesList("Select");
        //        }
        //        return model;

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //            dbContext.Dispose();
        //    }
        //}
        public ECDailyReceiptRptResModel GetECDailyReceiptDetails(ECDailyReceiptRptView model)
        {
            ECDailyReceiptRptResModel DailyReceiptsResModel = new ECDailyReceiptRptResModel();
            ECDailyReceiptDetailsModel ReportsDetails = null;
            List<ECDailyReceiptDetailsModel> ReportsDetailsList = new List<ECDailyReceiptDetailsModel>();
            try
            {
                dbContext = new KaveriEntities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                var ReceiptDetailsList = dbContext.GetECDailyReceiptDetails(model.FromDate ,model.ToDate ,model.SROfficeID).ToList();
                int counter = (model.startLen + 1); //To start Serial Number 
                DailyReceiptsResModel.TotalRecords = ReceiptDetailsList.Count;
                DailyReceiptsResModel.DailyReceiptDetailsList = new List<ECDailyReceiptDetailsModel>();
                Decimal TotalAmount=0;
                if (!model.IsExcel)
                {
                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        ReceiptDetailsList = ReceiptDetailsList.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }
                foreach (var item in ReceiptDetailsList)
                {
                    ReportsDetails = new ECDailyReceiptDetailsModel();
                    ReportsDetails.SrNo = counter++;
                    ReportsDetails.ReceiptNo =  Convert.ToString(item.ReceiptNumber);
                    ReportsDetails.AppNo = string.IsNullOrEmpty(item.ApplicationNumber) ? string.Empty : item.ApplicationNumber;
                    ReportsDetails.SrOfficeAppNo = string.IsNullOrEmpty(item.SROApplicationNumber) ? string.Empty : item.SROApplicationNumber;
                    ReportsDetails.AppName = string.IsNullOrEmpty(item.ApplicantName) ? string.Empty : item.ApplicantName;
                    ReportsDetails.IssuedBy = string.IsNullOrEmpty(item.LoginName) ? string.Empty : item.LoginName;
                    if (item.SearchFromDate != null && item.SearchToDate != null && item.NumberOfYears!=null)
                        ReportsDetails.PeriodOfSearch =item.NumberOfYears +" [" + item.SearchFromDate.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture) + " to " + item.SearchToDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) +"]";
                    else
                        ReportsDetails.PeriodOfSearch = "";

                    if (item.paidAmount!=null)
                        ReportsDetails.Amount = Convert.ToDecimal(item.paidAmount);
                    else 
                        ReportsDetails.Amount = 0;
                    
                    ReportsDetails.ReceiptType = string.IsNullOrEmpty(item.ReceiptType) ? "" : item.ReceiptType;

                    if (item.ReceiptDate != null)
                        ReportsDetails.ReceiptDate = item.ReceiptDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    else
                        ReportsDetails.ReceiptDate = "";

                    //Added By ShivamB to view this columns in the ECDailyRecieptDetails Grid Table on 07-09-2022
                    ReportsDetails.ModeOfPayment = item.PaymentMode.ToString();
                    ReportsDetails.ChallanNumber = item.ChallanNo.ToString();
                    //Added By ShivamB to view this columns in the ECDailyRecieptDetails Grid Table on 07-09-2022


                    DailyReceiptsResModel.DailyReceiptDetailsList.Add(ReportsDetails);
                    TotalAmount = TotalAmount + Convert.ToDecimal(item.paidAmount);

                    
                }
                DailyReceiptsResModel.TotalAmount = TotalAmount;
                return DailyReceiptsResModel;
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
        public int GetECDailyReceiptsTotalCount(ECDailyReceiptRptView model)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                dbContext = new KaveriEntities();
                var ReceiptDetailsList = dbContext.GetECDailyReceiptDetails(model.FromDate, model.ToDate, model.SROfficeID).ToList();
                return ReceiptDetailsList.Count();
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

    }
}