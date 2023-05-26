using CustomModels.Models.MISReports.ARegisterGenerationDetails;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ARegisterGenerationDetailsBAL
    {
        ARegisterGenerationDetailsDAL dalObject = new ARegisterGenerationDetailsDAL();



        public ARegisterGenerationDetailsModel ARegisterGenerationDetailsView(int OfficeID)
        {
            try
            {
                return dalObject.ARegisterGenerationDetailsView(OfficeID);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<ARegisterGenerationDetailsTableModel> GetARegisterGenerationReportsDetails(ARegisterGenerationDetailsModel model)
        {
            try
            {
                return dalObject.GetARegisterGenerationReportsDetails(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}