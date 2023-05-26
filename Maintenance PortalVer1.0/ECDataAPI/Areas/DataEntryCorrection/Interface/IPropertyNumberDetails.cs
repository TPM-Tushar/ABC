using CustomModels.Models.DataEntryCorrection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.DataEntryCorrection.Interface
{
    interface IPropertyNumberDetails
    {
        PropertyNumberDetailsViewModel GetPropertyNoDetailsView(int OfficeID, int PropertyID, long OrderId);
        DataEntryCorrectionResultModel LoadPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel);
        DataEntryCorrectionResultModel SelectBtnClick(DataEntryCorrectionViewModel dataEntryCorrectionViewModel);
        //Added By Madhur 29-07-2021
        bool DeletePropertyNoDetails(long keyID);

        bool DeactivatePropertyNoDetails(long keyID, int OrderId);

        //Added by Madhusoodan on 18/08/2021 to activate PropertyNoDetails
        bool ActivatePropertyNoDetails(long keyID, int OrderId);

        //Added by Shivam B on 10/05/2022 Populate SROList on DRO Change
        PropertyNumberDetailsAddEditModel GetSROListByDROCode(int DroCode);
        //Added by Shivam B on 10/05/2022 Populate SROList on DRO Change


        //added by mayank on 16/08/2021
        PropertyNumberDetailsAddEditModel GetVillageBySROCode(int SroCode);
        EditbtnResultModel EditBtnProperty(int OrderID);
        //Added By Madhur 29-07-2021

        AddPropertyNoDetailsResultModel AddUpdatePropertyNoDetails(PropertyNumberDetailsViewModel propertyNumberDetailsViewModel);
        AddPropertyNoDetailsResultModel UpdatePropertyNoDetails(PropertyNumberDetailsViewModel propertyNumberDetailsViewModel);

        PropertyNumberDetailsAddEditModel GetHobliDetailsOnVillageSroCode(long VillageCode, int SroCode);
    }
}
