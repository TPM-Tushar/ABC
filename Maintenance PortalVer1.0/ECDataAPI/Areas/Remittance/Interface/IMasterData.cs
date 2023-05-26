using CustomModels.Models.Remittance.MasterData;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.Remittance.Interface
{
    internal interface IMasterData
    {
        MasterDataResultModel MasterDataResult(MasterDataReportModel masterDataReportModel);
        MasterDataReportModel MasterDataReportview();

    }
}
