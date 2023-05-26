using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomModels.Models.LogAnalysis.ECDataAuditDetails
{
    public class ECDataAuditDetailsResponseModel
    {

        public long LogID { get; set; }
        public int LogTypeID { get; set; }
        public int SROCODE { get; set; }
        public long DOCUMENTID { get; set; }
        public long ITEMID { get; set; }
        public string SRONAME { get; set; }
        public string FRN { get; set; }
        public string DATEOFMODIFICATION { get; set; }
        public string MODIFICATION_AREA { get; set; }
        public int MODIFICATION_AREA_ID { get; set; }
        public string MODIFICATION_TYPE { get; set; }
        public string MODIFICATION_DESCRIPTION { get; set; }
        public string ModificationDescForPDF { get; set; }
        public string loginname { get; set; }
        public string IPADRESS { get; set; }
        public string hostname { get; set; }
        public string APPLICATION_NAME { get; set; }
        public string PHYSICAL_ADDRESS { get; set; }
        public System.Collections.Generic.List<MasterTableModel> LstModificationDetails { get; set; }

    }
}