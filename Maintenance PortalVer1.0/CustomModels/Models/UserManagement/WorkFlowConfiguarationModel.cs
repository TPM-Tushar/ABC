using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.UserManagement
{
    public class WorkFlowConfigurationGridWrapperModel
    {
        public WorkFlowConfigurationDataColumn[] ColumnArray { get; set; }
        public WorkFlowConfigurationModel[] dataArray { get; set; }
        // add another class array  ( document status wise )if required and at mvc controller assign it conditionally before generating response
    }
    public class WorkFlowConfigurationDataColumn
    {
        public string title { get; set; } // represents Label at table header
        public string data { get; set; } // represents property name which you want to bind a corresponding label
    }
    public class WorkFlowConfigurationModel
    {
        public int WorkFlowId{ get; set; }
        public String EncryptedId { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public String IsActiveIcon { get; set; }

        public List<SelectListItem> ActionList { get; set; }
        [Display(Name = "Action")]
        [RegularExpression("^[1-9]*$", ErrorMessage = "Action is required.")]
        public int ActionId { get; set; }
        public String ActionDesc { get; set; }


        public List<SelectListItem> FromRoleList { get; set; }
        [Display(Name = "From Role")]
       // [RegularExpression("^[1-9]*$", ErrorMessage = "From Role is required.")]
       // [Required(ErrorMessage = "Country is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "From Role is required.")]
        public int FromRoleId { get; set; }
        public String FromRoleDesc { get; set; }


        public List<SelectListItem> ToRoleList { get; set; }
        [Display(Name = "To Role")]
        //[RegularExpression("^[1-9]*$", ErrorMessage = "To Role is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "To Role is required.")]
        public int ToRoleId { get; set; }
        public String ToRoleDesc { get; set; }


        public List<SelectListItem> ServiceList { get; set; }
        [Display(Name = "Service / Module")]
        [Range(1, int.MaxValue, ErrorMessage = "Service is required.")]
        public int ServiceId { get; set; }
        public String ServiceName { get; set; }

        public List<SelectListItem> OfficeList { get; set; }
        [Display(Name = "Office ")]
       // [RegularExpression("^[0-9]*$", ErrorMessage = "Office is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Office is required.")]
        public int OfficeId { get; set; }
        public String OfficeName { get; set; }



        public String EditBtn { get; set; }
        public String DeleteBtn { get; set; }

    
        public bool IsForUpdate { get; set; }

        // Added by Shubham Bhagat on 17-12-2018
        public int ToRoleId_Hidden { get; set; }
        public int FromRoleId_Hidden { get; set; }

        [Display(Name = "To Add Reverse Office Configuration")]
        public bool ToAddReverseOfficeConfiguration { get; set; }

        [Display(Name = "Action")]
        public int ActionId_ReverseofficeConfiguration { get; set; }
        public List<SelectListItem> ActionList_ForReverseofficeConfiguration { get; set; }

        [Display(Name = "From Role")]
        public int FromRoleId_ReverseofficeConfiguration { get; set; }
        public List<SelectListItem> FromRoleList_ForReverseofficeConfiguration { get; set; }

        [Display(Name = "To Role")]
        public int ToRoleId_ReverseofficeConfiguration { get; set; }
        public List<SelectListItem> ToRoleList_ForReverseofficeConfiguration { get; set; }

        public List<SelectListItem> OfficeList_ForReverseofficeConfiguration { get; set; }
        [Display(Name = "Office ")]
        public int OfficeId_ReverseofficeConfiguration { get; set; }

        public List<SelectListItem> ServiceList_ForReverseofficeConfiguration { get; set; }
        [Display(Name = "Service / Module")]
        public int ServiceId_ReverseofficeConfiguration { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive_ReverseofficeConfiguration { get; set; }


        public int ActionId_ReverseofficeConfiguration_Hidden { get; set; }
        public int FromRoleId_ReverseofficeConfiguration_Hidden { get; set; }
        public int ToRoleId_ReverseofficeConfiguration_Hidden { get; set; }
        public int OfficeId_ReverseofficeConfiguration_Hidden { get; set; }
        public int ServiceId_ReverseofficeConfiguration_Hidden { get; set; }
        public bool IsActive_ReverseofficeConfiguration_Hidden { get; set; }


    }
}
