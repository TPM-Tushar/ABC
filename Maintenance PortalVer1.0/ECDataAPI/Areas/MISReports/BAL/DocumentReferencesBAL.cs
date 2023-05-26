using CustomModels.Models.MISReports.DocumentReferences;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class DocumentReferencesBAL : IDocumentReferences
    {
        public DocumentReferencesREQModel DocumentReferencesView(int OfficeID)
        {
            return (new DocumentReferencesDAL().DocumentReferencesView(OfficeID));
        }

        public DocumentReferencesWrapper DocumentReferencesDetails(DocumentReferencesREQModel model)
        {
            return (new DocumentReferencesDAL().DocumentReferencesDetails(model));
        }

        public int DocumentReferencesCount(DocumentReferencesREQModel model)
        {
            return (new DocumentReferencesDAL().DocumentReferencesCount(model));
        }
    }
}