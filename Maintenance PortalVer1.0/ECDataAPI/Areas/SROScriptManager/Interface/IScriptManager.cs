using CustomModels.Models.SROScriptManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.SROScriptManager.Interface
{
    interface IScriptManager
    {
        SROScriptManagerModel InsertInScriptManagerDetails(SROScriptManagerModel viewModel);

        ScriptManagerDetailWrapperModel LoadScriptManagerTable(SROScriptManagerModel viewModel);

        SROScriptManagerModel GetScriptDetails(SROScriptManagerModel viewModel);

        SROScriptManagerModel UpdateInScriptManagerDetails(SROScriptManagerModel viewModel);
        
        AppVersionDetailsModel AppVersionDetails(int OfficeID);

        AppVersionDetailWrapperModel LoadAppVersionDetails(AppVersionDetailsModel viewModel);

        AppVersionDetailsModel AddAppVersionDetails(AppVersionDetailsModel viewModel);

        AppVersionDetailsModel GetAppVersionDetails(AppVersionDetailsModel viewModel);

        AppVersionDetailsModel UpdateAppVersionDetails(AppVersionDetailsModel viewModel);

        DROScriptManagerModel InsertInDROScriptManagerDetails(DROScriptManagerModel viewModel);

        DROScriptManagerDetailWrapperModel LoadDROScriptManagerTable(DROScriptManagerModel viewModel);

        DROScriptManagerModel GetDROScriptDetails(DROScriptManagerModel viewModel);

        DROScriptManagerModel UpdateInDROScriptManagerDetails(DROScriptManagerModel viewModel);

        //Added by Omkar on 17-08-2020
        ApplyAppVersionModel ApplyAppVersionView();
        ApplyAppVersionModel ApplyAppVersion(ApplyAppVersionModel viewModel);
        ApplyAppVersionModel ApplyAppVersionDR(ApplyAppVersionModel viewModel);

        ApplyAppVersionModel SRDRList();
        ApplyAppVersionModel DRList();

    }
}
