using CustomModels.Models.MISReports.DataRestorationReport;
using CustomModels.Models.Remittance.BatchCompletionDetails;
using CustomModels.Models.Remittance.BatchCompletionDetails;
using CustomModels.Models.Remittance.MasterData;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.SRToCentralComService;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using static iTextSharp.text.pdf.events.IndexEvents;
//added by vijay on 16/02/2023

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class BatchCompletionDetailsDAL
    {
        BatchCompletionDetailsReportModel reportModel = new BatchCompletionDetailsReportModel();
        private SqlConnection Conn;

        BatchCompletionDetailsResultModel ResultModel = new BatchCompletionDetailsResultModel();


        //public DataTable GetData(string InputDate, int DocType)
        public DataTable GetData(int DocType)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["KaveriEntities"].ConnectionString;
                if (connectionString.ToLower().StartsWith("metadata="))
                {
                    System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder efBuilder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(connectionString);
                    connectionString = efBuilder.ProviderConnectionString;
                }
                Conn = new SqlConnection(connectionString);
                SqlDataAdapter sda = null;
                //sda.SelectCommand.CommandTimeout = 180;


                //added by vijay on 24 March 2023 


                string SqlString_for_DM = @"SELECT SROCode, SRONameE, L_Stamp5DateTime, DocumentTypeID, BatchDateTime,
    CASE WHEN BatchDateTime >= '2023-01-31' THEN 'Completed' ELSE 'Not Completed' END AS is_verified
    FROM (
        SELECT s.SROCode, s.SRONameE, ISNULL(
            CASE WHEN CONVERT(DATE, MAX(r.L_Stamp5DateTime)) = '1900-01-01'
                THEN '--'
                ELSE CONVERT(VARCHAR(50), MAX(r.L_Stamp5DateTime), 121) 
            END, '--'
        ) AS L_Stamp5DateTime, DocumentTypeID, (
            SELECT stamp5datetime
            FROM DocumentMaster dm 
            WHERE dm.SROCode = s.SROCode
                AND DocumentID = (
                    SELECT MAX(ToDocumentID)
                    FROM RPT_DocReg_NoCLBatchDetails rp
                    WHERE rp.SROCode = s.SROCode
                        AND rp.IsVerified = 1 and rp.DocumentTypeID = 1
                )
        ) AS BatchDateTime
        FROM SROMaster s
        LEFT OUTER JOIN RPT_DocReg_NoCLDetails r ON s.SROCode = r.SROCode AND r.DocumentTypeID = 1
        GROUP BY s.SROCode, s.SRONameE, DocumentTypeID
    ) OBJ where SROCode!=-2 and SROCode!=257

    ORDER BY BatchDateTime DESC";

                //ended by vijay





                // string SqlString_for_DM = "select s.SROCode,s.SRONameE,obj2.DataStatus,obj2.MaxBatchStamp5Datetime,obj2.maxdocid,obj2.MaxDMStamp5Datetime,obj2.maxTodocid from SROMaster s left outer join \r\n\r\n(Select * ,  \r\n,(Select Stamp5DateTime MaxBatchStamp5Datetime from documentmaster DM where DocumentID = OBJ1.maxdocid and  DM.SROCode = OBJ1.SROCode )MaxDMStamp5Datetime\r\n," +
                // "CASE When Isnull(OBJ1.maxdocid,0) > Isnull(OBJ1.maxTodocid,0) Then 'Not Complete' Else 'Complete' END as DataStatus\r\nFROM (Select s.srocode ,Max(r.ToDocumentID) maxTodocid,Max(d.DocumentID) maxdocid\r\nfrom [dbo].[RPT_DocReg_NoCLBatchDetails] r\r\nright outer join  dbo.SROMaster s on\r\ns.SROCode=r.SROCode\r\njoin  dbo.DocumentMaster d on\r\ns.SROCode=d.SROCode\r\n\t where Convert(Date,d.Stamp5DateTime) <=@InputDate\r\n\r\ngroup by s.SROCode \r\n\r\n) OBJ1 \r\n\r\n)obj2 on s.SROCode=obj2.SROCode   group by s.SROCode,s.SRONameE,obj2.DataStatus,obj2.MaxBatchStamp5Datetime,obj2.maxdocid,obj2.MaxDMStamp5Datetime,obj2.maxTodocid\r\n\r\norder by s.srocode";


                //string sqlstring_for_MR = "select s.SROCode,s.SRONameE,obj2.DataStatus,obj2.MaxBatchStamp5Datetime,obj2.maxRegid,obj2.MaxMRDateofReg,obj2.maxTodocid from SROMaster s left outer join \r\n\t(Select * , (Select DateOfRegistration MaxMRDateofReg from MarriageRegistration DM where RegistrationID = OBJ1.maxTodocid\r\n\r\n\tand DM.PSROCode = OBJ1.SROCode )MaxBatchStamp5Datetime  \r\n\r\n\r\n\t,(Select  DateOfRegistration MaxMRDateofReg from MarriageRegistration Dm where RegistrationID = OBJ1.maxRegid and  DM.PSROCode = OBJ1.SROCode )MaxMRDateofReg\r\n\t,CASE When Isnull(OBJ1.maxRegid,0) > Isnull(OBJ1.maxTodocid,0) Then 'Not Complete' Else 'Complete' END as DataStatus\r\n\tFROM (Select s.srocode,s.SRONameE,Max(r.ToDocumentID) maxTodocid,Max(m.RegistrationID) maxRegid\r\n\tfrom [dbo].[RPT_DocReg_NoCLBatchDetails] r\r\n\tright outer join  dbo.SROMaster s on\r\n\ts.SROCode=r.SROCode\r\n\t  join  dbo.MarriageRegistration m on\r\n\tm.PSROCode=s.SROCode\r\n\t where Convert(Date,m.DateOfRegistration) <=@InputDate\r\n\tgroup by s.SROCode,s.SRONameE\r\n\t) OBJ1\r\n\r\n\t\r\n)obj2 on s.SROCode=obj2.SROCode  \r\n   group by s.SROCode,s.SRONameE,obj2.DataStatus,obj2.MaxBatchStamp5Datetime,obj2.maxRegid,obj2.MaxMRDateofReg,obj2.maxTodocid\r\norder by s.srocode";

                /*For Marriage*/
                //string sqlstring_for_MR = "\r\nselect s.SROCode,s.SRONameE,obj2.DataStatus,obj2.MaxBatchStamp5Datetime,obj2.maxRegid,obj2.MaxMRDateofReg,obj2.maxTodocid from SROMaster s left outer join \r\n\t(Select * , (Select BatchDateTime MaxBatchStamp5Datetime from [dbo].[RPT_DocReg_NoCLBatchDetails]\r\nDM where  ToDocumentID= OBJ1.maxTodocid  and DM.SROCode = OBJ1.SROCode ) \r\nMaxBatchStamp5Datetime \r\n\r\n\r\n\t,(Select  DateOfRegistration MaxMRDateofReg from MarriageRegistration Dm where RegistrationID = OBJ1.maxRegid and  DM.PSROCode = OBJ1.SROCode )MaxMRDateofReg\r\n\t,CASE When Isnull(OBJ1.maxRegid,0) > Isnull(OBJ1.maxTodocid,0) Then 'Not Complete' Else 'Complete' END as DataStatus\r\n\tFROM (Select s.srocode,s.SRONameE,Max(r.ToDocumentID) maxTodocid,Max(m.RegistrationID) maxRegid\r\n\tfrom [dbo].[RPT_DocReg_NoCLBatchDetails] r\r\n\tright outer join  dbo.SROMaster s on\r\n\ts.SROCode=r.SROCode\r\n\t  join  dbo.MarriageRegistration m on\r\n\tm.PSROCode=s.SROCode\r\n\tgroup by s.SROCode,s.SRONameE\r\n\t) OBJ1\r\n\r\n\t\r\n)obj2 on s.SROCode=obj2.SROCode  group by s.SROCode,s.SRONameE,obj2.DataStatus,obj2.MaxBatchStamp5Datetime,obj2.maxRegid,obj2.MaxMRDateofReg,obj2.maxTodocid\r\norder by s.SROCode";

                //Added by vijay on 24 MAR 2023
                string sqlstring_for_MR = @"SELECT SROCode, SRONameE, L_Stamp5DateTime, DocumentTypeID, BatchDateTime,
    CASE WHEN BatchDateTime >= '2023-01-31' THEN 'Completed' ELSE 'Not Completed' END AS is_verified
    FROM (
        SELECT s.SROCode, s.SRONameE, ISNULL(
            CASE WHEN CONVERT(DATE, MAX(r.L_Stamp5DateTime)) = '1900-01-01'
                THEN '--'
                ELSE CONVERT(VARCHAR(50), MAX(r.L_Stamp5DateTime), 121) 
            END, '--'
        ) AS L_Stamp5DateTime, DocumentTypeID, (
            SELECT stamp5datetime
            FROM DocumentMaster dm 
            WHERE dm.SROCode = s.SROCode
                AND DocumentID = (
                    SELECT MAX(ToDocumentID)
                    FROM RPT_DocReg_NoCLBatchDetails rp
                    WHERE rp.SROCode = s.SROCode
                        AND rp.IsVerified = 1
                )
        ) AS BatchDateTime
        FROM SROMaster s
        LEFT OUTER JOIN RPT_DocReg_NoCLDetails r ON s.SROCode = r.SROCode AND r.DocumentTypeID = 2
        GROUP BY s.SROCode, s.SRONameE, DocumentTypeID
    ) OBJ
		where SROCode!=-2 and SROCode!=257   -- Added by Vijay to remove IGR and KGF office


    ORDER BY BatchDateTime DESC";   
                //End by Rushikesh on 27 Feb 2023

                SqlCommand cmd = new SqlCommand(SqlString_for_DM, Conn);
                // cmd.CommandTimeout
                SqlParameter[] param = new SqlParameter[1];

                //param[0] = new SqlParameter("@InputDate", InputDate); commented


                if (DocType == 0)
                {

                    sda = new SqlDataAdapter(SqlString_for_DM, Conn);
                    sda.SelectCommand.CommandTimeout = 300; // updated: 180;

                   // sda.SelectCommand.Parameters.AddWithValue("@InputDate", InputDate);

                }
                else
                {
                    sda = new SqlDataAdapter(sqlstring_for_MR, Conn);
                    sda.SelectCommand.CommandTimeout = 300; //updated : 120;

                   // sda.SelectCommand.Parameters.AddWithValue("@InputDate", InputDate);

                }


                DataTable dt = new DataTable();

                try
                {

                    Conn.Open();

                    sda.Fill(dt);

                }
                catch (SqlException se)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    sda.Dispose();
                    Conn.Close();
                }
                return dt;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public BatchCompletionDetailsReportModel BatchCompletionDetailsView()
        {
           return reportModel;
        }

        public BatchCompletionDetailsResultModel GetBatchCompletionDetails(BatchCompletionDetailsReportModel batchCompletionDetailsReportModel)
        {
            try
            {
                long SrCount = 1;
                BatchCompletionDetailsReportTableModel batchCompletionDetailsReportTableModel = new BatchCompletionDetailsReportTableModel();
                //var TillDate = Convert.ToDateTime(batchCompletionDetailsReportModel.TillDate);
                //TillDate = TillDate.Date;
                //var TillDate = batchCompletionDetailsReportModel.TillDate;
                //DataTable dt = GetData(TillDate, batchCompletionDetailsReportModel.DocType);

                //Updated by Rushikesh on 27 Feb 2023
                DataTable dt = GetData(batchCompletionDetailsReportModel.DocType);
                //End by Rushikesh on 27 Feb 2023

                ResultModel.BatchCompletionDetailsReportTableList = new List<BatchCompletionDetailsReportTableModel>();

                if (batchCompletionDetailsReportModel.DocType == 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        batchCompletionDetailsReportTableModel = new BatchCompletionDetailsReportTableModel();

                        //Added by Rushikesh on 27 Feb 2023
                        batchCompletionDetailsReportTableModel.srNo = SrCount++;
                        //End by Rushikesh on 27 Feb 2023

                        batchCompletionDetailsReportTableModel.SROCode = (int)dt.Rows[i]["srocode"];

                        /*
                        if (!(dt.Rows[i]["maxdocid"].ToString().Equals("")))

                            batchCompletionDetailsReportTableModel.MaxDocID = dt.Rows[i]["maxdocid"].ToString();
                        else
                            batchCompletionDetailsReportTableModel.MaxDocID = "NA";
                        if (!(dt.Rows[i]["maxTodocid"].ToString().Equals("")))
                        {
                            batchCompletionDetailsReportTableModel.MaxToDocID = dt.Rows[i]["maxTodocid"].ToString();

                        }
                        else
                        {
                            batchCompletionDetailsReportTableModel.MaxToDocID = "NA";
                        }

                        if (!(dt.Rows[i]["DataStatus"].ToString().Equals("")))
                        {
                            batchCompletionDetailsReportTableModel.IsBatchComplete = dt.Rows[i]["DataStatus"].ToString();

                        }
                        else
                            batchCompletionDetailsReportTableModel.IsBatchComplete = "NA";

                        if (!(dt.Rows[i]["MaxBatchStamp5Datetime"].ToString().Equals("")))
                        {
                            batchCompletionDetailsReportTableModel.RPT_DocReg_NoCLBatchDetails_MaxToDocID_Stamp5DateTime = dt.Rows[i]["MaxBatchStamp5Datetime"].ToString();

                        }
                        else
                            batchCompletionDetailsReportTableModel.RPT_DocReg_NoCLBatchDetails_MaxToDocID_Stamp5DateTime = "NA";

                        if (!(dt.Rows[i]["MaxDMStamp5Datetime"].ToString().Equals("")))
                        {

                            batchCompletionDetailsReportTableModel.documentmaster_MaxDocID_Stamp5DateTime = dt.Rows[i]["MaxDMStamp5Datetime"].ToString();
                        }
                        else
                            batchCompletionDetailsReportTableModel.documentmaster_MaxDocID_Stamp5DateTime = "NA";
                        */

                        //Added by Rushikesh on 28 Feb 2023
                        batchCompletionDetailsReportTableModel.SroName = dt.Rows[i]["SRONameE"].ToString();
                        //End by Rushikesh on 28 Feb 2023

                        batchCompletionDetailsReportTableModel.L_Stamp5DateTime = dt.Rows[i]["L_Stamp5DateTime"].ToString();
                        //batchCompletionDetailsReportTableModel.L_Stamp5DateTime = Convert.ToDateTime(dt.Rows[i]["L_Stamp5DateTime"]).ToString("dd/MM/yyyy  HH:mm:ss");
                        batchCompletionDetailsReportTableModel.Batchdatetime = dt.Rows[i]["BatchDateTime"].ToString();
                        batchCompletionDetailsReportTableModel.Is_verified = dt.Rows[i]["is_verified"].ToString();
                        ResultModel.BatchCompletionDetailsReportTableList.Add(batchCompletionDetailsReportTableModel);

                    }

                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        batchCompletionDetailsReportTableModel = new BatchCompletionDetailsReportTableModel();

                        /*
                        if (!(dt.Rows[i]["maxRegid"].ToString().Equals("")))
                            batchCompletionDetailsReportTableModel.MaxRegistrationID = dt.Rows[i]["maxRegid"].ToString();
                        else
                            batchCompletionDetailsReportTableModel.MaxRegistrationID = "NA";
                        if (!(dt.Rows[i]["srocode"]).ToString().Equals(""))
                            batchCompletionDetailsReportTableModel.SROCode = (int)dt.Rows[i]["srocode"];
                        else
                            batchCompletionDetailsReportTableModel.SROCode = 0;



                        if (!(dt.Rows[i]["maxTodocid"].ToString().Equals("")))
                        {
                            batchCompletionDetailsReportTableModel.MaxToDocID = dt.Rows[i]["maxTodocid"].ToString();

                        }
                        else
                        {
                            batchCompletionDetailsReportTableModel.MaxToDocID = "NA";
                        }
                        if (!(dt.Rows[i]["DataStatus"].ToString().Equals("")))
                        {
                            batchCompletionDetailsReportTableModel.IsBatchComplete = dt.Rows[i]["DataStatus"].ToString();

                        }
                        else
                            batchCompletionDetailsReportTableModel.IsBatchComplete = "NA";


                        if (!(dt.Rows[i]["MaxBatchStamp5Datetime"].ToString().Equals("")))
                        {
                            batchCompletionDetailsReportTableModel.RPT_DocReg_NoCLBatchDetails_MaxToDocID_Stamp5DateTime = dt.Rows[i]["MaxBatchStamp5Datetime"].ToString();

                        }
                        else
                            batchCompletionDetailsReportTableModel.RPT_DocReg_NoCLBatchDetails_MaxToDocID_Stamp5DateTime = "NA";

                        if (!(dt.Rows[i]["MaxMRDateofReg"].ToString().Equals("")))
                        {
                            batchCompletionDetailsReportTableModel.MarriageRegistration_MaxRegID_DateOfReg = dt.Rows[i]["MaxMRDateofReg"].ToString();

                        }
                        else
                            batchCompletionDetailsReportTableModel.MarriageRegistration_MaxRegID_DateOfReg = "NA";
                        */

                        //Updated by Rushikesh 28 Feb 2023
                        batchCompletionDetailsReportTableModel.srNo = SrCount++;
                        batchCompletionDetailsReportTableModel.SROCode = (int)dt.Rows[i]["srocode"];
                        batchCompletionDetailsReportTableModel.SroName = dt.Rows[i]["SRONameE"].ToString();
                        batchCompletionDetailsReportTableModel.L_Stamp5DateTime = dt.Rows[i]["L_Stamp5DateTime"].ToString();
                        //batchCompletionDetailsReportTableModel.L_Stamp5DateTime = Convert.ToDateTime(dt.Rows[i]["L_Stamp5DateTime"]).ToString("dd/MM/yyyy  HH:mm:ss");
                        batchCompletionDetailsReportTableModel.Batchdatetime = dt.Rows[i]["BatchDateTime"].ToString();
                        batchCompletionDetailsReportTableModel.Is_verified = dt.Rows[i]["is_verified"].ToString();
                        ResultModel.BatchCompletionDetailsReportTableList.Add(batchCompletionDetailsReportTableModel);

                    }
                }


                return ResultModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}





