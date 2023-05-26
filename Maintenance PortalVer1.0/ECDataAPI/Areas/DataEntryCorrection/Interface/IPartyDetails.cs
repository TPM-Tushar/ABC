using CustomModels.Models.DataEntryCorrection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.DataEntryCorrection.Interface
{
    interface IPartyDetails
    {
        PartyDetailsViewModel GetPartyDetailsView(int OfficeID, int PartyID);

        AddPartyDetailsResultModel AddUpdatePartyDetails(PartyDetailsViewModel partyDetailsViewModel);
        DataEntryCorrectionResultModel LoadPropertyDetailsPartyTabData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel);
        //Added By Madhur 29-07-2021
        DataEntryCorrectionResultModel SelectBtnPartyTabClick(DataEntryCorrectionViewModel dataEntryCorrectionViewModel);
        bool DeletePartyDetails(long keyId);
        bool DeactivatePartyDetails(long keyId, int OrderId);
        string EditBtnClickOrderTable(string DROrderNumber);
        //Added By Madhur 29-07-2021
    }
}
