using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.IntegrationCallExceptions
{
    public class IntegrationCallExceptionsModel
    {
        public String OfficeType { get; set; }

        [Display(Name = "Office Name")]
        public int OfficeTypeID { get; set; }

        public List<SelectListItem> OfficeTypeList { get; set; }

        #region For Datatable
        public long Logid { get; set; }

        public String SROCode { get; set; }

        public String ExceptionType { get; set; }

        public String InnerExceptionMsg { get; set; }

        public String ExceptionMsg { get; set; }

        public String ExceptionStackTrace { get; set; }

        public String ExceptionMethodName { get; set; }

        public String LogDate { get; set; }

        public String IsDRO { get; set; }

        public String DRO { get; set; }

        #endregion
    }
}
