using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.FRUITSIntegration
{
    public class KaveriFruitsIntegrationResultModel
    {
        public int TotalCount { get; set; }
        public List<KaveriFruitsIntegrationDetailModel> KaveriFruitsIntegrationDetailList { get; set; }

        public string ResponseCode { get; set; }

        public string  ResponseMsg { get; set; }

        public byte[] Form3 { get; set; }
    }

    public class KaveriFruitsIntegrationDetailModel
    {
        public int Sno { get; set; }
        public string ReferenceNo { get; set; }
        public string Form3 { get; set; }
        public string OfficeName { get; set; }
        public string TranXML { get; set; }
        public string AcknowledgementNo { get; set; }
        public string DataReceivedDate { get; set; }
    }

}
