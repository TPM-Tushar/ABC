using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Common
{
   public class ErrorPageModel
    {

       public string ErrorMessage { get; set; }
       public string URLToRedirect { get; set; }
       public string ButtonLabel { get; set; }

    }
}
