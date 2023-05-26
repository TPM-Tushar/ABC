using CustomModels.Models.DisableKaveri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.DisableKaveri.Interface
{
   public interface IDisableKaveri
    {
        DisableKaveriViewModel DisableKaveriView();

        UpdateDetailsModel UpdateDisableKaveriDetails(DisableKaveriViewModel disableKaveriViewModel);

        MenuDisabledOfficeIDModel GetMenuDisabledOfficeID(MenuDisabledOfficeIDModel menuDisabledOfficeIDModel);
    }
}
