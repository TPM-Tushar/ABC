#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   NotReadableDocDAL.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL Layer for Not Readable Documents Details.

*/
#endregion

using CustomModels.Models.Remittance.NotReadableDoc;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class NotReadableDocDAL : INotReadableDoc
    {
        KaveriEntities dbContext = null;
        public NotReadableDocModel NotReadableDocView(int OfficeID)
        {
            NotReadableDocModel resModel = new NotReadableDocModel();

             try
            {
                dbContext = new KaveriEntities();
                SelectListItem selectListItem = new SelectListItem();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> SROfficeList = new List<SelectListItem>();
                string FirstRecord = "All";
               
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
               
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
     
        public NotReadableDocResultModel GetNotReadableDocDetails(NotReadableDocModel notReadableDocModel)
        {

            NotReadableDocResultModel resultModel = new NotReadableDocResultModel();
            resultModel.notReadableDocTableModelList = new List<NotReadableDocTableModel>();
            NotReadableDocTableModel resModel = null;
            long SrCount = 1;

		   int SROfficeListID = Convert.ToInt32(notReadableDocModel.SROfficeID);
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
              
             
               // if (notReadableDocModel.SROfficeID != 0)
                //{
                    var queryDocumentScanDocVerifyLog = from DSDV in dbContext.DocumentScanDocVerifyLog
                                join DM in dbContext.DocumentMaster
                               // on DSDV.DocumentID equals DM.DocumentID
                                on new { p1 = DSDV.DocumentID, p2 = DSDV.SROCode } equals new { p1 = DM.DocumentID, p2 = DM.SROCode } into DSDVDM
                               from y in DSDVDM
                        
                              join sROMaster in dbContext.SROMaster
                              on DSDV.SROCode equals sROMaster.SROCode
                       
                              where
                                // DSDV.SROCode == notReadableDocModel.SROfficeID
                                 DSDV.IsSuccess == false
                            
                                && sROMaster.DistrictCode == (notReadableDocModel.DROfficeID == 0 ? sROMaster.DistrictCode : notReadableDocModel.DROfficeID)
                                && sROMaster.SROCode == (SROfficeListID == 0 ? sROMaster.SROCode : SROfficeListID)
                            
                               select new
                                {
                                    RegistrationNumber = y.FinalRegistrationNumber,
                                    LogDateTime = DSDV.LogDateTime,
                                    CDNumber = y.CDNumber,
                                    Document_Type = "Document",
                                    LogBy = DSDV.LogBy,
                                    //Added By Tushar on 20 Dec 2022
                                    Stamp5DateTimeD = y.Stamp5DateTime.ToString()
                                    //End By Tushar on 20 Dec 2022
                                 
                                };
                    var queryMarriageScanDocVerifyLog = from MSDV in dbContext.MarriageScanDocVerifyLog
                                join MR in dbContext.MarriageRegistration
                                //on MSDV.RegistrationID equals MR.RegistrationID
                                on new { p1 = MSDV.RegistrationID, p2 = MSDV.SROCode } equals new { p1 = MR.RegistrationID, p2 = MR.PSROCode } into DSDVDM
                                from y in DSDVDM
                                join sROMaster in dbContext.SROMaster
                                on y.PSROCode equals sROMaster.SROCode
                                where
                               //  MSDV.SROCode == notReadableDocModel.SROfficeID
                                 MSDV.IsSuccess == false
                            
                               && sROMaster.DistrictCode == (notReadableDocModel.DROfficeID == 0 ? sROMaster.DistrictCode : notReadableDocModel.DROfficeID)
                              && sROMaster.SROCode == (SROfficeListID == 0 ? sROMaster.SROCode : SROfficeListID)
                         
                               select new
                                {
                                    RegistrationNumber = y.MarriageCaseNo,
                                    LogDateTime = MSDV.LogDateTime,
                                    CDNumber = y.CDNumber,
                                    Document_Type = "Marriage",
                                    LogBy = MSDV.LogBy,
                                  //Added By Tushar on 20 Dec 2022
                                    Stamp5DateTimeD = y.DateOfRegistration.ToString()
                                  //End By Tushar on 20 Dec 2022

                                };

                    var queryNoticeScanDocVerifyLog = from NSDV in dbContext.NoticeScanDocVerifyLog
                                                      join NM in dbContext.NoticeMaster
                                                      // on NSDV.NoticeID equals NM.NoticeID
                                                      on new { p1 = NSDV.NoticeID, p2 = NSDV.SROCode } equals new { p1 = NM.NoticeID, p2 = NM.PSROCode } into DSDVDM
                                                      from y in DSDVDM
                                                     
                                                 
                                                      join sROMaster in dbContext.SROMaster
                                                      on y.PSROCode equals sROMaster.SROCode
                                                     
                                               
                                                      where
                                                        // NSDV.SROCode == notReadableDocModel.SROfficeID
                                                       NSDV.IsSuccess == false
                                                   
                                                       && sROMaster.DistrictCode == (notReadableDocModel.DROfficeID == 0 ? sROMaster.DistrictCode : notReadableDocModel.DROfficeID)
                                                       && sROMaster.SROCode == (SROfficeListID == 0 ? sROMaster.SROCode : SROfficeListID)
                                                  
                                                      select new
                                                      {
                                                          RegistrationNumber = y.NoticeNo,
                                                          LogDateTime = NSDV.LogDateTime,
                                                          CDNumber = y.CDNumber,
                                                          Document_Type = "Notice",
                                                          LogBy = NSDV.LogBy,
                                                          //Added By Tushar on 20 Dec 2022
                                                          Stamp5DateTimeD = y.NoticeIssuedDate.ToString()
                                                          //End By Tushar on 20 Dec 2022

                                                      };

                    var DetailsList = queryDocumentScanDocVerifyLog.Union(queryMarriageScanDocVerifyLog).Union(queryNoticeScanDocVerifyLog).OrderByDescending(x=>x.LogDateTime).ToList();

                    if (DetailsList != null)
                    {
                        foreach (var item in DetailsList)
                        {

                            resModel = new NotReadableDocTableModel();
                            resModel.srNo = SrCount++;
                        //Commented and Added By Tushar on 20 Dec 2022 for search filter error
                        //resModel.RegistrationNumber = item.RegistrationNumber == "" ? "--": item.RegistrationNumber;
                        //resModel.LogDateTime = item.LogDateTime.ToString("dd/MM/yyyy");
                        //resModel.CDNumber = item.CDNumber == "" ? "--" : item.CDNumber;
                        //resModel.Document_Type = item.Document_Type == "" ? "--" : item.Document_Type;
                        //resModel.LogBy = (item.LogBy == "" || item.LogBy == null) ? "--" : item.LogBy; ;
                        resModel.RegistrationNumber =string.IsNullOrEmpty(item.RegistrationNumber)  ? "--" : item.RegistrationNumber;
                        resModel.LogDateTime =string.IsNullOrEmpty(item.LogDateTime.ToString())?"--": item.LogDateTime.ToString("dd/MM/yyyy  HH:mm:ss");
                        resModel.CDNumber =string.IsNullOrEmpty(item.CDNumber) ? "--" : item.CDNumber;
                        resModel.Document_Type =string.IsNullOrEmpty(item.Document_Type)  ? "--" : item.Document_Type;
                        resModel.LogBy = string.IsNullOrEmpty(item.LogBy) ? "--" : item.LogBy;
                        //End By Tushar on 20 Dec 2022
                        //Added By Tushar on 20 Dec 2022
                        resModel.Stamp5DateTime = string.IsNullOrEmpty(item.Stamp5DateTimeD) ? "--" :Convert.ToDateTime(item.Stamp5DateTimeD).ToString("dd/MM/yyyy  HH:mm:ss");
                        //End By Tushar on 20 Dec 2022

                            resultModel.notReadableDocTableModelList.Add(resModel);


                        }
                    }
                //}

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
      
    }

 
    }