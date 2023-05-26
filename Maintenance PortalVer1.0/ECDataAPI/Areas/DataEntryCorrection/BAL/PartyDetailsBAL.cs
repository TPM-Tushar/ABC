#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   
    * Author Name       :   
    * Creation Date     :   
    * Last Modified By  :   
    * Last Modified On  :   
    * Description       :   
*/
#endregion

using CustomModels.Models.DataEntryCorrection;
using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Areas.DataEntryCorrection.DAL;
using ECDataAPI.Areas.DataEntryCorrection.Interface;
using ECDataAPI.Areas.SupportEnclosure.Interface;
using ECDataAPI.EcDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.SupportEnclosure.BAL
{
    public class PartyDetailsBAL : IPartyDetails
    {

        IPartyDetails partyDetailsDAL = new PartyDetailsDAL();

        public PartyDetailsViewModel GetPartyDetailsView(int OfficeID, int PartyID)
        {
            return partyDetailsDAL.GetPartyDetailsView(OfficeID, PartyID);
        }

        public AddPartyDetailsResultModel AddUpdatePartyDetails(PartyDetailsViewModel partyDetailsViewModel)
        {
            return partyDetailsDAL.AddUpdatePartyDetails(partyDetailsViewModel);
        }
        //Added by Madhur 29-07-2021
        public DataEntryCorrectionResultModel LoadPropertyDetailsPartyTabData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            return partyDetailsDAL.LoadPropertyDetailsPartyTabData(dataEntryCorrectionViewModel);
        }
 
        public DataEntryCorrectionResultModel SelectBtnPartyTabClick(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            return partyDetailsDAL.SelectBtnPartyTabClick(dataEntryCorrectionViewModel);
        }

        public string EditBtnClickOrderTable(string DROrderNumber)
        {
            return partyDetailsDAL.EditBtnClickOrderTable(DROrderNumber);
        }

        public bool DeletePartyDetails(long KeyId)
        {
            return partyDetailsDAL.DeletePartyDetails(KeyId);
        }
        public bool DeactivatePartyDetails(long KeyId, int OrderId)
        {
            return partyDetailsDAL.DeactivatePartyDetails(KeyId,OrderId);
        }
        //Added by Madhur 29-07-2021
    }
}