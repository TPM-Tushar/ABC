using CustomModels.Models.Remittance.NotReadableDoc;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class NotReadableDocBAL : INotReadableDoc
    {
        NotReadableDocDAL notReadableDoc = new NotReadableDocDAL();
        public NotReadableDocModel NotReadableDocView(int OfficeID)
        {
            return notReadableDoc.NotReadableDocView(OfficeID);
        }

      
        public NotReadableDocResultModel GetNotReadableDocDetails(NotReadableDocModel notReadableDocModel)
        {
            return notReadableDoc.GetNotReadableDocDetails(notReadableDocModel);
        }
        
    }
}