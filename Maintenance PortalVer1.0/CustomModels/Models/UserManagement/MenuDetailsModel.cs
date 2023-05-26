using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.UserManagement
{
    public class MenuDetailsModel
    {
        public String EncryptedID { get; set; }

        public int MenuID { get; set; }

        [Required(ErrorMessage = "Menu Name Required.")]
        //Changed by mayank
        //[RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Menu Name must be alphabet.")]
        [RegularExpression(@"^[a-zA-Z''-'\s0-9 ()]{1,40}$", ErrorMessage = "Menu Name must be alphabet.")]
        public string MenuName { get; set; }

        [Required(ErrorMessage = "Menu Name R Required.")]
        //[RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Menu Name R must be alphabet.")]
        [RegularExpression(@"^[a-zA-Z''-'\s0-9 ()]{1,40}$", ErrorMessage = "Menu Name R must be alphabet.")]
        public string MenuNameR { get; set; }


        //[RegularExpression("^(\\+?1?( ?.?-?\\(?\\d{3}\\)?) ?.?-?)?(\\d{3})( ?.?-? ?\\d{4})$", ErrorMessage = "Please enter a properly formatted Phone.")]
        //[DataType(DataType.Custom)]

        //[Display(Name = "Parent ID")]
        //[Required(ErrorMessage ="Parent ID Required.")]
        //[RegularExpression(@"^[0-9]*$",ErrorMessage ="Invalid Parent ID")]
        //[Range(0,32767,ErrorMessage ="Value for {0} must be between {1} and {2}.")]
        //public String ParentID { get; set; }
        public int ParentID { get; set; }

        [Display(Name = "Sequence")]
        [Required(ErrorMessage = "Sequence Required.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Sequence must be number.")]
        [Range(0, 32767, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public String Sequence { get; set; }
        //public short Sequence { get; set; }

        [Display(Name = "Vertical Level")]
        [Required(ErrorMessage = "Vertical Level Required.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Vertical Level must be number.")]
        [Range(0, 32767, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public String VerticalLevel { get; set; }
        //public short VerticalLevel { get; set; }

        [Display(Name = "Horizontal Sequence")]
        [Required(ErrorMessage = "Horizontal Sequence Required.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Horizontal Sequence must be number.")]
        [Range(0, 32767, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public String HorizontalSequence { get; set; }
        //public short  HorizontalSequence { get; set; }

        public bool IsActive { get; set; }

        //public String IsActiveString { get; set; }


        [Display(Name = "Level Group Code")]
        [Required(ErrorMessage = "Level Group Code Required.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Level Group Code must be number.")]
        [Range(0, 32767, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public String LevelGroupCode { get; set; }
        //public int LevelGroupCode { get; set; }

        [Display(Name = "Is Menu ID Parameter")]
        [Required(ErrorMessage = "Is Menu ID Parameter Required.")]
        //[Range(typeof(bool), "false", "true", ErrorMessage = "Is Menu ID Parameter Required.")]
        public Nullable<bool> IsMenuIDParameter { get; set; }

        //public String IsMenuIDParameterTemp { get; set; }

        public bool IsHorizontalMenu { get; set; }

        public bool IsUpdatable { get; set; }

        //public List<SelectListItem> RoleList { get; set; }

        //public int[] RoleListIds { get; set; }

        public List<SelectListItem> MenuList { get; set; }

        //public int TempParentId { get; set; }

        public String ParentMenu { get; set; }

        public MenuDetailsResponseModel MenuDetailsResponseModel { get; set; }

        public List<SelectListItem> FirstChildMenuDetailsList { get; set; }

        public int FirstChildMenuDetailsId { get; set; }

        public List<SelectListItem> SecondChildMenuDetailsList { get; set; }

        public int SecondChildMenuDetailsId { get; set; }

        public bool DropDownValuesCanChange { get; set; }

        public String MenuActionMappingButton { get; set; }

        // [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Please select Area")]
        public List<SelectListItem> ControllerActionDetails_AreaList { get; set; }

        //public int ControllerActionDetails_AreaListId { get; set; }
        [Required(ErrorMessage = "Area is required")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Please select Area")]
        public String ControllerActionDetails_AreaListId { get; set; }

        public List<SelectListItem> ControllerActionDetails_ControllerList { get; set; }
        [Required(ErrorMessage = "Controller is required")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Please select Controller")]
        public String ControllerActionDetails_ControllerListId { get; set; }
        //public int ControllerActionDetails_ControllerListId { get; set; }

        public List<SelectListItem> ControllerActionDetails_ActionList { get; set; }
        [Required(ErrorMessage = "Action is required")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Please select Action")]
        public String ControllerActionDetails_ActionListId { get; set; }
        //public int ControllerActionDetails_ActionListId { get; set; }

        public List<SelectListItem> MAS_Modules_ModuleList { get; set; }
        [Required(ErrorMessage = "Module is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Module is required.")]
        public int MAS_Modules_ModuleListId { get; set; }

        public String MapUnmapMenuActionButton { get; set; }

        public String DropDownValuesStatus { get; set; }
        public String ActionAssigned { get; set; }

        // For Activity Log
        public long UserIdForActivityLogFromSession { get; set; }

        //public bool IsParentMenuListUpdatable { get; set; }

        //public bool IsFirstChildMenuListUpdatable { get; set; }

        //public bool IsSecondChildMenuListUpdatable { get; set; }

        #region 5-4-2019 For Table LOG by SB
        public String UserIPAddress { get; set; }
        public String MenuIcon { get; set; }

        [MaxLength(255)]
        public String MenuDescription { get; set; }

        #endregion
    }
}
