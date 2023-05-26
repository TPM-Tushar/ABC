#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsBAL.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   BAL for Support Enclosure
*/
#endregion

using CustomModels.Models.DataEntryCorrection;
using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Areas.DataEntryCorrection.Interface;
using ECDataAPI.Areas.SupportEnclosure.DAL;
using ECDataAPI.Areas.SupportEnclosure.Interface;
using ECDataAPI.EcDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.SupportEnclosure.BAL
{
    public class PropertyNumberDetailsBAL : IPropertyNumberDetails
    {

        IPropertyNumberDetails properNoDetailsDAL = new PropertyNumberDetailsDAL();

        public PropertyNumberDetailsViewModel GetPropertyNoDetailsView(int OfficeID, int PropertyID, long OrderId)
        {
            return properNoDetailsDAL.GetPropertyNoDetailsView(OfficeID, PropertyID,OrderId);
        }

        public DataEntryCorrectionResultModel LoadPropertyDetailsData(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            return properNoDetailsDAL.LoadPropertyDetailsData(dataEntryCorrectionViewModel);
        }
        public DataEntryCorrectionResultModel SelectBtnClick(DataEntryCorrectionViewModel dataEntryCorrectionViewModel)
        {
            return properNoDetailsDAL.SelectBtnClick(dataEntryCorrectionViewModel);
        }

        public AddPropertyNoDetailsResultModel AddUpdatePropertyNoDetails(PropertyNumberDetailsViewModel propertyNumberDetailsViewModel)
        {
            return properNoDetailsDAL.AddUpdatePropertyNoDetails(propertyNumberDetailsViewModel);
        }

        //Added by Madhur 29-07-2021
        public EditbtnResultModel EditBtnProperty(int OrderID)
        {
            return properNoDetailsDAL.EditBtnProperty(OrderID);
        }

        public bool DeletePropertyNoDetails(long keyID)
        {
            return properNoDetailsDAL.DeletePropertyNoDetails(keyID);
        }
         public bool DeactivatePropertyNoDetails(long keyID, int OrderId)
        {
            return properNoDetailsDAL.DeactivatePropertyNoDetails(keyID,OrderId);
        }

        //Added by Shivam B on 10/05/2022 Populate SROList on DRO Change
        public PropertyNumberDetailsAddEditModel GetSROListByDROCode(int DroCode)
        {
            return properNoDetailsDAL.GetSROListByDROCode(DroCode);
        }
        //Added by Shivam B on 10/05/2022 Populate SROList on DRO Change


        //Added by mayank on 16/08/2021
        public PropertyNumberDetailsAddEditModel GetVillageBySROCode(int SroCode)
        {
            return properNoDetailsDAL.GetVillageBySROCode(SroCode);
        }

        public AddPropertyNoDetailsResultModel UpdatePropertyNoDetails(PropertyNumberDetailsViewModel propertyNumberDetailsViewModel)
        {
            return properNoDetailsDAL.UpdatePropertyNoDetails(propertyNumberDetailsViewModel);
        }

        public PropertyNumberDetailsAddEditModel GetHobliDetailsOnVillageSroCode(long VillageCode, int SroCode)
        {
            return properNoDetailsDAL.GetHobliDetailsOnVillageSroCode( VillageCode,  SroCode);
        }

        //Added by Madhusoodan on 18/08/2021 to activate PropertyNoDetails
        public bool ActivatePropertyNoDetails(long keyID, int OrderId)
        {
            return properNoDetailsDAL.ActivatePropertyNoDetails(keyID, OrderId);
        }
        //Added by Madhur 29-07-2021
    }
}