using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.CourtOrderDetails
{
    public class CourtOrderDetailsResModel
    {
        public List<CourtOrderDetailsModel> CourtOrderDetailsList { get; set; }
        public Decimal TotalAmount { get; set; }
    }
    public class CourtOrderDetailsModel
    {
        public int SrNo { get; set; }
        public string OrderNumber { get; set; }
        public string IssuingAuthority { get; set; }
        public string IssueDate { get; set; }
        public string DataEntryDate { get; set; }
        public string SROName { get; set; }
        public string RegistrationArticle { get; set; }
        public string Cancelation { get; set; }
        public string Description { get; set; }
        public string PropertyDetails { get; set; }
        public string PartyDetails { get; set; }
        public string Action { get; set; }
        public Decimal Amount { get; set; }
    }
}
