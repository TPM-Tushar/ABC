using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Dashboard
{
    public class DashboardPopupViewModel
    {

        public string heading { get; set; }
        public string PopupType { get; set; }
        public List<DashboardPopupContent> PopupContentList { get; set; }
        public List<RevenueCollectionModel> RevenueModelList { get; set; }

        // ADDED BY PANKAJ SAKHARE ON 21-09-2020
        public List<AverageRegTimeDetailsModel> AverageRegTimeDetailsModelList { get; set; }



        
    }

    public class DashboardPopupReqModel
    {

        public string selectedType { get; set; }
        public int SelectedOffice { get; set; }
        public string PopupType { get; set; }

        //Added By Raman Kalegaonkar on 23-06-2020
        [Display(Name = "Fin. Year")]
        public List<SelectListItem> FinYearList { get; set; }
        public int FinYearId { get; set; }


    }
    public class DashboardPopupContent
    {
        public string OFFICE_NAME { get; set; }
        public string AVG_START_TIME { get; set; }
        public string HIERARCHY { get; set; }
        public Nullable<long> seqno { get; set; }

    }
}
