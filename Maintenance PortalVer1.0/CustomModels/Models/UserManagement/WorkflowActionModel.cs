using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.UserManagement
{
    public class WorkflowActionGridWrapperModel
    {
        public WorkflowActionDataColumn[] ColumnArray { get; set; }
        public WorkflowActionModel[] dataArray { get; set; }
        // add another class array  ( document status wise )if required and at mvc controller assign it conditionally before generating response
    }
    public class WorkflowActionDataColumn
    {
        public string title { get; set; } // represents Label at table header
        public string data { get; set; } // represents property name which you want to bind a corresponding label
    }
    public  class WorkflowActionModel
    {
        public int ActionId { get; set; }
        public String EncryptedId { get; set; }
        public String ServiceName { get; set; }
        public String IsActiveIcon { get; set; }

        [Required(ErrorMessage = "Please Enter The Discription")]
        [Display(Name = "Discription")]
        [StringLength(100, ErrorMessage = "Discription must not be more than 100 char")]
        public string Discription { get; set; }


        [Required(ErrorMessage = "Please Enter The Discription(R)")]
        [Display(Name = "Discription(R) ")]
        [StringLength(100, ErrorMessage = "Name must not be more than 100 char")]
        public string DiscriptionR { get; set; }


        [Required(ErrorMessage = "Please Enter The Status Message")]
        [Display(Name = "Status Message ")]
        [StringLength(100, ErrorMessage = "Status Message must not be more than 100 char")]
        public string StatusMessage { get; set; }

        [Required(ErrorMessage = "Please Enter The Status Message(R)")]
        [Display(Name = "Status Message(R) ")]
        [StringLength(100, ErrorMessage = "Status Message must not be more than 100 char")]
        public string StatusMessageR { get; set; }

        public bool isActive { get; set; }
      

        public List<SelectListItem> ServiceList { get; set; }
        [Display(Name = "Service")]
        [RegularExpression("^[1-9]*$", ErrorMessage = "Please select Service")]
        public int ServiceId { get; set; }




        public String EditBtn { get; set; }
        public String DeleteBtn { get; set; }

        public bool IsForUpdate { get; set; }

    }
}
