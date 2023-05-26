using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.DiskUtilization
{
    public class DiskUtilizationREQModel
    {
        public List<ApplicationReqModel> ApplicationList { get; set; }

        [Display(Name = "Application Name")]
        public String ApplicationName { get; set; }


        public int ServerId { get; set; }

        public int StartLen { get; set; }

        public int TotalNum { get; set; }
        public bool IsExcel { get; set; }
        public String SearchValue { get; set; }

    }

    public class DiskUtilizationWrapper
    {

        public List<DiskUtilizationDetail> DiskUtilizationDetailList { get; set; }

        public int TotalRecords { get; set; }

        public bool isError { get; set; }
        public string sErrorMessage { get; set; }
    }

    public class DiskUtilizationDetail
    {
        public int SerialNumber { get; set; }

        public String DriveName { get; set; }

        public String DriveType { get; set; }

        public String FileSystem { get; set; }

        public String TotalSpace { get; set; }

        public String UsedSpace { get; set; }

        public String FreeSpace { get; set; }

        public String FreeSpacePercentage { get; set; }

    }

    public class ApplicationReqModel
    {
        public int ServerID { get; set; }

        public String ServerType { get; set; }

        public String IPAddress { get; set; }

        public String Description { get; set; }

        public bool IsConsidered { get; set; }

        public String Action { get; set; }

    }
}
