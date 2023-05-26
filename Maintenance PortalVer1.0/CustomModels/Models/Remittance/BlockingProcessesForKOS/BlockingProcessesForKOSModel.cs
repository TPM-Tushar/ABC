using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.BlockingProcessesForKOS
{

    public class BlockingProcessesForKOSReqModel
    {
        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsExcel { get; set; }
    }

    public class BlockingProcessesForKOSModel
    {
        public int SrNo { get; set; }
        public string session_id { get; set; }
        public string command { get; set; }
        public string blocking_session_id { get; set; }
        public string wait_type { get; set; }
        public int wait_time { get; set; }
        public string wait_resource { get; set; }
        public string TEXT { get; set; }
        public string DateTime { get; set; }
    }

    public class BlocingProcessesForKOSWrapperModel
    {
        public List<BlockingProcessesForKOSModel> blockingProcessesModelsList { get; set; }
        public bool IsError { get; set; }
        public String ErrorMessage { get; set; }
        public int TotalRecords { get; set; }

    }
}
