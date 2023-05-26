#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   NotReadableDocModel.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Model for Not Readable Documents Details.

*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.NotReadableDoc
{
   public class NotReadableDocModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }


        public int SROfficeID { get; set; }

        //Added By Tushar on 14 Nov 2022 
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int DROfficeID { get; set; }
        //End By Tushar on 14 Nov 2022
    }
    public class NotReadableDocResultModel
    {
        public List<NotReadableDocTableModel> notReadableDocTableModelList { get; set; }
    }

    public class NotReadableDocTableModel
    {
      public  long srNo { get; set; }
      public  string RegistrationNumber { get; set; }
      public  string LogDateTime { get; set; }

       public string CDNumber { get; set; }

       public string Document_Type { get; set; }

      public  string LogBy { get; set; }

        //Added By Tushar on 20 Dec 2022
        public string Stamp5DateTime { get; set; }
        //End By Tushar on 20 Dec 2022

    }
}
