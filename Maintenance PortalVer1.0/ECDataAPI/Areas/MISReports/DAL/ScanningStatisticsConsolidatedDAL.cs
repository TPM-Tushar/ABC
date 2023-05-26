using CustomModels.Models.MISReports.ScanningStatisticsConsolidated;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.MISReports.DAL
{
    public class ScanningStatisticsConsolidatedDAL
    {
        KaveriEntities dbContext = null;
        public ScanningStatisticsConsolidatedReqModel ScanningStatisticsConsolidatedView(int OfficeID)
        {
            ScanningStatisticsConsolidatedReqModel resModel = new ScanningStatisticsConsolidatedReqModel();

            try
            {
                dbContext = new KaveriEntities();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
               // resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                SelectListItem selectListItem = new SelectListItem();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> SROfficeList = new List<SelectListItem>();
                string FirstRecord = "All";

                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();


                short LevelID = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.LevelID).FirstOrDefault();
                int Kaveri1Code = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == OfficeID).Select(x => x.Kaveri1Code).FirstOrDefault();
                string kaveriCode = Kaveri1Code.ToString();

                resModel.SROfficeList = new List<SelectListItem>();
                resModel.DROfficeList = new List<SelectListItem>();

                if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.SR))
                {

                    string SroName = dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.SRONameE).FirstOrDefault();
                    int DroCode = Convert.ToInt32(dbContext.SROMaster.Where(x => x.SROCode == Kaveri1Code).Select(x => x.DistrictCode).FirstOrDefault());
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == DroCode).Select(x => x.DistrictNameE).FirstOrDefault();
                    string DroCode_string = Convert.ToString(DroCode);

                    sroNameItem = objCommon.GetDefaultSelectListItem(SroName, kaveriCode);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    resModel.DROfficeList.Add(droNameItem);
                    resModel.SROfficeList.Add(sroNameItem);

                }
                else if (LevelID == Convert.ToInt16(ApiCommonEnum.LevelDetails.DR))
                {
                    string DroName = dbContext.DistrictMaster.Where(x => x.DistrictCode == Kaveri1Code).Select(x => x.DistrictNameE).FirstOrDefault();

                    string DroCode_string = Convert.ToString(Kaveri1Code);
                    droNameItem = objCommon.GetDefaultSelectListItem(DroName, DroCode_string);
                    resModel.DROfficeList.Add(droNameItem);
                    resModel.SROfficeList = objCommon.GetSROfficesListByDisrictIDUsingFirstRecord(Kaveri1Code, FirstRecord);
                }
                else
                {

                    SelectListItem select = new SelectListItem();
                    select.Text = "All";
                    select.Value = "0";
                    resModel.SROfficeList.Add(select);
                    resModel.DROfficeList = objCommon.GetDROfficesList("All");

                }

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
            return resModel;
        }

        public ScanningStatisticsConsolidatedResModel GetScanningStatisticsConsolidatedDetails(ScanningStatisticsConsolidatedReqModel scanningStatisticsConsolidatedReqModel)
        {
            ScanningStatisticsConsolidatedResModel resultModel = new ScanningStatisticsConsolidatedResModel();
            resultModel.scanningStatisticsConsolidatedTablesList = new List<ScanningStatisticsConsolidatedTableModel>();
            ScanningStatisticsConsolidatedTableModel resModel = null;
            long SrCount = 1;

            int SROfficeListID = Convert.ToInt32(scanningStatisticsConsolidatedReqModel.SROfficeID);
            int DROfficeListID = Convert.ToInt32(scanningStatisticsConsolidatedReqModel.DROfficeID);
            //
            var Month = scanningStatisticsConsolidatedReqModel.DateTime_Date.Month;
            var Year = scanningStatisticsConsolidatedReqModel.DateTime_Date.Year;
            //
            //var ToDate = Convert.ToDateTime(scanningStatisticsConsolidatedReqModel.DateTime_Date).ToString("MM/yyyy");
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                // Commented and Added By Tushar on 11 April 2023 because timeout issue
                //         //
                //         #region QueryDocumentMaster
                //         var queryDocumentMaster = from SM in dbContext.ScanMaster
                //                                    join DM in dbContext.DocumentMaster

                //                                    on new { p1 = SM.DocumentID, p2 = SM.SROCode } equals new { p1 = DM.DocumentID, p2 = DM.SROCode } into DSDVDM
                //                                    from y in DSDVDM

                //                                    join sROMaster in dbContext.SROMaster
                //                                    on y.SROCode equals sROMaster.SROCode

                //                                    where

                //                                      SM.ScanDate.Value.Month == Month
                //                                      && SM.ScanDate.Value.Year == Year
                //                                      && sROMaster.DistrictCode == (DROfficeListID == 0 ? sROMaster.DistrictCode : DROfficeListID)
                //                                      && sROMaster.SROCode == (SROfficeListID == 0 ? sROMaster.SROCode : SROfficeListID)
                //                                      && SM.DocumentTypeID == 1

                //                                    select new
                //                                    {

                //                                        SROCode = SM.SROCode,
                //                                        DistrictCode = sROMaster.DistrictCode,
                //                                        Month = SM.ScanDate.Value.Month.ToString(),
                //                                        Total_S_Page = SM.Pages
                //                                    } into t1
                //                                    group t1 by t1.SROCode into g
                //                                    select new ScanningStatisticsConsolidatedTableModel
                //                                    {
                //                                        DistrictName = dbContext.DistrictMaster.Where(x => x.DistrictCode == g.FirstOrDefault().DistrictCode).Select(y => y.DistrictNameE).FirstOrDefault(),
                //                                        SROName = dbContext.SROMaster.Where(d => d.SROCode == g.FirstOrDefault().SROCode).Select(c => c.SRONameE).FirstOrDefault(),
                //                                        Month = g.FirstOrDefault().Month,
                //                                        Total_S_Page = g.Sum(x => x.Total_S_Page),
                //                                        DocType = "Document"
                //                                    };
                //         #endregion
                //         //
                //         #region QueryMarriageRegistration
                //         var queryMarriageRegistration = from SM in dbContext.ScanMaster
                //                                    join MR in dbContext.MarriageRegistration

                //                                    on new { p1 = SM.DocumentID, p2 = SM.SROCode } equals new { p1 = MR.RegistrationID, p2 = MR.PSROCode } into DSDVDM
                //                                    from y in DSDVDM

                //                                    join sROMaster in dbContext.SROMaster
                //                                    on y.PSROCode equals sROMaster.SROCode

                //                                    where

                //                                      SM.ScanDate.Value.Month == Month
                //                                      && SM.ScanDate.Value.Year == Year
                //                                      && sROMaster.DistrictCode == (DROfficeListID == 0 ? sROMaster.DistrictCode : DROfficeListID)
                //                                      && sROMaster.SROCode == (SROfficeListID == 0 ? sROMaster.SROCode : SROfficeListID)
                //                                      && SM.DocumentTypeID == 2

                //                                    select new
                //                                    {

                //                                        SROCode = SM.SROCode,
                //                                        DistrictCode = sROMaster.DistrictCode,
                //                                        Month = SM.ScanDate.Value.Month.ToString(),
                //                                        Total_S_Page = SM.Pages
                //                                    } into t1
                //                                    group t1 by t1.SROCode into g
                //                                    select new ScanningStatisticsConsolidatedTableModel
                //                                    {
                //                                        DistrictName = dbContext.DistrictMaster.Where(x => x.DistrictCode == g.FirstOrDefault().DistrictCode).Select(y => y.DistrictNameE).FirstOrDefault(),
                //                                        SROName = dbContext.SROMaster.Where(d => d.SROCode == g.FirstOrDefault().SROCode).Select(c => c.SRONameE).FirstOrDefault(),
                //                                        Month = g.FirstOrDefault().Month,
                //                                        Total_S_Page = g.Sum(x => x.Total_S_Page),
                //                                        DocType = "Marriage"
                //                                    };
                //         #endregion
                //         //
                //         #region QueryNoticeMaster
                //         var queryNoticeMaster = from SM in dbContext.ScanMaster
                //                                          join NM in dbContext.NoticeMaster

                //                                          on new { p1 = SM.DocumentID, p2 = SM.SROCode } equals new { p1 = NM.NoticeID, p2 = NM.PSROCode } into DSDVDM
                //                                          from y in DSDVDM

                //                                          join sROMaster in dbContext.SROMaster
                //                                          on y.PSROCode equals sROMaster.SROCode

                //                                          where

                //                                            SM.ScanDate.Value.Month == Month
                //                                            && SM.ScanDate.Value.Year == Year
                //                                            && sROMaster.DistrictCode == (DROfficeListID == 0 ? sROMaster.DistrictCode : DROfficeListID)
                //                                            && sROMaster.SROCode == (SROfficeListID == 0 ? sROMaster.SROCode : SROfficeListID)
                //                                            && SM.DocumentTypeID == 5

                //                                          select new
                //                                          {

                //                                              SROCode = SM.SROCode,
                //                                              DistrictCode = sROMaster.DistrictCode,
                //                                              Month = SM.ScanDate.Value.Month.ToString(),
                //                                              Total_S_Page = SM.Pages
                //                                          } into t1
                //                                          group t1 by t1.SROCode into g
                //                                          select new ScanningStatisticsConsolidatedTableModel
                //                                          {
                //                                              DistrictName = dbContext.DistrictMaster.Where(x => x.DistrictCode == g.FirstOrDefault().DistrictCode).Select(y => y.DistrictNameE).FirstOrDefault(),
                //                                              SROName = dbContext.SROMaster.Where(d => d.SROCode == g.FirstOrDefault().SROCode).Select(c => c.SRONameE).FirstOrDefault(),
                //                                              Month = g.FirstOrDefault().Month,
                //                                              Total_S_Page = g.Sum(x => x.Total_S_Page),
                //                                              DocType = "Marriage"
                //                                          };
                //         #endregion
                //         //
                //         #region QueryFirmMaster
                //         var queryFirmMaster = from SFSM in dbContext.SocietyFirmScanmaster
                //                                join FM in dbContext.FirmMaster

                //                                on new { p1 = SFSM.RegistrationID, p2 = SFSM.DRCode } equals new { p1 = (long)FM.RegistrationID, p2 = FM.DRCode } into DSDVDM
                //                                from y in DSDVDM

                //                                join districtMaster in dbContext.DistrictMaster
                //                                on y.DRCode equals districtMaster.DistrictCode

                //                                where

                //                                  SFSM.ScanDate.Value.Month == Month
                //                                  && SFSM.ScanDate.Value.Year == Year
                //                                  && districtMaster.DistrictCode == (DROfficeListID == 0 ? districtMaster.DistrictCode : DROfficeListID)
                //                                  && SFSM.IsSociety == false


                //                                select new
                //                                {

                //                                    // SROCode = DSDV.SROCode,
                //                                    DistrictCode = districtMaster.DistrictCode,
                //                                    Month = SFSM.ScanDate.Value.Month.ToString(),
                //                                    Total_S_Page = SFSM.Pages
                //                                } into t1
                //                                group t1 by t1.DistrictCode into g
                //                                select new ScanningStatisticsConsolidatedTableModel
                //                                {
                //                                    DistrictName = dbContext.DistrictMaster.Where(x => x.DistrictCode == g.FirstOrDefault().DistrictCode).Select(y => y.DistrictNameE).FirstOrDefault(),
                //                                    SROName = "--",
                //                                    Month = g.FirstOrDefault().Month,
                //                                    Total_S_Page = g.Sum(x => x.Total_S_Page),
                //                                    DocType = "Firm"
                //                                };
                //         #endregion
                //         //
                //         //var Result = queryDocumentMaster.Union(queryMarriageRegistration).Union(queryNoticeMaster).Union(queryFirmMaster).Select(x => new { x.SROName, x.DistrictName, x.Month,x.Total_S_Page }).OrderBy(x => x.DistrictName).GroupBy(x=>x.SROName).ToList();
                //           var ResultMarriageRegistrationAndNotice = queryMarriageRegistration.Union(queryNoticeMaster).GroupBy(x => x.SROName)
                //                          .Select(x => new ScanningStatisticsConsolidatedTableModel { DistrictName = x.FirstOrDefault().DistrictName, SROName = x.FirstOrDefault().SROName,  Month = x.FirstOrDefault().Month, Total_S_Page = x.Sum(g => g.Total_S_Page), DocType = x.FirstOrDefault().DocType })
                //                           ;
                //         //var Result = queryDocumentMaster.Union(queryMarriageRegistration).Union(queryNoticeMaster).Union(queryFirmMaster)
                //var Result = queryDocumentMaster.Union(ResultMarriageRegistrationAndNotice).Union(queryFirmMaster)
                //             //.GroupBy( x=>x.SROName)
                //             //.Select(x => new { SROName=x.FirstOrDefault().SROName, DistrictName= x.FirstOrDefault().DistrictName, Month=x.FirstOrDefault().Month, Total_S_Page = x.Sum(g => g.Total_S_Page) })
                //               .Select(x => new  { SROName = x.SROName, DistrictName = x.DistrictName, Month = x.Month, Total_S_Page = x.Total_S_Page, DocType = x.DocType })
                //             .OrderBy(z=>z.DistrictName)
                //             .ToList();
                //         //var Result = queryDocumentMaster.Union(queryMarriageRegistration).Union(queryNoticeMaster).Union(queryFirmMaster).OrderBy(x => x.DistrictName).ThenBy(y=>y.SROName).ToList();
                string MonthFullName = getFullName(Convert.ToInt32(Month));
                var Result = dbContext.USP_RPT_SCANNING_CONSOLIDATED(DROfficeListID, SROfficeListID, Month, Year).ToList();


                if (Result != null)
                {
                    foreach (var item in Result)
                    {

                        resModel = new ScanningStatisticsConsolidatedTableModel();
                        resModel.srNo = SrCount++;
                        resModel.DistrictName = item.DistrictName == "" ? "--" : item.DistrictName;
                        resModel.SROName = item.SRONAMEE == "" ? "--" : item.SRONAMEE;
                        resModel.Month = MonthFullName+" - "+ Year;
                        resModel.Total_S_Page = item.PAGECOUNT;
                        resModel.DocType = string.IsNullOrEmpty(item.DocumentType) ? "--" : item.DocumentType;



                        resultModel.scanningStatisticsConsolidatedTablesList.Add(resModel);


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
                {
                    dbContext.Dispose();
                }
            }
            return resultModel;
        }

       public string getFullName(int month)
        {
            return System.Globalization.CultureInfo.CurrentCulture.
                DateTimeFormat.GetMonthName
                (month);
        }
    }
}