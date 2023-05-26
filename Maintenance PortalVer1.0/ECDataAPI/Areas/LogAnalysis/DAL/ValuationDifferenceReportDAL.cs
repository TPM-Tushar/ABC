using CustomModels.Models.Common;
using CustomModels.Models.LogAnalysis.ValuationDifferenceReport;
using CustomModels.Security;
using ECDataAPI.Areas.LogAnalysis.BAL;
using ECDataAPI.Areas.LogAnalysis.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaigrSearchDB;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.LogAnalysis.DAL
{
    public class ValuationDifferenceReportDAL : IValuationDifferenceReport
    {
        private KaveriEntities dbContext = null;
        private KaigrSearchDB searchDBContext = null;
        public ValuationDiffReportViewModel ValuationDifferenceReportView(int OfficeID)
        {
            try
            {
                dbContext = new KaveriEntities();
                SelectListItem item = new SelectListItem();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                ValuationDiffReportViewModel model = new ValuationDiffReportViewModel();
                model.PropertyTypeList = new List<SelectListItem>();
                model.RegistrationArticleList = new List<SelectListItem>();
                // condition addded by shubham bhagat 6-3-2020
                // below code commented
                //model.RegIDArr = new int[] { 17, 18, 106 };
                model.RegIDArr =dbContext.RegistrationArticles.Select(x => x.RegArticleCode).ToArray();

                model.PropertyTypeList.Add(objCommon.GetDefaultSelectListItem("Select", "0"));
                model.PropertyTypeList.Add(objCommon.GetDefaultSelectListItem("Open Built Rate", "1"));
                model.PropertyTypeList.Add(objCommon.GetDefaultSelectListItem("Agriculture", "2"));
                model.PropertyTypeList.Add(objCommon.GetDefaultSelectListItem("Apartment -A (Urban)", "3"));
                model.PropertyTypeList.Add(objCommon.GetDefaultSelectListItem("Apartment -A (Rural)", "4"));

                model.RegistrationArticleList = dbContext.RegistrationArticles.OrderBy(c => c.ArticleNameE).Select(x =>
                new SelectListItem()
                {
                    Text = x.ArticleNameE,
                    Value = x.RegArticleCode.ToString()
                }).ToList();

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
        public ValuationDiffReportDataModel GetValuationDiffRptData(ValuationDiffReportViewModel model)
        {
            try
            {
                ValuationDiffReportDataModel ValuationDiffWrapperModel = new ValuationDiffReportDataModel();
                ValuationDiffWrapperModel.ValuationDiffReportRecList = new List<ValuationDiffReportRecordModel>();
                ValuationDiffReportRecordModel ValuationDiffRec = null;
                dbContext = new KaveriEntities();
                //dbContext.Database.CommandTimeout = 0;
                int SerialNo = 1;
                string sType = "A";

                if (model.PropertyID == 3)
                    sType = "U";
                if (model.PropertyID == 4)
                {
                    sType = "R";

                    // condition addded by shubham bhagat 5-3-2020
                    //model.PropertyID = 3;
                }

                // condition addded by shubham bhagat 6-3-2020
                //List<ECDataAPI.Entity.KaveriEntities.USP_RPT_UNDERVALUATION_SUMMARY_V2_Result> result = dbContext.USP_RPT_UNDERVALUATION_SUMMARY_V2(0, model.PropertyID, model.strRegArtId, sType).ToList();
                List<ECDataAPI.Entity.KaveriEntities.USP_RPT_UNDERVALUATION_SUMMARY_V2_Result> result = dbContext.USP_RPT_UNDERVALUATION_SUMMARY_V2(0, ((model.PropertyID == 4) ? 3 : model.PropertyID), model.strRegArtId, sType).ToList();
                int TotalRecords = 0;

                if (result != null)
                {
                    TotalRecords = result.Count();
                    result = result.Skip(model.StartLen).Take(model.TotalNum).ToList();


                    if (TotalRecords > 0)
                    {
                        decimal totalAmount = 0;
                        decimal Registration_Fees_Recovery__Probable_TotalAmount = 0;
                        decimal Total_TotalAmount = 0;

                        int iTotalOccurances = 0;
                        string ofcDetails = string.Empty;
                        foreach (var item in result)
                        {
                            ValuationDiffRec = new ValuationDiffReportRecordModel();
                            ValuationDiffRec.SROName = String.IsNullOrEmpty(item.SRONameE) ? String.Empty : item.SRONameE;
                            //ValuationDiffRec.StampDutyRecovery = item.Stamp_Duty_Recovery__Probable_ == 0 ? "<i style='color:#14673a; font-size: 17px;font-weight: bold;' >" + item.Stamp_Duty_Recovery__Probable_.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</i>" : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=PopulateDetailsTable('" + item.SROCODE + "','" + item.SRONameE.Trim() + "')><i>" + item.Stamp_Duty_Recovery__Probable_.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</i></a>";
                            ofcDetails = item.SROCODE + "','" + item.SRONameE.Trim() + "')\"><i>";
                            // added by shubham bhagat on 7-3-2020
                            //ValuationDiffRec.StampDutyRecovery = item.Stamp_Duty_Recovery__Probable_ == 0 ? "<i style='color:#14673a; font-size: 17px;font-weight: bold;' >" + item.Stamp_Duty_Recovery__Probable_.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</i>" : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=\"PopulateDetailsTable('" + ofcDetails + item.Stamp_Duty_Recovery__Probable_.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</i></a>";
                            ValuationDiffRec.StampDutyRecovery = item.Stamp_Duty_Recovery__Probable_.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));//== 0 ? "<i style='color:#14673a; font-size: 17px;font-weight: bold;' >" + item.Stamp_Duty_Recovery__Probable_.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</i>" : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=\"PopulateDetailsTable('" + ofcDetails + item.Stamp_Duty_Recovery__Probable_.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</i></a>";
                            ValuationDiffRec.StampDutyRecoveryForExcel = (item.Stamp_Duty_Recovery__Probable_ == null) ? 0 : item.Stamp_Duty_Recovery__Probable_;
                            ValuationDiffRec.TansactionsDone = (item.Total_occurances == null) ? string.Empty : item.Total_occurances.ToString();
                            ValuationDiffRec.SerialNo = (SerialNo++).ToString();

                            // added by shubham bhagat on 7-3-2020
                            ValuationDiffRec.Registration_Fees_Recovery__Probable_ = item.Registration_Fees_Recovery__Probable_.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                            ValuationDiffRec.Registration_Fees_Recovery__Probable_ForExcel = (item.Registration_Fees_Recovery__Probable_ == null) ? 0 : item.Registration_Fees_Recovery__Probable_;


                            ValuationDiffRec.Total = item.Total == null ?
                                "<i style='color:#14673a; font-size: 17px;font-weight: bold;' >" + 0.00 + "</i>"
                                : "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=\"PopulateDetailsTable('" + ofcDetails + item.Total.Value.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</i></a>";
                            ValuationDiffRec.TotalForExcel = (item.Total.Value==null)? 0 : item.Total.Value;

                            iTotalOccurances += item.Total_occurances;
                            ValuationDiffWrapperModel.ValuationDiffReportRecList.Add(ValuationDiffRec);
                            totalAmount += item.Stamp_Duty_Recovery__Probable_;
                            Registration_Fees_Recovery__Probable_TotalAmount += item.Registration_Fees_Recovery__Probable_;
                            Total_TotalAmount = Total_TotalAmount + (item.Total == null ? 0 : item.Total.Value);
                        }


                        //// remove these code before production
                        //ValuationDiffRec = new ValuationDiffReportRecordModel();
                        //ValuationDiffRec.TansactionsDone = 1 + "";
                        //ValuationDiffRec.StampDutyRecovery = "<a href='#' style='color:#14673a; font-size: 17px;font-weight: bold;' title='click here' onclick=\"PopulateDetailsTable('" + ofcDetails + 55.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</i></a>";
                        //ValuationDiffRec.SerialNo = "";
                        //ValuationDiffRec.SROName = "abc";
                        //ValuationDiffWrapperModel.ValuationDiffReportRecList.Add(ValuationDiffRec);

                        //// remove these code before production till here

                        if (string.IsNullOrEmpty(model.SearchValue))
                        {
                            ValuationDiffRec = new ValuationDiffReportRecordModel();


                            if (model.IsExcel)
                            {
                                ValuationDiffRec.TansactionsDoneForExcel = iTotalOccurances;
                                ValuationDiffRec.StampDutyRecoveryForExcel = totalAmount;
                                ValuationDiffRec.SerialNo = "";
                                ValuationDiffRec.SROName = "Total";
                                ValuationDiffRec.Registration_Fees_Recovery__Probable_ForExcel = Registration_Fees_Recovery__Probable_TotalAmount;
                                ValuationDiffRec.TotalForExcel = Total_TotalAmount;
                            }
                            else
                            {

                                ValuationDiffRec.TansactionsDone = "<b>" + iTotalOccurances.ToString() + "</b>";
                                ValuationDiffRec.StampDutyRecovery = "<b>" + totalAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</b>";
                                ValuationDiffRec.SerialNo = "";
                                ValuationDiffRec.SROName = "<b>Total</b>";
                                ValuationDiffRec.Total = "<b>" + Total_TotalAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</b>";
                                ValuationDiffRec.Registration_Fees_Recovery__Probable_ = "<b>" + Registration_Fees_Recovery__Probable_TotalAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")) + "</b>";

                            }
                            ValuationDiffWrapperModel.ValuationDiffReportRecList.Add(ValuationDiffRec);
                            TotalRecords++;
                        }

                    }
                }
                ValuationDiffWrapperModel.TotalRecords = TotalRecords;
                return ValuationDiffWrapperModel;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }



        }
        public ValuationDiffReportDataModel GetValuationDiffDetailedData(ValuationDiffReportViewModel model)
        {

            try
            {

                switch (model.PropertyID)
                {

                    case 1: //Open Built Rate
                        return GetValuationDiffForOpenBuiltRate(model);

                    case 2://Agriculture
                        return GetValuationDiffAgriculture(model);  // change here

                    case 3://Apartment
                        return GetValuationDiffForApartment(model); //change here   
                    case 4://Apartment
                        return GetValuationDiffForApartment(model); //change here

                    default:
                        return null;

                }


            }
            catch (Exception)
            {
                throw;
            }

        }
        public ValuationDiffReportDataModel GetValuationDiffForOpenBuiltRate(ValuationDiffReportViewModel model)
        {
            try
            {
                ValuationDiffReportDataModel ValuationDiffWrapperModel = new ValuationDiffReportDataModel();
                ValuationDiffWrapperModel.ValuationDiffReportDetailedRecList = new List<ValuationDiffRptDetailedRecordModel>();
                ValuationDiffRptDetailedRecordModel ValuationDiffDetailedRec = null;
                dbContext = new KaveriEntities();
                int TotalRecords = 0;

                //List<ECDataAPI.Entity.KaveriEntities.USP_RPT_UNDERVALUATION_DETAILS_Result> result = dbContext.USP_RPT_UNDERVALUATION_DETAILS(model.SROCode, model.PropertyID).ToList();
                List<ECDataAPI.Entity.KaveriEntities.USP_RPT_UNDERVALUATION_DETAILS_OPENBUILTRATE_Result> result = dbContext.USP_RPT_UNDERVALUATION_DETAILS_OPENBUILTRATE(model.SROCode, model.strRegArtId).ToList();


                TotalRecords = result.Count();

                if (!model.IsExcel)
                {
                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        result = result.Skip(model.StartLen).Take(model.TotalNum).ToList();
                    }
                }

                int iNumber = 1;
                int CharsToConsider = 20;
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        ValuationDiffDetailedRec = new ValuationDiffRptDetailedRecordModel();
                        ValuationDiffDetailedRec.RegistrationDate = item.RegistrationDate.HasValue ? item.RegistrationDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : "";
                        ValuationDiffDetailedRec.FinalRegistrationNumber = String.IsNullOrEmpty(item.FinalRegistrationNumber) ? string.Empty : item.FinalRegistrationNumber;
                        ValuationDiffDetailedRec.AreaName = string.IsNullOrEmpty(item.Area_Name) ? string.Empty : item.Area_Name;
                        ValuationDiffDetailedRec.NatureOfDocument = item.NatureOfDocument;
                        ValuationDiffDetailedRec.GuidancePerSquareFeetRate = item.Guidance_Value_Per_Square_Feet_Rate_adopted_at_the_time_of_RGN == null ? 0 : item.Guidance_Value_Per_Square_Feet_Rate_adopted_at_the_time_of_RGN.Value;


                        string sFregNum = "\"" + ValuationDiffDetailedRec.FinalRegistrationNumber + "\"";

                        if (model.IsExcel)
                        {
                            ValuationDiffDetailedRec.Registration_dump = string.IsNullOrEmpty(item.Registration_dump) ? "-" : item.Registration_dump;
                        }
                        else
                        {
                            ValuationDiffDetailedRec.Registration_dump = string.IsNullOrEmpty(item.Registration_dump) ? "-"
        : "<div style='color:#14673a;cursor:pointer;font-size: 17px;font-weight: bold;' onclick='ShowBuildingMeasrDetails(" + iNumber + "," + sFregNum + ")'> " + (item.Registration_dump.Length < CharsToConsider ? item.Registration_dump.Substring(0, item.Registration_dump.Length) : item.Registration_dump.Substring(0, CharsToConsider)) + ".... <div  style='display:none;' id='spnTblBldng_" + iNumber + "'>" + item.Registration_dump + "</div></div>";
                        }

                        ValuationDiffDetailedRec.Consideration = item.Consideration_Amount_in_Doc == null ? 0 : item.Consideration_Amount_in_Doc.Value;
                        ValuationDiffDetailedRec.PaidStampDuty = item.Paid_Stamp_Duty == null ? 0 : (item.Paid_Stamp_Duty.Value);
                        ValuationDiffDetailedRec.RegFeePaid = item.RegistrationFeesPaid;
                        ValuationDiffDetailedRec.RegisteredGuidanceValue = item.Registered_Guidance_Value == null ? 0 : (item.Registered_Guidance_Value.Value);


                        ValuationDiffDetailedRec.PayableStampDuty = item.Payable_Stamp_Duty == null ? 0 : (item.Payable_Stamp_Duty.Value);
                        ValuationDiffDetailedRec.StampDutyDifference = item.Stamp_Duty_Difference == null ? 0 : (item.Stamp_Duty_Difference.Value);
                        ValuationDiffDetailedRec.payableRegFee = item.payableRegFee == null ? 0 : (item.payableRegFee.Value);
                        ValuationDiffDetailedRec.RegFeeDifference = item.RegFeeDifference == null ? 0 : (item.RegFeeDifference.Value);
                        ValuationDiffDetailedRec.TotalDifference = item.RegFeeDifference == null ? 0 : (item.TotalDifference.Value);



                        //ValuationDiffDetailedRec.Registration_dump = "<a href='#' title='Dismissible popover' data-toggle='popover' data-trigger='focus' data-content='Click anywhere in the document to close this popover'>Click me</a>";


                        ValuationDiffDetailedRec.ClickToViewDocument = "<i style='cursor:pointer;' title='click here' class='fa fa-file' aria-hidden='true' onclick=GetValuationDocumentPopup('" + URLEncrypt.EncryptParameters(new String[] { "SROCODE=" + model.SROCode, "FRN=" + item.FinalRegistrationNumber }) + "')></i>";


                        ValuationDiffDetailedRec.Result = String.IsNullOrEmpty(item.Result) ? string.Empty : item.Result;

                        // ADDED BY SHUBHAM BHAGAT ON 6-3-2020
                        ValuationDiffDetailedRec.Registered_Per_Square_Feet_Rate = item.Registered_Per_Square_Feet_Rate == null ? 0 : item.Registered_Per_Square_Feet_Rate.Value;
                        ValuationDiffDetailedRec.Measurement__Square_Feet_ = item.Measurement__Square_Feet_ == null ? 0 : item.Measurement__Square_Feet_.Value;

                        //Raman Change This Code Before Deployment
                        //ValuationDiffDetailedRec.NatureOfDocument = string.IsNullOrEmpty("Hard Coded") ? string.Empty : "Hard Coded";
                        //ValuationDiffDetailedRec.RegFeePaid = 0;
                        iNumber++;
                        ValuationDiffWrapperModel.ValuationDiffReportDetailedRecList.Add(ValuationDiffDetailedRec);
                    }

                    ////Added By RamanK on 04-03-2020
                    //ValuationDiffDetailedRec.RegistrationDate = "12/02/2020";
                    //ValuationDiffDetailedRec.FinalRegistrationNumber = "GAN00124ABC";
                    //ValuationDiffDetailedRec.AreaName =string.Empty;
                    //ValuationDiffDetailedRec.NatureOfDocument = "";
                    //ValuationDiffDetailedRec.GuidancePerSquareFeetRate = 23;

                    //// commented by shubham bhagat on  5-3-2020
                    ////string sFregNum = "\"" + ValuationDiffDetailedRec.FinalRegistrationNumber + "\"";

                    //ValuationDiffDetailedRec.Registration_dump ="fdfgdg";


                    //ValuationDiffDetailedRec.Consideration =0;
                    //ValuationDiffDetailedRec.PaidStampDuty = 0;
                    //ValuationDiffDetailedRec.RegFeePaid = 0;
                    //ValuationDiffDetailedRec.RegisteredGuidanceValue = 0;


                    //ValuationDiffDetailedRec.PayableStampDuty = 0;
                    //ValuationDiffDetailedRec.StampDutyDifference = 0;
                    //ValuationDiffDetailedRec.payableRegFee = 0;
                    //ValuationDiffDetailedRec.RegFeeDifference = 0;
                    //ValuationDiffDetailedRec.TotalDifference = 0;
                    ////ValuationDiffDetailedRec.Registration_dump = "<a href='#' title='Dismissible popover' data-toggle='popover' data-trigger='focus' data-content='Click anywhere in the document to close this popover'>Click me</a>";
                    //ValuationDiffDetailedRec.ClickToViewDocument ="fdgsfdgfdg";
                    //ValuationDiffDetailedRec.Result = "fdsgfdgsfd";
                    ////Raman Change This Code Before Deployment
                    ////ValuationDiffDetailedRec.NatureOfDocument = string.IsNullOrEmpty("Hard Coded") ? string.Empty : "Hard Coded";
                    ////ValuationDiffDetailedRec.RegFeePaid = 0;
                    //iNumber++;
                    //ValuationDiffWrapperModel.ValuationDiffReportDetailedRecList.Add(ValuationDiffDetailedRec);


                }

                ValuationDiffWrapperModel.TotalRecords = TotalRecords;
                return ValuationDiffWrapperModel;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
        }
        public ValuationDiffReportDataModel GetValuationDiffAgriculture(ValuationDiffReportViewModel model)
        {
            try
            {
                ValuationDiffReportDataModel ValuationDiffWrapperModel = new ValuationDiffReportDataModel();
                ValuationDiffWrapperModel.ValuationDiffReportDetailedRecList = new List<ValuationDiffRptDetailedRecordModel>();
                ValuationDiffRptDetailedRecordModel ValuationDiffDetailedRec = null;
                dbContext = new KaveriEntities();
                int TotalRecords = 0;

                //List<ECDataAPI.Entity.KaveriEntities.USP_RPT_UNDERVALUATION_DETAILS_Result> result = dbContext.USP_RPT_UNDERVALUATION_DETAILS(model.SROCode, model.PropertyID).ToList();
                List<ECDataAPI.Entity.KaveriEntities.USP_RPT_UNDERVALUATION_DETAILS_AGRICULTURE_Result> result = dbContext.USP_RPT_UNDERVALUATION_DETAILS_AGRICULTURE(model.SROCode, model.strRegArtId).ToList();


                TotalRecords = result.Count();

                if (!model.IsExcel)
                {
                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        result = result.Skip(model.StartLen).Take(model.TotalNum).ToList();
                    }
                }

                int iNumber = 1;

                if (result != null)
                {
                    foreach (var item in result)
                    {
                        ValuationDiffDetailedRec = new ValuationDiffRptDetailedRecordModel();
                        ValuationDiffDetailedRec.RegistrationDate = item.RegistrationDate.HasValue ? item.RegistrationDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : "";
                        ValuationDiffDetailedRec.FinalRegistrationNumber = String.IsNullOrEmpty(item.FinalRegistrationNumber) ? string.Empty : item.FinalRegistrationNumber;

                        ValuationDiffDetailedRec.AreaName = string.IsNullOrEmpty(item.Area_Name) ? string.Empty : item.Area_Name;

                        ValuationDiffDetailedRec.NatureOfDocument = item.NatureOfDocument;

                        ValuationDiffDetailedRec.GuidancePerSquareFeetRate = item.Guidance_Value_adopted_at_the_time_of_RGN == null ? 0 : item.Guidance_Value_adopted_at_the_time_of_RGN.Value;

                        ValuationDiffDetailedRec.Measurement__Guntas = item.Measurement__Guntas_ ?? 0;
                        ValuationDiffDetailedRec.Registered_Per_Gunta_Rate = item.Registered_Per_Gunta_Rate ?? 0;

                        ValuationDiffDetailedRec.Consideration = item.Consideration_Amount_in_Doc == null ? 0 : item.Consideration_Amount_in_Doc.Value;
                        ValuationDiffDetailedRec.PaidStampDuty = item.Paid_Stamp_Duty == null ? 0 : (item.Paid_Stamp_Duty.Value);
                        ValuationDiffDetailedRec.RegFeePaid = item.RegistrationFeesPaid;
                        ValuationDiffDetailedRec.RegisteredGuidanceValue = item.Registered_Guidance_Value == null ? 0 : (item.Registered_Guidance_Value.Value);


                        ValuationDiffDetailedRec.PayableStampDuty = item.Payable_Stamp_Duty == null ? 0 : (item.Payable_Stamp_Duty.Value);
                        ValuationDiffDetailedRec.StampDutyDifference = item.Stamp_Duty_Difference == null ? 0 : (item.Stamp_Duty_Difference.Value);
                        ValuationDiffDetailedRec.payableRegFee = item.payableRegFee == null ? 0 : (item.payableRegFee.Value);
                        ValuationDiffDetailedRec.RegFeeDifference = item.RegFeeDifference == null ? 0 : (item.RegFeeDifference.Value);
                        ValuationDiffDetailedRec.TotalDifference = item.RegFeeDifference == null ? 0 : (item.TotalDifference.Value);



                        //ValuationDiffDetailedRec.Registration_dump = "<a href='#' title='Dismissible popover' data-toggle='popover' data-trigger='focus' data-content='Click anywhere in the document to close this popover'>Click me</a>";


                        ValuationDiffDetailedRec.ClickToViewDocument = "<i style='cursor:pointer;' title='click here' class='fa fa-file' aria-hidden='true' onclick=GetValuationDocumentPopup('" + URLEncrypt.EncryptParameters(new String[] { "SROCODE=" + model.SROCode, "FRN=" + item.FinalRegistrationNumber }) + "')></i>";


                        ValuationDiffDetailedRec.Result = String.IsNullOrEmpty(item.Result) ? string.Empty : item.Result;
                        //Raman Change This Code Before Deployment
                        //ValuationDiffDetailedRec.NatureOfDocument = string.IsNullOrEmpty("Hard Coded") ? string.Empty : "Hard Coded";
                        //ValuationDiffDetailedRec.RegFeePaid = 0;
                        iNumber++;
                        ValuationDiffWrapperModel.ValuationDiffReportDetailedRecList.Add(ValuationDiffDetailedRec);
                    }
                }
                //// REMOVE FROM HERE

                //ValuationDiffDetailedRec = new ValuationDiffRptDetailedRecordModel();
                //ValuationDiffDetailedRec.RegistrationDate = "08/05/1990";
                //ValuationDiffDetailedRec.FinalRegistrationNumber = "hardcoded";

                //ValuationDiffDetailedRec.AreaName = "hardcoded";
                //ValuationDiffDetailedRec.NatureOfDocument = "hardcoded";

                //ValuationDiffDetailedRec.GuidancePerSquareFeetRate = 1;

                //ValuationDiffDetailedRec.Measurement__Guntas = 32;
                //ValuationDiffDetailedRec.Registered_Per_Gunta_Rate = 33;

                //ValuationDiffDetailedRec.Consideration = 34;
                //ValuationDiffDetailedRec.PaidStampDuty = 35;
                //ValuationDiffDetailedRec.RegFeePaid = 36;
                //ValuationDiffDetailedRec.RegisteredGuidanceValue = 37;


                //ValuationDiffDetailedRec.PayableStampDuty = 38;
                //ValuationDiffDetailedRec.StampDutyDifference = 39;
                //ValuationDiffDetailedRec.payableRegFee = 40;
                //ValuationDiffDetailedRec.RegFeeDifference = 41;
                //ValuationDiffDetailedRec.TotalDifference = 42;




                //ValuationDiffDetailedRec.ClickToViewDocument = "HARDCODED";


                //ValuationDiffDetailedRec.Result = "hardcoded";

                ////Raman Change This Code Before Deployment
                ////ValuationDiffDetailedRec.NatureOfDocument = string.IsNullOrEmpty("Hard Coded") ? string.Empty : "Hard Coded";
                ////ValuationDiffDetailedRec.RegFeePaid = 0;
                //iNumber++;
                //ValuationDiffWrapperModel.ValuationDiffReportDetailedRecList.Add(ValuationDiffDetailedRec);

                //// REMOVE TILL HERE



                ValuationDiffWrapperModel.TotalRecords = TotalRecords;
                return ValuationDiffWrapperModel;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
        }
        public ValuationDiffReportDataModel GetValuationDiffForApartment(ValuationDiffReportViewModel model)

        {
            try
            {
                ValuationDiffReportDataModel ValuationDiffWrapperModel = new ValuationDiffReportDataModel();
                ValuationDiffWrapperModel.ValuationDiffReportDetailedRecList = new List<ValuationDiffRptDetailedRecordModel>();
                ValuationDiffRptDetailedRecordModel ValuationDiffDetailedRec = null;
                dbContext = new KaveriEntities();
                int TotalRecords = 0;

                //List<ECDataAPI.Entity.KaveriEntities.USP_RPT_UNDERVALUATION_DETAILS_Result> result = dbContext.USP_RPT_UNDERVALUATION_DETAILS(model.SROCode, model.PropertyID).ToList();
                List<ECDataAPI.Entity.KaveriEntities.USP_RPT_UNDERVALUATION_DETAILS_APARTMENT_A_Result> result = dbContext.USP_RPT_UNDERVALUATION_DETAILS_APARTMENT_A(model.SROCode, (model.PropertyID == 3 ? "U" : "R"), model.strRegArtId).ToList();


                TotalRecords = result.Count();

                if (!model.IsExcel)
                {
                    if (string.IsNullOrEmpty(model.SearchValue))
                    {
                        result = result.Skip(model.StartLen).Take(model.TotalNum).ToList();
                    }
                }

                int iNumber = 1;
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        ValuationDiffDetailedRec = new ValuationDiffRptDetailedRecordModel();
                        ValuationDiffDetailedRec.RegistrationDate = item.RegistrationDate.HasValue ? item.RegistrationDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : "";
                        ValuationDiffDetailedRec.FinalRegistrationNumber = String.IsNullOrEmpty(item.FinalRegistrationNumber) ? string.Empty : item.FinalRegistrationNumber;
                        ValuationDiffDetailedRec.NatureOfDocument = item.NatureOfDocument;
                        ValuationDiffDetailedRec.Consideration = item.Consideration ?? 0;
                        ValuationDiffDetailedRec.AreaName = string.IsNullOrEmpty(item.Area_Name) ? string.Empty : item.Area_Name;
                        ValuationDiffDetailedRec.Apartment_Name = string.IsNullOrEmpty(item.Apartment_Name) ? string.Empty : item.Apartment_Name;
                        ValuationDiffDetailedRec.Super_Builtup_Area_shown_in_Document = item.Super_Builtup_Area_shown_in_Document ?? 0;
                        ValuationDiffDetailedRec.Rate_as_per_G_V_notification_01_01_2019 = item.Rate_as_per_G_V_notification_01_01_2019 ?? 0;
                        ValuationDiffDetailedRec.Total_Value_on_Super_Builtup_Area = item.Total_Value_on_Super_Builtup_Area ?? 0;
                        ValuationDiffDetailedRec.PayableStampDuty = item.Payable_Stamp_Duty == null ? 0 : (item.Payable_Stamp_Duty.Value);
                        ValuationDiffDetailedRec.payableRegFee = item.payableRegFee == null ? 0 : (item.payableRegFee.Value);
                        ValuationDiffDetailedRec.TotalPayable = item.TotalPayable ?? 0;
                        ValuationDiffDetailedRec.Market_Value_calculated_as_per_document_at_the_time_of_Registration = item.Market_Value_calculated_as_per_document_at_the_time_of_Registration ?? 0;
                        ValuationDiffDetailedRec.PaidStampDuty = item.Paid_Stamp_Duty == null ? 0 : (item.Paid_Stamp_Duty.Value);
                        ValuationDiffDetailedRec.RegFeePaid = item.RegistrationFeesPaid;
                        ValuationDiffDetailedRec.TotalPaid = item.TotalPaid ?? 0;
                        ValuationDiffDetailedRec.Difference_between_the_Two = item.Difference_between_the_Two ?? 0;

                        ValuationDiffDetailedRec.ClickToViewDocument = "<i style='cursor:pointer;' title='click here' class='fa fa-file' aria-hidden='true' onclick=GetValuationDocumentPopup('" + URLEncrypt.EncryptParameters(new String[] { "SROCODE=" + model.SROCode, "FRN=" + item.FinalRegistrationNumber }) + "')></i>";

                        iNumber++;
                        ValuationDiffWrapperModel.ValuationDiffReportDetailedRecList.Add(ValuationDiffDetailedRec);
                    }

                    //ValuationDiffDetailedRec = new ValuationDiffRptDetailedRecordModel();
                    //ValuationDiffDetailedRec.RegistrationDate = "08/05/1990";
                    //ValuationDiffDetailedRec.FinalRegistrationNumber = "hardcoded";


                    //ValuationDiffDetailedRec.NatureOfDocument = "hardcoded";
                    //ValuationDiffDetailedRec.Consideration = 1;
                    //ValuationDiffDetailedRec.AreaName = "hardcoded";
                    //ValuationDiffDetailedRec.Apartment_Name = "hardcoded";

                    //ValuationDiffDetailedRec.Super_Builtup_Area_shown_in_Document = 2;

                    //ValuationDiffDetailedRec.Rate_as_per_G_V_notification_01_01_2019 = 3;
                    //ValuationDiffDetailedRec.Total_Value_on_Super_Builtup_Area = 4;
                    //ValuationDiffDetailedRec.PayableStampDuty = 5;

                    //ValuationDiffDetailedRec.payableRegFee = 6;
                    //ValuationDiffDetailedRec.TotalPayable = 7;
                    //ValuationDiffDetailedRec.Market_Value_calculated_as_per_document_at_the_time_of_Registration = 8;

                    //ValuationDiffDetailedRec.PaidStampDuty = 9;

                    //ValuationDiffDetailedRec.RegFeePaid = 10;
                    //ValuationDiffDetailedRec.TotalPaid = 11;
                    //ValuationDiffDetailedRec.Difference_between_the_Two = 12;



                    //ValuationDiffDetailedRec.ClickToViewDocument = "HARDCODED";



                    //iNumber++;
                    //ValuationDiffWrapperModel.ValuationDiffReportDetailedRecList.Add(ValuationDiffDetailedRec);


                }

                ValuationDiffWrapperModel.TotalRecords = TotalRecords;
                return ValuationDiffWrapperModel;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
        }
        public string GetSroName(int SROfficeID)
        {
            string SroName = String.Empty;
            try
            {
                dbContext = new KaveriEntities();
                SroName = dbContext.SROMaster.Where(x => x.SROCode == SROfficeID).Select(x => x.SRONameE).FirstOrDefault();
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
            return SroName;
        }
        public FileDisplayModel GetValuationDocument(ValuationDiffFileModel reqModel)
        {

            FileDisplayModel fileDisplayModel = new FileDisplayModel();
            DownloadCCModel objDownloadCC = null;

            Dictionary<String, String> decryptedParameters = null;
            String[] encryptedParameters = null;
            encryptedParameters = reqModel.encryptedId.Split('/');
            if (!(encryptedParameters.Length == 3))
                throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");


            decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

            string FinalRegistrationNumber = decryptedParameters["FRN"];
            int SROCode = Convert.ToInt32(decryptedParameters["SROCODE"]);
            int documentType = 1;
            string errorMessage = "";



            ECDataAPI.AnywhereCCService.PreRegCCService preRegCCService = new ECDataAPI.AnywhereCCService.PreRegCCService();

            //string fileName = preRegCCService.GetCCFileName(FinalRegistrationNumber, SROCode, documentType, false, string.Empty, string.Empty, ref errorMessage);
            ECDataAPI.AnywhereCCService.CCFileDetailsModel cCFileDetailsModel= preRegCCService.GetCCFileName(FinalRegistrationNumber, SROCode, documentType, false, string.Empty, string.Empty, ref errorMessage);



            //if (!string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(errorMessage))
            if (!string.IsNullOrEmpty(cCFileDetailsModel.CCFileName) && string.IsNullOrEmpty(errorMessage))
            {
                //objDownloadCC = new DownloadCCModel(fileName);
                objDownloadCC = new DownloadCCModel(cCFileDetailsModel.CCFileName);
                if (objDownloadCC.DownloadCCChunkFormat(ref errorMessage))
                {
                    //  return fileName;
                    fileDisplayModel.isFileExist = true;
                    fileDisplayModel.fileBytes = System.IO.File.ReadAllBytes(objDownloadCC.LocalFilePath);
                    System.IO.File.Delete(objDownloadCC.LocalFilePath);

                }
            }
            else
            {
                fileDisplayModel.Message = errorMessage;
            }

            return fileDisplayModel;
        }
        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                    dbContext.Dispose();
                if (searchDBContext != null)
                    searchDBContext.Dispose();
            }
            // free native resources
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}