using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Models
{
    public class ClientAuthenticationModel
    {
        public string ApiConsumerUserName { get; set; }
        public string ApiConsumerPassword { get; set; }
    }
}