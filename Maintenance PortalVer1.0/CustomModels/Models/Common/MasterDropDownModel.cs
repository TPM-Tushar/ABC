using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Common
{
    public class MasterDropDownModel
    {
        public List<SelectListItem> CountryDropDown { get; set; }
        public List<SelectListItem> StatesDropDown { get; set; }
        public List<SelectListItem> DistrictsDropDown { get; set; }
        public List<SelectListItem> TitleDropDown { get; set; }
        public List<SelectListItem> GenderDropDown { get; set; }
        public List<SelectListItem> ProfessionDropDown { get; set; }
        public List<SelectListItem> MaritalStatusDropDown { get; set; }   
        public List<SelectListItem> NationalityDropDown { get; set; }
        public List<SelectListItem> IdProofsTypeDropDown { get; set; }
        public List<SelectListItem> RelationTypeDropDown { get; set; }
        public List<SelectListItem> FinancialYearDropDown { get; set; }
        public List<SelectListItem> BookTypeDropDown { get; set; }
        
    }
}
