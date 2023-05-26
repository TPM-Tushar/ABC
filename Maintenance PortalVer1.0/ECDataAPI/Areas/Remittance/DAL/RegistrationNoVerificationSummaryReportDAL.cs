#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationNoVerificationSummaryReportDAL.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL Layer for Registration No Verification Summary Report .

*/
#endregion

using CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class RegistrationNoVerificationSummaryReportDAL
    {

        public RegistrationNoVerificationSummaryReportModel RegistrationNoVerificationSummaryReportView()
        {
            RegistrationNoVerificationSummaryReportModel resModel = new RegistrationNoVerificationSummaryReportModel();

            try
            {
                DateTime Now = DateTime.Now;
                var startDate = new DateTime(Now.Year, Now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                var GetDocumentTypeList = objCommon.GetDocumentType();
                //GetDocumentTypeList.RemoveRange(3, 2);
				//Commented and Added By Tushar on 28 Dec 2022
                GetDocumentTypeList.RemoveRange(3, 1);
                resModel.DocumentType = GetDocumentTypeList;
            }
            catch (Exception)
            {

                throw;
            }
            return resModel;
        }

        public RegistrationNoVerificationSummaryResultModel GetSummaryReportDetails(RegistrationNoVerificationSummaryReportModel registrationNoVerificationSummaryReportModel)
        {
            RegistrationNoVerificationSummaryResultModel resultModel = new RegistrationNoVerificationSummaryResultModel();
            resultModel.registrationNoVerificationSummaryTableList = new List<RegistrationNoVerificationSummaryTableModel>();
            RegistrationNoVerificationSummaryTableModel resModel = null;
            long SrCount = 1;
            var DocumentIDFinal = 15;
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();

                //Added By Tushar on 28 Dec 2022 for Document Type Firm 

                if (registrationNoVerificationSummaryReportModel.DocumentTypeId == 4)
                {
                    //1)
                    #region FirmResult_LA_CNA
                    var FirmResult_LA_CNA = (from FDCD in dbContext.FirmDataCentralizationDetails
                                             join FM in dbContext.FirmMaster

                                             on new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } equals new { p1 = (long)FM.RegistrationID, p2 = FM.DRCode } into FDCDFM
                                             from y in FDCDFM.DefaultIfEmpty()

                                             where

                                             FDCD.DateOfRegistration >= registrationNoVerificationSummaryReportModel.DateTime_Date
                                             && FDCD.DateOfRegistration < registrationNoVerificationSummaryReportModel.DateTime_ToDate
                                             && y.RegistrationID == null

                                             select new RegistrationNoVerificationFirmSummaryTableModelResult
                                             {
                                                 RegistrationID = FDCD.RegistrationID,
                                                 DroCode = FDCD.DROCode,
                                                 Type = "LA_CNA",
                                                 DateOfRegistration = FDCD.DateOfRegistration
                                             }).ToList();
                    #endregion
                    //2)
                    #region FirmResult_CA_LNA
                    var FirmResult_CA_LNA = (from FDCD in dbContext.FirmMaster
                                             join FM in dbContext.FirmDataCentralizationDetails

                                             on new { p1 = (long)FDCD.RegistrationID, p2 = FDCD.DRCode } equals new { p1 = FM.RegistrationID, p2 = FM.DROCode } into FDCDFM
                                             from y in FDCDFM.DefaultIfEmpty()



                                             where

                                             FDCD.DateOfRegistration >= registrationNoVerificationSummaryReportModel.DateTime_Date
                                             && FDCD.DateOfRegistration <= registrationNoVerificationSummaryReportModel.DateTime_ToDate
                                             && y.RegistrationID == null
                                             select new RegistrationNoVerificationFirmSummaryTableModelResult
                                             {
                                                 RegistrationID = (long)FDCD.RegistrationID,
                                                 DroCode = FDCD.DRCode,
                                                 Type = "CA_LNA",
                                                 DateOfRegistration = FDCD.DateOfRegistration
                                             }).ToList();
                    #endregion
                    //3) 
                    #region FirmResult_FN_Miss
                    var FirmResult_FN_Miss = (from FDCD in dbContext.FirmMaster
                                              join FM in dbContext.FirmDataCentralizationDetails

                                              on new { p1 = (long)FDCD.RegistrationID, p2 = FDCD.DRCode } equals new { p1 = FM.RegistrationID, p2 = FM.DROCode }


                                              where
                                               FDCD.DateOfRegistration >= registrationNoVerificationSummaryReportModel.DateTime_Date
                                              && FDCD.DateOfRegistration <= registrationNoVerificationSummaryReportModel.DateTime_ToDate
                                              && FDCD.FirmNumber != FM.FirmNumber


                                              select new RegistrationNoVerificationFirmSummaryTableModelResult
                                              {
                                                  RegistrationID = (long)FDCD.RegistrationID,
                                                  DroCode = FDCD.DRCode,
                                                  Type = "FN_Miss",
                                                  DateOfRegistration = FDCD.DateOfRegistration

                                              }).ToList();
                    #endregion
                    //4)
                    #region FirmResult_SC_LA_CNA
                    var FirmResult_SC_LA_CNA = (from FDCD in dbContext.FirmDataCentralizationDetails
                                                join FM in dbContext.FirmMaster
                                                on new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } equals new { p1 = (long)FM.RegistrationID, p2 = FM.DRCode } into e
                                                from c in e.DefaultIfEmpty()



                                                join SFUD in dbContext.ScannedFileUploadDetails

                                                on new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } equals new { p1 = SFUD.DocumentID, p2 = -SFUD.SROCode } into FDCDFM
                                                from y in FDCDFM.DefaultIfEmpty()

                                                where
                                                FDCD.DateOfRegistration >= registrationNoVerificationSummaryReportModel.DateTime_Date
                                                && FDCD.DateOfRegistration < registrationNoVerificationSummaryReportModel.DateTime_ToDate
                                                && y.DocumentID == null && FDCD.ScanFileName != "" && FDCD.ScanFileName != null

                                                select new RegistrationNoVerificationFirmSummaryTableModelResult
                                                {
                                                    RegistrationID = FDCD.RegistrationID,
                                                    DroCode = FDCD.DROCode,
                                                    Type = "SC_LA_CNA",
                                                    DateOfRegistration = FDCD.DateOfRegistration

                                                }).ToList();
                    #endregion

                    //5)
                    #region FirmResult_SC_CA_LNA
                    var FirmResult_SC_CA_LNA = (from SFUD in dbContext.ScannedFileUploadDetails
                                                join FM in dbContext.FirmMaster
                                                on new { p1 = SFUD.DocumentID, p2 = -SFUD.SROCode } equals new { p1 = (long)FM.RegistrationID, p2 = FM.DRCode } into e
                                                from c in e.DefaultIfEmpty()

                                                join FDCD in dbContext.FirmDataCentralizationDetails

                                                on new { p1 = SFUD.DocumentID, p2 = -SFUD.SROCode } equals new { p1 = FDCD.RegistrationID, p2 = FDCD.DROCode } into FDCDFM
                                                from y in FDCDFM.DefaultIfEmpty()

                                                where

                                                SFUD.UploadDateTime >= registrationNoVerificationSummaryReportModel.DateTime_Date
                                                && SFUD.UploadDateTime < registrationNoVerificationSummaryReportModel.DateTime_ToDate
                                                && y.RegistrationID == null && SFUD.DocumentTypeID == 7

                                                select new RegistrationNoVerificationFirmSummaryTableModelResult
                                                {

                                                    RegistrationID = SFUD.DocumentID,
                                                    DroCode = -SFUD.SROCode,
                                                    Type = "SC_CA_LNA",
                                                    DateOfRegistration = (DateTime?)SFUD.UploadDateTime,

                                                }).ToList();
                    #endregion
                    //6)

                    #region FirmResult_SC_FN_Miss
                    var QueryFirmResult_SC_FN_Miss = @"Select RegistrationID, DroCode,Type,DateOfRegistration
                    from(
                    SELECT
                    Distinct OBJ1.*, OBJ2.*
                    FROM
                    (
                    SELECT
                    FDCD.RegistrationID,
                    FDCD.DateOfRegistration as DateOfRegistration,
                    FDCD.DROCode as DroCode,
                    'SC_FN_Miss' as Type,

                    (Select CASE ISNULL(FDCD.ScanFileName, '') WHEN '' THEN '' ELSE Reverse(Substring(REVERSE(LTRIM(RTRIM(FDCD.ScanFileName))), Charindex('.', REVERSE(LTRIM(RTRIM(FDCD.ScanFileName)))) + 1, CharIndex('\',REVERSE(LTRIM(RTRIM(FDCD.ScanFileName))))-5)) END ScannedFileName) Local_ScanFileName
                    FROM
                    KaveriDR.FirmDataCentralizationDetails FDCD
                    left outer join
                    KaveriDR.FirmMaster SM ON SM.RegistrationID = FDCD.RegistrationID and SM.DRCode = FDCD.DROCode
                    where 
                    CONVERT(date, FDCD.DateOfRegistration) >= '" + registrationNoVerificationSummaryReportModel.DateTime_Date.ToString("yyyy/MM/dd") + @"' and
                    CONVERT(date, FDCD.DateOfRegistration) < '" + registrationNoVerificationSummaryReportModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
                    )OBJ1
                    inner JOIN
                    (
                    SELECT(Select  CASE CHARINDEX('.', ISNULL(scannedfilename, '')) When 0 THEN '' ELSE SUBSTRING(scannedfilename, 1, CHARINDEX('.', scannedfilename) - 1) END Scannedfilename) as Central_ScanFileName, UploadDateTime, DocumentID, SROCode as OBJ2SROCODE, DocumentTypeID FROM ScannedFileUploadDetails
                    where DocumentTypeID = 7
                    )OBJ2
                    ON
                    OBJ1.RegistrationID = OBJ2.DocumentID
                    and OBJ1.DROCode = -OBJ2.OBJ2SROCODE
                    where OBJ1.Local_ScanFileName != OBJ2.Central_ScanFileName 
                    and OBJ2.DocumentTypeID = 7
                    and CONVERT(date, OBJ1.DateOfRegistration) >= '" + registrationNoVerificationSummaryReportModel.DateTime_Date.ToString("yyyy/MM/dd") + @"' and
                    CONVERT(date, OBJ1.DateOfRegistration) < '" + registrationNoVerificationSummaryReportModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
                    )OBJ order by OBJ.UploadDateTime desc";



                    var QueryResult_FirmResult_SC_FN_Miss = GetFirmCentralizationReportResult(QueryFirmResult_SC_FN_Miss);


                    #endregion


                    var DistrictList = dbContext.DistrictMaster.Select(x => new { DistrictCode = x.DistrictCode }).ToList();
  
                    var QueryResultList = QueryResult_FirmResult_SC_FN_Miss.registrationNoVerificationFirmSummaryTableModelResults.Concat(FirmResult_CA_LNA).Concat(FirmResult_FN_Miss).Concat(FirmResult_SC_LA_CNA).Concat(FirmResult_SC_CA_LNA).Concat(FirmResult_LA_CNA).ToList();


                    var Result = DistrictList.GroupJoin(QueryResultList.Where(z=> (z.DateOfRegistration >= registrationNoVerificationSummaryReportModel.DateTime_Date.Date
                      && z.DateOfRegistration <= (registrationNoVerificationSummaryReportModel.DateTime_ToDate.Date))),
                    SROMasterResult => SROMasterResult.DistrictCode, RPT_DocReg_NoCLDetailsResult => RPT_DocReg_NoCLDetailsResult.DroCode,
                           (m, n) => new { SROMasterResult = m, RPT_DocReg_NoCLDetailsResult = n})
                           .SelectMany(
                          x => x.RPT_DocReg_NoCLDetailsResult.DefaultIfEmpty(),
                        (m, n) => new {
                            DistrictCode = m.SROMasterResult.DistrictCode,
                            FirmResult_LA_CNA = m.RPT_DocReg_NoCLDetailsResult.Count(s => s.Type == "LA_CNA"),
                            FirmResult_CA_LNA = m.RPT_DocReg_NoCLDetailsResult.Count(s => s.Type == "CA_LNA"),
                            FirmResult_FN_Miss = m.RPT_DocReg_NoCLDetailsResult.Count(s => s.Type == "FN_Miss"),
                            FirmResult_SC_LA_CNA = m.RPT_DocReg_NoCLDetailsResult.Count(s => s.Type == "SC_LA_CNA"),
                            FirmResult_SC_CA_LNA = m.RPT_DocReg_NoCLDetailsResult.Count(s => s.Type == "SC_CA_LNA"),
                            FirmResult_SC_FN_Miss = m.RPT_DocReg_NoCLDetailsResult.Count(s => s.Type == "SC_FN_Miss"),

                        })
                        .GroupBy(f => f.DistrictCode).Select(a => new {
                           
                            DistrictCode = a.FirstOrDefault().DistrictCode,
                            FirmResult_LA_CNA_Count = a.FirstOrDefault().FirmResult_LA_CNA,
                            FirmResult_CA_LNA_Count = a.FirstOrDefault().FirmResult_CA_LNA,
                            FirmResult_FN_Miss_Count = a.FirstOrDefault().FirmResult_FN_Miss,
                            FirmResult_SC_LA_CNA_Count = a.FirstOrDefault().FirmResult_SC_LA_CNA,
                            FirmResult_SC_CA_LNA_Count = a.FirstOrDefault().FirmResult_SC_CA_LNA,
                            FirmResult_SC_FN_Miss_Count = a.FirstOrDefault().FirmResult_SC_FN_Miss

                        }).Select(x=>new
                        {
                             DistrictName = dbContext.DistrictMaster.Where(d => d.DistrictCode == x.DistrictCode).Select(c => c.DistrictNameE).FirstOrDefault(),
                            FirmResult_LA_CNA_Count = x.FirmResult_LA_CNA_Count,
                            FirmResult_CA_LNA_Count = x.FirmResult_CA_LNA_Count,
                            FirmResult_FN_Miss_Count = x.FirmResult_FN_Miss_Count,
                            FirmResult_SC_LA_CNA_Count = x.FirmResult_SC_LA_CNA_Count,
                            FirmResult_SC_CA_LNA_Count = x.FirmResult_SC_CA_LNA_Count,
                            FirmResult_SC_FN_Miss_Count = x.FirmResult_SC_FN_Miss_Count
                        }).OrderByDescending(s=>s.FirmResult_SC_LA_CNA_Count)
                      .ToList();

                    if (Result != null)
                    {
                        foreach (var item in Result)
                        {

                            resModel = new RegistrationNoVerificationSummaryTableModel();
                            resModel.srNo = SrCount++;
                            resModel.DistrictName = string.IsNullOrEmpty(item.DistrictName) ? "--" : item.DistrictName;
                            resModel.FirmResult_CA_LNA_Count = item.FirmResult_CA_LNA_Count;
                            resModel.FirmResult_FN_Miss_Count = item.FirmResult_FN_Miss_Count;
                            resModel.FirmResult_LA_CNA_Count = item.FirmResult_LA_CNA_Count;
                            resModel.FirmResult_SC_CA_LNA_Count = item.FirmResult_SC_CA_LNA_Count;
                            resModel.FirmResult_SC_LA_CNA_Count = item.FirmResult_SC_LA_CNA_Count;
                            resModel.FirmResult_SC_FN_Miss_Count = item.FirmResult_SC_FN_Miss_Count;
                            resultModel.registrationNoVerificationSummaryTableList.Add(resModel);


                        }
                    }


                }
               //End By Tushar on 28 Dec 2022
                 else
                {
                var Result = dbContext.SROMaster.GroupJoin(dbContext.RPT_DocReg_NoCLDetails.Where(z => z.DocumentTypeID == registrationNoVerificationSummaryReportModel.DocumentTypeId && z.DocumentID >= DocumentIDFinal
                      && ((z.C_Stamp5DateTime == null ? z.L_Stamp5DateTime : z.C_Stamp5DateTime) >= registrationNoVerificationSummaryReportModel.DateTime_Date.Date
                      && (z.C_Stamp5DateTime == null ? z.L_Stamp5DateTime : z.C_Stamp5DateTime) <= (registrationNoVerificationSummaryReportModel.DateTime_ToDate.Date))),
                    SROMasterResult => SROMasterResult.SROCode, RPT_DocReg_NoCLDetailsResult => RPT_DocReg_NoCLDetailsResult.SROCode,
                           (x, y) => new { SROMasterResult = x, RPT_DocReg_NoCLDetailsResult = y })
                           .SelectMany(
                          x => x.RPT_DocReg_NoCLDetailsResult.DefaultIfEmpty(),
                        (x, y) => new { SroCode = x.SROMasterResult.SROCode,
                            ErrorType1 = x.RPT_DocReg_NoCLDetailsResult.Where(l => l.BatchID != null).Count(s => s.ErrorType.Contains("1")),
                            ErrorType2 = x.RPT_DocReg_NoCLDetailsResult.Where(l => l.BatchID != null).Count(s => s.ErrorType.Contains("2")),
                            ErrorType3 = x.RPT_DocReg_NoCLDetailsResult.Where(l => l.BatchID != null).Count(s => s.ErrorType.Contains("3")),
                            ErrorType4 = x.RPT_DocReg_NoCLDetailsResult.Where(l => l.BatchID != null).Count(s => s.ErrorType.Contains("4")),
                            ErrorType5 = x.RPT_DocReg_NoCLDetailsResult.Where(l => l.BatchID != null).Count(s => s.ErrorType.Contains("5")),
                            ErrorType6 = x.RPT_DocReg_NoCLDetailsResult.Where(l => l.BatchID != null).Count(s => s.ErrorType.Contains("6")),
                            ErrorType7 = x.RPT_DocReg_NoCLDetailsResult.Where(l => l.BatchID != null).Count(s => s.ErrorType.Contains("7")),
                            IsDuplicate = x.RPT_DocReg_NoCLDetailsResult.Count(s => s.IsDuplicate == true)
                        })
                        .GroupBy(x => x.SroCode).Select(a => new {
                            SROName = dbContext.SROMaster.Where(d => d.SROCode == a.FirstOrDefault().SroCode).Select(c => c.SRONameE).FirstOrDefault(),
                            ERRORType1Count = a.FirstOrDefault().ErrorType1,
                            ERRORType2Count = a.FirstOrDefault().ErrorType2,
                            ERRORType3Count = a.FirstOrDefault().ErrorType3,
                            ERRORType4Count = a.FirstOrDefault().ErrorType4,
                            ERRORType5Count = a.FirstOrDefault().ErrorType5,
                            ERRORType6Count = a.FirstOrDefault().ErrorType6,
                            ERRORType7Count = a.FirstOrDefault().ErrorType7,
                            IsDuplicateCount = a.FirstOrDefault().IsDuplicate
                        }).OrderByDescending(a=>a.IsDuplicateCount)
                      .ToList();
       
                if (Result != null)
                {
                    foreach (var item in Result)
                    {

                        resModel = new RegistrationNoVerificationSummaryTableModel();
                        resModel.srNo = SrCount++;
                        resModel.SROName = string.IsNullOrEmpty(item.SROName) ? "--" : item.SROName;
                        resModel.M_M = item.ERRORType1Count;
                        resModel.L_Missing = item.ERRORType2Count;
                        resModel.L_Additional = item.ERRORType3Count;
                        resModel.LP_CNP = item.ERRORType4Count;
                        resModel.CNP_LNP = item.ERRORType5Count;
                        resModel.CP_LNP = item.ERRORType6Count;
                        resModel.SM_M = item.ERRORType7Count;
                        resModel.Is_Duplicate = item.IsDuplicateCount;

                        resultModel.registrationNoVerificationSummaryTableList.Add(resModel);


                    }
                }
          }

            }
            catch(Exception ex)
            {
                throw;
            }finally
            {
                if(dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            return resultModel;
        }


        //Added By Tushar on 28 Dec 2022
        public RegistrationNoVerificationSummaryResultModel GetFirmCentralizationReportResult(string query)
        {
            RegistrationNoVerificationFirmSummaryTableModelResult tableModel = null;
            RegistrationNoVerificationSummaryResultModel firmCentralizationResultModel = new RegistrationNoVerificationSummaryResultModel();
            firmCentralizationResultModel.registrationNoVerificationFirmSummaryTableModelResults = new List<RegistrationNoVerificationFirmSummaryTableModelResult>();
            KaveriEntities dbContext = new KaveriEntities();
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
                                tableModel = new RegistrationNoVerificationFirmSummaryTableModelResult();


                                tableModel.RegistrationID = Convert.ToInt64(item["RegistrationID"]);
                                tableModel.DroCode = Convert.ToInt32(item["DROCode"]);
                                tableModel.Type = item["Type"].ToString();
                                tableModel.DateOfRegistration = Convert.ToDateTime(item["DateOfRegistration"]);
                         
                                firmCentralizationResultModel.registrationNoVerificationFirmSummaryTableModelResults.Add(tableModel);
                            }
                        }

            

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
        //End By Tushar on 28 Dec 2022

    }
}