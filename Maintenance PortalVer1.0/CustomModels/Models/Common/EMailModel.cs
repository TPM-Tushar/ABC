using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomModels.Models.Common
{
    public class EmailModel
    {
        public String EmailRecepient { get; set; }
        public String EmailCC { get; set; }
        public String EmailBCC { get; set; }
        public String EmailSubject { get; set; }
        public String EmailContent { get; set; }
        public String EmailDate { get; set; }
        public String RecepientName { get; set; }
        public String RecepientDesignation { get; set; }
        public String RecepientAddress { get; set; }

        public String AttachedFilePath { get; set; }

        public String UserType { get; set; }
        public String ElectorType { get; set; }
    }
}