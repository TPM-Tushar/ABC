using CustomModels.Models.Remittance.FirmCentralization;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class FirmCentralizationBAL : IFirmCentralization
    {
        FirmCentralizationDAL firmCentralizationDAL = new FirmCentralizationDAL();
       public FirmCentralizationModel FirmCentralizationView()
        {
            return firmCentralizationDAL.FirmCentralizationView();
        }
        public FirmCentralizationResultModel GetFirmCentralizationDetails(FirmCentralizationModel firmCentralizationModel)
        {
            return firmCentralizationDAL.GetFirmCentralizationDetails(firmCentralizationModel);
        }
        public FirmCentralizationResultModel GetFirmCentralizationLocalDetails(FirmCentralizationModel firmCentralizationModel)
        {
            return firmCentralizationDAL.GetFirmCentralizationLocalDetails(firmCentralizationModel);
        }
        public FirmCentralizationResultModel GetFirmCentralizationCentralDetails(FirmCentralizationModel firmCentralizationModel)
        {
            return firmCentralizationDAL.GetFirmCentralizationCentralDetails(firmCentralizationModel);
        }
    }

}