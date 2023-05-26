using CustomModels.Models.MISReports.FRUITSIntegration;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class FRUITSIntegrationBAL : IFRUITSIntegration
    {
        IFRUITSIntegration fRUITSIntegrationDAL = new FRUITSIntegrationDAL();

        public KaveriFruitsIntegrationResultModel DownloadForm3(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel)
        {
            try
            {
                return fRUITSIntegrationDAL.DownloadForm3(kaveriFruitsIntegrationViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public KaveriFruitsIntegrationResultModel DownloadTransXML(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel)
        {
            try
            {
               return fRUITSIntegrationDAL.DownloadTransXML(kaveriFruitsIntegrationViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public KaveriFruitsIntegrationResultModel GetFRUITSRecvDetails(KaveriFruitsIntegrationViewModel kaveriFruitsIntegrationViewModel)
        {
            try
            {
                return fRUITSIntegrationDAL.GetFRUITSRecvDetails(kaveriFruitsIntegrationViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public KaveriFruitsIntegrationViewModel KAVERIFRUITSIntegration(int OfficeID)
        {
            try
            {
                return fRUITSIntegrationDAL.KAVERIFRUITSIntegration(OfficeID);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}