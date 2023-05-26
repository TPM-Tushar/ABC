using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.UserManagement
{
    public class RoleDetailsModel
    {
        public String EncryptedID { get; set; }

        public short RoleID { get; set; }

        [Required(ErrorMessage = "Role Name Required.")]
        [RegularExpression(@"^[a-zA-Z0-9-. ]+$", ErrorMessage = "Role Name should contain alphabets & digits")]
       // [RegularExpression(@"^[a-zA-Z 0-9''-'\s]{1,40}$", ErrorMessage = "Role Name must be alphabet and digit.")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Role Name R Required.")]
        [RegularExpression(@"^[a-zA-Z0-9-. ]+$", ErrorMessage = "Role Name R should contain alphabets & digits.")]
        //[RegularExpression(@"^[a-zA-Z 0-9''-'\s]{1,40}$", ErrorMessage = "Role Name R must be alphabet and digit.")]
        public string RoleNameR { get; set; }

        public bool IsActive { get; set; }

        public bool IsForUpdate { get; set; }

        public RoleDetailsResponseModel RoleDetailsResponseModel { get; set; }

        public String MapMenuButton { get; set; }
        public String AssignedMenus { get; set; }

        public List<SelectListItem> ParentMenuDetailsList { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid parent menu.")]
        public int ParentMenuDetailsId { get; set; }
        
        public List<SelectListItem> FirstChildMenuDetailsList { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Invalid first child menu.")]
        public int FirstChildMenuDetailsId { get; set; }

        public List<SelectListItem> SecondChildMenuDetailsList { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Invalid second child menu.")]
        public int SecondChildMenuDetailsId { get; set; }

        public String MapUnmapButtonForParent { get; set; }

        public String MapUnmapButtonForFirstChild { get; set; }

        public String MapUnmapButtonForSecondChild { get; set; }

        public bool IsParentMenuMapped { get; set; }

        public bool IsFirstChildMenuMapped { get; set; }

        public List<String> FirstChildListString { get; set; }

        public List<String> SecondChildListString { get; set; }

        public String ParentMenuName { get; set; }

        public String FirstChildMenuName { get; set; }

        public long UserIdForActivityLogFromSession { get; set; }

        #region 3-4-2019 For Table LOG by SB
        public String UserIPAddress { get; set; }

        public String EditRoleButton { get; set; }
        public String DeleteRoleButton { get; set; }

        #endregion

        #region 09-04-2019 For Level drop down by shubham bhagat
        public List<SelectListItem> LevelList { get; set; }
        [Display(Name = "Level")]
        [Required(ErrorMessage = "Level is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Level is required.")]
        public int LevelID { get; set; }

        public int OldLevelID { get; set; }
        #endregion

        public short RoleIDFromSession { get; set; }
    }
}
