using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.DynamicDataReader
{
    public class ReadNewDataModel
    {
        public long QueryID { get; set; }
        public string Purpose { get; set; }
        [Display(Name = "Select Database")]
        public List<SelectListItem> DatabaseList { get; set; }
        public string DatabaseName { get; set; }
        [Display(Name = "Query Text")]
        public string QueryText { get; set; }
        public string LoginName { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
    }


    public class ReadNewDataResModel
    {
        public bool IsError { get; set; }
        public string ErrorDesc { get; set; }

    }
}
