using CustomModels.Models.MISReports.DocumentReferences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IDocumentReferences
    {
        DocumentReferencesREQModel DocumentReferencesView(int OfficeID);
        DocumentReferencesWrapper DocumentReferencesDetails(DocumentReferencesREQModel model);
        int DocumentReferencesCount(DocumentReferencesREQModel model);
    }
}
