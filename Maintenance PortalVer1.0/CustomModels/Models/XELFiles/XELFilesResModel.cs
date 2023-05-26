using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.XELFiles
{
    public class XELFilesResModel
    {
        public List<XELFilesModel> xelFilesModellST { get; set; }
    }

    public class XELFilesModel
    {
        public int SrNo { get; set; }

        public int SROCode { get; set; }

        public string OfficeName { get; set; }

        public string EventTime { get; set; }

        public string LoginName { get; set; }

        public string ServerName { get; set; }

        public string DatabaseName { get; set; }

        public string ApplicationName { get; set; }

        public string Statement { get; set; }

        public string HostName { get; set; }
        public string OfficeType { get; set; }

        


    }
}
