using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.DataTransmissionDetails
{
    public class DataTransReqModel
    {
        public bool IsExcel { get; set; }         public String SearchValue { get; set; }
        [Display(Name = "Database Name")]
        public List<SelectListItem> DataBaseList { get; set; }
        public String DBName { get; set; }
        public int StartLen { get; set; }
        public int TotalNum { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROList { get; set; }
        public int SROID { get; set; }
    }
    public class DataTransWrapperModel
    {
        public List<DataTransDetailsModel> DataTransDetailsModelList { get; set; }
        public int TotalRecords { get; set; }

        public String SroName { get; set; }
    }

    public class DataTransDetailsModel
    {
        public int SerialNumber { get; set; }
        public String TableName { get; set; }
        public long Count { get; set; }

    }

}
