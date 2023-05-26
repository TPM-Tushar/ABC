using CustomModels.Models.PendingDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.PendingDocuments.Interface
{

    public interface IPendingDocuments
    {

        PendingDocumentsViewModel PendingDocumentsView(int officeID);
        List<PendingDocumentsTableModel> PendingDocumentsAvailaibility(int SROCode);
    }
}