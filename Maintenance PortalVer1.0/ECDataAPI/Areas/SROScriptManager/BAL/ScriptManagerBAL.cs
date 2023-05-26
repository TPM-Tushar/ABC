
using CustomModels.Models.SROScriptManager;
using ECDataAPI.Areas.SROScriptManager.DAL;
using ECDataAPI.Areas.SROScriptManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.SROScriptManager.BAL
{
    public class ScriptManagerBAL : IScriptManager
    {
        IScriptManager DalObj = null;
        public SROScriptManagerModel InsertInScriptManagerDetails(SROScriptManagerModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.InsertInScriptManagerDetails(viewModel));
        }

        public ScriptManagerDetailWrapperModel LoadScriptManagerTable(SROScriptManagerModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.LoadScriptManagerTable(viewModel));
        }

        public SROScriptManagerModel GetScriptDetails(SROScriptManagerModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.GetScriptDetails(viewModel));
        }

        public SROScriptManagerModel UpdateInScriptManagerDetails(SROScriptManagerModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.UpdateInScriptManagerDetails(viewModel));
        }


        public AppVersionDetailsModel AppVersionDetails(int OfficeID)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.AppVersionDetails(OfficeID));

        }

        public AppVersionDetailWrapperModel LoadAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.LoadAppVersionDetails(viewModel));
        }

        public AppVersionDetailsModel AddAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.AddAppVersionDetails(viewModel));
        }

        public AppVersionDetailsModel GetAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.GetAppVersionDetails(viewModel));
        }

        public AppVersionDetailsModel UpdateAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.UpdateAppVersionDetails(viewModel));
        }

        public DROScriptManagerModel InsertInDROScriptManagerDetails(DROScriptManagerModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.InsertInDROScriptManagerDetails(viewModel));
        }

        public DROScriptManagerDetailWrapperModel LoadDROScriptManagerTable(DROScriptManagerModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.LoadDROScriptManagerTable(viewModel));
        }

        public DROScriptManagerModel GetDROScriptDetails(DROScriptManagerModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.GetDROScriptDetails(viewModel));
        }

        public DROScriptManagerModel UpdateInDROScriptManagerDetails(DROScriptManagerModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.UpdateInDROScriptManagerDetails(viewModel));
        }

        //Added by Omkar on 17-08-2020
        public ApplyAppVersionModel ApplyAppVersionView()
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.ApplyAppVersionView());

        }

        public ApplyAppVersionModel SRDRList()
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.SRDRList());

        }

        public ApplyAppVersionModel DRList()
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.DRList());

        }


        //Added by Omkar on 18-08-2020
        public ApplyAppVersionModel ApplyAppVersion(ApplyAppVersionModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.ApplyAppVersion(viewModel));

        }


        //Added by Omkar on 09-09-2020
        public ApplyAppVersionModel ApplyAppVersionDR(ApplyAppVersionModel viewModel)
        {
            DalObj = new ScriptManagerDAL();
            return (DalObj.ApplyAppVersionDR(viewModel));

        }


    }
}