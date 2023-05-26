using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Alerts
{
    public class SendSMSRequestModel
    {
        public string userName { get; set; }
        public string webServiceToken { get; set; }
        public string messageToBeSent { get; set; }
        [Display(Name = "Contact No")]
        public string toContact { get; set; }
        public DateTime scheduleDeliveryDateTime { get; set; }
    }
}
