using CustomModels.Models.Remittance.ScheduleAllocationAnalysis;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECDataUI.Common;
using ECDataUI.Filters;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace ECDataAPI.Areas.Remittance.DAL
{
    public class ScheduleAllocationAnalysisDAL : IScheduleAllocationAnalysis
    {
        KaveriEntities dbContext = null;
        ApiCommonFunctions objCommon = new ApiCommonFunctions();


        public ScheduleAllocationAnalysisResponseModel ScheduleAllocationAnalysisView(int OfficeID)
        {
            ScheduleAllocationAnalysisResponseModel responseModel = new ScheduleAllocationAnalysisResponseModel();

            try
            {
                dbContext = new KaveriEntities();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();

                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();

                responseModel.SROfficeList = new List<SelectListItem>();
                responseModel.DROfficeList = new List<SelectListItem>();

                string kaveriCode = Kaveri1Code.ToString();
                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {

                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);

                    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    responseModel.DROfficeList.Add(droNameItem);
                    responseModel.SROfficeList.Add(sroNameItem);

                }
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {

                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    responseModel.DROfficeList.Add(droNameItem);
                    responseModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, "Select");
                }
                else
                {
                    SelectListItem select = new SelectListItem();
                    select.Text = "All";
                    select.Value = "0";
                    responseModel.SROfficeList.Add(select);
                    //Commented By ShivamB on 30-09-2022 for adding All options in District,  DropDown parameter.
                    //responseModel.DROfficeList = objCommon.GetDROfficesList("Select");
                    //Ended By ShivamB on 30-09-2022 for adding All options in District DropDown parameter.

                    //Added By ShivamB on 30-09-2022 for adding All options in District DropDown parameter.
                    responseModel.DROfficeList = objCommon.GetDROfficesList("All");
                    //Ended By ShivamB on 30-09-2022 for adding All options in District DropDown parameter.
                }


                List<SelectListItem> regArticleList = new List<SelectListItem>();
                responseModel.RegArticleId = dbContext.RegistrationArticles.Select(c => c.RegArticleCode).ToArray();


                List<SelectListItem> list = (from RA in dbContext.RegistrationArticles
                                             select new SelectListItem()
                                             {
                                                 Text = RA.ArticleNameE.ToString(),
                                                 Value = RA.RegArticleCode.ToString(),
                                                 Selected = false,
                                             }).ToList();


                responseModel.RegArticleList = list;
                responseModel.IsSelectAll = false;
                
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

            return responseModel;
        }

        public int GetScheduleAllocationAnalysisDetailsTotalCount(ScheduleAllocationAnalysisResponseModel model)
        {

            KaveriEntities dbContext = null;
            var Result = 0;
            
            try
            {


                //Added By ShivamB on 30-09-2022 for adding Select all option in Year textBox.
                //int Year = Convert.ToInt32(model.Year);
                int StartYear = 1990;
                DateTime dateTime = DateTime.Now;
                int CurrentYear = dateTime.Year;
                int YearDiff = CurrentYear - StartYear;


                int[] Year = new int[YearDiff] ;
                Year[0] = 1991;
                int SROfficeListID = Convert.ToInt32(model.SROfficeListID);


                if (model.IsSelectAllYearSelected == true)
                {
                    for (int i = 1; i < YearDiff; i++)
                    {
                        Year[i] = Year[i-1] + 1;
                    }
                }
                else
                {
                    Year[0] = Convert.ToInt32(model.Year);
                }
                //Ended By ShivamB on 30-09-2022 for adding Select all option in Year textBox.


                dbContext = new KaveriEntities();

                //Added By ShivamB on 30-09-2022 for adding All options in District DropDown and SRO Dropdown parameter.
                if (model.DROfficeListID == 0 && model.SROfficeListID == 0)
                {
                    model.SROList = dbContext.SROMaster.Select(x => x.SROCode).ToArray(); 
                }
                else if (model.DROfficeListID != 0 && model.SROfficeListID == 0)
                {
                    model.SROList = dbContext.SROMaster.Where(x => x.DistrictCode == model.DROfficeListID).Select(y => y.SROCode).ToArray();
                }
                else
                {
                    model.SROList = dbContext.SROMaster.Where(x=>x.SROCode == SROfficeListID).Select(x=>x.SROCode).ToArray(); 
                }
                //Ended By ShivamB on 30-09-2022 for adding All options in District DropDown and SRO Dropdown parameter.
                



                if (model.IsPartyIdCheckBoxSelected == true && model.IsThroughVerifyCheckBoxSelected == true) //model.IsThroughVerifyCheckBoxSelected == true is Added by ShivamB on 30-09-2022 for checking IsThroughVerify is not null
                {
                    Result = (from DM in dbContext.DocumentMaster
                              join PM in dbContext.PropertyMaster
                              on new { DM.DocumentID, DM.SROCode } equals new { PM.DocumentID, SROCode = PM.RegSROCode }
                              join PS in dbContext.PropertySchedules
                              on new { PM.PropertyID, SROCode = PM.RegSROCode } equals new { PS.PropertyID, PS.SROCode }


                              where (
                              //Commented By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                              //(DM.SROCode == model.SROfficeListID)
                              //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.

                              //Added By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                              (model.SROList.Contains(DM.SROCode))
                              && (PS.IsThroughVerify != null)
                              //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                              && (PS.PartyIDs != null)
                              //&& (DM.Stamp5DateTime.Value.Year == Year)
                              && (Year.Contains(DM.Stamp5DateTime.Value.Year))
                              && (model.RegArticleId.Contains(DM.RegArticleCode))
                              && (PM.TotalArea > 0))


                              orderby DM.Stamp5DateTime
                              select new
                              {
                                  DM.FinalRegistrationNumber,
                                  DM.Stamp5DateTime
                              }).ToList().Count();
                }
                else if (model.IsPartyIdCheckBoxSelected == true && model.IsThroughVerifyCheckBoxSelected == false) //model.IsThroughVerifyCheckBoxSelected == true is Added by ShivamB on 30-09-2022 for checking IsThroughVerify is null
                {
                    Result = (from DM in dbContext.DocumentMaster
                              join PM in dbContext.PropertyMaster
                              on new { DM.DocumentID, DM.SROCode } equals new { PM.DocumentID, SROCode = PM.RegSROCode }
                              join PS in dbContext.PropertySchedules
                              on new { PM.PropertyID, SROCode = PM.RegSROCode } equals new { PS.PropertyID, PS.SROCode }
                              
                              where (
                              (model.SROList.Contains(DM.SROCode))
                              && (PS.IsThroughVerify == null)
                              && (PS.PartyIDs != null)
                              //&& (DM.Stamp5DateTime.Value.Year == Year)
                              && (Year.Contains(DM.Stamp5DateTime.Value.Year))
                              && (model.RegArticleId.Contains(DM.RegArticleCode))
                              && (PM.TotalArea > 0))


                              orderby DM.Stamp5DateTime
                              select new
                              {
                                  DM.FinalRegistrationNumber,
                                  DM.Stamp5DateTime
                              }).ToList().Count();
                }
                else if(model.IsPartyIdCheckBoxSelected == false && model.IsThroughVerifyCheckBoxSelected == true) //model.IsThroughVerifyCheckBoxSelected == true is Added by ShivamB on 30-09-2022 for checking IsThroughVerify is not null
                {
                    Result = (from DM in dbContext.DocumentMaster
                              join PM in dbContext.PropertyMaster
                              on new { DM.DocumentID, DM.SROCode } equals new { PM.DocumentID, SROCode = PM.RegSROCode }
                              join PS in dbContext.PropertySchedules
                              on new { PM.PropertyID, SROCode = PM.RegSROCode } equals new { PS.PropertyID, PS.SROCode }

                              where (
                              (model.SROList.Contains(DM.SROCode))
                              && (PS.IsThroughVerify != null)
                              && (PS.PartyIDs == null)
                              //&& (DM.Stamp5DateTime.Value.Year == Year)
                              && (Year.Contains(DM.Stamp5DateTime.Value.Year))
                              && (model.RegArticleId.Contains(DM.RegArticleCode))
                              && (PM.TotalArea > 0))

                              orderby DM.Stamp5DateTime
                              select new
                              {
                                  DM.FinalRegistrationNumber,
                                  DM.Stamp5DateTime
                              }).ToList().Count();
                }
                else
                {
                    Result = (from DM in dbContext.DocumentMaster
                              join PM in dbContext.PropertyMaster
                              on new { DM.DocumentID, DM.SROCode } equals new { PM.DocumentID, SROCode = PM.RegSROCode }
                              join PS in dbContext.PropertySchedules
                              on new { PM.PropertyID, SROCode = PM.RegSROCode } equals new { PS.PropertyID, PS.SROCode }
                              
                              where (
                              //Commented By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                              //(DM.SROCode == model.SROfficeListID)
                              //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                              //Added By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                              (model.SROList.Contains(DM.SROCode))
                              && (PS.IsThroughVerify == null)
                              //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                              && (PS.PartyIDs == null)
                              //&& (DM.Stamp5DateTime.Value.Year == Year)
                              && (Year.Contains(DM.Stamp5DateTime.Value.Year))
                              && (model.RegArticleId.Contains(DM.RegArticleCode))
                              && (PM.TotalArea > 0))
                              
                              orderby DM.Stamp5DateTime 
                              select new
                              {
                                  DM.FinalRegistrationNumber,
                                  DM.Stamp5DateTime
                              }).ToList().Count();
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

            return Result;
        }


        public ScheduleAllocationAnalysisResultModel GetScheduleAllocationAnalysisDetails(ScheduleAllocationAnalysisResponseModel model)
        {

            ScheduleAllocationAnalysisDetailsModel scheduleAllocationAnalysisDetailsModel = null;
            ScheduleAllocationAnalysisResultModel resultModel = new ScheduleAllocationAnalysisResultModel();
            resultModel.scheduleAllocationDetailsList = new List<ScheduleAllocationAnalysisDetailsModel>();
            
            
            KaveriEntities dbContext = null;

            try
            {
                dbContext = new KaveriEntities();

                //Added By ShivamB on 30-09-2022 for adding Select all option in Year textBox.
                int StartYear = 1990;
                DateTime dateTime = DateTime.Now;
                int CurrentYear = dateTime.Year;
                int YearDiff = CurrentYear - StartYear;


                int[] Year = new int[YearDiff];
                Year[0] = 1991;
                int SROfficeListID = Convert.ToInt32(model.SROfficeListID);
              

                if(model.IsSelectAllYearSelected == true)
                {
                    resultModel.Year = "All";

                    for (int i = 1; i<YearDiff; i++)
                    {
                        Year[i] = Year[i-1] + 1; 
                    }
                }
                else
                {
                    resultModel.Year = model.Year;
                    Year[0] = Convert.ToInt32(model.Year);
                }
                //Added By ShivamB on 30-09-2022 for adding Select all option in Year textBox.

                //Added By ShivamB on 30-09-2022 for adding All options in District DropDown and SRO Dropdown parameter.
                if (model.DROfficeListID == 0 && model.SROfficeListID == 0)
                {
                    resultModel.SROName = "All";
                    resultModel.DROName = "All";
                    model.SROList = dbContext.SROMaster.Select(x => x.SROCode).ToArray();
                }
                else if(model.DROfficeListID != 0 && model.SROfficeListID == 0)
                {
                    resultModel.DROName = dbContext.DistrictMaster.Where(x => x.DistrictCode == model.DROfficeListID).Select(y => y.DistrictNameE).FirstOrDefault();
                    resultModel.SROName = "All";
                    model.SROList = dbContext.SROMaster.Where(x => x.DistrictCode == model.DROfficeListID).Select(y => y.SROCode).ToArray();
                }
                else
                {
                    resultModel.DROName = dbContext.DistrictMaster.Where(x => x.DistrictCode == model.DROfficeListID).Select(y => y.DistrictNameE).FirstOrDefault();
                    resultModel.SROName = dbContext.SROMaster.Where(x => x.SROCode == model.SROfficeListID).Select(y => y.SRONameE).FirstOrDefault();
                    model.SROList = dbContext.SROMaster.Where(x => x.SROCode == SROfficeListID).Select(x => x.SROCode).ToArray();
                }
               
                //Ended By ShivamB on 30-09-2022 for adding All options in District DropDown and SRO Dropdown parameter.


                

                

              //  var Result = null;

                if (model.IsPartyIdCheckBoxSelected == true && model.IsThroughVerifyCheckBoxSelected == true)  //model.IsThroughVerifyCheckBoxSelected == true is Added by ShivamB on 30-09-2022 for checking IsThroughVerify is not null
                {
                    var Result = (from DM in dbContext.DocumentMaster
                                  join PM in dbContext.PropertyMaster
                                  on new { DM.DocumentID, DM.SROCode } equals new { PM.DocumentID, SROCode = PM.RegSROCode }
                                  join PS in dbContext.PropertySchedules
                                  on new { PM.PropertyID, SROCode = PM.RegSROCode } equals new { PS.PropertyID, PS.SROCode }
                                  
                                  orderby DM.Stamp5DateTime
                                  where (
                                  //Commented By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                                  //(DM.SROCode == model.SROfficeListID)
                                  //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                                  //Added By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                                  (model.SROList.Contains(DM.SROCode))
                                  && (PS.IsThroughVerify != null)
                                  //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                                  && (PS.PartyIDs != null)
                                  //&& (DM.Stamp5DateTime.Value.Year == Year)
                                  && (Year.Contains(DM.Stamp5DateTime.Value.Year))
                                  && (model.RegArticleId.Contains(DM.RegArticleCode))
                                  && (PM.TotalArea > 0))
                                  
                                  select new
                                  {
                                      DM.FinalRegistrationNumber,
                                      DM.Stamp5DateTime
                                  }).ToList();

                    Result = Result.Skip(model.startLen).Take(model.totalNum).ToList();

                    long SrNo = model.startLen;

                    if (Result != null)
                    {
                        foreach (var item in Result)
                        {
                            SrNo++;
                            ScheduleAllocationAnalysisDetailsModel detailsModel = new ScheduleAllocationAnalysisDetailsModel();
                            detailsModel.FinalRegistrationNumber = item.FinalRegistrationNumber;
                            detailsModel.SrNo = SrNo;
                            detailsModel.Stamp5DateTime = ((Convert.ToDateTime(item.Stamp5DateTime)).ToShortDateString()).ToString(); ;
                            resultModel.scheduleAllocationDetailsList.Add(detailsModel);
                        }
                    }
                }

                else if(model.IsPartyIdCheckBoxSelected == true && model.IsThroughVerifyCheckBoxSelected == false) //model.IsThroughVerifyCheckBoxSelected == true is Added by ShivamB on 30-09-2022 for checking IsThroughVerify is null
                {
                    var Result = (from DM in dbContext.DocumentMaster
                                  join PM in dbContext.PropertyMaster
                                  on new { DM.DocumentID, DM.SROCode } equals new { PM.DocumentID, SROCode = PM.RegSROCode }
                                  join PS in dbContext.PropertySchedules
                                  on new { PM.PropertyID, SROCode = PM.RegSROCode } equals new { PS.PropertyID, PS.SROCode }

                                  orderby DM.Stamp5DateTime
                                  where (
                                     (model.SROList.Contains(DM.SROCode))
                                     && (PS.IsThroughVerify == null)
                                     && (PS.PartyIDs != null)
                                    //&& (DM.Stamp5DateTime.Value.Year == Year)
                                     && (Year.Contains(DM.Stamp5DateTime.Value.Year))
                                     && (model.RegArticleId.Contains(DM.RegArticleCode))
                                     && (PM.TotalArea > 0)
                                  )
                                  select new
                                  {
                                      DM.FinalRegistrationNumber,
                                      DM.Stamp5DateTime
                                  }).ToList();

                    Result = Result.Skip(model.startLen).Take(model.totalNum).ToList();

                    long SrNo = model.startLen;

                    if (Result != null)
                    {
                        foreach (var item in Result)
                        {
                            SrNo++;
                            ScheduleAllocationAnalysisDetailsModel detailsModel = new ScheduleAllocationAnalysisDetailsModel();
                            detailsModel.FinalRegistrationNumber = item.FinalRegistrationNumber;
                            detailsModel.SrNo = SrNo;
                            detailsModel.Stamp5DateTime = ((Convert.ToDateTime(item.Stamp5DateTime)).ToShortDateString()).ToString(); ;
                            resultModel.scheduleAllocationDetailsList.Add(detailsModel);
                        }
                    }
                }

                else if(model.IsPartyIdCheckBoxSelected == false && model.IsThroughVerifyCheckBoxSelected == true) //model.IsThroughVerifyCheckBoxSelected == true is Added by ShivamB on 30-09-2022 for checking IsThroughVerify is not null
                {
                    var Result = (from DM in dbContext.DocumentMaster
                                  join PM in dbContext.PropertyMaster
                                  on new { DM.DocumentID, DM.SROCode } equals new { PM.DocumentID, SROCode = PM.RegSROCode }
                                  join PS in dbContext.PropertySchedules
                                  on new { PM.PropertyID, SROCode = PM.RegSROCode } equals new { PS.PropertyID, PS.SROCode }

                                  orderby DM.Stamp5DateTime
                                  where (
                                     (model.SROList.Contains(DM.SROCode))
                                     && (PS.IsThroughVerify != null)
                                     && (PS.PartyIDs == null)
                                  //&& (DM.Stamp5DateTime.Value.Year == Year)
                                  && (Year.Contains(DM.Stamp5DateTime.Value.Year))
                                     && (model.RegArticleId.Contains(DM.RegArticleCode))
                                     && (PM.TotalArea > 0)
                                  )
                                  select new
                                  {
                                      DM.FinalRegistrationNumber,
                                      DM.Stamp5DateTime
                                  }).ToList();

                    Result = Result.Skip(model.startLen).Take(model.totalNum).ToList();

                    long SrNo = model.startLen;

                    if (Result != null)
                    {
                        foreach (var item in Result)
                        {
                            SrNo++;
                            ScheduleAllocationAnalysisDetailsModel detailsModel = new ScheduleAllocationAnalysisDetailsModel();
                            detailsModel.FinalRegistrationNumber = item.FinalRegistrationNumber;
                            detailsModel.SrNo = SrNo;
                            detailsModel.Stamp5DateTime = ((Convert.ToDateTime(item.Stamp5DateTime)).ToShortDateString()).ToString(); ;
                            resultModel.scheduleAllocationDetailsList.Add(detailsModel);
                        }
                    }
                }

                else
                {
                     var Result = (from DM in dbContext.DocumentMaster
                                  join PM in dbContext.PropertyMaster
                                  on new { DM.DocumentID, DM.SROCode } equals new { PM.DocumentID, SROCode = PM.RegSROCode }
                                  join PS in dbContext.PropertySchedules
                                  on new { PM.PropertyID, SROCode = PM.RegSROCode } equals new { PS.PropertyID, PS.SROCode }

                                   orderby DM.Stamp5DateTime 
                                   where (
                                   //Commented By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                                   //(DM.SROCode == model.SROfficeListID)
                                   //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                                   //Added By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                                     (model.SROList.Contains(DM.SROCode))
                                     && (PS.IsThroughVerify == null)
                                  //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                                  && (PS.PartyIDs == null)
                                  //&& (DM.Stamp5DateTime.Value.Year == Year)
                                  && (Year.Contains(DM.Stamp5DateTime.Value.Year))
                                  && (model.RegArticleId.Contains(DM.RegArticleCode))
                                  && (PM.TotalArea > 0))
                                  
                                  select new
                                  {
                                      DM.FinalRegistrationNumber,
                                      DM.Stamp5DateTime
                                  }).ToList();

                    Result = Result.Skip(model.startLen).Take(model.totalNum).ToList();

                    long SrNo = model.startLen;

                    if (Result != null)
                    {
                        foreach (var item in Result)
                        {
                            SrNo++;
                            ScheduleAllocationAnalysisDetailsModel detailsModel = new ScheduleAllocationAnalysisDetailsModel();
                            detailsModel.FinalRegistrationNumber = item.FinalRegistrationNumber;
                            detailsModel.SrNo = SrNo;
                            detailsModel.Stamp5DateTime = ((Convert.ToDateTime(item.Stamp5DateTime)).ToShortDateString()).ToString(); ;
                            resultModel.scheduleAllocationDetailsList.Add(detailsModel);
                        }
                    }
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
            return resultModel;
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