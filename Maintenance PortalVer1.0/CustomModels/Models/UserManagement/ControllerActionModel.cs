using CustomModels.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.ControllerAction
{
    public class ControllerActionModel
    {
        [Display(Name = "ID")]
        public int CAID { get; set; }
        public string EncryptedID { get; set; }    

        [Display(Name = "Is Active")]
        [Required(ErrorMessage = "Active Status Required")]
        public bool IsActive { get; set; }

 
        public List<SelectListItem> RoleList { get; set; }
        [Display(Name = "Role")]
        // [RegularExpression("^[1-9]*$", ErrorMessage = "Please select Role")]
      //  [Required(ErrorMessage ="Please Select Role")]     
        public string[] RoleId { get; set; }
        public string role { get; set; }

        public List<SelectListItem> AreaList { get; set; }
        [Display(Name = "Area")]
        [Required(ErrorMessage = "Area is required")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Please select Area")]
        public String AreaNameId { get; set; }

        public List<SelectListItem> ControllerList { get; set; }
        [Display(Name = "Controller")]
        [Required(ErrorMessage = "Controller is required")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Please select Controller")]
        public String ControllerNameId { get; set; }


        public List<SelectListItem> ActionList { get; set; }
        [Display(Name = "Action")]
        [Required(ErrorMessage = "Action is required")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Please select Action")]
        public String ActionNameId { get; set; }


        public bool IsForUpdate { get; set; }

        // For Activity Log
        public long UserIdForActivityLogFromSession { get; set; }

        // Added on Shubham Bhagat on 18-12-2018
        public String ControllerNameId_Hidden { get; set; }
        public String ActionNameId_Hidden { get; set; }

        #region 5-4-2019 For Table LOG by SB
        public String UserIPAddress { get; set; }

        #endregion

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")] 
        public String Description { get; set; }

        public List<SelectListItem> MenuDetailsList { get; set; }
      //  [Range(1, int.MaxValue, ErrorMessage = "Invalid menu.")]

        [Display(Name ="Menu")]
        [Required(ErrorMessage = "Please Select Menu")]
        public int[]  MenuDetailsId { get; set; }


        public String AssignedToRoles { get; set; }
        public String ForMenu { get; set; }
        public String IsActiveStr { get; set; }
        public String Edit { get; set; }
        public String Delete { get; set; }
        public int  SrNo { get; set; }

        [Display(Name = "Is For Menu Action Mapping")]

        public bool IsForMenuActionMapping { get; set; }




    }
    public class KaveriAction
    {
        public string Name { get; set; }

        public bool IsHttpPost { get; set; }

        public bool IsHttpGet { get; set; }

        public bool IsHttpOther { get; set; }

    }

    public class KaveriController
    {
        public string Name { get; set; }

        public string Namespace { get; set; }

        public List<KaveriAction> MyActions { get; set; }
    }

    public class KaveriArea
    {
        public string Name { get; set; }

        public IEnumerable<string> Namespace { get; set; }

        public List<KaveriController> KaveriControllers { get; set; }
    }
    public class ControllerActionDataModel
    {
        public List<KaveriArea> AreaList { get; set; }
        public ControllerActionModel model { get; set; }
        public string EncryptedId { get; set; }
        public string ControllerName { get; set; }
        public string AreaName { get; set; }
    }
}
