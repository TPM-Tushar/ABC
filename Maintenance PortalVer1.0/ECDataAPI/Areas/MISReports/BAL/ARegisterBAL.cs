using CustomModels.Models.MISReports.ARegister;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ARegisterBAL : IARegister
    {
        IARegister dalObject = new ARegisterDAL();
        public ARegisterViewModel ARegisterView(ARegisterViewModel viewModel)
        {
            try
            {
                return dalObject.ARegisterView(viewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ARegisterResultModel GenerateReport(ARegisterViewModel aRegisterViewModel)
        {
            try
            {
                return dalObject.GenerateReport(aRegisterViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ARegisterResultModel ViewARegisterReport(string FileID)
        {
            try
            {
                return dalObject.ViewARegisterReport(FileID);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}