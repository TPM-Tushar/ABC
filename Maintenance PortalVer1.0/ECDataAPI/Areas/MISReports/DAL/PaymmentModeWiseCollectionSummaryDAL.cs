using CustomModels.Models.MISReports.PaymmentModeWiseCollectionSummary;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class PaymmentModeWiseCollectionSummaryDAL : IPaymentModeWiseCollectionSummary
    {
        KaveriEntities dbContext = null;
        public PaymmentModeWiseCollectionSummaryView PaymentModeWiseCollectionSummaryView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                PaymmentModeWiseCollectionSummaryView model = new PaymmentModeWiseCollectionSummaryView();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var ofcDetailsObj = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => new { x.Kaveri1Code, x.LevelID }).FirstOrDefault();
                model.SROfficeList = new List<SelectListItem>();
                model.DistrictList = new List<SelectListItem>();

                if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {
                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(DroCode) });
                    model.SROfficeList.Add(new SelectListItem() { Text = SroName, Value = ofcDetailsObj.Kaveri1Code.ToString() });
                }
                else if (ofcDetailsObj.LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == ofcDetailsObj.Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    model.DistrictList.Add(new SelectListItem() { Text = DroName, Value = Convert.ToString(ofcDetailsObj.Kaveri1Code) });
                    model.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(ofcDetailsObj.Kaveri1Code, "Select");
                }
                else
                {
                    model.SROfficeList.Add(new SelectListItem() { Text = "ALL", Value = "0" });
                    model.DistrictList = objCommon.GetDROfficesList("ALL");
                }
                model.PaymentModeList = objCommon.GetPaymentMode("All");
                model.ReceiptTypeList = objCommon.GetReceiptTypeList("All");
                model.ReceiptTypeID= Convert.ToInt32(ApiCommonEnum.ReceiptType.Document);
                SelectListItem item;
                var finYearList = dbContext.USP_FINANCIAL_YEAR().ToList();
                model.FinYearList = new List<SelectListItem>();
                foreach (var finYear in finYearList)
                {
                    item = new SelectListItem();
                    item.Text = Convert.ToString(finYear.FYEAR);
                    item.Value = Convert.ToString(finYear.YEAR);
                    model.FinYearList.Add(item);
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
        public PaymentModeWiseCollectionSummaryResModel GetPaymentModeWiseRPTTableData(PaymmentModeWiseCollectionSummaryView model)
        {
            PaymentModeWiseCollectionSummaryResModel ResModel = new PaymentModeWiseCollectionSummaryResModel();
            PaymentModeWiseDetaisModel ReportsDetails = null;
            List<PaymentModeWiseDetaisModel> ReportsDetailsList = new List<PaymentModeWiseDetaisModel>();
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                var ReceiptDetailsList = dbContext.USP_RPT_RECEIPT_COLLECTION_SUMMARY(model.DistrictID, model.SROfficeID, model.FinYearID, model.ReceiptTypeID, model.PaymentModeID).ToList();
                int counter = (model.startLen + 1); //To start Serial Number 
                Decimal TotalOfTotalCollection = 0;
                ResModel.TotalRecords = ReceiptDetailsList.Count;
                if (!model.IsExcel)
                {
                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        ReceiptDetailsList = ReceiptDetailsList.Skip(model.startLen).Take(model.totalNum).ToList();
                    }
                }
                Decimal RegistrationFeeCollected=0;
                Decimal TotalStampDutyCollected = 0;
                int TotalNoOfStampDuty = 0;
                Decimal AllOfTotalCollection = 0;
                ResModel.PaymentModewiseList = new List<PaymentModeWiseDetaisModel>();
                foreach (var item in ReceiptDetailsList)
                {
                    ReportsDetails = new PaymentModeWiseDetaisModel();
                    ReportsDetails.SrNo = counter++;
                    ReportsDetails.DistrictName = string.IsNullOrEmpty(item.DISTRICT_NAME) ? string.Empty : item.DISTRICT_NAME;
                    ReportsDetails.SROName = string.IsNullOrEmpty(item.SRO_NAME) ? string.Empty : item.SRO_NAME;
                    ReportsDetails.NoOfReceipts = item.RECEIPT_COUNT==null ? 0 : item.RECEIPT_COUNT.Value;
                    ReportsDetails.RegistrationFeeCollected = item.REGISTRATION_AND_OTHER_FEE == null ? "0.00" : item.REGISTRATION_AND_OTHER_FEE.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    RegistrationFeeCollected = RegistrationFeeCollected + item.REGISTRATION_AND_OTHER_FEE.Value;
                    ReportsDetails.NoOfStampDuty = item.STAMP_COUNT==null ? 0 : item.STAMP_COUNT.Value;
                    TotalNoOfStampDuty = TotalNoOfStampDuty + ReportsDetails.NoOfStampDuty;
                    ReportsDetails.StampDutyCollected = item.STAMP_DUTY == null ? "0.00" : item.STAMP_DUTY.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    TotalStampDutyCollected = TotalStampDutyCollected + (item.STAMP_DUTY == null ? 0 : item.STAMP_DUTY.Value);
                    ReportsDetails.TotalNoofReceiptsandStampDuty =item.TOTAL_COUNT.Value;
                    ReportsDetails.TotalCollection = item.TOTAL_COLLECTION==null ? "0.00" : item.TOTAL_COLLECTION.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    AllOfTotalCollection = AllOfTotalCollection + item.TOTAL_COLLECTION.Value;
                    ResModel.PaymentModewiseList.Add(ReportsDetails);
                    ResModel.TotalNoOfReceipts = ResModel.TotalNoOfReceipts + ReportsDetails.NoOfReceipts;
                    ResModel.TotalRegistrationFeeCollected = RegistrationFeeCollected.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    ResModel.TotalNoOfStampDuty = TotalNoOfStampDuty;
                    ResModel.TotalStampDutyCollected = TotalStampDutyCollected.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")); 
                    ResModel.AllTotalCollection = AllOfTotalCollection.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                    //TotalOfTotalCollection = TotalOfTotalCollection + ReportsDetails.Amount;
                }
                return ResModel;// DailyReceiptsResModel;
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