﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.CDWrittenReport
{
    public class CDWrittenReportViewModel
    {
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }
        public string SROIDEncrypted { get; set; }
        public bool IsExcel { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DistrictList { get; set; }

        public int DistrictID { get; set; }
        public String SearchValue { get; set; }
        public String ColumnName { get; set; }
        public String EncryptedSROCode { get; set; }

        public String ENCSROName { get; set; }

        public String ENCDistrictName { get; set; }

        [Display(Name = "CD Number")]
        public String CDNumber { get; set; }


        [Display(Name = "CD Number")]
        public List<SelectListItem> CDNumberList { get; set; }

        public int CDID { get; set; }

    }


}
