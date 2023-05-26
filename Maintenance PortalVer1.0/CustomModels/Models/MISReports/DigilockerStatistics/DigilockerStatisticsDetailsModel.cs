using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.DigilockerStatistics
{
    public class DigilockerStatisticsDetailsModel
    {
        public long TotalKOSUsers { get; set; }
        public long TotalKOSUsersLinkedToDigilocker { get; set; }
        public long TotalApplicationsSubmitted { get; set; }
        public long TotalApplicationsLinkedToDigilocker { get; set; }
        public long TotalCertificatesIssued { get; set; }
        public long TotalCertificatesPushedToDigilocker { get; set; }
        public long TotalCertificatesSearchedFromDigilocker { get; set; }

    }
}
