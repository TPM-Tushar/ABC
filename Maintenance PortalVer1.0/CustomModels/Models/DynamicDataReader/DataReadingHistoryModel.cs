using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.DynamicDataReader
{
    public class DataReadingHistoryModel
    {
        [Display(Name = "Select Database")]
        public List<SelectListItem> DatabaseList { get; set; }
        public string DatabaseName { get; set; }

        [Display(Name = "From Date")]
        public string FromDate { get; set; }

        [Display(Name = "To Date")]
        public String ToDate { get; set; }
        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsExcel { get; set; }
        public short CurrentRoleID { get; set; }


    }

    public class DataReadingHistoryDetailModel
    {
        public int SrNo { get; set; }
        public int QueryID { get; set; }
        public string DBName { get; set; }
        public string Date { get; set; }
        public int NoOfRows { get; set; }
        public string Purpose { get; set; }
        public string QueryText { get; set; }
        public string LoginName { get; set; }
        public string DBUserName { get; set; }
        public string QueryResultButtons { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }

    }

    public class DataReadingHistoryResModel
    {
        public List<DataReadingHistoryDetailModel> dataReadingHistoryDetailModels { get; set; }
        public Decimal TotalAmount { get; set; }
        public int TotalRecords { get; set; }

    }




}
