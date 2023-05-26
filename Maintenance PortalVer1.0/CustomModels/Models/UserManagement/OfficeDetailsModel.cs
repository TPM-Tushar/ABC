using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.UserManagement
{
    public class OfficeGridWrapperModel
    {
        public OfficeDataColumn[] ColumnArray { get; set; }
        public OfficeDetailsModel[] dataArray { get; set; }
        // add another class array  ( document status wise )if required and at mvc controller assign it conditionally before generating response
    }
    public class OfficeDataColumn
    {
        public string title { get; set; } // represents Label at table header
        public string data { get; set; } // represents property name which you want to bind a corresponding label
    }
    public class OfficeDetailsModel
    {

        //public int OfficeTypeId { get; set; }
        //public String OfficeNameE{ get; set; }
        public String EncryptedId { get; set; }
        public int OfficeId { get; set; }
        [Required(ErrorMessage = "Office Name is required.")]
        [Display(Name = "Office Name ")]
        [RegularExpression(@"^[a-zA-Z0-9-. ]+$", ErrorMessage = "Office Name should contain alphabets & digits only")]
        [StringLength(150, ErrorMessage = "Office Name shouldn't be greater than 150 char")]
        public String OfficeNameE { get; set; }

        //[Required(ErrorMessage = "Please Enter The Office Name")]
        [Display(Name = "Office Name(R) ")]
        [StringLength(150, ErrorMessage = "Office Name R shouldn't be greater than 150 char")]
        [RegularExpression(@"^[a-zA-Z0-9-. ]+$", ErrorMessage = "Office Name R should contain alphabets & digits only")]
        public String OfficeNameR { get; set; }

        [Required(ErrorMessage = "Short Name is required.")]
        [Display(Name = "Short Name ")]
        [StringLength(5, ErrorMessage = "Short Name shouldn't be greater than 5 char")]
        [RegularExpression(@"^[a-zA-Z0-9-. ]+$", ErrorMessage = "Short Name should contain alphabets & digits only")]
        public String ShortNameE { get; set; }

        //[Required(ErrorMessage = "Please Enter The Short Name")]
        [Display(Name = "Short Name(R) ")]
        [StringLength(50, ErrorMessage = "Name must not be more than 5 char")]
        public String ShortNameR { get; set; }

        public int GauriOfficeCode { get; set; }
        public int BhoomicensusCode { get; set; }

        [Display(Name = "Any Where Register Enabled")]
        public Boolean AnyWhereRegEnabled { get; set; }
        public String IsAnyWhereRegEnabledIcon { get; set; }

        [Required(ErrorMessage = "Office Address is required.")]
        [Display(Name = "Office Address ")]
        [StringLength(500, ErrorMessage = "Address must not be more than 500 char")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,500}", ErrorMessage = "Invalid office address.")]
        public String OfficeAddress { get; set; }

        //for populatong list
        [Display(Name = "Office Type")]
        public List<SelectListItem> OfficeTypeList { get; set; }
     //   [RegularExpression("^[1-9]*$", ErrorMessage = "Office Type is required.")]
        [Required(ErrorMessage = "Office Type is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Office Type is required.")]
        public int OfficeTypeId { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DistrictsList { get; set; }
        //[RegularExpression("^[1-9]*$", ErrorMessage = "District is required.")]
        [Required(ErrorMessage = "District is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "District is required.")]
        public int DistrictId { get; set; }


        #region Commented by shubham bhagat on 10-4-2019 requirement change
        // [Display(Name = "Taluka")]
        // public List<SelectListItem> TalukaList { get; set; }
        //// [RegularExpression("^[1-9]*$", ErrorMessage = "Taluka is required.")]
        // public int TalukaId { get; set; }
        #endregion



        [Display(Name = "Parent Office List")]
        public List<SelectListItem> ParentOfficeList { get; set; }
        [Display(Name = "Parent Office ")]
       // [RegularExpression("^[1-9]*$", ErrorMessage = "Parent Office is required.")]
        [Required(ErrorMessage = "Parent Office is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Parent Office is required.")]
        public int ParentOfficeId { get; set; }


        public String EditBtn { get; set; }
        public String DeleteBtn { get; set; }

        public bool IsForUpdate { get; set; }


        //for dispaly purpose
        public String ParentOfficeName { get; set; }
        public String DistrictName { get; set; }
        public String OfficeTypeName { get; set; }

        // For Activity Log
        public long UserIdForActivityLogFromSession { get; set; }



        // Changes on 15-12-2018 Final Changes in User Management

        public String ResponseMessage { get; set; }
        public bool ResponseStatus { get; set; }

        public bool IsAnyFirmRegisteredForCurrentOffice { get; set; }

        #region Commented by shubham bhagat on 10-4-2019 requirement change
        //public bool displayTalukaListHidden { get; set; }
        #endregion

        #region 5-4-2019 For Table LOG by SB
        public String UserIPAddress { get; set; }

        #endregion

    }

}
