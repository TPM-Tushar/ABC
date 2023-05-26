using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ESignConsumptionReport
{
    public class ESignStatusDetailsResModel
    {
        public List<ESignStatusDetailsTableModel> ESignStatusDetailsTableList { get; set; }

        public int TotalCount { get; set; }

    }

    public class ESignStatusDetailsTableModel
    {
        public int SerialNo { get; set; }

        public string ApplicationNumber { get; set; }
        public string ApplicationType { get; set; }
        public string ApplicationDate { get; set; }
        public string ESignRequestDate { get; set; }
        public string ESignRequestTransactionNo { get; set; }
        public string ESignResponseDate { get; set; }
        public string ESignResponseTransactionNo { get; set; }
        public string ESignResponseCode { get; set; }
        public string Status { get; set; }   // Success/Fail
        public string ApplicationStatus { get; set; } //App Document Status
        public string ApplicationSubmitDate { get; set; }
        public string ResponseErrorCode { get; set; }
        public string ResponseErrorMessage { get; set; }
    }
}
