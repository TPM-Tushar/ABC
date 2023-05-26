using CustomModels.Models.MenuHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.HomePage
{
    public class HomePageModel
    {

        public HomePageModel()
        {
            //        suboduleDetails = new SubModuleDetails();
        }







        public short RoleID { get; set; }
        public List<MenuItems> MenuListReturn { get; set; }
        public long UserID { get; set; }
        public short ModuleID { get; set; }


        public int DefaultModuleID { get; set; }
        public string DefaultModuleName { get; set; }

        public string UserName { get; set; }

        // added by akash
        public List<SelectListItem> Months { get; set; }
        [Display(Name = "Month")]
        [Required(ErrorMessage = "Month is required")]
        [Range(1, Int32.MaxValue, ErrorMessage = "• Month is required.")]
        public int MonthID { get; set; }

        public List<SelectListItem> Years { get; set; }
        [Display(Name = "Year")]
        [Required(ErrorMessage = "Year is required")]
        [Range(1, Int32.MaxValue, ErrorMessage = "• Year is required.")]
        public int YearID { get; set; }


        public short IsMobileNumberVerfied { get; set; } // shrinivas



    }

}
