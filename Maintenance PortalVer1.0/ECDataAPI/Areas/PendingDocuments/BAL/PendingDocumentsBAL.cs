using CustomModels.Models.PendingDocuments;
using ECDataAPI.Areas.PendingDocuments.DAL;
using ECDataAPI.Areas.PendingDocuments.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.PendingDocuments.BAL
{
    public class PendingDocumentsBAL : IPendingDocuments
    {
        IPendingDocuments PendingDocumentsDAL = new PendingDocumentsDAL();

        public PendingDocumentsViewModel PendingDocumentsView(int officeID)
        {
            return PendingDocumentsDAL.PendingDocumentsView(officeID);
        }



        public List<PendingDocumentsTableModel> PendingDocumentsAvailaibility(int SROCode)
        {
            List<PendingDocumentsTableModel> finalList = new List<PendingDocumentsTableModel>();

            try
            {
                return PendingDocumentsDAL.PendingDocumentsAvailaibility(SROCode);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

}