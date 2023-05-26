using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.KaveriSupport
{
    public class TicketDetailsModel
    {

        public long SrNo { get; set; }
        public string TicketNumber { get; set; }
        public string TicketDescription { get; set; }

        public string ModuleName { get; set; }
        public string Office { get; set; }

        public string RegistrationDateTime { get; set; }

        public bool IsActive { get; set; }



    }



    public class PrivateKeyDetailsModel
    {

        public long SrNo { get; set; }
        public string TicketNumber { get; set; }
        public string FileName { get; set; }
        public string UploadDateTime { get; set; }
        public bool IsActive { get; set; }

    }

    public class TicketDetailsListModel
    {
        public List<TicketDetailsModel> TicketDetailsList { get; set; }

        public List<PrivateKeyDetailsModel> PrivateKeyDetailsList { get; set; }


        public string ErrorMessage{get;set;}

    }




}
