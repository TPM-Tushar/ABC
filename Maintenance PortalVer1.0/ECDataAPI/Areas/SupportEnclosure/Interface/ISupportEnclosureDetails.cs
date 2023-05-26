using CustomModels.Models.SupportEnclosure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.SupportEnclosure.Interface
{
    public interface ISupportEnclosureDetails
    {
        SupportEnclosureDetailsViewModel SupportEnclosureDetails(int OfficeID);
        int GetSupportDocumentEnclosureTotalCount(SupportEnclosureDetailsViewModel model);
        SupportEnclosureDetailsResModel GetSupportDocumentEnclosureTableData(SupportEnclosureDetailsViewModel model);
        int GetSupportPartyEnclosureTotalCount(SupportEnclosureDetailsViewModel model);
        SupportEnclosureDetailsResModel GetSupportPartyEnclosureTableData(SupportEnclosureDetailsViewModel model);
        SupportEnclosureDetailsResModel GetSupportDocumentEnclosureBytes(SupportEnclosureDetailsModel model);
    }
}
