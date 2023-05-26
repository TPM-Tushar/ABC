using CustomModels.Models.ChallanNoDataEntryCorrection;
using ECDataAPI.Areas.ChallanNoDataEntryCorrection.DAL;
using ECDataAPI.Areas.ChallanNoDataEntryCorrection.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.ChallanNoDataEntryCorrection.BAL
{
    public class ChallanNoDataEntryCorrectionBAL : IChallanNoDataEntryCorrection
    {
        IChallanNoDataEntryCorrection dalOBJ = new ChallanNoDataEntryCorrectionDAL();

        public ChallanDetailsModel ChallanNoDataEntryCorrectionView(int officeID)
        {
            return dalOBJ.ChallanNoDataEntryCorrectionView(officeID);
        }
        
        public ChallanDetailsModel GetChallanReportDetails(ChallanDetailsModel model)
        {
            return dalOBJ.GetChallanReportDetails(model);
        }

        public ChallanNoDataEntryCorrectionResponse SaveChallanDetails(ChallanDetailsModel model)
        {
            return dalOBJ.SaveChallanDetails(model);
        }
        
        public ChallanNoDataEntryCorrectionResponse UpdateChallanDetails(string ChallanCorrectionID, string InstrumentNumber, string InstrumentDate, int SROCode, int DistrictCode)
        {
            return dalOBJ.UpdateChallanDetails(ChallanCorrectionID, InstrumentNumber,InstrumentDate, SROCode, DistrictCode);
        }

    }
}