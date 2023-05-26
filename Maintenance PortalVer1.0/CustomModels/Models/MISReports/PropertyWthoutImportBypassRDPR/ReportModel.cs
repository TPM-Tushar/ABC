/*File Header
 * Project Id: 
 * Project Name: Kaveri Maintainance Portal
 * File Name: KaveriIntegrationModel.cs
 * Author : Shubham Bhagat
 * Creation Date : 14 Oct 2019
 * Desc : Model for Kaveri Integration Module
 * ECR No : 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.PropertyWthoutImportBypassRDPR
{
    public class ReportModel
    {
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public DateTime DateTime_FromDate { get; set; }
        public DateTime DateTime_ToDate { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DistrictList { get; set; }
        public int DistrictID { get; set; }

        // For Child table popup
        public String ColumnName { get; set; }
        public int StartLen { get; set; }
        public int TotalNum { get; set; }

        public int SROCode { get; set; }

        public bool IsForExcelDownload { get; set; }
        public bool IsForSearch { get; set; }

        // add by sb on 04-12-2019
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }
    }

    public class ReportWrapperModel
    {
        public List<ReportDetail> ReportDetailList { get; set; }

        public int TotalCount { get; set; }
    }

    public class ReportDetail
    {
        public int SerialNo { get; set; }

        public String SROName { get; set; }

        public String TotalPropertiesRegistered { get; set; }

        public String Bhoomi { get; set; }

        public String E_Swathu { get; set; }

        public String UPOR { get; set; }

        public String Total_Properties_Registered_Without_Importing { get; set; }

        public String DistrictName { get; set; }


        //Added by shubham bhagat on 7-11-2019 for Mojani 
        public String Mojani { get; set; }

    }

    public class ReportDetailsModel
    {
        public int SerialNo { get; set; }
        public string DocumentNumber { get; set; }
        public string PropertyDetails { get; set; }
        public string Executant { get; set; }
        public string Claimant { get; set; }
        public string Reference_AcknowledgementNumber { get; set; }

        public string FinalRegistrationNumber { get; set; }
        public string IntegrationDepartmentName { get; set; }
        public string UploadDate { get; set; }
        public string NatureOfDocument { get; set; }
        public string VillageName { get; set; }

    }

    public class ReportDetailsWrapperModel
    {
        public List<ReportDetailsModel> ReportDetailsModelList { get; set; }

        public int TotalCount { get; set; }

        public String SROName { get; set; }
    }
}
