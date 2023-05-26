using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.BlockingProcesses
{
    public class BlockingProcessResponseModel
    {


        public string session_id { get; set; }
        public string command { get; set; }
        public string blocking_session_id { get; set; }
        public string wait_type { get; set; }
        public int wait_time { get; set; }
        public string wait_resource { get; set; }
        public string TEXT { get; set; }
        public System.DateTime DateTime { get; set; }
    }


    public class BlockingProcessWrapperModel
    {

        public List<BlockingProcessResponseModel> BlockingProcessList { get; set; }

    }
}
