using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Common
{
    

        public class EventAuditLoging
        {
            public long EventAuditID { get; set; }
            public long UserID { get; set; }
            public string UrlAccessed { get; set; }
            public string IPAddress { get; set; }
            public string Description { get; set; }
            public string AuditDataTime { get; set; }
            public short ServiceID { get; set; }

        }
    
}
