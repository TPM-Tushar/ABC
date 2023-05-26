using CustomModels.Models.ChallanNoDataEntryCorrection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.ChallanNoDataEntryCorrection.Interface
{
    public interface IChallanNoDataEntryCorrection
    {
        ChallanDetailsModel ChallanNoDataEntryCorrectionView(int officeID);

        //ChallanNoDataEntryCorrectionResponse UpdateChallanEntryDetails(ChallanNoDataEntryCorrectionViewModel challanNoDataEntryCorrectionViewModel);

        ChallanDetailsModel GetChallanReportDetails(ChallanDetailsModel model);

        ChallanNoDataEntryCorrectionResponse SaveChallanDetails(ChallanDetailsModel challanDetailsModel);

        ChallanNoDataEntryCorrectionResponse UpdateChallanDetails(string ChallanCorrectionID, string InstrumentNumber, string InstrumentDate, int SROCode, int DistrictCode);


    }
}
