using CustomModels.Models.DisableKaveri;
using ECDataAPI.Areas.DisableKaveri.DAL;
using ECDataAPI.Areas.DisableKaveri.Interface;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.DisableKaveri.BAL
{
    public class DisableKaveriBAL : IDisableKaveri
    {
        DisableKaveriDAL disableKaveriDAL = new DisableKaveriDAL();
        public DisableKaveriViewModel DisableKaveriView()
        {
            return disableKaveriDAL.DisableKaveriView();
        }

        public UpdateDetailsModel UpdateDisableKaveriDetails(DisableKaveriViewModel disableKaveriViewModel)
        {
            return disableKaveriDAL.UpdateDisableKaveriDetails(disableKaveriViewModel);
        }
        //Added By Tushar on 5 apr 2023
        public MenuDisabledOfficeIDModel GetMenuDisabledOfficeID(MenuDisabledOfficeIDModel menuDisabledOfficeIDModel)
        {
            return disableKaveriDAL.GetMenuDisabledOfficeID(menuDisabledOfficeIDModel);
        }
        //End By Tushar on 5 Apr 2023
    }
}