#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationNoVerificationSummaryReportModel.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Model for Registration No Verification Summary Report .

*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport
{
   public class RegistrationNoVerificationSummaryReportModel
    {

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        public DateTime DateTime_Date { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public DateTime DateTime_ToDate { get; set; }

        [Display(Name = " Document Type")]
        public int DocumentTypeId { get; set; }
        public List<SelectListItem> DocumentType { get; set; }
    }

    public class RegistrationNoVerificationSummaryResultModel
    {
       public List<RegistrationNoVerificationSummaryTableModel> registrationNoVerificationSummaryTableList;

        //Added By Tushar on 28 dec 2022

        public List<RegistrationNoVerificationFirmSummaryTableModelResult> registrationNoVerificationFirmSummaryTableModelResults  { get; set; }
        //End By Tushar on 28 Dec 2022
    }
    public class RegistrationNoVerificationSummaryTableModel
    {
        public long srNo { get; set; }

        public string  SROName { get; set; }

        public int M_M { get; set; }
             
        public int L_Missing { get; set; }
           
        public int L_Additional { get; set; }
        
        public int LP_CNP { get; set; }
          
        public int CNP_LNP { get; set; }
        
        public int CP_LNP { get; set; }
            
        public int SM_M { get; set; }
       
        public int Is_Duplicate { get; set; }

        //Added By Tushar on 28 dec 2022
        public string DistrictName { get; set; }

        public int FirmResult_LA_CNA_Count { get; set; }

        public int FirmResult_CA_LNA_Count { get; set; }

        public int FirmResult_FN_Miss_Count { get; set; }
        public int FirmResult_SC_LA_CNA_Count { get; set; }
        public int FirmResult_SC_CA_LNA_Count { get; set; }

        public int FirmResult_SC_FN_Miss_Count { get; set; }

        //End By Tushar on 28 Dec 2022
    }
    //Added By Tushar on 28 Dec 2022

    public class RegistrationNoVerificationFirmSummaryTableModelResult
    {
        public long RegistrationID { get; set; }

        public DateTime? DateOfRegistration { get; set; }

        public int DroCode { get; set; }

        public string Type { get; set; }
    }

    //End By Tushar on 28 dec 2022
}
