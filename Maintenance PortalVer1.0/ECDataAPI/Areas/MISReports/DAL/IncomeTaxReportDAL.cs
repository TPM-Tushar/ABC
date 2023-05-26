using CustomModels.Models.MISReports.IncomeTaxReport;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class IncomeTaxReportDAL : IIncomeTaxReport
    {

        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();

        
        public IncomeTaxReportResponseModel IncomeTaxReportView(int OfficeID)
        {
            IncomeTaxReportResponseModel incomeTaxReportResponseModel = new IncomeTaxReportResponseModel();

            try
            {
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                
                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                incomeTaxReportResponseModel.SROfficeList = new List<SelectListItem>();
                incomeTaxReportResponseModel.DROfficeList = new List<SelectListItem>();

                string kaveriCode = Kaveri1Code.ToString();
                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {

                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);

                    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    incomeTaxReportResponseModel.DROfficeList.Add(droNameItem);
                    incomeTaxReportResponseModel.SROfficeList.Add(sroNameItem);

                }
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {

                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    incomeTaxReportResponseModel.DROfficeList.Add(droNameItem);
                    incomeTaxReportResponseModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "Select");
                }
                else
                {
                    SelectListItem select = new SelectListItem();
                    select.Text = "Select";
                    select.Value = "0";
                    incomeTaxReportResponseModel.SROfficeList.Add(select);
                    incomeTaxReportResponseModel.DROfficeList = objCommon.GetDROfficesList("Select");
                }

                
                List<SelectListItem> FinList = new List<SelectListItem>();

                FinList.Insert(0, objCommon.GetDefaultSelectListItem("Select", "0"));
                //FinList.AddRange(dbContext.USP_FINANCIAL_YEAR().Select(x => new SelectListItem()
                //{
                //    Text = x.FYEAR,
                //    Value = Convert.ToString(x.YEAR)
                //}).ToList());
                FinList.AddRange(dbContext.USP_FINANCIAL_YEAR_FROM_GIVEN_YEAR(2010).Select(x => new SelectListItem()
                {
                    Text = x.FYEAR,
                    Value = Convert.ToString(x.YEAR)
                }).ToList());
                

                incomeTaxReportResponseModel.FinYearList = FinList;

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

            return incomeTaxReportResponseModel;
        }

        
        public IncomeTaxReportResultModel GetIncomeTaxReportDetails(IncomeTaxReportResponseModel model)
        {

            IncomeTaxReportDetailsModel incomeTaxReportDetailsModel = null;
            IncomeTaxReportResultModel incomeTaxReportResultModel = new IncomeTaxReportResultModel();
            incomeTaxReportResultModel.incomeTaxReportDetailsList = new List<IncomeTaxReportDetailsModel>();


            //List<IncomeTaxReportDetailsModel> incomeTaxReportDetailsList = new List<IncomeTaxReportDetailsModel>();
            KaveriEntities dbContext = null;
            
            try
            {
                dbContext = new KaveriEntities();
                
                var IncomeTaxReportList = dbContext.USP_RPT_INCOMETAX_REPORT(model.DROfficeListID, model.SROfficeListID, model.FinYearListID).Skip(model.startLen).Take(model.totalNum).ToList();

                incomeTaxReportResultModel.SROName = dbContext.SROMaster.Where(x => x.SROCode == model.SROfficeListID).Select(y => y.SRONameE).FirstOrDefault();
                incomeTaxReportResultModel.DROName = dbContext.DistrictMaster.Where(x => x.DistrictCode == model.DROfficeListID).Select(y => y.DistrictNameE).FirstOrDefault();


                //var FinYearName = dbContext.USP_FINANCIAL_YEAR_FROM_GIVEN_YEAR(2010).Select(x => new SelectListItem()
                //                {
                //                    Text = x.FYEAR,
                //                    Value = Convert.ToString(x.YEAR)
                //                }).ToList();

                incomeTaxReportResultModel.FinYearName = dbContext.USP_FINANCIAL_YEAR_FROM_GIVEN_YEAR(2010).Where(x => x.YEAR == model.FinYearListID).Select(x => x.FYEAR).FirstOrDefault();
                incomeTaxReportResultModel.isClickedOnSearchBtn = true;

                //FinYearName.Any(model.FinYearListID);



                foreach (var item in IncomeTaxReportList)
                {
                    
                    incomeTaxReportDetailsModel = new IncomeTaxReportDetailsModel();
                    

                    incomeTaxReportDetailsModel.ReportSrNo = Convert.ToInt64(item.REPORT_SNO);
                    incomeTaxReportDetailsModel.OriginalReportSrNo = Convert.ToInt64(item.ORIGINAL_REPORT_SNO);
                    incomeTaxReportDetailsModel.CustomerId = item.CUSTOMERID;
                    incomeTaxReportDetailsModel.PersonName = item.PERSON_NAME;
                    incomeTaxReportDetailsModel.DateOfBirth = item.DOB;
                    incomeTaxReportDetailsModel.FathersName = item.FATHERS_NAME;
                    incomeTaxReportDetailsModel.PanAckNo = item.PAN_ACK_NO;
                    incomeTaxReportDetailsModel.AadharNo = item.AADHAR_NO;
                    incomeTaxReportDetailsModel.IdentificationType = item.IDENTIFICATION_TYPE;
                    incomeTaxReportDetailsModel.IdentificationNumber = item.Identification_Number;
                    incomeTaxReportDetailsModel.FlatDoorBuilding = item.FLAT_DOOR_BUILDING;
                    incomeTaxReportDetailsModel.NameOfPremises = item.NAME_OF_PREM;
                    incomeTaxReportDetailsModel.RoadStreet = item.ROAD_STREET;
                    incomeTaxReportDetailsModel.AreaLocality = item.AREA_LOCALITY;
                    incomeTaxReportDetailsModel.CityTown = item.CITY_TOWN;
                    incomeTaxReportDetailsModel.PostalCode = item.POSTAL_CODE;
                    incomeTaxReportDetailsModel.StateCode = item.STATE_CODE;
                    incomeTaxReportDetailsModel.CountryCode = item.COUNTRY_CODE;
                    incomeTaxReportDetailsModel.MobileNo = item.MOBILE_NO;
                    incomeTaxReportDetailsModel.StdCode = item.STD_CODE;
                    incomeTaxReportDetailsModel.TelephoneNo = item.TELEPHONE_NO;
                    incomeTaxReportDetailsModel.EstimatedAgriIncome = Convert.ToDecimal(item.AGRI_INCOME);
                    incomeTaxReportDetailsModel.EstimatedNonAgriIncome = Convert.ToDecimal(item.NON_AGRI_INCOME);
                    incomeTaxReportDetailsModel.Remarks = item.REMARKS;
                    incomeTaxReportDetailsModel.Form60AckNo = item.FORM60_ACK_NO;
                    //incomeTaxReportDetailsModel.TransactionDate = item.TXN_DATE;
                    incomeTaxReportDetailsModel.TransactionDate = ((Convert.ToDateTime(item.TXN_DATE)).ToShortDateString()).ToString();
                    incomeTaxReportDetailsModel.TransactionID = item.TXN_ID;
                    incomeTaxReportDetailsModel.TransactionType = item.TXN_TYPE;
                    incomeTaxReportDetailsModel.TransactionAmount = Convert.ToDecimal(item.TXN_AMOUNT);
                    incomeTaxReportDetailsModel.TransactionMode = item.TXN_MODE;
                    
                    incomeTaxReportDetailsModel.FinYearListID = model.FinYearListID;
                    incomeTaxReportDetailsModel.isClickedOnSearchBtn = true;
                    incomeTaxReportResultModel.incomeTaxReportDetailsList.Add(incomeTaxReportDetailsModel);
                    
                    
                }
                
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
            return incomeTaxReportResultModel ;

        }

        
        public int GetIncomeTaxReportDetailsTotalCount(IncomeTaxReportResponseModel model)
        {

            KaveriEntities dbContext = null;
            List<USP_RPT_INCOMETAX_REPORT_Result> Result = null;

            try
            {
                dbContext = new KaveriEntities();
                Result = dbContext.USP_RPT_INCOMETAX_REPORT(model.DROfficeListID, model.SROfficeListID, model.FinYearListID).ToList();
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

            return Result.Count();
        }


        public string GetSroName(int SROfficeID)
        {
            string SroName;
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


    }
}