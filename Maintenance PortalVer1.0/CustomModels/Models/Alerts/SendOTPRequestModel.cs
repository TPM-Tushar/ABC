using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Alerts
{
    public class SendOTPRequestModel
    {
        public string userName { get; set; }
        public string webServiceToken { get; set; }
        [Display(Name = "OTP")]
        public string messageToBeSent { get; set; }
        [Display(Name = "Contact No")]
        public string toContact { get; set; }
        public DateTime scheduleDeliveryDateTime { get; set; }

        public long OTPRequestUserId { get; set; }

        public string OTPRequestEncryptedUId { get; set; }

        public short OTPTypeId { get; set; }
    }
}
