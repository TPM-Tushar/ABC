using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.UserManagement
{
   public class ControllerActionViewModel
    {
        public List<SelectListItem> filterMenuDetailsList { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Invalid menu.")]
        [Display(Name ="Menu")]
        public int filterMenuDetailsId { get; set; }
        public short CurrentRoleID { get; set; }
    }
}
