using CustomModels.Models.Remittance.MarriageAnalysisReport;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class MarriageAnalysisReportDAL : IMarriageAnalysisReport
    {
        KaveriEntities dbContext = null;
        public List<MarriageAnalysisReportTableModel> GetMarriageAnalysisReportsDetails(MarriageAnalysisReportModel reportModel)
        {
            //Added Temp
           // reportModel.SROfficeID = 182;
            //End Temp
            MarriageAnalysisReportTableModel tableModel = null;
            List<MarriageAnalysisReportTableModel> tableList = new List<MarriageAnalysisReportTableModel>();
            try
            {
                long SrCount = 1;
                dbContext = new KaveriEntities();
                int SROfficeListID = Convert.ToInt32(reportModel.SROfficeID);
                var Result = new List<MarriageAnalysisReportTableModel>();
                // true for NULL
                if (reportModel.BRIDEPersonID == false && reportModel.BRIDEGroomPersonID == false && reportModel.WitnessCount == false && reportModel.ReceiptCount == false)
                {
                    Result = (from marriageRegistration in dbContext.MarriageRegistration

                                  //join personDetails in dbContext.PersonDetails
                                  //on marriageRegistration.PSROCode equals personDetails.PSROCode
                                  //join sROMaster in dbContext.SROMaster
                                  //on personDetails.PSROCode equals sROMaster.SROCode
                                  //join personDetails in dbContext.PersonDetails
                              join sROMaster in dbContext.SROMaster
                             on marriageRegistration.PSROCode equals sROMaster.SROCode
                              //join sROMaster in dbContext.SROMaster
                              //on personDetails.PSROCode equals sROMaster.SROCode

                              where
                          //sROMaster.SROCode == reportModel.SROfficeID
                          sROMaster.DistrictCode == (reportModel.DROfficeID == 0 ? sROMaster.DistrictCode : reportModel.DROfficeID)
                      && sROMaster.SROCode == (SROfficeListID == 0 ? sROMaster.SROCode : SROfficeListID)

                           &&  marriageRegistration.DateOfRegistration >= reportModel.DateTime_FromDate
                             && marriageRegistration.DateOfRegistration <= reportModel.DateTime_ToDate

                              select new MarriageAnalysisReportTableModel
                              {
                                  SroName = sROMaster.SRONameE,
                                  MarriageCaseNo = marriageRegistration.MarriageCaseNo,
                                  RegistrationID = marriageRegistration.RegistrationID,
                                  Bride = (long?)marriageRegistration.Bride == null ? 0 : (long)marriageRegistration.Bride,
                                  BrideGroom = (long?)marriageRegistration.BrideGroom == null ? 0 : (long)marriageRegistration.BrideGroom

                              }

                                 ).Distinct().ToList();

                    if (Result != null)
                    {
                        foreach (var item in Result)
                        {
                            tableModel = new MarriageAnalysisReportTableModel();

                            tableModel.srNo = SrCount++;


                            tableModel.SroName = String.IsNullOrEmpty(item.SroName) ? "-" : item.SroName;
                            tableModel.MarriageCaseNo = String.IsNullOrEmpty(item.MarriageCaseNo) ? "-" : item.MarriageCaseNo;

                            tableModel.RegistrationID = item.RegistrationID;
                            tableModel.Bride = (long?)item.Bride == null ? 0 : (long)item.Bride;
                            tableModel.BrideGroom = (long?)item.BrideGroom == null ? 0 : (long)item.BrideGroom;
                            tableList.Add(tableModel);
                        }
                    }
                }
                else if(reportModel.BRIDEPersonID == true && reportModel.BRIDEGroomPersonID == true)
                {
                    #region BRIDEPersonID = NULL AND BRIDEGroomPersonID = NULL
                    string query = @"Select PSROCode,(Select SRONameE from SROMaster where SROCode = PSROCode ) SROName, MarriageCaseNo,RegistrationID, Bride,BrideGroom,OBJ2PersonID BRIDEPersonID,OBJ3PersonID BRIDEGroomPersonID
    from (
    SELECT
    Distinct OBJ1.*,OBJ2.OBJ2PersonID,OBJ3.OBJ3PersonID
    FROM
    (

    SELECT
    MarriageCaseNo,
    REGISTRATIONID,
    PSROCode,
    Bride,
    BrideGroom,
    convert(date,DateOfRegistration) as RegistrationDate,
    case
    when month(DateOfRegistration) >3 then year(DateOfRegistration)
    WHEN MONTH(DateOfRegistration) <= 3 THEN year(DateOfRegistration)-1
    END AS FINANCIALYEAR
    FROM
    ECDATA.DBO.MarriageRegistration MR
    inner join
    ecdata.DBO.SROMASTER SM ON SM.SROCODE = MR.PSROCode
    WHERE
    --PSROCODE = " + reportModel.SROfficeID + @"
PSROCODE = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
     AND SM.DistrictCode = CASE  " + reportModel.DROfficeID + @" WHEN 0 THEN SM.DistrictCode ELSE " + reportModel.DROfficeID + @" END 
	AND SM.SROCode = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
    AND DateOfRegistration IS NOT NULL
    -- AND DateOfRegistration >= " + reportModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"
    -- AND DateOfRegistration < " + reportModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"
  AND CONVERT(date, DateOfRegistration) >= '" + reportModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"'
  AND CONVERT(date, DateOfRegistration) < '" + reportModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
    )OBJ1
    Left Outer JOIN
    (
  -- SELECT PersonID OBJ2PersonID,PSROCODE as OBJ2PSROCODE FROM PersonDetails where PSROCODE =" + reportModel.SROfficeID + @"
   SELECT PersonID OBJ2PersonID,PSROCODE as OBJ2PSROCODE FROM PersonDetails where PSROCODE = CASE " + SROfficeListID + @" when 0 then PersonDetails.PSROCODE else " + SROfficeListID + @" end
    )OBJ2
    ON
    OBJ1.Bride =OBJ2.OBJ2PersonID
    and OBJ1.PSROCODE= OBJ2.OBJ2PSROCODE
  Left Outer JOIN
  (
 SELECT PersonID OBJ3PersonID,PSROCODE OBJ3PSROCODE FROM PersonDetails where PSROCODE = CASE " + SROfficeListID + @" when 0 then PersonDetails.PSROCODE else " + SROfficeListID + @" end
 )OBJ3
 ON
 OBJ1.BrideGroom =OBJ3.OBJ3PersonID
 and OBJ1.PSROCODE= OBJ3.OBJ3PSROCODE
    )OBJ
    where OBJ.OBJ2PersonID IS  NULL AND OBJ.OBJ3PersonID IS NULL"; 
                    #endregion
                    try
                    {
                        tableList = GetMarriageAnalysisReportResult(query, reportModel);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                else if (reportModel.BRIDEPersonID == true)
                {
                    #region BRIDEPersonID == NULL

                    string query = @"Select PSROCode,(Select SRONameE from SROMaster where SROCode = PSROCode ) SROName, MarriageCaseNo,RegistrationID, Bride,BrideGroom,OBJ2PersonID BRIDEPersonID 
    from (
    SELECT
    Distinct OBJ1.*,OBJ2.OBJ2PersonID
    FROM
    (

    SELECT
    MarriageCaseNo,
    REGISTRATIONID,
    PSROCode,
    Bride,
    BrideGroom,
    convert(date,DateOfRegistration) as RegistrationDate,
    case
    when month(DateOfRegistration) >3 then year(DateOfRegistration)
    WHEN MONTH(DateOfRegistration) <= 3 THEN year(DateOfRegistration)-1
    END AS FINANCIALYEAR
    FROM
    ECDATA.DBO.MarriageRegistration MR
    inner join
    ecdata.DBO.SROMASTER SM ON SM.SROCODE = MR.PSROCode
    WHERE
    --PSROCODE = " + reportModel.SROfficeID + @"
--PSROCODE = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
PSROCODE = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
     AND SM.DistrictCode = CASE  " + reportModel.DROfficeID + @" WHEN 0 THEN SM.DistrictCode ELSE " + reportModel.DROfficeID + @" END 
	AND SM.SROCode = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
    AND DateOfRegistration IS NOT NULL
    -- AND DateOfRegistration >= " + reportModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"
    -- AND DateOfRegistration < " + reportModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"
  AND CONVERT(date, DateOfRegistration) >= '" + reportModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"'
  AND CONVERT(date, DateOfRegistration) < '" + reportModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
    )OBJ1
    Left Outer JOIN
    (
  -- SELECT PersonID OBJ2PersonID,PSROCODE as OBJ2PSROCODE FROM PersonDetails where PSROCODE =" + reportModel.SROfficeID + @"
   SELECT PersonID OBJ2PersonID,PSROCODE as OBJ2PSROCODE FROM PersonDetails where PSROCODE = CASE " + SROfficeListID + @" when 0 then PersonDetails.PSROCODE else " + SROfficeListID + @" end
    )OBJ2
    ON
    OBJ1.Bride =OBJ2.OBJ2PersonID
    and OBJ1.PSROCODE= OBJ2.OBJ2PSROCODE
    )OBJ
    where OBJ.OBJ2PersonID IS  NULL"; 
                    #endregion

                    try
                    {
                        tableList = GetMarriageAnalysisReportResult(query, reportModel);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
                else if (reportModel.BRIDEGroomPersonID == true)
                {
                    #region BRIDEGroomPersonID = NULL
                    string query = @"Select PSROCode,(Select SRONameE from SROMaster where SROCode = PSROCode ) SROName, MarriageCaseNo,RegistrationID, Bride,BrideGroom,OBJ3PersonID BRIDEGroomPersonID
from (
SELECT
Distinct OBJ1.*,OBJ3.OBJ3PersonID


FROM
(

SELECT
MarriageCaseNo,
REGISTRATIONID,
PSROCode,
Bride,
BrideGroom,
convert(date,DateOfRegistration) as RegistrationDate,
case
when month(DateOfRegistration) >3 then year(DateOfRegistration)
WHEN MONTH(DateOfRegistration) <= 3 THEN year(DateOfRegistration)-1
END AS FINANCIALYEAR
FROM
ECDATA.DBO.MarriageRegistration MR
inner join
ecdata.DBO.SROMASTER SM ON SM.SROCODE = MR.PSROCode
WHERE
    --PSROCODE = " + reportModel.SROfficeID + @"
PSROCODE = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
 AND SM.DistrictCode = CASE  " + reportModel.DROfficeID + @" WHEN 0 THEN SM.DistrictCode ELSE " + reportModel.DROfficeID + @" END 
	AND SM.SROCode = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
    AND DateOfRegistration IS NOT NULL
     --AND DateOfRegistration >= " + reportModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"
     --AND DateOfRegistration < " + reportModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"
  AND CONVERT(date, DateOfRegistration) >= '" + reportModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"'
  AND CONVERT(date, DateOfRegistration) < '" + reportModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
   )OBJ1
Left Outer JOIN
(
--SELECT PersonID OBJ3PersonID,PSROCODE OBJ3PSROCODE FROM PersonDetails where PSROCODE =  " + SROfficeListID + @"
SELECT PersonID OBJ3PersonID,PSROCODE OBJ3PSROCODE FROM PersonDetails where PSROCODE = CASE " + SROfficeListID + @" when 0 then PersonDetails.PSROCODE else " + SROfficeListID + @" end
)OBJ3
ON
OBJ1.BrideGroom =OBJ3.OBJ3PersonID
and OBJ1.PSROCODE= OBJ3.OBJ3PSROCODE
)OBJ
where OBJ.OBJ3PersonID IS NULL"; 
                    #endregion

                    try
                    {
                        tableList = GetMarriageAnalysisReportResult(query, reportModel);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                else if (reportModel.WitnessCount == true)
                {
                    #region WitnessCount = NULL
                    string query = @"Select PSROCode,(Select SRONameE from SROMaster where SROCode = PSROCode ) SROName, MarriageCaseNo,RegistrationID, Bride,BrideGroom, WitnessCount
from (
SELECT
Distinct OBJ1.*,OBJ4.*


FROM
(

SELECT
MarriageCaseNo,
REGISTRATIONID,
PSROCode,
Bride,
BrideGroom,
convert(date,DateOfRegistration) as RegistrationDate,
case
when month(DateOfRegistration) >3 then year(DateOfRegistration)
WHEN MONTH(DateOfRegistration) <= 3 THEN year(DateOfRegistration)-1
END AS FINANCIALYEAR
FROM
ECDATA.DBO.MarriageRegistration MR
inner join
ecdata.DBO.SROMASTER SM ON SM.SROCODE = MR.PSROCode
WHERE
--PSROCODE = " + reportModel.SROfficeID + @"
PSROCODE = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
 AND SM.DistrictCode = CASE  " + reportModel.DROfficeID + @" WHEN 0 THEN SM.DistrictCode ELSE " + reportModel.DROfficeID + @" END 
	AND SM.SROCode = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
AND DateOfRegistration IS NOT NULL
-- AND DateOfRegistration >= " + reportModel.DateTime_FromDate.ToString("yyyy/mm/dd") + @"
-- AND DateOfRegistration < " + reportModel.DateTime_ToDate.ToString("yyyy/mm/dd") + @"
  AND CONVERT(date, DateOfRegistration) >= '" + reportModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"'
  AND CONVERT(date, DateOfRegistration) < '" + reportModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
)OBJ1
Left Outer JOIN
(
--SELECT SROCODE,RegistrationID OBJ4RegistrationID,count(WitnessID) WitnessCount FROM WitnessInfo where SROCODE = " + SROfficeListID + @" Group by SROCODE,RegistrationID
SELECT SROCODE,RegistrationID OBJ4RegistrationID,count(WitnessID) WitnessCount FROM WitnessInfo where SROCODE = CASE " + SROfficeListID + @" when 0 then WitnessInfo.SROCODE else " + SROfficeListID + @" end Group by SROCODE,RegistrationID
)OBJ4
ON
OBJ1.RegistrationID = OBJ4.OBJ4RegistrationID
and OBJ1.PSROCODE = OBJ4.SROCODE
)OBJ
where OBJ.WitnessCount IS NULL"; 
                    #endregion

                    try
                    {
                        tableList = GetMarriageAnalysisReportResult(query, reportModel);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                else if (reportModel.ReceiptCount == true)
                {
                    #region ReceiptCount = NULL
                    string query = @"Select PSROCode,(Select SRONameE from SROMaster where SROCode = PSROCode ) SROName, MarriageCaseNo,RegistrationID, Bride,BrideGroom, ReceiptCount
from (
SELECT
Distinct OBJ1.*,OBJ5.*


FROM
(

SELECT
MarriageCaseNo,
REGISTRATIONID,
PSROCode,
Bride,
BrideGroom,
convert(date,DateOfRegistration) as RegistrationDate,
case
when month(DateOfRegistration) >3 then year(DateOfRegistration)
WHEN MONTH(DateOfRegistration) <= 3 THEN year(DateOfRegistration)-1
END AS FINANCIALYEAR
FROM
ECDATA.DBO.MarriageRegistration MR
inner join
ecdata.DBO.SROMASTER SM ON SM.SROCODE = MR.PSROCode
WHERE
--PSROCODE = " + reportModel.SROfficeID + @"
PSROCODE = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
 AND SM.DistrictCode = CASE  " + reportModel.DROfficeID + @" WHEN 0 THEN SM.DistrictCode ELSE " + reportModel.DROfficeID + @" END 
	AND SM.SROCode = CASE " + SROfficeListID + @" when 0 then SM.SROCode else " + SROfficeListID + @" end
AND DateOfRegistration IS NOT NULL
-- AND DateOfRegistration >= " + reportModel.DateTime_FromDate.ToString("yyyy/mm/dd") + @"
-- AND DateOfRegistration < " + reportModel.DateTime_ToDate.ToString("yyyy/mm/dd") + @"
  AND CONVERT(date, DateOfRegistration) >= '" + reportModel.DateTime_FromDate.ToString("yyyy/MM/dd") + @"'
  AND CONVERT(date, DateOfRegistration) < '" + reportModel.DateTime_ToDate.ToString("yyyy/MM/dd") + @"'
)OBJ1
Left Outer JOIN
(
--SELECT SROCODE OBJ5SROCODE,DocumentID,Count(ReceiptID) ReceiptCount FROM ReceiptMaster where SROCODE = " + SROfficeListID + @" and ReceiptTypeID >1 GROUP By SROCODE,DocumentID
SELECT SROCODE OBJ5SROCODE,DocumentID,Count(ReceiptID) ReceiptCount FROM ReceiptMaster where SROCODE = CASE " + SROfficeListID + @" when 0 then ReceiptMaster.SROCODE else " + SROfficeListID + @" end and ReceiptTypeID >1 GROUP By SROCODE,DocumentID
)OBJ5
ON
OBJ1.RegistrationID =OBJ5.DocumentID
and OBJ1.PSROCODE= OBJ5.OBJ5SROCODE
)OBJ
where OBJ.ReceiptCount IS  NULL";

                    #endregion
                    try
                    {
                        tableList = GetMarriageAnalysisReportResult(query, reportModel);
                    }
                    catch (Exception ex)
                    {

                        throw;
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
            return tableList;
        }

        public MarriageAnalysisReportModel MarriageAnalysisReportModelView(int OfficeID)
        {
            MarriageAnalysisReportModel resModel = new MarriageAnalysisReportModel();

            try
            {
                dbContext = new KaveriEntities();
                SelectListItem selectListItem = new SelectListItem();
                ApiCommonFunctions objCommon = new ApiCommonFunctions();
                List<SelectListItem> SROfficeList = new List<SelectListItem>();
                string FirstRecord = "All";
                //  resModel.NatureOfDocumentList = objCommon.GetNatureOfDocumentList();
                SelectListItem sroNameItem = new SelectListItem();
                SelectListItem droNameItem = new SelectListItem();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
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


        public List<MarriageAnalysisReportTableModel> GetMarriageAnalysisReportResult(string query, MarriageAnalysisReportModel reportModel)
        {
            MarriageAnalysisReportTableModel tableModel = null;
            List<MarriageAnalysisReportTableModel> tableList = new List<MarriageAnalysisReportTableModel>();
            try
            {
                long SrCount = 1;
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
                                tableModel = new MarriageAnalysisReportTableModel();

                                tableModel.srNo = SrCount++;


                                tableModel.SroName = item["SROName"].ToString();
                                tableModel.MarriageCaseNo = item["MarriageCaseNo"].ToString();

                                //tableModel.RegistrationID = Convert.ToInt64(item["RegistrationID"].ToString());
                                tableModel.RegistrationID =(long)item["RegistrationID"];

                                if (item["Bride"] == DBNull.Value)
                                    tableModel.Bride = 0;
                                else
                                    tableModel.Bride = Convert.ToInt64(item["Bride"]);

                                if (item["BrideGroom"] == DBNull.Value)
                                    tableModel.BrideGroom = 0;
                                else
                                    tableModel.BrideGroom = Convert.ToInt64(item["BrideGroom"]);
                                if (reportModel.BRIDEPersonID == true && reportModel.BRIDEGroomPersonID == true)
                                {
                                    tableModel.BRIDEPersonID = String.IsNullOrEmpty(item["BRIDEPersonID"].ToString()) ? "--" : item["BRIDEPersonID"].ToString();
                                    tableModel.BRIDEGroomPersonID = String.IsNullOrEmpty(item["BRIDEGroomPersonID"].ToString()) ? "--" : item["BRIDEGroomPersonID"].ToString();
                                    //if (item["BRIDEPersonID"] == DBNull.Value)
                                    //    tableModel.BRIDEPersonID = 0;
                                    //else
                                    //    tableModel.BRIDEPersonID = Convert.ToInt64(item["BRIDEPersonID"]);

                                    //if (item["BRIDEGroomPersonID"] == DBNull.Value)
                                    //    tableModel.BRIDEGroomPersonID = 0;
                                    //else
                                    //    tableModel.BRIDEGroomPersonID = Convert.ToInt64(item["BRIDEGroomPersonID"]);
                                }
                                else if(reportModel.BRIDEPersonID == true)
                                {
                                    tableModel.BRIDEPersonID = String.IsNullOrEmpty(item["BRIDEPersonID"].ToString()) ? "--" : item["BRIDEPersonID"].ToString();
                                    //if (item["BRIDEPersonID"] == DBNull.Value)  //if NULL then 0
                                    //    tableModel.BRIDEPersonID = 0;
                                    //else
                                    //    tableModel.BRIDEPersonID = Convert.ToInt64(item["BRIDEPersonID"]);
                                }
                                else if(reportModel.BRIDEGroomPersonID == true)
                                {
                                    tableModel.BRIDEGroomPersonID = String.IsNullOrEmpty(item["BRIDEGroomPersonID"].ToString()) ? "--" : item["BRIDEGroomPersonID"].ToString();
                                    //if (item["BRIDEGroomPersonID"] == DBNull.Value) //if NULL then 0
                                    //    tableModel.BRIDEGroomPersonID = 0;
                                    //else
                                    //    tableModel.BRIDEGroomPersonID = Convert.ToInt64(item["BRIDEGroomPersonID"]);
                                }
                                //else
                                //{
                                //    tableModel.BRIDEPersonID = "--";
                                //    tableModel.BRIDEGroomPersonID = "--";
                                //    //if NULL then 0
                                //    //tableModel.BRIDEPersonID = 0;
                                //    //tableModel.BRIDEGroomPersonID = 0;
                                //}
                                tableList.Add(tableModel);
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
              
            }

            return tableList;
        }
    }
}