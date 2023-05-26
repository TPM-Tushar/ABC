using CustomModels.Models.Remittance.MissingSacnDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    interface IMissingScanDocument
    {
        MissingScanDocumentModel MissingSacnDocumentView();

        MissingScanDocumentResModel GetMissingScanDocumentDetails(MissingScanDocumentModel notReadableDocModel);
    }
}
