using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.QueryExecutionStatusReport
{
    public class QueryExecutionStatusReportModel
    {
        public string ReplicaType { get; set; }

        [Display(Name = "Select Database")]
        public List<SelectListItem> DatabaseList { get; set; }
        //public int DatabseId { get; set; }
        public string DatabaseName { get; set; }

        [Display(Name = "From Date")]
        public string FromDate { get; set; }

        //public DateTime FromDateTime { set; get; }

        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        [Required(ErrorMessage = "Top Rows is mandatory.")]
        [RegularExpression(@"^[0-9][0-9]{0,9}$", ErrorMessage = "Invalid Top Rows value, only +ve numbers are allow with max length 10")]
        [Display(Name = "Top Rows")]
        public int TopRows { get; set; }
        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsExcel { get; set; }


    }

    public class QueryExecutionStatusReportResModel
    {
        public List<QueryExecutionStatusReportDetailModel> queryExecutionStatusReportDetailList { get; set; }
        public Decimal TotalAmount { get; set; }
        public int TotalRecords { get; set; }

    }

    public class QueryExecutionStatusReportDetailModel
    {
        public int SrNo { get; set; }
        //public string DatabaseName { get; set; }
        public string QuerySqlText { get; set; }
        public string Query { get; set; }
        //public string ObjectName { get; set; }
        //public string ReplicaName { get; set; }

        public string QueryPlanXML { get; set; }
        public string QueryPlanXMLButton { get; set; }
        public string LastExecutionTime { get; set; }
        public double CountExecutions { get; set; }
        //public double MaxDurationSeconds { get; set; }
       // public double AvgDurationSeconds { get; set; }
        //public double LastDurationSeconds { get; set; }
        //public double MaxCpuTimeSeconds { get; set; }
        //public double AvgCpuTimeSeconds { get; set; }
       // public double LastCpuTimeSeconds { get; set; }
       // public double MaxPhysicalIOReads { get; set; }
       // public double AvgPhysicalIOReads { get; set; }
        //public double LastPhysicalIOReads { get; set; }
       // public double MaxLogicalIOReads { get; set; }
        //public double AvgLogicalIOReads { get; set; }
       // public double LastLogicalIOReads { get; set; }
       // public double MaxLogicalIOWrites { get; set; }
       // public double AvgLogicalIOWrites { get; set; }
        //public double LastLogicalIOWrites { get; set; }
       // public double MaxTempdbSpaceUsed { get; set; }
        //public double AvgTempdbSpaceUsed { get; set; }
        //public double LastTempdbSpaceUsed { get; set; }
        //public double MaxQueryMaxUsedMemory8kPages { get; set; }
        //public double AvgQueryMaxUsedMemory8kPages { get; set; }
        //public double LastQueryMaxUsedMemory8kPages { get; set; }
        //public double MaxRowCount { get; set; }
        //public double AvgRowCount { get; set; }
        //public double LastRowCount { get; set; }


        //public int SrNo { get; set; }
        //public string DatabaseName { get; set; }
        //public string QuerySqlText { get; set; }
        //public string ReplicaName { get; set; }
        //public string QueryPlanXML { get; set; }
        //public string LastExecutionTime { get; set; }
        //public string CountExecutions { get; set; }
        //public string MaxDurationSeconds { get; set; }
        //public string AvgDurationSeconds { get; set; }
        //public string LastDurationSeconds { get; set; }
        //public string MaxCpuTimeSeconds { get; set; }
        //public string AvgCpuTimeSeconds { get; set; }
        //public string LastCpuTimeSeconds { get; set; }
        //public string MaxPhysicalIOReads { get; set; }
        //public string AvgPhysicalIOReads { get; set; }
        //public string LastPhysicalIOReads { get; set; }
        //public string MaxLogicalIOReads { get; set; }
        //public string AvgLogicalIOReads { get; set; }
        //public string LastLogicalIOReads { get; set; }
        //public string MaxLogicalIOWrites { get; set; }
        //public string AvgLogicalIOWrites { get; set; }
        //public string LastLogicalIOWrites { get; set; }
        //public string MaxTempdbSpaceUsed { get; set; }
        //public string AvgTempdbSpaceUsed { get; set; }
        //public string LastTempdbSpaceUsed { get; set; }
        //public string MaxQueryMaxUsedMemory8kPages { get; set; }
        //public string AvgQueryMaxUsedMemory8kPages { get; set; }
        //public string LastQueryMaxUsedMemory8kPages { get; set; }
        //public string MaxRowCount { get; set; }
        //public string AvgRowCount { get; set; }
        //public string LastRowCount { get; set; }


        // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 21-05-2021
        public string creation_time { get; set; }
        public string total_worker_time { get; set; }
        public string max_worker_time { get; set; }
        public string last_worker_time { get; set; }
        public string total_elapsed_time { get; set; }
        public string max_elapsed_time { get; set; }
        public string last_elapsed_time { get; set; }
        public double total_physical_reads { get; set; }
        public double max_physical_reads { get; set; }
        public double last_physical_reads { get; set; }
        public double total_logical_reads { get; set; }
        public double max_logical_reads { get; set; }
        public double last_logical_reads { get; set; }
        public double total_logical_writes { get; set; }
        public double max_logical_writes { get; set; }
        public double last_logical_writes { get; set; }

        // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 21-05-2021





    }
}
