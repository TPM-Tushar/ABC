using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Alerts
{
    public class SendSMSResponseModel
    {
        public string errorString { get; set; }
        public string statusCode { get; set; }
        public string errorCode { get; set; }
        public string smsLogId { get; set; }
        public string queue { get; set; }
        public string toContact { get; set; }
        public DateTime timeStamp { get; set; }

        // Added by shubham bhagat on 20-04-2019 to show mobile number
        public String MobileNumber { get; set; }
    }
}
