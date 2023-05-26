using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Common
{
   public class ValidateFirmDetailsResponseModel
    {


        public string ValidationMessage { get; set; }

        public string URLToRedirect { get; set; }

        public bool IsAllValidationsTrue { get; set; }

        public string FirmRegistrationNumber { get; set; }

        public bool VerifyMobileNumber { get; set; } // shrinivas for mobile no verification

        public string ResponseMessageRegardingConfiguration { get; set; }

        public string URLToRedirectAfterSuccess { get; set; }


    }
}
