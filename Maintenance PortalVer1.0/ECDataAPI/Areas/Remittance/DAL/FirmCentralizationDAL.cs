using CustomModels.Models.Remittance.FirmCentralization;
using ECDataAPI.Entity.KaveriEntities;
using ECDataUI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class FirmCentralizationDAL
    {
        public FirmCentralizationModel FirmCentralizationView()
        {
            try
            {
                FirmCentralizationModel resModel = new FirmCentralizationModel();
                resModel.DROfficeList = new List<SelectListItem>();
                //
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //
                List<DistrictMaster> DROMasterList = new List<DistrictMaster>();
                CommonFunctions objCommon = new CommonFunctions();
                using (KaveriEntities dbContext = new KaveriEntities())
                {
                    //Updated by Rushikesh on 2 March 2023 -- changed select to 0
                    resModel.DROfficeList.Add(objCommon.GetDefaultSelectListItem("All", "0"));
                    DROMasterList = dbContext.DistrictMaster.ToList();
                    if (DROMasterList != null)
                    {
                        DROMasterList = DROMasterList.OrderBy(x => x.DistrictNameE).ToList();
                        foreach (var item in DROMasterList)
                        {
                            SelectListItem select = new SelectListItem();
                            select.Text = item.DistrictNameE;
                            select.Value = item.DistrictCode.ToString();
                            resModel.DROfficeList.Add(select);
                        }
                    }
                }
                return resModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public FirmCentralizationResultModel GetFirmCentralizationDetails(FirmCentralizationModel firmCentralizationModel)
        {

            FirmCentralizationResultModel resultModel = new FirmCentralizationResultModel();
            resultModel.DetailsList = new List<FirmCentralizationTableModel>();
            FirmCentralizationTableModel resModel = null;
            long SrCount = 1;
            KaveriEntities dbContext = null;
            
            try
            {
                dbContext = new KaveriEntities();
                var Result = new List<FirmCentralizationTableModel>();

                //var temp = firmCentralizationModel.DROfficeID;
                var temp1 = firmCentralizationModel.SearchBy;

                //Added by Rushikesh 2 March 2023
                if (firmCentralizationModel.SearchBy == null || firmCentralizationModel.SearchBy == "")
                {

                }
                //End by Rushikesh on 2 March 2023

                else if (firmCentralizationModel.DROfficeID != 0)
                {

                    switch (firmCentralizationModel.SearchBy)
                    {
                        case "LA_CNA":
                            {
                                #region FirmDataCentralizationDetails Left Outer Join FirmMaster
                                Result = (from FDCD in dbContext.FirmDataCentralizationDetails
                                          join FM in dbContext.FirmMaster

                                      on new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } equals new { p1 = (long)FM.RegistrationID, p2 = FM.DRCode } into FDCDFM
                                      from y in FDCDFM.DefaultIfEmpty()





                                      where
                                      FDCD.DROCode == firmCentralizationModel.DROfficeID
                                      && FDCD.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                                      && FDCD.DateOfRegistration < firmCentralizationModel.DateTime_ToDate
                                      && y.RegistrationID == null
                              
                                      select new FirmCentralizationTableModel
                                      {
                                          RegistrationID = FDCD.RegistrationID,
                                          L_FirmNumber = FDCD.FirmNumber,
                                          L_CDNumber = FDCD.CDNumber,
                              
                                          L_DateOfRegistration = FDCD.DateOfRegistration.ToString(),
                                          C_DateOfRegistration = y.DateOfRegistration.ToString(),

                                          L_DateOfRegistrationDate = FDCD.DateOfRegistration,
                                          //C_DateOfRegistrationDate = y.DateOfRegistration,

                                          C_FirmNumber = y.FirmNumber,
                                          C_CDNumber = y.CDNumber,
                                          L_ScanFileName = FDCD.ScanFileName
                                

                                      }).OrderByDescending(x=>x.L_DateOfRegistrationDate).ToList();
                            #endregion

                     
                            break;
                        }
                    case "CA_LNA":
                        {
                            #region FirmMaster Left Outer Join FirmDataCentralizationDetails
                            Result = (from FDCD in dbContext.FirmMaster
                                      join FM in dbContext.FirmDataCentralizationDetails

                                      on new { p1 = (long)FDCD.RegistrationID, p2 = FDCD.DRCode } equals new { p1 = FM.RegistrationID, p2 = FM.DROCode } into FDCDFM
                                      from y in FDCDFM.DefaultIfEmpty()


                                      
                                      where
                                      FDCD.DRCode == firmCentralizationModel.DROfficeID
                                      && FDCD.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                                      && FDCD.DateOfRegistration <= firmCentralizationModel.DateTime_ToDate
                                      && y.RegistrationID == null
                                      select new FirmCentralizationTableModel
                                      {
                                          RegistrationID = (long)FDCD.RegistrationID,
                                          L_FirmNumber = y.FirmNumber,
                                          L_CDNumber = y.CDNumber,
                                          C_FirmNumber = FDCD.FirmNumber,
                                          C_CDNumber = FDCD.CDNumber,
                                     
                                          C_DateOfRegistration = FDCD.DateOfRegistration.ToString(),
                                          L_DateOfRegistration = y.DateOfRegistration.ToString(),
                                          C_DateOfRegistrationDate = FDCD.DateOfRegistration,
                                          L_ScanFileName = y.ScanFileName
                                      }).OrderByDescending(x=>x.C_DateOfRegistrationDate).ToList();
                            #endregion
                            break;
                        }
                    case "FN_Miss":
                        {
                            #region  FirmMaster.FirmNumber != FM.FirmDataCentralizationDetails
                            Result = (from FDCD in dbContext.FirmMaster
                                      join FM in dbContext.FirmDataCentralizationDetails

                                      on new { p1 = (long)FDCD.RegistrationID, p2 = FDCD.DRCode } equals new { p1 = FM.RegistrationID, p2 = FM.DROCode }


                                      where
                                      FDCD.DRCode == firmCentralizationModel.DROfficeID
                                      && FDCD.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                                      && FDCD.DateOfRegistration <= firmCentralizationModel.DateTime_ToDate
                                      && FDCD.FirmNumber != FM.FirmNumber


                                      select new FirmCentralizationTableModel
                                      {
                                          RegistrationID = (long)FDCD.RegistrationID,
                                          L_FirmNumber = FM.FirmNumber,
                                          L_CDNumber = FM.CDNumber,
                                          C_FirmNumber = FDCD.FirmNumber,
                                          C_CDNumber = FDCD.CDNumber,
                                        
                                          C_DateOfRegistration = FDCD.DateOfRegistration.ToString(),
                                          L_DateOfRegistration = FM.DateOfRegistration.ToString(),
                                          C_DateOfRegistrationDate = FDCD.DateOfRegistration,
                                          L_ScanFileName = FM.ScanFileName
                                      }).OrderByDescending(x=>x.C_DateOfRegistrationDate).ToList();

                            #endregion
                            break;
                        }
                    case "SC_LA_CNA":
                        {
                

                            Result = (from FDCD in dbContext.FirmDataCentralizationDetails
                                      join FM in dbContext.FirmMaster
                                      on new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } equals new { p1 = (long)FM.RegistrationID, p2 = FM.DRCode } into e
                                      from c in e.DefaultIfEmpty()



                                      join SFUD in dbContext.ScannedFileUploadDetails

                                      on new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } equals new { p1 = SFUD.DocumentID, p2 = -SFUD.SROCode } into FDCDFM
                                      from y in FDCDFM.DefaultIfEmpty()





                                      where
                                      FDCD.DROCode == firmCentralizationModel.DROfficeID
                                      && FDCD.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                                      && FDCD.DateOfRegistration < firmCentralizationModel.DateTime_ToDate
                                      && y.DocumentID == null && FDCD.ScanFileName != "" && FDCD.ScanFileName != null
                                     
                                      select new FirmCentralizationTableModel
                                      {
                                          RegistrationID = FDCD.RegistrationID,
                                          L_FirmNumber = FDCD.FirmNumber,
                                          L_CDNumber = FDCD.CDNumber,
                               
                                          L_DateOfRegistration = FDCD.DateOfRegistration.ToString(),
                                          C_DateOfRegistration = c.DateOfRegistration.ToString(),
                                          L_DateOfRegistrationDate = FDCD.DateOfRegistration,
                                          C_FirmNumber =c.FirmNumber,
                                          C_CDNumber = c.CDNumber,
                                          L_ScanFileName = FDCD.ScanFileName,
                                          C_ScanFileName = y.ScannedFileName,
                                          UploadDateTime = y.UploadDateTime.ToString()
                                          
                                  

                                      }).OrderByDescending(x=>x.L_DateOfRegistrationDate).ToList();

                     
                            

                            break;
                        }
                    case "SC_CA_LNA":
                        {
                            #region ScannedFileUploadDetails Left Outer Join FirmDataCentralizationDetails


                      
                            Result = (from SFUD in dbContext.ScannedFileUploadDetails
                                      join FM in dbContext.FirmMaster
                                      on new { p1 = SFUD.DocumentID, p2 = -SFUD.SROCode } equals new { p1 = (long)FM.RegistrationID, p2 = FM.DRCode } into e
                                      from c in e.DefaultIfEmpty()



                                      join FDCD in dbContext.FirmDataCentralizationDetails

                                      on new { p1 = SFUD.DocumentID, p2 = -SFUD.SROCode } equals new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } into FDCDFM
                                      from y in FDCDFM.DefaultIfEmpty()





                                      where
                                      SFUD.SROCode == -firmCentralizationModel.DROfficeID
                                      && SFUD.UploadDateTime >= firmCentralizationModel.DateTime_FromDate
                                      && SFUD.UploadDateTime < firmCentralizationModel.DateTime_ToDate
                                      && y.RegistrationID == null && SFUD.DocumentTypeID == 7
                                      
                                      select new FirmCentralizationTableModel
                                      {
                                          RegistrationID = SFUD.DocumentID,
                                          L_FirmNumber = y.FirmNumber,
                                          L_CDNumber = y.CDNumber,
                                    
                                          L_DateOfRegistration = y.DateOfRegistration.ToString(),
                                          C_DateOfRegistration = c.DateOfRegistration.ToString(),
                           
                                          C_FirmNumber = c.FirmNumber,
                                          C_CDNumber = c.CDNumber,
                                          L_ScanFileName = y.ScanFileName,
                                          C_ScanFileName = SFUD.ScannedFileName,
                                          UploadDateTime = SFUD.UploadDateTime.ToString(),
                                          UploadDateTimeDate = SFUD.UploadDateTime,


                                      }).OrderByDescending(x=>x.UploadDateTime).ToList();
                        

                          
                            #endregion
                            break;
                        }
                    case "SC_FN_Miss":
                        {
                            #region SQL Query
                            string query = @"Select RegistrationID, Local_FirmNumber, Local_Date_of_Registration, Local_CD_Number, Local_ScanFileName,
Central_FirmNumber, Central_CDNumber, Central_ScanFileName, Central_DateOfRegistration, UploadDateTime
from(
SELECT
Distinct OBJ1.*, OBJ2.*
FROM
(
SELECT
FDCD.RegistrationID,
FDCD.FirmNumber as Local_FirmNumber,
FDCD.DateOfRegistration as Local_Date_of_Registration,
FDCD.CDNumber as Local_CD_Number,
SM.FirmNumber as Central_FirmNumber,
SM.CDNumber as Central_CDNumber,
SM.DateOfRegistration as Central_DateOfRegistration,
FDCD.DROCode,


(Select CASE ISNULL(FDCD.ScanFileName, '') WHEN '' THEN '' ELSE Reverse(Substring(REVERSE(LTRIM(RTRIM(FDCD.ScanFileName))), Charindex('.', REVERSE(LTRIM(RTRIM(FDCD.ScanFileName)))) + 1, CharIndex('\',REVERSE(LTRIM(RTRIM(FDCD.ScanFileName))))-5)) END ScannedFileName) Local_ScanFileName
FROM
KaveriDR.FirmDataCentralizationDetails FDCD
left outer join
KaveriDR.FirmMaster SM ON SM.RegistrationID = FDCD.RegistrationID and SM.DRCode = FDCD.DROCode
where FDCD.DROCode =" + firmCentralizationModel.DROfficeID + @"  and
CONVERT(date, FDCD.DateOfRegistration) >= '" + firmCentralizationModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"' and
CONVERT(date, FDCD.DateOfRegistration) < '" + firmCentralizationModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
)OBJ1
inner JOIN
(
SELECT(Select  CASE CHARINDEX('.', ISNULL(scannedfilename, '')) When 0 THEN '' ELSE SUBSTRING(scannedfilename, 1, CHARINDEX('.', scannedfilename) - 1) END Scannedfilename) as Central_ScanFileName, UploadDateTime, DocumentID, SROCode as OBJ2SROCODE, DocumentTypeID FROM ScannedFileUploadDetails
where DocumentTypeID = 7 and SROCode =" + -firmCentralizationModel.DROfficeID + @"
)OBJ2
ON
OBJ1.RegistrationID = OBJ2.DocumentID
and OBJ1.DROCode = -OBJ2.OBJ2SROCODE
where OBJ1.Local_ScanFileName != OBJ2.Central_ScanFileName and OBJ1.DROCode =" + firmCentralizationModel.DROfficeID + @"
and OBJ2.DocumentTypeID = 7
and CONVERT(date, OBJ1.Local_Date_of_Registration)  >= '" + firmCentralizationModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"' and
CONVERT(date, OBJ1.Local_Date_of_Registration) < '" + firmCentralizationModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
)OBJ order by OBJ.UploadDateTime desc";
                                #endregion

                                var QueryResult = GetFirmCentralizationReportResult(query);
                                if (QueryResult != null)
                                {
                                    Result = QueryResult.DetailsList;
                                }

                                break;
                            }
                    }
                }
                //Added by Rushikesh 2 March 2023
               //If DRO Office Code == 0 or For ALL
                else
                {
                    switch (firmCentralizationModel.SearchBy)
                    {
                        case "LA_CNA":
                            {
                                #region FirmDataCentralizationDetails Left Outer Join FirmMaster
                                Result = (from FDCD in dbContext.FirmDataCentralizationDetails
                                          join FM in dbContext.FirmMaster

                                          on new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } equals new { p1 = (long)FM.RegistrationID, p2 = FM.DRCode } into FDCDFM
                                          from y in FDCDFM.DefaultIfEmpty()

                                          where
                                          
                                          FDCD.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                                          && FDCD.DateOfRegistration < firmCentralizationModel.DateTime_ToDate
                                          && y.RegistrationID == null

                                          select new FirmCentralizationTableModel
                                          {
                                              RegistrationID = FDCD.RegistrationID,
                                              L_FirmNumber = FDCD.FirmNumber,
                                              L_CDNumber = FDCD.CDNumber,

                                              L_DateOfRegistration = FDCD.DateOfRegistration.ToString(),
                                              C_DateOfRegistration = y.DateOfRegistration.ToString(),

                                              L_DateOfRegistrationDate = FDCD.DateOfRegistration,
                                              //C_DateOfRegistrationDate = y.DateOfRegistration,

                                              C_FirmNumber = y.FirmNumber,
                                              C_CDNumber = y.CDNumber,
                                              L_ScanFileName = FDCD.ScanFileName


                                          }).OrderByDescending(x => x.L_DateOfRegistrationDate).ToList();
                                #endregion


                                break;
                            }
                        case "CA_LNA":
                            {
                                #region FirmMaster Left Outer Join FirmDataCentralizationDetails
                                Result = (from FDCD in dbContext.FirmMaster
                                          join FM in dbContext.FirmDataCentralizationDetails

                                          on new { p1 = (long)FDCD.RegistrationID, p2 = FDCD.DRCode } equals new { p1 = FM.RegistrationID, p2 = FM.DROCode } into FDCDFM
                                          from y in FDCDFM.DefaultIfEmpty()



                                          where

                                          FDCD.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                                          && FDCD.DateOfRegistration <= firmCentralizationModel.DateTime_ToDate
                                          && y.RegistrationID == null
                                          
                                          select new FirmCentralizationTableModel
                                          {
                                              RegistrationID = (long)FDCD.RegistrationID,
                                              L_FirmNumber = y.FirmNumber,
                                              L_CDNumber = y.CDNumber,
                                              C_FirmNumber = FDCD.FirmNumber,
                                              C_CDNumber = FDCD.CDNumber,

                                              C_DateOfRegistration = FDCD.DateOfRegistration.ToString(),
                                              L_DateOfRegistration = y.DateOfRegistration.ToString(),
                                              C_DateOfRegistrationDate = FDCD.DateOfRegistration,
                                              L_ScanFileName = y.ScanFileName
                                          }).OrderByDescending(x => x.C_DateOfRegistrationDate).ToList();
                                #endregion
                                break;
                            }
                        case "FN_Miss":
                            {
                                #region  FirmMaster.FirmNumber != FM.FirmDataCentralizationDetails
                                Result = (from FDCD in dbContext.FirmMaster
                                          join FM in dbContext.FirmDataCentralizationDetails

                                          on new { p1 = (long)FDCD.RegistrationID, p2 = FDCD.DRCode } equals new { p1 = FM.RegistrationID, p2 = FM.DROCode }


                                          where
                                          
                                          FDCD.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                                          && FDCD.DateOfRegistration <= firmCentralizationModel.DateTime_ToDate
                                          && FDCD.FirmNumber != FM.FirmNumber


                                          select new FirmCentralizationTableModel
                                          {
                                              RegistrationID = (long)FDCD.RegistrationID,
                                              L_FirmNumber = FM.FirmNumber,
                                              L_CDNumber = FM.CDNumber,
                                              C_FirmNumber = FDCD.FirmNumber,
                                              C_CDNumber = FDCD.CDNumber,

                                              C_DateOfRegistration = FDCD.DateOfRegistration.ToString(),
                                              L_DateOfRegistration = FM.DateOfRegistration.ToString(),
                                              C_DateOfRegistrationDate = FDCD.DateOfRegistration,
                                              L_ScanFileName = FM.ScanFileName
                                          }).OrderByDescending(x => x.C_DateOfRegistrationDate).ToList();

                                #endregion
                                break;
                            }
                        case "SC_LA_CNA":
                            {
                                Result = (from FDCD in dbContext.FirmDataCentralizationDetails
                                          join FM in dbContext.FirmMaster
                                          on new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } equals new { p1 = (long)FM.RegistrationID, p2 = FM.DRCode } into e
                                          from c in e.DefaultIfEmpty()



                                          join SFUD in dbContext.ScannedFileUploadDetails

                                          on new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } equals new { p1 = SFUD.DocumentID, p2 = -SFUD.SROCode } into FDCDFM
                                          from y in FDCDFM.DefaultIfEmpty()





                                          where
                                          
                                          FDCD.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                                          && FDCD.DateOfRegistration < firmCentralizationModel.DateTime_ToDate
                                          && y.DocumentID == null && FDCD.ScanFileName != "" && FDCD.ScanFileName != null

                                          select new FirmCentralizationTableModel
                                          {
                                              RegistrationID = FDCD.RegistrationID,
                                              L_FirmNumber = FDCD.FirmNumber,
                                              L_CDNumber = FDCD.CDNumber,

                                              L_DateOfRegistration = FDCD.DateOfRegistration.ToString(),
                                              C_DateOfRegistration = c.DateOfRegistration.ToString(),
                                              L_DateOfRegistrationDate = FDCD.DateOfRegistration,
                                              C_FirmNumber = c.FirmNumber,
                                              C_CDNumber = c.CDNumber,
                                              L_ScanFileName = FDCD.ScanFileName,
                                              C_ScanFileName = y.ScannedFileName,
                                              UploadDateTime = y.UploadDateTime.ToString()



                                          }).OrderByDescending(x => x.L_DateOfRegistrationDate).ToList();




                                break;
                            }
                        case "SC_CA_LNA":
                            {
                                #region ScannedFileUploadDetails Left Outer Join FirmDataCentralizationDetails



                                Result = (from SFUD in dbContext.ScannedFileUploadDetails
                                          join FM in dbContext.FirmMaster
                                          on new { p1 = SFUD.DocumentID, p2 = -SFUD.SROCode } equals new { p1 = (long)FM.RegistrationID, p2 = FM.DRCode } into e
                                          from c in e.DefaultIfEmpty()



                                          join FDCD in dbContext.FirmDataCentralizationDetails

                                          on new { p1 = SFUD.DocumentID, p2 = -SFUD.SROCode } equals new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } into FDCDFM
                                          from y in FDCDFM.DefaultIfEmpty()





                                          where
                                         
                                          SFUD.UploadDateTime >= firmCentralizationModel.DateTime_FromDate
                                          && SFUD.UploadDateTime < firmCentralizationModel.DateTime_ToDate
                                          && y.RegistrationID == null && SFUD.DocumentTypeID == 7

                                          select new FirmCentralizationTableModel
                                          {
                                              RegistrationID = SFUD.DocumentID,
                                              L_FirmNumber = y.FirmNumber,
                                              L_CDNumber = y.CDNumber,

                                              L_DateOfRegistration = y.DateOfRegistration.ToString(),
                                              C_DateOfRegistration = c.DateOfRegistration.ToString(),

                                              C_FirmNumber = c.FirmNumber,
                                              C_CDNumber = c.CDNumber,
                                              L_ScanFileName = y.ScanFileName,
                                              C_ScanFileName = SFUD.ScannedFileName,
                                              UploadDateTime = SFUD.UploadDateTime.ToString(),
                                              UploadDateTimeDate = SFUD.UploadDateTime,


                                          }).OrderByDescending(x => x.UploadDateTime).ToList();



                                #endregion
                                break;
                            }
                        case "SC_FN_Miss":
                            {
                                #region SQL Query
                                string query = @"Select RegistrationID, Local_FirmNumber, Local_Date_of_Registration, Local_CD_Number, Local_ScanFileName,
Central_FirmNumber, Central_CDNumber, Central_ScanFileName, Central_DateOfRegistration, UploadDateTime
from(
SELECT
Distinct OBJ1.*, OBJ2.*
FROM
(
SELECT
FDCD.RegistrationID,
FDCD.FirmNumber as Local_FirmNumber,
FDCD.DateOfRegistration as Local_Date_of_Registration,
FDCD.CDNumber as Local_CD_Number,
SM.FirmNumber as Central_FirmNumber,
SM.CDNumber as Central_CDNumber,
SM.DateOfRegistration as Central_DateOfRegistration,
FDCD.DROCode,


(Select CASE ISNULL(FDCD.ScanFileName, '') WHEN '' THEN '' ELSE Reverse(Substring(REVERSE(LTRIM(RTRIM(FDCD.ScanFileName))), Charindex('.', REVERSE(LTRIM(RTRIM(FDCD.ScanFileName)))) + 1, CharIndex('\',REVERSE(LTRIM(RTRIM(FDCD.ScanFileName))))-5)) END ScannedFileName) Local_ScanFileName
FROM
KaveriDR.FirmDataCentralizationDetails FDCD
left outer join
KaveriDR.FirmMaster SM ON SM.RegistrationID = FDCD.RegistrationID and SM.DRCode = FDCD.DROCode
where
CONVERT(date, FDCD.DateOfRegistration) >= '" + firmCentralizationModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"' and
CONVERT(date, FDCD.DateOfRegistration) < '" + firmCentralizationModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
)OBJ1
inner JOIN
(
SELECT(Select  CASE CHARINDEX('.', ISNULL(scannedfilename, '')) When 0 THEN '' ELSE SUBSTRING(scannedfilename, 1, CHARINDEX('.', scannedfilename) - 1) END Scannedfilename) as Central_ScanFileName, UploadDateTime, DocumentID, SROCode as OBJ2SROCODE, DocumentTypeID FROM ScannedFileUploadDetails
where DocumentTypeID = 7
)OBJ2
ON
OBJ1.RegistrationID = OBJ2.DocumentID
and OBJ1.DROCode = -OBJ2.OBJ2SROCODE
where OBJ1.Local_ScanFileName != OBJ2.Central_ScanFileName and OBJ2.DocumentTypeID = 7
and CONVERT(date, OBJ1.Local_Date_of_Registration)  >= '" + firmCentralizationModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"' and
CONVERT(date, OBJ1.Local_Date_of_Registration) < '" + firmCentralizationModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
)OBJ order by OBJ.UploadDateTime desc";
                                #endregion

                                var QueryResult = GetFirmCentralizationReportResult(query);
                                if (QueryResult != null)
                                {
                                    Result = QueryResult.DetailsList;
                                }

                                break;
                            }


                    }
                }
                //End by Rushikesh on 2 March 2023
                if (Result != null)
                {
                    foreach (var item in Result)
                    {
                        resModel = new FirmCentralizationTableModel();
                        resModel.Sr_No = SrCount++;
                        resModel.RegistrationID = Convert.ToInt64(item.RegistrationID);
                        resModel.L_FirmNumber = String.IsNullOrEmpty(item.L_FirmNumber) ? "--" : item.L_FirmNumber;
                        resModel.L_CDNumber = string.IsNullOrEmpty(item.L_CDNumber) ? "--" : item.L_CDNumber;
                        resModel.C_FirmNumber = string.IsNullOrEmpty(item.C_FirmNumber) ? "--" : item.C_FirmNumber;
                        resModel.C_CDNumber = String.IsNullOrEmpty(item.C_CDNumber) ? "--" : item.C_CDNumber;
                       
                        resModel.L_DateOfRegistration = item.L_DateOfRegistration == "" ? "--" : Convert.ToDateTime(item.L_DateOfRegistration).ToString("dd/MM/yyyy");
                        resModel.C_DateOfRegistration = item.C_DateOfRegistration == "" ? "--" : Convert.ToDateTime(item.C_DateOfRegistration).ToString("dd/MM/yyyy");

                        resModel.L_ScanFileName = string.IsNullOrEmpty(item.L_ScanFileName) ? "--" : GetScanFileName(item.L_ScanFileName);
                        resModel.C_ScanFileName = string.IsNullOrEmpty(item.C_ScanFileName) ? "--" : item.C_ScanFileName.Replace(".enc","").Trim();
                        resModel.UploadDateTime =string.IsNullOrEmpty(item.UploadDateTime) ? "--" : Convert.ToDateTime(item.UploadDateTime).ToString("dd/MM/yyyy");
                        resultModel.DetailsList.Add(resModel);
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

        //Get data for Excel
        public FirmCentralizationResultModel GetFirmCentralizationLocalDetails(FirmCentralizationModel firmCentralizationModel)
        {

            FirmCentralizationResultModel resultModel = new FirmCentralizationResultModel();
            resultModel.Local_DetailsList = new List<LocalFirmCentralizationTableModel>();
            LocalFirmCentralizationTableModel resModel = null;
            long SrCount = 1;
            KaveriEntities dbContext = null;
            var temp = firmCentralizationModel.DROfficeID;
            
            try
            {
                dbContext = new KaveriEntities();
                var Result = new List<FirmDataCentralizationDetails>();

                if (firmCentralizationModel.DROfficeID != 0)
                {

                   Result = dbContext.FirmDataCentralizationDetails.Where(x => x.DROCode == firmCentralizationModel.DROfficeID &&
                              x.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                              && x.DateOfRegistration < firmCentralizationModel.DateTime_ToDate).OrderByDescending(z => z.DateOfRegistration).ToList();
                }

                //Added by Rushikesh on 2 March 2023
                else
                {
                    Result = dbContext.FirmDataCentralizationDetails.Where(x => x.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                             && x.DateOfRegistration < firmCentralizationModel.DateTime_ToDate).OrderByDescending(z => z.DateOfRegistration).ToList();
                }
                //End by Rushikesh on 2 March 2023

                if (Result != null)
                {
                    foreach (var item in Result)
                    {

                        resModel = new LocalFirmCentralizationTableModel();
                        resModel.Sr_No = SrCount++;
                        resModel.RegistrationID = item.RegistrationID;
                        resModel.DROCode = item.DROCode;
                        resModel.FirmNumber =string.IsNullOrEmpty(item.FirmNumber)?"--":item.FirmNumber;
                        resModel.IsFirmDataCentralizaed = item.IsFirmDataCentralizaed;
                        resModel.CDNumber =string.IsNullOrEmpty(item.CDNumber)?"--":item.CDNumber;
                        resModel.DateOfRegistration = item.DateOfRegistration.ToString() == "" ? "--" : Convert.ToDateTime(item.DateOfRegistration).ToString("dd/MM/yyyy");
                        resModel.IsScanDocumentUploaded = item.IsScanDocumentUploaded == null ? false : item.IsScanDocumentUploaded;
                        resModel.ScanFileName =string.IsNullOrEmpty(item.ScanFileName)?"--":GetScanFileName(item.ScanFileName);
                        resModel.CDID = String.IsNullOrEmpty(item.CDID.ToString()) ? "--" : (item.CDID.ToString());
                        resModel.ScanDateTime = item.ScanDateTime.ToString() == "" ? "--" : Convert.ToDateTime(item.ScanDateTime).ToString("dd/MM/yyyy");
                        resultModel.Local_DetailsList.Add(resModel);
                    }
                }

                return resultModel;
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
        }

        public FirmCentralizationResultModel GetFirmCentralizationCentralDetails(FirmCentralizationModel firmCentralizationModel)
        {

            FirmCentralizationResultModel resultModel = new FirmCentralizationResultModel();
            resultModel.Central_DetailsList = new List<CentralFirmCentralizationTableModel>();
            CentralFirmCentralizationTableModel resModel = null;
            long SrCount = 1;
            KaveriEntities dbContext = null;
            var temp = firmCentralizationModel.DROfficeID;
            var Result = new List<FirmMaster>();
            try
            {
                dbContext = new KaveriEntities();

                if (firmCentralizationModel.DROfficeID != 0)
                {

                    Result = dbContext.FirmMaster.Where(x => x.DRCode == firmCentralizationModel.DROfficeID &&
                              x.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                              && x.DateOfRegistration < firmCentralizationModel.DateTime_ToDate).OrderByDescending(z => z.DateOfRegistration).ToList();
                }

                //Added by Rushikesh on 2 March 2023
                else
                {
                    Result = dbContext.FirmMaster.Where(x => x.DateOfRegistration >= firmCentralizationModel.DateTime_FromDate
                             && x.DateOfRegistration < firmCentralizationModel.DateTime_ToDate).OrderByDescending(z => z.DateOfRegistration).ToList();
                }
                //End by Rushikesh on 2 March 2023

                if (Result != null)
                {
                    foreach (var item in Result)
                    {

                        resModel = new CentralFirmCentralizationTableModel();
                        resModel.Sr_No = SrCount++;
                        resModel.RegistrationID = (long)item.RegistrationID;
                        resModel.DRCode = item.DRCode;
                        resModel.FirmNumber =string.IsNullOrEmpty(item.FirmNumber)?"--":item.FirmNumber;
                        resModel.Pages = item.Pages == null ? 0 : item.Pages;
                        resModel.IsScanned = item.IsScanned == null ?0:item.IsScanned;
                        resModel.Remarks = string.IsNullOrEmpty(item.Remarks)?"--":item.Remarks;
                        resModel.ReceiptID = item.ReceiptID == null ?0:item.ReceiptID;
                        resModel.FirmType =string.IsNullOrEmpty(item.FirmType)?"--":item.FirmType;
                
                        resModel.CDNumber =string.IsNullOrEmpty(item.CDNumber)?"--":item.CDNumber;
                        resModel.DateOfRegistration = item.DateOfRegistration.ToString() == "" ? "--" : Convert.ToDateTime(item.DateOfRegistration).ToString("dd/MM/yyyy");

                        resultModel.Central_DetailsList.Add(resModel);
                    }
                }

                return resultModel;
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
        }

        public string GetScanFileName(string ScanFileName)
        {
            try
            {
                if (ScanFileName.Contains(".enc"))
                {
                    var Result = new string(ScanFileName.ToCharArray().Reverse().ToArray());
                    var TempResultFirst = Result.IndexOf("\\");
                    var TempResultSecond = Result.Remove(TempResultFirst + 1);
                    var Res = new string(TempResultSecond.ToCharArray().Reverse().ToArray());
                    var ScanFileNameResult = Res.Replace("\\", "").Replace(".enc", "").Trim();
                    return ScanFileNameResult; 
                }else
                {
                    return ScanFileName;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public FirmCentralizationResultModel GetFirmCentralizationReportResult(string query)
        {
            FirmCentralizationTableModel tableModel = null;
            FirmCentralizationResultModel firmCentralizationResultModel = new FirmCentralizationResultModel();
            firmCentralizationResultModel.DetailsList = new List<FirmCentralizationTableModel>();
            KaveriEntities dbContext =new KaveriEntities();
            try
            {
               
                using (SqlConnection connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {

                            DataTable data = new DataTable();
                            data.Load(dataReader);

                            foreach (DataRow item in data.Rows)
                            {
                                tableModel = new FirmCentralizationTableModel();


                                tableModel.RegistrationID =Convert.ToInt64(item["RegistrationID"]);
                                tableModel.L_FirmNumber = item["Local_FirmNumber"].ToString();
                                tableModel.L_DateOfRegistration = item["Local_Date_of_Registration"].ToString();
                                tableModel.L_ScanFileName = item["Local_ScanFileName"].ToString();
                        
                                tableModel.C_FirmNumber = item["Central_FirmNumber"].ToString();
                                tableModel.C_CDNumber = item["Central_CDNumber"].ToString();
                                tableModel.C_ScanFileName = item["Central_ScanFileName"].ToString();
                                tableModel.C_DateOfRegistration = item["Central_DateOfRegistration"].ToString();
                                tableModel.UploadDateTime = item["UploadDateTime"].ToString();

                                firmCentralizationResultModel.DetailsList.Add(tableModel);
                            }
                        }

                        //

                        else
                        {

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
                {
                    dbContext.Dispose();
                }
            }

            return firmCentralizationResultModel;
        }
    }

}