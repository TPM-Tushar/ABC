using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.XELFiles
{
    public class RegisteredJobsListModel
    {
        public List<RegisteredJobsModel> RegisteredJobsModelLst { get; set; }
    }

    public class RegisteredJobsModel
    {
        public long JobID { get; set; }

        public int SROCode { get; set; }

        public string SROOfficeName { get; set; }
        public int FromYear { get; set; }

        public int ToYear { get; set; }
        public string FromMonth { get; set; }
        public string ToMonth { get; set; }

        public string RegisteredDateTime { get; set; }

        public string CompletedDateTime { get; set; }

        public bool IsJobCompleted { get; set; }

        public string Description { get; set; }
        public string OfficeType { get; set; }
    }
}
