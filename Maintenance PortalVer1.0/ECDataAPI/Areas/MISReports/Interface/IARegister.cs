using CustomModels.Models.MISReports.ARegister;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IARegister
    {
        ARegisterViewModel ARegisterView(ARegisterViewModel viewModel);

        ARegisterResultModel GenerateReport(ARegisterViewModel aRegisterViewModel);
        ARegisterResultModel ViewARegisterReport(string FileID);
    }
}
