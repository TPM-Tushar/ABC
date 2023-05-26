#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   QueryExecutionStatusReportDAL.cs
    * Author Name       :   Pankaj Sakhare
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.QueryExecutionStatusReport;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.ECDATADOCS;
using ECDataAPI.Entity.KaigrSearchDB;
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
    public class QueryExecutionStatusReportDAL : IQueryExecutionStatusReport
    {
        ApiCommonFunctions common = new ApiCommonFunctions();

        //Added by pankaj sakhare on 08-10-2020
        //to get view page model populate and return to view page
        /// <summary>
        /// QueryExecutionStatusReportView
        /// </summary>
        /// <returns>QueryExecutionStatusReportModel</returns>
        public QueryExecutionStatusReportModel QueryExecutionStatusReportView()
        {
            QueryExecutionStatusReportModel resModel = new QueryExecutionStatusReportModel();

            try
            {
                //resModel.DatabaseList = new List<SelectListItem>();
                List<SelectListItem> databaseList = new List<SelectListItem>();
                databaseList.Add(new SelectListItem { Text = "Select", Value = "0" });
                databaseList.Add(new SelectListItem { Text = "ECDATA", Value = "ECDATA" });
                databaseList.Add(new SelectListItem { Text = "KAIGR_SEARCHDB", Value = "KAIGR_SEARCHDB" });
                databaseList.Add(new SelectListItem { Text = "PEN_DOCS", Value = "PEN_DOCS" });
                databaseList.Add(new SelectListItem { Text = "ECDATA_DOCS", Value = "ECDATA_DOCS" });

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                resModel.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                resModel.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                resModel.DatabaseList = databaseList;

            }
            catch (Exception)
            {
                throw;
            }
            return resModel;
        }


        //Added by pankaj sakhare on 08-10-2020
        //to get detail datatable
        /// <summary>
        /// GetQueryExecutionStatusReport
        /// </summary>
        /// <param name="QueryExecutionStatusReportModel"></param>
        /// <returns>QueryExecutionStatusReportResModel</returns>
        public QueryExecutionStatusReportResModel GetQueryExecutionStatusReport(QueryExecutionStatusReportModel model)
        {
            QueryExecutionStatusReportResModel resModel = new QueryExecutionStatusReportResModel();
            List<QueryExecutionStatusReportDetailModel> detailList = new List<QueryExecutionStatusReportDetailModel>();
            string ReplicaType = String.Empty;
            string Catalog = String.Empty;
            string UserID = String.Empty;
            string Password = ConfigurationManager.AppSettings["PasswordForAllID"];
            string SqlConnectionString = string.Empty;
            string ConnectionStringForGettingPrimaryReplica = string.Empty;
            string PrimaryReplica = string.Empty;
            string SecondaryReplica = ConfigurationManager.AppSettings["SecondaryReplica"];
            //string DatabaseName = string.Empty;
            string ServerName = string.Empty;

            //for creating connection string as per user input 
            try
            {
                switch (model.DatabaseName)
                {
                    case "ECDATA":
                        //Catalog = model.DatabaseName;
                        //UserID = ConfigurationManager.AppSettings["ECDATA_ID"];

                        using (var entityFrameWorkConString = new EntityConnection(ConfigurationManager.ConnectionStrings["KaveriEntities"].ConnectionString))
                        {
                            var sqlConn = entityFrameWorkConString.StoreConnection as SqlConnection;

                            string[] sqlConnStrArray = sqlConn.ConnectionString.Split(';');
                            foreach (string x in sqlConnStrArray)
                            {
                                if (x.Contains("user id="))
                                {
                                    UserID = x;
                                }
                                if (x.Contains("password="))
                                {
                                    Password = x;
                                }
                            }
                            ServerName = sqlConn.DataSource;

                            Catalog = model.DatabaseName;

                        }


                        break;

                    case "PEN_DOCS":
                        Catalog = model.DatabaseName;
                        UserID = ConfigurationManager.AppSettings["PEN_DOCS_ID"];
                        Password = ConfigurationManager.AppSettings["PEN_DOCS_PASSWORD"];
                        //PEN_DOCS ENTITY IS NOT USED IN PROJECT THEREFORE HERE WE ARE ADDING SERVER NAME FROM 'COMMONREPLICA' KEY OF WEBCONFIG
                        ServerName = ConfigurationManager.AppSettings["CommonReplica"];

                        break;
                    case "KAIGR_SEARCHDB":
                        //Catalog = model.DatabaseName;
                        //UserID = ConfigurationManager.AppSettings["KAIGR_SEARCHDB_ID"];
                        using (var entityFrameWorkConString = new EntityConnection(ConfigurationManager.ConnectionStrings["KaigrSearchDB"].ConnectionString))
                        {
                            var sqlConn = entityFrameWorkConString.StoreConnection as SqlConnection;

                            string[] sqlConnStrArray = sqlConn.ConnectionString.Split(';');
                            foreach (string x in sqlConnStrArray)
                            {
                                if (x.Contains("user id="))
                                {
                                    UserID = x;
                                }
                                if (x.Contains("password="))
                                {
                                    Password = x;
                                }
                            }
                            ServerName = sqlConn.DataSource;

                            Catalog = model.DatabaseName;

                        }
                        break;
                    case "ECDATA_DOCS":
                        //Catalog = model.DatabaseName;
                        //UserID = ConfigurationManager.AppSettings["ECDATA_DOCS_ID"];
                        using (var entityFrameWorkConString = new EntityConnection(ConfigurationManager.ConnectionStrings["ECDATA_DOCS_Entities"].ConnectionString))
                        {
                            var sqlConn = entityFrameWorkConString.StoreConnection as SqlConnection;

                            string[] sqlConnStrArray = sqlConn.ConnectionString.Split(';');
                            foreach (string x in sqlConnStrArray)
                            {
                                if (x.Contains("user id="))
                                {
                                    UserID = x;
                                }
                                if (x.Contains("password="))
                                {
                                    Password = x;
                                }
                            }
                            ServerName = sqlConn.DataSource;

                            Catalog = model.DatabaseName;

                        }
                        break;
                }

                //IF COMMON REPLICA IS SELECTED THEN NO NEED TO CHECK PRIMARY AND SECONDARY REPLICA 
                if (!model.ReplicaType.Equals("CR"))
                {
                    //ConnectionStringForGettingPrimaryReplica = string.Format("Data Source = {0}; Initial Catalog = {1}; Persist Security Info = True; User ID = {2}; Password = {3}; ", ConfigurationManager.AppSettings["CommonReplica"], Catalog, UserID, Password);

                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 25-05-2021
                    // IF ELSE LOGIC ADDED
                    if (model.DatabaseName== "PEN_DOCS")
                        ConnectionStringForGettingPrimaryReplica = string.Format("Data Source = {0}; Initial Catalog = {1}; Persist Security Info = True;user id= {2};password=  {3}; ", ServerName, Catalog, UserID, Password);
                    else
                        ConnectionStringForGettingPrimaryReplica = string.Format("Data Source = {0}; Initial Catalog = {1}; Persist Security Info = True; {2};  {3};", ServerName, Catalog, UserID, Password);
                    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 25-05-2021

                    //to get primary replica server name from common replica
                    using (SqlConnection sqlConnection = new SqlConnection(ConnectionStringForGettingPrimaryReplica))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT @@SERVERNAME", sqlConnection))
                        {
                            sqlConnection.Open();
                            sqlCommand.CommandType = CommandType.Text;
                            PrimaryReplica = sqlCommand.ExecuteScalar().ToString();

                        }
                    }

                    if (!PrimaryReplica.Equals(ConfigurationManager.AppSettings["PrimaryReplica"], StringComparison.InvariantCultureIgnoreCase))
                    {
                        PrimaryReplica = ConfigurationManager.AppSettings["SecondaryReplica"];
                        SecondaryReplica = ConfigurationManager.AppSettings["PrimaryReplica"];
                    }

                }

                switch (model.ReplicaType)
                {
                    case "PR":
                        ReplicaType = PrimaryReplica;
                        //BELOW LINE COMMENTED BECAUSE USERID AND PASSWORD IS GETTING DIRECTLY FROM CONNECTION STRING WITH THERE KEY AND VALUE TOGETHER
                        //SqlConnectionString = String.Format("Data Source = {0};Initial Catalog = {1}; Persist Security Info = True; User ID = {2}; Password = {3};", ReplicaType, Catalog, UserID, Password);
                        
                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 25-05-2021
                        // IF ELSE LOGIC ADDED
                        if (model.DatabaseName == "PEN_DOCS")
                            SqlConnectionString = String.Format("Data Source = {0};Initial Catalog = {1}; Persist Security Info = True;user id= {2};password=  {3};", ReplicaType, Catalog, UserID, Password);
                        else 
                            SqlConnectionString = String.Format("Data Source = {0};Initial Catalog = {1}; Persist Security Info = True; {2}; {3};ApplicationIntent=READONLY;", ReplicaType, Catalog, UserID, Password);

                        break;

                    case "SR":
                        ReplicaType = SecondaryReplica;
                        //BELOW LINE COMMENTED BECAUSE USERID AND PASSWORD IS GETTING DIRECTLY FROM CONNECTION STRING WITH THERE KEY AND VALUE TOGETHER
                        //SqlConnectionString = String.Format("Data Source = {0};Initial Catalog = {1}; Persist Security Info = True; User ID = {2}; Password = {3}; ApplicationIntent=READONLY;", ReplicaType, Catalog, UserID, Password);
                        //SqlConnectionString = String.Format("Data Source = {0};Initial Catalog = {1}; Persist Security Info = True; {2}; {3}; ApplicationIntent=READONLY;", ReplicaType, Catalog, UserID, Password);
                        
                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 25-05-2021
                        // IF ELSE LOGIC ADDED
                        if (model.DatabaseName == "PEN_DOCS")
                            SqlConnectionString = String.Format("Data Source = {0};Initial Catalog = {1}; Persist Security Info = True;user id= {2};password=  {3};", ReplicaType, Catalog, UserID, Password);
                        else
                            SqlConnectionString = String.Format("Data Source = {0};Initial Catalog = {1}; Persist Security Info = True; {2}; {3};ApplicationIntent=READONLY;", ReplicaType, Catalog, UserID, Password);

                        break;

                    case "CR":
                        ReplicaType = ConfigurationManager.AppSettings["CommonReplica"];
                        //BELOW LINE COMMENTED BECAUSE USERID AND PASSWORD IS GETTING DIRECTLY FROM CONNECTION STRING WITH THERE KEY AND VALUE TOGETHER
                        //SqlConnectionString = String.Format("Data Source = {0};Initial Catalog = {1}; Persist Security Info = True; User ID = {2}; Password = {3};", ReplicaType, Catalog, UserID, Password);
                        
                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 25-05-2021
                        // IF ELSE LOGIC ADDED
                        if (model.DatabaseName == "PEN_DOCS")
                            SqlConnectionString = String.Format("Data Source = {0};Initial Catalog = {1}; Persist Security Info = True;user id= {2};password=  {3};", ReplicaType, Catalog, UserID, Password);
                        else
                            SqlConnectionString = String.Format("Data Source = {0};Initial Catalog = {1}; Persist Security Info = True;{2}; {3};", ReplicaType, Catalog, UserID, Password);

                        break;
                }

                //SqlConnectionString = @"Data Source = EGOVAR05\MSSQL2017; Initial Catalog = ECDATA; Persist Security Info = True; User ID = kaveri; Password = kaveri";
                //dbContext = new KaveriEntities();
                DateTime FromDate = Convert.ToDateTime(model.FromDate);
                DateTime ToDate = Convert.ToDateTime(model.ToDate);
                int Top = model.TopRows;
                DataSet dataSet = new DataSet();
                using (SqlConnection sqlConnection = new SqlConnection(SqlConnectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand("USP_RPT_QUERY_STATS", sqlConnection))
                    {
                        sqlConnection.Open();
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@FROMDATE", FromDate);
                        sqlCommand.Parameters.AddWithValue("@TODATE", ToDate);
                        sqlCommand.Parameters.AddWithValue("@TOP", Top);
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        sqlDataAdapter.Fill(dataSet);
                        //var res = sqlCommand.ExecuteReader();
                    }
                }

                if (dataSet != null)
                {
                    int i = 1;
                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            QueryExecutionStatusReportDetailModel detailModel = new QueryExecutionStatusReportDetailModel();
                            detailModel.SrNo = i++;

                            // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 21-05-2021
                            //detailModel.DatabaseName = Convert.ToString(dataRow["DATABASENAME"]);
                            //detailModel.QuerySqlText = Convert.ToString(dataRow["query_sql_text"]);
                            //detailModel.ObjectName = Convert.ToString(dataRow["Object_Name"]);
                            //detailModel.ReplicaName = Convert.ToString(dataRow["Replica_Name"]);
                            //detailModel.QueryPlanXML = Convert.ToString(dataRow["query_plan_xml"]);
                            //detailModel.LastExecutionTime = Convert.ToString(dataRow["last_execution_time"]);
                            //detailModel.CountExecutions = Convert.ToDouble(dataRow["count_executions"]);
                            //detailModel.MaxDurationSeconds = Convert.ToDouble(dataRow["max_duration_seconds"]);
                            //detailModel.AvgDurationSeconds = Convert.ToDouble(dataRow["avg_duration_seconds"]);
                            //detailModel.LastDurationSeconds = Convert.ToDouble(dataRow["last_duration_seconds"]);
                            //detailModel.MaxCpuTimeSeconds = Convert.ToDouble(dataRow["max_cpu_time_seconds"]);
                            //detailModel.AvgCpuTimeSeconds = Convert.ToDouble(dataRow["avg_cpu_time_seconds"]);
                            //detailModel.LastCpuTimeSeconds = Convert.ToDouble(dataRow["last_cpu_time_seconds"]);
                            //detailModel.MaxPhysicalIOReads = Convert.ToDouble(dataRow["max_physical_io_reads"]);
                            //detailModel.AvgPhysicalIOReads = Convert.ToDouble(dataRow["avg_physical_io_reads"]);
                            //detailModel.LastPhysicalIOReads = Convert.ToDouble(dataRow["last_physical_io_reads"]);
                            //detailModel.MaxLogicalIOReads = Convert.ToDouble(dataRow["max_logical_io_reads"]);
                            //detailModel.AvgLogicalIOReads = Convert.ToDouble(dataRow["avg_logical_io_reads"]);
                            //detailModel.LastLogicalIOReads = Convert.ToDouble(dataRow["last_logical_io_reads"]);
                            //detailModel.MaxLogicalIOWrites = Convert.ToDouble(dataRow["max_logical_io_writes"]);
                            //detailModel.AvgLogicalIOWrites = Convert.ToDouble(dataRow["avg_logical_io_writes"]);
                            //detailModel.LastLogicalIOWrites = Convert.ToDouble(dataRow["last_logical_io_writes"]);
                            //detailModel.MaxTempdbSpaceUsed = Convert.ToDouble(dataRow["max_tempdb_space_used"]);
                            //detailModel.AvgTempdbSpaceUsed = Convert.ToDouble(dataRow["avg_tempdb_space_used"]);
                            //detailModel.LastTempdbSpaceUsed = Convert.ToDouble(dataRow["last_tempdb_space_used"]);
                            //detailModel.MaxQueryMaxUsedMemory8kPages = Convert.ToDouble(dataRow["max_query_max_used_memory_8k_pages"]);
                            //detailModel.AvgQueryMaxUsedMemory8kPages = Convert.ToDouble(dataRow["avg_query_max_used_memory_8k_pages"]);
                            //detailModel.LastQueryMaxUsedMemory8kPages = Convert.ToDouble(dataRow["last_query_max_used_memory_8k_pages"]);
                            //detailModel.MaxRowCount = Convert.ToDouble(dataRow["max_rowcount"]);
                            //detailModel.AvgRowCount = Convert.ToDouble(dataRow["avg_rowcount"]);
                            //detailModel.LastRowCount = Convert.ToDouble(dataRow["last_rowcount"]);

                            detailModel.QuerySqlText = Convert.ToString(dataRow["query_sql_text"]);
                            detailModel.QueryPlanXML = Convert.ToString(dataRow["query_plan"]);
                            detailModel.creation_time = Convert.ToString(dataRow["creation_time"]);
                            detailModel.LastExecutionTime = Convert.ToString(dataRow["last_execution_time"]);
                            detailModel.CountExecutions = Convert.ToDouble(dataRow["execution_count"]);
                            detailModel.total_worker_time = Convert.ToString(dataRow["total_worker_time"]);
                            detailModel.max_worker_time = Convert.ToString(dataRow["max_worker_time"]);
                            detailModel.last_worker_time = Convert.ToString(dataRow["last_worker_time"]);
                            detailModel.total_elapsed_time = Convert.ToString(dataRow["total_elapsed_time"]);
                            detailModel.max_elapsed_time = Convert.ToString(dataRow["max_elapsed_time"]);
                            detailModel.last_elapsed_time = Convert.ToString(dataRow["last_elapsed_time"]);
                            detailModel.total_physical_reads = Convert.ToDouble(dataRow["total_physical_reads"]);
                            detailModel.max_physical_reads = Convert.ToDouble(dataRow["max_physical_reads"]);
                            detailModel.last_physical_reads = Convert.ToDouble(dataRow["last_physical_reads"]);
                            detailModel.total_logical_reads = Convert.ToDouble(dataRow["total_logical_reads"]);
                            detailModel.max_logical_reads = Convert.ToDouble(dataRow["max_logical_reads"]);
                            detailModel.last_logical_reads = Convert.ToDouble(dataRow["last_logical_reads"]);
                            detailModel.total_logical_writes = Convert.ToDouble(dataRow["total_logical_writes"]);
                            detailModel.max_logical_writes = Convert.ToDouble(dataRow["max_logical_writes"]);
                            detailModel.last_logical_writes = Convert.ToDouble(dataRow["last_logical_writes"]);


                            // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 21-05-2021

                            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 20-05-2021 
                            // ABOVE ASSIGNMENT CODE TO BE REPLACED BY BELOW CODE AND MODEL AND JAVASCRIPT AND VIEW CHANGES TO BE DONE
                            //detailModel.QuerySqlText = Convert.ToString(dataRow["query_sql_text"]);
                            //detailModel.QueryPlanXML = Convert.ToString(dataRow["query_plan"]);
                            //detailModel.creation_time = Convert.ToString(dataRow["creation_time"]);
                            //detailModel.LastExecutionTime = Convert.ToString(dataRow["last_execution_time"]);
                            //detailModel.CountExecutions = Convert.ToDouble(dataRow["execution_count"]);
                            //detailModel.total_worker_time = Convert.ToString(dataRow["total_worker_time"]);
                            //detailModel.max_worker_time = Convert.ToString(dataRow["max_worker_time"]);
                            //detailModel.last_worker_time = Convert.ToString(dataRow["last_worker_time"]);
                            //detailModel.total_elapsed_time = Convert.ToString(dataRow["total_elapsed_time"]);
                            //detailModel.max_elapsed_time = Convert.ToString(dataRow["max_elapsed_time"]);
                            //detailModel.last_elapsed_time = Convert.ToString(dataRow["last_elapsed_time"]);
                            //detailModel.total_physical_reads = Convert.ToDouble(dataRow["total_physical_reads"]);
                            //detailModel.max_physical_reads = Convert.ToDouble(dataRow["max_physical_reads"]);
                            //detailModel.last_physical_reads = Convert.ToDouble(dataRow["last_physical_reads"]);
                            //detailModel.total_physical_reads = Convert.ToDouble(dataRow["total_logical_reads"]);
                            //detailModel.max_logical_reads = Convert.ToDouble(dataRow["max_logical_reads"]);
                            //detailModel.last_physical_reads = Convert.ToDouble(dataRow["last_logical_reads"]);
                            //detailModel.total_physical_writes = Convert.ToDouble(dataRow["total_logical_writes"]);
                            //detailModel.max_logical_writes = Convert.ToDouble(dataRow["max_logical_writes"]);
                            //detailModel.last_physical_writes = Convert.ToDouble(dataRow["last_logical_writes"]);
                            // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 20-05-2021

                            #region commented for string datatype
                            //detailModel.LastExecutionTime = Convert.ToString(dataRow["last_execution_time"]);
                            //detailModel.CountExecutions = Convert.ToString(dataRow["count_executions"]);
                            //detailModel.MaxDurationSeconds = Convert.ToString(dataRow["max_duration_seconds"]);
                            //detailModel.AvgDurationSeconds = Convert.ToString(dataRow["avg_duration_seconds"]);
                            //detailModel.LastDurationSeconds = Convert.ToString(dataRow["last_duration_seconds"]);
                            //detailModel.MaxCpuTimeSeconds = Convert.ToString(dataRow["max_cpu_time_seconds"]);
                            //detailModel.AvgCpuTimeSeconds = Convert.ToString(dataRow["avg_cpu_time_seconds"]);
                            //detailModel.LastCpuTimeSeconds = Convert.ToString(dataRow["last_cpu_time_seconds"]);
                            //detailModel.MaxPhysicalIOReads = Convert.ToString(dataRow["max_physical_io_reads"]);
                            //detailModel.AvgPhysicalIOReads = Convert.ToString(dataRow["avg_physical_io_reads"]);
                            //detailModel.LastPhysicalIOReads = Convert.ToString(dataRow["last_physical_io_reads"]);
                            //detailModel.MaxLogicalIOReads = Convert.ToString(dataRow["max_logical_io_reads"]);
                            //detailModel.AvgLogicalIOReads = Convert.ToString(dataRow["avg_logical_io_reads"]);
                            //detailModel.LastLogicalIOReads = Convert.ToString(dataRow["last_logical_io_reads"]);
                            //detailModel.MaxLogicalIOWrites = Convert.ToString(dataRow["max_logical_io_writes"]);
                            //detailModel.AvgLogicalIOWrites = Convert.ToString(dataRow["avg_logical_io_writes"]);
                            //detailModel.LastLogicalIOWrites = Convert.ToString(dataRow["last_logical_io_writes"]);
                            //detailModel.MaxTempdbSpaceUsed = Convert.ToString(dataRow["max_tempdb_space_used"]);
                            //detailModel.AvgTempdbSpaceUsed = Convert.ToString(dataRow["avg_tempdb_space_used"]);
                            //detailModel.LastTempdbSpaceUsed = Convert.ToString(dataRow["last_tempdb_space_used"]);
                            //detailModel.MaxQueryMaxUsedMemory8kPages = Convert.ToString(dataRow["max_query_max_used_memory_8k_pages"]);
                            //detailModel.AvgQueryMaxUsedMemory8kPages = Convert.ToString(dataRow["avg_query_max_used_memory_8k_pages"]);
                            //detailModel.LastQueryMaxUsedMemory8kPages = Convert.ToString(dataRow["last_query_max_used_memory_8k_pages"]);
                            //detailModel.MaxRowCount = Convert.ToString(dataRow["max_rowcount"]);
                            //detailModel.AvgRowCount = Convert.ToString(dataRow["avg_rowcount"]);
                            //detailModel.LastRowCount = Convert.ToString(dataRow["last_rowcount"]);
                            #endregion
                            detailList.Add(detailModel);

                            //for query text button and sqlPlan button
                            if (!model.IsExcel)
                            {
                                //tring res2 = datarow["query_sql_text"].tostring().replace('"', '\'');
                                //res2 = datarow["query_sql_text"].tostring().replace(">", "'>'");
                                //string res = convert.tostring(res2).substring(0, detailmodel.querysqltext.length > 50 ? 50 : detailmodel.querysqltext.length);
                                //detailModel.Query = @"<a onclick='showmodelpopup(" + detailModel.SrNo + ")'>Query</a>";
                                detailModel.Query = "<button type ='button' class='btn btn-group-md btn-success' data-toggle='modal' data-target='#exampleModalLong' onclick=showmodelpopup('" + detailModel.SrNo + "') data-toggle='tooltip' data-placement='top' title='Click here'>Sql Text</ button>";
                                detailModel.QueryPlanXMLButton = "<button type ='button' class='btn btn-group-md btn-success' onclick=DownloadQueryPlanXML('" + detailModel.SrNo + "') data-toggle='tooltip' data-placement='top' title='Click here'><i style = 'padding-right:3%;' class='fa fa-download'></i> Download</ button>";
                            }
                        }
                    }
                }
                resModel.TotalRecords = detailList.Count;
                resModel.queryExecutionStatusReportDetailList = detailList;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                //if (dbContext != null)
                //    dbContext.Dispose();
            }
            return resModel;
        }
    }
}