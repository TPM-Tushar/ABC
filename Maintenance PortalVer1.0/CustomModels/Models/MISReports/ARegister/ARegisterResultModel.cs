using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.ARegister
{
    public class ARegisterResultModel
    {
        public string Url { get; set; }

        public string ResponseMessage { get; set; }

        public bool ResponseStatus { get; set; }

        public byte[] AregisterFileBytes { get; set; }

        public string MessageforGeneration { get; set; }
    }
   
}
