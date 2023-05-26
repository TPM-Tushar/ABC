using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomModels.Models.LogAnalysis.ECDataAuditDetails
{
    public class MasterTableModel
    {
        public string ColumnName { get; set; }
        public string PrevValue { get; set; }
        public string ModifiedValue { get; set; }
        public System.Collections.Generic.List<MasterTableModel> LstModificationDetails { get; set; }

    }
}